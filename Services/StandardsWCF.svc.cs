using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Activation;
using Telerik.Web.UI;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Linq;
using System;
using System.Reflection;

using Thinkgate.ExceptionHandling;

namespace Thinkgate.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class StandardsWCF : Interfaces.IStandardsWCF
    {
        public Dictionary<string, string> RequestStandardsCourseList(StandardsWCFVariables newObject)
        {
            DataTable standardsCourseList = null;
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            if (newObject.ClassID > 0)
            {
                standardsCourseList = Base.Classes.Standards.GetStandardsCoursesBy_Class_StandardSet(newObject.ClassID, newObject.StandardsSet, 0);
            }
            else
            {
                standardsCourseList = Base.Classes.Standards.GetStandardsCoursesBy_Teacher_StandardSet(newObject.LevelID, newObject.StandardsSet, 0);
            }

            Dictionary<string, string> standardsCourses = new Dictionary<string,string>();

            foreach (DataRow dr in standardsCourseList.Rows)
            {
                standardsCourses.Add(dr["CourseValue"].ToString(), dr["CourseText"].ToString());
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return standardsCourses;
        }

        public Dictionary<string, string> RequestStandardsSubjectList(StandardsWCFVariables newObject)
        {
            DataTable standardsSubjectList = null;
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            standardsSubjectList = Base.Classes.Standards.GetStandardsSubjectsBy_Teacher_StandardSet(newObject.LevelID, newObject.StandardsSet, 0);

            Dictionary<string, string> standardsSubjects = new Dictionary<string, string>();

            foreach (DataRow dr in standardsSubjectList.Rows)
            {
                standardsSubjects.Add(dr["SubjectValue"].ToString(), dr["SubjectText"].ToString());
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return standardsSubjects;
        }

        public List<string> LoadStandardsSubjectList(StandardsWCFVariables standardsVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            var standardCourseList = Base.Classes.CourseMasterList.GetStandardCoursesForUser(sessionObject.LoggedInUser);
            var subjectList = standardCourseList.FilterByGrade(standardsVars.Grade).GetSubjectList();
            var returnList = new List<string>();

            foreach (Base.Classes.Subject subject in subjectList)
            {
                returnList.Add(subject.DisplayText);
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnList;
        }

        public Dictionary<string, List<string>> LoadStandardsGradeSubjectCourseList(StandardsWCFVariables standardsVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            var standardCourseList = Base.Classes.CourseMasterList.GetStandardCoursesForUser(sessionObject.LoggedInUser);
            var standardsSetList = string.IsNullOrEmpty(standardsVars.StandardsSet) ? null : new List<string> {standardsVars.StandardsSet};
            var gradeList = (from g in standardCourseList.FilterByStandardsSets(standardsSetList).GetGradeList() select g).ToList();
            var selectedGrade = gradeList.Count == 1 ? (from g in gradeList select g.DisplayText).ToList() : null;
            var subjectList = (from s in standardCourseList.FilterByGradesAndStandardsSet(selectedGrade, standardsVars.StandardsSet).GetSubjectList() select s).ToList();
            var selectedSubject = subjectList.Count == 1 ? (from s in subjectList select s.DisplayText).ToList() : null;
            var courseList = standardCourseList.FilterByGradesSubjectsAndStandardSets(selectedGrade, selectedSubject, new List<string> { standardsVars.StandardsSet });
            var gradeSubjectCoursesDict = new Dictionary<string, List<string>>();

            gradeSubjectCoursesDict.Add("grade", (from g in gradeList select g.DisplayText).ToList());
            gradeSubjectCoursesDict.Add("subject", (from s in subjectList select s.DisplayText).ToList());
            gradeSubjectCoursesDict.Add("course", courseList.GetCourseNames().ToList());

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return gradeSubjectCoursesDict;
        }

        public Dictionary<string, List<string>> LoadStandardsSubjectCourseList(StandardsWCFVariables standardsVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            var standardCourseList = Base.Classes.CourseMasterList.GetStandardCoursesForUser(sessionObject.LoggedInUser);
            var selectedGrade = new List<string> { standardsVars.Grade };
            var subjectList = (from s in standardCourseList.FilterByGradesAndStandardsSet(selectedGrade, standardsVars.StandardsSet).GetSubjectList() select s).ToList();
            var selectedSubject = subjectList.Count == 1 ? (from s in subjectList select s.DisplayText).ToList() : null;

            var subjectCoursesDict = new Dictionary<string, List<string>>();

            if (standardsVars.StandardsSet != "")
            {
                var courseList = standardCourseList.FilterByGradesSubjectsAndStandardSets(selectedGrade, selectedSubject, new List<string> { standardsVars.StandardsSet });
                subjectCoursesDict.Add("course", courseList.GetCourseNames().ToList());
            }
            else
            {
                var newcourseList = standardCourseList.FilterByGradesSubjectsAndStandardSets(selectedGrade, selectedSubject, null);
                subjectCoursesDict.Add("course", newcourseList.GetCourseNames().ToList());
            }
            subjectCoursesDict.Add("subject", (from s in subjectList select s.DisplayText).ToList());

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return subjectCoursesDict;
        }

        public List<string> LoadStandardsCourseList(StandardsWCFVariables standardsVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            var standardCourseList = Base.Classes.CourseMasterList.GetStandardCoursesForUser(sessionObject.LoggedInUser);
            var gradeList = string.IsNullOrEmpty(standardsVars.Grade) ? null : new List<string> { standardsVars.Grade  };
            var subjectList = new List<string> { standardsVars.Subject };
            var standardsSetList = string.IsNullOrEmpty(standardsVars.StandardsSet) ? null : new List<string> {standardsVars.StandardsSet};
            var courseList = standardCourseList.FilterByGradesSubjectsAndStandardSets(gradeList, subjectList, standardsSetList);
            var returnList = courseList.GetCourseNames().ToList();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnList;
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request StandardsWCF", "StandardsWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end StandardsWCF", "StandardsWCF"); }
        }
    }
}
