﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class IDPGroup
    {
        [Key]
        [DisplayName("รหัสหลักสูตร")]
        public string IDPGroupId { get; set;}
        [DisplayName("ชื่อหลักสูตร")]
        public string IDPGroupName { get; set;}
        [DisplayName("ปี")]
        public string Year { get; set;}
        [DisplayName("จำนวนคน")]
        public int EmployeeEnrollmentCount { get; set; }

        public virtual CompetencyItem CompetencyItem { get; set;}
        public virtual Enrollment Enrollment { get; set;}
        public virtual Competency Competency { get; set; }
        public virtual User User { get; set;}
    }
}