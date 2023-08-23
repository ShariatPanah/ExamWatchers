using System.ComponentModel.DataAnnotations;

namespace ExamWatchers.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "کد پرسنلی")]
        [Required(ErrorMessage = "کد پرسنلی را وارد کنید!")]
        [StringLength(maximumLength: 15, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای کد پرسنلی 15 کاراکتر می باشد.")]
        public string PersonnelCode { get; set; }

        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز عبور را وارد کنید!")]
        [StringLength(maximumLength: 20, ErrorMessage = "حداکثر تعداد کاراکتر مجاز برای رمز عبور 20 کاراکتر می باشد.")]
        public string Password { get; set; }
    }
}