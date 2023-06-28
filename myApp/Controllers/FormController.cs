using myApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myApp.Controllers
{
    public class FormController : Controller
    {
        private App app;

        public FormController()
        {
            app = new App();
        }

        public ActionResult Form_1()
        {

            return View();
        }
    }
}