using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Documents
{
	public partial class Resources : TileControlBase
	{
		#region Properties

		protected Thinkgate.Base.Enums.EntityTypes _level;
		protected Int32 _levelID;
		protected bool _calendarIconVisible = false;
		private int _classID;
		private int _term;
		private int _planID;
		private string _type;
		private string _UseResourcesTileFilterToDisplay;

		protected Boolean _isPostBack;
		private Boolean _gradeVisible;
		private Boolean _subjectVisible;
		private Boolean _courseVisible;
		private Boolean _typeVisible;
		private Boolean _categoryVisible;
		private Boolean _subtypeVisible;
		protected Boolean _standardVisible;
		private Boolean _fullAccess;

		private const String _gradeFilterKey = "ReGradeFilterIdx";
		private const String _subjectFilterKey = "ReSubjectFilter";
		private const String _courseFilterKey = "ReCourseFilter";
		private const String _typeFilterKey = "ReTypeFilter";
		private const String _categoryFilterKey = "ReCategoryFilter";
		private const String _subtypeFilterKey = "ReSubtypeFilter";
		private const String _standardFilterKey = "ReStandardSetFilter";

		// We only want to show the first 100 standards.
		private const Int32 _maxStandards = 100;

		private DataTable _categoriesAndTypesDT;
		private DataTable _standardsDT;
		private CourseList _currCourseList;
		private List<Base.Classes.Resource> resource;
		private int standardID = 0;

		private IEnumerable<Grade> gradeList;
		private IEnumerable<Subject> subjectList;

		#endregion

		#region Asynchronous Methods
		//private void GetStandardCoursesForUser()
		//{
		//    _standardCourseList = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser, SessionObject.GlobalInputs);
		//}

		private void GetGrades()
		{
            gradeList = _currCourseList.GetGradeList();
		}

		private void GetSubjects()
		{
            subjectList = _currCourseList.GetSubjectList();
		}

		private void GetCategories()
		{
			if (_categoriesAndTypesDT == null) _categoriesAndTypesDT = Thinkgate.Base.Classes.Resource.GetResourceCategoriesDataTable(SessionObject.GlobalInputs);
		}

		private void GetStandards()
		{
			if (_standardsDT == null) _standardsDT = Base.Classes.Standards.GetStandardsForInstructionalPlan(1, _classID, _term, SessionObject.GlobalInputs);
		}

		private void GetResources()
		{
			resource = Thinkgate.Base.Classes.Resource.SearchResources(standardID, ViewState[_categoryFilterKey].ToString(),
				_type == "Resources" ? ViewState[_typeFilterKey].ToString() : _type, ViewState[_subtypeFilterKey].ToString(), SessionObject.GlobalInputs, grade: ViewState[_gradeFilterKey].ToString(), subject: ViewState[_subjectFilterKey].ToString(), course: ViewState[_courseFilterKey].ToString(),
				fullAccess: _fullAccess);
		}
		#endregion

		protected new void Page_Init(object sender, EventArgs e)
		{
			base.Page_Init(sender, e);

			if (Tile == null || Tile.TileParms == null) return;

			_classID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("selectID"));
			_term = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("term"));
			_planID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("planID"));
			//_type = Tile.TileParms.GetParm("type").ToString();
			//COMMENT: RE this was breaking code, not sure what/if is required put a condition to protect from breaking
			if (Tile.TileParms.GetParm("type") != null)
				_type = Tile.TileParms.GetParm("type").ToString();

			if (Tile.TileParms.GetParm("level") != null)
				_level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");

			if (Tile.TileParms.GetParm("levelID") != null)
				_levelID = (Int32)Tile.TileParms.GetParm("levelID");

			if (Tile.TileParms.GetParm("showCalendarIcon") != null)
				_calendarIconVisible = DataIntegrity.ConvertToBool(Tile.TileParms.GetParm("showCalendarIcon"));

			_UseResourcesTileFilterToDisplay = (Tile.TileParms.GetParm("UseResourcesTileFilterToDisplay") ?? "").ToString().ToLower();

            _currCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);

			_fullAccess = UserHasPermission(Permission.Resources_Access_All);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Tile == null) return;

			// Simulate IsPostBack.
			String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
			_isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

			// Create the initial viewstate values.
			if (ViewState[_gradeFilterKey] == null)
			{
				ViewState.Add(_gradeFilterKey, "All");
				ViewState.Add(_subjectFilterKey, "All");
				ViewState.Add(_courseFilterKey, "All");
				ViewState.Add(_typeFilterKey, _type == "Unit Plans" ? "Unit Plans" : "All");
				ViewState.Add(_categoryFilterKey, "All");
				ViewState.Add(_subtypeFilterKey, "All");
				ViewState.Add(_standardFilterKey, "All");
			}

			// Set the current filter visibility.
			SetFilterVisibility();
            if (_UseResourcesTileFilterToDisplay.Split(',').Count() > 4)
                lbxResources.Height = 160;

			List<AsyncPageTask> taskList = new List<AsyncPageTask>();
			if (!_isPostBack)
			{
				taskList.Add(new AsyncPageTask(GetGrades));
				taskList.Add(new AsyncPageTask(GetSubjects));
				taskList.Add(new AsyncPageTask(BuildCourses));
				taskList.Add(new AsyncPageTask(GetCategories));
				taskList.Add(new AsyncPageTask(GetStandards));
			}
			taskList.Add(new AsyncPageTask(GetResources));

			foreach (AsyncPageTask page in taskList)
			{
				PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "Resources", true);
				Page.RegisterAsyncTask(newTask);
			}
			taskList = null;
			Page.ExecuteRegisteredAsyncTasks();

			if (!_isPostBack)
			{
				BuildGrades();
				BuildSubjects();
				BuildCategories();
				BuildTypes();
				BuildCourses();
				BuildSubtypes();
				BuildStandards();
			}


			SearchResources();

			var addButtonVisible = false;
			var addButtonXParm = string.Empty;
			var addButtonCategoryParm = string.Empty;
			if (_type == "Unit Plans" && UserHasPermission(Permission.Icon_Add_UnitPlan))
			{
				addButtonVisible = true;
				addButtonXParm = "Unit Plan";
				addButtonCategoryParm = "Instructional";
			}
			else if (_type == "Lesson Plans" && UserHasPermission(Permission.Icon_Add_LessonPlans))
			{
				addButtonVisible = true;
				addButtonXParm = "Lesson Plan";
				addButtonCategoryParm = "Instructional";
			}
			else if (_type == "Pacing Documents" && UserHasPermission(Permission.Icon_Add_PacingDocuments))
			{
				addButtonVisible = true;
				addButtonXParm = "Pacing Document";
				addButtonCategoryParm = "Instructional";
			}
			else if (_type == "Resources" && UserHasPermission(Permission.Icon_Add_Resource))
			{
				addButtonVisible = true;
				addButtonXParm = "Resource";
			}

			btnAdd.Visible = addButtonVisible;
			if (addButtonVisible)
			{
				btnAdd.Attributes["onclick"] = "window.open('" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") +
											   "' + escape('fastsql_v2_direct.asp?ID=7266|search_documents_add&??x=" + addButtonXParm + "&??action=add&appName=E3&??category=" + addButtonCategoryParm + "'));";
			}
		}

		#region Filter Methods

		protected void SetFilterVisibility()
		{
			/******* 20121022 DHB ***************************************************************************
			 * Alamance QA 77 - This if construct was added because one of the users of the app (Alamance) 
			 * desired the Resources tile to filter by Subject instead of Type (which is the default).  So 
			 * district parm "ResourcesTileFilterToDisplay" was created for this case.  If this district 
			 * parm has a value in it, then use it to determine what filter to display - and only do this 
			 * when the "Type" param is "Resources"  otherwise, treat the displaying of filters as it was 
			 * before this request was dealt with.
			 ***********************************************************************************************/
			if (_type == "Resources" && !string.IsNullOrEmpty(_UseResourcesTileFilterToDisplay))
			{
				_gradeVisible = (_UseResourcesTileFilterToDisplay.Contains("grade"));
				_subjectVisible = (_UseResourcesTileFilterToDisplay.Contains("subject"));
				_courseVisible = (_UseResourcesTileFilterToDisplay.Contains("course"));
				_categoryVisible = (_UseResourcesTileFilterToDisplay.Contains("category"));
				_typeVisible = (_UseResourcesTileFilterToDisplay.Contains("type"));
				_subtypeVisible = (_UseResourcesTileFilterToDisplay.Contains("subtype"));
				_standardVisible = (_UseResourcesTileFilterToDisplay.Contains("standard"));
			}
			else
			{
				_gradeVisible = _type != "Resources";
				_subjectVisible = _type != "Resources";
				_courseVisible = _type == "Unit Plans" || _type == "Lesson Plans" || _type == "Pacing Documents";

				_categoryVisible = _level == EntityTypes.Standard;
				_typeVisible = (_type == "Resources" || _level == EntityTypes.Standard || _level == EntityTypes.InstructionalPlan);
				_subtypeVisible = (_type == "Unit Plans" || _level == EntityTypes.Standard);

				_standardVisible = (_level == EntityTypes.InstructionalPlan);

			}
		}

		protected void BuildGrades()
		{
			cmbGrade.Visible = _gradeVisible;
			if (_gradeVisible)
			{
				IEnumerable<Grade> gradeList;
				switch (_level)
				{
					case EntityTypes.Class:
					case EntityTypes.Teacher:
					case EntityTypes.District:
                        gradeList = _currCourseList.GetGradeList();
						break;
					case EntityTypes.School:
						gradeList = CourseMasterList.StandCourseList.GetGradeList();
						break;
					default:
                        gradeList = _currCourseList.GetGradeList();
						break;
				}

				DataTable dtGrade = new DataTable();
				dtGrade.Columns.Add("Grade", typeof(String));
				dtGrade.Columns.Add("CmbText", typeof(String));
				foreach (var g in gradeList)
					dtGrade.Rows.Add(g.DisplayText, g.DisplayText);


				DataRow newRow = dtGrade.NewRow();
				newRow["Grade"] = "All";
				newRow["CmbText"] = "Grade";
				dtGrade.Rows.InsertAt(newRow, 0);

				// Data bind the combo box.
				cmbGrade.DataTextField = "CmbText";
				cmbGrade.DataValueField = "Grade";
				cmbGrade.DataSource = dtGrade;
				cmbGrade.DataBind();

				// Initialize the current selection. Sometimes the filter item no longer exists when changing
				// tabs from School, District, Classroom.
				RadComboBoxItem item = this.cmbGrade.Items.FindItemByValue((String)this.ViewState[_gradeFilterKey], true) ?? this.cmbGrade.Items[0];
				ViewState[_gradeFilterKey] = item.Value;
				Int32 selIdx = cmbGrade.Items.IndexOf(item);
				cmbGrade.SelectedIndex = selIdx;
			}
		}

		protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			ViewState[_gradeFilterKey] = e.Value;

			SetFilterVisibility();
			SearchResources();
			BuildCourses();
		}

		protected void BuildSubjects()
		{
			cmbSubject.Visible = _subjectVisible;
			if (_subjectVisible)
			{
				// Now load the filter button tables and databind.
				DataTable dtSubject = new DataTable();
				dtSubject.Columns.Add("Subject");
				dtSubject.Columns.Add("CmbText", typeof(String));

				foreach (var s in subjectList)
				{
					dtSubject.Rows.Add(s.DisplayText, s.DisplayText);
				}

				DataRow newRow = dtSubject.NewRow();
				newRow["Subject"] = "All";
				newRow["CmbText"] = "Subject";
				dtSubject.Rows.InsertAt(newRow, 0);

				// Data bind the combo box.
				cmbSubject.DataTextField = "CmbText";
				cmbSubject.DataValueField = "Subject";
				cmbSubject.DataSource = dtSubject;
				cmbSubject.DataBind();

				// Initialize the current selection. Sometimes the filter item no longer exists when changing
				// tabs from School, District, Classroom.
				RadComboBoxItem item = this.cmbSubject.Items.FindItemByValue((String)this.ViewState[_subjectFilterKey], true) ?? this.cmbSubject.Items[0];
				ViewState[_subjectFilterKey] = item.Value;
				Int32 selIdx = cmbSubject.Items.IndexOf(item);
				cmbSubject.SelectedIndex = selIdx;
			}
		}

		protected void cmbSubject_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			ViewState[_subjectFilterKey] = e.Value;

			SetFilterVisibility();
			SearchResources();
			BuildCourses();
		}

		private void BuildCourses()
		{
			cmbCourse.Visible = _courseVisible;
			if (_courseVisible)
			{
				// Now load the filter button tables and databind.
				DataTable dtCourse = new DataTable();
				dtCourse.Columns.Add("Course");
				dtCourse.Columns.Add("CmbText", typeof(String));
				//var courseList = _standardCourseList.GetCourseNames().ToList();
				List<string> gradeList = null;
				List<string> subjectList = null;

				if (ViewState[_gradeFilterKey].ToString() != "All" && ViewState[_gradeFilterKey].ToString() != "" && ViewState[_gradeFilterKey].ToString() != null)
				{
					gradeList = new List<string>();
					gradeList.Add(ViewState[_gradeFilterKey].ToString());
				}

				if (ViewState[_subjectFilterKey].ToString() != "All" && ViewState[_subjectFilterKey].ToString() != "" && ViewState[_subjectFilterKey].ToString() != null)
				{
					subjectList = new List<string>();
					subjectList.Add(ViewState[_subjectFilterKey].ToString());
				}

				var courseList = _level == EntityTypes.Teacher ? CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(gradeList, subjectList).GetCourseNames().ToList() : CourseMasterList.CurrCourseList.FilterByGradesAndSubjects(gradeList, subjectList).GetCourseNames().ToList();
				courseList.Sort();

				foreach (var c in courseList)
				{
					dtCourse.Rows.Add(c, c);
				}

				DataRow newRow = dtCourse.NewRow();
				newRow["Course"] = "All";
				newRow["CmbText"] = "Course";
				dtCourse.Rows.InsertAt(newRow, 0);

				// Data bind the combo box.
				cmbCourse.DataTextField = "CmbText";
				cmbCourse.DataValueField = "Course";
				cmbCourse.DataSource = dtCourse;
				cmbCourse.DataBind();

				// Initialize the current selection. Sometimes the filter item no longer exists when changing
				// tabs from School, District, Classroom.
				RadComboBoxItem item = this.cmbCourse.Items.FindItemByValue((String)this.ViewState[_courseFilterKey], true) ?? this.cmbCourse.Items[0];
				ViewState[_courseFilterKey] = item.Value;
				Int32 selIdx = cmbCourse.Items.IndexOf(item);
				cmbCourse.SelectedIndex = selIdx;
				SearchResources();
			}
		}


		protected void cmbCourse_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			ViewState[_courseFilterKey] = e.Value;

			SetFilterVisibility();
			SearchResources();
		}

		private void BuildCategories()
		{
			cmbCategory.Visible = _categoryVisible;

			if (cmbCategory.Visible)
			{
				List<string> cats = (from i in _categoriesAndTypesDT.AsEnumerable() select i.Field<String>("Category")).Distinct().ToList();

				cmbCategory.Items.Clear();
				cmbCategory.Items.Add(new RadComboBoxItem("Category", "All"));

				foreach (string c in cats)
				{
					cmbCategory.Items.Add(new RadComboBoxItem(c, c));
				}

				RadComboBoxItem item = this.cmbCategory.Items.FindItemByValue((String)this.ViewState[_categoryFilterKey], true) ?? this.cmbCategory.Items[0];
				ViewState[_categoryFilterKey] = item.Value;
				cmbCategory.SelectedIndex = cmbCategory.Items.IndexOf(item);
			}
		}

		protected void cmbCategory_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			ViewState[_categoryFilterKey] = e.Value;

			SetFilterVisibility();
			SearchResources();
		}

		private void BuildTypes()
		{
			cmbType.Visible = _typeVisible;

			if (cmbType.Visible)
			{
				var categoryFilter = (ViewState[_categoryFilterKey].ToString() == "All") ? "" : ViewState[_categoryFilterKey].ToString();
				//TODO: Add dependency.
				List<string> types = (from i in _categoriesAndTypesDT.AsEnumerable()
									  where !i.Field<String>("TYPE").Contains("Lesson Plan") && !i.Field<String>("TYPE").Contains("Unit Plan") && !i.Field<String>("TYPE").Contains("Pacing Documents")
									  select i.Field<String>("TYPE")).Distinct().ToList();

				cmbType.Items.Clear();
				cmbType.Items.Add(new RadComboBoxItem("Type", "All"));

				foreach (string c in types)
				{
					cmbType.Items.Add(new RadComboBoxItem(c, c));
				}

				RadComboBoxItem item = this.cmbType.Items.FindItemByValue((String)this.ViewState[_typeFilterKey], true) ?? this.cmbType.Items[0];
				ViewState[_typeFilterKey] = item.Value;
				cmbType.SelectedIndex = cmbType.Items.IndexOf(item);
			}
		}

		protected void cmbType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			ViewState[_typeFilterKey] = e.Value;

			SetFilterVisibility();
			BuildSubtypes();
			SearchResources();
		}

		private void BuildSubtypes()
		{
			cmbSubtype.Visible = _subtypeVisible;

			if (cmbSubtype.Visible)
			{

				List<string> subtypes = ViewState[_typeFilterKey] == null ||
										ViewState[_typeFilterKey].ToString() == "All"
											? (from i in _categoriesAndTypesDT.AsEnumerable()
											   select i.Field<String>("SubType")).Distinct().ToList()
											: (from i in _categoriesAndTypesDT.AsEnumerable()
											   where i.Field<String>("TYPE") == ViewState[_typeFilterKey].ToString()
											   select i.Field<String>("SubType")).Distinct().ToList();

				//Filter Down

				cmbSubtype.Items.Clear();
				cmbSubtype.Items.Add(new RadComboBoxItem(_type == "Unit Plans" ? "Unit" : "Subtype", "All"));

				foreach (string c in subtypes)
				{
					cmbSubtype.Items.Add(new RadComboBoxItem(c, c));
				}

				RadComboBoxItem item = this.cmbSubtype.Items.FindItemByValue((String)this.ViewState[_subtypeFilterKey], true) ?? this.cmbSubtype.Items[0];
				ViewState[_subtypeFilterKey] = item.Value;
				cmbSubtype.SelectedIndex = cmbSubtype.Items.IndexOf(item);
			}
		}

		protected void cmbSubtype_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			ViewState[_subtypeFilterKey] = e.Value;

			SetFilterVisibility();
			SearchResources();
		}

		private void BuildStandards()
		{
			cmbStandard.Visible = _standardVisible;

			if (cmbStandard.Visible)
			{
				//TODO: Get Standards 

				foreach (DataRow row in _standardsDT.Rows)
				{
					cmbStandard.Items.Add(new RadComboBoxItem(row["StandardName"].ToString(), row["StandardID"].ToString()));
				}

				if (cmbStandard.Items.Count > 0)
				{
					RadComboBoxItem item = this.cmbStandard.Items.FindItemByValue((String)this.ViewState[_standardFilterKey], true);
					if (item == null) return;
					ViewState[_standardFilterKey] = item.Value;
					item.Checked = true;
					cmbStandard.SelectedIndex = cmbStandard.Items.IndexOf(item);
				}
			}
		}

		protected void cmbStandard_ItemChecked(object sender, RadComboBoxItemEventArgs e)
		{
			var dropdown = (RadComboBox)sender;
			ViewState[_standardFilterKey] = dropdown.CheckedItems.Count > 0 ? dropdown.CheckedItems[0].Value : "";

			SetFilterVisibility();
			SearchResources();
		}

		#endregion

		#region Resource Methods

		private void SearchResources()
		{
			if (_level == Base.Enums.EntityTypes.Standard)
				standardID = _levelID;
			else if (_standardVisible)
				standardID = DataIntegrity.ConvertToInt(ViewState[_standardFilterKey]);

			var type = _type == "Resources" ? ViewState[_typeFilterKey].ToString() : _type;

			resource = Thinkgate.Base.Classes.Resource.SearchResources(standardID, ViewState[_categoryFilterKey].ToString(),
				type, ViewState[_subtypeFilterKey].ToString(), SessionObject.GlobalInputs, grade: ViewState[_gradeFilterKey].ToString(), subject: ViewState[_subjectFilterKey].ToString(), course: ViewState[_courseFilterKey].ToString(),
				fullAccess: _fullAccess);

			lbxResources.DataSource = resource;
			lbxResources.DataBind();

			pnlGraphicNoResults.Visible = (resource.Count == 0);
			lbxResources.Visible = (resource.Count > 0);
		}

		protected void lbxResources_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
		{
			var listBoxItem = e.Item;

			var img = (System.Web.UI.HtmlControls.HtmlImage)listBoxItem.FindControl("imgAddToCalendar");
			var editLink = (HyperLink)listBoxItem.FindControl("btnGraphicUpdate");
			var viewLink = (HyperLink)listBoxItem.FindControl("lnkGraphicView");
			var dataItem = (Base.Classes.Resource)e.Item.DataItem;
			var lblResourceName = (Label)listBoxItem.FindControl("lblResourceName");
			var lnkResourceName = (HyperLink)listBoxItem.FindControl("lnkResourceName");

			if (img != null)
				img.Visible = _calendarIconVisible;

			if (editLink != null)
			{
				var link = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") +
					System.Web.HttpUtility.UrlEncode("display.asp?key=7266&fo=basic display&rm=page&xID=" + dataItem.ID + "&??hideButtons=Save And Return&??appName=E3");
				var editLinkVisible = false;

				lblResourceName.Visible = false;
				lnkResourceName.Visible = true;
				lnkResourceName.NavigateUrl = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "display.asp%3Fkey%3D7266%26fo%3Dbasic%20display%26rm%3Dpage%26xID%3D" + dataItem.ID + "%26%3F%3FhideButtons%3DSave%20And%20Return%26%3F%3FappName%3DE3";

				if (_type == "Unit Plans" && UserHasPermission(Permission.Icon_Edit_UnitPlan))
					editLinkVisible = true;
				else if (_type == "Lesson Plans" && UserHasPermission(Permission.Icon_Edit_LessonPlans))
					editLinkVisible = true;
				else if (_type == "Pacing Documents" && UserHasPermission(Permission.Icon_Edit_PacingDocuments))
					editLinkVisible = true;
				else if (_type == "Resources" && UserHasPermission(Permission.Icon_Edit_Resource))
					editLinkVisible = true;

				editLink.Visible = editLinkVisible;
				lnkResourceName.Visible = editLinkVisible;
				lblResourceName.Visible = !editLinkVisible;
				editLink.NavigateUrl = link;
			}

			if (viewLink != null)
			{
				var link = "";

				switch (dataItem.DocumentType)
				{
					case "External Content":
						link += ResolveUrl("~/upload/") + dataItem.ViewLink;
						break;
					case "Web Link":
						link += dataItem.ViewLink;
						break;
					default:
						link += ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + System.Web.HttpUtility.UrlEncode(dataItem.ViewLink);
						break;
				}

				viewLink.Target = "_blank";
				viewLink.NavigateUrl = link;
			}
		}

		#endregion
	}
}
