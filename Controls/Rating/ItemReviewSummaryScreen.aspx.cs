using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes.Comments;
using Thinkgate.Domain.Classes.Review;
using Thinkgate.Domain.Facades;
using System.Web.UI;

namespace Thinkgate.Controls.Rating
{
    public partial class ItemReviewSummaryScreen : BasePage
    {
        #region Variables

        private ReviewFacade _reviewFacade;
        private int _questionId;
        private LeaQuestionReviewAverages _leaQuestionReviewAverages;
        private ReviewRatingSummary _reviewRatingSummary;

        #endregion Variables

        #region Constants

        private const string RATING_ID = "RatingId";
        private const string HTML_NEW_LINE = "<br>";

        #endregion

        #region Events
        /// <summary>
        /// Fires when page loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Initialize();
            if (!Page.IsPostBack)
            {
                GetData();
                BindData();
            }
        }

        /// <summary>
        /// Fires when review details are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptReviewSummary_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            QuestionReview questionReivew = e.Item.DataItem as QuestionReview;

            if (questionReivew != default(QuestionReview))
            {
               SetCommentControl(e, questionReivew);
               SetRatingCreatedByAndDateControls(e, questionReivew);
               SetEditAndDeleteButtonRules(e, questionReivew);
            }
        }

        /// <summary>
        /// Fires when rating count datasource are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptRating_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            KeyValuePair<Domain.Classes.Review.Rating, int> rating = (KeyValuePair<Domain.Classes.Review.Rating, int>)e.Item.DataItem;

            RadRating ctrlRadRating = e.Item.FindControl("rating") as RadRating;
            if (ctrlRadRating != null)
            {
                ctrlRadRating.Value = rating.Key.RatingValue;
            }

            SetRatingReviewCountLink(e, rating);

        }

        /// <summary>
        /// Filter the review details based on user selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void showReviews_OnClick(object sender, EventArgs e)
        {
            LinkButton btnLink = sender as LinkButton;
            if (btnLink != null && btnLink.Attributes["RatingId"] != null)
            {
                GetQuestionRatingById(Convert.ToInt32(btnLink.Attributes["RatingId"]));
                GetAllReviewRatingSummary();
                BindData();
                if ((btnLink.Parent as RepeaterItem) != null)
                {
                    SetHiddenFieldClientId(string.Format("rptRating_{0}",btnLink.ClientID.ToString(CultureInfo.InvariantCulture)));
                }
                else
                {
                    SetHiddenFieldClientId(btnLink.ClientID.ToString(CultureInfo.InvariantCulture));
                }
                
            }
        }

        /// <summary>
        /// Deactivates the review for question
        /// Retore the grid and total count
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_OnClick(object sender, EventArgs e)
        {
            RadButton btnDelete = sender as RadButton;

            if (btnDelete != null)
            {
                _reviewFacade.DeactivateReviewForQuestion(Convert.ToInt32(btnDelete.Attributes["ReviewId"]));

                SetRefreshFlag();

                HiddenField hidClientId = FindPageControl<HiddenField>("hidActionId");
                if (hidClientId != null)
                {
                    LinkButton btnLink = FindPageControl<LinkButton>(hidClientId.Value);

                    if (btnLink != null)
                    {
                        GetQuestionRatingById(Convert.ToInt32(btnLink.Attributes["RatingId"]));
                    }
                }

                GetAllReviewRatingSummary();
                BindData();
            }
        }
        #endregion Events

        #region Private Methods

        /// <summary>
        /// Set the rating review count 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="rating"></param>
        private void SetRatingReviewCountLink(RepeaterItemEventArgs e, KeyValuePair<Domain.Classes.Review.Rating, int> rating)
        {
            Label ctrlReviewCountLabel = e.Item.FindControl("lblReviewCount") as Label;
            LinkButton ctrlReviewCount = e.Item.FindControl("btnReviewCount") as LinkButton;

            if (rating.Value == default(int))
            {
                if (ctrlReviewCountLabel != null)
                {
                    ctrlReviewCountLabel.Text = rating.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (ctrlReviewCount != null)
                {
                    ctrlReviewCount.Visible = false;
                }
            }
            else
            {
                if (ctrlReviewCountLabel != null)
                {
                    ctrlReviewCountLabel.Visible = false;
                }
                if (ctrlReviewCount != null)
                {
                    ctrlReviewCount.Attributes.Add(RATING_ID, rating.Key.Id.ToString(CultureInfo.InvariantCulture));
                    ctrlReviewCount.Text = rating.Value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>
        /// Set the Refresh Flag on parent window to refresh the review if there is change in review.
        /// </summary>
        private void SetRefreshFlag()
        {
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "setParentRefreshFlag", "setParentRefreshFlag();", true);
        }

        /// <summary>
        /// Set the hidden field value.
        /// Hidden Field will hold the client id of user action.
        /// </summary>
        /// <param name="clientId"></param>
        private void SetHiddenFieldClientId(string clientId)
        {
            HiddenField hidClientId = FindPageControl<HiddenField>("hidActionId");
            if (hidClientId != null)
            {
                hidClientId.Value = clientId;
            }
        }

        /// <summary>
        /// Set the comment control based on questionreview
        /// </summary>
        /// <param name="e"></param>
        /// <param name="questionReivew"></param>
        private void SetCommentControl(RepeaterItemEventArgs e, QuestionReview questionReivew)
        {
            Literal ctrlComment = e.Item.FindControl("ltrReviewLimited") as Literal;

            if (ctrlComment != null && questionReivew.ReviewComment != default(AdminTestQuestionComments))
            {
                ctrlComment.Text = questionReivew.ReviewComment.CommentText.Length > 100 ?
                                   questionReivew.ReviewComment.CommentText.Substring(0, 100) :
                                   questionReivew.ReviewComment.CommentText;

                if (questionReivew.ReviewComment.CommentText.Length > 100)
                {
                    LinkButton moreLink = e.Item.FindControl("btnComment") as LinkButton;
                    if (moreLink != null)
                    {
                        moreLink.Visible = true;
                    }

                    Literal ctrlCommentFull = e.Item.FindControl("ltrReviewFull") as Literal;
                    if (ctrlCommentFull != null)
                    {
                        ctrlCommentFull.Text = questionReivew.ReviewComment.CommentText;
                    }
                }
            }
        }

        /// <summary>
        /// Set Rating, CreatedBy and Date Control based on questionreview
        /// </summary>
        /// <param name="e"></param>
        /// <param name="questionReview"></param>
        private void SetRatingCreatedByAndDateControls(RepeaterItemEventArgs e, QuestionReview questionReview)
        {
            RadRating ctrlRating = e.Item.FindControl("rating") as RadRating;

            if (ctrlRating != null && questionReview.Rating != null)
            {
                ctrlRating.Value = questionReview.Rating.RatingValue;
            }

            Label ctrlReviewer = e.Item.FindControl("lblReviewer") as Label;

            if (ctrlReviewer != null)
            {
                ctrlReviewer.Text = String.Format(
                    "{0}{1}{2}",
                    questionReview.CreatorName,
                    HTML_NEW_LINE,
                    questionReview.Review.RoleName);
            }

            Label ctrlDate = e.Item.FindControl("lblDate") as Label;

            if (ctrlDate != null && questionReview.Review != default(Review))
            {
                ctrlDate.Text = questionReview.Review.CreatedDate.ToShortDateString();
            }
        }

        /// <summary>
        /// Set Edit and Delete button rules.
        /// Edit button will be available only for reviewer who creates the review.
        /// Delete button will be available only for reviewer who creates the review and who has Admin role. 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="questionReivew"></param>
        private void SetEditAndDeleteButtonRules(RepeaterItemEventArgs e, QuestionReview questionReivew)
        {
            RadButton btnEdit = e.Item.FindControl("btnEdit") as RadButton;
            if (btnEdit != null)
            {
                btnEdit.Visible = (questionReivew.CreatedById == SessionObject.LoggedInUser.UserId);
            }

            RadButton btnDelete = e.Item.FindControl("btnDelete") as RadButton;
            if (btnDelete != null)
            {
                btnDelete.Visible = ((questionReivew.CreatedById
                                      == SessionObject.LoggedInUser.UserId)
                                     || SessionObject.LoggedInUser.Roles.Any(
                                         any => any.RoleName.Contains("Admin")));
            }

        }

        /// <summary>
        /// Initialize the instances and get the respective itemId from query string
        /// </summary>
        private void Initialize()
        {
            _reviewFacade = new ReviewFacade(Base.Classes.AppSettings.ConnectionString);
            _questionId = GetValueFromQueryString<int>("ItemID");

            SetHiddenFieldClientId(btnShowAll.ClientID.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Get the data from database for questionid
        /// </summary>
        private void GetData()
        {
            _reviewRatingSummary = _reviewFacade.GetAllReviewRatingSummary(_questionId);
            _leaQuestionReviewAverages = _reviewFacade.GetAllQuestionRatings(_questionId, 0);
        }

        /// <summary>
        /// Filter rating for question Id
        /// </summary>
        /// <param name="ratingId"></param>
        private void GetQuestionRatingById(int ratingId)
        {
            _leaQuestionReviewAverages = _reviewFacade.GetAllQuestionRatings(_questionId, ratingId);
        }

        /// <summary>
        /// Get review rating summary
        /// </summary>
        private void GetAllReviewRatingSummary()
        {
            _reviewRatingSummary = _reviewFacade.GetAllReviewRatingSummary(_questionId);
        }

        /// <summary>
        /// Bind the data to the control
        /// </summary>
        private void BindData()
        {
            if (_reviewRatingSummary != default(ReviewRatingSummary))
            {
                SetReviewAverageTotalCountAndNaReview();

                if (_reviewRatingSummary.RatingSummary != null)
                {
                    rptRating.DataSource = _reviewRatingSummary.RatingSummary.Where(filter => filter.Key.RatingValue != -1);
                    rptRating.DataBind();
                }
            }

            if (_leaQuestionReviewAverages != default(LeaQuestionReviewAverages))
            {
                rptReviewSummary.DataSource = _leaQuestionReviewAverages.QuestionReviews;
                rptReviewSummary.DataBind();
            }
        }

        /// <summary>
        /// Set the review average, total count and n/a review count
        /// </summary>
        private void SetReviewAverageTotalCountAndNaReview()
        {
            lblTotalRating.Text = string.Format("{0} Reviews ", _reviewRatingSummary.ReviewCount);
            SetAverageRating();
            SetShowAll();
            SetNAReviewCount();
        }

        private void SetAverageRating()
        {
            lblAverageRating.Text = string.Format("{0} Stars", _reviewRatingSummary.AverageReview);
            HiddenField ctrlAverageRating = FindPageControl<HiddenField>("hidAverageRating");
            if (ctrlAverageRating != null)
            {
                ctrlAverageRating.Value =
                    _reviewRatingSummary.AverageReview.ToString(CultureInfo.InvariantCulture);
            }

            averageRating.Value = _reviewRatingSummary.AverageReview;
        }

        /// <summary>
        /// Set Show All 
        /// </summary>
        private void SetShowAll()
        {
            if (_reviewRatingSummary.RatingSummary.Any(any => any.Value != 0))
            {
                btnShowAll.Attributes.Add(RATING_ID, default(int).ToString(CultureInfo.InvariantCulture));
                lblShowAll.Visible = false;
            }
            else
            {
                btnShowAll.Visible = false;
            }

           
        }

        /// <summary>
        /// Set No review given count
        /// </summary>
        private void SetNAReviewCount()
        {
            if (_reviewRatingSummary.RatingSummary != null && _reviewRatingSummary.RatingSummary.Any(any => any.Key.RatingValue == -1))
            {
                int reviewCount = _reviewRatingSummary.RatingSummary.FirstOrDefault(first => first.Key.RatingValue == -1).Value;

                if (reviewCount != 0)
                {
                    lblNAReviewCount.Visible = false;

                    btnNAReviewCount.Text = reviewCount.ToString(CultureInfo.InvariantCulture);
                    btnNAReviewCount.Attributes.Add(
                        RATING_ID,
                        _reviewRatingSummary.RatingSummary.FirstOrDefault(
                            first => first.Key.RatingValue == -1)
                                            .Key.Id.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    btnNAReviewCount.Visible = false;
                }
            }
        }

        #endregion Private Methods
    }
}