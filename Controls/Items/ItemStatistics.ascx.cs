using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Base.Classes.Statistics;
using Thinkgate.Classes;
using Color = System.Drawing.Color;

namespace Thinkgate.Controls.Items
{
    /// <summary>
    /// The Item Statistics UI Tile
    /// </summary>
    public partial class ItemStatistics : TileControlBase
    {
        #region Constants

        private const string TileParam = "item";

        private const string BankQuestionTypeName = "BankQuestion";

        private const string DescriptionLableName = "lblDesc";
        private const string DistractorLabelName = "lblDistractor";
        private const string ValueLabelName = "lblValue";

        private const string TitleColumnName = "StatisticName";
        private const string ValueColumnName = "Value";
        private const string DistractorColumnName = "Distractor";

        private const string IdColumnName = "ID";

        private const string SampleSizeStatisticName = "Sample Size";
        private const string DistractorFrequencyName = "Frequency of distractors";

        private const string LoadingExceptionMessage = 
            "An error occured while loading Item Statistics Tile.";

        private const string ItemDataBoundExceptionMessage =
            "An error occured while binding Item Statistics data.";

        #endregion

        #region Fields

        private Type _questionType;
        private BankQuestion _oBankQuestion;
        private TestQuestion _oTestQuestion;

        private int _bankItemId;
        private int _testItemId;

        #endregion

        #region Properties

        /// <summary>
        /// Id of the corresponding Original 3rd Party Item
        /// </summary>
        private int BankItemId
        {
            get { return _bankItemId; }
        }

        /// <summary>
        /// Id of the Test Item
        /// </summary>
        private int TestItemId
        {
            get { return _testItemId; }
        }

        /// <summary>
        /// Foreground color used when statistic isn't found. 
        /// </summary>
        private Color HiddenColor
        {
            get { return Color.Gray; }
        }

        /// <summary>
        /// Foreground color used when statistic is found.
        /// </summary>
        private Color VisibleColor
        {
            get { return Color.Black;  }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Initial Page Load event handler, loads data
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">Standard Event args</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (IsPageInvalid()) { return; }

                LoadData();
            }

            catch (Exception ex)
            {
                throw new ApplicationException(LoadingExceptionMessage, ex);
            }
        }

        /// <summary>
        /// Binds a data source to either list box: Original or Calculated Item Statistics
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">ListBox Event Arguments</param>
        protected void lbxList_ItemDataBound(Object sender, RadListBoxItemEventArgs e)
        {
            try
            {
                BindItemData(e);
            }

            catch (Exception ex)
            {
                throw new ApplicationException(ItemDataBoundExceptionMessage, ex);
            }
        }

        #endregion

        #region Helper Methods

        private void LoadData()
        {
            ExtractQuestion();
            BindStatisticsData();
        }

        private void ExtractQuestion()
        {
            object item = ExtractItemObject();
            SetItemType(item);
            SetQuestion(item);
            SetItemIds();
        }

        private void SetItemType(object item)
        {
            Debug.Assert(item != null);

            _questionType = item.GetType();

            Debug.Assert(_questionType != null);
        }

        private void SetQuestion(object item)
        {
            Debug.Assert(item != null);
            Debug.Assert(_questionType != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(_questionType.Name));

            if (_questionType.Name == BankQuestionTypeName)
            {
                _oBankQuestion = item as BankQuestion;
                _oTestQuestion = null;
                Debug.Assert(_oBankQuestion != null);
            }
            else
            {
                _oTestQuestion = item as TestQuestion;
                _oBankQuestion = null;
                Debug.Assert(_oTestQuestion != null);
            }
        }

        private void SetItemIds()
        {
            SetBankItemId();
            SetTestItemId();
        }

        private void SetBankItemId()
        {
            if (IsSelectedItemATestItem())
            {
                Debug.Assert(_oTestQuestion != null);
                _bankItemId = _oTestQuestion.SharedContentID;
            }
            else
            {
                _bankItemId = _oBankQuestion.ID;
            }
        }

        private void SetTestItemId()
        {
            if (IsSelectedItemABankItem())
            {
                _testItemId = 0;
            }
            else
            {
                _testItemId = _oTestQuestion.ID;
            }
        }

        private bool IsSelectedItemABankItem()
        {
            return _oTestQuestion == null;
        }

        private bool IsSelectedItemATestItem()
        {
            return _oBankQuestion == null;
        }

        private bool IsPageInvalid()
        {
            Debug.Assert(Tile != null);
            Debug.Assert(Tile.TileParms != null);

            object item = ExtractItemObject();

            if (IsTileWrongType(item))
            {
                return true;
            }

            return false;
        }

        private bool IsTileWrongType(object item)
        {
            return item == null;
        }

        private object ExtractItemObject()
        {
            return Tile.TileParms.GetParm(TileParam);
        }

        private void BindStatisticsData()
        {
            BindOriginalItemStatisticsData();
            BindCalculatedItemStatisticsData();
        }

        private void BindOriginalItemStatisticsData()
        {
            DataTable dtOrig = ThinkgateStatistics.GetBankItemOriginalStatistics(BankItemId);
            lbxOriginalStatistics.DataSource = dtOrig;
            lbxOriginalStatistics.DataBind();
        }

        private void BindCalculatedItemStatisticsData()
        {
            DataTable dtCalc;

            if (IsSelectedItemATestItem())
            {
                dtCalc = ThinkgateStatistics.GetTestItemStatistics(TestItemId);
            }
            else
            {
                dtCalc = ThinkgateStatistics.GetCalcStatsForAllTestItemsMadeFromBankItem(BankItemId);
            }

            lbxCalculatedStatistics.DataSource = dtCalc;
            lbxCalculatedStatistics.DataBind();
        }

        private void BindItemData(RadListBoxItemEventArgs e)
        {
            RadListBoxItem listBoxItem = ExtractRadListBoxItem(e);

            RadListBoxItemState state = GetRadListBoxItemState(listBoxItem);

            ApplyRadListBoxItemState(listBoxItem, state);
        }

        private RadListBoxItem ExtractRadListBoxItem(RadListBoxItemEventArgs e)
        {
            Debug.Assert(e != null);
            Debug.Assert(e.Item != null);

            RadListBoxItem listBoxItem = e.Item;

            Debug.Assert(listBoxItem != null);

            return listBoxItem;
        }

        private RadListBoxItemState GetRadListBoxItemState(RadListBoxItem listBoxItem)
        {
            Debug.Assert(listBoxItem != null);

            var row = (DataRowView)(listBoxItem).DataItem;

            Debug.Assert(row != null);

            return new RadListBoxItemState
            {
                DataRowViewState = GetDataRowViewState(row),
            };
        }

        private DataRowViewState GetDataRowViewState(DataRowView row)
        {
            Debug.Assert(row != null);

            return new DataRowViewState
            {
                DescriptionLabelState = GetTitleLabelState(row),
                DistractorLabelState = GetDistractorLabelState(row),
                ValueLableState = GetValueLabelState(row)
            };
        }

        private LabelState GetTitleLabelState(DataRowView row)
        {
            Debug.Assert(row != null);

            object title = GetViewValue(row, TitleColumnName);

            bool isValueValid = IsDataRowViewValueValid(row);

            return new LabelState
            {
                Text = title.ToString(),
                ForegroundColor = isValueValid ? VisibleColor : HiddenColor
            };
        }

        private LabelState GetDistractorLabelState(DataRowView row)
        {
            Debug.Assert(row != null);

            object distractor = GetViewValue(row, DistractorColumnName);
            const string formatText = "{0} :";

            bool isValueValid = IsStatisticFrequencyDistractor(row)
                             && IsDataRowViewValueValid(row);

            return new LabelState
            {
                Text = isValueValid ? string.Format(formatText, distractor) : string.Empty,
                ForegroundColor = isValueValid ? VisibleColor : HiddenColor
            };
        }

        private LabelState GetValueLabelState(DataRowView row)
        {
            Debug.Assert(row != null);

            object value = GetViewValue(row, ValueColumnName);
            bool isValueValid = IsDataRowViewValueValid(row);

            return new LabelState
            {
                Text = isValueValid ? value.ToString() : string.Empty,
                ForegroundColor = isValueValid ? VisibleColor : HiddenColor
            };
        }

        private bool IsStatisticFrequencyDistractor(DataRowView row)
        {
            Debug.Assert(row != null);

            string distractorValue = row[ValueColumnName].ToString();

            return IsStatisticOfType(row, DistractorFrequencyName) 
                && IsValueViewValid(distractorValue);
        }

        private bool IsStatisticSampleSize(DataRowView row)
        {
            return IsStatisticOfType(row, SampleSizeStatisticName);
        }

        private bool IsStatisticOfType(DataRowView row, string statisticName)
        {
            Debug.Assert(row != null);

            var rowStatisticName = row[TitleColumnName] as string;

            return statisticName.Equals(rowStatisticName);
        }

        private bool IsDataRowViewColumnValueColumn(string columnName)
        {
            return columnName == ValueColumnName;
        }

        private string GetViewValue(DataRowView row, string columnName)
        {
            string resultObject = row[columnName].ToString();

            if (IsDataRowViewColumnValueColumn(columnName))
            {
                if (IsStatisticFrequencyDistractor(row))
                {
                    resultObject = ExtractDistractorFrequencyValue(resultObject);
                }

                if (IsStatisticSampleSize(row))
                {
                    resultObject = ExtractSampleSizeValue(resultObject);
                }
            }

            return resultObject;
        }

        private string ExtractDistractorFrequencyValue(object rowValue)
        {
            string result = string.Empty;

            decimal convertedValue;

            var textValue = rowValue as string;

            if (decimal.TryParse(textValue, out convertedValue))
            {
                Debug.Assert(convertedValue >= 0);
                Debug.Assert(convertedValue <= 1);
                
                result = convertedValue.ToString("P0");
            }
            
            return result;
        }

        private string ExtractSampleSizeValue(object rowValue)
        {
            string result = string.Empty;

            float convertedValue;

            var textValue = rowValue as string;

            if (float.TryParse(textValue, out convertedValue))
            {
                Debug.Assert(convertedValue >= 0);

                result = convertedValue.ToString("N0");
            }

            return result;
        }

        private bool IsDataRowViewValueValid(DataRowView row)
        {
            Debug.Assert(row != null);

            string rowValue = row[ValueColumnName] as string;

            return IsValueViewValid(rowValue);
        }

        private bool IsValueViewValid(string rowValue)
        {
            return !IsValueViewInvalid(rowValue);
        }

        private bool IsValueViewInvalid(string rowValue)
        {
            return string.IsNullOrWhiteSpace(rowValue);
        }

        private void ApplyRadListBoxItemState(RadListBoxItem item, RadListBoxItemState state)
        {
            Debug.Assert(item != null);
            Debug.Assert(state != null);

            var descriptionLabel = (Label)item.FindControl(DescriptionLableName);
            Debug.Assert(descriptionLabel != null);
            ApplyLableState(descriptionLabel, state.DataRowViewState.DescriptionLabelState);

            var distractorLabel = (Label)item.FindControl(DistractorLabelName);
            Debug.Assert(distractorLabel != null);
            ApplyLableState(distractorLabel, state.DataRowViewState.DistractorLabelState);

            var valueLabel = (Label)item.FindControl(ValueLabelName);
            Debug.Assert(valueLabel != null);
            ApplyLableState(valueLabel, state.DataRowViewState.ValueLableState);
        }

        private void ApplyLableState(Label label, LabelState state)
        {
            label.Text = state.Text;
            label.ForeColor = state.ForegroundColor;
        }

        #endregion
    }
}