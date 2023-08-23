using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;

namespace ExamWatchers.Controllers
{
    [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
    public class WatchersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Watchers
        public ActionResult Index()
        {
            var watchers = db.Watchers.Include(w => w.User);
            return View(watchers.ToList());
        }

        public ActionResult TakenExams(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var watcher = db.Watchers.Include(w => w.TakenExams.Select(s => s.Exam.ExamPlace.College)).FirstOrDefault(w => w.PersonnelCode == id);
            if (watcher == null)
                return HttpNotFound();

            ViewBag.WatcherName = watcher.User.FirstName + " " + watcher.User.LastName;
            var exams = watcher.TakenExams.ToList();
            return View(exams);
        }

        // GET: Watchers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Watcher watcher = db.Watchers.Find(id);
            if (watcher == null)
            {
                return HttpNotFound();
            }
            return View(watcher);
        }

        // GET: Watchers/Create
        public ActionResult Create()
        {
            var users = db.Users.Select(s => new
            {
                s.PersonnelCode,
                Name = s.FirstName + " " + s.LastName
            });

            ViewBag.PersonnelCode = new SelectList(users, "PersonnelCode", "Name");
            return View();
        }

        // POST: Watchers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Watcher watcher)
        {
            if (ModelState.IsValid)
            {
                // اگه شخص انتخاب شده قبلا به عنوان مراقب ثبت شده بود نباید اجازه بدیم دوباره ثبت بشه
                // و ارور نشون میدیم به کاربر
                if (db.Watchers.Any(w => w.PersonnelCode == watcher.PersonnelCode))
                {
                    var users2 = db.Users.Select(s => new
                    {
                        s.PersonnelCode,
                        Name = s.FirstName + " " + s.LastName
                    });

                    ViewBag.PersonnelCode = new SelectList(users2, "PersonnelCode", "Name", watcher.PersonnelCode);

                    ModelState.AddModelError("", "این مراقب قبلا ثبت شده است!");
                    return View(watcher);
                }

                db.Watchers.Add(watcher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var users = db.Users.Select(s => new
            {
                s.PersonnelCode,
                Name = s.FirstName + " " + s.LastName
            });

            ViewBag.PersonnelCode = new SelectList(users, "PersonnelCode", "Name", watcher.PersonnelCode);
            return View(watcher);
        }

        // GET: Watchers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Watcher watcher = db.Watchers.Find(id);
            if (watcher == null)
            {
                return HttpNotFound();
            }

            var users = db.Users.Select(s => new
            {
                s.PersonnelCode,
                Name = s.FirstName + " " + s.LastName
            });

            ViewBag.PersonnelCode = new SelectList(users, "PersonnelCode", "Name", watcher.PersonnelCode);
            return View(watcher);
        }

        // POST: Watchers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Watcher watcher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(watcher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var users = db.Users.Select(s => new
            {
                s.PersonnelCode,
                Name = s.FirstName + " " + s.LastName
            });

            ViewBag.PersonnelCode = new SelectList(users, "PersonnelCode", "Name", watcher.PersonnelCode);
            return View(watcher);
        }

        // GET: Watchers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Watcher watcher = db.Watchers.Find(id);
            if (watcher == null)
            {
                return HttpNotFound();
            }
            return View(watcher);
        }

        // POST: Watchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Watcher watcher = db.Watchers.Find(id);
            db.Watchers.Remove(watcher);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult PrintWatchersForm()
        {
            return View();
        }

        public ActionResult WatchersReport()
        {
            StiReport stiReport = null;
            try
            {
                stiReport = new StiReport();
                stiReport.Load(Server.MapPath("~/Reports/Watchers.mrt"));

                /// new connection string
                string myConnectionString = ConfigurationManager.ConnectionStrings["Watchers"].ConnectionString;

                stiReport.Dictionary.Databases.Clear();
                stiReport.Dictionary.Databases.Add(new StiSqlDatabase("Watchers", myConnectionString));

                var now = DateTime.Now;

                stiReport.Dictionary.Variables["ReportDateTime"].Value = now.ToString("yyyy/MM/dd HH:mm:ss");

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
