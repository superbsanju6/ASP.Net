using System;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Standpoint.Core.Utilities;
using System.Data;

namespace Thinkgate.ControlHost
{
    public partial class PreviewCurriculums : BasePage
    {
        private Class _selectedClass;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }

            int xID = GetDecryptedEntityId(X_ID);

            if (RecordExistsInCache(Key))
            {
                _selectedClass = (Class)Base.Classes.Cache.Get(Key);
            }
            else
            {
                _selectedClass = Class.GetClassByID(DataIntegrity.ConvertToInt(xID));
            }
            
            DataTable curriculumDataTable = _selectedClass.GetCurriculumDataTable();
            string htmlText = "";
            foreach(DataRow row in curriculumDataTable.Rows)
            {
                htmlText += row["Grade"].ToString() + " " + row["Subject"].ToString() + "-" + row["Course"].ToString() + "<br/>";
            }

            curriculumPreviewDiv.InnerHtml = htmlText;
        }

    }
}