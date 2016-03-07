using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Telerik.Web.UI;
using System.Data;
using System;
using Standpoint.Core.Utilities;
using System.Reflection;

using Thinkgate.ExceptionHandling;

namespace Thinkgate.Services
{
    // not happening about a new service, but StandardsWCF would not respond to call from radcombobox, I think perhaps due to difference between ajax enabled wcf and regular wcf
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class StandardsAJAX
    {

        [OperationContract]
        public RadComboBoxData StandardsByStandardSetGradeSubjectCourse(RadComboBoxContext context)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            RadComboBoxData result = new RadComboBoxData();
            string standardSet = context["StandardSet"].ToString();
            string grade = context["Grade"].ToString();
            string subject = context["Subject"].ToString();
            string course = context["Course"].ToString();
            bool includeAllOption = Standpoint.Core.Utilities.DataIntegrity.ConvertToBool(context["IncludeAllOption"]);

            var dtStandards = Thinkgate.Base.Classes.Standards.GetStandardsByStandardSetGradeSubjectCourse(standardSet, grade, subject,
                                                                                         course);
            var items = new List<RadComboBoxItemData>();
            if (dtStandards != null)
                if (dtStandards.Rows.Count > 0 && includeAllOption)
                {
                    items.Add(new RadComboBoxItemData() {Text = "All", Value = "-1"});
                }
                foreach (DataRow row in dtStandards.Rows)
                {
                    var item = new RadComboBoxItemData();
                    item.Text = row["StandardName"].ToString();
                    item.Value = row["ID"].ToString();
                    items.Add(item);    
                }
            
            result.Items = items.ToArray();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return result;
        }

        [OperationContract]
        public RadComboBoxData LoadTeachers(RadComboBoxContext context)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            RadComboBoxData result = new RadComboBoxData();
            var items = new List<RadComboBoxItemData>();
            
            object year;
            context.TryGetValue("Year", out year);
            if (year == null || String.IsNullOrEmpty(year.ToString()))
            {
                var empty_item = new RadComboBoxItemData();
                empty_item.Text = "Select Year to View Teahcers";
                empty_item.Value = "-1";
                empty_item.Enabled = false;
                items.Add(empty_item);
                result.Items = items.ToArray();
                /*result.Message = "Select Year to View Teachers";*/
                result.EndOfItems = true;
                Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
                return result;
            }

            object grade;
            context.TryGetValue("Grade", out grade);
            object schoolId;
            context.TryGetValue("SchoolId", out schoolId);
            var schoolList = new List<int>();
            if (schoolId != null && !String.IsNullOrEmpty(schoolId.ToString()))
            {
                schoolList.Add(DataIntegrity.ConvertToInt(schoolId));
            }
            var sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            var dtTeachers = Thinkgate.Base.Classes.Data.TeacherDB.GetTeachersBySchoolsDT(sessionObject.LoggedInUser.UserId, schoolList, year.ToString(), grade == null ? string.Empty : grade.ToString(), true);
            var count = 0;
            var addedCount = 0;
            int numberOfLoadedItems = context.NumberOfItems;
            foreach (DataRow row in dtTeachers.Rows)
            {
                if (++count < numberOfLoadedItems) continue;
                var item = new RadComboBoxItemData();
                item.Text = row["TeacherName"].ToString();
                item.Value = row["ID"].ToString();
                items.Add(item);
                if (++addedCount == 100) break;
            }

            int totalRows = dtTeachers.Rows.Count;
            if ((numberOfLoadedItems + addedCount) >= totalRows)
                result.EndOfItems = true;
            result.Message = String.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>",
                                       numberOfLoadedItems + addedCount, totalRows);
            result.Items = items.ToArray();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return result;
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request StandardsAJAXWCF", "StandardsAJAXWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end StandardsAJAXWCF", "StandardsAJAXWCF"); }
        }
    }
}
