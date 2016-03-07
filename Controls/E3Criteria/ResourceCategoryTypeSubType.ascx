<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceCategoryTypeSubType.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.ResourceCategoryTypeSubType" %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckBoxList.ascx"  %>

<e3:CheckBoxList ID="chkCategory" CriteriaName="Category" runat="server" Text="Category"/>
<e3:CheckBoxList ID="chkType" CriteriaName="Type" runat="server" Text="Type"/>
<e3:CheckBoxList ID="chkSubtype" CriteriaName="Subtype" runat="server" Text="Subtype"/>

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        CategoryPos: 0,
        TypePos: 1,
        SubtypePos:  2,

        PopulateControls: function () {
            var data = <%=CriteriaName%>DependencyData;
            var _this = <%=CriteriaName%>Controller;
            
            var categories = CriteriaDataHelpers.GetFieldDistinct(data, _this.CategoryPos);
            CategoryController.PopulateList(categories);

            var selectedCategories = CriteriaController.GetValuesAsSinglePropertyArray("Category", "Value");
            var filteredTypePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedCategories]);
            var filteredTypes = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredTypePos, _this.TypePos);
            TypeController.PopulateList(filteredTypes);
            
            var selectedTypes = CriteriaController.GetValuesAsSinglePropertyArray("Type", "Value");
            var filteredSubtypePos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedCategories, selectedTypes]);
            var filteredSubtype = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSubtypePos, _this.SubtypePos);
            SubtypeController.PopulateList(filteredSubtype);
            
        },
        
        OnChange: function () {
            var _this = <%=CriteriaName%>Controller;
            _this.PopulateControls();
        }
    }
</script>