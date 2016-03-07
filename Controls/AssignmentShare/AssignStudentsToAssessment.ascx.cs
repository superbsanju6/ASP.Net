using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Controls.Reports;

namespace Thinkgate.Controls.AssignmentShare
{
	public partial class AssignStudentsToAssessment : System.Web.UI.UserControl
	{


		#region Properties

		public event EventHandler AssignStudentsToAssessmentReloadReport;
		public SessionObject SessionObject { get; set; }
		public EntityTypes _level;
		public int _levelID;
		private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";
		private const int UserPage = 110;
		private string _selectedGrade;
		private string _selectedSchoolType;
		private string _selectedClass;
		protected string gradeListFilterKey = "GradeListFilter";
		public string HiddenGuid { get; set; }

		#endregion


		protected void Page_Init(object sender, EventArgs e)
		{
			SessionObject = (SessionObject)Session["SessionObject"];
		}

		protected void Page_Load(object sender, EventArgs e)
		{

			switch (_level)
			{
				case EntityTypes.Teacher:
				case EntityTypes.District:
					_selectedClass = SessionObject.StudentSearchParms.GetParm("PieChartValue") != null ? SessionObject.StudentSearchParms.GetParm("PieChartValue").ToString() : null;
					_selectedSchoolType = SessionObject.StudentSearchParms.GetParm("PieChartValue") != null ? SessionObject.StudentSearchParms.GetParm("PieChartValue").ToString().Replace(" ", "-s-") : null;
					break;
				case EntityTypes.School:
					var gradePos = DataIntegrity.ConvertToInt(SessionObject.StudentSearchParms.GetParm("PieChartValue"));
					var gradeList = (List<string>)SessionObject.StudentSearchParms.GetParm("GradeListFilter");
					_selectedGrade = gradeList != null && gradeList.Count > 0 ? gradeList[gradePos] : null;
					break;
			}

			LoadStudentSearchCriteriaControl();

		}

		#region Private Methods

		private Criteria LoadSearchCriteria()
		{
			var criteria = new Criteria();

			criteria.Add(new Criterion
			{
				Header = "Name",
				Key = "Name",
				Type = "String",
				Description = string.Empty,
				Locked = false,
				UIType = UIType.TextBox,
				Removable = true
			});

			criteria.Add(new Criterion
			{
				Header = "Student ID",
				Key = "StudentID",
				Type = "String",
				Description = string.Empty,
				Locked = false,
				UIType = UIType.TextBox,
				Removable = true
			});

			criteria.Add(new Criterion
			{
				Header = "RTI",
				Key = "RTI",
				Type = "String",
				Description = string.Empty,
				Locked = false,
				UIType = UIType.RTI
			});

			criteria.Add(new Criterion
			{
				Header = "Cluster",
				Key = "Cluster",
				Type = "String",
				Description = string.Empty,
				Locked = false,
				UIType = UIType.CheckBoxList,
				DataSource = Base.Classes.Cluster.GetClusterListForDropDown(UserPage),
				DataTextField = "Cluster",
				DataValueField = "Cluster",
				Removable = true
			});

			// School Type
			var schoolTypeDataTable = new DataTable();
			var schoolTypesForLoggedInUser = SchoolTypeMasterList.GetSchoolTypeListForUser(SessionObject.LoggedInUser);
			schoolTypeDataTable.Columns.Add("SchoolType");

			foreach (var s in schoolTypesForLoggedInUser)
			{
				schoolTypeDataTable.Rows.Add(s);
			}

			criteria.Add(new Criterion
			{
				Header = "School Type",
				Key = "SchoolType",
				Type = "String",
				Description = string.Empty,
				Locked = false,
				UIType = UIType.CheckBoxList,
				DataSource = schoolTypeDataTable,
				DataTextField = "SchoolType",
				DataValueField = "SchoolType",
				DefaultValue = _selectedSchoolType,
				Removable = true,
				ServiceUrl = "../../Services/School.svc/GetAllSchoolsFromSchoolTypes",
				ServiceOnSuccess = "getAllSchoolsFromSchoolTypes",
				Dependencies = new[]
                {
                    Criterion.CreateDependency("SchoolType", "SchoolTypes"),
                    Criterion.CreateDependency("School", "Schools")
                }
			});

			// School
			var schoolDataTable = new DataTable();
			var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
			schoolDataTable.Columns.Add("Name");
			schoolDataTable.Columns.Add("ID");

			foreach (var s in schoolsForLooggedInUser)
			{
				schoolDataTable.Rows.Add(s.Name, s.ID);
			}

			criteria.Add(new Criterion
			{
				Header = "School",
				Key = "School",
				Type = "String",
				Description = string.Empty,
				Locked = false,
				UIType = UIType.DropDownList,
				DataSource = schoolDataTable,
				DataTextField = "Name",
				DataValueField = "ID",
				Removable = true
			});

			criteria.Add(new Criterion
			{
				Header = "Grade",
				Key = "Grade",
				Type = "String",
				DataTextField = "Grade",
				DataValueField = "Grade",
				Description = string.Empty,
				Locked = false,
				DefaultValue = _selectedGrade,
				DataSource = Grade.GetGradeListForDropDown(UserPage, Permissions),
				UIType = UIType.CheckBoxList,
				Removable = true
			});

			criteria.Add(new Criterion
			{
				Header = "Demographics",
				Key = "Demographics",
				Type = "String",
				Description = string.Empty,
				Locked = false,
				UIType = UIType.Demographics
			});

			return criteria;
		}

		private void LoadStudentSearchCriteriaControl()
		{
			var ctlReportCriteria = (ReportCriteria)LoadControl("~/Controls/Reports/ReportCriteria.ascx");
			ctlReportCriteria.ID = "ctlStudentSearchResultsCriteria";

			if (string.IsNullOrEmpty(hiddenTextBox.Text))
			{
		
				HiddenGuid = Guid.NewGuid().ToString();
				hiddenTextBox.Text = HiddenGuid;
				ctlReportCriteria.Guid = HiddenGuid;
				ctlReportCriteria.Criteria = LoadSearchCriteria();
				ctlReportCriteria.FirstTimeLoaded = true;
			}
			else
			{
				HiddenGuid = hiddenTextBox.Text;
				ctlReportCriteria.Guid = hiddenTextBox.Text;
				ctlReportCriteria.FirstTimeLoaded = false;
			}

			ctlReportCriteria.ReloadReport += AssignStudentsToAssessmentReloadReport;

			criteraDisplayPlaceHolder.Controls.Add(ctlReportCriteria);
		}

		#endregion

	}
}