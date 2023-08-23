using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExamWatchers.ViewModels;
using Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;

namespace ExamWatchers.Controllers
{
    [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
    public class ExamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Exams
        public ActionResult Index()
        {
            // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
            // اگه کاربر جاری نقش ادمین رو داشت، پس مجازه که به کل دانشکده ها دسترسی داشته باشه
            IEnumerable<int> accessedCollegeIds;
            if (User.IsInRole(SecurityRoles.Admin))
                accessedCollegeIds = db.Colleges.Select(s => s.Id);
            else
            {
                accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
            }

            var exams = db.Exams.Include(e => e.Watchers)
                .Include(e => e.ExamPlace)
                .Where(e => accessedCollegeIds.Contains(e.ExamPlace.CollegeId));
            return View(exams.ToList());
        }

        [HttpGet]
        public ActionResult Watchers(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var exam = db.Exams.Include(e => e.Watchers).FirstOrDefault(e => e.Code == id);
            if (exam == null)
                return HttpNotFound();

            ViewBag.ExamTitle = exam.Title;
            ViewBag.ExamCode = exam.Code;
            var examsList = exam.Watchers
                .Select(s => new ExamWatcherDetailsViewModel
                {
                    WatcherCode = s.WatcherCode,
                    WatcherName = s.Watcher.User.FirstName + " " + s.Watcher.User.LastName,
                    SubmitDate = s.SubmitDate
                }).ToList();
            return View(examsList);
        }

        // GET: Exams/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // GET: Exams/Create
        public ActionResult Create()
        {
            //var vm = new CreateExamViewModel
            //{
            //    ExamStartTime = DateTime.Now.AddDays(5).Date
            //};

            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeId = new SelectList(colleges
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                }), "Id", "Title");

            ViewBag.ExamPlaceId = new SelectList(db.ExamPlaces, "Id", "Title");
            return View();
        }

        // POST: Exams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateExamViewModel vm)
        {
            if (ModelState.IsValid)
            {
                string startTime = vm.ExamStartTime.ToAsciiNumbers();
                string endTime = vm.ExamEndTime.ToAsciiNumbers();
                string endDateTime = startTime.Split(' ')[0] + " " + endTime;

                var exam = new Exam
                {
                    Code = vm.Code,
                    ExamPlaceId = vm.ExamPlaceId,
                    //RemainingCapacity = vm.WatchersCapacity,
                    Title = vm.Title,
                    WatchersCapacity = vm.WatchersCapacity,
                    ExamStartTime = DateTime.Parse(startTime),
                    ExamEndTime = DateTime.Parse(endDateTime)
                };

                db.Exams.Add(exam);
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

            ViewBag.CollegeId = new SelectList(colleges
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                }), "Id", "Title", vm.CollegeId);

            ViewBag.ExamPlaceId = new SelectList(db.ExamPlaces, "Id", "Title", vm.ExamPlaceId);
            return View(vm);
        }

        // GET: Exams/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Include(e => e.ExamPlace).FirstOrDefault(e => e.Code == id);
            if (exam == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CreateExamViewModel
            {
                Code = exam.Code,
                Title = exam.Title,
                CollegeId = exam.ExamPlace.CollegeId,
                ExamStartTime = exam.ExamStartTime.ToString(),
                ExamEndTime = exam.ExamEndTime.ToString(),
                //ExamStartTime = exam.ExamStartTime,
                //ExamEndTime = exam.ExamEndTime,
                ExamPlaceId = exam.ExamPlaceId,
                WatchersCapacity = exam.WatchersCapacity,
                //RemainingCapacity = exam.RemainingCapacity
            };

            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeId = new SelectList(colleges
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                }), "Id", "Title", exam.ExamPlace.CollegeId);

            ViewBag.ExamPlaceId = new SelectList(db.ExamPlaces, "Id", "Title", exam.ExamPlaceId);
            return View(viewModel);
        }

        // POST: Exams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateExamViewModel vm)
        {
            if (ModelState.IsValid)
            {
                string startTime = vm.ExamStartTime.ToAsciiNumbers();
                string endTime = vm.ExamEndTime.ToAsciiNumbers();
                string endDateTime = startTime.Split(' ')[0] + " " + endTime;

                var exam = new Exam
                {
                    Code = vm.Code,
                    Title = vm.Title,
                    ExamPlaceId = vm.ExamPlaceId,
                    WatchersCapacity = vm.WatchersCapacity,
                    //RemainingCapacity = vm.RemainingCapacity,
                    ExamStartTime = DateTime.Parse(startTime),
                    ExamEndTime = DateTime.Parse(endDateTime)
                };

                db.Entry(exam).State = EntityState.Modified;
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

            ViewBag.CollegeId = new SelectList(colleges
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                }), "Id", "Title", vm.CollegeId);

            ViewBag.ExamPlaceId = new SelectList(db.ExamPlaces, "Id", "Title", vm.ExamPlaceId);
            return View(vm);
        }

        // GET: Exams/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exams.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Exam exam = db.Exams.Find(id);
            db.Exams.Remove(exam);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult FillPlaces(int collegeId)
        {
            var places = db.ExamPlaces.Where(p => p.CollegeId == collegeId)
                    .Select(s => new
                    {
                        Id = s.Id,
                        Title = s.Title
                    })
                    .ToList();

            return Json(places, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintWatchersForm(string examCode)
        {
            //ViewBag.ExamCode = examCode;
            TempData["ExamCode"] = examCode;
            return View();
        }

        public ActionResult WatchersReport()
        {
            StiReport stiReport = null;
            try
            {
                stiReport = new StiReport();
                stiReport.Load(Server.MapPath("~/Reports/WatchersOfAnExam.mrt"));

                /// new connection string
                string myConnectionString = ConfigurationManager.ConnectionStrings["Watchers"].ConnectionString;

                stiReport.Dictionary.Databases.Clear();
                stiReport.Dictionary.Databases.Add(new StiSqlDatabase("Watchers", myConnectionString));

                var now = DateTime.Now;

                stiReport.Dictionary.Variables["ReportDateTime"].Value = now.ToString("yyyy/MM/dd HH:mm:ss");

                string examCode = TempData["ExamCode"].ToString();
                var exam = db.Exams.Include(e => e.ExamPlace).FirstOrDefault(e => e.Code == examCode);
                stiReport.Dictionary.Variables["ExamTitle"].Value = exam.Title;
                stiReport.Dictionary.Variables["ExamPlace"].Value = exam.ExamPlace.Title;
                stiReport.Dictionary.Variables["ExamDateTime"].Value = exam.ExamStartTime.ToString("yyyy/MM/dd HH:mm") + " - " + exam.ExamEndTime.ToString("HH:mm");
                stiReport["ExamCode"] = exam.Code;

                stiReport.Compile();
                stiReport.Render();

                //stiReport.Show();
            }
            catch (Exception ex)
            { }

            return StiMvcViewer.GetReportResult(stiReport);
        }

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
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
