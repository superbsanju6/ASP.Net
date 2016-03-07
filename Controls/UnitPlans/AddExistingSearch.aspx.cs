using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using System.Linq;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.UnitPlans
{
    public partial class AddExistingSearch : System.Web.UI.Page
    {
        private const string THINKGATE_RESOURCE = "thinkgate.resource";

        TreeProvider treeProvider = null;
        private bool filterExpiredMaterials = true;        
        static UserInfo theSharedUserRoles = null;

        /// <summary>
        /// Gets the type of the document.
        /// </summary>
        /// <value>
        /// The type of the document.
        /// </value>
        protected string DocumentType
        {
            get
            {
                return QueryHelper.GetString("doctype", "");
            }
        }


        /// <summary>
        /// Identifier of parent document. (For newly created documents.)
        /// </summary>
        protected string ParentNodeID
        {
            get
            {
                return QueryHelper.GetString("parentnodeid", "1");
            }
        }

        /// <summary>
        /// Identifier of parent document. (For newly created documents.)
        /// </summary>
        protected string Subtype
        {
            get
            {
                return QueryHelper.GetString("subType", "0");
            }
        }

        /// <summary>
        /// Identifier if we are copying document to Shared Folder
        /// </summary>
        static string IsShared
        {
            get
            {
                return QueryHelper.GetString("isshared", "0");
            }
        }       
            

        /// <summary>
        /// Gets the document action.
        /// </summary>
        /// <value>
        /// The document action. Default= "3"
        /// 
        /// Copy/Move:
        /// Value    Child Nodes      Associations
        /// =====    ===========      ============
        ///  "0"          N                 N  
        ///  "1"          N                 Y  
        ///  "2"          Y                 N  
        ///  "3"          Y                 Y
        /// 
        /// </value>
        protected string DocumentAction2
        {
            get
            {
                return QueryHelper.GetString("action", "3");
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            CMSContext.Init();
            DropDownList1_SelectedIndexChanged(sender, e);
        }


        protected void CopySelectedItems_Click(object sender, EventArgs e)
        {
            AddExistingMessage.Text = string.Empty;
            //TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(CMSContext.CurrentUser.UserName));

            //***
            UserInfo userInfo = (UserInfo)Session["KenticoUserInfo"];
            TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(userInfo.UserName));
            //***
            theSharedUserRoles=UserInfoProvider.GetUserInfo(userInfo.UserName); 

            int fromNodeID=0;
            string[] selitems = (SelectedItems.Value).Split(',');
            int parentNode;
            if (Int32.TryParse(ParentNodeID, out parentNode))
            {
                int expiredChildCount = 0;
                int expiredResourceAssociationCount = 0;
                CMS.DocumentEngine.TreeNode toNode = tp.SelectSingleNode(parentNode);
                foreach (string t in selitems)
                {
                    int nodeid;
                    string docEntry = t;
                    if (Int32.TryParse(docEntry, out nodeid))
                    {
                        fromNodeID = nodeid;
                        CMS.DocumentEngine.TreeNode fromNode = tp.SelectSingleNode(nodeid);
                        if (IsIncludeChildNodeRequested(DocumentAction.Value))
                            expiredChildCount += GetExpiredChildCountforNode(tp, fromNode, toNode, DocumentAction.Value);
                        if (IsIncludeAssociationRequested(DocumentAction.Value))
                            expiredResourceAssociationCount += GetExpiredAssociationCount(tp, fromNode);
                       
                    }
                }
                CopySelectedItemsConfirmed();
                
                if (expiredChildCount != 0)
                {
                    AddExistingMessage.Text = AddExistingMessage.Text + "\n" + expiredChildCount +
                                              " Usage right expired child nodes were not copied.";
                }
                if (expiredResourceAssociationCount != 0)
                {
                    AddExistingMessage.Text = AddExistingMessage.Text + "\n" + expiredResourceAssociationCount +
                                              " Usage right expired resource associations were not copied.";
                }
            }

            //if (selitems.Length >= 1 && !string.IsNullOrEmpty(SelectedItems.Value))
            //{
            //    string ddtype = DropDownList1.SelectedValue;
            //    AddExistingMessage.Text = selitems.Length + " document(s) successfully copied.";
            //    GetDocuments(userInfo, ddtype, DocumentType, Subtype);
            //    SelectedItems.Value = string.Empty;
            //}
            ///reface list after copy selected items.
        }

        internal static Boolean IsIncludeChildNodeRequested(string action)
        {
            if (action == null) action = "0";
            return (action == "1" || action == "3");
        }

        internal static Boolean IsIncludeAssociationRequested(string action)
        {
            if (action == null) action = "0";
            return (action == "2" || action == "3");
        }

        internal static int GetExpiredChildCountforNode(TreeProvider tp, CMS.DocumentEngine.TreeNode nodeToClone, CMS.DocumentEngine.TreeNode destinationNode, string action)
        {
            int i = 0;
            TreeNodeDataSet nodeToClone_DS = tp.SelectNodes(CMSContext.CurrentSiteName, nodeToClone.NodeAliasPath + "/%", "en-us", false);
            if (nodeToClone_DS != null && nodeToClone_DS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in nodeToClone_DS.Tables[0].Rows)
                {
                    CMS.DocumentEngine.TreeNode childDoc = DocumentHelper.GetDocument((int)row["NodeID"], "en-us", tp);
                    if (childDoc["ExpirationDate"] != null)
                    {
                        DateTime expirationDate = (DateTime)(childDoc["ExpirationDate"]);
                        if (expirationDate.Date < DateTime.Today)
                            i++;
                    }

                }
            }
            return i;
        }

        internal int GetExpiredAssociationCount(TreeProvider tp, CMS.DocumentEngine.TreeNode nodeToClone)
        {
            int count = 0;
            count = GetExpiredAssociationCountforNode(nodeToClone.NodeID);
            if (IsIncludeChildNodeRequested(DocumentAction.Value))
            {
                TreeNodeDataSet nodeToClone_DS = tp.SelectNodes(CMSContext.CurrentSiteName, nodeToClone.NodeAliasPath + "/%", "en-us", false);
                if (nodeToClone_DS != null && nodeToClone_DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in nodeToClone_DS.Tables[0].Rows)
                    {
                        if (!string.IsNullOrEmpty(row["NodeID"].ToString()))
                        {
                            count += GetExpiredAssociationCountforNode((int)row["NodeId"]);
                        }

                    }
                }
            }
            return count;
        }

        internal static int GetExpiredAssociationCountforNode(int nodeID)
        {
            int rowCount = 0;
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ToString()))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand("Thinkgate_GetExpiredAssociationCountforNode", sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@NodeId", nodeID);
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    rowCount = sqlCommand.ExecuteNonQuery();
                }
            }
            return rowCount < 0 ? 0 : rowCount;
        }

        protected void CopySelectedItemsConfirmed()
        {
            if (string.IsNullOrEmpty(SelectedItems.Value))
            {
                AddExistingMessage.Text = "Please select atleast one document to be copied.";
                return;
            }
            UserInfo userInfo = (UserInfo)Session["KenticoUserInfo"];
            TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(userInfo.UserName));
         
            string[] selitems = (SelectedItems.Value).Split(',');
            int parentNode;
            if (Int32.TryParse(ParentNodeID, out parentNode))
            {
                CMS.DocumentEngine.TreeNode toNode = tp.SelectSingleNode(parentNode);
                foreach (string t in selitems)
                {
                    int nodeid;
                    string docEntry = t;
                    if (Int32.TryParse(docEntry, out nodeid))
                    {
                        CMS.DocumentEngine.TreeNode fromNode = tp.SelectSingleNode(nodeid);
                        cloneNode(tp, fromNode, toNode, DocumentAction.Value);
                    }
                }
            }
            AddExistingMessage.Text = selitems.Length + " document(s) successfully copied.";
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ddtype = DropDownList1.SelectedValue;
            UserInfo userInfo = (UserInfo)Session["KenticoUserInfo"];//UserInfoProvider.GetUserInfo(CMSContext.CurrentUser.UserName);
            treeProvider = new TreeProvider(userInfo);
            GetDocuments(userInfo, ddtype, DocumentType, Subtype, filterExpiredMaterials);
        }

        internal static Boolean cloneNode(TreeProvider tp, CMS.DocumentEngine.TreeNode nodeToClone, CMS.DocumentEngine.TreeNode destinationNode, string action)
        {
            if (action == null) action = "0";
            bool includeChildNodes = (action == "1" || action == "3");
            bool includeAssociations = (action == "2" || action == "3");
            if ((tp != null) && (nodeToClone != null) && (destinationNode != null))
            {
                CMS.DocumentEngine.TreeNode newTreeNode = DocumentHelper.CopyDocument(nodeToClone, destinationNode.NodeID, false, tp);
                
                if (newTreeNode != null)
                {                    
                    //Remove Average Rating and Review count from the New Document
                    //US15667
                    if (IsShared == "true")
                    {
                        int allowUser = Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Delete))) + Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Read)));
                        int deniedUser = Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.Modify))) + Convert.ToInt32(Math.Pow(2, Convert.ToInt32(NodePermissionsEnum.ModifyPermissions)));
                        AclProvider aclUser = new AclProvider(tp);
                        aclUser.SetUserPermissions(newTreeNode, allowUser, deniedUser, theSharedUserRoles);
                    }

                    UpdateRatingAvgNCountsInKenticoDB(newTreeNode.NodeID);
                    
                    //Copy the parent node associations
                    if (includeAssociations)
                        CopyDocumentPlanAssociationDetails(nodeToClone.NodeID, newTreeNode.NodeID);

                    if (includeChildNodes)
                    {
                        DataSet nodeToClone_DS = tp.SelectNodes(CMSContext.CurrentSiteName, nodeToClone.NodeAliasPath + "/%", "en-us", false, null, null, null, 1);
                        if (nodeToClone_DS != null && nodeToClone_DS.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in nodeToClone_DS.Tables[0].Rows)
                            {
                                int src = (int) row["NodeId"];
                                CMS.DocumentEngine.TreeNode childNode = DocumentHelper.GetDocument(src, "en-us", tp);
                                if (childNode["ExpirationDate"] != null)
                                {
                                    DateTime expirationDate = Convert.ToDateTime(childNode["ExpirationDate"]);
                                    if (expirationDate.Date < DateTime.Today) continue;
                                }
                                
                                cloneNode(tp, childNode, newTreeNode, action);                                
                            }
                        }
                        }
                    }

                return true;
            }
            return false;
        }

         /// <summary>
        /// Copy Document Plan Association Details
        /// </summary>
        /// <param name="sourceDocumentID"></param>
        /// <param name="destinationDocumnetID"></param>
        /// <returns></returns>
        public static int CopyDocumentPlanAssociationDetails(int sourceDocumentId, int destinationDocumentId)
        {
            int rowCount = 0;
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ToString()))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand("Thinkgate_DocumentPlanAssociationDetails_Copy", sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@SourceDocumentId", sourceDocumentId);
                    sqlCommand.Parameters.AddWithValue("@DestinationDocumentId", destinationDocumentId);
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    rowCount = sqlCommand.ExecuteNonQuery();
                }
            }
            return rowCount;
        }




        public static void UpdateRatingAvgNCountsInKenticoDB(int nodeId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ToString()))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand("Thinkgate_UpdateRatingAverageAndCountInDocTypeTable", sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@nodeid", nodeId);
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Return Document Type References from Kentico DB for the tileParm _resourceToShow.
        /// </summary>
        private void GetDocuments(UserInfo ui, string filterType, string docType, string subtype, bool filterExpiredContent)
        {
            // Hide the template selection dropdown for thinkgate.resource since those do not have the concept of templates
            if (docType == THINKGATE_RESOURCE)
            {
                DropDownList1.Style["display"] = "none";
            }
            else
            {
                DropDownList1.Style["display"] = "block";
            }

            DataSet userNodeList = new DataSet();

            DataSet resourceToShow = KenticoHelper.GetTileMapLookupDataSet(docType);

            foreach (DataRow dr in resourceToShow.Tables[0].Rows)
            {
                DataSet tempUserNodeList = KenticoHelper.GetUserSharedDocuments(ui,
                    dr["KenticoDocumentTypeToShow"].ToString(), treeProvider, filterType, filterExpiredContent);
                DataColumn newColumn = new DataColumn("docDocumentType", typeof (System.String));
                newColumn.DefaultValue = dr["FriendlyName"].ToString();
                if (tempUserNodeList != null)
                {
                    tempUserNodeList.Tables[0].Columns.Add(newColumn);
                    if (userNodeList.Tables.Count > 0)
                        userNodeList.Tables[0].Merge(tempUserNodeList.Tables[0]);
                    else
                        userNodeList.Merge(tempUserNodeList);
                }
            }
            
            
            DataView filterdataView = userNodeList.Tables.Count > 0 ? userNodeList.Tables[0].DefaultView : new DataView();
            filterdataView.RowFilter = "SubType=" + subtype;

            if (filterdataView.Count > 0)
            {
                rptNames.DataSource = filterdataView;
                rptNames.DataBind();
            }
            else
            {
                rptNames.DataSource = returnEmptyDataSet(docType); //build empty dataset, probably a better way to do this...
                rptNames.DataBind();
            }
        }

        private static DataSet returnEmptyDataSet(string docType)
        {
            DataTable table1 = new DataTable();
            DataSet set = new DataSet(docType);

            string colName = string.Empty;
            switch (docType)
            {
                case "thinkgate.LessonPlan":
                    colName = "LessonPlanOverview";
                    break;
                case "thinkgate.UnitPlan":
                    colName = "UnitPlanOverview";
                    break;
                case "thinkgate.InstructionalPlan":
                    colName = "InstructionalPlanOverview";
                    break;
                case "thinkgate.resource":
                    colName = "Description";
                    break;
                default:
                    break;
            }

            table1.Columns.Add("NodeID");
            table1.Columns.Add(colName);
            table1.Columns.Add("DocumentName");
            table1.Columns.Add("NodeParentID");

            set.Tables.Add(table1);

            return set;
        }

    }

}