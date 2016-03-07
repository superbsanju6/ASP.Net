using System;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Web.UI;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Web.Configuration;
using Telerik.Web.UI;
using Thinkgate.Utilities;

namespace Thinkgate
{
    public partial class PortalSelection : Page
    {
        private SessionObject obj;
        protected void Page_Load(object sender, EventArgs e)
        {
            obj = (SessionObject)Session["SessionObject"];
            if (!Page.IsPostBack)
            {
                if (!Request.IsAuthenticated)
                {
                    // This is an unauthorized, authenticated request...
                    Response.Redirect("~/UnauthorizedAccess.aspx");
                }

                ThinkgateRole role = obj.LoggedInUser.Roles.Where(r => r.RoleName.Equals("Super Admin", StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                bool isSuperAdmin = role != null;
                bool isMessageCenterEnabled = IsMessageCenterEnabled();
                if ((!(bool)Session["MessageCenterPopupVisited"]) && !isSuperAdmin && isMessageCenterEnabled)
                {
                    Response.Redirect("~/MessageCenterPopup.aspx");
                }

                int tilesPerRow = string.IsNullOrEmpty(AppSettings.PortalSelection_Portals_Per_Row.ToString()) ? 6 : AppSettings.PortalSelection_Portals_Per_Row;
                LoadSearchScripts();
                BuildPortalIcons(tilesPerRow);
            }
        }

        private bool IsMessageCenterEnabled()
        {
            bool isMessageCenterEnabled = true;
            string isMessageCenterEnabledString;

            try
            {
                isMessageCenterEnabledString = WebConfigurationManager.AppSettings["MessageCenterEnabled"];
                bool.TryParse(isMessageCenterEnabledString, out isMessageCenterEnabled);
            }
            catch
            {
                // TODO:  Log exception to insturmentation framework
            }

            return isMessageCenterEnabled;
        }

        private void LoadSearchScripts()
        {
            if (Master != null)
            {
                RadScriptManager radScriptManager = default(RadScriptManager);

                Control scriptManager = Master.FindControl("RadScriptManager1");

                if (scriptManager != null)
                {
                    radScriptManager = (RadScriptManager)scriptManager;
                    radScriptManager.Services.Add(new ServiceReference("Services/LoginService.svc"));
                }
            }
        }

        protected void BuildPortalIcons(int iconsPerRow)
        {
            if (obj == null)
            {
                Session.Abandon();
                return;
            }

            // Show Demo Icons only for tgDemoAdmin
            if (obj.LoggedInUser.UserName == AppSettings.PortalSelection_Demo_User)
            {
                DemoPortalIcons.Style.Add("display", "block");
                PortalIcons.Style.Add("display", "none");
            }
            else
            {
                // Make sure we clear out HTML to prevent users from viewing source and potential hack
                DemoPortalIcons.InnerHtml = string.Empty;
            }

            int iconCounter = 0;
            string selectionList = string.Empty;

            DataTable portalSelectionRoles = ThinkgateRole.GetAllRolePortalSelections();

            if (obj.LoggedInUser.Roles.Count > 1)
            {
                selectionList += @"<ul id=""mainNavMenu"">";

                foreach (DataRow drPortalSelectionRoles in portalSelectionRoles.Rows)
                {
                    if (LoggedInUserHasPortalSelectionRole(DataIntegrity.ConvertToInt(drPortalSelectionRoles["ID"])))
                    {
                        // School
                        if (drPortalSelectionRoles["PortalName"].ToString() == "School")
                        {
                            for (int i = 0; i < DataIntegrity.ConvertToInt(obj.LoggedInUser.Schools.Count); i++)
                            {
                                selectionList += @"<li style=""width:100px;""><input type=""image"" ID=""classImgBtn"" 
                                                    src=" + drPortalSelectionRoles["PortalStartPageIcon"].ToString() + @" 
                                                    OnClick=""ImageButtonClick('" + drPortalSelectionRoles["ID"] + "','" + drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.Schools[i].Id.ToString(), obj.LoggedInUser.CipherKey)) + "','" + obj.LoggedInUser.Schools[i].Id + @"'); return false; ""/><br/><a style=""color: black;"">" + obj.LoggedInUser.Schools[i].Name + @"</a></li>";
                                iconCounter++;
                                if (iconCounter % iconsPerRow == 0) { selectionList += @"</ul><ul id=""mainNavMenu"">"; }
                            }
                        }
                        else if (drPortalSelectionRoles["PortalName"].ToString() == "District")
                        {
                            PropertyInfo myClassType = obj.LoggedInUser.GetType().GetProperty("District");
                            var myvalue = myClassType.GetValue(obj.LoggedInUser, null);
                            selectionList += @"<li style=""width:100px;""><input type=""image"" ID=""classImgBtn"" 
                                                    src=" + drPortalSelectionRoles["PortalStartPageIcon"].ToString() + @" 
                                                    OnClick=""ImageButtonClick('" + drPortalSelectionRoles["ID"] + "','" + drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(myvalue.ToString(), obj.LoggedInUser.CipherKey)) + "'," + "'1'" + @"); return false; ""/><br/><a style=""color: black;"">" + drPortalSelectionRoles["PortalName"] + @"</a></li>";

                            iconCounter++;
                            if (iconCounter % iconsPerRow == 0) { selectionList += @"</ul><ul id=""mainNavMenu"">"; }
                        }
                        else if (drPortalSelectionRoles["PortalName"].ToString() == "Teacher")
                        {
                            selectionList += @"<li style=""width:100px;""><input type=""image"" ID=""classImgBtn"" 
                                                    src=" + drPortalSelectionRoles["PortalStartPageIcon"].ToString() + @" 
                                                    OnClick=""ImageButtonClick('" + drPortalSelectionRoles["ID"] + "','" + drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.Page.ToString(), obj.LoggedInUser.CipherKey)) + "','" + obj.LoggedInUser.School + @"'); return false; ""/><br/><a style=""color: black;"">" + drPortalSelectionRoles["PortalName"] + @"</a></li>";

                            iconCounter++;
                            if (iconCounter % iconsPerRow == 0) { selectionList += @"</ul><ul id=""mainNavMenu"">"; }
                        }
                        else if (drPortalSelectionRoles["PortalName"].ToString() == "IMC" || drPortalSelectionRoles["PortalName"].ToString() == "RegionalCoordinator" || drPortalSelectionRoles["PortalName"].ToString() == "SectionChief" || drPortalSelectionRoles["PortalName"].ToString() == "LCO Administrator")
                        {
                            selectionList += @"<li style=""width:60px;""><input type=""image"" ID=""classImgBtn"" src=" + drPortalSelectionRoles["PortalStartPageIcon"].ToString() + @" OnClick=""window.location.href='" + drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.Page.ToString(), obj.LoggedInUser.CipherKey)) + @"'; return false;"" /><br/><a style=""color: black;"">" + drPortalSelectionRoles["PortalName"].ToString() + @" Portal</a></li>";
                            iconCounter++;
                            if (iconCounter % iconsPerRow == 0) { selectionList += @"</ul><ul id=""mainNavMenu"">"; }
                        }
                        //else if (drPortalSelectionRoles["PortalName"].ToString() == "System Administration")
                        else if (drPortalSelectionRoles["PortalName"].ToString() == "Parent Portal")
                        {
                            selectionList += @"<li style=""width:100px;""><input type=""image"" ID=""classImgBtn"" src=" + drPortalSelectionRoles["PortalStartPageIcon"].ToString() + @" OnClick=""window.location.href='" + drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.Page.ToString(), obj.LoggedInUser.CipherKey)) + @"'; return false;"" /><br/><a style=""color: black;"">" + drPortalSelectionRoles["PortalName"].ToString() + @"</a></li>";
                            iconCounter++;
                            if (iconCounter % iconsPerRow == 0) { selectionList += @"</ul><ul id=""mainNavMenu"">"; }
                        }
                        else
                        {
                            selectionList += @"<li style=""width:100px;""><input type=""image"" ID=""classImgBtn"" src=" + drPortalSelectionRoles["PortalStartPageIcon"].ToString() + @" OnClick=""window.location.href='" + drPortalSelectionRoles["PortalStartPage"].ToString() + @"'; return false;"" /><br/><a style=""color: black;"">" + drPortalSelectionRoles["PortalName"].ToString() + @" Portal</a></li>";
                            iconCounter++;
                            if (iconCounter % iconsPerRow == 0) { selectionList += @"</ul><ul id=""mainNavMenu"">"; }
                        }
                    }
                }

                selectionList += @"</ul>";
                PortalIcons.InnerHtml = selectionList;
            }
            else if (obj.LoggedInUser.Roles.Count == 1)
            {

                selectionList += @"<ul id=""mainNavMenu"">";
                string schoolid = string.Empty;
                foreach (DataRow drPortalSelectionRoles in portalSelectionRoles.Rows)
                {
                    if (LoggedInUserHasPortalSelectionRole(DataIntegrity.ConvertToInt(drPortalSelectionRoles["ID"])))
                    {
                        // School
                        if (drPortalSelectionRoles["PortalName"].ToString() == "School")
                        {
                            for (int i = 0; i < DataIntegrity.ConvertToInt(obj.LoggedInUser.Schools.Count); i++)
                            {
                                selectionList += @"<li style=""width:100px;""><input type=""image"" ID=""classImgBtn"" src=" + drPortalSelectionRoles["PortalStartPageIcon"].ToString() + @" OnClick=""window.location.href='" + drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.Schools[i].Id.ToString(), obj.LoggedInUser.CipherKey)) + @"'; return false;"" /><br/><a style=""color: white;"">" + obj.LoggedInUser.Schools[i].Name + @"</a></li>";
                                iconCounter++;
                                if (iconCounter % iconsPerRow == 0) { selectionList += @"</ul><ul id=""mainNavMenu"">"; }
                                schoolid = obj.LoggedInUser.Schools[i].Id.ToString();
                            }

                            if (iconCounter == 1)
                            {
                                Response.Redirect(drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(schoolid, obj.LoggedInUser.CipherKey)));
                            }
                        }
                        else if (drPortalSelectionRoles["PortalName"].ToString() == "District")
                        {
                            if (string.IsNullOrEmpty(obj.LoggedInUser.District.ToString()) || obj.LoggedInUser.District == 0)
                                ShowErrorAlert("Your account has a configuration issue.  Please contact support.  Error code: D-0");
                            else
                                Response.Redirect(drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.District.ToString(), obj.LoggedInUser.CipherKey)));
                        }
                        else if (drPortalSelectionRoles["PortalName"].ToString() == "Teacher")
                        {
                            if (string.IsNullOrEmpty(obj.LoggedInUser.Page.ToString()) || obj.LoggedInUser.Page == 0)
                                ShowErrorAlert("Your account has a configuration issue.  Please contact support.  Error code: P-0");
                            else
                                Response.Redirect(drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.Page.ToString(), obj.LoggedInUser.CipherKey)));
                        }
                        else if (drPortalSelectionRoles["PortalName"].ToString() == "IMC" || drPortalSelectionRoles["PortalName"].ToString() == "RegionalCoordinator" || drPortalSelectionRoles["PortalName"].ToString() == "SectionChief" || drPortalSelectionRoles["PortalName"].ToString() == "LCO Administrator")
                        {
                            if (string.IsNullOrEmpty(obj.LoggedInUser.Page.ToString()) || obj.LoggedInUser.Page == 0)
                                ShowErrorAlert("Your account has a configuration issue.  Please contact support.  Error code: P-0");
                            else
                                Response.Redirect(drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.Page.ToString(), obj.LoggedInUser.CipherKey)));
                        }
                        else if (drPortalSelectionRoles["PortalName"].ToString() == "Parent Portal")
                        {
                            selectionList += @"<li style=""width:100px;""><input type=""image"" ID=""classImgBtn"" src=" + drPortalSelectionRoles["PortalStartPageIcon"].ToString() + @" OnClick=""window.location.href='" + drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.Page.ToString(), obj.LoggedInUser.CipherKey)) + @"'; return false;"" /><br/><a style=""color: black;"">" + drPortalSelectionRoles["PortalName"].ToString() + @"</a></li>";
                            iconCounter++;
                            if (iconCounter % iconsPerRow == 0) { selectionList += @"</ul><ul id=""mainNavMenu"">"; }
                        } 

                        else if (drPortalSelectionRoles["PortalName"].ToString() == "State")
                        {
                            if (string.IsNullOrEmpty(obj.LoggedInUser.District.ToString()) || obj.LoggedInUser.District == 0)
                                ShowErrorAlert("Your account has a configuration issue.  Please contact support.  Error code: D-1");
                            else
                                Response.Redirect(drPortalSelectionRoles["PortalStartPage"].ToString().Replace("{*xID*}", Cryptography.EncryptString(obj.LoggedInUser.District.ToString(), obj.LoggedInUser.CipherKey)));
                        }
                        else
                        {
                            Response.Redirect(drPortalSelectionRoles["PortalStartPage"].ToString());
                        }
                    }
                }

                selectionList += @"</ul>";
                PortalIcons.InnerHtml = selectionList;
            }
        }

        protected void ShowErrorAlert(string sMessage)
        {
            Thinkgate.SiteMaster smaster = (Thinkgate.SiteMaster)this.Master;
            smaster.ShowFatalRadNotification(sMessage);
        }


        protected bool LoggedInUserHasPortalSelectionRole(int role)
        {
            if (obj == null)
            {
                Session.Abandon();
                return false;
            }

            if (obj.LoggedInUser.Roles != null)
            {
                for (int i = 0; i < DataIntegrity.ConvertToInt(obj.LoggedInUser.Roles.Count); i++)
                {
                    if (obj.LoggedInUser.Roles[i].RolePortalSelection == role)
                        return true;
                }
            }

            return false;
        }

        protected bool LoggedInUserHasRole(string role)
        {
            if (obj == null)
            {
                Session.Abandon();
                return false;
            }

            if (obj.LoggedInUser.Roles != null)
            {
                for (int i = 0; i < DataIntegrity.ConvertToInt(obj.LoggedInUser.Roles.Count); i++)
                {
                    if (obj.LoggedInUser.Roles[i].RoleName == role)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void teacherImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            obj.LoggedInUser.TeacherID = AppSettings.Demo_TeacherID.ToString();
            var teacherID = Cryptography.EncryptString(AppSettings.Demo_TeacherID.ToString(), obj.LoggedInUser.CipherKey);
            Response.Redirect("~/Record/Teacher.aspx?xID=" + teacherID, true);
        }

        protected void classImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var classID = Cryptography.EncryptString(AppSettings.Demo_ClassID.ToString(), obj.LoggedInUser.CipherKey);
            Response.Redirect("~/Record/Class.aspx?xID=" + classID, true);
        }

        protected void stateImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var districtID = Cryptography.EncryptString(AppSettings.Demo_DistrictID.ToString(), obj.LoggedInUser.CipherKey);
            //Response.Redirect("~/Record/StateAnalysis.aspx?xID=" + districtID, true);
            Response.Redirect("~/Record/State.aspx?xID=" + districtID, true);
        }

        protected void districtImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var districtID = Cryptography.EncryptString(AppSettings.Demo_DistrictID.ToString(), obj.LoggedInUser.CipherKey);
            Response.Redirect("~/Record/District.aspx?xID=" + districtID, true);
        }

        protected void studentImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var studentID = Cryptography.EncryptString(AppSettings.Demo_StudentID.ToString(), obj.LoggedInUser.CipherKey);
            Response.Redirect("~/Record/Student.aspx?xID=" + studentID, true);
        }

        protected void schoolImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var schoolID = Cryptography.EncryptString(AppSettings.Demo_SchoolID.ToString(), obj.LoggedInUser.CipherKey);
            Response.Redirect("~/Record/School.aspx?xID=" + schoolID, true);
        }

        protected void assessmentImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            var assessmentID = Cryptography.EncryptString(AppSettings.Demo_TestID.ToString(), obj.LoggedInUser.CipherKey);
            Response.Redirect("~/Record/AssessmentPage.aspx?xID=" + assessmentID, true);
        }

        protected void btnLCO_Click(object sender, ImageClickEventArgs e)
        {
            var studentID = Cryptography.EncryptString(AppSettings.Demo_TeacherID.ToString(), obj.LoggedInUser.CipherKey);
            switch (((System.Web.UI.WebControls.ImageButton)sender).ID.ToString().ToLower().Trim())
            {
                case "btnimc":
                    Response.Redirect("~/Record/IMC.aspx?xID=" + studentID, true);
                    break;
                case "btnregional":
                    Response.Redirect("~/Record/RegionalCoordinator.aspx?xID=" + studentID, true);
                    break;
                case "btnsection":
                    Response.Redirect("~/Record/SectionChief.aspx?xID=" + studentID, true);
                    break;
                case "btnlcoadmin":
                    Response.Redirect("~/Record/LCOAdministrator.aspx?xID=" + studentID, true);
                    break;
                default:
                    break;
            }
        }
    }
}
