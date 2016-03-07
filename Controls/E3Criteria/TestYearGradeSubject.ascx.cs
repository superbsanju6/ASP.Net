using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    
    public partial class TestYearGradeSubject : CriteriaBase
    {
        public object JsonDataSource;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            cmbAssessmentType.Required = true;
            cmbYear.Required = true;
            cmbSubject.Required = true;
            cmbGrade.Required = true;
            
            if (!IsPostBack)
            {
                cmbAssessmentType.OnChange = CriteriaName + "Controller.OnChange(criteriaName);";
                cmbYear.OnChange = CriteriaName + "Controller.OnChange(criteriaName);";
                cmbGrade.OnChange = CriteriaName + "Controller.OnChange(criteriaName);";
                
                cmbSubject.OnClientLoad = CriteriaName + "Controller.PopulateControls";

                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);      
            }
        }

        public static ArrayList BuildJsonArray(DataTable testYearGradeSubject)
        {
            var arry = new ArrayList();
            foreach (DataRow c in testYearGradeSubject.Rows)
            {
                arry.Add(new object[] { c["Test_Type"].ToString(), c["Year"].ToString(), c["Grade"].ToString(), c["Subject"].ToString() });
            }
            return arry;

        }

    }
}