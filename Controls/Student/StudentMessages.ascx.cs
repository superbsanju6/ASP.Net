using System;

namespace Thinkgate.Controls.Student
{
    public partial class StudentMessages : System.Web.UI.UserControl
    {
        public int? StudentID;

        protected void Page_Load(object sender, EventArgs e)
        {
            StudentID = 38822;
            LoadResults();
        }

        private void LoadResults()
        {
            //if (StudentID == null) return;

            RadGrid1.DataSource = Base.Classes.Data.StudentDB.GetStudentMessagesByID(StudentID.Value);
            RadGrid1.DataBind();
        }

        

    }
}