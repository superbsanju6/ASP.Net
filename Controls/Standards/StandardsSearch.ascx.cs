using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Telerik.Charting;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Standards
{
    public partial class StandardsSearch : TileControlBase
    {
        public static Thinkgate.Base.Enums.EntityTypes level;
        public static int levelID;
        protected bool _calendarIconVisible = false;
        public static int classID;
        public static string selectedFolder;
        private int _term;
        private int _planID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            if(!UserHasPermission(Base.Enums.Permission.Tile_Standards))
            {
                Tile.ParentContainer.Visible = false;
            }

            level = (Thinkgate.Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");
            levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));
            classID = level == Thinkgate.Base.Enums.EntityTypes.Class ? levelID : DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("selectID"));
            _planID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("planID"));
            _term = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("term"));
            selectedFolder = Tile.TileParms.GetParm("folder").ToString();
            standardsAssignedToItemTileGrid.Visible = false;
            StandardsCoursesXmlHttpPanel.Attributes["gridClientID"] = standardsSearchTileGrid.ClientID;
            StandardsSubjectsXmlHttpPanel.Attributes["gridClientID"] = standardsCurriculumSearchTileGrid.ClientID;

            if (Tile.TileParms.GetParm("showCalendarIcon") != null)
                _calendarIconVisible = DataIntegrity.ConvertToBool(Tile.TileParms.GetParm("showCalendarIcon"));

            var calendarColumn = standardsSearchTileGrid.Columns.FindByUniqueName("AddToCalendar");
            if (calendarColumn != null) calendarColumn.Visible = _calendarIconVisible;

            string postBackControlID = "";
            switch (level)
            {
                case Thinkgate.Base.Enums.EntityTypes.Student:
                case Thinkgate.Base.Enums.EntityTypes.Teacher:
                    postBackControlID = GetControlThatCausedPostBack(Parent.Page);
                    if(string.IsNullOrEmpty(postBackControlID)) postBackControlID = "";

                    if (selectedFolder == "Classes" && classID > 0)
                    {
                        LoadStandardsSearchButtonFilters();
                    }
                    else if (selectedFolder == "Curriculum" && classID == 0) {
                        standardsSetSearchDropdown.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(CurriculumFolder_StandardSetButton_SearchStandardsBySetAndCourse_Click);

                        if(postBackControlID.IndexOf("standardsSubjectSearchDropdown") == -1 && postBackControlID.IndexOf("standardsSetSearchDropdown") == -1)
                        {
                            LoadStandardsSearchButtonFilters();
                        }
                    }
                    break;

                case Thinkgate.Base.Enums.EntityTypes.Class:
                    postBackControlID = GetControlThatCausedPostBack(Parent.Page);
                    if(string.IsNullOrEmpty(postBackControlID)) postBackControlID = "";

                    selectedFolder = "Classes";
                    if (classID > 0)
                    {
                        standardsSetSearchDropdown.Width = 100;
                        standardsCourseSearchDropdown.Width = 100;
                        standardsSubjectSearchDropdown.Width = 100;
                        LoadStandardsSearchButtonFilters();
                    }
                    break;

                case Thinkgate.Base.Enums.EntityTypes.BankQuestion:
                    var oBQ = Tile.TileParms.GetParm("item") as BankQuestion;
                    var dtBQStds = BankQuestion.GetItemStandardsAsDataTable(oBQ);
                    int standardsRecordCount = dtBQStds.Rows.Count;

                    postBackControlID = GetControlThatCausedPostBack(Parent.Page) ?? "";
                    
                    /******************************************************************
                     * Set up the grids and controls we intend to show when this tile
                     * is used to display an item's standards.
                     * ***************************************************************/
                    standardsCurriculumSearchTileGrid.Visible = false;
                    standardsSearchTileGrid.Visible = false;
                    standardsSetSearchDropdown.Visible = false;
                    standardsCourseSearchDropdown.Visible = false;
                    standardsSubjectSearchDropdown.Visible = false;
                    standardSearchMoreLink.Visible = false;
                    searchHolder.Visible = false;

                    if (standardsRecordCount > 0)
                    {
                        standardsAssignedToItemTileGrid.Visible = true;
                        standardNoRecordsMsg.Visible = false;
                        standardsAssignedToItemTileGrid.DataSource = dtBQStds;
                        standardsAssignedToItemTileGrid.DataBind();
                        if (standardsRecordCount > 100) standardSearchMoreLink.Visible = true;
                    } 
                    else
                    {
                        standardNoRecordsMsg.Visible = true;
                    }
                    break;

                case Thinkgate.Base.Enums.EntityTypes.TestQuestion:
                    var oTQ = Tile.TileParms.GetParm("item") as TestQuestion;

                    //var drArray = StandardMasterList.dtStandards.Select("ID = " + oTQ.StandardID.ToString());
                    //if (drArray.Length > 0)
                    //{
                    //    oTQ.StandardName = drArray[0]["StandardName"].ToString();
                    //}

                    //oTQ.StandardName = StandardMasterList.GetStandardRowByID(oTQ.StandardID)["StandardName"].ToString() ?? "";
                    var drStd = Thinkgate.Base.Classes.Standards.GetStandardRowByID(oTQ.StandardID);
                    oTQ.StandardName = (drStd != null) ? drStd["StandardName"].ToString() : "";

                    DataTable dtTQStds = new DataTable();

                    dtTQStds.Columns.Add("ID", typeof (int));
                    dtTQStds.Columns.Add("StandardName", typeof (string));
                    dtTQStds.Columns.Add("StandardText", typeof(string));

                    var dr = dtTQStds.NewRow();
                    dr["ID"] = oTQ.StandardID;
                    dr["StandardName"] = oTQ.StandardName;
                    dr["StandardText"] = (drStd != null) ? drStd["Desc"].ToString() : "";
                    dtTQStds.Rows.Add(dr);
                    dtTQStds = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dtTQStds, "ID", "ID_Encrypted");

                    postBackControlID = GetControlThatCausedPostBack(Parent.Page) ?? "";
                    
                    /******************************************************************
                     * Set up the grids and controls we intend to show when this tile
                     * is used to display an item's standards.
                     * ***************************************************************/
                    standardsCurriculumSearchTileGrid.Visible = false;
                    standardsSearchTileGrid.Visible = false;
                    standardsSetSearchDropdown.Visible = false;
                    standardsCourseSearchDropdown.Visible = false;
                    standardsSubjectSearchDropdown.Visible = false;
                    standardSearchMoreLink.Visible = false;
                    searchHolder.Visible = false;
                    standardsAssignedToItemTileGrid.Visible = true;
                    standardNoRecordsMsg.Visible = false;
                    standardsAssignedToItemTileGrid.DataSource = dtTQStds;
                    standardsAssignedToItemTileGrid.DataBind();
                
                    break;
            }

            //For Now, If _calendarIconVisible is true we are on the Instructional Plan page and want to hide filters
            if (_calendarIconVisible)
            {
                standardsSetSearchDropdown.Visible = false;
                standardsCourseSearchDropdown.Visible = false;
                standardsSubjectSearchDropdown.Visible = false;
                radGridContainerDiv.Attributes["style"] = "width: 98%; height: 227px; padding: 3px; overflow: auto;"; //adding height
            }
        }

        protected void SearchStandardsBySetAndCourse_Click(object sender, EventArgs e)
        {
            if (_planID > 0)
            {
                SearchStandardsForInstructionalPlan();
                return;
            }
            //TODO: Analyze for performance on postback 
            string standardSet = standardsSetSearchDropdown.SelectedValue;
            string standardCourse = standardsCourseSearchDropdown.SelectedValue;
            string standardSubject = standardsSubjectSearchDropdown.SelectedValue;
            DataTable dt = null;

            if (standardSet == String.Empty) return;
            if (selectedFolder == "Classes" && standardCourse == String.Empty) return;

            if (selectedFolder == "Classes" && classID > 0)
            {
                dt = Base.Classes.Standards.SearchStandardsBy_Class_StandardsCourseAndSet(classID, standardSet, standardCourse, 0);
                standardsSearchTileGrid.Visible = true;
                standardsCurriculumSearchTileGrid.Visible = false;
            }
            else {
                dt = Base.Classes.Standards.SearchStandardsByTeacher_StandardsSubjectAndSet(levelID, standardSet, standardSubject, 0);
                standardsSearchTileGrid.Visible = false;
                standardsCurriculumSearchTileGrid.Visible = true;

            }

            int standardsRecordCount = 0;

            if (dt.Rows.Count > 0)
            {
                standardNoRecordsMsg.Visible = false;
                standardsRecordCount = DataIntegrity.ConvertToInt(dt.Select()[0].ItemArray[4]);

                dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "ID", "ID_Encrypted");

                if (selectedFolder == "Classes")
                {
                    standardsSearchTileGrid.DataSource = dt;
                    standardsSearchTileGrid.DataBind();
                }
                else
                {
                    dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "StandardCourseID", "StandardCourseID_Encrypted");
                    standardsCurriculumSearchTileGrid.DataSource = dt;
                    standardsCurriculumSearchTileGrid.DataBind();
                }
            }
            else
            {
                standardsSearchTileGrid.Visible = false;
                standardsCurriculumSearchTileGrid.Visible = false;
                standardNoRecordsMsg.Visible = true;
            }

            if (standardsRecordCount > 100)
            {
                standardSearchMoreLink.Visible = true;
            }
            else
            {
                standardSearchMoreLink.Visible = false;
            }
        }

        protected void SearchStandardsForInstructionalPlan()
        {
            DataTable dt = null;

            if (selectedFolder == "Classes" && classID > 0)
            {
                dt = Base.Classes.Standards.GetStandardsForInstructionalPlan(1, classID, _term);
                standardsSearchTileGrid.Visible = true;
                standardsCurriculumSearchTileGrid.Visible = false;
            }

            if (dt.Rows.Count > 0)
            {
                standardNoRecordsMsg.Visible = false;

                dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "StandardID", "ID_Encrypted");
                dt.Columns.Add("ID");
                dt.Columns.Add("Desc");
                foreach (DataRow r in dt.Rows)
                {
                    r["ID"] = r["StandardID"];
                    r["Desc"] = r["EssentialLearning"];
                }

                if (selectedFolder == "Classes")
                {
                    standardsSearchTileGrid.DataSource = dt;
                    standardsSearchTileGrid.DataBind();
                }
                else
                {
                    dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "StandardCourseID", "StandardCourseID_Encrypted");
                    standardsCurriculumSearchTileGrid.DataSource = dt;
                    standardsCurriculumSearchTileGrid.DataBind();
                }
            }
            else
            {
                standardsSearchTileGrid.Visible = false;
                standardsCurriculumSearchTileGrid.Visible = false;
                standardNoRecordsMsg.Visible = true;
            }

            standardSearchMoreLink.Visible = false;
        }

        protected void CurriculumFolder_StandardSetButton_SearchStandardsBySetAndCourse_Click(object sender, EventArgs e)
        {
            //TODO: Analyze for performance on postback 
            string standardSet = standardsSetSearchDropdown.Text;
            string standardSubject = (standardsSubjectSearchDropdown.Text == "Subject" ? "" : standardsSubjectSearchDropdown.SelectedValue);
            DataTable dt = null;

            if (standardSet == String.Empty) return;

            standardsSearchTileGrid.Visible = false;
            standardsCurriculumSearchTileGrid.Visible = true;

            dt = Base.Classes.Standards.SearchStandardsByTeacher_StandardsSubjectAndSet(levelID, standardSet, standardSubject, 0);
            int standardsRecordCount = 0;

            if (dt.Rows.Count > 0)
            {
                standardNoRecordsMsg.Visible = false;
                standardsRecordCount = DataIntegrity.ConvertToInt(dt.Select()[0].ItemArray[4]);

                dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "ID", "ID_Encrypted");
                dt = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(dt, "StandardCourseID", "StandardCourseID_Encrypted");
                    
                standardsCurriculumSearchTileGrid.DataSource = dt;
                standardsCurriculumSearchTileGrid.DataBind();
            }
            else
            {
                standardsSearchTileGrid.Visible = false;
                standardsCurriculumSearchTileGrid.Visible = false;
                standardNoRecordsMsg.Visible = true;
            }

            if (standardsRecordCount > 100)
            {
                standardSearchMoreLink.Visible = true;
            }
            else
            {
                standardSearchMoreLink.Visible = false;
            }

            string panelValue = "{\"ClassID\":\"0\", \"LevelID\":\"" + levelID.ToString() + "\", \"StandardsSet\":\"" + standardSet + "\"}";
            StandardsSubjectsXmlHttpPanel.Value = panelValue;
        }

        protected void LoadStandardsSearchButtonFilters()
        {
            var standardsList = Base.Classes.Standards.GetStandardSets(0);
            string selectedStandardSet = standardsSetSearchDropdown.SelectedValue;
            standardsSetSearchDropdown.Items.Clear();
            standardsSetSearchDropdown.Attributes["classID"] = classID.ToString();
            standardsSetSearchDropdown.Attributes["levelID"] = levelID.ToString();
            /*
            if (selectedFolder == "Curriculum")
            {
                standardsSearchSetList.ItemClick += new RadMenuEventHandler(CurriculumFolder_StandardSetButton_SearchStandardsBySetAndCourse_Click);
                standardsSearchSetList.OnClientItemBlur = "";
            }*/

            foreach (DataRow dr in standardsList.Rows)
            {
                RadComboBoxItem standardSetItem = new RadComboBoxItem();
                standardSetItem.Text = dr["Standard_Set"].ToString();
                standardSetItem.Value = dr["Standard_Set"].ToString();

                if (!String.IsNullOrEmpty(selectedStandardSet) && dr["Standard_Set"].ToString() == selectedStandardSet)
                {
                    standardSetItem.Selected = true;
                }

                standardsSetSearchDropdown.Items.Add(standardSetItem);
            }

            if (standardsSetSearchDropdown.Items.Count > 0)
            {
                if (standardsSetSearchDropdown.SelectedIndex == -1)
                {
                    standardsSetSearchDropdown.Items[0].Selected = true;
                }

                if (selectedFolder == "Classes")
                {
                    var standardsCourseList = Base.Classes.Standards.GetStandardsCoursesBy_Class_StandardSet(classID, standardsSetSearchDropdown.SelectedValue, 0);
                    standardsSetSearchDropdown.Visible = true;
                    standardsSubjectSearchDropdown.Visible = false;
                    standardsSetSearchDropdown.Attributes["xmlHttpPanelID"] = "StandardsCoursesXmlHttpPanel";

                    int selectedStandardsCourse = standardsCourseSearchDropdown.SelectedIndex;

                    standardsCourseSearchDropdown.Items.Clear();

                    standardsCourseSearchDropdown.DataSource = standardsCourseList;
                    standardsCourseSearchDropdown.DataTextField = "CourseText";
                    standardsCourseSearchDropdown.DataValueField = "CourseValue";
                    standardsCourseSearchDropdown.DataBind();

                    if (selectedStandardsCourse > -1)
                    {
                        standardsCourseSearchDropdown.SelectedIndex = selectedStandardsCourse;
                    }
                    else if (standardsCourseSearchDropdown.Items.Count > 0)
                    {
                        standardsCourseSearchDropdown.SelectedIndex = 0;
                        SearchStandardsBySetAndCourse_Click(null, null);
                    }
                }
                else if(selectedFolder == "Curriculum")
                {
                    var standardsSubjectList = Base.Classes.Standards.GetStandardsSubjectsBy_Teacher_StandardSet(levelID, standardsSetSearchDropdown.SelectedValue, 0);
                    standardsSubjectSearchDropdown.Visible = true;
                    standardsCourseSearchDropdown.Visible = false;
                    standardsSetSearchDropdown.Attributes["xmlHttpPanelID"] = "StandardsSubjectsXmlHttpPanel";

                    int selectedStandardsSubject = standardsSubjectSearchDropdown.SelectedIndex;

                    standardsSubjectSearchDropdown.Items.Clear();

                    standardsSubjectSearchDropdown.DataSource = standardsSubjectList;
                    standardsSubjectSearchDropdown.DataTextField = "SubjectText";
                    standardsSubjectSearchDropdown.DataValueField = "SubjectValue";
                    standardsSubjectSearchDropdown.DataBind();

                    if(selectedStandardsSubject > -1)
                    {
                        standardsSubjectSearchDropdown.SelectedIndex = selectedStandardsSubject;
                    }
                    else if (standardsSubjectSearchDropdown.Items.Count > 0)
                    {
                        standardsSubjectSearchDropdown.SelectedIndex = 0;
                        SearchStandardsBySetAndCourse_Click(null, null);
                    }
                }
            }
        }

        protected new static string GetControlThatCausedPostBack(System.Web.UI.Page page)
        {
            Control control = null;

            string ctrlname = page.Request.Params["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(ctrlname))
            {
                control = page.FindControl(ctrlname);
            }
            // if __EVENTTARGET is null, the control is a button type and we need to
            // iterate over the form collection to find it
            else
            {
                string ctrlStr = String.Empty;
                Control c = null;
                foreach (string ctl in page.Request.Form)
                {
                    //handle ImageButton they having an additional "quasi-property" in their Id which identifies
                    //mouse x and y coordinates
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        ctrlStr = ctl.Substring(0, ctl.Length - 2);
                        c = page.FindControl(ctrlStr);
                    }
                    else
                    {
                        c = page.FindControl(ctl);
                    }
                    if (c is System.Web.UI.WebControls.Button ||
                             c is System.Web.UI.WebControls.ImageButton)
                    {
                        control = c;
                        break;
                    }
                }
            }

            if (control == null && !string.IsNullOrEmpty(ctrlname) && ctrlname.IndexOf("standardsCourseSearchDropdown") > -1)
            {
                return ctrlname.Substring(ctrlname.IndexOf("standardsCourseSearchDropdown"));
            }

            return (control != null) ? control.ID : null;
        }

        protected void expandTile_Click(object sender, EventArgs e)
        {
            LinkButton link = (LinkButton)sender;

            if (link.ClientID == "standardSearchMoreLink")
            {
                SessionObject.StandardsSearchParms.DeleteParm("Standards Set");
                SessionObject.StandardsSearchParms.AddParm("Standards Set", standardsSetSearchDropdown.SelectedValue);
                SessionObject.StandardsSearchParms.DeleteParm("Standards Course");
                SessionObject.StandardsSearchParms.AddParm("Standards Course", standardsCourseSearchDropdown.SelectedValue);
                SessionObject.StandardsSearchParms.DeleteParm("Standards Subject");
                SessionObject.StandardsSearchParms.AddParm("Standards Subject", standardsSubjectSearchDropdown.SelectedValue);
            }

            if (Tile.ParentContainer != null)
                Tile.ParentContainer.ExpandTile(Tile, "Expand");
        }

        protected void SetStandardNameLinkAccess(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                var standardLink = item.FindControl("standardNameLink");
                var standardLabel = item.FindControl("standardNameLabel");

                if (standardLabel != null && standardLink != null)
                {
                    if(UserHasPermission(Base.Enums.Permission.Hyperlink_StandardName))
                    {
                        standardLink.Visible = true;
                        standardLabel.Visible = false;
                    }
                    else
                    {
                        standardLink.Visible = false;
                        standardLabel.Visible = true;
                    }
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void SetStandardSubjectLinkAccess(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                DataRowView itemDataRow = (DataRowView)item.DataItem;
                var standardLink = (LinkButton)item.FindControl("standardSubjectLink");
                var standardLabel = item.FindControl("standardSubjectLabel");

                if (standardLabel != null && standardLink != null)
                {
                    if (UserHasPermission(Base.Enums.Permission.Hyperlink_StandardName))
                    {
                        standardLink.Visible = true;
                        standardLabel.Visible = false;
                        standardLink.OnClientClick = string.Concat("customDialog({ url: '",
                            ResolveUrl("~/Controls/Standards/StandardsSearch_ExpandedV2.aspx?standardSet=" + itemDataRow["Standard_Set"] + "&standardCourseID=" + itemDataRow["StandardCourseID_Encrypted"]), "', maximize: true, maxwidth: 950, maxheight: 675 }); return false;");
                    }
                    else
                    {
                        standardLink.Visible = false;
                        standardLabel.Visible = true;
                    }
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }
    }
}