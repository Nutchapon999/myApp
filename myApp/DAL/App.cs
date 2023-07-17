using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using myApp.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;

namespace myApp.DAL
{
    public class App
    {
        private string connectionString;
        public App()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }

        public string ConnectionString;
        //Competency
        public List<Competency> GetCompetencies()
        {
            List<Competency> competencies = new List<Competency>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_COMPTY";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Competency competency = new Competency();

                    competency.CompetencyId = (string)reader["COMPETENCY_ID"];
                    competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];
                    competency.CompetencyNameEN = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_EN")) ? null : (string)reader["COMPETENCY_NAME_EN"];
                    competency.CompetencyDesc = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_DESC")) ? null : (string)reader["COMPETENCY_DESC"];
                    competency.Pl1 = reader.IsDBNull(reader.GetOrdinal("PL1")) ? null : (string)reader["PL1"];
                    competency.Pl2 = reader.IsDBNull(reader.GetOrdinal("PL2")) ? null : (string)reader["PL2"];
                    competency.Pl3 = reader.IsDBNull(reader.GetOrdinal("PL3")) ? null : (string)reader["PL3"];
                    competency.Pl4 = reader.IsDBNull(reader.GetOrdinal("PL4")) ? null : (string)reader["PL4"];
                    competency.Pl5 = reader.IsDBNull(reader.GetOrdinal("PL5")) ? null : (string)reader["PL5"];
                    competency.Active = (bool)reader["Active"];
                    competency.Type = reader.IsDBNull(reader.GetOrdinal("TYPE")) ? null : (string)reader["TYPE"];
                    competencies.Add(competency);
                }
                reader.Close();
            }

            return competencies;
        }
        public void CreateCompetency(Competency competency)
        {
            if (IsDuplicateCompetencyId(competency.CompetencyId))
            {
                throw new Exception(" รหัส Competency นี้มีการใช้แล้ว กรุณากรอกใหม่");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO IDP_COMPTY (COMPETENCY_ID, TYPE, COMPETENCY_NAME_TH, COMPETENCY_NAME_EN, COMPETENCY_DESC, PL1, PL2, PL3, PL4, PL5, ACTIVE) " +
                                "VALUES (@Id, @Type, @TH, @EN, @Desc, @Pl1, @Pl2, @Pl3, @Pl4, @Pl5, @Active)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", competency.CompetencyId);
                    command.Parameters.AddWithValue("@Type", (object)competency.Type ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TH", (object)competency.CompetencyNameTH ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EN", (object)competency.CompetencyNameEN ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Desc", (object)competency.CompetencyDesc ?? DBNull.Value);

                    command.Parameters.AddWithValue("@Pl1", (object)competency.Pl1 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl2", (object)competency.Pl2 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl3", (object)competency.Pl3 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl4", (object)competency.Pl4 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl5", (object)competency.Pl5 ?? DBNull.Value);

                    command.Parameters.AddWithValue("@Active", (object)competency.Active ?? DBNull.Value);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }
        private bool IsDuplicateCompetencyId(string competencyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_COMPTY WHERE COMPETENCY_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", competencyId);

                    connection.Open();

                    int existingCount = (int)command.ExecuteScalar();

                    return existingCount > 0;
                }
            }
        }
        public Competency EditCompetency(string competencyId, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_COMPTY WHERE COMPETENCY_ID = @CompetencyId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompetencyId", competencyId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Competency competency = new Competency();
                            competency.CompetencyId = (string)reader["COMPETENCY_ID"];
                            competency.Type = (string)reader["TYPE"];
                            competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];
                            competency.CompetencyNameEN = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_EN")) ? null : (string)reader["COMPETENCY_NAME_EN"];
                            competency.CompetencyDesc = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_DESC")) ? null : (string)reader["COMPETENCY_DESC"];
                            competency.Pl1 = reader.IsDBNull(reader.GetOrdinal("PL1")) ? null : (string)reader["PL1"];
                            competency.Pl2 = reader.IsDBNull(reader.GetOrdinal("PL2")) ? null : (string)reader["PL2"];
                            competency.Pl3 = reader.IsDBNull(reader.GetOrdinal("PL3")) ? null : (string)reader["PL3"];
                            competency.Pl4 = reader.IsDBNull(reader.GetOrdinal("PL4")) ? null : (string)reader["PL4"];
                            competency.Pl5 = reader.IsDBNull(reader.GetOrdinal("PL5")) ? null : (string)reader["PL5"];
                            competency.Active = (bool)reader["ACTIVE"];
                            return competency;
                        }
                    }
                }
            }
            return null;
        }
        public void UpdateCompetency(Competency competency, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"UPDATE IDP_COMPTY SET TYPE = @Type, COMPETENCY_NAME_TH = @TH, COMPETENCY_NAME_EN = @EN, COMPETENCY_DESC = @Desc, " +
                            "PL1 = @Pl1, PL2 = @Pl2, PL3 = @Pl3, PL4 = @Pl4, PL5 = @Pl5, ACTIVE = @Active, UPDATE_BY = @UpdateBy, UPDATE_ON = GETDATE() " +
                            "WHERE COMPETENCY_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Assign values to parameters
                    command.Parameters.AddWithValue("@Id", (object)competency.CompetencyId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Type", (object)competency.Type ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TH", (object)competency.CompetencyNameTH ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EN", (object)competency.CompetencyNameEN ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Desc", (object)competency.CompetencyDesc ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl1", (object)competency.Pl1 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl2", (object)competency.Pl2 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl3", (object)competency.Pl3 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl4", (object)competency.Pl4 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pl5", (object)competency.Pl5 ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Active", (object)competency.Active ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UpdateBy", username);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

            }
        }
        public void DeleteCompetency(string competencyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                DeleteCompetencyItemByCompetencyId(competencyId);

                string query = "DELETE FROM IDP_COMPTY WHERE COMPETENCY_ID = @CompetencyId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompetencyId", competencyId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private void DeleteCompetencyItemByCompetencyId(string competencyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "DELETE FROM IDP_GROUP_ITEM WHERE COMPETENCY_ID = @CompetencyId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompetencyId", competencyId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public string GetCompetencyNameById(string competencyId)
        {
            string competencyName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT COMPETENCY_NAME_TH FROM IDP_COMPTY WHERE COMPETENCY_ID = @CompetencyId";
                command.Parameters.AddWithValue("@CompetencyId", competencyId);

                connection.Open();

                // Assuming course_name is stored as a string column in the "Courses" table
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    competencyName = result.ToString();
                }
            }

            return competencyName;
        }
        public string GetTypeById(string id)
        {
            string type = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT TYPE FROM IDP_COMPTY WHERE COMPETENCY_ID = @CompetencyId";
                command.Parameters.AddWithValue("@CompetencyId", id);

                connection.Open();

                // Assuming the "ACTIVE" column is stored as a boolean in the "IDP_COMPTY" table
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    type = result.ToString();
                }
            }

            return type;
        }


        //IDP Group
        public List<IDPGroup> GetIDPGroups()
        {
            List<IDPGroup> iDPGroups = new List<IDPGroup>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_GROUP";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    IDPGroup iDPGroup = new IDPGroup();

                    iDPGroup.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                    iDPGroup.IDPGroupName = reader.IsDBNull(reader.GetOrdinal("IDP_GROUP_NAME")) ? null : (string)reader["IDP_GROUP_NAME"];
                    iDPGroup.Year = reader.IsDBNull(reader.GetOrdinal("YEAR")) ? null : (string)reader["YEAR"];

                    iDPGroups.Add(iDPGroup);
                }
                reader.Close();
            }

            return iDPGroups;
        }
        public void CreateIDPGroup(IDPGroup idpGroupId, string username)
        {
            if (IsDuplicateIDPGroupId(idpGroupId.IDPGroupId))
            {
                throw new Exception(" รหัสหลักสูตรนี้มีการใช้แล้ว กรุณากรอกใหม่");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO IDP_GROUP (IDP_GROUP_ID, IDP_GROUP_NAME, YEAR, CREATED_BY, CREATED_ON) " +
                                "VALUES (@Id, @Name, @Year, @CreatedBy, GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", idpGroupId.IDPGroupId);
                    command.Parameters.AddWithValue("@Name", (object)idpGroupId.IDPGroupName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Year", (object)idpGroupId.Year ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedBy", username);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }
        public IDPGroup EditIDPGroup(string idpGroupId, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_GROUP WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            IDPGroup iDPGroup = new IDPGroup();
                            iDPGroup.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                            iDPGroup.IDPGroupName = reader.IsDBNull(reader.GetOrdinal("IDP_GROUP_NAME")) ? null : (string)reader["IDP_GROUP_NAME"];
                            iDPGroup.Year = reader.IsDBNull(reader.GetOrdinal("YEAR")) ? null : (string)reader["YEAR"];

                            return iDPGroup;
                        }
                    }
                }
            }
            return null;
        }
        public void UpdateIDPGroup(IDPGroup idpGroup, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE IDP_GROUP SET IDP_GROUP_NAME = @Name, YEAR = @Year, UPDATE_BY = @UpdateBy, UPDATE_ON = GETDATE() WHERE IDP_GROUP_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", idpGroup.IDPGroupId);
                    command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(idpGroup.IDPGroupName) ? DBNull.Value : (object)idpGroup.IDPGroupName);
                    command.Parameters.AddWithValue("@Year", string.IsNullOrEmpty(idpGroup.Year) ? DBNull.Value : (object)idpGroup.Year);
                    command.Parameters.AddWithValue("@UpdateBy", username);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void DeleteIDPGroup(string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                DeleteCompetencyItemByIDPGroupId(idpGroupId);
                DeleteEnrollByIDPGroupId(idpGroupId);

                string query = "DELETE FROM IDP_GROUP WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private void DeleteCompetencyItemByIDPGroupId(string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM IDP_GROUP_ITEM WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private void DeleteEnrollByIDPGroupId(string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM IDP_USER_ENROLL WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private bool IsDuplicateIDPGroupId(string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_GROUP WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    connection.Open();

                    int existingCount = (int)command.ExecuteScalar();

                    return existingCount > 0;
                }
            }
        }
        public string GetIDPGroupNameByIDPGroupId(string idpGroupId)
        {
            string idpGroupName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT IDP_GROUP_NAME FROM IDP_GROUP WHERE IDP_GROUP_ID = @IDPGroupId";
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                // Assuming course_name is stored as a string column in the "Courses" table
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    idpGroupName = result.ToString();
                }
            }

            return idpGroupName;
        }
        public string GetYearById(string idpGroupId)
        {
            string year = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT YEAR FROM IDP_GROUP WHERE IDP_GROUP_ID = @IDPGroupId";
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    year = result.ToString();
                }
            }

            return year;
        }
        public List<IDPGroup> GetDetails(string idpGroupId)
        {
            List<IDPGroup> iDPGroups = new List<IDPGroup>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
               
                string query = "SELECT G.IDP_GROUP_ID, G.IDP_GROUP_NAME, G.YEAR, I.COMPETENCY_ID, C.COMPETENCY_NAME_TH ,I.PL, I.CRITICAL , EN.ID, HR.PREFIX, HR.FIRSTNAME_TH, HR.LASTNAME_TH, HR.JOBLEVEL, HR.POSITION, HR.DEPARTMENT_NAME, HR.COMPANY " +
                    "FROM IDP_GROUP G " +
                    "LEFT JOIN IDP_GROUP_ITEM I ON G.IDP_GROUP_ID = I.IDP_GROUP_ID " +
                    "LEFT JOIN IDP_COMPTY C ON C.COMPETENCY_ID = I.COMPETENCY_ID " +
                    "LEFT JOIN IDP_USER_ENROLL EN ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                    "LEFT JOIN MAS_USER_HR HR ON EN.ID = HR.ID " +
                    "WHERE G.IDP_GROUP_ID = @IDPGroupId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    IDPGroup iDPGroup = new IDPGroup();

                    iDPGroup.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                    iDPGroup.IDPGroupName = reader.IsDBNull(reader.GetOrdinal("IDP_GROUP_NAME")) ? null : (string)reader["IDP_GROUP_NAME"];
                    iDPGroup.Year = reader.IsDBNull(reader.GetOrdinal("YEAR")) ? null : (string)reader["YEAR"];

                    IDPGroupItem idpGroupItem = new IDPGroupItem();
                    idpGroupItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                    idpGroupItem.Pl = reader.IsDBNull(reader.GetOrdinal("PL")) ? null : (string)reader["PL"];
                    idpGroupItem.Critical = reader.IsDBNull(reader.GetOrdinal("CRITICAL")) ? false : (bool)reader["CRITICAL"];

                    Competency competency = new Competency();
                    competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];

                    Enrollment enrollment = new Enrollment();
                    enrollment.Id = reader.IsDBNull(reader.GetOrdinal("ID")) ? null : (string)reader["ID"];

                    User user = new User();
                    user.Prefix = reader.IsDBNull(reader.GetOrdinal("PREFIX")) ? null : (string)reader["PREFIX"];
                    user.FirstNameTH = reader.IsDBNull(reader.GetOrdinal("FIRSTNAME_TH")) ? null : (string)reader["FIRSTNAME_TH"];
                    user.LastNameTH = reader.IsDBNull(reader.GetOrdinal("LASTNAME_TH")) ? null : (string)reader["LASTNAME_TH"];
                    user.Company = reader.IsDBNull(reader.GetOrdinal("COMPANY")) ? null : (string)reader["COMPANY"];
                    user.Position = reader.IsDBNull(reader.GetOrdinal("POSITION")) ? null : (string)reader["POSITION"];
                    user.JobLevel = reader.IsDBNull(reader.GetOrdinal("JOBLEVEL")) ? null : (string)reader["JOBLEVEL"];
                    user.DepartmentName = reader.IsDBNull(reader.GetOrdinal("DEPARTMENT_NAME")) ? null : (string)reader["DEPARTMENT_NAME"];


                    iDPGroup.Competency = competency;
                    iDPGroup.IDPGroupItem = idpGroupItem;
                    iDPGroup.User = user;
                    iDPGroup.Enrollment = enrollment;

                    iDPGroups.Add(iDPGroup);
                }
                reader.Close();
            }

            return iDPGroups;
        }
        public int GetCountCompetency(string idpGroupId)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_GROUP_ITEM WHERE IDP_GROUP_ID = @IDPGroupId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }


        //IDP Group Item
        public List<IDPGroupItem> GetIDPGroupItems(string idpGroupId)
        {
            List<IDPGroupItem> competencyItems = new List<IDPGroupItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT CIT.IDP_GROUP_ITEM_ID, CIT.IDP_GROUP_ID, CIT.COMPETENCY_ID, C.COMPETENCY_NAME_TH, PL, CRITICAL, C.ACTIVE " +
                                        "FROM IDP_GROUP_ITEM AS CIT JOIN IDP_COMPTY AS C ON CIT.COMPETENCY_ID = C.COMPETENCY_ID " +
                                        "WHERE CIT.IDP_GROUP_ID = @IDPGroupId";
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        IDPGroupItem idpGroupItem = new IDPGroupItem();
                        idpGroupItem.IDPGroupItemId = (int)reader["IDP_GROUP_ITEM_ID"];
                        idpGroupItem.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                        idpGroupItem.CompetencyId = (string)reader["COMPETENCY_ID"];
                        idpGroupItem.Pl = (string)reader["PL"];
                        idpGroupItem.Critical = (bool)reader["CRITICAL"];

                        Competency competency = new Competency();
                        competency.CompetencyNameTH = (string)reader["COMPETENCY_NAME_TH"];
                        competency.Active = (bool)reader["Active"];

                        idpGroupItem.Competency = competency;

                        competencyItems.Add(idpGroupItem);
                    }
                }
            }

            return competencyItems;
        }
        public List<Competency> GetCompetencyAtActive()
        {
            List<Competency> competencies = new List<Competency>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_COMPTY WHERE ACTIVE = 1";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Competency competency = new Competency();

                    competency.CompetencyId = (string)reader["COMPETENCY_ID"];
                    competency.CompetencyNameTH = (string)reader["COMPETENCY_NAME_TH"];
                    competency.CompetencyNameEN = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_EN")) ? null : (string)reader["COMPETENCY_NAME_EN"];
                    competency.CompetencyDesc = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_DESC")) ? null : (string)reader["COMPETENCY_DESC"];
                    competency.Pl1 = reader.IsDBNull(reader.GetOrdinal("PL1")) ? null : (string)reader["PL1"];
                    competency.Pl2 = reader.IsDBNull(reader.GetOrdinal("PL2")) ? null : (string)reader["PL2"];
                    competency.Pl3 = reader.IsDBNull(reader.GetOrdinal("PL3")) ? null : (string)reader["PL3"];
                    competency.Pl4 = reader.IsDBNull(reader.GetOrdinal("PL4")) ? null : (string)reader["PL4"];
                    competency.Pl5 = reader.IsDBNull(reader.GetOrdinal("PL5")) ? null : (string)reader["PL5"];

                    competency.Type = (string)reader["TYPE"];
                    competencies.Add(competency);
                }
                reader.Close();
            }

            return competencies;
        }
        public List<string> GetCheckedCompetencyId(string idpGroupId)
        {
            List<string> CheckedCompetencyIds = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COMPETENCY_ID FROM IDP_GROUP_ITEM WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string competencyId = (string)reader["COMPETENCY_ID"];
                            CheckedCompetencyIds.Add(competencyId);
                        }
                    }
                }
            }

            return CheckedCompetencyIds;
        }
        public void InsertCompetency(List<Competency> selectedCompetencies, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (Competency competency in selectedCompetencies)
                {
                    string query = "INSERT INTO IDP_GROUP_ITEM (COMPETENCY_ID, IDP_GROUP_ID, PL, CRITICAL) VALUES (@CompetencyId, @IDPGroupId, @Pl, @Cri)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompetencyId", competency.CompetencyId);
                        command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);
                        command.Parameters.AddWithValue("@Pl", competency.IDPGroupItem.Pl);
                        
                        command.Parameters.AddWithValue("@Cri", competency.IDPGroupItem.Critical);


                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public string GetIDPGroupIdByIDPGroupItem(int competencyItemId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT IDP_GROUP_ID FROM IDP_GROUP_ITEM WHERE IDP_GROUP_ITEM_ID = @Id";
                command.Parameters.AddWithValue("@Id", competencyItemId);

                connection.Open();

                return (string)command.ExecuteScalar();
            }
        }
        public void DeleteIDPGroupItem(int idpGroupItemId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "DELETE FROM IDP_GROUP_ITEM WHERE IDP_GROUP_ITEM_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", idpGroupItemId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void UpdateIDPGroupItems(Dictionary<string, IDPGroupItem> idpGroupItems)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var kvp in idpGroupItems)
                {
                    var idpGroupItemId = kvp.Key;
                    var idpGroupItem = kvp.Value;

                    string updateQuery = "UPDATE IDP_GROUP_ITEM SET PL = @Pl, CRITICAL = @Cri WHERE IDP_GROUP_ITEM_ID = @Id";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Pl", idpGroupItem.Pl);
                        command.Parameters.AddWithValue("@Cri", idpGroupItem.Critical);
                        command.Parameters.AddWithValue("@Id", idpGroupItemId);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public List<string> GetIdsThatEnrollByIDPGroupId(string idpGroupId)
        {
            List<string> allIdsInEnroll = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID FROM IDP_USER_ENROLL WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = (string)reader["ID"];
                            allIdsInEnroll.Add(id);
                        }
                    }
                }
            }

            return allIdsInEnroll;
        }
        public int GetCountCompetencyThisId(string idpGroupId)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_GROUP_ITEM WHERE IDP_GROUP_ID = @IDPGroupId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }
        public int GetCountCompetencyOtherId(string idpGroupId, string year, string id)
        {

            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) AS OTHER " +
                               "FROM IDP_USER_ENROLL EN " +
                               "JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                               "JOIN IDP_GROUP_ITEM GI ON G.IDP_GROUP_ID = GI.IDP_GROUP_ID " +
                               "WHERE EN.ID = @id AND G.YEAR = @Year AND EN.IDP_GROUP_ID != @IDPGroupId " +
                               "GROUP BY EN.ID";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);
                command.Parameters.AddWithValue("@Year", year);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
            
        }


        //Employee
        public List<User> GetUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM MAS_USER_HR HR LEFT JOIN IDP_RESULT R ON HR.ID = R.ID AND R.YEAR = YEAR(GETDATE()) + 543";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User user = new User();

                    user.Id = (string)reader["ID"];
                    user.Prefix = reader.IsDBNull(reader.GetOrdinal("PREFIX")) ? null : (string)reader["PREFIX"];
                    user.FirstNameTH = reader.IsDBNull(reader.GetOrdinal("FIRSTNAME_TH")) ? null : (string)reader["FIRSTNAME_TH"];
                    user.LastNameTH = reader.IsDBNull(reader.GetOrdinal("LASTNAME_TH")) ? null : (string)reader["LASTNAME_TH"];
                    user.FirstNameEN = reader.IsDBNull(reader.GetOrdinal("FIRSTNAME_EN")) ? null : (string)reader["FIRSTNAME_EN"];
                    user.LastNameEN = reader.IsDBNull(reader.GetOrdinal("LASTNAME_EN")) ? null : (string)reader["LASTNAME_EN"];
                    user.Status = reader.IsDBNull(reader.GetOrdinal("STATUS")) ? null : (string)reader["STATUS"];
                    user.StatusDate = reader.IsDBNull(reader.GetOrdinal("STATUS_DATE")) ? null : (string)reader["STATUS_DATE"];
                    user.Company = reader.IsDBNull(reader.GetOrdinal("COMPANY")) ? null : (string)reader["COMPANY"];
                    user.Location = reader.IsDBNull(reader.GetOrdinal("LOCATION")) ? null : (string)reader["LOCATION"];
                    user.Position = reader.IsDBNull(reader.GetOrdinal("POSITION")) ? null : (string)reader["POSITION"];
                    user.JobLevel = reader.IsDBNull(reader.GetOrdinal("JOBLEVEL")) ? null : (string)reader["JOBLEVEL"];
                    user.CostCenter = reader.IsDBNull(reader.GetOrdinal("COSTCENTER")) ? null : (string)reader["COSTCENTER"];
                    user.Department = reader.IsDBNull(reader.GetOrdinal("DEPARTMENT")) ? null : (string)reader["DEPARTMENT"];
                    user.DepartmentName = reader.IsDBNull(reader.GetOrdinal("DEPARTMENT_NAME")) ? null : (string)reader["DEPARTMENT_NAME"];
                    user.Email = reader.IsDBNull(reader.GetOrdinal("EMAIL")) ? null : (string)reader["EMAIL"];
                    user.UserLogin = reader.IsDBNull(reader.GetOrdinal("USER_LOGIN")) ? null : (string)reader["USER_LOGIN"];
                    user.Enabled = reader.IsDBNull(reader.GetOrdinal("Enabled")) ? null : (string)reader["Enabled"];
                    user.ShiftWork = reader.IsDBNull(reader.GetOrdinal("SHIFTWORK")) ? null : (string)reader["SHIFTWORK"];
                    user.WorkCenter = reader.IsDBNull(reader.GetOrdinal("WORK_CENTER")) ? null : (string)reader["WORK_CENTER"];
                    user.HRPositionCode = reader.IsDBNull(reader.GetOrdinal("HRPositionCode")) ? null : (string)reader["HRPositionCode"];
                    user.JobRole = reader.IsDBNull(reader.GetOrdinal("JobRole")) ? null : (string)reader["JobRole"];
                    user.WorkAge = reader["WorkAge"].ToString();
                    user.StartWorkDate = reader.IsDBNull(reader.GetOrdinal("StartWorkDate")) ? null : (string)reader["StartWorkDate"];

                    Result result = new Result();
                    result.GUID = reader.IsDBNull(reader.GetOrdinal("GUID")) ? null : (string)reader["GUID"];

                    user.Result = result;

                    users.Add(user);
                }
                reader.Close();
            }

            return users;
        }
        public void DeleteEmployee(string id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DeleteEnrollById(id);

                string query = "DELETE FROM MAS_USER_HR WHERE ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private void DeleteEnrollById(string id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "DELETE FROM IDP_USER_ENROLL WHERE ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<Enrollment> GetIDPGroupByEmployee(string id)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT EN.ENROLL_ID, EN.IDP_GROUP_ID, G.IDP_GROUP_NAME, G.YEAR " +
                                        "FROM IDP_USER_ENROLL AS EN JOIN IDP_GROUP AS G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                                        "WHERE EN.ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enrollment enrollment = new Enrollment();
                        enrollment.EnrollId = (int)reader["ENROLL_ID"];
                        enrollment.IDPGroupId = (string)reader["IDP_GROUP_ID"];

                        IDPGroup iDPGroup = new IDPGroup();
                        iDPGroup.IDPGroupName = (string)reader["IDP_GROUP_NAME"];
                        iDPGroup.Year = (string)reader["YEAR"];

                        enrollment.IDPGroup = iDPGroup;

                        enrollments.Add(enrollment);
                    }
                }
            }

            return enrollments;
        }
        public List<string> GetCheckedIDPGroup(string id)
        {
            List<string> CheckedIDPGroupIds = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT IDP_GROUP_ID FROM IDP_USER_ENROLL WHERE ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string idpGroupId = reader.GetString(reader.GetOrdinal("IDP_GROUP_ID"));
                            CheckedIDPGroupIds.Add(idpGroupId);
                        }
                    }
                }
            }

            return CheckedIDPGroupIds;
        }
        public void InsertIDPGroup(List<IDPGroup> selectedIDPGroups, string id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (IDPGroup iDPGroup in selectedIDPGroups)
                {
                    string query = "INSERT INTO IDP_USER_ENROLL (IDP_GROUP_ID, ID, COMPETENCY_ALL, COMPETENCY_PASS, COMPETENCY_PER) VALUES " +
                        "(@IDPGroupId, @Id, 0, 0, 0)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@IDPGroupId", iDPGroup.IDPGroupId);
                        command.Parameters.AddWithValue("@Id", id);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public string GetIdByEnrollment(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT ID FROM IDP_USER_ENROLL WHERE ENROLL_ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                return (string)command.ExecuteScalar();
            }
        }
        public void DeleteIDPGroupByEmployee(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "DELETE FROM IDP_USER_ENROLL WHERE ENROLL_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


        //Form
        public List<IDPGroup> SelectIDPGroup()
        {
            List<IDPGroup> iDPGroups = new List<IDPGroup>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT F.IDP_GROUP_ID, F.IDP_GROUP_NAME, F.YEAR, HR.PREFIX, HR.FIRSTNAME_TH, HR.LASTNAME_TH, HR.JOBLEVEL, HR.EMAIL " +
                                "FROM IDP_GROUP F, MAS_USER_HR HR";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    IDPGroup iDPGroup = new IDPGroup();

                    iDPGroup.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                    iDPGroup.IDPGroupName = reader.IsDBNull(reader.GetOrdinal("IDP_GROUP_NAME")) ? null : (string)reader["IDP_GROUP_NAME"];
                    iDPGroup.Year = reader.IsDBNull(reader.GetOrdinal("YEAR")) ? null : (string)reader["YEAR"];

                    User user = new User();
                    user.Prefix = reader.IsDBNull(reader.GetOrdinal("PREFIX")) ? null : (string)reader["PREFIX"];
                    user.FirstNameTH = reader.IsDBNull(reader.GetOrdinal("FIRSTNAME_TH")) ? null : (string)reader["FIRSTNAME_TH"];
                    user.LastNameTH = reader.IsDBNull(reader.GetOrdinal("LASTNAME_TH")) ? null : (string)reader["LASTNAME_TH"];
                    user.JobLevel = reader.IsDBNull(reader.GetOrdinal("JOBLEVEL")) ? null : (string)reader["JOBLEVEL"];
                    user.Email = reader.IsDBNull(reader.GetOrdinal("EMAIL")) ? null : (string)reader["EMAIL"];


                    iDPGroup.User = user;

                    iDPGroups.Add(iDPGroup);
                }
                reader.Close();
            }

            return iDPGroups;
        }
        public List<Enrollment> GetEnrollEachYearById(string guid, string year)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * , " +
                                    "(SELECT COUNT(*) FROM IDP_GROUP_ITEM GI WHERE G.IDP_GROUP_ID = GI.IDP_GROUP_ID) AS competencies " +
                                    "FROM IDP_USER_ENROLL EN " +
                                    "JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID AND G.YEAR = @Year " +
                                    "JOIN IDP_RESULT R ON EN.ID = R.ID AND R.YEAR = @Year " +
                                    "WHERE EN.STATUS != 'Draft' AND R.GUID = @Guid";

                command.Parameters.AddWithValue("@Year", year);
                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enrollment enrollment = new Enrollment();
                        enrollment.EnrollId = (int)reader["ENROLL_ID"];
                        enrollment.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                        enrollment.Competencies = (int)reader["competencies"];
                        enrollment.Status= (string)reader["STATUS"];
                        //enrollment.Finish = (bool)reader["FINISH"];

                        IDPGroup iDPGroup = new IDPGroup();
                        iDPGroup.IDPGroupName = (string)reader["IDP_GROUP_NAME"];

                        enrollment.IDPGroup = iDPGroup;

                        enrollments.Add(enrollment);
                    }
                }
            }

            return enrollments;
        }
        public List<Enrollment> GetFormsByGuid(int EnrollmentId, string id, string guid)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT EN.ENROLL_ID, H.ID, HR.PREFIX, HR.FIRSTNAME_TH, HR.LASTNAME_TH, EN.IDP_GROUP_ID, G.IDP_GROUP_NAME, G.YEAR, I.COMPETENCY_ID, C.COMPETENCY_NAME_TH, I.PL, " +
                    "I.CRITICAL, F.GUID, F.RESULT_ITEM, F.REQUIREMENT, F.ACTUAL, F.GAP, F.PRIORITY, F.[PLAN], F.PLAN_DESC, F.Q1, F.Q2, F.Q3, F.Q4, F.RST_PLAN, C.PL1, C.PL2, C.PL3, C.PL4, C.PL5 " +
                    "FROM IDP_USER_ENROLL EN " +
                    "LEFT JOIN MAS_USER_HR HR ON EN.ID = HR.ID " +
                    "LEFT JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                    "LEFT JOIN IDP_GROUP_ITEM I ON I.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                    "LEFT JOIN IDP_COMPTY C ON I.COMPETENCY_ID = C.COMPETENCY_ID " +
                    "RIGHT JOIN IDP_RESULT H ON EN.ID = H.ID " +
                    "LEFT JOIN IDP_RESULT_ITEM F ON C.COMPETENCY_ID = F.COMPETENCY_ID AND H.GUID = F.GUID " +
                    "WHERE EN.ENROLL_ID = @EnrollmentId AND H.ID = @Id AND H.GUID = @Guid";

                command.Parameters.AddWithValue("@EnrollmentId", EnrollmentId);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enrollment enrollment = new Enrollment();
                        enrollment.EnrollId = (int)reader["ENROLL_ID"];
                        enrollment.IDPGroupId = (string)reader["IDP_GROUP_ID"];

                        User user = new User();
                        user.Id = (string)reader["ID"];

                        user.FirstNameTH = (string)reader["FIRSTNAME_TH"];
                        user.LastNameTH = (string)reader["LASTNAME_TH"];

                        IDPGroup iDPGroup = new IDPGroup();
                        iDPGroup.IDPGroupName = (string)reader["IDP_GROUP_NAME"];
                        iDPGroup.Year = (string)reader["YEAR"];

                        IDPGroupItem idpGroupItem = new IDPGroupItem();
                        idpGroupItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                        idpGroupItem.Pl = reader.IsDBNull(reader.GetOrdinal("PL")) ? null : (string)reader["PL"];
                        idpGroupItem.Critical = reader.IsDBNull(reader.GetOrdinal("CRITICAL")) ? false : (bool)reader["CRITICAL"];

                        Competency competency = new Competency();
                        competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];
                        competency.Pl1 = reader.IsDBNull(reader.GetOrdinal("PL1")) ? null : (string)reader["PL1"];
                        competency.Pl2 = reader.IsDBNull(reader.GetOrdinal("PL2")) ? null : (string)reader["PL2"];
                        competency.Pl3 = reader.IsDBNull(reader.GetOrdinal("PL3")) ? null : (string)reader["PL3"];
                        competency.Pl4 = reader.IsDBNull(reader.GetOrdinal("PL4")) ? null : (string)reader["PL4"];
                        competency.Pl5 = reader.IsDBNull(reader.GetOrdinal("PL5")) ? null : (string)reader["PL5"];

                        ResultItem resultItem = new ResultItem();
                        resultItem.Requirement = reader.IsDBNull(reader.GetOrdinal("REQUIREMENT")) ? 0 : (int)reader["REQUIREMENT"];
                        resultItem.Actual = reader.IsDBNull(reader.GetOrdinal("ACTUAL")) ? 0 : (int)reader["ACTUAL"];
                        resultItem.Gap = reader.IsDBNull(reader.GetOrdinal("GAP")) ? 0 : (int)reader["GAP"];
                        resultItem.Priority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                        resultItem.Plan = reader.IsDBNull(reader.GetOrdinal("PLAN")) ? null : (string)reader["PLAN"];
                        resultItem.PlanDesc = reader.IsDBNull(reader.GetOrdinal("PLAN_DESC")) ? null : (string)reader["PLAN_DESC"];
                        resultItem.Q1 = reader.IsDBNull(reader.GetOrdinal("Q1")) ? null : (string)reader["Q1"];
                        resultItem.Q2 = reader.IsDBNull(reader.GetOrdinal("Q2")) ? null : (string)reader["Q2"];
                        resultItem.Q3 = reader.IsDBNull(reader.GetOrdinal("Q3")) ? null : (string)reader["Q3"];
                        resultItem.Q4 = reader.IsDBNull(reader.GetOrdinal("Q4")) ? null : (string)reader["Q4"];
                        resultItem.RstPlan = reader.IsDBNull(reader.GetOrdinal("RST_PLAN")) ? null : (string)reader["RST_PLAN"];

                        enrollment.User = user;
                        enrollment.IDPGroup = iDPGroup;
                        enrollment.IDPGroupItem = idpGroupItem;
                        enrollment.Competency = competency;
                        enrollment.ResultItem = resultItem;

                        enrollments.Add(enrollment);
                    }
                }
            }

            return enrollments;
        }
        public void InsertResultDetails(IEnumerable<ResultItem> resultItems, string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO IDP_RESULT_ITEM (GUID, IDP_GROUP_ID, COMPETENCY_ID, REQUIREMENT, ACTUAL, GAP, PRIORITY, [PLAN], PLAN_DESC, Q1, Q2, Q3, Q4, RST_PLAN) VALUES" +
                    " (@Guid, @IDPGroupId, @CompetencyId, @Requir, @Actual, @Gap, @Priority, @Plan, @PlanDesc, @Q1, @Q2, @Q3, @Q4, @RstPlan)";

                connection.Open();

                foreach (var resultItem in resultItems)
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Guid", guid);
                        command.Parameters.AddWithValue("@IDPGroupId", resultItem.IDPGroupId);
                        command.Parameters.AddWithValue("@CompetencyId", resultItem.CompetencyId);
                        command.Parameters.AddWithValue("@Requir", resultItem.Requirement);
                        command.Parameters.AddWithValue("@Actual", resultItem.Actual);
                        command.Parameters.AddWithValue("@Gap", resultItem.Actual - resultItem.Requirement);
                        command.Parameters.AddWithValue("@Priority", (object)resultItem.Priority ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Plan", (object)resultItem.Plan ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PlanDesc", (object)resultItem.PlanDesc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Q1", (object)resultItem.Q1 ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Q2", (object)resultItem.Q2 ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Q3", (object)resultItem.Q3 ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Q4", (object)resultItem.Q4 ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RstPlan", (object)resultItem.RstPlan ?? DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateResultDetails(IEnumerable<ResultItem> resultItems, string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_RESULT_ITEM SET REQUIREMENT = @Requir, ACTUAL = @Actual, GAP = @Gap, PRIORITY = @Priority, [PLAN] = @Plan, PLAN_DESC = @PlanDesc, " +
                                    "Q1 = @Q1, Q2 = @Q2, Q3 = @Q3, Q4 = @Q4, RST_PLAN = @RstPlan " +
                                    "WHERE GUID = @Guid AND COMPETENCY_ID = @CompetencyId AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    foreach (ResultItem resultItem in resultItems)
                    {
                        updateCommand.Parameters.Clear();
                        updateCommand.Parameters.AddWithValue("@Guid", guid);
                        updateCommand.Parameters.AddWithValue("@CompetencyId", resultItem.CompetencyId);
                        updateCommand.Parameters.AddWithValue("@IDPGroupId", resultItem.IDPGroupId);
                        updateCommand.Parameters.AddWithValue("@Requir", resultItem.Requirement);
                        updateCommand.Parameters.AddWithValue("@Actual", resultItem.Actual);
                        updateCommand.Parameters.AddWithValue("@Gap", resultItem.Gap = resultItem.Actual - resultItem.Requirement);
                        updateCommand.Parameters.AddWithValue("@Priority", resultItem.Priority ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Plan", resultItem.Plan ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@PlanDesc", resultItem.PlanDesc ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Q1", (object)resultItem.Q1 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Q2", (object)resultItem.Q2 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Q3", (object)resultItem.Q3 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Q4", (object)resultItem.Q4 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@RstPlan", resultItem.RstPlan ?? (object)DBNull.Value);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public bool IsFormSubmitted(string Id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) " +
                    "FROM IDP_RESULT_ITEM RI " +
                    "JOIN IDP_RESULT H ON RI.GUID = H.GUID " +
                    "WHERE RI.IDP_GROUP_ID = @IDPGroupId AND H.ID = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);
                    command.Parameters.AddWithValue("@Id", Id);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
        public string GetPrefixById(string id)
        {
            string prefix = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT PREFIX FROM MAS_USER_HR WHERE ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    prefix = result.ToString();
                }
            }

            return prefix;
        }
        public string GetFirstNameById(string id)
        {
            string firstName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT FIRSTNAME_TH FROM MAS_USER_HR WHERE ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    firstName = result.ToString();
                }
            }

            return firstName;
        }
        public string GetLastNameById(string id)
        {
            string lastName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT LASTNAME_TH FROM MAS_USER_HR WHERE ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    lastName = result.ToString();
                }
            }

            return lastName;
        }
        public string GetCompanyById(string id)
        {
            string company = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT COMPANY FROM MAS_USER_HR WHERE ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    company = result.ToString();
                }
            }

            return company;
        }   
        public string GetJoblevelById(string id)
        {
            string joblevel = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT JOBLEVEL FROM MAS_USER_HR WHERE ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    joblevel = result.ToString();
                }
            }

            return joblevel;
        }
        public string GetDepartmentById(string id)
        {
            string department = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT DEPARTMENT_NAME FROM MAS_USER_HR WHERE ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    department = result.ToString();
                }
            }

            return department;
        }
        public string GetPositionById(string id)
        {
            string position = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT POSITION FROM MAS_USER_HR WHERE ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    position = result.ToString();
                }
            }

            return position;
        }
        public List<Enrollment> GetInfoEmployee(string id, string guid, string year)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT EN.ENROLL_ID, EN.ID, EN.IDP_GROUP_ID, G.IDP_GROUP_NAME, G.YEAR, I.COMPETENCY_ID, C.COMPETENCY_NAME_TH, I.PL, " +
                                        "I.CRITICAL, F.RESULT_ITEM AS ENROLL_DETAIL, F.REQUIREMENT, F.GUID, F.ACTUAL, F.GAP, F.PRIORITY, F.[PLAN], F.PLAN_DESC, " +
                                        "F.Q1, F.Q2, F.Q3, F.Q4, F.RST_PLAN, EN.STATUS " +
                                        "FROM IDP_USER_ENROLL EN " +
                                        "LEFT JOIN MAS_USER_HR HR ON EN.ID = HR.ID " +
                                        "LEFT JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                                        "LEFT JOIN IDP_GROUP_ITEM I ON I.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                                        "LEFT JOIN IDP_COMPTY C ON I.COMPETENCY_ID = C.COMPETENCY_ID " +
                                        "LEFT JOIN IDP_RESULT H ON H.ID = EN.ID " +
                                        "LEFT JOIN IDP_RESULT_ITEM F ON C.COMPETENCY_ID = F.COMPETENCY_ID AND H.GUID = F.GUID " +
                                        "WHERE EN.ID = @Id AND G.YEAR = @Year AND H.GUID = @GUID";

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@GUID", guid);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enrollment enrollment = new Enrollment();
                        enrollment.EnrollId = (int)reader["ENROLL_ID"];
                        enrollment.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                        enrollment.Id = (string)reader["ID"];
                        //enrollment.Finish = (bool)reader["FINISH"];

                        IDPGroup iDPGroup = new IDPGroup();
                        iDPGroup.IDPGroupName = (string)reader["IDP_GROUP_NAME"];
                        iDPGroup.Year = (string)reader["YEAR"];

                        IDPGroupItem idpGroupItem = new IDPGroupItem();
                        idpGroupItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                        idpGroupItem.Pl = reader.IsDBNull(reader.GetOrdinal("PL")) ? null : (string)reader["PL"];
                        idpGroupItem.Critical = reader.IsDBNull(reader.GetOrdinal("CRITICAL")) ? false : (bool)reader["CRITICAL"];

                        Competency competency = new Competency();
                        competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];

                        ResultItem resultItem = new ResultItem();
                        resultItem.Requirement = reader.IsDBNull(reader.GetOrdinal("REQUIREMENT")) ? 0 : (int)reader["REQUIREMENT"];
                        resultItem.Actual = reader.IsDBNull(reader.GetOrdinal("ACTUAL")) ? 0 : (int)reader["ACTUAL"];
                        resultItem.Gap = reader.IsDBNull(reader.GetOrdinal("GAP")) ? 0 : (int)reader["GAP"];
                        resultItem.Priority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                        resultItem.Plan = reader.IsDBNull(reader.GetOrdinal("PLAN")) ? null : (string)reader["PLAN"];
                        resultItem.PlanDesc = reader.IsDBNull(reader.GetOrdinal("PLAN_DESC")) ? null : (string)reader["PLAN_DESC"];
                        resultItem.Q1 = reader.IsDBNull(reader.GetOrdinal("Q1")) ? null : (string)reader["Q1"];
                        resultItem.Q2 = reader.IsDBNull(reader.GetOrdinal("Q2")) ? null : (string)reader["Q2"];
                        resultItem.Q3 = reader.IsDBNull(reader.GetOrdinal("Q3")) ? null : (string)reader["Q3"];
                        resultItem.Q4 = reader.IsDBNull(reader.GetOrdinal("Q4")) ? null : (string)reader["Q4"];
                        resultItem.RstPlan = reader.IsDBNull(reader.GetOrdinal("RST_PLAN")) ? null : (string)reader["RST_PLAN"];

                        enrollment.IDPGroup = iDPGroup;
                        enrollment.IDPGroupItem = idpGroupItem;
                        enrollment.Competency = competency;
                        enrollment.ResultItem = resultItem;

                        enrollments.Add(enrollment);
                    }
                }
            }

            return enrollments;
        }
        public int GetCountEnrollmentById(string id)
        {
            int enrolled = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM IDP_USER_ENROLL WHERE ID = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int count))
                {
                    enrolled = count;
                }
            }

            return enrolled;
        }
        public string GetIDPGroupNameById(string id, int enrollId)
        {
            string idpGroupName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT IDP_GROUP_NAME " +
                                    "FROM IDP_GROUP G JOIN IDP_USER_ENROLL EN ON G.IDP_GROUP_ID = EN.IDP_GROUP_ID " +
                                    "WHERE EN.ID = @Id AND EN.ENROLL_ID = @EnrollId";
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@EnrollId", enrollId);

                connection.Open();

                // Assuming course_name is stored as a string column in the "Courses" table
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    idpGroupName = result.ToString();
                }
            }

            return idpGroupName;
        }
        public int GetCompetencyPassByGuid(string guid)
        {
            int enrolled = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) " +
                "FROM IDP_RESULT R " +
                "JOIN IDP_RESULT_ITEM RI ON R.GUID = RI.GUID " +
                "WHERE R.GUID = @GUID AND RI.GAP >= 0", connection))
            {
                command.Parameters.AddWithValue("@GUID", guid);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int count))
                {
                    enrolled = count;
                }
            }

            return enrolled;
        }
        public void UpdateResult(string guid, int did, int pass, float per, string rank)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_DID = @Did, COMPETENCY_PASS = @Pass, COMPETENCY_PER = @Per, RANK = @Rank WHERE GUID = @GUID";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {


                    updateCommand.Parameters.AddWithValue("@GUID", guid);
                    updateCommand.Parameters.AddWithValue("@Per", per);
                    updateCommand.Parameters.AddWithValue("@Pass", pass);
                    updateCommand.Parameters.AddWithValue("@Rank", rank);
                    updateCommand.Parameters.AddWithValue("@Did", did);
                    //updateCommand.Parameters.AddWithValue("@Finish", true);

                    updateCommand.ExecuteNonQuery();

                }
            }
        }


        //Enrollment
        public List<Enrollment> GetEnrollments(string idpGroupId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT en.ENROLL_ID, en.ID, en.IDP_GROUP_ID, en.STATUS, hr.PREFIX, hr.FIRSTNAME_TH, hr.LASTNAME_TH, hr.POSITION, hr.DEPARTMENT_NAME, hr.JOBLEVEL, hr.COMPANY " +
                                      "FROM IDP_USER_ENROLL AS en JOIN MAS_USER_HR AS hr ON en.ID = hr.ID " +
                                      "WHERE en.IDP_GROUP_ID = @IDPGroupId";
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enrollment enrollment = new Enrollment();
                        enrollment.EnrollId = (int)reader["ENROLL_ID"];
                        enrollment.Id = (string)reader["ID"];
                        enrollment.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                        enrollment.Status = (string)reader["STATUS"];

                        User user = new User();
                        user.Prefix = reader["PREFIX"] != DBNull.Value ? (string)reader["PREFIX"] : null;
                        user.FirstNameTH = (string)reader["FIRSTNAME_TH"];
                        user.LastNameTH = (string)reader["LASTNAME_TH"];
                        user.Position = reader["POSITION"] != DBNull.Value ? (string)reader["POSITION"] : null;
                        user.DepartmentName = reader["DEPARTMENT_NAME"] != DBNull.Value ? (string)reader["DEPARTMENT_NAME"] : null;
                        user.JobLevel = reader["JOBLEVEL"] != DBNull.Value ? (string)reader["JOBLEVEL"] : null;
                        user.Company = reader["COMPANY"] != DBNull.Value ? (string)reader["COMPANY"] : null;

                        enrollment.User = user;

                        enrollments.Add(enrollment);
                    }
                }
            }

            return enrollments;
        }
        public List<User> GetEmployeeAtActive()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM MAS_USER_HR WHERE STATUS = 'ทำงาน'";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User user = new User();

                    user.Id = (string)reader["ID"];
                    user.Prefix = reader.IsDBNull(reader.GetOrdinal("PREFIX")) ? null : (string)reader["PREFIX"];
                    user.FirstNameTH = reader.IsDBNull(reader.GetOrdinal("FIRSTNAME_TH")) ? null : (string)reader["FIRSTNAME_TH"];
                    user.LastNameTH = reader.IsDBNull(reader.GetOrdinal("LASTNAME_TH")) ? null : (string)reader["LASTNAME_TH"];
                    user.FirstNameEN = reader.IsDBNull(reader.GetOrdinal("FIRSTNAME_EN")) ? null : (string)reader["FIRSTNAME_EN"];
                    user.LastNameEN = reader.IsDBNull(reader.GetOrdinal("LASTNAME_EN")) ? null : (string)reader["LASTNAME_EN"];
                    user.Status = reader.IsDBNull(reader.GetOrdinal("STATUS")) ? null : (string)reader["STATUS"];
                    user.StatusDate = reader.IsDBNull(reader.GetOrdinal("STATUS_DATE")) ? null : (string)reader["STATUS_DATE"];
                    user.Company = reader.IsDBNull(reader.GetOrdinal("COMPANY")) ? null : (string)reader["COMPANY"];
                    user.Location = reader.IsDBNull(reader.GetOrdinal("LOCATION")) ? null : (string)reader["LOCATION"];
                    user.Position = reader.IsDBNull(reader.GetOrdinal("POSITION")) ? null : (string)reader["POSITION"];
                    user.JobLevel = reader.IsDBNull(reader.GetOrdinal("JOBLEVEL")) ? null : (string)reader["JOBLEVEL"];
                    user.CostCenter = reader.IsDBNull(reader.GetOrdinal("COSTCENTER")) ? null : (string)reader["COSTCENTER"];
                    user.Department = reader.IsDBNull(reader.GetOrdinal("DEPARTMENT")) ? null : (string)reader["DEPARTMENT"];
                    user.DepartmentName = reader.IsDBNull(reader.GetOrdinal("DEPARTMENT_NAME")) ? null : (string)reader["DEPARTMENT_NAME"];
                    user.Email = reader.IsDBNull(reader.GetOrdinal("EMAIL")) ? null : (string)reader["EMAIL"];
                    user.UserLogin = reader.IsDBNull(reader.GetOrdinal("USER_LOGIN")) ? null : (string)reader["USER_LOGIN"];
                    user.Enabled = reader.IsDBNull(reader.GetOrdinal("Enabled")) ? null : (string)reader["Enabled"];
                    user.ShiftWork = reader.IsDBNull(reader.GetOrdinal("SHIFTWORK")) ? null : (string)reader["SHIFTWORK"];
                    user.WorkCenter = reader.IsDBNull(reader.GetOrdinal("WORK_CENTER")) ? null : (string)reader["WORK_CENTER"];
                    user.HRPositionCode = reader.IsDBNull(reader.GetOrdinal("HRPositionCode")) ? null : (string)reader["HRPositionCode"];
                    user.JobRole = reader.IsDBNull(reader.GetOrdinal("JobRole")) ? null : (string)reader["JobRole"];
                    user.WorkAge = reader["WorkAge"].ToString();
                    user.StartWorkDate = reader.IsDBNull(reader.GetOrdinal("StartWorkDate")) ? null : (string)reader["StartWorkDate"];

                    users.Add(user);
                }
                reader.Close();
            }

            return users;
        }
        public List<string> GetCheckedId(string idpGroupId)
        {
            List<string> CheckedIds = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID FROM IDP_USER_ENROLL WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string Id = (string)reader["ID"];
                            CheckedIds.Add(Id);
                        }
                    }
                }
            }

            return CheckedIds;
        }
        public void InsertEmployee(List<User> selectedUsers, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (User user in selectedUsers)
                {
                    string query = "INSERT INTO IDP_USER_ENROLL (IDP_GROUP_ID, ID, STATUS) " +
                                    "VALUES (@IDPGroupId, @Id, 'Draft')";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);
                        command.Parameters.AddWithValue("@Id", user.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public int GetCompetencyAllById(string id, string year)
        {
            int all = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) " +
                                                        "FROM IDP_GROUP_ITEM I " +
                                                        "JOIN IDP_GROUP G ON G.IDP_GROUP_ID = I.IDP_GROUP_ID " +
                                                        "JOIN IDP_USER_ENROLL EN ON G.IDP_GROUP_ID = EN.IDP_GROUP_ID " +
                                                        "WHERE EN.ID = @Id AND YEAR = @Year", connection))
            {

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                all = (int)command.ExecuteScalar();
            }

            return all;
        }
        public int GetCompetencyAllByStatus(string id, string year)
        {
            int all = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) " +
                                                        "FROM IDP_GROUP_ITEM I " +
                                                        "JOIN IDP_GROUP G ON G.IDP_GROUP_ID = I.IDP_GROUP_ID " +
                                                        "JOIN IDP_USER_ENROLL EN ON G.IDP_GROUP_ID = EN.IDP_GROUP_ID " +
                                                        "WHERE EN.ID = @Id AND YEAR = @Year AND EN.STATUS IN ('Submitted','Waiting','Success')", connection))
            {

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                all = (int)command.ExecuteScalar();
            }

            return all;
        }
        public int GetCompetencyAllByGuid(string guid)
        {
            int all = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COMPETENCY_ALL FROM IDP_RESULT WHERE GUID = @GUID", connection))
            {

                command.Parameters.AddWithValue("@GUID", guid);

                connection.Open();

                all = (int)command.ExecuteScalar();
            }

            return all;
        }
        public int GetCompetencyAllByIdAfterDelete(string id, string year)
        {
            int all = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) " +
                                                        "FROM IDP_GROUP_ITEM I " +
                                                        "JOIN IDP_GROUP G ON G.IDP_GROUP_ID = I.IDP_GROUP_ID " +
                                                        "JOIN IDP_USER_ENROLL EN ON G.IDP_GROUP_ID = EN.IDP_GROUP_ID " +
                                                        "WHERE EN.ID = @Id AND YEAR = @Year", connection))
            {

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                all = (int)command.ExecuteScalar();
            }

            return all;
        }
        public void InsertResultEmployees(List<User> selectedUsers, string year, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (User user in selectedUsers)
                {
                    int competencyAll = GetCompetencyAllById(user.Id, year); // Get competencyAll for each user

                    string resultQuery = "INSERT INTO IDP_RESULT (GUID, K2_NO, FORM_TYPE, FORM_ID, ID, SUBJECT, PLANT, " +
                                            "DEPARTMENT, COMPANY_CODE, REQUISITIONER, REQUISITIONER_EMAIL, " +
                                            "CREATED_BY, CREATED_ON, STARTEDWF_ON, COMPLETED_ON, CURRENT_APPROVER, " +
                                            "GR_LEVEL, YEAR, COMPETENCY_ALL, COMPETENCY_PASS, COMPETENCY_PER, RANK) " +
                                            "VALUES (@Guid, NULL, 'IDP', 'IDP01', @Id, @Subject, NULL, @Department, NULL, " +
                                            "NULL, NULL, @CreateBy, GETDATE(), NULL, NULL, NULL, NULL, @Year, @All, 0, 0, NULL)";

                    using (SqlCommand resultCommand = new SqlCommand(resultQuery, connection))
                    {
                        resultCommand.Parameters.AddWithValue("@Guid", Guid.NewGuid().ToString());
                        resultCommand.Parameters.AddWithValue("@Id", user.Id);
                        resultCommand.Parameters.AddWithValue("@Year", year);
                        resultCommand.Parameters.AddWithValue("@All", competencyAll);
                        resultCommand.Parameters.AddWithValue("@Subject", user.Prefix + user.FirstNameTH + user.LastNameTH);
                        resultCommand.Parameters.AddWithValue("@Department", user.Department);
                        resultCommand.Parameters.AddWithValue("@CreateBy", username);


                        resultCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateResultEmployees(List<User> selectedUsers, string year)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (User user in selectedUsers)
                {
                    int competencyAll = GetCompetencyAllById(user.Id, year); // Get updated competencyAll for each user

                    string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_ALL = @All WHERE ID = @Id AND YEAR = @Year";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@All", competencyAll);
                        updateCommand.Parameters.AddWithValue("@Id", user.Id);
                        updateCommand.Parameters.AddWithValue("@Year", year);

                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateResultEmployeeAfterDelete(string id, string year)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int all = GetCompetencyAllByIdAfterDelete(id, year);

                string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_ALL = @All WHERE ID = @Id AND YEAR = @Year";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@All", all);
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    updateCommand.Parameters.AddWithValue("@Year", year);

                    updateCommand.ExecuteNonQuery();
                }
                
            }
        }
        public void UpdateResultEmployeeAfterDeleteFromAddCompetency(int thisGroup, List<string> ids, string year, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (string id in ids)
                {
                    int otherGroup = 0;
                    if (GetCountIDPGroup(idpGroupId, year, id) > 0)
                    {
                        otherGroup = GetCountCompetencyOtherId(idpGroupId, year, id); 
                    }
                    string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_ALL = @ThisGroup + @OtherGroup WHERE ID = @Id AND YEAR = @Year";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@ThisGroup", thisGroup);
                        updateCommand.Parameters.AddWithValue("@OtherGroup", otherGroup);
                        updateCommand.Parameters.AddWithValue("@Id", id);
                        updateCommand.Parameters.AddWithValue("@Year", year);

                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public int GetCountIDPGroup(string idpGroupId, string year, string id)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) AS OTHER " +
                    "FROM IDP_USER_ENROLL EN " +
                    "JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                    "WHERE EN.ID = '11702' AND G.YEAR = '2566' AND EN.IDP_GROUP_ID != 'MC-Gp3'";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }
        public int GetCountEmployee(string idpGroupId)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_USER_ENROLL WHERE IDP_GROUP_ID = @IDPGroupId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }
        public string GetIDPGroupIdByEnrollment(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT IDP_GROUP_ID FROM IDP_USER_ENROLL WHERE ENROLL_ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                return (string)command.ExecuteScalar();
            }
        }
        public void UpdateEnrollmentStatus_1(string id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = 'In Progress' WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    updateCommand.ExecuteNonQuery();
                }

            }
        }
        public void UpdateEnrollmentStatus_2(string id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = 'Submitted' WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    updateCommand.ExecuteNonQuery();
                }

            }
        }
        public void DeleteEmployeeByIDPGroup(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "DELETE FROM IDP_USER_ENROLL WHERE ENROLL_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public bool IsAlreadyResultEachYear(List<User> selectedUsers, string year)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (User user in selectedUsers)
                {
                    string query = "SELECT COUNT(*) FROM IDP_RESULT WHERE ID = @Id AND YEAR = @Year";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Year", year);
                        command.Parameters.AddWithValue("@Id", user.Id);

                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            return true; 
                        }
                    }
                }

                return false; 
            }
        }
        public bool IsAlreadyResultEachYearByIds(List<string> ids, string year)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (string id in ids)
                {
                    string query = "SELECT COUNT(*) FROM IDP_RESULT WHERE ID = @Id AND YEAR = @Year";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Year", year);
                        command.Parameters.AddWithValue("@Id", id);

                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
        public void UpdateResultEmployeesById(List<string> ids, string year)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (string id in ids)
                {
                    int competencyAll = GetCompetencyAllById(id, year); // Get updated competencyAll for each user

                    string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_ALL = @All WHERE ID = @Id AND YEAR = @Year";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@All", competencyAll);
                        updateCommand.Parameters.AddWithValue("@Id", id);
                        updateCommand.Parameters.AddWithValue("@Year", year);

                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public string GetYearByEnrolled(int enrollId)
        {
            string year = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT YEAR FROM IDP_GROUP G JOIN IDP_USER_ENROLL EN ON G.IDP_GROUP_ID = EN.IDP_GROUP_ID " +
                                        "WHERE EN.ENROLL_ID = @EnrollId";
                command.Parameters.AddWithValue("@EnrollId", enrollId);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    year = result.ToString();
                }
            }

            return year;
        }
        public string GetGuidByIdAndYear(string id, string year)
        {
            string guid = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT H.GUID " +
                                        "FROM IDP_RESULT H " +
                                        "WHERE H.ID = @Id AND H.YEAR = @Year";
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    guid = result.ToString();
                }
            }

            return guid;
        }
        public string GetYearByGuid(string guid)
        {
            string year = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT YEAR FROM IDP_RESULT WHERE GUID = @Guid";

                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    year = result.ToString();
                }
            }

            return year;
        }
        public string GetIdByGuid(string guid)
        {
            string id = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT ID FROM IDP_RESULT WHERE GUID = @Guid";

                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    id = result.ToString();
                }
            }

            return id;
        }
        public int GetEnrollmentIdByIdAndIdpId(string id, string idpGroupId)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ENROLL_ID FROM IDP_USER_ENROLL WHERE IDP_GROUP_ID = @IDPGroupId AND ID = @Id";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }
    }
}   