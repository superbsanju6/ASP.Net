using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CMS.FormEngine;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Core.Constants;
using Thinkgate.Domain.Classes;
using CMS.DataEngine;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Services.Contracts.ConfigurationService;

namespace Thinkgate.Classes
{
	public class KenticoHelper
	{
        public static string KenticoUserName = string.Empty; 

		private const int DEFAULT_NUMBER_OF_RECORDS_TO_RETURN = 300;
        private const int DEFAULT_NUMBER_OF_RECORDS_ON_TILE = 50;
		private const string FORWARD_SLASH = "/";
        public static readonly string KenticoVirtualFolder = System.Web.Configuration.WebConfigurationManager.AppSettings["KenticoVirtualFolder"];
        public static readonly string KenticoVirtualFolderRelative = string.Format("/{0}", KenticoVirtualFolder);
        private const string DISTRICTS = "Districts";
        private const string STATE_DOCUMENTS = "Documents";
        private const string USERS = "Users";
        //US15667
        private const string SHARED = "Shared";
        private static readonly string CmsDeskRootPath = KenticoVirtualFolder + "/CMSModules/Content/CMSDesk/Edit/edit.aspx?action=new&parentculture=en-US";


        public class CurriculaAssociations
        {
            public int NodeId { get; set; }
            public int CurriculaId { get; set; }
        }

        public class StandardAssociations
        {
            public int NodeId { get; set; }
            public int StandardId { get; set; }
        }
        
		private static string GetKenticoCmsDeskEditUrl()
		{
            //TFS 21010 - Modfication was made to make sure we are getting the correct base URI.  The code was getting the base from the AppSetting KenticoServerURL
            string hostURL = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") + HttpContext.Current.Request.Url.Host;
            var baseUri = new Uri(hostURL);
            var completeUri = new Uri(baseUri, CmsDeskRootPath);
            return completeUri.ToString();
		}


        //US15667
        internal static int GetEnvironmentBaseTreeNodeID(string clientID, string aliasPath, string nodeName, TreeProvider treeProvider, string environmentRoot)
        {            
            string userAliasPath = aliasPath;            

            TreeNode node;
            try
            {
                string tmpstr = CleanUserAliasPath(userAliasPath);
                node = treeProvider.SelectSingleNode(CMSContext.CurrentSiteName, tmpstr, "en-us");
            }
            catch (Exception)
            {
                throw new Exception("Error locating node");
            }
            if (node != null)
            {
                int theUsersBaseTreeNodeID = node.NodeID;
                if (theUsersBaseTreeNodeID >= 0)
                {
                    return theUsersBaseTreeNodeID;
                }
                    throw new Exception("Node < 0");
                }
                int nodeToAddUserFolderTo = treeProvider.SelectSingleNode(CMSContext.CurrentSiteName, environmentRoot, "en-us").NodeID;
                //add folder as a child of nodeToAddUserFolderTo
                if (nodeToAddUserFolderTo >= 0)
                {
                    TreeNode newNode = TreeNode.New("CMS.Folder", treeProvider);

                    if (newNode != null)
                    {                        
                        newNode.DocumentName = clientID; 
                        newNode.DocumentCulture = "en-us";
                        newNode.Insert(nodeToAddUserFolderTo);                        

                        List<ThinkgateRole> thinkgateRoles = ThinkgateRole.GetRolesCollectionForApplication(1);
                        string clientId = DistrictParms.LoadDistrictParms().ClientID;
                        foreach (var role in thinkgateRoles)
                        {
                        DataRow[] rows = AdministrationDB.GetPermissionsInRoleID(role.RoleId).Select("Label like '%Shared%'");
                        int allowUser = 0;
                            AclProvider aclUser = new AclProvider(treeProvider);                            
                            
                            string kenticoRoleName=clientId.ToUpper() + "-Admin-".ToUpper() + role.RoleName.Replace(" ", "_").ToUpper();                            
                            RoleInfo roleInfo=RoleInfoProvider.GetRoleInfo(kenticoRoleName,CMSContext.CurrentSiteName);
                            bool isPermissionGiven = false;
                        foreach (DataRow row in rows)
                            {
                            if (row.ItemArray[1].ToString() == Permission.Create_Access_Shared_IM.ToString() && Convert.ToBoolean(row.ItemArray[2]))
                                {
                                    allowUser += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Create)));
                                    isPermissionGiven = true;
                                }
                            if (row.ItemArray[1].ToString() == Permission.Read_Access_Shared_IM.ToString() && Convert.ToBoolean(row.ItemArray[2]))
                                {
                                    allowUser += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Read)));
                                    isPermissionGiven = true;
                                }
                            if (row.ItemArray[1].ToString() == Permission.Delete_Access_Shared_IM.ToString() && Convert.ToBoolean(row.ItemArray[2]))
                                {
                                    allowUser += Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Delete)));
                                    isPermissionGiven = true;
                                }
                                
                            }
                        if (isPermissionGiven)
                            {
                            int deniedUser = Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Modify))) + Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.ModifyPermissions)));
                                aclUser.SetRolePermissions(newNode, allowUser, deniedUser, roleInfo.RoleID);
                            }
                        }                                     
                    }
                string tmpstr = CleanUserAliasPath(userAliasPath);
                    return treeProvider.SelectSingleNode(CMSContext.CurrentSiteName, tmpstr, "en-us").NodeID;
                }
                throw new Exception("Node < 0");
            }


		internal static int GetUsersBaseTreeNodeID(string clientID, string aliasPath, string userName, TreeProvider treeProvider)
		{

			string userAliasPath = BuildAliasPath(aliasPath, userName);


			TreeNode node;
			try
			{
				string tmpstr = CleanUserAliasPath(userAliasPath);
				node = treeProvider.SelectSingleNode(CMSContext.CurrentSiteName, tmpstr, "en-us");
			}
			catch (Exception)
			{
				throw new Exception("Error locating node");
			}

			if (node != null)
			{
				int theUsersBaseTreeNodeID = node.NodeID;
				if (theUsersBaseTreeNodeID >= 0)
				{
					return theUsersBaseTreeNodeID;
				}
					throw new Exception("Node < 0");
				}
				int nodeToAddUserFolderTo = treeProvider.SelectSingleNode(CMSContext.CurrentSiteName, aliasPath, "en-us").NodeID;
				//add folder as a child of nodeToAddUserFolderTo
				if (nodeToAddUserFolderTo >= 0)
				{
					TreeNode newNode = TreeNode.New("CMS.Folder", treeProvider);

					if (newNode != null)
					{

						//TO-DO: in the line below, this function will fail for the call in DocumentTypeList.ascx.cs.
						//It will work for GenericCMSDocumentTile.ascx.cs, though. The differences in the way the parameters are being used needs to be reconciled.
						newNode.DocumentName = userName; //it WAS this - clientId + "-" + userName;
						newNode.DocumentCulture = "en-us";
						newNode.Insert(nodeToAddUserFolderTo);

						// Get the user
						UserInfo theUser = UserInfoProvider.GetUserInfo(userName);

						// Get the role ID
						//RoleInfo role = RoleInfoProvider.GetRoleInfo("CMSBasicUsers_1", CMSContext.CurrentSiteName);

						// Prepare allowed / denied permissions
		            const int allowed = 127; //(64+32+16+8+4+2+1) = 127 full control  - Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Modify)))
		            const int denied = 0;

						// Create an instance of ACL provider
						AclProvider acl = new AclProvider(treeProvider);

						// Set role permissions
						//acl.SetRolePermissions(newNode, allowed, denied, role.RoleID);
						// Set user permissions
						acl.SetUserPermissions(newNode, allowed, denied, theUser);
					}

		        string tmpstr = CleanUserAliasPath(userAliasPath);
					return treeProvider.SelectSingleNode(CMSContext.CurrentSiteName, tmpstr, "en-us").NodeID;
				}
				throw new Exception("Node < 0");
			}

		private static string BuildAliasPath(string aliasPath, string userName)
		{
			string userAliasPath;
			if (!aliasPath.Contains(userName))
			{
				userAliasPath = aliasPath + FORWARD_SLASH + userName;
			}
			else
			{
				userAliasPath = aliasPath;
			}
			return userAliasPath;
		}


		private static string CleanUserAliasPath(string userAliasPath)
		{
			string returnVal = userAliasPath.Replace(".", "-");
			return returnVal;
		}



		internal static string GetFormURL(string docType, string usersBaseTreeNodeID)
		{
			string docTypeID = DataClassInfoProvider.GetDataClass(docType).ClassID.ToString();
            string formURL = GetKenticoCmsDeskEditUrl() + "&classid=" + docTypeID + "&parentnodeid=" + usersBaseTreeNodeID;
			return formURL;
		}

        internal static string GetKenticoUser(string loggedInUser)
		{
			//****************************************
			//**** Use districtParms for clientID ****
			//****************************************
			var nameCalculator = new KenticoNameCalculator();
			string clientID = DistrictParms.LoadDistrictParms().ClientID;
			return nameCalculator.CalculateUserName(clientID, loggedInUser);
		}

		internal static TreeProvider GetUserTreeProvider(string e3User)
		{
			CMSContext.Init();
			UserInfo userInfo = UserInfoProvider.GetUserInfo(GetKenticoUser(e3User));
			return new TreeProvider(userInfo);
		}

		internal static String GetKenticoMainFolderName(string clientId)
		{
            if (!string.IsNullOrEmpty(clientId))
			{
                var client = new ConfigurationServiceProxy();
                return client.GetClientConfigurationByClientId(clientId).ClientState;
			}
            return null;
		}

		/// <summary>
		/// Search Document Type  on the parameter documentscope
		/// </summary>
        /// <param name="ui"></param>
		/// <param name="className"></param>
		/// <param name="treeProvider"></param>
		/// <returns></returns>
        internal static List<UserNodeList> SearchDocumentTypeReferencesModified(UserInfo ui, string className, TreeProvider treeProvider, string documentScope, string where = null)
		{

			ResourceTypes resourceTypes = ResourceTypes.None;

			string clientId = DistrictParms.LoadDistrictParms().ClientID;
			string envState = GetKenticoMainFolderName(clientId);

			if (!string.IsNullOrWhiteSpace(documentScope))
			{
				resourceTypes = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), documentScope);
			}

			List<UserNodeList> userNodeList = new List<UserNodeList>();

		    if (resourceTypes == ResourceTypes.Forms)
			{
                string nodeAlias = "/" + envState + "/Forms/" + clientId + "/MTSS/%";

                DataSet documents = SearchUserDocs(nodeAlias, className, ui, treeProvider, resourceTypes, null, where);
				if (documents != null && documents.Tables.Count > 0)
				{
                    userNodeList = ReturnUserNodeList(documents, ui, className);
				}

			}

		    return userNodeList;
		}

        /// <summary>
       /// Method to get Kentico Documents Using Stored Procedure in Kentico DB
        /// </summary>
       /// <param name="loggedInUserName"></param>
        /// <param name="className"></param>
       /// <param name="documentScope"></param>
       /// <param name="includeExpiredDocuments"></param>
       /// <param name="curriculaIds"></param>
       /// <param name="standardIds"></param>
       /// <param name="type"></param>
       /// <param name="subtype"></param>
       /// <param name="maxResults"></param>
        /// <returns></returns>
        internal static List<UserNodeList> GetKenticoDocuments(string loggedInUserName, string className, string documentScope, bool includeExpiredDocuments, List<int> curriculaIds, List<int> standardIds, string type, string subtype, int maxResults )
        {
           string kenticoUserName = GetKenticoUser(loggedInUserName);
            string nodeAlias = string.Empty;

            ResourceTypes resourceTypes = (ResourceTypes)Enum.Parse(typeof(ResourceTypes), documentScope);

            string clientId = DistrictParms.LoadDistrictParms().ClientID;
            //TO DO:
            string envState = GetKenticoMainFolderName(clientId);

           List<UserNodeList> userNodeList = new List<UserNodeList>();

            switch (resourceTypes)
            {
                case ResourceTypes.MyDocuments:
                     nodeAlias = "/" + envState + "/Users/" + kenticoUserName + "/";
                    break;

                case ResourceTypes.DistrictDocuments:
                    nodeAlias = "/" + envState + "/Districts/" + clientId + "/";
                    break;

                case ResourceTypes.SharedDocuments:
                     nodeAlias = "/" + envState + "/Shared/" + clientId + "/";
                    break;

                case ResourceTypes.StateDocuments:
                    nodeAlias = "/" + envState + "/Documents/%";
                    break;

            }

            DataSet documents = GetKenticoDocumentsWithAssociations(nodeAlias, className, maxResults, includeExpiredDocuments, curriculaIds, standardIds, type, subtype);
                if (documents != null && documents.Tables.Count > 0)
               userNodeList = BuildUserNodeList(documents, kenticoUserName, className);

            return userNodeList;

            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="userName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private static List<UserNodeList> BuildUserNodeList(DataSet documents, string userName, string className)
                {
            List<UserNodeList> userNodeList = new List<UserNodeList>();
            UserInfo ui = UserInfoProvider.GetUserInfo(userName); 
            List<CurriculaAssociations> curriculaIds = new List<CurriculaAssociations>();
            List<StandardAssociations> standardIds = new List<StandardAssociations>();

            foreach (DataRow dataRow in documents.Tables[1].Rows)
            {
                CurriculaAssociations ca = new CurriculaAssociations();
                if (dataRow["docID"] != DBNull.Value && dataRow["curriculumID"] != DBNull.Value)
                {
                    ca.NodeId = Convert.ToInt32(dataRow["docID"]);
                    ca.CurriculaId = Convert.ToInt32(dataRow["curriculumID"]);
                }
                curriculaIds.Add(ca);
            }
            foreach (DataRow dataRow in documents.Tables[2].Rows)
            {
                StandardAssociations sa = new StandardAssociations();
                if (dataRow["docID"] != DBNull.Value && dataRow["standardID"] != DBNull.Value)
                {
                    sa.NodeId = Convert.ToInt32(dataRow["docID"]);
                    sa.StandardId = Convert.ToInt32(dataRow["standardID"]);
                }
                standardIds.Add(sa);
            }

            foreach (DataRow dataRow in documents.Tables[0].Rows)
            {

                UserNodeList userNode = new UserNodeList();
                userNode.DocumentId = Convert.ToInt32(dataRow["DocumentID"]);
                userNode.NodeName = (dataRow["NodeName"] == null || string.IsNullOrEmpty(dataRow["NodeName"].ToString()) ? "" : dataRow["NodeName"].ToString());
                userNode.NodeId = (dataRow["NodeID"] == null || string.IsNullOrEmpty(dataRow["NodeID"].ToString()) ? "" : dataRow["NodeID"].ToString());

                userNode.LastModified = (dataRow["DocumentModifiedWhen"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataRow["DocumentModifiedWhen"]));

                userNode.NodeAliasPath = (dataRow["NodeAliasPath"] == null || string.IsNullOrEmpty(dataRow["NodeAliasPath"].ToString()) ? "" : dataRow["NodeAliasPath"].ToString());

                userNode.ClassName = (dataRow["ClassName"] == null || string.IsNullOrEmpty(dataRow["ClassName"].ToString()) ? "" : dataRow["ClassName"].ToString());
                
                if (ui.UserID == DataIntegrity.ConvertToInt(dataRow["DocumentCreatedByUserID"].ToString()) || VerifyUserNodePermissions(ui, userNode.NodeAliasPath, (string.IsNullOrEmpty(ui.PreferredCultureCode) ? "en-US" : ui.PreferredCultureCode)))
                {
                    userNode.IsEditable = true;
                }
                //switch on classname to fillout the tile description area
                switch (className.ToLower())
                {
                    case "thinkgate.instructionalplan":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("InstructionalPlanOverview"))
                        {
                            userNode.Description = (dataRow["InstructionalPlanOverview"] == null || string.IsNullOrEmpty(dataRow["InstructionalPlanOverview"].ToString()) ? "" : dataRow["InstructionalPlanOverview"].ToString());

            }
                        break;
                    case "thinkgate.unitplan":
                    case "thinkgate.unitplan_ma":
                    case "thinkgate.unitplan_scberkeley":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("UnitPlanOverview"))
            {                
                            userNode.Description = (dataRow["UnitPlanOverview"] == null || string.IsNullOrEmpty(dataRow["UnitPlanOverview"].ToString()) ? "" : dataRow["UnitPlanOverview"].ToString());
                        }
                        break;
                    case "thinkgate.lessonplan":
                    case "thinkgate.lessonplan_ma":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("LessonPlanOverview"))
                {
                            userNode.Description = (dataRow["LessonPlanOverview"] == null || string.IsNullOrEmpty(dataRow["LessonPlanOverview"].ToString()) ? "" : dataRow["LessonPlanOverview"].ToString());
                }
                        break;
                    case "thinkgate.curriculumunit":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("Description"))
                        {
                            userNode.Description = (dataRow["Description"] == null || string.IsNullOrEmpty(dataRow["Description"].ToString()) ? "" : dataRow["Description"].ToString());
            }
                        break;
                    case "thinkgate.resource":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("Description"))
                        {
                            userNode.Description = (dataRow["Description"] == null || string.IsNullOrEmpty(dataRow["Description"].ToString()) ? "" : dataRow["Description"].ToString());
                            userNode.ResourceType = (dataRow["Type"] == null || string.IsNullOrEmpty(dataRow["Type"].ToString()) ? 0 : int.Parse(dataRow["Type"].ToString()));
                            userNode.ResourceSubType = (dataRow["SubType"] == null || string.IsNullOrEmpty(dataRow["SubType"].ToString()) ? 0 : int.Parse(dataRow["SubType"].ToString()));
                        }
                        break;
                    case "thinkgate.competencylist":
                        if (dataRow.Table.Columns.Contains("Description"))
                        {
                            userNode.Description = (dataRow["Description"] == null || string.IsNullOrEmpty(dataRow["Description"].ToString()) ? "" : dataRow["Description"].ToString());
                        }
                        break;
                    default:
                        userNode.Description = null;
                        break;
                }
                userNode.Description = StripHtmlTags(String.IsNullOrEmpty(userNode.Description) ? String.Empty : userNode.Description);
                userNode.Description = userNode.Description.Length <= 90 ? userNode.Description : userNode.Description.Truncate(90) + "...";
      
               userNode.AssociatedCurriculaIds =
                    curriculaIds.Where(x => x.NodeId.ToString() == userNode.NodeId)
                                .Select(x => x.NodeId).ToList();

               userNode.AssociatedStandardIds =
                   standardIds.Where(x => x.NodeId.ToString() == userNode.NodeId)
                               .Select(x => x.NodeId).ToList();
                  

                userNodeList.Add(userNode);
            }
            return userNodeList;

        }

        private static List<UserNodeList> ReturnUserNodeList(DataSet documents, UserInfo ui, string className)
        {
            List<UserNodeList> userNodeList = new List<UserNodeList>();
            foreach (DataRow dataRow in documents.Tables[0].Rows)
            {

                UserNodeList userNode = new UserNodeList();
                userNode.DocumentId = Convert.ToInt32(dataRow["DocumentId"]);
                userNode.NodeName = (dataRow["NodeName"] == null || string.IsNullOrEmpty(dataRow["NodeName"].ToString()) ? "" : dataRow["NodeName"].ToString());
                userNode.NodeId = (dataRow["NodeId"] == null || string.IsNullOrEmpty(dataRow["NodeId"].ToString()) ? "" : dataRow["NodeId"].ToString());
               
                userNode.LastModified = (dataRow["DocumentModifiedWhen"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataRow["DocumentModifiedWhen"]));

                userNode.NodeAliasPath = (dataRow["Nodealiaspath"] == null || string.IsNullOrEmpty(dataRow["Nodealiaspath"].ToString()) ? "" : dataRow["Nodealiaspath"].ToString());

                userNode.ClassName = (dataRow["ClassName"] == null || string.IsNullOrEmpty(dataRow["ClassName"].ToString()) ? "" : dataRow["ClassName"].ToString());


                if (ui.UserID == DataIntegrity.ConvertToInt(dataRow["DocumentCreatedByUserID"].ToString()) || VerifyUserNodePermissions(ui, userNode.NodeAliasPath, (string.IsNullOrEmpty(ui.PreferredCultureCode) ? "en-US" : ui.PreferredCultureCode)))
                {
                    userNode.IsEditable = true;
                }
                //switch on classname to fillout the tile description area
                switch (className.ToLower())
                {
                    case "thinkgate.instructionalplan":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("InstructionalPlanOverview"))
                        {
                            userNode.Description = (dataRow["InstructionalPlanOverview"] == null || string.IsNullOrEmpty(dataRow["InstructionalPlanOverview"].ToString()) ? "" : dataRow["InstructionalPlanOverview"].ToString());
                        
                        }
                        break;
                    case "thinkgate.unitplan":
                    case "thinkgate.unitplan_ma":
                    case "thinkgate.unitplan_scberkeley":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("UnitPlanOverview"))
                        {
                            userNode.Description = (dataRow["UnitPlanOverview"] == null || string.IsNullOrEmpty(dataRow["UnitPlanOverview"].ToString()) ? "" : dataRow["UnitPlanOverview"].ToString());
                        }
                        break;
                    case "thinkgate.lessonplan":
                    case "thinkgate.lessonplan_ma":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("LessonPlanOverview"))
                        {
                            userNode.Description = (dataRow["LessonPlanOverview"] == null || string.IsNullOrEmpty(dataRow["LessonPlanOverview"].ToString()) ? "" : dataRow["LessonPlanOverview"].ToString());
                        }
                        break;
                    case "thinkgate.curriculumunit":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("Description"))
                        {
                            userNode.Description = (dataRow["Description"] == null || string.IsNullOrEmpty(dataRow["Description"].ToString()) ? "" : dataRow["Description"].ToString());
                        }
                        break;
                    case "thinkgate.resource":
                        userNode.ExpirationDate = (dataRow["ExpirationDate"] == DBNull.Value ? DateTime.MaxValue.Date : Convert.ToDateTime(dataRow["ExpirationDate"]).Date);
                        if (dataRow.Table.Columns.Contains("Description"))
                        {
                            userNode.Description = (dataRow["Description"] == null || string.IsNullOrEmpty(dataRow["Description"].ToString()) ? "" : dataRow["Description"].ToString());
                            userNode.ResourceType = (dataRow["Type"] == null || string.IsNullOrEmpty(dataRow["Type"].ToString()) ? 0 : int.Parse(dataRow["Type"].ToString()));
                            userNode.ResourceSubType = (dataRow["SubType"] == null || string.IsNullOrEmpty(dataRow["SubType"].ToString()) ? 0 : int.Parse(dataRow["SubType"].ToString()));
                        }
                        break;
                    case "thinkgate.competencylist":
                        if (dataRow.Table.Columns.Contains("Description"))
                        {
                            userNode.Description = (dataRow["Description"] == null || string.IsNullOrEmpty(dataRow["Description"].ToString()) ? "" : dataRow["Description"].ToString());
                        }
                        break;
                    default:
                        userNode.Description = null;
                        break;
                }
                userNode.Description = StripHtmlTags(String.IsNullOrEmpty(userNode.Description) ? String.Empty : userNode.Description);
                userNode.Description = userNode.Description.Length <= 90 ? userNode.Description : userNode.Description.Truncate(90) + "...";

                //find the curricula associations for future filtering
                string expression = @"select curriculumID from thinkgate_docToCurriculums where docID=" + userNode.NodeId;
                DataSet dscurricula;
                try
                {

                    dscurricula = ConnectionHelper.ExecuteQuery(expression, (new QueryDataParameters()), QueryTypeEnum.SQLQuery, false);

                }
                catch (Exception ex)
                {

                    throw new Exception(String.Format("The Query did not give any results {0}\n{1}", expression, ex.Message));

                }

                if (dscurricula != null && dscurricula.Tables[0].Rows.Count > 0)
                {
                    List<int> associationLst = new List<int>();

                    foreach (DataRow dataRowAssoc in dscurricula.Tables[0].Rows)
                    {
                        associationLst.Add(Convert.ToInt32(dataRowAssoc["curriculumID"]));

                    }
                    userNode.AssociatedCurriculaIds = associationLst.ToArray();
                }

                //find the standard associations for future filtering
                expression = @"select standardID from thinkgate_docToStandards where docID=" + userNode.NodeId;
                DataSet dsstandard;
                try
                {

                    dsstandard = ConnectionHelper.ExecuteQuery(expression, (new QueryDataParameters()), QueryTypeEnum.SQLQuery, false);

                }
                catch (Exception ex)
                {

                    throw new Exception(String.Format("The Query did not give any results {0}\n{1}", expression, ex.Message));

                }

                if (dsstandard != null && dsstandard.Tables[0].Rows.Count > 0)
                {
                    List<int> associationLstStandard = new List<int>();

                    foreach (DataRow dataRowAssoc in dsstandard.Tables[0].Rows)
                    {
                        associationLstStandard.Add(Convert.ToInt32(dataRowAssoc["standardID"]));

                    }
                    userNode.AssociatedStandardIds = associationLstStandard.ToArray();
                }

                userNodeList.Add(userNode);
            }
            return userNodeList;

        }


		/// <summary>
		/// getUserSharedDocuments - Gets a combined DataSet of documents the passed in user can READ
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="className"></param>
		/// <param name="treeProvider"></param>
		/// <param name="filterType"></param>
		/// <returns>DataSet</returns>
        internal static DataSet GetUserSharedDocuments(UserInfo ui, string className, TreeProvider treeProvider, string filterType = null, bool filterExpiredContent = false)
		{
			string clientId = DistrictParms.LoadDistrictParms().ClientID;
			string envState = GetKenticoMainFolderName(clientId);
		    string userName = ui.UserName;

			DataSet sharedDocDataSet = new DataSet();

		    string nodeAlias = "/" + envState + "/Users/"+userName+"/%";
            DataSet userdocs = SearchUserDocs(nodeAlias, className, ui, treeProvider, ResourceTypes.SharedDocuments, filterType, null, filterExpiredContent);
			if (userdocs != null && userdocs.Tables.Count > 0)
			{
				sharedDocDataSet.Merge(userdocs);
			}

			nodeAlias = "/" + envState + "/Districts/" + clientId + "/%";
            DataSet disDocs = SearchUserDocs(nodeAlias, className, ui, treeProvider, ResourceTypes.SharedDocuments, filterType, null, filterExpiredContent);
			if (disDocs != null && disDocs.Tables.Count > 0)
			{
				sharedDocDataSet.Merge(disDocs);
			}

			nodeAlias = "/" + envState + "/Documents/%";
            DataSet stateDocs = SearchUserDocs(nodeAlias, className, ui, treeProvider, ResourceTypes.SharedDocuments, filterType, null, filterExpiredContent);
			if (stateDocs != null && stateDocs.Tables.Count > 0)
			{
				sharedDocDataSet.Merge(stateDocs);
			}

            //US15667
            nodeAlias = "/" + envState + "/Shared/" + clientId + "/%";
            disDocs = SearchUserDocs(nodeAlias, className, ui, treeProvider, ResourceTypes.SharedDocuments, filterType, null, filterExpiredContent);
            if (disDocs != null && disDocs.Tables.Count > 0)
            {
                sharedDocDataSet.Merge(disDocs);
            }

			return sharedDocDataSet.Tables.Count > 0 ? sharedDocDataSet : null;
		}

        private static DataSet SearchUserDocs(string treeselectionparamters, string className, UserInfo ui, TreeProvider treeProvider, ResourceTypes resourceTypes, string filterType, string where = null, bool filterExpiredContent = false)
		{
            DataSet dsResults;
			NodeSelectionParameters nsp = new NodeSelectionParameters();
			nsp.SiteName = CMSContext.CurrentSiteName;
			nsp.AliasPath = CleanUserAliasPath(treeselectionparamters);
			nsp.CultureCode = (string.IsNullOrEmpty(ui.PreferredCultureCode) ? "en-US" : ui.PreferredCultureCode); //CMSContext.CurrentUser.PreferredCultureCode;
			nsp.CombineWithDefaultCulture = false;
			nsp.ClassNames = className;
            nsp.Where = where;  //Build WHERE condition according to the search expression
            nsp.OrderBy = "DocumentName ASC"; // Order by DocumentName
            if (className.ToLower() == "thinkgate.curriculumunit")
            {
                nsp.MaxRelativeLevel = 1;
            }
            else
            {
                nsp.MaxRelativeLevel = -1;
            }
			nsp.SelectOnlyPublished = true;

			if (resourceTypes == ResourceTypes.MyDocuments)
			{
				nsp.SelectOnlyPublished = false;
			}
			else
			{
				nsp.SelectOnlyPublished = true;

			}
			nsp.TopN = GetNumberOfRecordsToReturn(DistrictParms.LoadDistrictParms().NumberOfKenticoRecordsToReturn);

			string flagname = string.Empty;
			switch (className.ToLower())
			{
				case "thinkgate.unitplan":
                case "thinkgate.unitplan_scberkeley":
                    flagname = "UnitPlanTemplateFlag";
					break;
                case "thinkgate.unitplan_ma":
					flagname = "UPMATemplateFlag";
					break;
				case "thinkgate.lessonplan":
                case "thinkgate.lessonplan_ma":
					flagname = "LessonPlanTemplateFlag";
					break;
				case "thinkgate.instructionalplan":
					flagname = "InstructionalPlanTemplateFlag";
					break;
				case "thinkgate.curriculumunit":
					flagname = "CurriculumUnitTemplateFlag";
					break;
				case "thinkgate.resource":
					flagname = "ResourceTemplateFlag";
					break;
				
			}


            if (filterType == "Templates" && !string.IsNullOrWhiteSpace(flagname))
			{
				nsp.Where = "(" + flagname + " = '1')";
			}
            if (filterType == "Non-Templates" && !string.IsNullOrWhiteSpace(flagname))
			{
				nsp.Where = "(" + flagname + " = '0' OR " + flagname + " IS NULL)";
			}

            if (filterExpiredContent)
            {
                if (!string.IsNullOrEmpty(nsp.Where))
                    nsp.Where = nsp.Where + " AND ";
                nsp.Where = nsp.Where +
                            " (ExpirationDate IS NULL OR CONVERT (date, ExpirationDate) >= CONVERT (date, GETDATE()))";
            }


			CurrentUserInfo currentUserInfo = new CurrentUserInfo(ui, true);
			try
			{
			    DataSet documents = DocumentHelper.GetDocuments(nsp, treeProvider);
				if (documents != null && documents.Tables[0].Rows.Count > 0)
				{
					//If the current user can't read the document then remove it from the result set
					dsResults = TreeSecurityProvider.FilterDataSetByPermissions(documents, NodePermissionsEnum.Read, currentUserInfo);
				}
				else
				{
					dsResults = documents;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(String.Format("The node passed treeselectionparamters does not exists/document does not exist {0}. \n{1}", treeselectionparamters, ex.Message));
			}
			return dsResults;

		}

        public List<UserNodeList> GetAllCompetencyList(string[] strResourcesToShowOnTile)
        {
            var ui = (UserInfo)HttpContext.Current.Session["KenticoUserInfo"];
            SessionObject sessionObject = (SessionObject)HttpContext.Current.Session["SessionObject"];
            
            List<UserNodeList> userNodeList=new List<UserNodeList>();
            
            TreeProvider treeProvider = GetUserTreeProvider(sessionObject.LoggedInUser.ToString());
            DataSet ds=new DataSet();
            foreach (string tileResource in strResourcesToShowOnTile)
            {
                ds =GetUserSharedDocuments(ui,tileResource,treeProvider);
            }   
            if(ds!=null)
            userNodeList = (from s in ds.Tables[0].AsEnumerable()
                             select new UserNodeList {DocumentId=Convert.ToInt32(s["DocumentID"].ToString()),FriendlyName=s["DocumentName"].ToString() }).ToList();

            return userNodeList;
        }


        public List<UserNodeList> GetCompetencyList(string[] strResourcesToShowOnTile)
        {
            var ui = (UserInfo)HttpContext.Current.Session["KenticoUserInfo"];
            SessionObject sessionObject = (SessionObject)HttpContext.Current.Session["SessionObject"];
        
            List<UserNodeList> userNodeList = new List<UserNodeList>();

            TreeProvider treeProvider = GetUserTreeProvider(sessionObject.LoggedInUser.ToString());
            DataSet ds = new DataSet();
            foreach (string tileResource in strResourcesToShowOnTile)
            {
                ds = GetUserSharedDocuments(ui, tileResource, treeProvider);
            }
            if (ds != null)
                userNodeList = (from s in ds.Tables[0].AsEnumerable()
                                 select new UserNodeList { NodeId  = s["DocumentNodeID"].ToString(), DocumentId = Convert.ToInt32(s["DocumentID"].ToString()), FriendlyName = s["DocumentName"].ToString() }).ToList();

            return userNodeList;
        }

        /// <summary>
        /// There can be 6 tiles on a screen.
        /// Each tile could have over 3000 records.
        /// Our goal is to increase the load time.
        /// By changing the number of records to load on each tile, we can increase the speed of this screen
        /// 
        /// This function is a copy of SearchUserDocs.
        /// The only difference is that it gets a parm called NumberOfKenticoRecordsOnTile to get the number of records on a tile
        /// </summary>
        /// <param name="treeselectionparamters"></param>
        /// <param name="className"></param>
        /// <param name="ui"></param>
        /// <param name="treeProvider"></param>
        /// <param name="resourceTypes"></param>
        /// <param name="filterType"></param>
        /// <param name="where"></param>
        /// <param name="filterExpiredContent"></param>
        /// <returns></returns>
        private static DataSet SearchUserDocsForTile(string treeselectionparamters, string className, UserInfo ui, TreeProvider treeProvider, ResourceTypes resourceTypes, string filterType, string where = null, bool filterExpiredContent = false)
        {
            DataSet dsResults;
            NodeSelectionParameters nsp = new NodeSelectionParameters();
            nsp.SiteName = CMSContext.CurrentSiteName;
            nsp.AliasPath = CleanUserAliasPath(treeselectionparamters);
            nsp.CultureCode = (string.IsNullOrEmpty(ui.PreferredCultureCode) ? "en-US" : ui.PreferredCultureCode); //CMSContext.CurrentUser.PreferredCultureCode;
            nsp.CombineWithDefaultCulture = false;
            nsp.ClassNames = className;
            nsp.Where = where;  //Build WHERE condition according to the search expression
            nsp.OrderBy = "DocumentModifiedWhen DESC"; // Order by DocumentName
            if (className.ToLower() == "thinkgate.curriculumunit")
            {
                nsp.MaxRelativeLevel = 1;
            }
            else
            {
                nsp.MaxRelativeLevel = -1;
            }
            nsp.SelectOnlyPublished = true;

            if (resourceTypes == ResourceTypes.MyDocuments)
            {
                nsp.SelectOnlyPublished = false;
            }
            else
            {
                nsp.SelectOnlyPublished = true;

            }
            nsp.TopN = GetNumberOfRecordsOnTile(DistrictParms.LoadDistrictParms().NumberOfKenticoRecordsOnTile);

            string flagname = string.Empty;
            switch (className.ToLower())
            {
                case "thinkgate.unitplan":
                case "thinkgate.unitplan_scberkeley":
                    flagname = "UnitPlanTemplateFlag";
                    break;
                case "thinkgate.unitplan_ma":
                    flagname = "UPMATemplateFlag";
                    break;
                case "thinkgate.lessonplan":
                case "thinkgate.lessonplan_ma":
                    flagname = "LessonPlanTemplateFlag";
                    break;
                case "thinkgate.instructionalplan":
                    flagname = "InstructionalPlanTemplateFlag";
                    break;
                case "thinkgate.curriculumunit":
                    flagname = "CurriculumUnitTemplateFlag";
                    break;
                case "thinkgate.resource":
                    flagname = "ResourceTemplateFlag";
                    break;

            }


            if (filterType == "Templates" && !string.IsNullOrWhiteSpace(flagname))
            {
                nsp.Where = "(" + flagname + " = '1')";
            }
            if (filterType == "Non-Templates" && !string.IsNullOrWhiteSpace(flagname))
            {
                nsp.Where = "(" + flagname + " = '0' OR " + flagname + " IS NULL)";
            }

            if (filterExpiredContent)
            {
                if (!string.IsNullOrEmpty(nsp.Where))
                    nsp.Where = nsp.Where + " AND ";
                nsp.Where = nsp.Where +
                            " (ExpirationDate IS NULL OR CONVERT (date, ExpirationDate) >= CONVERT (date, GETDATE()))";
            }


            CurrentUserInfo currentUserInfo = new CurrentUserInfo(ui, true);
            try
            {
                DataSet documents = DocumentHelper.GetDocuments(nsp, treeProvider);
                if (documents != null && documents.Tables[0].Rows.Count > 0)
                {
                    //If the current user can't read the document then remove it from the result set
                    dsResults = TreeSecurityProvider.FilterDataSetByPermissions(documents, NodePermissionsEnum.Read, currentUserInfo);
                }
                else
                {
                    dsResults = documents;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("The node passed treeselectionparamters does not exists/document does not exist {0}. \n{1}", treeselectionparamters, ex.Message));
            }
            return dsResults;

        }

        private static DataSet GetKenticoDocumentsWithAssociations(string nodeAlias, string className, int maxResults, bool includeExpiredDocuments, List<int> curriculaIds, List<int> standardIds, string type, string subtype)
        {
            bool parentLevelOnly = false;
            nodeAlias = CleanUserAliasPath(nodeAlias);
            if (className.ToLower() == "thinkgate.curriculumunit")
            {
                parentLevelOnly = true;
        }
            DataSet documents = MCU.GetKenticoDocumentsThroughSP(nodeAlias, className, maxResults, includeExpiredDocuments, curriculaIds, standardIds, type, subtype, parentLevelOnly);

           return documents;

        }

      
		/// <summary>
		/// VerifyNodePermissionsEnum
		/// </summary>
		/// <param name="ui"></param>
		/// <param name="nodeAliasPath"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		private static bool VerifyUserNodePermissions(UserInfo ui, string nodeAliasPath, string culture)
		{
			TreeProvider tree = new TreeProvider(ui);
			TreeNode node = tree.SelectSingleNode(CMSContext.CurrentSiteName, nodeAliasPath, culture);
			return TreeSecurityProvider.IsAuthorizedPerNode(node, NodePermissionsEnum.Modify, ui) == AuthorizationResultEnum.Allowed;
		}

		private static string StripHtmlTags(string source)
		{
			return Regex.Replace(source, "<.*?>", string.Empty);
		}


        /// <summary>
        /// Search Document Type  on the parameter documentscope
        /// </summary>
	    /// <param name="ui"></param>
        /// <param name="className"></param>
        /// <param name="treeProvider"></param>
	    /// <param name="documentScope"></param>
	    /// <param name="where"></param>
	    /// <param name="orderBy"></param>
	    /// <returns>DataSet</returns>
        internal static DataSet ExpandedSearchDocumentType(UserInfo ui, string className, TreeProvider treeProvider, ResourceTypes documentScope, string where, string orderBy, bool filterExpiredContent = false)
        {
            DataSet documents = null;         
	        string username = ui.UserName;
            if (!string.IsNullOrEmpty(KenticoUserName))
            {
                username = KenticoUserName;
            }

            string nodeAlias;
            string clientId = DistrictParms.LoadDistrictParms().ClientID;
            string envState = GetKenticoMainFolderName(clientId);           
            switch (documentScope)
            {
                case ResourceTypes.DistrictDocuments:
                    nodeAlias = FORWARD_SLASH + envState + FORWARD_SLASH + DISTRICTS + FORWARD_SLASH + clientId + FORWARD_SLASH + "%";
                    documents = ExpandedSearchUserDocs(nodeAlias, className, ui, treeProvider, documentScope, @where, orderBy, filterExpiredContent);
                    break;
                case ResourceTypes.StateDocuments:
                    nodeAlias = FORWARD_SLASH + envState + FORWARD_SLASH + STATE_DOCUMENTS + FORWARD_SLASH + "%";
                    documents = ExpandedSearchUserDocs(nodeAlias, className, ui, treeProvider, documentScope, @where, orderBy, filterExpiredContent);
                    break;
                case ResourceTypes.MyDocuments:
                    nodeAlias = FORWARD_SLASH + envState + FORWARD_SLASH + USERS + FORWARD_SLASH + username + "%";
                    documents = ExpandedSearchUserDocs(nodeAlias, className, ui, treeProvider, documentScope, @where, orderBy, filterExpiredContent);
                    break;
                case ResourceTypes.SharedDocuments:
                    nodeAlias = FORWARD_SLASH + envState + FORWARD_SLASH + SHARED + FORWARD_SLASH + clientId + FORWARD_SLASH + "%";
                    documents = ExpandedSearchUserDocs(nodeAlias, className, ui, treeProvider, documentScope, @where, orderBy, filterExpiredContent);
                    break;
            }
                      
            if (documents != null && documents.Tables.Count>0  && documents.Tables[0].Rows.Count > 0)            
                return documents;
            return null;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="treeselectionparamters"></param>
		/// <param name="className"></param>
		/// <param name="ui"></param>
		/// <param name="treeProvider"></param>
		/// <param name="where"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		private static DataSet ExpandedSearchUserDocs(string treeselectionparamters, string className, UserInfo ui, TreeProvider treeProvider, ResourceTypes resourceTypes, string where, string orderBy = "DocumentModifiedWhen desc", bool filterExpiredContent = false)
		{
		    DataSet dsResults;
			NodeSelectionParameters nsp = new NodeSelectionParameters();
			nsp.SiteName = CMSContext.CurrentSiteName;
			nsp.AliasPath = CleanUserAliasPath(treeselectionparamters);
			nsp.CultureCode = (string.IsNullOrEmpty(ui.PreferredCultureCode) ? "en-US" : ui.PreferredCultureCode); //CMSContext.CurrentUser.PreferredCultureCode;
			nsp.CombineWithDefaultCulture = false;
			nsp.ClassNames = className;
			nsp.Where = where;
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "DocumentModifiedWhen desc";
            }
			nsp.OrderBy = orderBy;
            nsp.MaxRelativeLevel = -1;
           

			nsp.SelectOnlyPublished = true;

			switch (resourceTypes)
			{
			    case ResourceTypes.All:
				nsp.SelectOnlyPublished = false;    //For expanded search.
			        break;
			    case ResourceTypes.MyDocuments:
				nsp.SelectOnlyPublished = false;
			        break;
			    default:
				nsp.SelectOnlyPublished = true;
			        break;
			}

            //TO DO:  WILL NEED ALL RECORDS WHEN STANDARD SET IS CHOSEN
			nsp.TopN = GetNumberOfRecordsToReturn(DistrictParms.LoadDistrictParms().NumberOfKenticoRecordsToReturn);

             if (filterExpiredContent)
            {
                if (!string.IsNullOrEmpty(nsp.Where))
                    nsp.Where = nsp.Where + " AND ";
                nsp.Where = nsp.Where +
                            " (ExpirationDate IS NULL OR CONVERT (date, ExpirationDate) >= CONVERT (date, GETDATE()))";
            }



			CurrentUserInfo currentUserInfo = new CurrentUserInfo(ui, true);
			try
			{
			    DataSet documents = DocumentHelper.GetDocuments(nsp, treeProvider);

				if (documents != null && documents.Tables[0].Rows.Count > 0)
				{
					//If the current user can't read the document then remove it from the result set
					dsResults = TreeSecurityProvider.FilterDataSetByPermissions(documents, NodePermissionsEnum.Read, currentUserInfo, true);
				}
				else
				{
					dsResults = documents;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(String.Format("The node passed treeselectionparamters does not exists/document does not exist {0}. \n{1}", treeselectionparamters, ex.Message));
			}

			return dsResults==null?new DataSet():dsResults;

		}

		/// <summary>
		/// Generic method used to retrieve lookup details from TG.LookupDetail table in Kentico.
		/// </summary>
		///  <param name="where"></param>
		/// <param name="orderBy"></param>
		/// <returns></returns>
		internal static List<LookupDetails> GetLookupDetailsByClient(string where = "", string orderBy = "")
		{
			List<LookupDetails> lookupData = new List<LookupDetails>();
			DataClassInfo customTable = DataClassInfoProvider.GetDataClass("TG.LookupDetails");
			if (customTable != null)
			{
                SessionObject sessionObject = (SessionObject)HttpContext.Current.Session["SessionObject"];
                string kenticoUserName = GetKenticoUser(sessionObject.LoggedInUser.ToString());
				UserInfo userInfo = UserInfoProvider.GetUserInfo(kenticoUserName);

                DataSet lookupDataSet = (new CustomTableItemProvider(userInfo)).GetItems("TG.LookupDetails", where, orderBy);
				if (lookupDataSet != null && lookupDataSet.Tables.Count > 0 && lookupDataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in lookupDataSet.Tables[0].Rows)
					{
					    if (CheckValidStateLEA(row["StateLEA"].ToString()))
					    {
						LookupDetails lookupTableEntity = new LookupDetails();

						lookupTableEntity.ID = DataIntegrity.ConvertToInt(row["ItemID"]);
						lookupTableEntity.Description = row["Description"].ToString();
						lookupTableEntity.Enum = DataIntegrity.ConvertToInt(row["Enum"]);
					        lookupTableEntity.LookupEnum = (LookupType) Enum.Parse(typeof (LookupType), row["LookupEnum"].ToString());

						lookupData.Add(lookupTableEntity);
					}
					}
					return lookupData;
				}
			}
			return lookupData;
		}

		/// <summary>
        /// Checks if the any of the value in the string sperated by comma has a match client State or LEA name. If the clientList is null or empty,
        /// it will still return true, Indicating it valid for all clients.
        /// </summary>
        /// <param name="clientList"></param>
        /// <returns></returns>

	    internal static Boolean CheckValidStateLEA(string clientList)
	    {
            if (String.IsNullOrEmpty(clientList))
	        return true;
            String clientId = DistrictParms.LoadDistrictParms().ClientID.Trim();
            string clientState = DistrictParms.LoadDistrictParms().State.Trim();
            string[] seperator = { "," };
            var clientListArray = clientList.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
	        foreach (string clientItem in clientListArray)
	        {
	            if (clientId.ToLower().Equals(clientItem.ToLower()))
	                return true;
	            if (clientState.ToLower().Equals(clientItem.ToLower()))
	                return true;
	        }
	        return false;
	    }


		protected static int GetNumberOfRecordsToReturn(string districtParmValue)
		{
			if (!string.IsNullOrWhiteSpace(districtParmValue))
			{
				int numberOfKenticoRecordsToReturn;
				return int.TryParse(districtParmValue, out numberOfKenticoRecordsToReturn) ? numberOfKenticoRecordsToReturn : DEFAULT_NUMBER_OF_RECORDS_TO_RETURN;
			}
		    return DEFAULT_NUMBER_OF_RECORDS_TO_RETURN;
		}

        protected static int GetNumberOfRecordsOnTile(string districtParmValue)
        {
            if (!string.IsNullOrWhiteSpace(districtParmValue))
            {
                int numberOfKenticoRecordsOnTile;
                return int.TryParse(districtParmValue, out numberOfKenticoRecordsOnTile) ? numberOfKenticoRecordsOnTile : DEFAULT_NUMBER_OF_RECORDS_ON_TILE;
            }
	        return DEFAULT_NUMBER_OF_RECORDS_ON_TILE;
        }

        public static List<KenticoMTSSFormData> GetKenticoFormData(List<UserNodeList> forms)
        {
            //Initialize Kentico
            CMSContext.Init();

            //Variables
            String siteName = CMSContext.CurrentSiteName;

            //Objects
            List<KenticoMTSSFormData> results = new List<KenticoMTSSFormData>();

            //Query Kentico for Form Data based on List of Forms passed into Method
            foreach (UserNodeList form in forms)
            {
                //Get Form by name. 
                //NOTE: PageName with BizForm webpart, MUST match Form name (Tools-Forms) for this to work. 
                //TODO: Need to see if form name is embedded in the object anywhere, it wasn't at first glance.
                BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(form.NodeName, siteName);

                //Lookup Form Records in Kentico Table
                if (bfi != null)
                {
                    // Get Class ID for BizForm
                    DataClassInfo dci = DataClassInfoProvider.GetDataClass(bfi.FormClassID);
                    if (dci != null)
                    {
                        //Query Kentico for all records (rows) associated with BizForm
                        GeneralConnection genConn = ConnectionHelper.GetConnection();
                        DataSet ds = genConn.ExecuteQuery(dci.ClassName + ".selectall", null, null, null);


                        var query = (from row in ds.Tables[0].AsEnumerable()
                                     where (!row.IsNull("Issue_Id"))
                                     select new KenticoMTSSFormData
                                     {
                                         FormName = form.NodeName,
                                         FormID = bfi.FormID,
                                         RecordID = ValidationHelper.GetInteger(ds.Tables[0].Rows[0][0], 0),
                                         FormHyperLink = "/" + KenticoVirtualFolder + "/CMSModules/Thinkgate/Forms/BizForm_Edit.aspx?formID=" + bfi.FormID.ToString() + "&formRecordID=" + ValidationHelper.GetInteger(ds.Tables[0].Rows[0][0], 0).ToString(),
                                         StudentID = Int32.Parse(String.Empty.CompareTo(row.Field<string>("Student_No")) == 0 ? "0" : row.Field<string>("Student_No")),
                                         InterventionID = Int32.Parse(String.Empty.CompareTo(row.Field<string>("Issue_Id")) == 0 ? "0" : row.Field<string>("Issue_Id"))
                                     }).ToList();
                        results.AddRange(query);

                    }
                }
            }

            //Now Count ALL Kentico Forms per User/Intervention
            var formCounts = from kn in results
                             group kn by new { kn.StudentID, kn.InterventionID } into grp
                             where grp.Any()
                             select new
                             {
                                 StudentID = grp.Key.StudentID,
                                 InterventionID = grp.Key.InterventionID,
                                 Count = grp.Count(),
                                 HyperLinkList = String.Join(",", grp.Select(kn => kn.FormHyperLink).ToArray()),
                                 FormNameList = String.Join(",", grp.Select(kn => kn.FormName).ToArray())
                             };

            //Join with Form Dataset to add Counts
            var merged = from rs in results
                         join q in formCounts
                         on new { rs.StudentID, rs.InterventionID } equals new { q.StudentID, q.InterventionID }
                         select new { rs, q };

            //Update Objects with FormCount + FormLinks
            foreach (var c in merged)
            {
                c.rs.FormCount = c.q.Count;
                c.rs.ListHyperLinks = c.q.HyperLinkList;
                c.rs.ListFormNames = c.q.FormNameList;
            }

            return results;
        }

        /// <summary>
        /// Find DB Name
        /// </summary>
        /// <returns></returns>
        public static string FindDBName()
        {
            string username = CMSContext.CurrentUser.UserName;
            if (!string.IsNullOrEmpty(username))
            {
                var nameParts = username.Split('-');
                var client = new ConfigurationServiceProxy();

                if (!string.IsNullOrEmpty(nameParts[0]))
                {
                    return client.GetPropertyValueByClientId(
                        nameParts[0],
                        ConfigurationConstants.DATABASE_NAME);
                }
            }
            return null;
		}

        public static DataSet GetTileMapLookupDataSet(string resourceToShow)
        {
            UserInfo userInfo = (UserInfo)HttpContext.Current.Session["KenticoUserInfo"];
            CustomTableItemProvider tp = new CustomTableItemProvider(userInfo);
            CMSContext.Init();
            int lookupval = GetDocTypeValueFromTileMap(resourceToShow);
            string filtercriteria = "LookupValue = " + lookupval + " and (ISNULL(StateLEA,'')='' or StateLEA = '" + DistrictParms.LoadDistrictParms().ClientID + "' or StateLEA = '" + DistrictParms.LoadDistrictParms().State + "')";
            DataSet ds = tp.GetItems("thinkgate.TileMap_Lookup", filtercriteria, string.Empty);
            return ds;
        }

        public static int GetDocTypeValueFromTileMap(string resourceToShow)
        {
            UserInfo userInfo = (UserInfo) HttpContext.Current.Session["KenticoUserInfo"];
            CustomTableItemProvider tp = new CustomTableItemProvider(userInfo);

            DataSet lookupDataSet = tp.GetItems("thinkgate.TileMap", "BaseTileType = '" + resourceToShow + "' ", string.Empty);
            int lookupval = 0;
            if (lookupDataSet.Tables[0].Rows.Count > 0)
            {
                lookupval = (int)lookupDataSet.Tables[0].Rows[0]["Value"];
            }
            //TODO: null check
            return lookupval;
        }

        #region Instruction Material

        public static DataSet GetTileMapLookupDataSet()
        {
            UserInfo userInfo = (UserInfo)HttpContext.Current.Session["KenticoUserInfo"];
            CustomTableItemProvider tp = new CustomTableItemProvider(userInfo);
            CMSContext.Init();
            string filtercriteria = " (ISNULL(StateLEA,'')='' or StateLEA = '" + DistrictParms.LoadDistrictParms().ClientID + "' or StateLEA = '" + DistrictParms.LoadDistrictParms().State + "')";
            DataSet ds = tp.GetItems("thinkgate.TileMap_Lookup", filtercriteria, string.Empty);
            return ds;
        }


        /// <summary>
        /// Search Document Type  on the parameter documentscope
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="className"></param>
        /// <param name="treeProvider"></param>
        /// <returns></returns>
        internal static List<UserNodeList> GetKenticoDocsForInstuctionAssignmentTiles(UserInfo ui, string className, TreeProvider treeProvider, string where = null)
        {
            string userName = ui.UserName;

            ResourceTypes resourceTypes = ResourceTypes.None;

            string clientId = DistrictParms.LoadDistrictParms().ClientID;
            string envState = GetKenticoMainFolderName(clientId);

            List<UserNodeList> userNodeList = new List<UserNodeList>();       

                string nodeAlias = "/" + envState + "/Users/" + userName + "/%";

                DataSet documents = SearchUserDocsForTile(nodeAlias, className, ui, treeProvider, resourceTypes, null,@where);

                if (documents != null && documents.Tables.Count > 0)
                {
                    userNodeList = ReturnUserNodeList(documents, ui, className);
                }
               
         
                nodeAlias = "/" + envState + "/Districts/" + clientId + "/%";

                documents = SearchUserDocsForTile(nodeAlias, className, ui, treeProvider, resourceTypes, null,where);
                if (documents != null && documents.Tables.Count > 0)
                {
                  userNodeList.AddRange( ReturnUserNodeList(documents, ui, className));

                }
                
            
                nodeAlias = "/" + envState + "/Documents/%";

                documents = SearchUserDocsForTile(nodeAlias, className, ui, treeProvider, resourceTypes, null,where);
                if (documents != null && documents.Tables.Count > 0)
                {
                    userNodeList.AddRange( ReturnUserNodeList(documents, ui, className));
                }
            
                nodeAlias = "/" + envState + "/Shared/" + clientId + "/%";

                documents = SearchUserDocsForTile(nodeAlias, className, ui, treeProvider, resourceTypes, null, where);
                if (documents != null && documents.Tables.Count > 0)
                {
                    userNodeList.AddRange(ReturnUserNodeList(documents, ui, className));
                }
            
            return userNodeList;
        }

        #endregion

    }
}