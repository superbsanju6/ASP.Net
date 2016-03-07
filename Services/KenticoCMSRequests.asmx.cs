using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.Services
{
    /// <summary>
    /// Summary description for KenticoCMSRequests
    /// </summary>
    [WebService(Namespace = "http://thinkgateplatform.net/services/KenticCMSRequests")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class KenticoCMSRequests : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public List<string[]> GetFormData(KenticoFormVariables formVars)
        {
            var kenticoForm = new KenticoFormData();
            kenticoForm.LoadFormData(formVars.StudentID, formVars.FormName);

            return kenticoForm.FormData;
        }

        [WebMethod]
        public MTSSComboBoxItemsWCF[] GetMTSSInterventions(MTSSComboBoxContextWCF context)
        {

            //Parse arguments from JS client
            int userpage = context.UserPage;
            String tier = context.Tier;

            //Get E3 Interventions by Tier
            E3InterventionDataObject E3obj = MTSSHelper.getE3InterventionData(userpage, true, 0, String.Empty, String.Empty, 0, "Both", tier);

            //Data Object
            MTSSComboBoxItemsWCF comboData = new MTSSComboBoxItemsWCF();

            //Item List
            List<MTSSComboBoxItemsWCF> result = new List<MTSSComboBoxItemsWCF>();

            //Add Default List Entries
            MTSSComboBoxItemsWCF d2 = new MTSSComboBoxItemsWCF();
            d2.Text = "No MTSS Alignment";
            d2.Value = "0";
            result.Add(d2);

            foreach (E3MTSSInterventions e3 in E3obj.InterventionsObject)
            {
                MTSSComboBoxItemsWCF combo = new MTSSComboBoxItemsWCF();
                combo.Text = e3.InterventionName;
                combo.Value = e3.InterventionID.ToString();
                result.Add(combo);
            }

            return result.ToArray();

        }
    }
}
