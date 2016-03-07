using System;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Classes;
using System.Data;

namespace Thinkgate.Controls.LCO
{
    public partial class AddCourse : BasePage
    {
        private const int UserPage = 110;
        private Int32 _courseId;

        private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            _courseId = Convert.ToInt32(Request.QueryString["courseId"]);
            if (Request.QueryString["senderPage"] == "identification")
            {
               
                Page.Title = "Edit Course";
            }
            
           
            if (!IsPostBack)
            {

                if (Request.QueryString["senderPage"] == "identification")
                {
                    _courseId = Convert.ToInt32(Request.QueryString["courseId"]);
                    Page.Title = "Edit Course";
                    BuildSubjects();
                    cmbProgramArea.SelectedValue = Request.QueryString["subject"];
                    cmbGrade.SelectedValue = Request.QueryString["grade"];
                    cmbImplementationYear.SelectedValue = Request.QueryString["year"];
                    txtCourse.Text = Request.QueryString["course"];
                    txtGrade.Text = Request.QueryString["grade"];
                    LabelGradeErrorMessage.Visible = false;
                    LabelImplementationYearErrorMessage.Visible = false;
                    txtGrade.Visible = true;
                    cmbGrade.Visible = false;
                    lblImplementationYear.Visible = false;
                    lblImplementationSemester.Visible = false;
                    cmbImplementationYear.Visible = false;
                    cmbSemester.Visible = false;
                    RadButtonOk.Visible = false;
                    RadButtonEditOk.Visible = true;
                    
                }
                else
                {
                    BuildSubjects();
                    BuildSemesters();
                    BindPageControls();
                    BINDYEAR();
                }
            }

            if (Request.Form["__EVENTTARGET"] == "RadButtonOk")
            {         
                ButtonOkClick(this, new EventArgs());
            }
            if (Request.Form["__EVENTTARGET"] == "RadButtonEditOk")
            {
                ButtonEditOkClick(this, new EventArgs());
            }
        }

        protected void ButtonOkClick(object sender, EventArgs e)
        {

            var programArea = cmbProgramArea.SelectedItem.Text;
            var course = txtCourse.Text.Trim();

          
                var grade = cmbGrade.SelectedItem.Text;
                var implementYear = GetYearAsString(cmbImplementationYear.SelectedItem.Text, cmbSemester.SelectedItem.Text, "Implementation", 0);
                var expirationDate = GetYearAsString(cmbImplementationYear.SelectedItem.Text, cmbSemester.SelectedItem.Text, "Implementation", 3);

                var courseCreated = Base.Classes.LCO.AddCourse(grade, programArea, course, implementYear, SessionObject.LoggedInUser.Page, expirationDate);

                if (courseCreated == null || courseCreated.Contains(0))
                {
                    LabelCourseErrorMessage.Text = "This course already exists!";

                    ScriptManager.RegisterStartupScript(this, typeof(AddCourse), "ErrorMessage", "selectTextBoxCourseID();", true);
                    return;
                }


                ScriptManager.RegisterStartupScript(this, typeof(AddCourse), "AddedCourse", "autoSizeWindow();", true);

                resultPanel.Visible = true;
                addPanel.Visible = false;

                TextBoxHiddenEncryptedCourseID.Text = Standpoint.Core.Classes.Encryption.EncryptString(string.Format("{0}", courseCreated[0]));

                lblResultMessage.Text = "Course successfully added";
            
        }

        protected void ButtonEditOkClick(object sender, EventArgs e)
        {

            var programArea = cmbProgramArea.SelectedItem.Text;
            var course = txtCourse.Text.Trim();
                Base.Classes.LCO.EditCourse(_courseId, programArea, course);
                ScriptManager.RegisterStartupScript(this, typeof(AddCourse), "EdittedCourse", "autoSizeWindow();", true);

                resultPanel.Visible = true;
                addPanel.Visible = false;

                TextBoxHiddenEncryptedCourseID.Text = Standpoint.Core.Classes.Encryption.EncryptString(_courseId.ToString());

                lblResultMessage.Text = "Course successfully editted";
           }
        

        private void BindPageControls()
        {
            cmbGrade.DataSource = Base.Classes.Grade.GetGradeListForDropDown(UserPage, Permissions);
            cmbGrade.DataTextField = "Grade";
            cmbGrade.DataValueField = "Grade";
            cmbGrade.DataBind();
            if (cmbGrade.Items.Count == 1)
            {
                cmbGrade.SelectedIndex = 0;
            }
            else
            {
                cmbGrade.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));
            }
        }

        private void BuildSubjects()
        {
            // Now load the filter button tables and databind.
            DataTable dtSubject = Thinkgate.Base.Classes.LCO.LoadProgramAreas();
            // The existing columns are 'Subject' and 'Abbreviation'.
            // Add a column for 'DropdownText'.
            foreach (DataRow row in dtSubject.Rows)
            {
                row["Abbreviation"] = (String)row["ProgramAreaName"];
            }

            // We will rename Abbreviation to CmbText.
            dtSubject.Columns["Abbreviation"].ColumnName = "CmbText";

            DataRow newRow = dtSubject.NewRow();
            newRow["ProgramAreaName"] = "All";
            newRow["CmbText"] = "Select";
            dtSubject.Rows.InsertAt(newRow, 0);

            // Data bind the combo box.
            cmbProgramArea.DataTextField = "CmbText";
            cmbProgramArea.DataValueField = "ProgramAreaName";
            cmbProgramArea.DataSource = dtSubject;
            cmbProgramArea.DataBind();
        }

        private void BuildSemesters()
        {
            // Now load the filter button tables and databind.
            DataTable dtSemesters = Thinkgate.Base.Classes.Semester.GetSemesterListForDropDownFallSummerSpring();
            // The existing columns are 'Subject' and 'Abbreviation'.
            // Add a column for 'DropdownText'.
            // Data bind the combo box.
            cmbSemester.DataTextField = "Semester";
            cmbSemester.DataValueField = "Semester";
            cmbSemester.DataSource = dtSemesters;
            cmbSemester.DataBind();
        }
        private string GetYearAsString(string year, string semester, string scenario, int expiration)
        {
            int yr_short = Convert.ToInt32((year).Substring(2));
            string sem = semester.ToLower();
            string yr;
            if (sem == "fall")
            {
                if (scenario.ToLower() == "implementation")
                {
                    yr = semester+" " +(yr_short.ToString() + "-" + (yr_short + 1).ToString());

                }
                else if (scenario.ToLower() == "expiration")
                {

                    yr = semester + " " + ((yr_short + expiration).ToString() + "-" + ((yr_short + expiration) + 1).ToString());

                }
                else yr = null;
                
            }
            else if (sem == "spring")
            {
                if (scenario.ToLower() == "implementation")
                {
                    yr = semester + " " + ((yr_short - 1).ToString() + "-" + (yr_short).ToString());

                }
                else if (scenario.ToLower() == "expiration")
                {

                    yr = semester + " " + ((yr_short + expiration - 1).ToString() + "-" + (yr_short + expiration).ToString());

                }
                else yr = null;

            }
            else yr=null;

            return (yr);
        }
        private void BINDYEAR()
        {
            DataTable dtyears = new DataTable();
            dtyears.Columns.Add("Years", typeof(int));
            int yr = (int)DateTime.Today.Year;
           
            for (int i = yr; i < yr + 20; i++)
            {
                DataRow newRow = dtyears.NewRow();
                newRow["Years"] = i;
                dtyears.Rows.Add(newRow);
            }
            cmbImplementationYear.DataTextField = "Years";
            cmbImplementationYear.DataValueField = "Years";
            cmbImplementationYear.DataSource = dtyears;
            cmbImplementationYear.DataBind();
        }
       
    }
}