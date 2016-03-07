using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using CMS.GlobalHelper;

namespace Thinkgate.Controls.Assessment
{
    public partial class SetPerformanceLevelList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [System.Web.Services.WebMethod]
        public static string GetPerofmranceLevel(int Id)
        {
            SetPerformanceLevel pLevel = new SetPerformanceLevel();
            
            
            DataTable dtpLevel = pLevel.GetPerformanceLevel(Id);

          return  dtpLevel.ToJSON(false);


        }

    }
}