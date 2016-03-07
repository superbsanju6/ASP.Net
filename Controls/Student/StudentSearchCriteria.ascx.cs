using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Thinkgate.Classes;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.Student
{
    public partial class StudentSearchCriteria : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SearchBtnClick(object sender, ImageClickEventArgs e)
        {
            var searchParms = new SearchParms();
            if (!String.IsNullOrEmpty(txtName.Text))
            {
                searchParms.AddParm("studentName", txtName.Text);
            }


        }
    }
}