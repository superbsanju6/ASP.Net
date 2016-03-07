using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Controls.Reports;

namespace Thinkgate.Controls.Credentials.PDF
{
    public partial class StudentCountListPdf : BasePage
    {
        DataSet ds = null;
        string providedUserPassword = string.Empty;
        string providedUserName = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["xCriteria"] != null)
            {
             
                if (!this.IsPostBack)
                {
                    BindHeaderData();
                    BindStudentCountGrid();
                }
            }

        }
        protected void BindHeaderData()
        {
            var Tempobjparams = Request.QueryString["xCriteria"].Split(';');
            int credentialID = Tempobjparams[15] == "" ? 0 : Convert.ToInt32(Tempobjparams[15]);

            ds = Base.Classes.Credentials.GetCredentials("ALL", "DETAIL", credentialID);
            if (ds != null)
            {
                if (ds.Tables.Count == 3)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lblcredentialName.Text = ds.Tables[0].Rows[0]["Name"].ToString();
                        rptrurl.DataSource = ds.Tables[2];
                        rptrurl.DataBind();
                    }

                }
            }

        }
        protected void BindStudentCountGrid()
        {
            ds = GetCredentialReport(Request.QueryString["xCriteria"]);
            if (ds != null && ds.Tables.Count == 2)
            {
                ds.Relations.Clear();
                ds.Relations.Add(new DataRelation("CredentialComments",
                    new DataColumn[] { ds.Tables[0].Columns["StudentID"], ds.Tables[0].Columns["CredentialID"] },
                    new DataColumn[] { ds.Tables[1].Columns["StudentID"], ds.Tables[1].Columns["CredentialID"] }
                    , false));                                
            }


            if (ds!=null)
            {
                if(ds.Tables.Count>0)
                {
                   
                    lblstudentcount.Text = ds.Tables[0].Rows.Count.ToString();
                    RepDetails.DataSource = ds.Tables[0];
                    RepDetails.DataBind();

                }

            }
            
        }

        public static DataSet GetCredentialReport(string selectedCriteria)
        {
            DataTable credentialReport = new DataTable();

            var Tempobjparams = selectedCriteria.Split(';');
            string selectedViewBy = Tempobjparams[0];
            int selectedSchoolID =Tempobjparams[1]==""?0: Convert.ToInt32(Tempobjparams[1]);
            int selectedTeacherId = Tempobjparams[2] == "" ? 0 : Convert.ToInt32(Tempobjparams[2]);
            int selectedClassID = Tempobjparams[3] == "" ? 0 : Convert.ToInt32(Tempobjparams[3]);
            int selectedGroupID = Tempobjparams[4] == "" ? 0 : Convert.ToInt32(Tempobjparams[4]);
            int selectedStudentID = Tempobjparams[5] == "" ? 0 : Convert.ToInt32(Tempobjparams[5]);
            string selectedAlignments = Tempobjparams[6];
            string selectedYear = Tempobjparams[7];


            string Race = Tempobjparams[8];
            string Students_With_Disabilities = Tempobjparams[9];
            string English_Language_Learner = Tempobjparams[10];
            string Economically_Disadvantaged = Tempobjparams[11];
            string Early_Intervention_Program = Tempobjparams[12];
            string Gender = Tempobjparams[13];
            string Gifted = Tempobjparams[14];
            int credentialID = Tempobjparams[15] == "" ? 0 : Convert.ToInt32(Tempobjparams[15]);
            int levelID = Tempobjparams[16] == "" ? 0 : Convert.ToInt32(Tempobjparams[16]);

            drGeneric_Int drAlignments = new drGeneric_Int();
            if (!string.IsNullOrEmpty(selectedAlignments))
            {
                string[] arAlignments = (selectedAlignments).Split(',');
                int alignment = 0;
                foreach (string strAlign in arAlignments)
                    if (Int32.TryParse(strAlign, out alignment))
                        drAlignments.Add(alignment);
            }



            var objparams = new object[] { 
                    selectedViewBy ?? "", 
                    selectedSchoolID, 
                    selectedTeacherId,
                    selectedClassID,
                    selectedStudentID,
                    selectedGroupID,
                    drAlignments.ToSql(),
                    selectedYear??"",
                    Race,
                   Students_With_Disabilities,
                   English_Language_Learner,
                   Economically_Disadvantaged,
                   Early_Intervention_Program,
                   Gender,
                   Gifted,
                    credentialID,
                    levelID
                };

            DataSet dsCredentialReport = ThinkgateDataAccess.FetchDataSet(Thinkgate.Base.Classes.Data.StoredProcedures.E3_CREDENTIALS_TRACKINGREPORT_GET, objparams);
            return dsCredentialReport;
        }



        protected void RepDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;           
            DataRowView row = (DataRowView)(item).DataItem;

            HiddenField hdCredentialID = item.FindControl("hdCredentialID") as HiddenField;
            if ((item.ItemType == ListItemType.Item) ||
                (item.ItemType == ListItemType.AlternatingItem))
            {
                Repeater rptrcomments = (Repeater)item.FindControl("rptrcomments");
                DataView dvCmt = row.CreateChildView("CredentialComments");
                rptrcomments.DataSource = dvCmt.ToTable().Rows.Count > 0 ? dvCmt : NoRecordHandling(dvCmt);
                rptrcomments.DataBind();
            }
        }

        private DataView NoRecordHandling(DataView dv)
        {
            DataView dvTemp = new DataView(dv.ToTable());
            if ((dv.ToTable().Rows.Count == 0))
            {
                DataTable table = new DataTable();

                DataColumn column;
                DataRow row;
                DataView view;

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = dv.ToTable().Columns[dv.ToTable().Columns.Count - 2].ColumnName;
                table.Columns.Add(column);
                row = table.NewRow();
                row[column.ColumnName] = "None";
                table.Rows.Add(row);
                dvTemp = new DataView(table);
            }
            return dvTemp;
        }
    }
}