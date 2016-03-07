namespace Thinkgate.Controls.School
{
    using System;
    using System.Web.UI;
    using Telerik.Web.UI;
    using Thinkgate.Classes;

    public partial class AddSchool : BasePage
    {
        private const int UserPage = 110;

        private const string Permissions = " 0000000000000000000000202200000000000000000000222000222222222000200022000000000000000000000000000000000000000000000000000000000000000002000000000000000200002000000000000000002000000000000000000000000000002222222000000000000000 00000002000000000000000000000000000000 220022202220000000000000000000000000000000000000000000000000000200000000000020202000000000000000000000000000000000000000000000000000000002000000000000000   0000000000000000002220000000000000000000000000002000   0                     ";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPageControls();
            }

            if (Request.Form["__EVENTTARGET"] == "RadButtonOk")
            {
                ButtonOkClick(this, new EventArgs());
            }
        }

        protected void ButtonOkClick(object sender, EventArgs e)
        {
            var schoolName = TextBoxSchoolName.Text.Trim();
            var abbreviation = TextBoxAbbreviation.Text.Trim();
            var schoolType = RadComboBoxSchoolType.SelectedItem.Text;
            var cluster = RadComboBoxCluster.SelectedItem.Text;
            var schoolID = TextBoxSchoolID.Text.Trim();
            var phone = TextBoxPhone.Text.Trim();

            var school = Base.Classes.School.AddSchool(schoolName, abbreviation, phone, schoolType, cluster, schoolID, UserPage);

            if (school == null)
            {
                LabelSchoolIDErrorMessage.Text = "A school already exists with this School ID.";

                ScriptManager.RegisterStartupScript(this, typeof(AddSchool), "ErrorMessage", "selectTextBoxSchoolID();", true);
                return;
            }

            ScriptManager.RegisterStartupScript(this, typeof(AddSchool), "AddedSchool", "autoSizeWindow();", true);

            resultPanel.Visible = true;
            addPanel.Visible = false;

            TextBoxHiddenEncryptedSchoolID.Text = Standpoint.Core.Classes.Encryption.EncryptString(string.Format("{0}", school.ID));

            lblResultMessage.Text = "School successfully added";
        }

        private void BindPageControls()
        {
            RadComboBoxSchoolType.DataSource = Base.Classes.SchoolType.GetSchoolTypeListForDropDown(UserPage);
            RadComboBoxSchoolType.DataTextField = "SchoolType";
            RadComboBoxSchoolType.DataValueField = "ID";
            RadComboBoxSchoolType.DataBind();
            RadComboBoxSchoolType.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));

            RadComboBoxCluster.DataSource = Base.Classes.Cluster.GetClusterListForDropDown(UserPage);
            RadComboBoxCluster.DataTextField = "Cluster";
            RadComboBoxCluster.DataValueField = "ID";
            RadComboBoxCluster.DataBind();
            RadComboBoxCluster.Items.Insert(0, (new RadComboBoxItem { Text = "Select", Value = "0" }));
        }
    }
}
