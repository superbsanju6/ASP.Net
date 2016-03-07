using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Services.Contracts.AssessmentUsageStatistics;
using Thinkgate.Services.Contracts.ServiceModel;
using Thinkgate.Services.Contracts.UsageStatistics;

namespace Thinkgate.Record
{
    public partial class RenderAssessmentItemUsageReportView : BasePage
    {
        private List<string> dtItemBank = new List<string>();
        private List<string> selectedSubjects = new List<string>();
        private List<string> details = new List<string>();
        private AssessmentItemUsageStatisticData _reportData;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _itemBank = Request.QueryString["ItemBank"];
            string _category = Request.QueryString["Category"];
            string _school = Request.QueryString["School"];
            string _grade = Request.QueryString["Grade"] == "All" ? null : Request.QueryString["Grade"];
            string _subject = Request.QueryString["Subject"];
            string _startDate = Request.QueryString["StartDate"];
            string _endDate = Request.QueryString["EndDate"];
            string _currDate = Request.QueryString["CurrDate"];

            details.Add("ItemBank: " + (Request.QueryString["ItemBank"] == "" ? "All" : _itemBank));
            details.Add("Category: " + Request.QueryString["Category"]);
            details.Add("School: " + (Request.QueryString["School"] == "" ? "All" : Request.QueryString["School"]));
            details.Add("Grade: " + (Request.QueryString["Grade"] == "" ? "All" : Request.QueryString["Grade"]));
            details.Add("Subject: " + (Request.QueryString["Subject"] == "" ? "All" : Request.QueryString["Subject"]));

            if (Request.QueryString["StartDate"] != string.Empty && Request.QueryString["EndDate"] != string.Empty)
            {
                details.Add("DateRange: " + (Request.QueryString["StartDate"] + "-" + Request.QueryString["EndDate"]));
            }
            else
            {
                details.Add("DateRange: Entire School Year");
            }

            if (_itemBank != "")
            {
                foreach (string part in _itemBank.Split(','))
                {
                    dtItemBank.Add(part.Trim());
                }
            }

            if (_subject != "")
            {
                foreach (string part in _subject.Split(','))
                {
                    selectedSubjects.Add(part.Trim());
                }
            }
            DistrictParms parms = DistrictParms.LoadDistrictParms();
            if (!IsPostBack)
            {
                var paramanters = new AssessmentItemUsageStatisticInputParameters()
                {
                    ClientID = parms.ClientID,
                    ItemBanks = dtItemBank,
                    Category = Request.QueryString["Category"],
                    School = Request.QueryString["School"] == "" ? null : Request.QueryString["School"],
                    Grade = Request.QueryString["Grade"] == "" ? null : Request.QueryString["Grade"],
                    Subjects = selectedSubjects,
                    StartDate = Request.QueryString["StartDate"] != string.Empty ? DateTime.Parse(Request.QueryString["StartDate"]) : default(DateTime?),
                    EndDate = Request.QueryString["EndDate"] != string.Empty ? DateTime.Parse(Request.QueryString["EndDate"]) : default(DateTime?)
                };

                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );

                var proxy =
                    new UsageStatisticsProxy(new SamlSecurityTokenSettings
                    {
                        SamlSecurityTokenizerAction = SamlSecurityTokenizerAction.UseThreadPrincipalIdentity,
                        ServiceCertificateStoreName = ConfigurationManager.AppSettings["ServiceCertificateStoreName"],
                        ServiceCertificateThumbprint = ConfigurationManager.AppSettings["ServiceCertificateThumbprint"]
                    });

                _reportData = proxy.GetAssessmentItemUsageStatistics(paramanters);

                GridView3.DataSource = details;
                GridView3.DataBind();

                GridView1.DataSource = from t in _reportData.ItemFrequencies
                                          select new 
                                           {
                                               t.ItemID,
                                               t.Frequency
                                           };
                GridView1.DataBind();
                GridView2.DataSource = _reportData.ItemBankFrequencies;
                GridView2.DataBind();

                lblDate.Text = _currDate;
            }
        }
    }
}