using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace myApp.Models
{
    public class UserFormAuth
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FormId { get; set; }
        public string ObjectName { get; set; }
        public string Value { get; set; }
        public string GroupId { get; set; }
    }
}