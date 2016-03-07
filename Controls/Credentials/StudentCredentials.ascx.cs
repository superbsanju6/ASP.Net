
using System;
using Thinkgate.Classes;
using Telerik.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;

namespace Thinkgate.Controls.Credentials
{
    public partial class StudentCredentials : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        { }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            this.Page.ClientScript.RegisterForEventValidation(this.btnPostbackTargetSTC.UniqueID, "StudentTileRefresh");
            base.Render(writer);
        }

        protected override void OnPreRender(EventArgs e)
        {
            LoadData();
            base.OnPreRender(e);
        }

        private void LoadData()
        {
            LoadAndBindStudentCredentails();
        }

        private void LoadAndBindStudentCredentails()
        {
            var s = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");
            if (s == null) return;

            drGeneric_String drStringObj = new drGeneric_String();
            drStringObj.Add("0");


            DataSet ds = Base.Classes.Credentials.GetCredentialsForStudent(s.ID, StudentCredentialsView.Summary, drStringObj);
            DataTable dtCredential = new DataTable();
            if (ds != null && ds.Tables.Count > 0)
            {
                dtCredential = ds.Tables[0];
            }
            studentCredentialGrid.DataSource = dtCredential;
            studentCredentialGrid.DataBind();
        }

        protected void btnPostbackTargetSTC_Click(object sender, EventArgs e)
        { }
    }
}