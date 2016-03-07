using CMS.DocumentEngine;
using CMS.SiteProvider;
using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.InstructionMaterial
{
    public partial class InstructionAssignment : TileControlBase
    {
        protected Thinkgate.Base.Enums.EntityTypes _level;
        private int _groupID,_classID;
        protected Boolean _isPostBack;
        private Boolean _typeVisible;
        private string _type;
        private string _UseResourcesTileFilterToDisplay;
        private List<Base.Classes.Resource> resource;
        private DataTable _categoriesAndTypesDT;
        private DataTable _groupStudentsDT;
        private const String _typeFilterKey = "ReTypeFilter";

        private List<UserNodeList> _imNodeList;
        private TreeProvider _treeProvider;
        private Base.Classes.Teacher _selectedTeacher=null;
        public bool expiredContentView = true;

        public int IMDueEnd = DistrictParms.LoadDistrictParms().InstructionMaterialDueEnd;
        public int IMAssignmentStart = DistrictParms.LoadDistrictParms().InstructionMaterialAssignmentStart;

        #region Public Fields

        public string ResourceToShow;

        public List<KeyValuePair<string, string>> StrResourcesToShowOnTile;



        #endregion

        #region Public Properties

        public static string PortalName { get; set; }

        #endregion


        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            if (Tile == null || Tile.TileParms == null) return;

            //if (Tile.TileParms.GetParm("level") != null)
            //    _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");

            _groupID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("groupID"));
            _classID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("classID"));
            _UseResourcesTileFilterToDisplay = (Tile.TileParms.GetParm("UseResourcesTileFilterToDisplay") ?? "").ToString().ToLower();

            if (!String.IsNullOrEmpty(Request.QueryString["childPage"]))
            {
                _selectedTeacher = (Base.Classes.Teacher)Tile.TileParms.GetParm("selectedTeacher");
            }
            if(_selectedTeacher!=null)
            {
                _groupStudentsDT = Group.GetIMGroupStudentsByUserId(_selectedTeacher.PersonID , _classID, _groupID);
            }
            else
            {
                _groupStudentsDT = Group.GetIMGroupStudentsByUserId(SessionObject.LoggedInUser.Page, _classID, _groupID);
            }
            
            expiredContentView = UserHasPermission(Permission.AllowViewForIMUsageRightExpiredContent);            

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // Simulate IsPostBack.
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            // Create the initial viewstate values.
            // ViewState.Add(_typeFilterKey, _type == "Unit Plans" ? "Unit Plans" : "All");             

            // Set the current filter visibility.
            // SetFilterVisibility();
            PortalName = SessionObject.CurrentPortal.ToString();

            if (UserHasPermission(Permission.Icon_Assign_InstructionMaterial))
                btnAddIM.Visible = true;
            else
                btnAddIM.Visible = false;

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            SetResourceTypesToShow();
            if (!_isPostBack)
            {

                PopulateInstructionAssignmentTile();

                taskList.Add(new AsyncPageTask(BindFilterType));
                taskList.Add(new AsyncPageTask(BindFilterGroup));
                taskList.Add(new AsyncPageTask(BindFilterStudent));
                foreach (AsyncPageTask page in taskList)
                {
                    PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "InstructionAssignment", true);
                    Page.RegisterAsyncTask(newTask);
                }
                taskList = null;
                Page.ExecuteRegisteredAsyncTasks();
            }
           
        }

        private void BuildTypes()
        {
            cmbType.Visible = _typeVisible;

            if (cmbType.Visible)
            {
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

        protected void SetFilterVisibility()
        {
            if (_type == "Resources" && !string.IsNullOrEmpty(_UseResourcesTileFilterToDisplay))
            {
                _typeVisible = (_UseResourcesTileFilterToDisplay.Contains("type"));
            }
            else
            {
                _typeVisible = (_type == "Resources" || _level == EntityTypes.InstructionMaterial);

            }
        }

        protected void cmbType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_typeFilterKey] = e.Value;
            string docType = (string.IsNullOrEmpty(e.Value) || e.Value == "") ? "" : e.Value;
            string group = cmbGroups.SelectedValue;
            int groupID = (string.IsNullOrEmpty(group) || group == "0") ? 0 : Convert.ToInt32(group);
            string student = cmbStudents.SelectedValue;
            string studentID = (string.IsNullOrEmpty(student) || student == "0") ? "0" : student;
            FilterAndDisplayResult(docType, groupID, studentID);

            SetFilterVisibility();
        }
        protected void cmbGroups_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_typeFilterKey] = e.Value;

            int groupID = (string.IsNullOrEmpty(e.Value) || e.Value == "0") ? 0 : Convert.ToInt32(e.Value);

            string doc = cmbType.SelectedValue;
            string docType = (string.IsNullOrEmpty(doc) || doc == "") ? "" : doc;

            string student = cmbStudents.SelectedValue;
            string studentID = (string.IsNullOrEmpty(student) || student == "0") ? "0" : student;

            FilterAndDisplayResult(docType, groupID, studentID);

            SetFilterVisibility();
        }
        protected void cmbStudents_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_typeFilterKey] = e.Value;
            string studentID = (string.IsNullOrEmpty(e.Value) || e.Value == "0") ? "0" : e.Value;

            string group = cmbGroups.SelectedValue;
            int groupID = (string.IsNullOrEmpty(group) || group == "0") ? 0 : Convert.ToInt32(group);

            string doc = cmbType.SelectedValue;
            string docType = (string.IsNullOrEmpty(doc) || doc == "") ? "" : doc;

            FilterAndDisplayResult(docType, groupID, studentID);

            SetFilterVisibility();
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            string doc = cmbType.SelectedValue;
            string docType = (string.IsNullOrEmpty(doc) || doc == "" || doc=="All") ? "" : doc;

            string group = cmbGroups.SelectedValue;
            int groupID = (string.IsNullOrEmpty(group) || group == "0" || group == "All") ? 0 : Convert.ToInt32(group);

            string student = cmbStudents.SelectedValue;
            string studentID = (string.IsNullOrEmpty(student) || student == "0" || student == "All") ? "0" : student;

            FilterAndDisplayResult(docType, groupID, studentID);
            SetFilterVisibility();
            DisplayInstructionAssignments();

        }

        private void GetCategories()
        {
            if (_categoriesAndTypesDT == null) _categoriesAndTypesDT = Thinkgate.Base.Classes.Resource.GetResourceCategoriesDataTable(SessionObject.GlobalInputs);
        }



        #region Private Methods For Instruction Assignment Tiles

        private void PopulateInstructionAssignmentTile()
        {
            //According to Client State Documents will be displayed
            LoadInstructionAssignments();
            DisplayInstructionAssignments();
        }

        private int GetDecryptedEntityId(String entityIDKey)
        {
            string EntityIdEncrypted = Request.QueryString[entityIDKey];
            if (EntityIdEncrypted.Count() < 20) return Convert.ToInt32(EntityIdEncrypted);
            int entityId = Cryptography.DecryptionToInt(EntityIdEncrypted, SessionObject.LoggedInUser.CipherKey);
            entityId = entityId == 0 ? Standpoint.Core.Classes.Encryption.DecryptStringToInt(EntityIdEncrypted) : entityId;
            EntityIdEncrypted = HttpUtility.UrlEncode(EntityIdEncrypted);
            return entityId;
        }

        private void LoadInstructionAssignments()
        {
            UserInfo ui=null;
            string Key = string.Empty;
            if (!String.IsNullOrEmpty(Request.QueryString["childPage"]))
            {

                _selectedTeacher = (Base.Classes.Teacher)Tile.TileParms.GetParm("selectedTeacher");                               
                if(_selectedTeacher!= null)
                {
                    string kenticousername = KenticoHelper.GetKenticoUser(_selectedTeacher.EmployeeID.ToString());
                    ui = UserInfoProvider.GetUserInfo(kenticousername);
                    SetTreeProvider(_selectedTeacher.EmployeeID.ToString());
                }                
            }
            else
            {
                ui = (UserInfo)Session["KenticoUserInfo"];
                SetTreeProvider(SessionObject.LoggedInUser.ToString());
            }


            List<FilterTypeValues> _NodeList = _groupStudentsDT
            .AsEnumerable()
            .Select(x => new FilterTypeValues { NodeId = x["NodeId"].ToString(), createddate = Convert.ToDateTime(x["CreatedDate"].ToString()) })
            .ToList();

            var whereClause = "0";
            int counter = 0;
            foreach(var x in _NodeList.Distinct().ToList())
            {

                if (counter == 0)
                    whereClause = "";
                whereClause = counter == 0 ? x.NodeId.ToString() : whereClause + "," + x.NodeId.ToString();
                counter = 1;
            }
           
            _imNodeList = new List<UserNodeList>();
            foreach (var tileResource in StrResourcesToShowOnTile)
            {
                _imNodeList.AddRange(KenticoHelper.GetKenticoDocsForInstuctionAssignmentTiles(ui, tileResource.Value, _treeProvider,"NodeID in (" + whereClause +")"));
            }
            //List<KeyValuePair<string, DateTime>> _NodeList = _groupStudentsDT
            //  .AsEnumerable()
            //  //.OrderBy(p => p["createddate"].ToString())
            //  .Select(s => new KeyValuePair<string, DateTime>(s.Field<string>("NodeID"), Convert.ToDateTime( s.Field<string>("createddate"))))
            //  .ToList();

            //_imNodeList.Where(x => string.Format("mm/dd/yy", x.ExpirationDate) != "12/31/99").ToList().Where(y => y.ExpirationDate.AddDays(IM_DueEnd) <= DateTime.Now.Date).ToList();   // ForEach(y => y.ExpirationDate = y.ExpirationDate.AddDays( IM_DueEnd));
            if(!expiredContentView)
            { 
                var _tempNodeList = new List<UserNodeList>();
                _tempNodeList.AddRange(_imNodeList.Where(x =>  x.ExpirationDate == Convert.ToDateTime("12/31/9999 12:00:00 AM")).ToList());
                _imNodeList = _imNodeList.Where(x => x.ExpirationDate < Convert.ToDateTime("12/31/9999 12:00:00 AM")).ToList();
            
                if(_imNodeList.Count>0)
                { 
                _tempNodeList.AddRange(_imNodeList.Where(y => y.ExpirationDate.AddDays(IMDueEnd) >= DateTime.Now.Date).ToList());                
                }
                _imNodeList = _tempNodeList;
            }
            _imNodeList = (from m in _imNodeList
                           join n in _NodeList on m.NodeId equals n.NodeId
                           select  m).ToList().Distinct().ToList();

            _imNodeList.ForEach(x => x.CreatedDate = (_NodeList.Where(y => y.NodeId == x.NodeId).FirstOrDefault().createddate));
            _imNodeList = _imNodeList.OrderByDescending(x => x.CreatedDate).ToList();
            ViewState["NodeList"] = _imNodeList;
        }

        private void DisplayInstructionAssignments()
        {
            if (_imNodeList != null && _imNodeList.Count()>0)
            {
               // _imNodeList.Sort((x, y) => -x.LastModified.CompareTo(y.LastModified));

                pnlNoResults.Visible = false;
                lbxResources.Visible = true;
                lbxResources.DataSource = _imNodeList.ToList();
                lbxResources.DataBind();
            }
            else
            {
                pnlNoResults.Visible = true;
                lbxResources.DataSource = null;
                lbxResources.DataBind();
                lbxResources.Visible = false;
            }
        }

        private void SetTreeProvider(string userName)
        {
            _treeProvider = KenticoHelper.GetUserTreeProvider(userName);
        }




        private void BindFilterType()
        {
            cmbType.Items.Clear();
            cmbType.EmptyMessage = "Type";            
            cmbType.Items.Add(new RadComboBoxItem("All", "All"));
            foreach (var str in StrResourcesToShowOnTile)
            {
                cmbType.Items.Add(new RadComboBoxItem(str.Key, str.Value));
            }
            cmbType.Text = "";
            cmbType.ClearSelection();
        }

        private void BindFilterGroup()
        {
            cmbGroups.Items.Clear();        
            cmbGroups.EmptyMessage = "Group";
            cmbGroups.Items.Add(new RadComboBoxItem("All", "All"));
            List<int> nodelist = ((List<UserNodeList>)ViewState["NodeList"]).Select(x => Convert.ToInt32(x.NodeId)).Distinct().ToList();
            var lstGroups = (from DataRow g in _groupStudentsDT.Rows
                             where nodelist.Contains((int)g["NodeId"])
                             && (int)g["GroupID"]!=0
                             select new { ID = (int)g["GroupID"], GroupName = (string)g["GroupName"] }).GroupBy(x => x.GroupName).Select(y => y.First());

            foreach (var grp in lstGroups.OrderBy(x=>x.GroupName))
            {
                cmbGroups.Items.Add(new RadComboBoxItem(grp.GroupName, grp.ID.ToString()));
            }
             cmbGroups.Text = "";
             cmbGroups.ClearSelection();
        }

        private void BindFilterStudent()
        {
            cmbStudents.Items.Clear();           
            cmbStudents.EmptyMessage = "Student";
            cmbStudents.Items.Add(new RadComboBoxItem("All", "All"));
            List<int> nodelist = ((List<UserNodeList>)ViewState["NodeList"]).Select(x => Convert.ToInt32(x.NodeId)).Distinct().ToList();
            var lstStudents = (from DataRow g in _groupStudentsDT.Rows
                               where nodelist.Contains((int)g["NodeId"])
                               select new { ID = (int)g["StudentID"], StudentName = (string)g["StudentName"] }).GroupBy(x => x.StudentName).Select(y => y.First());

            foreach (var grp in lstStudents.OrderBy(x=>x.StudentName))
            {
                cmbStudents.Items.Add(new RadComboBoxItem(grp.StudentName, grp.ID.ToString()));
            }
            cmbStudents.Text = "";
            cmbStudents.ClearSelection();
        }

        private void SetResourceTypesToShow()
        {
            DataSet resourcesToShow = KenticoHelper.GetTileMapLookupDataSet();

            StrResourcesToShowOnTile = resourcesToShow.Tables[0]
                .AsEnumerable()
                .OrderBy(p => p["ItemOrder"].ToString())
                .Select(s => new KeyValuePair<string, string>(s.Field<string>("FriendlyName"), s.Field<string>("KenticoDocumentTypeToShow")))
                .ToList().Where(k => k.Value.ToLower() != "thinkgate.competencylist").ToList();       
        }


        private void FilterAndDisplayResult(string type, int? groupID, string studentID = null)
        {
            string whereClause="";
            if (groupID != 0) whereClause += "GroupID=" + groupID;
            if (studentID != "0") whereClause += whereClause == "" ? "StudentID=" + studentID : " and StudentID=" + studentID;

            List<string> _NodeList = _groupStudentsDT.Select(whereClause).Select(x => x.Field<Int32>("NodeID").ToString()).ToList();
            if (ViewState["NodeList"] == null) return;
            _imNodeList = (List<UserNodeList>)ViewState["NodeList"];
            _imNodeList = _imNodeList.Where(x => _NodeList.Contains(x.NodeId)).ToList();
            if (type!=string.Empty)
                _imNodeList = _imNodeList.Where(x => x.ClassName.Trim().ToLower() == type.ToLower().ToString()).ToList();
        }

        #endregion



    }


    public class FilterTypeValues
    {
       public  string NodeId { get; set; }
       public  DateTime createddate { get; set; }
    }

}
