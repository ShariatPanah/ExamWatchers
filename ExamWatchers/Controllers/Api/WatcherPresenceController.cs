using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExamWatchers.ViewModels;

namespace ExamWatchers.Controllers.Api
{
    [AuthorizeRoles(SecurityRoles.Watcher)]
    public class WatcherPresenceController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public IHttpActionResult TogglePresence(string examCode)
        {
            if (string.IsNullOrWhiteSpace(examCode))
                return BadRequest();

            var exam = db.Exams.Include(e => e.Watchers).FirstOrDefault(e => e.Code == examCode);
            if (exam == null)
                return NotFound();

            var response = new ToggleWatcherPresenceResponseViewModel();
            if (exam.Watchers.Any(w => w.WatcherCode == User.Identity.Name))
            {
                db.WatchersExams.Remove(exam.Watchers.FirstOrDefault(e => e.WatcherCode == User.Identity.Name));
                response.ActionText = "ثبت حضور";
                response.ShowAction = true;
            }
            else
            {
                if (exam.RemainingCapacity == 0)
                    return BadRequest("تعداد مراقبین این امتحان تکمیل شده است!");

                var watcher = db.Watchers.Include(w => w.TakenExams).FirstOrDefault(u => u.PersonnelCode == User.Identity.Name);
                if (watcher.TakenExams.Sum(s => s.Exam.ExamEndTime.Subtract(s.Exam.ExamStartTime).TotalHours) >= watcher.WatchHours)
                    return BadRequest("تعداد ساعات مراقبت شما تکمیل شده است!");

                db.WatchersExams.Add(new WatcherExam
                {
                    ExamCode = exam.Code,
                    WatcherCode = User.Identity.Name
                });

                response.ActionText = "عدم حضور";
                response.ShowAction = true;
            }

            db.SaveChanges();

            if(exam.RemainingCapacity == 0)
            {
                response.IsOutOfCapacity = true;
                response.RemainingCapacityText = "تکمیل شده";
            }
            else
            {
                response.IsOutOfCapacity = false;
                response.RemainingCapacityText = exam.RemainingCapacity.ToString() + " نفر";
            }

            return Ok(response);
        }
    }
}
