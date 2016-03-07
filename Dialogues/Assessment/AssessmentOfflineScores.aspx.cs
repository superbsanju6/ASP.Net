using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Enums;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentOfflineScores : System.Web.UI.Page
    {
        #region Properties
        public long ClassID { get; set; }
        public long TestID { get; set; }
        public ScoreType ScoreType { get; set; }
        private Base.Classes.Assessment _assessment;
        #endregion Properties

        #region Member Variables
        List<Student> studentDTOList = default(List<Student>);
        #endregion Member Variables

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            GetSessionDetails();
            _assessment = Base.Classes.Assessment.GetAssessmentByID(DataIntegrity.ConvertToInt(TestID));
            if (!Page.IsPostBack)
            {
                GetStudentDetails();
                PopulateStudentDetails();
                Page.Title = _assessment.TestName;
            }
        }

        /// <summary>
        /// Binds the data to the repeater.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptScores_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Student studentDTO = e.Item.DataItem as Student;

            ((Label)e.Item.FindControl("lblStudents")).Text = studentDTO.StudentName;
            ((HiddenField)e.Item.FindControl("hidStudentID")).Value = studentDTO.ID.ToString();

            switch (this.ScoreType)
            {
                case Enums.ScoreType.P:                  
                    ((RadNumericTextBox)e.Item.FindControl("radTxtScores")).Visible = true;
                    ((RadioButtonList)e.Item.FindControl("radScore")).Visible = false;
                    RadNumericTextBox scores = ((RadNumericTextBox)e.Item.FindControl("radTxtScores"));
                    scores.Text = studentDTO.PercentScore;
                    scores.MinValue = 0.00;
                    scores.MaxValue = 100.00;
                    break;

                case Enums.ScoreType.S:
                    ((RadNumericTextBox)e.Item.FindControl("radTxtScores")).Visible = true;
                    ((RadioButtonList)e.Item.FindControl("radScore")).Visible = false;
                    RadNumericTextBox score = ((RadNumericTextBox)e.Item.FindControl("radTxtScores"));
                    score.Text = studentDTO.ScaleScore;
                    score.MinValue = 0.00;
                    score.MaxValue = 9999.99;
                    break;

                case Enums.ScoreType.F:
                case Enums.ScoreType.Y:
                    ((RadNumericTextBox)e.Item.FindControl("radTxtScores")).Visible = false;
                    ((RadioButtonList)e.Item.FindControl("radScore")).Visible = true;

                    if (this.ScoreType.Equals(ScoreType.Y))
                    {
                        ((RadioButtonList)e.Item.FindControl("radScore")).Items[0].Text = "Yes";
                        ((RadioButtonList)e.Item.FindControl("radScore")).Items[1].Text = "No";                        
                    }

                    if (!string.IsNullOrEmpty(studentDTO.PercentScore))
                        ((RadioButtonList)e.Item.FindControl("radScore")).SelectedValue = studentDTO.PercentScore;

                    break;
            }


        }

        /// <summary>
        /// Fires the event when user clicks on the save button.
        /// Save the changes made on score to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radSave_Click(object sender, EventArgs e)
        {
            this.SaveStudentScores();
            ScriptManager.RegisterStartupScript(this, typeof(string), "CloseDialog", "closeCurrentCustomDialog();", true);            
        }

        #endregion Events

        #region Methods

        /// <summary>
        /// Retrieves the student and scores from the database.
        /// </summary>
        void GetStudentDetails()
        {
            studentDTOList = Student.GetStudentInfo(ClassID, TestID, ScoreType.ToString());
        }

        /// <summary>
        /// Populate the student and score information from the database based on the classID and testID
        /// </summary>
        void PopulateStudentDetails()
        {
            if (studentDTOList != null)
            {
                rptScores.DataSource = studentDTOList;
                rptScores.DataBind();
            }
            else
            {
                this.radSave.Enabled = false;
            }
        }

        /// <summary>
        /// Save the scores to the respective students to the database.
        /// </summary>
        void SaveStudentScores()
        {
            dtGeneric_String_String studentScore = new dtGeneric_String_String();

            foreach (RepeaterItem rpItem in rptScores.Items.AsParallel())
            {
                string calculatedScore = default(string);

                if (ScoreType.Equals(Enums.ScoreType.P) || ScoreType.Equals(ScoreType.S))
                {

                    calculatedScore = (string.IsNullOrEmpty(((RadNumericTextBox)rpItem.FindControl("radTxtScores")).Text.Trim()) || ScoreType.Equals(ScoreType.S)) ? ((RadNumericTextBox)rpItem.FindControl("radTxtScores")).Text :
                                             (Convert.ToDouble(((RadNumericTextBox)rpItem.FindControl("radTxtScores")).Text.Trim()) * .01).ToString();

                }
                else
                {
                    calculatedScore = ((RadioButtonList)rpItem.FindControl("radScore")).SelectedValue;
                }


                studentScore.Add(((HiddenField)rpItem.FindControl("hidStudentID")).Value, calculatedScore);
            }

            Student.SaveStudentScore(ClassID, TestID, ScoreType.ToString(), studentScore);
        }

        /// <summary>
        /// Retrieve the classID, testID and scoreType from the querystring.
        /// </summary>
        void GetSessionDetails()
        {
            if (Request.QueryString["classID"] != null)
                this.ClassID = Convert.ToInt64(Request.QueryString["classID"]);

            if (Request.QueryString["assessmentID"] != null)
                this.TestID = Convert.ToInt64(Request.QueryString["assessmentID"]);

            if (Request.QueryString["scoreType"] != null)
                this.ScoreType = (ScoreType)Enum.Parse(typeof(ScoreType), Request.QueryString["scoreType"]);

            //this.ClassID = 8587;
            //this.TestID = 403;
            //this.ScoreType = Enums.ScoreType.Y;
        }

        #endregion Methods  
    }
}