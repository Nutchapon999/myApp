using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class CompetencyItem
    {
        public int CompetencyItemId { get; set; }   
        public string CompetencyId { get; set; }
        public string CompetencyFormId { get; set; }
        public string Pl { get; set; }
        public string Priority { get; set; }

        public virtual Competency Competency { get; set; }
    }
}