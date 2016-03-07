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
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Assessment
{
	public partial class AssessmentConfiguration : TileControlBase
	{
		protected Int32 _userID, _assessmentID;
		protected Thinkgate.Base.Classes.AssessmentInfo _assessmentInfo;
			
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Tile == null) return;
			_userID = SessionObject.LoggedInUser.Page;
			_assessmentID = (Int32)Tile.TileParms.GetParm("assessmentID");
			_assessmentInfo = Thinkgate.Base.Classes.Assessment.GetConfigurationInformation(_assessmentID, _userID);

			SetElementVisibility();

			DataTable dtOnlineContFmt = Base.Classes.Assessment.GetOnlineContentFormat(_userID);
			for (Int32 i = 0; i < dtOnlineContFmt.Rows.Count; i++)
			{
				if (String.Compare(_assessmentInfo.OnlineContentFormat, (String)dtOnlineContFmt.Rows[i]["OnlineContentFormat"], true) == 0)
					lblOnlineContent.Text = (String)dtOnlineContFmt.Rows[i]["DisplayName"];
			}

			DataTable dtDistractorLabels = Base.Classes.Assessment.GetDistractorLabels(_assessmentInfo.NumDistractors, _userID);
			for (Int32 i = 0; i < dtDistractorLabels.Rows.Count; i++)
			{
				if (String.Compare(_assessmentInfo.DistractorLabels, (String)dtDistractorLabels.Rows[i]["Value"], true) == 0)
					lblDistLabels.Text = (String)dtDistractorLabels.Rows[i]["DistractorLabel"];
			}

			lblContentType.Text = _assessmentInfo.ContentType;
			lblNumDist.Text = _assessmentInfo.NumDistractors.ToString();
			lblScoreType.Text = _assessmentInfo.ScoreType;
			lblPerfLevels.Text = _assessmentInfo.PerformanceLevels;
			lblPrintCols.Text = _assessmentInfo.PrintColumns.ToString();
			lblPrintSA.Text = _assessmentInfo.PrintShortAnswer ? "Yes" : "No";
			lblSource.Text = _assessmentInfo.Source;
			lblNumForms.Text = _assessmentInfo.NumForms.ToString();
			lblIncFieldTest.Text = _assessmentInfo.IncludeFieldTest ? "Yes" : "No";
			lblAuthor.Text = _assessmentInfo.Author;
			lblLastEdit.Text = _assessmentInfo.LastEdited.ToShortDateString();
		}

		/// <summary>
		/// Set the visibility of some elements based on _isItemBankAssessment.
		/// </summary>
		protected void SetElementVisibility()
		{
			Boolean isItemBankAssessment = String.Compare(_assessmentInfo.ContentType, "Item Bank", true) == 0;

			trNumForms.Visible = isItemBankAssessment && _assessmentInfo.AllowMultiForms;
			trInclFieldTest.Visible = isItemBankAssessment;

			trOnlineContent.Visible = !isItemBankAssessment;
			trSource.Visible = !isItemBankAssessment;

			trInclFieldTest.Visible = _assessmentInfo.AllowFieldTest;
		}
	}
}
