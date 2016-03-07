using System.Linq;
using System.Xml;
using Telerik.Charting.Styles;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;

namespace Thinkgate.Classes
{
    public static class StyleController
    {

        private static object _chartLock = new object();

        public static string GetPieChartColor(string name, int offset)
        {
            var chartConfig = ChartConfig.GetChartConfig();
            string color;
            chartConfig.ColorMap.TryGetValue(name, out color);
            if (string.IsNullOrEmpty(color))
            {
                color = chartConfig.ColorMap.Select(kvp => kvp.Value).ToList()[offset];
            }
            return color;
        }


        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static void SetSkin(this RadChart chart, string chartName)
        {
            var chartConfig = ChartConfig.GetChartConfig();
            string skinName;
            chartConfig.SkinNames.TryGetValue(chartName, out skinName);
            if (!string.IsNullOrEmpty(skinName))
            {
                chart.Skin = skinName;
            }
            
            var customSkin = GetChartConfigFile(chartConfig, chartName);
            if (customSkin != null)
            {
                customSkin.ApplyTo(chart.Chart);
            }
        }


        public static ChartSkin GetChartConfigFile(ChartConfig chartConfig, string source)
        {
            string skinFile;
            chartConfig.SkinFiles.TryGetValue(source, out skinFile);
            if (string.IsNullOrEmpty(skinFile))
            {
                return null;
            }

            var _cacheKey = skinFile;
            var chartSkin = Cache.Get(_cacheKey) as ChartSkin;
            if (chartSkin == null)
            {
                lock (_chartLock)
                {
                    chartSkin = Cache.Get(_cacheKey) as ChartSkin;

                    if (chartSkin == null)
                    {
                        var file = AppSettings.PhysicalAppPath + "//" + skinFile.Replace("/", "//");
                        var doc = new XmlDocument();
                        doc.Load(file);
                        chartSkin = new ChartSkin(skinFile) {XmlSource = doc};

                        Cache.Insert(_cacheKey, chartSkin, new System.Web.Caching.CacheDependency(file));
                    }
                }
            }
            return chartSkin;
        }

        public static XmlDocument LoadCustomSkinXML(string skinFile)
        {
            var skinText = new XmlDocument();
            skinText.Load(AppSettings.PhysicalAppPath + "//" + skinFile.Replace("/", "//"));
            return skinText;
        }


        public static ChartSkin GetChartConfigFile(string source)
        {
            var chartConfig = ChartConfig.GetChartConfig();
            return GetChartConfigFile(chartConfig, source);
        }

        /*public static string GetPieChartColor(string name, int offset)
        {
            var parms = Base.Classes.DistrictParms.LoadDistrictParms();
            
            switch (name)
            {
                case "High School":
                    return parms.GraphColors[0];
                case "Middle":
                    return parms.GraphColors[1];
                case "Elementary":
                    return parms.GraphColors[2];
                case "Multi-Types":
                    return parms.GraphColors[3];
                case "Other":
                    return parms.GraphColors[4];
             
                case "Pre-Kindergarten":
                    return parms.GraphColors[0];
                case "Kindergarten":
                    return parms.GraphColors[4];
                case "1st Grade":
                    return parms.GraphColors[2];
                case "2nd Grade":
                    return parms.GraphColors[3];
                case "3rd Grade":
                    return parms.GraphColors[1];
                case "4th Grade":
                    return parms.GraphColors[5];
                case "5th Grade":
                    return parms.GraphColors[6];
                case "6th Grade":
                    return parms.GraphColors[0];
                case "7th Grade":
                    return parms.GraphColors[3];
                case "8th Grade":
                    return parms.GraphColors[9];
                case "9th Grade":
                    return parms.GraphColors[10];
                case "10th Grade":
                    return parms.GraphColors[11];
                case "11th Grade":
                    return parms.GraphColors[12];
                case "12th Grade":
                    return parms.GraphColors[13];
                
            }

            return parms.GraphColors[0 + offset];
        }*/
    }
}
