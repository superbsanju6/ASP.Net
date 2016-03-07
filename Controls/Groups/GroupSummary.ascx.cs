using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Services.Contracts.Groups;

namespace Thinkgate.Controls.Groups
{
    public partial class GroupSummary : TileControlBase
    {
        #region Variables

        readonly GroupsProxy _groupProxy = new GroupsProxy();
        private List<GroupStudentDataContract> _groupsWithStudents;
        private int groupID=0;

        #endregion
        Thinkgate.Services.Contracts.Groups.GroupDataContract g;

        #region Asynchronous Methods
       

        private void LoadRosterTable()
        {
            if (_groupsWithStudents == null)
            {
                _groupsWithStudents = _groupProxy.GetStudentsInGroup(groupID, DistrictParms.LoadDistrictParms().ClientID);
            }          

            rosterGrid.DataSource = _groupsWithStudents.OrderBy(x => x.Name);
            rosterGrid_GraphicView.DataSource = _groupsWithStudents.OrderBy(x => x.Name);
            
        }
        #endregion

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            if (!UserHasPermission(Base.Enums.Permission.Tile_ClassSummary))
            {
                Tile.ParentContainer.Visible = false;
            }          

            g = (Thinkgate.Services.Contracts.Groups.GroupDataContract)Tile.TileParms.GetParm("group");

            groupID=g.ID;

            List<AsyncPageTask> taskList = new List<AsyncPageTask>();           
            taskList.Add(new AsyncPageTask(LoadRosterTable));

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "GroupSummary", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();        


            rosterGrid.DataBind();
            rosterGrid_GraphicView.DataBind();
           
        }

      
        protected void SetRTIImage(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GroupStudentDataContract stud = (GroupStudentDataContract)e.Item.DataItem;
                GridDataItem item = (GridDataItem)e.Item;
                Image summaryIcon = (Image)item.FindControl("listViewSummaryIcon");
                Image rtiImage = (Image)item["RTIImage"].Controls[0];
                var link = (HyperLink)item.FindControl("studentNameLinkListView");
                var label = (Label)item.FindControl("studentNameLabelListView");

                if (summaryIcon != null && !UserHasPermission(Base.Enums.Permission.Icon_Summary_Student))
                {
                    summaryIcon.Attributes["style"] = "display: none;";
                }
               
                if (link != null && label != null)
                {
                    link.Text = stud.Name;
                    label.Text = stud.Name;
                    if (UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
                    {
                        link.NavigateUrl = ResolveUrl("~/Record/Student.aspx?childPage=yes&xID=" + stud.ID);
                        link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                        link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";                        
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
                GroupStudentDataContract stud = (GroupStudentDataContract)e.Item.DataItem;
                GridDataItem item = (GridDataItem)e.Item;
                Image summaryIcon = (Image)item.FindControl("graphicalViewSummaryIcon");
                Image rtiImage = (Image)item.FindControl("GraphicViewRTIImage");
                var studentImage = (HyperLink)item.FindControl("StudentPhoto");
                var link = (HyperLink)item.FindControl("studentNameLinkGraphicView");
                var label = (Label)item.FindControl("studentNameLabelGraphicView");                

                //Hide summary icon based on parm
                if (summaryIcon != null && !UserHasPermission(Base.Enums.Permission.Icon_Summary_Student))
                {
                    summaryIcon.Attributes["style"] = "display: none;";
                }
               
                //Image hyperlink
                if (UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
                {
                    studentImage.NavigateUrl = ResolveUrl("~/Record/Student.aspx?childPage=yes&xID=" + stud.ID);
                }

                if (link != null && label != null)
                {
                    link.Text = stud.Name;
                    label.Text = stud.Name;
                    if (UserHasPermission(Base.Enums.Permission.Hyperlink_StudentName))
                    {
                        link.NavigateUrl = ResolveUrl("~/Record/Student.aspx?childPage=yes&xID=" + stud.ID);
                        link.Attributes["onmouseover"] = "this.style.textDecoration='none';";
                        link.Attributes["onmouseout"] = "this.style.textDecoration='underline';";                        
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
       

        protected static string GetContainerDivID(string controlID)
        {
            string searchText = "tileContainerDiv";
            string containerDivID = controlID.Substring(0, controlID.IndexOf(searchText) + searchText.Length);
            string remainingText = controlID.Replace(containerDivID, "");

            containerDivID = containerDivID + remainingText.Substring(0, remainingText.IndexOf("_"));

            return containerDivID;
        }        
    }  
}