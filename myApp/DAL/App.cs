﻿using DocumentFormat.OpenXml.Office2010.Excel;
using myApp.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
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

        public List<Competency> GetCompetencies()
        {
            List<Competency> competencies = new List<Competency>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM COMPTY";

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
                string query = "INSERT INTO COMPTY (COMPETENCY_ID, TYPE, COMPETENCY_NAME_TH, COMPETENCY_NAME_EN, COMPETENCY_DESC, PL1, PL2, PL3, PL4, PL5, ACTIVE) " +
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
                string query = "SELECT COUNT(*) FROM COMPTY WHERE COMPETENCY_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", competencyId);

                    connection.Open();

                    int existingCount = (int)command.ExecuteScalar();

                    return existingCount > 0;
                }
            }
        }
        public Competency EditCompetency(string competencyId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM COMPTY WHERE COMPETENCY_ID = @CompetencyId";

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
        public void UpdateCompetency(Competency competency)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"UPDATE COMPTY 
                         SET TYPE = @Type, 
                             COMPETENCY_NAME_TH = @TH, 
                             COMPETENCY_NAME_EN = @EN, 
                             COMPETENCY_DESC = @Desc, 
                             PL1 = @Pl1, 
                             PL2 = @Pl2, 
                             PL3 = @Pl3, 
                             PL4 = @Pl4, 
                             PL5 = @Pl5, 
                             ACTIVE = @Active 
                         WHERE COMPETENCY_ID = @Id";

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

                string query = "DELETE FROM COMPTY WHERE COMPETENCY_ID = @CompetencyId";

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

                string query = "DELETE FROM COMPTY_ITEM WHERE COMPETENCY_ID = @CompetencyId";

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
                command.CommandText = "SELECT COMPETENCY_NAME_TH FROM COMPTY WHERE COMPETENCY_ID = @CompetencyId";
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
                command.CommandText = "SELECT TYPE FROM COMPTY WHERE COMPETENCY_ID = @CompetencyId";
                command.Parameters.AddWithValue("@CompetencyId", id);

                connection.Open();

                // Assuming the "ACTIVE" column is stored as a boolean in the "COMPTY" table
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    type = result.ToString();
                }
            }

            return type;
        }


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
        public void CreateIDPGroup(IDPGroup idpGroupId)
        {
            if (IsDuplicateIDPGroupId(idpGroupId.IDPGroupId))
            {
                throw new Exception(" รหัสหลักสูตรนี้มีการใช้แล้ว กรุณากรอกใหม่");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO IDP_GROUP (IDP_GROUP_ID, IDP_GROUP_NAME, YEAR) " +
                                "VALUES (@Id, @Name, @Year)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", idpGroupId.IDPGroupId);
                    command.Parameters.AddWithValue("@Name", (object)idpGroupId.IDPGroupName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Year", (object)idpGroupId.Year ?? DBNull.Value);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }
        public IDPGroup EditIDPGroup(string idpGroupId)
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
        public void UpdateIDPGroup(IDPGroup idpGroup)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE IDP_GROUP SET IDP_GROUP_NAME = @Name, YEAR = @Year WHERE IDP_GROUP_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", idpGroup.IDPGroupId);
                    command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(idpGroup.IDPGroupName) ? DBNull.Value : (object)idpGroup.IDPGroupName);
                    command.Parameters.AddWithValue("@Year", string.IsNullOrEmpty(idpGroup.Year) ? DBNull.Value : (object)idpGroup.Year);

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
                string query = "DELETE FROM COMPTY_ITEM WHERE IDP_GROUP_ID = @IDPGroupId";

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
                string query = "DELETE FROM USER_ENROLL WHERE IDP_GROUP_ID = @IDPGroupId";

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
        public string GetIDPGroupNameById(string idpGroupId)
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
                    "LEFT JOIN COMPTY_ITEM I ON G.IDP_GROUP_ID = I.IDP_GROUP_ID " +
                    "LEFT JOIN COMPTY C ON C.COMPETENCY_ID = I.COMPETENCY_ID " +
                    "LEFT JOIN USER_ENROLL EN ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
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

                    CompetencyItem competencyItem = new CompetencyItem();
                    competencyItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                    competencyItem.Pl = reader.IsDBNull(reader.GetOrdinal("PL")) ? null : (string)reader["PL"]; 
                    competencyItem.Critical = reader.IsDBNull(reader.GetOrdinal("CRITICAL")) ? false : (bool)reader["CRITICAL"];

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
                    iDPGroup.CompetencyItem = competencyItem;
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
                string query = "SELECT COUNT(*) FROM COMPTY_ITEM WHERE IDP_GROUP_ID = @IDPGroupId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }

        
        public List<CompetencyItem> GetCompetencyItems(string idpGroupId)
        {
            List<CompetencyItem> competencyItems = new List<CompetencyItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT CIT.COMPETENCY_ITEM_ID, CIT.IDP_GROUP_ID, CIT.COMPETENCY_ID, C.COMPETENCY_NAME_TH, PL, CRITICAL, C.ACTIVE " +
                                        "FROM COMPTY_ITEM AS CIT JOIN COMPTY AS C ON CIT.COMPETENCY_ID = C.COMPETENCY_ID " +
                                        "WHERE CIT.IDP_GROUP_ID = @IDPGroupId";
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CompetencyItem competencyItem = new CompetencyItem();
                        competencyItem.CompetencyItemId = (int)reader["COMPETENCY_ITEM_ID"];
                        competencyItem.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                        competencyItem.CompetencyId = (string)reader["COMPETENCY_ID"];
                        competencyItem.Pl = (string)reader["PL"];
                        competencyItem.Critical = (bool)reader["CRITICAL"];

                        Competency competency = new Competency();
                        competency.CompetencyNameTH = (string)reader["COMPETENCY_NAME_TH"];
                        competency.Active = (bool)reader["Active"];

                        competencyItem.Competency = competency;

                        competencyItems.Add(competencyItem);
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
                string query = "SELECT * FROM COMPTY WHERE ACTIVE = 1";

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

                string query = "SELECT COMPETENCY_ID FROM COMPTY_ITEM WHERE IDP_GROUP_ID = @IDPGroupId";

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
                    string query = "INSERT INTO COMPTY_ITEM (COMPETENCY_ID, IDP_GROUP_ID, PL, CRITICAL) VALUES (@CompetencyId, @IDPGroupId, @Pl, @Cri)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompetencyId", competency.CompetencyId);
                        command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);
                        command.Parameters.AddWithValue("@Pl", competency.CompetencyItem.Pl);
                        
                        command.Parameters.AddWithValue("@Cri", competency.CompetencyItem.Critical);


                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public string GetIDPGroupIdByCompetencyItem(int competencyItemId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT IDP_GROUP_ID FROM COMPTY_ITEM WHERE COMPETENCY_ITEM_ID = @Id";
                command.Parameters.AddWithValue("@Id", competencyItemId);

                connection.Open();

                return (string)command.ExecuteScalar();
            }
        }
        public void DeleteCompetencyItem(int competencyItemId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "DELETE FROM COMPTY_ITEM WHERE COMPETENCY_ITEM_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", competencyItemId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void UpdateCompetencyItems(Dictionary<string, CompetencyItem> competencyItems)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var kvp in competencyItems)
                {
                    var competencyItemId = kvp.Key;
                    var competencyItem = kvp.Value;

                    string updateQuery = "UPDATE COMPTY_ITEM SET PL = @Pl, CRITICAL = @Cri WHERE COMPETENCY_ITEM_ID = @Id";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Pl", competencyItem.Pl);
                        command.Parameters.AddWithValue("@Cri", competencyItem.Critical);
                        command.Parameters.AddWithValue("@Id", competencyItemId);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        public List<User> GetUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM MAS_USER_HR";

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
        public void DeleteEmployee(String id)
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

                string query = "DELETE FROM USER_ENROLL WHERE ID = @Id";

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
                                        "FROM USER_ENROLL AS EN JOIN IDP_GROUP AS G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
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

                string query = "SELECT IDP_GROUP_ID FROM USER_ENROLL WHERE ID = @Id";

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
                    string query = "INSERT INTO USER_ENROLL (IDP_GROUP_ID, ID, COMPETENCY_ALL, COMPETENCY_PASS, COMPETENCY_PER, FINISH) VALUES " +
                        "(@IDPGroupId, @Id, 0, 0, 0, 0)";

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
                command.CommandText = "SELECT ID FROM USER_ENROLL WHERE ENROLL_ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                return (string)command.ExecuteScalar();
            }
        }
        public void DeleteIDPGroupByEmployee(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "DELETE FROM USER_ENROLL WHERE ENROLL_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


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
        public List<Enrollment> GetAllEnrollById(string id)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT EN.ENROLL_ID, EN.IDP_GROUP_ID, G.IDP_GROUP_NAME, CAST(EN.FINISH AS BIT) AS FINISH " +
                                      "FROM USER_ENROLL EN " +
                                      "JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                                      "WHERE EN.ID = @id";
                command.Parameters.AddWithValue("@id", id);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enrollment enrollment = new Enrollment();
                        enrollment.EnrollId = (int)reader["ENROLL_ID"];
                        enrollment.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                        enrollment.Finish = (bool)reader["FINISH"];

                        IDPGroup iDPGroup = new IDPGroup();
                        iDPGroup.IDPGroupName = (string)reader["IDP_GROUP_NAME"];

                        enrollment.IDPGroup = iDPGroup;

                        enrollments.Add(enrollment);
                    }
                }
            }

            return enrollments;
        }

        public int GetCountEnrolled(string id)
        {
            int enrolled = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM USER_ENROLL EN JOIN MAS_USER_HR HR ON EN.ID = HR.ID WHERE HR.ID = @Id", connection))
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
        public List<Enrollment> GetFormsById(int EnrollmentId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT EN.ENROLL_ID, HR.ID, HR.PREFIX, HR.FIRSTNAME_TH, HR.LASTNAME_TH, EN.IDP_GROUP_ID, G.IDP_GROUP_NAME, G.YEAR, I.COMPETENCY_ID, C.COMPETENCY_NAME_TH, I.PL, I.CRITICAL, F.FORM_ID, F.REQUIREMENT, F.ACTUAL, F.GAP, F.PRIORITY, F.[PLAN], F.PLAN_DESC, F.QUARTER, F.RST_PLAN " +
                    "FROM USER_ENROLL EN " +
                    "LEFT JOIN MAS_USER_HR HR ON EN.ID = HR.ID " +
                    "LEFT JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                    "LEFT JOIN COMPTY_ITEM I ON I.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                    "LEFT JOIN COMPTY C ON I.COMPETENCY_ID = C.COMPETENCY_ID " +
                    "LEFT JOIN FORM F ON HR.ID = F.ID AND C.COMPETENCY_ID = F.COMPETENCY_ID " +
                    "WHERE EN.ENROLL_ID = @EnrollmentId";
                command.Parameters.AddWithValue("@EnrollmentId", EnrollmentId);

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

                        CompetencyItem competencyItem = new CompetencyItem();
                        competencyItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                        competencyItem.Pl = reader.IsDBNull(reader.GetOrdinal("PL")) ? null : (string)reader["PL"];
                        competencyItem.Critical = reader.IsDBNull(reader.GetOrdinal("CRITICAL")) ? false : (bool)reader["CRITICAL"];

                        Competency competency = new Competency();
                        competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];

                        Form form = new Form();
                        form.Requirement = reader.IsDBNull(reader.GetOrdinal("REQUIREMENT")) ? 0 : (int)reader["REQUIREMENT"];
                        form.Actual = reader.IsDBNull(reader.GetOrdinal("ACTUAL")) ? 0 : (int)reader["ACTUAL"];
                        form.Gap = reader.IsDBNull(reader.GetOrdinal("GAP")) ? 0 : (int)reader["GAP"];
                        form.Priority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                        form.Plan = reader.IsDBNull(reader.GetOrdinal("PLAN")) ? null : (string)reader["PLAN"];
                        form.PlanDesc = reader.IsDBNull(reader.GetOrdinal("PLAN_DESC")) ? null : (string)reader["PLAN_DESC"];
                        form.Quarter = reader.IsDBNull(reader.GetOrdinal("QUARTER")) ? null : (string)reader["QUARTER"];
                        form.RstPlan = reader.IsDBNull(reader.GetOrdinal("RST_PLAN")) ? null : (string)reader["RST_PLAN"];

                        enrollment.User = user;
                        enrollment.IDPGroup = iDPGroup;
                        enrollment.CompetencyItem = competencyItem;
                        enrollment.Competency = competency;
                        enrollment.Form = form;

                        enrollments.Add(enrollment);
                    }
                }
            }

            return enrollments;
        }
        public void InsertResultDetails(IEnumerable<Form> forms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO FORM (IDP_GROUP_ID, ID, COMPETENCY_ID, REQUIREMENT, ACTUAL, GAP, PRIORITY, [PLAN], PLAN_DESC, QUARTER, RST_PLAN) VALUES" +
                    " (@IDPGroupId, @Id, @CompetencyId, @Requir, @Actual, @Gap, @Priority, @Plan, @PlanDesc, @Quarter, @RstPlan)";

                connection.Open();

                foreach (var form in forms)
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", form.Id);
                        command.Parameters.AddWithValue("@IDPGroupId", form.IDPGroupId);
                        command.Parameters.AddWithValue("@CompetencyId", form.CompetencyId);
                        command.Parameters.AddWithValue("@Requir", form.Requirement);
                        command.Parameters.AddWithValue("@Actual", form.Actual);
                        command.Parameters.AddWithValue("@Gap", form.Actual - form.Requirement);
                        command.Parameters.AddWithValue("@Priority", form.Priority ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Plan", form.Plan ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PlanDesc", form.PlanDesc ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Quarter", form.Quarter ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RstPlan", form.RstPlan ?? (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateResultDetails(IEnumerable<Form> forms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE FORM SET REQUIREMENT = @Requir, ACTUAL = @Actual, GAP = @Gap, PRIORITY = @Priority, [PLAN] = @Plan, PLAN_DESC = @PlanDesc, QUARTER = @Quarter, RST_PLAN = @RstPlan " +
                                     "WHERE ID = @Id AND COMPETENCY_ID = @CompetencyId AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    foreach (Form form in forms)
                    {
                        updateCommand.Parameters.Clear();
                        updateCommand.Parameters.AddWithValue("@Id", form.Id);
                        updateCommand.Parameters.AddWithValue("@CompetencyId", form.CompetencyId);
                        updateCommand.Parameters.AddWithValue("@IDPGroupId", form.IDPGroupId);
                        updateCommand.Parameters.AddWithValue("@Requir", form.Requirement);
                        updateCommand.Parameters.AddWithValue("@Actual", form.Actual);
                        updateCommand.Parameters.AddWithValue("@Gap", form.Gap = form.Actual - form.Requirement);
                        updateCommand.Parameters.AddWithValue("@Priority", form.Priority ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Plan", form.Plan ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@PlanDesc", form.PlanDesc ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Quarter", form.Quarter ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@RstPlan", form.RstPlan ?? (object)DBNull.Value);
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
                                "FROM FORM " +
                                "WHERE IDP_GROUP_ID = @IDPGroupId AND ID = @Id";
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
        public int GetCompetencyAllByEnrollId(string id)
        {
            int all = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM FORM WHERE ID = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                all = (int)command.ExecuteScalar();
            }

            return all;
        }
        public int GetCompetencyPassByEnrollId(string id)
        {
            int pass = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM FORM WHERE ID = @Id AND REQUIREMENT <= ACTUAL", connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                pass = (int)command.ExecuteScalar();
            }

            return pass;
        }
        public void UpdateEnrollFinish(int all, int pass, int per, int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE USER_ENROLL SET COMPETENCY_ALL = @All, COMPETENCY_PASS = @Pass, COMPETENCY_PER = @Per, FINISH = @Finish " +
                                     "WHERE ENROLL_ID = @EnrollId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    
                   
                    updateCommand.Parameters.AddWithValue("@EnrollId", enrollId);
                    updateCommand.Parameters.AddWithValue("@Per", per);
                    updateCommand.Parameters.AddWithValue("@Pass", pass);
                    updateCommand.Parameters.AddWithValue("@All", all);
                    updateCommand.Parameters.AddWithValue("@Finish", true);

                    updateCommand.ExecuteNonQuery();
                    
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
        public List<Enrollment> GetInfoEmployee(string id)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT EN.ENROLL_ID, EN.ID, EN.IDP_GROUP_ID, G.IDP_GROUP_NAME, G.YEAR, I.COMPETENCY_ID, C.COMPETENCY_NAME_TH, I.PL, " +
                    "I.CRITICAL, F.FORM_ID AS ENROLL_DETAIL, F.REQUIREMENT, " +
                    "F.ACTUAL, F.GAP, F.PRIORITY, F.[PLAN], F.PLAN_DESC, F.QUARTER, F.RST_PLAN, EN.COMPETENCY_ALL, EN.COMPETENCY_PASS, EN.COMPETENCY_PER, " +
                    "EN.FINISH " +
                    "FROM USER_ENROLL EN " +
                    "LEFT JOIN MAS_USER_HR HR ON EN.ID = HR.ID " +
                    "LEFT JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                    "LEFT JOIN COMPTY_ITEM I ON I.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                    "LEFT JOIN COMPTY C ON I.COMPETENCY_ID = C.COMPETENCY_ID " +
                    "LEFT JOIN FORM F ON HR.ID = F.ID AND C.COMPETENCY_ID = F.COMPETENCY_ID " +
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
                        enrollment.Id = (string)reader["ID"];
                        enrollment.CompetencyAll = (int)reader["COMPETENCY_ALL"];
                        enrollment.CompetencyPass = (int)reader["COMPETENCY_PASS"];
                        enrollment.CompetencyPer = (int)reader["COMPETENCY_PER"];
                        enrollment.Finish = (bool)reader["FINISH"];

                        IDPGroup iDPGroup = new IDPGroup();
                        iDPGroup.IDPGroupName = (string)reader["IDP_GROUP_NAME"];
                        iDPGroup.Year = (string)reader["YEAR"];

                        CompetencyItem competencyItem = new CompetencyItem();
                        competencyItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                        competencyItem.Pl = reader.IsDBNull(reader.GetOrdinal("PL")) ? null : (string)reader["PL"];
                        competencyItem.Critical = reader.IsDBNull(reader.GetOrdinal("CRITICAL")) ? false : (bool)reader["CRITICAL"];

                        Competency competency = new Competency();
                        competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];

                        Form form = new Form();
                        form.Requirement = reader.IsDBNull(reader.GetOrdinal("REQUIREMENT")) ? 0 : (int)reader["REQUIREMENT"];
                        form.Actual = reader.IsDBNull(reader.GetOrdinal("ACTUAL")) ? 0 : (int)reader["ACTUAL"];
                        form.Gap = reader.IsDBNull(reader.GetOrdinal("GAP")) ? 0 : (int)reader["GAP"];
                        form.Priority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                        form.Plan = reader.IsDBNull(reader.GetOrdinal("PLAN")) ? null : (string)reader["PLAN"];
                        form.PlanDesc = reader.IsDBNull(reader.GetOrdinal("PLAN_DESC")) ? null : (string)reader["PLAN_DESC"];
                        form.Quarter = reader.IsDBNull(reader.GetOrdinal("QUARTER")) ? null : (string)reader["QUARTER"];
                        form.RstPlan = reader.IsDBNull(reader.GetOrdinal("RST_PLAN")) ? null : (string)reader["RST_PLAN"];

                        enrollment.IDPGroup = iDPGroup;
                        enrollment.CompetencyItem = competencyItem;
                        enrollment.Competency = competency;
                        enrollment.Form = form;

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
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM USER_ENROLL WHERE ID = @Id", connection))
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

        public List<Enrollment> GetEnrollments(string idpGroupId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT en.ENROLL_ID, en.ID, en.IDP_GROUP_ID, hr.PREFIX, hr.FIRSTNAME_TH, hr.LASTNAME_TH, hr.POSITION, hr.DEPARTMENT_NAME, hr.JOBLEVEL, hr.COMPANY " +
                                      "FROM USER_ENROLL AS en JOIN MAS_USER_HR AS hr ON en.ID = hr.ID " +
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

                string query = "SELECT ID FROM USER_ENROLL WHERE IDP_GROUP_ID = @IDPGroupId";

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
                    string query = "INSERT INTO USER_ENROLL (IDP_GROUP_ID, ID, COMPETENCY_ALL, COMPETENCY_PASS, COMPETENCY_PER, FINISH) " +
                        "VALUES (@IDPGroupId, @Id, 0, 0, 0, 0)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);
                        command.Parameters.AddWithValue("@Id", user.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public int GetCountEmployee(string idpGroupId)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM USER_ENROLL WHERE IDP_GROUP_ID = @IDPGroupId";

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
                command.CommandText = "SELECT IDP_GROUP_ID FROM USER_ENROLL WHERE ENROLL_ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                return (string)command.ExecuteScalar();
            }
        }
        public void DeleteEmployeeByIDPGroup(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "DELETE FROM USER_ENROLL WHERE ENROLL_ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}   