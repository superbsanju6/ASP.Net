using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Thinkgate.Classes;

namespace Thinkgate.Controls
{
    public partial class DoubleScrollPanel : System.Web.UI.UserControl, Interfaces.IRotatorControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            tileScrollDiv1_postBackPage.Value = tileScrollDiv1_currentPage.Value;
            tileScrollDiv2_postBackPage.Value = tileScrollDiv2_currentPage.Value;
        }

        public void ChangeCarouselTwoVisibility(bool visible)
        {
            //secondPagingContents.Visible = visible;
            //tileDivWrapper1.Style["Height"] = visible ? "290px" : "665px";
            //tileDiv1.Style["Height"] = visible ? "307px" : "679px";
            
            /*Above code is commented and below height is applied as above is causings screen distortion in chrome.*/
            tileDivWrapper1.Style["Height"] = visible ? "290px" : "679px";
            tileDiv1.Style["Height"] = visible ? "307px" : "678px";


            this.tileDivWrapper2.Visible = visible;
        }

        public void ClearRotators()
        {
            tileDiv1.Controls.Clear();
            tileDiv2.Controls.Clear();            
        }

        public Control GetRotator1()
        {
            return tileDiv1;
        }

        public Control GetRotator2()
        {
            return tileDiv2;
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
            tileDiv1.Controls.Add(container);
            tileDiv1.Style["width"] = (tileDiv1.Controls.Count * 1000) + "px";
        }

        public void AddItemToRotator2(Container container)
        {
            tileDiv2.Controls.Add(container);
            tileDiv2.Style["width"] = (tileDiv2.Controls.Count * 1000) + "px";
        }

        public void ResetPageOnPostBack(string divIndex)
        {
            ReturnToPostBackPage.Value = divIndex;
        }

        public void AddNavigationButtons(Folder folder = null)
        {
            CreateRotatorNavigationButtons(buttonsContainer1, tileDiv1, 1);
            CreateRotatorNavigationButtons(buttonsContainer2, tileDiv2, 2);
            container1Label.Text = string.Empty;
            container2Label.Text = string.Empty;

            if (folder != null)
            {
                if (folder.Container1Label.Length > 0)
                    container1Label.Text = folder.Container1Label;

                if (folder.Container2Label.Length > 0)
                    container2Label.Text = folder.Container2Label;
            }
        }

        private void CreateRotatorNavigationButtons(Panel panel, HtmlGenericControl div, int divIndex)
        {
            panel.Controls.Clear();
            var counter = 1;

            if (div.Controls.Count > 1)
            {
                LinkButton linkButton = CreatePreviousButton(divIndex);
                panel.Controls.Add(linkButton);
            }

            foreach (Control item in div.Controls)
            {
                LinkButton linkButton = CreateLinkButton(counter, divIndex);
                panel.Controls.Add(linkButton);
                counter++;
            }

            if (div.Controls.Count > 1)
            {
                LinkButton linkButton = CreateNextButton(divIndex);
                panel.Controls.Add(linkButton);
            }
        }

        protected LinkButton CreatePreviousButton(int divIndex)
        {
            return new LinkButton
            {
                Text = "<",
                ID = string.Format("PrevButton{0}", divIndex),
                OnClientClick = string.Format("leftArrowClick('{0}'); return false;", divIndex),
                CssClass = "rotatorPageButton"
            };
        }

        protected LinkButton CreateNextButton(int divIndex)
        {
            return new LinkButton
            {
                Text = ">",
                ID = string.Format("NextButton{0}", divIndex),
                OnClientClick = string.Format("rightArrowClick('{0}'); return false;", divIndex),
                CssClass = "rotatorPageButton"
            };
        }

        protected LinkButton CreateLinkButton(int itemIndex, int divIndex)
        {
            return new LinkButton
            {
                Text = itemIndex.ToString(),
                ID = string.Format("Button{0}_{1}", divIndex, itemIndex),
                ClientIDMode = System.Web.UI.ClientIDMode.Static,
                OnClientClick = string.Format("DoubleScrollPanel_JumpToPage({0},'{1}'); return false;", itemIndex, divIndex),
                CssClass = "rotatorPageButton"
            };
        }    
    }
}
