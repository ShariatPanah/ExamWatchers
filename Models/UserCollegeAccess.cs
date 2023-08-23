using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class UserCollegeAccess
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserCode { get; set; }

        [ForeignKey("College")]
        public int CollegeId { get; set; }

        public virtual User User { get; set; }
        public virtual College College { get; set; }
    }
}
