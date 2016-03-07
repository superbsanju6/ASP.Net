using System;
using System.Data;
using System.Web.UI;
using Thinkgate.Classes;

namespace Thinkgate.Charts
{
    public partial class BarChart : UserControl
    {
        public DataTable Data;
        public string ChartGuid = Guid.NewGuid().ToString().Replace("-", "");
        public bool ShowLegend;
        public int Width { get; set; }
        public int Height { get; set; }
        public string HorizontalHeader { get; set; } //typically y-axis
        public string VerticalHeader { get; set; } //typically x-axis
        public string ChartTitle { get; set; }
        public string BarHexColor { get; set; }
        public int GridLines { get; set; }
        public string Title { get; set; }


        protected void Page_Init(object sender, EventArgs e)
        {            
            var div = new System.Web.UI.HtmlControls.HtmlGenericControl("div") {ID = "barChart" + ChartGuid};
            phDiv.Controls.Clear();
            phDiv.Controls.Add(div);

            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data == null) return;
            
            GenerateJavascript();
        }


        private void GenerateJavascript()
        {
            //string js = "google.setOnLoadCallback(drawChart"+ChartGuid+");";
            string js = string.Empty;

            js += " function drawChart" + ChartGuid + "() {";
            js += " var data = new google.visualization.DataTable();";
            js += " data.addColumn('string', '" + VerticalHeader + "');";
            js += " data.addColumn('number', '" + HorizontalHeader + "');";
            js += " data.addRows([";
            int i = 0;
                foreach (DataRow row in Data.Rows)
                {
                    if (i > 0)
                        js += ",";

                    js += "['" + row[VerticalHeader] + " (" + row[HorizontalHeader] + ")', " + row[HorizontalHeader] + "]";
                    i++;
                }
            js += " ]); ";

            js += "var options = {";
            js += "backgroundColor: {fill: 'transparent'},";
            js += !string.IsNullOrEmpty(BarHexColor)
                ?  "series: [{color: '" + BarHexColor + "'}],"
                : "";
            js += "title: '" + (String.IsNullOrEmpty(ChartTitle) ? "" : ChartTitle ) + "',";
            js += "vAxis: {title: '', textPosition: 'in',textStyle:{fontSize: 12, fontName: 'Arial Black'} },";
            js += "chartArea: {left: 5, width:'100%'},";
            js += "hAxis: {gridlines: {count: " + (GridLines > 0 ? GridLines.ToString() : "10") + "}, title: '" + Title + "'},";
                if (Width > 0)
                {
                    js += "width: "+Width+",";
                }
            if (Height > 0)
                {
                    js += "height: "+Height+",";
                }
            js += "legend: {position: '"+ (ShowLegend ? "right" : "none")  +"'}";
            js += "};";

            js += " var chart = new google.visualization.BarChart(document.getElementById('barChart"+ChartGuid+"'));";
            js += " chart.draw(data, options);";

            js += "}";

            js += "if (document.getElementById('barChart" + ChartGuid + "')) drawChart" + ChartGuid + "();";
            //var literal = new LiteralControl("<script type='text/javascript'>" + js + "</script>");
            //phScript.Controls.Clear();
            //phScript.Controls.Add(literal);
            ScriptManager.RegisterStartupScript(Page, typeof (Page), ChartGuid, js, true);

        }
    }
}