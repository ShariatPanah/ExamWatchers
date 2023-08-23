using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Models;

namespace ExamWatchers.Controllers
{
    [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
    public class ExamPlacesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ExamPlaces
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

            var examPlaces = db.ExamPlaces.Include(e => e.College).Where(e => accessedCollegeIds.Contains(e.CollegeId));
            return View(examPlaces.ToList());
        }

        public ActionResult Exams(int? placeId)
        {
            if (placeId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var place = db.ExamPlaces.Include(p => p.Exams.Select(e => e.Watchers)).FirstOrDefault(p => p.Id == placeId);
            if (place == null)
                return HttpNotFound();

            ViewBag.PlaceId = place.Id;
            ViewBag.PlaceTitle = place.Title;
            return View(place.Exams.ToList());
        }

        // GET: ExamPlaces/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamPlace examPlace = db.ExamPlaces.Find(id);
            if (examPlace == null)
            {
                return HttpNotFound();
            }
            return View(examPlace);
        }

        // GET: ExamPlaces/Create
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

            ViewBag.CollegeIds = new SelectList(colleges
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                }), "Id", "Title");

            return View();
        }

        // POST: ExamPlaces/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExamPlace examPlace)
        {
            if (ModelState.IsValid)
            {
                db.ExamPlaces.Add(examPlace);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // واکشی دانشکده هایی که کاربر جاری بهشون دسترسی داره
            // اگر کاربر جاری نقش مدیر سیستم رو داشت پس مجازه که به کل دانشکده ها دسترسی داشته باشه
            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeIds = new SelectList(colleges
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                }), "Id", "Title", examPlace.CollegeId);

            return View(examPlace);
        }

        // GET: ExamPlaces/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamPlace examPlace = db.ExamPlaces.Find(id);
            if (examPlace == null)
            {
                return HttpNotFound();
            }

            // واکشی دانشکده هایی که کاربر جاری بهشون دسترسی داره
            // اگر کاربر جاری نقش مدیر سیستم رو داشت پس مجازه که به کل دانشکده ها دسترسی داشته باشه
            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeIds = new SelectList(colleges
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                }), "Id", "Title", examPlace.CollegeId);

            return View(examPlace);
        }

        // POST: ExamPlaces/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExamPlace examPlace)
        {
            if (ModelState.IsValid)
            {
                db.Entry(examPlace).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // واکشی دانشکده هایی که کاربر جاری بهشون دسترسی داره
            // اگر کاربر جاری نقش مدیر سیستم رو داشت پس مجازه که به کل دانشکده ها دسترسی داشته باشه
            IEnumerable<College> colleges;
            if (User.IsInRole(SecurityRoles.Admin))
                colleges = db.Colleges;
            else
            {
                // واکشی آیدی دانشکده هایی که کاربر جاری بهشون دسترسی داره
                var accessedCollegeIds = db.Users.FirstOrDefault(u => u.PersonnelCode == User.Identity.Name).Accesses.Select(s => s.CollegeId);
                colleges = db.Colleges.Where(c => accessedCollegeIds.Contains(c.Id));
            }

            ViewBag.CollegeIds = new SelectList(colleges
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                }), "Id", "Title", examPlace.CollegeId);

            return View(examPlace);
        }

        // GET: ExamPlaces/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamPlace examPlace = db.ExamPlaces.Find(id);
            if (examPlace == null)
            {
                return HttpNotFound();
            }
            return View(examPlace);
        }

        // POST: ExamPlaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExamPlace examPlace = db.ExamPlaces.Find(id);
            db.ExamPlaces.Remove(examPlace);
            db.SaveChanges();
            return RedirectToAction("Index");
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
