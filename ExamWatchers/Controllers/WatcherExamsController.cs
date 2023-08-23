using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExamWatchers.ViewModels;
using Models;

namespace ExamWatchers.Controllers
{
    [Authorize]
    public class WatcherExamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WatcherExams
        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult Index()
        {
            var watchersExams = db.WatchersExams.Include(w => w.Exam.ExamPlace.College).Include(w => w.Watcher.User);
            return View(watchersExams.ToList());
        }

        [Authorize(Roles = SecurityRoles.Watcher)]
        public ActionResult Exams()
        {
            // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
            var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
            IEnumerable<College> colleges = db.Colleges.Include(c => c.ExamPlaces.Select(s => s.Exams))
                .Where(c => accessedCollegeIds.Contains(c.Id));

            ViewBag.CollegeId = new SelectList(colleges.Select(c => new
            {
                c.Id,
                c.Title,
            }), "Id", "Title");

            var exams = colleges.FirstOrDefault().ExamPlaces.SelectMany(e =>
                                 e.Exams.Select(s => new ExamForWatcherViewModel
                                 {
                                     Code = s.Code,
                                     ExamEndTime = s.ExamEndTime,
                                     ExamPlace = s.ExamPlace,
                                     ExamPlaceId = s.ExamPlaceId,
                                     ExamStartTime = s.ExamStartTime,
                                     Title = s.Title,
                                     Watchers = s.Watchers,
                                     WatchersCapacity = s.WatchersCapacity
                                 }))
                                .OrderBy(o => o.ExamStartTime)
                                .ToList();

            foreach (var item in exams)
            {
                if (!item.Watchers.Any(w => w.WatcherCode == User.Identity.Name))
                {
                    if (item.RemainingCapacity != 0)
                    {
                        item.ActionText = "ثبت حضور";
                        item.ShowActions = true;
                    }
                    else if (item.RemainingCapacity == 0)
                    {
                        item.ActionText = "";
                        item.ShowActions = false;
                    }
                }
                else if (item.Watchers.Any(w => w.WatcherCode == User.Identity.Name))
                {
                    item.ActionText = "عدم حضور";
                    item.ShowActions = true;
                }
            }

            return View(exams);
        }

        // GET: WatcherExams/Details/5
        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WatcherExam watcherExam = db.WatchersExams.Find(id);
            if (watcherExam == null)
            {
                return HttpNotFound();
            }
            return View(watcherExam);
        }

        // GET: WatcherExams/Create
        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult Create()
        {
            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeId = new SelectList(colleges.Select(c => new
            {
                c.Id,
                c.Title,
            }), "Id", "Title");

            ViewBag.ExamCode = new SelectList(db.Exams, "Code", "Title");
            ViewBag.WatcherCode = new SelectList(db.Watchers.Select(s => new
            {
                s.PersonnelCode,
                Name = s.User.FirstName + " " + s.User.LastName
            }), "PersonnelCode", "Name");
            return View();
        }

        // POST: WatcherExams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult Create(WatcherExam watcherExam)
        {
            if (ModelState.IsValid)
            {
                // بررسی میکنیم اگه قبلا رکوردی با این کد امتحان و کد پرسنلی ثبت شده بود
                // یعنی قبلا این مراقب برای امتحان موردنظر ثبت شده
                if (db.WatchersExams.Any(w => w.ExamCode == watcherExam.ExamCode && w.WatcherCode == watcherExam.WatcherCode))
                {
                    ModelState.AddModelError("", "این مراقب قبلا برای این امتحان ثبت شده است!");
                }
                else if (db.Exams.Include(e => e.Watchers).FirstOrDefault(e => e.Code == watcherExam.ExamCode).RemainingCapacity == 0)
                {
                    ModelState.AddModelError("", "ظرفیت مراقبین این امتحان تکمیل شده است!");
                }
                else
                {
                    db.WatchersExams.Add(watcherExam);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeId = new SelectList(colleges.Select(c => new
            {
                c.Id,
                c.Title,
            }), "Id", "Title");

            ViewBag.ExamCode = new SelectList(db.Exams, "Code", "Title", watcherExam.ExamCode);
            ViewBag.WatcherCode = new SelectList(db.Watchers.Select(s => new
            {
                s.PersonnelCode,
                Name = s.User.FirstName + " " + s.User.LastName
            }), "PersonnelCode", "Name", watcherExam.WatcherCode);
            return View(watcherExam);
        }

        // GET: WatcherExams/Edit/5
        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WatcherExam watcherExam = db.WatchersExams.Find(id);
            if (watcherExam == null)
            {
                return HttpNotFound();
            }

            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeId = new SelectList(colleges.Select(c => new
            {
                c.Id,
                c.Title,
            }), "Id", "Title");

            ViewBag.ExamCode = new SelectList(db.Exams, "Code", "Title", watcherExam.ExamCode);
            ViewBag.WatcherCode = new SelectList(db.Watchers.Select(s => new
            {
                s.PersonnelCode,
                Name = s.User.FirstName + " " + s.User.LastName
            }), "PersonnelCode", "Name", watcherExam.WatcherCode);
            return View(watcherExam);
        }

        // POST: WatcherExams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult Edit(WatcherExam watcherExam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(watcherExam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeId = new SelectList(colleges.Select(c => new
            {
                c.Id,
                c.Title,
            }), "Id", "Title");

            ViewBag.ExamCode = new SelectList(db.Exams, "Code", "Title", watcherExam.ExamCode);
            ViewBag.WatcherCode = new SelectList(db.Watchers.Select(s => new
            {
                s.PersonnelCode,
                Name = s.User.FirstName + " " + s.User.LastName
            }), "PersonnelCode", "Name", watcherExam.WatcherCode);
            return View(watcherExam);
        }

        // GET: WatcherExams/Delete/5
        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WatcherExam watcherExam = db.WatchersExams.Find(id);
            if (watcherExam == null)
            {
                return HttpNotFound();
            }
            return View(watcherExam);
        }

        // POST: WatcherExams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult DeleteConfirmed(int id)
        {
            WatcherExam watcherExam = db.WatchersExams.Find(id);
            db.WatchersExams.Remove(watcherExam);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(SecurityRoles.Watcher)]
        public PartialViewResult GetExams(int collegeId)
        {
            var college = db.Colleges.Include(c => c.ExamPlaces.Select(e => e.Exams)).FirstOrDefault(c => c.Id == collegeId);

            IEnumerable<ExamForWatcherViewModel> exams = new List<ExamForWatcherViewModel>();
            if (college != null)
            {
                exams = college.ExamPlaces.SelectMany(e =>
                                e.Exams.Select(s => new ExamForWatcherViewModel
                                {
                                    Code = s.Code,
                                    ExamEndTime = s.ExamEndTime,
                                    ExamPlace = s.ExamPlace,
                                    ExamPlaceId = s.ExamPlaceId,
                                    ExamStartTime = s.ExamStartTime,
                                    Title = s.Title,
                                    Watchers = s.Watchers,
                                    WatchersCapacity = s.WatchersCapacity
                                }))
                                .OrderBy(o => o.ExamStartTime)
                                .ToList();

                foreach (var item in exams)
                {
                    if (!item.Watchers.Any(w => w.WatcherCode == User.Identity.Name))
                    {
                        if (item.RemainingCapacity != 0)
                        {
                            item.ActionText = "ثبت حضور";
                            item.ShowActions = true;
                        }
                        else if (item.RemainingCapacity == 0)
                        {
                            item.ActionText = "";
                            item.ShowActions = false;
                        }
                    }
                    else if (item.Watchers.Any(w => w.WatcherCode == User.Identity.Name))
                    {
                        item.ActionText = "عدم حضور";
                        item.ShowActions = true;
                    }
                }
            }

            return PartialView("_PartialExamsForWatchersList", exams);
        }

        [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
        public ActionResult GetWatchersAndExams(int collegeId)
        {
            var exams = db.Exams.Where(e => e.ExamPlace.CollegeId == collegeId)
                .Select(s => new
                {
                    s.Code,
                    s.Title
                });

            var watchers = db.Watchers.Where(w => w.User.Accesses.Select(a => a.CollegeId).Contains(collegeId))
                .Select(s => new
                {
                    s.PersonnelCode,
                    Name = s.User.FirstName + " " + s.User.LastName
                });

            return Json(new { exams, watchers }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
