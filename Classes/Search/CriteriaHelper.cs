
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Thinkgate.Classes.Search;
using System.Text;
using Thinkgate.Base.Classes;
using System;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.DataAccess;

namespace Thinkgate.Classes.Search
{
    /// <summary>
    /// Member of DAL
    /// </summary>
    public class CriteriaHelper
    {
        private static string rootConnectionString = ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ConnectionString;

        #region Public Static Methods

        public static List<KeyValuePair> GetStandardSetList()
        {
            string sql = "SELECT [StandardSet] FROM [StandardSets] WHERE  ISNULL([StandardsSearch], '') <> 'no'";
            
            List<KeyValuePair> standardSets = new List<KeyValuePair>();
            //standardSets.Insert(0, new KeyValuePair("0", " -- Select --"));

            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    KeyValuePair standardSet = new KeyValuePair(dr["StandardSet"].ToString(), dr["StandardSet"].ToString());
                    standardSets.Add(standardSet);
                }
            }
            return standardSets;
        }

        public static List<KeyValuePair> GetGradesList()
        {
            string sql = string.Format("SELECT DISTINCT Grade FROM CurrCourses C WHERE C.Active = 'Yes'  ORDER BY Grade ASC");

            List<KeyValuePair> grades = new List<KeyValuePair>();
            //grades.Insert(0, new KeyValuePair("0", " -- Select --"));

            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    KeyValuePair grade = new KeyValuePair(dr["Grade"].ToString(), dr["Grade"].ToString());
                    grades.Add(grade);
                }
            }
            return grades;
        }

        public static List<KeyValuePair> GetSubjectsList(string grade)
        {
            string sql = string.Empty;
            if (!string.IsNullOrWhiteSpace(grade))
            {
                sql = string.Format("SELECT DISTINCT Subject FROM CurrCourses AS C WHERE C.Grade='{0}' AND C.Active='{1}'", grade, "Yes");
            }
            else
            {
                sql = string.Format("SELECT DISTINCT Subject FROM CurrCourses AS C WHERE C.Active='Yes'");
            }

            List<KeyValuePair> subjects = new List<KeyValuePair>();
            //subjects.Insert(0, new KeyValuePair("0", " -- Select --"));

            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    KeyValuePair subject = new KeyValuePair(dr["Subject"].ToString(), dr["Subject"].ToString());
                    subjects.Add(subject);
                }
            }
            return subjects;
        }

        public static List<KeyValuePair> GetCoursesList(string standardSet, string grade, string subject)
        {
            string sql = string.Empty;
            if (!string.IsNullOrWhiteSpace(standardSet))
            {
                sql = string.Format("SELECT DISTINCT Course FROM Standards AS S WHERE S.standard_set='{0}' AND S.grade='{1}' AND S.subject='{2}' ORDER BY Course ASC", standardSet, grade, subject);
            }
            else
            {
                sql = string.Format("SELECT DISTINCT Course FROM Standards ORDER BY Course ASC");
            }

            List<KeyValuePair> courses = new List<KeyValuePair>();
            courses.Insert(0, new KeyValuePair("0", " -- Select --"));

            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    KeyValuePair course = new KeyValuePair(dr["Course"].ToString(), dr["Course"].ToString());
                    courses.Add(course);
                }
            }
            return courses;
        }

        /// <summary>
        /// Do a string parse of the criteria and build a string based SQL query.
        /// </summary>
        /// <param name="standardSet"></param>
        /// <param name="grade"></param>
        /// <param name="subject"></param>
        /// <param name="course"></param>
        /// <param name="searchOption"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static List<ExampleDataDTO> GetFilteredData(string standardSet, string grade, string subject, string course, string searchOption, string searchText)
        {
            List<ExampleDataDTO> gridData = new List<ExampleDataDTO>();

            string sql = "SELECT StandardName, Term, Level, [Desc] FROM Standards S ";
            string whereClause = string.Empty;
            string setsClause = string.Empty;
            string gradesClause = string.Empty;
            string subjectsClause = string.Empty;

            if (!string.IsNullOrWhiteSpace(standardSet))
            {
                string[] sets = standardSet.Split(new char[1] { ',' });
                setsClause += string.Join(",", sets.Select(t => "'" + t.Trim() + "'").ToArray());
                whereClause += " WHERE S.standard_set IN(" + setsClause + ") ";
            }
            if (!string.IsNullOrWhiteSpace(grade))
            {
                string[] grades = grade.Split(new char[1] { ',' });
                gradesClause += string.Join(",", grades.Select(g => "'" + g.Trim() + "'").ToArray());
                if (string.IsNullOrWhiteSpace(whereClause))
                {
                    whereClause += " WHERE S.grade IN(" + gradesClause + ") ";
                }
                else
                {
                    whereClause += " AND S.grade IN(" + gradesClause + ") ";
                }
            }
            if (!string.IsNullOrWhiteSpace(subject))
            {
                string[] subjects = subject.Split(new char[1] { ',' });
                subjectsClause += string.Join(",", subjects.Select(s => "'" + s.Trim() + "'").ToArray());
                if (string.IsNullOrWhiteSpace(whereClause))
                {
                    whereClause += " WHERE S.subject IN(" + subjectsClause + ")";
                }
                else
                {
                    whereClause += " AND S.subject IN(" + subjectsClause + ")";
                }
            }
            if (!string.IsNullOrWhiteSpace(course))
            {
                if (string.IsNullOrWhiteSpace(whereClause))
                {
                    whereClause += string.Format(" WHERE Course='{0}'", course);
                }
                else
                {
                    whereClause += string.Format(" AND Course='{0}'", course);
                }
            }
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string[] searchWords = System.Text.RegularExpressions.Regex.Split(searchText, " ");
                string[] searchColumns = new string[] { "StandardName", "Subject", "Course", "Level", "[Desc]" };

                if (string.IsNullOrWhiteSpace(whereClause))
                {
                    whereClause += " WHERE ";
                }
                else
                {
                    whereClause += " AND ";
                }

                if (searchOption == "key")
                {
                    whereClause += " (Keywords LIKE '%" + searchText + "%') ";
                }
                else if (searchOption == "any")
                {
                    string anyWordsCondition = TextSearchCondition(searchWords, searchColumns, true);
                    whereClause += anyWordsCondition;
                }
                else if (searchOption == "all")
                {
                    string allWordsCondition = TextSearchCondition(searchWords, searchColumns, false);
                    whereClause += allWordsCondition;
                }
                else if (searchOption == "exact")
                {
                    whereClause += " ( StandardName LIKE '%" + searchText + "%' ";
                    whereClause += " OR Subject LIKE '%" + searchText + "%' ";
                    whereClause += " OR Course LIKE '%" + searchText + "%' ";
                    whereClause += " OR Level LIKE '%" + searchText + "%' ";
                    whereClause += " OR [Desc] LIKE '%" + searchText + "%') ";
                }
            }

            if (!string.IsNullOrWhiteSpace(whereClause))
                sql += whereClause;
            sql += " ORDER BY 1 ";

            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ExampleDataDTO dto = new ExampleDataDTO();

                    dto.StandardName = row["StandardName"].ToString();
                    dto.Term = row["Term"].ToString();
                    dto.Level = row["Level"].ToString();
                    dto.Description = row["Desc"].ToString();

                    gridData.Add(dto);
                }
            }
            return gridData;
        }

        private static string TextSearchCondition(string[] searchWords, string[] searchColumns, bool anyWords)
        {
            StringBuilder perColumnCondition = new StringBuilder(" (");
            for (int j = 0; j < searchColumns.Length; j++)
            {
                StringBuilder perWordCondition = new StringBuilder(" (");
                for (int i = 0; i < searchWords.Length; i++)
                {
                    if (i > 0)
                    {
                        perWordCondition.Append(anyWords == true ? " OR " : " AND ");
                    }
                    perWordCondition
                        .Append(" ")
                        .Append(searchColumns[j])
                        .Append(" LIKE '%")
                        .Append(searchWords[i])
                        .Append("%' ");
                }
                perWordCondition.Append(") ");
                if (j > 0)
                    perColumnCondition.Append(" OR ").Append(perWordCondition);
                else
                    perColumnCondition.Append(perWordCondition);
            }
            perColumnCondition.Append(") ");
            return perColumnCondition.ToString();
        }

        public static string getLocalDbConnectionString(string connectionString)
        {
            // TODO: Logic to replace config DB with client DB...
            return connectionString;
        }

        /// <summary>
        /// Move to DAL
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="selectSQL"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string connectionString, string selectSQL)
        {
            DataTable dataTable = new DataTable();

            if (selectSQL != null)
            {
                string connectionStringToUse = string.Empty;

                if (connectionString != null)
                {
                    connectionStringToUse = connectionString;
                }
                else
                {
                    connectionStringToUse = rootConnectionString;
                }

                SqlConnection sqlConnection = new SqlConnection(connectionStringToUse);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = new SqlCommand(selectSQL, sqlConnection);

                try
                {
                    sqlConnection.Open();
                }
                catch (SqlException ex)
                {
                    return dataTable;
                }

                try
                {
                    sqlDataAdapter.Fill(dataTable);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return dataTable;
        }

        #endregion

        public static List<KeyValuePair> GetStandardLevelList(string StandardSet)
        {
            string sql = "SELECT distinct [Level] FROM [Standards] WHERE  Standard_Set = '" + StandardSet + "' ";

            List<KeyValuePair> standardSets = new List<KeyValuePair>();
            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    KeyValuePair standardSet = new KeyValuePair(dr["Level"].ToString(), dr["Level"].ToString());
                    standardSets.Add(standardSet);
                }
            }
            return standardSets;
        }

        /// <summary>
        /// Get Standard Levels By StandardList Data 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable GetStandardLevelbyStandardList(Thinkgate.Base.DataAccess.dtGeneric_Int ids,string StandardSet, int WorksheetId)
        {
            SqlParameterCollection parms = new SqlCommand().Parameters;
            SqlParameter idCollections = new SqlParameter("StandardIds", SqlDbType.Structured);
            idCollections.TypeName = "dbo.Generic_Int";
            idCollections.Value = ids;
            parms.Add(idCollections);
            parms.AddWithValue("StandardSet", StandardSet);
            parms.AddWithValue("WorksheetId", WorksheetId);

            DataTable dt = ThinkgateDataAccess.FetchDataTable(AppSettings.ConnectionString, StoredProcedures.E3_GET_STANDARDLEVEL_BY_STANDARDLIST, CommandType.StoredProcedure, parms);
            return dt;
        }
    }
}