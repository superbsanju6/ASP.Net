using System;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;

namespace Thinkgate.Controls.School
{
    public partial class SchoolIdentification_Edit : BasePage
    {
        private Base.Classes.School _selectedSchool;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            LoadRecordObject();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadSchool();

            if (_selectedSchool == null)
            {
                RedirectToPortalSelectionScreen();
            }

            PopulateProfileLabels();
        }

        private void LoadSchool()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                if (RecordExistsInCache(Key))
                {
                    _selectedSchool = (Base.Classes.School)Base.Classes.Cache.Get(Key);
                }
            }
        }

        protected override String TypeKey
        {
            get { return EntityTypes.School + "_"; }
        }

        protected override object LoadRecord(int xId)
        {
            return Base.Classes.School.GetSchoolByID(DataIntegrity.ConvertToInt(xId));
        }

        private void PopulateProfileLabels()
        {            
            txtName.Text = _selectedSchool.Name;
            txtSchoolID.Text = _selectedSchool.SchoolID;
            txtAbbreviation.Text = _selectedSchool.Abbreviation;
            txtPhone.Text = _selectedSchool.Phone;
            txtType.Text = _selectedSchool.Type;
            txtCluster.Text = _selectedSchool.Cluster;
        }

        protected void btnSaveProfileInfo_Save(object sender, EventArgs e)
        {
        }
    }
}