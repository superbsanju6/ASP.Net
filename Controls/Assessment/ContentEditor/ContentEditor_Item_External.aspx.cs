using System;
using System.Globalization;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using Standpoint.Core.Utilities;
using System.Data;
using System.Web.Script.Serialization;
using Thinkgate.Base.Enums;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Assessment.ContentEditor.External
{
    public class AssessmentItemResponseWithItemID : AssessmentItemResponse
    {
        public int ItemID { get; set; }
        public string ItemType { get; set; }

        public AssessmentItemResponseWithItemID(int id, string distractorText, char letter)
            : base(id, distractorText, letter)
        {
        }
    }

    public partial class ContentEditor_Item : BasePage
    {
        public int _iItemID { get; set; }
        private string _sItemID;
        private string _sQuestionType;
        private int _iAssessmentID;
        private TestQuestion _itemTestQuestion;
        private BankQuestion _itemBankQUestion;
        
        protected bool useTestQuestion;
        
        private JavaScriptSerializer _serializer;

        //Advanced Item
        protected string _Dok;
        private DataTable _dtResults;
        public int Rbt;
        public int Webb;
        public int Marzano;
        public string ItemDiff;
        public decimal ItemPerc;

        public class StandardListForArray
        {
            public string EncID { get; set; }
            public string StandardName { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject) Session["SessionObject"];
            UpdateItem();
            LoadItem();
            BindPageControls();

            SetItemPageValues();

            LoadData();
            BindComboBoxes();

            /************************************************************
             * This page needs to behave slightly different depending on 
             * whether it is displaying for a Test Question or a Bank
             * Question.  This piece of logic handles that behavior.
             ***********************************************************/
            if (_sQuestionType == "TestQuestion")
            {
                comboRBT.Enabled = false;
                comboWebb.Enabled = false;
                comboMarzano.Enabled = false;

                switch (_Dok)
                {
                    case "Webb":
                        comboWebb.Enabled = true;
                        break;

                    case "Marzano":
                        comboMarzano.Enabled = true;
                        break;

                    case "RBT":
                        comboRBT.Enabled = true;
                        break;
                }
            }

            ScriptManager.RegisterStartupScript(this, typeof (string), "appClient", "var appClient = '" + AppClient() + "';", true);
            ScriptManager.RegisterStartupScript(this, typeof (string), "debug_message", "function debug_message(message) {}", true);
        }

        private void UpdateItem()
        {
            //#9006: The following code has been added to handle the item update in Mozilla Firefox browser, when 'cmbItemType' dropdown 
            //SelectedIndexChanged event is fired. 
            //This is needed becasue the Service methods are not getiting called from 'AssessmentItem_Change_Key_Value_Info'
            //and 'AssessmentItem_Change_ItemWeight' functions from 'Assessment_ChangeField_ItemType' function of 'ContentEditor_Item.js' file. 
            if (Request["__EVENTTARGET"] != null)
            {
                if (Request["__EVENTTARGET"].ToString(CultureInfo.CurrentCulture) == "cmbItemType")
                {
                    try
                    {
                        int itemID = Cryptography.GetDecryptedIDFromEncryptedValue(divEncItemID.InnerHtml, SessionObject.LoggedInUser.CipherKey);
                        int assessmentID = Cryptography.GetDecryptedIDFromEncryptedValue(divAssessmentID.InnerHtml, SessionObject.LoggedInUser.CipherKey);

                        if (assessmentID > 0)
                        {
                            TestQuestion.UpdateField(assessmentID, itemID, 0, cmbItemType.Attributes["field"].ToString(CultureInfo.CurrentCulture), cmbItemType.SelectedValue);
                            TestQuestion.UpdateField(assessmentID, itemID, 0, cmbScoreType.Attributes["field"].ToString(CultureInfo.CurrentCulture), "W");
                            TestQuestion.UpdateField(assessmentID, itemID, 0, cmbRubricType.Attributes["field"].ToString(CultureInfo.CurrentCulture), "B");
                            TestQuestion.UpdateField(assessmentID, itemID, 0, cmbRubricScoring.Attributes["field"].ToString(CultureInfo.CurrentCulture), "2");
                            TestQuestion.UpdateField(assessmentID, itemID, 0, "RubricID", "0");

                            TestQuestion.UpdateField(assessmentID, itemID, 0, "ItemWeight", "Normal");
                        }
                        else
                        {
                            SessionObject sessionObject = (SessionObject)Session["SessionObject"];

                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, cmbItemType.Attributes["field"].ToString(CultureInfo.CurrentCulture), cmbItemType.SelectedValue);
                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, cmbScoreType.Attributes["field"].ToString(CultureInfo.CurrentCulture), "W");
                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, cmbRubricType.Attributes["field"].ToString(CultureInfo.CurrentCulture), "B");
                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, cmbRubricScoring.Attributes["field"].ToString(CultureInfo.CurrentCulture), "2");
                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, "RubricID", "0");

                            BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, "ItemWeight", "Normal");
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Update Error", "MessageAlert('Error during update: " + ex.Message + "');", true);
                    }
                }
            }
        }

        private void LoadItem()
        {
            _sItemID = "" + Request.QueryString["xID"];
            _sQuestionType = "" + Request.QueryString["qType"];
            _iItemID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey);
            _iAssessmentID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "AssessmentID");
            lbl_TestCategory.Value = Request.QueryString["TestCategory"];

            if (_sItemID == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                if (_sQuestionType.Equals("TestQuestion"))
                {
                    // Remove the copy item button if we are coming from the item editor.
                    if (_iItemID == 0 && _iAssessmentID > 0 && !String.IsNullOrEmpty(_sItemID))
                    {
                        _itemTestQuestion = TestQuestion.GetTestQuestionByID(DataIntegrity.ConvertToInt(_sItemID), false);
                        _iItemID = _itemTestQuestion.ID;
                        if (_itemTestQuestion.TestID != _iAssessmentID)
                        {
                            SessionObject.RedirectMessage = "Invalid entity ID provided in URL.";
                            Response.Redirect("~/PortalSelection.aspx", true);
                        }
                    }
                    else
                    {
                        _itemTestQuestion = TestQuestion.GetTestQuestionByID(_iItemID, false);
                    }
                    useTestQuestion = true;
                    //_itemTestQuestion.LoadItemAndScoreTypes();
                    //_itemTestQuestion.LoadRubric();
                }
                else
                {
                    _itemBankQUestion = BankQuestion.GetQuestionByID(_iItemID);
                    _itemBankQUestion.LoadAddendum();
                    useTestQuestion = false;
                }

                LoadStandards();
                LoadItemText();

            }

        }

        private void LoadItemText()
        {
            rptrItemQuestions.DataSource = useTestQuestion ? _itemTestQuestion.Responses : _itemBankQUestion.Responses;
            rptrItemQuestions.DataBind();
        }

        private void LoadStandards()
        {
            List<Base.Classes.Standards> lStandards = new List<Base.Classes.Standards>();
            if (useTestQuestion)
            {
                Base.Classes.Standards itemStandards = Base.Classes.Standards.GetStandardByID(_itemTestQuestion.StandardID);
                if (itemStandards != null)
                {
                    lStandards.Add(itemStandards);
                }
            }
            else
            {
                lStandards = _itemBankQUestion.Standards;
            }
            LoadStandardsArray(lStandards);
        }

        protected void LoadStandardsArray(List<Base.Classes.Standards> lStandards)
        {
            List<StandardListForArray> std = new List<StandardListForArray>();

            foreach (var stds in lStandards)
            {
                StandardListForArray s = new StandardListForArray();
                s.EncID = Standpoint.Core.Classes.Encryption.EncryptString(stds.ID.ToString(CultureInfo.CurrentCulture));
                s.StandardName = stds.StandardName;
                std.Add(s);
            }
            if (std.Count == 1)
            {
                lblStandardNamePanel.Attributes.Add("onclick", "stopPropagation(event); OpenStandardText('" + std[0].EncID + "')");
                imgRemoveStandard.Attributes.Add("onclick", "stopPropagation(event); RemoveStandardfromItem('" + std[0].EncID + "')");
                lblStandardNamePanel.Text = lStandards[0].StandardName;

                if (useTestQuestion)
                    imgRemoveStandard.Style.Add("display", "none");
            }
            if (std.Count > 1)
            {
                rptrStandards.DataSource = std;
                rptrStandards.DataBind();
                lblStandardNamePanel.Text = "Standards listed below...";
            }
            if (std.Count != 1)
            {
                imgRemoveStandard.Style.Add("display", "none");
            }
            if (_serializer == null) _serializer = new JavaScriptSerializer();
            ScriptManager.RegisterStartupScript(this, typeof (string), "ItemStandardsArray", "var ItemStandardsArray=" + _serializer.Serialize(std) + "; ", true);
        }

        private void SetItemPageValues()
        {
            divEncItemID.InnerHtml = Standpoint.Core.Classes.Encryption.EncryptString(useTestQuestion ? _itemTestQuestion.ID.ToString(CultureInfo.CurrentCulture) : _itemBankQUestion.ID.ToString(CultureInfo.CurrentCulture));
            divItemID.InnerHtml = useTestQuestion ? _itemTestQuestion.ID.ToString(CultureInfo.CurrentCulture) : _itemBankQUestion.ID.ToString(CultureInfo.CurrentCulture);
            if (!_sQuestionType.Equals(""))
            {
                divAssessmentID.InnerHtml = Standpoint.Core.Classes.Encryption.EncryptString(useTestQuestion ? _iAssessmentID.ToString(CultureInfo.CurrentCulture) : "");
            }
            divTestQ.InnerHtml = Standpoint.Core.Classes.Encryption.EncryptString(useTestQuestion ? "true" : "false");

            
            ItemContentTile_hdnThumbnailURL.Value = @"display.asp?formatoption=search results&key=9120&retrievemode=searchpage&ahaveSkipped=normal&??Question=" +
                                                    (useTestQuestion ? _itemTestQuestion.ID : _itemBankQUestion.ID) + @"&??TestID=" + (useTestQuestion ? "-1" : "0") + @"&qreview=y'";

         }

        private void BindPageControls()
        {
            LoadItemType();
            //ShowHideCorrectAnswerText();
        }

        //private void ShowHideCorrectAnswerText()
        //{
        //    QuestionTypes questionTypes = (QuestionTypes)Enum.Parse(typeof(QuestionTypes), cmbItemType.SelectedValue.ToString());

        //    if (questionTypes == QuestionTypes.S || questionTypes == QuestionTypes.E)
        //    { divCorrectAnswer.Style.Add("display", "none"); }
        //    else
        //    { divCorrectAnswer.Style.Add("display", "inline"); }
        //}

        private void LoadItemType()
        {
            if (!IsPostBack)
            {
                RadComboBoxItem newitem = new RadComboBoxItem();
                newitem.Text = "Multiple Choice (3 Distractors)";
                newitem.Value = QuestionTypes.MC3.ToString();
                if (newitem.Value == (useTestQuestion ? _itemTestQuestion.QuestionType.ToString() : _itemBankQUestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                newitem = new RadComboBoxItem();
                newitem.Text = "Multiple Choice (4 Distractors)";
                newitem.Value = QuestionTypes.MC4.ToString();
                if (newitem.Value == (useTestQuestion ? _itemTestQuestion.QuestionType.ToString() : _itemBankQUestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                newitem = new RadComboBoxItem();
                newitem.Text = "Multiple Choice (5 Distractors)";
                newitem.Value = QuestionTypes.MC5.ToString();
                if (newitem.Value == (useTestQuestion ? _itemTestQuestion.QuestionType.ToString() : _itemBankQUestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                newitem = new RadComboBoxItem();
                newitem.Text = "True/False";
                newitem.Value = QuestionTypes.T.ToString();
                if (newitem.Value == (useTestQuestion ? _itemTestQuestion.QuestionType.ToString() : _itemBankQUestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                newitem = new RadComboBoxItem();
                newitem.Text = "Short Answer";
                newitem.Value = QuestionTypes.S.ToString();
                if (newitem.Value == (useTestQuestion ? _itemTestQuestion.QuestionType.ToString() : _itemBankQUestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);

                newitem = new RadComboBoxItem();
                newitem.Text = "Essay";
                newitem.Value = QuestionTypes.E.ToString();
                if (newitem.Value == (useTestQuestion ? _itemTestQuestion.QuestionType.ToString() : _itemBankQUestion.QuestionType.ToString()))
                    newitem.Selected = true;
                cmbItemType.Items.Add(newitem);
            }

            LoadScoreType();

        }

        private void LoadScoreType()
        {
            cmbScoreType.Items.Clear();

            RadComboBoxItem newitem = new RadComboBoxItem();
            newitem.Text = "Correct/Incorrect";
            newitem.Value = "W";
            newitem.Attributes["ItemTypes"] = "TMSE";
            cmbScoreType.Items.Add(newitem);

            newitem = new RadComboBoxItem();
            newitem.Text = "Partial Credit";
            newitem.Value = "P";
            newitem.Attributes["ItemTypes"] = "SE";

            cmbScoreType.Items.Add(newitem);

            newitem = new RadComboBoxItem();
            newitem.Text = "Rubric";
            newitem.Value = "R";
            newitem.Attributes["ItemTypes"] = "SE";
            cmbScoreType.Items.Add(newitem);

            //Decide which items should be visible based on
            //cmbItemType and which one is selected.
            foreach (RadComboBoxItem scoreTypeItem in cmbScoreType.Items)
            {
                scoreTypeItem.Visible = scoreTypeItem.Attributes["ItemTypes"].ToString(CultureInfo.CurrentCulture).Contains(cmbItemType.SelectedValue);
                scoreTypeItem.Selected = (scoreTypeItem.Value == (useTestQuestion ? _itemTestQuestion.ScoreType : _itemBankQUestion.ScoreType));
            }
            LoadRubricTypes();
        }

        private void LoadRubricTypes()
        {
            DataTable dtRubricTypes = Rubric.RubricTypesSelect();
            cmbRubricType.DataTextField = "Rubrictype";
            cmbRubricType.DataValueField = "Rubricval";
            cmbRubricType.DataSource = dtRubricTypes;
            cmbRubricType.DataBind();
            RadComboBoxItem someitem;

            if (cmbScoreType.SelectedValue == "R")
            {
                divRubricTypeLabel.Style["display"] = "inline";

                if (useTestQuestion)
                    someitem = cmbRubricType.FindItemByValue((_itemTestQuestion.RubricType));
                else
                    someitem = cmbRubricType.FindItemByValue((_itemBankQUestion.RubricType));

                if (someitem != null)
                    someitem.Selected = true;
            }
            else
            {
                //rubric types control should be hidden now, but set it up
                //to default value so if the user's actions cause it to
                //appear, then it will show the default value
                divRubricTypeLabel.Style["display"] = "none";
                someitem = cmbRubricType.FindItemByValue("B");
                if (someitem != null) someitem.Selected = true;
            }

            //Since the value of this dropdownlist effects cmbRubricScoring,
            //refresh that control as well.
            LoadRubricScoreTypes();
        }

        private void LoadRubricScoreTypes()
        {
            DataTable dtRubricTypes = Rubric.RubricScoreTypesSelect();
            cmbRubricScoring.DataTextField = "RubricScoreText";
            cmbRubricScoring.DataValueField = "RubricPoints";
            cmbRubricScoring.DataSource = dtRubricTypes;
            cmbRubricScoring.DataBind();

            //Add "Custom" option to the dropdownlist
            RadComboBoxItem newItem = new RadComboBoxItem();
            newItem = new RadComboBoxItem();
            newItem.Text = "Custom";
            newItem.Value = "0";
            cmbRubricScoring.Items.Add(newItem);


            //Decide whether cmbRubricScore should be visible
            if (divRubricTypeLabel.Style["display"] == "inline" && cmbRubricType.SelectedValue == "B")
            {
                divRubricScoringLabel.Style["display"] = "inline";

                //if the item's rubric is currently null, then display a default 2-pt rubric
                if (useTestQuestion)
                    newItem = cmbRubricScoring.FindItemByValue(_itemTestQuestion.RubricPoints.ToString(CultureInfo.CurrentCulture));
                else
                    newItem = cmbRubricScoring.FindItemByValue(_itemBankQUestion.RubricPoints.ToString(CultureInfo.CurrentCulture));
                if (newItem != null)
                    newItem.Selected = true;
            }
            else
            {
                //rubric Scoring control should be hidden now, but set it up
                //to default value so if the user's actions cause it to
                //appear, then it will show the default value
                divRubricScoringLabel.Style["display"] = "none";
                newItem = cmbRubricScoring.FindItemByValue("2");
                if (newItem != null) newItem.Selected = true;
            }

            //Since the value of this dropdownlist effects pnlItem_Rubric,
            //refresh that control as well.
            LoadPnlItem_Rubric();
        }
        private void BindComboBoxes()
        {
            comboRBT.DataTextField = "Text";
            comboRBT.DataValueField = "Code";
            comboRBT.DataSource = Rigor.LoadRigorListByDOK("RBT");
            comboRBT.DataBind();

            RadComboBoxItem someitemRbt = comboRBT.FindItemByValue(Rbt.ToString(CultureInfo.CurrentCulture));
            if (someitemRbt != null) someitemRbt.Selected = true;

            comboMarzano.DataTextField = "Text";
            comboMarzano.DataValueField = "Code";
            comboMarzano.DataSource = Rigor.LoadRigorListByDOK("Marzano");
            comboMarzano.DataBind();

            RadComboBoxItem someitemMarzano = comboMarzano.FindItemByValue(Marzano.ToString(CultureInfo.CurrentCulture));
            if (someitemMarzano != null) someitemMarzano.Selected = true;

            comboWebb.DataTextField = "Text";
            comboWebb.DataValueField = "Code";
            comboWebb.DataSource = Rigor.LoadRigorListByDOK("Webb");
            comboWebb.DataBind();

            RadComboBoxItem someitemWebb = comboWebb.FindItemByValue(Webb.ToString(CultureInfo.CurrentCulture));
            if (someitemWebb != null) someitemWebb.Selected = true;

            comboDifficultyIndex.DataTextField = "Value";
            comboDifficultyIndex.DataValueField = "Key";
            comboDifficultyIndex.DataSource = DifficultyIndex.PossibleValueListDictionary();
            comboDifficultyIndex.DataBind();
            RadComboBoxItem someitemDifficultyIndex = comboDifficultyIndex.FindItemByValue(ItemDiff);
            if (someitemDifficultyIndex != null) someitemDifficultyIndex.Selected = true;
        }

        private void LoadData()
        {

            _dtResults = new DataTable();
            if (_iItemID == 0)
            {

            }
            else
            {
                if (_sQuestionType.Equals("TestQuestion"))
                    _dtResults = TestQuestion.GetAdvancedItemInfo(_iItemID);
                else
                    _dtResults = BankQuestion.GetAdvancedItemInfo(_iItemID);

                foreach (DataRow r in _dtResults.Rows)
                {
                    Rbt = DataIntegrity.ConvertToInt(r["RBT"]);
                    Webb = DataIntegrity.ConvertToInt(r["Webb"]);
                    Marzano = DataIntegrity.ConvertToInt(r["Marzano"]);
                    ItemDiff = r["DifficultyIndex"].ToString();
                    ItemPerc = DataIntegrity.ConvertToDecimal(r["DifficultyParameter"]);
                }
            }

            var dparms = DistrictParms.LoadDistrictParms();
            _Dok = dparms.DOK;

        }

        private void LoadPnlItem_Rubric()
        {
            //Decide whether to display the rubric panel or not
            //should display if rubric present, 

            if (divRubricTypeLabel.Style["display"] == "none")
            {
                //Item's scoretype is not rubric, so hide the rubric panel
                pnlItem_Rubric.Style["display"] = "none";

                //custom rubric panel should be hidden now, but set it up
                //to default value so if the user's actions cause it to
                //appear, then it will show the default value
                trRubric_No.Style.Add("display", "inline");
                trRubric_Yes.Style.Add("display", "none");
            }
            else if (

				//The following two conditions represent custom rubrics.
                (divRubricScoringLabel.Style["display"] == "inline" && cmbRubricScoring.SelectedValue == "0")
                ||
                (divRubricTypeLabel.Style["display"] == "inline" && divRubricScoringLabel.Style["display"] == "none")
                )
            {
                //Item's score type is rubric and rubric is custom, so display rubric panel.
                pnlItem_Rubric.Style["display"] = "block";


                //Decide what within the Rubric Panel should show.    
                if ((useTestQuestion ? _itemTestQuestion.RubricID : _itemBankQUestion.RubricID) == 0)
                {
                    //User has not imported a rubric yet, so display the rubric panel to allow user to import a rubric from the rubric search screen.
                    trRubric_No.Style.Add("display", "inline");
                    trRubric_Yes.Style.Add("display", "none");
				}
                else
                {
                    //User has already imported a rubric so display the rubric panel to allow user to delete the imported rubric so they can import another.
					if (useTestQuestion) 
					{
						lblRubricName.Text = _itemTestQuestion.Rubric.Name;
						lblRubricScoring.Text = _itemTestQuestion.Rubric.Scoring;
						lblRubricType.Text = _itemTestQuestion.Rubric.TypeDesc;
						if (_itemTestQuestion.RubricType == "A")
						{
							lblRubricType.Text += " " + _itemTestQuestion.Rubric.CriteriaCount.ToString() + " x " + _itemTestQuestion.Rubric.MaxPoints.ToString();
							ContentEditor_Item_divRubricPanelRubricScoring.Style["display"] = "none";
						}
						else
						{
							ContentEditor_Item_divRubricPanelRubricScoring.Style["display"] = "inline";
						}
						ContentEditor_Item_hdnRubricContent.InnerHtml = _itemTestQuestion.Rubric.Content;
					}
					else 
					{
						lblRubricName.Text = _itemBankQUestion.Rubric.Name;
						lblRubricScoring.Text = _itemBankQUestion.Rubric.Scoring;
						lblRubricType.Text = _itemBankQUestion.Rubric.TypeDesc;
						if (_itemBankQUestion.RubricType == "A")
						{
							lblRubricType.Text += " " + _itemBankQUestion.Rubric.CriteriaCount.ToString() + " x " + _itemBankQUestion.Rubric.MaxPoints.ToString() + " Points";
							ContentEditor_Item_divRubricPanelRubricScoring.Style["display"] = "none";
						}
						else
						{
							ContentEditor_Item_divRubricPanelRubricScoring.Style["display"] = "inline";
						}
						ContentEditor_Item_hdnRubricContent.InnerHtml = _itemBankQUestion.Rubric.Content;
						ContentEditor_Item_hdnRubricID.Value = _itemBankQUestion.RubricID.ToString();
					}
                    ContentEditor_Item_imgRemoveRubric.Style["display"] = "inline";
                    trRubric_Yes.Style.Add("display", "inline");
                    trRubric_No.Style.Add("display", "none");
                }
            }

            else
            {
                pnlItem_Rubric.Style["display"] = "block";

                //Item's rubric is a default rubric.  Only show the rubric info and a hyperlink in the rubric 
                //panel - no upload, add new, or remove functionality because item does not specify a custom 
                //rubric.
                lblRubricName.Text = (useTestQuestion ? _itemTestQuestion.Rubric.Name : _itemBankQUestion.Rubric.Name);
                lblRubricScoring.Text = (useTestQuestion ? _itemTestQuestion.Rubric.Scoring : _itemBankQUestion.Rubric.Scoring);
                lblRubricType.Text = (useTestQuestion ? _itemTestQuestion.Rubric.TypeDesc : _itemBankQUestion.Rubric.TypeDesc);

                ContentEditor_Item_imgRemoveRubric.Style["display"] = "none";
                ContentEditor_Item_hdnRubricID.Value = (useTestQuestion ? -_itemTestQuestion.RubricPoints : -_itemBankQUestion.RubricPoints).ToString();
                
                trRubric_Yes.Style.Add("display", "block");
                trRubric_No.Style.Add("display", "none");
            }
        }
    }
}
