using System;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using System.Data;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentConfiguration_Edit : BasePage
	{
		protected SessionObject _sessionObject;
		protected Int32 _userID, _assessmentID;
		protected String _cacheKey;
		protected Thinkgate.Base.Classes.AssessmentInfo _assessmentInfo;
		protected DataTable _dtContentType, _dtNumForms, _dtInclFieldTest, _dtNumDistractors, _dtDistractorLabels;
		protected DataTable _dtPerfLevels, _dtPrintCols, _dtPrintShortAns, _dtOnlineContFmt;

		protected void Page_Load(object sender, EventArgs e)
		{
			_sessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];
			_userID = _sessionObject.LoggedInUser.Page;

			if(_assessmentInfo == null)
				LoadAssessmentInfo();


			lblAuthor.Text = _assessmentInfo.Author;
			lblCreated.Text = _assessmentInfo.LastEdited.ToShortDateString();

			SetElementVisibility();

			if(!IsPostBack)
			{
				BuildContentTypes();
				BuildNumForms();
				BuildInclFieldTest();
				BuildNumDistractors();
				BuildPerformanceLevels();
				BuildPrintColumns();
				BuildPrintShortAnswer();
				BuildOnlineContentFormat();
				tbxSource.Text = _assessmentInfo.Source;
			}
		}

		private void LoadAssessmentInfo()
		{
			if (Request.QueryString["xID"] == null)
			{
				SessionObject.RedirectMessage = "No assessment ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else
			{
                _assessmentID = GetDecryptedEntityId(X_ID);
				_cacheKey = "AssessmentInfo_" + _assessmentID;

				if (!RecordExistsInCache(_cacheKey))
				{
					_assessmentInfo = Thinkgate.Base.Classes.Assessment.GetConfigurationInformation(_assessmentID, _userID);
					if(_assessmentInfo != null)
						Thinkgate.Base.Classes.Cache.Insert(_cacheKey, _assessmentInfo);
					else
					{
						SessionObject.RedirectMessage = "Could not find the assessment.";
						Response.Redirect("~/PortalSelection.aspx", true);
					}
				}
				else
					_assessmentInfo = (Base.Classes.AssessmentInfo)Cache[_cacheKey];
			}
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

		protected void BuildContentTypes()
		{
			_dtContentType = Base.Classes.Assessment.GetContentTypes(_userID);
			cmbContentType.DataSource = _dtContentType;
			cmbContentType.DataBind();

			// Set text to null so that the "EmptyMessage" will show. In this case it is <Select One>.
			// Setting the current index to -1 does not seem to work.
			cmbContentType.Text = null;
			for (Int32 i = 0; i < _dtContentType.Rows.Count; i++)
			{
				if (String.Compare(_assessmentInfo.ContentType, (String)_dtContentType.Rows[i]["ContentType"], true) == 0)
				{
					cmbContentType.SelectedIndex = i;
					break;
				}
			}
		}

		protected void cmbContentType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.ContentType = e.Text;
			SetElementVisibility();
			SetOkState();
		}

		protected void BuildNumForms()
		{
			DataTable dtNumForms = new DataTable();
			DataColumn col = dtNumForms.Columns.Add("NumForms", typeof(String));
			DataRow row;
			for(Int32 i = 1; i <= 99; i++)
			{
				row = dtNumForms.NewRow();
				row[col] = i.ToString();
				dtNumForms.Rows.Add(row);
			}
			cmbNumForms.DataSource = dtNumForms;
			cmbNumForms.DataBind();

			cmbNumForms.Text = null;
			cmbNumForms.SelectedIndex = _assessmentInfo.NumForms - 1;
		}

		protected void cmbNumForms_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.NumForms = Int32.Parse(e.Text);
			SetOkState();
		}

		protected void BuildInclFieldTest()
		{
			_dtInclFieldTest = new DataTable();
			DataColumn col = _dtInclFieldTest.Columns.Add("InclFieldTest", typeof(String));
			DataRow row = _dtInclFieldTest.NewRow();
			row[col] = "Yes";
			_dtInclFieldTest.Rows.Add(row);
			row = _dtInclFieldTest.NewRow();
			row[col] = "No";
			_dtInclFieldTest.Rows.Add(row);
			cmbInclFieldTest.DataSource = _dtInclFieldTest;
			cmbInclFieldTest.DataBind();

			cmbInclFieldTest.Text = null;
			String currVal = _assessmentInfo.IncludeFieldTest ? "Yes" : "No";
			for (Int32 i = 0; i < _dtInclFieldTest.Rows.Count; i++)
			{
				if (String.Compare(currVal, (String)_dtInclFieldTest.Rows[i]["InclFieldTest"], true) == 0)
				{
					cmbInclFieldTest.SelectedIndex = i;
					break;
				}
			}
		}

		protected void cmbInclFieldTest_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.IncludeFieldTest = String.Compare(e.Text, "Yes", true) == 0;
			SetOkState();
		}

		protected void BuildNumDistractors()
		{
			_dtNumDistractors = Base.Classes.Assessment.GetNumberDistractors(_userID);
			cmbNumDistractors.DataSource = _dtNumDistractors;
			cmbNumDistractors.DataBind();

			cmbNumDistractors.Text = null;
			for (Int32 i = 0; i < _dtNumDistractors.Rows.Count; i++)
			{
				if (String.Compare(_assessmentInfo.NumDistractors.ToString(), _dtNumDistractors.Rows[i]["NumDistractors"].ToString(), true) == 0)
				{
					cmbNumDistractors.SelectedIndex = i;
					break;
				}
			}
			BuildDistractorLabels();
		}

		protected void cmbNumDistractors_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.NumDistractors = Int32.Parse(e.Text);
			_assessmentInfo.DistractorLabels = null;
			BuildDistractorLabels();
			SetOkState();
		}

		protected void BuildDistractorLabels()
		{
			_dtDistractorLabels = Base.Classes.Assessment.GetDistractorLabels(_assessmentInfo.NumDistractors, _userID);
			cmbDistractorLabels.DataSource = _dtDistractorLabels;
			cmbDistractorLabels.DataBind();

			cmbDistractorLabels.Text = null;
			for (Int32 i = 0; i < _dtDistractorLabels.Rows.Count; i++)
			{
				if (String.Compare(_assessmentInfo.DistractorLabels, (String)_dtDistractorLabels.Rows[i]["Value"], true) == 0)
				{
					cmbDistractorLabels.SelectedIndex = i;
					break;
				}
			}
		}

		protected void cmbDistractorLabels_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.DistractorLabels = e.Value;
			SetOkState();
		}

		protected void BuildPerformanceLevels()
		{
			_dtPerfLevels = Base.Classes.Assessment.GetPerformanceLevels(_assessmentID, _userID);
			cmbPerfLevels.DataSource = _dtPerfLevels;
			cmbPerfLevels.DataBind();

			cmbPerfLevels.Text = null;
			for (Int32 i = 0; i < _dtPerfLevels.Rows.Count; i++)
			{
				if (String.Compare(_assessmentInfo.PerformanceLevels, (String)_dtPerfLevels.Rows[i]["PerformanceLevel"], true) == 0)
				{
					cmbPerfLevels.SelectedIndex = i;
					break;
				}
			}
		}

		protected void cmbPerfLevels_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.PerformanceLevels = e.Text;
			SetOkState();
		}

		protected void BuildPrintColumns()
		{
			_dtPrintCols = new DataTable();
			DataColumn col = _dtPrintCols.Columns.Add("PrintColumn", typeof(String));
			DataRow row = _dtPrintCols.NewRow();
			row[col] = "1";
			_dtPrintCols.Rows.Add(row);
			row = _dtPrintCols.NewRow();
			row[col] = "2";
			_dtPrintCols.Rows.Add(row);

			cmbPrintCols.DataSource = _dtPrintCols;
			cmbPrintCols.DataBind();

			cmbPrintCols.Text = null;
			for (Int32 i = 0; i < _dtPrintCols.Rows.Count; i++)
			{
				if (String.Compare(_assessmentInfo.PrintColumns.ToString(), (String)_dtPrintCols.Rows[i]["PrintColumn"], true) == 0)
				{
					cmbPrintCols.SelectedIndex = i;
					break;
				}
			}
		}

		protected void cmbPrintCols_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.PrintColumns = Int32.Parse(e.Text);
			SetOkState();
		}

		protected void BuildPrintShortAnswer()
		{
			_dtPrintShortAns = new DataTable();
			DataColumn col = _dtPrintShortAns.Columns.Add("PrintShortAnswer", typeof(String));
			DataRow row = _dtPrintShortAns.NewRow();
			row[col] = "Yes";
			_dtPrintShortAns.Rows.Add(row);
			row = _dtPrintShortAns.NewRow();
			row[col] = "No";
			_dtPrintShortAns.Rows.Add(row);

			cmbPrintShortAns.DataSource = _dtPrintShortAns;
			cmbPrintShortAns.DataBind();

			cmbPrintShortAns.Text = null;
			String currVal = _assessmentInfo.PrintShortAnswer ? "Yes" : "No";
			for (Int32 i = 0; i < _dtPrintShortAns.Rows.Count; i++)
			{
				if (String.Compare(currVal, (String)_dtPrintShortAns.Rows[i]["PrintShortAnswer"], true) == 0)
				{
					cmbPrintShortAns.SelectedIndex = i;
					break;
				}
			}
		}

		protected void cmbPrintShortAns_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.PrintShortAnswer = String.Compare(e.Text, "Yes", true) == 0;
			SetOkState();
		}

		protected void BuildOnlineContentFormat()
		{
			_dtOnlineContFmt = Base.Classes.Assessment.GetOnlineContentFormat(_userID);
			cmbOnlineContFmt.DataSource = _dtOnlineContFmt;
			cmbOnlineContFmt.DataBind();

			cmbOnlineContFmt.Text = null;
			for (Int32 i = 0; i < _dtOnlineContFmt.Rows.Count; i++)
			{
				if (String.Compare(_assessmentInfo.OnlineContentFormat, (String)_dtOnlineContFmt.Rows[i]["OnlineContentFormat"], true) == 0)
				{
					cmbOnlineContFmt.SelectedIndex = i;
					break;
				}
			}
		}

		protected void cmbOnlineContFmt_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
		{
			_assessmentInfo.OnlineContentFormat = e.Value;
			SetOkState();
		}

		protected void tbxSource_TextChanged(object sender, EventArgs e)
		{
			_assessmentInfo.Source = tbxSource.Text;
		}


		protected void SetOkState()
		{
			Thinkgate.Base.Classes.Cache.Insert(_cacheKey, _assessmentInfo);
			okButton.Enabled = !String.IsNullOrEmpty(_assessmentInfo.DistractorLabels);
		}

		protected void okButton_Click(object sender, EventArgs e)
		{
			Thinkgate.Base.Classes.Assessment.SaveConfigurationInformation(_assessmentInfo, _assessmentID, _userID);
		}

		protected void cancelButton_Click(object sender, EventArgs e)
		{
		}
	}
}