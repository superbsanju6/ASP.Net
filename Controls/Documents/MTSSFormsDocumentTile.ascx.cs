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
using Thinkgate.Domain.Classes;
using CultureInfo = System.Globalization.CultureInfo;
using CMS.FormEngine;
using CMS.SettingsProvider;
using CMS.DataEngine;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.GlobalHelper;

namespace Thinkgate.Controls.Documents
{
    public partial class MTSSFormsDocumentTile : TileControlBase
    {
        //Variables
        private string _type;
        public int _clientBaseLookupEnum;
        private string visibleTab = null;

        //Objects
        private List<UserNodeList> userNodeList;
        private List<UserNodeList> userNodeList504;
        private EntityTypes _level;
        private Int32 _levelID;
        private MTSSSessionObject _MTSSSession;
        private UserInfo ui;
        private TreeProvider tree;

        //Constants
        private const String FORWARD_SLASH = "/";
        private const String KenticoUserInfo = "KenticoUserInfo";
        private const String E3UserInfo = "SessionObject";
        private const String PageSessionStateName = "MTSSForms";
        private const String KenticoItemType = "CMS.MenuItem";
        private const String Tier1 = "Analysis";
        private const String Tier2_3 = "Intervention";
        private const String InterventionBehavioral = "Behavioral";
        private const String InterventionAcademic = "Academic";
        private const String InterventionAll = "Behavioral, Academic";
        private const String Form504WhereClause = "DocumentID IN (SELECT DocumentID FROM CMS_DocumentCategory dc INNER JOIN CMS_Category c ON dc.CategoryID = c.CategoryID WHERE C.CategoryDisplayName = '504')";
        private const String FormWhereClause = "DocumentID NOT IN (SELECT DocumentID FROM CMS_DocumentCategory dc INNER JOIN CMS_Category c ON dc.CategoryID = c.CategoryID WHERE C.CategoryDisplayName = '504')";

        #region Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Tile == null || Tile.TileParms == null) return;

            /**********************************************************************************************
            *  Only update for this control firing (Telerik Ajax postback for all user controls)          *
            *  Looks for postback control id's containing "MTSS Forms" (quick solution)                   *
            **********************************************************************************************/
            if (IsPostBack)
            {
                String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
                if (!String.IsNullOrEmpty(postBackControlID)
                    && !postBackControlID.StartsWith("folder")
                    && !postBackControlID.StartsWith("tileContainer")
                    && !postBackControlID.Contains("MTSSForms"))
                {
                    return;
                }
            }



            //COMMENT: RE this was breaking code, not sure what/if is required put a condition to protect from breaking
            if (Tile.TileParms.GetParm("type") != null)
                _type = Tile.TileParms.GetParm("type").ToString();

            if (Tile.TileParms.GetParm("level") != null)
                _level = (Base.Enums.EntityTypes)Tile.TileParms.GetParm("level");

            if (Tile.TileParms.GetParm("levelID") != null)
                _levelID = (Int32)Tile.TileParms.GetParm("levelID");

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            initializeSession();

            setTabSecurity();

            //Needed to close popup window
            btnCancelWhere.CommandArgument = wndAddDocument.ClientID;
            btnOkWhere.CommandArgument = wndAddDocument.ClientID;
            KenticoVirtualFolder.Value = KenticoHelper.KenticoVirtualFolder;

            for (int i = wndWindowManager.Windows.Count - 1; i > -1; i--)
            {
                if (!new List<string> { "wndAddDocument", "wndCmsNewDocumentShell" }.Contains(wndWindowManager.Windows[i].ID))
                {
                    wndWindowManager.Windows.Remove(wndWindowManager.Windows[i]);
                }
            }
            if (!IsPostBack)
            {
                addNew(tree, Tile.TileParms);
                RefreshMTSSData();
            }
        }

        protected void cmbUnaligned_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RefreshMTSSData();
        }

        #endregion

        #region Functions

        private void setTabSecurity()
        {
            if (_level == EntityTypes.District)
            {
                if (UserHasPermission(Permission.Tab_504_RTIFormsTile_DistrictPortal))
                {
                    Tab504Forms.Visible = true;
                    visibleTab = "504";
                }
                if (UserHasPermission(Permission.Tab_Unaligned_RTIFormsTile_DistrictPortal))
                {
                    TabUnAlignedView.Visible = true;
                    visibleTab = "UnAligned";
                }
                if (UserHasPermission(Permission.Tab_Templates_RTIFormsTile_DistrictPortal))
                {
                    TabTemplatesView.Visible = true;
                    visibleTab = "Templates";
                }

            }
            else if (_level == EntityTypes.School)
            {
                if (UserHasPermission(Permission.Tab_504_RTIFormsTile_SchoolPortal))
                {
                    Tab504Forms.Visible = true;
                    visibleTab = "504";
                }
                if (UserHasPermission(Permission.Tab_Unaligned_RTIFormsTile_SchoolPortal))
                {
                    TabUnAlignedView.Visible = true;
                    visibleTab = "UnAligned";
                }
                if (UserHasPermission(Permission.Tab_Templates_RTIFormsTile_SchoolPortal))
                {
                    TabTemplatesView.Visible = true;
                    visibleTab = "Templates";
                }
            }
            else if (_level == EntityTypes.Teacher)
            {
                if (UserHasPermission(Permission.Tab_504_RTIFormsTile_TeacherPortal))
                {
                    Tab504Forms.Visible = true;
                    visibleTab = "504";
                }
                if (UserHasPermission(Permission.Tab_Unaligned_RTIFormsTile_TeacherPortal))
                {
                    TabUnAlignedView.Visible = true;
                    visibleTab = "UnAligned";
                }
                if (UserHasPermission(Permission.Tab_Templates_RTIFormsTile_TeacherPortal))
                {
                    TabTemplatesView.Visible = true;
                    visibleTab = "Templates";
                }
            }

            //Configure Active Tab if enabled
            if (visibleTab != null)
            {
                RadTabStrip2.Tabs.FindTabByText(visibleTab).Selected = true;
            }
            else
            {
                //Disable Add button if no tabs enabled
                btnAdd.Style["Display"] = "None";
            }
        }

        private void initializeSession()
        {
            //UserPage as hidden field
            userpage.Value = SessionObject.LoggedInUser.Page.ToString();

            //Kentico Objects
            ui = (UserInfo)Session[KenticoUserInfo];
            tree = KenticoHelper.GetUserTreeProvider(SessionObject.LoggedInUser.ToString());

            //MTSS Session
            if (Session[PageSessionStateName] == null)
            {
                _MTSSSession = new MTSSSessionObject();
                _MTSSSession.SchoolID = 0;
                _MTSSSession.Grade = String.Empty;
                _MTSSSession.Subject = String.Empty;
                _MTSSSession.StudentID = 0;
                _MTSSSession.UserPage = SessionObject.LoggedInUser.Page;
                _MTSSSession.UserCrossCourses = SessionObject.LoggedInUser.HasPermission(Permission.User_Cross_Courses);
                Session[PageSessionStateName] = _MTSSSession;

            }
            else
            {
                _MTSSSession = (MTSSSessionObject)Session[PageSessionStateName];
            }
        }

        private void RefreshMTSSData()
        {
            //School must be selected and at least one tab must be visible to refresh data
            if (visibleTab != null)
            {
                //Initialize Object
                userNodeList = new List<UserNodeList>();
                userNodeList504 = new List<UserNodeList>();

                //Find Kentico ASPX pages that are hosting BizForms
                userNodeList = KenticoHelper.SearchDocumentTypeReferencesModified(ui, KenticoItemType, tree, "Forms", FormWhereClause);
                userNodeList504 = KenticoHelper.SearchDocumentTypeReferencesModified(ui, KenticoItemType, tree, "Forms", Form504WhereClause);

                //Node.GetPreviewLink(CurrentUser.UserName, isFile);
                //CMS.DocumentEngine.TreeNode tn = CMS.DocumentEngine.TreeNode.New();

                //Normal Forms
                if (userNodeList != null)
                {
                    MTSSFormsList.DataSource = userNodeList.ToList();
                    MTSSFormsList.DataBind();
                }

                //504 forms
                if (userNodeList504 != null)
                {
                    Forms504List.DataSource = userNodeList504.ToList();
                    Forms504List.DataBind();
                }

                //Get Kentico Form Records
                List<KenticoMTSSFormData> kentico = KenticoHelper.GetKenticoFormData(userNodeList);

                //Call RTI Stored Procedure in E3 
                E3InterventionDataObject E3obj = MTSSHelper.getE3InterventionData(_MTSSSession.UserPage, _MTSSSession.UserCrossCourses, _MTSSSession.SchoolID, _MTSSSession.Grade, _MTSSSession.Subject, _MTSSSession.StudentID, Tier2_3);

                //Populate Missing Results
                populateUnalignedResults(kentico, E3obj);

                //Add New DropDowns
                PopulateInterventionStudentDropDown();
                PopulateFormDropDown(userNodeList);
            }

        }

        private void PopulateInterventionStudentDropDown()
        {
            E3InterventionDataObject E3obj = MTSSHelper.getE3InterventionData(_MTSSSession.UserPage, true, 0, String.Empty, String.Empty, 0, "Both");

            cmbStudentName.DataSource = E3obj.StudentsObject.ToList();
            cmbStudentName.DataTextField = "StudentName";
            cmbStudentName.DataValueField = "StudentID";
            cmbStudentName.DataBind();

        }

        private void PopulateFormDropDown(List<UserNodeList> userNodes)
        {
            cmbFormName.DataSource = userNodes.ToList();
            cmbFormName.DataTextField = "NodeName";
            cmbFormName.DataValueField = "NodeAliasPath";
            cmbFormName.DataBind();

        }

        private void addNew(TreeProvider tp, TileParms tParms)
        {
            string theClientID = string.Empty;
            theClientID = DistrictParms.LoadDistrictParms().ClientID;

            if (theClientID != null && theClientID != string.Empty)
            {
                btnAdd.Visible = true; // might need to disable via E3 roles later
                if (btnAdd.Visible)
                {
                    //if a district admin or a state admin then display options to choose, if niether then only one option and skip this form and move ahead to next
                    var session = (SessionObject)Session["SessionObject"];
                    btnAdd.Attributes["onclick"] = string.Format("javascript:openCmsDialogWindow($find('{0}'));", wndAddDocument.ClientID);
                }
            }
        }

        private RadButton GetSelectedRadButtonByGroupName(string groupName)
        {
            return Controls.OfType<RadButton>().FirstOrDefault(radButton => radButton.Checked && String.Compare(radButton.GroupName, groupName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private void populateUnalignedResults(List<KenticoMTSSFormData> kentico, E3InterventionDataObject E3obj)
        {
            Table table = new Table();

            //Create HTML Table for tile
            if (MTSSFormscmbUnaligned.SelectedValue == "RTI")
            {
                table = MTSSHelper.GenerateHTMLTableForMissingInterventions(kentico, E3obj);
            }
            else if (MTSSFormscmbUnaligned.SelectedValue == "KENTICO")
            {
                table = MTSSHelper.GenerateHTMLTableForMissingForms(kentico, E3obj);
            }

            if (table.Rows.Count <= 0)
            {
                Panel p1 = new Panel();
                p1.Controls.Add(new LiteralControl("No results found for selected criteria"));
                p1.Style.Add("position", "relative");
                p1.Style.Add("top", "50%");
                SortByFormsPlaceHolder.Controls.Add(p1);

            }
            else
            {

                SortByFormsPlaceHolder.Controls.Add(table);
            }

        }

        #endregion


    }

}
