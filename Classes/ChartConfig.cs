using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Thinkgate.Base.Classes;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Classes
{
    [Serializable]

    public class ChartConfig
    {
        public Dictionary<String, String> ColorMap;
        public Dictionary<String, String> SkinNames;
        public Dictionary<String, String> SkinFiles;
        
        private static object _chartLock = new object();

        public ChartConfig(XDocument configFile)
        {
            var root = configFile.Element("ChartConfig");
            ColorMap = root.Elements("ColorMap").Elements("Map")
                .Select(sp => new
                {
                    Value = (string)(sp.Attribute("Value")),
                    Color = (string)(sp.Attribute("Color"))
                })
                .ToDictionary(sp => sp.Value, sp => sp.Color);

            SkinFiles = root.Elements("Skins").Elements("Chart")
                .Select(sp => new
                {
                    Name = (string)(sp.Attribute("Name")),
                    SkinFile = (string)(sp.Attribute("CustomSkinFile"))
                })
                .ToDictionary(sp => sp.Name, sp => sp.SkinFile);

            SkinNames = root.Elements("Skins").Elements("Chart")
                .Select(sp => new
                {
                    Name = (string)(sp.Attribute("Name")),
                    SkinName = (string)(sp.Attribute("SkinName"))
                })
                .ToDictionary(sp => sp.Name, sp => sp.SkinName);

            //SkinTexts = new Dictionary<string, XmlDocument>();
        }

        public static ChartConfig GetChartConfig()
        {
            string _cacheKey = "ChartConfig";
            var chartConfig = Cache.Get(_cacheKey) as ChartConfig;
            if (chartConfig == null)
            {
                lock (_chartLock)
                {
                    chartConfig = Cache.Get(_cacheKey) as ChartConfig;
                    if (chartConfig == null)
                    {
                        var parms = Thinkgate.Base.Classes.DistrictParms.LoadDistrictParms();
                        var file = AppSettings.PhysicalAppPath + "\\Config\\" + parms.ChartConfigFile;

                        try
                        {
                            var doc = XDocument.Load(file);
                            chartConfig = new ChartConfig(doc);
                        } catch (System.IO.FileNotFoundException ex)
                        {
                            // hopefully the base config file exists, otherwise, not sure what to do
                            string errorMessage = "Chart config file specified in Parms,  " + file + ", does not exist. Attempting to load base configuration.";

                            ThinkgateEventSource.Log.ApplicationError(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, errorMessage, ex.ToString());
                            var doc = XDocument.Load(AppSettings.PhysicalAppPath + "\\Config\\chart_config1.xml");
                            chartConfig = new ChartConfig(doc);
                        }

                        Cache.Insert(_cacheKey, chartConfig, new System.Web.Caching.CacheDependency(file));
                    }
                }
            }
            return chartConfig;
        }
    }
}
