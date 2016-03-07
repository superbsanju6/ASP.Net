using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using System.Collections;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    /// <summary>
    /// Custom dependent control for Grade, Subject, Course & Standard Set
    /// </summary>
    public partial class RegionSchoolTypeSchool : CriteriaBase
    {
        public object JsonDataSource;
        private SessionObject _sessionObject;

        public DropDownList CmbSchoolType
        {
            get { return cmbSchoolType; }
        }

        public DropDownList CmbSchool
        {
            get { return cmbSchool; }
        }

		public bool ShowStandardLevels = false;

        protected new void Page_Init(object sender, EventArgs e)
        {
            if (_sessionObject == null)
            {
                _sessionObject = (SessionObject)Session["SessionObject"];
            }
            #region Criterion: Cluster
            DataTable dtCluster = Thinkgate.Base.Classes.SchoolMasterList.GetClustersForUser(_sessionObject.LoggedInUser).ToDataTable("Cluster");
            cmbRegion.DataSource = dtCluster;
            cmbRegion.DataTextField = "Cluster";
            cmbRegion.DataValueField = "Cluster";
            cmbRegion.DataBind();
            
            if (dtCluster.Rows.Count == 1)
            {
                string Region = dtCluster.Rows[0]["Cluster"].ToString();
                cmbRegion.DefaultTexts = new List<string> { Region };
                cmbRegion.ReadOnly = true;
            }
            #endregion

            #region Pre Selected Loggedin User Name

            //RadComboBoxItem item = new RadComboBoxItem();
            //item.Text = _sessionObject.LoggedInUser.UserFullName.ToString();
            //item.Value = _sessionObject.LoggedInUser.UserFullName.ToString();
            //cmbUserName.ComboBox.Items.Add(item);
            //cmbUserName.DefaultTexts = new List<string> { item.Text };
            //cmbUserName.ReadOnly = true;
            #endregion

        }

        public ArrayList BuildJsonArray(List<Base.Classes.School> schools)
        {
            var arry = new ArrayList();
            foreach (var c in schools)
            {
                arry.Add(new object[]
                                 {
                                     c.Type, c.Name
                                 });
            }
            return arry;
        }
        private List<String> PossibleDefaultTexts(object input)
        {
            if (input == null) return null;
            var list = new List<String>();
            list.AddRange(input.ToString().Split(','));
            return list;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cmbRegion.OnChange = CriteriaName + "Controller.OnRegionChange();";
                CmbSchoolType.OnChange = CriteriaName + "Controller.OnSchoolTypeChange();";
                CmbSchool.OnChange = CriteriaName + "Controller.OnSchoolChange();";
                CmbSchool.OnClientLoad = CriteriaName + "Controller.PopulateSchoolTypeControls";
                ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);
            }
        }

    }
}