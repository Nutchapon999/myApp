using somboonCL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using myApp.DAL;
using myApp.Models;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using Antlr.Runtime.Misc;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using OfficeOpenXml;
using System.Web.Helpers;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Web.Services.Description;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Cryptography;

namespace myApp.Controllers
{
    public class HomeController : Controller
    {
        private App app;

        public HomeController()
        {
            app = new App();
        }

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);

        OleDbConnection Econ;


        #region COMPETENCY
        public ActionResult Competency()
        {
            
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                List<Competency> competencies = app.GetCompetencies();

                return View(competencies);
            }
            else
            {

                return RedirectToAction("Error", "Home");
            }
        }
        public ActionResult CreateCompetency()
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                ViewBag.Username = username;

                return View();
            }
            else
            {
      
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult CreateCompetency(Competency competency)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                try
                {
                    app.CreateCompetency(competency);

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("@Id"))
                    {
                        ViewBag.ErrorMessage = "  กรุณากรอกรหัส Competency";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = ex.Message;
                    }

                    return View(competency);
                }
            }
            return View(competency);
        }
        public ActionResult EditCompetency(string id)
        {
            string type = app.GetTypeById(id);
            string competencyName = app.GetCompetencyNameById(id);  
            ViewBag.CompetencyId = id;
            ViewBag.CompetencyName = competencyName;
            ViewBag.CompetencyType = type;
            ViewBag.Username = Request.Cookies["username"].Value;
            Competency competency = app.EditCompetency(id, ViewBag.Username);
            return View(competency);
        }
        [HttpPost]
        public ActionResult EditCompetency(Competency competency)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                app.UpdateCompetency(competency, ViewBag.Username);
                return RedirectToAction("Competency", "Home");
            }
            return RedirectToAction("Competency", "Home");
        }
        public ActionResult DeleteCompetency(string CompetencyId)
        {
            app.DeleteCompetency(CompetencyId);
            return null;
        }
        #endregion

        #region IDP GROUP
        public ActionResult IDPGroup()
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                List<IDPGroup> iDPGroups = app.GetIDPGroups();

                foreach (var idpGroup in iDPGroups)
                {
                    idpGroup.EmployeeEnrollmentCount = app.GetCountEmployee(idpGroup.IDPGroupId);
                    idpGroup.EmployeeCompetencyCount = app.GetCountCompetency(idpGroup.IDPGroupId);
                }


                return View(iDPGroups);

            }
            else
            {

                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult CreateIDPGroup(IDPGroup iDPGroup)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                try
                {
                    app.CreateIDPGroup(iDPGroup, ViewBag.Username);
                    return RedirectToAction("IDPGroup", "Home");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("@Id"))
                    {
                        ModelState.AddModelError("IDPGroupId", "กรุณากรอกรหัส IDP Group");
                    }
                    else
                    {
                        ModelState.AddModelError("IDPGroupId", "เกิดข้อผิดพลาด โปรดกรอกใหม่อีกที");
                    }
                    TempData["ErrorMessage"] = ex.Message;
                }
            }


            // Get the list of IDPGroups and return the "IDPGroup" view within the modal
            List<IDPGroup> iDPGroups = app.GetIDPGroups();
            foreach (var idpGroup in iDPGroups)
            {
                idpGroup.EmployeeEnrollmentCount = app.GetCountEmployee(idpGroup.IDPGroupId);
                idpGroup.EmployeeCompetencyCount = app.GetCountCompetency(idpGroup.IDPGroupId);
            }

            return RedirectToAction("IDPGroup", "Home");
        }
        [HttpPost]
        public ActionResult EditIDPGroup(IDPGroup iDPGroup)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            try
            {
                app.UpdateIDPGroup(iDPGroup, ViewBag.Username);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("IDPGroup");
        }
        [HttpPost]
        public ActionResult CopyIDPGroup(IDPGroup iDPGroup, bool Emp = false, bool Cmpt = false)
        {
            string copyIDP = Request.Form["IDPGroupIdCopy"];

            ViewBag.Username = Request.Cookies["username"].Value;

            if (ModelState.IsValid)
            {
                try
                {
                    string year = app.GetYearById(copyIDP);
                    app.CreateIDPGroup(iDPGroup, ViewBag.Username);
                    if(Cmpt == true)
                    {
                        List<IDPGroupItem> copyIDPGroupItems = app.GetIDPGroupItems(copyIDP);
                        app.InsertIDPGroupItemCopy(copyIDPGroupItems, iDPGroup);
                    }
                    if(Emp == true)
                    {
                        List<Enrollment> copyEnrolls = app.GetEnrollments(copyIDP);
                        app.InsertEnrollCopy(copyEnrolls, iDPGroup);
                        List<User> userCopies = app.GetUsersById(copyEnrolls);  
                        app.InsertResultEmployees(userCopies, iDPGroup.Year, ViewBag.Username, iDPGroup.IDPGroupId);
                    }
                    
                    return RedirectToAction("IDPGroup", "Home");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("@Id"))
                    {
                        ModelState.AddModelError("IDPGroupId", "กรุณากรอกรหัส IDP Group");
                    }
                    else
                    {
                        ModelState.AddModelError("IDPGroupId", "เกิดข้อผิดพลาด โปรดกรอกใหม่อีกที");
                    }
                    TempData["ErrorMessage"] = ex.Message;
                }
            }

            List<IDPGroup> iDPGroups = app.GetIDPGroups();
            foreach (var idpGroup in iDPGroups)
            {
                idpGroup.EmployeeEnrollmentCount = app.GetCountEmployee(idpGroup.IDPGroupId);
                idpGroup.EmployeeCompetencyCount = app.GetCountCompetency(idpGroup.IDPGroupId);
            }

            return RedirectToAction("IDPGroup", "Home");
        }
        [HttpPost]
        public ActionResult DeleteIDPGroup(string idpGroupId)
        {
            try
            {
                app.DeleteIDPGroup(idpGroupId);
            }
            catch(Exception ex) 
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return Json(new { success = true, message = "IDP group deleted successfully." });
        }
        public ActionResult DetailIDPGroup(string idpGroupId)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = usernameCookie.Value;

                string idpGroupName = app.GetIDPGroupNameByIDPGroupId(idpGroupId);
                string year = app.GetYearById(idpGroupId);
                int members = app.GetCountEmployee(idpGroupId);
                int competencies = app.GetCountCompetency(idpGroupId);

                List<IDPGroupItem> iDPGroupItems = app.GetIDPGroupItems(idpGroupId);
                List<Enrollment> enrollments = app.GetEnrollments(idpGroupId);

                //List<IDPGroup> iDPGroups = app.GetDetails(idpGroupId);

                ViewBag.IDPGroupId = idpGroupId;
                ViewBag.IDPGroupName = idpGroupName;
                ViewBag.Year = year;
                ViewBag.Member = members;
                ViewBag.Competency = competencies;

                ViewBag.IDPGroupItem = iDPGroupItems;
                ViewBag.Enrollment = enrollments;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        #endregion

        #region USER
        public ActionResult Employee()
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                List<User> users = app.GetUsers();
                return View(users);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        public ActionResult DeleteEmployee(string id)
        {
            app.DeleteEmployee(id);
            return RedirectToAction("Employee");
        }
        public ActionResult AddIDPGroup(string id)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;

                List<Enrollment> enrollments = app.GetIDPGroupByEmployee(id);

                ViewBag.Id = id;
                User user = app.GetUserById(id);
                if(user != null)
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

                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult AddIDPGroup(List<string> idpGroupIds, string id, bool isChecked)
        {
            if (idpGroupIds != null && idpGroupIds.Any())
            {
                foreach (var idpGroupId in idpGroupIds) 
                {
                    if (!isChecked)
                    {
                        try
                        {
                            app.UpdateEnrollmentStatus_1(id, idpGroupId);

                            int count = app.GetCountCompetencyThisId(idpGroupId);
                            string guid = app.GetGuidById_IDPGroupId(id, idpGroupId);

                            List<IDPGroupItem> iDPGroupItems = app.GetIDPGroupItems(idpGroupId);

                            string year = app.GetYearByGuid(guid);

                            List<ResultItem> actual2 = app.GetPreActual2(id, year);

                            app.InsertResultDetails(iDPGroupItems, guid, count, actual2);
                        }
                        catch 
                        {
                            TempData["ErrorMessage"] = "ทำไม่ได้";
                        }
                    }
                    else
                    {
                        app.UpdateEnrollmentStatus_6(id, idpGroupId);
                    }
                }
            }
            return RedirectToAction("AddIDPGroup", new { id = id });
        }
        public ActionResult SelectIDPGroup(string id)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                List<IDPGroup> iDPGroups = app.GetIDPGroups();

                List<string> enrolledIDPGroupId = app.GetCheckedIDPGroup(id);

                List<IDPGroup> availableIDPGroupId = iDPGroups.Where(g => !enrolledIDPGroupId.Contains(g.IDPGroupId)).ToList();

                availableIDPGroupId.ForEach(g => g.Enrollment = new Enrollment());

                ViewBag.Id = id;

                User user = app.GetUserById(id);
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

                foreach (var idpGroup in iDPGroups)
                {
                    idpGroup.EmployeeEnrollmentCount = app.GetCountEmployee(idpGroup.IDPGroupId);
                    idpGroup.EmployeeCompetencyCount = app.GetCountCompetency(idpGroup.IDPGroupId);
                }

                return View(availableIDPGroupId);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult SelectedIDPGroup(List<string> iDPGroupIds, string id)
        {
            ViewBag.Username = Request.Cookies["username"].Value;

            if (iDPGroupIds == null)
            {
                return RedirectToAction("AddIDPGroup", new { id = id });
            }

            List<IDPGroup> selectedIDPGroups = new List<IDPGroup>();


            List<string> enrolledIDPGroups = app.GetCheckedIDPGroup(id);

            foreach (string iDPGroupId in iDPGroupIds)
            {
                if (enrolledIDPGroups.Contains(iDPGroupId))
                {

                    return RedirectToAction("SelectStudent", new { id = id });
                }

                IDPGroup iDPGroup = app.GetIDPGroups().FirstOrDefault(g => g.IDPGroupId == iDPGroupId);
                if (iDPGroup != null)
                {
                    selectedIDPGroups.Add(iDPGroup);
                }
            }

            app.InsertIDPGroup(selectedIDPGroups, id);

            app.InsertResultEmployees2(selectedIDPGroups, ViewBag.Username, id);

            return RedirectToAction("AddIDPGroup", new { id = id });
        }
        public ActionResult DeleteIDPGroupByEmployee(int enrollId)
        {
            string id = app.GetIdByEnrollment(enrollId);
            string idpGroupId = app.GetIDPGroupIdByEnrollment(enrollId);
            try
            {
                app.DeleteIDPGroupByEmployee(enrollId);
                app.DeleteResult(id, idpGroupId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return null;

        }
        #endregion

        #region IDP GROUP ITEM
        public ActionResult AddCompetency(string idpGroupId)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
                bool canAdd = app.CheckIfIDPGroupIsDraft(idpGroupId);

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                ViewBag.CanAdd = canAdd;
                ViewBag.Massage = "IDP Group นี้ใช้งานแล้วและไม่สามารถเพิ่มได้";

                int count = app.GetCountCompetency(idpGroupId);

                string idpGroupName = app.GetIDPGroupNameByIDPGroupId(idpGroupId);
                string year = app.GetYearById(idpGroupId);
                List<IDPGroupItem> competencyItems = app.GetIDPGroupItems(idpGroupId);
                ViewBag.IDPGroupId = idpGroupId;
                ViewBag.IDPGroupName = idpGroupName;
                ViewBag.Year = year;
                ViewBag.Count = count;
                return View(competencyItems);
                
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult AddCompetency(string idpGroupId, Dictionary<string, IDPGroupItem> idpGroupItems)
        {
            int count = app.GetCountResult(idpGroupId);
            app.UpdateIDPGroupItems(idpGroupItems, idpGroupId);
            if( count > 0)
            {
                app.UpdateResultItem(idpGroupItems);
            }
            return RedirectToAction("AddCompetency", new { idpGroupId = idpGroupId });
            
        }
        public ActionResult SelectCompetency(string idpGroupId)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;

                List<Competency> competencies = app.GetCompetencyAtActive();

                List<string> enrolledSubjectCodes = app.GetCheckedCompetencyId(idpGroupId);

                List<Competency> availableSubjects = competencies.Where(c => !enrolledSubjectCodes.Contains(c.CompetencyId)).ToList();

                availableSubjects.ForEach(c => c.IDPGroupItem = new IDPGroupItem());

                string idpGroupName = app.GetIDPGroupNameByIDPGroupId(idpGroupId);
                string year = app.GetYearById(idpGroupId);

                ViewBag.IDPGroupId = idpGroupId;
                ViewBag.IDPGroupName = idpGroupName;
                ViewBag.Year = year;
                return View(availableSubjects);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult SelectedCompetency(List<string> competencyIds, string idpGroupId, Dictionary<string, string> plValues, Dictionary<string, string> priorityValues)
        {
            if (competencyIds == null)
            {
                return RedirectToAction("AddCompetency", new { id = idpGroupId });
            }

            List<Competency> selectedCompetencies = new List<Competency>();

            string year = app.GetYearById(idpGroupId);


            List<string> enrolledSubjectCodes = app.GetCheckedCompetencyId(idpGroupId);

            List<string> allIdsInEnroll = app.GetIdsThatEnrollByIDPGroupId(idpGroupId);

            foreach (string competencyId in competencyIds)
            {
                Competency competency = app.GetCompetencyAtActive().FirstOrDefault(c => c.CompetencyId == competencyId);
                if (competency != null)
                {
                    string selectedPl = plValues.ContainsKey(competencyId) ? plValues[competencyId] : null;
                    string selectedPriority = priorityValues.ContainsKey(competencyId) ? priorityValues[competencyId] : null;

                    competency.IDPGroupItem = new IDPGroupItem(); // Initialize CompetencyItem if null
                    competency.IDPGroupItem.Pl = selectedPl;
                    competency.IDPGroupItem.Critical = false;

                    selectedCompetencies.Add(competency);
                }
            }

            app.InsertCompetency(selectedCompetencies, idpGroupId);

            bool hasExistingResults = app.IsAlreadyResultEachYearByIds(allIdsInEnroll, year);
            if (hasExistingResults)
            {
                app.UpdateResultEmployeesById(allIdsInEnroll, idpGroupId);
            }


            return RedirectToAction("AddCompetency", new { idpGroupId = idpGroupId });
        }
        public ActionResult DeleteIDPGroupItem(int idpGroupItem)
        {
            
            string idpGroupId = app.GetIDPGroupIdByIDPGroupItem(idpGroupItem);
            string year = app.GetYearById(idpGroupId);
            List<string> allIdsInEnroll = app.GetIdsThatEnrollByIDPGroupId(idpGroupId);

            try
            {
                app.DeleteIDPGroupItem(idpGroupItem, idpGroupId);

                int thisGroup = app.GetCountCompetencyThisId(idpGroupId);
    
                app.UpdateResultEmployeeAfterDeleteFromAddCompetency(thisGroup, allIdsInEnroll, idpGroupId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return null;
        }
        #endregion

        #region USER ENROLL
        public ActionResult AddEmployee(string idpGroupId)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                List<Enrollment> enrollments = app.GetEnrollments(idpGroupId);

                string IDPGroupName = app.GetIDPGroupNameByIDPGroupId(idpGroupId);
                string year = app.GetYearById(idpGroupId);

                ViewBag.IDPGroupID = idpGroupId;
                ViewBag.IDPGroupName = IDPGroupName;
                ViewBag.Year = year;


                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult AddEmployee(List<string> Ids, string idpGroupId, bool isChecked)
        {
            System.Web.HttpCookie usernameCookie = Request.Cookies["username"];
            string username = usernameCookie.Value;

            string position = app.GetPositionByCookie(username);

            if (Ids != null && Ids.Any())
            {
                foreach (var id in Ids)
                {
                    string status = app.GetStatus(id, idpGroupId);

                    if (!isChecked)
                    {
                        try
                        {
                            if(status == "Draft")
                            {
                                app.UpdateEnrollmentStatus_1(id, idpGroupId);
                                int count = app.GetCountCompetencyThisId(idpGroupId);
                                string guid = app.GetGuidById_IDPGroupId(id, idpGroupId);

                                List<IDPGroupItem> iDPGroupItems = app.GetIDPGroupItems(idpGroupId);

                                string year = app.GetYearByGuid(guid);

                                List<ResultItem> actual2 = app.GetPreActual2(id, year);

                                app.UpdateStartWorkFlow(guid, username);
                                app.InsertWorkflowHS0(position, username);
                                app.InsertResultDetails(iDPGroupItems, guid, count, actual2);
                            }
                        }
                        catch
                        {
                            TempData["ErrorMessage"] = "ทำไม่ได้";
                        }

                        //return RedirectToAction("AddEmployee", new { idpGroupId = idpGroupId });
                    }
                    else
                    {

                        try
                        {
                            app.UpdateEnrollmentStatus_6(id, idpGroupId);
                            app.InsertWorkflowHS3(position, username);
                        }
                        catch (Exception ex)
                        {
                            TempData["ErrorMessage"] = ex.Message;
                        }

                    }                                                                                   
                }
            }
            return RedirectToAction("AddEmployee", new { idpGroupId = idpGroupId });
        }
        public ActionResult SelectEmployee(string idpGroupId)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                List<User> users = app.GetEmployeeAtActive();

                List<string> enrolledIds = app.GetCheckedId(idpGroupId);

                List<User> availableIds = users.Where(u => !enrolledIds.Contains(u.Id)).ToList();

                availableIds.ForEach(u => u.Enrollment = new Enrollment());

                string IDPGroupName = app.GetIDPGroupNameByIDPGroupId(idpGroupId);
                string year = app.GetYearById(idpGroupId);

                ViewBag.Username = Request.Cookies["username"].Value;
                ViewBag.IDPGroupID = idpGroupId;
                ViewBag.IDPGroupName = IDPGroupName;
                ViewBag.Year = year;

                return View(availableIds);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult SelectedEmployee(List<string> userIds, string idpGroupId)
        {
            ViewBag.Username = Request.Cookies["username"].Value;

            if (userIds == null)
            {
                return RedirectToAction("AddEmployee", new { idpGroupId = idpGroupId });
            }

            List<User> selectedUsers = new List<User>();
            string year = app.GetYearById(idpGroupId);

            List<string> enrolledUsers = app.GetCheckedId(idpGroupId);

            foreach (string userId in userIds)
            {
                if (enrolledUsers.Contains(userId))
                {
                    return RedirectToAction("SelectStudent", new { idpGroupId = idpGroupId });
                }

                User user = app.GetEmployeeAtActive().FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    selectedUsers.Add(user);
                }
            }

            app.InsertEmployee(selectedUsers, idpGroupId);

            app.InsertResultEmployees(selectedUsers, year, ViewBag.Username, idpGroupId);

            return RedirectToAction("AddEmployee", new { idpGroupId = idpGroupId });
        }
        [HttpPost]
        public ActionResult DeleteEmployeeByIDPGroup(int enrollId) 
        {
            string idpGroupId = app.GetIDPGroupIdByEnrollment(enrollId);
            string id = app.GetIdByEnrollment(enrollId);
            //bool canDelete = app.CheckIfEnrollIsDecline(enrollId);

            try
            {
                app.DeleteEmployeeByIDPGroup(enrollId);
                
                app.DeleteResult(id, idpGroupId);
                
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }


            return null;
        }
        #endregion

        #region UPLOAD COMPETENCY
        public ActionResult UploadCompetency()
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = Request.Cookies["username"].Value;
                int rowCount = TempData.ContainsKey("RowCount") ? int.Parse(TempData["RowCount"].ToString()) : 0;
                TempData["RowCount"] = rowCount.ToString();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult UploadCompetency(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                //int rowCount = GetExcelRowCount(file) - 1;
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/Excel/" + filename;

                file.SaveAs(Server.MapPath(filepath));
                InsertExceldata1(filepath, filename);

                TempData["UploadSuccess"] = true;
                //TempData["RowCount"] = rowCount.ToString();
            }


            return RedirectToAction("UploadCompetency");
        }
        private void ExcelConn(string filePath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", filePath);
            Econ = new OleDbConnection(constr);
        }
        private void InsertExceldata1(string FilePath, string FileName)
        {
            string fullpath = Server.MapPath("/Excel/") + FileName;
            ExcelConn(fullpath);
            String query = string.Format("select * from [{0}]", "Sheet1$");

            try
            {
                OleDbCommand Ecom = new OleDbCommand(query, Econ);
                Econ.Open();

                DataSet ds = new DataSet();
                OleDbDataAdapter oda = new OleDbDataAdapter(query, Econ);
                Econ.Close();
                oda.Fill(ds);

                DataTable dt = ds.Tables[0];


                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
                {
                    con.Open();
                    SqlCommand checkExistCommand = new SqlCommand("SELECT COMPETENCY_ID FROM IDP_COMPTY WHERE COMPETENCY_ID = @CompetencyId", con);
                    SqlCommand insertCommand = new SqlCommand("INSERT INTO IDP_COMPTY (COMPETENCY_ID, COMPETENCY_NAME_TH, COMPETENCY_NAME_EN, COMPETENCY_DESC, PL1, PL2, PL3, PL4, PL5, ACTIVE, TYPE, DELETED) " +
                                                             "VALUES (@CompetencyId, @CompetencyNameTH, @CompetencyNameEN, @CompetencyDesc, @Pl1, @Pl2, @Pl3, @Pl4, @Pl5, @Active, @Type, @Delete)", con);

                    foreach (DataRow row in dt.Rows)
                    {
                        string competencyId = row["COMPETENCY_ID"].ToString();
                        checkExistCommand.Parameters.Clear();
                        checkExistCommand.Parameters.AddWithValue("@CompetencyId", competencyId);

                        object existingCode = checkExistCommand.ExecuteScalar();
                        if (existingCode == null)
                        {
                            insertCommand.Parameters.Clear();
                            insertCommand.Parameters.AddWithValue("@CompetencyId", competencyId);
                            insertCommand.Parameters.AddWithValue("@CompetencyNameTH", row["COMPETENCY_NAME_TH"]);
                            insertCommand.Parameters.AddWithValue("@CompetencyNameEN", row["COMPETENCY_NAME_EN"]);
                            insertCommand.Parameters.AddWithValue("@CompetencyDesc", row["COMPETENCY_DESC"]);
                            insertCommand.Parameters.AddWithValue("@Pl1", row["PL1"]);
                            insertCommand.Parameters.AddWithValue("@Pl2", row["PL2"]);
                            insertCommand.Parameters.AddWithValue("@Pl3", row["PL3"]);
                            insertCommand.Parameters.AddWithValue("@Pl4", row["PL4"]);
                            insertCommand.Parameters.AddWithValue("@Pl5", row["PL5"]);
                            insertCommand.Parameters.AddWithValue("@Active", row["ACTIVE"]);
                            insertCommand.Parameters.AddWithValue("@Type", row["TYPE"]);
                            insertCommand.Parameters.AddWithValue("@Delete", row["DELETED"]);

                            insertCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand updateCommand = new SqlCommand("UPDATE IDP_COMPTY SET COMPETENCY_NAME_TH = @CompetencyNameTH, COMPETENCY_NAME_EN = @CompetencyNameEN, COMPETENCY_DESC = @CompetencyDesc, " +
                                "PL1 = @Pl1, PL2 = @Pl2, PL3 = @Pl3, PL4 = @Pl4, PL5 = @Pl5, Active = @Active, TYPE = @Type, DELETED = @Delete WHERE COMPETENCY_ID = @CompetencyId", con);

                            updateCommand.Parameters.AddWithValue("@CompetencyId", competencyId);
                            updateCommand.Parameters.AddWithValue("@CompetencyNameTH", row["COMPETENCY_NAME_TH"]);
                            updateCommand.Parameters.AddWithValue("@CompetencyNameEN", row["COMPETENCY_NAME_EN"]);
                            updateCommand.Parameters.AddWithValue("@CompetencyDesc", row["COMPETENCY_DESC"]);
                            updateCommand.Parameters.AddWithValue("@Pl1", row["PL1"]);
                            updateCommand.Parameters.AddWithValue("@Pl2", row["PL2"]);
                            updateCommand.Parameters.AddWithValue("@Pl3", row["PL3"]);
                            updateCommand.Parameters.AddWithValue("@Pl4", row["PL4"]);
                            updateCommand.Parameters.AddWithValue("@Pl5", row["PL5"]);
                            updateCommand.Parameters.AddWithValue("@Active", row["ACTIVE"]);
                            updateCommand.Parameters.AddWithValue("@Type", row["TYPE"]);
                            updateCommand.Parameters.AddWithValue("@Delete", row["DELETED"]);

                            updateCommand.ExecuteNonQuery();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = "เกิดข้อผิดพลาดในการอัปโหลด: " + ex.Message;
            }
        }
        #endregion

        #region UPLOAD IDP GROUP
        public ActionResult UploadIDPGroup()
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult UploadIDPGroup(HttpPostedFileBase file)
        {

            ViewBag.Username = Request.Cookies["username"].Value;
            if (file != null && file.ContentLength > 0)
            {
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/Excel/" + filename;
                file.SaveAs(Server.MapPath(filepath));
                InsertExceldata3(filepath, filename , ViewBag.Username);
                TempData["UploadSuccess"] = true;

            }

            return RedirectToAction("UploadIDPGroup");
        }
        private void InsertExceldata3(string FilePath, string FileName, string username)
        {
            string fullpath = Server.MapPath("/Excel/") + FileName;
            ExcelConn(fullpath);
            String query = string.Format("select * from [{0}]", "Sheet1$");

            try
            {
                OleDbCommand Ecom = new OleDbCommand(query, Econ);
                Econ.Open();

                DataSet ds = new DataSet();
                OleDbDataAdapter oda = new OleDbDataAdapter(query, Econ);
                Econ.Close();
                oda.Fill(ds);

                DataTable dt = ds.Tables[0];

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
                {
                    con.Open();
                    SqlCommand checkExistCommand = new SqlCommand("SELECT IDP_GROUP_ID FROM IDP_GROUP WHERE IDP_GROUP_ID = @IDPGroupId", con);
                    SqlCommand insertCommand = new SqlCommand("INSERT INTO IDP_GROUP (IDP_GROUP_ID, IDP_GROUP_NAME, YEAR, CREATED_BY, CREATED_ON) " +
                                                             "VALUES (@IDPGroupId, @IDPGroupName, @Year, @Username, GETDATE())", con);

                    foreach (DataRow row in dt.Rows)
                    {
                        string IDPGroupId = row["IDP_GROUP_ID"].ToString();
                        checkExistCommand.Parameters.Clear();
                        checkExistCommand.Parameters.AddWithValue("@IDPGroupId", IDPGroupId);

                        object existingCode = checkExistCommand.ExecuteScalar();
                        if (existingCode == null && !string.IsNullOrEmpty(IDPGroupId))
                        {
                            insertCommand.Parameters.Clear();
                            insertCommand.Parameters.AddWithValue("@IDPGroupId", IDPGroupId);
                            insertCommand.Parameters.AddWithValue("@IDPGroupName", row["IDP_GROUP_NAME"]);
                            insertCommand.Parameters.AddWithValue("@Year", row["YEAR"]);
                            insertCommand.Parameters.AddWithValue("@Username", username);
                       

                            insertCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand updateCommand = new SqlCommand("UPDATE IDP_GROUP SET IDP_GROUP_ID = @IDPGroupId, IDP_GROUP_NAME = @IDPGroupName, " +
                                                                   "YEAR = @Year " +
                                                                   "WHERE IDP_GROUP_ID = @IDPGroupId", con);

                            updateCommand.Parameters.AddWithValue("@IDPGroupId", IDPGroupId);
                            updateCommand.Parameters.AddWithValue("@IDPGroupName", row["IDP_GROUP_NAME"]);
                            updateCommand.Parameters.AddWithValue("@Year", row["YEAR"]);



                            updateCommand.ExecuteNonQuery();
                            //SqlCommand selectOldDataCommand = new SqlCommand("SELECT IDP_GROUP_NAME, YEAR FROM IDP_GROUP WHERE IDP_GROUP_ID = @IDPGroupId", con);
                            //selectOldDataCommand.Parameters.AddWithValue("@IDPGroupId", IDPGroupId);

                            //using (SqlDataReader reader = selectOldDataCommand.ExecuteReader())
                            //{
                            //    if (reader.Read())
                            //    {
                            //        string oldGroupName = reader["IDP_GROUP_NAME"].ToString();
                            //        int oldYear = Convert.ToInt32(reader["YEAR"]);

                            //        string newGroupName = row["IDP_GROUP_NAME"].ToString();
                            //        int newYear = Convert.ToInt32(row["YEAR"]);

                            //        reader.Close();


                            //        if (oldGroupName != newGroupName || oldYear != newYear)
                            //        {
                            //            // Update data
                            //            SqlCommand updateCommand = new SqlCommand("UPDATE IDP_GROUP SET IDP_GROUP_NAME = @IDPGroupName, YEAR = @Year WHERE IDP_GROUP_ID = @IDPGroupId", con);

                            //            updateCommand.Parameters.AddWithValue("@IDPGroupId", IDPGroupId);
                            //            updateCommand.Parameters.AddWithValue("@IDPGroupName", oldGroupName);
                            //            updateCommand.Parameters.AddWithValue("@Year", oldYear);

                            //            updateCommand.ExecuteNonQuery();
                            //        }
                            //        else
                            //        {
                            //            SqlCommand updateCommand = new SqlCommand("UPDATE IDP_GROUP SET IDP_GROUP_NAME = @IDPGroupName, YEAR = @Year WHERE IDP_GROUP_ID = @IDPGroupId", con);

                            //            updateCommand.Parameters.AddWithValue("@IDPGroupId", IDPGroupId);
                            //            updateCommand.Parameters.AddWithValue("@IDPGroupName", newGroupName);
                            //            updateCommand.Parameters.AddWithValue("@Year", newYear);

                            //            updateCommand.ExecuteNonQuery();
                            //        }
                            //    }
                            //}
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = "เกิดข้อผิดพลาดในการอัปโหลด: " + ex.Message;
            }
        }
        #endregion

        #region EMAIL
        public ActionResult SendEmail()
        {
            List<IDPGroup> competencyForms = app.SelectIDPGroup();
            return View(competencyForms);
        }
        [HttpPost]
        public ActionResult SendEmail(string IDPGroupId, string SelectedUser)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("PondPoP Za", "pondpopza19@gmail.com"));
                message.To.Add(MailboxAddress.Parse("sr.nutchapon_st@tni.ac.th"));
                message.Subject = "แบบประเมิน IDP";
                message.Body = new TextPart("plain")
                {
                    Text = @"กรุณาช่วยระบุลำดับความสำคัญของ IDP Group ด้วย " +
                           "รหัส IDP Group: " +
                           " และได้แนบลิ้งค์ไว้ดังนี้ ขอขอบพระคุณอย่างยิ่ง :D"
                };

                string senderEmail = "pondpopza19@gmail.com";
                string senderPassword = "xnuqpadqupsyahgv";

                using (SmtpClient smtpClient = new SmtpClient())
                {
                    try
                    {
                        smtpClient.Connect("smtp.gmail.com", 465, true);
                        smtpClient.Authenticate(senderEmail, senderPassword);

                        smtpClient.Send(message);

                        TempData["SendSuccess"] = true;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                    }
                    finally
                    {
                        smtpClient.Disconnect(true);
                    }
                }
            }
            else
            {
                TempData["ConnectionError"] = "ไม่สามารถเชื่อมต่อกับ Wi-Fi ได้";
            }

            return RedirectToAction("SendEmail");
        }
        #endregion

        #region FORM
        public ActionResult SelectForm(string user, string year)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                ViewBag.User = user;

                string id = app.GetIdByCookie(user);

                ViewBag.Id = id;
                ViewBag.Year = year;

                List<Enrollment> enrollments = app.GetEnrollEachYearByUsername(user, year);
                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        public ActionResult Form(string user, string idpGroupId, string guid)
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
                string yearIDP = app.GetYearByEnrolled(enrollmentId);
                string status = app.GetStatus(id, idpGroupId);
                string approver = app.GetApprover(guid);

                ViewBag.EnrollmentId = enrollmentId;
                ViewBag.Id = id;
                ViewBag.Approver = approver;
                ViewBag.Year = yearIDP;

                User userLogin = app.GetUserByGuid(guid);
                if (userLogin != null)
                {
                    ViewBag.Prefix = userLogin.Prefix;
                    ViewBag.FirstName = userLogin.FirstNameTH;
                    ViewBag.LastName = userLogin.LastNameTH;
                    ViewBag.Company = userLogin.Company;
                    ViewBag.Joblevel = userLogin.JobLevel;
                    ViewBag.Department = userLogin.Department;
                    ViewBag.Position = userLogin.Position;
                    ViewBag.UserLogin = userLogin.UserLogin;
                }
                ViewBag.Year = app.GetYearById(idpGroupId);
                ViewBag.IDPGroupId = idpGroupId;
                ViewBag.IDPGroupName = IDPGroupName;
                ViewBag.Guid = guid;
                ViewBag.Status = status;

                List<RemarkHS> remarkHS = app.GetRemark(guid);
                List<Goodness> goodnesses = app.GetGoodnessByUser(ViewBag.UserLogin, ViewBag.Year);

                ViewBag.Remark = remarkHS;
                ViewBag.Goodness = goodnesses;

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
        //    string position = app.GetJoblevelByCookie(username);
        //    string status = app.GetStatus(Id, IDPGroup);
        //    int count = app.GetCountCompetencyThisId(IDPGroup);
        //    string user = app.GetUserLoginByEnrollId(enrollId);

        //    //INSERT AND UPDATE RESULTITEMS
        //    if (isFormSubmitted)
        //    {
        //        List<ResultItem> resultItemsBefore = app.GetResultItemByGuidBeforeUpdate(Guid);
        //        app.UpdateResultDetails(forms.Values, Guid);

        //        //REMARK
        //        if (status == "1st Evaluating" || status == "2nd Evaluating")
        //        {
        //            app.InsertRemark(remark, username, position, Guid);
        //        }
        //        List<ResultItem> resultItemsAfter = app.GetResultItemByGuidAfterUpdate(Guid);
        //        List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
        //        app.InsertLogOnUpdateResultItems(resultItemIds, username, resultItemsBefore, resultItemsAfter, status, Guid);
        //    }
        //    else
        //    {
        //        //app.InsertResultDetails(forms.Values, Guid, count);
        //        //LOG DATA
        //        //List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
        //        //List<ResultItem> resultItems = app.GetResultItemByGuidOnInsert(Guid);
        //        //app.InsertLogOnInsertResultItems(resultItemIds, username, resultItems, Guid);
        //    }

        //    //Calculate Values for Result
        //    int all = app.GetCompetencyAllByGuid(Guid);
        //    int pass = app.GetCompetencyPassByGuid(Guid);

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

        //    return RedirectToAction("Form", "Home", new { user = user, idpGroupId = IDPGroup, guid = Guid });

        //}

        [HttpPost]
        public ActionResult SaveForm(int enrollId, string Guid, string IDPGroupId)
        {
            string fileUploadPath = ConfigurationManager.AppSettings["FileUploadPath"].ToString();
            string username = Request.Cookies["username"].Value;
            string Id = app.GetIdByEnrollment(enrollId);
            var form = HttpContext.Request.Form;
            List<ResultItem> resultItems = new List<ResultItem>();
            string status = app.GetStatus(Id, IDPGroupId);
            string joblevel = app.GetJoblevelByCookie(username);
            string userLogin = app.GetUserLoginByEnrollId(enrollId);
            //string position = app.GetPositionByCookie(username);

            int count = app.GetCompetencyAllByGuid(Guid);

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
                var fileEditKey = "FileEdit_" + i;
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
                var fileEditValue = form[fileEditKey];
                var fileId = "";
                {
                    if (fileValue != null && fileValue.ContentLength > 0)
                    {
                        HttpPostedFileBase f = fileValue;
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

                        int j = i + 1;

                        var user = username.Replace(".", "-");
                        var filenameGuid = "IDP_" + Guid + "_" + j;
                        fileId = filenameGuid;

                        //string filePath = Path.Combine(fileUploadPath, fileId);
                        //f.SaveAs(filePath);
                    }
                    else if (fileEditValue != "")
                    {
                        fileId = fileEditValue;
                    }
                    else
                    {
                        fileId = null;
                    }
                }

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
                    FileId = fileId,
                    Actual2 = parsedActual2

                };

                resultItems.Add(resultItem);

            }

           
                //LOG DATA
                List<ResultItem> resultItemsBefore = app.GetResultItemByGuidBeforeUpdate(Guid);
                //UPDATE RESULTITEMS
                app.UpdateForm(resultItems, Guid);

                //LOG DATA
                List<ResultItem> resultItemsAfter = app.GetResultItemByGuidAfterUpdate(Guid);
                List<int> resultItemIds = app.GetResultItemIdByGuid(Guid);
                app.InsertLogAdmin(resultItemIds, username, resultItemsBefore, resultItemsAfter, Guid);
            

           

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
            return RedirectToAction("Form", "Home", new { user = userLogin, idpGroupId = IDPGroupId, guid = Guid });
        }
        #endregion

        #region INFO
        public ActionResult Info(string user, string idpGroupId, string guid)
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
                ViewBag.Year = year;
                string id = app.GetIdByCookie(user);
                User us = app.GetUserByCookie(user);
                if (user != null)
                {
                    ViewBag.Prefix = us.Prefix;
                    ViewBag.FirstName = us.FirstNameTH;
                    ViewBag.LastName = us.LastNameTH;
                    ViewBag.Company = us.Company;
                    ViewBag.Joblevel = us.JobLevel;
                    ViewBag.Department = us.Department;
                    ViewBag.Position = us.Position;
                }
                //ViewBag.Count = count;
                ViewBag.Id = id;
                ViewBag.IDPGroupID = idpGroupId;
                ViewBag.IDPGroupName = idpGroupName;
                ViewBag.Guid = guid;
                string status = app.GetStatus(id, idpGroupId);
                ViewBag.Status = status;

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

        #region GOODNESS
        public ActionResult Goodness(string year) 
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {

                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");
                bool isGood = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Goodness");

                List<User> users = app.GetEmployeeAtActive();

                ViewBag.isAdmin = isAdmin;
                ViewBag.isGood = isGood;
                ViewBag.Username = username;
                ViewBag.Year = year;
                ViewBag.User = users;
                List<Goodness> goodnessList = app.GetGoodness(year);
                
                return View(goodnessList);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        [HttpPost]
        public ActionResult InsertGoodness(string Year, List<string> userIds)
        {
            System.Web.HttpCookie usernameCookie = Request.Cookies["username"];
            string username = usernameCookie.Value;

            if (usernameCookie != null)
            {
                var form = HttpContext.Request.Form;

                var str = form["Type"];
                var typeVal = str.Substring(0 , str.Length - 1);
                var companyVal = form["Company"];
                var descVal = form["Desc"];
                var dateVal = form["Date"];
                var hourVal = form["Hour"];
                var fileVal = Request.Files["File"];
                var fileId = "";
                if (fileVal != null && fileVal.ContentLength > 0)
                {
                    HttpPostedFileBase f = fileVal;
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
                    var filenameGuid = "Goodness_" + Year;
                    fileId = filenameGuid;

                    //string filePath = Path.Combine(fileUploadPath, filenameGuid);
                    //f.SaveAs(filePath);
                }

                if (typeVal != null && companyVal != null && descVal != null && hourVal != null)
                {
                    Goodness goodness = new Goodness
                    {
                        Type = typeVal,
                        Company = companyVal,
                        Date = dateVal,
                        Desc = descVal,
                        Hour = hourVal,
                        FileID = fileId,
                    };

                    foreach (var id in userIds)
                    {
                        app.InsertGoodnessById(goodness, id, Year);
                    }
                }

                return RedirectToAction("Goodness", "Home", new { year = Year });
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }

        }
        [HttpPost]
        public ActionResult EditGoodness(string TypeEdit, string CompanyEdit, int GDId, string DescEdit, string DateEdit, string HourEdit)
         {
            Goodness goodness = new Goodness();
            goodness.GDId = GDId;
            goodness.Type = TypeEdit;
            goodness.Company = CompanyEdit;
            goodness.Desc = DescEdit;
            goodness.Date = DateEdit;
            goodness.Hour = HourEdit;

            app.UpdateGoodness(goodness);

            return null;
        }
        [HttpPost]
        public ActionResult DeleteGoodness(int GDId)
        {
            app.DeleteGoodness(GDId);
            return null;
        }
        #endregion

        #region DOWNLOAD
        public ActionResult Download()
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {

                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;

                List<User> downloads = app.GetListDownload();
                List<IDPGroup> iDPGroups = app.GetIDPGroups();

                ViewBag.iDPGroups = iDPGroups;
                return View(downloads);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
        #endregion
    }
}