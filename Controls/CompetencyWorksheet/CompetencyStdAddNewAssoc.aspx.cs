using CMS.GlobalHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.Controls.CompetencyWorksheet
{
    public partial class CompetencyStdAddNewAssoc : BasePage
    {
        public int Assoccount;

        private static readonly string CMSConnectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        private static readonly string ElementsConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
        private Thinkgate.Base.DataAccess.dtGeneric_Int _standardids = new Base.DataAccess.dtGeneric_Int();
        public int AssociationCount;
        public string _isNew { get { return Request.QueryString["IsNew"]; } }

		private static readonly string DefaultStandardSets = DistrictParms.LoadDistrictParms().AddRemove_Competency_DefaultStandardSetList;

        public string PreviousPage
        {
            get
            {
                return QueryHelper.GetString("prevpage", "");
            }
        }

        public string DocumentID
        {
            get
            {
                return Request.QueryString["parentnodeid"];
            }
        }

        public string EncryptedWrkshtId { get { return Standpoint.Core.Classes.Encryption.EncryptInt(Convert.ToInt32(Session["docId"])); }  }
        private Thinkgate.Base.DataAccess.dtGeneric_Int standards
        {
            get
            {
                string[] selitems = (SelectedItems.Value).Split(',');
                for (var i = 0; i < selitems.Length; i++)
                {
                    int nodeid;
                    string docEntry = selitems[i];
                    if (Int32.TryParse(docEntry, out nodeid))
                    {
                        _standardids.Add(nodeid);
                    }
                }
                return _standardids;
            }

        }


        public string ClientDatabaseName
        {
            get
            {
                return QueryHelper.GetString("clientid", "");
            }
        }
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DocID.Value = QueryHelper.GetString("parentnodeid", "");
                Session["docId"] = DocID.Value;                

            }           
        }
        /// <summary>
        /// Add new standards into Worksheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddSelectedItems_Click(object sender, EventArgs e)
        {
            string docid = this.DocumentID;
            CompetencyWorkSheet.AddDeleteStandardIntoWorksheet(docid, standards, 1, PreviousPage!=""?1:0);
            SelectedItems.Value = null;
          
        }
        /// <summary>
        /// Delete standard from worksheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DelSelectedItems_Click(object sender, EventArgs e)
        {
            string docid = this.DocumentID;
            CompetencyWorkSheet.AddDeleteStandardIntoWorksheet(docid, standards, 2);
            SelectedItems.Value = null;
        }
        /// <summary>
        /// Validate UI
        /// </summary>
        /// <param name="SelectedItems"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string ValidateUI(string SelectedItems)
        {
            
            string DocumentID = Convert.ToString(System.Web.HttpContext.Current.Session["docId"]);
            string[] selitems = SelectedItems.Split(',');
            DataTable dt = CompetencyWorkSheet.GetWorksheetById(DocumentID, "getWorksheetStandards"); ;

            string[] availablestandards = (from row in dt.AsEnumerable()
                                           select row[0].ToString()).ToArray();


            var dd = selitems.Except(availablestandards).ToList();

            return (dd.Count() + availablestandards.Count()).ToString();
        }

        /// <summary>
        /// Validate UI
        /// </summary>
        /// <param name="SelectedItems"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string ValidateWorksheetHistory(string SelectedItems)
        {
            string Counter="0";
            string DocumentID = Convert.ToString(System.Web.HttpContext.Current.Session["docId"]);
            Thinkgate.Base.DataAccess.dtGeneric_Int standardids = new Base.DataAccess.dtGeneric_Int();
            string[] selitems = SelectedItems.Split(',');
            for (var i = 0; i < selitems.Length; i++)
            {
                int nodeid;
                string docEntry = selitems[i];
                if (Int32.TryParse(docEntry, out nodeid))
                {
                    standardids.Add(nodeid);
                }
            }

            DataTable dt = CompetencyWorkSheet.ValidateWorksheetHistory(DocumentID, standardids); ;

            if (dt != null) Counter = Convert.ToString(dt.Rows[0][0]);
            return Counter;
        }

        #region Service Methods

		private static List<string> GetStandardSetValueFromParmsTable()
		{
			if (DefaultStandardSets != null)
			{
				List<string> standardSetValuesList = DefaultStandardSets.Split(',').Select(values => values.Trim()).ToList().Where(x => !string.IsNullOrEmpty(x)).ToList();

				return standardSetValuesList;
			}
			return null;
		}

        /// <summary>
        /// Get Standard Set List
        /// </summary>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string getStandardSetList()
        {
			StringBuilder sb = new StringBuilder();
			sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");

			List<string> standardSetValueList = GetStandardSetValueFromParmsTable();
			if (standardSetValueList != null && standardSetValueList.Count > 0)
	        {
				foreach (string standard in standardSetValueList)
				{
					sb.Append("<option value='" + standard + "'>" + standard + "</option>");
				}
				return sb.ToString();
	        }

            DataTable dt = CompetencyWorkSheet.GetWorksheetById(Convert.ToString(System.Web.HttpContext.Current.Session["docId"]), "getStandardSetList");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string rowItem = dr["StandardSet"].ToString();

                    sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Standard Count
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string getStandardCount(string docid)
        {
            DataTable dt = CompetencyWorkSheet.GetWorksheetById(docid, "getStandardCount");
            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    string rowItem = dr["COUNT"].ToString();

                    sb.Append(rowItem);

                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Current Standards
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string getCurrentStandards(string docid)
        {
            DataTable dt = CompetencyWorkSheet.GetWorksheetById(docid, "getCurrentStandards");
            if (dt.Rows.Count > 0)
            {
                return dt.ToJSON(false);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Get District From User Name
        /// </summary>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        private static string getDistrictFromUserName()
        {
            string theDistrict = string.Empty;

            string username = CMS.CMSHelper.CMSContext.CurrentUser.UserName;
            string[] wrk = username.Split('-');

            theDistrict = wrk[0];

            return theDistrict;
        }

        /// <summary>
        /// Get Standard Set Grade
        /// </summary>
        /// <param name="standardSet"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string getStandardSetGrade(string standardSet)
        {
            var scrubbedStandardSet = standardSet.Replace("'", "''");
            DataTable dt = CompetencyWorkSheet.GetWorksheetById(scrubbedStandardSet, "getStandardSetGrade");
            if (dt.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<option  value=\"0\"> ----- Select Item ----- </option>");
                foreach (DataRow dr in dt.Rows)
                {
                    string rowItem = dr["Grade"].ToString();

                    sb.Append("<option value='" + rowItem + "'>" + rowItem + "</option>");

                }
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Standard Set Grade Subject
        /// </summary>
        /// <param name="standardSet"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string getStandardSetGradeSubject(string standardSet, string grade)
        {
            DataTable dt = CompetencyWorkSheet.GetWorksheetById(standardSet, "getStandardSetGradeSubject", grade);
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
        /// <summary>
        /// Get Standard Set Grade Subject Course
        /// </summary>
        /// <param name="standardSet"></param>
        /// <param name="grade"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string getStandardSetGradeSubjectCourse(string standardSet, string grade, string subject)
        {
            DataTable dt = CompetencyWorkSheet.GetWorksheetById(standardSet, "getStandardSetGradeSubjectCourse", grade, subject);
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
        /// <summary>
        /// Get Standards List
        /// </summary>
        /// <param name="standardSet"></param>
        /// <param name="standardSetVal"></param>
        /// <param name="grade"></param>
        /// <param name="gradeVal"></param>
        /// <param name="subject"></param>
        /// <param name="subjectVal"></param>
        /// <param name="course"></param>
        /// <param name="courseVal"></param>
        /// <param name="searchOption"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string GetStandardsList(string standardSet, string standardSetVal, string grade, string gradeVal, string subject, string subjectVal, string course, string courseVal, string searchOption, string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = HttpContext.Current.Server.UrlDecode(searchText);
            }
            if (string.IsNullOrWhiteSpace(gradeVal) || gradeVal == "0" || gradeVal == "null" || gradeVal == "undefined")
            {
                grade = null;
            }
            if (string.IsNullOrWhiteSpace(standardSetVal) || standardSetVal == "0" || standardSetVal == "null" || standardSetVal == "undefined")
            {
                standardSet = null;
            }
            if (string.IsNullOrWhiteSpace(subjectVal) || subjectVal == "0" || subjectVal == "null" || subjectVal == "undefined")
            {
                subject = null;
            }
            if (string.IsNullOrWhiteSpace(courseVal) || courseVal == "0" || courseVal == "null" || courseVal == "undefined")
            {
                course = null;
            }
            if (string.IsNullOrWhiteSpace(searchText))
            {
                searchText = null;
                searchOption = null;
            }

            DataTable dt = Base.Classes.Standards.GetStandardsListWithTextSearch(grade, standardSet, subject, course,
            searchText, searchOption);
             return dt.Rows.Count > 0 ? dt.ToJSON(false) : null;
        }        
        #endregion
        #region Reusable Methods
        
        /// <summary>
        /// Text Search Condition
        /// </summary>
        /// <param name="searchWords"></param>
        /// <param name="searchColumns"></param>
        /// <param name="anyWords"></param>
        /// <returns></returns>
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
        
        #endregion
    }
}