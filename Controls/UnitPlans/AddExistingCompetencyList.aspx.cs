using System;
using System.Configuration;
using System.Data;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;

using Thinkgate.Base.Classes.Data;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using System.Linq;
using System.Collections.Generic;
using Thinkgate.Base.Classes;
using System.Text;
using System.ComponentModel;



namespace Thinkgate.Controls.UnitPlans
{
    public partial class AddExistingCompetencyList : System.Web.UI.Page
    {
        private string rootConnectionString = ConfigurationManager.ConnectionStrings["root_application"].ConnectionString;
        private const string THINKGATE_CompetencyList = "Thinkgate.CompetencyList";
        private CourseList _currCourseList;
        private static List<int> filterCurriculumIDs = new List<int>();
        private static bool isPostBack = false;
        private static DataTable allUsers = new DataTable();

        public SessionObject SessionObject;

        private enum KenticoDocumentType
        {
            InstructionalPlan = 1,
            UnitPlan = 2,
            LessonPlan = 3,
            Resource = 4,
            CurriculumUnit = 5,
            CompetencyList = 6
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
            SessionObject = (SessionObject)Session["SessionObject"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CMSContext.Init();
            if (!Page.IsPostBack)
            {
                BindAllGrades();
                Session["parentnodeid"] = QueryHelper.GetString("parentnodeid", "1");
                isPostBack = true;
                requestType.Value = Convert.ToString(QueryHelper.GetString("reqtype", "0"));
                for (int i = wndWindowManager.Windows.Count - 1; i > -1; i--)
                {
                    if (!new List<string> { "wndAddDocument", "wndCmsNewDocumentShell" }.Contains(wndWindowManager.Windows[i].ID))
                    {
                        wndWindowManager.Windows.Remove(wndWindowManager.Windows[i]);
                    }
                }
            }
        }



        [System.Web.Services.WebMethod]
        public static string CopySelectedItems(string NodeId, string desc, string currID)
        {
            int parentNode;
            string name = "";
            string[] selitems = (NodeId).Split(',');
            UserInfo userInfo = (UserInfo)System.Web.HttpContext.Current.Session["KenticoUserInfo"];
            SessionObject SessionObject = (SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(userInfo.UserName));
            string ParentNodeID = Convert.ToString(System.Web.HttpContext.Current.Session["parentnodeid"]);
            DataTable tdatatable = new DataTable();

            if (Int32.TryParse(ParentNodeID, out parentNode))
            {
                CMS.DocumentEngine.TreeNode toNode = tp.SelectSingleNode(parentNode);
                for (var i = 0; i < selitems.Length; i++)
                {
                    int nodeid;
                    string docEntry = selitems[i];
                    if (Int32.TryParse(docEntry, out nodeid))
                    {
                        CMS.DocumentEngine.TreeNode fromNode = tp.SelectSingleNode(nodeid);
                        #region For Competency List
                        //Create a new Tree node at the specified 
                        CMS.DocumentEngine.TreeNode node = CMS.DocumentEngine.TreeNode.New(THINKGATE_CompetencyList, tp);
                        //Get Document Page Template Id from Competency Class.
                        CMS.SettingsProvider.DataClassInfo ci = CMS.SettingsProvider.DataClassInfoProvider.GetDataClass(THINKGATE_CompetencyList);

                        name = fromNode.DocumentName;
                        if (name != "" && name.Length > 50)
                            name = name.Substring(0, 50);

                        if (desc != "" && desc.Length > 100)
                            desc = desc.Substring(0, 100);

                        if (ParentNodeID != null)
                        {

                            //Now set the properties for the document if the parent node found
                            node.DocumentName = fromNode.DocumentName;
                            node.NodeName = fromNode.NodeName;
                            node.NodeAlias = fromNode.NodeAliasPath;
                            node.DocumentCulture = fromNode.DocumentCulture;
                            node.SetValue("Name", name);
                            node.SetValue("Description", desc);
                            node.SetValue("Sharing", false);
                            node.SetValue("Standards", "[[S]]");
                            node.SetValue("Curricula", "[[C]]");
                            node.DocumentPageTemplateID = ci.ClassDefaultPageTemplateID;

                            //Now insert the document at the proper place New document
                            node.Insert(toNode.NodeID);
                            int newNodeId = node.NodeID;                           
                            CopyDocumentPlanAssociationDetails(fromNode.NodeID, newNodeId, int.Parse(currID));                         

                        }
                        #endregion
                    }
                }
            }


            if (selitems.Length >= 1)
            {
                return selitems.Length + " document(s) successfully copied.";
            }
            return "";       

        }

        [System.Web.Services.WebMethod]
        public static string ValidateDocCurricula(string NodeId)
        {
            UserInfo userInfo = (UserInfo)System.Web.HttpContext.Current.Session["KenticoUserInfo"];
            TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(userInfo.UserName));
            string[] selitems = (NodeId).Split(',');
            CMS.DocumentEngine.TreeNode fromNode;
            DataTable tdatatable = new DataTable();
            fromNode = null;
            for (var i = 0; i < selitems.Length; i++)
            {
                int nodeid;
                string docEntry = selitems[i];
                if (Int32.TryParse(docEntry, out nodeid))
                {
                    fromNode = tp.SelectSingleNode(nodeid);
                    tdatatable = getCurrentCurricula(fromNode.NodeID);
                }
            }

            if (tdatatable.Rows.Count > 1)
            {
                return tdatatable.ToJSON(false);
            }
            return string.Empty;
        }

        [System.Web.Services.WebMethod]
        public static string ValidateDocStandards(string NodeId)
        {
            UserInfo userInfo = (UserInfo)System.Web.HttpContext.Current.Session["KenticoUserInfo"];
            TreeProvider tp = new TreeProvider(UserInfoProvider.GetFullUserInfo(userInfo.UserName));
            DataTable dsstandard = new DataTable();
            string[] selitems = (NodeId).Split(',');
            CMS.DocumentEngine.TreeNode fromNode;            
            fromNode = null;
            for (var i = 0; i < selitems.Length; i++)
            {
                int nodeid;
                string docEntry = selitems[i];
                if (Int32.TryParse(docEntry, out nodeid))
                {
                    fromNode = tp.SelectSingleNode(nodeid);                    
                    Base.DataAccess.dtGeneric_Int _standardids = new Base.DataAccess.dtGeneric_Int();
                    _standardids.Add(fromNode.NodeID);                    
                    dsstandard = CompetencyWorkSheet.GetCurrStabdardsById_Kentico(_standardids, true);
                }
            }

            if (dsstandard!=null && dsstandard.Rows.Count>500)
            {
                return dsstandard.Rows.Count.ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// Copy Document Plan Association Details
        /// </summary>
        /// <param name="sourceDocumentID"></param>
        /// <param name="destinationDocumnetID"></param>
        /// <returns></returns>
        public static DataTable CopyDocumentPlanAssociationDetails(int sourceDocumentId, int destinationDocumentId, int CurriculumId)
        {
            //int rowCount = 0;
            SessionObject SessionObject = (SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            int userId = Convert.ToInt32(SessionObject.GlobalInputs[1].Value);
            DataTable dt = CompetencyWorkSheet.DocumentAssociationForCompetency(userId, sourceDocumentId, destinationDocumentId, CurriculumId);
            if (dt != null)
            {
                return dt;
            }
            return new DataTable();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string getDistrictFromUserName()
        {
            string theDistrict = string.Empty;
            UserInfo ui = (UserInfo)System.Web.HttpContext.Current.Session["KenticoUserInfo"];
            string username = ui.UserName;

            string[] wrk = username.Split('-');

            theDistrict = wrk[0];

            return theDistrict;
        }

        public static DataTable getCurrentCurricula(int docid)
        {

            DataTable dt = CompetencyWorkSheet.GetCompetencyCurriculum(docid, getDistrictFromUserName());
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Get All Resource Types by client.
        /// </summary>
        /// 
        [System.Web.Services.WebMethod]
        public static string BindResourceTypes()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (System.Web.HttpContext.Current.Session["docTypeList"] != null)
            {
                sb.Append("<option value=\"All\"> ---- Select Item ----</option>");
                sb.Append("<option value=\"All\">All</option>");

                List<KeyValuePair<string, string>> docTypeList = new List<KeyValuePair<string, string>>();
                docTypeList = (List<KeyValuePair<string, string>>)System.Web.HttpContext.Current.Session["docTypeList"];

                if (docTypeList != null)
                {
                    foreach (var s in docTypeList)
                    {
                        sb.Append("<option value='" + s.Key + "'> " + s.Value + "</option>");
                    }
                }
                isPostBack = false;
            }

            return sb.ToString();
        }

        private void BindAllGrades()
        {
            Thinkgate.Base.Classes.Administration Admin = new Thinkgate.Base.Classes.Administration();
            _currCourseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);

            var gradeList = _currCourseList.GetGradeList();
            var dtGrade = new DataTable();
            dtGrade.Columns.Add("ID");
            dtGrade.Columns.Add("Grade");
            foreach (var grade in gradeList)
            {
                var row = dtGrade.NewRow();
                row["ID"] = grade.DisplayText;
                row["Grade"] = grade.DisplayText;
                dtGrade.Rows.Add(row);
            }

            DataRow newRow = dtGrade.NewRow();
            newRow["ID"] = "All";
            newRow["Grade"] = " ---- Select Item ---- ";
            dtGrade.Rows.InsertAt(newRow, 0);

            // Data bind the combo box.
            GradeDdl.DataTextField = "Grade";
            GradeDdl.DataValueField = "ID";
            GradeDdl.DataSource = dtGrade;
            GradeDdl.DataBind();

        }

        [System.Web.Services.WebMethod]
        public static string BindCompetencyGrid(string scope, string classname, string grade, string gradeVal, string subject, string subjectVal, string course, string courseVal, string searchOption, string searchText)
        {
            List<CompetencyDocument> lstResource = new List<CompetencyDocument>();
            List<CompetencyDocument> resultResource = new List<CompetencyDocument>();
            StringBuilder whereclause = new StringBuilder();
            ResourceTypes oscope = ResourceTypes.All;
            string strwhereclause = string.Empty;
            string oclassname = "All";
            string ograde = null;
            string osubject = null;
            string ocourse = null;
            string strwordstring = string.Empty;
            string selectedsearchoption = "any";
            #region Code
            UserInfo ui = (UserInfo)System.Web.HttpContext.Current.Session["KenticoUserInfo"];
            SessionObject SessionObject = (SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            TreeProvider treeProvider = new TreeProvider(ui);

            treeProvider = Thinkgate.Classes.KenticoHelper.GetUserTreeProvider(SessionObject.LoggedInUser.ToString());

            DataSet lookupDataSet = new DataSet();
            List<CompetencyDocument> lstCLCUUPLP = new List<CompetencyDocument>();

            if (scope != "undefined")
            {
                oscope = (ResourceTypes)System.Enum.Parse(typeof(ResourceTypes), scope);
            }
            else
            {
                oscope = ResourceTypes.All;
            }
            oclassname = classname != "undefined" ? classname : "All";

            if (gradeVal != "All" && gradeVal != "" && gradeVal != "undefined")
            {
                ograde = gradeVal;
            }
            else
            {
                ograde = null;
            }
            if (subjectVal != "" && subjectVal != "0" && subjectVal != "undefined")
            {
                osubject = subjectVal;
            }
            else
            {
                osubject = null;
            }
            if (courseVal != "" && courseVal != "0" && courseVal != "undefined")
            {
                ocourse = courseVal;
            }
            else
            {
                ocourse = null;
            }




            strwhereclause = whereclause.ToString();
            allUsers = AdministrationDB.GetAllUsers();
            //For CL, CM, UP, and LP
            if (oclassname != "All")
            {
                lstCLCUUPLP.AddRange(GetDocumentResource(ui, oclassname, treeProvider, strwhereclause,
                    string.Empty, strwordstring, lookupDataSet, oscope));
            }           
            else
            {
                DataSet ds = GetListTypes();
                List<KeyValuePair<string, string>> doctypeList = new List<KeyValuePair<string, string>>();
                if (ds != null && ds.Tables.Count > 0)
                {
                    IList<KeyValuePair<string, string>> docList = ds.Tables[0].AsEnumerable().Select(x => new KeyValuePair<string, string>(x["KenticoDocumentTypeToShow"].ToString(), x["FriendlyName"].ToString())).ToList();

                    foreach (var item in docList)
                    {
                        var genricList = GetDocumentResource(ui, item.Key, treeProvider, strwhereclause, string.Empty, strwordstring, lookupDataSet, oscope);
                        lstCLCUUPLP.AddRange(genricList);
                        if (genricList.Count > 0) doctypeList.Add(item);
                    }
                    System.Web.HttpContext.Current.Session["docTypeList"] = doctypeList;
                }

            }

            //Merging Resource result set to CL,CM,UP, and LP result set
            lstResource.AddRange(lstCLCUUPLP);

            if (!(ograde == null && osubject == null && ocourse == null))
            {
                lstResource = FilterList(ograde, osubject, ocourse, lstResource);
            }

            selectedsearchoption = searchOption;
            if (!string.IsNullOrWhiteSpace(searchText) && searchText != "undefined")
            {
                switch (selectedsearchoption)
                {
                    case "all":
                        string[] filterstr2 = searchText.ToLower().Trim().Split(' ').Select(z => z.Trim()).ToArray();
                        lstResource = lstResource.Where(x => filterstr2.All(y => x.DocumentType.ToLower().Contains(y)) || filterstr2.All(y => x.ResourceName.ToLower().Contains(y)) || filterstr2.All(y => x.Description.ToLower().Contains(y)) || filterstr2.All(y => x.AuthorName.ToLower().Contains(y))).ToList();
                        break;
                    case "exact":
                        lstResource = lstResource.Where(x => x.DocumentType.ToLower().Contains(searchText.ToLower()) || x.ResourceName.ToLower().Contains(searchText.ToLower()) || x.Description.ToLower().Contains(searchText.ToLower()) || x.AuthorName.ToLower().Contains(searchText.ToLower())).ToList();
                        break;

                    case "author":
                        lstResource = lstResource.Where(x => x.AuthorName.ToLower().Contains(searchText.ToLower())).ToList();
                        break;

                    default:
                        string[] filterstr = searchText.ToLower().Trim().Split(' ').Select(z => z.Trim()).ToArray();
                        lstResource = lstResource.Where(x => filterstr.Any(y => x.DocumentType.ToLower().Contains(y)) || filterstr.Any(y => x.ResourceName.ToLower().Contains(y)) || filterstr.Any(y => x.Description.ToLower().Contains(y)) || filterstr.Any(y => x.AuthorName.ToLower().Contains(y))).ToList();
                        break;
                }

            }

            lstResource = (from oderrow in lstResource.ToList()
                           orderby oderrow.DocumentNodeID descending
                           select oderrow).ToList();


            resultResource =
                    lstResource.GroupBy(
                        x =>
                            new
                            {
                                x.ID,
                                x.DocumentNodeID,
                                x.DocumentName,
                                x.Description,
                                x.ViewLink,
                                x.NodeAliasPath,
                                x.DocumentForeignKeyValue,
                                x.DocumentType,
                                x.AuthorName
                            })
                        .Select(r => new CompetencyDocument()
                        {
                            ID = r.Key.ID,
                            DocumentNodeID = r.Key.DocumentNodeID,
                            ResourceName = r.Key.DocumentName,
                            Description = r.Key.Description,
                            ViewLink = r.Key.ViewLink,
                            NodeAliasPath = r.Key.NodeAliasPath,
                            DocumentForeignKeyValue = r.Key.DocumentForeignKeyValue,
                            DocumentType = r.Key.DocumentType,
                            AuthorName = r.Key.AuthorName,
                        }).ToList<CompetencyDocument>();

            DataTable tCollection = new DataTable();
            tCollection = ConvertToDataTable(resultResource);
            return tCollection.ToJSON(false);
            #endregion
        }

        /// <summary>
        /// Returns a dataset of curriculums based on grade, subject and course from standards table
        /// </summary>
        private static List<int> GetCurriculumIDsforStandards(string grade, string subject, string course)
        {
            List<int> nfilterStandardIDs = new List<int>();
            DataSet ds = MCU.GetCurriculumCousesforClassCourses(grade, subject, course);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        nfilterStandardIDs.Add((int)row["ID"]);
                    }
                }
            }
            return nfilterStandardIDs;
        }

        /// <summary>
        /// Filters the list results based on dropdown filters
        /// </summary>
        private static List<CompetencyDocument> FilterList(string grade, string subject, string course, List<CompetencyDocument> lstResource)
        {


            // curriculumIDs for current filter settings
            List<int> filterStandardIDs = new List<int>();
            filterStandardIDs = GetCurriculumIDsforStandards(grade, subject, course);
            List<CompetencyDocument> nodesToRemove = new List<CompetencyDocument>();
            List<int> filteredNodeIds = new List<int>();

            // show only matches for curriculum IDs
            foreach (CompetencyDocument nl in lstResource)
            {
                bool show = false;
                var nodeListStandardIDs = nl.AssociatedStandardIds.ToList();

                if (nodeListStandardIDs.Any() && filterStandardIDs.Any())
                {
                    if ((from s in nodeListStandardIDs
                         join f in filterStandardIDs on Convert.ToInt32(s.ToString()) equals Convert.ToInt32(f.ToString())
                         select s).ToList().Count > 0)
                        show = true;
                }

                // if show flag is false, or if we had no IDs to compare against, hide the node.                    
                if (show)
                {
                    filteredNodeIds.Add(nl.ID);
                }

            }

            if (filteredNodeIds.Count != 0)
            {
                lstResource = lstResource.Where(x => filteredNodeIds.Contains(x.ID)).ToList();
            }
            else
            {
                lstResource = new List<CompetencyDocument>();
            }
            return lstResource;
        }


        protected static List<CompetencyDocument> GetDocumentResource(UserInfo ui, string className, TreeProvider treeProvider, string whereClause, string orderby, string whereTextClause, DataSet lookupDataSet, ResourceTypes pResourceType = ResourceTypes.All)
        {
            List<CompetencyDocument> lstResource = new List<CompetencyDocument>();
            List<CompetencyDocument> finalstResource = new List<CompetencyDocument>();
            DataSet resourcesToShow = KenticoHelper.GetTileMapLookupDataSet(className);

            string[] _strResourcesToShowOnTile;
            _strResourcesToShowOnTile = resourcesToShow.Tables[0].AsEnumerable().Select(s => s.Field<string>("KenticoDocumentTypeToShow")).ToArray<string>();
            foreach (DataRow dr in resourcesToShow.Tables[0].Select().OrderBy(p => p["ItemOrder"].ToString()))
            {
                string[] ColumnNames = new string[] { dr["DescriptionColumnName"].ToString(), dr["NameColumnName"].ToString(), dr["AttachmentColumnName"].ToString(), dr["FriendlyName"].ToString() };

                List<CompetencyDocument> Resource = GetDocuments(ui, dr["KenticoDocumentTypeToShow"].ToString(), treeProvider, whereClause, string.Empty, ColumnNames, whereTextClause, lookupDataSet, pResourceType);

                if (Resource != null && Resource.Count == 0)
                {
                    Resource = new List<CompetencyDocument>();
                }
                lstResource.AddRange(Resource);
            }
            Base.DataAccess.dtGeneric_Int _standardids = new Base.DataAccess.dtGeneric_Int();
            var documentIds = lstResource.Select(x => x.DocumentNodeID).ToList();
            foreach (var id in documentIds)
            {
                _standardids.Add(id);
            }

            DataTable dsstandard = new DataTable();
            try
            {
                dsstandard = CompetencyWorkSheet.GetCurrStabdardsById_Kentico(_standardids, false);
            }
            catch (Exception ex)
            {

                throw new Exception(String.Format("The Query did not give any results {0}", ex.Message));

            }
            List<DocStandardsCollections> collections = new List<DocStandardsCollections>();
            foreach (DataRow row in dsstandard.Rows)
            {
                DocStandardsCollections col = new DocStandardsCollections();
                col.StandardId = Convert.ToInt32(row[0]);
                col.DocID = Convert.ToInt32(row[1]);
                collections.Add(col);
            }


            finalstResource = (from m in lstResource
                               select new CompetencyDocument { DocID = m.DocID, AuthorName = m.AuthorName, Description = m.Description, DocumentForeignKeyValue = m.DocumentForeignKeyValue, DocumentName = m.DocumentName, DocumentNodeID = m.DocumentNodeID, DocumentType = m.DocumentType, ID = m.ID, ItemID = m.ItemID, NodeAliasPath = m.NodeAliasPath, ReferenceID = m.ReferenceID, ResourceName = m.ResourceName, ListType = m.ListType, ViewLink = m.ViewLink, AssociatedStandardIds = collections.Where(y => y.DocID == m.DocumentNodeID).Select(z => z.StandardId).Distinct().ToArray() }).ToList();

            return finalstResource;
        }

        protected static DataSet GetListTypes()
        {

            CustomTableItemProvider tp = new CMS.SiteProvider.CustomTableItemProvider((UserInfo)System.Web.HttpContext.Current.Session["KenticoUserInfo"]);
            string filtercriteria = " (ISNULL(StateLEA,'')='' or StateLEA = '" + DistrictParms.LoadDistrictParms().ClientID + "' or StateLEA = '" + DistrictParms.LoadDistrictParms().State + "') and LookupValue<>" + ((int)KenticoDocumentType.Resource).ToString() + " ORDER BY FriendlyName";
            DataSet resourcesToShow = tp.GetItems("thinkgate.TileMap_Lookup", filtercriteria, string.Empty);
            return resourcesToShow;
        }

        protected static List<CompetencyDocument> GetDocuments(UserInfo userInfo, string className, TreeProvider treeProvider, string where, string orderby, string[] ColumnNames, string whereTextClause, DataSet lookupDataSet, ResourceTypes pResourceType = ResourceTypes.All)
        {
            if (whereTextClause != "")
            {
                where += where == "" ? ColumnNames[1] + whereTextClause : " AND " + ColumnNames[1] + whereTextClause;
            }
            DataSet resourceDS;
            if (className == "thinkgate.UnitPlan" || className == "thinkgate.InstructionalPlan")
            {
                 resourceDS = KenticoHelper.ExpandedSearchDocumentType(userInfo, className, treeProvider, pResourceType, where, orderby, true);
            }
            else
            {
                 resourceDS = KenticoHelper.ExpandedSearchDocumentType(userInfo, className, treeProvider, pResourceType, where, orderby);
            }
            List<CompetencyDocument> lstResource = new List<CompetencyDocument>();
            string clientID = DistrictParms.LoadDistrictParms().ClientID;
            if (resourceDS != null)
            {

                lstResource = (from res in resourceDS.Tables[0].AsEnumerable()
                               join u in allUsers.AsEnumerable()
                               on res.Field<string>("NodeOwnerUserName") equals KenticoHelper.GetKenticoUser(u.Field<string>("UserName"))
                               into lj
                               from u in lj.DefaultIfEmpty()
                               select new CompetencyDocument
                               {
                                   NodeAliasPath = res["NodeAliasPath"].ToString(),
                                   ResourceName = res[ColumnNames[1]].ToString(),
                                   Description = res[ColumnNames[0]].ToString(),
                                   ViewLink = ColumnNames[2] != "" ? string.IsNullOrEmpty(res[ColumnNames[2]].ToString()) ? "" : res[ColumnNames[2]].ToString() : string.Empty,
                                   ItemID = Convert.ToInt32(res["DocumentID"]),
                                   DocID = Convert.ToInt32(res["DocumentNodeID"]),
                                   ID = Convert.ToInt32(res["DocumentID"]),
                                   DocumentName = res["DocumentName"].ToString(),
                                   DocumentForeignKeyValue = Convert.ToInt32(res["DocumentForeignKeyValue"].ToString()),
                                   ListType = res["ClassName"].ToString(),
                                   DocumentType = ColumnNames[3],
                                   DocumentNodeID = Convert.ToInt32(res["DocumentNodeID"]),
                                   AuthorName = u != null ? u["LastName"] + "," + u["FirstName"] : "STATE USER"
                               }).ToList<CompetencyDocument>();

                return lstResource;

            }
            return lstResource;
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

    }
    public class CompetencyDocument
    {
        public int ItemID { get; set; }
        public int DocID { get; set; }
        public int ID { get; set; }
        public int DocumentNodeID { get; set; }
        public int ReferenceID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public string NodeAliasPath { get; set; }
        public string ResourceName { get; set; }
        public string Description { get; set; }
        public string ViewLink { get; set; }
        public int DocumentForeignKeyValue { get; set; }
        public string ListType { get; set; }
        public string AuthorName { get; set; }
        public IEnumerable<int> AssociatedStandardIds { get; set; }

    }
    public class DocStandardsCollections
    {
        public int DocID { get; set; }
        public int StandardId { get; set; }
    }
}
