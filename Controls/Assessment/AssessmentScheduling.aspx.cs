using System.Web;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Spreadsheet;
using Standpoint.Core.Utilities;
using Standpoint.Core.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Base.Enums.AssessmentScheduling;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Controls.E3Criteria.Associations;
using Thinkgate.Controls;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Controls.E3Criteria.AssessmentSchedule;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentScheduling : BasePage
    {
        protected string UserRole { get; set; }
        protected string AssessmentCategory { get; set; }
		protected string AssessmentID { get; set; }
        public string ImageWebFolder { get; set; }
        private bool AllowClassLevelSchedules { get; set; }
        private bool AllowClassroomAssessmentScheduling { get; set; }
        private bool IncludeUnproofedAssessments { get; set; }
        public List<AssessmentSchedule> SearchResults{ get; set; }
		public List<Scheduling.Schedule> SchedulesList { get; set; }
		public List<Scheduling.ScheduleType> ScheduleTypesList { get; set; }
		public List<Scheduling.ScheduleLevel> ScheduleLevelsList { get; set; }
		public AssessmentScheduleLevels SchedLevelForSave;
		public string PageGuid;
		private AssessmentScheduleLevels scheduleCriteria_ScheduleLevel;
		public string DataTableCount;
		private DistrictParms districtParms;
        private bool isSecuredFlag;
        protected Dictionary<string, bool> dictionaryItem;

        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += SearchHandler;

            base.Page_Init(sender, e);

			//if (Master != null)
			//{
			//	RadScriptManager radScriptManager;
			//	var scriptManager = Master.FindControl("RadScriptManager1");
			//	if (scriptManager != null)
			//	{
			//		radScriptManager = (RadScriptManager)scriptManager;

			//		radScriptManager.Scripts.Add(new System.Web.UI.ScriptReference("~/Scripts/AssessmentSearch.js"));

			//	}
			//}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			//TODO: determine correct Permission
			//_Perm_Resource_Link_Ok = UserHasPermission(Permission.Hyperlink_Resource_Name);

			UserRole = Encryption.DecryptString(Request.QueryString["xID"].ToString());
			AssessmentCategory = Encryption.DecryptString(Request.QueryString["yID"].ToString());
			AssessmentID = (Request.QueryString["zID"] == null) ? "": Encryption.DecryptString(Request.QueryString["zID"].ToString());

			/************************************************************************************************
             * Security - Verify querystring parameters and user's roles are sufficient.
             ************************************************************************************************/

			Validate_Request();

			/************************************************************************************************
			 ************************************************************************************************
			 *							Initial Page Load.
			 ************************************************************************************************
			 ************************************************************************************************/
			var serializer = new JavaScriptSerializer();

			/************************************************************************************************
			 * Populate Class Properties and viewstate.
			 ************************************************************************************************/

			ImageWebFolder = (Request.ApplicationPath.Equals("/") ? string.Empty : Request.ApplicationPath) + "/Images/";

			districtParms = DistrictParms.LoadDistrictParms();

			//In this version of the app, are admins allowed to manage scheduling at the class level?
			AllowClassLevelSchedules = (districtParms.AssessmentSchedulerScheduleLevel == AssessmentScheduleLevels.Class.ToString());

			//In this version of the app, are admins allowed to set up scheduling for classroom assessments?
			AllowClassroomAssessmentScheduling = (districtParms.AssessmentSchedulerClassroomAssessments);

			//In this version of the app, are admins allowed to set up scheduling for proofed assessments, unproofed or both?
			IncludeUnproofedAssessments = (UserRole == "State") ?
												 districtParms.AssessmentSchedulerProofedOptionState :
												 districtParms.AssessmentSchedulerProofedOption; //Originally for Gwinnett

			//Load objects from viewstate if they are present, otherwise, pull from database.
			if (ViewState["ScheduleTypesList"] != null)
			{
				ScheduleTypesList = (List<Scheduling.ScheduleType>)ViewState["ScheduleTypesList"];
				ScheduleLevelsList = (List<Scheduling.ScheduleLevel>)ViewState["ScheduleLevelsList"]; ;
			}
			else
			{
				drGeneric_String_String AssessScheduleKeys = new drGeneric_String_String();
				var schedInstances = new List<Scheduling.Schedule>();
				var schedTypes = new List<Scheduling.ScheduleType>();
				var schedLevels = new List<Scheduling.ScheduleLevel>();

				Scheduling.GetScheduling(SessionObject.GlobalInputs,
										 ScheduleDocTypes.Assessment,
										 AssessmentScheduleLevels.District,
										 AssessScheduleKeys,
										 ref schedInstances,
										 ref schedTypes,
										 ref schedLevels);

				ScheduleTypesList = schedTypes;
				//Stash data from ScheduleTypesList in viewstate because we don't want to query for them again.
				ViewState["ScheduleTypesList"] = ScheduleTypesList;

				ScheduleLevelsList = schedLevels;

				//Stash key data from ScheduleLevelsList in viewstate because we don't want to query for them again.
				ViewState["ScheduleLevelsList"] = ScheduleLevelsList;
	
			}

			if (ViewState["SchedLevelForSave"] != null) SchedLevelForSave = (AssessmentScheduleLevels)ViewState["SchedLevelForSave"];

			if (!IsPostBack)
			{
				/************************************************************************************************
				 * Set up certain criteria controls with their selection values using Json arrays as datasources.
				 ************************************************************************************************/

				LoadCriteriaControls();

				/************************************************************************************************
				 * Reach up to the master page's RadAjaxPanel, and attach to it a javascript script to be 
				 * executed whenever the RadAjaxPanel's ajax call is executed.
				 ************************************************************************************************/
				RadAjaxPanel radAjaxPanel = (RadAjaxPanel)this.Master.FindControl("AjaxPanelResults");
				radAjaxPanel.ResponseScripts.Add("updateSelectedRowCount(0);");

				/************************************************************************************************
				 * If a AssessmentCategory was passed in, then set the Category combo box in the ScheduleCriteria control
				 * to this value and make it read only.
				 ************************************************************************************************/
				if (!string.IsNullOrEmpty(AssessmentCategory))
				{
					ctrlScheduleCriteria.DefaultCategory = AssessmentCategory;
					ctrlScheduleCriteria.CategoryReadOnly = true;
			}

			}

			if (radGridResults.DataSource == null)
            {
                radGridResults.Visible = false;
            }

            if (IsPostBack)
            {
				radGridResults.Visible = true;
			}

		}


        /// <summary>
        /// Handler for the Search Button
        /// This function is mapped to the SearchHandler Delegate in the Search.Master page. See Page_Init.
        /// </summary>
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {


			if (!string.IsNullOrEmpty(hdnAjxpEditCtrlValues.Value))
			{
				SaveEditControlChanges(hdnAjxpEditCtrlValues.Value);
				hdnAjxpEditCtrlValues.Value = "";
			}

            BindDataToGrid();

			// Build the Schedule_Edit Control. Couldn't do this before now, because we couldn't 
			// determine scheduleCriteria_ScheduleLevel until after calling BindDataToGrid().

			switch (SchedLevelForSave)
			{
				case AssessmentScheduleLevels.State:
					ctrlSchedEdit.ScheduleTypes = ScheduleTypesList.FindAll(x => "Admin, Content".IndexOf(x.TypeName) >= 0);
					break;

				case AssessmentScheduleLevels.ClientID:
					ctrlSchedEdit.ScheduleTypes = ScheduleTypesList.FindAll(x => "Admin, Content".IndexOf(x.TypeName) >= 0);
					break;

				case AssessmentScheduleLevels.District:
					ctrlSchedEdit.ScheduleTypes = ScheduleTypesList.FindAll(x => "Admin, Content, Print".IndexOf(x.TypeName) >= 0);
					break;

				case AssessmentScheduleLevels.School:
					ctrlSchedEdit.ScheduleTypes = ScheduleTypesList.FindAll(x => "Admin, Content".IndexOf(x.TypeName) >= 0);
					break;

				case AssessmentScheduleLevels.Class:
					ctrlSchedEdit.ScheduleTypes = ScheduleTypesList.FindAll(x => "Admin".IndexOf(x.TypeName) >= 0);
					break;
			}

			ctrlSchedEdit.RenderEditControl();

		}

        public void BindDataToGrid()
        {

            CriteriaController criteriaController = Master.CurrentCriteria();

            //
            // Reference Schedule criteria node and extract the selected values.
            //
            var scheduleCriteria_Category = "";
			scheduleCriteria_ScheduleLevel = AssessmentScheduleLevels.District;
            var scheduleCriteria = criteriaController.ParseCriteria<ScheduleCriteria.ValueObject>("ScheduleCriteria");
            if (scheduleCriteria.Count > 0) scheduleCriteria_Category = scheduleCriteria[0].Category;

			//
			// Reference District criteria node and extract the selected values.
			//
			var districts2_District = "";
			var districts2Criteria = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Districts2");
			if (districts2Criteria.Count > 0) districts2_District = districts2Criteria[0].Value;

            //
            // Reference District criteria node and extract the selected values.
            //
            var Status_Status = "";
            var StatusStatus = criteriaController.ParseCriteria<E3Criteria.DropDownList.ValueObject>("Status");
            if (StatusStatus.Count > 0) Status_Status = StatusStatus[0].Value;

			//
            // Reference Curriculum criteria node and extract the selected values. 
            //
            var curriculum_Grade = "";
            var curriculum_Subject = "";
            var curriculum_Curriculum = "";
            var curriculum_Type = "";
            var curriculum_Term = "";
            var curriculumCriteria = criteriaController.ParseCriteria<Curriculum.ValueObject>("Curriculum");
			Thinkgate.Base.Classes.Course currCourseSelected;
			int curriculum_ID = 0;
            if (curriculumCriteria.Count > 0)
            {
                curriculum_Grade = curriculumCriteria[0].Grade;
                curriculum_Subject = curriculumCriteria[0].Subject;
                curriculum_Curriculum = curriculumCriteria[0].Curriculum;

				currCourseSelected = CourseMasterList.CurrCourseList.Find(x => 
																				x.Grade.DisplayText == (string.IsNullOrEmpty(curriculum_Grade) ? x.Grade.DisplayText: curriculum_Grade) 
																				&& 
																				x.Subject.DisplayText == (string.IsNullOrEmpty(curriculum_Subject) ? x.Subject.DisplayText : curriculum_Subject) 
																				&& 
																				x.CourseName == (string.IsNullOrEmpty(curriculum_Curriculum) ? x.CourseName : curriculum_Curriculum)
																		);
				//curriculum_ID = currCourseSelected.ID;                
                curriculum_Type = curriculumCriteria[0].Type;
                curriculum_Term = curriculumCriteria[0].Term;
            }

            //
            //  Reference Classes criteria node and extract the selected values.
            //
            var classes_Teacher = "";
            var classes_Semester = "";
            var classes_Period = "";
            var classes_Section = "";
            var classes_Block = "";
            var classesCriteria = criteriaController.ParseCriteria<E3Criteria.AssessmentSchedule.ClassesCriteria.ValueObject>("Classes");
            if (classesCriteria.Count > 0)
            {
                classes_Teacher = classesCriteria[0].Teacher;
                classes_Semester = classesCriteria[0].Semester;
                classes_Period = classesCriteria[0].Period;
                classes_Section = classesCriteria[0].Section;
                classes_Block = classesCriteria[0].Block;
            }

            //
            //  Reference Schools criteria node and extract the selected values.
            //
            var schools_SchoolType = "";
            var schools_School = "";
            //var schools_SchoolID = "";
            var schoolsCriteria = criteriaController.ParseCriteria<E3Criteria.Associations.Schools.ValueObject>("Schools");
            if (schoolsCriteria.Count > 0)
            {
                schools_SchoolType = schoolsCriteria[0].SchoolType;
                schools_School = schoolsCriteria[0].School;
                //schools_SchoolID = schoolsCriteria[0].SchoolId;
            }

			//
			// Decide what columns to show.  To do this we must consider the UserRole (two choices at this time), the 
            // Assessment Category (three choices at this time) and the schedule Level (four possibilities at this 
            // time), however, after mapping out all the possible combinations (24 of them), its very sparse per the 
            // switch construct below.  Although switching on assessment category is unnecessary, I have left it in
            // to help with the thinking process should this logic ever need changing.
			var colDistrict = radGridResults.Columns[2];
			var colSchool = radGridResults.Columns[3];
			var colClass = radGridResults.Columns[4];
			var colTeacher = radGridResults.Columns[5];

			colDistrict.Visible = false;
			colSchool.Visible = false;
			colClass.Visible = false;
			colTeacher.Visible = false;

			switch (UserRole)
			{
				case "State":
                    colDistrict.Visible = true;

					//TODO: Schedule Level Dropdown should be a key/value pair instead of a one-column array list of string, 
					//      because when user selects:  
					//      "Assessment" or "District" then we should pass"Assessment" to the SP, 
					//      "School" then we should pass"District" to the SP, 
					//      "Class" then we should pass "School" to the SP.
					//
					//      For now, I am just going to use a switch to make this happen.  If the underlying SP should ever
					//      be re-written, then we can remove this goofy translation.
					switch (scheduleCriteria[0].ScheduleLevel)
					{
						case "Assessment":
							scheduleCriteria_ScheduleLevel = AssessmentScheduleLevels.State;
							break;

						case "District":
							scheduleCriteria_ScheduleLevel = AssessmentScheduleLevels.ClientID;
							break;
					}
					break;

				case "District":
					//TODO: Schedule Level Dropdown should be a key/value pair instead of a one-column array list of string, 
                    //      because when user selects:  
                    //      "Assessment" or "District" then we should pass"Assessment" to the SP, 
                    //      "School" then we should pass"District" to the SP, 
                    //      "Class" then we should pass "School" to the SP.
                    //
                    //      For now, I am just going to use a switch to make this happen.  If the underlying SP should ever
                    //      be re-written, then we can remove this goofy translation.
                    switch (scheduleCriteria[0].ScheduleLevel)
                    {
						case "State":
							scheduleCriteria_ScheduleLevel = AssessmentScheduleLevels.State;
							break;

						case "ClientID":
							scheduleCriteria_ScheduleLevel = AssessmentScheduleLevels.ClientID;
							break;

						case "Assessment":
							scheduleCriteria_ScheduleLevel = AssessmentScheduleLevels.District;
                            break;

                        case "School":
							scheduleCriteria_ScheduleLevel = AssessmentScheduleLevels.School;
                            break;

                        case "Class":
							scheduleCriteria_ScheduleLevel = AssessmentScheduleLevels.Class;
                            break;
                    }

					switch (scheduleCriteria_Category)
			        {
				        case "State":
				        case "District":
                        case "Classroom":
                            switch (scheduleCriteria_ScheduleLevel)
			                {
								case AssessmentScheduleLevels.School:
                                    colSchool.Visible = true;
                                    break;

								case AssessmentScheduleLevels.Class:
                                    colSchool.Visible = true;
                                    colClass.Visible = true;
                                    colTeacher.Visible = true;
                                    break;
                            }
					        break;
                    }

                    break;
			}

            //
            // Now make the call to the business object get our resulting dataset.
            //

            //if (Status_Status == string.Empty /*|| IncludeUnproofedAssessments*/)
            //    Status_Status = "All";            
            bool HasSecurePermission = false;
            if (UserHasPermission(Permission.Access_SecureTesting))
            {
                HasSecurePermission = true;
            }
            

            var searchResults = AssessmentSchedule.SearchAssessmentSchedules(gi: SessionObject.GlobalInputs,
																					ScheduleLevel: scheduleCriteria_ScheduleLevel,
                                                                                     //CurrGrade:curriculum_Grade,
                                                                                     ClassGrade: curriculum_Grade,
                                                                                     //CurrSubject: curriculum_Subject,
                                                                                     ClassSubject: curriculum_Subject,                                                                                     
                                                                                     //CurrCourse: curriculum_Curriculum,
                                                                                     ClassCourse: curriculum_Curriculum,
                                                                                     Teacher: classes_Teacher,
                                                                                     Section: classes_Section,
                                                                                     Period: classes_Period,
                                                                                     Block: classes_Block,
                                                                                     Semester: classes_Semester,
                                                                                     School:schools_School,
                                                                                     SchoolType: schools_SchoolType,
                                                                                     TestType: curriculum_Type,
                                                                                     Term: curriculum_Term,
                                                                                     UserPage: SessionObject.LoggedInUser.Page, 
                                                                                     Year: districtParms.Year,
                                                                                     Curriculum: "",//curriculum_ID.ToString(),
                                                                                     TestCategory: scheduleCriteria_Category,
                                                                                     DistrictID: districts2_District,
                                                                                     Proofed: Status_Status,
                                                                                     HasSecurePermission: HasSecurePermission);
                                                                                     


			//TODO: Lacking sufficient time to properly code what the SP returns as ScheduleInstance data, I have commented this section out and will alternately populate this object within the Caller's logic.
			//switch (scheduleCriteria_ScheduleLevel)
			//{
			//	case ScheduleLevels.District:
			//		foreach (var aSched in searchResults) { AssessScheduleKeys.Add(aSched.TestID, aSched.TestID); Debug.WriteLine(aSched.TestID.ToString() + " ^ " + aSched.TestID.ToString()); }
			//		break;

			//	case ScheduleLevels.School:
			//		foreach (var aSched in searchResults) AssessScheduleKeys.Add(aSched.TestID, aSched.SchoolID);
			//		break;

			//	case ScheduleLevels.Class:
			//		foreach (var aSched in searchResults) AssessScheduleKeys.Add(aSched.TestID, aSched.ClassID);
			//		break;
			//}


			//TODO: Lacking sufficient time to properly code what the SP returns as ScheduleInstance data, I am populating the ScheduleInstances object directly from here.
			//
			DataTableCount = searchResults.Count.ToString();
			var targetSchedLevel = ScheduleLevelsList.Find(x => x.ID == (int)scheduleCriteria_ScheduleLevel);

			var parentSchedLevel = (scheduleCriteria_ScheduleLevel == AssessmentScheduleLevels.District) ? 
									targetSchedLevel :
									ScheduleLevelsList.Find(x => x.ID == Math.Max((int)scheduleCriteria_ScheduleLevel - 1, 1));

			var adminSchedType = ScheduleTypesList.Find(x => x.TypeName == "Admin");
			var contentSchedType = ScheduleTypesList.Find(x => x.TypeName == "Content");
			var printSchedType = ScheduleTypesList.Find(x => x.TypeName == "Print");

			var serializer = new JavaScriptSerializer();
			var SchedKeysList = new List<string>();

			SchedulesList = new List<Scheduling.Schedule>();
			Scheduling.Schedule sched;
			string[] arryParsedID;
			foreach (var assessSched in searchResults)
			{

				arryParsedID = assessSched.ID.Split('_');

				SchedKeysList.Add(arryParsedID[0] + "|" + arryParsedID[2]);

				sched = new Scheduling.Schedule()
				{

					ScheduleTypeID = adminSchedType.TypeID,
					ScheduleTypeName = adminSchedType.TypeName,
					CssStyle = adminSchedType.DisplayStyle,
					DocumentTypeID = adminSchedType.DocTypeID,
					IsStartable = adminSchedType.IsStartable,
					IsEndable = adminSchedType.IsEndable,
					IsLockable = adminSchedType.IsLockable, 
					DisplayOrder = adminSchedType.DisplayOrder,
					schedID = DataIntegrity.ConvertToInt(arryParsedID[0]),  //TODO: When you get back to using schedule tables, this will point to ID in ScheduleInstances table.
					schedLvlID = (int)scheduleCriteria_ScheduleLevel,
					schedLvlEntityID = DataIntegrity.ConvertToInt(arryParsedID[2]),
					LevelLabel = targetSchedLevel.Name, 
					ParentLevelLabel = string.IsNullOrEmpty(parentSchedLevel.Name) ? "" : parentSchedLevel.Name,
					Locked = assessSched.AdminLock,
					Lock_Inherited = DataIntegrity.ConvertToBool(assessSched.AdminLock_Inherited),
					Lock_Begin = assessSched.AdminLock_Begin,
					Lock_End = assessSched.AdminLock_End, 
					Lock_Min = assessSched.AdminLock_Min, 
					Lock_Max = assessSched.AdminLock_Max,
					Lock_EffectiveBegin = assessSched.AdminLock_EffectiveBegin,
					Lock_EffectiveEnd = assessSched.AdminLock_EffectiveEnd,
					Lock_Inactive = adminSchedType.UnlockLabel,
					Lock_Active = adminSchedType.LockLabel, 
					Lock_Label = "Admin" //assessSched.AdminLock_Label
				};
				SchedulesList.Add(sched);
				if (scheduleCriteria_ScheduleLevel == AssessmentScheduleLevels.District || scheduleCriteria_ScheduleLevel == AssessmentScheduleLevels.School)
				{
					sched = new Scheduling.Schedule()
					{
						ScheduleTypeID = contentSchedType.TypeID,
						ScheduleTypeName = contentSchedType.TypeName,
						CssStyle = contentSchedType.DisplayStyle,
						DocumentTypeID = contentSchedType.DocTypeID,
						IsStartable = contentSchedType.IsStartable,
						IsEndable = contentSchedType.IsEndable,
						IsLockable = contentSchedType.IsLockable,
						DisplayOrder = adminSchedType.DisplayOrder,
						schedID = DataIntegrity.ConvertToInt(arryParsedID[0]),  //TODO: When you get back to using schedule tables, this will point to ID in ScheduleInstances table.
						schedLvlID = (int)scheduleCriteria_ScheduleLevel,
						schedLvlEntityID = DataIntegrity.ConvertToInt(arryParsedID[2]),
						LevelLabel = targetSchedLevel.Name,
						ParentLevelLabel = parentSchedLevel.Name,
						Locked = assessSched.ContentLock,
						Lock_Begin = assessSched.ContentLock_Begin,
						Lock_End = assessSched.ContentLock_End,
						Lock_Inherited = DataIntegrity.ConvertToBool(assessSched.ContentLock_Inherited),
						Lock_Min = assessSched.ContentLock_Min,
						Lock_Max = assessSched.ContentLock_Max,
						Lock_EffectiveBegin = assessSched.ContentLock_EffectiveBegin,
						Lock_EffectiveEnd = assessSched.ContentLock_EffectiveEnd,
						Lock_Inactive = contentSchedType.UnlockLabel,
						Lock_Active = contentSchedType.LockLabel,
						Lock_Label = assessSched.ContentLock_Label
					};
					SchedulesList.Add(sched);

					if (scheduleCriteria_ScheduleLevel == AssessmentScheduleLevels.District)
					{
						sched = new Scheduling.Schedule()
						{
							ScheduleTypeID = printSchedType.TypeID,
							ScheduleTypeName = printSchedType.TypeName,
							CssStyle = printSchedType.DisplayStyle,
							DocumentTypeID = printSchedType.DocTypeID,
							IsStartable = printSchedType.IsStartable,
							IsEndable = printSchedType.IsEndable,
							IsLockable = printSchedType.IsLockable,
							DisplayOrder = printSchedType.DisplayOrder,
							schedID = DataIntegrity.ConvertToInt(arryParsedID[0]),  //TODO: When you get back to using schedule tables, this will point to ID in ScheduleInstances table.
							schedLvlID = (int)scheduleCriteria_ScheduleLevel,
							schedLvlEntityID = DataIntegrity.ConvertToInt(arryParsedID[2]),
							LevelLabel = targetSchedLevel.Name,
							ParentLevelLabel = parentSchedLevel.Name,
							Locked = assessSched.PrintLock,
							Lock_Begin = assessSched.PrintLock_Begin,
							Lock_End = assessSched.PrintLock_End,
							Lock_Inherited = DataIntegrity.ConvertToBool(assessSched.PrintLock_Inherited),
							Lock_Min = assessSched.PrintLock_Min,
							Lock_Max = assessSched.PrintLock_Max,
							Lock_EffectiveBegin = assessSched.PrintLock_EffectiveBegin,
							Lock_EffectiveEnd = assessSched.PrintLock_EffectiveEnd,
							Lock_Inactive = printSchedType.UnlockLabel,
							Lock_Active = printSchedType.LockLabel,
							Lock_Label = assessSched.PrintLock_Label
						};
						SchedulesList.Add(sched);
					}
				}

			}

            radGridResults.DataSource = searchResults;

            radGridResults.DataBind();
			
			//Stash key data from SchedulesList in viewstate because we don't want to query for them again.
			if (ViewState["SchedKeys"] == null) ViewState.Add("SchedKeys", "");
			ViewState["SchedKeys"] = serializer.Serialize(SchedKeysList);

			if (ViewState["SchedLevelForSave"] == null) ViewState.Add("SchedLevelForSave",0);
			SchedLevelForSave = scheduleCriteria_ScheduleLevel;
			ViewState["SchedLevelForSave"] = (int)SchedLevelForSave;
        }

        protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            CriteriaController criteriaController = Master.CurrentCriteria();
            SearchHandler(sender, criteriaController);
            //BindDataToGrid();
            //ctrlSchedEdit.RenderEditControl();
        }

        protected void radGridResults_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)e.Item.FindControl("chkAll");

                var list = radGridResults.DataSource as List<AssessmentSchedule>;
                if (list.Count == 0)
                {
                    chk.Disabled = true;
                }
                else
                {
                    chk.Attributes["onclick"] = "selectAll(this,'" + radGridResults.ClientID + "'," + DataTableCount + ");";
                    chk.Disabled = false;
                }
            }

			if (e.Item is GridDataItem)
            {
                
                GridItem gridItem = e.Item;
				System.Web.UI.HtmlControls.HtmlInputCheckBox chk = (System.Web.UI.HtmlControls.HtmlInputCheckBox)e.Item.FindControl("chkRowInput");
                chk.Attributes["onclick"] = "selectThisRow(this,'" + radGridResults.ClientID + "'," + e.Item.ItemIndex + ");";
                chk.Attributes.Add("rowIndex", e.Item.ItemIndex.ToString());

				//
                // We are going to assign to the Schedules.ascx control, those schedules that apply to it
                //

                //Start by getting references to our row, our ascx control, the dataItem object that applies to our row
				var gridRowCtrl = (GridDataItem)e.Item;

				var ctrlSchedules = (Thinkgate.Controls.Schedules)gridRowCtrl["colSchedules"].FindControl("ctrlSchedules");
				var assessSched = (AssessmentSchedule)gridRowCtrl.DataItem;
				var arryParsedID = assessSched.ID.Split('_');

                //Next use the "FindAll" method to pull out of the ScheduleList, those rows that apply to our ascx 
                //control (according to dataItem), and assign these to the ascx control's DataSource property.
				ctrlSchedules.DataSource = SchedulesList.FindAll(x => x.schedID == DataIntegrity.ConvertToInt(arryParsedID[0]) && 
																	  x.schedLvlEntityID == DataIntegrity.ConvertToInt(arryParsedID[2]));

                //Next, call the ascx control's RenderSchedTable method which will populate the ascx control with the schedule information.
                ctrlSchedules.RenderSchedTable();

				//
				// Next, set up the hyperlink with needed NavigateURL value
				//
				//System.Web.UI.WebControls.HyperLink hLink = (System.Web.UI.WebControls.HyperLink)gridRowCtrl["colAssessmentLink"].Controls[0];
				//hLink.NavigateUrl = "~/Record/AssessmentObjects.aspx?xID=" + Encryption.EncryptString(assessSched.TestID.ToString());
                //hLink.Text = assessSched.TestName;
                HyperLink assessmentNameLabel;
                assessmentNameLabel = (HyperLink)gridItem.FindControl("lnkListTestName");
                assessmentNameLabel.NavigateUrl = "~/Record/AssessmentObjects.aspx?xID=" + Encryption.EncryptString(assessSched.TestID.ToString());
                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                if (assessSched.Secure && hasPermission)
                {
                    var img = e.Item.FindControl("imgIconSecure");
                    img.Visible = true;
                }
                

            }
        }


		protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
        {
            BindDataToGrid();
        }

        protected void RadGrid_NeedDataSource(object sender, EventArgs e)
        {
        }

		protected void SaveEditControlChanges(string JSONObj)
		{
			var serializer = new JavaScriptSerializer();
			List<SchedKey> schedList = new List<SchedKey>();
			List<Scheduling.ScheduleEditValues> SchedEditValuesList;

			bool selectAll = new bool();
	
			var header = radGridResults.MasterTableView.GetItems(GridItemType.Header)[0];
			if (header != null)
			{
				if (header.FindControl("chkAll") != null)
				{
				HtmlInputCheckBox chk = (HtmlInputCheckBox)header.FindControl("chkAll");

					if (chk.Attributes["checked"] == "checked")
					{
						selectAll = true;
					}
				}
			}

			if (selectAll)
			{
				/************************************************************************************************
				 * Fetch values for all assessment schedules from ViewState.
				 ************************************************************************************************/
				if (ViewState["SchedKeys"] != null)
				{
					string[] arryKeyValues;
					foreach (var keyString in serializer.Deserialize<List<string>>((string)ViewState["SchedKeys"]))
					{
						arryKeyValues = keyString.Split('|');
						schedList.Add(new SchedKey()
						{
							AssessmentID = DataIntegrity.ConvertToInt(arryKeyValues[0]),
							SchedLvlEntityID = DataIntegrity.ConvertToInt(arryKeyValues[1])
						});
					}
				}

			}
			else
			{

				/************************************************************************************************
				 * Fetch values from rad grid.
				 ************************************************************************************************/
				foreach (GridEditFormItem item in radGridResults.MasterTableView.GetItems(GridItemType.EditFormItem))
				{
					var chkBox = (HtmlInputCheckBox )item["colSelected"].FindControl("chkRowInput");

					string[] arryParsedID;
					if (chkBox != null && chkBox.Checked)
					{
						arryParsedID = chkBox.Value.Split('_');
						schedList.Add(new SchedKey() {AssessmentID = DataIntegrity.ConvertToInt(arryParsedID[0]), 
													 SchedLvlEntityID = DataIntegrity.ConvertToInt(arryParsedID[2])});
					}
				}
			}

			if (schedList.Count > 0)
			{

				SchedEditValuesList = serializer.Deserialize<List<Scheduling.ScheduleEditValues>>(JSONObj);
				//SchedEditValuesList = serializer.Deserialize<List<Scheduling.ScheduleEditValues>>(Request["__EVENTARGUMENT"].ToString());

				foreach (var key in schedList)
				{
					AssessmentSchedule.UpdateSchedules(key.AssessmentID,
													   SchedLevelForSave,
													   key.SchedLvlEntityID,
													   SchedEditValuesList);

				}
			}
		}

		/// <summary>
		/// Perform a series of checks to make sure that user has sufficient rights and that proper query string parameters were passed in.
		/// </summary>
		private void Validate_Request()
		{

            //
            // Verify that a User Role was provided (through querystring parameter "yID" (User))
            // Getting reference from ViewAssessmentsV2.aspx and ViewAssessments.aspx are using these five levels so appending these two(Class, Teacher) also.
            if ("School, State, District, Class, Teacher".ToLower().IndexOf(UserRole.ToLower()) < 0)
            {
                SessionObject.RedirectMessage = "Insufficient information to load request page.  Please contact your system administrator.";
                Response.Redirect("~/PortalSelection.aspx", true);
                return;
            }

            //
            // Verify that a AssessmentCategory was provided (through querystring parameter "yID" (category))
            //
            if ("State, District, Classroom, none".ToLower().IndexOf(AssessmentCategory.ToLower()) < 0)
            {
                SessionObject.RedirectMessage = "Insufficient information to load request page.  Please contact your system administrator.";
                Response.Redirect("~/PortalSelection.aspx", true);
                return;
            }

		}

		private void LoadCriteriaControls()
		{

			var serializer = new JavaScriptSerializer();
			//
			// If user is a state admin, then display the District Criteria control, otherwise, hide it.
			//
			divCtrlDistrictContainer.Attributes.CssStyle.Add("display", (UserRole.ToLower() == "state") ? "block" : "none");


			//
			//Set up Criteria Controls with datasource
			//
			ctrlClasses.JsonDataSource = serializer.Serialize(CourseMasterList.ClassCourseList.BuildJsonArray());
			//ctrlClasses.JsonDataSource = serializer.Serialize(Semester.BuildJsonArray());                
			ctrlSchools.JsonDataSource = serializer.Serialize(SchoolMasterList.BuildJsonArrayForTypeAndSchool());

            var statusList  = new List<System.Collections.Generic.KeyValuePair<string, string>>();
            statusList.Add(new KeyValuePair<string, string>("Proofed", "Yes"));
            
            //Parm controls unproofed option (Originally Gwinnett Specific)
            if(IncludeUnproofedAssessments)
                statusList.Add(new KeyValuePair<string, string>("Unproofed", "No"));
            ctrlStatus.DataSource = statusList;

            List<string> statusDefaults = new List<string>();
            statusDefaults.Add("Proofed");
            ctrlStatus.DefaultTexts = statusDefaults;

			//
			//Set up Criteria Controls with datasource
			//
			//TODO: hard coded for now 
			var districtList = new List<System.Collections.Generic.KeyValuePair<string, string>>();
			districtList.Add(new KeyValuePair<string, string>("District_01", "1"));
			districtList.Add(new KeyValuePair<string, string>("District_02", "2"));
			districtList.Add(new KeyValuePair<string, string>("District_03", "3"));
			districtList.Add(new KeyValuePair<string, string>("District_04", "4"));
			districtList.Add(new KeyValuePair<string, string>("District_05", "5"));
			districtList.Add(new KeyValuePair<string, string>("District_06", "6"));
			ctrlDistricts2.DataSource = districtList;

			//
			// Set up the Schedule Criteria control with datasources
			//
			var categoryArry = new ArrayList();
			DataTable tblCategories = Thinkgate.Base.Classes.Assessment.GetCategories(SessionObject.LoggedInUser.UserId);
			string category;
			foreach (DataRow row in tblCategories.Rows)
			{
				category = row["TestCategory"].ToString();

				//test by permission whether "Classroom" category should be a part of the criteria list)
				if (category != "Classroom" || AllowClassroomAssessmentScheduling) categoryArry.Add(new Object[] { category });
			}
			//

			var schedLvlArry = new ArrayList();
			//TODO: Schedule Level Dropdown should be a key/value pair instead of a one-column array list of string, 
			//      because when user selects:  
			//      "Assessment" or "District" then we should pass"Assessment" to the SP, 
			//      "School" then we should pass"District" to the SP, 
			//      "Class" then we should pass "School" to the SP.
			//
			//      For now, I am just going to use a switch to make this happen.  If the underlying SP should ever
			//      be re-written, then we can remove this goofy translation.

			schedLvlArry.Add(new object[] { "Assessment" });

			if (UserRole == "State")
			{
				schedLvlArry.Add(new object[] { "District" });
			}

			if (UserRole == "District")
			{
				if (districtParms.AssessmentSchedulerScheduleLevel == AssessmentScheduleLevelTexts.School.ToString() ||
					districtParms.AssessmentSchedulerScheduleLevel == AssessmentScheduleLevelTexts.Class.ToString()) schedLvlArry.Add(new object[] { AssessmentScheduleLevels.School.ToString() });

				if (AllowClassLevelSchedules) schedLvlArry.Add(new object[] { AssessmentScheduleLevelTexts.Class.ToString() });
			}

			// Now load the data into the ScheduleCriteria control's DataSources property.
			ctrlScheduleCriteria.DataSources.CategoryData = categoryArry;
			ctrlScheduleCriteria.DataSources.ScheduleLevelData = schedLvlArry;


			//
			// Set up the Curriculum criteria control with datasources
			// Note that as we build an object for the JsonDatasource for the criteria control to use, there will 
			// be one property (holding a big fat 3-column array of objects) in the object to populate the Grade, 
			// Subject, Criteria combo boxes, then another property in the object (a single column array of 
			// objects) to populate the Type combo box, and then a third property in the object (single column 
			// array of objects) to populate the term combo box.

			// Build an arraylist of the assessment types
			var listTypes = new ArrayList();
			switch (UserRole)
			{
				case "State":
					listTypes = TypeForCategoryJsonArray("State"); //TestTypes.TypeForCategoryJsonArray("State");
					break;
				case "District":
					listTypes =  TypeForCategoryJsonArray("District"); //TestTypes.TypeForCategoryJsonArray("District");
					break;
				case "Class":
					listTypes = TypeForCategoryJsonArray("Classroom");// TestTypes.TypeForCategoryJsonArray("Classroom");
					break;
			}

			var listTerms = Thinkgate.Base.Classes.Assessment.GetTermsJsonArray();

			// Build an arraylist of the Terms
			ctrlCurriculum.DataSources.GradeSubjectCurriculumData = CourseMasterList.CurrCourseList.BuildJsonArray();
			ctrlCurriculum.DataSources.TypeData = listTypes;
			ctrlCurriculum.DataSources.TermData = listTerms;
		}


        private ArrayList TypeForCategoryJsonArray(string Category)
        {            
            ArrayList typeList = new ArrayList();
            dictionaryItem = TestTypes.TypeWithSecureFlag(Category);
            isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            if (dictionaryItem != null && dictionaryItem.Select(c => c.Key).Any())
            {
                if (isSecuredFlag && UserHasPermission(Permission.Access_SecureTesting))
                {
                    typeList.Add(new object[] { dictionaryItem.Select(c => c.Key).ToArray() });
                }
                else
                {
                    typeList.Add(new object[] { dictionaryItem.Where(c => !c.Value).Select(c => c.Key).ToArray() });
                }
                
            }
            return typeList;
        }

	}



	public class SchedKey
	{
		public int AssessmentID;
		public int SchedLvlEntityID;
	}

}