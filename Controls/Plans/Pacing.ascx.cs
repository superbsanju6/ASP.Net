using System;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using Thinkgate.Base.Enums;

namespace Thinkgate.Controls.Plans
{
    public partial class Pacing : TileControlBase
    {
        private DateTime _currentDay;
        private DateTime _currentWeekStart;
        private DateTime _currentMonthStart;

        private int _selectedPlanID;
        private InstructionalPlan _selectedPlan;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _selectedPlanID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("planID"));

            var key = "InstructionalPlan_" + _selectedPlanID;
            if (Base.Classes.Cache.Get(key) == null) return; //Uh Oh - Not Loaded properly
            _selectedPlan = ((InstructionalPlan)Base.Classes.Cache.Get(key));

            if (IsPostBack && lblCurrentDay.Text.Length > 0)
            {
                _currentDay = DataIntegrity.ConvertToDate(lblCurrentDay.Text);
                _currentWeekStart = DataIntegrity.ConvertToDate(hiddenCurrentStartOfWeek.Value);
                _currentMonthStart = DataIntegrity.ConvertToDate(hiddenCurrentMonthStart.Value);

            }
            else
            {
                _currentDay = DateTime.Now;
                _currentWeekStart = GetStartOfWeek();
                _currentMonthStart = GetStartOfMonth();                
            }

            LoadViews();
        }

        #region RadCalendar Events

        /*
        protected void RadCalendar1_SelectionChanged(object sender, Telerik.Web.UI.Calendar.SelectedDatesEventArgs e)
        {
            _currentDay = e.SelectedDates.Count - 1 >= 0 ? e.SelectedDates[e.SelectedDates.Count - 1].Date : DateTime.Now;
            _currentWeekStart = GetStartOfWeek();
            _currentMonthStart = GetStartOfMonth();

            LoadViews();
        }
        */

        protected void RadCalendar1_DayRender(object sender, Telerik.Web.UI.Calendar.DayRenderEventArgs e)
        {
            var currentDate = e.Day.Date;
            if (_currentMonthStart <= currentDate && currentDate <= _currentMonthStart.AddMonths(1))
            {
                var events = _selectedPlan.CalendarEvents.FindAll(t => t.StartDate.Day == currentDate.Day);

                if (events.Count > 0)
                {
                    TableCell currentCell = e.Cell;
                    currentCell.Style["background-color"] = "Navy";
                    currentCell.Style["color"] = "White";
                }
            }
        }

        #endregion

        #region NextPreviousButtonClicks

        protected void previousButton_Click(object sender, EventArgs e)
        {
            switch (_currentDay.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    _currentDay = _currentDay.AddDays(-3);
                    break;

                case DayOfWeek.Saturday:
                    _currentDay = _currentDay.AddDays(-2);
                    break;

                default:
                    _currentDay = _currentDay.AddDays(-1);
                    break;
            }

            _currentWeekStart = GetStartOfWeek();
            _currentMonthStart = GetStartOfMonth();

            LoadViews();
        }

        protected void nextButton_Click(object sender, EventArgs e)
        {
            switch (_currentDay.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    _currentDay = _currentDay.AddDays(3);
                    break;

                case DayOfWeek.Saturday:
                    _currentDay = _currentDay.AddDays(2);
                    break;

                default:
                    _currentDay = _currentDay.AddDays(1);
                    break;
            }

            _currentWeekStart = GetStartOfWeek();
            _currentMonthStart = GetStartOfMonth();

            LoadViews();
        }

        protected void previousWeekBtn_Click(object sender, EventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            _currentDay = _currentWeekStart;
            _currentMonthStart = GetStartOfMonth();

            LoadViews();
        }

        protected void nextWeekBtn_Click(object sender, EventArgs e)
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            _currentDay = _currentWeekStart;
            _currentMonthStart = GetStartOfMonth();

            LoadViews();
        }

        #endregion
       
        #region LoadViews

        private void LoadViews()
        {
            LoadDayView();
            LoadWeekView();
            LoadMonthView();
        }

        private void LoadDayView()
        {
            lblCurrentDay.Text = _currentDay.ToLongDateString();

            BindDayViewRepeater(dayViewStandardsRepeater, lblNoStandards, "Standard");
            BindDayViewRepeater(dayViewLessonPlansRepeater, lblNoLessonPlans, "Lesson Plan");
            BindDayViewRepeater(dayViewResourcesRepeater, lblNoResources, "Resource");
            BindDayViewRepeater(dayViewAssessmentsRepeater, lblNoAssessments, "Assessment");
        }

        private void BindDayViewRepeater(Repeater repeater, Label lblNoResults, string apptType)
        {
            var data = _selectedPlan.CalendarEvents.FindAll(t => t.AppointmentType == apptType && t.StartDate.Date == _currentDay.Date);
            lblNoResults.Visible = (data.Count == 0);
            repeater.DataSource = data;
            repeater.DataBind();
        }

        protected void lbxLessonPlans_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            var item = e.Item;
            var lnkPlanName = (HyperLink)item.FindControl("lnkPlanName");
            var dataItem = (Standpoint.Core.Entities.Appointment)e.Item.DataItem;

            if (lnkPlanName != null)
            {
                if (UserHasPermission(Permission.Edit_Resource))
                {
                    /*var link = "window.open('" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=")
                             + "' + escape('display.asp?key=7266&fo=basic display&rm=page&xID=" + dataItem.CategoryID_Encrypted
                             + "&??hideButtons=Save And Return&??appName=E3')); return false;";*/

                    lnkPlanName.NavigateUrl = ResolveUrl(dataItem.Description); //Temporarily using Description to hold view link.
                    //lnkPlanName.Attributes.Add("onclick", link);
                }
            }
        }

        protected void lbxResources_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            var item = e.Item;
            var lnkResourceName = (HyperLink)item.FindControl("lnkResourceName");
            var dataItem = (Standpoint.Core.Entities.Appointment)e.Item.DataItem;

            if (lnkResourceName != null)
            {
                if (UserHasPermission(Permission.Edit_Resource))
                {
                    /*var link = "window.open('" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") 
                             + "' + escape('display.asp?key=7266&fo=basic display&rm=page&xID=" + dataItem.CategoryID_Encrypted
                             + "&??hideButtons=Save And Return&??appName=E3')); return false;";*/
                    lnkResourceName.Target = "_blank";
                    lnkResourceName.NavigateUrl = ResolveUrl(dataItem.Description); //Temporarily using Description to hold view link.
                    //lnkResourceName.Attributes.Add("onclick", link);
                }
            }
        }

        private void LoadWeekView()
        {
            hiddenCurrentStartOfWeek.Value = _currentWeekStart.ToLongDateString();
            lblCurrentWeek.Text = string.Format("{0} - {1}", _currentWeekStart.ToString("MMM dd"), (_currentWeekStart.AddDays(5)).ToString("MMM dd, yyyy"));

            var daysData = new List<InstructionalPlanDay>();

            //For each day of the week, create a subset of calendarEvents
            for (var i = 0; i < 5; i++)
            {
                daysData.Add(_selectedPlan.GetDay(_currentWeekStart.AddDays(i)));
            }

            dayOfWeekRepeater.DataSource = daysData;
            dayOfWeekRepeater.DataBind();
        }

        private void LoadMonthView()
        {
            hiddenCurrentMonthStart.Value = _currentMonthStart.ToLongDateString();
            RadCalendar1.SelectedDate = _currentDay;
        }

        #endregion

        #region HelperMethods

        private DateTime GetStartOfWeek()
        {
            int diff = _currentDay.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return _currentDay.AddDays(-1 * diff).Date;
        }

        private DateTime GetStartOfMonth()
        {
            return new DateTime(_currentDay.Year, _currentDay.Month, 1);
        }

        protected void expandTile_Click(object sender, EventArgs e)
        {
            if (Tile.ParentContainer != null)
                Tile.ParentContainer.ExpandTile(Tile, "Expand");
        }

        #endregion

    }
}