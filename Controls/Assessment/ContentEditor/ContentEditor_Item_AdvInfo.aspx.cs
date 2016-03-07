using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Data;
using Telerik.Web.UI;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public partial class ContentEditor_Item_AdvInfo : BasePage
    {
        public string _sQuestionType;
        public int _iItemID;
        public int _iAssessmentID;
        protected string _Dok;
        private DataTable dtResults;
        public int Rbt;
        public int Webb;
        public int Marzano;
        public string ItemDiff;
        public decimal ItemPerc;

        protected void Page_Load(object sender, EventArgs e)
        {
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
        }

        private void LoadData()
        {
            _sQuestionType = "" + Request.QueryString["qType"];
            _iItemID = GetDecryptedEntityId(X_ID);
            _iAssessmentID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "AssessmentID");

            dtResults = new DataTable();
            if (_iItemID == 0)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                if (_sQuestionType.Equals("TestQuestion"))
                    dtResults = TestQuestion.GetAdvancedItemInfo(_iItemID);
                else
                    dtResults = BankQuestion.GetAdvancedItemInfo(_iItemID);

                foreach (DataRow r in dtResults.Rows)
                {
                    Rbt = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(r["RBT"]);
                    Webb = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(r["Webb"]);
                    Marzano = Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(r["Marzano"]);
                    ItemDiff = r["DifficultyIndex"].ToString();
                    ItemPerc = Standpoint.Core.Utilities.DataIntegrity.ConvertToDecimal(r["DifficultyParameter"]);
                }
            }

            var dparms = DistrictParms.LoadDistrictParms();
            _Dok = dparms.DOK;

        }


        private void BindComboBoxes()
        {
            comboRBT.DataTextField = "Text";
            comboRBT.DataValueField = "Code";
            comboRBT.DataSource = Rigor.LoadRigorListByDOK("RBT");
            comboRBT.DataBind();

            RadComboBoxItem someitemRbt = comboRBT.FindItemByValue(Rbt.ToString());
            if(someitemRbt!=null) someitemRbt.Selected = true;

            comboMarzano.DataTextField = "Text";
            comboMarzano.DataValueField = "Code";
            comboMarzano.DataSource = Rigor.LoadRigorListByDOK("Marzano");
            comboMarzano.DataBind();

            RadComboBoxItem someitemMarzano = comboMarzano.FindItemByValue(Marzano.ToString());
            if (someitemMarzano != null) someitemMarzano.Selected = true;

            comboWebb.DataTextField = "Text";
            comboWebb.DataValueField = "Code";
            comboWebb.DataSource = Rigor.LoadRigorListByDOK("Webb");
            comboWebb.DataBind();

            RadComboBoxItem someitemWebb = comboWebb.FindItemByValue(Webb.ToString());
            if (someitemWebb != null) someitemWebb.Selected = true;

            comboDifficultyIndex.DataTextField = "Value";
            comboDifficultyIndex.DataValueField = "Key";
            comboDifficultyIndex.DataSource = DifficultyIndex.PossibleValueListDictionary();
            comboDifficultyIndex.DataBind();
            RadComboBoxItem someitemDifficultyIndex = comboDifficultyIndex.FindItemByValue(ItemDiff.ToString());
            if (someitemDifficultyIndex != null) someitemDifficultyIndex.Selected = true;
        }
    }
}