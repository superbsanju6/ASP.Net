using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Standpoint.Core.Utilities;
using Standpoint.Core.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using System.Linq;

namespace Thinkgate.Controls.LCO
{
    public partial class Approvals : TileControlBase
    {
        protected Thinkgate.Base.Classes.LCO _lco;
        private Int32 _userID;
        private EntityTypes _level;
        private List<RadListBoxItem> _rlApproved = new List<RadListBoxItem>();
        private List<RadListBoxItem> _rlPending = new List<RadListBoxItem>();

        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            base.Page_Init(sender, e);
            _userID = SessionObject.LoggedInUser.Page;

            if (Tile == null) return;

            _lco = ((Thinkgate.Base.Classes.LCO)Tile.TileParms.GetParm("LCO"));
            _level = SessionObject.LCOrole;

            SetControlVisibility();
            if (!IsPostBack)
            {
                BuildLists();
            }
        }

        //protected void LCOApprovals_RadTabStrip_TabClick(object sender, RadTabStripEventArgs e)
        //{
        //    switch (((RadTabStrip)sender).SelectedTab.Text)
        //    {
        //        case "Approved":
        //            RadListBoxApprovalsApproved.Visible = true;
        //            RadListBoxApprovalsPending.Visible = false;
        //            break;
        //        case "Pending":
        //            RadListBoxApprovalsApproved.Visible = false;
        //            RadListBoxApprovalsPending.Visible = true;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        private void BuildLists()
        {
            foreach (DataRow lea in Base.Classes.LCO.LoadLEAsByCourseID(_lco.CourseID).Rows)
            {
                RadListBoxItem ri = new RadListBoxItem();
                if (DataIntegrity.ConvertToBool(lea["IsApproved"]))
                {
                    RadListBoxApprovalsApproved.Items.Add(ri);
                  
                }
                else
                {
                    RadListBoxApprovalsPending.Items.Add(ri);
                }

                HyperLink lnk = (HyperLink)ri.FindControl("lnkLEA");
                lnk.Text = lea["LEAName"].ToString() + " - " + lea["IMCName"].ToString();

                var link = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") +
                    System.Web.HttpUtility.UrlEncode("display.asp?fo=basic%20display&rm=page&key=7266&xID=" + DataIntegrity.ConvertToInt(lea["DocID"]) + "&??hideButtons=Save And Return,Delete,copydocument,cancel&??appName=E3");

                if (UserHasPermission(Permission.Hyperlink_LEA_Approvals))
                {
                    lnk.NavigateUrl = link;
                }
                else
                {
                    lnk.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        private void SetControlVisibility()
        {
           divAdd.Visible = (UserHasPermission(Permission.Create_Approval) && (_level == EntityTypes.IMC) && (_lco.IMCID != _userID) && !Base.Classes.LCO.UserHasApprovalForCourse(_userID, _lco.CourseID));
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            modConfirm.Show();
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            var DocID = Base.Classes.LCO.AddCourse(_lco.Grade, _lco.ProgramArea, _lco.Course, _lco.ImplementationYear, _userID, _lco.ExpirationYear);
            divAdd.Visible = false;
            var link = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") +
                System.Web.HttpUtility.UrlEncode("display.asp?fo=basic%20display&rm=page&key=7266&xID=" + DocID[1] + "&??hideButtons=Save And Return,Delete,copydocument,cancel&??appName=E3");
            Response.Redirect(link);
        }
    }
}