using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Thinkgate.Classes;

namespace Thinkgate
{
    public partial class CalculateHaloCost : System.Web.UI.Page
    {
        SessionObject sessionObject;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
            sessionObject = (SessionObject)Session["SessionObject"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HaloCostPerBox.Value = Thinkgate.Base.Classes.DistrictParms.LoadDistrictParms().HaloFormsCost;
                HaloFormsPerBox.Value = Thinkgate.Base.Classes.DistrictParms.LoadDistrictParms().HaloFormsPerBox;
                HaloShippingCost.Value = Thinkgate.Base.Classes.DistrictParms.LoadDistrictParms().HaloShippingCost;
                HaloShippingCostFlatRate.Value = Thinkgate.Base.Classes.DistrictParms.LoadDistrictParms().HaloShippingCostFlatRate;
            }

            FormCost.Text = FormCostHidden.Value;
            ShippingCost.Text = ShippingCostHidden.Value;
            TotalCost.Text = TotalCostHidden.Value;
        }

        protected void radButtonEmail_Click(object sender, EventArgs e)
        {
            //CalculateCosts();  MKR: Costs are already calculated client side.  Don't do it again.
            EmailHaloCostsData();
        }

        /// <summary>
        /// Email Halo Costs Data
        /// </summary>
        private void EmailHaloCostsData()
        {
            string recType = string.Empty;              //Identifies an order to stored proc
            string username = string.Empty;
            string createdate = string.Empty;
            string district = string.Empty;

            string phoneNumber = string.Empty;
            string poNumber = string.Empty;
            string email = string.Empty;
            string school = string.Empty;
            string orderQty = radComboBoxQty.SelectedValue;
            string dateNeeded = string.Empty;
            string billingPOC = string.Empty;
            string billingStreet = string.Empty;
            string billingCityState = string.Empty;
            string billingZip = string.Empty;
            string formCost = FormCostHidden.Value;
            string shipCost = ShippingCostHidden.Value;
            string totalCost = TotalCostHidden.Value;
            string shipPOC = string.Empty;
            string shipStreet = string.Empty;
            string shipCityState = string.Empty;
            string shipZip = string.Empty;
            string comments = string.Empty;
            string email1 = radTextBoxEmail1.Text;
            string email2 = radTextBoxEmail2.Text;

            try
            {
                string SQL = "@@RecType=" + recType + "@@@@UserName=" + username + "@@@@CreateDate=" + createdate + "@@@@PhoneNbr=" + phoneNumber +
                    "@@@@PONbr=" + poNumber + "@@@@Email=" + email + "@@@@District=" + district + "@@@@School=" + school + "@@@@OrderQty=" + orderQty +
                    "@@@@DateNeeded=" + dateNeeded + "@@@@blgPOC=" + billingPOC + "@@@@blgStreet=" + billingStreet + "@@@@blgCityState=" + billingCityState +
                    "@@@@blgZip=" + billingZip + "@@@@fmtFormCost=" + formCost + "@@@@fmtShipCost=" + shipCost + "@@@@fmtTotalCost=" + totalCost +
                    "@@@@shpgPOC=" + shipPOC + "@@@@shpgStreet=" + shipStreet + "@@@@shpgCityState=" + shipCityState + "@@@@shpgZip=" + shipZip +
                    "@@@@Comments=" + comments + "@@@@Email1=" + email1 + "@@@@Email2=" + email2 + "@@";


                DataTable dataTable = Thinkgate.Base.Classes.HaloContent.SaveHaloOrderData(SQL);
                string successMessage = "Your email has been sent";
                ScriptManager.RegisterStartupScript(this, typeof(string), "CalculateHaloCostsSuccess", "MessageAlert('Success:<br /> " + successMessage + "');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "CalculateHaloCostsFailed", "MessageAlert('Error:<br /> '" + ex.Message + "');", true);
            }
        }

        /// <summary>
        /// Calculate Cost
        /// </summary>
        private void CalculateCosts()
        {
            Int32 ordQuantity = int.Parse(radComboBoxQty.SelectedValue);
            Int32 tierPrice = 0;

            if (ordQuantity < 25000)
            {
                tierPrice = 100;
            }
            else if (ordQuantity > 24999 && ordQuantity < 50000)
            {
                tierPrice = 70;
            }
            else if (ordQuantity > 49999 && ordQuantity < 75000)
            {
                tierPrice = 54;
            }
            else if (ordQuantity > 74999 && ordQuantity < 100000)
            {
                tierPrice = 51;
            }
            else
            {
                tierPrice = 30;
            }

            Decimal formCost = Convert.ToDecimal(ordQuantity / 1000 * tierPrice);
            FormCost.Text = formCost.ToString("#,##0.00");

            Decimal shippingCost = Convert.ToDecimal(ordQuantity / 5000 * 35);
            ShippingCost.Text = shippingCost.ToString("#,##0.00");

            Decimal totalCost = Convert.ToDecimal(formCost + shippingCost);
            TotalCost.Text = totalCost.ToString("#,##0.00");
        }
    }
}