using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.TempScan
{
    public partial class WesCTExml : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string _x = !String.IsNullOrEmpty(Request["x"]) ? Request["x"] : string.Empty;

            if (_x == "check")
            {
                Response.Write("found");
            }
        }
    }
}