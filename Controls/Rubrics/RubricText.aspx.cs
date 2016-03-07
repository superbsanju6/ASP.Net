using System;
using Thinkgate.Base.Classes;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.Controls.Rubrics
{
    public partial class RubricText : System.Web.UI.Page
    {
        public SessionObject SessionObject;
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject) Session["SessionObject"];
            if (!IsPostBack)
            {
                LoadRubricText();
            }
        }

        protected void LoadRubricText()
        {
            
			if (Request.QueryString["xID"] != null)
			{
				int rubricId = DataIntegrity.ConvertToInt(Request.QueryString["xID"]);
				if (rubricId != 0)
				{
					var rubric = Rubric.GetRubricByID(rubricId);
					if (rubric == null)
					{
						lblRubricText.Text = "Specified rubric not found.";
						return;
					}
					lblRubricText.Text = rubric.Content;
					PageTitle.Text = rubric.Name;
				}

			}
			else if (Request.QueryString["yID"] != null)
			{
				var ItemID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey, "yID");

				if (ItemID != 0 && ItemID != 0)
				{
					var item = TestQuestion.GetTestQuestionByID(ItemID);
					if (item.Rubric == null)
					{
						lblRubricText.Text = "Specified rubric not found.";
						return;
					}
					lblRubricText.Text = item.Rubric.Content;
					PageTitle.Text = item.Rubric.Name;
				}
			}
        }
    }
}