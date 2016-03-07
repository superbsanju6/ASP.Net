using System;
using Standpoint.Core.Utilities;
using System.Collections.Generic;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class SearchPager : System.Web.UI.UserControl
    {
        public int ResultCount;
        public int PageSize;
        private int _numberOfPages;
        private List<int> _pageList;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public override void DataBind()
        {
            _numberOfPages = DataIntegrity.ConvertToInt(Math.Ceiling(ResultCount / DataIntegrity.ConvertToDouble(PageSize)));
            PagerDiv.Visible = _numberOfPages > 1;
            rgPageLast.Attributes.Add("maxPage", _numberOfPages.ToString());
            _pageList = new List<int>();
            for (var j = 1; j <= _numberOfPages; j++ )
            {
                _pageList.Add(j);
            }
            PageList.DataSource = _pageList;
            resultsFoundText.InnerHtml = "Results found: " + ResultCount + " in " + _numberOfPages + " page" + (_numberOfPages == 1 ? "." : "s.");
            base.DataBind();   
        }

        protected string GetItemClass(string pageNumber)
        {
            if (pageNumber.Equals("1")) return "rgCurrentPage";
            return "";
        }


        public int GetNumberOfPages()
        {
            return _numberOfPages;
        }
    }
}