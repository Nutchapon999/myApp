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
        public string IDPGroupId { get; set; }
        public string Pl { get; set; }
        public bool Critical { get; set; }

        public virtual Competency Competency { get; set; }
    }
}