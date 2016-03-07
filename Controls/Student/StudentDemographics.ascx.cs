using System;
using System.Reflection;
using CMS.GlobalHelper.UniGraphConfig;
using DocumentFormat.OpenXml.Drawing;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Thinkgate.Core.Extensions;

namespace Thinkgate.Controls.Student
{
    public partial class StudentDemographics : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack) //Comment code:: due to advisement Checklist tile's grade onSelectionChanged page gets load and StudentDemographic data were not bind.
            //{
                LoadData();
            //}
        }

        private void LoadData()
        {
            /* Get our student object from TileParms collection */
            var oStudent = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");

            /* Get the list of demographics used in this district */
            var listDemographics = Demographic.GetListOfDemographics();

            /* Get the list of additional demographics that are to be displayed for this particular client */
            var listAdditionalDemographics = AdditionalDemographics.GetAdditionalDemographicsForStudent(oStudent.ID,
                AdditionalDemographics.AdditionalDemographicClass.Demographics); // Send the tile we are working with as enum

            /* populate Demographic information into table */
            HtmlTableRow oRow;
            HtmlTableCell oCell;

            /* Dynamically build the table to display the demographics.  
             * Use the demographic class's DemoField value and 
             * reflection methods to display correct value in student 
             * object to display.                                        */

            foreach (var oDemog in listDemographics.Select(d1 => new { d1.Label, d1.DemoField }).Distinct().OrderBy(d2 => d2.DemoField))
            {
                var propName = "DemoField" + oDemog.DemoField.ToString();
                if (oStudent.GetType().GetProperty(propName) != null)
                {
                    // New row and cell
                    oRow = new HtmlTableRow();
                    oCell = new HtmlTableCell();
                    oCell.Attributes.Add("class", "fieldLabel");
                    oCell.InnerText = oDemog.Label + ":";
                    oRow.Cells.Add(oCell); // Add cell to row

                    // Create new instance of cell 
                    oCell = new HtmlTableCell();
                    // Set cell's value to display
                    var innerText = oStudent.GetType().GetProperty(propName).GetValue(oStudent, null).ToString();
                    if (propName == "DemoField9" && string.IsNullOrEmpty(innerText))
                        oCell.InnerText = "No";
                    else
                        oCell.InnerText = innerText;


                    // Add the cell to the current row then row to HtmlTable 
                    oRow.Cells.Add(oCell);
                    tblStudentDemographics.Rows.Add(oRow);
                }
            }

            /* US 13893 
             * DATE:03_25_2014 -jdw: Additional Demographics */
            #region Dynamically build the AdditionalDemographics using Name as Label and Value as the data to display

            // Iterate through the list of AdditionalDemographics
            foreach (var additionalDemo in listAdditionalDemographics)
            {
                // New row and cell
                oRow = new HtmlTableRow();
                oCell = new HtmlTableCell();
                oCell.Attributes.Add("class", "fieldLabel"); // Styling
                oCell.InnerText = additionalDemo.Name + ":"; // Set cell contents
                oRow.Cells.Add(oCell); // Add cell to row

                // Create new instance of cell 
                oCell = new HtmlTableCell();
                
                // If SP returns a value that is of 'bit' type (True/False)
                // replace with Yes/No, respectively
                if (additionalDemo.Value.ToLower() == "true") 
                { 
                    additionalDemo.Value = "Yes"; 
                }
                if (additionalDemo.Value.ToLower() == "false")
                {
                    additionalDemo.Value = "No";
                }
                
                // Set cell's value to display
                oCell.InnerText = additionalDemo.Value;

                // Add the cell to the current row then row to HtmlTable 
                oRow.Cells.Add(oCell);
                
                tblStudentDemographics.Rows.Add(oRow);
            }
        }
            #endregion
    }

}

//}