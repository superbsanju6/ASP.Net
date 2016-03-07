using System;
using System.Linq;
using Thinkgate.Base.Classes;
using System.Web;
using System.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Utilities;

namespace Thinkgate.Classes
{
    /// <summary>
    /// This is the Lowest level "base" page in our inheritance chain.  It automatically loads DistrictParms and the
    /// SessionObject, contains encrytion/decryption methods, methods to redirect the user and more.  It also contains
    /// several virtual methods that can be overridden if the base implementation will not work for your purposes.
    /// </summary>
    public class BasePage : Page
    {
        #region Variables

        private SessionObject _sessionObject;
        public ClientConfigHelper ConfigHelper;
        protected string EntityIdEncrypted = string.Empty;
        protected DistrictParms DistrictParms = DistrictParms.LoadDistrictParms();
        
        private String _key;

        #endregion

        #region Constants

        protected const String X_ID = "xID";
        protected const String Y_ID = "yID";
        public const int NUM_BYTES_IN_MEGABYTE = 1048576;
   
        #endregion

        #region Properties

        /// <summary>
        /// Property shared by all pages inheriting from base page.  It is created in the Page_Init event and
        /// stays valid for the lifecycle of the website.  Session Objects should no longer be created in child classes.
        /// This property should be referenced instead
        /// </summary>
        protected SessionObject SessionObject
        {
            get
            {
                if (_sessionObject == null)
                {
                    _sessionObject = (SessionObject)Session["SessionObject"];
                }

                return _sessionObject;
            }
            set { _sessionObject = value; }
        }

        /// <summary>
        /// Property shared by all Pages inheriting from BasePage.  This key is automatically assembled
        /// in BasePage and can be used in child pages to retreive preloaded records from the Cache.
        /// </summary>
        protected String Key
        {
            get { return _key; }
            set { _key = value; }

        }

        public string ItemThumbnailWebPathDistrict { get; set; }

        #endregion

        #region Page Events

        /// <summary>
        /// Performs all Base initialization for any of our child pages.  It initializes a reusable 
        /// SessionObject, it loads our reusable District Parameters, and performs client configuration
        /// to ready the website for operation
        /// </summary>
        /// <param name="sender">The object that fired this event</param>
        /// <param name="e">EventArgs passed to the eventHandler</param>
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
            
            SessionObject = (SessionObject)Session["SessionObject"];
            
            if (SessionObject != null)
            {
                SessionObject.CleanUpSession(Page.AppRelativeVirtualPath);
            }

            AppSettings.LoadAdditionalRequestLevelSettings(Page, Server, Request);

            // don't know why this is required on AssessmentItemsReorder.aspx, I reference this and it will not work if the variable is local to that page.
            ItemThumbnailWebPathDistrict = AppSettings.ItemThumbnailWebPath_District;

            DistrictParms = DistrictParms.LoadDistrictParms();
            ConfigHelper = new ClientConfigHelper(DistrictParms.ClientID);
            
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected T GetValueFromQueryString<T>(string parameter)
        {
            return IsQueryStringMissingParameter(parameter)
                ? default(T)
                : (T)Convert.ChangeType(Request.QueryString[parameter], typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlName"></param>
        /// <returns></returns>
        protected T FindPageControl<T>(string controlName)
        {
            return (T)(Page.FindControl(controlName) != null
                ? Convert.ChangeType(Page.FindControl(controlName), typeof(T))
                : default(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userControl"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        protected T FindPageControl<T>(string userControl, string controlName)
        {
            UserControl userCtrl = Page.FindControl(userControl) as UserControl;

            return (T)(userCtrl != null && userCtrl.FindControl(controlName) != null
                ? Convert.ChangeType(userCtrl.FindControl(controlName), typeof(T))
                : default(T));
        }

        /// <summary>
        /// This method provides a centralized located for retreiving the Encrypted Entity ID
        /// from the Query String
        /// </summary>
        /// <returns>The encypted ID in the Query String</returns>
        protected virtual String GetEncryptedEntityId(String entityIDKey)
        {
            EntityIdEncrypted = String.Empty;
            if (!IsQueryStringMissingParameter(entityIDKey))
            {
                EntityIdEncrypted = Request.QueryString[entityIDKey];
            }

            return EntityIdEncrypted;
        }

        /// <summary>
        /// This is the base implementation of decrypting the keys for our data objects
        /// It can be overridden if need be like in child classes in the case that
        /// the logic needs to differ
        /// </summary>
        /// <returns></returns>
        protected virtual int GetDecryptedEntityId(String entityIDKey)
        {
            EntityIdEncrypted = Request.QueryString[entityIDKey];
            if (EntityIdEncrypted.Count() < 20) return Convert.ToInt32(EntityIdEncrypted);
            int entityId = Cryptography.DecryptionToInt(EntityIdEncrypted, SessionObject.LoggedInUser.CipherKey);
            entityId = entityId == 0 ? Standpoint.Core.Classes.Encryption.DecryptStringToInt(EntityIdEncrypted) : entityId;
            EntityIdEncrypted = HttpUtility.UrlEncode(EntityIdEncrypted);
            return entityId;
        }

        /// <summary>
        /// This method should be overridden in each child of RecordPage
        /// so that each child can load its own data
        /// </summary>
        /// <param name="xId">The primary key of the object to be loaded</param>
        /// <returns>Whatever object type the child page needs to load</returns>
        protected virtual object LoadRecord(int xId)
        {
            return null;
        }

        /// <summary>
        /// This is the base implementation of Updating our Pages cache of data objects.
        /// It can be overridden if need be like in InstructionPlan in the case that
        /// extra logic needs to be implemented.  In most cases this method will suffice
        /// and will not need to be overridden
        /// </summary>
        /// <param name="key">The key of the object we are inserting into the cache</param>
        /// <param name="record">The data object we are inserting into the cache</param>
        protected virtual void UpdateCache(String key, object record)
        {
           Thinkgate.Base.Classes.Cache.Insert(key, record, null, DateTime.Now.AddHours(1), TimeSpan.Zero);
        }

        /// <summary>
        /// This Property should be overridden in any class inheriting from RecordPage
        /// in order to implement its own specific TypeKey
        /// </summary>
        protected virtual String TypeKey
        {
            get { return "RecordPage_"; }
        }

        /// <summary>
        /// Checks Request.QueryString to determine if it contains an entity ID
        /// </summary>
        /// <returns>true if the QueryString contains and ID; false otherwise</returns>
        protected virtual bool IsQueryStringMissingParameter(String parameter)
        {
            return Request.QueryString[parameter] == null;
        }

        /// <summary>
        /// Redirects the user to the PortalSelection page along with a Redirect Message of:
        /// "No entity ID provided in URL."
        /// </summary>
        protected void RedirectToPortalSelectionScreen()
        {
            SessionObject.RedirectMessage = "No entity ID provided in URL.";
            Response.Redirect("~/PortalSelection.aspx", true);
        }

        /// <summary>
        /// Redirects the user to the PortalSelection page along a custom redirect message.
        /// NOTE!!!  If the redirect message is going to be "No entity ID provided in URL.", use the overload with no
        /// arguments as that is the default redirect message
        /// </summary>
        /// <param name="redirectMessage">Custom redirect message to display to the user</param>
        protected void RedirectToPortalSelectionScreenWithCustomMessage(String redirectMessage)
        {
            SessionObject.RedirectMessage = redirectMessage;
            Response.Redirect("~/PortalSelection.aspx", true);
        }

        /// <summary>
        /// Redirects the user to the PortalSelection page without Fatal error message.
        /// </summary>
        protected void RedirectToPortalSelectionScreenWithoutErrorMesaage()
        {
            Response.Redirect("~/PortalSelection.aspx", true);
        }

        /// <summary>
        /// Reurn true if the currently logged in user has the passed permission.
        /// </summary>
        /// <param name="permission">The Permission we are checking for in the logged in user</param>
        /// <returns>Reurn true if the logged in user has the passed permission; false otherwise</returns>
        protected Boolean UserHasPermission(Permission permission)
        {
            if (SessionObject == null)
            {
                SessionObject = (SessionObject)Session["SessionObject"];
            }
            //if sessionobject is still null, send to login screen
            if (SessionObject == null || SessionObject.LoggedInUser == null)
            {
                Services.Service2.KillSession();
            }

            return SessionObject.LoggedInUser.HasPermission(permission);
        }

        /// <summary>
        /// This property is used to identify the ClientID of the running application
        /// </summary>
        /// <returns>ClientID of the running application</returns>
        protected string AppClient()
        {
            return (HttpRuntime.AppDomainAppVirtualPath == "/") ? "/" : HttpRuntime.AppDomainAppVirtualPath + "/";
        }

        /// <summary>
        /// This method can be called from any child class and will return weather or not a record
        /// exists in the Cache given its key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if the record exists in the Cache; false otherwise</returns>
        protected bool RecordExistsInCache(string key)
        {
            return (Thinkgate.Base.Classes.Cache.Get(key) != null);
        }

        /// <summary>
        /// This method decrypts the xID parameter in the QueryString, Assembles a Cache Key based on the decrypted xID,
        /// then attempts to load the appropriate record given the xID.  If the record is loaded it is then inserted into the Cache.
        /// 
        /// This method is called automatically during the initialization of any RecordPage.
        /// It is also called in the Page_Init event of SchoolIdentification_Edit, and ClassSummary_Expanded. 
        /// </summary>
        protected void LoadRecordObject()
        {
            RedirectOnMissingId();
            int xId = GetDecryptedEntityId(X_ID);
            RedirectOnInvalidId(xId);

            AssembleKey(xId);
            var record = LoadRecord(xId);

            if (record != null)
            {
                UpdateCache(Key, record);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is called automatically from LoadRecordObject() and Assembles the key needed to retreive the 
        /// appropriate record from the Cache.
        /// </summary>
        /// <param name="xId"></param>
        private void AssembleKey(int xId)
        {
            Key = TypeKey + xId;
        }

        /// <summary>
        /// Checks the decrypted value of the xID value extracted from Request.QueryString
        /// and redirects the user to the Portal Selection screen if xID is invalid or could not
        /// be decrypted.
        /// </summary>
        /// <param name="xId"></param>
        private void RedirectOnInvalidId(int xId)
        {
            if (xId == 0)
            {
                RedirectToPortalSelectionScreenWithoutErrorMesaage();
            }
        }

        /// <summary>
        /// Checks the Request.QueryString for an xID value.  If the QueryString has no xID value
        /// the website will redirect the user to the Portal Selection Screen.
        /// </summary>
        private void RedirectOnMissingId()
        {
            if (Request.QueryString["xID"] == null)
            {
                RedirectToPortalSelectionScreen();
            }
        }

        #endregion

        public bool CanValueBeExtractedFromQueryString(string parameter, out int result)
        {
            if (Request.QueryString[parameter] != null)
            {
                var value = Request.QueryString[parameter];
                if (Int32.TryParse(value, out result))
                {
                    return true;
                }
            }

            result = 0;
            return false;
        }

    }
}
