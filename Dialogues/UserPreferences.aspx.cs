using System;
using System.Drawing;
using Thinkgate.Classes;

namespace Thinkgate.Dialogues
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public SessionObject SessionObject;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
            SessionObject = (SessionObject)Session["SessionObject"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionObject.BackgroundColor != null)
            {
                colorHexVal.Value = (SessionObject.BackgroundColor == "#ffffff") ? "#f0f0f0" : SessionObject.BackgroundColor;
            }
            else
            {
                colorHexVal.Value = "#f0f0f0";
            }
        }

        protected void SaveBtnClick(object sender, EventArgs e)
        {
            if (rbStripes.Checked)
                SessionObject.BackgroundClass = "stripes";
            else if (rbRadial.Checked)
            {
                SessionObject.BackgroundClass = "radial";
            }
            else if (rbWaterfall.Checked)
            {
                SessionObject.BackgroundClass = "waterfall";
            }
            else
            {
                return;
            }

            SessionObject.BackgroundColor = ColorTranslator.ToHtml(RadColorPicker.SelectedColor);
                        
            const string script = "<SCRIPT type='text/javascript'> parent.location.reload(); </script>";

            ClientScript.RegisterClientScriptBlock(GetType(), "refresh", script);
        }
    }
}