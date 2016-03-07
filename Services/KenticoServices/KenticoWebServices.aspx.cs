using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Classes.Search;
using Thinkgate.Domain.Classes;
using CMS.GlobalHelper;
using Thinkgate.Services.Contracts.LearningMedia;
using Thinkgate.Base.Enums;

namespace Thinkgate.Services.KenticoServices
{
    public partial class KenticoWebServices : System.Web.UI.Page
    {
        private static string rootConnectionString = ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ConnectionString;
        private static string cmsConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            var resource = Request.QueryString["resource"];
            var url = Request.QueryString["url"];

            if (resource == null || url == null) return;

            switch (resource)
            {
                case "PBSLearningMedia":
                    Response.Redirect(GetExternalResource(url).AbsoluteUri, true);
                    break;
            }
        }

        [System.Web.Services.WebMethod]
        public static string getsubtype(string type)
        {
            List<LookupDetails> resourceTypes = KenticoHelper.GetLookupDetailsByClient(null, null);

            var resultsubtype = new List<KeyValuePair>();

            if (type != "")
            {
                resultsubtype = (from s in resourceTypes
                                 where Convert.ToInt32(s.LookupEnum) == int.Parse(type)
                                 select new KeyValuePair { Key = s.Enum.ToString(), Value = s.Description }).ToList();
            }
            else
            {
                resultsubtype = (from s in resourceTypes
                                 where (from t in resourceTypes
                                        where t.LookupEnum == Base.Enums.LookupType.DocumentType
                                        select t.Enum.ToString()).ToList().Contains(s.LookupEnum.ToString())
                                 select new KeyValuePair { Key = s.Enum.ToString(), Value = s.Description }).Distinct(new KeyValueComparer()).ToList();
            }
            resultsubtype = resultsubtype.OrderBy(v => v.Value).ToList();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<option value=\"0\">Select</option>");

            foreach (var s in resultsubtype)
            {
                sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
            }

            return sb.ToString();
        }


        [System.Web.Services.WebMethod]
        public static string GetFormType(string subtype)
        {
            LookupDetail enumlkpDetail;
            string resourceToShow = string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (Enum.TryParse(subtype, true, out enumlkpDetail))
            {
                switch (enumlkpDetail)
                {
                    case LookupDetail.CurriculumMapForm:
                        resourceToShow = "thinkgate.InstructionalPlan";

                        break;
                    case LookupDetail.UnitPlanForm:
                        resourceToShow = "thinkgate.UnitPlan";
                        break;
                    case LookupDetail.LessonPlanForm:
                        resourceToShow = "thinkgate.LessonPlan";
                        break;
                    case LookupDetail.StateModelCurriculumForm:
                        resourceToShow = "thinkgate.curriculumUnit";
                        break;
                    case LookupDetail.StateModelCurriculumUnitForm:
                        resourceToShow = "thinkgate.UnitPlan";
                        break;
                    case LookupDetail.Resource:
                        resourceToShow = "thinkgate.resource";
                        break;

                }
                if (!string.IsNullOrEmpty(resourceToShow))
                {
                    DataSet tileMapDataSet = KenticoHelper.GetTileMapLookupDataSet(resourceToShow);
                    if (tileMapDataSet.Tables[0].Rows.Count > 1)
                    {
                        var resultsubtype = (from s in tileMapDataSet.Tables[0].AsEnumerable()
                                             select new KeyValuePair { Key = s["FriendlyName"].ToString(), Value = s["FriendlyName"].ToString() }).ToList();
                        resultsubtype = resultsubtype.OrderBy(v => v.Value).ToList();
                        foreach (var s in resultsubtype)
                        {
                            sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                        }
                        return sb.ToString();
                    }
                }
            }
            return null;
        }
        public static string StandardsByStandardSetGradeSubjectCourse(string standardSet, string grade, string subject, string course)
        {
            var dtStandards = Thinkgate.Base.Classes.Standards.GetStandardsByStandardSetGradeSubjectCourse(standardSet, grade, subject, course);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<option value=\"0\">Select Standards</option>");
            if (dtStandards != null)
            {
                var resultsubtype = (from s in dtStandards.AsEnumerable()
                                     select new KeyValuePair { Key = s["ID"].ToString(), Value = s["StandardName"].ToString() }).ToList();
                resultsubtype = resultsubtype.OrderBy(v => v.Value).ToList();
                foreach (var s in resultsubtype)
                {
                    sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                }
            }

            return sb.ToString();

        }

        [System.Web.Services.WebMethod]
        public static string getSubjects(string grade)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<option value=\"0\">Select Subject</option>");

            grade = string.IsNullOrWhiteSpace(grade) ? string.Empty : grade == "0" ? string.Empty : grade;
            Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            var dtStandards = Thinkgate.Base.Classes.Subject.GetSubjectListForDropDown(grade, sessionObject.LoggedInUser.Page, string.Empty);

            if (dtStandards != null)
            {
                var resultsubtype = (from s in dtStandards.AsEnumerable()
                                     select new KeyValuePair { Key = s["ListVal"].ToString(), Value = s["ListVal"].ToString() }).ToList();

                resultsubtype = resultsubtype.GroupBy(g => new { g.Value }).OrderBy(v => v.Key.Value).Select(r => new KeyValuePair()
                {
                    Key = r.Key.Value,
                    Value = r.Key.Value,
                }).ToList<KeyValuePair>();

                foreach (var s in resultsubtype)
                {
                    sb.Append("<option value='" + s.Key + "'> " + s.Value + "</option>");
                }
            }
            return sb.ToString();
        }

        [System.Web.Services.WebMethod]
        public static string getCurrCourses(string grade, string subject)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<option value=\"0\">Select Curriculum</option>");

            string sql = string.Empty;
            if (grade != "0" && subject != "0")
            {
                sql = string.Format("select distinct Course from currcourses where grade='{0}' and Subject='{1}' order by course", grade, subject);
            }
            else if (grade == "0" && subject != "0")
            {
                sql = string.Format("select distinct Course from currcourses where subject='{0}' order by course", subject);
            }
            else if (grade != "0" && subject == "0")
            {
                sql = string.Format("select distinct Course from currcourses where grade='{0}' order by course", grade);
            }
            else if (grade == "0" && subject == "0")
            {
                sql = string.Format("select distinct Course from currcourses order by course");
            }
            DataTable dataTable = CriteriaHelper.GetDataTable(rootConnectionString, sql);
            if (dataTable != null)
            {
                var resultsubtype = (from s in dataTable.AsEnumerable()
                                     select new KeyValuePair { Key = s["Course"].ToString(), Value = s["Course"].ToString() }).ToList();
                foreach (var s in resultsubtype)
                {
                    sb.Append("<option value='" + s.Key + "'> " + s.Value + "</option>");
                }
            }
            return sb.ToString();
        }

        //Standards Control for Resource Expanded Search.
        [System.Web.Services.WebMethod]
        public static string GetStandardGrade(string standardSet)
        {
            string sql = string.Format(@"select distinct Grade from Standards as s where s.standard_set='{0}' order by grade", standardSet);
            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<option value='0'>Select Grade</option>");
            if (standardsDataTable != null)
            {
                List<KeyValuePair> StandardSetTable = (from s in standardsDataTable.AsEnumerable()
                                                       select new KeyValuePair { Key = s["Grade"].ToString(), Value = s["Grade"].ToString() }).ToList<KeyValuePair>();
                foreach (var s in StandardSetTable)
                {
                    sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                }
            }

            return sb.ToString();



        }

        //Standards Control for Competency Tracking Report Portal 
        [System.Web.Services.WebMethod]
        public static string GetStandardGradeList(string standardSet)
        {
            string sql = string.Format(@"select distinct  Grade from Standards as s where s.standard_set='{0}' order by  grade", standardSet);
            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);
            return standardsDataTable.ToJSON(false);


        }


        [System.Web.Services.WebMethod]
        public static string GetStandardSubject(string standardSet, string grade)
        {
            string sql = string.Format("select distinct Subject from Standards as s where s.standard_set='{0}' and s.grade='{1}' order by subject", standardSet, grade);
            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<option value=\"0\">Select Subject</option>");
            if (standardsDataTable != null)
            {
                List<KeyValuePair> StandardSetTable = (from s in standardsDataTable.AsEnumerable()
                                                       select new KeyValuePair { Key = s["Subject"].ToString(), Value = s["Subject"].ToString() }).ToList<KeyValuePair>();
                foreach (var s in StandardSetTable)
                {
                    sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                }
            }

            return sb.ToString();
        }


        [System.Web.Services.WebMethod]
        public static string GetStandardSubjectList(string standardSet, string grade)
        {
            string sql = string.Format("select distinct Subject from Standards as s where s.standard_set='{0}' and s.grade='{1}' order by subject", standardSet, grade);
            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);

            return standardsDataTable.ToJSON(false);
        }

        [System.Web.Services.WebMethod]
        public static string GetStandardCourse(string standardSet, string grade, string subject)
        {
            string sql = string.Format("select distinct Course from Standards as s where s.standard_set='{0}' and s.grade='{1}' and s.subject='{2}' order by course", standardSet, grade, subject);
            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<option value=\"0\">Select Course</option>");
            if (standardsDataTable != null)
            {
                List<KeyValuePair> StandardSetTable = (from s in standardsDataTable.AsEnumerable()
                                                       select new KeyValuePair { Key = s["Course"].ToString(), Value = s["Course"].ToString() }).ToList<KeyValuePair>();
                StandardSetTable = StandardSetTable.OrderBy(v => v.Value).ToList();
                foreach (var s in StandardSetTable)
                {
                    sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                }
            }

            return sb.ToString();
        }

        [System.Web.Services.WebMethod]
        public static string GetStandardCourseList(string standardSet, string grade, string subject)
        {
            string sql = string.Format("select distinct Course from Standards as s where s.standard_set='{0}' and s.grade='{1}' and s.subject='{2}' order by course", standardSet, grade, subject);
            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);
            return standardsDataTable.ToJSON(false);
            
        }

        private static string GetCurrentDatabaseName(string connectionstringname)
        {
            SqlConnection sqlConnection = null;
            string name = string.Empty;
            try
            {
                sqlConnection = new SqlConnection(connectionstringname);
                sqlConnection.Open();

                name = sqlConnection.Database;
            }
            catch
            { }
            finally
            {
                if (sqlConnection != null)
                {
                    if (sqlConnection.State == ConnectionState.Open)
                        sqlConnection.Close();
                }
            }
            return name;
        }

        private static string getLocalDbConnectionString(string connString)
        {
            //return connString.Replace("ThinkgateConfig", ThinkgateKenticoHelper.FindDBName());
            return rootConnectionString;
        }

        [System.Web.Services.WebMethod]
        public static string getStandardSetGrade(string standardSet)
        {
            string sql = string.Format("select distinct Grade from Standards as s where s.standard_set='{0}' order by Grade asc", standardSet);
            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<option value=\"0\"> ----- Select Item ----- </option>");
                foreach (DataRow dr in dt.Rows)
                {
                    string rowItem = dr["Grade"].ToString();
                    if (!string.IsNullOrWhiteSpace(rowItem))
                    {
                        sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");
                    }
                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private static string getDistrictFromParms()
        {
            string districtId = string.Empty;
            string sql = @"SELECT Val FROM Parms Where Name = 'District'";
            string ConnectionString = getLocalDbConnectionString(rootConnectionString);
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                districtId = (string)command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                //something
            }
            finally
            {
                conn.Close();
            }


            return districtId;
        }

        [System.Web.Services.WebMethod]
        public static string getStandardsList(string standardSet, string standardSetVal, string grade, string gradeVal, string subject, string subjectVal, string course, string courseVal, string searchOption, string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = HttpContext.Current.Server.UrlDecode(searchText);
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT distinct st.ID, st.Standard_Set, st.Grade, st.Subject, st.Course, st.Level, st.StandardName, st.\"Desc\" as Description ");
            sb.Append("FROM  [Standards] st ");
            sb.Append("Inner Join  [StandardSets] ss on st.Standard_Set=ss.StandardSet ");
            sb.Append("Inner JOIN [StandardCourses] sc ");
            sb.Append("ON st.[Standard_Set] = sc.[StandardSet] ");
            sb.Append("WHERE  ISNULL(ss.[StandardsSearch], '') <> 'no' ");

            if (!string.IsNullOrWhiteSpace(gradeVal) && gradeVal != "0" && gradeVal != "null" && gradeVal != "undefined")
            {
                sb.Append(" AND st.[Grade] = '" + grade + "'");
            }
            if (!string.IsNullOrWhiteSpace(standardSetVal) && standardSetVal != "0" && standardSetVal != "null" && standardSetVal != "undefined")
            {
                sb.Append(" AND ss.[StandardSet] = '" + standardSet + "'");
            }
            if (!string.IsNullOrWhiteSpace(subjectVal) && subjectVal != "0" && subjectVal != "null" && subjectVal != "undefined")
            {
                sb.Append(" AND st.[Subject] = '" + subject + "'");
            }
            if (!string.IsNullOrWhiteSpace(courseVal) && courseVal != "0" && courseVal != "null" && courseVal != "undefined")
            {
                sb.Append(" AND st.Course = '" + course + "'");
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string[] searchWords = System.Text.RegularExpressions.Regex.Split(searchText, " ");
                string[] searchColumns = new string[] { "ss.StandardSet", " st.Grade", "st.StandardName", "st.[Level]", "st.Course", "st.\"Desc\"", "st.Subject" };

                if (searchOption == "any")
                {
                    string anyWordsCondition = TextSearchCondition(searchWords, searchColumns, true);
                    sb.Append(" And " + anyWordsCondition);
                }
                else if (searchOption == "all")
                {
                    string allWordsCondition = TextSearchCondition(searchWords, searchColumns, false);
                    sb.Append(" And " + allWordsCondition);
                }
                else if (searchOption == "exact")
                {
                    sb.Append(" AND ( ss.StandardSet LIKE '%" + searchText + "%' ");
                    sb.Append(" OR st.[Grade] LIKE '%" + searchText + "%' ");
                    sb.Append(" OR st.[StandardName] LIKE '%" + searchText + "%' ");
                    sb.Append(" OR st.Level LIKE '%" + searchText + "%' ");
                    sb.Append(" OR st.Course LIKE '%" + searchText + "%' ");
                    sb.Append(" OR st.Subject LIKE '%" + searchText + "%' ");
                    sb.Append(" OR st.\"Desc\" LIKE '%" + searchText + "%') ");
                }
            }

            string sql = sb.ToString();
            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                return dt.ToJSON(false);
            }
            else
            {
                return null;
            }
        }

        [System.Web.Services.WebMethod]
        public static string getStandardSetGradeSubjectCourse(string standardSet, string grade, string subject)
        {
            string sql = string.Format("SELECT DISTINCT s.[Course] FROM [StandardCourses] s JOIN [StandardSets] ss ON s.[StandardSet] = ss.[StandardSet] WHERE ISNULL(ss.[StandardsSearch], '') <> 'no' AND ss.[StandardSet] = '{0}' AND s.[Grade] = '{1}' AND s.[Subject] = '{2}' ORDER BY s.[Course]", standardSet, grade, subject);
            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");

                foreach (DataRow dr in dt.Rows)
                {
                    string rowItem = dr["Course"].ToString();

                    sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
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

        [System.Web.Services.WebMethod]
        public static string getStandardSetList()
        {
            string sql = "select distinct Standard_Set from Standards";
            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
                foreach (DataRow dr in dt.Rows)
                {
                    string rowItem = dr["Standard_Set"].ToString();

                    sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        [System.Web.Services.WebMethod]
        public static string getStandardSetGradeSubject(string standardSet, string grade)
        {
            //string sql = string.Format("select distinct Subject from Standards as s where s.standard_set='{0}' and s.grade='{1}' order by Subject asc", standardSet, grade);
            string sql = string.Format("SELECT DISTINCT s.[Subject] FROM  [StandardCourses] s JOIN [StandardSets] ss ON s.[StandardSet] = ss.[StandardSet]  WHERE ISNULL(ss.[StandardsSearch], '') <> 'no' AND ss.[StandardSet] = '{0}' AND s.[Grade] = '{1}' ORDER BY s.[Subject]", standardSet, grade);
            DataTable dt = GetDataTable(getLocalDbConnectionString(rootConnectionString), sql);
            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
                foreach (DataRow dr in dt.Rows)
                {
                    string rowItem = dr["Subject"].ToString();

                    sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        [System.Web.Services.WebMethod]
        public static string GetAssociatedStandards(string standardSet, string grade, string subject, string course)
        {
            EnvironmentParametersFactory ef = new EnvironmentParametersFactory(AppSettings.ConnectionStringName);

            string sql = string.Format(@"select distinct dr.standardid, s.StandardName
                                        from {0}.dbo.thinkgate_doctostandards dr
                                        inner join standards s on s.id=dr.standardid  
                                        where s.standard_set='{1}' and s.grade='{2}' and s.subject='{3}' and s.course='{4}'                                     
                                        order by s.StandardName", GetCurrentDatabaseName(cmsConnectionString), standardSet, grade, subject, course);
            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);

            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=\"0\">Select Standards</option>");
            if (standardsDataTable != null)
            {
                List<KeyValuePair> StandardsTable = (from s in standardsDataTable.AsEnumerable()
                                                     select new KeyValuePair { Key = s["StandardID"].ToString(), Value = s["StandardName"].ToString() }).ToList<KeyValuePair>();

                foreach (var s in StandardsTable)
                {
                    sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                }
            }

            return sb.ToString();
        }

        [System.Web.Services.WebMethod]
        public static string GetStandardsByStandardSetGradeSubjectCourse(string standardSet, string grade, string subject, string course)
        {
            string sql = string.Format(@"select s.ID, s.Standard_Set, s.Grade, s.Subject, s.Course, s.Level, s.StandardName
                                        from   {0}..[Standards] s join {0}..[StandardSets] ss on s.[Standard_Set] = ss.[StandardSet]
                                        where  isnull(ss.[StandardsSearch], '') <> 'no' 
                                        and ss.[StandardSet] = '{1}' and s.[Grade] = '{2}' and s.[Subject] = '{3}' and s.Course = '{4}'
                                        order by s.[StandardName]", GetCurrentDatabaseName(rootConnectionString), standardSet, grade, subject, course);

            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<option value=\"0\">Select Standards</option>");
            if (standardsDataTable != null)
            {
                List<KeyValuePair> StandardsTable = (from s in standardsDataTable.AsEnumerable()
                                                     select new KeyValuePair { Key = s["ID"].ToString(), Value = s["StandardName"].ToString() }).ToList<KeyValuePair>();

                foreach (var s in StandardsTable)
                {
                    sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                }
            }

            return sb.ToString();
        }


        [System.Web.Services.WebMethod]
        public static string GetStandardsByStandardSetGradeSubjectCourseList(string standardSet, string grade, string subject, string course)
        {
            string sql = string.Format(@"select s.ID, s.StandardName
                                        from   {0}..[Standards] s join {0}..[StandardSets] ss on s.[Standard_Set] = ss.[StandardSet]
                                        where  isnull(ss.[StandardsSearch], '') <> 'no' 
                                        and ss.[StandardSet] = '{1}' and s.[Grade] = '{2}' and s.[Subject] = '{3}' and s.Course = '{4}'
                                        order by s.[StandardName]", GetCurrentDatabaseName(rootConnectionString), standardSet, grade, subject, course);

            DataTable standardsDataTable = GetDataTable(rootConnectionString, sql);

            return standardsDataTable.ToJSON(false);
        }


        /// <summary>
        /// GetExternalResource - Returns a Uri after going through Login By Pass to access media resource.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Uri</returns>
        [System.Web.Services.WebMethod]
        public static Uri GetExternalResource(string url)
        {
            if (url == null) throw new ArgumentNullException("url");

            var wgbhKey = ConfigurationManager.AppSettings["WGBHKey"];
            var wgbhCreationToken = ConfigurationManager.AppSettings["WGBHCreationTokenUri"];
            var wgbhUserEmail = ConfigurationManager.AppSettings["WGBHUserEmail"];
            var wgbhLearningMediaId = ConfigurationManager.AppSettings["WGBHLearningMediaID"];

            var learningMediaLoginByPassInfo = new LearningMediaLoginByPassInfo
            {
                Key = wgbhKey,
                CreationTokenUri = new Uri(wgbhCreationToken),
                UserEmail = wgbhUserEmail,
                LearningMediaUserId = wgbhLearningMediaId
            };
            var learningMediaProxy = new LearningMediaProxy();
            var redirectUrl = learningMediaProxy.GetLoginByPassUriForResource(learningMediaLoginByPassInfo, url);
            return redirectUrl;
        }
        private static DataTable GetDataTable(string connectionString, string selectSQL)
        {
            DataTable dataTable = new DataTable();

            if (selectSQL != null)
            {
                string connectionStringToUse = string.Empty;

                if (connectionString != null)
                { connectionStringToUse = connectionString; }
                else
                { connectionStringToUse = rootConnectionString; }

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

        //Teacher Control for Resource Expanded Search.
        [System.Web.Services.WebMethod]
        public static string GetSchoolGrade(string schoolSet)
        {
            string sql = string.Format(@"SELECT  DISTINCT  (Select  convert(nvarchar(25),Val) From Parms where Name='ClientId')AS ClientID,  c.district,            
                                                            S.ID AS School,S.SchoolName,au.UserId,au.LastName,au.FirstName,au.MiddleName, CASE WHEN au.MiddleName IS NULL OR au.MiddleName=' ' THEN au.LastName  + ', ' +au.FirstName
                                                            ELSE  au.LastName + ', ' +au.FirstName  +', ' +au.MiddleName END TeacherNameFull , au.UserName,cc.Grade  FROM Schools  S    
                                                            INNER JOIN Classes  c ON S.ID =C.School AND  S.ID={0}       
                                                            INNER JOIN ClassCourses  cc On cc.Id = c.ClassCourseId       
                                                            INNER JOIN CTXref  ctx ON CTX.CLASSPAGE=C.ID and ctx.Type='primary'   
                                                            INNER JOIN  aspnet_Users au ON au.Page = ctx.TeacherPage       
                                                            where au.District= c.District Order by au.LastName,au.FirstName,au.MiddleName ASC", schoolSet);

            DataTable schoolsGradeDataTable = GetDataTable(rootConnectionString, sql);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            schoolsGradeDataTable.DefaultView.Sort = "Grade ASC";
            DataTable schoolGradDataTable = schoolsGradeDataTable.DefaultView.ToTable(true, "Grade");


            if (schoolsGradeDataTable.Rows.Count == 1)
            {
                if (schoolsGradeDataTable != null)
                {
                    List<KeyValuePair> GradKeyValuePairList = (from s in schoolsGradeDataTable.AsEnumerable()
                                                               select new KeyValuePair { Key = s["Grade"].ToString(), Value = s["Grade"].ToString() }).ToList<KeyValuePair>();
                    foreach (var s in GradKeyValuePairList)
                    {
                        sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                    }

                    sb.Append("$");

                    List<KeyValuePair> TeacherKeyValuePairList = (from s in schoolsGradeDataTable.AsEnumerable()
                                                                  select new KeyValuePair { Key = s["TeacherNameFull"].ToString(), Value = s["UserName"].ToString() }).ToList<KeyValuePair>();

                    foreach (var s in TeacherKeyValuePairList)
                    {
                        if (s.Key != "" || !string.IsNullOrEmpty(s.Key))
                        {
                            sb.Append("<option value='" + s.Value + "'> " + s.Key + " </option>");
                        }
                    }
                }
            }
            else
            {
                if (schoolGradDataTable.Rows.Count == 1)
                {
                    if (schoolGradDataTable != null)
                    {
                        List<KeyValuePair> GradKeyValuePairList = (from s in schoolGradDataTable.AsEnumerable()
                                                                   select new KeyValuePair { Key = s["Grade"].ToString(), Value = s["Grade"].ToString() }).ToList<KeyValuePair>();
                        foreach (var s in GradKeyValuePairList)
                        {
                            sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                        }

                        sb.Append("$");

                        schoolsGradeDataTable.DefaultView.Sort = "TeacherNameFull ASC";
                        DataTable schoolGradUserDataTable = schoolsGradeDataTable.DefaultView.ToTable(true, new string[] { "TeacherNameFull", "UserName" });

                        sb.Append("<option value='0'>Select Name</option>");

                        if (schoolGradUserDataTable != null)
                        {
                            List<KeyValuePair> UserNameKeyValuePairList = (from s in schoolGradUserDataTable.AsEnumerable()
                                                                           select new KeyValuePair { Key = s["TeacherNameFull"].ToString(), Value = s["UserName"].ToString() }).ToList<KeyValuePair>();
                            foreach (var s in UserNameKeyValuePairList)
                            {
                                if (s.Key != "" || !string.IsNullOrEmpty(s.Key))
                                {
                                    sb.Append("<option value='" + s.Value + "'> " + s.Key + " </option>");
                                }
                            }
                        }

                    }
                }
                else
                {
                    sb.Append("<option value='0'>Select Grade</option>");

                    if (schoolGradDataTable != null)
                    {
                        List<KeyValuePair> GradKeyValuePairList = (from s in schoolGradDataTable.AsEnumerable()
                                                                   select new KeyValuePair { Key = s["Grade"].ToString(), Value = s["Grade"].ToString() }).ToList<KeyValuePair>();
                        foreach (var s in GradKeyValuePairList)
                        {
                            sb.Append("<option value='" + s.Key + "'> " + s.Value + " </option>");
                        }
                    }
                }

            }
            return sb.ToString();
        }
        [System.Web.Services.WebMethod]
        public static string GetSchoolGradeName(string gradeSet, string schoolSet)
        {
            string sql = string.Format(@" SELECT DISTINCT Top 10 (Select  convert(nvarchar(25),Val) From Parms where Name='ClientId')AS ClientID,  c.district,   
                                                                       S.ID AS School,S.SchoolName,au.UserId,au.UserId,au.LastName,au.FirstName,au.MiddleName,
                                                                      CASE WHEN au.MiddleName IS NULL OR au.MiddleName=' ' THEN au.LastName  + ', ' +au.FirstName
                                                           ELSE  au.LastName + ', ' +au.FirstName  +', ' +au.MiddleName END TeacherNameFull , au.UserName,cc.Grade  FROM Schools  S    
                                                           INNER JOIN Classes  c ON S.ID =C.School AND S.ID={0}        
                                                           INNER JOIN ClassCourses  cc On cc.Id = c.ClassCourseId  AND cc.Grade='{1}'     
                                                           INNER JOIN CTXref  ctx ON CTX.CLASSPAGE=C.ID and ctx.Type='primary'
                                                           INNER JOIN  aspnet_Users au ON au.Page = ctx.TeacherPage           
                                                           where au.District= c.District Order by au.LastName,au.FirstName,au.MiddleName ASC ", schoolSet, gradeSet);

            DataTable GradeTeacherNamerDataTable = GetDataTable(rootConnectionString, sql);

            GradeTeacherNamerDataTable.DefaultView.Sort = "TeacherNameFull ASC";
            DataTable schoolGradDataTable = GradeTeacherNamerDataTable.DefaultView.ToTable(true, new string[] { "TeacherNameFull", "UserName" });

            System.Text.StringBuilder sb = new System.Text.StringBuilder();


            sb.Append("<option value='0'>Select Name</option>");

            if (schoolGradDataTable != null)
            {
                if (schoolGradDataTable.Rows.Count == 1)
                {
                    sb.Clear();
                }

                List<KeyValuePair> GradKeyValuePairList = (from s in schoolGradDataTable.AsEnumerable()
                                                           select new KeyValuePair { Key = s["TeacherNameFull"].ToString(), Value = s["UserName"].ToString() }).ToList<KeyValuePair>();
                foreach (var s in GradKeyValuePairList)
                {
                    if (s.Key != "" || !string.IsNullOrEmpty(s.Key))
                    {
                        sb.Append("<option value='" + s.Value + "'> " + s.Key + " </option>");
                    }
                }
            }
            return sb.ToString();
        }

    }
}