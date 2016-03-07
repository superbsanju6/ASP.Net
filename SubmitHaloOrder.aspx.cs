using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;

namespace Thinkgate
{
    public partial class SubmitHaloOrder : System.Web.UI.Page
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

                lblUserName.Text = sessionObject.LoggedInUser.UserFullName;
                lblOrderDate.Text = DateTime.Today.ToShortDateString();

                // Get the Client Name 
                Thinkgate.Base.Classes.DistrictParms districtParms = Thinkgate.Base.Classes.DistrictParms.LoadDistrictParms();
                lblDistrict.Text = districtParms.ClientName;

                // Load Schools Data
                LoadSchools();
            }

            FormCost.Text = FormCostHidden.Value;
            ShippingCost.Text = ShippingCostHidden.Value;
            TotalCost.Text = TotalCostHidden.Value;
        }

        protected void radSubmit_Click(object sender, EventArgs e)
        {
            //CalculateCosts();  MKR: Costs are already calculated client side.  Don't do it again.
            SaveHaloOrderData();
        }

        /// <summary>
        /// Gets the school list and bind with RadComboBox
        /// </summary>
        private void LoadSchools()
        {
            try
            {
                var schoolDataTable = new DataTable();
                var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(sessionObject.LoggedInUser);
                schoolDataTable.Columns.Add("Name");
                schoolDataTable.Columns.Add("ID");

                foreach (var s in schoolsForLooggedInUser)
                {
                    schoolDataTable.Rows.Add(s.Name, s.ID);
                }

                radSchool.DataTextField = "Name";
                radSchool.DataValueField = "ID";
                radSchool.DataSource = schoolDataTable;
                radSchool.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "LoadSchoolsFailed", "MessageAlert('Error:<br /> '" + ex.Message + "');", true);
            }
        }

        /// <summary>
        /// Save Halo Order Data
        /// </summary>
        private void SaveHaloOrderData()
        {
            string recType = "O";              //Identifies an order to stored proc
            string username = lblUserName.Text;
            string createdate = DateTime.Today.ToShortDateString();
            string district = lblDistrict.Text;

            string phoneNumber = radPhone.Text;
            string poNumber = radPONumber.Text;
            string email = radEmail.Text;
            string school = radSchool.SelectedItem.Text;
            string orderQty = radQuantity.SelectedValue;
            string dateNeeded = radDateNeeded.SelectedDate.Value.ToShortDateString();
            string billingPOC = radBillingPOC.Text;
            string billingStreet = radBillingStreet.Text;
            string billingCityState = radBillingCity.Text;
            string billingZip = radBillingZip.Text;
            string formCost = FormCostHidden.Value;
            string shipCost = ShippingCostHidden.Value;
            string totalCost = TotalCostHidden.Value;
            string shipPOC = radShippingPOC.Text;
            string shipStreet = radShippingStreet.Text;
            string shipCityState = radShippingState.Text;
            string shipZip = radShippingZip.Text;
            string comments = radComments.Text;
            string email1 = "";
            string email2 = "";

            try
            {
                string SQL = "@@RecType=" + recType + "@@@@UserName=" + username + "@@@@CreateDate=" + createdate + "@@@@PhoneNbr=" + phoneNumber + 
                    "@@@@PONbr=" + poNumber + "@@@@Email=" + email + "@@@@District=" + district + "@@@@School=" + school + "@@@@OrderQty=" + orderQty +
                    "@@@@DateNeeded=" + dateNeeded + "@@@@blgPOC=" + billingPOC + "@@@@blgStreet=" + billingStreet + "@@@@blgCityState=" + billingCityState +
                    "@@@@blgZip=" + billingZip + "@@@@fmtFormCost=" + formCost + "@@@@fmtShipCost=" + shipCost + "@@@@fmtTotalCost=" + totalCost +
                    "@@@@shpgPOC=" + shipPOC + "@@@@shpgStreet=" + shipStreet + "@@@@shpgCityState=" + shipCityState + "@@@@shpgZip=" + shipZip +
                    "@@@@Comments=" + comments + "@@@@Email1=" + email1 + "@@@@Email2=" + email2 + "@@";


                DataTable dataTable = Thinkgate.Base.Classes.HaloContent.SaveHaloOrderData(SQL);
                string successMessage = "Your order has been successfully submitted.  An order confirmation has been emailed.";
                ScriptManager.RegisterStartupScript(this, typeof(string), "HaloOrderSuccess", "MessageAlert('Success:<br /> " + successMessage + "');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "HaloOrderFailed", "MessageAlert('Error:<br /> '" + ex.Message + "');", true);
            }
        }

        /// <summary>
        /// Calculate Cost
        /// </summary>
        private void CalculateCosts() 
        {
                Int32 ordQuantity = int.Parse(radQuantity.SelectedValue);
                Int32 tierPrice = 0;

                if (ordQuantity < 25000) {
                    tierPrice = 100;
                }
                else if (ordQuantity > 24999 && ordQuantity < 50000) {
                    tierPrice = 70;
                }
                else if (ordQuantity > 49999 && ordQuantity < 75000) {
                    tierPrice = 54;
                }
                else if (ordQuantity > 74999 && ordQuantity < 100000) {
                    tierPrice = 51;
                }
                else {
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