using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.E3Criteria.CriteriaControls
{
    public partial class StandSetList : System.Web.UI.UserControl
    {
        private List<int> _standardIds = new List<int>();
        public List<int> StandardIds { get { return _standardIds; } set { _standardIds = value; } }
        public string StandardAlignment { get; set; }
        public event EventHandler SaveCancelButtonClick;

        protected void Page_Load(object sender, EventArgs e)
        {
            
                StandSetListPanel.Style.Add("display", "none");
           
        }

        public void BuildStandardSetList()
        {
            hidStandardAlignment.Value = StandardAlignment;

            StandardSetTable.Rows.Clear();
            DataTable standardSetDataTable = Base.Classes.Standards.GetStandardsByIDs(StandardIds);
            standardSetDataTable = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(standardSetDataTable, "StandardID", "ID_Encrypted");
            foreach (DataRow standardId in standardSetDataTable.Rows)
            {
                Image imageRemove = new Image
                {
                    ID = "RemoveColumn_" + standardId["StandardID"],
                    ImageUrl = Request.ApplicationPath + "/Images/x.png"
                };

                TableRow tableRow = new TableRow();
                TableCell tableCell = new TableCell();
                 
                tableCell.Controls.Add(imageRemove);
                tableRow.Cells.Add(tableCell);
                tableCell = new TableCell { Text = "<a href='" +
                    Request.ApplicationPath + "/Record/StandardsPage.aspx?xID=" + standardId["ID_Encrypted"] +
                    "' target='_blank'>" + standardId["Standard_Set"] + "." + standardId["StandardName"] + "</a> - " + standardId["Desc"] };
                tableCell.Style.Add("word-wrap", "break-word");
                tableRow.Cells.Add(tableCell);
                tableCell = new TableCell { Text = standardId["StandardID"].ToString() };
                tableCell.Style.Add("display", "none");
                tableRow.Cells.Add(tableCell);
                StandardSetTable.Rows.Add(tableRow);
            }
            StandSetListPanel.Style["display"] = "";
        }

        protected void Save_Button_Clicked(object sender, EventArgs e)
        {
            List<int> standardSetIds = new List<int>();
            if (hidStandardSetSelected.Value.Length > 0)
            {
                string[] idStrings = hidStandardSetSelected.Value.Split(',');

                foreach (string idString in idStrings)
                {
                    standardSetIds.Add(Convert.ToInt32(idString));
                }
            }
            StandardIds = standardSetIds;
            StandardAlignment = hidStandardAlignment.Value;

            if (SaveCancelButtonClick != null) SaveCancelButtonClick(this, e);
        }
        protected void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            if (SaveCancelButtonClick != null) SaveCancelButtonClick(this, e);
        }
    }
}