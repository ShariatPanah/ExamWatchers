using ExamWatchers.ViewModels;
using Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace ExamWatchers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (!db.Users.Any(u => u.PersonnelCode == user.PersonnelCode))
                {
                    //user.Role = SecurityRoles.Watcher;

                    db.Users.Add(user);
                    db.SaveChanges();

                    ViewBag.Message = "ثبت نام شما با موفقیت انجام شد، پس از تأیید از سوی مدیر میتوانید وارد سایت شوید.";
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "این کد پرسنلی قبلا ثبت شده است!");
                    return View(user);
                }
            }

            return View(user);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.PersonnelCode == login.PersonnelCode && u.Password == login.Password);

                if (user != null)
                {
                    if (!user.IsConfirmed)
                    {
                        ModelState.AddModelError("", "اکانت شما تأیید نشده است! لازم است اکانت شما توسط مدیر تأیید شود.");
                        return View(login);
                    }

                    // ذخیره اطلاعات کاربر لاگین شده در کوکی مرورگر
                    FormsAuthentication.SetAuthCookie(login.PersonnelCode, true);

                    // طبق نقشی که کاربر داره به صفحه مناسب هدایتش میکنیم
                    if (user.Role == SecurityRoles.Admin)
                        return RedirectToAction("Index", "Colleges");
                    else if (user.Role == SecurityRoles.CollegeAdmin)
                        return RedirectToAction("Index", "Exams");
                    else
                        return RedirectToAction("Exams", "WatcherExams");
                }
                else
                {
                    ModelState.AddModelError("", "کد پرسنلی یا رمز عبور اشتباه است!");
                    return View(login);
                }
            }

            return View(login);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }
    }
}