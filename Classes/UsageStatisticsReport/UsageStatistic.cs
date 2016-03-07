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
using Thinkgate.Base.Classes.UsageStatisticsReport;
using Thinkgate.Services.Contracts.UsageStatistics;

namespace Thinkgate.Classes.UsageStatisticsReport
{

    /// <summary>
    /// Provides data and metadata for usage statistic report. to called from controller.
    /// </summary>
    public class UsageStatisticResultData
    {
        public List<UsageStatisticData> lstUsageData { get; set; }
        public List<UsageGridMetaData> lstMetaData { get; set; }
        public List<UsageGridGroupDetail> lstGridColumGroup { get; set; }
        public List<string> graphicalViewData { get; set; }

        public void PrepareReportData(UsageStatisticInputParameters criteriaObject, IEnumerable<UsageStatisticData> lstUsageDataFromService)
        {
            DataTable dtMappings = UsageStatisticMappings.GetUsageMappings();
            var lstComponentType = criteriaObject.ComponentType.Split(',').ToList();
            this.lstUsageData = lstUsageDataFromService.ToList();
            this.lstMetaData = GetMetaDataList(lstComponentType, dtMappings);
            this.lstGridColumGroup = GetUsageGridGroupDetails(lstComponentType, dtMappings);
            if (lstUsageDataFromService.ToList().Count()>=1)
            this.graphicalViewData = GetChartData(lstComponentType, lstUsageDataFromService.ToList()[0]);
        }

        public List<UsageGridMetaData> GetMetaDataList(List<string> lstComponentType, DataTable dtMappings)
        {
            List<UsageGridMetaData> lst = new List<UsageGridMetaData>();

            foreach (DataRow row in dtMappings.Rows)
            {
                lst.Add(new UsageGridMetaData() { data = row["ColumnName"].ToString(), title = row["DisplayName"].ToString(), visible = IsColumnVisible(lstComponentType, row["ColumnName"].ToString(), row["DisplayGroup"].ToString()) });
            }
            return lst;
        }


        public List<UsageGridGroupDetail> GetUsageGridGroupDetails(List<string> lstComponentType, DataTable dtMappings)
        {
            List<UsageGridGroupDetail> lstUsageGridGroupDetails = new List<UsageGridGroupDetail>();

            foreach (DataRow row in dtMappings.Rows)
            {
                bool flag = false;
                foreach (UsageGridGroupDetail oGroupDetail in lstUsageGridGroupDetails)
                {
                    if (oGroupDetail.GroupTitle.Equals(row["DisplayGroup"].ToString()))
                    {
                        oGroupDetail.ColumnSpan = oGroupDetail.ColumnSpan + 1;
                        flag = true;
                        break;
                    }
                }
                if (!flag && IsGroupVisible(lstComponentType, row["DisplayGroup"].ToString()))
                {
                    UsageGridGroupDetail oUsageGridGroupDetail = new UsageGridGroupDetail();
                    oUsageGridGroupDetail.ColumnSpan = 1;
                    oUsageGridGroupDetail.GroupTitle = row["DisplayGroup"].ToString();
                    lstUsageGridGroupDetails.Add(oUsageGridGroupDetail);
                }
            }
            return lstUsageGridGroupDetails;
        }

        public bool IsColumnVisible(List<string> lstComponentType, string strColumnName, string groupName)
        {
            /*If none or all  is selected every column should be visible*/
            if (lstComponentType == null || lstComponentType.Count() == 0 || lstComponentType.Contains(UsageStatisticReportComponentType.All.ToString()))
            {
                return true;
            }

            /*IF group is not specified it is visible in comon section*/
            if (groupName.ToLower() == "")
                return true;

            foreach (var componentType in lstComponentType)
            {
                if (groupName.ToLower().IndexOf(componentType.ToLower()) != -1)
                    return true;
            }
            return false;
        }

        public bool IsGroupVisible(List<string> lstComponentType, string strGroup)
        {
            /*Return all groups is none or all is selected in component type*/
            bool visible = false;
            if (lstComponentType == null || lstComponentType.Count() == 0 || lstComponentType.Contains(UsageStatisticReportComponentType.All.ToString()))
                visible = true;
            else if (strGroup == "")
                visible = true;
            else if (strGroup.Contains(UsageStatisticReportComponentType.Profile.ToString()) && lstComponentType.Contains(UsageStatisticReportComponentType.Profile.ToString()))
                visible = true;
            else if (strGroup.Contains(UsageStatisticReportComponentType.Login.ToString()) && lstComponentType.Contains(UsageStatisticReportComponentType.Login.ToString()))
                visible = true;
            else if (strGroup.Contains(UsageStatisticReportComponentType.Assessment.ToString()) && lstComponentType.Contains(UsageStatisticReportComponentType.Assessment.ToString()))
                visible = true;
            else if (strGroup.Contains(UsageStatisticReportComponentType.Instruction.ToString()) && lstComponentType.Contains(UsageStatisticReportComponentType.Instruction.ToString()))
                visible = true;
            return visible;
        }

        private List<string> GetChartData(List<string> lstComponentType, UsageStatisticData lstUsageData)
        {
            string rawData = "";
            string rawActualData = string.Empty;
            DataTable dtMappings = UsageStatisticMappings.GetUsageMappings();
            List<string> listgraphData = new List<string>();

            bool has = lstComponentType.Any(item => item == UsageStatisticReportComponentType.All.ToString());

            if (has)
            {
                List<string> lstContentType = Enum.GetNames(typeof(UsageStatisticReportComponentType)).ToList();

                foreach (var itemcontent in lstContentType)
                {
                    if (itemcontent == UsageStatisticReportComponentType.All.ToString())
                        break;
                    rawData = "";
                    var results = (from myRow in dtMappings.AsEnumerable()
                                   where myRow.Field<string>("DisplayGraphGroup") != "-" && myRow.Field<string>("GraphContentType").Contains(itemcontent)
                                   select myRow).ToList();

                    var datafilter = (from m in results
                                      group m by new { DisplayGraphGroup = m["DisplayGraphGroup"], GraphType = m["GraphType"] } into g
                                      where g.Count() > 1
                                      select new { ChartTitle = g.Key.DisplayGraphGroup, ChartType = g.Key.GraphType, Count = g.Count() }).ToList();
                    if (datafilter.Count() > 1)
                    {
                        foreach (var row in datafilter)
                        {
                            rawData = "[['" + row.ChartTitle + "','" + row.ChartType + "'],";

                            var itemlist = (from dt in results.AsEnumerable()
                                            where dt["DisplayGraphGroup"].ToString().Trim() == row.ChartTitle.ToString().Trim()
                                            select dt).ToArray();
                            
                            
                            List<int> allValues = new List<int>();
                            rawActualData = string.Empty;

                            foreach (var item2 in itemlist)
                            {
                                var value = lstUsageData.GetType().GetProperty(item2["ColumnName"].ToString()).GetValue(lstUsageData, null);

                                rawActualData += "['" + item2["DisplayName"].ToString() + "', " + Convert.ToInt32(value) + "],";

                                allValues.Add(Convert.ToInt32(value));
                            }

                            if (allValues.Any(any => any != 0))
                            {
                                rawData += rawActualData;
                            }

                            listgraphData.Add((rawData == "[" ? rawData : rawData.Substring(0, rawData.LastIndexOf(","))) + "]");
                        }
                    }
                    else
                    {
                        if (results.Any())
                        {
                            rawData = "[['" + results[0]["DisplayGraphGroup"] + "','" + results[0]["GraphType"] + "'],";

                            List<int> allValues = new List<int>();
                            rawActualData = string.Empty;

                            foreach (var item2 in results)
                            {
                                var value = lstUsageData.GetType().GetProperty(item2["ColumnName"].ToString()).GetValue(lstUsageData, null);
                                rawActualData += "['" + item2["DisplayName"].ToString() + "', " + Convert.ToInt32(value) + "],";

                                allValues.Add(Convert.ToInt32(value));
                            }

                            if (allValues.Any(any => any != 0))
                            {
                                rawData += rawActualData;
                            }

                            listgraphData.Add((rawData == "[" ? rawData : rawData.Substring(0, rawData.LastIndexOf(","))) + "]");
                        }
                    }
                }
            }
            else
            {
                foreach (var item in lstComponentType)
                {
                    rawData = "";
                    var results = (from myRow in dtMappings.AsEnumerable()
                                   where myRow.Field<string>("DisplayGraphGroup") != "-" && myRow.Field<string>("GraphContentType").Contains(item)
                                   select myRow).ToList();

                    var datafilter = (from m in results
                                      group m by new { DisplayGraphGroup = m["DisplayGraphGroup"], GraphType = m["GraphType"] } into g
                                      where g.Count() > 1
                                      select new { ChartTitle = g.Key.DisplayGraphGroup, ChartType = g.Key.GraphType, Count = g.Count() }).ToList();

                    if (datafilter.Count() > 1)
                    {
                        foreach (var row in datafilter)
                        {
                            rawData = "[['" + row.ChartTitle + "','" + row.ChartType + "'],";
                            var itemlist = (from dt in results.AsEnumerable()
                                            where dt["DisplayGraphGroup"].ToString().Trim() == row.ChartTitle.ToString().Trim()
                                            select dt).ToArray();
                            foreach (var item2 in itemlist)
                            {
                                var value = lstUsageData.GetType().GetProperty(item2["ColumnName"].ToString()).GetValue(lstUsageData, null);
                                rawData += "['" + item2["DisplayName"].ToString() + "', " + Convert.ToInt32(value) + "],";
                            }
                            listgraphData.Add((rawData == "[" ? rawData : rawData.Substring(0, rawData.LastIndexOf(","))) + "]");
                        }
                    }
                    else
                    {
                        rawData = "[['" + results[0]["DisplayGraphGroup"] + "','" + results[0]["GraphType"] + "'],";
                        foreach (var item2 in results)
                        {
                            var value = lstUsageData.GetType().GetProperty(item2["ColumnName"].ToString()).GetValue(lstUsageData, null);
                            rawData += "['" + item2["DisplayName"].ToString() + "', " + Convert.ToInt32(value) + "],";

                        }
                        listgraphData.Add((rawData == "[" ? rawData : rawData.Substring(0, rawData.LastIndexOf(","))) + "]");
                    }
                }
            }           
        
            return listgraphData;
        }
    }
}
