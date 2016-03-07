using System;
using System.Web.UI;
using ClosedXML.Excel;
using System.Data;
using System.Web;
using Telerik.Web.UI;
using System.Collections.Generic;

namespace Thinkgate.Classes
{
    public class TileControlBase : UserControl
    {
        public SessionObject SessionObject;
        public Tile Tile;
        
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }

            SessionObject = (SessionObject)Session["SessionObject"];
        }

        public virtual List<DockCommand> AdditionalCommands()
        {
            return new List<DockCommand>();
        }

        public void SetParms(TileParms tileParms)
        {
            if (Tile == null) return;

            Tile.TileParms = tileParms;
        }

        public void AddTileCommand(Control ctrl)
        {
            if (Tile == null || Tile.ParentDock == null) return;

            var extraCommandPanel = Tile.ParentDock.TitlebarContainer.FindControl("extraCommandPanel");

            if (extraCommandPanel == null) return;
            extraCommandPanel.Controls.Add(ctrl);
        }

        /// <summary>
        /// Reurn true if the currently logged in user has the passed permission.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        protected Boolean UserHasPermission(Thinkgate.Base.Enums.Permission permission)
        {
            if (SessionObject == null)
            {
                SessionObject = (SessionObject)Session["SessionObject"];
            }

            return SessionObject.LoggedInUser.HasPermission(permission);
        }

        protected string GetControlThatCausedPostBack(System.Web.UI.Page page)
        {
            Control control = GetControlObjThatCausedPostBack(page);
            return (control != null) ? control.ID : null;
        }

        protected Control GetControlObjThatCausedPostBack(System.Web.UI.Page page)
        {
            Control control = null;

            string ctrlname = page.Request.Params["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(ctrlname))
            {
                control = page.FindControl(ctrlname);
            }
            // if __EVENTTARGET is null, the control is a button type and we need to
            // iterate over the form collection to find it
            else
            {
                string ctrlStr = String.Empty;
                Control c = null;
                foreach (string ctl in page.Request.Form)
                {
                    //handle ImageButton they having an additional "quasi-property" in their Id which identifies
                    //mouse x and y coordinates
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        ctrlStr = ctl.Substring(0, ctl.Length - 2);
                        c = page.FindControl(ctrlStr);
                    }
                    else
                    {
                        c = page.FindControl(ctl);
                    }
                    if (c is System.Web.UI.WebControls.Button ||
                        c is System.Web.UI.WebControls.ImageButton)
                    {
                        control = c;
                        break;
                    }
                }
            }
            return control;
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
                            if(sender && sender._postBackSettings && sender._postBackSettings.sourceElement && sender._postBackSettings.sourceElement.id)
                            {
                                // Check to be sure this async postback is actually requesting the file download.
                                if (sender._postBackSettings.sourceElement.id == '" + clientId + "" + @"') {                                    
                                    var iframe = document.createElement('iframe');                                                    
                                    iframe.src = '" + path + @"';                                    
                                    iframe.style.display = 'none';                                    
                                    document.body.appendChild(iframe);                                    
                                }
                            }
                        }
                    </script>";
        }

        public XLWorkbook ConvertDataTableToSingleSheetWorkBook(DataTable dt, string sheet = "")
        {
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
                        workbook.Worksheet(1).Cell(rowCount, colCount).Value = item.ToString();
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

        public virtual void ExportToExcel()
        {
            
        }
    }
}
