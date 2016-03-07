using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;
using Thinkgate.Services.Contracts.UsageStatistics;

namespace Thinkgate.Classes.UsageStatisticsReport
{
    #region Usage report grid group column title and span preparation
    public class UsageGridColumnGroup
    {
        Dictionary<string, string> dicColumnTitleGroup = new Dictionary<string, string>();
        public UsageGridColumnGroup()
        {
            //dicColumnTitleGroup.Add("SrNo", "");
            dicColumnTitleGroup.Add("Level", "");
            dicColumnTitleGroup.Add("School", "");
            dicColumnTitleGroup.Add("User", "");
            dicColumnTitleGroup.Add("CurrentStudents", "Login Statistics");
            dicColumnTitleGroup.Add("CurrentClasses", "Login Statistics");
            dicColumnTitleGroup.Add("CurrentStaff", "Login Statistics");
            dicColumnTitleGroup.Add("TotalLogins", "Login Statistics");
            dicColumnTitleGroup.Add("LoginUsers", "Login Statistics");
            dicColumnTitleGroup.Add("PctLoggedIn", "Login Statistics");

            dicColumnTitleGroup.Add("ClassroomTestCreated", "Classroom Assessments");
            dicColumnTitleGroup.Add("ClassroomTestAdministered", "Classroom Assessments");
            dicColumnTitleGroup.Add("ClassroomTestResults", "Classroom Assessments");
            dicColumnTitleGroup.Add("ClassroomPaperTestResults", "Classroom Assessments");
            dicColumnTitleGroup.Add("ClassroomOnlineTestResults", "Classroom Assessments");

            dicColumnTitleGroup.Add("DistrictTestCreated", "District Assessments");
            dicColumnTitleGroup.Add("DistrictTestAdministered", "District Assessments");
            dicColumnTitleGroup.Add("DistrictTestResults", "District Assessments");
            dicColumnTitleGroup.Add("DistrictPaperTestResults", "District Assessments");
            dicColumnTitleGroup.Add("DistrictOnlineTestResults", "District Assessments");

            dicColumnTitleGroup.Add("CurriculumMap", "Instruction");
            dicColumnTitleGroup.Add("UnitPlan", "Instruction");
            dicColumnTitleGroup.Add("LessonPlan", "Instruction");
            dicColumnTitleGroup.Add("Other", "Instruction");
        }

        List<UsageGridGroupDetail> lstUsageGridGroupDetails = new List<UsageGridGroupDetail>();

        public List<UsageGridGroupDetail> GetUsageGridGroupDetails(IEnumerable<UsageStatisticData> lstUsageData, UsageStatisticInputParameters criteriaObject)
        {
            foreach (var prop in System.Type.GetType("UsageStatisticData").GetProperties())
            {
                var strColumnName = prop.Name;

                if (dicColumnTitleGroup[strColumnName.Replace("_", "")] != null)
                {
                    bool flag = false;
                    foreach (UsageGridGroupDetail oGroupDetail in lstUsageGridGroupDetails)
                    {
                        if (oGroupDetail.GroupTitle.Equals(dicColumnTitleGroup[strColumnName.Replace("_", "")]))
                        {
                            oGroupDetail.ColumnSpan = oGroupDetail.ColumnSpan + 1;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        UsageGridGroupDetail oUsageGridGroupDetail = new UsageGridGroupDetail();
                        oUsageGridGroupDetail.ColumnSpan = 1;
                        oUsageGridGroupDetail.GroupTitle = dicColumnTitleGroup[strColumnName.Replace("_", "")];
                        lstUsageGridGroupDetails.Add(oUsageGridGroupDetail);
                    }
                }
            }


            foreach (UsageGridGroupDetail oGroupDetail in lstUsageGridGroupDetails)
            {
                if (oGroupDetail.GroupTitle.Equals(" "))
                {
                    oGroupDetail.ColumnSpan = oGroupDetail.ColumnSpan + 1;
                    break;
                }
            }

            if (criteriaObject.ComponentType == null || criteriaObject.ComponentType.Count() == 0 || criteriaObject.ComponentType.Contains(UsageStatisticReportComponentType.All.ToString()))
            {
                return lstUsageGridGroupDetails;
            }

            var xpr = lstUsageGridGroupDetails.Where(x => x.GroupTitle.Contains("Login")).Any();

            if (!criteriaObject.ComponentType.Contains(UsageStatisticReportComponentType.Login.ToString()) && lstUsageGridGroupDetails.Where(x => x.GroupTitle.Contains("Login")).Any())
            {
                lstUsageGridGroupDetails.Remove(lstUsageGridGroupDetails.Where(x => x.GroupTitle.Contains("Login")).First());
            }

            if (!criteriaObject.ComponentType.Contains(UsageStatisticReportComponentType.Assessment.ToString()) && lstUsageGridGroupDetails.Where(x => x.GroupTitle.Contains("Assessment")).Any())
            {
                var lstSelectedTypes = lstUsageGridGroupDetails.Where(x => x.GroupTitle.Contains("Assessment")).Select(x => x).ToList();
                foreach (var usageGroup in lstSelectedTypes)
                {
                    lstUsageGridGroupDetails.Remove(lstUsageGridGroupDetails.Where(x => x.GroupTitle.Contains("Assessment")).First());
                }
            }
            if (!criteriaObject.ComponentType.Contains(UsageStatisticReportComponentType.Instruction.ToString()) && lstUsageGridGroupDetails.Where(x => x.GroupTitle.Contains("Instruction")).Any())
            {
                lstUsageGridGroupDetails.Remove(lstUsageGridGroupDetails.Where(x => x.GroupTitle.Contains("Instruction")).First());
            }


            return lstUsageGridGroupDetails;
        }
    }
    #endregion
}
