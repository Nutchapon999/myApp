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
                        ViewBag.ErrorMessage = "  กรุณากรอกรหัสวิชา";
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


        //Competency Form
        public ActionResult CompetencyForm()
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            List<CompetencyForm> competencyForms = app.GetCompetencyForms();   
            return View(competencyForms);
        }
        public ActionResult CreateCompetencyForm()
        {

            ViewBag.Username = Request.Cookies["username"].Value;

            return View();
        }
        [HttpPost]
        public ActionResult CreateCompetencyForm(CompetencyForm competencyForm)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                try
                {
                    app.CreateCompetencyForm(competencyForm);

                    return RedirectToAction("CompetencyForm", "Home");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("@Id"))
                    {
                        ViewBag.ErrorMessage = "  กรุณากรอกรหัสหลักสูตร";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = ex.Message;
                    }

                    return View(competencyForm);
                }
            }
            return View(competencyForm);
        }
        public ActionResult EditCompetencyForm(string id)
        {
            string competencyFormName = app.GetCompetencyFormNameById(id);
            string year = app.GetYearById(id);
            ViewBag.CompetencyFormId = id;
            ViewBag.CompetencyFormName = competencyFormName;
            ViewBag.Year = year;
            ViewBag.Username = Request.Cookies["username"].Value;
            CompetencyForm competencyForm = app.EditCompetencyForm(id);
            return View(competencyForm);
        }
        [HttpPost]
        public ActionResult EditCompetencyForm(CompetencyForm competencyForm)
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            if (ModelState.IsValid)
            {
                app.UpdateCompetencyForm(competencyForm);
                return RedirectToAction("CompetencyForm");
            }
            return View(competencyForm);
        }
        public ActionResult DeleteCompetencyForm(string id)
        {
            app.DeleteCompetencyForm(id);
            return RedirectToAction("CompetencyForm");
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



        //Competency Item
        public ActionResult AddCompetency(string id)
        {
            string competencyFormName = app.GetCompetencyFormNameById(id);
            List<CompetencyItem> competencyItems = app.GetCompetencyItems(id);
            ViewBag.CompetencyFormId = id;
            ViewBag.CompetencyName = competencyFormName;
            return View(competencyItems);
        }
        public ActionResult SelectCompetency(string id)
        {
            List<Competency> competencies = app.GetCompetencyAtActive();

            List<string> enrolledSubjectCodes = app.GetCheckedCompetencyId(id);

            List<Competency> availableSubjects = competencies.Where(c => !enrolledSubjectCodes.Contains(c.CompetencyId)).ToList();

            availableSubjects.ForEach(c => c.CompetencyItem = new CompetencyItem());

            ViewBag.CompetencyFormId = id;
            return View(availableSubjects);
        }
        [HttpPost]
        public ActionResult SelectedCompetency(List<string> competencyIds, string competencyFormId, Dictionary<string, string> plValues, Dictionary<string, string> priorityValues)
        {
            if (competencyIds == null)
            {
                return RedirectToAction("AddCompetency", new { id = competencyFormId });
            }

            List<Competency> selectedCompetencies = new List<Competency>();
            string id = competencyFormId;

            List<string> enrolledSubjectCodes = app.GetCheckedCompetencyId(competencyFormId);

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

            return RedirectToAction("AddCompetency", new { id = competencyFormId });
        }
        public ActionResult DeleteCompetencyItem(int id)
        {
            string competencyFormId = app.GetCompetencyFormIdByCompetencyItem(id); // Retrieve the courseId associated with the deleted enrollment

            app.DeleteCompetencyItem(id);

            return RedirectToAction("AddCompetency", new { id = competencyFormId });
        }
        

        //User Enroll
        public ActionResult AddEmployee(string id)
        {
            List<Enrollment> enrollments = app.GetEnrollments(id);
            ViewBag.CompetencyFormId = id;
            return View(enrollments);
        }
        public ActionResult SelectEmployee(string id)
        {
            List<User> users = app.GetEmployeeAtActive();

            List<string> enrolledIds = app.GetCheckedId(id);

            List<User> availableIds = users.Where(u => !enrolledIds.Contains(u.Id)).ToList();

            availableIds.ForEach(u => u.Enrollment = new Enrollment());

            ViewBag.CompetencyFormId = id;
            return View(availableIds);
        }
        [HttpPost]
        public ActionResult SelectedEmployee(List<string> userIds, string competencyFormId)
        {
            if (userIds == null)
            {
                return RedirectToAction("AddEmployee", new { id = competencyFormId });
            }

            List<User> selectedUsers = new List<User>();
            string id = competencyFormId;

            List<string> enrolledUsers = app.GetCheckedId(competencyFormId);

            foreach (string userId in userIds)
            {
                if (enrolledUsers.Contains(userId))
                {

                    return RedirectToAction("SelectStudent", new { id = competencyFormId });
                }

                User user = app.GetEmployeeAtActive().FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    selectedUsers.Add(user);
                }
            }

            app.InsertEmployee(selectedUsers, id);

            return RedirectToAction("AddEmployee", new { id = competencyFormId });
        }


        //Upload Competency
        public ActionResult UploadCompetency()
        {
            ViewBag.Username = Request.Cookies["username"].Value;
            return View();
        }
        [HttpPost]
        public ActionResult UploadCompetency(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                int rowCount = GetExcelRowCount(file) - 1;
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/Excel/" + filename;
                file.SaveAs(Server.MapPath(filepath));
                InsertExceldata1(filepath, filename);
                TempData["UploadSuccess"] = true;
                TempData["RowCount"] = rowCount.ToString();
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
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = "เกิดข้อผิดพลาดในการอัปโหลด: " + ex.Message;
            }
        }
        private int GetExcelRowCount(HttpPostedFileBase file)
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
            List<CompetencyForm> competencyForms = app.SelectIDPGroup();
            return View(competencyForms);
        }
        [HttpPost]
        public ActionResult SendEmail(string CompetencyFormId, string SelectedUser)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("PondPoP Za", "pondpopza19@gmail.com"));
                message.To.Add(MailboxAddress.Parse(SelectedUser));
                message.Subject = "แบบประเมิน IDP";
                message.Body = new TextPart("plain")
                {
                    Text = @"กรุณาช่วยระบุลำดับความสำคัญของ IDP Group ด้วย " +
                           "รหัส IDP Group: " + CompetencyFormId +
                           " และได้แนบลิ้งค์ไว้ดังนี้ https://www.google.com ขอขอบพระคุณอย่างยิ่ง :D"
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