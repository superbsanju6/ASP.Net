using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class DurationControlV2 : Criterion
    {

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                for (int i = 0; i < 24; i++)
                    DurationHours.Items.Add(i.ToString(CultureInfo.InvariantCulture));
                for (int i = 0; i < 60; i += 5)
                    DurationMinutes.Items.Add(i.ToString(CultureInfo.InvariantCulture));

                rtbDuration.Text = string.Empty;
                DurationMinutes.Text = "0";
                DurationHours.Text = "0";

                ClientScriptManager clientScript = Page.ClientScript;
                StringBuilder javaScriptBuilder = new StringBuilder();

                javaScriptBuilder.Append(" $('#" + rtbDuration.ClientID + "').inputmask('integer', {allowMinus: false, allowPlus: false});");

                clientScript.RegisterStartupScript(GetType(), rtbDuration.ClientID,
                            javaScriptBuilder.ToString(), true);
            }
        }

        #endregion
    }
}