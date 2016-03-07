
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.DocumentEngine;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;

namespace Thinkgate
{
    public partial class MessageCenterPopup : Thinkgate.Classes.BasePage
    {
        #region Private Variables

        private static List<Thinkgate.Base.Classes.MessageCenter> messageCenter = new List<Thinkgate.Base.Classes.MessageCenter>();
        private static int messageCenterIndex;
        private int messageCenterNodeID = 0;

        #endregion

        #region Event Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.Get("MessageCenterNodeID") != null)
            {
                messageCenterNodeID = DataIntegrity.ConvertToInt(Request.QueryString.Get("MessageCenterNodeID"));
                if (messageCenterNodeID > 0)
                {
                    messageCenter = new List<Thinkgate.Base.Classes.MessageCenter>();
                    messageCenter = Thinkgate.Base.Classes.MessageCenter.GetMessageCenter(ConfigurationManager.AppSettings["CMSSiteName"], ConfigurationManager.AppSettings["CMSTreePathToMessages"], "TG.MessageCenter", ConfigurationManager.AppSettings["CMSSiteUrl"], SessionObject.LoggedInUser.UserName.ToString(), SessionObject.LoggedInUser.UserId.ToString(), "DocumentNodeID = '" + messageCenterNodeID + "' ", string.Empty, false);
                    if (messageCenter != null && messageCenter.Count > 0)
                    {
                        /* Mark this as view popup and close on OK click. */
                        messageCenterIndex = -1;
                        messageContainer.Style.Add("width", "740px;");
                        LoadMessageCenter();
                        MessageCenterFooter.Visible = false;
                        UserAgreementFooter.Visible = false;
                    }
                }
            }
            else
            {
                if (!IsPostBack)
                {
                    Session["MessageCenterPopupVisited"] = true;
                    messageCenterIndex = -1;
                    messageContainer.Style.Add("width", "1000px;");
                    GetMessageCenter();
                    LoadMessageCenter();
                }
            }
        }

        private void GetMessageCenter()
        {
            messageCenter = new List<Thinkgate.Base.Classes.MessageCenter>();
            messageCenter = Thinkgate.Base.Classes.MessageCenter.GetMessageCenter(ConfigurationManager.AppSettings["CMSSiteName"], ConfigurationManager.AppSettings["CMSTreePathToMessages"], "TG.MessageCenter", ConfigurationManager.AppSettings["CMSSiteUrl"], SessionObject.LoggedInUser.UserName.ToString(), SessionObject.LoggedInUser.UserId.ToString(), "ClientTargets Is Not Null", "MessageCenterEnum DESC");
            List<Thinkgate.Base.Classes.MessageCenter> temp = new List<Base.Classes.MessageCenter>();
            if (messageCenter != null && messageCenter.Count > 0)
            {
                string clientId = DistrictParms.LoadDistrictParms().ClientID;
                bool hasPermission = UserHasPermission(Permission.Add_Message);
                string currentUserRoleName = SessionObject.LoggedInUser.Roles[0].RoleName;
                bool isTeacher = currentUserRoleName.Equals("teacher", StringComparison.InvariantCultureIgnoreCase);
                bool includeForAllClients = false;
                bool includeMessage = false;
                bool messageAvailableToUser = false;

                foreach (Base.Classes.MessageCenter message in messageCenter)
                {
                    includeMessage = false;
                    messageAvailableToUser = false;

                    includeForAllClients = message.ClientTargets.ToString().ToUpperInvariant() == "ALL";
                    if (includeForAllClients || message.ClientTargets.ToString().Split('|').Where(c => c.ToString() == clientId).Count() > 0)
                        {
                            if (DataIntegrity.ConvertToInt(message.UserGroupEnum.ToString()) == 1)
                            {
                                messageAvailableToUser = true;
                            }
                            if (DataIntegrity.ConvertToInt(message.UserGroupEnum.ToString()) == 3 && isTeacher == true)
                            {
                                messageAvailableToUser = true;
                            }
                            if (DataIntegrity.ConvertToInt(message.UserGroupEnum.ToString()) == 2 && isTeacher == false)
                            {
                                messageAvailableToUser = true;
                            }
                            if (messageAvailableToUser)
                            {
                                if (message.PostOn == null && message.RemoveOn == null)
                                { includeMessage = true; }

                                if (message.PostOn == null && (message.RemoveOn != null && DateTime.Now.Date <= message.RemoveOn))
                                {
                                    includeMessage = true;
                                }
                                if (message.RemoveOn == null && (message.PostOn != null && DateTime.Now.Date >= message.PostOn))
                                {
                                    includeMessage = true;
                                }
                                if (DateTime.Now.Date >= message.PostOn && DateTime.Now.Date <= message.RemoveOn)
                                {
                                    includeMessage = true;
                                }
                            }
                            if (includeMessage)
                            {
                                temp.Add(message);
                            }
                        }
                    }
                messageCenter = new List<Base.Classes.MessageCenter>();
                messageCenter = temp;
            }
        }

        protected void IAgree_Click(object sender, EventArgs e)
        {
            if (messageCenterIndex < messageCenter.Count)
            {
                messageCenter[messageCenterIndex].UserResponse = Base.Enums.UserResponse.Agree;
                LoadMessageCenter();
            }
        }

        protected void IDisagree_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void DoNotShow_CheckedChanged(object sender, EventArgs e)
        {
            if (messageCenterIndex < messageCenter.Count)
            {
                messageCenter[messageCenterIndex].UserResponse = DoNotShow.Checked ? Base.Enums.UserResponse.DoNotShow : Base.Enums.UserResponse.Show;
                DoNotShow.Checked = false;
            }
        }

        protected void OK_Click(object sender, EventArgs e)
        {
            LoadMessageCenter();
        }

        #endregion

        #region Private Methods

        private void SaveUserRespone()
        {
            string userMappingTableClassName = "TG.MessageCenterUserMapping";

            if (messageCenter != null && messageCenter.Count > 0)
            {
                foreach (Thinkgate.Base.Classes.MessageCenter message in messageCenter)
                {
                    Thinkgate.Base.Classes.MessageCenter.UpdateMessageCenterUserAcceptance(userMappingTableClassName, message.DocumentNodeID, SessionObject.LoggedInUser.UserId.ToString(),SessionObject.LoggedInUser.UserName, message.UserResponse);
                }
            }
            Response.Redirect("~/PortalSelection.aspx");
        }

        private void LoadMessageCenter()
        {
            messageCenterIndex++;
            if (messageCenterIndex < messageCenter.Count)
            {
                messageCenter[messageCenterIndex].UserResponse = Base.Enums.UserResponse.Show;
                Description.Text = messageCenter[messageCenterIndex].Description;
                Title.Text = messageCenter[messageCenterIndex].Title;
                if (messageCenter[messageCenterIndex].MessageCenterEnum == Base.Enums.LookupDetail.Message)
                {
                    
                    UserAgreementFooter.Visible = false;
                    MessageCenterFooter.Visible = true;
                }
                else if (messageCenter[messageCenterIndex].MessageCenterEnum == Base.Enums.LookupDetail.UserAgreement)
                {
                    UserAgreementFooter.Visible = true;
                    MessageCenterFooter.Visible = false;
                }
                rptAttachments.DataSource = messageCenter[messageCenterIndex].Attachments;
                rptAttachments.DataBind();
            }
            else
            {
                SaveUserRespone();
            }
        }

        #endregion

    }
}