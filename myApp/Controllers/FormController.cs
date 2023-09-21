using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using myApp.DAL;
using myApp.Models;
using somboonCL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myApp.Controllers
{
    public class FormController : Controller
    {
        private App app;
        private WorkFlow workFlow = new WorkFlow();

        public FormController()
        {
            app = new App();
        }

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);
        OleDbConnection Econ;

        #region Index
        public ActionResult Index(string Year)
        {
            var username = "";

            // Cookie
            if (ConfigurationManager.AppSettings["IsDev"].ToString().ToLower() == "true")
            {
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "suchada.t"); // Goodness
                System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "Ong-Ard.sin"); // Admin 
                //System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "Rattanachai.p"); // User 
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
            List<WorkFlow> workFlows = workFlow.GetWorkflows(username, Year);
            ViewBag.WorkFlows = workFlows;
            return View(enrollments);
        }
        #endregion

        #region FORM    
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
        //[HttpPost]
        //public ActionResult SaveResultDetails(int enrollId, Dictionary<string, ResultItem> forms, bool isChecked, string remark, string Guid)
        //{
        //    string username = Request.Cookies["username"].Value;
        //    string IDPGroup = app.GetIDPGroupIdByEnrollment(enrollId);
        //    string Id = app.GetIdByEnrollment(enrollId);
        //    string Year = app.GetYearByEnrolled(enrollId);
        //    bool isFormSubmitted = app.IsFormSubmitted(Id, IDPGroup);
        //    string joblevel = app.GetJoblevelByCookie(username);
        //    string position = app.GetPositionByCookie(username);
        //    string status = app.GetStatus(Id, IDPGroup);
        //    int count = app.GetCountCompetencyThisId(IDPGroup);
        //    string user = app.GetUserLoginByEnrollId(enrollId);

        //    List<ResultItem> actual1NotNullItems = new List<ResultItem>();
        //    List<ResultItem> actual1NullItems = new List<ResultItem>();

        //    List<ResultItem> actual2NotNullItems = new List<ResultItem>();
        //    List<ResultItem> actual2NullItems = new List<ResultItem>();

        //    foreach (var resultItem in forms.Values)
        //    {
        //        if(resultItem.Actual1 != 0)
        //        {
        //            actual1NotNullItems.Add(resultItem);
        //        }
        //        else
        //        {
        //            actual1NullItems.Add(resultItem);
        //        }

        //        if(resultItem.Actual2 != 0)
        //        {
        //            actual2NotNullItems.Add(resultItem);
        //        }
        //        else
        //        {
        //            actual2NullItems.Add(resultItem);
                    
        //        }
        //    }
        //    if (actual1NotNullItems.Count > 0 || actual2NotNullItems.Count > 0)
        //    {
        //        //INSERT AND UPDATE RESULTITEMS
        //        if (isFormSubmitted)
        //        {
        //            //LOG DATA
        //            List<ResultItem> resultItemsBefore = app.GetResultItemByGuidBeforeUpdate(Guid);
        //            //UPDATE RESULTITEMS
        //            app.UpdateResultDetails(forms.Values, Guid);

        //            //REMARK
        //            if (status == "1st Evaluating" || (status == "2nd Evaluating" && actual2NullItems.Count == 0))
        //            {
        //                app.InsertRemark(remark, username, joblevel, Guid);
        //            }
        //            //LOG DATA
        //            List<ResultItem> resultItemsAfter = app.GetResultItemByGuidAfterUpdate(Guid);
        //            List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
        //            app.InsertLogOnUpdateResultItems(resultItemIds, username, resultItemsBefore, resultItemsAfter, status, Guid);
        //        }
        //        else
        //        {
        //            //INSERT RESULTITEMS
        //            //app.InsertResultDetails(forms.Values, Guid, count);
        //            //LOG DATA
        //            //List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
        //            //List<ResultItem> resultItems = app.GetResultItemByGuidOnInsert(Guid);
        //            //app.InsertLogOnInsertResultItems(resultItemIds, username, resultItems, Guid);
        //        }
        //    }
        //    if (actual1NullItems.Count > 0)
        //    {
        //        TempData["ErrorMessage"] = "มี Actual1 บางแถวที่ยังไม่ได้เลือก";
        //        return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroup, guid = Guid });
        //    }
        //    if(status == "2nd Evaluating")
        //    {
        //        if (actual2NullItems.Count > 0)
        //        {
        //            TempData["ErrorMessage"] = "มี Actual2 บางแถวที่ยังไม่ได้เลือก";
        //            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroup, guid = Guid });
        //        }
        //    }



        //    //UPDATE STATUS(SUBMIT)
        //    if (!isChecked)
        //    {
        //        //WORKFLOW UPDATE
        //        if (status == "1st Evaluating" || status == "2nd Evaluating")
        //        {
        //            RemarkHS remarkId = app.GetDescRemarkId(Guid);
        //            app.InsertWorkflowHS2(position, username, status, remarkId);
        //        }
        //        else
        //        {
        //            app.InsertWorkflowHS1(position, username, status);
        //        }

        //        bool is1stEvaluated = app.is1stEvaluated(enrollId);
        //        bool isDeveloped = app.isDeveloped(enrollId);
        //        bool is2ndEvaluated = app.is2ndEvaluated(enrollId);
        //        if (is1stEvaluated)
        //        {
        //            app.UpdateEnrollmentStatus_3(Id, IDPGroup); //DEVELOPING
        //        }
        //        else if (isDeveloped)
        //        {
        //            app.UpdateEnrollmentStatus_4(Id, IDPGroup); //2ND EVALUATING
        //        }
        //        else if (is2ndEvaluated)
        //        {
        //            app.UpdateEnrollmentStatus_5(Id, IDPGroup); //SUCCESS
        //        }
        //        else
        //        {
        //            app.UpdateEnrollmentStatus_2(Id, IDPGroup); //1ST EVALUATING
        //        }
                
        //        app.UpdateApprover(username, Guid);
        //    }

        //    int all = app.GetCompetencyAllByGuid(Guid);
        //    int pass = app.GetCompetencyPassByGuid(Guid);

        //    //CALCULATE VALUES FOR RESULT
        //    float per = (float)pass / all * 100;
        //    string rank;

        //    switch (per)
        //    {
        //        case var p when p >= 100:
        //            rank = "M";
        //            break;
        //        case var p when p < 100 && p >= 70:
        //            rank = "C";
        //            break;
        //        default:
        //            rank = "L";
        //            break;
        //    }

        //    app.UpdateResult(Guid, pass, per, rank);

        //    return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroup, guid = Guid});
            
        //}

        [HttpPost]
        public ActionResult SaveForm(int enrollId, string Guid, string IDPGroupId, bool isSave, string remark)
        {
            string fileUploadPath = ConfigurationManager.AppSettings["FileUploadPath"].ToString();
            string username = Request.Cookies["username"].Value;
            string Id = app.GetIdByEnrollment(enrollId);
            var form = HttpContext.Request.Form;
            List<ResultItem> resultItems = new List<ResultItem>();
            string status = app.GetStatus(Id, IDPGroupId);
            string joblevel = app.GetJoblevelByCookie(username);
            string position = app.GetPositionByCookie(username);

            int count = app.GetCompetencyAllByGuid(Guid);

            List<ResultItem> actual1NotNullItems = new List<ResultItem>();
            List<ResultItem> actual1NullItems = new List<ResultItem>();

            List<ResultItem> actual2NotNullItems = new List<ResultItem>();
            List<ResultItem> actual2NullItems = new List<ResultItem>();

            List<ResultItem> criticalRes = new List<ResultItem>();
            List<ResultItem> hasGap = new List<ResultItem>();

            for (var i = 0; i < count; i++)
            {
                var criticalKey = "Critical_" + i;
                var requireKey = "Requirement_" + i;
                var actual1Key = "Actual1_" + i;
                var priorityKey = "Priority_" + i;
                var typeKey = "TypePlan_" + i;
                var devPlanKey = "DevPlan_" + i;
                var Q1key = "Q1_" + i;
                var Q2Key = "Q2_" + i;
                var Q3Key = "Q3_" + i;
                var Q4Key = "Q4_" + i;
                var devRstKey = "DevRst_" + i;
                var fileKey = "File_" + i;
                var actual2Key = "Actual2_" + i;

                var criticalValue = form[criticalKey];

                var requireValue = form[requireKey];
                var actual1Value = form[actual1Key];
                var priorityValue = form[priorityKey];
                var typeValue = form[typeKey];
                var devPlanValue = string.IsNullOrEmpty(form[devPlanKey]) ? null : form[devPlanKey];
                var Q1Value = form[Q1key];
                var Q2Value = form[Q2Key];
                var Q3Value = form[Q3Key];
                var Q4Value = form[Q4Key];
                var devRstValue = form[devRstKey];
                var actual2Value = form[actual2Key];
                var fileValue = Request.Files[fileKey];
                

                int parsedRequire = Convert.ToInt32(requireValue);
                int parsedActual1 = Convert.ToInt32(actual1Value);
                int parsedActual2 = Convert.ToInt32(actual2Value);

                bool parsedCritical;
                bool.TryParse(criticalValue, out parsedCritical);

                ResultItem resultItem = new ResultItem
                {
                    Critical = parsedCritical,
                    Requirement = parsedRequire,
                    Actual1 = parsedActual1,
                    Priority = priorityValue,
                    TypePlan = typeValue,
                    DevPlan = devPlanValue,
                    Q1 = Q1Value,
                    Q2 = Q2Value,
                    Q3 = Q3Value,
                    Q4 = Q4Value,
                    DevRst = devRstValue,
                    Actual2 = parsedActual2

                };

                if (resultItem.Actual1 != 0)
                {
                    actual1NotNullItems.Add(resultItem);
                }
                else
                {
                    actual1NullItems.Add(resultItem);
                }

                if (resultItem.Actual2 != 0)
                {
                    actual2NotNullItems.Add(resultItem);
                }
                else
                {
                    actual2NullItems.Add(resultItem);

                }

                if(resultItem.Critical == true)
                {
                    criticalRes.Add(resultItem);
                }
                else
                {
                    hasGap.Add(resultItem);
                }

                resultItems.Add(resultItem);

            }

            if (actual1NotNullItems.Count > 0 || actual2NotNullItems.Count > 0)
            {
                //LOG DATA
                List<ResultItem> resultItemsBefore = app.GetResultItemByGuidBeforeUpdate(Guid);
                //UPDATE RESULTITEMS
                app.UpdateForm(resultItems, Guid);

                
                //LOG DATA
                List<ResultItem> resultItemsAfter = app.GetResultItemByGuidAfterUpdate(Guid);
                List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
                app.InsertLogUser(resultItemIds, username, resultItemsBefore, resultItemsAfter, status, Guid);
            }

            if (!isSave)
            {

                if (actual1NullItems.Count > 0)
                {
                    TempData["ErrorMessage"] = "มี Actual1 บางแถวที่ยังไม่ได้เลือก";
                    return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                }
                if (status == "2nd Evaluating")
                {
                    if (actual2NullItems.Count > 0)
                    {
                        TempData["ErrorMessage"] = "มี Actual2 บางแถวที่ยังไม่ได้เลือก";
                        return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                    }
                }
                foreach (var criticalResultItem in criticalRes)
                {
                    if (string.IsNullOrEmpty(criticalResultItem.Priority) && criticalResultItem.Actual1 < criticalResultItem.Requirement)
                    {
                        TempData["ErrorMessage"] = "มี Competency ที่เป็น Critical และมี Gap แต่ยังไม่ได้ระบุ Priority";
                        return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                    }
                    else if (string.IsNullOrEmpty(criticalResultItem.TypePlan) && criticalResultItem.Actual1 < criticalResultItem.Requirement)
                    {
                        TempData["ErrorMessage"] = "มี Competency ที่เป็น Critical และมี Gap แต่ยังไม่ได้ระบุ TypePlan";
                        return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                    }
                    if (string.IsNullOrEmpty(criticalResultItem.DevPlan) && criticalResultItem.Actual1 < criticalResultItem.Requirement)
                    {
                        TempData["ErrorMessage"] = "มี Competency ที่เป็น Critical และมี Gap แต่ยังไม่ได้ระบุ Development Plan";
                        return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                    }
                    else if (string.IsNullOrEmpty(criticalResultItem.Q1) &&
                            string.IsNullOrEmpty(criticalResultItem.Q2) &&
                            string.IsNullOrEmpty(criticalResultItem.Q3) &&
                            string.IsNullOrEmpty(criticalResultItem.Q4) &&
                            criticalResultItem.Actual1 < criticalResultItem.Requirement)
                    {
                        TempData["ErrorMessage"] = "มี Competency ที่เป็น Critical และมี Gap แต่ยังไม่ได้ระบุ Quarter";
                        return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                    }
                    
                    else if (string.IsNullOrEmpty(criticalResultItem.DevRst) && (status == "2nd Evaluating" || status == "Developing") && criticalResultItem.Actual1 < criticalResultItem.Requirement)
                    {
                        TempData["ErrorMessage"] = "มี Competency ที่เป็น Critical และมี Gap แต่ยังไม่ได้ระบุ Development Result";
                        return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                    }
                }
                if ((status == "1st Evaluating" && actual1NullItems.Count == 0) || (status == "2nd Evaluating" && actual2NullItems.Count == 0))
                {
                    if (remark == "") { remark = null; }
                    app.InsertRemark(remark, username, joblevel, Guid);
                }
                //foreach(var hasGapResultItem in hasGap)
                //{
                //    if(hasGapResultItem.Actual1 < hasGapResultItem.Requirement)
                //    {
                //        if (string.IsNullOrEmpty(hasGapResultItem.Priority))
                //        {
                //            TempData["ErrorMessage"] = "มี Competency ที่เป็น Gap แต่ยังไม่ได้ระบุ Priority";
                //            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                //        }
                //        else if (string.IsNullOrEmpty(hasGapResultItem.TypePlan))
                //        {
                //            TempData["ErrorMessage"] = "มี Competency ที่เป็น Gap แต่ยังไม่ได้ระบุ TypePlan";
                //            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                //        }
                //        else if (string.IsNullOrEmpty(hasGapResultItem.DevPlan))
                //        {
                //            TempData["ErrorMessage"] = "มี Competency ที่เป็น Gap แต่ยังไม่ได้ระบุ Development Plan";
                //            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                //        }
                //        else if (string.IsNullOrEmpty(hasGapResultItem.Q1) &&
                //                string.IsNullOrEmpty(hasGapResultItem.Q2) &&
                //                string.IsNullOrEmpty(hasGapResultItem.Q3) &&
                //                string.IsNullOrEmpty(hasGapResultItem.Q4))
                //        {
                //            TempData["ErrorMessage"] = "มี Competency ที่เป็น Gap แต่ยังไม่ได้ระบุ Quarter";
                //            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                //        }

                //        else if (string.IsNullOrEmpty(hasGapResultItem.DevRst) && (status == "2nd Evaluating" || status == "Developing"))
                //        {
                //            TempData["ErrorMessage"] = "มี Competency ที่เป็น Gap แต่ยังไม่ได้ระบุ Development Result";
                //            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
                //        }
                //    }
                //}

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
                    app.UpdateEnrollmentStatus_3(Id, IDPGroupId); //DEVELOPING
                }
                else if (isDeveloped)
                {
                    app.UpdateEnrollmentStatus_4(Id, IDPGroupId); //2ND EVALUATING
                }
                else if (is2ndEvaluated)
                {
                    app.UpdateEnrollmentStatus_5(Id, IDPGroupId); //SUCCESS
                    app.UpdateWorkflowCompelete(Guid);
                }
                else
                {
                    app.UpdateEnrollmentStatus_2(Id, IDPGroupId); //1ST EVALUATING
                }

                app.UpdateApprover(username, Guid);
            }
            

            int all = app.GetCompetencyAllByGuid(Guid);
            int pass;
            if (status != "2nd Evaluating" && status != "Success" && status != "Decline")
            {
                 pass = app.GetCompetencyPassByGap1(Guid);
            }
            else
            {
                 pass = app.GetCompetencyPassByGap2(Guid);
            }

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

            if (status != "2nd Evaluating" && status != "Success" && status != "Decline")
            {
                app.UpdateResultA1(Guid, pass, per, rank);
            }
            else
            {
                app.UpdateResultA2(Guid, pass, per, rank);
            }

            return RedirectToAction("Form", "Form", new { idpGroupId = IDPGroupId, guid = Guid });
        }

        #endregion

        #region GOODNESS

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
        /*[HttpPost]
        public ActionResult InsertGoodness()
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
        }*/
        [HttpPost]
        public ActionResult InsertGoodness()
        {
            //ConnectToData cd = new ConnectToData();
            System.Web.HttpCookie usernameCookie = Request.Cookies["username"];
            string username = usernameCookie.Value;

            string fileUploadPath = ConfigurationManager.AppSettings["FileUploadPath"].ToString();

            int count = app.GetCountGoodness(username) + 1;
            
            int index = 0;
            while (true)
            {
                var typeKey = $"Type_{index}";
                if (Request.Form.AllKeys.Contains(typeKey))
                {
                    var type = Request.Form[$"Type_{index}"];
                    var company = Request.Form[$"Company_{index}"];
                    var desc = Request.Form[$"Desc_{index}"];
                    var date = Request.Form[$"Date_{index}"];
                    var hour = Request.Form[$"Hour_{index}"];
                    var year = Request.Form[$"Year_{index}"];

                    var file = Request.Files[$"File_{index}"];
                    var fileId = "";
                    if (file != null && file.ContentLength > 0)
                    {
                        HttpPostedFileBase f = file;
                        string fname;
                        if (f.FileName.Contains("\\"))
                        {
                            string[] testfiles = f.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = f.FileName;
                        }
                        string Type = string.Empty;
                        var splitName = fname.Split('.');
                        Type = splitName[splitName.Length - 1];

                        var user = username.Replace(".", "-");
                        var filenameGuid = "Goodness_" + year + "_" + user + "_" + count;
                        fileId = filenameGuid;

                        //string filePath = Path.Combine(fileUploadPath, filenameGuid);
                        //f.SaveAs(filePath);
                    }

                    if ((type != null && type != "") && (company != null && company != "") && (date != null && date != "") && (hour != null && hour != ""))
                    {
                        List<Goodness> goodnessList = new List<Goodness>();
                        Goodness goodness = new Goodness
                        {
                            Type = type,
                            Company = company,
                            Date = date,
                            Hour = hour,
                            Desc = desc,
                            FileID = fileId,
                        };
                        goodnessList.Add(goodness);
                        app.InsertGoodness(goodnessList, username, year);
                    }

                    index++;
                }
                else
                {
                    break;
                }
            }
            return Json(new { success = true });
        }
        #endregion

        #region INFO
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
                List<RemarkHS> remarkHs = app.GetRemark(guid);
                Result result = app.GetResult(guid);

                ViewBag.All = result.CompetencyAll;
                ViewBag.Pass1 = result.CompetencyPass1;
                ViewBag.Pass2 = result.CompetencyPass2;
                ViewBag.Per1 = result.CompetencyPer1;
                ViewBag.Per2 = result.CompetencyPer2;
                ViewBag.Rank1 = result.Rank1;
                ViewBag.Rank2 = result.Rank2;

                ViewBag.Remark = remarkHs;

                return View(results);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        #endregion
    }
}