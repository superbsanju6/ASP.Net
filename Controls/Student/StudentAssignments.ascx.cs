using Standpoint.Core.Classes;
using System;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Domain;
using Thinkgate.Domain.Classes;

namespace Thinkgate.Controls.Student
{
    public partial class StudentAssignments : TileControlBase
    {
        private int _ID;        
        AssignShareMode _Mode;
        public bool _ComboLoaded { get { Object obj = ViewState["ComboLoaded"]; if (obj == null) { return false; } else { return (bool)obj; } } set { ViewState["ComboLoaded"] = value; } }
                
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set Mode.            
            _Mode = (AssignShareMode)Tile.TileParms.GetParm("assignmentSharingTypeID");
                        
            // if combo not loaded, load it.
            if (!_ComboLoaded)
            {
                LoadCombo();
                _ComboLoaded = true;
            }
                                 
            // Load the results grid
            LoadGrid();                      
        }

        private void LoadCombo()
        {
            // Conditionally load combo box based on previously set parm
            switch (_Mode)
            {
                case AssignShareMode.Teacher:
                    // Teacher
                    _ID = Convert.ToInt32(Tile.TileParms.GetParm("assessmentID"));                    
                    LoadTypesComboForTeacher();
                    break;
                case AssignShareMode.Student:
                    // Student
                    _ID = (int)Tile.TileParms.GetParm("levelID");
                    LoadTypesComboForStudent();                    
                    break;                
            }
        }

        
        private void LoadGrid()
        {
            // TypeID determines with what information to render the tile (teachercentric or studentcentric).
            switch (_Mode)
            {
                case AssignShareMode.Teacher:
                    // Teacher
                    _ID = Convert.ToInt32(Tile.TileParms.GetParm("assessmentID"));
                    LoadAssignmentsGridForTeacher(0);                                                         
                    break;
                case AssignShareMode.Student:
                    // Student
                    _ID = (int)Tile.TileParms.GetParm("levelID");
                    LoadAssignmentsGridForStudent(0);                    
                    break;
                default:
                    return;
            }
        }

        private void LoadAssignmentsGridForTeacher(int participantTypeId)
        {            
            var assignmentSharing = new AssignmentSharing(AppSettings.ConnectionStringName);           

            if (participantTypeId == 0)
                grdAssignments.DataSource = assignmentSharing.GetAssignmentParticipants(_ID);
            else
                grdAssignments.DataSource = assignmentSharing.GetAssignmentParticipantsByParticipantType(_ID, participantTypeId);

            grdAssignments.DataBind();
        }

        private void LoadAssignmentsGridForStudent(int assignmentTypeId)
        {
            var assignmentSharing = new AssignmentSharing(AppSettings.ConnectionStringName);

            if (assignmentTypeId == 0)
                grdAssignments.DataSource = assignmentSharing.GetParticipantAssignments(_ID);
            else
                grdAssignments.DataSource = assignmentSharing.GetParticipantAssignmentsByAssignmentType(_ID, assignmentTypeId);

            grdAssignments.DataBind();
        }

        /// <summary>
        /// Fired each time an item is bound to the Assignments grid.
        /// </summary>        
        protected void grdAssignments_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                // Create link from name field
                GridDataItem dataItem = (GridDataItem)e.Item;                                
                HyperLink nameLink = (HyperLink)dataItem["Name"].Controls[0];
                string url = string.Empty;

                // Create link conditionally by portal
                switch (_Mode)
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

        private void LoadTypesComboForTeacher()
        {                           
            var context = new AssignmentSharing(AppSettings.ConnectionStringName);
            cmbType.DataTextField = "ParticipantTypeName";
            cmbType.DataValueField = "ParticipantTypeID";
            cmbType.DataSource = context.GetParticipantTypes();
            cmbType.DataBind();            
            RadComboBoxItem item = new RadComboBoxItem("Type", "0");
            cmbType.Items.Insert(0, item);                        
        }

        protected void cmbType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            // get selected type from combo
            int type = Convert.ToInt32(cmbType.SelectedValue);

            switch (_Mode)
            {
                case AssignShareMode.Teacher:
                    // Teacher
                    LoadAssignmentsGridForTeacher(type);                 
                    break;
                case AssignShareMode.Student:
                    // Student
                    LoadAssignmentsGridForStudent(type);
                    break;
                default:
                    return;
            }
        }

        private void LoadTypesComboForStudent()
        {
            var context = new AssignmentSharing(AppSettings.ConnectionStringName);
            cmbType.DataTextField = "TypeDisplayName";
            cmbType.DataValueField = "ContentTypeID";
            cmbType.DataSource = context.GetAssignmentTypes();
            cmbType.DataBind();
            RadComboBoxItem item = new RadComboBoxItem("Type", "0");
            cmbType.Items.Insert(0, item);
        }

    }
}