using System;

namespace Thinkgate.Controls.Student
{
    public partial class StudentAssessmentResultsChart : Thinkgate.Classes.TileControlBase
    {
        public int? StudentID;

        protected void Page_Load(object sender, EventArgs e)
        {
            StudentID = 38822;
            LoadResults();

            //SessionObject.ExpandingTile_Control = this;
        }

        private void LoadResults()
        {
            if (StudentID == null) return;

            ResultsChart.DataSource = Base.Classes.Data.StudentDB.GetStudentAssessmentResultsByID(StudentID.Value);
            ResultsChart.DataBind();
        }
    }
}