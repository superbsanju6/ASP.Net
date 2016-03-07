using System;
using System.Globalization;
using System.Web.UI;
using System.Collections.Generic;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Record
{
    public partial class Class : RecordPage
    {
        #region Variables

        private int _classId;
        private Base.Classes.Class _selectedClass;
        private RadMenuItem _miCopy, _miDelete;
        private bool _thisIsAClone;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.Class + "_"; }
        }

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                /********************************************************************
                 * Set up banner for Item Page to display the following actions
                 * as menu options:
                 *          Copy    - Copies current item into User's personal Bank
                 *          Delete  - Deletes the current Item from the system.
                 * *****************************************************************/
                var siteMaster = Master as SiteMaster;
                if (siteMaster != null)
                {
                    siteMaster.BannerType = BannerType.ObjectScreen;
                    _miCopy = new RadMenuItem("Copy");
                    _miCopy.Enabled = UserHasPermission(Permission.Menu_Actions_Class_Copy);
                    _miDelete = new RadMenuItem("Delete");
                    siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, _miCopy);
                    _miDelete.Enabled = UserHasPermission(Permission.Menu_Actions_Class_Delete);
                    siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, _miDelete);
                    _miDelete.Visible = UserHasPermission(Permission.Menu_Actions_Delete_Class);
                    _miCopy.Visible = UserHasPermission(Permission.Menu_Actions_Copy_Class);
                    siteMaster.Banner.AddOnClientItemClicked(Banner.ContextMenu.Actions, "actionsMenuItemClicked");
                }
                
            }
            SetupFolders();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadClass();
            if (_selectedClass == null) return;
            if (!IsPostBack)
            {
                SessionObject.clickedClass = null;
            }

           LoadDefaultFolderTiles();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                className.Text = _selectedClass.ClassName;
                 
                // Hide the single folder for now.
                FoldersControl.Visible = false;

                //load up hidden input box with encrypted item ID and item type.
                Class_hdnEncryptedData.Value = _classId.ToString(CultureInfo.CurrentCulture);

                /****************************************************************
                 * Find the RadScriptManager control and insert service into it.
                 * 
                 * The RadScriptManager is in the Site.Master masterpage. 
                 * Because that master page is used by so many pages, we don't 
                 * want to add services to Site.Master's script manager.  The
                 * pages that don't need the service don't need the extra overhead
                 * that the service causes.  So we must find the scriptManager and
                 * add the service to it from here.
                 * **************************************************************/
                var pageScriptMgr = Page.Master.FindControl("RadScriptManager1") as RadScriptManager;
                var newSvcRef = new ServiceReference("~/Services/ClassWCF.Svc");
                pageScriptMgr.Services.Add(newSvcRef);
            }
        }

        protected void Page_LoadComplete()
        {
            var siteMaster = Master as SiteMaster;
            if (siteMaster != null)
            {
                if (_miDelete != null && _miCopy != null)
                {
                    if (!_miDelete.Visible && !_miCopy.Visible)
                        siteMaster.BannerControl.HideContextMenu(Banner.ContextMenu.Actions);
                }
            }
        }

        #endregion

        private void LoadClass()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _classId = GetDecryptedEntityId(X_ID);
                                
                if (RecordExistsInCache(Key))
                {
                    _selectedClass = (Base.Classes.Class)Base.Classes.Cache.Get(Key);
                    className.Text = _selectedClass.ClassName;
                }

                if (IsCurrentClassAClone())
                {
                    _thisIsAClone = true;
                    SessionObject.GenericPassThruParm = null;
                }

            }
        }

        /// <summary>
        ///  In the Banner menu for the Class object page (this page) there is 
        ///  an option to make a "copy" the class.  Upon selecting this menu 
        /// option, a copy of the class is made and the user is taken to the 
        /// copy of that class instead of the original.  In that case an 
        /// additional notification is displayed on the Class object page to 
        /// indicate "This is a copy of an existing class."  In order to 
        /// display this, we therefore need to know whether we're displaying a 
        /// copy (clone) or not.  If we are displaying a copy then 
        /// SessionObject.GenericPassThruParm will have the string "CLONE:" 
        /// in it.  We must then clear out the GenericPassThruParm value in 
        /// case calls to the Class object page are not clones.
        /// </summary>
        /// <returns>True if the class is a clone; false otherwise</returns>
        private bool IsCurrentClassAClone()
        {
            return !String.IsNullOrEmpty(SessionObject.GenericPassThruParm) &&
                   SessionObject.GenericPassThruParm.StartsWith("CLONE:") &&
                   SessionObject.GenericPassThruParm.EndsWith(Request.QueryString["xID"]);
        }

        #region Folder Methods

        private void SetupFolders()
        {
            Folders = new List<Folder>
            {
                new Folder(
                    "Information",
                    "~/Images/new/folder_profile.png",
                    LoadInformationTiles,
                    "~/ContainerControls/TileContainerDockSix.ascx",
                    6)
            };

            ctlFolders.BindFolderList(Folders);
        }

        #endregion

        #region Tile Methods

        private void LoadInformationTiles()
        {
            var classTileParms = new TileParms();
            classTileParms.AddParm("class", _selectedClass);
            classTileParms.AddParm("level", EntityTypes.Class);
            classTileParms.AddParm("levelID", _selectedClass.ID);
            classTileParms.AddParm("folder", "Information");
            if (_thisIsAClone) classTileParms.AddParm("clonedClassID",_selectedClass.ID);

            var classTileParms2 = new TileParms();
            classTileParms2.AddParm("class", _selectedClass);
            classTileParms2.AddParm("level", EntityTypes.Class);
            classTileParms2.AddParm("levelID", _selectedClass.ID);
            classTileParms2.AddParm("folder", "Information");
            if (_thisIsAClone) classTileParms2.AddParm("clonedClassID", _selectedClass.ID);

            TileParms districtAssessmentParms = new TileParms(classTileParms.Parms);
            districtAssessmentParms.AddParm("category", "District");

            SessionObject.DistrictTileParms = districtAssessmentParms;

            TileParms classroomAssessmentParms = new TileParms(classTileParms2.Parms);
            classroomAssessmentParms.AddParm("category", "Classroom");

            var expandUrl = UserHasPermission(Permission.Icon_Expand_Class) ? "../Controls/Class/ClassSummary_Expanded.aspx?xID=" + EntityIdEncrypted : string.Empty;
            var editUrl = UserHasPermission(Permission.Edit_Class) ? "../Controls/Class/ClassSummary_Edit.aspx?xID=" + EntityIdEncrypted : string.Empty;

            // Page 1 upper row
            Rotator1Tiles.Add(new Tile(Permission.Tile_Identification_Class, "Identification", "~/Controls/Class/ClassIdentification.ascx", false, classTileParms, null, expandUrl, editUrl));
            Rotator1Tiles.Add(new Tile(Permission.Tile_Teachers_Class, "Teachers", "~/Controls/Teacher/Teachers.ascx", false, classTileParms, null, expandUrl, editUrl, true, "graphical"));
            Rotator1Tiles.Add(new Tile(Permission.Tile_Roster_Class, "Roster", "~/Controls/Class/ClassRoster.ascx", false, classTileParms, null, expandUrl, editUrl, true, "graphical"));
            // Page 1 lower row

            Rotator1Tiles.Add(new Tile("District Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, districtAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=District&level=Class", null));
            Rotator1Tiles.Add(new Tile("Classroom Assessments", "~/Controls/Assessment/ViewAssessmentsV2.ascx", false, classroomAssessmentParms, null, "../Controls/Assessment/AssessmentSearchExpanded.aspx?category=Classroom&level=Class", null));
            Rotator1Tiles.Add(new Tile("Assessment Results", "~/Controls/Assessment/AssessmentResults.ascx", false, classTileParms, null, "../Controls/Assessment/AssessmentResults_ExpandedV2.aspx", null, false, null, "expandAssessmentResults"));


            // Page 2 upper row
            //_rotator1Tiles.Add(new Tile("Resources", "~/Controls/Documents/Resources.ascx", false, classTileParms, null, "../Controls/Resources/ResourceSearch.aspx"));


            DistrictParms districtParms = DistrictParms.LoadDistrictParms();
            if (districtParms.Tile_Kentico_Resources == "Yes")
            {
                var resourceTileParms3 = new TileParms();
                resourceTileParms3.AddParm("resourceToShow", GetKenticoResourceTileResourceTypeName());
                resourceTileParms3.AddParm("title", "Resources");
                resourceTileParms3.AddParm("height", "500");
                resourceTileParms3.AddParm("width", "900");
                resourceTileParms3.AddParm("type", "Resources");
                Rotator1Tiles.Add(new Tile("Resources", "~/Controls/Documents/GenericCMSDocumentTile.ascx", false, resourceTileParms3, null, "../Controls/Resources/ResourceSearchKentico.aspx"));

            }
            if (UserHasPermission(Permission.Tile_Class_Attendance))
            {
                Rotator1Tiles.Add(new Tile("Attendance", "~/Controls/Class/ClassAttendance.ascx", false, classTileParms));
            }
            if (UserHasPermission(Permission.Tile_Discipline_Class))
            {
                Rotator1Tiles.Add(new Tile("Discipline", "~/Controls/Class/ClassDiscipline.ascx", false, classTileParms));
            }
            // Page 2 lower row
            if (UserHasPermission(Permission.Tile_Demographics_Class))
            {
                Rotator1Tiles.Add(new Tile("Demographics", "~/Controls/PlaceholderTile.ascx", false, classTileParms));
            }
            if (UserHasPermission(Permission.Tile_UDF_Class))
            {
                Rotator1Tiles.Add(new Tile("UDFs", "~/Controls/Class/ClassUserDefinedFields.ascx", false, classTileParms));
            }
            //_rotator1Tiles.Add(new Tile("Common Core State Standards", "~/Controls/Standards/StandardsSearch.ascx", false, classTileParms));

            Rotator1Tiles.Add(new Tile("Standards", "~/Controls/Standards/StandardsSearch.ascx", false, classTileParms, null, "../Controls/Standards/StandardsSearch_ExpandedV2.aspx"));

            //            _rotator1Tiles.Add(new Tile("Grades", "~/Controls/Class/ClassGrades.ascx", false, classTileParms));
            //            _rotator1Tiles.Add(new Tile("Assessments", "~/Controls/Class/ClassAssessments.ascx", false, classTileParms));
            //           _rotator1Tiles.Add(new Tile("Results", "~/Controls/Class/ClassResults.ascx", false, classTileParms));
            //            _rotator1Tiles.Add(new Tile("Early Warning", "~/Controls/Class/ClassEarlyWarning.ascx", false, classTileParms));
            //            _rotator1Tiles.Add(new Tile("Readiness", "~/Controls/Class/ClassReadiness.ascx", false, classTileParms));
        }

        #endregion

        protected override object LoadRecord(int xId)
        {
            return Base.Classes.Class.GetClassByID(xId);
        }
    }
}
