using System;
using Thinkgate.Classes;
using Telerik.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Teacher
{
    public partial class ThinkgateUniversity : TileControlBase
    {
        private const String _courseTypeFilterKey = "TGUCourseTypeFilter";
        private const String _enrollmentFilterKey = "TGUEnrollmentFilter";
         
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create the initial viewstate values.
            if (ViewState[_courseTypeFilterKey] == null)
            {
                ViewState.Add(_courseTypeFilterKey, "All");
                ViewState.Add(_enrollmentFilterKey, "All");
            }

            if (!IsPostBack)
            {
                BuildFilters();
                LoadData();
            }
        }

        private void BuildFilters()
        {
            cmbCourseType.Items.Clear();
            cmbCourseType.Items.Add(new RadComboBoxItem("Course Type", "All"));
            cmbCourseType.Items.Add(new RadComboBoxItem("Online", "Online"));
            cmbCourseType.Items.Add(new RadComboBoxItem("Instructor-Led", "Instructor-Led"));
            cmbCourseType.Items.Add(new RadComboBoxItem("Resource", "Resource"));

            cmbEnrollmentType.Items.Clear();
            cmbEnrollmentType.Items.Add(new RadComboBoxItem("Enrollment", "All"));
            cmbEnrollmentType.Items.Add(new RadComboBoxItem("Open", "Open"));
            cmbEnrollmentType.Items.Add(new RadComboBoxItem("Required", "Required"));
            cmbEnrollmentType.Items.Add(new RadComboBoxItem("Conditional", "Conditional"));
        }


        private void LoadData()
        {
            var courses = Thinkgate.Base.Classes.Data.TeacherDB.GetThinkgateUniversityCourses(SessionObject.LoggedInUser.Page, ViewState[_courseTypeFilterKey].ToString(), ViewState[_enrollmentFilterKey].ToString(), 0);
        }

        protected void cmbCourseType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_courseTypeFilterKey] = e.Value;
            LoadData();
        }

        protected void cmbEnrollmentType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[_enrollmentFilterKey] = e.Value;
            LoadData();
        }

        protected void lbxRegisteredList_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = e.Item;
            DataRowView row = (DataRowView)(listBoxItem).DataItem;

            var imgCourseType = (Image)listBoxItem.FindControl("imgCourseType");
            var imgCourseRequired = (Image)listBoxItem.FindControl("imgCourseRequired");
            var imgBtnLaunch = (ImageButton)listBoxItem.FindControl("imgBtnLaunch");
            var lnkCourseName = (HyperLink)listBoxItem.FindControl("lnkCourseName");

            //Set Course Type Image URL, visibility of launch icon, and URL of launch icon
            switch (row["CourseType"].ToString())
            {
                case "Online Course":
                    imgCourseType.ImageUrl = "~/Images/onlinecourse.png";
                    imgBtnLaunch.Visible = true;
                    imgBtnLaunch.OnClientClick = "window.open('" + ResolveUrl("~/LumenixBridge.aspx") + "')";
                    break;

                case "Instructor-Led":
                    imgCourseType.ImageUrl = "~/Images/instructorled.png";
                    imgBtnLaunch.Visible = false;
                    break;

                case "Resource":
                    imgCourseType.ImageUrl = "~/Images/tguresource.png";
                    imgBtnLaunch.Visible = false;
                    break;
            }
            
            //Set visibility of required
            imgCourseRequired.Visible = DataIntegrity.ConvertToBool(row["Required"]);
            
            //Set url of course name
            lnkCourseName.NavigateUrl = ResolveUrl("~/LumenixBridge.aspx");
        }
    }
}