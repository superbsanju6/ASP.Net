using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;

namespace Thinkgate.Controls.Teacher
{
    public partial class TeacherSearchResults : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {            
            var dt = Thinkgate.Base.Classes.Data.TeacherDB.Search();
            var htmls = new List<string>();
            var titles = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                titles.Add("Teacher ID: " + row["TeacherID"].ToString());
                htmls.Add(row["Teacher_Name"].ToString()                         
                            + "<br/><center><a href='javascript: openTeacher(" + row["page"] + ")'>View</a></center>");
            }

            var htmlLists = (Chunk<string>(htmls, 30)).ToList();
            var titleLists = (Chunk<string>(titles, 30)).ToList();

            for (int i = 0; i < titleLists.Count; i++)
            {
                var htmlsList = (htmlLists[i]).ToList<string>();
                var titlesList = (titleLists[i]).ToList<string>();

                var container = (TileContainerThirty)LoadControl("~/Controls/TileContainerThirty.ascx");
                container.SetTitles(titlesList);
                container.SetHTMLToContent(htmlsList);

                var item = new RadRotatorItem();
                item.Controls.Add(container);
                tilesRotator.Items.Add(item);
            }

            AddNavigationButtons();
        }

        public IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        private void AddNavigationButtons()
        {
            ButtonsContainer.Controls.Clear();

            foreach (RadRotatorItem item in tilesRotator.Items)
            {
                LinkButton linkButton = CreateLinkButton(item.Index);
                ButtonsContainer.Controls.Add(linkButton);
            }
        }

        private LinkButton CreateLinkButton(int itemIndex)
        {
            // Create the LinkButton
            LinkButton button = new LinkButton();
            button.Text = (itemIndex + 1).ToString();// The test of the button
            button.ID = string.Format("Button{0}", itemIndex);// Assign an unique ID
            button.OnClientClick = string.Format("showItemByIndex({0},'{1}'); return false;", itemIndex, tilesRotator.ClientID);

            // Class which is applied to the newly added button
            button.CssClass = "buttonClass";
            return button;
        }
    }
}