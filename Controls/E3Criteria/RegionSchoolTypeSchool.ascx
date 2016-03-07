<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="RegionSchoolTypeSchool.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.RegionSchoolTypeSchool" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="SchoolTypeSchool" Src="~/Controls/E3Criteria/SchoolTypeSchool.ascx" %>

<e3:DropDownList ID="cmbRegion" CriteriaName="Region" runat="server" Text="Region" EmptyMessage="Select a Region" DataTextField="Name" DataValueField="Value" />
<e3:DropDownList ID="cmbSchoolType" CriteriaName="SchoolType" runat="server" Text="School Type" EmptyMessage="Select a School Type" />
<e3:DropDownList ID="cmbSchool" CriteriaName="School" runat="server" Text="School"  EmptyMessage="Select a School" />


<script type="text/javascript">
    var RestrictValueOptions = {};
   var <%=CriteriaName%>Controller = {
        InitialLoad: true,
        SchoolTypePos: 0,
        SchoolPos: 1,
        isSingle: 0,

        PopulateSchoolTypeControls: function (sourceCriteriaName, args) {
            var data = <%=CriteriaName%>DependencyData;
            var _this = <%=CriteriaName%>Controller;
            
            var SchoolTypes = CriteriaDataHelpers.GetFieldDistinct(data, _this.SchoolTypePos);
            if (typeof sourceCriteriaName == "undefined" || typeof sourceCriteriaName == "object") {
                sourceCriteriaName = 'SchoolType';
            }
            if (sourceCriteriaName == 'SchoolType') {
                <%=cmbSchoolType.CriteriaName%>Controller.PopulateList(SchoolTypes);
            }
            
          
            if (SchoolTypes.length == 1) {
                if (_this.isSingle == 0)
                    <%=cmbSchoolType.CriteriaName%>Controller.DefaultTexts = [SchoolTypes[0]];
            }

            // filter school based on school type
            var selectedSchoolTypes = CriteriaController.GetValuesAsSinglePropertyArray("SchoolType", "Text");

            var filteredSchools;
            if (selectedSchoolTypes == "All") {
                var filteredSchoolPos = CriteriaDataHelpers.GetFilteredDataPositions(data, []);
                filteredSchools = CriteriaDataHelpers.GetFilteredFieldDistinctTextValue(data, filteredSchoolPos, _this.SchoolPos);
            }
            else
            {
                var filteredSchoolPos = CriteriaDataHelpers.GetFilteredDataPositions(data, [selectedSchoolTypes]);
                filteredSchools = CriteriaDataHelpers.GetFilteredFieldDistinctTextValue(data, filteredSchoolPos, _this.SchoolPos,2);
            }
            if (typeof sourceCriteriaName != "undefined") <%=cmbSchool.CriteriaName%>Controller.PopulateList(filteredSchools.sort(), 0, 1);

           

            if (filteredSchools.length == 1) {
                if (_this.isSingle == 0)
                    <%=cmbSchool.CriteriaName%>Controller.DefaultTexts = [filteredSchools[0][0]];
            }

            if (_this.InitialLoad) {
                _this.InitialLoad = false;
                if (SchoolTypes.length == 1 || filteredSchools.length == 1) {
                    _this.isSingle = 1;
                }
                <%=cmbSchoolType.CriteriaName%>Controller.CheckDefaultTexts();
                <%=cmbSchool.CriteriaName%>Controller.CheckDefaultTexts();
               
            }
        },

       OnSchoolTypeChange: function () {
           var _this = <%=CriteriaName%>Controller;
           if (_this.isSingle == 0) {           
            _this.PopulateSchoolTypeControls('School');
           
                var Region = CriteriaController.GetValuesAsSinglePropertyArray("Region", "Text");
                var schooltype = CriteriaController.GetValuesAsSinglePropertyArray("SchoolType", "Text");
                PopulateUsageReportGrade(Region, schooltype, null);
            }
        },
        
        OnSchoolChange: function () {
            var _this = <%=CriteriaName%>Controller;
             _this.PopulateSchoolControls('School');
        },

        OnRegionChange: function (){
          //  var _this = <%=CriteriaName%>Controller;
           // var Region = CriteriaController.GetValuesAsSinglePropertyArray("Region", "Text");
         //   PopulateUsageReportGrade(Region, null, null);
        },

       PopulateSchoolControls: function (sourceCriteriaName, args) {
           var _this = <%=CriteriaName%>Controller;
            if (_this.isSingle == 0) {
                var Region = CriteriaController.GetValuesAsSinglePropertyArray("Region", "Text");
                var schooltype = CriteriaController.GetValuesAsSinglePropertyArray("SchoolType", "Text");
                var school = CriteriaController.GetValuesAsSinglePropertyArray("School", "Value");
                PopulateUsageReportGrade(Region, schooltype, school);
            }
        }
   
    }
    
</script>