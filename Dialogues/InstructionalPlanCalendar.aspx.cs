using System;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Collections.Generic;
using System.Collections;
using Thinkgate.Base.DataAccess;
using Standpoint.Core.Data;
using System.Web;
using System.Text;

namespace Thinkgate.Dialogues
{
    public partial class InstructionalPlanCalendar : BasePage
    {
        SessionObject sessionObject;
        public int _planID;
        public Base.Classes.InstructionalPlan _selectedPlan;
        public Base.Classes.Assessment _selectedAssessment;
        public Base.Classes.Resource _selectedResource;
        public Base.Classes.LessonPlan _selectedLessonPlan;
        public Base.Classes.Standards _selectedStandard;

        public String newEventTargetType
        {
            get
            {
                if (_selectedAssessment != null)
                    return "Assessment";

                if (_selectedResource != null)
                    return "Resource";

                if (_selectedLessonPlan != null)
                    return "LessonPlan";

                if (_selectedStandard != null)
                    return "Standard";

                return "";
            } 
        }

        public int newEventTargetID
        {
            get
            {
                if (_selectedAssessment != null)
                    return _selectedAssessment.AssessmentID;

                if (_selectedResource != null)
                    return _selectedResource.ID;

                if (_selectedLessonPlan != null)
                    return _selectedLessonPlan.ID;

                if (_selectedStandard != null)
                    return _selectedStandard.ID;

                return 0;
            }
        }

        public String newEventTitle 
        { 
            get 
            {
                if (_selectedAssessment != null)
                    return _selectedAssessment.Description;

                if (_selectedResource != null)
                    return _selectedResource.ResourceName;

                if (_selectedLessonPlan != null)
                    return _selectedLessonPlan.PlanName;

                if (_selectedStandard != null)
                    return _selectedStandard.StandardName;

                return ""; 
            } 
        }

        #region Page events
        
        protected void Page_Load(object sender, EventArgs e)
        {
            sessionObject = (SessionObject)Session["SessionObject"];

            if (Request.QueryString["xID"] == null)
            {
                SessionObject.RedirectMessage = "No item type provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _planID = DataIntegrity.ConvertToInt(Request.QueryString["xID"]);
                var key = "InstructionalPlan_" + _planID;
                if (!RecordExistsInCache(key)) return;
                _selectedPlan = ((Base.Classes.InstructionalPlan)Base.Classes.Cache.Get(key));

                if (Request.QueryString["assessmentID"] != null)
                {
                    _selectedAssessment = Base.Classes.Assessment.GetAssessmentByID(DataIntegrity.ConvertToInt(Request.QueryString["assessmentID"]));
                }

                if (Request.QueryString["resourceID"] != null)
                {
                    _selectedResource = Base.Classes.Resource.GetDummyResourceByID(DataIntegrity.ConvertToInt(Request.QueryString["resourceID"]));
                }

                if (Request.QueryString["lessonplanid"] != null)
                {
                    _selectedLessonPlan = Base.Classes.LessonPlan.GetPlanByID(DataIntegrity.ConvertToInt(Request.QueryString["lessonplanid"]));
                }

                if (Request.QueryString["standardid"] != null)
                {
                    _selectedStandard = Base.Classes.Standards.GetStandardByID(DataIntegrity.ConvertToInt(Request.QueryString["standardid"]));
                }

                InstructionalPlanScheduler.DataSource = _selectedPlan.CalendarEvents;
                InstructionalPlanScheduler.DataBind();

                if (Request.QueryString["view"] != null) //If a day is passed in query string, flip to day view and that date
                {
                    switch (Request.QueryString["view"].ToString())
                    {
                        case "Day":
                            InstructionalPlanScheduler.SelectedView = SchedulerViewType.DayView;
                            break;

                        case "Week":
                            InstructionalPlanScheduler.SelectedView = SchedulerViewType.WeekView;
                            break;

                        case "Month":
                        default:
                            InstructionalPlanScheduler.SelectedView = SchedulerViewType.MonthView;
                            break;
                    }
                                    
                }

                if(Request.QueryString["day"] != null) //If a day is passed in query string, flip to day view and that date
                {
                    InstructionalPlanScheduler.SelectedView = SchedulerViewType.DayView;
                    InstructionalPlanScheduler.SelectedDate = DataIntegrity.ConvertToDate(Request.QueryString["day"]);
                }                
            }
        }

        protected void InstructionalPlanScheduler_AppointmentCommand(object sender, AppointmentCommandEventArgs e)
        {
            if (e.CommandName == "Export")
            {
                WriteCalendar(RadScheduler.ExportToICalendar(e.Container.Appointment));
            }
        }

        protected void exportButton_Click(object sender, ImageClickEventArgs e)
        {
            WriteCalendar(RadScheduler.ExportToICalendar(InstructionalPlanScheduler.Appointments));
        }

        protected void InstructionalPlanScheduler_AppointmentCreated(object sender, AppointmentCreatedEventArgs e)
        {
            var description = e.Appointment.Description;
            var previewLink = (HyperLink)e.Container.FindControl("previewLink");
            var previewImage = (Image)e.Container.FindControl("previewImage");
            previewLink.Visible = !String.IsNullOrEmpty(description);
            previewImage.Visible = !String.IsNullOrEmpty(description);

            if(!String.IsNullOrEmpty(description))
            {
                previewLink.NavigateUrl = ResolveUrl(description);
            }
        }

        protected void InstructionalPlanScheduler_AppointmentUpdate(object sender, AppointmentUpdateEventArgs e)
        {
            _selectedPlan.UpdateCalendarEvent(DataIntegrity.ConvertToInt(e.ModifiedAppointment.ID), e.ModifiedAppointment.Start,
                                                e.ModifiedAppointment.End, e.ModifiedAppointment.Subject);
        }
        protected void InstructionalPlanScheduler_AppointmentInsert(object sender, AppointmentInsertEventArgs e)
        {
            Appointment appt = e.Appointment;
            _selectedPlan.AddCalendarEvent(appt.Start, appt.End, appt.Subject, newEventTargetType, newEventTargetID);
        }

        #endregion

        private void WriteCalendar(string data)
        {
            HttpResponse response = Page.Response;

            response.Clear();
            response.Buffer = true;

            response.ContentType = "text/calendar";
            response.ContentEncoding = Encoding.UTF8;
            response.Charset = "utf-8";

            response.AddHeader("Content-Disposition", "attachment;filename=\"InstructionalPlanCalendar.ics\"");

            response.Write(data);
            response.End();
        }

        #region Properties
        /// <summary>
        /// Get the current user id.
        /// </summary>
        #endregion

    }
}