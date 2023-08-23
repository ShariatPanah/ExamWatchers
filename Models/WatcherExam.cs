using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WatcherExam
    {
        public WatcherExam()
        {
            SubmitDate = DateTime.Now;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Exam")]
        [Display(Name = "کد امتحان")]
        public string ExamCode { get; set; }

        [ForeignKey("Watcher")]
        [Display(Name = "مراقب")]
        public string WatcherCode { get; set; }

        [Display(Name = "تاریخ ثبت")]
        [Required(ErrorMessage = "تاریخ ثبت را وارد کنید!")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [ScaffoldColumn(false)]
        public DateTime SubmitDate { get; set; }

        public virtual Exam Exam { get; set; }
        public virtual Watcher Watcher { get; set; }
    }
}
