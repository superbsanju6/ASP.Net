using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Web.UI;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class TreeDropdownList : CriteriaBase
    {
        public object DataSource;
        public string DataTextField;
        public string DataValueField;
        public string OnClientLoad;
        public string EmptyMessage;
        public List<String> DefaultTexts;
        public object JsonDataSource;
        public bool LoadDefaults = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //ddlAlignments.OnClientSelectedIndexChanged = CriteriaName + "Controller.OnChange";
                RestrictValueCount = RestrictValueOptions.OnlyOneAppliedAtATime;   // dropdown list can only have 1 value
                //ddlAlignments.EmptyMessage = EmptyMessage;
                if (!String.IsNullOrEmpty(OnClientLoad))
                {
                    ddlAlignments.OnClientLoad = OnClientLoad;
                }
                else
                {
                    if (DefaultTexts != null && DefaultTexts.Count > 0 && LoadDefaults)
                    {
                        ddlAlignments.OnClientLoad = CriteriaName + "Controller.CheckDefaultTexts";
                    }
                }
                if (JsonDataSource != null) ScriptManager.RegisterStartupScript(this, typeof(string), CriteriaName + "DependencyData", "var " + CriteriaName + "DependencyData = " + JsonDataSource + ";", true);
                //DataBind();
                BindAlignmentsTree();
            }
            ConfigureCriteriaHeader(CriteriaHeader, RadToolTip1);
        }

        public override void DataBind()
        {
            ddlAlignments.DataSource = DataSource;
            ddlAlignments.DataTextField = DataTextField;
            ddlAlignments.DataValueField = DataValueField;
            ddlAlignments.DataBind();
            base.DataBind();
        }

        private bool IsAlignmentDropdownPostback()
        {
            return Request.Params["__EVENTTARGET"] != null && Request.Params["__EVENTARGUMENT"] != null && Request.Params["__EVENTTARGET"].EndsWith(this.ddlAlignments.ID) && Request.Params["__EVENTARGUMENT"].Equals("OnClientEntryAdded");
        }

        public string DefaultTextAsJs()
        {
            return (new JavaScriptSerializer()).Serialize(DefaultTexts);
        }

        public class ValueObject
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }

        public RadDropDownTree ComboBox
        {
            get { return ddlAlignments; }
        }

        public RadToolTip ToolTip
        {
            get { return RadToolTip1; }
        }

        private List<string> GetSelectedNodeWithChildren()
        {
            List<string> alignmentIds = new List<string>();

            if (ddlAlignments.EmbeddedTree.SelectedNode != null)
            {
                alignmentIds.Add(ddlAlignments.EmbeddedTree.SelectedNode.Value);
                foreach (RadTreeNode node in ddlAlignments.EmbeddedTree.SelectedNode.Nodes)
                {
                    alignmentIds.Add(node.Value);
                }
            }

            return alignmentIds;
        }

        private void BindAlignmentsTree()
        {
            if (!IsPostBack)
            {
                DataTable dtAlignments = GetAlignmentsData();
                if (dtAlignments != null && dtAlignments.Rows.Count > 1)
                {
                    ddlAlignments.DataSource = dtAlignments;
                    ddlAlignments.DataBind();

                    if (ddlAlignments.EmbeddedTree.Nodes.Count > 0)
                    {
                        FormatParentNodes(ddlAlignments.EmbeddedTree.Nodes[0]);
                    }
                }
            }
        }

        private void FormatParentNodes(RadTreeNode node)
        {
            if (node.Text.Equals("All", StringComparison.InvariantCultureIgnoreCase) && node.Index <= 0)
            {
                node = node.Next;
            }
            if (node.Nodes.Count > 0)
            {
                node.Font.Bold = true;
                foreach (RadTreeNode childNode in node.Nodes)
                {
                    FormatParentNodes(childNode);
                }
                if (node.Next != null)
                {
                    node = node.Next;
                    FormatParentNodes(node);
                }
            }
        }

        private DataTable GetAlignmentsData()
        {
            return Base.Classes.Credentials.GetAllAlignments();
        }
        protected void CommonStandardTree_OnNodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            DataRowView dataSourceRow = (DataRowView)e.Node.DataItem;
            e.Node.Attributes["ButtonText"] = dataSourceRow["CredentialAlignment"].ToString();
        }

       

        protected void ddlAlignments_DataBound(object sender, EventArgs e)
        {
            ((RadDropDownTree)sender).ExpandAllDropDownNodes();
        }

        protected void ddlAlignments_EntryAdded(object sender, DropDownTreeEntryEventArgs e)
        { }

        protected void ddlAlignments_NodeDataBound(object sender, DropDownTreeNodeDataBoundEventArguments e)
        {
            DataRowView row = (DataRowView)e.DropDownTreeNode.DataItem;
            if (e.DropDownTreeNode.Text.Equals("All", StringComparison.InvariantCultureIgnoreCase))
            {
                RadTreeNode node = ddlAlignments.EmbeddedTree.Nodes.FindNodeByValue(e.DropDownTreeNode.Value);
                if (node != null)
                {
                    node.Font.Bold = true;
                }
            }
        }
    }
}