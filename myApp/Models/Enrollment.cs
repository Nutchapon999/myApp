using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollId { get; set; } 
        public string Id { get; set; }
        public string CompetencyFormId { get; set; }

        public virtual CompetencyForm CompetencyForm { get; set; }
        public virtual User User { get; set; }
    }
}