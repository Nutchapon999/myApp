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

        //Index (Competency)
        public ActionResult Index()
        {
            var username = "";

            // Cookie
            if (ConfigurationManager.AppSettings["IsDev"].ToString().ToLower() == "true")
            {
                System.Web.HttpCookie UserCookie = new System.Web.HttpCookie("username", "Pondpopza");
                HttpContext.Response.Cookies.Add(UserCookie);
                username = UserCookie.Value;
            }
            else
            {
                username = K2UserAuthen.GetUserAut(ConfigurationManager.AppSettings["PageK2Five"].ToString()).Username;
            }

            ViewBag.Username = username;
            List<Competency> competencies = app.GetCompetencies();

            return View(competencies);
        }
        public ActionResult CreateCompetency()
        {

            ViewBag.Username = Request.Cookies["username"].Value;

            return View();
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
            Competency competency = app.EditCompetency(id);
            return View(competency);
        }
        [HttpPost]
        public ActionResult EditCompetency(Competency competency)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                app.UpdateCompetency(competency);
                return RedirectToAction("Index");
            }
            return View(competency);
        }
        public ActionResult DeleteCompetency(string id)
        {
            app.DeleteCompetency(id);
            return RedirectToAction("Index");
        }


        //IDP GROUP
        public ActionResult IDPGroup()
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            List<IDPGroup> iDPGroups = app.GetIDPGroups();

            // Loop through each IDPGroup and update the enrollment count
            foreach (var idpGroup in iDPGroups)
            {
                idpGroup.EmployeeEnrollmentCount = app.GetCountEmployee(idpGroup.IDPGroupId);
            }

            return View(iDPGroups);
        }
        public ActionResult CreateIDPGroup()
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            return View();
        }
        [HttpPost]
        public ActionResult CreateIDPGroup(IDPGroup iDPGroup)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                try
                {
                    app.CreateIDPGroup(iDPGroup);

                    return RedirectToAction("IDPGroup", "Home");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("@Id"))
                    {
                        ViewBag.ErrorMessage = "  กรุณากรอกรหัส IDP Group";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = ex.Message;
                    }

                    return View(iDPGroup);
                }
            }
            return View(iDPGroup);
        }
        public ActionResult EditIDPGroup(string id)
        {
            string idpGroupName = app.GetIDPGroupNameById(id);
            string year = app.GetYearById(id);
            ViewBag.IDPGroupId = id;
            ViewBag.IDPGroupName = idpGroupName;
            ViewBag.Year = year;
            ViewBag.Username = Request.Cookies["username"].Value;
            IDPGroup iDPGroup = app.EditIDPGroup(id);
            return View(iDPGroup);
        }
        [HttpPost]
        public ActionResult EditIDPGroup(IDPGroup iDPGroup)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                app.UpdateIDPGroup(iDPGroup);
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
            string idpGroupName = app.GetIDPGroupNameById(id);
            string year = app.GetYearById(id);
            int members = app.GetCountEmployee(id);
            int competencies = app.GetCountCompetency(id);
            ViewBag.Username = Request.Cookies["username"].Value;
            List<IDPGroup> iDPGroups = app.GetDetails(id);
            ViewBag.IDPGroupId = id;
            ViewBag.IDPGroupName = idpGroupName;
            ViewBag.Year = year;
            ViewBag.Member = members;
            ViewBag.Competency = competencies;
            return View(iDPGroups);
        }
       

        //HR User
        public ActionResult Employee()
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            List<User> users = app.GetUsers();
            return View(users);
        }
        public ActionResult DeleteEmployee(string id)
        {
            app.DeleteEmployee(id);
            return RedirectToAction("Employee");
        }
        public ActionResult AddIDPGroup(string id)
        {
            List<Enrollment> enrollments = app.GetIDPGroupByEmployee(id);

            ViewBag.Id = id;

            return View(enrollments);
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

        }


        //Competency Item
        public ActionResult AddCompetency(string id)
        {
            string idpGroupName = app.GetIDPGroupNameById(id);
            List<CompetencyItem> competencyItems = app.GetCompetencyItems(id);
            ViewBag.IDPGroupId = id;
            ViewBag.IDPGroupName = idpGroupName;
            return View(competencyItems);
        }
        public ActionResult SelectCompetency(string id)
        {
            List<Competency> competencies = app.GetCompetencyAtActive();

            List<string> enrolledSubjectCodes = app.GetCheckedCompetencyId(id);

            List<Competency> availableSubjects = competencies.Where(c => !enrolledSubjectCodes.Contains(c.CompetencyId)).ToList();

            availableSubjects.ForEach(c => c.CompetencyItem = new CompetencyItem());

            ViewBag.IDPGroupId = id;
            return View(availableSubjects);
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

            List<string> enrolledSubjectCodes = app.GetCheckedCompetencyId(idpGroupId);

            foreach (string competencyId in competencyIds)
            {
                Competency competency = app.GetCompetencyAtActive().FirstOrDefault(c => c.CompetencyId == competencyId);
                if (competency != null)
                {
                    string selectedPl = plValues.ContainsKey(competencyId) ? plValues[competencyId] : null;
                    string selectedPriority = priorityValues.ContainsKey(competencyId) ? priorityValues[competencyId] : null;

                    competency.CompetencyItem = new CompetencyItem(); // Initialize CompetencyItem if null
                    competency.CompetencyItem.Pl = selectedPl;
                    competency.CompetencyItem.Priority = selectedPriority;

                    selectedCompetencies.Add(competency);
                }
            }

            app.InsertCompetency(selectedCompetencies, id);

            return RedirectToAction("AddCompetency", new { id = idpGroupId });
        }
        public ActionResult DeleteCompetencyItem(int id)
        {
            string idpGroupId = app.GetIDPGroupIdByCompetencyItem(id); 

            app.DeleteCompetencyItem(id);

            return RedirectToAction("AddCompetency", new { id = idpGroupId });
        }
        

        //User Enroll
        public ActionResult AddEmployee(string id)
        {
            List<Enrollment> enrollments = app.GetEnrollments(id);

            ViewBag.IDPGroupID = id;
      
            return View(enrollments);
        }
        public ActionResult SelectEmployee(string id)
        {
            List<User> users = app.GetEmployeeAtActive();

            List<string> enrolledIds = app.GetCheckedId(id);

            List<User> availableIds = users.Where(u => !enrolledIds.Contains(u.Id)).ToList();

            availableIds.ForEach(u => u.Enrollment = new Enrollment());

            ViewBag.IDPGroupID = id;

            return View(availableIds);
        }
        [HttpPost]
        public ActionResult SelectedEmployee(List<string> userIds, string idpGroupId)
        {
            if (userIds == null)
            {
                return RedirectToAction("AddEmployee", new { id = idpGroupId });
            }

            List<User> selectedUsers = new List<User>();

            string id = idpGroupId;

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

            app.InsertEmployee(selectedUsers, id);

            return RedirectToAction("AddEmployee", new { id = idpGroupId });
        }
        public ActionResult DeleteEmployeeByIDPGroup(int id)
        {
            string idpGroupId = app.GetIDPGroupIdByEnrollment(id);

            app.DeleteEmployeeByIDPGroup(id);

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
                    SqlCommand checkExistCommand = new SqlCommand("SELECT COMPETENCY_ID FROM COMPTY WHERE COMPETENCY_ID = @CompetencyId", con);
                    SqlCommand insertCommand = new SqlCommand("INSERT INTO COMPTY (COMPETENCY_ID, COMPETENCY_NAME_TH, COMPETENCY_NAME_EN, COMPETENCY_DESC, PL1, PL2, PL3, PL4, PL5, ACTIVE, TYPE) " +
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
                            SqlCommand updateCommand = new SqlCommand("UPDATE COMPTY SET CompetencyNameTH = @CompetencyNameTH, CompetencyNameEN = @CompetencyNameEN, CompetencyDesc = @CompetencyDesc, " +
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
        /*private int GetExcelRowCount(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                ExcelWorkbook workbook = package.Workbook;

                if (workbook.Worksheets.Count > 0)
                {
                    ExcelWorksheet worksheet = workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    return rowCount;
                }

                
                return 0;
            }
        }*/

        

        //Upload Employee
        public ActionResult UploadEmployee()
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            return View();
        }
        [HttpPost]
        public ActionResult UploadEmployee(HttpPostedFileBase file)
        {
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


        public ActionResult FormEmail(string id)
        {
            return View();
        }

    }
}