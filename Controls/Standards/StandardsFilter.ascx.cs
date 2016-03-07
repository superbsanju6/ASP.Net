using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Telerik.Charting;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;

namespace Thinkgate.Controls.Standards
{
    public partial class StandardsFilter : TileControlBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;
            //return; //Preventing this code from running because it is incomplete

            DataSet standardsFilterDataSet = Base.Classes.Standards.GetStandardsFilters(SessionObject.LoggedInUser.Page);
            //DataTable standardsFilterTable = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(standardsFilterDataSet.Tables[0], "FilterID", "FilterID_Encrypted");
            DataTable standardsFilterTable = standardsFilterDataSet.Tables[0];

            standardsFilterDataSet.Tables.Remove(standardsFilterDataSet.Tables[0]);
            standardsFilterDataSet.Tables.Add(standardsFilterTable);

            if (standardsFilterDataSet.Tables[0].Rows.Count > 0)
            {
                standardsFilterDefaultTextSpan.Visible = false;


                standardsFilterRadTree.DataTextField = "Name";
                standardsFilterRadTree.DataFieldID = "ID";
                standardsFilterRadTree.DataFieldParentID = "ParentID";

                standardsFilterRadTree.DataSource = standardsFilterDataSet;
                standardsFilterRadTree.DataBind();

                BtnAdd.Attributes["onclick"] = "return false;";
                BtnAdd.Attributes["style"] = "cursor:default; margin-top: -1px;";
                BtnAddSpan.Attributes["style"] = "opacity:.5; filter:progid:DXImageTransform.Alpha(opacity=50); filter:alpha(opacity=50);"; //BJC 8/2/2012: opacity fix for IE8.
                BtnAddDiv.Attributes["style"] = "padding: 0; opacity:.5; filter:progid:DXImageTransform.Alpha(opacity=50); filter:alpha(opacity=50);"; //BJC 8/2/2012: opacity fix for IE8.
            }
            else
            {
                standardsFilterDefaultTextSpan.Visible = true;
                standardsFilterRadTree.Visible = false;

                HyperLink link = new HyperLink();
                link.NavigateUrl = "~/Controls/Standards/StandardsFilterEdit.aspx?filterName=";

                BtnAdd.Attributes["onclick"] = "customDialog({url: '" + link.ResolveClientUrl(link.NavigateUrl) + "', title: 'Edit Standard Filter',maximize: true}); return false;";
            }

        }
        
        protected void StandardsFilterRadTree_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            var node = e.Node;
            
            if(node.ParentNode == null)
            {
                node.CssClass = "RadTreeView_Thinkgate_TreeView_ParentNode";
                var link = (ImageButton)node.FindControl("editLink");
                if (link != null)
                {
                    var dataRowView = (DataRowView)node.DataItem;
                    HyperLink tempLink = new HyperLink();
                    tempLink.NavigateUrl = "~/Controls/Standards/StandardsFilterEdit.aspx?filterName=";
                    string linkURL = tempLink.ResolveClientUrl(tempLink.NavigateUrl) + dataRowView["Name"];
                    link.OnClientClick = "customDialog({url: '" + linkURL + "', title: 'Edit Standard Filter',maximize: true}); return false;";
                    link.Visible = true;
                    link.Attributes["style"] = "cursor:pointer;";
                }
            }
        }

        public void StandardFilterRefresh_Click(object sender, EventArgs e)
        {
            string js = "var modalWin = $find('RadWindow1Url'); if(modalWin) { modalWin.remove_beforeClose(modalWin.confirmBeforeClose); setTimeout(function () { modalWin.close(); }, 0); }";
            System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(StandardsFilter), "standardFilterRefresh", js, true);
        }
    }
}