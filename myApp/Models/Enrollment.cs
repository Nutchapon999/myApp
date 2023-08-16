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
        public string Status { get; set; }
        public int Competencies { get; set; }

        public virtual IDPGroup IDPGroup { get; set; }
        public virtual User User { get; set; }
        public virtual IDPGroupItem IDPGroupItem { get; set; }
        public virtual Competency Competency { get; set; }
        public virtual ResultItem ResultItem { get; set; }
        public virtual Result Result { get; set; }
        public virtual RemarkHS RemarkHS { get; set; }


    }
}