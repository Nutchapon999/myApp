using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public int ResultItem { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string ColumnUpdated { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set;}

    }
}