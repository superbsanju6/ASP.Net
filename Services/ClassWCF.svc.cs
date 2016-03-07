using System.ServiceModel.Activation;
using System.Collections.Generic;
using Thinkgate.Base.DataAccess;
using Standpoint.Core.Utilities;
using System;
using System.Reflection;

using Thinkgate.ExceptionHandling;

namespace Thinkgate.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ClassWCF : Interfaces.IClassWCF
    {
        public string DeleteClass(ClassWCFVariables classVariables)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            string returnMessage = string.Empty;

            if (SessionObject.LoggedInUser.HasPermission(Base.Enums.Permission.Edit_Class))
                returnMessage = Thinkgate.Base.Classes.Class.RemoveClass(classVariables.ClassID, SessionObject.LoggedInUser.Page);
            else
                returnMessage = "Error: User does not have rights to delete classes.";

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnMessage;
        }

        public string UpdateClass(ClassWCFVariables classVariables)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            string returnMessage = Thinkgate.Base.Classes.Class.SaveClassChanges(classVariables.ClassID, classVariables.Course, classVariables.Section, classVariables.Period, classVariables.Semester, classVariables.Block, classVariables.RetentionFlag, classVariables.School, classVariables.UserID);

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnMessage;
        }

        public List<object> LoadClassSubjectList(ClassWCFVariables classVariables)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            var classCourseList = Base.Classes.CourseMasterList.GetClassCoursesForUser(sessionObject.LoggedInUser);
            var subjectList = classCourseList.FilterByGrade(classVariables.Grade).GetSubjectList();
            var returnList = new List<object>();

            foreach (Base.Classes.Subject subject in subjectList)
            {
                returnList.Add(new object[] { subject.DisplayText, subject.DisplayText });
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnList;
        }

        public List<object> LoadClassCourseList(ClassWCFVariables classVariables)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            var classCourseList = Base.Classes.CourseMasterList.GetClassCoursesForUser(sessionObject.LoggedInUser);
            var gradeList = new List<string>();
            var subjectList = new List<string>();
            gradeList.Add(classVariables.Grade);
            subjectList.Add(classVariables.Subject);
            var courseList = classCourseList.FilterByGradesAndSubjects(gradeList, subjectList);
            var returnList = new List<object>();

            foreach (Base.Classes.Course course in courseList)
            {
                returnList.Add(new object[] { course.ID, course.CourseName });
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnList;
        }

        /// <summary>
        /// Create copy of the class specified by the classID parameter.
        /// </summary>
        /// <param name="classVariables">Object holding key properties of the targeted class.</param>
        /// <returns>ID of the copy of the original class.</returns>
        public string CopyClass(ClassWCFVariables classVariables)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            var dr = ThinkgateDataAccess.FetchDataRow("E3_Class_Copy", new object[] {classVariables.ClassID});

            if (dr == null) return null;

            var copyIDEncrypted = Standpoint.Core.Classes.Encryption.EncryptInt(DataIntegrity.ConvertToInt(dr["CopyID"]));

            Thinkgate.Classes.SessionObject oSessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            oSessionObject.GenericPassThruParm = "CLONE:" + copyIDEncrypted;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return oSessionObject.GenericPassThruParm;
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request ClassWCF", "ClassWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end ClassWCF", "ClassWCF"); }
        }
    }
}
