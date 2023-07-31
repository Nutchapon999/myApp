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


        //Competency
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

                return RedirectToAction("Index", "Form");
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
      
                return RedirectToAction("Index", "Home");
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
        public ActionResult DeleteCompetency(string id)
        {
            app.DeleteCompetency(id);
            return RedirectToAction("Index");
        }


        //IDP GROUP
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
        /*public ActionResult CreateIDPGroup()
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

                return RedirectToAction("Index", "Home");
            }
        }*/
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

        public ActionResult EditIDPGroup(string id)
        {
            string idpGroupName = app.GetIDPGroupNameByIDPGroupId(id);
            string year = app.GetYearById(id);
            ViewBag.IDPGroupId = id;
            ViewBag.IDPGroupName = idpGroupName;
            ViewBag.Year = year;
            ViewBag.Username = Request.Cookies["username"].Value;
            IDPGroup iDPGroup = app.EditIDPGroup(id, ViewBag.Username);
            return View(iDPGroup);
        }
        [HttpPost]
        public ActionResult EditIDPGroup(IDPGroup iDPGroup)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                app.UpdateIDPGroup(iDPGroup, ViewBag.Username);
                return RedirectToAction("IDPGroup");
            }
            return View(iDPGroup);
        }
        public ActionResult DeleteIDPGroup(string id)
        {
            app.DeleteIDPGroup(id);
            return RedirectToAction("IDPGroup");
        }
        public ActionResult DetailIDPGroup(string id)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = usernameCookie.Value;

                string idpGroupName = app.GetIDPGroupNameByIDPGroupId(id);
                string year = app.GetYearById(id);
                int members = app.GetCountEmployee(id);
                int competencies = app.GetCountCompetency(id);
                
                List<IDPGroup> iDPGroups = app.GetDetails(id);

                ViewBag.IDPGroupId = id;
                ViewBag.IDPGroupName = idpGroupName;
                ViewBag.Year = year;
                ViewBag.Member = members;
                ViewBag.Competency = competencies;

                return View(iDPGroups);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }
       

        //HR User
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
        /*public ActionResult AddIDPGroup(string id)
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

                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult SelectIDPGroup(string id)
        {
            List<IDPGroup> iDPGroups = app.GetIDPGroups();

            List<string> enrolledIDPGroupId = app.GetCheckedIDPGroup(id);

            List<IDPGroup> availableIDPGroupId = iDPGroups.Where(g => !enrolledIDPGroupId.Contains(g.IDPGroupId)).ToList();

            availableIDPGroupId.ForEach(g => g.Enrollment = new Enrollment());

            ViewBag.Id = id;
            return View(availableIDPGroupId);
        }
        [HttpPost]
        public ActionResult SelectedIDPGroup(List<string> iDPGroupIds, string userId)
        {
            if (iDPGroupIds == null)
            {
                return RedirectToAction("AddIDPGroup", new { id = userId });
            }

            List<IDPGroup> selectedIDPGroups = new List<IDPGroup>();

            string id = userId;

            List<string> enrolledIDPGroups = app.GetCheckedIDPGroup(userId);

            foreach (string iDPGroupId in iDPGroupIds)
            {
                if (enrolledIDPGroups.Contains(iDPGroupId))
                {

                    return RedirectToAction("SelectStudent", new { id = userId });
                }

                IDPGroup iDPGroup = app.GetIDPGroups().FirstOrDefault(g => g.IDPGroupId == iDPGroupId);
                if (iDPGroup != null)
                {
                    selectedIDPGroups.Add(iDPGroup);
                }
            }

            app.InsertIDPGroup(selectedIDPGroups, id);

            return RedirectToAction("AddIDPGroup", new { id = userId });
        }
        public ActionResult DeleteIDPGroupByEmployee(int id)
        {
            string userId = app.GetIdByEnrollment(id);

            app.DeleteIDPGroupByEmployee(id);

            return RedirectToAction("AddIDPGroup", new { id = userId });

        }*/


        //Competency Item
        public ActionResult AddCompetency(string id)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;

                string idpGroupName = app.GetIDPGroupNameByIDPGroupId(id);
                string year = app.GetYearById(id);
                List<IDPGroupItem> competencyItems = app.GetIDPGroupItems(id);
                ViewBag.IDPGroupId = id;
                ViewBag.IDPGroupName = idpGroupName;
                ViewBag.Year = year;
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
            app.UpdateIDPGroupItems(idpGroupItems);

            return RedirectToAction("AddCompetency", new { id = idpGroupId });
        }
        public ActionResult SelectCompetency(string id)
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

                List<string> enrolledSubjectCodes = app.GetCheckedCompetencyId(id);

                List<Competency> availableSubjects = competencies.Where(c => !enrolledSubjectCodes.Contains(c.CompetencyId)).ToList();

                availableSubjects.ForEach(c => c.IDPGroupItem = new IDPGroupItem());

                ViewBag.IDPGroupId = id;
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

            string id = idpGroupId;
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

            app.InsertCompetency(selectedCompetencies, id);

            bool hasExistingResults = app.IsAlreadyResultEachYearByIds(allIdsInEnroll, year);
            if (hasExistingResults)
            {
                app.UpdateResultEmployeesById(allIdsInEnroll, year);
            }


            return RedirectToAction("AddCompetency", new { id = idpGroupId });
        }
        public ActionResult DeleteIDPGroupItem(int id)
        {
            string idpGroupId = app.GetIDPGroupIdByIDPGroupItem(id);
            string year = app.GetYearById(idpGroupId);
            List<string> allIdsInEnroll = app.GetIdsThatEnrollByIDPGroupId(idpGroupId);

            app.DeleteIDPGroupItem(id);

            int thisGroup = app.GetCountCompetencyThisId(idpGroupId);
    
            app.UpdateResultEmployeeAfterDeleteFromAddCompetency(thisGroup, allIdsInEnroll, year, idpGroupId);

            return RedirectToAction("AddCompetency", new { id = idpGroupId });
        }



        //User Enroll
        public ActionResult AddEmployee(string id)
        {
            HttpCookie usernameCookie = Request.Cookies["username"];
            if (usernameCookie != null)
            {
                string username = usernameCookie.Value;
                List<UserFormAuth> auths = app.GetUserFormAuths();
                bool isAdmin = auths.Exists(auth => auth.Username == username && auth.ObjectName == "AUTH" && auth.Value == "Admin");

                ViewBag.isAdmin = isAdmin;
                ViewBag.Username = username;
                List<Enrollment> enrollments = app.GetEnrollments(id);

                string IDPGroupName = app.GetIDPGroupNameByIDPGroupId(id);
                string year = app.GetYearById(id);

                ViewBag.IDPGroupID = id;
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
        public ActionResult AddEmployee(List<string> enrollIds, string idpGroupId)
        {
            if (enrollIds != null && enrollIds.Any())
            {
                foreach (var enrollId in enrollIds)
                {
                    // อัปเดตสถานะเป็น "In Progress" สำหรับ enrollId ที่เลือก
                    app.UpdateEnrollmentStatus_1(enrollId, idpGroupId);
                }
            }
            return RedirectToAction("AddEmployee", new { id = idpGroupId });
        }
        public ActionResult SelectEmployee(string id)
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

                List<string> enrolledIds = app.GetCheckedId(id);

                List<User> availableIds = users.Where(u => !enrolledIds.Contains(u.Id)).ToList();

                availableIds.ForEach(u => u.Enrollment = new Enrollment());

                string IDPGroupName = app.GetIDPGroupNameByIDPGroupId(id);
                string year = app.GetYearById(id);

                ViewBag.Username = Request.Cookies["username"].Value;
                ViewBag.IDPGroupID = id;
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
                return RedirectToAction("AddEmployee", new { id = idpGroupId });
            }

            List<User> selectedUsers = new List<User>();
            string year = app.GetYearById(idpGroupId);

            List<string> enrolledUsers = app.GetCheckedId(idpGroupId);

            foreach (string userId in userIds)
            {
                if (enrolledUsers.Contains(userId))
                {
                    return RedirectToAction("SelectStudent", new { id = idpGroupId });
                }

                User user = app.GetEmployeeAtActive().FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    selectedUsers.Add(user);
                }
            }

            app.InsertEmployee(selectedUsers, idpGroupId);

            /*bool hasExistingResults = app.IsAlreadyResultEachYear(selectedUsers, year);
            if (hasExistingResults)
            {
                app.UpdateResultEmployees(selectedUsers, year);
            }
            else
            {
                app.InsertResultEmployees(selectedUsers, year, ViewBag.Username);
            }*/

            app.InsertResultEmployees(selectedUsers, year, ViewBag.Username, idpGroupId);

            return RedirectToAction("AddEmployee", new { id = idpGroupId });
        }
        public ActionResult DeleteEmployeeByIDPGroup(int id) 
        {
            //ตอนกด Delete แล้ว
            string idpGroupId = app.GetIDPGroupIdByEnrollment(id);
            string year = app.GetYearById(idpGroupId);
            string empid = app.GetIdByEnrollment(id);

            app.DeleteEmployeeByIDPGroup(id);

            //หลังกด Delete 

            //app.UpdateResultEmployeeAfterDelete(empid, year);

            return RedirectToAction("AddEmployee", new { id = idpGroupId });
        }


        //Upload Competency
        public ActionResult UploadCompetency()
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            int rowCount = TempData.ContainsKey("RowCount") ? int.Parse(TempData["RowCount"].ToString()) : 0;
            TempData["RowCount"] = rowCount.ToString();
            return View();
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
                    SqlCommand insertCommand = new SqlCommand("INSERT INTO IDP_COMPTY (COMPETENCY_ID, COMPETENCY_NAME_TH, COMPETENCY_NAME_EN, COMPETENCY_DESC, PL1, PL2, PL3, PL4, PL5, ACTIVE, TYPE) " +
                                                             "VALUES (@CompetencyId, @CompetencyNameTH, @CompetencyNameEN, @CompetencyDesc, @Pl1, @Pl2, @Pl3, @Pl4, @Pl5, @Active, @Type)", con);

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

                            insertCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand updateCommand = new SqlCommand("UPDATE IDP_COMPTY SET CompetencyNameTH = @CompetencyNameTH, CompetencyNameEN = @CompetencyNameEN, CompetencyDesc = @CompetencyDesc, " +
                                "Pl1 = @Pl1, Pl2 = @Pl2, Pl3 = @Pl3, Pl4 = @Pl4, Pl5 = @Pl5, Active = @Active, Type = @Type WHERE COMPETENCY_ID = @CompetencyId", con);

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
     

        //Upload Employee
        public ActionResult UploadEmployee()
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            return View();
        }
        [HttpPost]
        public ActionResult UploadEmployee(HttpPostedFileBase file)
        {
            ViewBag.Username = Request.Cookies["username"].Value;

            if (file != null && file.ContentLength > 0)
            {
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/Excel/" + filename;
                file.SaveAs(Server.MapPath(filepath));
                InsertExceldata2(filepath, filename);
                TempData["UploadSuccess"] = true;

            }

            return RedirectToAction("UploadEmployee");
        }
        private void InsertExceldata2(string FilePath, string FileName)
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
                    SqlCommand checkExistCommand = new SqlCommand("SELECT ID FROM MAS_USER_HR WHERE ID = @Id", con);
                    SqlCommand insertCommand = new SqlCommand("INSERT INTO MAS_USER_HR (ID, PREFIX, FIRSTNAME_TH, LASTNAME_TH, FIRSTNAME_EN, LASTNAME_EN, STATUS, STATUS_DATE, COMPANY" +
                        "                                       , LOCATION, POSITION, JOBLEVEL, COSTCENTER, DEPARTMENT, DEPARTMENT_NAME, EMAIL, USER_LOGIN, Enabled, SHIFTWORK" +
                        "                                       , WORK_CENTER, HRPositionCode, JobRole, WorkAge, StartWorkDate) " +
                                                             "VALUES (@Id, @Prefix, @FirstNameTH, @LastNameTH, @FirstNameEN, @LastNameEN, @Status, @StatusDate, @Company, " +
                                                             "@Location, @Position, @JobLevel, @CostCenter, @Department, @DepartmentName, @Email, @UserLogin, @Enabled, @ShiftWork, " +
                                                             "@WorkCenter, @HRPositionCode, @JobRole, @WorkAge, @StartWorkDate)", con);

                    foreach (DataRow row in dt.Rows)
                    {
                        string Id = row["ID"].ToString();
                        checkExistCommand.Parameters.Clear();
                        checkExistCommand.Parameters.AddWithValue("@Id", Id);

                        object existingCode = checkExistCommand.ExecuteScalar();
                        if (existingCode == null)
                        {
                            insertCommand.Parameters.Clear();
                            insertCommand.Parameters.AddWithValue("@Id", Id);
                            insertCommand.Parameters.AddWithValue("@Prefix", row["PREFIX"]);
                            insertCommand.Parameters.AddWithValue("@FirstNameTH", row["FIRSTNAME_TH"]);
                            insertCommand.Parameters.AddWithValue("@LastNameTH", row["LASTNAME_TH"]);
                            insertCommand.Parameters.AddWithValue("@FirstNameEN", row["FIRSTNAME_EN"]);
                            insertCommand.Parameters.AddWithValue("@LastNameEN", row["LASTNAME_EN"]);
                            insertCommand.Parameters.AddWithValue("@Status", row["STATUS"]);
                            insertCommand.Parameters.AddWithValue("@StatusDate", row["STATUS_DATE"]);
                            insertCommand.Parameters.AddWithValue("@Company", row["COMPANY"]);
                            insertCommand.Parameters.AddWithValue("@Location", row["LOCATION"]);
                            insertCommand.Parameters.AddWithValue("@Position", row["POSITION"]);
                            insertCommand.Parameters.AddWithValue("@JobLevel", row["JOBLEVEL"]);
                            insertCommand.Parameters.AddWithValue("@CostCenter", row["COSTCENTER"]);
                            insertCommand.Parameters.AddWithValue("@Department", row["DEPARTMENT"].ToString());
                            insertCommand.Parameters.AddWithValue("@DepartmentName", row["DEPARTMENT_NAME"]);
                            insertCommand.Parameters.AddWithValue("@Email", row["EMAIL"]);
                            insertCommand.Parameters.AddWithValue("@UserLogin", row["USER_LOGIN"]);
                            insertCommand.Parameters.AddWithValue("@Enabled", row["Enabled"]);
                            insertCommand.Parameters.AddWithValue("@ShiftWork", row["SHIFTWORK"]);
                            insertCommand.Parameters.AddWithValue("@WorkCenter", row["WORK_CENTER"]);
                            insertCommand.Parameters.AddWithValue("@HRPositionCode", row["HRPositionCode"]);
                            insertCommand.Parameters.AddWithValue("@JobRole", row["JobRole"]);
                            insertCommand.Parameters.AddWithValue("@WorkAge", row["WorkAge"]);
                            insertCommand.Parameters.AddWithValue("@StartWorkDate", row["StartWorkDate"]);
                                    
                            insertCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand updateCommand = new SqlCommand("UPDATE MAS_USER_HR SET PREFIX = @Prefix, FIRSTNAME_TH = @FirstNameTH, LASTNAME_TH = @LastNameTH, " +
                                "FIRSTNAME_EN = @FirstNameEN, LASTNAME_EN = @LastNameEN, STATUS = @Status, STATUS_DATE = @StatusDate, COMPANY = @Company, LOCATION = @Location, " +
                                "POSITION = @Position, JOBLEVEL = @JobLevel, COSTCENTER = @CostCenter, DEPARTMENT = @Department, DEPARTMENT_NAME = @DepartmentName, EMAIL = @Email, " +
                                "USER_LOGIN = @UserLogin, Enabled = @Enabled, SHIFTWORK = @ShiftWork, WORK_CENTER = @WorkCenter, HRPositionCode = @HRPositionCode, JobRole = @JobRole, " +
                                "WorkAge = @WorkAge, StartWorkDate = @StartWorkDate WHERE Id = @Id", con);

                            updateCommand.Parameters.AddWithValue("@Id", Id);
                            updateCommand.Parameters.AddWithValue("@Prefix", row["PREFIX"]);
                            updateCommand.Parameters.AddWithValue("@FirstNameTH", row["FIRSTNAME_TH"]);
                            updateCommand.Parameters.AddWithValue("@LastNameTH", row["LASTNAME_TH"]);
                            updateCommand.Parameters.AddWithValue("@FirstNameEN", row["FIRSTNAME_EN"]);
                            updateCommand.Parameters.AddWithValue("@LastNameEN", row["LASTNAME_EN"]);
                            updateCommand.Parameters.AddWithValue("@Status", row["STATUS"]);
                            updateCommand.Parameters.AddWithValue("@StatusDate", row["STATUS_DATE"]);
                            updateCommand.Parameters.AddWithValue("@Company", row["COMPANY"]);
                            updateCommand.Parameters.AddWithValue("@Location", row["LOCATION"]);
                            updateCommand.Parameters.AddWithValue("@Position", row["POSITION"]);
                            updateCommand.Parameters.AddWithValue("@JobLevel", row["JOBLEVEL"]);
                            updateCommand.Parameters.AddWithValue("@CostCenter", row["COSTCENTER"]);
                            updateCommand.Parameters.AddWithValue("@Department", row["DEPARTMENT"].ToString());
                            updateCommand.Parameters.AddWithValue("@DepartmentName", row["DEPARTMENT_NAME"]);
                            updateCommand.Parameters.AddWithValue("@Email", row["EMAIL"]);
                            updateCommand.Parameters.AddWithValue("@UserLogin", row["USER_LOGIN"]);
                            updateCommand.Parameters.AddWithValue("@Enabled", row["Enabled"]);
                            updateCommand.Parameters.AddWithValue("@ShiftWork", row["SHIFTWORK"]);
                            updateCommand.Parameters.AddWithValue("@WorkCenter", row["WORK_CENTER"]);
                            updateCommand.Parameters.AddWithValue("@HRPositionCode", row["HRPositionCode"]);
                            updateCommand.Parameters.AddWithValue("@JobRole", row["JobRole"]);
                            updateCommand.Parameters.AddWithValue("@WorkAge", row["WorkAge"]);
                            updateCommand.Parameters.AddWithValue("@StartWorkDate", row["StartWorkDate"]);
                          

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

        //Upload IDP Group
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
                        if (existingCode == null)
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
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = "เกิดข้อผิดพลาดในการอัปโหลด: " + ex.Message;
            }
        }


        //Email
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


        //Form
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
                //bool isGM = auths.Exists(auth => auth.Username == username && auth.ObjectName == "COST_CENTER" && auth.Value == "1050100");
                ViewBag.isAdmin = isAdmin;
                //ViewBag.isGM = isGM;
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
                if (status == "1st Evaluating" || status == "2nd Evaluating")
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
            if (status == "Evaluating")
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

            return RedirectToAction("Form", "Home", new { user = user, idpGroupId = IDPGroup, guid = Guid });

        }


        //info
        public ActionResult Info(string user, string year)
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

                int count = app.GetCountEnrollmentById(id);
                string guid = app.GetGuidByIdAndYear(id, year);
                string prefix = app.GetPrefixById(id);
                string firstName = app.GetFirstNameById(id);
                string lastName = app.GetLastNameById(id);
                string company = app.GetCompanyById(id);
                string joblevel = app.GetJoblevelById(id);
                string department = app.GetDepartmentById(id);
                string position = app.GetPositionById(id);

                ViewBag.Prefix = prefix;
                ViewBag.FirstName = firstName;
                ViewBag.LastName = lastName;
                ViewBag.Company = company;
                ViewBag.Joblevel = joblevel;
                ViewBag.Department = department;
                ViewBag.Position = position;
                ViewBag.Count = count;
                ViewBag.Year = year;
                ViewBag.Id = id;

                List<Enrollment> enrollments = app.GetInfoEmployeeByCookie(user, year);

                return View(enrollments);
            }
            else
            {
                return RedirectToAction("Index", "Form");
            }
        }



    }
}