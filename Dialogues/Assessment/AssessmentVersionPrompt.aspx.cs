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
using Standpoint.Core.Classes;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentVersionPrompt : BasePage
	{
		const Int32 cbxUnchecked = 0;
		const Int32 cbxFilled = 1;
		const Int32 cbxChecked = 2;
		public const String numAssessmentCol = "NumAssessmentCopies", numAnswerKeyCol = "NumAnswerKeyCopies", numRubricsCol = "NumRubricsCopies";
		public const String assessmentEnabledCol = "AssessmentEnabled", answerKeyEnabledCol = "AnswerKeyEnabled", rubricsEnabledCol = "RubricsEnabled";
		SessionObject sessionObject;

		protected void Page_Load(object sender, EventArgs e)
		{
			sessionObject = (SessionObject)Session["SessionObject"];

			Int32 assessmentID;
			if (Request.QueryString["xID"] == null || !Int32.TryParse(Encryption.DecryptString(Request.QueryString["xID"]), out assessmentID))
			{
				sessionObject.RedirectMessage = "No assessment ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else
			{
				AssessmentID = assessmentID;
				AssessmentName = Request.QueryString["yID"];

				if(!IsPostBack)
				{
				}
			}
		}


        /// <summary>
        /// The assessment id property.
        /// </summary>
        protected Int32 AssessmentID
        {
            get { return Int32.Parse(Encryption.DecryptString(inpAssessmentID.Value)); }
            set { inpAssessmentID.Value = Encryption.EncryptString(value.ToString()); }
        }

		/// <summary>
		/// The assessment name.
		/// </summary>
		protected String AssessmentName
		{
			get { return inpAssessmentName.Value; }
			set { inpAssessmentName.Value = value; }
		}


	}
}