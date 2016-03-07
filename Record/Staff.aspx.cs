using System;
using System.Collections.Generic;
using Standpoint.Core.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;

namespace Thinkgate.Record
{
	public partial class Staff : RecordPage
    {
        #region Variables

        public int StaffId;
		public Base.Classes.Staff SelectedStaff;
		private RadMenuItem _miDelete;

        #endregion

        #region Page Events

        protected new void Page_Init(object sender, EventArgs e)
		{
			if(!IsPostBack)
			{
				/********************************************************************
				 * Set up banner for Staff Page to display the following actions
				 * as menu options:
				 *          Delete  - Deletes the current Staff from the system.
				 * *****************************************************************/
				var siteMaster = Master as SiteMaster;
                if (siteMaster != null)

				{
                    siteMaster.BannerType = BannerType.ObjectScreen;

                    _miDelete = new RadMenuItem("Delete");
                    siteMaster.Banner.AddMenuItem(Banner.ContextMenu.Actions, _miDelete);
				    _miDelete.Visible = UserHasPermission(Permission.Menu_Actions_Delete);
                    siteMaster.Banner.AddOnClientItemClicked(Banner.ContextMenu.Actions, "actionsMenuItemClicked");
                }
                
                /* Stash the encrypted ID into a hidden element so the javascript 
                 * code for this page can access it on the browser side.
                 */
                StaffPageEncryptedID.Value = Request.QueryString["xID"];

			}
			SetupFolders();  
			InitPage(ctlFolders, ctlDoublePanel, sender, e);
			LoadStaff();
			if (SelectedStaff == null) return;
			LoadDefaultFolderTiles();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
            if (IsPostBack && Request["__EVENTTARGET"].Contains(doubleRotatorPanel.ClientID))
			{
				var eventArg = Request["__EVENTARGUMENT"];
				if (eventArg.Contains("~"))
				{
					var dockId = eventArg.Substring(0, eventArg.IndexOf("~"));
					SessionObject.TileClicked = dockId;
					SessionObject.LastElementsFolder_TileClicked = SessionObject.Elements_ActiveFolder;
					ReloadTilesControl("Profile");
				}
			}
		}

        protected void Page_LoadComplete()
        {
            var siteMaster = Master as SiteMaster;
            if (siteMaster != null)
            {
                if (_miDelete != null)
                {
                    if (!_miDelete.Visible || !UserHasPermission(Permission.Menu_MyProfile_Actions))
                        siteMaster.BannerControl.HideContextMenu(Banner.ContextMenu.Actions);
                }
            }
        }

		#endregion
		
		private void LoadStaff()
		{
            StaffId = String.IsNullOrEmpty(Request.QueryString["xID"]) ? 59494 : GetDecryptedEntityId(X_ID);
			var key = "Staff_" + StaffId;
			if (!RecordExistsInCache(key))
			{
				SelectedStaff = Base.Classes.Staff.GetStaffByID(StaffId);
				Base.Classes.Cache.Insert(key, SelectedStaff);
			}
			else
			{
				SelectedStaff = ((Base.Classes.Staff) Base.Classes.Cache.Get(key));
			}

			staffName.Text = String.Concat(SelectedStaff.FirstName, " ", SelectedStaff.MiddleName, " ", SelectedStaff.LastName);
				
			if (!String.IsNullOrEmpty(SelectedStaff.Picture))
				staffImage.ImageUrl = AppSettings.ProfileImageUserWebPath + '/' + SelectedStaff.Picture;

            /* Stash the staff guid into a hidden element so the javascript 
                * code for this page can access it on the browser side.
                */
            StaffGuid.Value = SelectedStaff.UserID.ToString();
		}

		#region Folder Methods

		private void SetupFolders()
		{
			Folders = new List<Folder>();
			Folders.Add(new Folder("Profile", "~/Images/new/folder_profile.png", LoadProfileTiles, "~/ContainerControls/TileContainerDockSix.ascx", 6));

			ctlFolders.BindFolderList(Folders);
		}

		#endregion

		#region Tile Methods

		private void LoadProfileTiles()
		{
			if (SelectedStaff == null) return;

			var staffTileParms = new TileParms();
			var staffIdEncrypted = Encryption.EncryptInt(StaffId);
			staffTileParms.AddParm("staff", SelectedStaff);

			Rotator1Tiles.Add(new Tile("Identification", 
                                        "~/Controls/Staff/StaffIdentification.ascx", 
                                        false,
										staffTileParms, 
                                        null, 
                                        null,
										SessionObject.LoggedInUser.UserId == SelectedStaff.UserID ?
                                            (UserHasPermission(Permission.Icon_Edit_Profile_Identification_Self) ? String.Concat("../Controls/Staff/StaffIdentification_Edit.aspx?xID=",staffIdEncrypted) : null) :
                                            (UserHasPermission(Permission.Icon_Edit_Profile_Identification) ? String.Concat("../Controls/Staff/StaffIdentification_Edit.aspx?xID=",staffIdEncrypted) : null), 
                                        false, 
                                        null, 
                                        null,
										"openStaffIdentificationEditRadWindow"));
            //TODO:DHB 2012-08-30 The IsNonClassroomTeacher_Evaluation and IsSchoolAdministrator properties are currently hardcoded to true.  However, it is anticipated that down the road, it will be used to determine when a non classroom instructor (NCI) is still to be considered a teacher.  See email from HCB on 8/30/2012.
            if (
                    (SessionObject.LoggedInUser.IsNonClassroomTeacher_Evaluation || SessionObject.LoggedInUser.IsSchoolAdministrator_Evaluation) &&
                    UserHasPermission(Permission.Tile_Evaluations_SchoolTeacherNCI_MyProfile) 
                    ||
                    (SessionObject.LoggedInUser.IsClassroomTeacher_Evaluation || SessionObject.LoggedInUser.IsSchoolAdministrator_Evaluation) &&
                    UserHasPermission(Permission.Tile_Evaluations_SchoolTeacherCI_MyProfile) 
                    ||
                    SessionObject.LoggedInUser.IsSchoolAdministrator_Evaluation && UserHasPermission(Permission.Tile_Evaluations_SchoolAdministrator)
                )
			{
				TileParms evalTileParms = new TileParms();

                if (SessionObject.LoggedInUser.IsSchoolAdministrator_Evaluation)
                        evalTileParms.AddParm("staffType", "SA");
                else if (SessionObject.LoggedInUser.IsClassroomTeacher_Evaluation)
                    evalTileParms.AddParm("staffType", "CI");
                else if (SessionObject.LoggedInUser.IsNonClassroomTeacher_Evaluation)
                    evalTileParms.AddParm("staffType", "NCI");        

				Rotator1Tiles.Add(new Tile("Personal Evaluations", "~/Controls/Staff/StaffEvaluation.ascx", false, evalTileParms));
			}
		}

		#endregion

        protected void deleteStaffXmlHttpPanel_ServiceRequest(object sender, RadXmlHttpPanelEventArgs e)
        {
            try
            {
                Base.Classes.Staff.DeleteStaff(e.Value);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
	}
}
