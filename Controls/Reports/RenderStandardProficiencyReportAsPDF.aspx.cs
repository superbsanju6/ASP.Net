using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Telerik.Web.UI;
using Thinkgate.Classes;
using System.IO;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.Reports
{
    public partial class RenderStandardProficiencyReportAsPDF : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetReportConfiguration();
        }

        private void SetReportConfiguration()
        {
            try
            {
                SessionObject sessionObject = (SessionObject)Session["SessionObject"];
                var sSRSServerInfo = Thinkgate.Base.Classes.Reports.GetSSRSServerInfo(sessionObject.GlobalInputs);
                var sSRSReportPaths = Thinkgate.Base.Classes.Reports.GetSSRSReportPaths("StateStandardsReport");

                if (sSRSServerInfo.Rows.Count == 0 || sSRSReportPaths.Rows.Count == 0)
                { return; }

                reportServerUrl.Value = sSRSServerInfo.Rows[0]["URL"].ToString();
                reportPath.Value = sSRSReportPaths.Rows[0]["Path"].ToString();
                reportUsername.Value = sSRSServerInfo.Rows[0]["Username"].ToString();
                reportPassword.Value = sSRSServerInfo.Rows[0]["Password"].ToString();
                reportDomain.Value = sSRSServerInfo.Rows[0]["Domain"].ToString();

                GeneratePDFReport();
            }
            catch (Exception ex)
            {
                lblMessage.Visible = true;
                lblMessage.Text = ex.Message;
            }
        }


        private void GeneratePDFReport()
        {
            string Grade = string.Empty; ;
            string Subject = string.Empty; ;
            string Course = string.Empty; ;
            string StudentId = string.Empty; ;

            if (Request.QueryString["Grade"] != null)
            { Grade =Standpoint.Core.Classes.Encryption.DecryptString(Request.QueryString["Grade"].ToString()); }

            if (Request.QueryString["Subject"] != null)
            { Subject = Standpoint.Core.Classes.Encryption.DecryptString(Request.QueryString["Subject"].ToString()); }

            if (Request.QueryString["Course"] != null)
            { Course =Standpoint.Core.Classes.Encryption.DecryptString(Request.QueryString["Course"].ToString()); }

            if (Request.QueryString["StudentId"] != null)
            { StudentId = Standpoint.Core.Classes.Encryption.DecryptString(Request.QueryString["StudentId"]); }

            var DParms = DistrictParms.LoadDistrictParms();
            string ClientID = DParms.ClientID;
            string Year = DParms.Year;

            Microsoft.Reporting.WebForms.ReportViewer rptViewer = new Microsoft.Reporting.WebForms.ReportViewer();
            rptViewer.ProcessingMode = ProcessingMode.Remote;

            rptViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials(reportUsername.Value, reportPassword.Value, reportDomain.Value);
            rptViewer.ServerReport.ReportServerUrl = new Uri(reportServerUrl.Value);
            rptViewer.ServerReport.ReportPath = reportPath.Value;

            var reportParameterCollection = new ReportParameterCollection();
            reportParameterCollection.Add(new ReportParameter("ClientID", ClientID));
            reportParameterCollection.Add(new ReportParameter("Year", Year));
            reportParameterCollection.Add(new ReportParameter("Grade", Grade));
            reportParameterCollection.Add(new ReportParameter("Subject", Subject));
            reportParameterCollection.Add(new ReportParameter("Course", Course));
            reportParameterCollection.Add(new ReportParameter("StudentTGID", StudentId));

            rptViewer.ServerReport.SetParameters(reportParameterCollection);
            rptViewer.ServerReport.Refresh();

            Warning[] warnings;
            string[] streamids;
            string mimeType, encoding, extension;

            byte[] bytes = rptViewer.ServerReport.Render("PDF", string.Empty, out mimeType, out encoding, out extension, out streamids, out warnings);

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline; filename=StateStandardsReport.pdf");
                Response.AddHeader("content-length", bytes.Length.ToString());
                Response.BinaryWrite(memoryStream.ToArray());
                Response.Flush();
                Response.End();
            }
        }
    }
}