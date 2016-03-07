using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using System.Collections;
using Thinkgate.Services;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentAddendumsDetail : System.Web.UI.Page
    {
        protected SessionObject SessionObject;
        protected string subject;
        protected int courseID;
        protected string courseName;
        protected string grade;
        protected string type;
        protected int term;
        protected string description;
        protected string testCategory;
        protected int currUserID;
        protected StandardRigorLevels rigorLevels;
        protected static StandardAddendumList standardAddendumList;

        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(type);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Page.Session["SessionObject"];
            subject = SessionObject.AssessmentBuildParms.ContainsKey("Subject") ? SessionObject.AssessmentBuildParms["Subject"] : "";
            courseID = SessionObject.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]) : 0;
            Base.Classes.Course assessmentCourse = CourseMasterList.GetCurrCourseById(courseID);
            courseName = assessmentCourse != null ? assessmentCourse.CourseName : "";
            grade = SessionObject.AssessmentBuildParms.ContainsKey("Grade") ? SessionObject.AssessmentBuildParms["Grade"] : "";
            type = SessionObject.AssessmentBuildParms.ContainsKey("Type") ? SessionObject.AssessmentBuildParms["Type"] : "";
            term = SessionObject.AssessmentBuildParms.ContainsKey("Term") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Term"]) : 0;
            description = SessionObject.AssessmentBuildParms.ContainsKey("Description") ? SessionObject.AssessmentBuildParms["Description"] : "";
            currUserID = SessionObject.LoggedInUser != null ? (SessionObject.LoggedInUser.Page > 0 ? SessionObject.LoggedInUser.Page : AppSettings.Demo_TeacherID) : AppSettings.Demo_TeacherID;
            testCategory = SessionObject.AssessmentBuildParms.ContainsKey("TestCategory") ? SessionObject.AssessmentBuildParms["TestCategory"] : "";

            bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
            Dictionary<string, bool> dictionaryItem;
            dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(testCategory);
            bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();

            hiddenAccessSecureTesting.Value = hasPermission.ToString();
            hiddenIsSecuredFlag.Value = isSecuredFlag.ToString();
            hiddenSecureType.Value = SecureType.ToString();

            rigorLevels = SessionObject.Standards_RigorLevels_ItemCounts;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                switch (Request.QueryString["headerImg"])
                {
                    case "repairtool":
                        headerImg.Src = "../../Images/repairtool.png";
                        headerImg.Attributes["headerImgName"] = "repairtool";
                        break;
                    default:
                        headerImg.Visible = false;
                        break;
                }
                standardAddendumList = new StandardAddendumList();
                assessmentTitle.InnerHtml = "Term " + term.ToString() + " " + type + " - " + grade + " Grade " + subject + " " + (courseName == subject ? "" : courseName) + " - " + description;

                if (!string.IsNullOrEmpty(Request.QueryString["page"]))
                {
                    if (Request.QueryString["page"].ToString() == "summary")
                    {
                        standardAddendumList = (StandardAddendumList)Session["AddendumSummary"];
                        Session.Remove("AddendumSummary");
                    }
                }

                LoadSelectedStandards();
            }
        }

        protected void LoadSelectedStandards()
        {
            List<KeyValuePair<int, string>> standards = rigorLevels.StandardItemNames.ToList();
            List<int> standardIds = standards.Select(st => st.Key).ToList();
            DataTable standardsStateNbrs = Standards.GetStandardsStateNbrByID(standardIds);

            itemsSpecified.Value = rigorLevels.StandardItemTotals.Select(x => Convert.ToInt32(x.Value[1])).ToArray().Sum().ToString();
            initialTotalItemCount.Value = itemsSpecified.Value;
            if(standardAddendumList != null)
            {
                if (standardAddendumList.TotalItemCount > 0)
                {
                    itemsSpecified.Value = standardAddendumList.TotalItemCount.ToString();
                }
            }
            DataSet addendumsDataSet = Addendum.GetAddendumsByStandards(standardIds);

            if (addendumsDataSet != null && addendumsDataSet.Tables.Count > 0)
            {
                GenerateAddendumTableHeader(standardsStateNbrs);
                GenerateAddendumTableDetails(addendumsDataSet.Tables[0], standardsStateNbrs);

                if (addendumsDataSet.Tables[0].Rows.Count == 0)
                {
                    addendumTable.Attributes["style"] = "display:none;";
                    lblNoAddendum.Visible = true;
                    noAddendum.Value = true.ToString();
                }
            }
        }

        private void GenerateAddendumTableHeader(DataTable standards)
        {
            HtmlTableRow row = new HtmlTableRow();

            foreach (DataRow dataRow in standards.Rows)
            {
                string stateNbr = dataRow["StateNbr"].ToString();

                HtmlTableCell standardColumn1Cell = new HtmlTableCell();
                standardColumn1Cell.Attributes["class"] = "contentElement";
                standardColumn1Cell.Attributes["valign"] = "top";
                standardColumn1Cell.Attributes["standardID"] = dataRow["StandardID"].ToString();
                standardColumn1Cell.ColSpan = 2;
                standardColumn1Cell.Attributes["title"] = stateNbr + " : " + dataRow["Description"].ToString();

                if (stateNbr.Length > 7)
                {
                    stateNbr = stateNbr.Substring(stateNbr.Length - 7, 7);
                }

                HtmlGenericControl labelStateNbr = new HtmlGenericControl();
                labelStateNbr.InnerText = string.Format("{0}", stateNbr);
                labelStateNbr.Attributes["class"] = "contentLabel";
                labelStateNbr.Attributes["StdID"] = dataRow["StandardID"].ToString();
                labelStateNbr.Attributes["StdName"] = dataRow["StandardName"].ToString();

                HtmlGenericControl labelCount = new HtmlGenericControl();
                KeyValuePair<int, ArrayList> itemCounts = rigorLevels.StandardItemTotals.ToList().Find(i => i.Key == Convert.ToInt32(dataRow["StandardID"].ToString()));
                int count = Convert.ToInt32(itemCounts.Value[1]);
                if (standardAddendumList != null)
                {
                    if (standardAddendumList.StandardCounts != null)
                    {
                        count = standardAddendumList.StandardCounts.Where(x => x.StandardID == Convert.ToInt32(dataRow["StandardID"])).ToList()[0].Count;
                    }
                }

                labelCount.InnerText = string.Format(" ({0})", count.ToString());
                labelCount.Attributes["class"] = "contentLabelNormal";

                standardColumn1Cell.Attributes["originalItemCount"] = itemCounts.Value[1].ToString();
                standardColumn1Cell.Attributes["newItemCount"] = count.ToString();

                standardColumn1Cell.Controls.Add(labelStateNbr);
                standardColumn1Cell.Controls.Add(labelCount);
                row.Cells.Add(standardColumn1Cell);

                standardsTableFixedRow.Rows.Add(row);
            }          
        }

        private void GenerateAddendumTableDetails(DataTable addendumsDataTable, DataTable standards)
        {
            DataTable addendums = addendumsDataTable.DefaultView.ToTable(true, "AddendumID", "AddendumName", "ItemBank");

            foreach (DataRow addendum in addendums.Rows)
            {
                HtmlTableRow row = new HtmlTableRow();
                row.Attributes["AddendumID"] = addendum["AddendumID"].ToString();

                HtmlTableCell selectAllItemsCell = new HtmlTableCell();
                selectAllItemsCell.Attributes["class"] = "contentSelectAllItems";

                HtmlInputCheckBox chkSelectAllItems = new HtmlInputCheckBox();
                chkSelectAllItems.ID = addendum["AddendumID"].ToString().ToLower() + addendum["ItemBank"].ToString().ToLower();
                chkSelectAllItems.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                chkSelectAllItems.Attributes["onchange"] = "SelectItems(this, " + addendum["AddendumID"].ToString() + "); return false;";
                chkSelectAllItems.Attributes["AddendumIDItemBank"] = addendum["AddendumID"].ToString().ToLower() + addendum["ItemBank"].ToString().ToLower();
                if (standardAddendumList != null)
                {
                    if (standardAddendumList.SelectAllStates != null)
                    {
                        chkSelectAllItems.Checked = standardAddendumList.SelectAllStates.Where(x => x.SelectAllId == addendum["AddendumID"].ToString().ToLower() + addendum["ItemBank"].ToString().ToLower()).ToList()[0].State;
                    }
                }
                selectAllItemsCell.Controls.Add(chkSelectAllItems);

                HtmlTableCell addendumNameCell = new HtmlTableCell();
                addendumNameCell.Attributes["class"] = "contentAddendumName";
                string addendumIdEnc = Standpoint.Core.Classes.Encryption.EncryptString(addendum["AddendumID"].ToString());

                HyperLink addendumNameCellData = new HyperLink();
                addendumNameCellData.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                addendumNameCellData.NavigateUrl = "javascript: void(0)";
                addendumNameCellData.Attributes["onclick"] = "OpenAddendumText('" + addendumIdEnc + "'); return false;";
                addendumNameCellData.Attributes["AddendumID"] = addendum["AddendumID"].ToString();
                addendumNameCellData.Attributes["title"] = addendum["AddendumName"].ToString();
                addendumNameCellData.Attributes["class"] = "nameLinkPartialDisplay";
                addendumNameCellData.Text = addendum["AddendumName"].ToString();
                addendumNameCell.Controls.Add(addendumNameCellData);

                HtmlGenericControl addendumNameIBData = new HtmlGenericControl();
                addendumNameIBData.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                addendumNameIBData.InnerText = string.Format(" ({0})", addendum["ItemBank"].ToString());
                addendumNameIBData.Attributes["title"] = addendum["ItemBank"].ToString();
                addendumNameCell.Controls.Add(addendumNameIBData);

                HtmlTableCell selectedItemsCell = new HtmlTableCell();
                selectedItemsCell.Attributes["class"] = "contentHeaderSelItemsNormal";
                int count = 0;
                if(standardAddendumList != null)
                {
                    if (standardAddendumList.AddendumCounts != null && standardAddendumList.AddendumCounts.Count() > 0)
                    {
                        var addendumCountsList = standardAddendumList.AddendumCounts.Where(x => x.AddendumID == Convert.ToInt32(addendum["AddendumID"]));
                        if (addendumCountsList.Count() > 0)
                        {
                            count = addendumCountsList.ToList()[0].Count;
                    }
                }
                }
                selectedItemsCell.InnerText = count.ToString();

                row.Cells.Add(selectAllItemsCell);
                row.Cells.Add(addendumNameCell);
                row.Cells.Add(selectedItemsCell);

                /* Add row into fixed columns table */
                standardsTableFixedCols.Rows.Add(row);

                HtmlTableRow rowDetail = new HtmlTableRow();
                rowDetail.Attributes["AddendumID"] = addendum["AddendumID"].ToString();

                foreach (DataRow dataRow in standards.Rows)
                {
                    int itemCount = 0;
                    DataRow[] rows = addendumsDataTable.Select("AddendumID = " + addendum["AddendumID"].ToString() + " And StandardId = " + dataRow["StandardID"].ToString() + " ");
                    if (rows != null && rows.Length > 0)
                    {
                        itemCount = Convert.ToInt32(rows[0]["ItemCount"].ToString());
                    }

                    HtmlTableCell standardCell1 = new HtmlTableCell();
                    HtmlTableCell standardCell2 = new HtmlTableCell();
                    
                    if (itemCount > 0)
                    {
                        HtmlInputText itemCountCell = new HtmlInputText();
                        itemCountCell.ID = addendum["AddendumID"].ToString().ToLower() + addendum["ItemBank"].ToString().ToLower() + rows[0]["StandardId"].ToString().ToLower() + "_input";
                        itemCountCell.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        itemCountCell.Size = 2;
                        itemCountCell.Attributes["standardID"] = rows[0]["StandardId"].ToString();
                        itemCountCell.Attributes["itemCount"] = itemCount.ToString();
                        itemCountCell.Attributes["onchange"] = "if(this.value=='') this.className='empty';";
                        itemCountCell.Attributes["onkeypress"] = "return onlyNumsLessOrEqualToCount('" + itemCountCell.ID + "', this, event);";
                        itemCountCell.Attributes["onkeyup"] = "getTotalItemCount('" + initialTotalItemCount.Value + "', this, true);";
                        standardCell1.Attributes["class"] = "contentElementData";

                        standardCell1.Attributes["standardID"] = rows[0]["StandardId"].ToString();
                        standardCell2.Attributes["standardID"] = rows[0]["StandardId"].ToString();

                        if(standardAddendumList != null)
                        {
                            if(standardAddendumList.AddendumLevels != null)
                            {
                                List<StandardAddendum> list = standardAddendumList.AddendumLevels.Where(x => x.AddendumID == Convert.ToInt32(addendum["AddendumID"])).ToList();
                                if(list.Count > 0)
                                {
                                    list = list.Where(x => x.StandardID == Convert.ToInt32(rows[0]["StandardId"])).ToList();
                                    if(list.Count > 0)
                                        itemCountCell.Value = list[0].ItemCount.ToString();
                                }
                            }
                        }

                        standardCell1.Controls.Add(itemCountCell);

                        standardCell2.Attributes["class"] = "contentElementData";
                        standardCell2.InnerHtml = itemCount.ToString();
                    }
                    else
                    {
                        standardCell1.Attributes["class"] = "contentElementInactive";
                        standardCell1.InnerHtml = "&nbsp;&nbsp;";

                        standardCell2.Attributes["class"] = "contentElementInactive";
                        standardCell2.InnerHtml = "&nbsp;&nbsp;";
                    }

                    rowDetail.Cells.Add(standardCell1);
                    rowDetail.Cells.Add(standardCell2);
                    standardsTable.Rows.Add(rowDetail);
                }
            }
        }

        protected void generateButton_Click(object sender, EventArgs e)
        {
            string rigorSelection = hdnAddendumSelection.Value;
            System.Web.Script.Serialization.JavaScriptSerializer jSerializer =
               new System.Web.Script.Serialization.JavaScriptSerializer();
            StandardAddendumList assessmentVars = jSerializer.Deserialize<StandardAddendumList>(rigorSelection);         

            #region CODE BLOCK-Update session values            
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            foreach (StandardAddendum item in assessmentVars.AddendumLevels)
            {
             
                SessionObject.Standards_Addendumevels_ItemCounts.ClearStandardItemTotal(item.StandardID);                

                SessionObject.Standards_Addendumevels_ItemCounts.RemoveLevel(item.StandardID, item.AddendumID);
                if (item.ItemCount > 0)
                {
                    SessionObject.Standards_Addendumevels_ItemCounts.AddLevel(item.StandardID, item.AddendumID, item.ItemCount);
                }
            }

            //Store total item count for standard
            SessionObject.Standards_Addendumevels_ItemCounts.AddStandardItemTotal(assessmentVars.StandardCounts);           
            

            AssessmentWCF obj = new AssessmentWCF();
            string newAssessmentID = obj.RequestNewAssessmentID(new AssessmentWCFVariables());
            Thinkgate.Base.Classes.Grade gradeOrdinal = new Thinkgate.Base.Classes.Grade(grade);

            string isSecure = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["isSecure"]))
            {
                isSecure = Convert.ToString(Request.QueryString["isSecure"]);
            }
            string title = "Term ";
            //if (isSecure == "true")
            //{
            //    title = "[SECURE]Term ";
            //}
            newAssessmentTitle.Value = title + term + " " + type + " - " + gradeOrdinal.GetFriendlyName() + " Grade " + subject + " " + courseName;

            ScriptManager.RegisterStartupScript(this, typeof(string), "createAssessment", "goToNewAssessment('" + newAssessmentID + "');", true);
            #endregion
        }

        protected void summaryButton_Click(object sender, EventArgs e)
        {
            string addendumSelection = hdnAddendumSelection.Value;

            System.Web.Script.Serialization.JavaScriptSerializer jSerializer =
               new System.Web.Script.Serialization.JavaScriptSerializer();
            StandardAddendumList assessmentVars = jSerializer.Deserialize<StandardAddendumList>(addendumSelection);

            Session["AddendumSummary"] = assessmentVars;

            ScriptManager.RegisterStartupScript(this, typeof(string), "assessmentSummary", "goToAssessmentSummary();", true);
        }
    }
    [Serializable()]
    public class  StandardAddendumList
    {
        public IEnumerable<StandardAddendum> AddendumLevels { get; set; }
        public int TotalItemCount { get; set; }
        public IEnumerable<StandardCountList> StandardCounts { get; set; }
        public IEnumerable<AddendumCount> AddendumCounts { get; set; }
        public IEnumerable<SelectAllState> SelectAllStates { get; set; }
    }
    [Serializable()]
    public class StandardAddendum
    {
        public int AddendumID { get; set; }
        public int StandardID { get; set; }
        public int ItemCount { get; set; }
    }

    [Serializable()]
    public class StandardCountList
    {
        public int StandardID { get; set; }
        public string StandardName { get; set; }
        public int Count { get; set; }       
    }

    [Serializable()]
    public class AddendumCount
    {
        public int AddendumID { get; set; }
        public string AddendumName { get; set; }
        public int Count { get; set; }
    }

    [Serializable()]
    public class SelectAllState
    {
        public string SelectAllId { get; set; }
        public bool State { get; set; }
    }

}