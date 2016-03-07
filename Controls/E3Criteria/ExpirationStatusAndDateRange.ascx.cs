using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class ExpirationStatusAndDateRange : CriteriaBase
    {

        public string ContentType {get; set;}

        public DropDownList DDLExpirationStatus
        {
            get { return ddlExpirationStatus; }
        }
        public DateRange DRCopyRightExpiryDate
        {
            get { return drCopyRightExpiryDate; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlExpirationStatus.OnChange = CriteriaName + "Controller.OnChange();";
                ddlExpirationStatus.OnChange = CriteriaName + "Controller.OnChange();";

                ddlExpirationStatus.DataSource = ExpirationStatusList(ContentType);
                ddlExpirationStatus.DefaultTexts = PossibleDefaultTexts("Include Expired "+ContentType);
                
            }
        }
        private static List<NameValue> ExpirationStatusList(string contentType)
        {
            return new List<NameValue>
                {
                    new NameValue("Include Expired "+contentType, "I"),
                    new NameValue("Exclude Expired "+contentType,"E"),
                    new NameValue("Show only Expired "+contentType,"O")
                };
        }
        private List<String> PossibleDefaultTexts(object input)
        {
            if (input == null) return null;
            var list = new List<String>();
            list.AddRange(input.ToString().Split(','));
            return list;
        }
    }
}