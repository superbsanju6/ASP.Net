using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using System.Collections.Generic;
using System.Linq;
using Thinkgate.Base.Enums;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentStandards : System.Web.UI.Page
    {
        protected SessionObject SessionObject;
        protected int teacherID;
        protected int classID;
        protected Class classObj;
        protected string classSubject;
        protected string classCourse;
        protected string classGrade;
        protected string classTerm;
        protected int currUserID;
        protected string testCategory;
        protected string type;
        protected string defaultStandardSet;
        protected StandardRigorLevels rigorLevels;
        protected dtItemBank selectedItemBanksTable;
        protected dtItemBank itemBankListTable;
        protected dtItemBank itemBankListTableByLabel;
        protected string contentType;

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
            SessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];
            currUserID = SessionObject.LoggedInUser != null ? (SessionObject.LoggedInUser.Page > 0 ? SessionObject.LoggedInUser.Page : AppSettings.Demo_TeacherID) : AppSettings.Demo_TeacherID;
            testCategory = SessionObject.AssessmentBuildParms.ContainsKey("TestCategory") ? SessionObject.AssessmentBuildParms["TestCategory"] : "Classroom";
            type = SessionObject.AssessmentBuildParms.ContainsKey("Type") ? SessionObject.AssessmentBuildParms["Type"] : "";
            rigorLevels = SessionObject.Standards_RigorLevels_ItemCounts;
            contentType = SessionObject.AssessmentBuildParms.ContainsKey("Content")
                              ? SessionObject.AssessmentBuildParms["Content"]
                              : "";


            classGrade = SessionObject.AssessmentBuildParms.ContainsKey("Grade") ? SessionObject.AssessmentBuildParms["Grade"] : "";
            classSubject = SessionObject.AssessmentBuildParms.ContainsKey("Subject") ? SessionObject.AssessmentBuildParms["Subject"] : "";
            classTerm = SessionObject.AssessmentBuildParms.ContainsKey("Term") ? SessionObject.AssessmentBuildParms["Term"] : "";
            assessmentSelection.Value = SessionObject.AssessmentBuildParms.ContainsKey("AssessmentSelection") ? SessionObject.AssessmentBuildParms["AssessmentSelection"] : "";
            //Selected Item Banks table
            selectedItemBanksTable = new dtItemBank();

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (contentType != "External")
            {
                LoadItemBankTables();
            }
            else
            {
                ftItemBankContaincer.Visible = false;
            }

            if (!IsPostBack)
            {
                defaultStandardSet = Request.QueryString["prevstandardSet"];
                assessmentID.Value = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_TestID.ToString());

                switch (Request.QueryString["headerImg"])
                {
                    case "lightningbolt":
                        headerImg.Src = "../../Images/lightningbolt.png";
                        headerImg.Attributes["headerImgName"] = "lightningbolt";
                        nextButton.Attributes["style"] = "display:none;";
                        generateButton.Attributes["style"] = "display:block;";
                        break;
                    case "repairtool":
                        headerImg.Src = "../../Images/repairtool.png";
                        headerImg.Attributes["headerImgName"] = "repairtool";
                        nextButton.Attributes["style"] = "display:block;";
                        generateButton.Attributes["style"] = "display:none;";
                        break;
                    default:
                        headerImg.Visible = false;
                        break;
                }

                LoadStandardsButton();
                if (contentType == "External")
                {
                    itemBankContainer.Attributes["style"] = "display:none;";
                    generateButton.Text = "  Create External Assessment  ";
                    generateButton.Attributes["style"] = "";
                    nextButton.Attributes["style"] = "display:none;";
                }
                else
                {
                    LoadItemBankButton();
                }

                assessmentContentType.Value = contentType;

                LoadStandardsTable();
                SetNewAssessmentTitleValue();
            }

            if (!String.IsNullOrEmpty(defaultStandardSet))
                standardsSetDropdown.SelectedValue = defaultStandardSet;

        }

        protected void LoadItemBankTables()
        {

            bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
            Dictionary<string, bool> dictionaryItem = TestTypes.TypeWithSecureFlag(testCategory);
            bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();

            hiddenAccessSecureTesting.Value = hasPermission.ToString();
            hiddenIsSecuredFlag.Value = isSecuredFlag.ToString();
            hiddenSecureType.Value = SecureType.ToString();

            if (hasPermission && isSecuredFlag && SecureType)
                testCategory = type;
            
            itemBankListTable = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, testCategory);
            itemBankListTableByLabel = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, testCategory).DistinctByLabel();

            if (!IsPostBack)
            {
                if (SessionObject.ItemBanks != null && SessionObject.ItemBanks.Count > 0)
                {
                    foreach(string item in SessionObject.ItemBanks)
                    {
                        if (itemBankListTable.Select("Label = '" + item.ToString() + "'").Length == 0) continue;

                        DataRow itemBankRow = itemBankListTable.Select("Label = '" + item.ToString() + "'")[0];

                        selectedItemBanksTable.Add(DataIntegrity.ConvertToInt(itemBankRow["TargetType"]),
                                                   itemBankRow["Target"].ToString(),
                                                   DataIntegrity.ConvertToInt(itemBankRow["ApprovalSource"]),
                                                   itemBankRow["Label"].ToString(),
                                                   itemBankRow["isFieldTestBank"].ToString());
                    }
                }
                else
                {
                    if(SessionObject.ItemBanks != null) SessionObject.ItemBanks.Clear();

                    foreach (DataRow row in itemBankListTableByLabel.Rows)
                    {
                        if (itemBankListTable.Select("Label = '" + row["Label"] + "'").Length == 0) continue;

                        DataRow itemBankRow = itemBankListTable.Select("Label = '" + row["Label"] + "'")[0];

                        selectedItemBanksTable.Add(DataIntegrity.ConvertToInt(itemBankRow["TargetType"]),
                                                   itemBankRow["Target"].ToString(),
                                                   DataIntegrity.ConvertToInt(itemBankRow["ApprovalSource"]),
                                                   itemBankRow["Label"].ToString(),
                                                   itemBankRow["isFieldTestBank"].ToString());

                        if(SessionObject.ItemBanks != null) SessionObject.ItemBanks.Add(row["Label"].ToString());
                    }
                }
            }
        }

        protected void LoadStandardsButton()
        {
            //var standardSets = CourseMasterList.StandardSets;
            // Fixed for #8390 
            // In the standardsets table there is a column called standardsearch, if it is set to "no", the standard set should not show in any drop downs.
            var standardSets = Thinkgate.Base.Classes.Standards.GetStandardSets(currUserID);
            standardsSetDropdown.Items.Clear();

            // TFS 1528/3698
            if ((testCategory == "District" || testCategory == "State") && SessionObject.LoggedInUser.HasPermission(Base.Enums.Permission.Tile_Blueprint))
            {
                int courseID = SessionObject.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]) : 0;
                Base.Classes.Course assessmentCourse = CourseMasterList.GetCurrCourseById(courseID);
                classCourse = assessmentCourse != null ? assessmentCourse.CourseName : "";

                var dp = DistrictParms.LoadDistrictParms();

                DataTable blueprintAvail = Standards.CheckBlueprintAvailable(classGrade, classSubject, classCourse, type, classTerm);
                if (blueprintAvail.Rows.Count > 0)
                {
                    if (blueprintAvail.Rows[0]["Exists"].ToString() == "True")
                    {
                        RadComboBoxItem item = new RadComboBoxItem();
                        item.Text = "Blueprint";
                        item.Value = "Blueprint";
                        standardsSetDropdown.Items.Add(item);

                    }

                }

            }

            foreach (DataRow sSet in standardSets.Rows)
            {
                RadComboBoxItem item = new RadComboBoxItem();
                item.Text = sSet["Standard_Set"].ToString();
                item.Value = sSet["Standard_Set"].ToString();


                //Don't default to blue print as it builds session state first
                if (SessionObject.AssessmentBuildParms.ContainsKey("StandardSet") && SessionObject.AssessmentBuildParms["StandardSet"] == sSet["Standard_Set"].ToString())
                {
                    item.Selected = true;
                }

                standardsSetDropdown.Items.Add(item);
            }

            if (standardsSetDropdown.Items.Count > 0 && !SessionObject.AssessmentBuildParms.ContainsKey("StandardSet"))
            {
                if (String.IsNullOrEmpty(defaultStandardSet))
                    standardsSetDropdown.Items[0].Selected = true;
                else
                    standardsSetDropdown.SelectedValue = defaultStandardSet;
            }
        }

        protected void LoadItemBankButton()
        {
            itemBankDropdown.Items.Clear();
            ftitemBankDropdown.Items.Clear();
            int itemBankDropdownCheckedItemsTotal = 0;
            int ftitemBankDropdownCheckedItemsTotal = 0;

            // Added check to see FieldTest is turned on for the test type or not
            DataTable fielTestAvail = Standards.CheckFieldTestAvailable(type);
            if (fielTestAvail.Rows.Count > 0)
            {
                if (fielTestAvail.Rows[0]["Exists"].ToString() == "False")
                {
                    ftItemBankContaincer.Visible = false;
                }
            }
            RadComboBoxItem itemBankListItemAll = new RadComboBoxItem();
            itemBankListItemAll.Text = "All";
            itemBankListItemAll.Value = "All";
            itemBankDropdown.Items.Add(itemBankListItemAll);

            CheckBox allItemCheckbox = (CheckBox)itemBankListItemAll.FindControl("itemBankCheckBox");
            Label allItemLabel = (Label)itemBankListItemAll.FindControl("itemBankLabel");
            if (SessionObject.ItemBanks != null && (SessionObject.ItemBanks.Count == 0 || SessionObject.ItemBanks.Count == itemBankListTableByLabel.Rows.Count))
            {
                if (allItemCheckbox != null)
                {
                    allItemCheckbox.Checked = true;
                    itemBankDropdown.Text = "All";
                }
            }
            if (allItemLabel != null)
            {
                allItemLabel.Text = "All";
            }

            // For field test items
            RadComboBoxItem ftitemBankListItemAll = new RadComboBoxItem();
            ftitemBankListItemAll.Text = "All";
            ftitemBankListItemAll.Value = "All";
            ftitemBankDropdown.Items.Add(ftitemBankListItemAll);

            CheckBox allftItemCheckbox = (CheckBox)ftitemBankListItemAll.FindControl("ftitemBankCheckBox");
            Label allftItemLabel = (Label)ftitemBankListItemAll.FindControl("ftitemBankLabel");
            if (SessionObject.ItemBanks != null && (SessionObject.ItemBanks.Count == 0 || SessionObject.ItemBanks.Count == itemBankListTableByLabel.Rows.Count))
            {
                if (allftItemCheckbox != null)
                {
                    allftItemCheckbox.Checked = true;
                    ftitemBankDropdown.Text = "All";
                }
            }
            if (allftItemLabel != null)
            {
                allftItemLabel.Text = "All";
            }

            //Build itemBank ListBox
            foreach (DataRow dr in itemBankListTableByLabel.Rows)
            {

                if (itemBankListTable.Select("Label = '" + dr["Label"] + "'").Length == 0) continue;

                if (itemBankListTable.Select("Label = '" + dr["Label"] + "'")[0]["isFieldTestBank"].ToString() == "False")
                {

                    RadComboBoxItem itemBankListItem = new RadComboBoxItem();
                    Label itemBankLabel = (Label)itemBankListItem.FindControl("itemBankLabel");
                    itemBankListItem.Text = dr["Label"].ToString();
                    itemBankListItem.Value = dr["Label"].ToString();
                    DataRow itemBankRow = itemBankListTable.Select("Label = '" + dr["Label"] + "'")[0];

                    itemBankListItem.Attributes["targetType"] = itemBankRow["TargetType"].ToString();
                    itemBankListItem.Attributes["target"] = itemBankRow["Target"].ToString();
                    itemBankListItem.Attributes["approvalSource"] = itemBankRow["ApprovalSource"].ToString();
                    itemBankListItem.Attributes["itemBanklabel"] = itemBankRow["Label"].ToString();
                    itemBankListItem.Attributes["isFieldTestBank"] = itemBankRow["isFieldTestBank"].ToString();

                    //if (itemBankRow["isFieldTestBank"].ToString() == "False")
                    //{
                    itemBankDropdown.Items.Add(itemBankListItem);

                    CheckBox itemCheckbox = (CheckBox)itemBankListItem.FindControl("itemBankCheckbox");
                    Label itemLabel = (Label)itemBankListItem.FindControl("itemBankLabel");
                    int itemExistsInSession = selectedItemBanksTable.Select("Label = '" + dr["Label"].ToString() + "'").Length;

                    if (itemCheckbox != null)
                    {
                        if (SessionObject.ItemBanks != null && (SessionObject.ItemBanks.Count == 0 || SessionObject.ItemBanks.Count == itemBankListTableByLabel.Rows.Count))
                        {
                            itemCheckbox.Checked = true;
                            itemBankDropdownCheckedItemsTotal++;
                        }
                        else if (itemExistsInSession > 0)
                        {
                            itemCheckbox.Checked = true;
                            itemBankDropdownCheckedItemsTotal++;
                            itemBankDropdown.Text = dr["Label"].ToString();
                        }
                    }
                    if (itemLabel != null)
                    {
                        itemLabel.Text = dr["Label"].ToString();
                    }
                    //  }
                }
                // for field test item banks
                else
                {
                    RadComboBoxItem itemBankListItem = new RadComboBoxItem();
                    Label itemBankLabel = (Label)itemBankListItem.FindControl("ftitemBankLabel");
                    itemBankListItem.Text = dr["Label"].ToString();
                    itemBankListItem.Value = dr["Label"].ToString();
                    DataRow itemBankRow = itemBankListTable.Select("Label = '" + dr["Label"] + "'")[0];

                    itemBankListItem.Attributes["targetType"] = itemBankRow["TargetType"].ToString();
                    itemBankListItem.Attributes["target"] = itemBankRow["Target"].ToString();
                    itemBankListItem.Attributes["approvalSource"] = itemBankRow["ApprovalSource"].ToString();
                    itemBankListItem.Attributes["ftitemBanklabel"] = itemBankRow["Label"].ToString();
                    itemBankListItem.Attributes["isFieldTestBank"] = itemBankRow["isFieldTestBank"].ToString();

                    ftitemBankDropdown.Items.Add(itemBankListItem);

                    CheckBox ftitemCheckbox = (CheckBox)itemBankListItem.FindControl("ftitemBankCheckbox");
                    Label ftitemLabel = (Label)itemBankListItem.FindControl("ftitemBankLabel");
                    int ftitemExistsInSession = selectedItemBanksTable.Select("Label = '" + dr["Label"].ToString() + "'").Length;

                    if (ftitemCheckbox != null)
                    {
                        if (SessionObject.ItemBanks != null && (SessionObject.ItemBanks.Count == 0 || SessionObject.ItemBanks.Count == itemBankListTableByLabel.Rows.Count))
                        {
                            ftitemCheckbox.Checked = true;
                            ftitemBankDropdownCheckedItemsTotal++;
                        }
                        else if (ftitemExistsInSession > 0)
                        {
                            ftitemCheckbox.Checked = true;
                            ftitemBankDropdownCheckedItemsTotal++;
                            ftitemBankDropdown.Text = dr["Label"].ToString();
                        }
                    }
                    if (ftitemLabel != null)
                    {
                        ftitemLabel.Text = dr["Label"].ToString();
                    }

                }
            }

            if (itemBankDropdownCheckedItemsTotal == itemBankListTableByLabel.Rows.Count)
            {
                itemBankDropdown.Items[0].Checked = true;
                itemBankDropdown.Items[0].Selected = true;
            }
            else if (itemBankDropdownCheckedItemsTotal > 1)
            {
                itemBankDropdown.Text = "Multiple";
            }
            // for Field test item banks
            if (ftitemBankDropdownCheckedItemsTotal == itemBankListTableByLabel.Rows.Count)
            {
                ftitemBankDropdown.Items[0].Checked = true;
                ftitemBankDropdown.Items[0].Selected = true;
            }
            else if (ftitemBankDropdownCheckedItemsTotal > 1)
            {
                ftitemBankDropdown.Text = "Multiple";
            }

            RadComboBoxItem findButton = new RadComboBoxItem();
            itemBankDropdown.Items.Add(findButton);

            CheckBox findButtonCheckbox = (CheckBox)findButton.FindControl("itemBankCheckbox");
            if (findButtonCheckbox != null)
            {
                findButtonCheckbox.InputAttributes["style"] = "display:none;";
            }

            // for Field test item banks
            RadComboBoxItem findFTButton = new RadComboBoxItem();
            ftitemBankDropdown.Items.Add(findFTButton);

            CheckBox findFTButtonCheckbox = (CheckBox)findFTButton.FindControl("ftitemBankCheckbox");
            if (findFTButtonCheckbox != null)
            {
                findFTButtonCheckbox.InputAttributes["style"] = "display:none;";
            }
        }

        protected void LoadStandardsTable()
        {
            string grade = SessionObject.AssessmentBuildParms["Grade"];
            string subject = SessionObject.AssessmentBuildParms["Subject"];
            int courseID = DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]);
            int term = DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Term"]);
            string testYear = SessionObject.AssessmentBuildParms["Year"];

            string standardSet = String.IsNullOrEmpty(defaultStandardSet)? standardsSetDropdown.SelectedValue: defaultStandardSet;
            hiddenStandard.Value = standardSet;
            int totalItemCount = 0;
            int ftTotalItemCount = 0;
            int totalSelectedCount = 0;
            int ftTotalSelectedCount = 0;

            DistrictParms dParms = DistrictParms.LoadDistrictParms();
            List<int> usedQuestions = new List<int>();
            if (testCategory == "District" || testCategory == "State")
            {
                usedQuestions = Thinkgate.Base.Classes.Assessment.GetUsedQuestions(courseID, testYear, !dParms.AutoGenTestReuseQuestions, !dParms.AutoGenTestReuseQuestionsUnproofed);
            }

            // Added check to see FieldTest is turned on for the test type or not
            DataTable fielTestAvail = Standards.CheckFieldTestAvailable(type);

            DataTable standardsResults;
            //PBI 1528
            if (standardSet == "Blueprint")
            {
                if (SessionObject.AssessmentBuildParms.ContainsKey("StandardSet"))
                    SessionObject.AssessmentBuildParms["StandardSet"] = "Blueprint";
                else SessionObject.AssessmentBuildParms.Add("StandardSet","Blueprint");

                //Get Blueprint data
                standardsResults = Thinkgate.Base.Classes.Standards.GetStandardsByBlueprint(courseID, testCategory, type, term, usedQuestions, selectedItemBanksTable, testYear);

                if (standardsResults != null)
                {
                    if (standardsResults.Rows.Count > 0)
                    {
                        if (SessionObject.AssessmentBuildParms.ContainsKey("BlueprintID"))
                            SessionObject.AssessmentBuildParms["BlueprintID"] = standardsResults.Rows[0]["BlueprintID"].ToString();
                        else
                        {
                            //Save Detailed Blueprint to Session State
                            SessionObject.AssessmentBuildParms.Add("BlueprintID", standardsResults.Rows[0]["BlueprintID"].ToString());
                            SaveInitialBluePrintToSessionState(standardsResults);
                        }
                    }
                }
            }
            else
            {
                if (CourseMasterList.GetCurrCourseById(courseID) == null)
                {
                    standardsResults = Thinkgate.Base.Classes.Standards.GetStandardsByGradeSubjectCourse(grade, subject, "", standardSet, testCategory, type, usedQuestions, selectedItemBanksTable);
                }
                else
                {
                    standardsResults = Thinkgate.Base.Classes.Standards.GetStandardsByCurriculum(standardSet, testCategory, type, courseID, usedQuestions, selectedItemBanksTable);
                }
            }
            standardsTable.Rows.Clear();
            bool hyperlinkStandardNamePermission = SessionObject.LoggedInUser.HasPermission(Base.Enums.Permission.Hyperlink_StandardName);
            foreach (DataRow dr in standardsResults.Rows)
            {
                TableRow row = new TableRow();
                TableCell cell1 = new TableCell();
                TableCell cell2 = new TableCell();
                TableCell cell3 = new TableCell();
                TableCell cell4 = new TableCell();
                TableCell cell5 = new TableCell();
                TableCell cell6 = new TableCell();
                TableCell cell7 = new TableCell();
                TableCell cell8 = new TableCell(); //Hidden Standardid
                TableCell cell9 = new TableCell(); //Hidden Rigor

                //Column 1: Select Standards Checkbox
                HtmlInputCheckBox cell1Data = new HtmlInputCheckBox();
                cell1Data.ID = "standardsCheckbox_" + dr["StandardID"].ToString();
                cell1Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                cell1Data.Attributes["onclick"] = "selectStandard('" + dr["StandardID"].ToString() + "', this.checked);";
                cell1.Width = 100;
                cell1.CssClass = "alignCellCenter";
                cell1.Controls.Add(cell1Data);


                // PBI: 1528
                //if (standardSet == "Blueprint")
                //   cell1Data.Checked = true;

                //Column 2:Operational Items
                HtmlInputText cell2Data = new HtmlInputText();
                cell2Data.ID = "itemsEnteredInput_" + dr["StandardID"].ToString();
                cell2Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                cell2Data.Attributes["class"] = "itemsCountInput";
                cell2Data.Attributes["onkeypress"] = "return onlyNumsLessOrEqualToCount('" + dr["StandardID"].ToString() + "', this, event);";
                cell2Data.Attributes["onkeyup"] = "deleteAndBackspace_onKeyup('" + dr["StandardID"].ToString() + "', this, event);";

                // PBI: 1528
                if (standardSet != "Blueprint")
                    cell2Data.Attributes["itemBank"] = dr["ItemBank"].ToString();

                // PBI: 1528
                if (standardSet == "Blueprint")
                {
                    cell2Data.Value = dr["BPOperationalCount"].ToString();
                    //cell2Data.Attributes["disabled"] = "disabled";
                }

                if (rigorLevels.StandardItemTotals.ContainsKey(DataIntegrity.ConvertToInt(dr["StandardID"])) )
                {
                    if (rigorLevels.StandardItemTotals[DataIntegrity.ConvertToInt(dr["StandardID"])][0].ToString() == standardSet)
                    {
                        cell2Data.Value = rigorLevels.StandardItemTotals[DataIntegrity.ConvertToInt(dr["StandardID"])][1].ToString();
                        if (cell2Data.Value == "0") cell2Data.Value = "";
                        else cell1Data.Checked = true;
                        totalSelectedCount += DataIntegrity.ConvertToInt(DataIntegrity.ConvertToInt(rigorLevels.StandardItemTotals[DataIntegrity.ConvertToInt(dr["StandardID"])][1]));
                    }

                }
                //if (DataIntegrity.ConvertToInt(dr["ItemCount"]) == 0) cell2Data.Disabled = true;
                cell2Data.Size = 5;
                cell2.Width = 100;
                cell2.CssClass = "alignCellCenter";
                cell2.Controls.Add(cell2Data);

                //Column 3:Available Items
                HtmlGenericControl cell3Data = new HtmlGenericControl("DIV");
                cell3Data.ID = "standardsCountLabel_" + dr["StandardID"].ToString();
                cell3Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                cell3Data.InnerText = dr["OperationalItemCount"].ToString();
                cell3Data.Style.Add("margin", "2px");

                cell3.Width = 100;
                cell3.CssClass = "alignCellCenter";
                cell3.Controls.Add(cell3Data);

                //Column 4:Field test items
                HtmlInputText cell4Data = new HtmlInputText();
                cell4Data.ID = "ftitemsEnteredInput_" + dr["StandardID"].ToString();
                cell4Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                cell4Data.Attributes["class"] = "ftitemsCountInput";
                cell4Data.Attributes["onkeypress"] = "return FTonlyNumsLessOrEqualToCount('" + dr["StandardID"].ToString() + "', this, event);";
                cell4Data.Attributes["onkeyup"] = "deleteAndBackspace_onKeyup('" + dr["StandardID"].ToString() + "', this, event);";

                if (standardSet == "Blueprint")
                {
                    cell4Data.Value = dr["BPFieldTestCount"].ToString();

                    if ((dr["BPFieldTestCount"].ToString() != string.Empty && Convert.ToInt32(dr["BPFieldTestCount"].ToString()) > 0) || (dr["BPOperationalCount"].ToString() != string.Empty && Convert.ToInt32(dr["BPOperationalCount"].ToString()) > 0))
                        cell1Data.Checked = true;
                }

                if (rigorLevels.StandardItemTotals.ContainsKey(DataIntegrity.ConvertToInt(dr["StandardID"])))
                {
                    if (rigorLevels.StandardItemTotals[DataIntegrity.ConvertToInt(dr["StandardID"])][0].ToString() == standardSet)
                    {
                        cell4Data.Value = rigorLevels.StandardItemTotals[DataIntegrity.ConvertToInt(dr["StandardID"])][1].ToString();
                        if (cell4Data.Value == "0") cell4Data.Value = "";
                        else cell1Data.Checked = true;
                        ftTotalSelectedCount += DataIntegrity.ConvertToInt(DataIntegrity.ConvertToInt(rigorLevels.StandardItemTotals[DataIntegrity.ConvertToInt(dr["StandardID"])][1]));
                    }
                }
                //if (DataIntegrity.ConvertToInt(dr["ItemCount"]) == 0) cell2Data.Disabled = true;
                cell4Data.Size = 5;
                cell4.Width = 100;
                cell4.CssClass = "alignCellCenter";
                cell4.Controls.Add(cell4Data);

                //Column 5:Available Items
                RadButton cell5Data = new RadButton();
                cell5Data.ID = "ftstandardsCountButton_" + dr["StandardID"].ToString();
                cell5Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                cell5Data.Text = dr["FieldTestItemCount"].ToString();
                cell5Data.Width = 50;
                cell5Data.CssClass = "radDropdownBtn";
                cell5Data.AutoPostBack = false;

                if (DataIntegrity.ConvertToInt(dr["FieldTestItemCount"]) == 0)
                {
                    cell5Data.Enabled = false;
                    cell5Data.Skin = "Office2010Black";
                }
                else
                {

                    cell5Data.Skin = "Web20";
                    if (standardSet != "Blueprint")
                    {
                        cell5Data.Attributes["onclick"] = "loadStandardCounts(null, null, {obj: this, standardID: '" + dr["StandardID"] + "', standardName: '" + dr["StandardName"] +
                            "', itemCountInput: $('#" + cell2Data.ID + "').attr('value'), headerImg: '" + Request.QueryString["headerImg"] + "'}); return false;";
                    }
                    else
                    {
                        cell5Data.Attributes["onclick"] = "loadStandardCounts(null, null, {obj: this, standardID: '" + dr["StandardID"] + "', standardName: '" + dr["StandardName"] +
                          "', itemCountInput: $('#" + cell2Data.ID + "').attr('value'), headerImg: '" + Request.QueryString["headerImg"] + "', standardSet: '" + "Blueprint" + "', blueprintID: '" + dr["BlueprintID"] + "'}); return false;";
                    }
                    cell5Data.Attributes["xmlHttpPanelID"] = "submitStandardCountsXmlHttpPanel";

                }
                cell5.Width = 100;
                cell5.CssClass = "alignCellCenter";
                cell5.Controls.Add(cell5Data);

                //Column 6:Standard Name
                HyperLink cell6Data = new HyperLink();
                cell6Data.ID = "itemName_" + dr["StandardID"].ToString();
                cell6Data.ClientIDMode = ClientIDMode.Static;
                cell6Data.Text = dr["StandardName"].ToString();
                cell6Data.NavigateUrl = "~/Record/StandardsPage.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(dr["StandardID"].ToString());
                cell6Data.Attributes["onclick"] = "window.open('" + cell6Data.ResolveClientUrl(cell6Data.NavigateUrl) + "'); return false;";
                cell6.Width = 220;
                cell6.CssClass = "alignCellLeft noBorderRight";
                if (hyperlinkStandardNamePermission)
                {
                    cell6.Controls.Add(cell6Data);
                }
                else
                {
                    cell6.Text = dr["StandardName"].ToString();
                }

                //Column 7:Standard Text
                HtmlGenericControl cell7Data = new HtmlGenericControl("DIV");
                string standardText = dr["StandardText"].ToString();
                string standardText_ellipsed = standardText.Length > 35 ? standardText.Substring(0, 35) + ". . ." : standardText;

                ImageButton closeButton = new ImageButton();
                closeButton.ImageUrl = "~/Images/close_StandardText.png";
                string closeButtonImgURL = closeButton.ResolveClientUrl(closeButton.ImageUrl);

                cell7Data.InnerHtml = "<a href='javascript:void()' class='standardTextLink' onclick='displayFullDescription(this); return false;'>" + standardText_ellipsed +
                    "</a><div class='fullText'><img src='" + closeButtonImgURL + "' onclick='this.parentNode.style.display=&quot;none&quot;;' style='position:relative; float:right; cursor:pointer;' />" + standardText + "</div>";
                cell7.CssClass = "alignCellLeft noBorderLeft";
                cell7.Controls.Add(cell7Data);

                //Column 8: Hidden Standardid
                HiddenField cell8Data = new HiddenField();
                cell8Data.ID = "stid_" + dr["StandardID"].ToString();
                cell8Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                cell8Data.Value = dr["StandardID"].ToString();
                cell8.Controls.Add(cell8Data);

                //Column 9: Hidden Blueprint Rigor if available
                HiddenField cell9Data = new HiddenField();
                cell9Data.ID = "rigor_" + dr["StandardID"].ToString();
                cell9Data.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                cell9Data.Value = "0,0,0,0,"; //Needed as default value for JS client
                cell9.Controls.Add(cell9Data);


                if (standardSet == "Blueprint")
                {
                    cell9Data.Value = String.IsNullOrEmpty(dr["OperationalDOKCount"].ToString()) ? "0,0,0,0,0," : dr["OperationalDOKCount"].ToString();
                }
                /*********************************************************************************
                *                               Add cells to row                                 *
                *********************************************************************************/
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                row.Cells.Add(cell3);

                // Added check to see FieldTest is turned on for the test type or not
                if (fielTestAvail.Rows.Count > 0)
                {
                    if (dParms.FieldTest_Enabled == "Yes" && fielTestAvail.Rows[0]["Exists"].ToString() == "True")
                    {
                        row.Cells.Add(cell4);
                        row.Cells.Add(cell5);
                    }
                    else
                    {
                        thFieldTestItem.Visible = false;
                        thFieldTestAvail.Visible = false;
                    }
                }

                row.Cells.Add(cell6);
                row.Cells.Add(cell7);
                row.Cells.Add(cell8);
                row.Cells.Add(cell9);

                totalItemCount += DataIntegrity.ConvertToInt(dr["OperationalItemCount"]);

                ftTotalItemCount += DataIntegrity.ConvertToInt(dr["FieldTestItemCount"]);
                standardsTable.Rows.Add(row);
            }

            totalSelected.Value = totalSelectedCount.ToString();
            totalCount.Value = totalItemCount.ToString();

            // Added check to see FieldTest is turned on for the test type or not
            if (fielTestAvail.Rows.Count > 0)
            {
                if (dParms.FieldTest_Enabled == "Yes" && fielTestAvail.Rows[0]["Exists"].ToString() == "True")
                {
                    ftTotalSelected.Value = ftTotalSelectedCount.ToString();
                    ftTotalCount.Value = ftTotalItemCount.ToString();
                }
                else
                {
                    ftTotalSelected.Visible = false;
                    ftTotalCount.Visible = false;
                }
            }


            ftTotalSelected.Value = ftTotalSelectedCount.ToString();
            ftTotalCount.Value = ftTotalItemCount.ToString();

            standardsTable.DataBind();
        }

        protected void LoadStandardsTable_Click(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {

            if (contentType != "External")
            {
                selectedItemBanksTable.Rows.Clear();
                if (SessionObject.ItemBanks != null) SessionObject.ItemBanks.Clear();

                //Reset StandardSet
                SessionObject.AssessmentBuildParms["StandardSet"] = e.Value;

                foreach (RadComboBoxItem item in itemBankDropdown.Items)
                {
                    CheckBox itemCheckbox = (CheckBox) item.FindControl("itemBankCheckbox");
                    Label itemLabel = (Label) item.FindControl("itemBankLabel");

                    if (itemCheckbox != null && itemCheckbox.Checked && itemLabel.Text != "All" &&
                        itemLabel.Text.IndexOf("<img") == -1)
                    {
                        DataRow itemBankRow = itemBankListTable.Select("Label = '" + itemLabel.Text + "'")[0];

                        selectedItemBanksTable.Add(DataIntegrity.ConvertToInt(itemBankRow["TargetType"]),
                                                   itemBankRow["Target"].ToString(),
                                                   DataIntegrity.ConvertToInt(itemBankRow["ApprovalSource"]),
                                                   itemBankRow["Label"].ToString(),
                                                   itemBankRow["isFieldTestBank"].ToString());

                        if (SessionObject.ItemBanks != null) SessionObject.ItemBanks.Add(itemLabel.Text);
                    }
                }

                LoadStandardsTable();
                LoadItemBankButton();
            }
            else
            {
                LoadStandardsTable();
                ftItemBankContaincer.Visible = false;
            }
        }

        protected void LoadStandardsTable_Selected(object sender, EventArgs e)
        {
            if (contentType != "External")
            {
                selectedItemBanksTable.Rows.Clear();
                if (SessionObject.ItemBanks != null) SessionObject.ItemBanks.Clear();

                foreach (RadComboBoxItem item in itemBankDropdown.Items)
                {
                    CheckBox itemCheckbox = (CheckBox) item.FindControl("itemBankCheckbox");
                    Label itemLabel = (Label) item.FindControl("itemBankLabel");

                    if (itemCheckbox != null && itemCheckbox.Checked && itemLabel.Text != "All" &&
                        itemLabel.Text.IndexOf("<img") == -1)
                    {
                        DataRow itemBankRow = itemBankListTable.Select("Label = '" + itemLabel.Text + "'")[0];

                        selectedItemBanksTable.Add(DataIntegrity.ConvertToInt(itemBankRow["TargetType"]),
                                                   itemBankRow["Target"].ToString(),
                                                   DataIntegrity.ConvertToInt(itemBankRow["ApprovalSource"]),
                                                   itemBankRow["Label"].ToString(),
                                                    itemBankRow["isFieldTestBank"].ToString());

                        if (SessionObject.ItemBanks != null) SessionObject.ItemBanks.Add(itemLabel.Text);
                    }
                }

                // For field test items
                foreach (RadComboBoxItem item in ftitemBankDropdown.Items)
                {
                    CheckBox ftitemCheckbox = (CheckBox)item.FindControl("ftitemBankCheckbox");
                    Label ftitemLabel = (Label)item.FindControl("ftitemBankLabel");

                    if (ftitemCheckbox != null && ftitemCheckbox.Checked && ftitemLabel.Text != "All" &&
                        ftitemLabel.Text.IndexOf("<img") == -1)
                    {
                        DataRow ftitemBankRow = itemBankListTable.Select("Label = '" + ftitemLabel.Text + "'")[0];

                        selectedItemBanksTable.Add(DataIntegrity.ConvertToInt(ftitemBankRow["TargetType"]),
                                                   ftitemBankRow["Target"].ToString(),
                                                   DataIntegrity.ConvertToInt(ftitemBankRow["ApprovalSource"]),
                                                   ftitemBankRow["Label"].ToString(),
                                                   ftitemBankRow["isFieldTestBank"].ToString());

                        if (SessionObject.ItemBanks != null) SessionObject.ItemBanks.Add(ftitemLabel.Text);
                    }
                }

                LoadStandardsTable();
                LoadItemBankButton();
            }
            else
            {
                LoadStandardsTable();
                ftItemBankContaincer.Visible = false;
            }
        }

        protected void SetNewAssessmentTitleValue()
        {
            string grade = SessionObject.AssessmentBuildParms["Grade"];
            Grade gradeOrdinal = new Grade(grade);
            string subject = SessionObject.AssessmentBuildParms["Subject"];
            int courseID = DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]);
            Base.Classes.Course assessmentCourse = CourseMasterList.GetCurrCourseById(courseID);
            string courseName = assessmentCourse != null ? assessmentCourse.CourseName : "";
            string type = SessionObject.AssessmentBuildParms["Type"];
            string term = SessionObject.AssessmentBuildParms["Term"];

            courseName = courseName == subject ? "" : courseName;
            newAssessmentTitle.Value = "Term " + term + " " + type + " - " + gradeOrdinal.GetFriendlyName() + " Grade " + subject + " " + courseName;
        }

        //Saves initial blue print data in Session State for Assessment WCF Service
        private void SaveInitialBluePrintToSessionState(DataTable standardsResults)
        {
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            //Get Rigor Names from DB
            List<string> rigorNames = (from row in Base.Classes.Rigor.ListAsDataTable().AsEnumerable()
                                       select row.Field<string>("Text")).ToList();

            //Loop through all Standardids in Blueprint
            for (int x = 0; x < standardsResults.Rows.Count; x++)
            {
                int standardid = (Int32)standardsResults.Rows[x]["StandardID"];

                int standardItemCount = 0;
                int totalItemCount = 0;

                //Blueprint Rigor count is passed into WCF
                String rigorString = (String)standardsResults.Rows[x]["OperationalDOkCount"];
                String[] rigorArray = rigorString.Split(',');


                //Loop through all rigor levels and add item counts to the Standards_RigorLevels_ItemCounts dictionary
                for (int i = 0; i < rigorNames.Count-1; i++)
                {
                    string rigorLevelName = rigorNames[i].ToString();
                    int multipleChoice3Count = 0;
                    int multipleChoice4Count = 0;
                    int multipleChoice5Count = 0;
                    int shortAnswerCount = 0;
                    int essayCount = 0;
                    int trueFalseCount = 0;
                    int blueprintCount = DataIntegrity.ConvertToInt(rigorArray[i]);
                    totalItemCount += blueprintCount;

                    SessionObject.Standards_RigorLevels_ItemCounts.RemoveLevel(standardid, rigorLevelName);

                    if (multipleChoice3Count > 0 || multipleChoice4Count > 0 || multipleChoice5Count > 0 || shortAnswerCount > 0 || essayCount > 0 || trueFalseCount > 0 || blueprintCount > 0)
                    {
                        standardItemCount += multipleChoice3Count + multipleChoice4Count + multipleChoice5Count + shortAnswerCount + essayCount + trueFalseCount + blueprintCount;
                        SessionObject.Standards_RigorLevels_ItemCounts.AddLevel(standardid, rigorLevelName, multipleChoice3Count, multipleChoice4Count, multipleChoice5Count, shortAnswerCount, essayCount, trueFalseCount, blueprintCount);
                    }
                }

                //If assessmentVars.TotalItemCount is greater than the rigor level itemCount, then add blank rows for the difference
                if (totalItemCount > standardItemCount)
                {
                    int itemCountDifference = totalItemCount - standardItemCount;
                    SessionObject.Standards_RigorLevels_ItemCounts.AddLevel(standardid, "", itemCountDifference);
                }

                //Store total item count for standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemTotal(standardid, "Blueprint", totalItemCount);

            } //End Loop

        }

    }
}