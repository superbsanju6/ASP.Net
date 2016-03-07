using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;

namespace Thinkgate.Classes
{
    public class SearchTileControlBase : TileControlBase
    {
        public void LoadResults(DataTable dataSource, Panel panel, 
                                 HtmlGenericControl div, 
                                 Panel tileResultsPanel,
                                 Panel gridResultsPanel,
                                 RadGrid grid,
                                 string resultType, string contentTemplate)
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
    }
}