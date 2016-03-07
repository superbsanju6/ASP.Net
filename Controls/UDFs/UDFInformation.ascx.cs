using System;
using Thinkgate.Classes;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using System.Data;

namespace Thinkgate.Controls.UDFs
{
	public partial class UDFInformation : TileControlBase
	{
	    private string _level;
	    private int _levelID;
	    private UDF.Levels _udfLevel;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Tile == null) return;

			_level = Tile.TileParms.GetParm("level").ToString();
		    _levelID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("levelID"));

            if (String.IsNullOrEmpty(_level) || _levelID == 0) return;

            switch(_level)
            {
                case "Student":
                    _udfLevel = UDF.Levels.Students;
                    break;
                case "Class":
                    _udfLevel = UDF.Levels.Classes;
                    break;
                case "Teacher":
                    _udfLevel = UDF.Levels.Teachers;
                    break;
                case "School":
                    _udfLevel = UDF.Levels.Schools;
                    break;
                case "Standards":
                    _udfLevel = UDF.Levels.Standards;
                    break;
            }

		    LoadUDFs();
		}

        public void LoadUDFs()
        {
            var udfList = UDF.GetUDFList(_udfLevel, _levelID);

            if (udfList.Count > 0)
            {
                var udfTable = new HtmlTable();
                udfTable.Attributes["class"] = "fieldValueTable";
                udfTable.Width = "100%";

                foreach (UDF udf in udfList)
                {
                    var tableRow = new HtmlTableRow();
                    var labelCell = new HtmlTableCell();
                    var valueCell = new HtmlTableCell();

                    labelCell.Attributes["class"] = "fieldLabel";
                    labelCell.InnerHtml = udf.Name + ":";

                    valueCell.InnerHtml = udf.Value;

                    tableRow.Cells.Add(labelCell);
                    tableRow.Cells.Add(valueCell);

                    udfTable.Rows.Add(tableRow);

                    udfInformationContainer.Controls.Add(udfTable);
                    udfTable.DataBind();
                }
            }
        }
	}
}