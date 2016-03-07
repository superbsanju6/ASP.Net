using System;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;

namespace Thinkgate.ControlHost
{
    public partial class PreviewTestQuestion : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            var xID = GetDecryptedEntityId(X_ID);

            string type = string.Empty;
             type = Request.QueryString["type"] ?? "TestQuestion";

            QuestionBase item;
            if (type.Equals("TestQuestion"))
            {
                item = TestQuestion.GetTestQuestionByID(xID);
            } else
            {
                item = BankQuestion.GetQuestionByID(xID);
            }
            if (item == null) return;

            testQuestionPlaceHolder.Controls.Add(new System.Web.UI.LiteralControl(item.RenderToHTML(type)));

            // TFS Task #906 - Not necessary to display the standard name in BankQuestion item preveiw 
            if (type.Equals("TestQuestion"))
                standardPlaceHolder.Controls.Add(new System.Web.UI.LiteralControl(item.RenderStandardToHTML()));

            addendumPlaceHolder.Controls.Add(new System.Web.UI.LiteralControl(item.RenderAddendumToHTML()));
            rubricPlaceHolder.Controls.Add(new System.Web.UI.LiteralControl(item.RenderRubricToHTML()));
        }

    }
}