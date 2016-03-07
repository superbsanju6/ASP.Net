using System;
using System.Text;
using System.Web.UI;
using Standpoint.Core.ExtensionMethods;
using Thinkgate.Base.Enums;
using Thinkgate.Classes.Search;
using Thinkgate.Enums.Search;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class TextBoxEdit : Criterion
    {
        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txt.Text = string.Empty;
                CriteriaHeaderText.Text = Header;
                if (EditMask.IsNotNull())
                {
                    ClientScriptManager clientScript = Page.ClientScript;
                    StringBuilder javaScriptBuilder = new StringBuilder();
                

                   switch (EditMask)
                    {
                        case Thinkgate.Base.Enums.EditMaskType.DateMask:
                            javaScriptBuilder.Append(" $('#" + txt.ClientID + "').inputmask('dd/mm/yyyy', {placeholder: '_'});");
                            break;
                        case Thinkgate.Base.Enums.EditMaskType.TimeMask:
                            javaScriptBuilder.Append(" $('#" + txt.ClientID + "').inputmask('hh:mm:ss', {placeholder: '_'});");
                            break;
                        case Thinkgate.Base.Enums.EditMaskType.DateTimeMask:
                            javaScriptBuilder.Append(" $('#" + txt.ClientID + "').inputmask('datetime', {placeholder: '_'});");
                            break;
                        case Thinkgate.Base.Enums.EditMaskType.NumericMask:
                            javaScriptBuilder.Append(" $('#" + txt.ClientID + "').inputmask('integer', {allowMinus: false, allowPlus: false});");
                            break;
                        case Thinkgate.Base.Enums.EditMaskType.DecimalMask:
                            javaScriptBuilder.Append(" $('#" + txt.ClientID + "').inputmask('decimal', {digits:" + DecimalPositions + "});");
                            break;
                        case Thinkgate.Base.Enums.EditMaskType.UrlMask:
                            javaScriptBuilder.Append(" $('#" + txt.ClientID + "').inputmask('url');");
                            break;
                        case Thinkgate.Base.Enums.EditMaskType.PhoneMask:
                            javaScriptBuilder.Append(" $('#" + txt.ClientID + "').inputmask('mask', {'mask': '(999) 999-9999'});");
                            break;
                    }
                    
                   clientScript.RegisterStartupScript(GetType(), txt.ClientID,
                                javaScriptBuilder.ToString(), true);
                }
                if (IsRequired)
                {
                    RequiredCriteriaIndicator.Text = "*";
                    RequiredCriteriaIndicator.Style.Add("color", "red");
                    RequiredCriteriaIndicator.Style.Add("font-weight", "bold");
                }
            }
        }

        #endregion
    }
}