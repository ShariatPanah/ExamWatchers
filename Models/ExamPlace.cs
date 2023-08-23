using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ExamPlace
    {
        public ExamPlace()
        {
            ModifiedDate = DateTime.Now;
            Exams = new List<Exam>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "عنوان حوزه امتحانی")]
        [Required(ErrorMessage = "عنوان حوزه را وارد کنید!")]
        [StringLength(maximumLength: 80, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای عنوان 80 کاراکتر می باشد.")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        //[Required(ErrorMessage = "توضیحات را وارد کنید!")]
        [StringLength(maximumLength: 200, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای توضیحات 200 کاراکتر می باشد.")]
        public string Description { get; set; }

        [Display(Name = "آخرین ویرایش")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        [ScaffoldColumn(false)]
        public DateTime ModifiedDate { get; set; }

        [Display(Name ="دانشکده")]
        [ForeignKey("College")]
        [ScaffoldColumn(false)]
        public int CollegeId { get; set; }
        public virtual College College { get; set; }

        public virtual IList<Exam> Exams { get; set; }
    }
}
