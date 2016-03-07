using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes.Comments;
using Thinkgate.Domain.Classes.Review;
using Thinkgate.Domain.Facades;

namespace Thinkgate.Controls.Rating
{
    public partial class ItemReviewMainScreen : BasePage
    {
        #region Variables
        private ReviewFacade _reviewFacade;
        private QuestionReview _questionReview;
        private int _questionId;
        private string _source;
        private string _actionId;
        private static List<Domain.Classes.Review.Rating> _ratings;
        private const string SPECIAL_POPULATION_USER_CONTROL = "specialPopulationUC";
        #endregion Variables

        #region Private Properties

        private DropDownList AgeDropDownList
        {
            get { return FindPageControl<DropDownList>(SPECIAL_POPULATION_USER_CONTROL, "ddlAge"); }
        }

        private DropDownList GradeDropDownList
        {
            get { return FindPageControl<DropDownList>(SPECIAL_POPULATION_USER_CONTROL, "ddlGrade"); }
        }

        private DropDownList RoleDropDownList
        {
            get { return FindPageControl<DropDownList>("ddlRoles"); }
        }

        private Repeater SpecialPopulationRepeater
        {
            get { return FindPageControl<Repeater>(SPECIAL_POPULATION_USER_CONTROL, "rptSpecialPopulation"); }
        }
        #endregion Private Properties

        #region Events
        /// <summary>
        /// Fires on page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Initialize();
            SetHiddenField();

            if (!Page.IsPostBack)
            {
                GetData();
                GetRatings();
                PopulateRolesList();
                SetControls();
                SetControlAccess();
            }

            if (Request.Form["__EVENTTARGET"] != null && !string.IsNullOrEmpty(Request.Form["__EVENTTARGET"]))
            {
                RaiseEventByTarget(Request.Form["__EVENTTARGET"]);
            }
        }
        #endregion Events

        #region Private Methods

        /// <summary>
        /// Disables the Roles selection control if the logged in user only has one Role.  If the logged in user 
        /// has more than one Role associated with them, then the control remains Enabled
        /// </summary>
        private void SetControlAccess()
        {
            ddlRoles.Enabled = !HasUserOnlyOneRole;
        }

        /// <summary>
        /// Check whether user has only one role
        /// </summary>
        private bool HasUserOnlyOneRole
        {
            get
            {
                return (SessionObject.LoggedInUser.Roles != null
                       && SessionObject.LoggedInUser.Roles.Count == 1);
            }
        }


        /// <summary>
        /// Populates the roles dropdown list with the Roles of the logged in User
        /// </summary>
        private void PopulateRolesList()
        {
            //Populate role details
            ddlRoles.DataSource = SessionObject.LoggedInUser.Roles.Distinct();
            ddlRoles.DataTextField = "RoleName";
            ddlRoles.DataValueField = "RoleId";
            ddlRoles.DataBind();
        }

        /// <summary>
        /// Get the initial details from database.
        /// </summary>
        private void GetData()
        {
            _questionReview = _reviewFacade.GetAllQuestionReview(_questionId, SessionObject.LoggedInUser.UserId);
        }

        /// <summary>
        /// Get the rating values
        /// </summary>
        private void GetRatings()
        {
            _ratings = _reviewFacade.GetAllRatings().ToList();
        }

        /// <summary>
        /// Set controls intial values. 
        /// </summary>
        private void SetControls()
        {
            if (_questionReview != default(QuestionReview))
            {
                SetComment();
                SetAgeDropDown();
                SetGradeDropDown();
                SetRoleDropDown();
                SetRating();
                SetSpecialPopulation();
            }
            else
            {
                SetInitialRules();
            }
        }

        /// <summary>
        /// Set the inital rule for the checkbox 
        /// </summary>
        private void SetInitialRules()
        {
            chkNoRating.Checked = true;
        }

        /// <summary>
        /// Set special population control
        /// </summary>
        private void SetSpecialPopulation()
        {
            if (_questionReview.SpecialPopulations != null)
            {
                specialPopulationUC.SpecialPopulations = new List<int>();
                _questionReview.SpecialPopulations.ForEach(loop => specialPopulationUC.SpecialPopulations.Add(loop.Id));
            }
        }

        /// <summary>
        /// Set the rating control and No rating checkbox
        /// </summary>
        private void SetRating()
        {
            if (_questionReview.Review != default(Review))
            {
                if (_ratings != null
                    && _ratings.Exists(exists => exists.Id == _questionReview.Review.RatingId))
                {
                    decimal ratingValue =
                        _ratings.Find(find => find.Id == _questionReview.Review.RatingId)
                               .RatingValue;

                    if (ratingValue == -1)
                    {
                        chkNoRating.Checked = true;
                        rating.ReadOnly = true;
                    }
                    else
                    {
                        rating.Value =  Convert.ToInt32(ratingValue);
                    }
                }
            }
        }

        /// <summary>
        /// Set the comment
        /// </summary>
        private void SetComment()
        {
            if (_questionReview.ReviewComment != default(AdminTestQuestionComments))
            {
                txtReivew.Text = _questionReview.ReviewComment.CommentText;
            }
        }

        /// <summary>
        /// Set role dropdown
        /// </summary>
        private void SetRoleDropDown()
        {
            if(_questionReview.Review != default(Review) && !string.IsNullOrEmpty(_questionReview.Review.RoleName))
            {
                if (SessionObject.LoggedInUser.Roles.Exists(
                        exists => exists.RoleName == _questionReview.Review.RoleName))
                {
                    ddlRoles.SelectedValue =
                        SessionObject.LoggedInUser.Roles.Find(
                            find => find.RoleName == _questionReview.Review.RoleName).RoleId.ToString();
                }
            }
        }

        /// <summary>
        /// Set Grade Dropdown Value
        /// </summary>
        private void SetGradeDropDown()
        {
            if (GradeDropDownList != null && _questionReview.Grade != default(Grade))
            {
                specialPopulationUC.GradeId = _questionReview.Grade.Id;
            }
        }

        /// <summary>
        /// Set Age Dropdown Value
        /// </summary>
        private void SetAgeDropDown()
        {
            if (AgeDropDownList != null && _questionReview.Age != default(Age))
            {
                specialPopulationUC.AgeId = _questionReview.Age.Id;
            }
        }

        /// <summary>
        /// Initialize the instances and get the respective itemId from query string
        /// </summary>
        private void Initialize()
        {
            _reviewFacade = new ReviewFacade(Base.Classes.AppSettings.ConnectionString);
            _questionId = GetValueFromQueryString<int>("ItemID");
            _source = GetValueFromQueryString<string>("Source");
            _actionId = GetValueFromQueryString<string>("ActionID");
        }

        /// <summary>
        /// Set hidden field values
        /// </summary>
        private void SetHiddenField()
        {
            HiddenField ctrlActionId = FindPageControl<HiddenField>("hidActionId");
            if (ctrlActionId != null)
            {
                ctrlActionId.Value = _actionId;
            }

            HiddenField ctrlSource = FindPageControl<HiddenField>("hidSource");
            if (ctrlSource != null)
            {
                ctrlSource.Value = _source;
            }

        }

        /// <summary>
        /// Get the event raised from javascript
        /// </summary>
        /// <param name="eventName"></param>
        private void RaiseEventByTarget(string eventName)
        {
            if (eventName == "radSubmit")
            {
                SubmitReview();
            }
        }

        /// <summary>
        /// Get the question review details
        /// </summary>
        private void GetQuestionReview()
        {
            _questionReview.QuestionId = _questionId;
            _questionReview.CreatedById = SessionObject.LoggedInUser.UserId;
        }

        /// <summary>
        /// Get review details
        /// Role, Rating and No Checked selection
        /// </summary>
        private void GetReview()
        {
            if (_questionReview.Review == default(Review))
                _questionReview.Review = new Review();

            _questionReview.Review.RoleName = RoleDropDownList.SelectedItem.Text;

            if (chkNoRating.Checked && _ratings.Exists(find=>find.RatingValue == -1))
            {
                _questionReview.Review.RatingId = _ratings.Find(find => find.RatingValue == -1).Id;
            }
            else if (_ratings.Exists(find => find.RatingValue == rating.Value))
            {
                _questionReview.Review.RatingId = _ratings.Find(find => Convert.ToInt32(find.RatingValue) == rating.Value).Id;
            }
        }

        /// <summary>
        /// Get review comment details
        /// </summary>
        private void GetReviewComment()
        {


            if (_questionReview.ReviewComment == default(AdminTestQuestionComments))
                _questionReview.ReviewComment = new AdminTestQuestionComments();

            _questionReview.ReviewComment.CommentText = txtReivew.Text;

        }

        /// <summary>
        /// Get selected special populations
        /// </summary>
        private void GetSpecialPopulation()
        {
            _questionReview.SpecialPopulations = new List<SpecialPopulation>();

            foreach (string itemId in from RepeaterItem repeaterItem in SpecialPopulationRepeater.Items
                                      select repeaterItem.FindControl("chkSpecialPopulation") as RadButton
                                          into chkSpecialPopulation
                                          where chkSpecialPopulation != null && 
                                                chkSpecialPopulation.Checked
                                          select chkSpecialPopulation.Attributes["ItemID"])
            {
                _questionReview.SpecialPopulations.Add(new SpecialPopulation
                {
                    Id = Convert.ToInt32(itemId)
                });
            }
        }

        /// <summary>
        /// Get selected grade details
        /// </summary>
        private void GetGrade()
        {
            if (_questionReview.Grade == default(Grade))
                _questionReview.Grade = new Grade();

            _questionReview.Grade.Id = Convert.ToInt32(GradeDropDownList.SelectedValue);
        }

        /// <summary>
        /// Get selected age details
        /// </summary>
        private void GetAge()
        {
            if (_questionReview.Age == default(Age))
                _questionReview.Age = new Age();

            _questionReview.Age.Id = Convert.ToInt32(AgeDropDownList.SelectedValue);
        }

        /// <summary>
        /// Close the dialog 
        /// </summary>
        private void CloseAndRefreshDialog()
        {
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "closeDialog", "closeAndRefresh();", true);
        }

        /// <summary>
        /// Save the review details
        /// </summary>
        private void SubmitReview()
        {
            _questionReview = _reviewFacade.GetAllQuestionReview(_questionId, SessionObject.LoggedInUser.UserId);

            if (_questionReview == default(QuestionReview))
                _questionReview = new QuestionReview();

            GetQuestionReview();
            GetAge();
            GetGrade();
            GetReview();
            GetReviewComment();
            GetSpecialPopulation();

            

            _reviewFacade.SubmitReview(_questionReview);
            CloseAndRefreshDialog();
        }
        #endregion Private Methods
    }
}