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
    [AuthorizeRoles(SecurityRoles.Admin, SecurityRoles.CollegeAdmin)]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            // همه کاربرها به جز مدیر سیستم رو واکشی کن
            // کد پرسنلی مدیر سیستم عدد صفر هستش
            return View(db.Users.Where(u => u.PersonnelCode != "0").ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            var user = new User
            {
                Organ = EnumTypes.OrganTypes.معاونت
            };

            ViewBag.Roles = new List<SecurityRoleViewModel>
            {
                new SecurityRoleViewModel{ RoleName = SecurityRoles.Admin },
                new SecurityRoleViewModel{ RoleName = SecurityRoles.CollegeAdmin },
                new SecurityRoleViewModel{ RoleName = SecurityRoles.Watcher }
            };

            ViewBag.Colleges = db.Colleges.Select(s => new CollegeViewModel
            {
                Id = s.Id,
                Title = s.Title
            });

            return View(user);
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user, List<string> CollegesSelectedOnView, byte watchHours)
        {
            // اگه لیستمون نال نباشه همون خودش رو توی خودش جاگذاری کن
            // در غیراینصورت یعنی اگه لیست نال بود اون رو نیو کن واسه اینکه
            // وقتی داریم ازش استفاده میکنم با خطای نال بودن لیست برخورد نکنیم
            CollegesSelectedOnView = CollegesSelectedOnView ?? new List<string>();

            if (ModelState.IsValid)
            {
                // چون نقش مدیر سیستم به کل سیستم و اطلاعات دسترسی داره نیازی به ثبت دسترسی به دانشکده ها نیست 
                // واسه همین اول چک میکنیم که نقش انتخاب شده برای کاربر جدید هرچیزی غیر از نقش مدیر سیستم بود 
                // اون موقع دسترسی به دانشکده ها رو براش ثبت میکنیم
                if (user.Role != SecurityRoles.Admin)
                {
                    foreach (var item in CollegesSelectedOnView)
                    {
                        user.Accesses.Add(new UserCollegeAccess
                        {
                            CollegeId = Convert.ToInt32(item),
                            UserCode = user.PersonnelCode,
                        });
                    }
                }

                // افزودن کاربر جدید به جدول کاربرها
                db.Users.Add(user);

                // اگه نقش انتخاب شده برای کاربر جدید نقش مراقب بود علاوه بر ثبت کاربر جدید در جدول کاربرها
                // باید یه رکورد جدید با این کد پرسنلی داخل جدول مراقبین هم ایجاد کنیم
                if (user.Role == SecurityRoles.Watcher)
                {
                    db.Watchers.Add(new Watcher
                    {
                        PersonnelCode = user.PersonnelCode,
                        WatchHours = watchHours
                    });
                }

                // اعمال تغییرات انجام شده (منظور همون درج رکوردهای جدید هستش) در دیتابیس
                // تا زمانی که این متد رو فراخوانی نکردیم، هیچکدوم از رکوردهامون داخل دیتابیس ثبت نمیشن
                // این مکانیسم انتیتی فریمورک هستش
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new List<SecurityRoleViewModel>
            {
                new SecurityRoleViewModel{ RoleName = SecurityRoles.Admin, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.Admin) },
                new SecurityRoleViewModel{ RoleName = SecurityRoles.CollegeAdmin, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.CollegeAdmin) },
                new SecurityRoleViewModel{ RoleName = SecurityRoles.Watcher, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.Watcher) }
            };

            ViewBag.Colleges = db.Colleges.Select(s => new CollegeViewModel
            {
                Id = s.Id,
                Title = s.Title,
                IsSelected = CollegesSelectedOnView.Contains(s.Id.ToString())
            });

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Include(u => u.Accesses).FirstOrDefault(u => u.PersonnelCode == id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var selectedColleges = user.Accesses.Select(a => a.CollegeId).ToList();

            ViewBag.Roles = new List<SecurityRoleViewModel>
            {
                new SecurityRoleViewModel{ RoleName = SecurityRoles.Admin, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.Admin) },
                new SecurityRoleViewModel{ RoleName = SecurityRoles.CollegeAdmin, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.CollegeAdmin) },
                new SecurityRoleViewModel{ RoleName = SecurityRoles.Watcher, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.Watcher) }
            };

            ViewBag.Colleges = db.Colleges.Select(s => new CollegeViewModel
            {
                Id = s.Id,
                Title = s.Title,
                IsSelected = selectedColleges.Contains(s.Id)
            });

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user, List<string> CollegesSelectedOnView)
        {
            // اگه لیستمون نال نباشه همون خودش رو توی خودش جاگذاری کن
            // در غیراینصورت یعنی اگه لیست نال بود اون رو نیو کن واسه اینکه
            // وقتی داریم ازش استفاده میکنم با خطای نال بودن لیست برخورد نکنیم
            CollegesSelectedOnView = CollegesSelectedOnView ?? new List<string>();

            if (ModelState.IsValid)
            {
                var userInDb = db.Users.Include(u => u.Accesses).FirstOrDefault(u => u.PersonnelCode == user.PersonnelCode);

                // واکشی آیدی دانشکده های قابل دسترسی برای این کاربر
                var accessedColleges = userInDb.Accesses.ToList();

                // دسترسی به دانشکده هایی که تیکشون برداشته شده رو حذف کن
                db.UserCollegeAccess.RemoveRange(accessedColleges.FindAll(a => !CollegesSelectedOnView.Contains(a.CollegeId.ToString())));

                // آیدی تمامی دانشکده هایی که قبلا داشتیم رو حذف کن و فقط روی دانشکده های جدید که تیک خوردن تمرکز کن
                CollegesSelectedOnView.RemoveAll(c => accessedColleges.Select(a => a.CollegeId.ToString()).ToList().Contains(c));
                foreach (var item in CollegesSelectedOnView)
                {
                    userInDb.Accesses.Add(new UserCollegeAccess
                    {
                        CollegeId = Convert.ToInt32(item),
                        UserCode = userInDb.PersonnelCode,
                    });
                }

                // اگه نقش قبلی کاربر نقش مراقب بوده و الان اون نقش تغییر پیدا کرده و نقشی غیر از مراقب بهش دادن
                // باید رکورد مربوط به این کاربر رو از داخل جدول مراقبین حذف کنیم و به طبع آن تمامی مراقبت هایی
                // که کاربر قبلا به عنوان نقش مراقب انتخاب کرده بود هم حذف میشن
                // منظور همون جلسات امتحانیه که طرف به عنوان مراقب براشون اعلام حضور کرده بود
                if (userInDb.Role == SecurityRoles.Watcher && user.Role != SecurityRoles.Watcher)
                {
                    var watcher = db.Watchers.FirstOrDefault(w => w.PersonnelCode == user.PersonnelCode);
                    db.Watchers.Remove(watcher);
                }

                // اگه نقش انتخاب شده برای کاربر نقش مراقب بود باید یه رکورد جدید با این کد پرسنلی داخل جدول
                // مراقبین ایجاد کنیم
                if (user.Role == SecurityRoles.Watcher && userInDb.Role != SecurityRoles.Watcher)
                {
                    db.Watchers.Add(new Watcher
                    {
                        PersonnelCode = user.PersonnelCode,
                        WatchHours = 0
                    });
                }

                userInDb.FirstName = user.FirstName;
                userInDb.LastName = user.LastName;
                userInDb.Password = user.Password;
                userInDb.Organ = user.Organ;
                userInDb.Role = user.Role;

                // نشانه گذاری یا معرفی آبجکت کاربر واکشی شده به عنوان تغییریافته
                // یعنی انتیتی فریمورک خودش میفهمه چه پراپرتی هایی از این ابجکت تغییر کردن
                // و اون تغییرات رو داخل دیتابیس اعمال میکنه
                db.Entry(userInDb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new List<SecurityRoleViewModel>
            {
                new SecurityRoleViewModel{ RoleName = SecurityRoles.Admin, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.Admin) },
                new SecurityRoleViewModel{ RoleName = SecurityRoles.CollegeAdmin, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.CollegeAdmin) },
                new SecurityRoleViewModel{ RoleName = SecurityRoles.Watcher, IsSelected = CheckSelectionStatus(user.Role, SecurityRoles.Watcher) }
            };

            ViewBag.Colleges = db.Colleges.Select(s => new CollegeViewModel
            {
                Id = s.Id,
                Title = s.Title,
                IsSelected = CollegesSelectedOnView.Contains(s.Id.ToString())
            });

            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ConfirmUser(string code)
        {
            if (code == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(code);
            if (user == null)
                return HttpNotFound();

            user.IsConfirmed = true;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        private bool CheckSelectionStatus(string selectedRole, string securityRole)
        {
            return selectedRole == securityRole;
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
