using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Telerik.Web.UI;
using Thinkgate.Classes;
using System.IO;
using Thinkgate.Base.Classes;
using System.Data;

namespace Thinkgate.Controls.Reports
{
    public partial class StarReportGeneratePDF : System.Web.UI.Page
    {
        private CourseList _curriculumCourseList;
        SessionObject sessionObject = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            SetReportConfiguration();
        }

        private void SetReportConfiguration()
        {
            try
            {
                sessionObject = (SessionObject)HttpContext.Current.Session["SessionObject"];
                var sSrsServerInfo = Thinkgate.Base.Classes.Reports.GetSSRSServerInfo(sessionObject.GlobalInputs);
                var sSrsReportPaths = Thinkgate.Base.Classes.Reports.GetSSRSReportPaths("MultipleAssessment");

                if (sSrsServerInfo.Rows.Count == 0 || sSrsReportPaths.Rows.Count == 0)
                { return; }

                reportServerUrl.Value = sSrsServerInfo.Rows[0]["URL"].ToString();
                reportPath.Value = sSrsReportPaths.Rows[0]["Path"].ToString();
                reportUsername.Value = sSrsServerInfo.Rows[0]["Username"].ToString();
                reportPassword.Value = sSrsServerInfo.Rows[0]["Password"].ToString();
                reportDomain.Value = sSrsServerInfo.Rows[0]["Domain"].ToString();

                GeneratePDFReport();
            }
            catch (Exception ex)
            {
                lblMessage.Visible = true;
                lblMessage.InnerText = ex.Message;
            }
        }

        private void GeneratePDFReport()
        {
            string Grade = string.Empty; ;
            string Subject = string.Empty; ;
            string Course = string.Empty; ;
            string SchoolId = string.Empty; ;

            if (Request.QueryString["Grade"] != null)
            { Grade = Request.QueryString["Grade"].ToString(); }

            if (Request.QueryString["Subject"] != null)
            { Subject = Request.QueryString["Subject"].ToString(); }

            if (Request.QueryString["Course"] != null)
            { Course = Request.QueryString["Course"].ToString(); }

            if (Request.QueryString["SchoolId"] != null)
            { SchoolId = Request.QueryString["SchoolId"]; }
            
            string clientid = DistrictParms.LoadDistrictParms().ClientID;
            string userPage = sessionObject.LoggedInUser.Page.ToString();

            string currID = GetCurriculumID(Grade,Subject,Course);

            Microsoft.Reporting.WebForms.ReportViewer rptViewer = new Microsoft.Reporting.WebForms.ReportViewer
                                                                  {
                                                                      ProcessingMode =
                                                                          ProcessingMode.Remote
                                                                  };

            rptViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials(reportUsername.Value, reportPassword.Value, reportDomain.Value);
            rptViewer.ServerReport.ReportServerUrl = new Uri(reportServerUrl.Value);
            rptViewer.ServerReport.ReportPath = reportPath.Value;

            rptViewer.ShowParameterPrompts = false;

            var reportParameterCollection = new ReportParameterCollection
                                            {
                                                new ReportParameter("ClientID", clientid),
                                                new ReportParameter("CurrID", currID),
                                                new ReportParameter("UserPage", userPage),
                                                new ReportParameter("SchoolID", SchoolId)
                                            };

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
                Response.AddHeader("content-disposition", "inline; filename=MultipleAssessmentPerformance.pdf");
                Response.AddHeader("content-length", bytes.Length.ToString());
                Response.BinaryWrite(memoryStream.ToArray());
                Response.Flush();
                Response.End();
            }
        }

        private string GetCurriculumID(string grade, string subject, string course)
        {
            string CurriculumID = string.Empty;
            string sql = string.Format(@"Select ID from CurrCourses Where Grade='{0}' and Subject='{1}' and Course='{2}'",grade,subject,course);
            DataTable GradeDataTable = GetDataTable(sql);
            if (GradeDataTable != null)
            {
                CurriculumID = GradeDataTable.Rows[0]["ID"].ToString();
            }

            return CurriculumID;
        }

        private DataTable GetDataTable(string selectSQL)
        {
            DataTable dataTable = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ConnectionString;
            if (selectSQL != null)
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = new SqlCommand(selectSQL, sqlConnection);

                try
                {
                    sqlConnection.Open();
                }
                catch (SqlException ex)
                {
                    return dataTable;
                }
                try
                {
                    sqlDataAdapter.Fill(dataTable);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return dataTable;
        }
    }
}