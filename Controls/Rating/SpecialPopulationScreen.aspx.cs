using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes.Review;
using Thinkgate.Domain.Facades;

namespace Thinkgate.Controls.Rating
{
    public partial class SpecialPopulationScreen : BasePage
    {
        #region Variables
        private ReviewFacade _reviewFacade;
        private QuestionReview _questionReview;
        private int _questionId;
        private Guid _userId;
        private bool _isEnabled;
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

        private Repeater SpecialPopulationRepeater
        {
            get { return FindPageControl<Repeater>(SPECIAL_POPULATION_USER_CONTROL, "rptSpecialPopulation"); }
        }
        #endregion Private Properties

        #region Events
        /// <summary>
        /// Fires the event on page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Initialize();
            SetControlState();
            if (!Page.IsPostBack)
            {
                GetData();
                SetControl();
            }
        }
        #endregion Events

        #region Private Methods
        /// <summary>
        /// Create the instance of Review Facade
        /// Get all querystring values
        /// </summary>
        private void Initialize()
        {
            _reviewFacade = new ReviewFacade(Base.Classes.AppSettings.ConnectionString);
            _questionId = Convert.ToInt32(GetValueFromQueryString<string>("QuestionID"));
            _userId = new Guid(GetValueFromQueryString<string>("UserID"));
            _isEnabled = GetValueFromQueryString<bool>("IsEnabled");
        }

        /// <summary>
        /// Set the control state based on _isReadOnly variable
        /// </summary>
        private void SetControlState()
        {
            pnlControl.Enabled = _isEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetData()
        {
            _questionReview = _reviewFacade.GetAllQuestionReview(_questionId, _userId);
        }

        /// <summary>
        /// Set special population controls
        /// Age, Grade and Special Population.
        /// </summary>
        private void SetControl()
        {
            SetGradeDropDown();
            SetAgeDropDown();
            SetSpecialPopulation();
        }

        /// <summary>
        /// Set grade dropdown value
        /// </summary>
        private void SetGradeDropDown()
        {
            if (GradeDropDownList != null && _questionReview.Grade != default(Grade))
            {
                specialPopulationUC.GradeId = _questionReview.Grade.Id;
            }
        }

        /// <summary>
        /// Set age dropdown value
        /// </summary>
        private void SetAgeDropDown()
        {
            if (AgeDropDownList != null && _questionReview.Age != default(Age))
            {
                specialPopulationUC.AgeId = _questionReview.Age.Id;
            }
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

        #endregion Private Methods
    }
}