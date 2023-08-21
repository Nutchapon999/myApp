using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class ResultItem
    {
        public string GUID { get; set; }
        public bool Critical { get; set; }
        public int ResultItemId { get; set; }
        public string IDPGroupId { get; set; }
        public string CompetencyId { get; set; }
        public int Requirement { get; set;}
        [Required]
        public int Actual1 { get; set;}
        public int Gap1 { get; set;}
        public string Priority { get; set;}
        public string TypePlan { get; set;}
        public string DevPlan { get; set;}
        public string Q1 { get; set;}
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
        public string DevRst { get; set;}
        public string FileId { get; set; }
        [Required]
        public int Actual2 { get; set; }
        public int Gap2 { get; set; }

        // คุณสมบัติที่เก็บค่าเริ่มต้นของแต่ละคอลัมน์
        public int OriginalActual1 { get; set; }
        public int OriginalGap1 { get; set; }
        public string OriginalPriority { get; set; }
        public string OriginalType { get; set; }
        public string OriginalDevPlan { get; set; }
        public string OriginalQ1 { get; set; }
        public string OriginalQ2 { get; set; }
        public string OriginalQ3 { get; set; }
        public string OriginalQ4 { get; set; }
        public string OriginalDevRst { get; set; }
        public string OriginalFileID { get; set; }
        public int OriginalActual2 { get; set; }
        public int OriginalGap2 { get; set; }

       
    }
}