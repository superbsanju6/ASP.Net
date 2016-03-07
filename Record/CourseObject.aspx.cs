using System;
using System.Collections.Generic;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Base.Enums;


namespace Thinkgate.Record
{
    public partial class CourseObject : RecordPage
    {
        #region Variables

        private Int32 _courseId;
        private LCO _lco;
        private EntityTypes _level;
        private int _userId;

        #endregion

        #region Properties

        protected override String TypeKey
        {
            get { return EntityTypes.CourseObject + "_"; }
        }

        #endregion

        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            _level = SessionObject.LCOrole;
            _userId = SessionObject.LoggedInUser.Page;
            GetLcOforCourse();
            LoadCourseObjectFolders();
            LoadCourseObjectTiles();
            InitPage(ctlFolders, ctlDoublePanel, sender, e);
            LoadDefaultFolderTiles();
            if (SessionObject.SelectedUser == null) return;
        }

        private void GetLcOforCourse()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                _courseId = GetDecryptedEntityId(X_ID);
                if (!RecordExistsInCache(Key))
                {
                    _lco = LCO.GetCourseObjectByCourseID(_courseId);
                }
                else
                {
                    _lco = (LCO)Base.Classes.Cache.Get(Key);
                }

                lcoName.Text = _lco.Grade + " " + _lco.CourseNumber + " " + _lco.Course;
            }
        }

        private void LoadCourseObjectFolders()
        {
            Folders = new List<Folder>();

            Folders.Add(new Folder("Course Object", "~/Images/new/folder_classes.png", LoadCourseObjectTiles,
                             "~/ContainerControls/TileContainer_3_1.ascx", 3,
                             "~/ContainerControls/TileContainer_3_1.ascx", 0, string.Empty,
                             string.Empty));

            ctlFolders.BindFolderList(Folders);
        }

        private void LoadCourseObjectTiles()
        {
            if (UserHasPermission(Permission.Tile_Identification_Course))
            {
                string identificationEditURL = "../Controls/LCO/AddCourse.aspx?senderPage=identification&grade="+_lco.Grade+"&subject="+_lco.ProgramArea+"&course="+_lco.Course+"&courseId="+_lco.CourseID;
                TileParms identificationCourse = new TileParms();
                identificationCourse.AddParm("LCO", _lco);
                Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_Identification_Course, "Identification", "~/Controls/LCO/CourseIdentification.ascx", false, identificationCourse, null, null, ((_level == EntityTypes.IMC) && (_lco.IMCID == _userId)&& (_lco.IsRegionRequested==false)) ? identificationEditURL : null,
                                            false, null, null, "openAssessmentObjectIdentificationEditRadWindow")); //(_lco.CreatedBy == SessionObject.LoggedInUser.UserName) ? "../Controls/LCO/AddCourse.aspx" : null));
            }


            if (UserHasPermission(Permission.Tile_Approvals))
            {
                TileParms ApprovalCourse = new TileParms();
                ApprovalCourse.AddParm("LCO", _lco);
                Rotator1Tiles.Add(new Tile(Base.Enums.Permission.Tile_Approvals, "Approvals", "~/Controls/LCO/Approvals.ascx", false, ApprovalCourse, null, null));
            }
            //LoadAssessmentTiles();
        }

        protected override object LoadRecord(int xId)
        {
            return LCO.GetCourseObjectByCourseID(xId);
        }
    }
}