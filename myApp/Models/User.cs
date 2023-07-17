using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Prefix { get; set; }
        public string FirstNameTH { get; set; }
        public string LastNameTH { get; set; }
        public string FirstNameEN { get; set; }
        public string LastNameEN { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Position { get; set; }
        public string JobLevel { get; set; }
        public string CostCenter { get; set; }
        public string Department { get; set; }
        public string DepartmentName { get; set; }
        public string Email { get; set; }
        public string UserLogin { get; set; }
        public string Enabled { get; set; }
        public string ShiftWork { get; set; }
        public string WorkCenter { get; set; }
        public string HRPositionCode { get; set; }
        public string JobRole { get; set; }
        public string WorkAge { get; set; }
        public string StartWorkDate { get; set; }

        public virtual Enrollment Enrollment { get; set; }
        public virtual Result Result { get; set; }
    }
}