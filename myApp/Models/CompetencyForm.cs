using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class CompetencyForm
    {
        [Key]
        [DisplayName("รหัสหลักสูตร")]
        public string CompetencyFormId { get; set;}
        [DisplayName("ชื่อหลักสูตร")]
        public string CompetencyFormName { get; set;}
        [DisplayName("ปี")]
        public string Year { get; set;}

        public virtual User User { get; set;}
    }
}