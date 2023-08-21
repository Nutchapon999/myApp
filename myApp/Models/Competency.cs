using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myApp.Models
{
    public class Competency
    {
        [Key]
        [DisplayName("รหัส Competency")]
        public string CompetencyId { get; set; }
        [DisplayName("ชื่อภาษาไทย")]
        public string CompetencyNameTH { get; set; }
        [DisplayName("ชื่อภาษาอังกฤษ")]
        public string CompetencyNameEN { get; set; }
        [DisplayName("รายละเอียด")]
        public string CompetencyDesc { get; set; }
        [DisplayName("PL1")]
        public string Pl1 { get; set; }
        [DisplayName("PL2")]
        public string Pl2 { get; set; }
        [DisplayName("PL3")]
        public string Pl3 { get; set; }
        [DisplayName("PL4")]
        public string Pl4 { get; set; }
        [DisplayName("PL5")]
        public string Pl5 { get; set; }
        [DisplayName("ชนิด")]
        public string Type { get; set; }
  
        [DisplayName("ใช้งานอยู่")]
        public bool Active { get; set; }
        public bool Delete { get; set; }
        public virtual IDPGroupItem IDPGroupItem { get; set; }
    }
}