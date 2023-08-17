using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
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
                System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "suchada.t"); // GM 1050100
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "Pond.Popza"); // User 2000000
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "pissamai.t"); //User 1050100
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "rattanaporn.p"); //User 1050100
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "Ong-Ard.sin"); // Admin & GM 1050100
                HttpContext.Response.Cookies.Add(UserCookie);
                username = UserCookie.Value;

            }
            else
            {
                username = K2UserAuthen.GetUserAut(ConfigurationManager.AppSettings["PageK2Five"].ToString()).Username;
            }


            List<UserFormAuth> auths = app.GetUserFormAuths();
            bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
            bool isGood = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Goodness");

            ViewBag.isAdmin = isAdmin;
            ViewBag.isGood = isGood;
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
                ViewBag.isAdmin = isAdmin;
                
                ViewBag.Username = username;

                string id = app.GetIdByGuid(guid);

                int enrollmentId = app.GetEnrollmentIdByIdAndIdpId(id, idpGroupId);
                
                string IDPGroupName = app.GetIDPGroupNameById(id, enrollmentId);
                string year = app.GetYearByEnrolled(enrollmentId);
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
                    ViewBag.DepartmentName = user.DepartmentName;
                    ViewBag.Position = user.Position;
                    ViewBag.UserLogin = user.UserLogin;
                }
                ViewBag.Year = app.GetYearById(idpGroupId);
                ViewBag.IDPGroupId = idpGroupId;
                ViewBag.IDPGroupName = IDPGroupName;
                ViewBag.Guid = guid;
                ViewBag.Status = status;

                List<RemarkHS> remarkHS = app.GetRemark(guid);
                List<Goodness> goodnesses = app.GetGoodnessByUser(ViewBag.UserLogin, year);

                ViewBag.Remark = remarkHS;
                ViewBag.Goodness = goodnesses;

                //List<ResultItem> actual2 = app.GetPreActual2(ViewBag.UserLogin, year);


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
            string username = Request.Cookies["username"].Value;
            string IDPGroup = app.GetIDPGroupIdByEnrollment(enrollId);
            string Id = app.GetIdByEnrollment(enrollId);
            string Year = app.GetYearByEnrolled(enrollId);
            bool isFormSubmitted = app.IsFormSubmitted(Id, IDPGroup);
            string joblevel = app.GetJoblevelByCookie(username);
            string position = app.GetPositionByCookie(username);
            string status = app.GetStatus(Id, IDPGroup);
            int count = app.GetCountCompetencyThisId(IDPGroup);
            string user = app.GetUserLoginByEnrollId(enrollId);

            List<ResultItem> actual1NotNullItems = new List<ResultItem>();
            List<ResultItem> actual1NullItems = new List<ResultItem>();

            List<ResultItem> actual2NotNullItems = new List<ResultItem>();
            List<ResultItem> actual2NullItems = new List<ResultItem>();

            foreach (var resultItem in forms.Values)
            {
                if(resultItem.Actual1 != 0)
                {
                    actual1NotNullItems.Add(resultItem);
                }
                else
                {
                    actual1NullItems.Add(resultItem);
                }

                if(resultItem.Actual2 != 0)
                {
                    actual2NotNullItems.Add(resultItem);
                }
                else
                {
                    actual2NullItems.Add(resultItem);
                }
            }
            if (actual1NotNullItems.Count > 0 || actual2NotNullItems.Count > 0)
            {
                //INSERT AND UPDATE RESULTITEMS
                if (isFormSubmitted)
                {
                    //LOG DATA
                    List<ResultItem> resultItemsBefore = app.GetResultItemByGuidBeforeUpdate(Guid);
                    //UPDATE RESULTITEMS
                    app.UpdateResultDetails(forms.Values, Guid);

                    //REMARK
                    if (status == "1st Evaluating" || (status == "2nd Evaluating" && actual2NullItems.Count == 0))
                    {
                        app.InsertRemark(remark, username, joblevel, Guid);
                    }
                    //LOG DATA
                    List<ResultItem> resultItemsAfter = app.GetResultItemByGuidAfterUpdate(Guid);
                    List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
                    app.InsertLogOnUpdateResultItems(resultItemIds, username, resultItemsBefore, resultItemsAfter, status, Guid);
                }
                else
                {
                    //INSERT RESULTITEMS
                    //app.InsertResultDetails(forms.Values, Guid, count);
                    //LOG DATA
                    //List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
                    //List<ResultItem> resultItems = app.GetResultItemByGuidOnInsert(Guid);
                    //app.InsertLogOnInsertResultItems(resultItemIds, username, resultItems, Guid);
                }
            }
            if (actual1NullItems.Count > 0)
            {
                TempData["ErrorMessage"] = "มี Actual1 บางแถวที่ยังไม่ได้เลือก";
                return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroup, guid = Guid });
            }
            if(status == "2nd Evaluating")
            {
                if (actual2NullItems.Count > 0)
                {
                    TempData["ErrorMessage"] = "มี Actual2 บางแถวที่ยังไม่ได้เลือก";
                    return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroup, guid = Guid });
                }
            }



            //UPDATE STATUS(SUBMIT)
            if (!isChecked)
            {
                //WORKFLOW UPDATE
                if (status == "1st Evaluating" || status == "2nd Evaluating")
                {
                    RemarkHS remarkId = app.GetDescRemarkId(Guid);
                    app.InsertWorkflowHS2(position, username, status, remarkId);
                }
                else
                {
                    app.InsertWorkflowHS1(position, username, status);
                }

                bool is1stEvaluated = app.is1stEvaluated(enrollId);
                bool isDeveloped = app.isDeveloped(enrollId);
                bool is2ndEvaluated = app.is2ndEvaluated(enrollId);
                if (is1stEvaluated)
                {
                    app.UpdateEnrollmentStatus_3(Id, IDPGroup); //DEVELOPING
                }
                else if (isDeveloped)
                {
                    app.UpdateEnrollmentStatus_4(Id, IDPGroup); //2ND EVALUATING
                }
                else if (is2ndEvaluated)
                {
                    app.UpdateEnrollmentStatus_5(Id, IDPGroup); //SUCCESS
                }
                else
                {
                    app.UpdateEnrollmentStatus_2(Id, IDPGroup); //1ST EVALUATING
                }
                
                app.UpdateApprover(username, Guid);
            }

            int all = app.GetCompetencyAllByGuid(Guid);
            int pass = app.GetCompetencyPassByGuid(Guid);

            //CALCULATE VALUES FOR RESULT
            float per = (float)pass / all * 100;
            string rank;

            switch (per)
            {
                case var p when p >= 100:
                    rank = "M";
                    break;
                case var p when p < 100 && p >= 70:
                    rank = "C";
                    break;
                default:
                    rank = "L";
                    break;
            }

            app.UpdateResult(Guid, pass, per, rank);

            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroup, guid = Guid});
            
        }
        public ActionResult Goodness(string Year)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {

                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                ViewBag.Year = Year;
                List<Goodness> goodnessList = app.GetGoodnessByUser(username, Year);
                return View(goodnessList);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult InsertGoodness(string Year)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                string[] types = Request.Form.GetValues("Type");;
                string[] companies = Request.Form.GetValues("Company");
                string[] date = Request.Form.GetValues("Date");
                string[] hour = Request.Form.GetValues("Hour");
                string[] desc = Request.Form.GetValues("Desc");

                types = types.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (types != null && companies != null && date != null && hour != null)
                {
                    List<Goodness> goodnessList = new List<Goodness>();

                    for (int i = 0; i < types.Length; i++)
                    {
                        Goodness goodness = new Goodness
                        {
                            Type = types[i],
                            //Type = otherTypes[i],
                            Company = companies[i],
                            Date = date[i],
                            Hour = hour[i],
                            Desc = desc[i]
                        };

                        goodnessList.Add(goodness);
                    }

                    app.InsertGoodness(goodnessList, username, Year);
                }

                return RedirectToAction("Goodness", "Form", new { year = Year });
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
            
        }


        //INFO
        public ActionResult Info(string idpGroupId, string guid)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                string year = app.GetYearByGuid(guid);
                string idpGroupName = app.GetIDPGroupNameByIDPGroupId(idpGroupId);

                string id = app.GetIdByCookie(username);
                int count = app.GetCountEnrollmentById(id);
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
                ViewBag.Id = id;
                ViewBag.IDPGroupID = idpGroupId;
                ViewBag.IDPGroupName = idpGroupName;
                ViewBag.Guid = guid;
                string status = app.GetStatus(id, idpGroupId);
                ViewBag.Status = status;

                ViewBag.Year = year;

                List<Result> results = app.GetInfoEmployeeByGuid(guid);

                return View(results);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
    }
}