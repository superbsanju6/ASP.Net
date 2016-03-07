using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class UsageReportGradeSubjectCourse : CriteriaBase
    {
        private const string ROLE_DISTRICTADMIN = "District Administrator";
        private const string ROLE_SCHOOLADMIN = "School Administrator";
        private const string ROLE_TEACHER = "Teacher";

        public object JsonDataSource;
        private SessionObject _sessionObject;
        public string UserID = string.Empty;

        public DropDownList CmbGrade
        {
            get { return cmbGrade;}
        }

        public DropDownList CmbSubject
        {
            get { return cmbSubject;}
        }

        public DropDownList CmbCourse
        {
            get { return cmbCourse; }
        }

        public Text CmbUser
        {
            get { return txtUserName; }
        }

		public bool ShowStandardLevels = false;

        protected new void Page_Init(object sender, EventArgs e)
        {
            if (_sessionObject == null)
            {
                _sessionObject = (SessionObject)Session["SessionObject"];

                if (_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_DISTRICTADMIN, StringComparison.InvariantCultureIgnoreCase)) != null)
                { }
                if (_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_SCHOOLADMIN, StringComparison.InvariantCultureIgnoreCase)) != null)
                { }
                else if (_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_TEACHER, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    UserID = _sessionObject.LoggedInUser.UserId.ToString();
                }
            }            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                cmbGrade.OnChange = CriteriaName + "Controller.OnGradeChange();";
                cmbSubject.OnChange = CriteriaName + "Controller.OnSubjectChange();";
                cmbCourse.OnClientLoad = CriteriaName + "Controller.PopulateControls";
                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);      
            }
        }

		public class GradeSubjectCourse
		{
			public object GradeSubjectCourseData;
		}
    }
}