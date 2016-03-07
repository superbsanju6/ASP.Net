using System;
using System.Data;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using ClosedXML.Excel;
using System.Linq;
namespace Thinkgate.Classes
{
    public class ExpandedSearchPage : BasePage
    {
        protected new void Page_Init(object sender, EventArgs e)
        {
            //Empty Page_Init function to prevent the BasePage Page_Init from getting called 
            //for ExpandedPages
        }

        public void LoadResults(
            DataTable dataSource,
            Panel panel,
            HtmlGenericControl div,
            Panel tileResultsPanel,
            Panel gridResultsPanel,
            RadGrid grid,
            string resultType,
            string contentTemplate)
        {
            if (resultType == "Grid")
            {
                grid.DataSource = dataSource;
                grid.DataBind();

                tileResultsPanel.Visible = false;
                gridResultsPanel.Visible = true;
            }
            else
            {
                var resultsControl = new Controls.DynamicTileContainer(4, 113, 525, false);
                resultsControl.DataSource = null;
                resultsControl.ContentTileTemplate = contentTemplate;
                resultsControl.DataSource = dataSource;
                resultsControl.DataBind();

                tileResultsPanel.Visible = true;
                gridResultsPanel.Visible = false;

                div.Controls.Clear();
                div.Controls.Add(resultsControl);

                AddNavigationButtons(panel, div);
            }
        }

        public void AddNavigationButtons(Panel panel, HtmlGenericControl div)
        {
            try
            {
                CreateRotatorNavigationButtons(panel, div, 1);
            }
            catch (Exception)
            {
            }
        }

        private void CreateRotatorNavigationButtons(Panel panel, HtmlGenericControl div, int divIndex)
        {
            panel.Controls.Clear();

            double pages = div.Controls[0].Controls[0].Controls.Count / 6.0; //6 is the number of columns
            pages = Math.Ceiling(pages);

            div.Style["Width"] = (pages * 738) + "px";

            if (pages > 1)
            {
                LinkButton linkButton = CreatePreviousButton(divIndex);
                panel.Controls.Add(linkButton);
            }

            for (int i = 1; i <= pages; i++)
            {
                LinkButton linkButton = CreateLinkButton(i, divIndex);
                panel.Controls.Add(linkButton);
            }

            if (pages > 1)
            {
                LinkButton linkButton = CreateNextButton(divIndex);
                panel.Controls.Add(linkButton);
            }
        }

        protected LinkButton CreatePreviousButton(int divIndex)
        {
            // Create the LinkButton
            LinkButton button = new LinkButton();
            button.Text = "<";
            button.ID = string.Format("PrevButton{0}", divIndex);
            button.OnClientClick = string.Format("leftArrowClick('{0}'); return false;", divIndex);

            // Class which is applied to the newly added button
            button.CssClass = "rotatorPageButton";
            return button;
        }

        protected LinkButton CreateNextButton(int divIndex)
        {
            // Create the LinkButton
            LinkButton button = new LinkButton();
            button.Text = ">";
            button.ID = string.Format("NextButton{0}", divIndex);
            button.OnClientClick = string.Format("rightArrowClick('{0}'); return false;", divIndex);

            // Class which is applied to the newly added button
            button.CssClass = "rotatorPageButton";
            return button;
        }

        protected LinkButton CreateLinkButton(int itemIndex, int divIndex)
        {
            // Create the LinkButton
            LinkButton button = new LinkButton();
            button.Text = itemIndex.ToString();// The test of the button
            button.ID = string.Format("Button{0}_{1}", divIndex, itemIndex);// Assign an unique ID
            button.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            button.OnClientClick = string.Format("DoubleScrollPanel_JumpToPage({0},'{1}'); return false;", itemIndex, divIndex);

            // Class which is applied to the newly added button
            button.CssClass = "rotatorPageButton";
            return button;
        }

        public static string BuildStartupScript(string clientId, string path = "", string guid = "")
        {
            path = string.IsNullOrEmpty(path) ? HttpContext.Current.Request.ApplicationPath : path;
            if (path.Substring(path.Length - 1, 1) != "/") path = path + "/";
            path += "FileExport.aspx?sessionID=" + guid;
            return @"<script type='text/javascript' language='javascript'>
                        var prm = Sys.WebForms.PageRequestManager.getInstance();
                        prm.add_pageLoading(PageLoading);

                        function PageLoading(sender, args) {
                            
                            // Check to be sure this async postback is actually requesting the file download.
                            if (sender._postBackSettings.sourceElement.id == '" + clientId + "" + @"') {
                                
                                var iframe = document.createElement('iframe');
                                iframe.src = '" + path + @"';
                                iframe.style.display = 'none';
                                document.body.appendChild(iframe);
                            }
                        }
                    </script>";
        }

        public Boolean ifLEAListContainsLEA(string LEAList, string LEA)
        {
            return (LEAList.Split('|').Where(c => c.Trim() == LEA)).Count() > 0;
        }

        public Boolean ifLEAListContainsLEAsInState(string LEAList, string State)
        {
            return (LEAList.Split('|').Where(c => c.Trim().StartsWith(State))).Count() > 0;
        }

        public XLWorkbook ConvertDataTableToSingleSheetWorkBook(DataTable dt, string sheet = "")
        {
            dt = RemoveNotAllowedColumnToExcel(dt);
            XLWorkbook workbook = new XLWorkbook();

            if (dt.Rows.Count > 0)
            {
                workbook.Worksheets.Add(sheet ?? "Sheet1");

                int colCount;
                //Write second rows first so that we can calculate width of for the headers
                var rowCount = 2;
                foreach (DataRow row in dt.Rows) // Loop over the rows.
                {
                    colCount = 1;
                    foreach (var item in row.ItemArray) // Loop over the items.
                    {
                        workbook.Worksheet(1).Cell(rowCount, colCount).RichText.AddText(item.ToString());
                        colCount++;
                    }

                    rowCount++;
                }

                colCount = 1; //reset columns
                foreach (DataColumn column in dt.Columns)
                {
                    workbook.Worksheet(1).Cell(1, colCount).Value = column.ColumnName;
                    workbook.Worksheet(1).Cell(1, colCount).Style.Font.SetBold();
                    workbook.Worksheet(1).Column(colCount).AdjustToContents();
                    colCount++;
                }


                return workbook;
            }
            else
            {
                return null;
            }
        }

        public DataTable RemoveNotAllowedColumnToExcel(DataTable dataTable)
        {
            string[] notAllowedColumnToExcel =
            {
                "Targetted",
                "Targeted",
                "ItemClassId",
                "DisplayDashboard",
                "EncryptedID",
                "disablecheckbox",
                "EncryptedTestID",
                "TestEventID",
                "ClassID",
                "ItemClassID",
                "SchoolID",
                "IsGroup",
                "CourseID",
                "ID",
                "UserPage"
            };

            foreach (var notAllowedColumnName in notAllowedColumnToExcel)
            {
                if (dataTable.Columns.Contains(notAllowedColumnName))
                {
                    dataTable.Columns.Remove(notAllowedColumnName);
                }
            }
            return dataTable;
        }

    }
}
