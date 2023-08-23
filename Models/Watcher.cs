using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Models
{
    public class Watcher
    {
        public Watcher()
        {
            TakenExams = new List<WatcherExam>();
        }

        [Key, ForeignKey("User")]
        [Display(Name = "کد پرسنلی")]
        [StringLength(maximumLength: 15, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای کد پرسنلی 15 کاراکتر می باشد.")]
        public string PersonnelCode { get; set; }

        [Display(Name = "تعداد ساعات مراقبت")]
        [Required(ErrorMessage = "تعداد ساعات مراقبت را تعیین کنید.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:N0} ساعت")]
        [Range(minimum: 0, maximum: 255, ErrorMessage = "ساعات مراقبت باید عددی بین 0 تا 255 باشد!")]
        public byte WatchHours { get; set; }

        [NotMapped]
        [Display(Name = "تعداد ساعات اخذ شده")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:N0} ساعت")]
        [ScaffoldColumn(false)]
        public int TakenHours { get { return (int)TakenExams.Sum(s => s.Exam.ExamEndTime.Subtract(s.Exam.ExamStartTime).TotalHours); } }

        public virtual User User { get; set; }
        public virtual IList<WatcherExam> TakenExams { get; set; }
    }
}
