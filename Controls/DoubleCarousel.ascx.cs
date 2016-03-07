using System.Web.UI;
using System;
using Thinkgate.Classes;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Thinkgate.Controls
{
    public partial class DoubleCarousel : System.Web.UI.UserControl, Interfaces.IRotatorControl
    {        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void ChangeCarouselTwoVisibility(bool visible)
        {
            separatorRow.Visible = visible;
            buttonsContainer2.Visible = visible;
            prevButton2.Visible = visible;
            nextButton2.Visible = visible;
            tilesRotator2.Visible = visible;

            tilesRotator1.Height = visible ? Unit.Pixel(280) : Unit.Pixel(572);
        }

        public void ClearRotators()
        {
            tilesRotator1.Controls.Clear();
            tilesRotator1.Items.Clear();
            tilesRotator2.Controls.Clear();
            tilesRotator2.Items.Clear();
        }
    
        public Control GetRotator1()
        {
            return tilesRotator1;
        }

        public Control GetRotator2()
        {
            return tilesRotator2;
        }
    
        public Panel GetButtonsContainer1()
        {
            return buttonsContainer1;           
        }

        public Panel GetButtonsContainer2()
        {
            return buttonsContainer2;
        }
    
        public void AddItemToRotator1(Container container)
        {
            var item = new RadRotatorItem();
            item.Controls.Add(container);
            tilesRotator1.Items.Add(item);
        }

        public void AddItemToRotator2(Container container)
        {
            var item = new RadRotatorItem();
            item.Controls.Add(container);
            tilesRotator2.Items.Add(item);
        }

        public void AddNavigationButtons(Folder folder = null)
        {
            CreateRotatorNavigationButtons(buttonsContainer1, tilesRotator1);
            CreateRotatorNavigationButtons(buttonsContainer2, tilesRotator2);
        }

        private void CreateRotatorNavigationButtons(Panel panel, RadRotator rotator)
        {
            panel.Controls.Clear();

            foreach (RadRotatorItem item in rotator.Items)
            {
                LinkButton linkButton = CreateLinkButton(item.Index, rotator);
                panel.Controls.Add(linkButton);
            }
        }

        protected LinkButton CreateLinkButton(int itemIndex, RadRotator rotator)
        {
            // Create the LinkButton
            LinkButton button = new LinkButton();
            button.Text = (itemIndex + 1).ToString();// The test of the button
            button.ID = string.Format("Button{0}_{1}", rotator.ClientID, itemIndex);// Assign an unique ID
            button.OnClientClick = string.Format("showItemByIndex({0},'{1}'); return false;", itemIndex, rotator.ClientID);

            // Class which is applied to the newly added button
            button.CssClass = "rotatorPageButton";
            return button;
        }
    }
}