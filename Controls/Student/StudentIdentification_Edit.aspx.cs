using System;
using System.IO;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Student
{
    public partial class StudentIdentification_Edit : BasePage
    {

        protected Base.Classes.Student _selectedStudent;
        private int _UserPage;

        private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadStudentObject();

            if (!IsPostBack)
            {
                Page.Title = "Edit Student Identification";
                // Get Page # and Permissions for this user.
                _UserPage = SessionObject.LoggedInUser.Page;
                LoadValuesIntoEditableControls();
            }

            switch (Request.Form["__EVENTTARGET"])
            {
                case "RadButtonOk":
                    ButtonOkClick(this, new EventArgs());
                    break;

                case "rbPhotoSave":
                    btnUploadPhotoClick(this, new EventArgs());
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

        /// <summary>
        /// Populate/setup each editable control with its initial value.  Also provide each an attribute 
        /// with its initial value so on the client side, javascript function can determine whether the 
        /// user has changed the value.
        /// </summary>
        private void LoadValuesIntoEditableControls()
        {
            TextBoxFirstName.Text = _selectedStudent.FirstName;
            TextBoxFirstName.Attributes.Add("initialValue", TextBoxFirstName.Text);

            TextBoxMiddleName.Text = _selectedStudent.MiddleName;
            TextBoxMiddleName.Attributes.Add("initialValue", TextBoxMiddleName.Text);

            TextBoxLastName.Text = _selectedStudent.LastName;
            TextBoxLastName.Attributes.Add("initialValue", TextBoxLastName.Text);

            TextBoxEmail.Text = _selectedStudent.Email;
            TextBoxEmail.Attributes.Add("initialValue", TextBoxEmail.Text);

            TextBoxStudentID.Text = _selectedStudent.StudentID;
            TextBoxStudentID.Attributes.Add("initialValue", TextBoxStudentID.Text);

            hdnPhotoFilename.Value = "";
            hdnPhotoFilename.Attributes.Add("initialValue", hdnPhotoFilename.Value);
            imgPhoto.Src = AppSettings.ProfileImageStudentWebPath + "/" + _selectedStudent.ProfilePhoto;
            /**********************************************
              * search for and wire in User's profile image.  
              * If image does not exist or cannot be found,
              * then go with a default image.
              *********************************************/
            if (!string.IsNullOrEmpty(_selectedStudent.ProfilePhoto) &&
                File.Exists(Server.MapPath(AppSettings.ProfileImageStudentWebPath + "/" + _selectedStudent.ProfilePhoto)))
            {
                imgPhoto.Src = AppSettings.ProfileImageStudentWebPath + "/" + _selectedStudent.ProfilePhoto;
            }
            else
            {
                imgPhoto.Src = "../../Images/person.png";
            }

            //Provide configuration for our Telerik Rad Upload control with values from AppSettings.

            RadUpload1.InputSize = AppSettings.ProfileImageMaxFileNameLength;
            RadUpload1.MaxFileSize = AppSettings.ProfileImageMaxFileSize;
            RadUpload1.AllowedFileExtensions = AppSettings.ProfileImageAllowedFileTypes.Split(',');
            RadUpload1.TargetPhysicalFolder = Server.MapPath(AppSettings.ProfileImageStudentWebPath);

            BindPageControls();
        }


        protected void ButtonOkClick(object sender, EventArgs e)
        {
            var firstName = TextBoxFirstName.Text.Trim();
            var middleName = TextBoxMiddleName.Text.Trim();
            var lastName = TextBoxLastName.Text.Trim();
            var email = TextBoxEmail.Text.Trim();
            var studentID = TextBoxStudentID.Text.Trim();
            var grade = RadComboBoxGrade.SelectedValue.Trim();
            var schoolID = RadComboBoxSchool.SelectedValue.Trim();
            var photoFileName = hdnPhotoFilename.Value;


            if (!string.IsNullOrEmpty(email) && !Validations.IsValidEmail(email))
            {
                lblResultMessage.Text = "Email address is not formatted properly.";
                LabelEmailErrorMessage.Text = "Please enter a valid email address.";
                ScriptManager.RegisterStartupScript(this, typeof(StudentIdentification_Edit), "ErrorMessage", "selectTextBoxEmail();", true);
                return;
            }


            var results = Base.Classes.Data.StudentDB.SaveStudentChanges(firstName, middleName, lastName, email, studentID, grade, schoolID, photoFileName, _selectedStudent.ID);

            if (results.IndexOf("Error") >= 0)
            {
                lblResultMessage.Text = results;
                LabelStudentIDErrorMessage.Text = "A student already exists with this Student ID.";
                ScriptManager.RegisterStartupScript(this, typeof(StudentIdentification_Edit), "ErrorMessage", "selectTextBoxStudentID();", true);
                return;
            }
            else
            {
                lblResultMessage.Text = results;

                //attempt to delete the old file
                var oldFilePath = AppSettings.ProfileImageStudentWebPath + "/" + hdnPhotoFilename.Attributes["initialValue"];
                if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
            }

            ScriptManager.RegisterStartupScript(this, typeof(StudentIdentification_Edit), "SavedStudent", "autoSizeWindow();", true);

            resultPanel.Visible = true;
            addPanel.Visible = false;
        }

        private void BindPageControls()
        {
            RadComboBoxGrade.ItemDataBound += new RadComboBoxItemEventHandler(RadComboBoxGrade_ItemDataBound);
            RadComboBoxGrade.DataSource = Base.Classes.Grade.GetGradeListForDropDown(_UserPage, Permissions);
            RadComboBoxGrade.DataTextField = "Grade";
            RadComboBoxGrade.DataValueField = "Grade";
            RadComboBoxGrade.DataBind();
            RadComboBoxGrade.Attributes.Add("initialValue", _selectedStudent.Grade.ToString());

            RadComboBoxSchool.ItemDataBound += new RadComboBoxItemEventHandler(RadComboBoxSchool_ItemDataBound);
            RadComboBoxSchool.DataSource = Base.Classes.School.GetSchoolListForDropDown(_UserPage);
            RadComboBoxSchool.DataTextField = "TXT";
            RadComboBoxSchool.DataValueField = "VAL";
            RadComboBoxSchool.DataBind();
            RadComboBoxSchool.Attributes.Add("initialValue", _selectedStudent.SchoolID.ToString());
        }

        private void RadComboBoxGrade_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            if (e.Item.Value == _selectedStudent.Grade.ToString())
            {
                e.Item.Selected = true;
            }
        }

        private void RadComboBoxSchool_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            if (e.Item.Value == _selectedStudent.SchoolID.ToString())
            {
                e.Item.Selected = true;
            }
        }

        protected void btnUploadPhotoClick(object sender, EventArgs e)
        {
            if (RadUpload1.UploadedFiles.Count > 0)
            {
                var newFile = RadUpload1.UploadedFiles[0];
                /* is the file name of the uploaded file 50 characters or less?  
                    * The database field that holds the file name is 50 chars.
                    * */
                if (newFile.ContentLength > AppSettings.ProfileImageMaxFileSize)
                {
                    labelPhotoErrorMessage.Text = "The file " + newFile.FileName.Substring(newFile.FileName.LastIndexOf(@"\") + 1) + " is too big. The maximum size limit is " + AppSettings.ProfileImageMaxFileSize.ToString("0,000") + " bytes.  Please select another file.";
                }
                else
                {
                    if (newFile.GetName().Length <= AppSettings.ProfileImageMaxFileNameLength)
                    {
                        var uploadFolder = Server.MapPath(AppSettings.ProfileImageStudentWebPath);

                        string newFileName;

                        do
                        {
                            newFileName = (Path.GetRandomFileName()).Replace(".", "") + newFile.GetExtension();
                        }
                        while (System.IO.File.Exists(Path.Combine(uploadFolder, newFileName)));

                        try
                        {
                            newFile.SaveAs(Path.Combine(uploadFolder, newFileName));
                            if (!string.IsNullOrEmpty(hdnPhotoFilename.Value))
                            {
                                var oldFile = Path.Combine(uploadFolder, hdnPhotoFilename.Value);
                                if (File.Exists(oldFile))
                                {
                                    File.Delete(oldFile);
                                }
                            }
                            hdnPhotoFilename.Value = newFileName;
                            imgPhoto.Src = AppSettings.ProfileImageStudentWebPath + "/" + newFileName;
                            _selectedStudent.ProfilePhoto = newFileName;
                            Base.Classes.Data.StudentDB.SaveStudentChanges(_selectedStudent.FirstName,
                                                                              _selectedStudent.MiddleName,
                                                                              _selectedStudent.LastName,
                                                                              _selectedStudent.Email,
                                                                              _selectedStudent.StudentID,
                                                                              _selectedStudent.Grade.ToString(),
                                                                              _selectedStudent.SchoolID.ToString(),
                                                                              _selectedStudent.ProfilePhoto,
                                                                              _selectedStudent.ID);
                        }
                        catch
                        {
                            labelPhotoErrorMessage.Text = "Image Import unsuccessful. Please contact system adminstrator.";
                        }
                    }
                    else
                    {
                        labelPhotoErrorMessage.Text = "Image filename length must be 50 characters or less. Please rename the file and try again.";
                    }
                }
            }
            else
            {
                labelPhotoErrorMessage.Text = "Image import unsuccessful. No image specified.";
            }

        }

    }
}
