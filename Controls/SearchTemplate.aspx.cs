using System;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;

namespace Thinkgate.Controls
{
    public partial class SearchTemplate : BasePage
    {
        public string ImageWebFolder { get; set; }
        
        protected new void Page_Init(object sender, EventArgs e)
        {
            Master.Search += new SearchMaster.SearchHandler(SearchHandler);
            base.Page_Init(sender, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
                
        }

        /// <summary>
        /// Handler for the Search Button
        /// </summary>
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            // You typically would bind data to your result object here
        }

    }
}