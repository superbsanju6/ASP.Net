using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Student
{
    public partial class StudentDemographics_Edit : BasePage
    {
        private Base.Classes.Student _selectedStudent;
        private List<Base.Classes.Demographic> _lstDemographics;
        private const string demoName = "demoName";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadStudent();
                LoadDemographics();
                PopulateTable();
            }

            var pageScriptMgr = Page.Master.FindControl("RadScriptManager2") as RadScriptManager;
            var newSvcRef = new ServiceReference("~/Services/Service2.Svc");
            pageScriptMgr.Services.Add(newSvcRef);
        }

        private void LoadStudent()
        {
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                int studentID = GetDecryptedEntityId(X_ID);
                _selectedStudent = Base.Classes.Data.StudentDB.GetStudentByID(studentID);
                hidStudentID.Value = studentID.ToString();
            }
        }

        private void LoadDemographics()
        {
            _lstDemographics = Base.Classes.Demographic.GetListOfDemographics();
        }

        private void PopulateTable()
        {
            List<RadComboBox> lstComboxForDemographics = GetDemographicComboBoxes();
            SetDefaultValuesForStudent(lstComboxForDemographics);

            foreach(RadComboBox cmb in lstComboxForDemographics)
            {
                cmb.Style.Add(HtmlTextWriterStyle.Margin, "3px");

                Label lbl = new Label();
                lbl.Style.Add(HtmlTextWriterStyle.Margin, "3px");
                lbl.Text = cmb.Attributes[demoName].ToString() + ":";

                TableCell cellLabel = new TableCell();
                cellLabel.Style.Add(HtmlTextWriterStyle.TextAlign, "right");
                cellLabel.Controls.Add(lbl);

                TableCell cellCombo = new TableCell();
                cellCombo.Controls.Add(cmb);

                TableRow newRow = new TableRow();
                newRow.Cells.Add(cellLabel);
                newRow.Cells.Add(cellCombo);

                tblDemographics.Rows.Add(newRow);
            }
        }

        private List<RadComboBox> GetDemographicComboBoxes()
        {
            List<RadComboBox> lstComboBoxes = new List<RadComboBox>();
            foreach (Demographic demo in _lstDemographics)
            {
                if (demo.Label.Trim().ToLower() != "cumulative")
                {
                    RadComboBox cmbDemo = lstComboBoxes.Find(x => x.ID.Contains(demo.DemoField.ToString()));
                    if (cmbDemo == null)
                    {
                        cmbDemo = new RadComboBox();
                        cmbDemo.ID = "cmbDemoField" + demo.DemoField;
                        cmbDemo.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cmbDemo.CssClass = "comboDemographics";
                        cmbDemo.Attributes.Add(demoName, demo.Label);
                        lstComboBoxes.Add(cmbDemo);
                    }

                    if (demo.Label.Trim().ToLower() == "gender")
                    {
                        cmbDemo.Items.Add(new RadComboBoxItem(demo.Abbreviation, demo.Value));
                    }
                    else
                    {
                        cmbDemo.Items.Add(new RadComboBoxItem(demo.Value, demo.Value));
                    }

                    if (demo.Value.Trim().ToLower() == "yes")
                    {
                        cmbDemo.Items.Add(new RadComboBoxItem("No", "No"));
                    }
                }
            }
            return lstComboBoxes;
        }

        private void SetDefaultValuesForStudent(List<RadComboBox> lstComboxForDemographics)
        {
            PropertyInfo[] studentProperties = _selectedStudent.GetType().GetProperties();
                
            foreach (RadComboBox cmb in lstComboxForDemographics)
            { 
                foreach(PropertyInfo property in studentProperties)
                {
                    if (cmb.ID.Trim().ToLower() == "cmb" + property.Name.Trim().ToLower())
                    {
                        if (string.IsNullOrEmpty(property.GetValue(_selectedStudent, null).ToString()))
                        {
                            if (cmb.SelectedValue.Trim().ToLower() == "yes")
                            {
                                cmb.SelectedValue = "No";
                            }
                        }
                        else
                        {
                            cmb.SelectedValue = property.GetValue(_selectedStudent, null).ToString();
                        }
                    }
                }
            }
        }
    }
}