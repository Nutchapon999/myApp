using DocumentFormat.OpenXml.Bibliography;
using myApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace myApp.DAL
{
    public class WorkFlow
    {
        private string connectionString;
        public WorkFlow()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }

        public string ConnectionString;
        public string K2_NO { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string Year { get; set; }
        public string Action { get; set; }
        public string Remark { get; set; }
        public string RemarkDate { get; set; }

        public List<WorkFlow> GetWorkflows(string username, string year)
        {
            List<WorkFlow> workFlows = new List<WorkFlow>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT F.*, R.EN_STATUS, R.YEAR, R.REMARK, R.REMARK_DATE FROM F_K2_IDP_RESULT(@BeginYear,@LastYear,@Username) F " +
                                        "JOIN (SELECT RE.*, EN.STATUS AS EN_STATUS, RM.REMARK, RM.REMARK_DATE " +
                                        "FROM IDP_RESULT RE " +
                                        "JOIN IDP_USER_ENROLL EN ON RE.ID = EN.ID " +
                                        "JOIN REMARK_HISTORY RM ON RE.GUID = RM.FORM_GUID) " +
                                        "R ON R.K2_NO = F.K2_NO WHERE ACTION_BY = 'WORKFLOW'";

                var beginYear = "01/01/" + year;
                var lastYear = "31/12/" + year;

                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@BeginYear", beginYear);
                command.Parameters.AddWithValue("@LastYear", lastYear);


                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WorkFlow workFlow = new WorkFlow();
                        workFlow.K2_NO = (string)reader["K2_NO"];
                        workFlow.Action = (string)reader["ACTION_BY"];
                        workFlow.Subject = (string)reader["SUBJECT"];
                        workFlow.Status = (string)reader["EN_STATUS"];
                        workFlow.Year = (string)reader["YEAR"];
                        workFlow.Remark = (string)reader["REMARK"];
                        DateTime remarkDate = (DateTime)reader["REMARK_DATE"];
                        workFlow.RemarkDate = remarkDate.ToString("MM/dd/yyyy");

                        workFlows.Add(workFlow);
                    }
                }
            }

            return workFlows;
        }

    }
}