using System;
using System.ComponentModel.DataAnnotations;

namespace ExamWatchers.ViewModels
{
    public class CreateExamViewModel
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
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public string ExamStartTime { get; set; }
        //public DateTime ExamStartTime { get; set; }

        [Display(Name = "زمان پایان")]
        [Required(ErrorMessage = "زمان پایان امتحان را وارد کنید!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public string ExamEndTime { get; set; }
        //public DateTime ExamEndTime { get; set; }

        [Display(Name = "ظرفیت مراقبین")]
        [Required(ErrorMessage = "ظرفیت مراقبین را تعیین کنید.")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:### نفر}")]
        [Range(minimum: 1, maximum: 255, ErrorMessage = "ظرفیت مراقبین باید عددی بین 1 تا 255 باشد!")]
        public byte WatchersCapacity { get; set; }

        [Display(Name = "ظرفیت باقیمانده")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:### نفر}")]
        [ScaffoldColumn(false)]
        public byte RemainingCapacity { get; set; }

        [Display(Name = "حوزه امتحانی")]
        [ScaffoldColumn(false)]
        public int ExamPlaceId { get; set; }

        [Display(Name = "دانشکده")]
        [ScaffoldColumn(false)]
        public int CollegeId { get; set; }
    }
}