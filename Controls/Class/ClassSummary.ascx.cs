using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using System.Linq;
using Thinkgate.Base.Classes.Data;
using System.IO;

namespace Thinkgate.Controls.Class
{
    public partial class ClassSummary : TileControlBase
    {
        Thinkgate.Base.Classes.Class c; 

        #region Asynchronous Methods
        private void LoadTeacherTable()
        {
            //DataTable teachersTbl = new DataTable();
            List<Base.Classes.Teacher> lstTeachers = c.Teachers;
           
            foreach (Base.Classes.Teacher t in lstTeachers)
            {
                t.ID_Encrypted = t.PersonID.ToString();
            }
            teachersGrid.DataSource = lstTeachers;
            teachersGrid_GraphicalView.DataSource = lstTeachers;
        }

        private void LoadRosterTable()
        {
            List<Base.Classes.Student> lstStudents = c.Students;

            foreach (Base.Classes.Student s in lstStudents)
            {
                s.IDEncrypted = s.ID.ToString();
            }

            //rosterGrid.DataSource = lstStudents;
            //rosterGrid_GraphicView.DataSource = lstStudents;
            rosterGrid.DataSource = lstStudents.OrderBy(x => x.StudentName);
            rosterGrid_GraphicView.DataSource = lstStudents.OrderBy(x => x.StudentName);
            //load list view
        }          
        #endregion

        //Parms Dictionary includes: class
        protected new void Page_Init(object sender, EventArgs e)
        { 
            base.Page_Init(sender, e);
        }

        public override List<DockCommand> AdditionalCommands()
        {
            var additionalCommands = new List<DockCommand>();
            if (UserHasPermission(Permission.Icon_FCATRoster_ClassSummary))
                additionalCommands.Add(Container.GetDockCommand("Test Results", "stateResultsCommand", ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + System.Web.HttpUtility.UrlEncode("fastsql_v2_direct.asp?id=7140|classroster&??pclass=" + ((Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class")).ID + "&??Categories=district,state&report_year=no&start_tab_by_name=District Results&isE3=yes")));
            return additionalCommands;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            if(!UserHasPermission(Base.Enums.Permission.Tile_ClassSummary))
            {
                Tile.ParentContainer.Visible = false;
            }

            /*******************************************************
             * Determine whether we have permission to display tabs
             * ****************************************************/
            rosterRadTab.Visible = UserHasPermission(Base.Enums.Permission.Tab_Roster_ClassSummary);
            teachersRadTab.Visible = UserHasPermission(Base.Enums.Permission.Tab_Teachers_ClassSummary);
            identificationRadTab.Visible = UserHasPermission(Base.Enums.Permission.Tab_Identification_ClassSummary);

            c = (Thinkgate.Base.Classes.Class)Tile.TileParms.GetParm("class");

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();
            taskList.Add(new AsyncPageTask(LoadTeacherTable));
            taskList.Add(new AsyncPageTask(LoadRosterTable));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "ClassSummary", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            //Set tab attributes for toggle view
            classSummaryRadTabStrip.Attributes["toggleViewIndexList"] = "0,1";
            classSummaryRadTabStrip.Attributes["containerDivID"] = GetContainerDivID(this.ClientID);

            //load graphic view
            teachersGrid.DataBind();
            teachersGrid_GraphicalView.DataBind();

            rosterGrid.DataBind();
            rosterGrid_GraphicView.DataBind();

            PopulateProfileLabels(c);
        }

        protected void TeachersGrid_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            //Control image = e.Item.FindControl("");
            //DataRowView currentRow = (DataRowView)e.Item.DataItem;
            ////Add the image id to the tooltip manager
            //this.teacherToolTipManager.TargetControls.Add(image.ClientID, currentRow.Row["ID"].ToString(), true);
        }

        private void PopulateProfileLabels(Thinkgate.Base.Classes.Class c)
        {
            lblSchoolName.Text = c.School.Name;
            lblPrimaryTeacher.Text = c.PrimaryTeacher;
            lblGrade.Text = c.Grade.DisplayText;
            lblSubject.Text = c.Subject.DisplayText;
            lblCourse.Text = c.Course.CourseName;
            lblSection.Text = c.Section;
            lblPeriod.Text = c.Period;
            lblSemester.Text = c.Semester;
            lblYear.Text = c.Year;
            lblBlock.Text = c.Block;
            lblRetainOnResync.Text = c.RetainOnResync ? "Yes" : "No";
        }

        protected void expandTile_Click(object sender, EventArgs e)
        {
            LinkButton link = (LinkButton)sender;

            if (Tile.ParentContainer != null)
                Tile.ParentContainer.ExpandTile(Tile, "Expand");
        }

        protected void SummaryPopup_Click(object sender, CommandEventArgs e)
        {

        }

        protected void SetRTIImage(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                Base.Classes.Student stud = (Base.Classes.Student)e.Item.DataItem;
                GridDataItem item = (GridDataItem)e.Item;
                Image summaryIcon = (Image) item.FindControl("listViewSummaryIcon");
                Image rtiImage = (Image)item["RTIImage"].Controls[0];
                var link = (HyperLink)item.FindControl("studentNameLinkListView");
                var label = (Label)item.FindControl("studentNameLabelListView");

                string rtiImageURL = (stud.Plans == null ? "" : stud.Plans);

                if(summaryIcon != null && !UserHasPermission(Base.Enums.Permission.Icon_Summary_Student))
                {
                    summaryIcon.Attributes["style"] = "display: none;";
                }

                if (rtiImage != null && rtiImageURL.Length > 0 && rtiImageURL.IndexOf("blank") == -1)
                {
                    rtiImage.ImageUrl = ResolveUrl("~/Images/" + rtiImageURL);
                }

                if (link != null && label != null)
                {
                    link.Text = stud.StudentName;
                    label.Text = stud.StudentName;
                    if (UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
                    {
                        link.NavigateUrl = ResolveUrl("~/Record/Student.aspx?childPage=yes&xID=" + stud.IDEncrypted);
                        link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                        link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";
                        //link.Attributes["style"] = "color:#00F; text-decoration:underline; cursor:pointer;";
                        link.Visible = true;
                        label.Visible = false;
                    }
                    else
                    {
                        link.Visible = false;
                        label.Visible = true;
                    }
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void SetRTIImage_GraphicView(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                Base.Classes.Student stud = (Base.Classes.Student)e.Item.DataItem;
                GridDataItem item = (GridDataItem)e.Item;
                Image summaryIcon = (Image)item.FindControl("graphicalViewSummaryIcon");
                Image rtiImage = (Image)item.FindControl("GraphicViewRTIImage");
                var studentImage = (HyperLink)item.FindControl("StudentPhoto");
                var link = (HyperLink)item.FindControl("studentNameLinkGraphicView");
                var label = (Label)item.FindControl("studentNameLabelGraphicView");

                string rtiImageURL = (String.IsNullOrEmpty(stud.Plans.ToString()) ? "" : stud.Plans);


                //Default if it's null in the DB, or missing from the web server path
                if (String.IsNullOrEmpty(stud.IconImage) || !File.Exists(Server.MapPath(AppSettings.ProfileImageStudentWebPath + "/" + stud.IconImage)))
                {
                    studentImage.ImageUrl = ResolveUrl("~/Images/new/male_student.png");
                }
                else studentImage.ImageUrl = AppSettings.ProfileImageStudentWebPath + '/' + stud.IconImage;

                //Hide summary icon based on parm
                if (summaryIcon != null && !UserHasPermission(Base.Enums.Permission.Icon_Summary_Student))
                {
                    summaryIcon.Attributes["style"] = "display: none;";
                }

                //Set RTI image
                if (rtiImage != null && rtiImageURL.Length > 0 && rtiImageURL.IndexOf("blank") == -1)
                {
                    rtiImage.ImageUrl = ResolveUrl("~/Images/" + rtiImageURL);
                }

                //Image hyperlink
                if(UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
                {
                    studentImage.NavigateUrl = ResolveUrl("~/Record/Student.aspx?childPage=yes&xID=" + stud.IDEncrypted);
                }
                
                if (link != null && label != null)
                {
                    link.Text = stud.StudentName;
                    label.Text = stud.StudentName;
                    if (UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
                    {
                        link.NavigateUrl = ResolveUrl("~/Record/Student.aspx?childPage=yes&xID=" + stud.IDEncrypted);
                        link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                        link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";
                        //link.Attributes["style"] = "color:#00F; text-decoration:underline; cursor:pointer;";
                        link.Visible = true;
                        label.Visible = false;
                    }
                    else
                    {
                        link.Visible = false;
                        label.Visible = true;
                        link.Attributes["style"] = "color:#3B3B3B; text-decoration:none;";
                    }
                }

            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void SetTeacherIcons(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                Base.Classes.Teacher teacher = (Base.Classes.Teacher)e.Item.DataItem;
                GridDataItem item = (GridDataItem)e.Item;
                Image summaryIcon = (Image) item.FindControl("listViewSummaryIconTeacher");
                Image primaryTeacherIcon = (Image)item.FindControl("PrimaryTeacherIcon");
                Image DegreeIcon = (Image)item["DegreeIcon"].Controls[0];
                Image CertificationIcon = (Image)item["CertificationIcon"].Controls[0];

                bool showPrimaryTeacherIcon = teacher.IsPrimary;
                bool showDegreeIcon = teacher.IsPrimary;
                bool showCertificationIcon = teacher.IsPrimary;

                if (summaryIcon != null && !UserHasPermission(Base.Enums.Permission.Icon_Summary_Teacher))
                {
                    summaryIcon.Attributes["style"] = "display: none;";
                }

                if (primaryTeacherIcon != null && showPrimaryTeacherIcon)
                {
                    primaryTeacherIcon.ImageUrl = "~/Images/star_icon.png";
                    primaryTeacherIcon.ToolTip = "Primary teacher";
                }
                if (DegreeIcon != null && showDegreeIcon)
                {
                    DegreeIcon.ImageUrl = "~/Images/diploma_icon.jpg";
                }
                if (CertificationIcon != null && showCertificationIcon)
                {
                    CertificationIcon.ImageUrl = "~/Images/certificate_icon_small.jpg";
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void SetTeacherIcons_GraphicView(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                Base.Classes.Teacher teacher = (Base.Classes.Teacher)e.Item.DataItem;
                GridDataItem item = (GridDataItem)e.Item;
                Image summaryIcon = (Image) item.FindControl("graphicalViewSummaryIconTeacher");
                Image primaryTeacherIcon = (Image)item.FindControl("GraphicViewPrimaryTeacherIcon");
                Image DegreeIcon = (Image)item.FindControl("GraphicViewDegreeIcon");
                Image CertificationIcon = (Image)item.FindControl("GraphicViewCertificationIcon");

                bool showPrimaryTeacherIcon = teacher.IsPrimary;
                bool showDegreeIcon = teacher.IsPrimary;
                bool showCertificationIcon = teacher.IsPrimary;

                if (summaryIcon != null && !UserHasPermission(Base.Enums.Permission.Icon_Summary_Teacher))
                {
                    summaryIcon.Attributes["style"] = "display: none;";
                }

                if (primaryTeacherIcon != null && showPrimaryTeacherIcon)
                {
                    primaryTeacherIcon.ImageUrl = "~/Images/star_icon.png";
                    primaryTeacherIcon.ToolTip = "Primary teacher";
                }
                if (DegreeIcon != null && showDegreeIcon)
                {
                    DegreeIcon.ImageUrl = "~/Images/diploma_icon.jpg";
                }
                if (CertificationIcon != null && showCertificationIcon)
                {
                    CertificationIcon.ImageUrl = "~/Images/certificate_icon_small.jpg";
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected static string GetContainerDivID(string controlID)
        {
            string searchText = "tileContainerDiv";
            string containerDivID = controlID.Substring(0, controlID.IndexOf(searchText)+searchText.Length);
            string remainingText = controlID.Replace(containerDivID, "");

            containerDivID = containerDivID + remainingText.Substring(0, remainingText.IndexOf("_"));

            return containerDivID;
        }

        #region TOOLTIP methods

        protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
        {
            this.UpdateToolTip(args.Value, args.UpdatePanel);
        }

        private void UpdateToolTip(string elementID, UpdatePanel panel)
        {
            Control ctrl = Page.LoadControl("ProductDetails.ascx");
            panel.ContentTemplateContainer.Controls.Add(ctrl);
        }

        #endregion
    }
}
