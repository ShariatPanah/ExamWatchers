using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamWatchers.ViewModels
{
    public class ExamForWatcherViewModel
    {
        [Key]
        [Display(Name = "کد امتحان")]
        [Required(ErrorMessage = "کد امتحان را وارد کنید!")]
        [StringLength(maximumLength: 15, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای کد امتحان 15 کاراکتر می باشد.")]
        public string Code { get; set; }

        [Display(Name = "عنوان امتحان")]
        [Required(ErrorMessage = "عنوان امتحان را وارد کنید!")]
        [StringLength(maximumLength: 40, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای عنوان امتحان 40 کاراکتر می باشد.")]
        public string Title { get; set; }

        [Display(Name = "تاریخ و زمان شروع")]
        [Required(ErrorMessage = "تاریخ امتحان را وارد کنید!")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime ExamStartTime { get; set; }

        [Display(Name = "زمان پایان")]
        [Required(ErrorMessage = "زمان پایان امتحان را وارد کنید!")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime ExamEndTime { get; set; }

        [Display(Name = "ظرفیت مراقبین")]
        [Required(ErrorMessage = "ظرفیت مراقبین را تعیین کنید.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:N0}" + " نفر")]
        [Range(minimum: 1, maximum: 255, ErrorMessage = "ظرفیت مراقبین باید عددی بین 1 تا 255 باشد!")]
        public byte WatchersCapacity { get; set; }

        [Display(Name = "ظرفیت باقیمانده")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:N0}" + " نفر")]
        [NotMapped]
        public int RemainingCapacity { get { return WatchersCapacity - Watchers.Count; } }

        [Display(Name = "حوزه امتحانی")]
        [ForeignKey("ExamPlace")]
        public int ExamPlaceId { get; set; }
        public virtual ExamPlace ExamPlace { get; set; }

        public virtual IList<WatcherExam> Watchers { get; set; }

        public string ActionText { get; set; }
        public bool ShowActions { get; set; }
    }
}