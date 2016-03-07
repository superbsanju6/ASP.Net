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
    public partial class CourseCatalog : TileControlBase
    {
        // Level will be one of IMC, RegionalCoordinator, SectionChief;
      
        private List<Thinkgate.Base.Classes.LCO> _alllcos;
        protected Boolean _isPostBack;
        protected String _LEAFilterKey = "LEAFilter";
        protected String _subjectFilterKey = "SubjectFilter";
        protected String _gradeFilterKey = "GradeFilter";
       
        protected new void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            base.Page_Init(sender, e);
            if (Tile == null) return;
            _alllcos = (List<Thinkgate.Base.Classes.LCO>)Tile.TileParms.GetParm("alllcos");
         
            AttatchLevelToKeys();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _alllcos = (List<Thinkgate.Base.Classes.LCO>)Tile.TileParms.GetParm("alllcos");
          
            // Simulate IsPostBack.
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            if (ViewState[_gradeFilterKey] == null)
            {
                ViewState.Add(_subjectFilterKey, "All");
                ViewState.Add(_LEAFilterKey, "All");
                ViewState.Add(_gradeFilterKey, "All");
            }

          
            if (!_isPostBack)
            {
                BuildGrade();
                BuildSubjects();
                BuildLEAs();
            }
            BuildPendingLCO();
        }

        private void AttatchLevelToKeys()
        {
            _subjectFilterKey += Tile.Title.Replace(" ", string.Empty);
            _gradeFilterKey += Tile.Title.Replace(" ", string.Empty);
            _LEAFilterKey += Tile.Title.Replace(" ", string.Empty);
        }
        protected void BuildGrade()
        {
            DataTable dtNew = new DataTable();
            dtNew.Columns.Add("DropdownText");
            dtNew.Columns.Add("Grade");
            dtNew.Columns.Add("CmbText");

            DataRow newRow = dtNew.NewRow();
            newRow["DropdownText"] = "All";
            newRow["Grade"] = "All";
            newRow["CmbText"] = "Grade";
            dtNew.Rows.Add(newRow);

            foreach (var sub in _alllcos.Select(lco => lco.Grade).Distinct().ToList())
            {
                newRow = dtNew.NewRow();
                newRow["DropdownText"] = sub;
                newRow["Grade"] = sub;
                newRow["CmbText"] = sub;
                dtNew.Rows.Add(newRow);
            }

            cmbGrade.DataTextField = "CmbText";
            cmbGrade.DataValueField = "Grade";
            cmbGrade.DataSource = dtNew;
            cmbGrade.DataBind();

            RadComboBoxItem item = this.cmbGrade.Items.FindItemByValue((String)this.ViewState[_gradeFilterKey], true) ?? this.cmbGrade.Items[0];
            ViewState[_gradeFilterKey] = item.Value;
            Int32 selIdx = cmbGrade.Items.IndexOf(item);
            cmbGrade.SelectedIndex = selIdx;
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

            foreach (var sub in _alllcos.Select(lco => lco.ProgramArea).Distinct().ToList())
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

            foreach (var lea in _alllcos.Select(lco => lco.LEAName).Distinct().ToList())
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
                case "cmbgrade":
                    ViewState[_gradeFilterKey] = e.Value;
                    break;
                case "cmbsubject":
                    ViewState[_subjectFilterKey] = e.Value;
                    break;
                case "cmblea":
                    ViewState[_LEAFilterKey] = e.Value;
                    break;
               default:
                    break;
            }
            BuildPendingLCO();
        }

        protected void BuildPendingLCO()
        {
            lbxList.Items.Clear();
            foreach (var course in _alllcos)
            {
                 if (course.Grade == cmbGrade.SelectedItem.Text || cmbGrade.SelectedItem.Value.ToLower() == "all")
                {
                    if (course.ProgramArea == cmbSubject.SelectedItem.Text || cmbSubject.SelectedItem.Value.ToLower() == "all")
                    {
                        if (course.LEAName == cmbLEA.SelectedItem.Text || cmbLEA.SelectedItem.Value.ToLower() == "all")
                        {
                            RadListBoxItem radItem = new RadListBoxItem();
                            lbxList.Items.Add(radItem);

                            HyperLink hlkList = (HyperLink)radItem.FindControl("lnkListCourseName");
                            hlkList.Text = (course.CourseNumber != "0" && course.CourseNumber != "") ? course.CourseNumber + " - " + course.Course : course.Course;
                            hlkList.NavigateUrl = "~/Record/CourseObject.aspx?xID=" + Encryption.EncryptInt(course.CourseID);
                        }

                    }
                }
            }

            divListNoResults.Visible = (lbxList.Items.Count < 1);
            lbxList.Visible = (lbxList.Items.Count > 0);
        }

       
    }
}