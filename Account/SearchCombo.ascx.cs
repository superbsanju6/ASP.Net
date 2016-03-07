using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Thinkgate.Account
{
    public partial class SearchCombo : System.Web.UI.UserControl
    {
        public event EventHandler ComboSelect;
        private Base.Classes.Administration adm = new Base.Classes.Administration();
        private const string comboType = "comboType";

        public enum ComboModes
        {
            None,
            Permissions,
            Roles,
            Portals,
            Users,
            Schools,
            SchoolTypes,
            Subjects
        }

        public ComboModes ComboMode
        {
            get { return ViewState[comboType] != null ? (ComboModes)Enum.Parse(typeof(ComboModes), ViewState[comboType].ToString()) : ComboModes.None; }
            set { ViewState[comboType] = value.ToString(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindComboBox();
            }
        }

        public void ReloadGUI(ComboModes mode)
        {
            ComboMode = mode;
            cmb.Text = cmb.EmptyMessage;
            BindComboBox();
        }

        public string GetComboTextByValue(string value)
        {
            return cmb.FindItemByValue(value).Text;
        }

        private void BindComboBox()
        {
            cmb.Items.Clear();

            switch (ComboMode)
            {
                case ComboModes.Permissions:
                    cmb.DataTextField = "permissionname";
                    cmb.DataValueField = "permissionid";
                    cmb.DataSource = adm.GetPermissionsAll();
                    break;
                case ComboModes.Roles:
                    cmb.DataTextField = "rolename";
                    cmb.DataValueField = "roleid";
                    cmb.DataSource = adm.GetRolesAll();
                    break;
                case ComboModes.Schools:
                    cmb.DataTextField = "name";
                    cmb.DataValueField = "id";
                    cmb.DataSource = adm.GetSchoolsAll(); ;
                    break;
                case ComboModes.Subjects:
                    cmb.DataTextField = "displaytext";
                    cmb.DataValueField = "displaytext";
                    cmb.DataSource = adm.GetSubjectsAll();
                    break;
                case ComboModes.SchoolTypes:
                    cmb.DataTextField = "name";
                    cmb.DataValueField = "name";
                    cmb.DataSource = adm.GetSchoolTypes();
                    break;
                case ComboModes.Portals:
                    cmb.DataTextField = "portalname";
                    cmb.DataValueField = "id";
                    cmb.DataSource = adm.GetPortalSelections();
                    break;
                default:
                    cmb.DataSource = null;
                    break;
            }
            cmb.DataBind();
        }

        protected void cmb_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (ComboSelect != null && ComboMode != ComboModes.None && ((RadComboBox)o).FindItemByText(e.Text) != null)
            {
                this.Attributes.Add(this.ID.ToString(), ((RadComboBox)o).FindItemByText(e.Text).Value);
                ComboSelect(this, e);
            }
        }

    }
}