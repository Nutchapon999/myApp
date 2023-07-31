using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using myApp.DAL;
using myApp.Models;
using somboonCL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
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

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);
        OleDbConnection Econ;

        //Index
        public ActionResult Index(string Year)
        {
            var username = "";

            // Cookie
            if (ConfigurationManager.AppSettings["IsDev"].ToString().ToLower() == "true")
            {
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "suchada.t"); // GM 1050100
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "Pond.Popza"); // User 2000000
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "pissamai.t"); //User 1050100
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "rattanaporn.p"); //User 1050100
                System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "Ong-Ard.sin"); // Admin & GM 1050100
                HttpContext.Response.Cookies.Add(UserCookie);
                username = UserCookie.Value;

            }
            else
            {
                username = K2UserAuthen.GetUserAut(ConfigurationManager.AppSettings["PageK2Five"].ToString()).Username;
            }


            List<UserFormAuth> auths = app.GetUserFormAuths();
            bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
            //bool isGM = auths.Exists(auth => auth.Username == username && auth.ObjectName == "COST_CENTER" && auth.Value == "2000000");

            ViewBag.isAdmin = isAdmin;
            //ViewBag.isGM = isGM;
            ViewBag.Username = username;

            if (string.IsNullOrEmpty(Year))
            {
                Year = (DateTime.Now.Year + 543).ToString();
            }

            ViewBag.Year = Year;

            List<Enrollment> enrollments = app.GetEnrollEachYearByUsername(username, Year);
            return View(enrollments);
        }
        public ActionResult Form(string idpGroupId, string guid)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
                bool isGM = auths.Exists(auth => auth.Username == username && auth.ObjectName == "COST_CENTER" && auth.Value == "1050100");
                ViewBag.isAdmin = isAdmin;
                ViewBag.isGM = isGM;
                ViewBag.Username = username;

                string id = app.GetIdByGuid(guid);

                int enrollmentId = app.GetEnrollmentIdByIdAndIdpId(id, idpGroupId);
                
                string IDPGroupName = app.GetIDPGroupNameById(id, enrollmentId);
                string yearIDP = app.GetYearByEnrolled(enrollmentId);
                string status = app.GetStatus(id, idpGroupId);
                string approver = app.GetApprover(guid);

                ViewBag.EnrollmentId = enrollmentId;
                ViewBag.Id = id;
                ViewBag.Approver = approver;
                
                User user = app.GetUserByGuid(guid);
                if (user != null)
                {
                    ViewBag.Prefix = user.Prefix;
                    ViewBag.FirstName = user.FirstNameTH;
                    ViewBag.LastName = user.LastNameTH;
                    ViewBag.Company = user.Company;
                    ViewBag.Joblevel = user.JobLevel;
                    ViewBag.Department = user.Department;
                    ViewBag.Position = user.Position;
                    ViewBag.UserLogin = user.UserLogin;
                }
                ViewBag.Year = app.GetYearById(idpGroupId);
                ViewBag.IDPGroupId = idpGroupId;
                ViewBag.IDPGroupName = IDPGroupName;
                ViewBag.Guid = guid;
                ViewBag.Status = status;

                List<Enrollment> enrollments = app.GetFormsByGuid(enrollmentId, id, guid);
                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
            
        }
        [HttpPost]
        public ActionResult SaveResultDetails(int enrollId, Dictionary<string, ResultItem> forms, bool isChecked, string remark, string Guid)
        {
            string[] types = Request.Form.GetValues("Type");
            string[] companies = Request.Form.GetValues("Company");
            string[] date = Request.Form.GetValues("Date");
            string[] hour = Request.Form.GetValues("Hour");

            string username = Request.Cookies["username"].Value;
            string IDPGroup = app.GetIDPGroupIdByEnrollment(enrollId);
            string Id = app.GetIdByEnrollment(enrollId);
            string Year = app.GetYearByEnrolled(enrollId);
            bool isFormSubmitted = app.IsFormSubmitted(Id, IDPGroup);
            string position = app.GetJoblevelByCookie(username);
            string status = app.GetStatus(Id, IDPGroup);
            int count = app.GetCountCompetencyThisId(IDPGroup);
            string user = app.GetUserLoginByEnrollId(enrollId);

            if (types != null && companies != null && date != null && hour != null)
            {
                List<Goodness> goodnessList = new List<Goodness>();

                for (int i = 0; i < types.Length; i++)
                {
                    Goodness goodness = new Goodness
                    {
                        Type = types[i],
                        Company = companies[i],
                        Date = date[i],
                        Hour = hour[i],
                    };

                    goodnessList.Add(goodness);
                }

                // Insert data into the database
                app.InsertGoodness(goodnessList, Guid, user);
            }

            //INSERT AND UPDATE RESULTITEMS
            if (isFormSubmitted)
            {
                List<ResultItem> resultItemsBefore = app.GetResultItemByGuidBeforeUpdate(Guid);
                app.UpdateResultDetails(forms.Values, Guid);

                //REMARK
                if(status == "1st Evaluating" || status == "2nd Evaluating")
                {
                    app.InsertRemark(remark, username, position, Guid);
                }
                List<ResultItem> resultItemsAfter = app.GetResultItemByGuidAfterUpdate(Guid);
                List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
                app.InsertLogOnUpdateResultItems(resultItemIds, username, resultItemsBefore, resultItemsAfter, status, Guid);
            }
            else
            {
                app.InsertResultDetails(forms.Values, Guid, count);
                //LOG DATA
                List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
                List<ResultItem> resultItems = app.GetResultItemByGuidOnInsert(Guid);
                app.InsertLogOnInsertResultItems(resultItemIds, username, resultItems, Guid);
            }
                

            //Update Status
            if (!isChecked)
            { 
                bool is1stEvaluated = app.is1stEvaluated(enrollId);
                bool isDeveloped = app.isDeveloped(enrollId);
                bool is2ndEvaluated = app.is2ndEvaluated(enrollId);
                if (is1stEvaluated)
                {
                    app.UpdateEnrollmentStatus_3(Id, IDPGroup); //Developing
                }
                else if (isDeveloped)
                {
                    app.UpdateEnrollmentStatus_4(Id, IDPGroup); //2nd Evaluating
                }
                else if (is2ndEvaluated)
                {
                    app.UpdateEnrollmentStatus_5(Id, IDPGroup); //Success
                }
                else
                {
                    app.UpdateEnrollmentStatus_2(Id, IDPGroup); //1st Evaluating
                }
                app.UpdateApprover(username, Guid);
            }

            int all = app.GetCompetencyAllByStatus(Id, Year);
            int pass = app.GetCompetencyPassByGuid(Guid);
            int didThis = app.GetCountCompetencyThisId(IDPGroup);
            int didOther = app.GetCountCompetencyDid(Guid);

            //Calculate Values for Result
            if(status == "Evaluating")
            {
                if (all == didOther)
                {
                    didThis = didOther;
                }
                else
                {
                    didThis += didOther;
                }
            }
            
            float per = (float)pass / didThis * 100;

            string rank;

            if (per >= 100)
            {
                rank = "M";
            }
            else if (per < 100 && per >= 70)
            {
                rank = "C";
            }
            else
            {
                rank = "L";
            }

            app.UpdateResult(Guid, didThis, pass, per, rank);

            /*if(isChecked)
            {
                return RedirectToAction("Check", "Form");
            }*/

            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroup, guid = Guid});
            
        }
        public ActionResult Check()
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
                bool isGM = auths.Exists(auth => auth.Username == username && auth.ObjectName == "COST_CENTER" && auth.Value == "2000000" || auth.Value == "1050100");
                ViewBag.isAdmin = isAdmin;
                ViewBag.isGM = isGM;
                ViewBag.Username = username;

                string id = app.GetIdByCookie(username);
                string year = (DateTime.Now.Year + 543).ToString();

                List<string> departments = app.GetValuesByCookie(username);

                List<Enrollment> enrollments = app.GetCheckForms(year, username, departments);
                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        public ActionResult FormCheck(string idpGroupId, string user)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
                bool isGM = auths.Exists(auth => auth.Username == username && auth.ObjectName == "COST_CENTER" && auth.Value == "1050100");
                ViewBag.isAdmin = isAdmin;
                ViewBag.isGM = isGM;
                ViewBag.Username = username;

                string id = app.GetIdByCookie(username);
                
                ViewBag.Id = id;
                User emp = app.GetUserByCookie(user);
                if (emp != null)
                {
                    ViewBag.Prefix = emp.Prefix;
                    ViewBag.FirstName = emp.FirstNameTH;
                    ViewBag.LastName = emp.LastNameTH;
                    ViewBag.Company = emp.Company;
                    ViewBag.Joblevel = emp.JobLevel;
                    ViewBag.Department = emp.Department;
                    ViewBag.Position = emp.Position;
                }
                ViewBag.Year = app.GetYearById(idpGroupId);
                ViewBag.IDPGroupId = idpGroupId;

                int enrollmentUser = app.GetEnrollmentByCookie(user , idpGroupId);
                string year = app.GetYearByEnrolled(enrollmentUser);
                ViewBag.EnrollmentId = enrollmentUser;
                List<Enrollment> enrollments = app.GetFormsByCookie(enrollmentUser, user, year);
                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }

        }
        //info
        public ActionResult Info(string year)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;

                string id = app.GetIdByCookie(username);
                int count = app.GetCountEnrollmentById(id);
                //string year = (DateTime.Now.Year + 543).ToString();
                User user = app.GetUserByCookie(username);
                if (user != null)
                {
                    ViewBag.Prefix = user.Prefix;
                    ViewBag.FirstName = user.FirstNameTH;
                    ViewBag.LastName = user.LastNameTH;
                    ViewBag.Company = user.Company;
                    ViewBag.Joblevel = user.JobLevel;
                    ViewBag.Department = user.Department;
                    ViewBag.Position = user.Position;
                }
                ViewBag.Count = count;
                ViewBag.Year = year;
                ViewBag.Id = id;

                List<Enrollment> enrollments = app.GetInfoEmployeeByCookie(username,  year);

                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }

        
    }
}