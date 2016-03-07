<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SchoolTypeSchool.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.SchoolTypeSchool" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>

 <e3:DropDownList ID="cmbSchoolType" CriteriaName="SchoolType" runat="server" Text="School Type" EmptyMessage="None" />
 <e3:DropDownList ID="cmbSchool" CriteriaName="School" runat="server" Text="School"  EmptyMessage="None" />

<script type="text/javascript">
    var <%=CriteriaName%>Controller = {
        SchoolTypePos: 0,
        SchoolPos: 1,

        PopulateControls: function (sourceCriteriaName, args) {
            var data = <%=CriteriaName%>DependencyData;
            var _this = <%=CriteriaName%>Controller;
            
            var SchoolTypes = CriteriaDataHelpers.GetFieldDistinct(data, _this.SchoolTypePos);
            if (typeof sourceCriteriaName == "undefined" || typeof sourceCriteriaName == "object") {
                sourceCriteriaName = 'SchoolType';
            }
            if (sourceCriteriaName == 'SchoolType') {
                <%=cmbSchoolType.CriteriaName%>Controller.PopulateList(SchoolTypes);
            }

            // filter school based on school type
            var selectedSchoolTypes = CriteriaController.GetValuesAsSinglePropertyArray("SchoolType", "Text");
            
            var filteredSchoolPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedSchoolTypes]);
            var filteredSchools = CriteriaDataHelpers.GetFilteredFieldDistinct(data, filteredSchoolPos, _this.SchoolPos);
            if (typeof sourceCriteriaName != "undefined" && sourceCriteriaName != 'School') <%=cmbSchool.CriteriaName%>Controller.PopulateList(filteredSchools.sort());

            if (_this.InitialLoad) {
                _this.InitialLoad = false;
                <%=cmbSchoolType.CriteriaName%>Controller.CheckDefaultTexts();
                <%=cmbSchool.CriteriaName%>Controller.CheckDefaultTexts();
             }
        },
        
        OnChange: function () {
            var _this = <%=CriteriaName%>Controller;
            _this.PopulateControls('School');
        }
    }
</script>