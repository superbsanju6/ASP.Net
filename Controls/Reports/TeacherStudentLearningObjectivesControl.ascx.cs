using System;
using System.Reflection;

using Thinkgate.Classes;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Controls.Reports
{
    public partial class TeacherStudentLearningObjectivesControl : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["folder"] != null && !String.IsNullOrEmpty(Request.QueryString["folder"]))
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                ThinkgateEventSource.Log.LoggedUserReportAccess(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, sessionObject.LoggedInUser.UserName + " has accessed '" + Request.QueryString["folder"].ToString() + "' report", Request.QueryString["folder"].ToString(), sessionObject.LoggedInUser.UserName);
            }
            TeacherStudentLearningObjectivesFrame.Attributes["src"] = ResolveUrl("~/Controls/Reports/TeacherStudentLearningObjectivesPage.aspx");
        }
    }
}


