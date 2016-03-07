
using System;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Credentials
{
    public partial class CredentialAlignments : BasePage
    {
        public string CredentialIDs { get; set; }
        private DataTable allAlignments = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            hdnSavedAlignments.Value = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString.Get("crdAlignments")))
            {
                hdnSavedAlignments.Value = Request.QueryString.Get("crdAlignments");
            }
            BuildAlignmentAccordion();
            SelectCredentialAlignments();

            if (!IsPostBack)
            { }
        }

        private void BuildAlignmentAccordion()
        {
            allAlignments = GetAllAlignments();

            allAlignments.DefaultView.RowFilter = "ParentID Is Null";
            DataTable alignments = allAlignments.DefaultView.ToTable();

            rptAlignments.DataSource = alignments;
            rptAlignments.DataBind();
        }

        private void SelectCredentialAlignments()
        {
            string[] savedAlignments = hdnSavedAlignments.Value.Split(',');
            rptAlignments.Items.Cast<RepeaterItem>().Where(ri => ri.ItemType == ListItemType.Item || ri.ItemType == ListItemType.AlternatingItem).ToList().ForEach(item =>
            {
                CheckBox checkBox = (CheckBox)item.FindControl("chkRoot");
                if (checkBox != null)
                {
                    string nodeValue = checkBox.Attributes["nodeValue"];

                    if (savedAlignments.Contains(nodeValue))
                    {
                        checkBox.Checked = true;
                    }
                    /* 2nd level alignments */
                    Repeater childRepeater = (Repeater)item.FindControl("rptChildAlignments");
                    if (childRepeater != null)
                    {
                        childRepeater.Items.Cast<RepeaterItem>().Where(ri => ri.ItemType == ListItemType.Item || ri.ItemType == ListItemType.AlternatingItem).ToList().ForEach(childItem =>
                        {
                            CheckBox childCheckBox = (CheckBox)childItem.FindControl("chkChild");
                            string childNodeValue = childCheckBox.Attributes["nodeValue"];

                            if (savedAlignments.Contains(childNodeValue))
                            {
                                childCheckBox.Checked = true;
                            }
                        });
                    }
                }
            });
        }

        private DataTable GetAllAlignments()
        {
            DataTable alignments = Base.Classes.Credentials.GetAllAlignments();
            if (alignments != null && alignments.Rows.Count > 0 && alignments.Rows[0]["CredentialAlignment"].ToString().Equals("All", StringComparison.InvariantCultureIgnoreCase))
            {
                alignments.Rows.RemoveAt(0);
            }
            return alignments;
        }

        protected void rptAccordion_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            DataRowView rowView = (DataRowView)e.Item.DataItem;
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string id = rowView["ID"].ToString();

                allAlignments.DefaultView.RowFilter = "ParentID = " + id + " ";
                DataTable childTable = allAlignments.DefaultView.ToTable();

                Repeater repeater = (Repeater)e.Item.FindControl("rptChildAlignments");
                if (repeater != null)
                {
                    repeater.DataSource = childTable;
                    repeater.DataBind();
                }
            }
        }
    }
}