using System;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentPrint : BasePage
	{
		const Int32 cbxUnchecked = 0;
		const Int32 cbxFilled = 1;
		const Int32 cbxChecked = 2;
		Int32 _userID;
		public const String numAssessmentCol = "NumAssessmentCopies", numAnswerKeyCol = "NumAnswerKeyCopies", numRubricsCol = "NumRubricsCopies";
		public const String assessmentEnabledCol = "AssessmentEnabled", answerKeyEnabledCol = "AnswerKeyEnabled", rubricsEnabledCol = "RubricsEnabled",instructionsEnabledCol = "InstructionsEnabled";
		SessionObject sessionObject;
        private bool printHTML = false;
        private bool printWord = false;
       public Boolean hasRubric = false;

		protected void Page_Load(object sender, EventArgs e)
		{
			sessionObject = (SessionObject)Session["SessionObject"];
			_userID = sessionObject.LoggedInUser.Page;
			
			if (Request.QueryString["xID"] == null)
			{
				sessionObject.RedirectMessage = "No assessment ID provided in URL.";
				Response.Redirect("~/PortalSelection.aspx", true);
			}
			else
			{
                AssessmentID = GetDecryptedEntityId(X_ID);
								
				if(!String.IsNullOrEmpty(Request.QueryString["yID"]))
					this.Title = "Print: " + Request.QueryString["yID"];

				// See if there is a print option passed.
				if(!String.IsNullOrEmpty(Request.QueryString["zID"]))
					PrintOption = Request.QueryString["zID"];

                if (!String.IsNullOrEmpty(Request.QueryString["PrintHTML"]))
                    printHTML = true;

                if (!String.IsNullOrEmpty(Request.QueryString["PrintWord"]))
                    printWord = true;

                

                if (printHTML || printWord)
                {
                    Base.Classes.Assessment _selectedAssessment = Base.Classes.Assessment.GetAssessmentByID(AssessmentID);
                    string isProofed = _selectedAssessment.IsProofed ? "Yes" : "No";
                    if (printHTML)
                    {              
						Response.Redirect(ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + System.Web.HttpUtility.UrlEncode("display.asp?key=7020&formatoption=search results&retrievemode=searchpage&??QPAGE=0&??STEID=0&??STELIST=&??CLASSTESTEVENTID=0&??ANSWER SEQUENCE=0&??HighlightCorrect=Yes&??BTP=" + AssessmentID.ToString() + "&f=1col&??FormID=ALL&??Proof=" + isProofed));
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + System.Web.HttpUtility.UrlEncode("display.asp?key=7114&formatoption=search%20results&retrievemode=searchpage&??QPAGE=0&??STEID=0&??STELIST=&??CLASSTESTEVENTID=0&??HighlightCorrect=Yes&??ANSWER%20SEQUENCE=0&??BTP=" + AssessmentID.ToString() + "&??outformat=word&??FormID=ALL&??Proof=" + isProofed));
                    }
                }
                else
                {
                    if (!IsPostBack)
                    {
                        BuildPrintData();

                        Base.Classes.Assessment _selectedAssessment = Base.Classes.Assessment.GetAssessmentByID(AssessmentID);
                        inpContentType.Value = _selectedAssessment.ContentType.Trim().ToUpper();                        
                    }
                    else
                    {                        
                        CheckTextBoxChange();
                    }
                }

                if (Request.Form["__EVENTTARGET"] == "cbxRubrics")
                {
                    cbxRubrics.SelectedToggleStateIndex = int.Parse(Request.Form["__EVENTARGUMENT"].ToString());
                    //Fire event
                    cbxRubrics_Click(this, new EventArgs());                    
                }
			}
		}


		/// <summary>
		/// The assessment id property.
		/// </summary>
		protected Int32 AssessmentID
		{
			get { return Int32.Parse(inpAssessmentID.Value); }
			set { inpAssessmentID.Value = value.ToString(); }
		}

		/// <summary>
		/// Print option may be empty for default, 'assmt', or 'upload'.
		/// </summary>
		protected String PrintOption
		{
			get { return inpPrintOption.Value; }
			set { inpPrintOption.Value = value; }
		}

		/// <summary>
		/// Get the information we need for printing.
		/// </summary>
		protected void BuildPrintData()
		{
			DataSet ds = Thinkgate.Base.Classes.Assessment.GetPrintData(_userID, AssessmentID, PrintOption);
			DataTable dtPrintInfo = ds.Tables[0];
			// Add columns for the number of copies.
			dtPrintInfo.Columns.Add(numAssessmentCol, typeof(Int32));
			dtPrintInfo.Columns.Add(numAnswerKeyCol, typeof(Int32));
			dtPrintInfo.Columns.Add(numRubricsCol, typeof(Int32));
			dtPrintInfo.Columns.Add(assessmentEnabledCol, typeof(Boolean));
			dtPrintInfo.Columns.Add(answerKeyEnabledCol, typeof(Boolean));
			dtPrintInfo.Columns.Add(rubricsEnabledCol, typeof(Boolean));
           
			foreach (DataRow row in dtPrintInfo.Rows)
			{
				row[numAssessmentCol] = row[numAnswerKeyCol] = row[numRubricsCol] = 0;
				row[assessmentEnabledCol] = String.Compare(row["Assessment"].ToString(), "Disabled", true) != 0;
				row[answerKeyEnabledCol] = String.Compare(row["AnswerKey"].ToString(), "Disabled", true) != 0;
				row[rubricsEnabledCol] = true; // For now rubrics are never disabled.
                
			}

			ViewState["dtPrintInfo"] = dtPrintInfo;

			String assessmentType;
			Boolean hasUpload,hasInstructions;
            Thinkgate.Base.Classes.Assessment.GetPrintOptions(ds, out assessmentType, out hasRubric, out hasUpload, out hasInstructions);
			ViewState["assessmentType"] = assessmentType;
			ViewState["hasRubric"] = hasRubric;
			ViewState["hasUpload"] = hasUpload;
            ViewState["hasInstructions"] = hasInstructions;

			// Set the assessment column label.
			lblAssessment.Text = (dtPrintInfo.Rows.Count == 0 || String.Compare(dtPrintInfo.Rows[0]["Reviewer"].ToString(), "No Print", true) == 0) ? "Assessment" : "Reviewer Assessment";
			RefreshBinding(dtPrintInfo);
		}

		/// <summary>
		/// Refreshes all data bindings and sets some visibilities.
		/// </summary>
		/// <param name="dtPrintInfo"></param>
		protected void RefreshBinding(DataTable dtPrintInfo)
		{
			rptForms.DataSource = dtPrintInfo;
			rptForms.DataBind();
			lblTotalAssessment.DataBind();
			lblTotalAnswerKey.DataBind();
			lblTotalRubric.DataBind();

			// Set visibility.
			String assessmentType = (String)ViewState["assessmentType"];
			Base.Enums.Permission assessmentPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_Assessment" + assessmentType, true);
			Base.Enums.Permission answerKeyPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_AnswerKey" + assessmentType, true);
			Base.Enums.Permission rubricsPerm = (Base.Enums.Permission)Enum.Parse(typeof(Base.Enums.Permission), "Print_Rubrics" + assessmentType, true);          

			inpShowAssessment.Value = UserHasPermission(assessmentPerm) && dtPrintInfo.Rows.Count > 0 &&
																(String.Compare(dtPrintInfo.Rows[0]["Assessment"].ToString(), "No Print", true) != 0 ||
																 String.Compare(dtPrintInfo.Rows[0]["Reviewer"].ToString(), "No Print", true) != 0) ? "true" : "false";
			inpShowAnswerKey.Value = UserHasPermission(answerKeyPerm) && dtPrintInfo.Rows.Count > 0 &&
															 String.Compare(dtPrintInfo.Rows[0]["AnswerKey"].ToString(), "No Print", true) != 0 ? "true" : "false";
			inpShowRubrics.Value = (dtPrintInfo.Rows.Count > 0 && (Boolean)ViewState["hasRubric"] &&
														 UserHasPermission(rubricsPerm)) ? "true" : "false";
            inpShowInstructions.Value = (dtPrintInfo.Rows.Count > 0 && (Boolean)ViewState["hasInstructions"]) ? "true" : "false";

            Boolean isAnyAssessmentDisabled = false, isAnyAnswerKeyDisabled = false, isAnyRubricDisabled = false;
			foreach(DataRow row in dtPrintInfo.Rows)
			{
				if(!(Boolean)row[assessmentEnabledCol])
					isAnyAssessmentDisabled = true;
				if (!(Boolean)row[answerKeyEnabledCol])
					isAnyAnswerKeyDisabled = true;
				if (!(Boolean)row[rubricsEnabledCol])
					isAnyRubricDisabled = true;               
			}
			imgInfoAssessment.Visible = isAnyAssessmentDisabled;
			imgInfoAnswerKey.Visible = isAnyAnswerKeyDisabled;
			imgInfoRubrics.Visible = isAnyRubricDisabled;
            imgInfoInstructions.Enabled = ViewState["hasInstructions"]!=null? (Boolean)ViewState["hasInstructions"]:false;
            cbxInstructions.Enabled = ViewState["hasInstructions"] != null ? (Boolean)ViewState["hasInstructions"] : false;

			// Finally set the print button to disabled if no copies to print.
			/*Boolean printEnabled = false;
			foreach(DataRow r in dtPrintInfo.Rows)
			{
				if((Int32)r[numAssessmentCol] > 0 || (Int32)r[numAnswerKeyCol] > 0 || (Int32)r[numRubricsCol] > 0)
				{
					printEnabled = true;
					break;
				}
			}
			btnPrint.Enabled = printEnabled;
						*/
				
				//var serializer = new JavaScriptSerializer();
				//ScriptManager.RegisterStartupScript(this, typeof(string), "printInfo", serializer.Serialize(dtPrintInfo), true);
						//ScriptManager.RegisterStartupScript(this, typeof(string), "printInfo", "var PrintInfo = 'test';", true);
		}

		/// <summary>
		/// Print the assessment.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnPrint_Click(object sender, EventArgs e)
		{
			DataTable dtPrintInfo = (DataTable)ViewState["dtPrintInfo"];
#if false // TESTING ONLY
			string systemImagePath = Server.MapPath((Request.ApplicationPath.Equals("/") ? "" : Request.ApplicationPath) + "/Images/");
			Thinkgate.Base.Classes.Assessment.RenderToPdfDoc(AssessmentID, systemImagePath);
#endif
		}

		protected void cbxAssessment_Click(object sender, EventArgs e)
		{
			Int32 stateIdx = cbxAssessment.SelectedToggleStateIndex;
			// Go to checked state if filled.
			if(stateIdx == cbxFilled)
				cbxAssessment.SelectedToggleStateIndex = stateIdx = cbxChecked;

			DataTable dtPrintInfo = (DataTable)ViewState["dtPrintInfo"];
			foreach(DataRow row in dtPrintInfo.Rows)
				row[numAssessmentCol] = (stateIdx == cbxUnchecked) || !(Boolean)row[assessmentEnabledCol] ? 0 : 1;
			RefreshBinding(dtPrintInfo);
		}

		protected void cbxAnswerKey_Click(object sender, EventArgs e)
		{
			Int32 stateIdx = cbxAnswerKey.SelectedToggleStateIndex;
			// Go to checked state if filled.
			if (stateIdx == cbxFilled)
				cbxAnswerKey.SelectedToggleStateIndex = stateIdx = cbxChecked;

			DataTable dtPrintInfo = (DataTable)ViewState["dtPrintInfo"];
			foreach (DataRow row in dtPrintInfo.Rows)
				row[numAnswerKeyCol] = (stateIdx == cbxUnchecked) || !(Boolean)row[answerKeyEnabledCol] ? 0 : 1;
			RefreshBinding(dtPrintInfo);
		}

		protected void cbxRubrics_Click(object sender, EventArgs e)
		{
			Int32 stateIdx = cbxRubrics.SelectedToggleStateIndex;
            //// Go to checked state if filled.
            //if (stateIdx == cbxFilled)
            //    cbxRubrics.SelectedToggleStateIndex = stateIdx = cbxChecked; 
             

			DataTable dtPrintInfo = (DataTable)ViewState["dtPrintInfo"];
			foreach (DataRow row in dtPrintInfo.Rows)
				row[numRubricsCol] = (stateIdx == cbxUnchecked) || !(Boolean)row[rubricsEnabledCol] ? 0 : 1;
			RefreshBinding(dtPrintInfo);
		}
        protected void cbxInstructions_Click(object sender, EventArgs e)
		{
            Int32 stateIdx = cbxInstructions.SelectedToggleStateIndex;
			// Go to checked state if filled.
			if (stateIdx == cbxFilled)
                cbxInstructions.SelectedToggleStateIndex = stateIdx = cbxChecked;

			DataTable dtPrintInfo = (DataTable)ViewState["dtPrintInfo"];           
			RefreshBinding(dtPrintInfo);
		}

        

		/// <summary>
		/// See if a text box has changed and handle any changes.
		/// </summary>
		protected void CheckTextBoxChange()
		{
			String postBackControlID = GetControlThatCausedPostBack(this);
			// Not a text box.
			if(!postBackControlID.StartsWith("tbx"))
				return;

			// Must be one of our numeric text boxes.
			DataTable dtPrintInfo = (DataTable)ViewState["dtPrintInfo"];
			if(String.Compare(postBackControlID, "tbxAssessment", true) == 0)
				cbxAssessment.SelectedToggleStateIndex = CheckNumericColumn(numAssessmentCol, assessmentEnabledCol, postBackControlID, dtPrintInfo);
			else if (String.Compare(postBackControlID, "tbxAnswerKey", true) == 0)
				cbxAnswerKey.SelectedToggleStateIndex = CheckNumericColumn(numAnswerKeyCol, answerKeyEnabledCol, postBackControlID, dtPrintInfo);
			else if (String.Compare(postBackControlID, "tbxRubrics", true) == 0)
				cbxRubrics.SelectedToggleStateIndex = CheckNumericColumn(numRubricsCol, rubricsEnabledCol, postBackControlID, dtPrintInfo);

			RefreshBinding(dtPrintInfo);
		}

		/// <summary>
		/// Check a numeric column for range and bogus characters.
		/// Return the state of the checkbox that heads the display column.
		/// </summary>
		/// <param name="colName"></param>
		/// <returns></returns>
		protected Int32 CheckNumericColumn(String colName, String enabledColName, String tbxName, DataTable dtPrintInfo)
		{
			foreach(RepeaterItem item in rptForms.Items)
			{
				TextBox tbx = item.FindControl(tbxName) as TextBox;
				if(tbx != null)
				{
					Int32 idx = item.ItemIndex;
					String txt = tbx.Text;
					Int32 value;
					if(!Int32.TryParse(txt, out value) || value < 0 || value > 99)
						value = 0;
					dtPrintInfo.Rows[idx][colName] = value;
				}
			}

			// Determine the checkbox state. Unchecked if all values are 0,
			// Checked if all values are 1, Filled otherwise.
			Boolean isAllZero = true, isAllOne = true;
			foreach(DataRow row in dtPrintInfo.Rows)
			{
				Int32 n = (Int32)row[colName];
				Boolean e = (Boolean)row[enabledColName];
				if(n != 0 && e)
					isAllZero = false;
				if(n != 1 && e)
					isAllOne = false;
			}

            if (colName == numRubricsCol)
            {
                return (isAllZero) ? cbxUnchecked : cbxFilled;
            }
            else
            {
                return (isAllZero) ? cbxUnchecked : (isAllOne) ? cbxChecked : cbxFilled;
            }
			
		}

		public Int32 ColumnTotal(String colName)
		{
			DataTable dtPrintInfo = (DataTable)ViewState["dtPrintInfo"];
			Int32 n = 0;
			foreach(DataRow row in dtPrintInfo.Rows)
				n += (Int32)row[colName];
			return n;
		}

		protected string GetControlThatCausedPostBack(System.Web.UI.Page page)
		{
			Control control = null;

			string ctrlname = page.Request.Params["__EVENTTARGET"];
			if (!string.IsNullOrEmpty(ctrlname))
			{
				control = page.FindControl(ctrlname);
			}
			// if __EVENTTARGET is null, the control is a button type and we need to
			// iterate over the form collection to find it
			else
			{
				string ctrlStr = String.Empty;
				Control c = null;
				foreach (string ctl in page.Request.Form)
				{
					//handle ImageButton they having an additional "quasi-property" in their Id which identifies
					//mouse x and y coordinates
					if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
					{
						ctrlStr = ctl.Substring(0, ctl.Length - 2);
						c = page.FindControl(ctrlStr);
					}
					else
					{
						c = page.FindControl(ctl);
					}
					if (c is System.Web.UI.WebControls.Button ||
													 c is System.Web.UI.WebControls.ImageButton)
					{
						control = c;
						break;
					}
				}
			}
			return (control != null) ? control.ID : null;
		}

	}
}