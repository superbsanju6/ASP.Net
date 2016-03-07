using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Controls.Teacher;
using System.Reflection;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TeacherWCF : Interfaces.ITeacherWCF
    {
        public Dictionary<string, string> RequestSubjectList(TeacherWCFVariables newObject)
        {
            DataTable subjectList = null;
            
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            subjectList = Thinkgate.Base.Classes.Data.TeacherDB.GetSubjectsForTeacher(newObject.TeacherID, newObject.Grade, 0);

            Dictionary<string, string> subjects = new Dictionary<string,string>();

            foreach (DataRow dr in subjectList.Rows)
            {
                subjects.Add(dr["SubjectValue"].ToString(), dr["SubjectText"].ToString());
            }

            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemTotals.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemNames.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardRigorLevel.Clear();
            SessionObject.ItemBanks.Clear();

            return subjects;
        }

        public Dictionary<string, string> RequestCourseList(TeacherWCFVariables newObject)
        {
            DataTable courseList = null;
            
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            courseList = Thinkgate.Base.Classes.Data.TeacherDB.GetCoursesForTeacher(newObject.TeacherID, newObject.Grade, newObject.Subject, 0);

            Dictionary<string, string> courses = new Dictionary<string, string>();

            foreach (DataRow dr in courseList.Rows)
            {
                courses.Add(dr["CourseValue"].ToString(), dr["CourseText"].ToString());
            }

            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemTotals.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemNames.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardRigorLevel.Clear();
            SessionObject.ItemBanks.Clear();

            return courses;
        }

        public string ClearStandardsList(TeacherWCFVariables newObject)
        {
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemTotals.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemNames.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardRigorLevel.Clear();
            SessionObject.ItemBanks.Clear();

            return "standards cleared";
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request TeacherWCF", "TeacherWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end TeacherWCF", "TeacherWCF"); }
        }
    }
}
