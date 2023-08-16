using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class Result
    {
        public string GUID { get; set; }  
        public string K2_No { get; set; }
        public string FormType { get; set; }
        public string FormId { get; set; }
        public string IDPGroupID { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Plant { get; set; }
        public string Department { get; set; }
        public string CompanyCode { get; set; }
        public string Requisitioner { get; set; }
        public string RequisitionerEmail { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime StartWorkFlowOn { get; set; }
        public DateTime CompeletedOn { get; set; }
        public string CurrentApprover { get; set;}
        public int CompetencyAll { get; set; }
        public int CompetencyPass { get; set;}
        public float CompetencyPer { get; set;}
        public string Rank { get; set; }
        public string Year { get; set; }

        public virtual ResultItem ResultItem { get; set; }
        public virtual Competency Competency { get; set; }
    }
}