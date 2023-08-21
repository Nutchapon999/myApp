using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class Goodness
    {
        public int GDId { get; set; }
        public string Type { get; set; }
        public string Company { get; set; }
        public string Date { get; set; }
        public string Hour { get; set; }
        public string Desc { get; set; }
        public string FileID { get; set; }

        public virtual User User { get; set; }
    }
}