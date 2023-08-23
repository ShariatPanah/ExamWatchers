using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class User
    {
        public User()
        {
            IsConfirmed = false;
            RegisterDate = DateTime.Now;
            Accesses = new List<UserCollegeAccess>();
        }

        [Key]
        [Display(Name = "کد پرسنلی")]
        [Required(ErrorMessage = "کد پرسنلی را وارد کنید!")]
        [StringLength(maximumLength: 15, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای کد پرسنلی 15 کاراکتر می باشد.")]
        public string PersonnelCode { get; set; }

        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز عبور را وارد کنید!")]
        [StringLength(maximumLength: 20, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای رمز عبور 20 کاراکتر می باشد.")]
        public string Password { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "نام را وارد کنید!")]
        [StringLength(maximumLength: 25, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای نام 25 کاراکتر می باشد.")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "نام خانوادگی را وارد کنید!")]
        [StringLength(maximumLength: 40, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای نام خانوادگی 40 کاراکتر می باشد.")]
        public string LastName { get; set; }

        [Display(Name ="نقش کاربری")]
        [StringLength(30, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای نقش کاربری 30 کاراکتر می باشد")]
        public string Role { get; set; }

        [Display(Name = "بخش سازمانی")]
        public EnumTypes.OrganTypes Organ { get; set; }

        [Display(Name = "تایید شده؟")]
        [ScaffoldColumn(false)]
        public bool IsConfirmed { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        [Required(ErrorMessage = "تاریخ ثبت نام را وارد کنید!")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        [ScaffoldColumn(false)]
        public DateTime RegisterDate { get; set; }

        public virtual IList<UserCollegeAccess> Accesses { get; set; }
    }
}
