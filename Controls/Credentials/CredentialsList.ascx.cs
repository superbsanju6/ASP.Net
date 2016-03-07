
using System;
using Thinkgate.Classes;
using Telerik.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Thinkgate.Base.DataAccess;
using System.Data.SqlClient;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.Credentials
{
    public partial class CredentialsList : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
            addNewCredential.Visible = UserHasPermission(Base.Enums.Permission.Icon_AddNew_Credential);
        }

        private void LoadData()
        {
            /* Get our student object from TileParms collection. */
            LoadAndBindCredentials();
        }

        private void LoadAndBindCredentials()
        {
            DataSet dsCredential = Base.Classes.Credentials.GetCredentials("ALL", "SUMMARY");
            if (dsCredential != null && dsCredential.Tables.Count > 0)
            {
                DataTable dt = dsCredential.Tables[0];
                DataView dv = dt.AsDataView();
                dv.RowFilter = "ISACTIVE=1";
                RadGridActiveCredential.DataSource = dv;
                RadGridActiveCredential.DataBind();
                dv.RowFilter = "ISACTIVE=0";
                RadGridDeactivatedCredential.DataSource = dv;
                RadGridDeactivatedCredential.DataBind();
            }
        }

        protected void btnPostbackTarget_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}