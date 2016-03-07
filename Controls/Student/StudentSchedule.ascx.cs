using System;

namespace Thinkgate.Controls.Student
{
    public partial class StudentSchedule : System.Web.UI.UserControl
    {
        public int? StudentID;

        protected void Page_Load(object sender, EventArgs e)
        {
            StudentID = 38797;
            LoadStudentSchedule();
        }

        private void LoadStudentSchedule()
        {
            if (StudentID == null) return;
            gridStudentSchedule.DataSource = Base.Classes.Data.StudentDB.GetStudentSchedule(StudentID.Value);
            gridStudentSchedule.DataBind();
        }
    }
}