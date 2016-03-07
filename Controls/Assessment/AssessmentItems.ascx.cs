using System;
using System.Data;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Assessment
{
    public partial class AssessmentItems : TileControlBase
    {
        // In the page load we want to decide whether to display the search icon (magnifying glass) depending 
        // on whether the user has permission to search or not. Since this control is "reused" to create 
        // tiles for different types of assessment components: Items, Images, Rubrics, Addendums, we can 
        // determine which type of search permission to use in the page init event handler since there is 
        // already a nice SWITCH statement already in place, then stash it in this variable for later use.

        public Base.Enums.Permission SearchPermissionType;

        // Do the same with the add icon.
        public Base.Enums.Permission AddPermissionType;

        protected new void Page_Init(object sender, EventArgs e)
        {
            
        }

        private void LoadControls()
        {
            if (Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
            SessionObject = (SessionObject)Session["SessionObject"];

            var user = SessionObject.LoggedInUser;
            if (user == null) return;

            if (string.IsNullOrEmpty(Tile.Title)) return;

            DataTable dt = new DataTable();

            switch (Tile.Title)
            {
                case "Rubrics":
                    dt = Thinkgate.Base.Classes.Assessment.LoadUserRubricCountsByBank(user);
                    SearchPermissionType = Base.Enums.Permission.Search_Rubric;
                    AddPermissionType = Base.Enums.Permission.Create_Rubric;
                    btnAdd.Attributes["onclick"] = "addNewItem('Rubric');";
                    break;

                case "Items":
                    dt = Thinkgate.Base.Classes.Assessment.LoadUserItemCountsByBank(user);
                    SearchPermissionType = Base.Enums.Permission.Search_Item;
                    AddPermissionType = Base.Enums.Permission.Create_Item;
                    btnAdd.Attributes["onclick"] = "addNewItem('Item');";
                    break;

                case "Images":
                    dt = Thinkgate.Base.Classes.Assessment.LoadUserImageCountsByBank(user);
                    SearchPermissionType = Base.Enums.Permission.Search_Image;
                    AddPermissionType = Base.Enums.Permission.Create_Image;
                    btnAdd.Attributes["onclick"] = "addNewItem('Image');";
                    break;

                case "Addendums":
                    dt = Thinkgate.Base.Classes.Assessment.LoadUserAddendumCountsByBank(user);
                    SearchPermissionType = Base.Enums.Permission.Search_Addendum;
                    AddPermissionType = Base.Enums.Permission.Create_Addendum;
                    btnAdd.Attributes["onclick"] = "addNewItem('Addendum');";
                    break;

                default:
                    dt = null;
                    break;
            }

            if (dt == null) return;

            // create "third party data" table.
            var thirdPartyData = new DataTable();
            thirdPartyData.Columns.Add("ItemBank");
            thirdPartyData.Columns.Add("ItemCount");

            // create "local data" table.
            var localData = new DataTable();
            localData.Columns.Add("ItemBank");
            localData.Columns.Add("ItemCount");

            // loop through rows seperating out 3rd party and local data.
            foreach (DataRow r in dt.Rows)
            {
                if (r["Category"].ToString() == "3rd Party")
                {
                    thirdPartyData.Rows.Add(r["ItemBank"], r["ItemCount"]);
                }
                else
                {
                    localData.Rows.Add(r["ItemBank"], r["ItemCount"]);
                }
            }

            // if "local" table has rows, show barchart1.
            if (localData.Rows.Count > 0)
            {
                barchart1.Visible = true;
                barchart1.Data = localData;
                barchart1.Height = thirdPartyData.Rows.Count > 0 ? 105 : 211;
                barchart1.VerticalHeader = "ItemBank";
                barchart1.HorizontalHeader = "ItemCount";
                barchart1.ShowLegend = false;
            }


            // if "third party" table has rows, show barchart2.            
            if (thirdPartyData.Rows.Count > 0)
            {
                barchart2.Visible = true;
                barchart2.Data = thirdPartyData;
                barchart2.Height = localData.Rows.Count > 0 ? 105 : 211;
                barchart2.VerticalHeader = "ItemBank";
                barchart2.HorizontalHeader = "ItemCount";
                barchart2.ShowLegend = false;
            }

            // if both tables have no rows, show the "no items" div.
            if (thirdPartyData.Rows.Count < 1 && localData.Rows.Count < 1)
            {
                var noItemsDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");

                noItemsDiv.Style.Add("width", "311px");
                noItemsDiv.Style.Add("height", "211px");
                noItemsDiv.Style.Add("background-color", "white");
                noItemsDiv.InnerText = "No items found.";
                phNoItems.Controls.Add(noItemsDiv);
                barchart1.Visible = false; //to be safe that both don't display
                barchart2.Visible = false;

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            LoadControls();

            var extractedexpand = 
                string.IsNullOrEmpty(Tile.ExpandJSFunctionOverride) ? 
                    BuildCustomDialogPath(Tile.ExpandedControlPath, "SearchItem" + "_" + Tile.Title, 950, 675) 
                : Tile.ExpandJSFunctionOverride;
            
            btnSearch.Attributes.Add("onclick", extractedexpand);

            btnSearch.Visible = (UserHasPermission(SearchPermissionType));
            btnAdd.Visible = (UserHasPermission(AddPermissionType));
        }

        public string BuildCustomDialogPath(string path, string name, int? width = null, int? height = null)
        {
            var sizeblock = "autoSize: true";
            if (width != null  && height != null  ) {

                sizeblock = "width: "+ width +", height: "+ height;
            }
            return "customDialog({ url: ('" + path + "')," + sizeblock + " , name: '" + name + "' });";
        }
    }
}