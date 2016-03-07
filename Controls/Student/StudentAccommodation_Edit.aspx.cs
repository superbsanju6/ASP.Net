using System;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Student
{
    public partial class StudentAccommodation_Edit : BasePage
    {

        protected Base.Classes.Student _selectedStudent;
        private int _UserPage;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadStudentObject();

            if (!IsPostBack)
            {
                Page.Title = "Edit Student Accommodation";
                // Get Page # and Permissions for this user.
                _UserPage = SessionObject.LoggedInUser.Page;
                BindPageControls();
            }

            switch (Request.Form["__EVENTTARGET"])
            {
                case "RadButtonOk":
                    ButtonOkClick(this, new EventArgs());
                    break;

            }
        }

        /// <summary>
        /// decrypt the xID query parameter and use to create a Student Object.  
        /// If no xID parameter, then return with message.
        /// </summary>
        private void LoadStudentObject()
        {
            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                var studentID = GetDecryptedEntityId(X_ID);
                 _selectedStudent = Base.Classes.Data.StudentDB.GetStudentByID(studentID);
 
            }
        }

        private void BindPageControls()
        {
            LabelStudentName.Text = _selectedStudent.StudentName;
            LabelStudentName.Attributes.Add("initialValue", LabelStudentName.Text);
            gridSubjectLevel.DataSource = _selectedStudent.StudentAccommodations;
            gridSubjectLevel.DataBind();
        }


        protected void ButtonOkClick(object sender, EventArgs e)
        {
            var results = Save_Accommodations();

            lblResultMessage.Text = results;

            ScriptManager.RegisterStartupScript(this, typeof(StudentAccommodation_Edit), "SavedStudent", "autoSizeWindow();", true);

            resultPanel.Visible = true;
            addPanel.Visible = false;
        }



        protected string Save_Accommodations()
        {
            try
            {
               foreach (GridDataItem item in gridSubjectLevel.Items)
                {
                    int id = Convert.ToInt32(item["ID"].Text);
                    int studentid = Convert.ToInt32(item["StudentID"].Text);
                    CheckBox chk = item.FindControl("CheckBox2") as CheckBox;
                    bool status = chk.Checked;
                    if (status != Convert.ToBoolean(item["Value"].Text))
                    {
                        StudentAccommodations.UpdateAccommodationByID(studentid, id, status);
                    }
                }
                return "Updates successful";
            }
            catch (ApplicationException e)
            {
                return e.ToString();
            }
        }

    }
}
