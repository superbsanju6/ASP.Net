using Standpoint.Core.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.Reports
{
    public partial class CompetencyTrackingDetailsStandardPDF : System.Web.UI.Page
    {
        //--TODO REPLACE HARDCODES VALUES ONCE DEPENDENT FUNCATIONALITY IS READY        
        public int StudentId;
        private int techerID;
        private int standardID;
        private int workSheetID;
        private int roblicSortOrder;
        private int viewBySelectedValue;

        //--TODO REMOVE OR USE ONCE DEPENDENT FUNCTIONALITY IS READY
        private int RubricItemSort, LevelID, LevelObjectID;
        string StandardName = String.Empty;
        private string ID_Encrypted;
        DataSet ds = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.QueryString["StandardID"] != null && Request.QueryString["StudentID"] != null && Request.QueryString["WorksheetID"] != null)
                {
                    QueryStringData();
                    Base.Classes.CompetencyTracking.CompetencyTrackingReport obj = new Base.Classes.CompetencyTracking.CompetencyTrackingReport();
                    ds = obj.GetStudentStandardList(workSheetID, standardID, roblicSortOrder, LevelID, LevelObjectID, StudentId);
                    BindRepeaterData();
                }
            }
        }

        private void QueryStringData()
        {

            if (!string.IsNullOrEmpty(Request.QueryString["techerID"]))
            {
                techerID = Convert.ToInt32(Request.QueryString["techerID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StandardID"]))
            {
                standardID = Convert.ToInt32(Request.QueryString["StandardID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["WorksheetID"]))
            {
                workSheetID = Convert.ToInt32(Request.QueryString["WorksheetID"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["roblicSortOrder"]))
            {
                roblicSortOrder = Convert.ToInt32(Request.QueryString["roblicSortOrder"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["viewbySelectedValue"]))
            {
                viewBySelectedValue = Convert.ToInt32(Request.QueryString["viewbySelectedValue"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StudentID"]))
            {
                StudentId = Convert.ToInt32(Request.QueryString["StudentID"]);
            }
        }


        protected void BindRepeaterData()
        {
            if (ds != null)
            {
                if (ds.Tables.Count == 2)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lblStudentName.Text = ds.Tables[0].Rows[0]["StudentName"].ToString();
                        lblStandardDesc.Text = ds.Tables[0].Rows[0]["StandardDesc"].ToString();
                        StandardName = ds.Tables[0].Rows[0]["StandardName"].ToString();
                        lblrubricvalue.Text = ds.Tables[0].Rows[0]["Name"].ToString();
                        lnkStandard.Text = StandardName;                        
                        lblcount.Text = ds.Tables[1].Rows.Count.ToString();
                    }
                    RepDetails.DataSource = ds.Tables[1];
                    RepDetails.DataBind();
                }
            }

        }
    }
}