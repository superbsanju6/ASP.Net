using System;
using Standpoint.Core.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;

namespace Thinkgate.Dialogues
{
    public partial class RubricItemResources : BasePage
    {
        private int _rubricItemID;
        private string _category = "";

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _rubricItemID = DataIntegrity.ConvertToInt(Request.QueryString["xID"]);
            }

            if (Request.QueryString["category"] != null)
                _category = Request.QueryString["category"].ToString();

            var dt = Thinkgate.Base.Classes.Rubric.GetRubricItemResources(_rubricItemID, _category);
            lblNoneFound.Visible = (dt == null || dt.Rows.Count == 0);
            repeaterResources.DataSource = dt;
            repeaterResources.DataBind();
        }
    }
}