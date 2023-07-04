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
        public string IDPGroupId { get; set; }
        public bool Finish { get; set; }
        public int CompetencyAll { get; set; }
        public int CompetencyPass { get; set; }
        public int CompetencyPer { get; set; }


        public virtual IDPGroup IDPGroup { get; set; }
        public virtual User User { get; set; }
        public virtual CompetencyItem CompetencyItem { get; set; }
        public virtual Competency Competency { get; set; }
        public virtual Form Form { get; set; }


    }
}