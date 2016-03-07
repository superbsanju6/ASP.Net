using System.Data;
using Telerik.Web.UI;

namespace Thinkgate.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web;

    using Thinkgate.Base.Classes;
    using Thinkgate.Classes;
    using Standpoint.Core.Utilities;
    using System;
    using System.Reflection;

    using Thinkgate.ExceptionHandling;

    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GradeSubjectCourse
    {
        #region Properties

        public SessionObject SessionObject
        {
            get
            {
                if (HttpContext.Current.Session == null)
                {
                    return null;
                }

                if (HttpContext.Current.Session["SessionObject"] == null)
                {
                    return null;
                }

                return HttpContext.Current.Session["SessionObject"] as SessionObject;
            }
        }

        #endregion

        #region Operation Contracts

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCourseContainer GetClassSubjectsAndCourses(GradeSubjectCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var grades = container.Grades == null ? null : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();

            var courseList = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
                grades == null ? null : (grades.Count > 0 ? grades : null));

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCourseContainer(courseList, container);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCurriculumCourseContainer GetClassSubjectsCoursesAndCurriculums(GradeSubjectCurriculumCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var grades = container.Grades == null ? null : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();

            var courseList = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
                grades == null ? null : (grades.Count > 0 ? grades : null));

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCurriculumCourseContainer(courseList, container);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCourseContainer GetCurrSubjectsAndCourses(GradeSubjectCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var grades = container.Grades == null ? null : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
            var subjects = container.Subjects == null ? null : container.Subjects.FindAll(s => s.Selected).Select(s => s.DisplayText).ToList();

            var courseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
                grades == null ? null : (grades.Count > 0 ? grades : null), subjects == null ? null : (subjects.Count > 0 ? subjects : null));

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCourseContainer(courseList, container);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCurriculumCourseContainer GetClassCoursesAndCurriculum(GradeSubjectCurriculumCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var subjects = container.Subjects == null ? null : container.Subjects.FindAll(s => s.Selected).Select(s => s.DisplayText).ToList();

            var courseList = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
                null,
                subjects == null ? null : (subjects.Count > 0 ? subjects : null));

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCurriculumCourseContainer(courseList, container, false, false);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCourseContainer GetClassCourses(GradeSubjectCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var grades = container.Grades == null ? null : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
            var subjects = container.Subjects == null ? null : container.Subjects.FindAll(s => s.Selected).Select(s => s.DisplayText).ToList();

            // Fixed for #8431
            //var courseList = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
            //    null,
            //    subjects == null ? null : (subjects.Count > 0 ? subjects : null));

            var courseList = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
                grades == null ? null : ( grades.Count > 0 ? grades : null),
                subjects == null ? null : (subjects.Count > 0 ? subjects : null));

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCourseContainer(courseList, container, false, false);
        }


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCourseContainer GetCurrCourses(GradeSubjectCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var grades = container.Grades == null ? null : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
            var subjects = container.Subjects == null ? null : container.Subjects.FindAll(s => s.Selected).Select(s => s.DisplayText).ToList();

            var courseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
                grades == null ? null : (grades.Count > 0 ? grades : null),
                subjects == null ? null : (subjects.Count > 0 ? subjects : null));

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCourseContainer(courseList, container, false, false);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCurriculumCourseContainer GetCurriculumsByClassCourse(GradeSubjectCurriculumCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            CourseList courseList;
            if (container.Courses.Count == 1 && container.Courses[0].DisplayText.Contains("Select a"))
            {
                courseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            }
            else if(container.Courses.Count > 0)
            {
                if (container.Courses[0].DisplayText.Contains("Select a") && CourseMasterList.CurrCourseFromClassCourseDict.ContainsKey(container.Courses[1].ID))
                {
                    courseList = CourseMasterList.CurrCourseFromClassCourseDict[container.Courses[1].ID];
                }
                else if (CourseMasterList.CurrCourseFromClassCourseDict.ContainsKey(container.Courses[0].ID))
                {
                    courseList = CourseMasterList.CurrCourseFromClassCourseDict[container.Courses[0].ID];
                }
                else
                {
                    courseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
                }
            }
            else
            {
                courseList = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCurriculumCourseContainer(courseList, container, false, false);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCourseContainer GetStandardSubjectsAndCourses(GradeSubjectCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            var grades = container.Grades == null ? null : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();

            var courseList = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
                grades == null ? null : (grades.Count > 0 ? grades : null));

            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCourseContainer(courseList, container, false);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectCourseContainer GetStandardCourses(GradeSubjectCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            var grades = container.Grades == null ? null : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
            var subjects = container.Subjects == null ? null : container.Subjects.FindAll(s => s.Selected).Select(s => s.DisplayText).ToList();

            var courseList = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser).FilterByGradesAndSubjects(
                grades == null ? null : (grades.Count > 0 ? grades : null),
                subjects == null ? null : (subjects.Count > 0 ? subjects : null));

            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectCourseContainer(courseList, container, false, false);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public StandardSetGradeSubjectCourseContainer GetStandardSetsGradesSubjectsCourses(StandardSetGradeSubjectCourseContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            var userStandardCourses = CourseMasterList.GetStandardCoursesForUser(SessionObject.LoggedInUser);

            var courseList = new CourseList();


            var standardSets = new List<string>();
            var grades = new List<string>(); 
            var subjects = new List<string>();

            var tempCourse = container.Courses == null ? null : container.Courses.FirstOrDefault(c => c.Value.Substring(c.Value.IndexOf('_') + 1) != "0");
            var courseName = tempCourse != null ? tempCourse.DisplayText : null;

       
                standardSets = container.StandardSet == null ? null : container.StandardSet.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
                grades = container.Grades == null ? null : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
                subjects = container.Subjects == null ? null : container.Subjects.FindAll(s => s.Selected).Select(s => s.DisplayText).ToList();

                courseList = userStandardCourses.FilterByGradesSubjectsStandardSetsAndCourse(grades, subjects, standardSets, courseName != null ? new List<string> { courseName.ToString() } : null);

            
            var intermediate = SessionObject.LoggedInUser == null ? null : CreateStandardSetGradeSubjectCourseContainer(courseList, container, true, true);
            return intermediate;
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectSchoolTeacherYearContainer GetTeachersBySchools(GradeSubjectSchoolTeacherYearContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var years = container.Years == null ? new List<string>() : container.Years.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
            var grades = container.Grades == null ? new List<string>() : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
            var schoolList = new List<Base.Classes.School>();
            if (container.Schools.Count > 0 && !container.Schools[0].DisplayText.Contains("Select a"))
                schoolList.Add(new Base.Classes.School(
                    DataIntegrity.ConvertToInt(container.Schools[0].Value.Replace(container.SchoolsKey + "_", "")),
                    container.Schools[0].DisplayText
                    ));
            else if(grades.Count > 0 && !grades[0].Contains("Select a"))
            {
                schoolList = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
                schoolList = schoolList.FindAll(s => s.Grades.IndexOf(grades[0]) > -1);
            }
            else
            {
                schoolList = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            }

            var teacherList = container.Schools.Count == 0 ||
                              (container.Schools.Count == 1 && container.Schools[0].DisplayText.Contains("Select a")) ||
                              container.Years.Count == 0 ||
                              (container.Years.Count == 1 && container.Years[0].DisplayText.Contains("Select a"))
                                  ? new List<Base.Classes.Teacher>()
                                  : Thinkgate.Base.Classes.Data.TeacherDB.GetTeachersBySchools(schoolList.Select(s => s.ID).ToList(), years.First(), false);

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectSchoolTeacherYearContainer(schoolList, teacherList, container, false);
        }

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public GradeSubjectSchoolTeacherYearContainer GetSchoolsAndTeachersByGrade(GradeSubjectSchoolTeacherYearContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var years = container.Years == null ? new List<string>() : container.Years.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
            var grades = container.Grades == null ? new List<string>() : container.Grades.FindAll(g => g.Selected).Select(g => g.DisplayText).ToList();
            var schools = container.Schools == null ? new List<int>() : container.Schools.FindAll(s => s.Selected).Select(s => s.ID).ToList();
            var schoolList = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);

            if (grades.Count > 0 && !grades[0].Contains("Select a"))
            {
                schoolList = schoolList.FindAll(s => s.Grades.IndexOf(grades[0]) > -1);
            }

            var teacherList = container.Schools.Count == 0 ||
                              (container.Schools.Count == 1 && container.Schools[0].DisplayText.Contains("Select a")) ||
                              container.Years.Count == 0 ||
                              (container.Years.Count == 1 && container.Years[0].DisplayText.Contains("Select a"))
                                  ? new List<Base.Classes.Teacher>()
                                  : schools.Count > 0
                                        ? Thinkgate.Base.Classes.Data.TeacherDB.GetTeachersBySchools(schools, years.First())
                                        : Thinkgate.Base.Classes.Data.TeacherDB.GetTeachersBySchools(schoolList.Select(s => s.ID).ToList(), years.First());

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return SessionObject.LoggedInUser == null ? null : CreateGradeSubjectSchoolTeacherYearContainer(schoolList, teacherList, container);
        }

        [OperationContract]
        public RadComboBoxData LoadGradeForSchoolTeacherCurriculaControl(RadComboBoxContext context)
        {
            RadComboBoxData result = new RadComboBoxData();
            var items = new List<RadComboBoxItemData>();


            object schoolID;
            object teacherID;

            context.TryGetValue("TeacherID", out teacherID);
            context.TryGetValue("SchoolID", out schoolID);

            // If schoolID is not provided, prompt the user to select the School first.
            if (schoolID == null || String.IsNullOrEmpty(schoolID.ToString()))
            {
                var empty_item = new RadComboBoxItemData();
                empty_item.Text = "Select a School to View Grades";
                empty_item.Value = "-1";
                empty_item.Enabled = false;
                items.Add(empty_item);
                result.Items = items.ToArray();
                result.EndOfItems = true;
                return result;
            }

            // Check if teacher is already been selected and if so pull the list of grades belonging to the teacher.
            List<Base.Classes.Grade> teacherGrade = new List<Base.Classes.Grade>();

            if (teacherID != null && !String.IsNullOrEmpty(teacherID.ToString()))
            {
                var dtTeacherGrades = Base.Classes.Teacher.GetGradesForTeacher(DataIntegrity.ConvertToInt(teacherID));
                foreach (DataRow row in dtTeacherGrades.Rows)
                {
                    teacherGrade.Add(new Base.Classes.Grade() { DisplayText = row["Grade"].ToString() });
                }
            }

            List<Base.Classes.Grade> gradeList;
            if (CourseMasterList.GradesForSchoolsDict.TryGetValue(DataIntegrity.ConvertToInt(schoolID), out gradeList))
            {
                if (teacherGrade.Count > 0)
                    gradeList = gradeList.Where(x => teacherGrade.Any(y => x.DisplayText.ToLower().Trim().Equals(y.DisplayText.ToLower().Trim()))).ToList();

                foreach (var grade in gradeList.Distinct().OrderBy(x => x.DisplayText))
                {
                    var item = new RadComboBoxItemData();
                    item.Text = grade.DisplayText;
                    item.Value = grade.DisplayText;
                    items.Add(item);
                }

                result.Items = items.ToArray();
            }
            return result;
        }


        [OperationContract]
        public RadComboBoxData LoadSubjectForSchoolTeacherCurriculaControl(RadComboBoxContext context)
        {
            RadComboBoxData result = new RadComboBoxData();
            var items = new List<RadComboBoxItemData>();


            object schoolID;
            object teacher;
            object grade;

            context.TryGetValue("TeacherID", out teacher);
            context.TryGetValue("SchoolID", out schoolID);
            context.TryGetValue("Grade", out grade);

            // If grade is not provided, prompt the user to select the Grade first.
            if (grade == null || String.IsNullOrEmpty(grade.ToString()))
            {
                var empty_item = new RadComboBoxItemData();
                empty_item.Text = "Select a Grade to View Subject";
                empty_item.Value = "-1";
                empty_item.Enabled = false;
                items.Add(empty_item);
                result.Items = items.ToArray();
                result.EndOfItems = true;
                return result;
            }

            int teacherID = (teacher == null || String.IsNullOrEmpty(teacher.ToString()))
                ? 0
                : DataIntegrity.ConvertToInt(teacher);

            DataTable subjectList =
                Base.Classes.Teacher.GetSubjectsForSchoolTeacherGrade(
                    DataIntegrity.ConvertToInt(schoolID),
                    teacherID,
                    grade.ToString());
            foreach (DataRow subject in subjectList.Rows)
            {
                var item = new RadComboBoxItemData();
                item.Text = subject["SubjectText"].ToString();
                item.Value = subject["SubjectValue"].ToString();
                items.Add(item);
            }

            result.Items = items.ToArray();
            return result;
        }


        [OperationContract]
        public RadComboBoxData LoadCourseForSchoolTeacherCurriculaControl(RadComboBoxContext context)
        {
            RadComboBoxData result = new RadComboBoxData();
            var items = new List<RadComboBoxItemData>();


            object schoolID;
            object teacher;
            object grade;
            object subject;

            context.TryGetValue("TeacherID", out teacher);
            context.TryGetValue("SchoolID", out schoolID);
            context.TryGetValue("Grade", out grade);
            context.TryGetValue("Subject", out subject);

            if (grade == null || subject == null || String.IsNullOrEmpty(subject.ToString()) || String.IsNullOrEmpty(grade.ToString()))
            {
                var empty_item = new RadComboBoxItemData();
                empty_item.Text = "Select a Subject to View Courses";
                empty_item.Value = "-1";
                empty_item.Enabled = false;
                items.Add(empty_item);
                result.Items = items.ToArray();
                result.EndOfItems = true;
                return result;
            }

            int teacherID = (teacher == null || String.IsNullOrEmpty(teacher.ToString()))
                 ? 0
                 : DataIntegrity.ConvertToInt(teacher);

            DataTable courseList =
                Base.Classes.Teacher.GetCoursesForSchoolTeacherGrade(
                    DataIntegrity.ConvertToInt(schoolID),
                    teacherID,
                    grade.ToString(),
                    subject.ToString());

            if (courseList.Rows.Count > 1)
            {
                var itemAll = new RadComboBoxItemData();
                itemAll.Text = "All";
                itemAll.Value = "All";
                items.Add(itemAll);
            }
            foreach (DataRow course in courseList.Rows)
            {
                var item = new RadComboBoxItemData();
                item.Text = course["CourseText"].ToString();
                item.Value = course["CourseValue"].ToString();
                items.Add(item);
            }

            result.Items = items.ToArray();
            return result;
        }

        [OperationContract]
        public RadComboBoxData LoadTeachersForSchoolTeacherCurriculaControl(RadComboBoxContext context)
        {
            RadComboBoxData result = new RadComboBoxData();
            var items = new List<RadComboBoxItemData>();

            object schoolID;
            object grade;
            object subject;
            object course;
            context.TryGetValue("Grade", out grade);
            context.TryGetValue("Subject", out subject);
            context.TryGetValue("Course", out course);
            context.TryGetValue("SchoolID", out schoolID);

            if (schoolID == null || String.IsNullOrEmpty(schoolID.ToString()))
            {
                var empty_item = new RadComboBoxItemData();
                empty_item.Text = "Select a School to View Teachers";
                empty_item.Value = "-1";
                empty_item.Enabled = false;
                items.Add(empty_item);
                result.Items = items.ToArray();
                result.EndOfItems = true;
                return result;
            }

            var districtParms = DistrictParms.LoadDistrictParms();
            var dtTeachers = Base.Classes.Data.TeacherDB.GetTeachersBySchoolsAndCurriculum(DataIntegrity.ConvertToInt(schoolID), districtParms.Year, grade == null ? "" : grade.ToString());
            var count = 0;
            var addedCount = 0;
            int numberOfLoadedItems = context.NumberOfItems;
            foreach (DataRow row in dtTeachers.Rows)
            {
                if (++count <= numberOfLoadedItems) continue;
                var item = new RadComboBoxItemData();
                item.Text = row["TeacherName"].ToString();
                item.Value = row["TeacherPage"].ToString();
                items.Add(item);
                if (++addedCount == 100) break;
            }

            int totalRows = dtTeachers.Rows.Count;
            if ((numberOfLoadedItems + addedCount) >= totalRows)
                result.EndOfItems = true;
            result.Message = String.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>",
                                       numberOfLoadedItems + addedCount, totalRows);
            result.Items = items.ToArray();
            return result;
        }

        #endregion

        #region Private Static Methods

        private static GradeSubjectCourseContainer CreateGradeSubjectCourseContainer(
            CourseList courseList,
            GradeSubjectCourseContainer container,
            bool returnGrades = true,
            bool returnSubjects = true,
            bool returnCourses = true)
        {
            if (courseList == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var result = new GradeSubjectCourseContainer();

            // Courses
            if (returnCourses)
            {
                foreach (var course in courseList)
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.Courses != null)
                    {
                        var passedInCourse = (from c in container.Courses where c.DisplayText == course.CourseName select c).FirstOrDefault();
                        if (passedInCourse != null)
                        {
                            selected = passedInCourse.Selected;
                            value = string.IsNullOrEmpty(passedInCourse.Value) ? string.Format("{0}_{1}", container.CoursesKey, passedInCourse.Value) : passedInCourse.Value;
                        }
                    }

                    result.Courses.Add(
                        new Course
                        {

                            DisplayText = course.CourseName,

                            Selected = selected,
                            Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.CoursesKey, course.ID) : value
                        });
                }

                result.Courses.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));
            }

            // Grades
            if (returnGrades)
            {
                foreach (var grade in courseList.GetGradeList())
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.Grades != null)
                    {
                        var passedInGrade = (from g in container.Grades where g.DisplayText == grade.DisplayText select g).FirstOrDefault();
                        if (passedInGrade != null)
                        {
                            selected = passedInGrade.Selected;
                            value = string.IsNullOrEmpty(passedInGrade.DisplayText) ? string.Format("{0}_{1}", container.GradesKey, passedInGrade.DisplayText) : passedInGrade.DisplayText;
                            
                        }
                    }

                    result.Grades.Add(new Grade { 
                        DisplayText = grade.DisplayText, 
                        Selected = selected,
                        Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.GradesKey, grade.DisplayText) : value
                    });
                }

                result.Grades.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));
            }

            // Subjects
            if (returnSubjects)
            {
                foreach (var subjectText in courseList.GetSubjectList().Select(s => s.DisplayText).Distinct())
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.Subjects != null)
                    {
                        var passedInSubject = (from s in container.Subjects where s.DisplayText == subjectText select s).FirstOrDefault();
                        if (passedInSubject != null)
                        {
                            selected = passedInSubject.Selected;
                            value = string.IsNullOrEmpty(passedInSubject.Value) ? string.Format("{0}_{1}", container.SubjectsKey, passedInSubject.DisplayText) : passedInSubject.Value;
                        }
                    }

                    result.Subjects.Add(new Subject
                    {
                        DisplayText = subjectText,
                        Selected = selected,
                        Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.SubjectsKey, subjectText) : value
                    });
                }
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return result;
        }

        private static StandardSetGradeSubjectCourseContainer CreateStandardSetGradeSubjectCourseContainer(
            CourseList courseList,
            StandardSetGradeSubjectCourseContainer container,
            bool returnGrades = true,
            bool returnSubjects = true,
            bool returnCourses = true,
            bool returnStandardSets = true)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            var result = new StandardSetGradeSubjectCourseContainer();
            var gradeSubjectCourseContainerBase = CreateGradeSubjectCourseContainer(courseList,
                    new GradeSubjectCourseContainer()
                    {
                        Courses = container.Courses,
                        CoursesKey = container.CoursesKey,
                        Grades = container.Grades,
                        GradesKey = container.GradesKey,
                        Subjects = container.Subjects,
                        SubjectsKey = container.SubjectsKey
                    },
                    returnGrades, returnSubjects, returnCourses);

            if (returnStandardSets)
            {
                foreach (var standardSet in CourseMasterList.StandardSets)
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.StandardSet != null)
                    {
                        var passedInStandardSet = (from s in container.StandardSet where s.DisplayText == standardSet select s).FirstOrDefault();
                        if (passedInStandardSet != null)
                        {
                            selected = passedInStandardSet.Selected;
                            value = passedInStandardSet.Value;
                        }
                    }

                    //result.Grades.Add(new Grade { DisplayText = grade.DisplayText, Selected = selected, Value = value });
                    result.StandardSet.Add(new StandardSet
                    {
                        DisplayText = standardSet,
                        Selected = selected,
                        Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.StandardSetKey, standardSet) : value
                    });
                }
            }

            result.Grades = gradeSubjectCourseContainerBase.Grades;
            result.GradesKey = gradeSubjectCourseContainerBase.GradesKey;
            result.Subjects = gradeSubjectCourseContainerBase.Subjects;
            result.SubjectsKey = gradeSubjectCourseContainerBase.SubjectsKey;
            result.Courses = gradeSubjectCourseContainerBase.Courses;
            result.CoursesKey = gradeSubjectCourseContainerBase.CoursesKey;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return result;
        }

        private static GradeSubjectCurriculumCourseContainer CreateGradeSubjectCurriculumCourseContainer(
            CourseList courseList,
            GradeSubjectCurriculumCourseContainer container,
            bool returnGrades = true,
            bool returnSubjects = true,
            bool returnCourses = true,
            bool returnCurriculums = true)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            var result = new GradeSubjectCurriculumCourseContainer();
            var gradeSubjectCourseContainerBase = CreateGradeSubjectCourseContainer(courseList,
                    new GradeSubjectCourseContainer()
                    {
                        Courses = container.Courses,
                        CoursesKey = container.CoursesKey,
                        Grades = container.Grades,
                        GradesKey = container.GradesKey,
                        Subjects = container.Subjects,
                        SubjectsKey = container.SubjectsKey
                    },
                    returnGrades, returnSubjects, returnCourses);

            if (returnCurriculums)
            {
                foreach (var course in courseList)
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.Curriculums != null)
                    {
                        var passedInCourse = (from c in container.Curriculums where c.DisplayText == course.CourseName select c).FirstOrDefault();
                        if (passedInCourse != null)
                        {
                            selected = passedInCourse.Selected;
                            value = string.IsNullOrEmpty(passedInCourse.Value) ? string.Format("{0}_{1}", container.CurriculumsKey, passedInCourse.ID) : passedInCourse.Value;
                        }
                    }

                    result.Curriculums.Add(
                        new Course
                        {
                            DisplayText = course.Grade + "-" + course.CourseName,
                            Selected = selected,
                            Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.CurriculumsKey, course.ID) : value
                        });
                }

                result.Curriculums.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));
            }

            result.Grades = gradeSubjectCourseContainerBase.Grades;
            result.GradesKey = gradeSubjectCourseContainerBase.GradesKey;
            result.Subjects = gradeSubjectCourseContainerBase.Subjects;
            result.SubjectsKey = gradeSubjectCourseContainerBase.SubjectsKey;
            result.Courses = gradeSubjectCourseContainerBase.Courses;
            result.CoursesKey = gradeSubjectCourseContainerBase.CoursesKey;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return result;
        }

        private static GradeSubjectSchoolTeacherContainer CreateGradeSubjectSchoolTeacherContainer(
            List<Base.Classes.School> schoolList,
            List<Base.Classes.Teacher> teacherList,
            GradeSubjectSchoolTeacherContainer container,
            bool returnSchools = true,
            bool returnTeachers = true)
        {
            var result = new GradeSubjectSchoolTeacherContainer();

            if (returnSchools)
            {
                foreach (var school in schoolList)
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.Schools != null)
                    {
                        var passedInSchool = (from s in container.Schools
                                              where
                                                  DataIntegrity.ConvertToInt(s.Value.Replace(container.SchoolsKey + "_", "")) == school.ID
                                              select s).FirstOrDefault();
                        if (passedInSchool != null)
                        {
                            selected = passedInSchool.Selected;
                            value = string.IsNullOrEmpty(passedInSchool.Value) ? string.Format("{0}_{1}", container.SchoolsKey, passedInSchool.Value) : passedInSchool.Value;
                        }
                    }

                    result.Schools.Add(
                        new SchoolObj
                        {
                            DisplayText = school.Name,
                            Selected = selected,
                            Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.SchoolsKey, school.ID) : value
                        });
                }

                result.Schools.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));
            }

            if (returnTeachers)
            {
                foreach (var teacher in teacherList)
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.Teachers != null)
                    {
                        var passedInTeacher = (from s in container.Teachers
                                              where
                                                  DataIntegrity.ConvertToInt(s.Value.Replace(container.TeachersKey + "_", "")) == teacher.PersonID
                                              select s).FirstOrDefault();
                        if (passedInTeacher != null)
                        {
                            selected = passedInTeacher.Selected;
                            value = string.IsNullOrEmpty(passedInTeacher.Value) ? string.Format("{0}_{1}", container.TeachersKey, passedInTeacher.Value) : passedInTeacher.Value;
                        }
                    }

                    result.Teachers.Add(
                        new Teacher
                        {
                            DisplayText = teacher.TeacherName,
                            Selected = selected,
                            Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.TeachersKey, teacher.PersonID) : value
                        });
                }

                result.Teachers.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));
            }

            return result;
        }

        private static GradeSubjectSchoolTeacherYearContainer CreateGradeSubjectSchoolTeacherYearContainer(
            List<Base.Classes.School> schoolList,
            List<Base.Classes.Teacher> teacherList,
            GradeSubjectSchoolTeacherYearContainer container,
            bool returnSchools = true,
            bool returnTeachers = true)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var result = new GradeSubjectSchoolTeacherYearContainer();

            if (returnSchools)
            {
                foreach (var school in schoolList)
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.Schools != null)
                    {
                        var passedInSchool = (from s in container.Schools
                                              where
                                                  DataIntegrity.ConvertToInt(s.Value.Replace(container.SchoolsKey + "_", "")) == school.ID
                                              select s).FirstOrDefault();
                        if (passedInSchool != null)
                        {
                            selected = passedInSchool.Selected;
                            value = string.IsNullOrEmpty(passedInSchool.Value) ? string.Format("{0}_{1}", container.SchoolsKey, passedInSchool.Value) : passedInSchool.Value;
                        }
                    }

                    result.Schools.Add(
                        new SchoolObj
                        {
                            DisplayText = school.Name,
                            Selected = selected,
                            Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.SchoolsKey, school.ID) : value
                        });
                }

                result.Schools.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));
            }

            if (returnTeachers)
            {
                foreach (var teacher in teacherList)
                {
                    var selected = false;
                    var value = string.Empty;

                    if (container.Teachers != null)
                    {
                        var passedInTeacher = (from s in container.Teachers
                                               where
                                                   DataIntegrity.ConvertToInt(s.Value.Replace(container.TeachersKey + "_", "")) == teacher.PersonID
                                               select s).FirstOrDefault();
                        if (passedInTeacher != null)
                        {
                            selected = passedInTeacher.Selected;
                            value = string.IsNullOrEmpty(passedInTeacher.Value) ? string.Format("{0}_{1}", container.TeachersKey, passedInTeacher.Value) : passedInTeacher.Value;
                        }
                    }

                    result.Teachers.Add(
                        new Teacher
                        {
                            DisplayText = teacher.TeacherName,
                            Selected = selected,
                            Value = string.IsNullOrEmpty(value) ? string.Format("{0}_{1}", container.TeachersKey, teacher.PersonID) : value
                        });
                }

                result.Teachers.Sort((x, y) => string.Compare(x.DisplayText, y.DisplayText));
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return result;
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request GradeSubjectCourseWCF", "GradeSubjectCourseWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end GradeSubjectCourseWCF", "GradeSubjectCourseWCF"); }
        }

        #endregion
        
    }


    // This is an example of how to use DataContracts. This should be moved to another file.
    [DataContract]
    public class GradeSubjectCourseContainer
    {
        public GradeSubjectCourseContainer()
        {
            Courses = new List<Course>();
            Grades = new List<Grade>();
            Subjects = new List<Subject>();
        }

        [DataMember]
        public List<Course> Courses { get; set; }

        [DataMember]
        public string CoursesKey { get; set; }

        [DataMember]
        public List<Grade> Grades { get; set; }

        [DataMember]
        public string GradesKey { get; set; }

        [DataMember]
        public List<Subject> Subjects { get; set; }

        [DataMember]
        public string SubjectsKey { get; set; }
    }

    // This is an example of how to use DataContracts. This should be moved to another file.
    [DataContract]
    public class StandardSetGradeSubjectCourseContainer : GradeSubjectCourseContainer
    {
        public StandardSetGradeSubjectCourseContainer()
        {
            StandardSet = new List<StandardSet>();
        }

        [DataMember]
        public List<StandardSet> StandardSet { get; set; }

        [DataMember]
        public string StandardSetKey { get; set; }

    }

    [DataContract]
    public class GradeSubjectCurriculumCourseContainer : GradeSubjectCourseContainer
    {
        public GradeSubjectCurriculumCourseContainer()
        {
            Curriculums = new List<Course>();
        }

        [DataMember]
        public List<Course> Curriculums { get; set; }

        [DataMember]
        public string CurriculumsKey { get; set; }
    }

    [DataContract]
    public class GradeSubjectSchoolTeacherYearContainer : GradeSubjectCourseContainer
    {
        public GradeSubjectSchoolTeacherYearContainer()
        {
            Schools = new List<SchoolObj>();
            Teachers = new List<Teacher>();
            Years = new List<Year>();
        }

        [DataMember]
        public List<SchoolObj> Schools { get; set; }

        [DataMember]
        public string SchoolsKey { get; set; }

        [DataMember]
        public List<Teacher> Teachers { get; set; }

        [DataMember]
        public string TeachersKey { get; set; }

        [DataMember]
        public List<Year> Years { get; set; }

        [DataMember]
        public string YearsKey { get; set; }
    }

    [DataContract]
    public class GradeSubjectSchoolTeacherContainer : GradeSubjectCourseContainer
    {
        public GradeSubjectSchoolTeacherContainer()
        {
            Schools = new List<SchoolObj>();
            Teachers = new List<Teacher>();
        }

        [DataMember]
        public List<SchoolObj> Schools { get; set; }

        [DataMember]
        public string SchoolsKey { get; set; }

        [DataMember]
        public List<Teacher> Teachers { get; set; }

        [DataMember]
        public string TeachersKey { get; set; }
    }

    [DataContract]
    public class Course : GenericControl
    {
        [DataMember]
        public int ClassCourseID { get; set; }

        [DataMember]
        public int ID { get; set; }
    }

    [DataContract]
    public class SchoolObj : GenericControl
    {
        [DataMember]
        public int ID { get; set; }
    }

    [DataContract]
    public class Grade : GenericControl
    {
    }

    [DataContract]
    public class Subject : GenericControl
    {
    }

    [DataContract]
    public class StandardSet : GenericControl
    {
    }

    [DataContract]
    public class Teacher : GenericControl
    {
    }

    [DataContract]
    public class Year : GenericControl
    {
    }

    [DataContract]
    public class GenericControl
    {
        [DataMember]
        public string DisplayText { get; set; }

        [DataMember]
        public bool Selected { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
