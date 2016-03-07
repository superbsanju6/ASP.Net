using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.Credentials.PDF
{
    public partial class StudentCredentialPdf : System.Web.UI.Page
    {
        DataSet ds = null;
        string providedUserPassword = string.Empty;
        string providedUserName = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindRepeaterData();
            }
        }
        protected void BindRepeaterData()
        {
            ds = Base.Classes.Credentials.GetCredentialsForStudentPDF(Convert.ToInt32(Request.QueryString["xSID"]));
            if (ds != null)
            {
                if (ds.Tables.Count == 4)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lblstuaname.Text = ds.Tables[0].Rows[0]["StudentName"].ToString();
                        lblcredentialcount.Text = ds.Tables[0].Rows[0]["CredentialsCount"].ToString();
                    }
                    RepDetails.DataSource = ds.Tables[1];
                    RepDetails.DataBind();
                }
            }

        }

        protected void RepDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            HiddenField hdCredentialID = item.FindControl("hdCredentialID") as HiddenField;
            if ((item.ItemType == ListItemType.Item) ||
                (item.ItemType == ListItemType.AlternatingItem))
            {
                Repeater rptrcomments = (Repeater)item.FindControl("rptrcomments");
                DataView dvCmt = ds.Tables[3].DefaultView;
                dvCmt.RowFilter = "CredentialID=" + hdCredentialID.Value;

                rptrcomments.DataSource = dvCmt.ToTable().Rows.Count > 0 ? dvCmt : NoRecordHandling(dvCmt);
                rptrcomments.DataBind();

                Repeater rptrurl = (Repeater)item.FindControl("rptrurl");
                DataView dvUrl = ds.Tables[2].DefaultView;
                dvUrl.RowFilter = "ID=" + hdCredentialID.Value;
                rptrurl.DataSource = dvUrl.ToTable().Rows.Count > 0 ? dvUrl : NoRecordHandling(dvUrl); ;
                rptrurl.DataBind();
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
                column.ColumnName = dv.ToTable().Columns[dv.ToTable().Columns.Count - 1].ColumnName;
                table.Columns.Add(column);
                row = table.NewRow();
                row[column.ColumnName] = "None";
                table.Rows.Add(row);
                dvTemp = new DataView(table);
            }
            return dvTemp;
        }

        protected void rptrurl_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                LinkButton lnkb = (LinkButton)e.Item.FindControl("lblURL");
                if (lnkb !=null && lnkb.Text.Equals("None", StringComparison.CurrentCultureIgnoreCase))
                {
                    Label lbl = new Label();
                    lbl.Text = "None";
                    e.Item.Controls.Add(lbl);
                    e.Item.Controls.Remove(e.Item.FindControl("lblURL"));
                }
            }
        }
    }
}