using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes.Comments;
using System.Web.UI.WebControls;
using Thinkgate.Domain.Facades;

namespace Thinkgate.Dialogues.Assessment
{
    /// <summary>
    /// This class shows the AdministratorTestQuestionComments for a given student
    /// on a given test.  It displays all TestQuestions that have been marked to Allow Comments in the 
    /// following format:
    /// 
    /// Item Number: Test Question Text
    /// 
    /// Administrator Comment
    /// </summary>
    public partial class ViewAssessmentItemComments : BasePage
    {
        #region Variables 

        private CommentFacade _commentFacade;
        private IList<AdminTestQuestionComments> _administratorComments;
        private int _assessmentId;
        private int _studentId;
        String _serverPath = String.Empty;

        #endregion

        #region Constants 

        private const String PAGE_TITLE = "View Assessment Comments";
        private const String NO_COMMENT_ENTERED = "No comment entered";
        private const String NO_COMMENTS_FOR_STUDENT =
            "This assessment contains no comments for the selected student";
        private const string ASSESSMENT_ID_QUERY_STRING_KEY = "assessmentID";
        private const string STUDENT_ID_QUERY_STRING_KEY = "studentID";
        private const string TEST_QUESTION_IMAGE_KEY_UPPERCASE = "SRC=\"UPLOAD";
        private const string UPLOADS_VIRTUAL_DIRECTORY = "~/Upload";

        #endregion 

        /// <summary>
        /// Eventhandler to catch the PageLoad event and initialize the page.
        /// </summary>
        /// <param name="sender">object that fired this event</param>
        /// <param name="e">EventArgs passes from the object that fired the event</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _serverPath = Server.MapPath(UPLOADS_VIRTUAL_DIRECTORY);

            _commentFacade = new CommentFacade(AppSettings.ConnectionString);
            Title = PAGE_TITLE;
            ExtractParameters();
            _administratorComments =
                _commentFacade.GetAdminTestQuestionCommentsForAssessment(
                    _studentId,
                    _assessmentId).ToList();
            SetHeaderText();
            BindTestQuestionComments();
        }

        /// <summary>
        /// Causes our list of AdminTestQuestionComments to DataBind to our repeater control
        /// in order to display all the data in the UI
        /// </summary>
        private void BindTestQuestionComments()
        {
            if (lblHeader.Text != NO_COMMENTS_FOR_STUDENT)
            {
                rptComments.DataSource = _administratorComments;
                rptComments.DataBind();
            }
        }

        /// <summary>
        /// Checks to see if the assessmentID and StudentID have been passed in the query string.
        /// If they are present they values are set into the _assessmentID and _studentID variables.
        /// If they are missing the User will be directed back to the portal selection screen and 
        /// will see an error message.
        /// </summary>
        private void ExtractParameters()
        {
            if (IsQueryStringMissingParameter(ASSESSMENT_ID_QUERY_STRING_KEY)
                || IsQueryStringMissingParameter(STUDENT_ID_QUERY_STRING_KEY))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                int.TryParse(Request.QueryString[ASSESSMENT_ID_QUERY_STRING_KEY], out _assessmentId);
                int.TryParse(Request.QueryString[STUDENT_ID_QUERY_STRING_KEY], out _studentId);
            }
        }

        /// <summary>
        /// Sets the header text of this page.  If a student has AdministratorTestQuestionComments
        /// entered, the header text will be Comments for [Student Name].
        /// If there are no AdministratorTestQuestionComments entered, the header text will display the 
        /// value of the NO_COMMENTS_FOR_STUDENT constant.
        /// </summary>
        private void SetHeaderText()
        {
            String headerText = NO_COMMENTS_FOR_STUDENT;

            if (_administratorComments != null && _administratorComments.Count > 0)
            {
                AdminTestQuestionComments questionComment = FindCommentWithStudentName(_administratorComments);
                if (questionComment != null)
                {
                    headerText = String.Format("Comments for {0} ", questionComment.StudentName);
                }
            }

            lblHeader.Text = headerText;
        }

        /// <summary>
        /// This eventhandler catches the event that fires everytime a row is databound on our repeater 
        /// control.  It calls methods to set the Test Question Text and set the comment text on the UI
        /// </summary>
        /// <param name="sender">The object that fires this event</param>
        /// <param name="e">RepeaterItemEventArgs passed by the object that fires this event</param>
        protected void OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            AdminTestQuestionComments testQuestionComment =
                e.Item.DataItem as AdminTestQuestionComments;

            if (testQuestionComment != null)
            {
                SetTestQuestionText(e, testQuestionComment);
                SetCommentText(e, testQuestionComment);
            }
        }

        /// <summary>
        /// This method sets the text of the test question number and the test question text
        /// </summary>
        /// <param name="e">RepeaterItemEventArgs that contains the RepeaterItem we are working with</param>
        /// <param name="testQuestionComment">The AdminTestQuestionComments object that this row is bound to</param>
        private void SetTestQuestionText(
            RepeaterItemEventArgs e,
            AdminTestQuestionComments testQuestionComment)
        {
            Label itemCtrl = e.Item.FindControl("lblItem") as Label;
            if (itemCtrl != null)
            {
                ParseAndReplaceImagesInQuestionText(testQuestionComment);

                itemCtrl.Text = string.Format(
                    "Item {0}: {1}",
                    testQuestionComment.TestQuestionNumber.ToString(CultureInfo.CurrentCulture),
                    testQuestionComment.TestQuestionText);
            }
        }

        /// <summary>
        /// Parses the question text, checks to see if it contains an image and if so replaces the
        /// img src path with the mapped path of the uploads Virtual Directory
        /// </summary>
        /// <param name="testQuestionComment">The TestQuestionComment object whose text we are going to reformat so that the image(s) will display properly</param>
        private void ParseAndReplaceImagesInQuestionText(AdminTestQuestionComments testQuestionComment)
        {
            if (testQuestionComment.TestQuestionText.ToUpper(CultureInfo.CurrentCulture).Contains(TEST_QUESTION_IMAGE_KEY_UPPERCASE))
            {
                testQuestionComment.TestQuestionText =
                    Regex.Replace(
                        testQuestionComment.TestQuestionText,
                        "Upload",
                        _serverPath,
                        RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// This method sets the text AdministratorTestQuestionComment onto the UI
        /// </summary>
        /// <param name="e">RepeaterItemEventArgs that contains the RepeaterItem we are working with</param>
        /// <param name="testQuestionComment">The AdminTestQuestionComments object that this row is bound to</param>
        private void SetCommentText(
            RepeaterItemEventArgs e,
            AdminTestQuestionComments testQuestionComment)
        {
            Literal commentCtrl = e.Item.FindControl("ltrComments") as Literal;
            if (commentCtrl != null)
            {
                if (!String.IsNullOrEmpty(testQuestionComment.CommentText))
                {
                    //If there is a comment present, then that means it was entered from the Legacy code...
                    //Legacy encodes whitespace and special characters with escape character
                    //(example a "space" character is encoded as %20)
                    //so we need to decode the escape characters so it renders correctly
                    commentCtrl.Text = HttpUtility.UrlDecode(testQuestionComment.CommentText, Encoding.ASCII);
                }
                else
                {
                    //If there is no comment then we do not need to decode since NO_COMMENT_ENTERED
                    //is a c# constant and does not contain any special encoding
                    commentCtrl.Text = NO_COMMENT_ENTERED;
                }
            }
        }

        /// <summary>
        /// This method loops through all the AdminTestQuestionComments that are associated with this test
        /// for the selected student in order to retreive the Students name.
        /// </summary>
        /// <param name="testQuestionsThatMightHaveAComment">A List containing all the AdminTestQuestionComments 
        /// that are associated with this test for the selected student</param>
        /// <returns>If at least one AdminTestQuestionComments object contains a students name, that object is returned; else returns null</returns>
        private AdminTestQuestionComments FindCommentWithStudentName(IList<AdminTestQuestionComments> testQuestionsThatMightHaveAComment)
        {
            foreach (AdminTestQuestionComments questionComment in testQuestionsThatMightHaveAComment)
            {
                if (!String.IsNullOrEmpty(questionComment.StudentName))
                {
                    return questionComment;
                }
            }

            return null;
        }
    }
}