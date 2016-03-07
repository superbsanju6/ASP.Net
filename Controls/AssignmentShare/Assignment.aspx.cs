using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using System;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Controls.Reports;
using Thinkgate.Domain.Classes;
using Thinkgate.Utilities;
using assessmentAlias = Thinkgate.Base.Classes;

namespace Thinkgate.Controls.AssignmentShare
{
    public partial class Assignment : ExpandedSearchPage
    {
        /// <summary>
        /// Properties
        /// </summary>
        public AssignShareMode Mode { get { Object obj = ViewState["Mode"]; if (obj == null) { return 0; } else { return (AssignShareMode)obj; } } set { ViewState["Mode"] = value; } }
        public int ContentID { get { Object obj = ViewState["ContentID"]; if (obj == null) { return 0; } else { return (int)obj; } } set { ViewState["ContentID"] = value; } }
        public int EntityTypeID { get { Object obj = ViewState["EntityTypeID"]; if (obj == null) { return 0; } else { return (int)obj; } } set { ViewState["EntityTypeID"] = value; } }        
		protected string EncDistrictID { get; set; }
        
		private const string template = "<a class='searchLink' href='javascript: openStudent(\"@@##ID##@@\")'><div align='center'>"
								+ "<img width='50px' src='../../images/new/search_student_@@Demo6@@.png' border='0'/></div> "
								+ "@@StudentName@@<br/>@@StudentID@@<br/>@@Grade@@, @@Abbreviation@@</a>";

		private const string templateNoLink = "<div align='center'>"
								+ "<img width='50px' src='../../images/new/search_student_@@Demo6@@.png' border='0'/></div> "
								+ "@@StudentName@@<br/>@@StudentID@@<br/>@@Grade@@, @@Abbreviation@@";

		public EntityTypes _level;
	
		public int _levelID;

        
        /// <summary>
        /// Fired when page loads.
        /// </summary>        
        protected void Page_Load(object sender, EventArgs e)
        {

			if (!Page.User.Identity.IsAuthenticated || Session == null || Session.IsNewSession || Session["SessionObject"] == null)
			{
				Services.Service2.KillSession();
			}

            // mpf
			//ParseRequestQueryString();


            if (!Page.IsPostBack)
            {
                // Get EntityTypeID (Assessment, Student, etc.)
                EntityTypeID = Request.QueryString.Get("EntityTypeID") == null ? 0 : Convert.ToInt32(Request.QueryString.Get("EntityTypeID"));

                // Get ContentID
                ContentID = Request.QueryString.Get("ContentId") == null ? 0 : Convert.ToInt32(Request.QueryString.Get("ContentId"));

                // Get Mode
                Mode = (AssignShareMode)Convert.ToInt32(Request.QueryString.Get("Mode"));                

				//Populate EncDistrictID property, we'll need it in the aspx side to provide encrypted district ID.
				EncDistrictID = Standpoint.Core.Classes.Encryption.EncryptString(SessionObject.LoggedInUser.District.ToString());

                // TODOMPF: Testing - remove this line after developing.
                //DEBUG();                
                
				//Assign function to event handler
				ucAssignStudentsToAssessment.AssignStudentsToAssessmentReloadReport += AssignStudentsToAssessment_ReloadReport;
				//
                // Load Data                
                LoadData();
            }                                                                                            
        }
            
        /// <summary>
        /// This method is only for testing.  It is overriding page level parameters to allow dev to hit page immediately after login.
        /// </summary>
        private void DEBUG()
        {            
            // Assessment
            //EntityTypeID = 1;
            // Student
            EntityTypeID = 2;
            // Using this for teacher mode
            //ContentID = 5238;            
            // Using this for student mode
            ContentID = 38771;             
            //Mode = AssignShareMode.Teacher;
            Mode = AssignShareMode.Student;
        }

        /// <summary>
        /// Load form Data.
        /// </summary>
        private void LoadData()
        {
            // specify content types and load methods here.
            switch (EntityTypeID)
            {
                case 1:
                    LoadAssessment();
                    break;
                case 2:
                    LoadStudent();
                    break;
            }
        }

        /// <summary>
        /// Loads the form when in "Assessment" mode.
        /// </summary>
        private void LoadAssessment()
        {
            // fill out assessment related info                        
            assessmentAlias.Assessment ass = assessmentAlias.Assessment.GetAssessmentByID(ContentID);
            lblContentType.Text = "Assessment";
            lblAssignmentName.Text = string.Format("{0}: {1}", ass.TestName, ass.Description);            
        
            // set page title
            Page.Title = string.Format("{0}: {1} - Assignments", ass.TestName, ass.Description);
        }

        /// <summary>
        /// Loads the form when in "Student" mode.
        /// </summary>
        private void LoadStudent()
        {
            // fill out assessment related info                                    
            Base.Classes.Student stu = Base.Classes.Data.StudentDB.GetStudentByID(ContentID);
            lblContentType.Text = "Student";
            lblAssignmentName.Text = string.Format("{0} {1}", stu.FirstName, stu.LastName);

            // set page title
            Page.Title = string.Format("{0} {1} - Assignments", stu.FirstName, stu.LastName);

            // hide the EditCommandColumn in the Selected grid (can't edit due date in student mode)
            foreach (GridColumn column in gridSelected.MasterTableView.RenderColumns)
            {         
                if (column.ColumnType == "GridEditCommandColumn")
                {
                    GridEditCommandColumn editcol = (GridEditCommandColumn)column;
                    editcol.Display = false;
                }
            }
        }
        
        /// <summary>
        /// Fired when each control is created for the Selected grid
        /// </summary>        
        protected void gridSelected_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem)
            {
                // find the "Remove All" header button and tie an event handler to it
                GridHeaderItem headerItem = (GridHeaderItem)e.Item;
                RadButton btnRemoveAll = (RadButton)headerItem.FindControl("btnRemoveAll");
                if (btnRemoveAll != null)
                {
                    btnRemoveAll.Click += btnRemoveAll_Click;
                }
            }
            else if (e.Item is GridDataItem)
            {
                // find the "Remove" button and tie an event to it
                GridDataItem dataItem = (GridDataItem)e.Item;
                RadButton btnRemove = (RadButton)dataItem.FindControl("btnRemove");
                if (btnRemove != null)
                {
                    btnRemove.Click += btnRemove_Click;
                }
            }

            // disable datepicker freetext box in edit mode
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                GridEditableItem item = e.Item as GridEditableItem;
                RadDatePicker picker = (RadDatePicker)item["DueDate"].Controls[0];
                picker.DateInput.ReadOnly = true;                    
            }
        }

        /// <summary>
        /// Handles removing one participant from grid.
        /// </summary>        
        private void btnRemove_Click(object sender, EventArgs e)
        {
            int assignmentID;
            int participantID;

            // get id of content
            int contentID = ContentID;
            RadButton btn = (RadButton)sender;
            GridDataItem item = (GridDataItem)btn.NamingContainer;

            // Get ID of clicked row
            int rowInfoID = (int)item.GetDataKeyValue("ID");
            
            switch (Mode)
            {
                case AssignShareMode.Teacher:
                    // in this case, contendID is the Assignment, rowInfoID is the participant
                    assignmentID = contentID;
                    participantID = rowInfoID;
                    assessmentAlias.Assignment.DeleteAssignedParticipant(assignmentID, participantID);
                    break;
                case AssignShareMode.Student:
                    // in this case, rowInfoID is the assignment, contentID is the student
                    assignmentID = rowInfoID;
                    participantID = contentID;
                    assessmentAlias.Assignment.DeleteParticipantAssignment(rowInfoID, participantID);
                    break;
            }

            // Rebind grid
            gridSelected.Rebind();
        }

        /// <summary>
        /// Deletes all participants for the current assignment.
        /// </summary>
        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            foreach (GridDataItem row in gridSelected.Items)
            {
                int assignmentID;
                int participantID;
                
                // Get ID of clicked row
                int rowInfoID = (int)row.GetDataKeyValue("ID");

                switch (Mode)
                {
                    case AssignShareMode.Teacher:
                        // in this case, contendID is the Assignment, rowInfoID is the participant
                        assignmentID = ContentID;
                        participantID = rowInfoID;
                        assessmentAlias.Assignment.DeleteAssignedParticipant(assignmentID, participantID);
                        break;
                    case AssignShareMode.Student:
                        // in this case, rowInfoID is the assignment, contentID is the student
                        assignmentID = rowInfoID;
                        participantID = ContentID;
                        assessmentAlias.Assignment.DeleteParticipantAssignment(rowInfoID, participantID);
                        break;
                }                                                      
            }

            // rebind grid
            gridSelected.Rebind();
        }

        /// <summary>
        /// Handles getting data and rebinding "Selected" grid any time it needs a datasource.
        /// </summary>        
        protected void gridSelected_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {            
            switch (Mode)
            {
                case AssignShareMode.Teacher:
                    gridSelected.DataSource = assessmentAlias.Assignment.GetSelectedParticipants(ContentID);
                    break;
                case AssignShareMode.Student:
                    gridSelected.DataSource = assessmentAlias.Assignment.GetSelectedAssignments(ContentID);
                    break;
            }            
        }

        /// <summary>
        /// Fired when the grid row update button is clicked
        /// </summary>
        protected void gridSelected_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            if (Mode == AssignShareMode.Teacher)
            {
                // Update due date.
                GridEditableItem editedItem = (GridEditableItem)e.Item;
                int rowInfoID = Convert.ToInt32(editedItem.GetDataKeyValue("ID"));
                
                // ContentID is assignment, rowinfoID is the participantID.
                int assignmentID = ContentID;
                int participantID = rowInfoID;

                // Get reference to the datepicker.
                RadDatePicker pkr = (RadDatePicker)editedItem["DueDate"].Controls[0];
                DateTime? dueDate = pkr.SelectedDate;
                assessmentAlias.Assignment.UpdateAssignmentDueDateForSelectedParticipant(assignmentID, participantID, dueDate);
            }                        
        }        
        		
        /// <summary>
        /// Creates NavURL property for datagrid "name" field when in the Teacher portal.
        /// </summary>
        private string GetNavURLPropertyForTeacherPortal(GridDataItem dataItem)
        {
            // Determine participant type
            ParticipantType participantType = (ParticipantType)Convert.ToInt32(dataItem["TypeID"].Text);
            // Get info id
            int infoID = Convert.ToInt32(dataItem["InfoID"].Text);

            string navURL = string.Empty;

            switch (participantType)
            {
                case ParticipantType.Student:
                    // Create encrypted id link to student
                    Base.Classes.Student stu = Base.Classes.Data.StudentDB.GetStudentByID(infoID);
                    stu.IDEncrypted = stu.ID.ToString();
                    navURL = ResolveUrl("~/Record/Student.aspx?childPage=yes&xID=" + stu.IDEncrypted);
                    break;
                case ParticipantType.Class:
                    // Create hyperlink to class                    
                    navURL = ResolveUrl("~/Record/Class.aspx?xID=" + Encryption.EncryptString(infoID.ToString()));
                    break;
                case ParticipantType.Teacher:
                    // Create hyperlink to teacher
                    Base.Classes.Teacher tch = Base.Classes.Data.TeacherDB.GetTeacherByPage(infoID);
                    tch.ID_Encrypted = tch.PersonID.ToString();
                    navURL = ResolveUrl("~/Record/Teacher.aspx?childPage=yes&xID=" + tch.ID_Encrypted);
                    break;
                case ParticipantType.School:
                    // Create hyperlink to school                    
                    navURL = ResolveUrl("~/Record/School.aspx?childPage=yes&xID=" + Encryption.EncryptString(infoID.ToString()));
                    break;
                case ParticipantType.Group:
                    // TODO: Create hyperlink to group?
                    break;
            }

            return navURL;
        }

        /// <summary>
        /// Creates NavURL property for datagrid "name" field when in the Student portal.
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        private string GetNavURLPropertyForStudentPortal(GridDataItem dataItem)
        {
            // Determine assignment type
            AssignmentType assignmentType = (AssignmentType)Convert.ToInt32(dataItem["TypeID"].Text);
            // Get info id
            int infoID = Convert.ToInt32(dataItem["InfoID"].Text);

            string navURL = string.Empty;

            switch (assignmentType)
            {
                case AssignmentType.Assessment:
                    // Create encrypted id link to Assessment                    
                    Base.Classes.Assessment ass = Base.Classes.Assessment.GetAssessmentByID(infoID);
                    navURL = ResolveUrl("~/Record/AssessmentObjects.aspx?xID=" + Encryption.EncryptString(infoID.ToString()));
                    break;
            }        
        
            return navURL;
        }

        protected void gridSelected_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                // Create link from name field
                GridDataItem dataItem = (GridDataItem)e.Item;
                HyperLink nameLink = (HyperLink)dataItem["Name"].Controls[0];
                nameLink.Target = "_blank";
                string url = string.Empty;

                // Create link conditionally by portal
                switch (Mode)
                {
                    case AssignShareMode.Teacher:
                        url = GetNavURLPropertyForTeacherPortal(dataItem);
                        break;
                    case AssignShareMode.Student:
                        url = GetNavURLPropertyForStudentPortal(dataItem);
                        break;
                }

                // Set navigate url for "name" field                                      
                nameLink.NavigateUrl = url;
            }
		}
	
		public Control BuildTemplateSearchPager(int numberOfPages)
		{

			var wrapperDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
			var leftControls = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
			var centerControls = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
			var pageWrapper = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
			var rightControls = new System.Web.UI.HtmlControls.HtmlGenericControl("div");


			#region Add everything to left control
			//Add attributes to left
			leftControls.Attributes.Add("class", "rgWrap rgArrPart1");
			//add two inputs to left
			var firstPageBtn = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
			firstPageBtn.Attributes.Add("type", "button");
			firstPageBtn.Attributes.Add("title", "First Page");
			firstPageBtn.Attributes.Add("class", "rgPageFirst");
			firstPageBtn.Attributes.Add("value", " ");
			firstPageBtn.Attributes.Add("onclick", "goToPage(1);");
			firstPageBtn.Attributes.Add("style", "border:0px;");

			var prevPageBtn = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
			prevPageBtn.Attributes.Add("type", "button");
			prevPageBtn.Attributes.Add("title", "Previous Page");
			prevPageBtn.Attributes.Add("class", "rgPagePrev");
			prevPageBtn.Attributes.Add("value", " ");
			prevPageBtn.Attributes.Add("onclick", "goToPrevPage();");
			prevPageBtn.Attributes.Add("style", "border:0px;");

			leftControls.Controls.Add(firstPageBtn);
			leftControls.Controls.Add(prevPageBtn);
			#endregion

			#region Add everything to center control
			//Add attributes to center
			centerControls.Attributes.Add("class", "rgWrap rgNumPart");
			centerControls.Attributes.Add("id", "pagingScrollWrapper");

			pageWrapper.Attributes.Add("id", "numberWrapper");

			//add spans to pageWrapper inside of a tags for each page
			int i = 1;
			while (i <= numberOfPages)
			{

				var pageElement = new System.Web.UI.HtmlControls.HtmlGenericControl("a") { InnerHtml = "<span>" + i.ToString() + "</span>" };
				pageElement.Attributes.Add("id", "pageTag_" + i.ToString());
				pageElement.Attributes.Add("onclick", "goToPage(" + i.ToString() + ");");
				pageElement.Attributes.Add("class", (i == 1) ? "rgCurrentPage" : "");
				pageWrapper.Controls.Add(pageElement);
				i++;
                    
		}
        
			centerControls.Controls.Add(pageWrapper);

			#endregion

			#region Add everything to right control
			//Add attributes to left
			rightControls.Attributes.Add("class", "rgWrap rgArrPart2");
			//add two inputs to left
			var nextpageBtn = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
			nextpageBtn.Attributes.Add("type", "button");
			nextpageBtn.Attributes.Add("title", "Next Page");
			nextpageBtn.Attributes.Add("class", "rgPageNext");
			nextpageBtn.Attributes.Add("value", " ");
			nextpageBtn.Attributes.Add("onclick", "goToNextPage(1);");
			nextpageBtn.Attributes.Add("style", "border:0px;");

			var lastPageBtn = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
			lastPageBtn.Attributes.Add("type", "button");
			lastPageBtn.Attributes.Add("title", "Last Page");
			lastPageBtn.Attributes.Add("class", "rgPageLast");
			lastPageBtn.Attributes.Add("value", " ");
			lastPageBtn.Attributes.Add("onclick", "goToPage(" + numberOfPages + ");");
			lastPageBtn.Attributes.Add("style", "border:0px;");

			rightControls.Controls.Add(nextpageBtn);
			rightControls.Controls.Add(lastPageBtn);
			#endregion


			//add two button inputs to right

			//Add attributes to wrapper
			wrapperDiv.Attributes.Add("class", "rgWrap");
			wrapperDiv.Attributes.Add("id", "templateSearchPage");
			wrapperDiv.Attributes.Add("lastpage", numberOfPages.ToString());
			wrapperDiv.Style.Add("width", "200px");
			//add left to wrapper
			wrapperDiv.Controls.Add(leftControls);
			//add center to wrapper
			wrapperDiv.Controls.Add(centerControls);
			//add right to wrapper
			wrapperDiv.Controls.Add(rightControls);

			return wrapperDiv;

		}

		protected void RadGridResults_PageIndexChanged(object sender, Telerik.Web.UI.GridPageChangedEventArgs e)
		{
			SearchStudents("Grid");
		}

		protected void radGridResults_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
		{
			var item = e.Item as GridDataItem;

			if (item != null)
			{
				var link = (HyperLink)item["ViewHyperLinkColumn"].Controls[0];

				var id = string.Format("{0}", DataBinder.Eval(item.DataItem, "ID"));
				var encryptedId = Encryption.EncryptString(id);

				if (UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
				{
                    link.NavigateUrl = string.Format("~/Record/Student.aspx?childPage=yes&xID={0}", encryptedId);
				}
			}
		}

		protected void OnSortCommand(object sender, GridSortCommandEventArgs e)
		{
			SearchStudents("Grid");
		}

		private void AssignStudentsToAssessment_ReloadReport(object sender, EventArgs e)
		{
			this.SearchStudents("Grid");
		}

		public void SearchStudents(string resultType = "Grid")
		{
			var studentSearchParms = (Criteria)Session["Criteria_" + ucAssignStudentsToAssessment.HiddenGuid];

			var studentName = studentSearchParms.FindCriterionHeaderByText("Name").ReportStringVal ?? string.Empty;
			var studentID = studentSearchParms.FindCriterionHeaderByText("Student ID").ReportStringVal ?? string.Empty;

			var cluster = string.Join(",", studentSearchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Cluster") && !string.IsNullOrEmpty(r.ReportStringVal)).Select(t => t.Object));
			var schoolType = string.Join(",", studentSearchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("SchoolType") && !string.IsNullOrEmpty(r.ReportStringVal)).Select(t => t.Object));

			var schoolID = DataIntegrity.ConvertToInt(studentSearchParms.FindCriterionHeaderByText("School").ReportStringVal);

			var grades = string.Join(",", studentSearchParms.CriterionList.FindAll(r => !r.IsHeader && r.Key.Contains("Grade") && !string.IsNullOrEmpty(r.ReportStringVal)).Select(t => t.Object));

			// OLD CODE

			//var studentName = studentSearchParms.GetParm("Student Name") != null ? studentSearchParms.GetParm("Student Name").ToString() : string.Empty;
			//var studentID = studentSearchParms.GetParm("Student ID") != null ? studentSearchParms.GetParm("Student ID").ToString() : string.Empty;
			//var loginID = studentSearchParms.GetParm("Login ID") != null ? studentSearchParms.GetParm("Login ID").ToString() : string.Empty;
			//var grades = studentSearchParms.GetParm("Grades") != null ? string.Join(",", (List<string>)studentSearchParms.GetParm("Grades")) : string.Empty;
			//var cluster = string.Empty;
			//var schoolType = studentSearchParms.GetParm("School Type") != null ? string.Join(",", (List<string>)studentSearchParms.GetParm("School Type")) : string.Empty;
			//var schoolID = studentSearchParms.GetParm("School ID") != null ? DataIntegrity.ConvertToInt(studentSearchParms.GetParm("School ID")) : 0;

			var tier2RTI = string.Empty;
			var tier3RTI = string.Empty;
			var inactiveRTI = string.Empty;

			var rtiControl = studentSearchParms.FindCriterionHeaderByText("RTI");

			if (rtiControl != null && !string.IsNullOrEmpty(rtiControl.ReportStringVal))
			{
				var rtiSerializer = new JavaScriptSerializer();
				var rtiObject = rtiSerializer.Deserialize<ReportCriteria.RTIJsonObject>(rtiControl.ReportStringVal);
				if (rtiObject != null)
				{
					foreach (var tier in rtiObject.items)
					{
						switch (tier.text)
						{
							case "Former Year":
								inactiveRTI = "yes";
								break;

							case "Current Tier 2":
								tier2RTI = "yes";
								break;

							case "Current Tier 3":
								tier3RTI = "yes";
								break;
						}
					}
				}
			}

			var demoString = string.Empty;
			var demographicControl = studentSearchParms.FindCriterionHeaderByText("Demographics");

			if (demographicControl != null && !string.IsNullOrEmpty(demographicControl.ReportStringVal))
			{
				var serializer = new JavaScriptSerializer();
				var demographicObject = serializer.Deserialize<ReportCriteria.DemographicJsonObject>(demographicControl.ReportStringVal);
				if (demographicObject != null)
				{
					demoString = demographicObject.items.Aggregate(string.Empty, (current, demographic) => current + ("@@D" + demographic.demoField + "=" + demographic.value + "@@"));
				}
			}

			DataTable dataSource = Base.Classes.Data.StudentDB.Search(0, _level.ToString(), _levelID, studentName, studentID, grades, cluster, schoolType, schoolID, tier2RTI, tier3RTI, inactiveRTI, string.Empty, demoString);
			lblSearchResultCount.Text = "Results Found: " + (dataSource == null ? 0 : dataSource.Rows.Count);


			//TODO:DHB - Need to turn datasource into a JSon array to be consumed by the hosting page.
			//LoadResults(dataSource, buttonsContainer1, tileDiv1, tileResultsPanel, gridResultsPanel, radGridResults, resultType,
			//	UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName) ? template : templateNoLink);
		}

		private void ParseRequestQueryString()
		{
			if (string.IsNullOrEmpty(Request.QueryString["level"]))
			{
				SessionObject.RedirectMessage = "No level provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}

			_level = (EntityTypes)EnumUtils.enumValueOf(Request.QueryString["level"], typeof(EntityTypes));

			if (string.IsNullOrEmpty(Request.QueryString["levelID"]))
			{
				SessionObject.RedirectMessage = "No levelID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}

			_levelID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "levelID");
		}
                        
    }
}