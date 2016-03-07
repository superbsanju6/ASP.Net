using System;
using Standpoint.Core.Utilities;


namespace Thinkgate.Controls.Student
{
    public partial class Message : System.Web.UI.UserControl
    {
        public int? StudentID;
        public string subject;
        public string body;

        protected void Page_Load(object sender, EventArgs e)
        {
            StudentID = 38822;

            FillData();
        }

        private void FillData()
        {
            bodyPanel.InnerHtml = body;
            subjectPanel.InnerHtml = subject;
        }

    }
}