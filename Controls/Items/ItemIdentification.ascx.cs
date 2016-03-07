using System;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.Items
{
    public partial class ItemIdentification : TileControlBase
    {
        private Type _questionType;
        private QuestionBase _oQuestionBase;
        private BankQuestion _oBankQuestion;
        private TestQuestion _oTestQuestion;       

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (Tile.TileParms.GetParm("item") == null) return;

            _questionType = Tile.TileParms.GetParm("item").GetType();
            if (_questionType.Name == "BankQuestion")
            {
                _oBankQuestion = (BankQuestion) Tile.TileParms.GetParm("item");
                _oQuestionBase = (QuestionBase) _oBankQuestion;
            }
            else
            {
                _oTestQuestion = (TestQuestion) Tile.TileParms.GetParm("item");
                _oQuestionBase = (QuestionBase) _oTestQuestion;
            }

            lblGrade.Text = _oQuestionBase.Grade;
            lblSubject.Text = _oQuestionBase.Subject;
            lblItemBanks.Text = ItemBankMasterList.Filtered_ItemBank_Labels(_oQuestionBase.ItemBankList, SessionObject.LoggedInUser);
            lblStatus.Text = _oQuestionBase.Status;
            lblReservation.Text = _oQuestionBase.Reservation;
            lblAnchorItem.Text = _oQuestionBase.IsAnchorItem ? "Yes" : "No";
           
            
            switch (_oQuestionBase.QuestionType)
            {
                case QuestionTypes.E:
                    lblType.Text = "Essay";
                    break;
                case QuestionTypes.MC3:
                    lblType.Text = "Multiple Choice (3 Distractors)";
                    break;
                case QuestionTypes.MC4:
                    lblType.Text = "Multiple Choice (4 Distractors)";
                    break;
                case QuestionTypes.MC5:
                    lblType.Text = "Multiple Choice (5 Distractors)";
                    break;
                case QuestionTypes.S:
                    lblType.Text = "Short Answer";
                    break;
                case QuestionTypes.T:
                    lblType.Text = "True/False";
                    break;
                default:
                    lblType.Text = "Unknown";
                    break;
            }
            

            lblScoreType.Text = _oQuestionBase.ScoreType;

            lblKeywords.Text = _oQuestionBase.Keywords;
            lblCopyright.Text = String.IsNullOrEmpty(_oQuestionBase.Copyright) ? "No" : _oQuestionBase.Copyright;
            lblCopyRightExpiryDate.Text = _oQuestionBase.CopyRightExpiryDate == null ? "Not defined" : _oQuestionBase.CopyRightExpiryDate.GetValueOrDefault().Date.ToString("MM/dd/yyyy");
            lblSource.Text = _oQuestionBase.Source;
            lblCredit.Text = _oQuestionBase.Credit;



            /* Figure out contextually how to display rubric info.  If the question's scoretype is not rubric then there is no need to display the "rubric type", "Rubric Scoring" or Rubric Name.
            * If the question's scoretype is rubric, display rubric type, display rubric scoring, then consider whether it is a "Basic", "holistic", or "analytical" rubric:
            *      If "Basic", then
            *              Also make rubric scoring a hyperlink - when clicked provide pop-up window of rubric's criteria.
            *              don't display rubric name since it is basic (and there isn't any name).
            *      
            *      If "Holistic", then
            *              display rubric name.
            *              Also make rubric name a hyperlink - when clicked provide pop-up window of rubric's criteria.
            *              in popup, provide a link to take user to rubric object page.
            *              
            *      If "Analytical", then
            *              Make rubric scoring a hyperlink - when clicked provide pop-up window of rubric's criteria.
            *              In popup, provide a link to take user to the rubric object page.
            *              Don't display rubric name since it is Analytical.
            *              
            */

            if (_oQuestionBase.ScoreType == "Rubric")
            {
                lblRubricType.Text = _oQuestionBase.Rubric.TypeDesc;
                rowRubricType.Style["display"] = "block";

                lblRubricScoring.Text = _oQuestionBase.Rubric.Scoring;
                rowRubricScoring.Style["display"] = "block";

                ItemIDTile_hdnRubricName.Value = _oQuestionBase.Rubric.Name;
                ItemIDTile_hdnRubricContent.Value = _oQuestionBase.Rubric.Content;

                switch (_oQuestionBase.Rubric.TypeDesc)
                {
                    case "Basic":
                        hlRubricScoring.Attributes.Add("onClick", "displayRubricCriteria();");
                        rowRubricName.Style["display"] = "none";
                        break;
                    case "Analytical":
                        ItemIDTile_hdnRubricPageUrl.Value = "../../Record/RubricPage.aspx?xID=" + _oQuestionBase.Rubric.ID_Encrypted;
                        hlRubricScoring.Attributes.Add("onClick", "displayRubricCriteria();");
                        rowRubricName.Style["display"] = "none";
                        break;
                    case "Holistic":
                        lblRubricName.Text = _oQuestionBase.Rubric.Name;
                        ItemIDTile_hdnRubricPageUrl.Value = "../../Record/RubricPage.aspx?xID=" + _oQuestionBase.Rubric.ID_Encrypted;
                        hlRubricName.Attributes.Add("onClick", "displayRubricCriteria();");
                        hlRubricScoring.HRef = "";
                        rowRubricName.Style["display"] = "block";
                        break;
                }
            }
        }
    }
}