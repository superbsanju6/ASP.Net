using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;

namespace Thinkgate.Controls
{
     [ToolboxData("<{0}:DynamicTileContainer runat=server></{0}:DynamicTileContainer>")]
    public class DynamicTileContainer : CompositeDataBoundControl
    {
         //public RadRotator Rotator = new RadRotator();
         public Panel Panel = new Panel();

         [Bindable(true), Category("Default"), DefaultValue(""), Description("True if each tile should have a title section.")]
         public bool HasTitle
         {
             get { return DataIntegrity.ConvertToBool(ViewState["DynamicTileContainer_HasTitle"]); }
             set { ViewState["DynamicTileContainer_HasTitle"] = value; }
         }

         [Bindable(true), Category("Default"), DefaultValue(""), Description("The number or rows.")]
         public int Rows
         {
             get { return DataIntegrity.ConvertToInt(ViewState["DynamicTileContainer_Rows"]); }
             set { ViewState["DynamicTileContainer_Rows"] = value; }
         }

         [Bindable(true), Category("Default"), DefaultValue(""), Description("The width of each rotator item.")]
         public int RotatorItemWidth
         {
             get { return DataIntegrity.ConvertToInt(ViewState["DynamicTileContainer_RotatorItemWidth"]); }
             set { ViewState["DynamicTileContainer_RotatorItemWidth"] = value; }
         }

         [Bindable(true), Category("Default"), DefaultValue(""), Description("The height of each rotator item.")]
         public int RotatorItemHeight
         {
             get { return DataIntegrity.ConvertToInt(ViewState["DynamicTileContainer_RotatorItemHeight"]); }
             set { ViewState["DynamicTileContainer_RotatorItemHeight"] = value; }
         }

         [Bindable(true), Category("Default"), DefaultValue(""), Description("The title for each tile.")]
         public string TileTitle
         {
             get { return ViewState["DynamicTileContainer_TileTitle"].ToString(); }
             set { ViewState["DynamicTileContainer_TileTitle"] = value; }
         }

         [Bindable(true), Category("Default"), DefaultValue(""), Description("The html for each tile.")]
         public string ContentTileTemplate
         {
             get { return ViewState["DynamicTileContainer_ContentTileTemplate"].ToString(); }
             set { ViewState["DynamicTileContainer_ContentTileTemplate"] = value; }
         }

         private int PanelItemHeight
         {
             get { return (RotatorItemHeight / Rows) - 5; }
         }

         public DynamicTileContainer()
         {
         }

         public DynamicTileContainer(int rows = 5, int rotatorItemWidth = 160, int rotatorItemHeight = 700, bool hasTitle = true)
         {
             Rows = rows;
             RotatorItemWidth = rotatorItemWidth;
             RotatorItemHeight = rotatorItemHeight;
             HasTitle = hasTitle;
         }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            //Rotator.RenderControl(output);
            Panel.RenderControl(output);
        }

        protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
        {
            var count = 0;

            Panel.Controls.Clear();
            Panel.ID = "tilesPanel";            
            Panel.Height = new System.Web.UI.WebControls.Unit(RotatorItemHeight);
            Panel.Width = new System.Web.UI.WebControls.Unit("100%");
            Panel.ClientIDMode = System.Web.UI.ClientIDMode.Static;            
            Panel.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;             

            /*
            Rotator.Items.Clear();
            Rotator.ID = "tilesRotator";

            Rotator.ItemWidth = new System.Web.UI.WebControls.Unit(RotatorItemWidth);
            Rotator.Height = new System.Web.UI.WebControls.Unit(RotatorItemHeight);
            Rotator.Width = new System.Web.UI.WebControls.Unit("100%");

            Rotator.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            Rotator.RotatorType = RotatorType.Buttons;
            Rotator.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;
            Rotator.ScrollDuration = 500;
            Rotator.WrapFrames = false;            
            */

                        
            if (dataSource != null)
            {
                var e = dataSource.GetEnumerator();
                var addedTable = false;
                var table = new Table();
                table.Attributes.Add("style", "float:left");

                while (e.MoveNext())
                {
                    var datarow = e.Current;
                    
                    if (count % Rows == 0) //End of Row
                    {
                        if (table.Rows.Count > 0)
                        {
                            //var rotatorItem = new RadRotatorItem();
                            //rotatorItem.Controls.Add(table);
                            //Rotator.Items.Add(rotatorItem);
                            Panel.Controls.Add(table);
                            addedTable = true;
                        }
                        
                        //Start a new table
                        table = new Table();
                        table.Attributes.Add("style", "float:left");
                        addedTable = false;
                    }

                    var row = new TableRow();
                    var cell = new TableCell();
                                        
                    cell.Controls.Add(GetContentPanel(datarow, count));

                    row.Cells.Add(cell);
                    table.Rows.Add(row);
                    
                    ++count;
                }
                if (!addedTable) Panel.Controls.Add(table);
                //Controls.Add(Rotator);
                Controls.Add(Panel);
            }
            return count;
        }

        private Panel GetContentPanel(object datarow, int itemNumber)
        {
            var containerPanel = new Panel();

            containerPanel.CssClass = "tileContainerSmall";
            containerPanel.ID = "tileContainerDiv" + itemNumber;
            containerPanel.Width = new System.Web.UI.WebControls.Unit(RotatorItemWidth - 5);
            containerPanel.Height = new System.Web.UI.WebControls.Unit(PanelItemHeight - 5);

            if (HasTitle)
            {
                var titlePanel = new Panel();
                titlePanel.CssClass = "tileTitleSmall";
                titlePanel.ID = "tileTitleDiv" + itemNumber;
                titlePanel.Width = new System.Web.UI.WebControls.Unit(RotatorItemWidth - 15);

                AddContentToPanel(datarow, TileTitle, titlePanel);
                containerPanel.Controls.Add(titlePanel);
            }

            var contentPanel = new Panel();
            contentPanel.CssClass = "tileContentSmall";
            contentPanel.ID="tileContentDiv" + itemNumber;

            AddContentToPanel(datarow, ContentTileTemplate, contentPanel);

            containerPanel.Controls.Add(contentPanel);

            return containerPanel;
        }

        private void AddContentToPanel(object datarow, string template, Panel panel)
        {
            var content = new System.Web.UI.HtmlControls.HtmlGenericControl();
            var row = (System.Data.DataRowView)datarow;

            //Do replacements
            var pos = template.IndexOf("@@");
            var counter = 0;
            while (pos > -1 && counter < 100)
            {
                var column = template.Substring(pos+2, template.IndexOf("@@",pos+2)-(pos+2));

                //Encrypt Column
                if(column.StartsWith("##") && column.EndsWith("##"))
                {
                    column = column.Replace("#", "");                    
                    template = template.Replace("@@##" + column + "##@@",
                                            Standpoint.Core.Classes.Encryption.EncryptString(row[column].ToString()));
                }
                else
                {
                    template = template.Replace("@@" + column + "@@", row[column].ToString());
                }

                counter++;
                pos = template.IndexOf("@@");
            }

            content.InnerHtml = template;
            panel.Controls.Add(content);
        }
    }
}