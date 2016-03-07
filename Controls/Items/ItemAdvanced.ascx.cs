

using System;
using System.Text;
using System.Data;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.Items
{
    public partial class ItemAdvanced : TileControlBase
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
            HtmlTableRow oRow;
            HtmlTableCell oCell;
            HtmlGenericControl oDiv;
            if (Tile.TileParms.GetParm("item") == null) return;

            _questionType = Tile.TileParms.GetParm("item").GetType();
            if (_questionType.Name == "BankQuestion")
            {
                _oBankQuestion = (BankQuestion)Tile.TileParms.GetParm("item");
                _oQuestionBase = (QuestionBase)_oBankQuestion;
            }
            else
            {
                _oTestQuestion = (TestQuestion)Tile.TileParms.GetParm("item");
                _oQuestionBase = (QuestionBase)_oTestQuestion;
            }

            /*******************************************************************************
             * Although for any given version of the Elements application there is only one
             * type of rigor (either Webb, Marzano, or RBT), and this is determined by the 
             * value of DOK in the Parms table in the database, a bank question may have 
             * all three rigors in its full rigor string. Its just that only one of the 
             * three rigors is used.  If displaying a bank question, we intend to display 
             * all the rigors listed in the full rigor string.  The full rigor string looks 
             * something like:
             *      Webb=3|RBT=4|Marzano=4|
             * We get to parse out the different rigors and then use the numeric code of 
             * each and fetch from the database the actual text or description for the code.
             * 
             * We expect each rigor to be divided by vertical bar '|', and each rigor to be 
             * be of a format: {rigor name}={numeric code} (e.g. RBT=2).
             * 
             * If displaying a test question, then we will only display the single rigor de-
             * fined at the district.
             * *****************************************************************************/

            var rgxRigorFormat = new Regex(@"[a-zA-Z]+=\d+"); //this regex pattern should match above format for rigor.


            /*list of rigors and codes to pass to the database in order to look up code's text*/
            dtGeneric_String_Int dtListOfRigors = new dtGeneric_String_Int();

            if (_questionType.Name == "BankQuestion")
            {
                /*array of strings from full string rigor, each holding a rigor=value pattern, we hope.*/
                string[] aRigors = _oQuestionBase.Rigor.FullRigorString.Split('|');

                /*loop through array of strings and collect a set of legit rigor/code values in dtListOfRigors*/
                foreach (var sRigor in aRigors)
                {
                    if (rgxRigorFormat.IsMatch(sRigor))
                    {
                        dtListOfRigors.Add(sRigor.Substring(0, sRigor.IndexOf('=')), DataIntegrity.ConvertToInt(sRigor.Substring(sRigor.IndexOf('=') + 1)));
                    }
                }

                /*Pass dtListOfRigors via SP to database in order to look up each rigor's code's text value.*/
                var dt = ThinkgateDataAccess.FetchDataTable("E3_RIGOR_GetDescriptionsFromListOfCodes", new object[] { dtListOfRigors.ToSql() });

                /* Loop through our result set and build into our tableItemAdvanced html table for displaying */
                DataRow drRigor;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    drRigor = dt.Rows[i];
                    oRow = new HtmlTableRow();

                    oCell = new HtmlTableCell();
                    oCell.Attributes.Add("class", "fieldLabel");
                    oCell.Attributes.Add("style", "width:130px");
                    oCell.InnerText = drRigor["Field"].ToString() + ":";
                    oRow.Cells.Add(oCell);

                    oCell = new HtmlTableCell();
                    oCell.InnerText = drRigor["Text"].ToString();
                    oRow.Cells.Add(oCell);

                    tblItemAdvanced.Rows.Insert(i, oRow);
                }
            }
            else
            {
                var distParms = DistrictParms.LoadDistrictParms();
                oRow = new HtmlTableRow();

                oCell = new HtmlTableCell();
                oCell.Attributes.Add("class", "fieldLabel");
                oCell.Attributes.Add("style", "width:130px");
                oCell.InnerText = distParms.DOK; 
                oRow.Cells.Add(oCell);

                oCell = new HtmlTableCell();
                oCell.InnerText = _oQuestionBase.Rigor.ToString();
                oRow.Cells.Add(oCell);

                tblItemAdvanced.Rows.Insert(0, oRow);
            }

            lblItemDifficulty.Text = string.Format("{0:#.00##}", _oQuestionBase.ItemDifficulty);
            lblDifficultyIndex.Text = _oQuestionBase.DifficultyIndex.ToString();

            /******************************************************
             * The number of Distractor Rationale values vary, so 
             * we need to build the HTML to represent these.
             * ***************************************************/
            foreach (var sDistractorRationale in _oQuestionBase.DistractorRationale)
            {
                oDiv = new HtmlGenericControl();
                oDiv.InnerHtml = sDistractorRationale;

                oCell = new HtmlTableCell();
                oCell.ColSpan = 2;
                oCell.Controls.Add(oDiv);
                
                oRow = new HtmlTableRow();
                oRow.Cells.Add(oCell);

                tblItemAdvanced.Rows.Add(oRow);
            }
        }
    }
}
