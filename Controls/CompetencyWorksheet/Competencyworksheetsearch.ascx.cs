using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using CultureInfo = System.Globalization.CultureInfo;

namespace Thinkgate.Controls.CompetencyWorksheet
{
    public partial class Competencyworksheetsearch : TileControlBase
    {
        public SessionObject session;
        public string _resourceToShow;
        private List<Base.Classes.CompetencyWorkSheet> competencyws;
        private CourseList _curriculumCourseList;
        private Int32 _teacherID;
        private string subject, grade;
        private string course;
        public Int32 classid;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            session = (SessionObject)Session["SessionObject"];
            string titlekey = string.Empty;
            _teacherID = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(session.LoggedInUser.Page);

            LoadWorksheets();
            LoadWorkSheetData(competencyws);
            _resourceToShow = (string)Tile.TileParms.GetParm("resourceToShow");

            if (!Page.IsPostBack)
            {

                for (int i = wndWindowManager.Windows.Count - 1; i > -1; i--)
                {
                    if (!new List<string> { "wndAddDocument", "wndCmsNewDocumentShell" }.Contains(wndWindowManager.Windows[i].ID))
                    {
                        wndWindowManager.Windows.Remove(wndWindowManager.Windows[i]);
                    }
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (UserHasPermission(Permission.Icon_Addnew_Competencyworksheet))
            {
                {
                    btnAdd.Visible = true;
                    btnAdd.Attributes["onclick"] = string.Format("javascript:openCmsDialogWindows($find('{0}'),'{1}','{2}');", wndAddDocument.ClientID, btnOkNew.ClientID, "");

                }

            }
            else
            {

                btnAdd.Visible = false;

            }
        }


        protected void groupSearchButton_smallTile_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            var searhtextbox = worksheetSearchText_smallTile.Value.ToLower();
            if (searhtextbox != "")
            {
                // here we are finding worksheets which have either name,description,groupname or classname in the searchtextbox.
                LoadWorksheets();
                competencyws = competencyws.Where(x => x.Name.ToLower().Contains(searhtextbox) || x.Description.ToLower().Contains(searhtextbox)).ToList();
                LoadWorkSheetData(competencyws);
            }
            else
            {
                LoadWorksheets();
                LoadWorkSheetData(competencyws);
            }

        }

        private void LoadWorkSheetData(List<Base.Classes.CompetencyWorkSheet> competencyws)
        {
            if (competencyws != null && competencyws.Count > 0)
            {
                lbxList.DataSource = competencyws;
                lbxList.DataBind();
                lbxList.Visible = true;
                pnlNoResults.Visible = false;

            }
            else
            {
                pnlNoResults.Visible = true;
                lbxList.DataSource = null;
                lbxList.DataBind();
                lbxList.Visible = false;

            }
        }
        private void LoadWorksheets()
        {
            subject = session.clickedClass.Subject.DisplayText != "" ? session.clickedClass.Subject.DisplayText : string.Empty;
            course = session.clickedClass.Course.CourseName != "" ? session.clickedClass.Course.CourseName : string.Empty;
            grade = session.clickedClass.Grade.DisplayText != "" ? session.clickedClass.Grade.DisplayText : string.Empty;
            classid = session.clickedClass.ID;
            competencyws = CompetencyWorkSheet.GetCompetencyWorksheets(session.LoggedInUser.UserId, session.LoggedInUser.Page, session.clickedClass.ID, session.clickedClass.ClassCourseID, grade, subject, course);

        }

    }
}