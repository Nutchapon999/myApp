using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using myApp.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

namespace myApp.DAL
{
    public class App
    {
        private string connectionString;
        public App()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }


        #region COMPETENCY
        public List<Competency> GetCompetencies()
        {
            List<Competency> competencies = new List<Competency>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_COMPTY WHERE DELETED = 1";

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
                string query = "UPDATE IDP_COMPTY SET TYPE = @Type, COMPETENCY_NAME_TH = @TH, COMPETENCY_NAME_EN = @EN, COMPETENCY_DESC = @Desc, " +
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

                //DeleteCompetencyItemByCompetencyId(competencyId);

                string query = "UPDATE IDP_COMPTY SET DELETED = 0 WHERE COMPETENCY_ID = @CompetencyId";

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
        public List<Competency> GetCompetencyByType(string type)
        {
            List<Competency> competencies = new List<Competency>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_COMPTY WHERE DELETED = 1 AND TYPE = @Type";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Type", type);

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
        #endregion

        #region IDP GROUP
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
        public List<IDPGroup> GetIDPGroupsByYear(string year)
        {
            List<IDPGroup> iDPGroups = new List<IDPGroup>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_GROUP WHERE YEAR = @Year";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Year", year);

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
                throw new Exception(" รหัส" + idpGroupId.IDPGroupId +  "นี้มีการใช้แล้ว กรุณากรอกใหม่");
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
        public void UpdateIDPGroup(IDPGroup idpGroup, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                bool canUpdate = CheckIfIDPGroupIsSomeOne(idpGroup.IDPGroupId);
                if (canUpdate)
                {
                    throw new Exception("IDP Group นี้ใช้งานแล้วและไม่สามารถแก้ไขได้");
                }
                else
                {
                    string query = "UPDATE IDP_GROUP SET IDP_GROUP_NAME = @Name, YEAR = @Year, UPDATE_BY = @UpdateBy, UPDATE_ON = GETDATE() WHERE IDP_GROUP_ID = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", idpGroup.IDPGroupId);
                        command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(idpGroup.IDPGroupName) ? DBNull.Value : (object)idpGroup.IDPGroupName);
                        command.Parameters.AddWithValue("@Year", string.IsNullOrEmpty(idpGroup.Year) ? DBNull.Value : (object)idpGroup.Year);
                        command.Parameters.AddWithValue("@UpdateBy", username);

                    
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public bool CheckIfIDPGroupIsSomeOne(string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_USER_ENROLL WHERE IDP_GROUP_ID = @IdpGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdpGroupId", idpGroupId);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
        public void DeleteIDPGroup(string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool canDelete = CheckIfIDPGroupIsDraft(idpGroupId);
                if(canDelete)
                {
                    throw new Exception("IDP Group นี้ใช้งานแล้วและไม่สามารถลบได้");
                }
                else
                {
                    DeleteCompetencyItemByIDPGroupId(idpGroupId);
                    DeleteEnrollByIDPGroupId(idpGroupId);
                    DeleteResultAllByIDPGroupId(idpGroupId);

                    string query = "DELETE FROM IDP_GROUP WHERE IDP_GROUP_ID = @IDPGroupId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    
                        command.ExecuteNonQuery();
                    }
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
        public void InsertEnrollCopy(List<Enrollment> enrollments, IDPGroup iDPGroup)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (Enrollment enrollment in enrollments)
                {
                    string query = "INSERT INTO IDP_USER_ENROLL (IDP_GROUP_ID, ID, STATUS) " +
                                    "VALUES (@IDPGroupId, @Id, 'Draft')";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDPGroupId", iDPGroup.IDPGroupId);
                        command.Parameters.AddWithValue("@Id", enrollment.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void InsertIDPGroupItemCopy(List<IDPGroupItem> iDPGroupItems, IDPGroup iDPGroup)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (IDPGroupItem iDPGroupItem in iDPGroupItems)
                {
                    string query = "INSERT INTO IDP_GROUP_ITEM (COMPETENCY_ID, IDP_GROUP_ID, PL, CRITICAL) " +
                                    "VALUES (@CompetencyId, @IDPGroupId, @Pl, @Cri)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IDPGroupId", iDPGroup.IDPGroupId);
                        command.Parameters.AddWithValue("@CompetencyId", iDPGroupItem.CompetencyId);
                        command.Parameters.AddWithValue("@Pl", iDPGroupItem.Pl);
                        command.Parameters.AddWithValue("@Cri", iDPGroupItem.Critical);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public List<IDPGroup> getIDPGroupByYear(string year)
        {
            List<IDPGroup> iDPGroups = new List<IDPGroup>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_GROUP WHERE YEAR = @Year";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Year", year);

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
        #endregion

        #region IDP GROUP ITEM
        public List<IDPGroupItem> GetIDPGroupItems(string idpGroupId)
        {
            List<IDPGroupItem> competencyItems = new List<IDPGroupItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT CIT.IDP_GROUP_ITEM_ID, CIT.IDP_GROUP_ID, CIT.COMPETENCY_ID, C.COMPETENCY_NAME_TH, PL, CRITICAL, C.ACTIVE, " +
                                        "C.PL1, C.PL2, C.PL3, C.PL4, C.PL5 " +
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
                        idpGroupItem.Pl = reader.IsDBNull(reader.GetOrdinal("PL")) ? null : (string)reader["PL"];
                        idpGroupItem.Critical = (bool)reader["CRITICAL"];

                        Competency competency = new Competency();
                        competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];
                        competency.Pl1 = reader.IsDBNull(reader.GetOrdinal("PL1")) ? null : (string)reader["PL1"];
                        competency.Pl2 = reader.IsDBNull(reader.GetOrdinal("PL2")) ? null : (string)reader["PL2"];
                        competency.Pl3 = reader.IsDBNull(reader.GetOrdinal("PL3")) ? null : (string)reader["PL3"];
                        competency.Pl4 = reader.IsDBNull(reader.GetOrdinal("PL4")) ? null : (string)reader["PL4"];
                        competency.Pl5 = reader.IsDBNull(reader.GetOrdinal("PL5")) ? null : (string)reader["PL5"];
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
                    string query = "INSERT INTO IDP_GROUP_ITEM (COMPETENCY_ID, IDP_GROUP_ID, PL, CRITICAL) " +
                                    "VALUES (@CompetencyId, @IDPGroupId, '0', @Cri)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompetencyId", competency.CompetencyId);
                        command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);
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
        public void DeleteIDPGroupItem(int idpGroupItemId, string idpGroupId)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool canDelete = CheckIfIDPGroupIsDraft(idpGroupId);
                if(canDelete)
                {
                    throw new Exception("IDP Group นี้ใช้งานแล้วและไม่สามารถลบได้");
                }
                else
                {
                    string query = "DELETE FROM IDP_GROUP_ITEM WHERE IDP_GROUP_ITEM_ID = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", idpGroupItemId);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateIDPGroupItems(Dictionary<string, IDPGroupItem> idpGroupItems, string idpGroupId)
        {
            if (idpGroupItems.Count == 0)
            {
                return; 
            }

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
        public bool CheckIfIDPGroupIsDraft(string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_USER_ENROLL WHERE IDP_GROUP_ID = @IdpGroupId AND STATUS != 'Draft'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdpGroupId", idpGroupId);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
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
        public int GetCountResult(string idpGroupId)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_RESULT WHERE IDP_GROUP_ID = @IDPGroupId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }
        public void UpdateResultItem(Dictionary<string, IDPGroupItem> idpGroupItems, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var kvp in idpGroupItems)
                {
                    var idpGroupItemId = kvp.Key;
                    var idpGroupItem = kvp.Value;

                    string updateQuery = "UPDATE IDP_RESULT_ITEM SET REQUIREMENT = @Pl, CRITICAL = @Cri WHERE COMPETENCY_ID = @Id";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Pl", idpGroupItem.Pl);
                        command.Parameters.AddWithValue("@Cri", idpGroupItem.Critical);
                        command.Parameters.AddWithValue("@Id", idpGroupItem.CompetencyId);

                        command.ExecuteNonQuery();
                    }

                    
                }
            }
        }
        public List<Result> GetResultByIDPGroupId(string idpGroupId)
        {
            List<Result> results = new List<Result>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_RESULT WHERE IDP_GROUP_ID = @IDPGroupId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Result result = new Result();

                    result.GUID = (string)reader["GUID"];
                    result.Id = (string)reader["ID"];

                    results.Add(result);
                }
                reader.Close();
            }

            return results;
        }
        public List<ResultItem> GetResultItemByGuid(string guid)
        {
            List<ResultItem> resultItems = new List<ResultItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_RESULT_ITEM WHERE GUID = @Guid";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ResultItem resultItem = new ResultItem();

                    resultItem.GUID = (string)reader["GUID"];
                    resultItem.ResultItemId = (int)reader["RESULT_ITEM"];
                    resultItem.Requirement = (int)reader["REQUIREMENT"];
                    resultItem.Actual1 = (int)reader["ACTUAL1"];
                    resultItem.Actual2 = (int)reader["ACTUAL2"];

                    resultItems.Add(resultItem);
                }
                reader.Close();
            }

            return resultItems;
        }
        public void UpdateGaps(List<ResultItem> resultItems)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (ResultItem resultItem in resultItems)
                {

                    string updateQuery = "UPDATE IDP_RESULT_ITEM SET GAP1 = @Gap1, GAP2 = @Gap2 WHERE GUID = @Guid AND RESULT_ITEM = @ResultItem";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Gap1", resultItem.Actual1 - resultItem.Requirement);
                        command.Parameters.AddWithValue("@Gap2", resultItem.Actual2 - resultItem.Requirement);
                        command.Parameters.AddWithValue("@Guid", resultItem.GUID);
                        command.Parameters.AddWithValue("@ResultItem", resultItem.ResultItemId);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateResult(List<Result> results, string idpGroupId)
        {
            
            foreach (Result result in results)
            {
                    string status = GetStatus(result.Id, idpGroupId);

                    int all = GetCompetencyAllByGuid(result.GUID);
                    int pass1;
                    int pass2;
                    
                        pass1 = GetCompetencyPassByGap1(result.GUID);
                    
                        pass2 = GetCompetencyPassByGap2(result.GUID);
                    

                    //CALCULATE VALUES FOR RESULT
                    float per1 = (float)pass1 / all * 100;
                    float per2 = (float)pass2 / all * 100;
                    string rank1;
                    string rank2;

                    switch (per1)
                    {
                        case var p when p >= 100:
                            rank1 = "M";
                            break;
                        case var p when p < 100 && p >= 70:
                            rank1 = "C";
                            break;
                        default:
                            rank1 = "L";
                            break;
                    }
                    switch (per2)
                    {
                        case var p when p >= 100:
                            rank2 = "M";
                            break;
                        case var p when p < 100 && p >= 70:
                            rank2 = "C";
                            break;
                        default:
                            rank2 = "L";
                            break;
                    }

                    
                    UpdateResultA1(result.GUID, pass1, per1, rank1);
                    
                    UpdateResultA2(result.GUID, pass2, per2, rank2);

            }
        }
        #endregion

        #region EMPLOYEE
        public List<User> GetUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM MAS_USER_HR ORDER BY JOBLEVEL ASC";

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
                command.CommandText = "SELECT EN.ENROLL_ID, EN.IDP_GROUP_ID, EN.STATUS, G.IDP_GROUP_NAME, G.YEAR " +
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
                        enrollment.Status = (string)reader["STATUS"];

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
                    string query = "INSERT INTO IDP_USER_ENROLL (ID, IDP_GROUP_ID, STATUS) VALUES " +
                        "(@Id, @IDPGroupId, 'Draft')";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@IDPGroupId", iDPGroup.IDPGroupId);
                        command.Parameters.AddWithValue("@Id", id);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public string GetIdByEnrollment(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT ID FROM IDP_USER_ENROLL WHERE ENROLL_ID = @EnrollId";
                command.Parameters.AddWithValue("@EnrollId", enrollId);

                connection.Open();

                return (string)command.ExecuteScalar();
            }
        }
        public void DeleteIDPGroupByEmployee(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool canDelete = CheckIfEnrollIsNotDraft(enrollId);

                if (canDelete)
                {
                    throw new Exception("IDP Group นี้ใช้งานแล้วและไม่สามารถแก้ไขได้");
                }
                else
                {
                    string query = "DELETE FROM IDP_USER_ENROLL WHERE ENROLL_ID = @EnrollId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EnrollId", enrollId);

                    
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public List<User> getEmployeeByDepartment(string Department)
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "SELECT * FROM MAS_USER_HR WHERE DEPARTMENT_NAME = @Department ORDER BY JOBLEVEL ASC";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Department", Department);


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
        public List<User> getEmployeeByDepartmentActive(string Department)
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "SELECT * FROM MAS_USER_HR WHERE DEPARTMENT_NAME = @Department AND STATUS = 'ทำงาน' ORDER BY JOBLEVEL ASC";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Department", Department);


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


                    users.Add(user);
                }
                reader.Close();
            }

            return users;
        }

        #endregion

        #region FORM
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
        public List<Enrollment> GetEnrollEachYearByUsername(string username, string year)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * , (SELECT COUNT(*) FROM IDP_GROUP_ITEM GI WHERE G.IDP_GROUP_ID = GI.IDP_GROUP_ID) AS competencies " +
                    "FROM IDP_USER_ENROLL EN " +
                    "JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID AND G.YEAR = @year " +
                    "JOIN IDP_RESULT R ON EN.ID = R.ID AND R.YEAR = @year AND EN.IDP_GROUP_ID = R.IDP_GROUP_ID " +
                    "JOIN MAS_USER_HR HR ON HR.ID = EN.ID " +
                    "WHERE EN.STATUS != 'Draft' AND HR.USER_LOGIN = @Username";

                command.Parameters.AddWithValue("@Year", year);
                command.Parameters.AddWithValue("@Username", username);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enrollment enrollment = new Enrollment();
                        enrollment.EnrollId = (int)reader["ENROLL_ID"];
                        enrollment.IDPGroupId = (string)reader["IDP_GROUP_ID"];
                        enrollment.Competencies = (int)reader["competencies"];
                        enrollment.Status = (string)reader["STATUS"];
                        //enrollment.Finish = (bool)reader["FINISH"];

                        IDPGroup iDPGroup = new IDPGroup();
                        iDPGroup.IDPGroupName = (string)reader["IDP_GROUP_NAME"];

                        Result result = new Result();
                        result.GUID = reader.IsDBNull(reader.GetOrdinal("GUID")) ? null : (string)reader["GUID"];
                        result.K2_No = reader.IsDBNull(reader.GetOrdinal("K2_NO")) ? null : (string)reader["K2_NO"];
                        result.Year = (string)reader["YEAR"];
                        result.CurrentApprover = reader.IsDBNull(reader.GetOrdinal("CURRENT_APPROVER")) ? null : (string)reader["CURRENT_APPROVER"];


                        enrollment.IDPGroup = iDPGroup;
                        enrollment.Result = result;

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
                                        "I.CRITICAL, F.GUID, F.RESULT_ITEM, F.REQUIREMENT, F.ACTUAL1, F.GAP1, F.PRIORITY, F.TYPE_PLAN, F.DEV_PLAN, F.Q1, F.Q2, F.Q3, F.Q4, F.DEV_RST, F.ACTUAL2, F.GAP2, F.FILE_ID, " +
                                        "C.PL1, C.PL2, C.PL3, C.PL4, C.PL5 " +
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
                        user.Prefix = reader.IsDBNull(reader.GetOrdinal("PREFIX")) ? null : (string)reader["PREFIX"];
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
                        resultItem.Actual1 = reader.IsDBNull(reader.GetOrdinal("ACTUAL1")) ? 0 : (int)reader["ACTUAL1"];
                        resultItem.Gap1 = reader.IsDBNull(reader.GetOrdinal("GAP1")) ? 0 : (int)reader["GAP1"];
                        resultItem.Priority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                        resultItem.TypePlan = reader.IsDBNull(reader.GetOrdinal("TYPE_PLAN")) ? null : (string)reader["TYPE_PLAN"];
                        resultItem.DevPlan = reader.IsDBNull(reader.GetOrdinal("DEV_PLAN")) ? null : (string)reader["DEV_PLAN"];
                        resultItem.Q1 = reader.IsDBNull(reader.GetOrdinal("Q1")) ? null : (string)reader["Q1"];
                        resultItem.Q2 = reader.IsDBNull(reader.GetOrdinal("Q2")) ? null : (string)reader["Q2"];
                        resultItem.Q3 = reader.IsDBNull(reader.GetOrdinal("Q3")) ? null : (string)reader["Q3"];
                        resultItem.Q4 = reader.IsDBNull(reader.GetOrdinal("Q4")) ? null : (string)reader["Q4"];
                        resultItem.DevRst = reader.IsDBNull(reader.GetOrdinal("DEV_RST")) ? null : (string)reader["DEV_RST"];
                        resultItem.Actual2 = reader.IsDBNull(reader.GetOrdinal("ACTUAL2")) ? 0 : (int)reader["ACTUAL2"];
                        resultItem.Gap2 = reader.IsDBNull(reader.GetOrdinal("GAP2")) ? 0 : (int)reader["GAP2"];
                        resultItem.FileId = reader.IsDBNull(reader.GetOrdinal("FILE_ID")) ? null : (string)reader["FILE_ID"];

                        /*RemarkHS remark = new RemarkHS();
                        remark.Name = reader.IsDBNull(reader.GetOrdinal("NAME")) ? null : (string)reader["NAME"];
                        remark.Position = reader.IsDBNull(reader.GetOrdinal("POSITION")) ? null : (string)reader["POSITION"];
                        remark.Remark = reader.IsDBNull(reader.GetOrdinal("REMARK")) ? null : (string)reader["REMARK"];
                        remark.RemarkDate = reader.IsDBNull(reader.GetOrdinal("REMARK_DATE")) ? null : ((DateTime)reader["REMARK_DATE"]).ToString("yyyy-MM-dd");*/

                        enrollment.User = user;
                        enrollment.IDPGroup = iDPGroup;
                        enrollment.IDPGroupItem = idpGroupItem;
                        enrollment.Competency = competency;
                        enrollment.ResultItem = resultItem;
                        //enrollment.RemarkHS = remark;

                        enrollments.Add(enrollment);
                    }
                }       
            }

            return enrollments;
        }
        public void InsertResultDetails(List<IDPGroupItem> iDPGroupItems, string guid, int count, List<ResultItem> resultItems)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO IDP_RESULT_ITEM (GUID, RESULT_ITEM, CRITICAL, IDP_GROUP_ID, COMPETENCY_ID, REQUIREMENT, ACTUAL1, GAP1, PRIORITY, TYPE_PLAN, DEV_PLAN, Q1, Q2, Q3, Q4, DEV_RST, ACTUAL2, GAP2) VALUES" +
                    " (@Guid, @ResultItem, @Critical, @IDPGroupId, @CompetencyId, @Require, @Actual1, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)";

                connection.Open();

                int resultItemIndex = 1;

                foreach (var iDPGroupItem in iDPGroupItems)
                {
                    ResultItem matchingResultItem = resultItems.FirstOrDefault(r => r.CompetencyId == iDPGroupItem.CompetencyId);

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Guid", guid);
                        command.Parameters.AddWithValue("@ResultItem", resultItemIndex);
                        command.Parameters.AddWithValue("@Critical", iDPGroupItem.Critical);
                        command.Parameters.AddWithValue("@IDPGroupId", iDPGroupItem.IDPGroupId);
                        command.Parameters.AddWithValue("@CompetencyId", iDPGroupItem.CompetencyId);
                        command.Parameters.AddWithValue("@Require", iDPGroupItem.Pl);

                        if (matchingResultItem != null)
                        {
                            command.Parameters.AddWithValue("@Actual1", matchingResultItem.Actual2);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Actual1", 0); 
                        }

                        command.ExecuteNonQuery();
                    }

                    resultItemIndex++;
                    if (resultItemIndex > count) break;
                }
            }
        }
        public void UpdateForm(List<ResultItem> resultItems, string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_RESULT_ITEM SET REQUIREMENT = @Requirement, ACTUAL1 = @Actual1, GAP1 = @Gap1, PRIORITY = @Priority, TYPE_PLAN = @Type, DEV_PLAN = @DevPlan, " +
                                    "Q1 = @Q1, Q2 = @Q2, Q3 = @Q3, Q4 = @Q4, DEV_RST = @DevRst, ACTUAL2 = @Actual2, GAP2 = @Gap2, FILE_ID = NULL " +
                                    "WHERE GUID = @Guid AND RESULT_ITEM = @ResultItem";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    for (int i = 0; i < resultItems.Count; i++)
                    {
                        ResultItem resultItem = resultItems[i];

                        updateCommand.Parameters.AddWithValue("@Guid", guid);
                        updateCommand.Parameters.AddWithValue("@Requirement", (object)resultItem.Requirement ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Actual1", (object)resultItem.Actual1 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Gap1", resultItem.Actual1 - resultItem.Requirement);
                        updateCommand.Parameters.AddWithValue("@Priority", string.IsNullOrEmpty(resultItem.Priority) ? (object)DBNull.Value : resultItem.Priority);
                        updateCommand.Parameters.AddWithValue("@Type", string.IsNullOrEmpty(resultItem.TypePlan) ? (object)DBNull.Value : resultItem.TypePlan);
                        updateCommand.Parameters.AddWithValue("@DevPlan", (object)resultItem.DevPlan ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Q1", (object)resultItem.Q1 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Q2", (object)resultItem.Q2 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Q3", (object)resultItem.Q3 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Q4", (object)resultItem.Q4 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@DevRst", (object)resultItem.DevRst ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Actual2", (object)resultItem.Actual2 ?? DBNull.Value);
                        updateCommand.Parameters.AddWithValue("@Gap2", resultItem.Actual2 - resultItem.Requirement);
                        updateCommand.Parameters.AddWithValue("@ResultItem", i + 1); 

                        updateCommand.ExecuteNonQuery();

                        updateCommand.Parameters.Clear();
                    }
                }
            }
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
        }public int GetCountEnrollmentEachYearById(string id, string year)
        {
            int enrolled = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM IDP_USER_ENROLL EN " +
                                                        "JOIN IDP_GROUP G ON EN.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                                                        "WHERE ID = @Id AND YEAR = @Year AND EN.STATUS != 'Decline'", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Year", year);

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
        public int GetCompetencyPassByGap1(string guid)
        {
            int enrolled = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) " +
                "FROM IDP_RESULT R " +
                "JOIN IDP_RESULT_ITEM RI ON R.GUID = RI.GUID " +    
                "WHERE R.GUID = @GUID AND RI.GAP1 >= 0", connection))
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
        public int GetCompetencyPassByGap2(string guid)
        {
            int enrolled = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) " +
                "FROM IDP_RESULT R " +
                "JOIN IDP_RESULT_ITEM RI ON R.GUID = RI.GUID " +
                "WHERE R.GUID = @GUID AND RI.GAP2 >= 0", connection))
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
        public void UpdateResultA1(string guid, int pass, float per, string rank)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_PASS1 = @Pass, COMPETENCY_PER1 = @Per, RANK1 = @Rank WHERE GUID = @GUID";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {


                    updateCommand.Parameters.AddWithValue("@GUID", guid);
                    updateCommand.Parameters.AddWithValue("@Per", per);
                    updateCommand.Parameters.AddWithValue("@Pass", pass);
                    updateCommand.Parameters.AddWithValue("@Rank", rank);

                    updateCommand.ExecuteNonQuery();

                }
            }
        }
        public void UpdateResultA2(string guid, int pass, float per, string rank)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_PASS2 = @Pass, COMPETENCY_PER2 = @Per, RANK2 = @Rank WHERE GUID = @GUID";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {


                    updateCommand.Parameters.AddWithValue("@GUID", guid);
                    updateCommand.Parameters.AddWithValue("@Per", per);
                    updateCommand.Parameters.AddWithValue("@Pass", pass);
                    updateCommand.Parameters.AddWithValue("@Rank", rank);

                    updateCommand.ExecuteNonQuery();

                }
            }
        }
        public List<int> GetResultItemIdByGuid(string guid)
        {
            List<int> resultItems = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT RESULT_ITEM " +
                                    "FROM IDP_RESULT_ITEM " +
                                    "WHERE GUID = @Guid";

                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int resultItemId = reader.IsDBNull(reader.GetOrdinal("RESULT_ITEM")) ? 0 : (int)reader["RESULT_ITEM"];
                        resultItems.Add(resultItemId);
                    }
                }
            }

            return resultItems;
        }
        public List<ResultItem> GetResultItemByGuidBeforeUpdate(string guid)
        {
            List<ResultItem> resultItems = new List<ResultItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * " +
                                    "FROM IDP_RESULT_ITEM " +
                                    "WHERE GUID = @Guid";

                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ResultItem resultItem = new ResultItem();
                        resultItem.ResultItemId = reader.IsDBNull(reader.GetOrdinal("RESULT_ITEM")) ? 0 : (int)reader["RESULT_ITEM"];
                        resultItem.Requirement = reader.IsDBNull(reader.GetOrdinal("REQUIREMENT")) ? 0 : (int)reader["REQUIREMENT"];
                        resultItem.OriginalActual1 = reader.IsDBNull(reader.GetOrdinal("ACTUAL1")) ? 0 : (int)reader["ACTUAL1"];
                        resultItem.OriginalGap1 = reader.IsDBNull(reader.GetOrdinal("GAP1")) ? 0 : (int)reader["GAP1"];
                        resultItem.OriginalPriority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                        resultItem.OriginalType = reader.IsDBNull(reader.GetOrdinal("TYPE_PLAN")) ? null : (string)reader["TYPE_PLAN"];
                        resultItem.OriginalDevPlan = reader.IsDBNull(reader.GetOrdinal("DEV_PLAN")) ? null : (string)reader["DEV_PLAN"];
                        resultItem.OriginalQ1 = reader.IsDBNull(reader.GetOrdinal("Q1")) ? null : (string)reader["Q1"];
                        resultItem.OriginalQ2 = reader.IsDBNull(reader.GetOrdinal("Q2")) ? null : (string)reader["Q2"];
                        resultItem.OriginalQ3 = reader.IsDBNull(reader.GetOrdinal("Q3")) ? null : (string)reader["Q3"];
                        resultItem.OriginalQ4 = reader.IsDBNull(reader.GetOrdinal("Q4")) ? null : (string)reader["Q4"];
                        resultItem.OriginalDevRst = reader.IsDBNull(reader.GetOrdinal("DEV_RST")) ? null : (string)reader["DEV_RST"];
                        resultItem.OriginalActual2 = reader.IsDBNull(reader.GetOrdinal("ACTUAL2")) ? 0 : (int)reader["ACTUAL2"];
                        resultItem.OriginalGap2 = reader.IsDBNull(reader.GetOrdinal("GAP2")) ? 0 : (int)reader["GAP2"];

                        resultItems.Add(resultItem);
                    }
                }
            }

            return resultItems;
        }
        public List<ResultItem> GetResultItemByGuidAfterUpdate(string guid)
        {
            List<ResultItem> resultItems = new List<ResultItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * " +
                                    "FROM IDP_RESULT_ITEM " +
                                    "WHERE GUID = @Guid";

                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ResultItem resultItem = new ResultItem();
                        resultItem.ResultItemId = reader.IsDBNull(reader.GetOrdinal("RESULT_ITEM")) ? 0 : (int)reader["RESULT_ITEM"];
                        resultItem.Requirement = reader.IsDBNull(reader.GetOrdinal("REQUIREMENT")) ? 0 : (int)reader["REQUIREMENT"];
                        resultItem.Actual1 = reader.IsDBNull(reader.GetOrdinal("ACTUAL1")) ? 0 : (int)reader["ACTUAL1"];
                        resultItem.Gap1 = reader.IsDBNull(reader.GetOrdinal("GAP1")) ? 0 : (int)reader["GAP1"];
                        resultItem.Priority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                        resultItem.TypePlan = reader.IsDBNull(reader.GetOrdinal("TYPE_PLAN")) ? null : (string)reader["TYPE_PLAN"];
                        resultItem.DevPlan = reader.IsDBNull(reader.GetOrdinal("DEV_PLAN")) ? null : (string)reader["DEV_PLAN"];
                        resultItem.Q1 = reader.IsDBNull(reader.GetOrdinal("Q1")) ? null : (string)reader["Q1"];
                        resultItem.Q2 = reader.IsDBNull(reader.GetOrdinal("Q2")) ? null : (string)reader["Q2"];
                        resultItem.Q3 = reader.IsDBNull(reader.GetOrdinal("Q3")) ? null : (string)reader["Q3"];
                        resultItem.Q4 = reader.IsDBNull(reader.GetOrdinal("Q4")) ? null : (string)reader["Q4"];
                        resultItem.DevRst = reader.IsDBNull(reader.GetOrdinal("DEV_RST")) ? null : (string)reader["DEV_RST"];
                        resultItem.Actual2 = reader.IsDBNull(reader.GetOrdinal("ACTUAL2")) ? 0 : (int)reader["ACTUAL2"];
                        resultItem.Gap2 = reader.IsDBNull(reader.GetOrdinal("GAP2")) ? 0 : (int)reader["GAP2"];

                        resultItems.Add(resultItem);
                    }
                }
            }

            return resultItems;
        }
        public void InsertLogUser (List<int> resultItemIds, string username, List<ResultItem> resultItemsBefore, List<ResultItem> resultItemsAfter, string status, string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (int resultItemId in resultItemIds)
                {
                    ResultItem resultItemBefore = resultItemsBefore.Find(item => item.ResultItemId == resultItemId);
                    ResultItem resultItemAfter = resultItemsAfter.Find(item => item.ResultItemId == resultItemId);

                    if (resultItemAfter != null && resultItemBefore != null)
                    {
                        string query = "INSERT INTO IDP_LOG (GUID, ITEM, UPDATED_BY, UPDATED_ON, COLUMN_UPDATED, OLD_VALUE, NEW_VALUE) " +
                                       "VALUES (@Guid, @ResultItemId, @Username, GETDATE(), @ColumnUpdated, @OldValue, @NewValue)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            if(resultItemBefore.OriginalActual1 != resultItemAfter.Actual1)
                            {
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Actual1");
                                command.Parameters.AddWithValue("@OldValue", resultItemBefore.OriginalActual1.ToString());
                                command.Parameters.AddWithValue("@NewValue", resultItemAfter.Actual1.ToString());

                                command.ExecuteNonQuery();
                                if(resultItemAfter.Actual1 >= resultItemAfter.Requirement && resultItemBefore.Actual1 != 0)
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@Guid", guid);
                                    command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                    command.Parameters.AddWithValue("@Username", username);
                                    command.Parameters.AddWithValue("@ColumnUpdated", "Priority");
                                    command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalPriority ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Priority ?? DBNull.Value);

                                    command.ExecuteNonQuery();

                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@Guid", guid);
                                    command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                    command.Parameters.AddWithValue("@Username", username);
                                    command.Parameters.AddWithValue("@ColumnUpdated", "TypePlan");
                                    command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalType ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.TypePlan ?? DBNull.Value);

                                    command.ExecuteNonQuery();

                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@Guid", guid);
                                    command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                    command.Parameters.AddWithValue("@Username", username);
                                    command.Parameters.AddWithValue("@ColumnUpdated", "DevPlan");
                                    command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalDevPlan ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.DevPlan ?? DBNull.Value);

                                    command.ExecuteNonQuery();

                                    if (resultItemBefore.OriginalQ1 == "1")
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Guid", guid);
                                        command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@ColumnUpdated", "Q1");
                                        command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ1 ?? DBNull.Value);
                                        command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q1 ?? DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }

                                    if (resultItemBefore.OriginalQ2 == "1")
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Guid", guid);
                                        command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@ColumnUpdated", "Q2");
                                        command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ2 ?? DBNull.Value);
                                        command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q2 ?? DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }

                                    if (resultItemBefore.OriginalQ3 == "1")
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Guid", guid);
                                        command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@ColumnUpdated", "Q3");
                                        command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ3 ?? DBNull.Value);
                                        command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q3 ?? DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }

                                    if (resultItemBefore.OriginalQ4 == "1")
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Guid", guid);
                                        command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@ColumnUpdated", "Q4");
                                        command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ4 ?? DBNull.Value);
                                        command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q4 ?? DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }
                                    
                                    continue;
                                }

                            }
                            if (resultItemBefore.OriginalPriority != resultItemAfter.Priority)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Priority");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalPriority ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Priority ?? DBNull.Value);

                                command.ExecuteNonQuery();

                            }
                            if (resultItemBefore.OriginalType != resultItemAfter.TypePlan)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "TypePlan");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalType ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.TypePlan ?? DBNull.Value);

                                command.ExecuteNonQuery();

                            }
                            if (resultItemBefore.OriginalDevPlan != resultItemAfter.DevPlan)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "DevPlan");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalDevPlan ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.DevPlan ?? DBNull.Value);

                                command.ExecuteNonQuery();

                            }
                            if (resultItemBefore.OriginalQ1 != resultItemAfter.Q1)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Q1");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ1 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q1 ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }

                            if (resultItemBefore.OriginalQ2 != resultItemAfter.Q2)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Q2");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ2 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q2 ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }

                            if (resultItemBefore.OriginalQ3 != resultItemAfter.Q3)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Q3");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ3 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q3 ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }

                            if (resultItemBefore.OriginalQ4 != resultItemAfter.Q4)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Q4");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ4 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q4 ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }

                            if(status == "Developing" && resultItemAfter.Actual1 < resultItemAfter.Requirement)
                            {
                                
                                if (resultItemBefore.OriginalDevRst != resultItemAfter.DevRst)
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@Guid", guid);
                                    command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                    command.Parameters.AddWithValue("@Username", username);
                                    command.Parameters.AddWithValue("@ColumnUpdated", "DevRst");
                                    command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalDevRst ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.DevRst ?? DBNull.Value);

                                    command.ExecuteNonQuery();
                                }

                            }
                            if (status == "2nd Evaluating" && resultItemBefore.OriginalDevRst != resultItemAfter.DevRst)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "DevRst");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalDevRst ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.DevRst ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }
                            if (status == "2nd Evaluating")
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Actaul2");
                                command.Parameters.AddWithValue("@OldValue", resultItemBefore.Actual2.ToString());
                                command.Parameters.AddWithValue("@NewValue", resultItemAfter.Actual2.ToString());

                                command.ExecuteNonQuery();
                            }
                            

                        }
                    }
                }
            }
        }
        public void InsertLogAdmin(List<int> resultItemIds, string username, List<ResultItem> resultItemsBefore, List<ResultItem> resultItemsAfter, string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (int resultItemId in resultItemIds)
                {
                    ResultItem resultItemBefore = resultItemsBefore.Find(item => item.ResultItemId == resultItemId);
                    ResultItem resultItemAfter = resultItemsAfter.Find(item => item.ResultItemId == resultItemId);

                    if (resultItemAfter != null && resultItemBefore != null)
                    {
                        string query = "INSERT INTO IDP_LOG (GUID, ITEM, UPDATED_BY, UPDATED_ON, COLUMN_UPDATED, OLD_VALUE, NEW_VALUE) " +
                                       "VALUES (@Guid, @ResultItemId, @Username, GETDATE(), @ColumnUpdated, @OldValue, @NewValue)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            if (resultItemBefore.OriginalActual1 != resultItemAfter.Actual1)
                            {
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Actual1");
                                command.Parameters.AddWithValue("@OldValue", resultItemBefore.OriginalActual1.ToString());
                                command.Parameters.AddWithValue("@NewValue", resultItemAfter.Actual1.ToString());

                                command.ExecuteNonQuery();
                                if (resultItemAfter.Actual1 >= resultItemAfter.Requirement && resultItemBefore.Actual1 != 0)
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@Guid", guid);
                                    command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                    command.Parameters.AddWithValue("@Username", username);
                                    command.Parameters.AddWithValue("@ColumnUpdated", "Priority");
                                    command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalPriority ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Priority ?? DBNull.Value);

                                    command.ExecuteNonQuery();

                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@Guid", guid);
                                    command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                    command.Parameters.AddWithValue("@Username", username);
                                    command.Parameters.AddWithValue("@ColumnUpdated", "TypePlan");
                                    command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalType ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.TypePlan ?? DBNull.Value);

                                    command.ExecuteNonQuery();

                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@Guid", guid);
                                    command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                    command.Parameters.AddWithValue("@Username", username);
                                    command.Parameters.AddWithValue("@ColumnUpdated", "DevPlan");
                                    command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalDevPlan ?? DBNull.Value);
                                    command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.DevPlan ?? DBNull.Value);

                                    command.ExecuteNonQuery();

                                    if (resultItemBefore.OriginalQ1 == "1")
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Guid", guid);
                                        command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@ColumnUpdated", "Q1");
                                        command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ1 ?? DBNull.Value);
                                        command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q1 ?? DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }

                                    if (resultItemBefore.OriginalQ2 == "1")
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Guid", guid);
                                        command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@ColumnUpdated", "Q2");
                                        command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ2 ?? DBNull.Value);
                                        command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q2 ?? DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }

                                    if (resultItemBefore.OriginalQ3 == "1")
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Guid", guid);
                                        command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@ColumnUpdated", "Q3");
                                        command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ3 ?? DBNull.Value);
                                        command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q3 ?? DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }

                                    if (resultItemBefore.OriginalQ4 == "1")
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@Guid", guid);
                                        command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@ColumnUpdated", "Q4");
                                        command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ4 ?? DBNull.Value);
                                        command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q4 ?? DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }

                                    continue;
                                }

                            }
                            if (resultItemBefore.OriginalPriority != resultItemAfter.Priority)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Priority");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalPriority ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Priority ?? DBNull.Value);

                                command.ExecuteNonQuery();

                            }
                            if (resultItemBefore.OriginalType != resultItemAfter.TypePlan)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "TypePlan");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalType ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.TypePlan ?? DBNull.Value);

                                command.ExecuteNonQuery();

                            }
                            if (resultItemBefore.OriginalDevPlan != resultItemAfter.DevPlan)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "DevPlan");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalDevPlan ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.DevPlan ?? DBNull.Value);

                                command.ExecuteNonQuery();

                            }
                            if (resultItemBefore.OriginalQ1 != resultItemAfter.Q1)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Q1");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ1 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q1 ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }
                            if (resultItemBefore.OriginalQ2 != resultItemAfter.Q2)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Q2");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ2 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q2 ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }
                            if (resultItemBefore.OriginalQ3 != resultItemAfter.Q3)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Q3");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ3 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q3 ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }
                            if (resultItemBefore.OriginalQ4 != resultItemAfter.Q4)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Q4");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalQ4 ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.Q4 ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }
                            if (resultItemBefore.OriginalDevRst != resultItemAfter.DevRst)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "DevRst");
                                command.Parameters.AddWithValue("@OldValue", (object)resultItemBefore.OriginalDevRst ?? DBNull.Value);
                                command.Parameters.AddWithValue("@NewValue", (object)resultItemAfter.DevRst ?? DBNull.Value);

                                command.ExecuteNonQuery();
                            }
                            if (resultItemBefore.OriginalActual2 != resultItemAfter.Actual2)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@Guid", guid);
                                command.Parameters.AddWithValue("@ResultItemId", resultItemId);
                                command.Parameters.AddWithValue("@Username", username);
                                command.Parameters.AddWithValue("@ColumnUpdated", "Actaul2");
                                command.Parameters.AddWithValue("@OldValue", resultItemBefore.OriginalActual2.ToString());
                                command.Parameters.AddWithValue("@NewValue", resultItemAfter.Actual2.ToString());

                                command.ExecuteNonQuery();
                            }


                        }
                    }
                }
            }
        }
        public void InsertRemark(string remark, string username, string position, string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO REMARK_HISTORY (FORM_GUID, USER_K2, NAME, POSITION, REMARK, REMARK_DATE) " +
                                "VALUES (@Guid, @Username, @Username, @Position, @Remark, GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Remark", (object)remark ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Guid", guid);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void UpdateApprover(string username ,string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE IDP_RESULT SET CURRENT_APPROVER = @Username WHERE GUID = @Guid";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Guid", guid);
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

            }
        }
        public string GetApprover(string guid)
        {
            string approver = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT CURRENT_APPROVER FROM IDP_RESULT WHERE GUID = @Guid";
                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                // Assuming course_name is stored as a string column in the "Courses" table
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    approver = result.ToString();
                }
            }

            return approver;
        }
        public User GetUserByGuid(string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_RESULT R JOIN MAS_USER_HR HR ON R.ID = HR.ID WHERE GUID = @Guid";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Guid", guid);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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

                            return user;
                        }
                    }
                }
            }
            return null;
        }
        public User GetUserByUserLogin(string userLogin)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM MAS_USER_HR WHERE USER_LOGIN = @User";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@User", userLogin);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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

                            return user;
                        }
                    }
                }
            }
            return null;
        }
        public User GetUserById(string id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM MAS_USER_HR WHERE ID = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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

                            return user;
                        }
                    }
                }
            }
            return null;
        }
        public string GetUserLoginByEnrollId(int enrollId)
        {
            string user = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT HR.USER_LOGIN " +
                                        "FROM IDP_USER_ENROLL EN " +
                                        "JOIN MAS_USER_HR HR ON EN.ID = HR.ID " +
                                        "WHERE EN.ENROLL_ID = @EnrollId";
                command.Parameters.AddWithValue("@EnrollId", enrollId);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    user = result.ToString();
                }
            }

            return user;
        }
        public void DeleteResult(string id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM IDP_RESULT WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void DeleteResultAllByIDPGroupId(string idpGroupId) 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM IDP_RESULT WHERE IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<ResultItem> GetPreActual2(string id, string year)
        {
            List<ResultItem> resultItems = new List<ResultItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT COMPETENCY_ID, ACTUAL2 " +
                                        "FROM IDP_RESULT_ITEM RI " +
                                        "RIGHT JOIN IDP_RESULT R ON R.GUID = RI.GUID " +
                                        "JOIN MAS_USER_HR HR ON HR.ID = R.ID " +
                                        "JOIN IDP_USER_ENROLL EN ON EN.IDP_GROUP_ID = R.IDP_GROUP_ID AND EN.ID = R.ID " +
                                        "WHERE HR.ID = @Id AND R.YEAR = @Year - 1 AND EN.STATUS = 'Success'";

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ResultItem resultItem = new ResultItem();
                        resultItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                        resultItem.Actual2 = reader.IsDBNull(reader.GetOrdinal("ACTUAL2")) ? 0 : (int)reader["ACTUAL2"];
                        resultItems.Add(resultItem);
                    }
                }
            }

            return resultItems;
        }
        public void UpdateWorkflowCompelete(string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_RESULT SET COMPLETED_ON = GETDATE() WHERE GUID = @Guid";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Guid", guid);

                    updateCommand.ExecuteNonQuery();
                }

            }
        }
        public int CountPL0(string idpGroupId)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM IDP_GROUP_ITEM WHERE PL = 0 AND IDP_GROUP_ID = @IdpGroupId", connection))
            {

                command.Parameters.AddWithValue("@IdpGroupId", idpGroupId);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }
        #endregion

        #region ENROLLMENT
        public List<Enrollment> GetEnrollments(string idpGroupId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * " +
                                      "FROM IDP_USER_ENROLL AS en " +
                                      "JOIN MAS_USER_HR hr ON en.ID = hr.ID " +
                                      "JOIN IDP_RESULT RE ON EN.ID = RE.ID AND EN.IDP_GROUP_ID = RE.IDP_GROUP_ID " +
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
                        user.UserLogin = reader["USER_LOGIN"] != DBNull.Value ? (string)reader["USER_LOGIN"] : null;


                        Result result = new Result();
                        result.GUID = reader["GUID"] != DBNull.Value ? (string)reader["GUID"] : null;

                        enrollment.Result = result;
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
        public int CountUserIDPGroupByYear(string year, string id)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) " +
                                                        "FROM IDP_USER_ENROLL EN " +
                                                        "JOIN IDP_RESULT RE ON EN.ID = RE.ID AND EN.IDP_GROUP_ID = RE.IDP_GROUP_ID " +
                                                        "WHERE RE.YEAR = @Year AND EN.ID = @Id AND EN.STATUS != 'Decline'", connection))
            {

                command.Parameters.AddWithValue("@Year", year);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
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
        public int GetCompetencyAll(string id, string idpGroupId)
        {
            int all = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) " +
                                                        "FROM IDP_GROUP_ITEM I " +
                                                        "JOIN IDP_GROUP G ON G.IDP_GROUP_ID = I.IDP_GROUP_ID " +
                                                        "JOIN IDP_USER_ENROLL EN ON G.IDP_GROUP_ID = EN.IDP_GROUP_ID " +
                                                        "WHERE EN.ID = @Id AND EN.IDP_GROUP_ID = @IDPGroupId", connection))
            {

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

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
        public void InsertResultEmployees(List<User> selectedUsers, string year, string username, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (User user in selectedUsers)
                {
                    StringBuilder builder = new StringBuilder();
                    Enumerable
                       .Range(65, 26)
                        .Select(e => ((char)e).ToString()) 
                        .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                        .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                        .OrderBy(e => Guid.NewGuid())
                        .Take(11)
                        .ToList().ForEach(e => builder.Append(e));
                    string K2_No = "IDP_" + builder.ToString();
                    int competencyAll = GetCompetencyAll(user.Id, idpGroupId);

                    string resultQuery = "INSERT INTO IDP_RESULT (GUID, K2_NO, FORM_TYPE, FORM_ID, IDP_GROUP_ID, ID, COMPETENCY_ALL, COMPETENCY_PASS1, COMPETENCY_PASS2, COMPETENCY_PER1, COMPETENCY_PER2, " +
                                            "YEAR, RANK1, RANK2, SUBJECT, PLANT, DEPARTMENT, COMPANY_CODE, REQUISITIONER, REQUISITIONER_EMAIL, " +
                                            "CREATED_BY, CREATED_ON, STARTEDWF_ON, COMPLETED_ON, CURRENT_APPROVER, GR_LEVEL) " +
                                            "VALUES (@Guid, @K2No, 'IDP', 'IDP01', @IDPGroupId, @Id, @All, 0, 0, 0, 0, " +
                                            "@Year, NULL, NULL, @Subject, NULL, @Department, NULL, NULL, NULL, @CreateBy, GETDATE(), NULL, NULL, NULL, NULL)";

                    using (SqlCommand resultCommand = new SqlCommand(resultQuery, connection))
                    {
                        resultCommand.Parameters.AddWithValue("@Guid", Guid.NewGuid().ToString());
                        resultCommand.Parameters.AddWithValue("@K2No", K2_No);
                        resultCommand.Parameters.AddWithValue("@Id", user.Id);
                        resultCommand.Parameters.AddWithValue("@Year", year);
                        resultCommand.Parameters.AddWithValue("@All", competencyAll);
                        resultCommand.Parameters.AddWithValue("@Subject", user.Prefix + user.FirstNameTH + user.LastNameTH);
                        resultCommand.Parameters.AddWithValue("@Department", user.Department);
                        resultCommand.Parameters.AddWithValue("@CreateBy", username);
                        resultCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                        resultCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public void InsertResultEmployeesByUpload(User user, string year, string username, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                int competencyAll = GetCompetencyAll(user.Id, idpGroupId);

                string resultQuery = "INSERT INTO IDP_RESULT (GUID, K2_NO, FORM_TYPE, FORM_ID, IDP_GROUP_ID, ID, COMPETENCY_ALL, COMPETENCY_PASS1, COMPETENCY_PASS2, COMPETENCY_PER1, COMPETENCY_PER2, " +
                                        "YEAR, RANK1, RANK2, SUBJECT, PLANT, DEPARTMENT, COMPANY_CODE, REQUISITIONER, REQUISITIONER_EMAIL, " +
                                        "CREATED_BY, CREATED_ON, STARTEDWF_ON, COMPLETED_ON, CURRENT_APPROVER, GR_LEVEL) " +
                                        "VALUES (@Guid, NULL, 'IDP', 'IDP01', @IDPGroupId, @Id, @All, 0, 0, 0, 0, " +
                                        "@Year, NULL, NULL, @Subject, NULL, @Department, NULL, NULL, NULL, @CreateBy, GETDATE(), NULL, NULL, NULL, NULL)";

                using (SqlCommand resultCommand = new SqlCommand(resultQuery, connection))
                {
                    resultCommand.Parameters.AddWithValue("@Guid", Guid.NewGuid().ToString());
                    resultCommand.Parameters.AddWithValue("@Id", user.Id);
                    resultCommand.Parameters.AddWithValue("@Year", year);
                    resultCommand.Parameters.AddWithValue("@All", competencyAll);
                    resultCommand.Parameters.AddWithValue("@Subject", user.Prefix + user.FirstNameTH + user.LastNameTH);
                    resultCommand.Parameters.AddWithValue("@Department", user.Department);
                    resultCommand.Parameters.AddWithValue("@CreateBy", username);
                    resultCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    resultCommand.ExecuteNonQuery();
                }
                
            }
        }
        public void UpdateStartWorkFlow(string guid, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                string updateQuery = "UPDATE IDP_RESULT SET STARTEDWF_ON = GETDATE(), CURRENT_APPROVER = @Username WHERE GUID = @Guid";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Guid", guid);
                    updateCommand.Parameters.AddWithValue("@Username", username);

                    updateCommand.ExecuteNonQuery();
                }
                
            }
        }
        public void InsertResultEmployees2(List<IDPGroup> selectedIDPGroups, string username, string id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                User user = GetUserById(id);

                foreach (IDPGroup iDPGroup in selectedIDPGroups)
                {
                    StringBuilder builder = new StringBuilder();
                    Enumerable
                       .Range(65, 26)
                        .Select(e => ((char)e).ToString())
                        .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                        .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                        .OrderBy(e => Guid.NewGuid())
                        .Take(11)
                        .ToList().ForEach(e => builder.Append(e));
                    string K2_No = "IDP_" + builder.ToString();
                    //int competencyAll = GetCompetencyAllById(user.Id, year); // Get competencyAll for each user
                    int competencyAll = GetCompetencyAll(id, iDPGroup.IDPGroupId);

                    string resultQuery = "INSERT INTO IDP_RESULT (GUID, K2_NO, FORM_TYPE, FORM_ID, IDP_GROUP_ID, ID, COMPETENCY_ALL, COMPETENCY_PASS1, COMPETENCY_PASS2, COMPETENCY_PER1, COMPETENCY_PER2, " +
                                            "YEAR, RANK1, RANK2, SUBJECT, PLANT, DEPARTMENT, COMPANY_CODE, REQUISITIONER, REQUISITIONER_EMAIL, " +
                                            "CREATED_BY, CREATED_ON, STARTEDWF_ON, COMPLETED_ON, CURRENT_APPROVER, GR_LEVEL) " +
                                            "VALUES (@Guid, @K2No, 'IDP', 'IDP01', @IDPGroupId, @Id, @All, 0, 0, 0, 0, " +
                                            "@Year, NULL, NULL, @Subject, NULL, @Department, NULL, NULL, NULL, @CreateBy, GETDATE(), NULL, NULL, NULL, NULL)";

                    using (SqlCommand resultCommand = new SqlCommand(resultQuery, connection))
                    {
                        resultCommand.Parameters.AddWithValue("@Guid", Guid.NewGuid().ToString());
                        resultCommand.Parameters.AddWithValue("@K2No", K2_No);
                        resultCommand.Parameters.AddWithValue("@Id", id);
                        resultCommand.Parameters.AddWithValue("@Year", iDPGroup.Year);
                        resultCommand.Parameters.AddWithValue("@All", competencyAll);
                        resultCommand.Parameters.AddWithValue("@Subject", user.Prefix + user.FirstNameTH + user.LastNameTH);
                        resultCommand.Parameters.AddWithValue("@Department", user.Department);
                        resultCommand.Parameters.AddWithValue("@CreateBy", username);
                        resultCommand.Parameters.AddWithValue("@IDPGroupId", iDPGroup.IDPGroupId);

                        resultCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public List<User> GetUsersById(List<Enrollment> copyEnrolls)
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (Enrollment enrollment in copyEnrolls)
                {
                    string query = "SELECT * FROM MAS_USER_HR WHERE ID = @Id";

                    using (SqlCommand resultCommand = new SqlCommand(query, connection))
                    {
                        resultCommand.Parameters.AddWithValue("@Id", enrollment.Id);

                        using (SqlDataReader reader = resultCommand.ExecuteReader())
                        {
                            if (reader.Read())
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
                        }
                    }
                }
            }

            return users;
        }
        public void UpdateResultEmployeeAfterDeleteFromAddCompetency(int thisGroup, List<string> ids, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (string id in ids)
                {
                    //int othergroup = 0;
                    //if (getcountidpgroup(idpgroupid, year, id) > 0)
                    //{
                    //    othergroup = getcountcompetencyotherid(idpgroupid, year, id);
                    //}
                    string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_ALL = @ThisGroup WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@ThisGroup", thisGroup);
                        //updateCommand.Parameters.AddWithValue("@OtherGroup", otherGroup);
                        updateCommand.Parameters.AddWithValue("@Id", id);
                        updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
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

                string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = 'Self' WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId AND STATUS = 'Draft'";

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

                string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = '1st Evaluating' WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    updateCommand.ExecuteNonQuery();
                }

            }
        }
        public void UpdateEnrollmentStatus_3(string id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = 'Developing' WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    updateCommand.ExecuteNonQuery();
                }

            }
        }
        public void UpdateEnrollmentStatus_4(string id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = '2nd Evaluating' WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    updateCommand.ExecuteNonQuery();
                }

            }
        }
        public void UpdateEnrollmentStatus_5(string id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = 'Success' WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Id", id);
                    updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    updateCommand.ExecuteNonQuery();
                }

            }
        }
        public void UpdateEnrollmentStatus_6(string id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool canUpdate = GetCurrentStatus(id, idpGroupId);
                if(canUpdate)
                {
                    throw new Exception("ไม่สามารถลบได้");
                }
                else
                {
                    string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = 'Decline' WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Id", id);
                        updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public void UpdateEnrollmentStatus_7(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = "UPDATE IDP_USER_ENROLL SET STATUS = 'Delete' WHERE ENROLL_ID = @EnrollId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@EnrollId", enrollId);

                    updateCommand.ExecuteNonQuery();
                }

            }
        }
        public bool GetCurrentStatus(string id, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT STATUS FROM IDP_USER_ENROLL WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                    connection.Open();

                    string status = command.ExecuteScalar() as string;

                    return !string.IsNullOrEmpty(status) && (status == "Success" || status == "Draft");
                }
            }
        }
        public void DeleteEmployeeByIDPGroup(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool canDelete = CheckIfEnrollIsNotDraft(enrollId);
                //bool canUpdate = CheckIfEnrollIsDecline(enrollId);

                /*if (canUpdate)
                {
                    UpdateEnrollmentStatus_7(enrollId);
                }*/
                if(canDelete)
                {
                    throw new Exception("พนักงานคนนี้อยู่ในช่วงทำแบบประเมิน ไม่สามารถลบได้");
                }
                else
                {
                    string query = "DELETE FROM IDP_USER_ENROLL WHERE ENROLL_ID = @EnrollId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EnrollId", enrollId);

                    
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public bool CheckIfEnrollIsNotDraft(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT STATUS FROM IDP_USER_ENROLL WHERE ENROLL_ID = @EnrollId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EnrollId", enrollId);

                    connection.Open();

                    string status = command.ExecuteScalar() as string;

                    return !string.IsNullOrEmpty(status) && (status != "Draft");
                }
            }
        }
        public bool CheckIfEnrollIsDecline(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT STATUS FROM IDP_USER_ENROLL WHERE ENROLL_ID = @EnrollId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EnrollId", enrollId);

                    connection.Open();

                    string status = command.ExecuteScalar() as string;

                    return !string.IsNullOrEmpty(status) && (status == "Decline");
                }
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
        public void UpdateResultEmployeesById(List<string> ids, string idpGroupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (string id in ids)
                {
                    int competencyAll = GetCompetencyAll(id, idpGroupId); 

                    string updateQuery = "UPDATE IDP_RESULT SET COMPETENCY_ALL = @All WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@All", competencyAll);
                        updateCommand.Parameters.AddWithValue("@Id", id);
                        updateCommand.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

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
        public string GetStatus(string id, string idpGroupId)
        {
            string status = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT STATUS FROM IDP_USER_ENROLL WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupId";

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@IDPGroupId", idpGroupId);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    status = result.ToString();
                }
            }

            return status;
        }
        public string GetGuidById_IDPGroupId(string id, string idpGroupId)
        {
            string guid = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT GUID FROM IDP_RESULT WHERE ID = @Id AND IDP_GROUP_ID = @IDPGroupID";
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@IDPGroupID", idpGroupId);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    guid = result.ToString();
                }
            }

            return guid;
        }
        #endregion

        #region AUTH
        public List<UserFormAuth> GetUserFormAuths()
        {
            List<UserFormAuth> userFormAuths = new List<UserFormAuth>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM USER_FORM_AUTH";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    UserFormAuth userFormAuth = new UserFormAuth();

                    userFormAuth.Username = reader.IsDBNull(reader.GetOrdinal("USERNAME")) ? null : (string)reader["USERNAME"];
                    userFormAuth.FormId = reader.IsDBNull(reader.GetOrdinal("FORM_ID")) ? null : (string)reader["FORM_ID"];
                    userFormAuth.ObjectName = reader.IsDBNull(reader.GetOrdinal("OBJECT_NAME")) ? null : (string)reader["OBJECT_NAME"];
                    userFormAuth.Value = reader.IsDBNull(reader.GetOrdinal("VALUE")) ? null : (string)reader["VALUE"];
                    userFormAuth.GroupId = reader.IsDBNull(reader.GetOrdinal("GROUP_ID")) ? null : (string)reader["GROUP_ID"];

                    userFormAuths.Add(userFormAuth);
                }
                reader.Close();
            }

            return userFormAuths;
        }
        #endregion

        #region CLIENT
        public User GetUserByCookie(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM MAS_USER_HR WHERE USER_LOGIN = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
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

                            return user;
                        }
                    }
                }
            }
            return null;
        }
        public string GetIdByCookie(string username)
        {
            string id = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT ID FROM MAS_USER_HR WHERE USER_LOGIN = @Username";

                command.Parameters.AddWithValue("@Username", username);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    id = result.ToString();
                }
            }

            return id;
        }
        public bool is1stEvaluated(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT STATUS FROM IDP_USER_ENROLL WHERE ENROLL_ID = @EnrollId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EnrollId", enrollId);

                    object result = command.ExecuteScalar();

                    if (result != null && result.ToString().Equals("1st Evaluating", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        public bool isDeveloped(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT STATUS FROM IDP_USER_ENROLL WHERE ENROLL_ID = @EnrollId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EnrollId", enrollId);

                    object result = command.ExecuteScalar();

                    if (result != null && result.ToString().Equals("Developing", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        public bool is2ndEvaluated(int enrollId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT STATUS FROM IDP_USER_ENROLL WHERE ENROLL_ID = @EnrollId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EnrollId", enrollId);

                    object result = command.ExecuteScalar();

                    if (result != null && result.ToString().Equals("2nd Evaluating", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        public string GetJoblevelByCookie(string username)
        {
            string joblevel = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT JOBLEVEL FROM MAS_USER_HR WHERE USER_LOGIN = @Username";
                command.Parameters.AddWithValue("@Username", username);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    joblevel = result.ToString();
                }
            }

            return joblevel;
        }
        public string GetPositionByCookie(string username)
        {
            string position = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT POSITION FROM MAS_USER_HR WHERE USER_LOGIN = @Username";
                command.Parameters.AddWithValue("@Username", username);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    position = result.ToString();
                }
            }

            return position;
        }
        #endregion

        #region GOODNESS
        public List<Goodness> GetGoodness(string year)
        {
            List<Goodness> goodnesses = new List<Goodness>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * FROM IDP_GOODNESS G JOIN MAS_USER_HR HR ON G.NAME = HR.USER_LOGIN WHERE YEAR = @Year";

                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Goodness goodness = new Goodness();
                        goodness.GDId = (int)reader["GD_ID"];
                        goodness.Type = (string)reader["TYPE"];
                        goodness.Company = (string)reader["COMPANY"];
                        goodness.Desc = reader.IsDBNull(reader.GetOrdinal("DESCRIPTION")) ? null : (string)reader["DESCRIPTION"];
                        goodness.Date = (string)reader["DATE"];
                        goodness.Hour = (string)reader["HOUR"];
                        goodness.FileID = reader["FILE_ID"] != DBNull.Value ? (string)reader["FILE_ID"] : null;

                        User user = new User();
                        user.Prefix = reader["PREFIX"] != DBNull.Value ? (string)reader["PREFIX"] : null;
                        user.FirstNameTH = (string)reader["FIRSTNAME_TH"];
                        user.LastNameTH = (string)reader["LASTNAME_TH"];
                        user.Position = reader["POSITION"] != DBNull.Value ? (string)reader["POSITION"] : null;
                        user.DepartmentName = reader["DEPARTMENT_NAME"] != DBNull.Value ? (string)reader["DEPARTMENT_NAME"] : null;
                        user.JobLevel = reader["JOBLEVEL"] != DBNull.Value ? (string)reader["JOBLEVEL"] : null;
                        user.Company = reader["COMPANY"] != DBNull.Value ? (string)reader["COMPANY"] : null;

                        goodness.User = user;
                        goodnesses.Add(goodness);
                    }
                }
            }

            return goodnesses;
        }
        public List<Goodness> GetGoodnessByUser(string username, string year)
        {
            List<Goodness> goodnesses = new List<Goodness>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * FROM IDP_GOODNESS WHERE NAME = @Username AND YEAR = @Year";

                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Year", year);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Goodness goodness = new Goodness();
                        goodness.Type = (string)reader["TYPE"];
                        goodness.Company = (string)reader["COMPANY"];
                        goodness.Desc = reader.IsDBNull(reader.GetOrdinal("DESCRIPTION")) ? null : (string)reader["DESCRIPTION"];
                        goodness.Date = (string)reader["DATE"];
                        goodness.Hour = (string)reader["HOUR"];
                        goodness.FileID = (string)reader["FILE_ID"];
                        goodnesses.Add(goodness);
                    }
                }
            }

            return goodnesses;
        }
        public void InsertGoodness(List<Goodness> goodnessList, string user, string year)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (Goodness goodness in goodnessList)
                {
                    string query = "INSERT INTO IDP_GOODNESS (NAME, TYPE, COMPANY, DATE, HOUR, DESCRIPTION, YEAR, FILE_ID) " +
                                   "VALUES (@User, @Type, @Company, @Date, @Hour, @Desc, @Year, @File)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Type", goodness.Type);
                        command.Parameters.AddWithValue("@Company", goodness.Company);
                        command.Parameters.AddWithValue("@Date", goodness.Date);
                        command.Parameters.AddWithValue("@Hour", goodness.Hour);
                        command.Parameters.AddWithValue("@User", user);
                        command.Parameters.AddWithValue("@Desc", (object)goodness.Desc ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Year", year);
                        command.Parameters.AddWithValue("@File", goodness.FileID);

                        command.ExecuteNonQuery();
                    }
                }
            }

        }
        public void InsertGoodnessById(Goodness goodness, string id, string year)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string user = GetUserLoginById(id);

                
                string query = "INSERT INTO IDP_GOODNESS (NAME, TYPE, COMPANY, DATE, HOUR, DESCRIPTION, YEAR, FILE_ID) " +
                                "VALUES (@User, @Type, @Company, @Date, @Hour, @Desc, @Year, @FileId)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Type", goodness.Type);
                    command.Parameters.AddWithValue("@Company", goodness.Company);
                    command.Parameters.AddWithValue("@Date", goodness.Date);
                    command.Parameters.AddWithValue("@Hour", goodness.Hour);
                    command.Parameters.AddWithValue("@User", user);
                    command.Parameters.AddWithValue("@Desc", (object)goodness.Desc ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@FileID", goodness.FileID);


                    command.ExecuteNonQuery();
                }
                
            }
        }
        public string GetUserLoginById(string id)
        {
            string user = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT USER_LOGIN FROM MAS_USER_HR WHERE ID = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                {
                    user = result.ToString();
                }
            }

            return user;
        }
        public void UpdateGoodness(Goodness goodness)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE IDP_GOODNESS SET TYPE = @Type, DESCRIPTION = @Desc, DATE = @Date, HOUR = @Hour, COMPANY = @Company " +
                            "WHERE GD_ID = @GDId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Assign values to parameters
                    command.Parameters.AddWithValue("@GDId", (object)goodness.GDId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Type", (object)goodness.Type ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Desc", (object)goodness.Desc ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Date", (object)goodness.Date ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Hour", (object)goodness.Hour ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Company", (object)goodness.Company ?? DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void DeleteGoodness(int gdid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM IDP_GOODNESS WHERE GD_ID = @GDId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GDId", gdid);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public int GetCountGoodness(string username)
        {
            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM IDP_GOODNESS WHERE NAME = @Username";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }
        public List<User> GetListUserByGoodness(string company, string department)
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "SELECT * " +
                               "FROM MAS_USER_HR HR " +
                               "WHERE STATUS = 'ทำงาน'";

                if (company != null)
                {
                    query += " AND HR.COMPANY = @Company";
                }
                if (department != null)
                {
                    query += " AND HR.DEPARTMENT_NAME = @Department";
                }

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Company", (object)company ?? DBNull.Value);
                command.Parameters.AddWithValue("@Department", (object)department ?? DBNull.Value);
   
                if (company != null)
                {
                    command.Parameters.AddWithValue("@Control", 1);
                }
                else
                {
                    command.Parameters.AddWithValue("@Control", 0);

                }
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
                    
                    users.Add(user);
                }
                reader.Close();
            }

            return users;
        }
        #endregion

        #region REMARK
        public List<RemarkHS> GetRemark(string guid)
        {
            List<RemarkHS> remarks = new List<RemarkHS>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * FROM REMARK_HISTORY WHERE FORM_GUID = @Guid ORDER BY REMARK_DATE ASC";

                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RemarkHS remark = new RemarkHS();
                        remark.Name = (string)reader["NAME"];
                        remark.Position = (string)reader["POSITION"];
                        remark.Remark = reader.IsDBNull(reader.GetOrdinal("REMARK")) ? null : (string)reader["REMARK"];
                        remark.RemarkDate = reader.IsDBNull(reader.GetOrdinal("REMARK_DATE")) ? null : ((DateTime)reader["REMARK_DATE"]).ToString("yyyy-MM-dd");
                        remarks.Add(remark);
                    }
                }
            }

            return remarks;
        }
        public RemarkHS GetDescRemarkId(string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT TOP 1 * FROM REMARK_HISTORY WHERE FORM_GUID = @Guid ORDER BY REMARK_DATE DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Guid", guid);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            RemarkHS remark = new RemarkHS();
                            long idValue = reader.GetInt64(reader.GetOrdinal("ID"));
                            remark.Id = new BigInteger(idValue);

                            return remark;
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region WORKFLOW HISTORY
        public void InsertWorkflowHS0(string position, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO WORKFLOW_HISTORY (ACTION_DATE, ACTIVITY_NAME, ACTION, ACTION_BY, ACTION_BY_FULLNAME, REMARK_ID) " +
                                "VALUES (GETDATE(), 'Draft', 'Draft', @Username, @Fullname, NULL)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Fullname", username + "/" + position);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void InsertWorkflowHS1(string position, string username, string status)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO WORKFLOW_HISTORY (ACTION_DATE, ACTIVITY_NAME, ACTION, ACTION_BY, ACTION_BY_FULLNAME, REMARK_ID) " +
                                "VALUES (GETDATE(), @Status, @Status, @Username, @Fullname, NULL)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Fullname", username + "/" + position);
                    command.Parameters.AddWithValue("@Status", status);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void InsertWorkflowHS2(string position, string username, string status, RemarkHS remark)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO WORKFLOW_HISTORY (ACTION_DATE, ACTIVITY_NAME, ACTION, ACTION_BY, ACTION_BY_FULLNAME, REMARK_ID) " +
                                "VALUES (GETDATE(), @Status, @Status, @Username, @Fullname, @Remark)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Fullname", username + "/" + position);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@Remark", (long)remark.Id);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void InsertWorkflowHS3(string position, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO WORKFLOW_HISTORY (ACTION_DATE, ACTIVITY_NAME, ACTION, ACTION_BY, ACTION_BY_FULLNAME, REMARK_ID) " +
                                "VALUES (GETDATE(), 'Decline', 'Decline', @Username, @Fullname, NULL)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Fullname", username + "/" + position);

                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region DOWNLOAD
        public List<User> GetListDownload()
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * " +
                                "FROM MAS_USER_HR HR " +
                                "JOIN IDP_RESULT R ON HR.ID = R.ID " +
                                "JOIN IDP_USER_ENROLL EN ON EN.ID = R.ID AND EN.IDP_GROUP_ID = R.IDP_GROUP_ID";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User user = new User();

                    user.Id = (string)reader["ID"];
                    user.Company = reader.IsDBNull(reader.GetOrdinal("COMPANY")) ? null : (string)reader["COMPANY"];
                    user.JobLevel = reader.IsDBNull(reader.GetOrdinal("JOBLEVEL")) ? null : (string)reader["JOBLEVEL"];
                    user.CostCenter = reader.IsDBNull(reader.GetOrdinal("COSTCENTER")) ? null : (string)reader["COSTCENTER"];

                    Result result = new Result();
                    result.Year = (string)reader["YEAR"];

                    Enrollment enrollment = new Enrollment();
                    enrollment.Status = (string)reader["STATUS"];

                    user.Result = result;
                    users.Add(user);
                }
                reader.Close();
            }

            return users;
        }
        public List<User> GetListDownloadByFilter(string company, string year, string costCenter, string userId, string status)
        {
            List<User> users = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string query = "SELECT * " +
                               "FROM MAS_USER_HR HR " +
                               "JOIN IDP_RESULT R ON HR.ID = R.ID " +
                               "JOIN IDP_RESULT_ITEM RI ON RI.GUID = R.GUID " +
                               "JOIN IDP_USER_ENROLL EN ON R.IDP_GROUP_ID = EN.IDP_GROUP_ID " +
                               "WHERE 1 = @Control";

                if (company != null)
                {
                    query += " AND HR.COMPANY = @Company";
                }

                if (year != null)
                {
                    query += " AND R.YEAR = @Year";
                }

                if (costCenter != null)
                {
                    query += " AND HR.COSTCENTER = @CostCenter";
                }

                if (userId != null)
                {
                    query += " AND HR.ID = @UserId";
                }

                if (status != null)
                {
                    query += " AND EN.STATUS = @Status";
                }

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Company", (object)company ?? DBNull.Value);
                command.Parameters.AddWithValue("@Year", (object)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@CostCenter", (object)costCenter ?? DBNull.Value);
                command.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value);
                if (company != null || year != null || costCenter != null || userId != null || status != null)
                {
                    command.Parameters.AddWithValue("@Control", 1);
                }
                else
                {
                    command.Parameters.AddWithValue("@Control", 0);

                }
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

                    ResultItem resultItem = new ResultItem();
                    resultItem.ResultItemId = reader.IsDBNull(reader.GetOrdinal("RESULT_ITEM")) ? 0 : (int)reader["RESULT_ITEM"];
                    resultItem.Critical = reader.IsDBNull(reader.GetOrdinal("CRITICAL")) ? false : (bool)reader["CRITICAL"];
                    resultItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                    resultItem.IDPGroupId = reader.IsDBNull(reader.GetOrdinal("IDP_GROUP_ID")) ? null : (string)reader["IDP_GROUP_ID"];
                    resultItem.Requirement = reader.IsDBNull(reader.GetOrdinal("REQUIREMENT")) ? 0 : (int)reader["REQUIREMENT"];
                    resultItem.Actual1 = reader.IsDBNull(reader.GetOrdinal("ACTUAL1")) ? 0 : (int)reader["ACTUAL1"];
                    resultItem.Gap1 = reader.IsDBNull(reader.GetOrdinal("GAP1")) ? 0 : (int)reader["GAP1"];
                    resultItem.Priority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                    resultItem.TypePlan = reader.IsDBNull(reader.GetOrdinal("TYPE_PLAN")) ? null : (string)reader["TYPE_PLAN"];
                    resultItem.DevPlan = reader.IsDBNull(reader.GetOrdinal("DEV_PLAN")) ? null : (string)reader["DEV_PLAN"];
                    resultItem.Q1 = reader.IsDBNull(reader.GetOrdinal("Q1")) ? null : (string)reader["Q1"];
                    resultItem.Q2 = reader.IsDBNull(reader.GetOrdinal("Q2")) ? null : (string)reader["Q2"];
                    resultItem.Q3 = reader.IsDBNull(reader.GetOrdinal("Q3")) ? null : (string)reader["Q3"];
                    resultItem.Q4 = reader.IsDBNull(reader.GetOrdinal("Q4")) ? null : (string)reader["Q4"];
                    resultItem.DevRst = reader.IsDBNull(reader.GetOrdinal("DEV_RST")) ? null : (string)reader["DEV_RST"];
                    resultItem.Actual2 = reader.IsDBNull(reader.GetOrdinal("ACTUAL2")) ? 0 : (int)reader["ACTUAL2"];
                    resultItem.Gap2 = reader.IsDBNull(reader.GetOrdinal("GAP2")) ? 0 : (int)reader["GAP2"];

                    Result result = new Result();

                    result.IDPGroupID = reader.IsDBNull(reader.GetOrdinal("IDP_GROUP_ID")) ? null : (string)reader["IDP_GROUP_ID"];
                    result.Year = reader.IsDBNull(reader.GetOrdinal("YEAR")) ? null : (string)reader["YEAR"];

                    user.ResultItem = resultItem;
                    user.Result = result;

                    users.Add(user);
                }
                reader.Close();
            }

            return users;
        }
        #endregion

        #region INFO
        public List<Result> GetInfoEmployeeByGuid(string guid)
        {
            List<Result> results = new List<Result>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * " +
                                        "FROM IDP_GROUP G " +
                                        "LEFT JOIN IDP_GROUP_ITEM I ON I.IDP_GROUP_ID = G.IDP_GROUP_ID " +
                                        "LEFT JOIN IDP_COMPTY C ON I.COMPETENCY_ID = C.COMPETENCY_ID " +
                                        "RIGHT JOIN IDP_RESULT H ON G.IDP_GROUP_ID = H.IDP_GROUP_ID " +
                                        "LEFT JOIN IDP_RESULT_ITEM F ON C.COMPETENCY_ID = F.COMPETENCY_ID AND H.GUID = F.GUID " +
                                        "WHERE H.GUID = @Guid";


                command.Parameters.AddWithValue("@Guid", guid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        Result result = new Result();

                        result.CompetencyAll = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ALL")) ? 0 : (int)reader["COMPETENCY_ALL"];
                        result.CompetencyPass1 = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_PASS1")) ? 0 : (int)reader["COMPETENCY_PASS1"];
                        result.CompetencyPass2 = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_PASS2")) ? 0 : (int)reader["COMPETENCY_PASS2"];

                        if (reader.IsDBNull(reader.GetOrdinal("COMPETENCY_PER1")))
                        {
                            result.CompetencyPer1 = 0;
                        }
                        else if (float.TryParse(reader["COMPETENCY_PER1"].ToString(), out float competencyPer))
                        {
                            result.CompetencyPer1 = (float)Math.Round(competencyPer, 2);
                        }
                        if (reader.IsDBNull(reader.GetOrdinal("COMPETENCY_PER2")))
                        {
                            result.CompetencyPer2 = 0;
                        }
                        else if (float.TryParse(reader["COMPETENCY_PER2"].ToString(), out float competencyPer))
                        {
                            result.CompetencyPer2 = (float)Math.Round(competencyPer, 2);
                        }
                        result.Rank1 = reader.IsDBNull(reader.GetOrdinal("RANK1")) ? null : (string)reader["RANK1"];
                        result.Rank2 = reader.IsDBNull(reader.GetOrdinal("RANK2")) ? null : (string)reader["RANK2"];
                        result.Year = reader.IsDBNull(reader.GetOrdinal("YEAR")) ? null : (string)reader["YEAR"];

                        ResultItem resultItem = new ResultItem();
                        resultItem.Requirement = reader.IsDBNull(reader.GetOrdinal("REQUIREMENT")) ? 0 : (int)reader["REQUIREMENT"];
                        resultItem.Actual1 = reader.IsDBNull(reader.GetOrdinal("ACTUAL1")) ? 0 : (int)reader["ACTUAL1"];
                        resultItem.Gap1 = reader.IsDBNull(reader.GetOrdinal("GAP1")) ? 0 : (int)reader["GAP1"];
                        resultItem.Priority = reader.IsDBNull(reader.GetOrdinal("PRIORITY")) ? null : (string)reader["PRIORITY"];
                        resultItem.TypePlan = reader.IsDBNull(reader.GetOrdinal("TYPE_PLAN")) ? null : (string)reader["TYPE_PLAN"];
                        resultItem.DevPlan = reader.IsDBNull(reader.GetOrdinal("DEV_PLAN")) ? null : (string)reader["DEV_PLAN"];
                        resultItem.Q1 = reader.IsDBNull(reader.GetOrdinal("Q1")) ? null : (string)reader["Q1"];
                        resultItem.Q2 = reader.IsDBNull(reader.GetOrdinal("Q2")) ? null : (string)reader["Q2"];
                        resultItem.Q3 = reader.IsDBNull(reader.GetOrdinal("Q3")) ? null : (string)reader["Q3"];
                        resultItem.Q4 = reader.IsDBNull(reader.GetOrdinal("Q4")) ? null : (string)reader["Q4"];
                        resultItem.DevRst = reader.IsDBNull(reader.GetOrdinal("DEV_RST")) ? null : (string)reader["DEV_RST"];
                        resultItem.Actual2 = reader.IsDBNull(reader.GetOrdinal("ACTUAL2")) ? 0 : (int)reader["ACTUAL2"];
                        resultItem.Gap2 = reader.IsDBNull(reader.GetOrdinal("GAP2")) ? 0 : (int)reader["GAP2"];
                        resultItem.FileId = reader.IsDBNull(reader.GetOrdinal("FILE_ID")) ? null : (string)reader["FILE_ID"];

                        IDPGroup iDPGroup = new IDPGroup();
                        iDPGroup.IDPGroupName = (string)reader["IDP_GROUP_NAME"];
                        iDPGroup.Year = (string)reader["YEAR"];

                        IDPGroupItem idpGroupItem = new IDPGroupItem();
                        idpGroupItem.CompetencyId = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ID")) ? null : (string)reader["COMPETENCY_ID"];
                        idpGroupItem.Pl = reader.IsDBNull(reader.GetOrdinal("PL")) ? null : (string)reader["PL"];
                        idpGroupItem.Critical = reader.IsDBNull(reader.GetOrdinal("CRITICAL")) ? false : (bool)reader["CRITICAL"];

                        Competency competency = new Competency();
                        competency.CompetencyId = (string)reader["COMPETENCY_ID"];
                        competency.CompetencyNameTH = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_NAME_TH")) ? null : (string)reader["COMPETENCY_NAME_TH"];
                        competency.Pl1 = reader.IsDBNull(reader.GetOrdinal("PL1")) ? null : (string)reader["PL1"];
                        competency.Pl2 = reader.IsDBNull(reader.GetOrdinal("PL2")) ? null : (string)reader["PL2"];
                        competency.Pl3 = reader.IsDBNull(reader.GetOrdinal("PL3")) ? null : (string)reader["PL3"];
                        competency.Pl4 = reader.IsDBNull(reader.GetOrdinal("PL4")) ? null : (string)reader["PL4"];
                        competency.Pl5 = reader.IsDBNull(reader.GetOrdinal("PL5")) ? null : (string)reader["PL5"];
                        competency.Type = reader.IsDBNull(reader.GetOrdinal("TYPE")) ? null : (string)reader["TYPE"];


                        result.IDPGroup = iDPGroup;
                        result.IDPGroupItem = idpGroupItem;
                        result.Competency = competency;
                        result.ResultItem = resultItem;


                        results.Add(result);
                    }
                }
            }

            return results;
        }
        public Result GetResult(string guid)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM IDP_RESULT WHERE GUID = @Guid";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Guid", guid);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Result result = new Result();
                            result.CompetencyAll = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_ALL")) ? 0 : (int)reader["COMPETENCY_ALL"];
                            result.CompetencyPass1 = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_PASS1")) ? 0 : (int)reader["COMPETENCY_PASS1"];
                            result.CompetencyPass2 = reader.IsDBNull(reader.GetOrdinal("COMPETENCY_PASS2")) ? 0 : (int)reader["COMPETENCY_PASS2"];

                            if (reader.IsDBNull(reader.GetOrdinal("COMPETENCY_PER1")))
                            {
                                result.CompetencyPer1 = 0;
                            }
                            else if (float.TryParse(reader["COMPETENCY_PER1"].ToString(), out float competencyPer))
                            {
                                result.CompetencyPer1 = (float)Math.Round(competencyPer, 2);
                            }
                            if (reader.IsDBNull(reader.GetOrdinal("COMPETENCY_PER2")))
                            {
                                result.CompetencyPer2 = 0;
                            }
                            else if (float.TryParse(reader["COMPETENCY_PER2"].ToString(), out float competencyPer))
                            {
                                result.CompetencyPer2 = (float)Math.Round(competencyPer, 2);
                            }
                            result.Rank1 = reader.IsDBNull(reader.GetOrdinal("RANK1")) ? null : (string)reader["RANK1"];
                            result.Rank2 = reader.IsDBNull(reader.GetOrdinal("RANK2")) ? null : (string)reader["RANK2"];

                            return result;
                        }
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
