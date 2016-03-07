using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Telerik.Web.UI;
using ClosedXML.Excel;

using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;

namespace Thinkgate.Controls.Items
{
    public partial class Items : TileControlBase
    {
        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            bool _isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");

            if (Session["SessionObject"] == null)
            { Services.Service2.KillSession(); }

            SessionObject = (SessionObject)Session["SessionObject"];
            var user = SessionObject.LoggedInUser;
            if (user == null) return;

            if (Tile == null) return;

            if (string.IsNullOrEmpty(Tile.Title)) return;

            if (!_isPostBack)
            {
                BindItemBanks();
                LoadControls();
            }

            LoadSummaryContent();
        }

        #endregion

        #region Protected Events

        protected void cmbItembank_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value))
            {
                //standardInitialText.Visible = true;
                rigorInitialText.Visible = true;

                //standardDistributionView.Visible = false;
                rigorDistributionView.Visible = false;
            }
            else
            {
                string taxonomy = DistrictParms.LoadDistrictParms().DOK;
                //LoadStandardDistribution(e.Value);
                LoadRigorDistribution(e.Value, taxonomy);

                //standardInitialText.Visible = false;
                rigorInitialText.Visible = false;

                //standardDistributionView.Visible = true;
                rigorDistributionView.Visible = true;
            }
        }

        //protected void Items_ItemDataBound(object sender, GridItemEventArgs e)
        //{
        //    if (e.Item is GridDataItem)
        //    {
        //        Label standard = (Label)e.Item.FindControl("lblStandard");
        //        if (standard.Text.Length > 15)
        //        { standard.Text = standard.Text.Substring(0, 15) + "..."; }
        //    }
        //}

        protected void exportToExcelIcon_Click(object sender, EventArgs e)
        {
            dtItemBank itemBanks = (dtItemBank)ViewState["dtItemBank"];

            DataSet dataSet = new DataSet();
            DataTable dtSummary = new DataTable();
            // DataTable dtStandard = new DataTable();
            DataTable dtRigor = new DataTable();

            var districtParms = DistrictParms.LoadDistrictParms();
            string clientName = districtParms.ClientName;
            string taxonomy = districtParms.DOK;

            string itemBank = cmbItembank.SelectedItem == null ? string.Empty : cmbItembank.SelectedItem.Text;
            string target = cmbItembank.SelectedItem == null ? string.Empty : cmbItembank.SelectedValue;
            string countAsof = lblCountAsof.Text;

            dtSummary = Thinkgate.Base.Classes.Items.GetSummaryData(itemBanks);
            dataSet.Tables.Add(dtSummary);

            if (!string.IsNullOrEmpty(target))
            {
                //dtStandard = Thinkgate.Base.Classes.Items.GetStandardDistData(target);
                dtRigor = Thinkgate.Base.Classes.Items.GetRigorData(target, taxonomy);
            }
            //dataSet.Tables.Add(dtStandard);
            dataSet.Tables.Add(dtRigor);

            XLWorkbook workbook = new XLWorkbook();

            List<string> sheetnames = new List<string>();
            sheetnames.Add("Item Bank Counts");
            //sheetnames.Add("Item Standard Distribution");
            sheetnames.Add("Item Rigor Distribution");

            List<string> fields = new List<string>();
            fields.Add("LEA:");
            fields.Add("Item Bank:");
            fields.Add("Content:");
            fields.Add("Counts as of:");
            fields.Add("Total items:");

            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                List<string> fieldsValues = new List<string>();
                fieldsValues.Add(clientName);

                if (i == 0)
                { fieldsValues.Add("All"); }
                else if (string.IsNullOrEmpty(itemBank))
                { fieldsValues.Add("No Item Bank Selected"); }
                else
                { fieldsValues.Add(itemBank); }

                fieldsValues.Add(sheetnames[i]);
                fieldsValues.Add("'" + countAsof);

                DataTable dataTable = dataSet.Tables[i];
                var worksheet = workbook.Worksheets.Add(sheetnames[i]);
                int rowCount = 7;
                int colCount = 1;
                Int64 totalItems = 0;

                if (!string.IsNullOrEmpty(itemBank) || i == 0)
                {
                    switch (i)
                    {
                        case 0:
                            worksheet.Cell(6, 1).Value = "Item Bank";
                            worksheet.Cell(6, 2).Value = "Items";
                            foreach (DataRow dataRow in dataTable.Rows)
                            {
                                if (Convert.ToString(dataRow["Category"]).ToUpper() == "LOCAL")
                                {
                                    worksheet.Cell(rowCount, 1).Value = dataRow["ItemBank"];
                                    worksheet.Cell(rowCount, 2).Value = dataRow["Distribution"];
                                    totalItems += Convert.ToInt64(dataRow["Distribution"]);
                                    rowCount++;
                                }
                            }
                            foreach (DataRow dataRow in dataTable.Rows)
                            {
                                if (Convert.ToString(dataRow["Category"]).ToUpper() != "LOCAL")
                                {
                                    worksheet.Cell(rowCount, 1).Value = dataRow["ItemBank"];
                                    worksheet.Cell(rowCount, 2).Value = dataRow["Distribution"];
                                    totalItems += Convert.ToInt64(dataRow["Distribution"]);
                                    rowCount++;
                                }
                            }
                            colCount = 2;
                            break;
                        case 1:
                            //    worksheet.Cell(6, 1).Value = "Standard";
                            //    worksheet.Cell(6, 2).Value = "% of Item Bank";
                            //    worksheet.Cell(6, 3).Value = "Items";
                            //    foreach (DataRow dataRow in dataTable.Rows)
                            //    {
                            //        worksheet.Cell(rowCount, 1).Value = dataRow["StandardName"];
                            //        worksheet.Cell(rowCount, 2).Value = dataRow["PctItemBank"] + "%";
                            //        worksheet.Cell(rowCount, 3).Value = dataRow["StandardItems"];
                            //        totalItems = Convert.ToInt64(dataRow["BankItems"]);
                            //        rowCount++;
                            //    }
                            //    colCount = 3;
                            //    worksheet.Range(7, 2, rowCount - 1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            //    break;

                            //case 2:
                            worksheet.Cell(6, 1).Value = "Distribution";
                            worksheet.Cell(6, 2).Value = taxonomy;
                            worksheet.Cell(6, 3).Value = "MC3";
                            worksheet.Cell(6, 4).Value = "MC4";
                            worksheet.Cell(6, 5).Value = "MC5";
                            worksheet.Cell(6, 6).Value = "S/A";
                            worksheet.Cell(6, 7).Value = "Essay";
                            worksheet.Cell(6, 8).Value = "T/F";
                            foreach (DataRow dataRow in dataTable.Rows)
                            {
                                worksheet.Cell(rowCount, 1).Value = Convert.ToInt64(dataRow["Distribution"]) == 0 ? "" : dataRow["Distribution"];
                                worksheet.Cell(rowCount, 2).Value = dataRow["TaxonomyDimension"];
                                worksheet.Cell(rowCount, 3).Value = dataRow["MC3"] == null ? "" : dataRow["MC3"];
                                worksheet.Cell(rowCount, 4).Value = dataRow["MC4"] == null ? "" : dataRow["MC4"];
                                worksheet.Cell(rowCount, 5).Value = dataRow["MC5"] == null ? "" : dataRow["MC5"];
                                worksheet.Cell(rowCount, 6).Value = dataRow["ShortAns"] == null ? "" : dataRow["ShortAns"];
                                worksheet.Cell(rowCount, 7).Value = dataRow["Essay"] == null ? "" : dataRow["Essay"];
                                worksheet.Cell(rowCount, 8).Value = dataRow["TF"] == null ? "" : dataRow["TF"];
                                totalItems += Convert.ToInt64(dataRow["Distribution"]);
                                rowCount++;
                            }
                            colCount = 8;
                            break;
                    }
                    worksheet.Row(6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(6, 1, rowCount - 1, colCount).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(6, 1, rowCount - 1, colCount).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                for (int j = 1; j <= 5; j++)
                {
                    worksheet.Cell(j, 1).Value = fields[j - 1];
                    worksheet.Cell(j, 1).Style.Font.SetBold();
                    worksheet.Cell(j, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                for (int j = 1; j <= colCount; j++)
                { worksheet.Column(j).AdjustToContents(); }

                fieldsValues.Add(totalItems.ToString());

                for (int j = 1; j <= 5; j++)
                {
                    worksheet.Cell(j, 2).Value = fieldsValues[j - 1];
                    worksheet.Cell(j, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                Response.Clear();
                Response.Buffer = true;
                workbook.SaveAs(memoryStream);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");
                Response.BinaryWrite(memoryStream.ToArray());
                Response.Flush();
                Response.End();
            }
        }

        #endregion

        #region Private Methods

        private void LoadControls()
        {
            btnAdd.Visible = (UserHasPermission(Base.Enums.Permission.Create_Item));
            btnAdd.Attributes["onclick"] = "addNewItem('Item');";

            var extractedexpand =
                string.IsNullOrEmpty(Tile.ExpandJSFunctionOverride) ?
                    BuildCustomDialogPath(Tile.ExpandedControlPath, "SearchItem" + "_" + Tile.Title, 950, 675)
                : Tile.ExpandJSFunctionOverride;
        }

        private void BindItemBanks()
        {
            dtItemBank itemBanks = ItemBankMasterList.GetItemBanksForStandardSearch(SessionObject.LoggedInUser, null);
            DataTable dataTable = Thinkgate.Base.Classes.Items.GetItemBanks(itemBanks);

            cmbItembank.DataTextField = "ItemBank";
            cmbItembank.DataValueField = "Target";
            cmbItembank.DataSource = dataTable;
            cmbItembank.DataBind();

            ViewState["dtItemBank"] = itemBanks;
        }

        private void LoadSummaryContent()
        {
            dtItemBank itemBanks = (dtItemBank)ViewState["dtItemBank"];
            DataTable dtSummary = Thinkgate.Base.Classes.Items.GetSummaryData(itemBanks);

            // return, if dtSummary is null
            if (dtSummary == null) return;

            // Show 'No Items', if 'local' & '3rd Party' ItemBank has no data.
            if (dtSummary.Rows.Count == 0)
            {
                var noItemsDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                noItemsDiv.Style.Add("width", "311px");
                noItemsDiv.Style.Add("height", "184px");
                noItemsDiv.InnerText = "No items found.";
                summaryNotFound.Controls.Add(noItemsDiv);
                barChartLocal.Visible = false;
                barChartThirdParty.Visible = false;
            }
            else
            {
                // Set 'Counts as of'
                lblCountAsof.Text = ((DateTime)(dtSummary.Rows[0]["Asof"])).ToString("M/d/yyyy");

                // Create datasource for 'local' ItemBank.
                var localData = new DataTable();
                localData.Columns.Add("ItemBank");
                localData.Columns.Add("Distribution");

                // Create datasource for '3rd Party' ItemBank.
                var thirdPartyData = new DataTable();
                thirdPartyData.Columns.Add("ItemBank");
                thirdPartyData.Columns.Add("Distribution");

                // Loop through rows seperating out 3rd party and local data.
                foreach (DataRow dataRow in dtSummary.Rows)
                {
                    string category = Convert.ToString(dataRow["Category"]).ToUpper();

                    if (category == "LOCAL")
                    { localData.Rows.Add(dataRow["ItemBank"], dataRow["Distribution"]); }
                    else
                    { thirdPartyData.Rows.Add(dataRow["ItemBank"], dataRow["Distribution"]); }
                }

                // Show barChartLocal, if 'local' ItemBank has data.
                if (localData.Rows.Count > 0)
                {
                    barChartLocal.Data = localData;
                    barChartLocal.Height = thirdPartyData.Rows.Count > 0 ? 90 : 180;
                    barChartLocal.VerticalHeader = "ItemBank";
                    barChartLocal.HorizontalHeader = "Distribution";
                    barChartLocal.ShowLegend = false;
                    barChartLocal.Visible = true;
                }

                // Show barChartThirdParty, if '3rd Party' ItemBank has data.
                if (thirdPartyData.Rows.Count > 0)
                {
                    barChartThirdParty.Data = thirdPartyData;
                    barChartThirdParty.Height = localData.Rows.Count > 0 ? 90 : 180;
                    barChartThirdParty.VerticalHeader = "ItemBank";
                    barChartThirdParty.HorizontalHeader = "Distribution";
                    barChartThirdParty.ShowLegend = false;
                    barChartThirdParty.Visible = true;
                }
            }
        }

        //private void LoadStandardDistribution(string target)
        //{
        //    DataTable dtStandardDist = new DataTable();
        //    dtStandardDist = Thinkgate.Base.Classes.Items.GetStandardDistData(target);

        //    // Bind standard distribution
        //    radGridStandard.DataSource = dtStandardDist;
        //    radGridStandard.DataBind();
        //}

        private void LoadRigorDistribution(string target, string taxonomy)
        {
            DataTable dtRigorDist = new DataTable();
            dtRigorDist = Thinkgate.Base.Classes.Items.GetRigorData(target, taxonomy);

            // Build rigor dstribution content
            StringBuilder tableContent = new StringBuilder();
            Int32 mc3, mc4, mc5, sa, essay, tf, distributionCount = 0;
            Int64 totalCount = 0;
            foreach (DataRow dataRow in dtRigorDist.Rows)
            {
                totalCount += Convert.ToInt64(dataRow["Distribution"]);

                distributionCount = dataRow["Distribution"] == DBNull.Value ? 0 : (Int32)dataRow["Distribution"];
                mc3 = dataRow["MC3"] == DBNull.Value ? 0 : (Int32)dataRow["MC3"];
                mc4 = dataRow["MC4"] == DBNull.Value ? 0 : (Int32)dataRow["MC4"];
                mc5 = dataRow["MC5"] == DBNull.Value ? 0 : (Int32)dataRow["MC5"];
                sa = dataRow["ShortAns"] == DBNull.Value ? 0 : (Int32)dataRow["ShortAns"];
                essay = dataRow["Essay"] == DBNull.Value ? 0 : (Int32)dataRow["Essay"];
                tf = dataRow["TF"] == DBNull.Value ? 0 : (Int32)dataRow["TF"];

                tableContent.AppendFormat(@"<tr>
									<td class='cellCommon cellContent'>{0}</td>
									<td class='cellCommon cellContentLeft'>{13}</td>
									<td class='cellCommon {1}'>{2}</td>
									<td class='cellCommon {3}'>{4}</td>
									<td class='cellCommon {5}'>{6}</td>
									<td class='cellCommon {7}'>{8}</td>
                                    <td class='cellCommon {9}'>{10}</td>
                                    <td class='cellCommon {11}'>{12}</td>
							</tr>",
                            Content(distributionCount),
                            ContentStyle(mc3), Content(mc3),
                            ContentStyle(mc4), Content(mc4),
                            ContentStyle(mc5), Content(mc5),
                            ContentStyle(sa), Content(sa),
                            ContentStyle(essay), Content(essay),
                            ContentStyle(tf), Content(tf),
                            dataRow["TaxonomyDimension"].ToString());
            }

            // Start rigor distribution table
            StringBuilder table = new StringBuilder();
            table.Append(@"<table border='1' style=cellpadding:0;' width='100%'>");

            // Build top header row.
            table.AppendFormat(@"<tr>
								    <th class='cellCommon cellHeader'>Distribution</th>
								    <th class='cellCommon cellHeader'>{0}</th>
								    <th class='cellCommon cellHeader' colspan='6'>Item Criteria Summary</th>
							    </tr>", taxonomy);

            // Build second header row.
            table.AppendFormat(@"<tr>
								    <td class='cellCommon cellContent'>{0}</td>
								    <td class='cellCommon cellContent'></td>
								    <th class='cellCommon cellHeader cellHover'>MC3</th>
                                    <th class='cellCommon cellHeader cellHover'>MC4</th>
                                    <th class='cellCommon cellHeader cellHover'>MC5</th>
								    <th class='cellCommon cellHeader cellHover'>S/A</th>
								    <th class='cellCommon cellHeader cellHover'>Essay</th>
								    <th class='cellCommon cellHeader cellHover'>T/F</th>
							    </tr>", totalCount);

            // Append table content.
            table.Append(tableContent);

            // End table
            table.Append(@"</table>");

            tableRigor.Text = table.ToString();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), this.ClientID + "_bindtooltip", "bindToolTip();", true);
        }

        #endregion

        #region Public methods

        public string BuildCustomDialogPath(string path, string name, int? width = null, int? height = null)
        {
            var sizeblock = "autoSize: true";
            if (width != null && height != null)
            {

                sizeblock = "width: " + width + ", height: " + height;
            }

            return "customDialog({ url: ('" + path + "')," + sizeblock + " , name: '" + name + "' });";
        }

        public String ContentStyle(Int32 count)
        {
            return (count > 0) ? "cellContent" : "";
        }

        public String Content(Int32 count)
        {
            return (count > 0) ? count.ToString() : "";
        }

        #endregion
    }
}