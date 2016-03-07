using System;
using System.Globalization;
using System.Text;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class Duration : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rtbDuration.Attributes.Add("onblur", CriteriaName + "Controller.OnChange()");
                DurationHours.Attributes.Add("onblur", CriteriaName + "Controller.OnChange()");
                DurationMinutes.Attributes.Add("onblur", CriteriaName + "Controller.OnChange()");
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // can only have 1 value
           
                for (int i = 0; i < 24; i++)
                    DurationHours.Items.Add(i.ToString(CultureInfo.InvariantCulture));
                for (int i = 0; i < 60; i += 5)
                    DurationMinutes.Items.Add(i.ToString(CultureInfo.InvariantCulture));

                rtbDuration.Text = "0";
                DurationMinutes.Text = "0";
                DurationHours.Text = "0";
                

                ClientScriptManager clientScript = Page.ClientScript;
                StringBuilder javaScriptBuilder = new StringBuilder();

                javaScriptBuilder.Append(" $('#" + rtbDuration.ClientID + "').inputmask('integer', {allowMinus: false, allowPlus: false});");

                clientScript.RegisterStartupScript(GetType(), rtbDuration.ClientID,
                            javaScriptBuilder.ToString(), true);

            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }


        /// <summary>
        /// Simple name/value object to store the two values we have into the CriteriaSet
        /// </summary>
        [Serializable]
        public class ValueObject
        {
            public string Text { get; set; }
        }
    }
}