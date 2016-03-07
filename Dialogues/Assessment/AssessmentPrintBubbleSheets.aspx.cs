using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web;
using Telerik.Charting;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Collections.Generic;
using System.Collections;
using Thinkgate.Base.Enums;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentPrintBubbleSheets : BasePage
    {
        SessionObject sessionObject;

        protected void Page_Load(object sender, EventArgs e)
        {
            sessionObject = (SessionObject)Session["SessionObject"];

            if (Request.QueryString["assessmentCategory"] != null)
            { SetPrintAssessmentsBubbleSheetsOptions(Request.QueryString["assessmentCategory"].ToString()); }

            Int64 cteID;
            if (Request.QueryString["cteIDList"] != null)
            {
                CteIDList = !CteIDList.Contains(',') ? Request.QueryString["cteIDList"].ToString() + "," : Request.QueryString["cteIDList"].ToString();
            }

            else if (Request.QueryString["xID"] == null || !Int64.TryParse(Request.QueryString["xID"], out cteID))
            {
                sessionObject.RedirectMessage = "No test event ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                CteID = cteID;

                // Get the csv list of student ids passed in the url.
                if (Request.QueryString["yID"] != null)
                    StudentIdsCsv = Request.QueryString["yID"];

                // Disable some selections if no students selected.
                if (String.IsNullOrEmpty(StudentIdsCsv))
                {
                    btnStudentForm.Enabled = false;
                    btnHaloCal.Enabled = false;
                    btnHaloNonCal.Enabled = false;
                }

                if (!IsPostBack)
                {
                    // Print button initially disabled.
                    //					btnPrint.Enabled = false;
                }
            }
        }

        /// <summary>
        /// The user id.
        /// </summary>
        /// <summary>
        /// The class test event id property.
        /// </summary>
        protected Int64 CteID
        {
            get { return Int64.Parse(inpCteID.Value); }
            set { inpCteID.Value = value.ToString(); }
        }

        /// <summary>
        /// A csv list of selected students.
        /// </summary>
        protected String StudentIdsCsv
        {
            get { return inpStudentIdsCsv.Value; }
            set { inpStudentIdsCsv.Value = value; }
        }

        /// <summary>
        /// A csv list of selected students.
        /// </summary>
        protected String CteIDList
        {
            get { return inpCteID.Value.ToString(); }
            set { inpCteID.Value = value; }
        }

        private void SetPrintAssessmentsBubbleSheetsOptions(string assessmentCategory)
        {
            string[] pPSFormTypes = null;
            AssessmentCategories assessmentCategories = (AssessmentCategories)Enum.Parse(typeof(AssessmentCategories), assessmentCategory, true);

            if (assessmentCategories == AssessmentCategories.Classroom)
            { pPSFormTypes = DistrictParms.LoadDistrictParms().ClassroomPPSFormTypes.Split(',').Select(pp => { return pp.Trim(); }).ToArray().Where(x => !string.IsNullOrEmpty(x)).ToArray(); }

            if (assessmentCategories == AssessmentCategories.District)
            { pPSFormTypes = DistrictParms.LoadDistrictParms().DistrictPPSFormTypes.Split(',').Select(pp => { return pp.Trim(); }).ToArray().Where(x => !string.IsNullOrEmpty(x)).ToArray(); }

            foreach (string pPSFormType in pPSFormTypes)
            {
                PrintAssessmentsBubbleSheetsOptions printAssessmentsBubbleSheetsOptions = (PrintAssessmentsBubbleSheetsOptions)Enum.Parse(typeof(PrintAssessmentsBubbleSheetsOptions), pPSFormType, true);
                switch (printAssessmentsBubbleSheetsOptions)
                {
                    case PrintAssessmentsBubbleSheetsOptions.Student:
                        btnStudentForm.Enabled = true;
                        break;
                    case PrintAssessmentsBubbleSheetsOptions.Blank:
                        btnBlankForm.Enabled = true;
                        break;
                    case PrintAssessmentsBubbleSheetsOptions.Roster:
                        btnRosterForm.Enabled = true;
                        break;
                    case PrintAssessmentsBubbleSheetsOptions.Halo:
                        btnHaloCal.Enabled = true;
                        break;
                    case PrintAssessmentsBubbleSheetsOptions.Halocalibration:
                        btnHaloNonCal.Enabled = true;
                        break;
                }
            }
        }
    }
}