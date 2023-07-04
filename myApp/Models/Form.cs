using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class Form
    {
        public int FormId { get; set; }
        public string IDPGroupId { get; set; }
        public string Id { get; set;}
        public string CompetencyId { get; set; }
        public int Requirement { get; set;}
        public int Actual { get; set;}
        public int Gap { get; set;}
        public string Priority { get; set;}
        public string Plan { get; set;}
        public string PlanDesc { get; set;}
        public string Quarter { get; set;}
        public string RstPlan { get; set;}

        public virtual User User { get; set; }
        public virtual Enrollment Enrollment { get; set; }
        public virtual Result Results { get; set; }
        public virtual IDPGroup IDPGroup { get; set; }
        public virtual CompetencyItem CompetencyItem { get; set; }
        public virtual Competency Competency { get; set; }
       
    }
}