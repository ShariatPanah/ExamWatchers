﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Models
{
    public class EnumTypes
    {
        public enum OrganTypes : byte
        {
            [Display(Name = "معاونت")]
            معاونت = 1,

            [Display(Name = "پژوهشی")]
            پژوهشی = 2,

            [Display(Name = "مالی")]
            مالی = 3,

            [Display(Name = "آموزش")]
            آموزش = 4
        }

        public static List<SelectListItem> PairedHoursList = new List<SelectListItem>()
        {
            new SelectListItem { Text = "07:00", Value = "07:00"},
            new SelectListItem { Text = "08:00", Value = "08:00"},
            new SelectListItem { Text = "09:00", Value = "09:00"},
            new SelectListItem { Text = "10:00", Value = "10:00"},
            new SelectListItem { Text = "11:00", Value = "11:00"},
            new SelectListItem { Text = "12:00", Value = "12:00"},
            new SelectListItem { Text = "13:00", Value = "13:00"},
            new SelectListItem { Text = "14:00", Value = "14:00"},
            new SelectListItem { Text = "15:00", Value = "15:00"},
            new SelectListItem { Text = "16:00", Value = "16:00"},
            new SelectListItem { Text = "17:00", Value = "17:00"},
            new SelectListItem { Text = "18:00", Value = "18:00"},
            new SelectListItem { Text = "19:00", Value = "19:00"},
            new SelectListItem { Text = "20:00", Value = "20:00"},
            new SelectListItem { Text = "21:00", Value = "21:00"},
            new SelectListItem { Text = "22:00", Value = "22:00"},
            new SelectListItem { Text = "23:00", Value = "23:00"},
            new SelectListItem { Text = "00:00", Value = "00:00"},
            new SelectListItem { Text = "01:00", Value = "01:00"},
            new SelectListItem { Text = "02:00", Value = "02:00"},
            new SelectListItem { Text = "03:00", Value = "03:00"},
            new SelectListItem { Text = "04:00", Value = "04:00"},
            new SelectListItem { Text = "05:00", Value = "05:00"},
            new SelectListItem { Text = "06:00", Value = "06:00"}
        };
    }
}
