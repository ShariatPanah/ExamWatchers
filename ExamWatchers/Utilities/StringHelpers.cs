using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class StringHelpers
{
    /// تبدیل اعداد فارسی یونیکد به اعداد انگلیسی با کد اسکی
    /// چون داخل کدهای سی شارپ میخوایم تبدیل انجام بدیم
    /// وقتی اعداد فارسی باشن ارور میگیریم
    public static string ToAsciiNumbers(this string num)
    {
        num = num.Replace('۰', '0');
        num = num.Replace('۱', '1');
        num = num.Replace('۲', '2');
        num = num.Replace('۳', '3');
        num = num.Replace('۴', '4');
        num = num.Replace('۵', '5');
        num = num.Replace('۶', '6');
        num = num.Replace('۷', '7');
        num = num.Replace('۸', '8');
        num = num.Replace('۹', '9');

        return num;
    }
}