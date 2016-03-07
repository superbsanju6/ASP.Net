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
    public partial class PendingApprovals : TileControlBase
    {
        // Level will be one of IMC, RegionalCoordinator, SectionChief;
        private EntityTypes _level;
        private List<Thinkgate.Base.Classes.LCO> _lcos;
        private Int32 _userID;
        protected Boolean _isPostBack;
        protected String _LEAFilterKey = "LEAFilter";
        protected String _subjectFilterKey = "SubjectFilter";
        protected String _statusFilterKey = "StatusFilter";

        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            base.Page_Init(sender, e);
            if (Tile == null) return;

            _level = SessionObject.LCOrole;
            _lcos = (List<Thinkgate.Base.Classes.LCO>)Tile.TileParms.GetParm("lcos");
            _userID = SessionObject.LoggedInUser.Page;

            SetControlVisibility();

            AttatchLevelToKeys();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _level = SessionObject.LCOrole;
            _lcos = (List<Thinkgate.Base.Classes.LCO>)Tile.TileParms.GetParm("lcos");
            _userID = SessionObject.LoggedInUser.Page;

            // Simulate IsPostBack.
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            if (ViewState[_subjectFilterKey] == null)
            {
                ViewState.Add(_subjectFilterKey, "All");
                ViewState.Add(_LEAFilterKey, "All");
                ViewState.Add(_statusFilterKey, "All");
            }

            SetControlVisibility();

            if (!_isPostBack)
            {
                BuildSubjects();
                BuildStatus();
                BuildLEAs();
            }
            BuildPendingLCO();
        }

        private void AttatchLevelToKeys()
        {
            _subjectFilterKey += Tile.Title.Replace(" ", string.Empty);
            _statusFilterKey += Tile.Title.Replace(" ", string.Empty);
            _LEAFilterKey += Tile.Title.Replace(" ", string.Empty);
        }

        protected void BuildSubjects()
        {
            DataTable dtNew = new DataTable();
            dtNew.Columns.Add("DropdownText");
            dtNew.Columns.Add("Subject");
            dtNew.Columns.Add("CmbText");

            DataRow newRow = dtNew.NewRow();
            newRow["DropdownText"] = "All";
            newRow["Subject"] = "All";
            newRow["CmbText"] = "Subject";
            dtNew.Rows.Add(newRow);

            foreach (var sub in _lcos.Select(lco => lco.ProgramArea).Distinct().ToList())
            {
                newRow = dtNew.NewRow();
                newRow["DropdownText"] = sub;
                newRow["Subject"] = sub;
                newRow["CmbText"] = sub;
                dtNew.Rows.Add(newRow);
            }

            cmbSubject.DataTextField = "CmbText";
            cmbSubject.DataValueField = "Subject";
            cmbSubject.DataSource = dtNew;
            cmbSubject.DataBind();

            RadComboBoxItem item = this.cmbSubject.Items.FindItemByValue((String)this.ViewState[_subjectFilterKey], true) ?? this.cmbSubject.Items[0];
            ViewState[_subjectFilterKey] = item.Value;
            Int32 selIdx = cmbSubject.Items.IndexOf(item);
            cmbSubject.SelectedIndex = selIdx;
        }

        protected void BuildStatus()
        {
            DataTable dtNew = new DataTable();
            dtNew.Columns.Add("DropdownText");
            dtNew.Columns.Add("Status");
            dtNew.Columns.Add("CmbText");

            DataRow newRow = dtNew.NewRow();
            newRow["DropdownText"] = "All";
            newRow["Status"] = "All";
            newRow["CmbText"] = "Status";
            dtNew.Rows.Add(newRow);

            foreach (var sub in _lcos.Select(lco => lco.Status).Distinct().ToList())
            {
                newRow = dtNew.NewRow();
                newRow["DropdownText"] = sub;
                newRow["Status"] = sub;
                newRow["CmbText"] = sub;
                dtNew.Rows.Add(newRow);
            }

            cmbStatus.DataTextField = "CmbText";
            cmbStatus.DataValueField = "Status";
            cmbStatus.DataSource = dtNew;
            cmbStatus.DataBind();

            RadComboBoxItem item = this.cmbStatus.Items.FindItemByValue((String)this.ViewState[_statusFilterKey], true) ?? this.cmbStatus.Items[0];
            ViewState[_statusFilterKey] = item.Value;
            Int32 selIdx = cmbStatus.Items.IndexOf(item);
            cmbStatus.SelectedIndex = selIdx;
        }

        protected void BuildLEAs()
        {
            DataTable dtNew = new DataTable();
            dtNew.Columns.Add("DropdownText");
            dtNew.Columns.Add("LEA");
            dtNew.Columns.Add("CmbText");

            DataRow newRow = dtNew.NewRow();
            newRow["DropdownText"] = "All";
            newRow["LEA"] = "All";
            newRow["CmbText"] = "LEA";
            dtNew.Rows.Add(newRow);

            foreach (var lea in _lcos.Select(lco => lco.LEAName).Distinct().ToList())
            {
                newRow = dtNew.NewRow();
                newRow["DropdownText"] = lea;
                newRow["LEA"] = lea;
                newRow["CmbText"] = lea;
                dtNew.Rows.Add(newRow);
            }

            cmbLEA.DataTextField = "CmbText";
            cmbLEA.DataValueField = "LEA";
            cmbLEA.DataSource = dtNew;
            cmbLEA.DataBind();

            RadComboBoxItem item = this.cmbLEA.Items.FindItemByValue((String)this.ViewState[_LEAFilterKey], true) ?? this.cmbLEA.Items[0];
            ViewState[_LEAFilterKey] = item.Value;
            Int32 selIdx = cmbLEA.Items.IndexOf(item);
            cmbLEA.SelectedIndex = selIdx;
        }

        protected void cmb_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            switch (((RadComboBox)sender).ID.ToLower().ToString())
            {
                case "cmbsubject":
                    ViewState[_subjectFilterKey] = e.Value;
                    break;
                case "cmblea":
                    ViewState[_LEAFilterKey] = e.Value;
                    break;
                case "cmbstatus":
                    ViewState[_statusFilterKey] = e.Value;
                    break;
                default:
                    break;
            }
            BuildPendingLCO();
        }

        protected void BuildPendingLCO()
        {
            lbxList.Items.Clear();
            foreach (var course in _lcos)
            {
                if (course.ProgramArea == cmbSubject.SelectedItem.Text || cmbSubject.SelectedItem.Value.ToLower() == "all")
                {
                    if (course.Status == cmbStatus.SelectedItem.Text || cmbStatus.SelectedItem.Value.ToLower() == "all")
                    {
                        if (course.LEAName == cmbLEA.SelectedItem.Text || cmbLEA.SelectedItem.Value.ToLower() == "all")
                        {
                            
                                RadListBoxItem radItem = new RadListBoxItem();
                                lbxList.Items.Add(radItem);

                                Image imgProofed = (Image)radItem.FindControl("testImg");
                                imgProofed.ImageUrl = "~/Images/under_review.png";

                                HyperLink hlkList = (HyperLink)radItem.FindControl("lnkListCourseName");
                                hlkList.Text = course.Course;
                                hlkList.NavigateUrl = "~/Record/CourseObject.aspx?xID=" + Encryption.EncryptInt(course.CourseID);

                                if (_level == EntityTypes.RegionalCoordinator || _level == EntityTypes.SectionChief)
                                {
                                    Label lblLEA = (Label)radItem.FindControl("lblLEA");
                                    lblLEA.Text = "LEA: " + course.LEAName;
                                }
                                else
                                {
                                    ((Label)radItem.FindControl("lblLEA")).Visible = false;
                                }

                                Label lblRequestDate = (Label)radItem.FindControl("lblRequestDate");
                                if (!course.IsApproved && !course.IsSectionRequested && course.IsRegionRequested)
                                {
                                    lblRequestDate.Text = "Region Approval Requested: " + course.DateRCRequested;
                                }
                                else if (!course.IsApproved && course.IsSectionRequested && course.IsRegionRequested)
                                {
                                    lblRequestDate.Text = "State Approval Requested: " + course.DateSCRequested;
                                }
                           
                        }              
                    }
                }
            }

            divListNoResults.Visible = (lbxList.Items.Count < 1);
            lbxList.Visible = (lbxList.Items.Count > 0);
        }

        private void SetControlVisibility()
        {
            cmbLEA.Visible = UserHasPermission(Permission.Filter_LEA_ApprovedTile);
        }
    }
}