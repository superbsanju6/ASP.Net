namespace Thinkgate
{
    using System;
    using System.Web.Security;
    using Thinkgate.Classes;
    using System.Web.UI;

    public partial class SearchExpanded : System.Web.UI.MasterPage
    {
        #region Properties

        public SessionObject SessionObject { get; set; }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.User.Identity.IsAuthenticated || Session == null || Session.IsNewSession || Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }

            ScriptManager.RegisterStartupScript(this, typeof(string), "SessionBridgeVariable", " var SessionBridge='" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "';", true);
        }

        public Control BuildTemplateSearchPager(int numberOfPages)
        {

            var wrapperDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            var leftControls = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            var centerControls = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            var pageWrapper = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            var rightControls = new System.Web.UI.HtmlControls.HtmlGenericControl("div");


            #region Add everything to left control
            //Add attributes to left
            leftControls.Attributes.Add("class", "rgWrap rgArrPart1");
            //add two inputs to left
            var firstPageBtn = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
            firstPageBtn.Attributes.Add("type", "button");
            firstPageBtn.Attributes.Add("title", "First Page");
            firstPageBtn.Attributes.Add("class", "rgPageFirst");
            firstPageBtn.Attributes.Add("value", " ");
            firstPageBtn.Attributes.Add("onclick", "goToPage(1);");
            firstPageBtn.Attributes.Add("style", "border:0px;");

            var prevPageBtn = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
            prevPageBtn.Attributes.Add("type", "button");
            prevPageBtn.Attributes.Add("title", "Previous Page");
            prevPageBtn.Attributes.Add("class", "rgPagePrev");
            prevPageBtn.Attributes.Add("value", " ");
            prevPageBtn.Attributes.Add("onclick", "goToPrevPage();");
            prevPageBtn.Attributes.Add("style", "border:0px;");

            leftControls.Controls.Add(firstPageBtn);
            leftControls.Controls.Add(prevPageBtn);
            #endregion

            #region Add everything to center control
            //Add attributes to center
            centerControls.Attributes.Add("class", "rgWrap rgNumPart");
            centerControls.Attributes.Add("id", "pagingScrollWrapper");

            pageWrapper.Attributes.Add("id", "numberWrapper");

            //add spans to pageWrapper inside of a tags for each page
            int i = 1;
            while (i <= numberOfPages)
            {

                var pageElement = new System.Web.UI.HtmlControls.HtmlGenericControl("a") { InnerHtml = "<span>" + i.ToString() + "</span>" };
                pageElement.Attributes.Add("id", "pageTag_" + i.ToString());
                pageElement.Attributes.Add("onclick", "goToPage(" + i.ToString() + ");");
                pageElement.Attributes.Add("class", (i == 1) ? "rgCurrentPage" : "");
                pageWrapper.Controls.Add(pageElement);
                i++;
            }

            centerControls.Controls.Add(pageWrapper);

            #endregion

            #region Add everything to right control
            //Add attributes to left
            rightControls.Attributes.Add("class", "rgWrap rgArrPart2");
            //add two inputs to left
            var nextpageBtn = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
            nextpageBtn.Attributes.Add("type", "button");
            nextpageBtn.Attributes.Add("title", "Next Page");
            nextpageBtn.Attributes.Add("class", "rgPageNext");
            nextpageBtn.Attributes.Add("value", " ");
            nextpageBtn.Attributes.Add("onclick", "goToNextPage(1);");
            nextpageBtn.Attributes.Add("style", "border:0px;");

            var lastPageBtn = new System.Web.UI.HtmlControls.HtmlGenericControl("input");
            lastPageBtn.Attributes.Add("type", "button");
            lastPageBtn.Attributes.Add("title", "Last Page");
            lastPageBtn.Attributes.Add("class", "rgPageLast");
            lastPageBtn.Attributes.Add("value", " ");
            lastPageBtn.Attributes.Add("onclick", "goToPage(" + numberOfPages + ");");
            lastPageBtn.Attributes.Add("style", "border:0px;");

            rightControls.Controls.Add(nextpageBtn);
            rightControls.Controls.Add(lastPageBtn);
            #endregion


            //add two button inputs to right

            //Add attributes to wrapper
            wrapperDiv.Attributes.Add("class", "rgWrap");
            wrapperDiv.Attributes.Add("id", "templateSearchPage");
            wrapperDiv.Attributes.Add("lastpage", numberOfPages.ToString());
            wrapperDiv.Style.Add("width", "200px");
            //add left to wrapper
            wrapperDiv.Controls.Add(leftControls);
            //add center to wrapper
            wrapperDiv.Controls.Add(centerControls);
            //add right to wrapper
            wrapperDiv.Controls.Add(rightControls);

            return wrapperDiv;

        }

    }
}
