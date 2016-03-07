
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Credentials
{
    public enum StudentCredentialsAddRemove { Add, Remove, RemoveTemp }

    [ScriptService]
    public class CredentialWebServices : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public string CreateCredentialAssociation(string currentStatus, int studentCredentialId, int credentialId, int studentId)
        {
            SessionObject session = (SessionObject)HttpContext.Current.Session["SessionObject"];
            bool saveStatus = false;


            if (currentStatus.Equals("Select", System.StringComparison.InvariantCultureIgnoreCase))
            {                
                 /* Create association */
                    saveStatus = Base.Classes.Credentials.AddRemoveCredentialWithStudent(studentId, credentialId, session.LoggedInUser.Page, StudentCredentialsAddRemove.Add.ToString()); 
            }
            else if (currentStatus.Equals("Unselect", System.StringComparison.InvariantCultureIgnoreCase))
            {
                /* Delete association and data */
                  //  saveStatus = Base.Classes.Credentials.AddRemoveCredentialWithStudent(studentId, credentialId, session.LoggedInUser.Page, StudentCredentialsAddRemove.Remove.ToString());
                /*Above code is commented becoz on unselect only user interface need to change and actual database change will take place when user hits update button.*/
                saveStatus = true;
            }
            return (new JavaScriptSerializer()).Serialize(new { PreviousStatus = currentStatus, NewStatus = currentStatus == "Select" ? "Unselect" : "Select", SaveStatus = saveStatus });
        }

        [WebMethod(EnableSession = true)]
        public string RemoveTemporaryCredentialAssociation(int studentId)
        {
            SessionObject session = (SessionObject)HttpContext.Current.Session["SessionObject"];
            bool saveStatus = false;
            saveStatus = Base.Classes.Credentials.AddRemoveCredentialWithStudent(studentId, 0, session.LoggedInUser.Page, StudentCredentialsAddRemove.RemoveTemp.ToString());
            
            return (new JavaScriptSerializer()).Serialize(new { SaveStatus = saveStatus });
        }

    }
}
