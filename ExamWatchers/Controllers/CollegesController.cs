using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Models;

namespace ExamWatchers.Controllers
{
    [AuthorizeRoles(SecurityRoles.Admin)]
    public class CollegesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Colleges
        public ActionResult Index()
        {
            return View(db.Colleges.ToList());
        }

        // GET: Colleges/ExamPlaces/5
        public ActionResult ExamPlaces(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var college = db.Colleges.Include(c => c.ExamPlaces).FirstOrDefault(c => c.Id == id);
            if (college == null)
                return HttpNotFound();

            ViewBag.CollegeTitle = college.Title;
            ViewBag.CollegeId = college.Id;
            return View(college.ExamPlaces.ToList());
        }

        public ActionResult Exams(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var college = db.Colleges.Include(c => c.ExamPlaces.Select(s => s.Exams.Select(e => e.Watchers))).FirstOrDefault(c => c.Id == id);
            if (college == null)
                return HttpNotFound();

            ViewBag.CollegeTitle = college.Title;
            ViewBag.CollegeId = college.Id;
            return View(college.ExamPlaces.SelectMany(s => s.Exams).ToList());
        }

        // GET: Colleges/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College college = db.Colleges.Find(id);
            if (college == null)
            {
                return HttpNotFound();
            }
            return View(college);
        }

        // GET: Colleges/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Colleges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(College college)
        {
            if (ModelState.IsValid)
            {
                db.Colleges.Add(college);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(college);
        }

        // GET: Colleges/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College college = db.Colleges.Find(id);
            if (college == null)
            {
                return HttpNotFound();
            }
            return View(college);
        }

        // POST: Colleges/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(College college)
        {
            if (ModelState.IsValid)
            {
                db.Entry(college).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(college);
        }

        // GET: Colleges/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College college = db.Colleges.Find(id);
            if (college == null)
            {
                return HttpNotFound();
            }
            return View(college);
        }

        // POST: Colleges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            College college = db.Colleges.Find(id);
            db.Colleges.Remove(college);
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
