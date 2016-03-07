using System;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Utilities;
using WebSupergoo.ABCpdf9;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Linq;
using Thinkgate.Services;
using System.Web;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentSummary : System.Web.UI.Page
    {
        protected SessionObject SessionObject;
        protected int classID;
        protected string subject;
        protected int courseID;
        protected string courseName;
        protected string grade;
        protected string type;
        protected int term;
        protected string content;
        protected string includeFieldTest;
        protected string description;
        protected int currUserID;
        protected int standardID_URLParm;
        protected string standardName_URLParm;
        protected int itemCountInput_URLParm;
        protected string testCategory;
        protected static StandardRigorLevels rigorLevels;
        protected int _assessmentID;
        protected String cacheKey;
        protected Base.Classes.Assessment _assessment;
        private DataSet _ds;
        private bool isPDFView;
        protected static StandardAddendumLevels addendumLevels;

        #region Check Secure
        protected Dictionary<string, bool> dictionaryItem;
        bool isSecuredFlag;
        private Int32 AssessmentID = 0;
        public Thinkgate.Base.Classes.Assessment selectedAssessment;
        bool SecureTitle = false;
        string AssessmentType = string.Empty;
        public bool SecureType
        {
            get
            {
                var tt = TestTypes.GetByName(selectedAssessment.TestType);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }
        public bool AssessmentSecureType
        {
            get
            {
                var tt = TestTypes.GetByName(AssessmentType);
                if (tt != null)
                    return tt.Secure;
                return false;
            }
        }
        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];

           if (String.IsNullOrEmpty(Request.QueryString["xID"]) && String.IsNullOrEmpty(Request.QueryString["yID"]))
            {
                subject = SessionObject.AssessmentBuildParms.ContainsKey("Subject") ? SessionObject.AssessmentBuildParms["Subject"] : "";
                courseID = SessionObject.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Course"]) : 0;
                Base.Classes.Course assessmentCourse = CourseMasterList.GetCurrCourseById(courseID);
                courseName = assessmentCourse != null ? assessmentCourse.CourseName : "";
                grade = SessionObject.AssessmentBuildParms.ContainsKey("Grade") ? SessionObject.AssessmentBuildParms["Grade"] : "";
                type = SessionObject.AssessmentBuildParms.ContainsKey("Type") ? SessionObject.AssessmentBuildParms["Type"] : "";
                term = SessionObject.AssessmentBuildParms.ContainsKey("Term") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Term"]) : 0;
                content = SessionObject.AssessmentBuildParms.ContainsKey("Content") ? SessionObject.AssessmentBuildParms["Content"] : "";
                includeFieldTest = SessionObject.AssessmentBuildParms.ContainsKey("IncludeFieldTest") ? SessionObject.AssessmentBuildParms["IncludeFieldTest"] : "";
                description = SessionObject.AssessmentBuildParms.ContainsKey("Description") ? SessionObject.AssessmentBuildParms["Description"] : "";
                currUserID = SessionObject.LoggedInUser != null ? (SessionObject.LoggedInUser.Page > 0 ? SessionObject.LoggedInUser.Page : AppSettings.Demo_TeacherID) : AppSettings.Demo_TeacherID;
                testCategory = SessionObject.AssessmentBuildParms.ContainsKey("TestCategory") ? SessionObject.AssessmentBuildParms["TestCategory"] : "Classroom";
                AssessmentType = type;
                bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                Dictionary<string, bool> dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(testCategory);
                bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
                
                hiddenAccessSecureTesting.Value = hasPermission.ToString();
                hiddenIsSecuredFlag.Value = isSecuredFlag.ToString();
                hiddenSecureType.Value = AssessmentSecureType.ToString();

            }
            else
            {
                if (_assessment == null)
                    LoadAssessment();
               
                    bool hasPermission = SessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
                    if (!IsPostBack)
                    {
                        if (_assessment.AssessmentID != 0)
                        {
                            selectedAssessment = Base.Classes.Assessment.GetAssessmentByID(_assessment.AssessmentID);
                            dictionaryItem = TestTypes.TypeWithSecureFlag(selectedAssessment.TestCategory);
                            isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
                            SecureTitle = hasPermission && isSecuredFlag && SecureType;

                        }
                    

                    hiddenAccessSecureTesting.Value = hasPermission.ToString();
                    hiddenIsSecuredFlag.Value = isSecuredFlag.ToString();
                    hiddenSecureType.Value = SecureType.ToString();

                    subject = _assessment.Subject;
                    courseName = _assessment.Course;
                    grade = _assessment.Grade;
                    type = _assessment.TestType;
                    term = DataIntegrity.ConvertToInt(_assessment.Term);
                    content = _assessment.ContentType;

                    if (SessionObject.AssessmentBuildParms.ContainsKey("IncludeFieldTest"))
                    {
                        SessionObject.AssessmentBuildParms.Remove("IncludeFieldTest");
                    }

                    SessionObject.AssessmentBuildParms.Add(
                        "IncludeFieldTest",
                        _assessment.IncludeFieldTest ? "Yes" : "No");
                    includeFieldTest = _assessment.IncludeFieldTest ? "Yes" : "No";

                    description = _assessment.Description;
                    currUserID = SessionObject.LoggedInUser != null
                        ? (SessionObject.LoggedInUser.Page > 0
                            ? SessionObject.LoggedInUser.Page
                            : AppSettings.Demo_TeacherID)
                        : AppSettings.Demo_TeacherID;

                    _ds = ThinkgateDataAccess.FetchDataSet(
                        "E3_Assessment_GetByID",
                        new object[]
                        {
                            _assessmentID,
                            currUserID
                        });
                    _ds.Tables[0].TableName = "Summary";
                    _ds.Tables[1].TableName = "StandardQuestionCounts";
                    _ds.Tables[2].TableName = "RigorCounts";
                    isPDFView = Request.QueryString["printPDFView"] == "yes";
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                switch (Request.QueryString["headerImg"])
                {
                    case "lightningbolt":
                        headerImg.Src = "../../Images/lightningbolt.png";
                        headerImg.Attributes["headerImgName"] = "lightningbolt";
                        break;
                    case "magicwand":
                        headerImg.Src = "../../Images/magicwand.png";
                        headerImg.Attributes["headerImgName"] = "magicwand";
                        break;
                    case "repairtool":
                        headerImg.Src = "../../Images/repairtool.png";
                        headerImg.Attributes["headerImgName"] = "repairtool";
                        break;
                    default:
                        headerImg.Visible = false;
                        break;
                }

               
                addendumDistributionContainerDiv.Visible = false;
                lblAddendum.Visible = false;

                if (!string.IsNullOrEmpty(Request.QueryString["page"]))
                {
                    hdnCallBackPage.Value = Request.QueryString["page"].ToString();
                    if (hdnCallBackPage.Value == "rigor")
                    {
                        rigorLevels = new StandardRigorLevels();
                        List<AssessmentWCFVariables> lstAssessmentWCFVariables = (List<AssessmentWCFVariables>)Session["ItemRigorSummary"];
                        #region CODE BLOCK-Update session values
                       
                        foreach (AssessmentWCFVariables assessmentVars in lstAssessmentWCFVariables)
                        {

                            int itemCount = 0;

                            //Loop through all rigor levels and add item counts to the rigorLevels
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

                                    rigorLevels.RemoveLevel(assessmentVars.StandardID, rigorLevelName);

                                    if (multipleChoice3Count > 0 || multipleChoice4Count > 0 || multipleChoice5Count > 0 || shortAnswerCount > 0 || essayCount > 0 || trueFalseCount > 0 || blueprintCount > 0)
                                    {
                                        itemCount += multipleChoice3Count + multipleChoice4Count + multipleChoice5Count + shortAnswerCount + essayCount + trueFalseCount + blueprintCount;
                                        rigorLevels.AddLevel(assessmentVars.StandardID, rigorLevelName, multipleChoice3Count, multipleChoice4Count, multipleChoice5Count, shortAnswerCount, essayCount, trueFalseCount, blueprintCount);
                                    }
                                }
                            }

                            //Store total item count for standard
                            rigorLevels.AddStandardItemTotal(assessmentVars.StandardID, assessmentVars.StandardSet, assessmentVars.TotalItemCount);


                            rigorLevels.ClearStandardItemName(DataIntegrity.ConvertToInt(assessmentVars.StandardID));

                            //Set new name value for the selected standard
                            rigorLevels.AddStandardItemName(DataIntegrity.ConvertToInt(assessmentVars.StandardID), assessmentVars.StandardName.ToString());
                        }
                        #endregion
                    }
                    else if (hdnCallBackPage.Value == "addendum")
                    {
                        lblRigor.Visible = false;
                        rigorTable.Visible = false;
                        addendumDistributionContainerDiv.Visible = true;
                        lblAddendum.Visible = true;
                        StandardAddendumList assessmentAddendumVars = (StandardAddendumList)Session["AddendumSummary"];
                        addendumLevels = new StandardAddendumLevels();

                        foreach (StandardAddendum item in assessmentAddendumVars.AddendumLevels)
                        {
                            addendumLevels.ClearStandardItemTotal(item.StandardID);

                            addendumLevels.RemoveLevel(item.StandardID, item.AddendumID);
                            if (item.ItemCount > 0)
                            {
                                addendumLevels.AddLevel(item.StandardID, item.AddendumID, item.ItemCount);
                            }
                        }

                        //Store total item count for standard
                        addendumLevels.AddStandardItemTotal(assessmentAddendumVars.StandardCounts);

                        foreach (StandardCountList item in assessmentAddendumVars.StandardCounts)
                        {
                            addendumLevels.ClearStandardItemName(item.StandardID);
                            addendumLevels.AddStandardItemName(DataIntegrity.ConvertToInt(item.StandardID), item.StandardName.ToString());
                        }

                        foreach (AddendumCount item in assessmentAddendumVars.AddendumCounts)
                        {
                            addendumLevels.ClearAddendumCount(item.AddendumID);
                            addendumLevels.AddAddendumCount(DataIntegrity.ConvertToInt(item.AddendumID), HttpUtility.UrlDecode(item.AddendumName.ToString()), item.Count);
                        }

                        addendumItemCountCell.Text = "Item Count (" + addendumLevels.AddendumCounts.Select(x => Convert.ToInt32(x.Value[1])).ToArray().Sum().ToString() + ")";
                    }
                }

                 assessmentTitle.InnerHtml = "Term " + term.ToString() + " " + type + " - " + grade + " Grade " + subject + " " + (courseName == subject ? string.Empty : courseName) + " - " + description;
                              
                contentType.InnerHtml = content;
                includeFieldTestValue.InnerHtml = includeFieldTest;
                if (content == "External")
                {
                    generateButton.Text = "  Create External Assessment  ";
                }

                if (_assessment == null)
                {
                    if (hdnCallBackPage.Value == "addendum")
                    {
                        LoadAddendumDistributionTable();
                    }
                    else
                    {
                        LoadRigorDistributionTable();
                    }
                    LoadStandardDistributionTable();
                    printButton.Visible = false;

                    if(testCategory == "Classroom")
                    {
                        includeFieldTestLabel.Visible = false;
                        includeFieldTestValue.Visible = false;
                        numberOfAddendumsLabel_dup.Visible = true;
                        addendumCount_dup.Visible = true;
                        numberOfAddendumsLabel_orig.Visible = false;
                        addendumCount.Visible = false;
                    }
                }
                else
                {
                    generateButton.Visible = false;
                    cancelButton.Visible = false;
                    backButton.OnClientClick = "closeSummary(); return false;";
                    backButton.Text = " Close ";
                    printButton.Visible = SessionObject.LoggedInUser.HasPermission(Base.Enums.Permission.Icon_Print_AssessmentContentSummary);
                    rubricCount.InnerHtml = _assessment.RubricCount > 0 ?
                        "<a href=\"javascript:void();\" onclick=\"parent.customDialog({ name: 'RadWindowAddAssessment', url: '../ControlHost/PreviewRubrics.aspx?xID=" + Encryption.EncryptInt(_assessmentID) +
                        "', maximize: true, maxwidth: 500, maxheight: 400 }); return false;\">" + _assessment.RubricCount.ToString() + "</a>" : "0";
                    addendumCount.InnerHtml = _assessment.AddendumCount > 0 ?
                        "<a href=\"javascript:void();\" onclick=\"parent.customDialog({ name: 'RadWindowAddAssessment', url: '../ControlHost/PreviewAddendums.aspx?xID=" + Encryption.EncryptInt(_assessmentID) +
                        "', maximize: true, maxwidth: 500, maxheight:400 }); return false;\">" + _assessment.AddendumCount.ToString() + "</a>" : "0";
                    addendumCount_dup.InnerHtml = _assessment.AddendumCount > 0 ?
                        "<a href=\"javascript:void();\" onclick=\"parent.customDialog({ name: 'RadWindowAddAssessment', url: '../ControlHost/PreviewAddendums.aspx?xID=" + Encryption.EncryptInt(_assessmentID) +
                        "', maximize: true, maxwidth: 500, maxheight: 400 }); return false;\">" + _assessment.AddendumCount.ToString() + "</a>" : "0";

                    LoadStandardDistributionTable_AssessmentObjectScreen();
                    LoadItemBankDistributionTable();
                    LoadRigorDistributionTable_AssessmentObjectScreen();

                    if (_assessment.Category == AssessmentCategories.Classroom)
                    {
                        includeFieldTestLabel.Visible = false;
                        includeFieldTestValue.Visible = false;
                        numberOfAddendumsLabel_dup.Visible = true;
                        addendumCount_dup.Visible = true;
                        numberOfAddendumsLabel_orig.Visible = false;
                        addendumCount.Visible = false;
                    }
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
                {
            if (isPDFView)
            {
                    backButton.Visible = false;
                    printButton.Visible = false;
                    distributionContainerDiv.Attributes["style"] = "width: 95%;";
                    standardDistScrollContainerDiv.Attributes["class"] = string.Empty;
                    itemBankDistScrollContainerDiv.Attributes["class"] = string.Empty;
                    formContainerDiv.Attributes["class"] = string.Empty;
                    formContainerDiv.Attributes["style"] = "text-align:left;";
                    itemBankLabel.Attributes["style"] = "font-family: Sans-Serif, Arial;font-weight: normal;font-size: .8em;";
                    standardDistLabel.Attributes["style"] = "font-family: Sans-Serif, Arial;font-weight: normal;font-size: .8em;";
                    standardsTableContainerDiv.Attributes["style"] = "width:300px; border:solid 2px #000;";
                    itemBankTableContainerDiv.Attributes["style"] = "width:300px; border:solid 2px #000;";
                    rubricAndAddendumLabels.Attributes["style"] = "float:left;";
                    rubricAndAddendumCountValues.Attributes["style"] = "float: left;margin-left: 20px;margin-right: 50px;color: #888;";
                    spanBreak.Visible = true;
                    
                    Doc doc = AssessmentSummaryToPdfDoc(null);
                    byte[] theData = doc.GetData();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");
                    Response.AddHeader("content-length", theData.Length.ToString());
                    Response.BinaryWrite(theData);
                return;
                }
            base.Render(writer);
            }

        private void LoadAssessment()
        {
            if (Request.QueryString["xID"] == null ||
                    (_assessmentID = Cryptography.GetDecryptedID(SessionObject.LoggedInUser.CipherKey)) <= 0)
            {
                SessionObject.RedirectMessage = "No assessment ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _assessment = Base.Classes.Assessment.GetAssessmentAndQuestionsByID(_assessmentID);
                if (_assessment == null)
                {
                    SessionObject.RedirectMessage = "Could not find the assessment.";
                    Response.Redirect("~/PortalSelection.aspx", true);
                }
            }
        }

        protected void LoadRigorDistributionTable()
        {
            DataTable rigorDistributionTable = new DataTable();
            if (hdnCallBackPage.Value == "rigor")
            {
                rigorDistributionTable = rigorLevels.BuildRigorDistributionTable();
            }
            else
            {
                rigorDistributionTable = SessionObject.Standards_RigorLevels_ItemCounts.BuildRigorDistributionTable();
            }
            int distributionTotalCount = 0;

            if (rigorDistributionTable.Rows.Count > 0)
            {
                rigorLevelLabel.InnerHtml = Base.Classes.Rigor.Dok;

                foreach (DataRow dr in rigorDistributionTable.Rows)
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

                    //Distribution Cell
                    cell1.InnerHtml = DataIntegrity.ConvertToInt(dr["Distribution"]) == 0 ? "&nbsp;" : dr["Distribution"].ToString();
                    cell1.Attributes["class"] = "contentDistribution";

                    if (DataIntegrity.ConvertToInt(dr["Distribution"]) > 0)
                    {
                        distributionTotalCount += DataIntegrity.ConvertToInt(dr["Distribution"]);
                    }

                    //Rigor Names
                    cell2.InnerHtml = dr["Text"].ToString() == "N/A" ? "Not Specified" : dr["Text"].ToString();
                    cell2.Attributes["class"] = "contentLabel";
                    if (dr["Text"].ToString() == "Blank Items")
                    {
                        cell2.Attributes["style"] = "color:#36F";
                    }


                    //Multiple Choice 3
                    cell3.InnerHtml = DataIntegrity.ConvertToInt(dr["MultipleChoice3"]) == 0 ? "&nbsp;" : dr["MultipleChoice3"].ToString();
                    if (dr["MultipleChoice3"].ToString() == "" || dr["MultipleChoice3"].ToString() == "0")
                    {
                        cell3.Attributes["class"] = "contentElementInactive";
                        cell3.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                    }
                    else
                    {
                        cell3.Attributes["class"] = "contentElement";
                        if (isPDFView) cell3.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }

                    //Multiple Choice 4
                    cell4.InnerHtml = DataIntegrity.ConvertToInt(dr["MultipleChoice4"]) == 0 ? "&nbsp;" : dr["MultipleChoice4"].ToString();
                    if (dr["MultipleChoice4"].ToString() == "" || dr["MultipleChoice4"].ToString() == "0")
                    {
                        cell4.Attributes["class"] = "contentElementInactive";
                        //cell4.Attributes["style"] = "font-style:italic;text-align:center;background-color: #C0C0C0;font-weight: normal;";
                        cell4.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                        //cell4.ColSpan = 4;
                    }
                    else
                    {
                        cell4.Attributes["class"] = "contentElement";
                        if (isPDFView) cell4.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }

                    //Multiple Choice 5
                    cell5.InnerHtml = DataIntegrity.ConvertToInt(dr["MultipleChoice5"]) == 0 ? "&nbsp;" : dr["MultipleChoice5"].ToString();
                    if (dr["MultipleChoice5"].ToString() == "" || dr["MultipleChoice5"].ToString() == "0")
                    {
                        cell5.Attributes["class"] = "contentElementInactive";
                        cell5.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                    }
                    else
                    {
                        cell5.Attributes["class"] = "contentElement";
                        if (isPDFView) cell5.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }


                    //Special case blank items
                    if (dr["Text"].ToString() == "Blank Items")
                    {
                        cell6.InnerHtml = "n/a";
                        cell6.Attributes["class"] = "contentElementInactive";
                        cell6.Attributes["style"] = "font-style:italic;text-align:center;background-color: #C0C0C0;font-weight: normal;";
                        cell6.ColSpan = 4;
                    }
                    //Non-Blank items
                    else
                    {
                        //Short Answer
                        cell6.InnerHtml = DataIntegrity.ConvertToInt(dr["ShortAnswer"]) == 0
                                              ? "&nbsp;"
                                              : dr["ShortAnswer"].ToString();
                        cell6.Attributes["class"] = "contentElement";
                        if (isPDFView) cell4.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                        if (dr["ShortAnswer"].ToString() == "" || dr["ShortAnswer"].ToString() == "0")
                        {
                            cell6.Attributes["class"] = "contentElementInactive";
                            cell6.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                        }
                        else
                        {
                            cell6.Attributes["class"] = "contentElement";
                            if (isPDFView) cell6.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                        }

                        //Essay
                        cell7.InnerHtml = DataIntegrity.ConvertToInt(dr["Essay"]) == 0 ? "&nbsp;" : dr["Essay"].ToString();
                        cell7.Attributes["class"] = "contentElement";
                        if (isPDFView) cell7.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                        if (dr["Essay"].ToString() == "" || dr["Essay"].ToString() == "0")
                        {
                            cell7.Attributes["class"] = "contentElementInactive";
                            cell7.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                        }
                        else
                        {
                            cell7.Attributes["class"] = "contentElement";
                            if (isPDFView) cell7.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                        }

                        //True False
                        cell8.InnerHtml = DataIntegrity.ConvertToInt(dr["TrueFalse"]) == 0
                                              ? "&nbsp;"
                                              : dr["TrueFalse"].ToString();
                        cell8.Attributes["class"] = "contentElement";
                        if (isPDFView) cell8.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                        if (dr["TrueFalse"].ToString() == "" || dr["TrueFalse"].ToString() == "0")
                        {
                            cell8.Attributes["class"] = "contentElementInactive";
                            cell8.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                        }
                        else
                        {
                            cell8.Attributes["class"] = "contentElement";
                            if (isPDFView) cell6.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                        }

                        cell9.Attributes["class"] = "contentElementInactive";
                        cell9.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                        cell9.InnerHtml = dr["BPCount"].ToString();
                    }

                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    row.Cells.Add(cell3);
                    row.Cells.Add(cell4);
                    row.Cells.Add(cell5);
                    row.Cells.Add(cell6);
                    row.Cells.Add(cell7);
                    row.Cells.Add(cell8);
                    row.Cells.Add(cell9);


                    rigorTable.Rows.Add(row);
                }

                rigorTable.DataBind();
            }

            distributionTotal.InnerHtml = distributionTotalCount.ToString();
        }

        private void LoadRigorDistributionTable_AssessmentObjectScreen()
        {
            DataTable rigorDistributionTable = _ds.Tables[2];
            int distributionTotalCount = 0;

            if (rigorDistributionTable.Rows.Count > 0)
            {
                rigorLevelLabel.InnerHtml = rigorDistributionTable.Rows[0]["RigorType"].ToString();

                distributionTotalCount += (from DataRow dr in rigorDistributionTable.Rows where DataIntegrity.ConvertToInt(dr["ItemCount"]) > 0 
                                           select DataIntegrity.ConvertToInt(dr["ItemCount"])).Sum();

                AssmtItemSum.ColSpan = 6;
                notSpecifiedCell.Visible = false;
                notSpecifiedCell.Style["display"] = "none";

                foreach (DataRow dr in rigorDistributionTable.Rows)
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

                    string countPercent = " (" + Math.Round(100.0 * DataIntegrity.ConvertToInt(dr["ItemCount"]) / distributionTotalCount).ToString() + "%)";

                    //Distribution
                    cell1.InnerHtml = DataIntegrity.ConvertToInt(dr["ItemCount"]) == 0 ? "&nbsp;" : dr["ItemCount"].ToString() + countPercent;
                    cell1.Attributes["class"] = "contentDistribution";

                    //Rigor
                    cell2.InnerHtml = dr["Text"].ToString() == "Blank" ? "Blank Items" : dr["Text"].ToString();
                    cell2.Attributes["class"] = "contentLabel";
                    if (dr["Text"].ToString() == "Blank")
                    {
                        cell2.Attributes["style"] = "color:#36F";
                    }

                    //Multiple Choice 3
                    cell3.InnerHtml = DataIntegrity.ConvertToInt(dr["MultipleChoice3"]) == 0 ? "&nbsp;" : dr["MultipleChoice3"].ToString();
                    if (dr["MultipleChoice3"].ToString() == "" || dr["MultipleChoice3"].ToString() == "0")
                    {
                        cell3.Attributes["class"] = "contentElementInactive";
                        cell3.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                    }
                    else
                    {
                        cell3.Attributes["class"] = "contentElement";
                        if (isPDFView) cell3.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }

                    //Multiple Choice 4
                    cell4.InnerHtml = DataIntegrity.ConvertToInt(dr["MultipleChoice4"]) == 0 ? "&nbsp;" : dr["MultipleChoice4"].ToString();
                    if (dr["MultipleChoice4"].ToString() == "" || dr["MultipleChoice4"].ToString() == "0")
                    {
                        cell4.Attributes["class"] = "contentElementInactive";
                        cell4.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                    }
                    else
                    {
                        cell4.Attributes["class"] = "contentElement";
                        if (isPDFView) cell4.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }

                    //Multiple Choice 5
                    cell5.InnerHtml = DataIntegrity.ConvertToInt(dr["MultipleChoice5"]) == 0 ? "&nbsp;" : dr["MultipleChoice5"].ToString();
                    if (dr["MultipleChoice5"].ToString() == "" || dr["MultipleChoice5"].ToString() == "0")
                    {
                        cell5.Attributes["class"] = "contentElementInactive";
                        cell5.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                    }
                    else
                    {
                        cell5.Attributes["class"] = "contentElement";
                        if (isPDFView) cell5.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }

                    //Short Answer
                    cell6.InnerHtml = DataIntegrity.ConvertToInt(dr["ShortAnswer"]) == 0
                                            ? "&nbsp;"
                                            : dr["ShortAnswer"].ToString();
                    cell6.Attributes["class"] = "contentElement";
                    if (isPDFView) cell6.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    if (dr["ShortAnswer"].ToString() == "" || dr["ShortAnswer"].ToString() == "0")
                    {
                        cell6.Attributes["class"] = "contentElementInactive";
                        cell6.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                    }
                    else
                    {
                        cell6.Attributes["class"] = "contentElement";
                        if (isPDFView) cell6.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }

                    //Essay
                    cell7.InnerHtml = DataIntegrity.ConvertToInt(dr["Essay"]) == 0 ? "&nbsp;" : dr["Essay"].ToString();
                    cell7.Attributes["class"] = "contentElement";
                    if (isPDFView) cell7.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    if (dr["Essay"].ToString() == "" || dr["Essay"].ToString() == "0")
                    {
                        cell7.Attributes["class"] = "contentElementInactive";
                        cell7.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                    }
                    else
                    {
                        cell7.Attributes["class"] = "contentElement";
                        if (isPDFView) cell5.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }

                    //True False
                    cell8.InnerHtml = DataIntegrity.ConvertToInt(dr["TrueFalse"]) == 0
                                            ? "&nbsp;"
                                            : dr["TrueFalse"].ToString();
                    cell8.Attributes["class"] = "contentElement";
                    if (isPDFView) cell8.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    if (dr["TrueFalse"].ToString() == "" || dr["TrueFalse"].ToString() == "0")
                    {
                        cell8.Attributes["class"] = "contentElementInactive";
                        cell8.Attributes["style"] = "background-color: #C0C0C0;text-align: center;font-weight: normal;";
                    }
                    else
                    {
                        cell8.Attributes["class"] = "contentElement";
                        if (isPDFView) cell8.Attributes["style"] = "font-weight: bold;width: 10%;text-align: center;padding: 3px;";
                    }

                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    row.Cells.Add(cell3);
                    row.Cells.Add(cell4);
                    row.Cells.Add(cell5);
                    row.Cells.Add(cell6);
                    row.Cells.Add(cell7);
                    row.Cells.Add(cell8);

                    rigorTable.Rows.Add(row);
                }

                rigorTable.DataBind();
            }

            distributionTotal.InnerHtml = distributionTotalCount.ToString();
        }

        protected void LoadAddendumDistributionTable()
        {
            DataTable addendumDistributionTable = addendumLevels.BuildAddendumDistributionTable();

            int rowsCount = 6;
             
            if (addendumDistributionTable.Rows.Count > 0)
            {
                string cell1Width;
                if (addendumDistributionTable.Rows.Count > rowsCount)
                {
                    cell1Width = "width:79%;";
                }
                else
                {
                    cell1Width = "width:342px;";
                }

                foreach (DataRow dr in addendumDistributionTable.Rows)
                {
                    TableRow row = new TableRow();
                    TableCell cell1 = new TableCell();
                    TableCell cell2 = new TableCell();

                    string addendumIdEnc = Standpoint.Core.Classes.Encryption.EncryptString(dr["AddendumID"].ToString());

                    HyperLink addendumNameCellData = new HyperLink();
                    addendumNameCellData.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    addendumNameCellData.NavigateUrl = "javascript: void(0)";
                    addendumNameCellData.Attributes["onclick"] = "OpenAddendumText('" + addendumIdEnc + "'); return false;";
                    addendumNameCellData.Text = dr["AddendumName"].ToString();
                    cell1.Controls.Add(addendumNameCellData);
                    cell1.Attributes["style"] = cell1Width + (addendumDistributionTable.Rows.Count < rowsCount ? "border-bottom:solid 1px #000;" : "");

                    cell2.Text = dr["ItemCount"].ToString();
                    cell2.Attributes["class"] = "standardContentElement";
                    cell2.Attributes["style"] = (addendumDistributionTable.Rows.Count < rowsCount ? "border-bottom:solid 1px #000;" : "");
                    cell2.Attributes["style"] = cell2.Attributes["style"] + "font-weight: bold;text-align: center;padding: 3px;";

                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);

                    addendumTable.Rows.Add(row);
                }

                addendumTable.DataBind();
            }
        }

        protected void LoadStandardDistributionTable()
        {
            DataTable standardDistributionTable = new DataTable();
            if (hdnCallBackPage.Value == "rigor")
            {
                standardDistributionTable = rigorLevels.BuildStandardDistributionTable();
            }
            else if(hdnCallBackPage.Value == "addendum")
            {
                standardDistributionTable = addendumLevels.BuildStandardDistributionTable();
            }
            else
            {
                standardDistributionTable = SessionObject.Standards_RigorLevels_ItemCounts.BuildStandardDistributionTable();
            }

            if (standardDistributionTable.Rows.Count > 0)
            {
                string cell1Width;
                string cell2Width;
                if (standardDistributionTable.Rows.Count > 4)
                {
                    cell1Width = "width:41%;";
                    cell2Width = "width:48%;";
                }
                else
                {
                    cell1Width = "width:110px;";
                    cell2Width = "width:128px;";
                    if (isPDFView) standardDistScrollContainerDiv.Attributes["style"] = "height: 85px;";
                }

                foreach (DataRow dr in standardDistributionTable.Rows)
                {
                    TableRow row = new TableRow();
                    TableCell cell1 = new TableCell();
                    TableCell cell2 = new TableCell();
                    TableCell cell3 = new TableCell();

                    string standardLink = Page.ResolveUrl("~/Record/StandardsPage.aspx?xID=" + Standpoint.Core.Classes.Encryption.EncryptString(dr["StandardID"].ToString()));
                    string txtCell1 = SessionObject.LoggedInUser.HasPermission(Base.Enums.Permission.Hyperlink_StandardName) ?
                        "<a href=\"javascript:void();\" onclick=\"window.open('" + standardLink + "'); return false;\">" + dr["Standard"].ToString() + "</a>" :
                        dr["Standard"].ToString();

                    cell1.Text = txtCell1;
                    cell1.ToolTip = dr["Standard"].ToString();
                    cell1.Attributes["class"] = "contentLabel";
                    cell1.Attributes["style"] = cell1Width + (standardDistributionTable.Rows.Count < 4 ? "border-bottom:solid 1px #000;" : "");

                    cell2.Text = string.IsNullOrEmpty(dr["Percentage"].ToString()) ? "&nbsp;" : dr["Percentage"].ToString();
                    cell2.Attributes["class"] = "standardContentElement";
                    cell2.Attributes["style"] = cell2Width + (standardDistributionTable.Rows.Count < 4 ? "border-bottom:solid 1px #000;" : "");
                    cell2.Attributes["style"] = cell2.Attributes["style"] + "font-weight: bold;text-align: center;padding: 3px;";

                    cell3.Text = string.IsNullOrEmpty(dr["Items"].ToString()) ? "&nbsp;" : dr["Items"].ToString();
                    cell3.Attributes["class"] = "standardContentElement";
                    if (standardDistributionTable.Rows.Count < 4)
                    {
                        cell3.Attributes["style"] = "border-bottom:solid 1px #000;";
                    }
                    cell3.Attributes["style"] = cell3.Attributes["style"] + "font-weight: bold;text-align: center;padding: 3px;";

                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    row.Cells.Add(cell3);

                    standardTable.Rows.Add(row);
                }

                standardTable.DataBind();
            }
        }

        protected void LoadStandardDistributionTable_AssessmentObjectScreen()
        {
            DataTable standardDistributionTable = _ds.Tables["StandardQuestionCounts"];

            if (standardDistributionTable.Rows.Count > 0)
            {
                string cell1Width;
                string cell2Width;
                if (standardDistributionTable.Rows.Count > 4 && !isPDFView)
                {
                    cell1Width = "width:41%;";
                    cell2Width = "width:48%;";
                }
                else
                {
                    cell1Width = "width:110px;";
                    cell2Width = "width:128px;";
                    if(isPDFView) standardDistScrollContainerDiv.Attributes["style"] = "height: 85px;";
                }

                foreach (DataRow dr in standardDistributionTable.Rows)
                {
                    TableRow row = new TableRow();
                    TableCell cell1 = new TableCell();
                    TableCell cell2 = new TableCell();
                    TableCell cell3 = new TableCell();

                    HyperLink link = new HyperLink();
                    link.NavigateUrl = "~/Record/StandardsPage.aspx?xID=" + Encryption.EncryptInt(DataIntegrity.ConvertToInt(dr["StandardID"]));

                    cell1.Text = string.IsNullOrEmpty(dr["StandardName"].ToString()) ? "&nbsp;" : "<a href=\"javascript:void();\" onclick=\"window.open('" + link.ResolveClientUrl(link.NavigateUrl) + "'); return false;\">" + dr["StandardName"].ToString() + "</a>";
                    cell1.Attributes["class"] = "contentLabel";
                    cell1.Attributes["style"] = cell1Width + (standardDistributionTable.Rows.Count < 4 ? "border-bottom:solid 1px #000;" : "");

                    cell2.Text = Math.Round(100.0 * DataIntegrity.ConvertToInt(dr["StandardQuestionCount"]) / DataIntegrity.ConvertToInt(dr["TotalItemCount"])).ToString() + "%";
                    cell2.Attributes["class"] = "standardContentElement";
                    cell2.Attributes["style"] = cell2Width + (standardDistributionTable.Rows.Count < 4 ? "border-bottom:solid 1px #000;" : "");
                    cell2.Attributes["style"] = cell2.Attributes["style"] + "font-weight: bold;text-align: center;padding: 3px;";

                    cell3.Text = string.IsNullOrEmpty(dr["StandardQuestionCount"].ToString()) ? "&nbsp;" : dr["StandardQuestionCount"].ToString();
                    cell3.Attributes["class"] = "standardContentElement";
                    if (standardDistributionTable.Rows.Count < 4)
                    {
                        cell3.Attributes["style"] = "border-bottom:solid 1px #000;";
                    }
                    cell3.Attributes["style"] = cell3.Attributes["style"] + "font-weight: bold;text-align: center;padding: 3px;";

                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    row.Cells.Add(cell3);

                    standardTable.Rows.Add(row);
                }

                standardTable.DataBind();
            }
        }

        protected void LoadItemBankDistributionTable()
        {
            DataTable itemBankDistributionTable = ThinkgateDataAccess.FetchDataTable("E3_Assessment_ItemBankDistribution", new object[] { _assessmentID });

            if (itemBankDistributionTable.Rows.Count > 0)
            {
                string cell1Width;
                string cell2Width;
                if (itemBankDistributionTable.Rows.Count > 4 && !isPDFView)
                {
                    cell1Width = "width:41%;";
                    cell2Width = "width:48%;";
                }
                else
                {
                    cell1Width = "width:110px;";
                    cell2Width = "width:128px;";
                    if (isPDFView) itemBankDistScrollContainerDiv.Attributes["style"] = "height: 85px;";
                }

                foreach (DataRow dr in itemBankDistributionTable.Rows)
                {
                    TableRow row = new TableRow();
                    TableCell cell1 = new TableCell();
                    TableCell cell2 = new TableCell();
                    TableCell cell3 = new TableCell();

                    cell1.Text = string.IsNullOrEmpty(dr["ItemBank"].ToString()) ? "&nbsp;" : dr["ItemBank"].ToString();
                    cell1.Attributes["class"] = "contentLabel";
                    cell1.Attributes["style"] = cell1Width + (itemBankDistributionTable.Rows.Count < 4 ? "border-bottom:solid 1px #000;" : "");

                    cell2.Text = string.IsNullOrEmpty(dr["Percentage"].ToString()) ? "&nbsp;" : dr["Percentage"].ToString();
                    cell2.Attributes["class"] = "standardContentElement";
                    cell2.Attributes["style"] = cell2Width + (itemBankDistributionTable.Rows.Count < 4 ? "border-bottom:solid 1px #000;" : "");
                    cell2.Attributes["style"] = cell2.Attributes["style"] + "font-weight: bold;text-align: center;padding: 3px;";

                    cell3.Text = string.IsNullOrEmpty(dr["ItemCount"].ToString()) ? "&nbsp;" : dr["ItemCount"].ToString();
                    cell3.Attributes["class"] = "standardContentElement";
                    if (itemBankDistributionTable.Rows.Count < 4)
                    {
                        cell3.Attributes["style"] = "border-bottom:solid 1px #000;";
                    }
                    cell3.Attributes["style"] = cell3.Attributes["style"] + "font-weight: bold;text-align: center;padding: 3px;";

                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    row.Cells.Add(cell3);

                    itemBankTable.Rows.Add(row);
                }

                itemBankTable.DataBind();
            }
        }

        protected Doc AssessmentSummaryToPdfDoc(PdfRenderSettings settings = null)
        {
            var dp = DistrictParms.LoadDistrictParms();
            if (settings == null) settings = new PdfRenderSettings();

            StringWriter sw = new StringWriter();
            HtmlTextWriter w = new HtmlTextWriter(sw);
            summaryContent.Attributes["style"] = "font-family: Sans-Serif, Arial;font-weight: bold;position: relative;font-size: .8em;";
            summaryContent.RenderControl(w);
            string result_html = sw.GetStringBuilder().ToString();

            int topOffset = settings.HeaderHeight > 0 ? settings.HeaderHeight : 0;
            int bottomOffset = settings.FooterHeight > 0 ? settings.FooterHeight : 0;

            Doc doc = new Doc();
            doc.HtmlOptions.HideBackground = true;
            doc.HtmlOptions.PageCacheEnabled = false;
            doc.HtmlOptions.UseScript = true;
            doc.HtmlOptions.Timeout = 36000;
            doc.HtmlOptions.BreakZoneSize = 100;    // I experiemented with this being 99% instead of 100%, but you end up with passages getting cut off in unflattering ways. This may lead to more blank space... but I think it's the lessor of evils
            doc.HtmlOptions.ImageQuality = 70;

            doc.MediaBox.String = "0 0 " + settings.PageWidth + " " + settings.PageHeight;
            doc.Rect.String = settings.LeftMargin + " " + (0 + bottomOffset).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + (settings.PageHeight - topOffset).ToString();
            doc.HtmlOptions.AddTags = true;
            doc.SetInfo(0, "ApplyOnLoadScriptOnceOnly", "1");

            List<int> forms = new List<int>();

            int theID = doc.AddImageHtml(result_html);

            Thinkgate.Base.Classes.Assessment.ChainPDFItems(doc, theID, forms, settings, dp.PdfPrintPageLimit); 

            if (settings.HeaderHeight > 0 && !String.IsNullOrEmpty(settings.HeaderText))
            {
                /*HttpServerUtility Server = HttpContext.Current.Server;
                headerText = Server.HtmlDecode(headerText);*/
                Doc headerDoc = new Doc();

                headerDoc.MediaBox.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                headerDoc.Rect.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                headerDoc.VPos = 0.5;
                int headerID = headerDoc.AddImageHtml(settings.HeaderText);

                if (!String.IsNullOrEmpty(settings.HeaderText))
                {
                    int form_ref = 0;
                    for (int i = 1; i <= doc.PageCount; i++)
                    {
                        if (form_ref < forms.Count && forms[form_ref] == i)
                        {
                            form_ref++;
                        }
                        else
                        {
                            if (i > 1 || settings.ShowHeaderOnFirstPage)
                            {
                                doc.PageNumber = i;
                                doc.Rect.String = settings.LeftMargin + " " + (settings.PageHeight - settings.HeaderHeight).ToString() + " " + (settings.PageWidth - settings.RightMargin).ToString() + " " + settings.PageHeight;	//LEFT, BOTTOM,WIDTH, HEIGHT
                                doc.VPos = 0.5;

                                theID = doc.AddImageDoc(headerDoc, 1, null);
                                theID = doc.AddImageToChain(theID);
                            }
                        }
                    }
                }
            }

            for (int i = 1; i <= doc.PageCount; i++)
            {
                doc.PageNumber = i;
                doc.Flatten();
            }
            return doc;
        }

        protected void generateButton_Click(object sender, EventArgs e)
        {
            if (hdnCallBackPage.Value == "rigor")
            {
                SessionObject.Standards_RigorLevels_ItemCounts = rigorLevels;
                Session.Remove("ItemRigorSummary");
            }
            else if (hdnCallBackPage.Value == "addendum")
            {
                SessionObject.Standards_Addendumevels_ItemCounts = addendumLevels;
                Session.Remove("AddendumSummary");
            }

            AssessmentWCF obj = new AssessmentWCF();
            string newAssessmentID = obj.RequestNewAssessmentID(new AssessmentWCFVariables());
            Thinkgate.Base.Classes.Grade gradeOrdinal = new Thinkgate.Base.Classes.Grade(grade);
            newAssessmentTitle.Value = "Term " + term + " " + type + " - " + gradeOrdinal.GetFriendlyName() + " Grade " + subject + " " + courseName;

            ScriptManager.RegisterStartupScript(this, typeof(string), "createAssessment", "goToNewAssessment('" + newAssessmentID + "');", true);
        }
    }
        


}
