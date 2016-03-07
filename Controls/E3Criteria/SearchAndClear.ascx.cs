using System;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class SearchAndClear : System.Web.UI.UserControl
    {
        public string StarterText;
        public string AfterSearchText;
        public string OnClientSearch;
        public string OnClientClear;
        public bool RunSearchOnPageLoad;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnSearch.Text = StarterText;
            btnSearch.Attributes.Add("AfterSearchText", AfterSearchText);
            btnSearch.OnClientClicked = OnClientSearch ?? "btnSearchClick";
            btnClear.OnClientClicked = OnClientClear ?? "btnClearClick";
            if (RunSearchOnPageLoad)
                ScriptManager.RegisterStartupScript(this, typeof(string), "AssessmentItems", "searchOnLoad();", true);
        }

    }
}