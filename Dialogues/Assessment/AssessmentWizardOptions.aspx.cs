using System;
using System.Data;
using System.Web.UI;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using System.Linq;
using Thinkgate.Domain.Classes;
using Thinkgate.Utilities;
using System.Collections.Generic;

namespace Thinkgate.Dialogues.Assessment
{
    public partial class AssessmentWizardOptions : System.Web.UI.Page
    {
        protected SessionObject _sessionObject = null;
        protected StandardRigorLevels rigorLevels;

        public int CountAddendums { get; set; }

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
                    default:
                        headerImg.Visible = false;
                        break;
                }
                SetNewAssessmentTitleValue();
            }
            CheckSelectedStandards();
        }

        protected void SetNewAssessmentTitleValue()
        {
            _sessionObject = (SessionObject)Page.Session["SessionObject"];
            string grade = _sessionObject.AssessmentBuildParms["Grade"];
            Grade gradeOrdinal = new Grade(grade);
            string subject = _sessionObject.AssessmentBuildParms["Subject"];
            int courseID = DataIntegrity.ConvertToInt(_sessionObject.AssessmentBuildParms["Course"]);
            Base.Classes.Course assessmentCourse = CourseMasterList.GetCurrCourseById(courseID);
            string courseName = assessmentCourse != null ? assessmentCourse.CourseName : "";
            string type = _sessionObject.AssessmentBuildParms["Type"];
            string term = _sessionObject.AssessmentBuildParms["Term"];

            courseName = courseName == subject ? "" : courseName;
            newAssessmentTitle.Value = "Term " + term + " " + type + " - " + gradeOrdinal.GetFriendlyName() + " Grade " + subject + " " + courseName;
        }

        protected void CheckSelectedStandards()
        {
            _sessionObject = (SessionObject)Page.Session["SessionObject"];
            rigorLevels = _sessionObject.Standards_RigorLevels_ItemCounts;

            blankItemCounts.Value = rigorLevels.BlankItemCounts.Count.ToString();

            List<int> standardIds = rigorLevels.StandardItemNames.ToList().Select(st => st.Key).ToList();
            DataSet addendumsDataSet = Base.Classes.Addendum.GetAddendumsByStandards(standardIds);

            if (addendumsDataSet != null && addendumsDataSet.Tables.Count > 0)
            {
                CountAddendums = addendumsDataSet.Tables[0].Rows.Count;
                btnAddendumType.Attributes["CountAddendums"] = CountAddendums.ToString();
            }
        }
    }
}