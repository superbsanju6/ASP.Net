using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;


namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public partial class ContentEditor_Addendum_Questions : BasePage
    {
        public List<Base.Classes.BankQuestion> _questionList;
        private dtItemBank _itemBankTbl;

        protected void Page_Load(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
            int AddendumID = GetDecryptedEntityId(X_ID);
            _itemBankTbl = ItemBankMasterList.GetItemBanksForUser(SessionObject.LoggedInUser, ThinkgatePermission.PermissionLevelValues.ItemBankEdit, "Search");
            string ItemReservation = "No";
            if (UserHasPermission(Base.Enums.Permission.Access_ItemReservation))
            {
                ItemReservation = "Yes";
            }
            _questionList = Base.Classes.BankQuestion.GetQuestionsByAddendumID(AddendumID, ItemReservation, _itemBankTbl);

        }

        protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            RadGrid1.DataSource = _questionList;
        }

        protected void RadGrid1_ItemDataBound1(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                Thinkgate.Base.Classes.QuestionBase qb = ((Thinkgate.Base.Classes.QuestionBase)(((Telerik.Web.UI.GridItem)(dataItem)).DataItem));
                string ds = "";
                if (qb.Responses.Count > 0)
                {
                    int i = 1;
                    foreach (AssessmentItemResponse r in qb.Responses)
                    {
                        ds += r.Letter + ". " + r.DistractorText + "<br/>";
                        i++;
                    }
                }
                //string ds = dataItem["AddendumID"].Text + "<br/>";// +dataItem["Answer_Text2"].Text;
                
                dataItem["Distractors"].Text = ds.ToString();
            }

            if (e.Item is GridFooterItem)
            {
                
            }
        } 


    }
}