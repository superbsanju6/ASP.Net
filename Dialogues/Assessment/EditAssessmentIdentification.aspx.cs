using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web;
using Telerik.Charting;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.Dialogues.Assessment
{
	public partial class EditAssessmentIdentification : System.Web.UI.Page
	{
				public SessionObject SessionObject;
				protected int teacherID;
				protected int classID;
				protected Class classObj;
				protected string classSubject;
				protected string classCourse;
				protected string classGrade;
				protected string classType;
				protected int classTerm;
				protected string classContent;
				protected string classDescription;
				protected StandardRigorLevels rigorLevels;

		protected void Page_Init(object sender, EventArgs e)
		{
						SessionObject = (Thinkgate.Classes.SessionObject)Page.Session["SessionObject"];
						teacherID = DataIntegrity.ConvertToInt(SessionObject.TeacherTileParms.GetParm("userID"));
						classID = DataIntegrity.ConvertToInt(SessionObject.TeacherTileParms.GetParm("levelID"));
						classObj = (Class)SessionObject.TeacherTileParms.GetParm("class");
						classSubject = SessionObject.AssessmentBuildParms.ContainsKey("Subject") ? SessionObject.AssessmentBuildParms["Subject"] : (classObj != null ? classObj.Subject.DisplayText : "");
						classCourse = SessionObject.AssessmentBuildParms.ContainsKey("Course") ? SessionObject.AssessmentBuildParms["Course"] : (classObj != null ? classObj.Course.CourseName : "");
						classGrade = SessionObject.AssessmentBuildParms.ContainsKey("Grade") ? SessionObject.AssessmentBuildParms["Grade"] : (classObj != null ? classObj.Grade.DisplayText : "");
						classType = SessionObject.AssessmentBuildParms.ContainsKey("Type") ? SessionObject.AssessmentBuildParms["Type"] : "";
						classTerm = SessionObject.AssessmentBuildParms.ContainsKey("Term") ? DataIntegrity.ConvertToInt(SessionObject.AssessmentBuildParms["Term"]) : 0;
						classContent = SessionObject.AssessmentBuildParms.ContainsKey("Content") ? SessionObject.AssessmentBuildParms["Content"] : "";
						classDescription = SessionObject.AssessmentBuildParms.ContainsKey("Description") ? SessionObject.AssessmentBuildParms["Description"] : "";
						rigorLevels = SessionObject.Standards_RigorLevels_ItemCounts;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
						if (!IsPostBack)
						{
								assessmentID.Value = Standpoint.Core.Classes.Encryption.EncryptString(AppSettings.Demo_TestID.ToString());

								switch (Request.QueryString["headerImg"])
								{
										case "repairtool":
												headerImg.Src = "../../Images/repairtool.png";
												headerImg.Attributes["headerImgName"] = "repairtool";
												break;
										case "lightningbolt":
												headerImg.Src = "../../Images/lightningbolt.png";
												headerImg.Attributes["headerImgName"] = "lightningbolt";
												nextButton.OnClientClick = "createAssessment('submitQuickBuildXmlHttpPanel'); return false;";
												break;
										case "magicwand":
												headerImg.Src = "../../Images/magicwand.png";
												headerImg.Attributes["headerImgName"] = "magicwand";
												backButton.Visible = true;
												break;
										default:
												headerImg.Visible = false;
												break;
								}

								LoadGradeButtonFilter();
								LoadSubjectsButtonFilter();
								LoadCoursesButtonFilter();
								//LoadTypeButtonFilter();
								SetSelectedTermButtonFilter();
								SetSelectedContentButtonFilter();

								descriptionInput.Value = classDescription;

								if(rigorLevels.StandardItemTotals.Count > 0)
								{
										standardsCleared.Value = "false";
								}
						}
		}

				protected void LoadGradeButtonFilter()
				{
						var gradeListTable = Base.Classes.Teacher.GetGradesForTeacher(teacherID, 0);
						gradeList.Items.Clear();
						gradeList.Attributes["teacherID"] = teacherID.ToString();

						foreach (DataRow dr in gradeListTable.Rows)
						{
								RadMenuItem gradeListItem = new RadMenuItem(dr["Grade"].ToString());
								Grade gradeOrdinal = new Grade(dr["Grade"].ToString());
								gradeListItem.Attributes["gradeOrdinal"] = gradeOrdinal.GetFriendlyName();

								if (dr["Grade"].ToString() == classGrade)
								{
										gradeListItem.Selected = true;
										gradeButton.Text = dr["Grade"].ToString();
								}

								gradeListItem.Width = 200;
								gradeList.Items.Add(gradeListItem);
						}

						if (gradeList.SelectedValue.Length == 0 && gradeList.Items.Count == 1)
						{
								gradeList.Items[0].Selected = true;
								gradeButton.Text = gradeList.SelectedValue;
						}
				}

				protected void LoadSubjectsButtonFilter()
				{
						var grade = gradeList.SelectedValue.Length > 0 ? gradeList.SelectedValue : "";
						var subjectListTable = Base.Classes.Teacher.GetSubjectsForTeacher(teacherID, grade, 0);

						subjectList.Items.Clear();
						subjectList.Attributes["teacherID"] = teacherID.ToString();

						foreach (DataRow dr in subjectListTable.Rows)
						{
								RadMenuItem subjectListItem = new RadMenuItem(dr["SubjectText"].ToString());
								if (dr["SubjectText"].ToString() == classSubject)
								{
										subjectListItem.Selected = true;
										subjectButton.Text = dr["SubjectText"].ToString();
								}

								subjectListItem.Width = 200;
								subjectList.Items.Add(subjectListItem);
						}

						if (subjectList.SelectedValue.Length == 0 && subjectList.Items.Count == 1)
						{
								subjectList.Items[0].Selected = true;
								subjectButton.Text = subjectList.SelectedValue;
						}
				}

				protected void LoadCoursesButtonFilter()
				{
						var grade = gradeList.SelectedValue.Length > 0 ? gradeList.SelectedValue : "";
						var subject = subjectList.SelectedValue.Length > 0 ? subjectList.SelectedValue : "";
						var courseListTable = Base.Classes.Teacher.GetCoursesForTeacher(teacherID, grade, subject, 0);

						courseList.Items.Clear();
						courseList.Attributes["teacherID"] = teacherID.ToString();

						foreach (DataRow dr in courseListTable.Rows)
						{
								RadMenuItem courseListItem = new RadMenuItem(dr["CourseText"].ToString());
								if (dr["CourseText"].ToString() == classCourse)
								{
										courseListItem.Selected = true;
										courseButton.Text = dr["CourseText"].ToString();
								}

								courseListItem.Width = 200;
								courseList.Items.Add(courseListItem);
						}

						if (courseList.SelectedValue.Length == 0 && courseList.Items.Count == 1)
						{
								courseList.Items[0].Selected = true;
								courseButton.Text = courseList.SelectedValue;
						}
				}

                // TODO  MERGE ISSUE
                //protected void LoadTypeButtonFilter()
                //{
                //        var typeListTable = Base.Classes.Teacher.GetTestTypesForTeacher("Classroom", 0);

                //        typeList.Items.Clear();

                //        foreach (DataRow dr in typeListTable.Rows)
                //        {
                //                RadMenuItem typeListItem = new RadMenuItem(dr["Type"].ToString());
                //                if (dr["Type"].ToString() == classType)
                //                {
                //                        typeListItem.Selected = true;
                //                        typeButton.Text = dr["Type"].ToString();
                //                }

                //                typeListItem.Width = 200;
                //                typeList.Items.Add(typeListItem);
                //        }

                //        if (typeList.Items.Count > 0)
                //        {
                //                typeList.Items[0].Selected = true;
                //                typeButton.Text = typeList.SelectedValue;
                //        }
                //}

				protected void SetSelectedTermButtonFilter()
				{
						if (classTerm > 0)
						{
								termList.Items[classTerm - 1].Selected = true;
								termButton.Text = classTerm.ToString();
						}
				}

				protected void SetSelectedContentButtonFilter()
				{
						RadMenuItem contentListItem = contentList.Items.FindItemByText(classContent);

						if (contentListItem != null)
						{
								contentListItem.Selected = true;
								contentButton.Text = classContent;
						}
				}
	}
}