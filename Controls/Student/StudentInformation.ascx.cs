using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Web.UI.HtmlControls;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Thinkgate.Base;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Student
{
    public partial class StudentInformation : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var s = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");
            if (s == null) return;

            var i = s.StudentName.IndexOf(',');

            if (i > 0)
                lblStudentName.Text = s.StudentName.Substring(i + 1) + " " + s.StudentName.Substring(0, i);
            else
                lblStudentName.Text = s.StudentName;
            
            /* These are the labels that are hard-coded on the front-side of this control and set here with Demographic data.
             * Keep in mind, they will still show up first until because of there placement on the front-side until we can 
             * get the Demographics displaying dynamically. */// TODO: Make Demographics dynamic as well, like AdditionalDemographics
            lblStudentID.Text = s.StudentID;
            lblSchool.Text = s.SchoolName;
            lblGrade.Text = s.Grade != null ? s.Grade.DisplayText : "";
            if (s.Email.Length > 0) hlStudentEmail.HRef = "mailto:" + s.Email;
            lblEmail.Text = s.Email;

            /* DATE: 03_19_2013    US: 13893 
             * -jdw: Additional Demographics */
            #region JDW:  Dynamically build the AdditionalDemographics using Name as Label and Value as the data to display

            /* Get the list of additional demographics for Identification Tile */
            var listAdditionalDemographics = AdditionalDemographics.GetAdditionalDemographicsForStudent(s.ID,
                AdditionalDemographics.AdditionalDemographicClass.Identification);

            /* Create Html table elements */
            HtmlTableRow oRow;
            HtmlTableCell oCell;

            // Iterate through the list of AdditionalDemographics
            foreach (var additionalDemo in listAdditionalDemographics)
            {
                // New row and cell
                oRow = new HtmlTableRow();
                oCell = new HtmlTableCell();
                oCell.Attributes.Add("class", "fieldLabel"); oCell.Attributes.Add("style", "width: 95px;"); /* Styling being added dynamically
                                                                                                             * This is how Demographics tile works */

                // Change label 'Home Address' so that the address data fits on one line
                if (additionalDemo.Name.ToLower() == "home address")
                    { additionalDemo.Name = "Address"; }
                
                // Set cell contents
                oCell.InnerText = additionalDemo.Name + ":"; 
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

           

            #endregion
        }
           

        #region JDW: Leaving this in case we uncomment the literal labels on front side for any reason
        //// Iterate through and display the information from AdditionalDemographics list
        //foreach (var ad in listAdditionalDemographics) // The labels/fields are put on this control at design time. New data requires new labels/fields
        //{                                              // to be added. We should be building this dynamically like the table on StudentDemographics.aspx.cs
        //    switch (ad.Name)                           
        //    {
        //        case "Home Phone":
        //            lblPhone.Text = ad.Value;
        //            break;
        //        case "Home Address":
        //            lblStudentAddress.Text = ad.Value;
        //            break;
        //        case "Birthdate":
        //            lblBirthDate.Text = ad.Value;
        //            break;
        //    }
        //}
        #endregion


    }


}
