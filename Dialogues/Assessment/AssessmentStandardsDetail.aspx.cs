using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Services;
using Telerik.Web.UI;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentStandardsDetail : System.Web.UI.Page
    {
        protected SessionObject SessionObject;
        protected int teacherID;
        protected int classID;
        protected string subject;
        protected int courseID;
        protected string courseName;
        protected string grade;
        protected string type;
        protected int term;
        protected string description;
        protected int currUserID;
        protected int standardID_URLParm;
        protected string standardName_URLParm;
        protected int itemCountInput_URLParm;
        protected string testCategory;
        protected StandardRigorLevels rigorLevels;
        protected dtItemBank selectedItemBanksTable;
        protected dtItemBank itemBankListTable;
        protected dtItemBank itemBankListTableByLabel;
        protected string standardSet = "";
        protected int blueprintID;
        protected static List<AssessmentWCFVariables> lstAssessmentWCFVariables;

        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(this.type);
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

            testCategory = SessionObject.AssessmentBuildParms.ContainsKey("TestCategory") ? SessionObject.AssessmentBuildParms["TestCategory"] : "Classroom";
            rigorLevels = SessionObject.Standards_RigorLevels_ItemCounts;

            //Selected Item Banks table
            selectedItemBanksTable = new dtItemBank();       
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadItemBankTables();

            if (!IsPostBack)
            {
                assessmentID.Value = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_TestID.ToString());
                lstAssessmentWCFVariables = new List<AssessmentWCFVariables>();
                assessmentItemCount.Value = rigorLevels.StandardItemTotals.Select(x => Convert.ToInt32(x.Value[1])).ToArray().Sum().ToString();
                if (!string.IsNullOrEmpty(Request.QueryString["page"]))
                {
                    if (Request.QueryString["page"].ToString() == "summary")
                    {
                        lstAssessmentWCFVariables = (List<AssessmentWCFVariables>)Session["ItemRigorSummary"];
                        assessmentItemCount.Value = lstAssessmentWCFVariables.Select(x => Convert.ToInt32(x.TotalItemCount)).ToArray().Sum().ToString();
                        Session.Remove("ItemRigorSummary");
                    }
                }
                LoadStandardsList();
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

                assessmentTitle.InnerHtml = "Term " + term.ToString() + " " + type + " - " + grade + " Grade " + subject + " " + (courseName == subject ? "" : courseName) + " - " + description;

                //itemsSpecified value is the input amount entered by the user or the total available amount if input amount is greater. 
                //This is done to avoid blank item counts which do not apply to this screen.
                //itemsSpecified.Value = itemCountInput_URLParm > itemTotalAvail_URLParm ? itemTotalAvail_URLParm.ToString() : itemCountInput_URLParm.ToString();
                //*****************************************************************************

                //assessmentItemCount.Value
                
            }
        }

        protected void LoadItemBankTables()
        {
            bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
            Dictionary<string, bool> dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(testCategory);
            bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            if (hasPermission && isSecuredFlag && SecureType)
                    testCategory = type;

            hiddenAccessSecureTesting.Value = hasPermission.ToString();
            hiddenIsSecuredFlag.Value = isSecuredFlag.ToString();
            hiddenSecureType.Value = SecureType.ToString();

            itemBankListTable = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, testCategory);
            itemBankListTableByLabel = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, testCategory).DistinctByLabel();
            if (SessionObject.ItemBanks != null && SessionObject.ItemBanks.Count > 0)
            {
                foreach (string item in SessionObject.ItemBanks)
                {
                    DataRow newRow = selectedItemBanksTable.NewRow();
                    DataRow itemBankRow = itemBankListTable.Select("Label = '" + item.ToString() + "'")[0];

                    selectedItemBanksTable.Add(DataIntegrity.ConvertToInt(itemBankRow["TargetType"]),
                                                itemBankRow["Target"].ToString(),
                                                DataIntegrity.ConvertToInt(itemBankRow["ApprovalSource"]),
                                                itemBankRow["Label"].ToString());
                }
            }
            else
            {
                if (SessionObject.ItemBanks != null) SessionObject.ItemBanks.Clear();

                foreach (DataRow row in itemBankListTableByLabel.Rows)
                {
                    DataRow itemBankRow = itemBankListTable.Select("Label = '" + row["Label"] + "'")[0];

                    selectedItemBanksTable.Add(DataIntegrity.ConvertToInt(itemBankRow["TargetType"]),
                                                itemBankRow["Target"].ToString(),
                                                DataIntegrity.ConvertToInt(itemBankRow["ApprovalSource"]),
                                                itemBankRow["Label"].ToString());

                    if (SessionObject.ItemBanks != null) SessionObject.ItemBanks.Add(row["Label"].ToString());
                }
            }
        }

        protected void LoadStandardsList()
        {
            List<KeyValuePair<int, string>> standards = new List<KeyValuePair<int, string>>();
            standards = rigorLevels.StandardItemNames.ToList();
            List<int> standardIds = standards.Select(st => st.Key).ToList();
            DataTable standardsStateNbrs;
            standardsStateNbrs = Standards.GetStandardsStateNbrByID(standardIds);
            standardsList.DataSource = standardsStateNbrs;
            standardsList.DataBind();
            standardsList.SelectedIndex = 0;
            if (standardsStateNbrs.Rows.Count > 0)
            {
                SetAllValues(Convert.ToInt32(standardsStateNbrs.Rows[0]["StandardID"]), standardsStateNbrs.Rows[0]["StateNbr"].ToString());
                LoadStandardsTable();
            }
        }

        protected void SetAllValues(int standardId, string standardStateNbr)
        {
            standardID_URLParm = standardId;
            lblStandardStateNbr.InnerHtml = " (" + standardStateNbr + ")";
            ArrayList standards = rigorLevels.StandardItemTotals.Where(s => s.Key == standardID_URLParm).ToList()[0].Value;
            standardID.Value = standardID_URLParm.ToString();
            standardSet = standards[0].ToString();
            hiddenstandardSet.Value = standardSet;
            itemCountInput_URLParm = Convert.ToInt32(standards[1]);
            itemsSpecified.Value = itemCountInput_URLParm.ToString();
            standardName.InnerHtml = rigorLevels.StandardItemNames.Where(s => s.Key == standardID_URLParm).ToList()[0].Value.ToString();
            RadListBoxItem listItem = (RadListBoxItem)standardsList.SelectedItem;
            standardName.Attributes["title"] = ((Label)listItem.FindControl("lblStandardStateNbr")).ToolTip;
        }

        protected void LoadStandardsTable()
        {
            DataTable standardsDetailTable;
            //PBI:2937
            if (standardSet == "Blueprint")
                standardsDetailTable = Standards.GetStandardsRigorDistributionByBlueprint(standardID_URLParm, blueprintID, testCategory, type, term, selectedItemBanksTable);
            else
                standardsDetailTable = Standards.GetStandardRigorDistribution(standardID_URLParm, testCategory, type, selectedItemBanksTable, currUserID);


            if (standardsDetailTable.Rows.Count > 0)
            {
                dokType.InnerHtml = standardsDetailTable.Rows[0]["Rigor"].ToString();
                Dictionary<string, RigorLevelCounts> rigorLevelCounts = new Dictionary<string, RigorLevelCounts>();
                Dictionary<string, RigorLevelCounts> rigorLevelCountsDistribution = new Dictionary<string, RigorLevelCounts>();

                foreach (DataRow dr in standardsDetailTable.Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell1 = new HtmlTableCell();
                    HtmlTableCell cell2 = new HtmlTableCell();
                    HtmlTableCell cell3 = new HtmlTableCell();
                    HtmlTableCell cell4 = new HtmlTableCell();
                    HtmlTableCell cell5 = new HtmlTableCell();
                    HtmlTableCell cell6 = new HtmlTableCell();
                    HtmlTableCell cell7 = new HtmlTableCell();
                    HtmlTableCell cell8 = new HtmlTableCell();
                    HtmlTableCell cell9 = new HtmlTableCell();
                    HtmlTableCell cell10 = new HtmlTableCell();
                    HtmlTableCell cell11 = new HtmlTableCell();
                    HtmlTableCell cell12 = new HtmlTableCell();
                    HtmlTableCell cell13 = new HtmlTableCell();
                    HtmlTableCell cell14 = new HtmlTableCell();
                    HtmlTableCell cell15 = new HtmlTableCell();

                    int itemCount = 0;

                    cell1.Attributes["class"] = "contentLabel";
                    cell1.InnerHtml = dr["Text"].ToString() == "N/A" ? "Not Specified" : dr["Text"].ToString();

                    itemCount = DataIntegrity.ConvertToInt(dr["MultipleChoice3"].ToString());
                    if (itemCount > 0)
                    {
                        HtmlInputText cell2Data = new HtmlInputText();
                        cell2Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_multiplechoice3_input";
                        cell2Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell2Data.Size = 5;
                        cell2Data.Attributes["itemCount"] = dr["MultipleChoice3"].ToString();
                        cell2Data.Attributes["rigorLevel"] = dr["Text"].ToString();
                        cell2Data.Attributes["class"] = "empty";
                        cell2Data.Attributes["onpaste"] = "return false";
                        cell2Data.Attributes["onchange"] = "if(this.value=='') this.className='empty';";
                        cell2Data.Attributes["onkeypress"] = "return onlyNumsLessOrEqualToCount('" + cell2Data.ID + "', this, event);";
                        cell2Data.Attributes["onkeyup"] = "getTotalItemCount('" + itemCountInput_URLParm.ToString() + "');";
                        cell2.Attributes["class"] = "contentElement";
                        cell2.Controls.Add(cell2Data);
                    }
                    else
                    {
                        cell2.Attributes["class"] = "contentElementInactive";
                        cell2.InnerHtml = "&nbsp;&nbsp;";
                    }

                    if (itemCount > 0)
                    {
                        HyperLink cell3Data = new HyperLink();
                        cell3Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_multiplechoice3_link";
                        cell3Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell3Data.NavigateUrl = "javascript: void()";
                        cell3Data.Attributes["onclick"] = "return false;";
                        cell3Data.Text = dr["MultipleChoice3"].ToString();
                        cell3.Attributes["class"] = "contentElement";
                        cell3.Controls.Add(cell3Data);
                    }
                    else
                    {
                        cell3.InnerHtml = "0";
                        cell3.Attributes["class"] = "contentElementInactive";
                    }

                    itemCount = DataIntegrity.ConvertToInt(dr["MultipleChoice4"].ToString());
                    if (itemCount > 0)
                    {
                        HtmlInputText cell12Data = new HtmlInputText();
                        cell12Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_multiplechoice4_input";
                        cell12Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell12Data.Size = 5;
                        cell12Data.Attributes["itemCount"] = dr["MultipleChoice4"].ToString();
                        cell12Data.Attributes["rigorLevel"] = dr["Text"].ToString();
                        cell12Data.Attributes["class"] = "empty";
                        cell12Data.Attributes["onpaste"] = "return false";
                        cell12Data.Attributes["onchange"] = "if(this.value=='') this.className='empty';";
                        cell12Data.Attributes["onkeypress"] = "return onlyNumsLessOrEqualToCount('" + cell12Data.ID + "', this, event);";
                        cell12Data.Attributes["onkeyup"] = "getTotalItemCount('" + itemCountInput_URLParm.ToString() + "');";
                        cell12.Attributes["class"] = "contentElement";
                        cell12.Controls.Add(cell12Data);
                    }
                    else
                    {
                        cell12.Attributes["class"] = "contentElementInactive";
                        cell12.InnerHtml = "&nbsp;&nbsp;";
                    }

                    if (itemCount > 0)
                    {
                        HyperLink cell13Data = new HyperLink();
                        cell13Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_multiplechoice4_link";
                        cell13Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell13Data.NavigateUrl = "javascript: void()";
                        cell13Data.Attributes["onclick"] = "return false;";
                        cell13Data.Text = dr["MultipleChoice4"].ToString();
                        cell13.Attributes["class"] = "contentElement";
                        cell13.Controls.Add(cell13Data);
                    }
                    else
                    {
                        cell13.InnerHtml = "0";
                        cell13.Attributes["class"] = "contentElementInactive";
                    }

                    itemCount = DataIntegrity.ConvertToInt(dr["MultipleChoice5"].ToString());
                    if (itemCount > 0)
                    {
                        HtmlInputText cell14Data = new HtmlInputText();
                        cell14Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_multiplechoice5_input";
                        cell14Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell14Data.Size = 5;
                        cell14Data.Attributes["itemCount"] = dr["MultipleChoice5"].ToString();
                        cell14Data.Attributes["rigorLevel"] = dr["Text"].ToString();
                        cell14Data.Attributes["class"] = "empty";
                        cell14Data.Attributes["onpaste"] = "return false";
                        cell14Data.Attributes["onchange"] = "if(this.value=='') this.className='empty';";
                        cell14Data.Attributes["onkeypress"] = "return onlyNumsLessOrEqualToCount('" + cell14Data.ID + "', this, event);";
                        cell14Data.Attributes["onkeyup"] = "getTotalItemCount('" + itemCountInput_URLParm.ToString() + "');";
                        cell14.Attributes["class"] = "contentElement";
                        cell14.Controls.Add(cell14Data);
                    }
                    else
                    {
                        cell14.Attributes["class"] = "contentElementInactive";
                        cell14.InnerHtml = "&nbsp;&nbsp;";
                    }

                    if (itemCount > 0)
                    {
                        HyperLink cell15Data = new HyperLink();
                        cell15Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_multiplechoice5_link";
                        cell15Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell15Data.NavigateUrl = "javascript: void()";
                        cell15Data.Attributes["onclick"] = "return false;";
                        cell15Data.Text = dr["MultipleChoice5"].ToString();
                        cell15.Attributes["class"] = "contentElement";
                        cell15.Controls.Add(cell15Data);
                    }
                    else
                    {
                        cell15.InnerHtml = "0";
                        cell15.Attributes["class"] = "contentElementInactive";
                    }

                    itemCount = DataIntegrity.ConvertToInt(dr["ShortAnswer"].ToString());
                    if (itemCount > 0)
                    {
                        HtmlInputText cell4Data = new HtmlInputText();
                        cell4Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_shortanswer_input";
                        cell4Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell4Data.Size = 5;
                        cell4Data.Attributes["itemCount"] = dr["ShortAnswer"].ToString();
                        cell4Data.Attributes["rigorLevel"] = dr["Text"].ToString();
                        cell4Data.Attributes["class"] = "empty";
                        cell4Data.Attributes["onpaste"] = "return false";
                        cell4Data.Attributes["onkeypress"] = "return onlyNumsLessOrEqualToCount('" + cell4Data.ID + "', this, event);";
                        cell4Data.Attributes["onkeyup"] = "getTotalItemCount('" + itemCountInput_URLParm.ToString() + "');";
                        cell4.Attributes["class"] = "contentElement";
                        cell4.Controls.Add(cell4Data);
                    }
                    else
                    {
                        cell4.Attributes["class"] = "contentElementInactive";
                        cell4.InnerHtml = "&nbsp;&nbsp;";
                    }

                    if (itemCount > 0)
                    {
                        HyperLink cell5Data = new HyperLink();
                        cell5Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_shortanswer_link";
                        cell5Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell5Data.NavigateUrl = "javascript: void()";
                        cell5Data.Attributes["onclick"] = "return false;";
                        cell5Data.Text = dr["ShortAnswer"].ToString();
                        cell5.Attributes["class"] = "contentElement";
                        cell5.Controls.Add(cell5Data);
                    }
                    else
                    {
                        cell5.InnerHtml = "0";
                        cell5.Attributes["class"] = "contentElementInactive";
                    }

                    itemCount = DataIntegrity.ConvertToInt(dr["Essay"].ToString());
                    if (itemCount > 0)
                    {
                        HtmlInputText cell6Data = new HtmlInputText();
                        cell6Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_essay_input";
                        cell6Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell6Data.Size = 5;
                        cell6Data.Attributes["itemCount"] = dr["Essay"].ToString();
                        cell6Data.Attributes["rigorLevel"] = dr["Text"].ToString();
                        cell6Data.Attributes["class"] = "empty";
                        cell6Data.Attributes["onpaste"] = "return false";
                        cell6Data.Attributes["onkeypress"] = "return onlyNumsLessOrEqualToCount('" + cell6Data.ID + "', this, event);";
                        cell6Data.Attributes["onkeyup"] = "getTotalItemCount('" + itemCountInput_URLParm.ToString() + "');";
                        cell6.Attributes["class"] = "contentElement";
                        cell6.Controls.Add(cell6Data);
                    }
                    else
                    {
                        cell6.Attributes["class"] = "contentElementInactive";
                        cell6.InnerHtml = "&nbsp;&nbsp;";
                    }

                    if (itemCount > 0)
                    {
                        HyperLink cell7Data = new HyperLink();
                        cell7Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_essay_link";
                        cell7Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell7Data.NavigateUrl = "javascript: void()";
                        cell7Data.Attributes["onclick"] = "return false;";
                        cell7Data.Text = dr["Essay"].ToString();
                        cell7.Attributes["class"] = "contentElement";
                        cell7.Controls.Add(cell7Data);
                    }
                    else
                    {
                        cell7.InnerHtml = "0";
                        cell7.Attributes["class"] = "contentElementInactive";
                    }

                    itemCount = DataIntegrity.ConvertToInt(dr["TrueFalse"].ToString());
                    if (itemCount > 0)
                    {
                        HtmlInputText cell8Data = new HtmlInputText();
                        cell8Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_truefalse_input";
                        cell8Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell8Data.Size = 5;
                        cell8Data.Attributes["itemCount"] = dr["TrueFalse"].ToString();
                        cell8Data.Attributes["rigorLevel"] = dr["Text"].ToString();
                        cell8Data.Attributes["class"] = "empty";
                        cell8Data.Attributes["onpaste"] = "return false";
                        cell8Data.Attributes["onkeypress"] = "return onlyNumsLessOrEqualToCount('" + cell8Data.ID + "', this, event);";
                        cell8Data.Attributes["onkeyup"] = "getTotalItemCount('" + itemCountInput_URLParm.ToString() + "');";
                        cell8.Attributes["class"] = "contentElement";
                        cell8.Controls.Add(cell8Data);
                    }
                    else
                    {
                        cell8.Attributes["class"] = "contentElementInactive";
                        cell8.InnerHtml = "&nbsp;&nbsp;";
                    }

                    if (itemCount > 0)
                    {
                        HyperLink cell9Data = new HyperLink();
                        cell9Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_truefalse_link";
                        cell9Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        cell9Data.NavigateUrl = "javascript: void()";
                        cell9Data.Attributes["onclick"] = "return false;";
                        cell9Data.Text = dr["TrueFalse"].ToString();
                        cell9.Attributes["class"] = "contentElement";
                        cell9.Controls.Add(cell9Data);
                    }
                    else
                    {
                        cell9.InnerHtml = "0";
                        cell9.Attributes["class"] = "contentElementInactive";
                    }

                    // PBI: 2937

                    if (standardSet == "Blueprint")
                    {
                        BlueprintHeader.Visible = true;


                        itemCount = DataIntegrity.ConvertToInt(dr["OperationalBPCount"].ToString());

                        if (itemCount > 0)
                        {
                            HyperLink cell11Data = new HyperLink();
                            cell11Data.ID = (dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower()) + "_blueprint_link";
                            cell11Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                            cell11Data.NavigateUrl = "javascript: void()";
                            cell11Data.Attributes["onclick"] = "return false;";
                            cell11Data.Text = dr["OperationalBPCount"].ToString();
                            cell11.Attributes["class"] = "contentElement";
                            cell11.Controls.Add(cell11Data);
                        }
                    }
                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    row.Cells.Add(cell3);
                    row.Cells.Add(cell12);
                    row.Cells.Add(cell13);
                    row.Cells.Add(cell14);
                    row.Cells.Add(cell15);
                    row.Cells.Add(cell4);
                    row.Cells.Add(cell5);
                    row.Cells.Add(cell6);
                    row.Cells.Add(cell7);
                    row.Cells.Add(cell8);
                    row.Cells.Add(cell9);

                    if (standardSet == "Blueprint")
                    {
                        row.Cells.Add(cell11);
                    }

                    standardsTable.Rows.Add(row);

                    if (dr["Text"].ToString() != "N/A")
                    {
                        if (standardSet == "Blueprint")
                        {
                            rigorLevelCounts.Add(dr["Text"].ToString(),
                                                 new RigorLevelCounts(DataIntegrity.ConvertToInt(dr["MultipleChoice3"]),
                                                                      DataIntegrity.ConvertToInt(dr["MultipleChoice4"]),
                                                                      DataIntegrity.ConvertToInt(dr["MultipleChoice5"]),
                                                                      DataIntegrity.ConvertToInt(dr["ShortAnswer"]),
                                                                      DataIntegrity.ConvertToInt(dr["Essay"]),
                                                                      DataIntegrity.ConvertToInt(dr["TrueFalse"]),
                                                                      DataIntegrity.ConvertToInt(dr["OperationalBPCount"])));
                        }
                        else
                        {
                            rigorLevelCounts.Add(dr["Text"].ToString(),
                                                new RigorLevelCounts(DataIntegrity.ConvertToInt(dr["MultipleChoice3"]),
                                                                     DataIntegrity.ConvertToInt(dr["MultipleChoice4"]),
                                                                     DataIntegrity.ConvertToInt(dr["MultipleChoice5"]),
                                                                     DataIntegrity.ConvertToInt(dr["ShortAnswer"]),
                                                                     DataIntegrity.ConvertToInt(dr["Essay"]),
                                                                     DataIntegrity.ConvertToInt(dr["TrueFalse"]),
                                                                     0
                                                                     ));
                        }
                        rigorLevelCountsDistribution.Add(dr["Text"].ToString(), new RigorLevelCounts());
                    }
                }

                standardsTable.DataBind();

                if (lstAssessmentWCFVariables != null)
                {
                    List<AssessmentWCFVariables> assessmentWCFVariables = lstAssessmentWCFVariables.Where(x => x.StandardID == standardID_URLParm).ToList();
                    if (assessmentWCFVariables.Count > 0)
                    {
                        AssessmentWCFVariables assessmentWCFVariable = assessmentWCFVariables[0];
                        int i = 0;
                        foreach (DataRow dr in standardsDetailTable.Rows)
                        {
                            string rigorLevel = dr["Text"].ToString() == "N/A" ? "na" : dr["Text"].ToString().ToLower();
                            HtmlInputText input;
                            if (assessmentWCFVariable.MultipleChoice3Counts != null)
                            {
                                if (Convert.ToInt32(assessmentWCFVariable.MultipleChoice3Counts[i]) > 0)
                                {
                                    input = (HtmlInputText)standardsTable.FindControl(rigorLevel + "_multiplechoice3_input");
                                    if (input != null)
                                    {
                                        input.Value = assessmentWCFVariable.MultipleChoice3Counts[i].ToString();
                                        input.Attributes["class"] = "";
                                    }
                                }
                            }

                            if (assessmentWCFVariable.MultipleChoice4Counts != null)
                            {
                                if (Convert.ToInt32(assessmentWCFVariable.MultipleChoice4Counts[i]) > 0)
                                {
                                    input = (HtmlInputText)standardsTable.FindControl(rigorLevel + "_multiplechoice4_input");
                                    if (input != null)
                                    {
                                        input.Value = assessmentWCFVariable.MultipleChoice4Counts[i].ToString();
                                        input.Attributes["class"] = "";
                                    }
                                }
                            }

                            if (assessmentWCFVariable.MultipleChoice5Counts != null)
                            {
                                if (Convert.ToInt32(assessmentWCFVariable.MultipleChoice5Counts[i]) > 0)
                                {
                                    input = (HtmlInputText)standardsTable.FindControl(rigorLevel + "_multiplechoice5_input");
                                    if (input != null)
                                    {
                                        input.Value = assessmentWCFVariable.MultipleChoice5Counts[i].ToString();
                                        input.Attributes["class"] = "";
                                    }
                                }
                            }

                            if (assessmentWCFVariable.ShortAnswerCounts != null)
                            {
                                if (Convert.ToInt32(assessmentWCFVariable.ShortAnswerCounts[i]) > 0)
                                {
                                    input = (HtmlInputText)standardsTable.FindControl(rigorLevel + "_shortanswer_input");
                                    if (input != null)
                                    {
                                        input.Value = assessmentWCFVariable.ShortAnswerCounts[i].ToString();
                                        input.Attributes["class"] = "";
                                    }
                                }
                            }

                            if (assessmentWCFVariable.EssayCounts != null)
                            {
                                if (Convert.ToInt32(assessmentWCFVariable.EssayCounts[i]) > 0)
                                {
                                    input = (HtmlInputText)standardsTable.FindControl(rigorLevel + "_essay_input");
                                    if (input != null)
                                    {
                                        input.Value = assessmentWCFVariable.EssayCounts[i].ToString();
                                        input.Attributes["class"] = "";
                                    }
                                }
                            }

                            if (assessmentWCFVariable.TrueFalseCounts != null)
                            {
                                if (Convert.ToInt32(assessmentWCFVariable.TrueFalseCounts[i]) > 0)
                                {
                                    input = (HtmlInputText)standardsTable.FindControl(rigorLevel + "_truefalse_input");
                                    if (input != null)
                                    {
                                        input.Value = assessmentWCFVariable.TrueFalseCounts[i].ToString();
                                        input.Attributes["class"] = "";
                                    }
                                }
                            }

                            if (assessmentWCFVariable.BlueprintCounts != null)
                            {
                                if (Convert.ToInt32(assessmentWCFVariable.BlueprintCounts[i]) > 0)
                                {
                                    input = (HtmlInputText)standardsTable.FindControl(rigorLevel + "_blueprint_input");
                                    if (input != null)
                                    {
                                        input.Value = assessmentWCFVariable.BlueprintCounts[i].ToString();
                                        input.Attributes["class"] = "";
                                    }
                                }
                            }
                            i++;
                        }

                        itemsSpecified.Value = assessmentWCFVariable.TotalItemCount.ToString();
                    }
                }
            }
        }

        private int GetRigorLevelCount(RigorLevelCounts rigorLevelCounts, string questionType)
        {
            switch (questionType)
            {
                case "MultipleChoice3":
                    return rigorLevelCounts.MultipleChoice3Count;
                case "MultipleChoice4":
                    return rigorLevelCounts.MultipleChoice4Count;
                case "MultipleChoice5":
                    return rigorLevelCounts.MultipleChoice5Count;
                case "ShortAnswer":
                    return rigorLevelCounts.ShortAnswerCount;
                case "Essay":
                    return rigorLevelCounts.EssayCount;
                case "TrueFalse":
                    return rigorLevelCounts.TrueFalseCount;
                case "OperationalBPCount":

                    return rigorLevelCounts.BlueprintCount;
                default:
                    return 0;
            }
        }

        private RigorLevelCounts GetRigorLevelCounts(RigorLevelCounts rigorLevelCounts, int itemCount, string questionType)
        {
            switch (questionType)
            {
                case "MultipleChoice3":
                    return new RigorLevelCounts(itemCount, rigorLevelCounts.MultipleChoice4Count, rigorLevelCounts.MultipleChoice5Count, rigorLevelCounts.ShortAnswerCount, rigorLevelCounts.EssayCount, rigorLevelCounts.TrueFalseCount, rigorLevelCounts.BlueprintCount);
                case "MultipleChoice4":
                    return new RigorLevelCounts(rigorLevelCounts.MultipleChoice3Count, itemCount, rigorLevelCounts.MultipleChoice5Count, rigorLevelCounts.ShortAnswerCount, rigorLevelCounts.EssayCount, rigorLevelCounts.TrueFalseCount, rigorLevelCounts.BlueprintCount);
                case "MultipleChoice5":
                    return new RigorLevelCounts(rigorLevelCounts.MultipleChoice3Count, rigorLevelCounts.MultipleChoice4Count, itemCount, rigorLevelCounts.ShortAnswerCount, rigorLevelCounts.EssayCount, rigorLevelCounts.TrueFalseCount, rigorLevelCounts.BlueprintCount);
                case "ShortAnswer":
                    return new RigorLevelCounts(rigorLevelCounts.MultipleChoice3Count, rigorLevelCounts.MultipleChoice4Count, rigorLevelCounts.MultipleChoice5Count, itemCount, rigorLevelCounts.EssayCount, rigorLevelCounts.TrueFalseCount, rigorLevelCounts.BlueprintCount);
                case "Essay":
                    return new RigorLevelCounts(rigorLevelCounts.MultipleChoice3Count, rigorLevelCounts.MultipleChoice4Count, rigorLevelCounts.MultipleChoice5Count, rigorLevelCounts.ShortAnswerCount, itemCount, rigorLevelCounts.TrueFalseCount, rigorLevelCounts.BlueprintCount);
                case "TrueFalse":
                    return new RigorLevelCounts(rigorLevelCounts.MultipleChoice3Count, rigorLevelCounts.MultipleChoice4Count, rigorLevelCounts.MultipleChoice5Count, rigorLevelCounts.ShortAnswerCount, rigorLevelCounts.EssayCount, itemCount, rigorLevelCounts.BlueprintCount);
                case "OperationalBPCount":
                    return new RigorLevelCounts(rigorLevelCounts.MultipleChoice3Count, rigorLevelCounts.MultipleChoice4Count, rigorLevelCounts.MultipleChoice5Count, rigorLevelCounts.ShortAnswerCount, rigorLevelCounts.EssayCount, rigorLevelCounts.TrueFalseCount, itemCount);
                default:
                    return rigorLevelCounts;
            }
        }

        protected void standardsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rigorSelection = hdnRigorSelection.Value;
            System.Web.Script.Serialization.JavaScriptSerializer jSerializer =
                new System.Web.Script.Serialization.JavaScriptSerializer();
            AssessmentWCFVariables assessmentWCFVariable = jSerializer.Deserialize<AssessmentWCFVariables>(rigorSelection);
            if (lstAssessmentWCFVariables != null)
                lstAssessmentWCFVariables = lstAssessmentWCFVariables.Where(x => x.StandardID != assessmentWCFVariable.StandardID).ToList();
            lstAssessmentWCFVariables.Add(assessmentWCFVariable);
            SetAllValues(Convert.ToInt32(standardsList.SelectedItem.Value), standardsList.SelectedItem.Text.ToString());
            LoadStandardsTable();
        }

        protected void updateButton_Click(object sender, EventArgs e)
        {
            string rigorSelection = hdnRigorSelection.Value;
            System.Web.Script.Serialization.JavaScriptSerializer jSerializer =
               new System.Web.Script.Serialization.JavaScriptSerializer();
            AssessmentWCFVariables assessmentWCFVariable = jSerializer.Deserialize<AssessmentWCFVariables>(rigorSelection);
            if (lstAssessmentWCFVariables != null)
            {
                lstAssessmentWCFVariables =
                    lstAssessmentWCFVariables.Where(
                        x => x.StandardID != assessmentWCFVariable.StandardID).ToList();
                lstAssessmentWCFVariables.Add(assessmentWCFVariable);
            }

            #region CODE BLOCK-Update session values
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            foreach (AssessmentWCFVariables assessmentVars in lstAssessmentWCFVariables)
            {

                int itemCount = 0;

                //Set the StandardItemTotals value to 0 for the selected standard
                SessionObject.Standards_RigorLevels_ItemCounts.ClearStandardItemTotal(assessmentVars.StandardID);

                //Loop through all rigor levels and add item counts to the Standards_RigorLevels_ItemCounts dictionary
                if (assessmentVars.RigorLevels != null)
                {
                    for (int i = 0; i < assessmentVars.RigorLevels.Count; i++)
                    {
                        string rigorLevelName = assessmentVars.RigorLevels[i].ToString();
                        int multipleChoice3Count = DataIntegrity.ConvertToInt(assessmentVars.MultipleChoice3Counts[i]);
                        int multipleChoice4Count = DataIntegrity.ConvertToInt(assessmentVars.MultipleChoice4Counts[i]);
                        int multipleChoice5Count = DataIntegrity.ConvertToInt(assessmentVars.MultipleChoice5Counts[i]);
                        int shortAnswerCount = DataIntegrity.ConvertToInt(assessmentVars.ShortAnswerCounts[i]);
                        int essayCount = DataIntegrity.ConvertToInt(assessmentVars.EssayCounts[i]);
                        int trueFalseCount = DataIntegrity.ConvertToInt(assessmentVars.TrueFalseCounts[i]);
                        int blueprintCount = DataIntegrity.ConvertToInt(assessmentVars.BlueprintCounts[i]);

                        SessionObject.Standards_RigorLevels_ItemCounts.RemoveLevel(assessmentVars.StandardID, rigorLevelName);

                        if (multipleChoice3Count > 0 || multipleChoice4Count > 0 || multipleChoice5Count > 0 || shortAnswerCount > 0 || essayCount > 0 || trueFalseCount > 0 || blueprintCount > 0)
                        {
                            itemCount += multipleChoice3Count + multipleChoice4Count + multipleChoice5Count + shortAnswerCount + essayCount + trueFalseCount + blueprintCount;
                            SessionObject.Standards_RigorLevels_ItemCounts.AddLevel(assessmentVars.StandardID, rigorLevelName, multipleChoice3Count, multipleChoice4Count, multipleChoice5Count, shortAnswerCount, essayCount, trueFalseCount, blueprintCount);
                        }
                    }
                }

                //Store total item count for standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemTotal(assessmentVars.StandardID, assessmentVars.StandardSet, assessmentVars.TotalItemCount);
            }

            #endregion

            AssessmentWCF obj = new AssessmentWCF();
            string newAssessmentID = obj.RequestNewAssessmentID(new AssessmentWCFVariables());
            Thinkgate.Base.Classes.Grade gradeOrdinal = new Thinkgate.Base.Classes.Grade(grade);
            newAssessmentTitle.Value = "Term " + term + " " + type + " - " + gradeOrdinal.GetFriendlyName() + " Grade " + subject + " " + courseName;

            ScriptManager.RegisterStartupScript(this, typeof(string), "createAssessment", "goToNewAssessment('" + newAssessmentID + "');", true);
        }

        protected void summaryButton_Click(object sender, EventArgs e)
        {
            string rigorSelection = hdnRigorSelection.Value;
            System.Web.Script.Serialization.JavaScriptSerializer jSerializer =
               new System.Web.Script.Serialization.JavaScriptSerializer();
            AssessmentWCFVariables assessmentWCFVariable = jSerializer.Deserialize<AssessmentWCFVariables>(rigorSelection);
            if (lstAssessmentWCFVariables != null)
                lstAssessmentWCFVariables = lstAssessmentWCFVariables.Where(x => x.StandardID != assessmentWCFVariable.StandardID).ToList();
            lstAssessmentWCFVariables.Add(assessmentWCFVariable);
            
            var standards = rigorLevels.StandardItemTotals.Select(x => x.Key).ToArray();
            foreach (var std in rigorLevels.StandardItemTotals)
            {
                if (lstAssessmentWCFVariables.Where(x => x.StandardID == std.Key).ToList().Count == 0)
                {
                    AssessmentWCFVariables assessmentWCF = new AssessmentWCFVariables();
                    assessmentWCF.StandardID = std.Key;
                    assessmentWCF.TotalItemCount = int.Parse(std.Value[1].ToString());
                    assessmentWCF.StandardName = rigorLevels.StandardItemNames.Where(x => x.Key == std.Key).Select(x => x.Value).FirstOrDefault();
                    assessmentWCF.StandardSet = std.Value[0].ToString();
                    lstAssessmentWCFVariables.Add(assessmentWCF);
                }
            }
            Session["ItemRigorSummary"] = lstAssessmentWCFVariables;

            ScriptManager.RegisterStartupScript(this, typeof(string), "assessmentSummary", "goToAssessmentSummary();", true);
        }
        
    }
}