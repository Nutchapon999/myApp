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
        public App app;

        public FormController()
        {
            app = new App();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}