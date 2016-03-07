using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;
using System.Web.UI.HtmlControls;

namespace Thinkgate.Controls.Plans
{
    public partial class LessonPlans : TileControlBase
    {
        //Add to Calendar icon visible
        protected bool CalendarIconVisible = false;
        private DataTable _categoriesAndTypesDT;
        private DataTable _standardsDT;
        private const String StandardsFilterKey = "ReStandardsFilter";
        private const String TypeFilterKey = "ReTypeFilter";
        private int _classID;
        private int _term;
        private int _planID;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (Tile == null) return;

            _classID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("selectID"));
            _term = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("term"));
            _planID = DataIntegrity.ConvertToInt(Tile.TileParms.GetParm("planID"));
           
            if (Tile.TileParms.GetParm("showCalendarIcon") != null)
            {
                CalendarIconVisible = DataIntegrity.ConvertToBool(Tile.TileParms.GetParm("showCalendarIcon"));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            BtnAdd.Attributes["onclick"] = "window.open('" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") +
            "' + escape('fastsql_v2_direct.asp?ID=7266|search_documents_add&??x=Lesson Plans&??action=add&appName=E3'));";

            var postBackControlID = GetButtonControlThatCausedPostBack(Page);
            if (!string.IsNullOrEmpty(postBackControlID) && (postBackControlID.Contains("resourceStandardsDropdown") || postBackControlID.Contains("resourceTypesDropdown")))
                return;

            SearchPlans();

            if (ViewState[TypeFilterKey] == null)
                ViewState.Add(TypeFilterKey, "All");
            if (ViewState[StandardsFilterKey] == null)
                ViewState.Add(StandardsFilterKey, new List<int>());

            BuildTypes();
            BuildStandards();
        }

        protected void SearchPlans()
        {
            lbxPlans.DataSource = Base.Classes.LessonPlan.GetSamplePlans();
            lbxPlans.DataBind();
        }

        protected void lbxPlans_ItemDataBound(Object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
        {
            var listBoxItem = e.Item;

            var img = (System.Web.UI.HtmlControls.HtmlImage)listBoxItem.FindControl("imgAddToCalendar");
            var editLink = (ImageButton)listBoxItem.FindControl("btnGraphicUpdate");
            var viewLink = (HyperLink)listBoxItem.FindControl("lnkGraphicView");
            var dataItem = (Base.Classes.LessonPlan)e.Item.DataItem;
            var lblPlanName = (Label)listBoxItem.FindControl("lblPlanName");
            var lnkPlanName = (HyperLink)listBoxItem.FindControl("lnkPlanName");

            if (img != null)
                img.Visible = CalendarIconVisible;

            if (editLink != null)
            {
                if (UserHasPermission(Permission.Edit_Resource))
                {
                    var link = "window.open('" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "' + escape('display.asp?key=7266&fo=basic display&rm=page&xID=" + dataItem.ID +
                        "&??hideButtons=Save And Return&??appName=E3')); return false;";

                    lblPlanName.Visible = false;
                    lnkPlanName.Visible = true;
                    lnkPlanName.Target = "_blank";
                    //lnkPlanName.NavigateUrl = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "display.asp?key=7266&fo=basic display&rm=page&xID=" + dataItem.ID + "&??hideButtons=Save And Return&??appName=E3";
                    lnkPlanName.NavigateUrl = ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "display.asp%3Fkey%3D7266%26fo%3Dbasic%20display%26rm%3Dpage%26xID%3D" + dataItem.ID + "%26%3F%3FhideButtons%3DSave%20And%20Return%26%3F%3FappName%3DE3";
                    //lnkPlanName.Attributes.Add("onclick", link);

                    editLink.Visible = true;
                    editLink.PostBackUrl = "javascript:void(0);";
                    editLink.OnClientClick = link;
                }
                else
                {
                    lblPlanName.Visible = true;
                    lnkPlanName.Visible = false;
                    editLink.Visible = false;
                }
            }

            if (viewLink != null)
            {
                var link = "";

                switch (dataItem.DocumentType)
                {
                    case "External Content":
                        link += ResolveUrl("~/upload/") + dataItem.ViewLink;
                        break;
                    case "Web Link":
                        link += dataItem.ViewLink;
                        break;
                    default:
                        link += ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + System.Web.HttpUtility.UrlEncode(dataItem.ViewLink);
                        break;
                }

                viewLink.Target = "_blank";
                viewLink.NavigateUrl = link;
            }
        }

        private void BuildTypes()
        {
            if (_categoriesAndTypesDT == null) _categoriesAndTypesDT = Thinkgate.Base.Classes.Resource.GetResourceCategoriesDataTable();

            //TODO: Add dependency.
            var types = (from i in _categoriesAndTypesDT.AsEnumerable() 
                                  where i.Field<String>("TYPE") == "Lesson Plans" || i.Field<String>("TYPE") == "Unit Plans" 
                                  select i.Field<String>("TYPE")).Distinct().ToList();

            resourceTypesDropdown.Items.Clear();
            resourceTypesDropdown.Items.Add(new RadComboBoxItem("Type", "All"));

            foreach (var c in types)
            {
                resourceTypesDropdown.Items.Add(new RadComboBoxItem(c, c));
            }

            var item = this.resourceTypesDropdown.Items.FindItemByValue((String)this.ViewState[TypeFilterKey], true) ?? this.resourceTypesDropdown.Items[0];
            ViewState[TypeFilterKey] = item.Value;
            resourceTypesDropdown.SelectedIndex = resourceTypesDropdown.Items.IndexOf(item);
        }

        private void BuildStandards()
        {
            var selectedStandards = (List<int>)ViewState[StandardsFilterKey];
            if (_standardsDT == null) _standardsDT = Base.Classes.Standards.GetStandardsForInstructionalPlan(1, _classID, _term);
            if (_standardsDT == null || _standardsDT.Rows.Count == 0) return;

            resourceStandardsDropdown.Items.Clear();
            var resourceStandardsDropdownCheckedItemsTotal = 0;

            var resourceStandardsListItemAll = new RadComboBoxItem();
            resourceStandardsListItemAll.Text = "All Standards";
            resourceStandardsListItemAll.Value = "All Standards";
            resourceStandardsDropdown.Items.Add(resourceStandardsListItemAll);

            var allItemCheckbox = (CheckBox)resourceStandardsListItemAll.FindControl("resourceStandardsCheckBox");
            var allItemLabel = (Label)resourceStandardsListItemAll.FindControl("resourceStandardsLabel");
            allItemCheckbox.Attributes["onclick"] = "RadCombobox_CustomMultiSelectDropdown_evaluateCheckedItem(this, '" + resourceStandardsDropdown.ClientID + "', 'resourceStandards')";
            if (selectedStandards.Count == _standardsDT.Rows.Count || selectedStandards.Count == 0)
            {
                if (allItemCheckbox != null)
                {
                    allItemCheckbox.Checked = true;
                    resourceStandardsDropdown.Text = "All Standards";
                }
            }
            if (allItemLabel != null)
            {
                allItemLabel.Text = "All Standards";
            }

            foreach (DataRow row in _standardsDT.Rows)
            {
                var resourceStandardsListItem = new RadComboBoxItem();
                resourceStandardsListItem.Text = row["StandardName"].ToString();
                resourceStandardsListItem.Value = row["StandardID"].ToString();

                resourceStandardsDropdown.Items.Add(resourceStandardsListItem);

                var itemCheckbox = (CheckBox)resourceStandardsListItem.FindControl("resourceStandardsCheckbox");
                var itemLabel = (Label)resourceStandardsListItem.FindControl("resourceStandardsLabel");
                itemCheckbox.Attributes["onclick"] = "RadCombobox_CustomMultiSelectDropdown_evaluateCheckedItem(this, '" + resourceStandardsDropdown.ClientID + "', 'resourceStandards')";
                bool itemExistsInSession = selectedStandards.Contains(DataIntegrity.ConvertToInt(row["StandardID"]));

                if (itemCheckbox != null)
                {
                    if (selectedStandards.Count == _standardsDT.Rows.Count || selectedStandards.Count == 0)
                    {
                        itemCheckbox.Checked = true;
                        resourceStandardsDropdownCheckedItemsTotal++;
                    }
                    else if (itemExistsInSession)
                    {
                        itemCheckbox.Checked = true;
                        resourceStandardsDropdownCheckedItemsTotal++;
                        resourceStandardsDropdown.Text = row["StandardName"].ToString();
                    }
                }
                if (itemLabel != null)
                {
                    itemLabel.Text = row["StandardName"].ToString();
                }
            }

            if (resourceStandardsDropdownCheckedItemsTotal == _standardsDT.Rows.Count)
            {
                resourceStandardsDropdown.Items[0].Checked = true;
                var itemCheckbox = (CheckBox)resourceStandardsDropdown.Items[0].FindControl("resourceStandardsCheckbox");
                itemCheckbox.Checked = true;
                resourceStandardsDropdown.Items[0].Selected = true;
            }
            else if (resourceStandardsDropdownCheckedItemsTotal > 1)
            {
                resourceStandardsDropdown.Text = "Multiple";
            }

            var findButton = new RadComboBoxItem();
            resourceStandardsDropdown.Items.Add(findButton);

            var findButtonCheckbox = (CheckBox)findButton.FindControl("resourceStandardsCheckbox");
            var findButtonImg = (HtmlImage)findButton.FindControl("okImgBtn");
            findButtonImg.Src = ResolveUrl("~/Images/ok.png");
            findButtonCheckbox.Attributes["onclick"] = "RadCombobox_CustomMultiSelectDropdown_evaluateCheckedItem(this, '" + resourceStandardsDropdown.ClientID + "', 'resourceStandards')";
            if (findButtonCheckbox != null)
            {
                findButtonCheckbox.InputAttributes["style"] = "display:none;";
            }
        }

        protected void cmbType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ViewState[TypeFilterKey] = e.Value;
            SearchPlans();
            BuildTypes();
            BuildStandards();
        }

        protected void ResourceStandards_OnSelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var selectedStandards = new List<int>();

            foreach (RadComboBoxItem item in resourceStandardsDropdown.Items)
            {
                var itemCheckbox = (CheckBox)item.FindControl("resourceStandardsCheckbox");
                Label itemLabel = (Label)item.FindControl("resourceStandardsLabel");

                if (itemCheckbox != null && itemCheckbox.Checked && itemLabel.Text != "All" && itemLabel.Text.IndexOf("<img") == -1)
                {
                    selectedStandards.Add(DataIntegrity.ConvertToInt(item.Value));
                }
            }

            ViewState[StandardsFilterKey] = selectedStandards;
            SearchPlans();
            BuildTypes();
            BuildStandards();
        }

        protected static string GetButtonControlThatCausedPostBack(System.Web.UI.Page page)
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
                foreach (string ctl in page.Request.Form)
                {
                    //handle ImageButton they having an additional "quasi-property" in their Id which identifies
                    //mouse x and y coordinates
                    Control c = null;
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        var ctrlStr = ctl.Substring(0, ctl.Length - 2);
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

            if (control == null && !string.IsNullOrEmpty(ctrlname))
            {
                if (ctrlname.IndexOf("resourceStandardsDropdown") > -1)
                {
                    return ctrlname.Substring(ctrlname.IndexOf("resourceStandardsDropdown"));
                }

                if (ctrlname.IndexOf("resourceTypesDropdown") > -1)
                {
                    return ctrlname.Substring(ctrlname.IndexOf("resourceTypesDropdown"));
                }

                return null;
            }

            return (control != null) ? control.ID : null;
        }
    }    
}