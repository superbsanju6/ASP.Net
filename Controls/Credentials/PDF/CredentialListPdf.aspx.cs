using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.Credentials.PDF
{
    public partial class CredentialListPdf : System.Web.UI.Page
    {
        DataSet ds = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindRepeaterData();
            }
        }
        protected void BindRepeaterData()
        {
            ds = Base.Classes.Credentials.GetCredentialListPDF("ALL", "DETAIL");
            RepDetails.DataSource = ds.Tables[0];
            RepDetails.DataBind();
        }

        protected void RepDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            HiddenField hdCredentialID = item.FindControl("hdCredentialID") as HiddenField;
            if ((item.ItemType == ListItemType.Item) ||
                (item.ItemType == ListItemType.AlternatingItem))
            {
                Repeater rptrcomments = (Repeater)item.FindControl("rptrAlignment");
                var query =
                from credential in ds.Tables[0].AsEnumerable()
                join credentialAlignment in ds.Tables[2].AsEnumerable()
                on credential.Field<int>("ID") equals
                    credentialAlignment.Field<int>("CredentialId")
                join alignment in ds.Tables[1].AsEnumerable()
                on credentialAlignment.Field<int>("AlignmentId") equals
                    alignment.Field<int>("ID")
                where credential.Field<int>("ID") == Convert.ToInt32(hdCredentialID.Value)
                select new
                {
                    ID =alignment.Field<int>("ID"),
                    CredentialAlignment =alignment.Field<string>("CredentialAlignment"),
                };
                rptrcomments.DataSource = query;
                rptrcomments.DataBind();
            }
        }
    }
}