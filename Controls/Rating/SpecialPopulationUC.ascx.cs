using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Classes.Base;
using Thinkgate.Domain.Classes.Review;
using Thinkgate.Domain.Facades;

namespace Thinkgate.Controls.Rating
{
    public partial class SpecialPopulationUc : UserControlBase
    {
        #region Private Variables

        private ReviewFacade _reviewFacade;

        #endregion Private Variables

        #region Public Variables

        public int AgeId { get; set; }
        public int GradeId { get; set; }
        public List<int> SpecialPopulations { get; set; }

        #endregion Public Variables

        #region Events

        /// <summary>
        /// Fires on user control load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Initialize();

            if (!Page.IsPostBack)
            {
                PopulateData();
            }
        }

        /// <summary>
        /// Fires on item bound of special population list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void specialPopulations_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            SpecialPopulation specialPopulation = e.Item.DataItem as SpecialPopulation;

            if (specialPopulation != null)
            {
                if (SpecialPopulations != null
                    && SpecialPopulations.Exists(exists => exists == specialPopulation.Id))
                {
                    RadButton checkBox = e.Item.FindControl("chkSpecialPopulation") as RadButton;
                    if (checkBox != null)
                    {
                        checkBox.Checked = true;
                    }

                }
            }
        }
        #endregion Events

        #region Private Methods
        /// <summary>
        /// Initialize the respective variables
        /// </summary>
        private void Initialize()
        {
            _reviewFacade = new ReviewFacade(Base.Classes.AppSettings.ConnectionString);
        }

        /// <summary>
        /// Populate the data from review facade and bind the dropdowns.
        /// </summary>
        private void PopulateData()
        {
            PopulateAgeDropDown();
            SetAgeSelection();

            PopulateGradeDropDown();
            SetGradeSelection();

            PopulateSpecialPopulations();
        }

        /// <summary>
        /// Populates the Grades drop down list with all the Grades from the Database
        /// </summary>
        private void PopulateGradeDropDown()
        {
            //Populate grade details
            List<Grade> grades = _reviewFacade.GetAllGrades().ToList();

            if (grades.Count > 0)
            {
                grades.Insert(0, new Grade { Id = default(int), Grades = "Select Grade" });
            }

            ddlGrade.DataSource = grades;
            ddlGrade.DataTextField = "Grades";
            ddlGrade.DataValueField = "Id";
            ddlGrade.DataBind();
        }

        /// <summary>
        /// On an edit to an existing review this method will display the last saved Grade.  On a new review it
        /// will default to the first grade in the list.
        /// </summary>
        private void SetGradeSelection()
        {
            ddlGrade.SelectedValue = GradeId.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Populates the Age drop down list with all the Ages (5-18) from the Database
        /// </summary>
        private void PopulateAgeDropDown()
        {
            ddlAge.Items.Add(new ListItem
                             {
                                 Text = "Select Age",
                                 Value = default(int).ToString(CultureInfo.InvariantCulture)
                             });

            _reviewFacade.GetAllAges().ToList().ForEach(
                loop => 
                    ddlAge.Items.Add(new ListItem
                                         {
                                             Text = loop.Ages.ToString(CultureInfo.InvariantCulture),
                                             Value = loop.Id.ToString(CultureInfo.InvariantCulture)
                                         }));
        }

        /// <summary>
        /// On an edit to an existing review this method will display the last saved age.  On a new review it
        /// will default to the first age in the list.
        /// </summary>
        private void SetAgeSelection()
        {
            ddlAge.SelectedValue = AgeId.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Loads all the special populations from the database into the UI
        /// </summary>
        private void PopulateSpecialPopulations()
        {
            //Populate special population details
            rptSpecialPopulation.DataSource = _reviewFacade.GetAllSpecialPopulations(true);
            rptSpecialPopulation.DataBind();
        }
        
        #endregion Private Methods
    }
}