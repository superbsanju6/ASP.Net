using System;
using System.Web.Security;

namespace Thinkgate.Account
{
    public partial class RegistrationAscx : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void CreateAccountButton_Click(object sender, EventArgs e)
        {
            MembershipCreateStatus createStatus;
        	bool autoApprove = this.Page.User.IsInRole("Administrators");

        	MembershipUser newUser =
                System.Web.Security.Membership.CreateUser(Username.Text, Password.Text,
                                                          Email.Text, "Color",
                                                          "Red", autoApprove,
                                                          out createStatus);

            switch (createStatus)
            {
                case MembershipCreateStatus.Success:
                    CreateAccountResults.Text = "The user account was successfully created!";
					if(autoApprove) SendMail();
                    break;

                case MembershipCreateStatus.DuplicateUserName:
                    CreateAccountResults.Text = "There already exists a user with this username.";
                    break;

                case MembershipCreateStatus.DuplicateEmail:
                    CreateAccountResults.Text = "There already exists a user with this email address.";
                    break;

                case MembershipCreateStatus.InvalidEmail:
                    CreateAccountResults.Text = "There email address you provided in invalid.";
                    break;

                case MembershipCreateStatus.InvalidAnswer:
                    CreateAccountResults.Text = "There security answer was invalid.";
                    break;

                case MembershipCreateStatus.InvalidPassword:
                    CreateAccountResults.Text =
                        "The password you provided is invalid. It must be seven characters long and have at least one non-alphanumeric character.";
                    break;

                default:
                    CreateAccountResults.Text = "There was an unknown error; the user account was NOT created.";
                    break;
            }
        }

		protected void SendMail()
		{
			
		}

    }
}
