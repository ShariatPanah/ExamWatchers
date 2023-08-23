using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class College
    {
        public College()
        {
            ModifiedDate = DateTime.Now;
            ExamPlaces = new List<ExamPlace>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "عنوان دانشکده")]
        [Required(ErrorMessage = "عنوان دانشکده را وارد کنید!")]
        [StringLength(maximumLength: 30, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای عنوان 30 کاراکتر می باشد.")]
        public string Title { get; set; }

        [Display(Name = "آخرین ویرایش")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        [ScaffoldColumn(false)]
        public DateTime ModifiedDate { get; set; }

        public virtual IList<ExamPlace> ExamPlaces { get; set; }
    }
}
