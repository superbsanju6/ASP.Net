<%@ Page Title="" Language="C#" MasterPageFile="~/KenticoSearch.Master" AutoEventWireup="true" CodeBehind="ResourceSearchKentico.aspx.cs" Inherits="Thinkgate.Controls.Resources.ResourceSearchKentico" EnableEventValidation="false" %>

<%@ Import Namespace="Thinkgate.Classes" %>
<%@ Register Src="~/Controls/E3Criteria/SearchControl.ascx" TagPrefix="ksc" TagName="SearchControl" %>


<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">  

 
    <style>
        ::-ms-clear {
         display: none;
        }

         .ui-dialog {
             /*width: auto !important;*/
             /*min-height: 30px !important;*/
             background-color: #ccc !important;
             border: 1px solid black;
             -webkit-border-radius: 4px;
             -moz-border-radius: 4px;
             border-radius: 4px;
             padding-left: 3px;
             margin-left: 10px;
             height: auto !important;
             position: absolute;
         }

        #divSearchControl_Container {
            background: #808080 !important;
            /*margin-top: 5px;*/
        }

        .chkboxList {
            margin-top: 1px;
            height: 25px;
            width: 180px;
            line-height: 25px;
            display: block;
            clear: both;
            border: 1px solid black;
            border-top-left-radius: 10px;
            border-top-right-radius: 30px;
            border-bottom-left-radius: 30px;
            border-bottom-right-radius: 10px;
            background-color: white;
        }

        .tooltip-block {
            cursor: pointer;
        }
        
        img {
            border: none;
        }

        .resourceButton {
            margin-top: 1em;
        }

        .resourceIcon {
            padding-top: .5em;
            cursor: pointer;
        }

        /*override to show borders form selected and hovered row*/
        .RadGrid_Web20 .rgSelectedRow td, .RadGrid_Web20 .rgHoveredRow td {
            border-color: #FFFFFF #FFFFFF #CFD9E7 #829CBF;
            border-style: solid;
            border-width: 0 1px 1px;
            padding-left: 7px;
            padding-right: 7px;
        }

        .disabledButton {
            -khtml-opacity: .40;
            -moz-opacity: .40;
            -ms-filter: "alpha(opacity=40)";
            filter: alpha(opacity=30);
            filter: progid:DXImageTransform.Microsoft.Alpha(opacity=40);
            opacity: .40;
        }

        #divToolTipTarget_InteractivityType {
            height: 100px !important;
        }

        #leftColumn {
            overflow-y: hidden !important;
        }
            
        #divSearchControl_Container {
            height: 509px !important;
        }

        #MainContent_SearchLMRIControl_divCriteriaPlaceHolder {
            height: 500px !important;
        }

        @-moz-document url-prefix() {
         #leftColumn {
                overflow-y: hidden !important;
        }
            
            #divSearchControl_Container {
                height: 501px !important;
        }

            #MainContent_SearchLMRIControl_divCriteriaPlaceHolder {
                height: 488px !important;
        }
    }

        .rgDataDiv {
            height:505px !important;
        }
    </style>


        <telerik:RadWindowManager ID="TelerikWindowManager" runat="server">   
    </telerik:RadWindowManager> 
     

    <div id="MainWrapperDiv" runat="server" style="width: 100%; height: 100%!important;">
		<div style="height: 100%;">
			<div id="wrapperColumnRight" style="float: right; width: 100%; margin-left: -210px; height: 100%">
                <div id="leftColumn" style="width: 200px; float: left; height: 100%; background-color: #848484; overflow-x: hidden; overflow-y: auto; position: static;">
	                <asp:UpdatePanel ID="UpdatePanelButtons" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <div style="text-align: center;">
                                <div style="display: block; margin: 0">
                                    <asp:ImageButton ID="Button11" runat="server" OnClientClick="hideToolTips(); return checkSourceSelection();" OnClick="btnGetValue_Click" ImageUrl="../../Images/search-1.png" />
                                    <asp:ImageButton ID="Button22" runat="server" OnClientClick="resetSearch();  resetResourceSearch(); return true;" OnClick="btnReset_Click" ImageUrl="../../Images/clear-1.png" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>	

                    <div id="criteriaListTabs">
                        <ul>
                            <li><a href="#basicCriteriaListDiv" onclick="closeCurrentDialogImmediately();">Resources</a></li>
                            <li><a href="#LMRICriteriaListDiv" clientidmode="Static" id="tabLRMI" runat="server" onclick="closeCurrentDialogImmediately();">Tags</a></li>
                        </ul>
                        <div id="basicCriteriaListDiv">
                             <asp:UpdatePanel ID="UpdatePanelSearchControl" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div style="margin-left: -20px">
                                        <ksc:SearchControl runat="server" ID="SearchControl" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                        <div id="LMRICriteriaListDiv">
                            <asp:UpdatePanel ID="UpdatePanelLMRISearchControl" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div style="margin-left: -20px">
                                        <ksc:SearchControl runat="server" ID="SearchLMRIControl" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

            <div id="columnExpander" style="width: 10px; float: left; height: 610px; background-color: #CCCCCC">
                    <div id="columnExpanderHandle" onclick="toggleSidebar()" style="cursor: pointer; height: 100px; background-color: #0F3789; position: relative; top: 42%">
                        <img id="columnExpanderHandleImage" style="position: relative; left: 1px; top: 42%; width: 8px" src="../../Images/arrow_gray_left.gif" alt="Column Handler" />
				</div>
			</div>

        <div id="rightColumn" style="margin-left: 210px; height: 100%; width: 100%">
             <div style="float: left">
                <asp:UpdateProgress ID="UpdateProgressResults" runat="server">
                    <ProgressTemplate>
                                <div style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; background-color: white; z-index: 9999999; filter: alpha(opacity=70);">
                            <asp:Image ID="imgUpdateProgress" Width="43" Height="11" runat="server" ImageUrl="~/Styles/Thinkgate_Window/Common/loading.gif" Style="padding: 10px; position: fixed; top: 15%; left: 50%;" />
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:UpdatePanel ID="UpdatePanelSearchResults" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <div>
                            <asp:Label ID="Label1" runat="server" ForeColor="Red" Font-Bold="true" Text="" Visible="false"></asp:Label>
                               <telerik:RadGrid runat="server" ID="radGridResults" AutoGenerateColumns="False" Width="680"
                        AllowFilteringByColumn="False" PageSize="20" AllowPaging="True" AllowSorting="True" Height="610"
                                        OnPageIndexChanged="RadGridResults_PageIndexChanged" AllowMultiRowSelection="true" OnPageSizeChanged="RadGridResults_PageSizeChanged"
                                        OnItemDataBound="radGridResults_ItemDataBound" OnSortCommand="OnSortCommand" Skin="Web20" OnNeedDataSource="RadGrid_NeedDataSource">
                                        <PagerStyle Mode="NextPrev"></PagerStyle>
                    <ClientSettings EnableRowHoverStyle="true">
                                            <Resizing AllowColumnResize="true" AllowResizeToFit="true" ResizeGridOnColumnResize="true" ClipCellContentOnResize="true" />
                        <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="true" />
                                            <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="550px"></Scrolling>
                    </ClientSettings>
                                        <MasterTableView TableLayout="Fixed" AllowNaturalSort="True">
                        <Columns>

                                                <telerik:GridTemplateColumn HeaderText="Student Assignments" DataField="StudentAssignment" UniqueName="StudentAssignment" ItemStyle-Font-Size="Small" HeaderStyle-Width="85">
                                                    <ItemTemplate>
                                                        <a id="lnkStudentAssignment" href="#" onclick="ManageAssignment('<%#  Convert.ToString( Eval("DocumentNodeID")) %>','<%# GroupID %>','<%# ClassID %>','<%# Eval("ExpirationDate","{0:MM/dd/yy}")=="01/01/01" || Eval("ExpirationDate","{0:MM/dd/yy}")=="12/31/99" ? false : Convert.ToDateTime( Eval("ExpirationDate")).Date< DateTime.Now.Date %>','<%# Eval("ExpirationDate").ToString() %>')">Manage Assignment</a>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>

                            <telerik:GridBoundColumn DataField="Source" HeaderText="Source" HeaderStyle-Width="70" ShowSortIcon="true" ItemStyle-Font-Size="Small" />

                                                <telerik:GridTemplateColumn HeaderText="Name"  DataField="ResourceNameReadOnly" UniqueName="ResourceNameReadOnly" ItemStyle-Font-Size="Small" HeaderStyle-Width="165">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblResourceName" runat="server"><%# Eval("ResourceNameReadOnly")%></asp:Label>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>


                            <telerik:GridTemplateColumn HeaderText="Name" DataField="ResourceName" UniqueName="Name" ItemStyle-Font-Size="Small" HeaderStyle-Width="165">
                                <ItemTemplate>
                                    <label runat="server" id="lblName"></label>

                                    <a id="A1" style="text-decoration: underline; cursor: pointer" onclick="Resource_onClick('<%# Eval("Source")%>', '<%# Convert.ToString(Eval("Source"))=="PBS Learning Media"?Eval("URL"):Eval("ID_Encrypted")%>', '<%# Eval("NodeAliasPath")%>');"><%# Eval("ResourceName")%></a>
                                                        <br />
                                    
                                                        <asp:ImageButton ID="AddAsNewMyDocsResource" runat="server" ImageUrl="~/Images/resources.png" Width="16" ToolTip="Add Resource to My Docs" OnClick="AddResource_Click" CommandArgument='<%# Eval("Source")+"|@|"+Eval("ID_Encrypted")+"|@|"+Eval("NodeAliasPath")+"|@|"+Eval("ResourceName")+"|@|"+Eval("Description")+"|@|"+Eval("URL")%>' />

                                    <%--<asp:Button runat="server" ID="AddAsNewMyDocsResource" Text="Add to My Docs" Visible="True" CssClass="resourceButton" ToolTip="Add Resource to My Docs" OnClick="AddResource_Click"/>--%>
                                    <%--<br/>--%>
                                    <%--<asp:Button runat="server" ID="AddAsNewMyDocsResource"      Text="M" Visible="True" CssClass="resourceButton" ToolTip="Add Resource to My Docs" />--%>
                                    <%--<asp:Button runat="server" ID="AddAsNewDistrictResource"    Text="D" Visible="True" CssClass="resourceButton" ToolTip="Add Resource to District"/>--%>
                                    <%--<asp:Button runat="server" ID="AddAsNewStateResource"       Text="S" Visible="True" CssClass="resourceButton" ToolTip="Add Resource to State"/>--%>
                                    <%--<asp:Button runat="server" ID="AddAsNewResourceButton" Text="Add as New Resource" Visible="False" CssClass="resourceButton"/>--%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="View" DataField="ViewLink" HeaderStyle-Width="45" ItemStyle-Font-Size="Small" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                                        <a href="<%# Eval("ViewLink") %>" target="_blank">
                                                            <asp:Image ID="Image1" runat="server" ImageUrl="../Images/ViewPage.png" Width="27" /></a>
                                    <asp:HyperLink ID="ViewLinkAnchor" runat="server" ImageUrl="../Images/ViewPage.png" Width="27" Visible="False"></asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            

                            <telerik:GridDateTimeColumn DataField="Created" HeaderText="Created" HeaderStyle-Width="100" ShowSortIcon="true" ItemStyle-Font-Size="Small" Visible="False" />
                            <telerik:GridBoundColumn DataField="Creator" HeaderText="Author/Creator" HeaderStyle-Width="100" ShowSortIcon="true" ItemStyle-Font-Size="Small" Visible="False" />
                            <telerik:GridBoundColumn DataField="Publisher" HeaderText="Publisher" HeaderStyle-Width="100" ShowSortIcon="true" ItemStyle-Font-Size="Small" Visible="False" />
                            
                            <telerik:GridBoundColumn DataField="Type" HeaderText="Type" ShowSortIcon="true" ItemStyle-Font-Size="Small" HeaderStyle-Width="50" />
                            <telerik:GridBoundColumn DataField="Subtype" HeaderText="Subtype" ShowSortIcon="true" ItemStyle-Font-Size="Small" HeaderStyle-Width="40" />
                                                <telerik:GridBoundColumn DataField="Description" HeaderText="Description" ShowSortIcon="true" ItemStyle-Font-Size="Small" HeaderStyle-Width="100" />
                            <telerik:GridTemplateColumn HeaderText="Expiration Date" DataField="ExpirationDate" UniqueName="ExpirationDate" ItemStyle-Font-Size="Small" HeaderStyle-Width="60">
                                <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblExpirationDate"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Average Rating" DataField="AverageRating" UniqueName="AverageRating" ItemStyle-Font-Size="Small" HeaderStyle-Width="50" SortExpression="AverageRating">
                                <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblAverageRating"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
                </div>
            </div>
        </div>

    <div style="float: left; margin-left: 10px; max-width: 80%;">
       
                    <asp:Label ID="ErrorMessages" runat="server" ForeColor="Red" Font-Bold="true" Text="" Visible="false"></asp:Label>
                 


                
        <asp:PlaceHolder ID="initialDisplayText" runat="server"></asp:PlaceHolder>

                </div>
           
    <div>
        <table id="standardsData" class="display" style="width: 95%; border: 0"></table>
    </div>
    <%-- ReSharper disable once UnusedParameter --%>
    <script type="text/javascript">
        
        var NationalLearningRegistry = "National Learning Registry";
        var PBSLearningMedia = "PBS Learning Media";
        var DefaultRun = '<%:this.DefaultRun%>';

        var TeachersMyDocsfolder = "TeachersMyDocsfolder";        
        $(function () {
            $("#divSchoolGradeName").hide();
            $("#criteriaListTabs").tabs();
            setInitialCriteria();
        });

        function setInitialCriteria() {
            //Disable "keywords"
            keywordsSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_Keywords").onclick = "";            
        }

        $(document).ready(function () {
            window.top["findFrameWindowByUrl"] = function (url) {
                if (window.top && window.top.frames && window.top.frames.length && window.top.frames.length > 0) {
                    for (var i = 0; i <= window.top.frames.length; i++) {
                        var framePath = window.top.frames[i].location.pathname;
                        if (framePath.indexOf(url) > 0) {
                            return window.top.frames[i].window;
                        }
                    }
                }
                return null;
            };

            if (DefaultRun.toLowerCase() == "true") {
                setTimeout(function () {
                    <%= this.Page.ClientScript.GetPostBackEventReference(this.Button11, "DefaultRun") %>
                }, 0);
            }
        });
        // When the user selects to search by external source
        // gray out other selections option to avoid using searches
        // that are unavailible to an outside API
        var keywordsSelection = document.getElementById('MainContent_SearchControl_Keywords_TextBoxEdit_CriteriaHeaderText').parentNode.parentNode.parentNode;

        var sourceSelection = document.getElementById('MainContent_SearchControl_Source_DropdownList_ddl');
        var typeSelection = document.getElementById('MainContent_SearchControl_Type_DropdownList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var subtypeSelection = document.getElementById('MainContent_SearchControl_SubType_DropdownList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var curriculumSelection = document.getElementById('MainContent_SearchControl_Curriculum_GradesSubjectsCurricula_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var standardsSelection = document.getElementById('MainContent_SearchControl_Standards_GradesSubjectsStandards_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var expirationDateSelection = document.getElementById('MainContent_SearchControl_ExpirationDate_DateRange_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var averageRatingSelection = document.getElementById('MainContent_SearchControl_AverageRating_CheckBoxList_CriteriaHeaderText').parentNode.parentNode.parentNode;

        var assessedSelection = document.getElementById('MainContent_SearchLMRIControl_Assessed_GradesSubjectsStandards_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var teachesSelection = document.getElementById('MainContent_SearchLMRIControl_Teaches_GradesSubjectsStandards_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var requiresSelection = document.getElementById('MainContent_SearchLMRIControl_Requires_GradesSubjectsStandards_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var textComplexitySelection = document.getElementById('MainContent_SearchLMRIControl_TextComplexity_TextBoxEdit_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var readingLevelSelection = document.getElementById('MainContent_SearchLMRIControl_ReadingLevel_TextBoxEdit_CriteriaHeaderText').parentNode.parentNode.parentNode;

        var gradesSelection = document.getElementById('MainContent_SearchLMRIControl_Grades_CheckBoxList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var mediaTypeSelection = document.getElementById('MainContent_SearchLMRIControl_MediaType_CheckBoxList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var languageSelection = document.getElementById('MainContent_SearchLMRIControl_Language_DropdownList_CriteriaHeaderText').parentNode.parentNode.parentNode;

        var educationalSubjectSelection = document.getElementById('MainContent_SearchLMRIControl_EducationalSubject_CheckBoxList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var educationalUseSelection = document.getElementById('MainContent_SearchLMRIControl_EducationalUse_CheckBoxList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var activityTypeSelection = document.getElementById('MainContent_SearchLMRIControl_InteractivityType_CheckBoxList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var learningResourceSelection = document.getElementById('MainContent_SearchLMRIControl_LearningResource_CheckBoxList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var targetAudienceSelection = document.getElementById('MainContent_SearchLMRIControl_EndUser_CheckBoxList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var durationSelection = document.getElementById('MainContent_SearchLMRIControl_Duration_Duration_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var ageAppropriateSelection = document.getElementById('MainContent_SearchLMRIControl_AgeAppropriate_DropdownList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var publisherSelection = document.getElementById('MainContent_SearchLMRIControl_Publisher_TextBoxEdit_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var creatorSelection = document.getElementById('MainContent_SearchLMRIControl_Creator_TextBoxEdit_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var usageRightsSelection = document.getElementById('MainContent_SearchLMRIControl_UsageRights_DropdownList_CriteriaHeaderText').parentNode.parentNode.parentNode;
        var SchoolGradeNameSelection = document.getElementById('MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_CriteriaHeaderText').parentNode.parentNode.parentNode;

        sourceSelection.onchange = function () {

            //code written to make teacher options selected value 0 e.g make it default on changes 
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option[value='0']").attr("selected", "selected");
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades option[value='0']").attr("selected", "selected");
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option[value='0']").attr("selected", "selected");

            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option[value='0']").prop("selected", true);
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades option[value='0']").prop("selected", true);
            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option[value='0']").prop("selected", true);


            $('#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades').children('option').remove();
            $('#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames').children('option').remove();

            $('#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades').children().remove();
            $('#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames').children().remove();

            $('#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades').append($('<option></option>').val("0").html("Select Grade"));
            $('#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames').append($('<option></option>').val("0").html("Select Name"));

            $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool").val('0').attr("selected", "selected");

            resetResourceSearch();
            $(document.getElementById("MainContent_SearchLMRIControl_MediaType_CheckBoxList_chk")).empty();
            addToCheckBoxListControl(document.getElementById("MainContent_SearchLMRIControl_MediaType_CheckBoxList_chk"), lrmiMedicaJsonObject);

            if (sourceSelection.value != "0") {
                switch (sourceSelection.value) {
                case "District":
                case "MyDocs":
                case "State":
                case "Internal Source":
                        $("#divSchoolGradeName").hide();
                        $("#divToolTipTarget_SchoolGradeName_Selected").hide();
                break;
                case NationalLearningRegistry:
                    resetResourceSearchByType(NationalLearningRegistry);
                        $("#divSchoolGradeName").hide();
                        $("#divToolTipTarget_SchoolGradeName_Selected").hide();
                break;
                case PBSLearningMedia:
                    resetResourceSearchByType(PBSLearningMedia);
                        $("#divSchoolGradeName").hide();
                        $("#divToolTipTarget_SchoolGradeName_Selected").hide();
                        break;
                    case TeachersMyDocsfolder:
                        $("#divSchoolGradeName").show();
                break;
                default:
                        $("#divSchoolGradeName").hide();
                        $("#divToolTipTarget_SchoolGradeName_Selected").hide();

                break;
                }
            } else {
                $("#divSchoolGradeName").hide();
                $("#divToolTipTarget_SchoolGradeName_Selected").hide();
            }

            window.closeToolTip(this);
        };


        function resetResourceSearchByType(type) {
            typeSelection.style.backgroundColor = 'gray';
            document.getElementById('divToolTipSource_Type').onclick = "";
            subtypeSelection.style.backgroundColor = 'gray';
            document.getElementById('divToolTipSource_SubType').onclick = "";
            curriculumSelection.style.backgroundColor = 'gray';
            document.getElementById('divToolTipSource_Curriculum').onclick = "";
            standardsSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_Standards").onclick = "";
            expirationDateSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_ExpirationDate").onclick = "";
            removeCriteria(document.getElementById("divToolTipTarget_ExpirationDate_Selected"));
            averageRatingSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_AverageRating").onclick = "";
            removeCriteria(document.getElementById("divToolTipTarget_AverageRating_Selected"));
            assessedSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_Assessed").onclick = "";
            teachesSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_Teaches").onclick = "";
            requiresSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_Requires").onclick = "";
            textComplexitySelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_TextComplexity").onclick = "";
            readingLevelSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_ReadingLevel").onclick = "";
            educationalSubjectSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_EducationalSubject").onclick = "";
            learningResourceSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_LearningResource").onclick = "";
            durationSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_Duration").onclick = "";
            educationalUseSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_EducationalUse").onclick = "";
            document.getElementById("divToolTipSource_SchoolGradeName").onclick = "";
            
            if (type == PBSLearningMedia) {
                creatorSelection.style.backgroundColor = 'gray';
                document.getElementById("divToolTipSource_Creator").onclick = "";
                publisherSelection.style.backgroundColor = 'gray';
                document.getElementById("divToolTipSource_Publisher").onclick = "";
                ageAppropriateSelection.style.backgroundColor = 'gray';
                document.getElementById("divToolTipSource_AgeAppropriate").onclick = "";
                activityTypeSelection.style.backgroundColor = 'gray';
                document.getElementById("divToolTipSource_InteractivityType").onclick = "";
                targetAudienceSelection.style.backgroundColor = 'gray';
                document.getElementById("divToolTipSource_EndUser").onclick = "";
                usageRightsSelection.style.backgroundColor = 'gray';
                document.getElementById("divToolTipSource_UsageRights").onclick = "";
                $(document.getElementById("MainContent_SearchLMRIControl_MediaType_CheckBoxList_chk")).empty();
                addToCheckBoxListControl(document.getElementById("MainContent_SearchLMRIControl_MediaType_CheckBoxList_chk"), pbsMedicaJsonObject);
            }

            if (type == NationalLearningRegistry) {
                // Enable "Keywords"
                keywordsSelection.style.backgroundColor = '#fff';
                document.getElementById("divToolTipSource_Keywords").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Keywords'); };
                
                gradesSelection.style.backgroundColor = 'gray';
                document.getElementById("divToolTipSource_Grades").onclick = "";
                mediaTypeSelection.style.backgroundColor = 'gray';
                document.getElementById("divToolTipSource_MediaType").onclick = "";
            }
        }

        function resetResourceSearch() {
            //Reset the source selection
            if ($('#divToolTipTarget_Source_Selected_Text') != undefined && $('#divToolTipTarget_Source_Selected_Text')) {
                $('#divToolTipTarget_Source_Selected_Text').text("");
            }

            //Disable "keywords"
            keywordsSelection.style.backgroundColor = 'gray';
            document.getElementById("divToolTipSource_Keywords").onclick = "";

            typeSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Type").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Type'); };
            subtypeSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_SubType").onclick = function () { window.showTooltip(this, 'divToolTipTarget_SubType'); };
            curriculumSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Curriculum").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Curriculum'); };
            standardsSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Standards").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Standards'); };
            expirationDateSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_ExpirationDate").onclick = function () { window.showTooltip(this, 'divToolTipTarget_ExpirationDate'); };
            averageRatingSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_AverageRating").onclick = function () { window.showTooltip(this, 'divToolTipTarget_AverageRating'); };

            assessedSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Assessed").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Assessed'); };
            teachesSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Teaches").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Teaches'); };
            requiresSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Requires").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Requires'); };
            textComplexitySelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_TextComplexity").onclick = function () { window.showTooltip(this, 'divToolTipTarget_TextComplexity'); };
            readingLevelSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_ReadingLevel").onclick = function () { window.showTooltip(this, 'divToolTipTarget_ReadingLevel'); };

            gradesSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Grades").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Grades'); };
            mediaTypeSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_MediaType").onclick = function () { window.showTooltip(this, 'divToolTipTarget_MediaType'); };
            languageSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Language").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Language'); };

            educationalSubjectSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_EducationalSubject").onclick = function () { window.showTooltip(this, 'divToolTipTarget_EducationalSubject'); };
            educationalUseSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_EducationalUse").onclick = function () { window.showTooltip(this, 'divToolTipTarget_EducationalUse'); };
            activityTypeSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_InteractivityType").onclick = function () { window.showTooltip(this, 'divToolTipTarget_InteractivityType'); };
            learningResourceSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_LearningResource").onclick = function () { window.showTooltip(this, 'divToolTipTarget_LearningResource'); };
            targetAudienceSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_EndUser").onclick = function () { window.showTooltip(this, 'divToolTipTarget_EndUser'); };
            durationSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Duration").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Duration'); };
            ageAppropriateSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_AgeAppropriate").onclick = function () { window.showTooltip(this, 'divToolTipTarget_AgeAppropriate'); };
            creatorSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Creator").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Creator'); };
            publisherSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_Publisher").onclick = function () { window.showTooltip(this, 'divToolTipTarget_Publisher'); };
            usageRightsSelection.style.backgroundColor = '#fff';
            document.getElementById("divToolTipSource_UsageRights").onclick = function () { window.showTooltip(this, 'divToolTipTarget_UsageRights'); };
            document.getElementById("divToolTipSource_SchoolGradeName").onclick = function () { window.showTooltip(this, 'divToolTipTarget_SchoolGradeName'); };
            
            removeCriteria(document.getElementById("divToolTipTarget_MediaType_Selected_Text"));
            
            if ($("#divSchoolGradeName")) {
                $("#divSchoolGradeName").hide();

                $("#hdnIsTeacherClick").val('NO');

                //code written to make teacher options selected value 0 e.g make it default on changes 
                $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool option[value='0']").attr("selected", "selected");
                $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades option[value='0']").attr("selected", "selected");
                $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option[value='0']").attr("selected", "selected");

                $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades option[value='0']").empty();
                $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option[value='0']").empty();

                $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades option[value='0']").append($('<option></option>').val("0").html("Select Grade"));
                $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames option[value='0']").append($('<option></option>').val("0").html("Select Name"));

            }

        }
        // Check to ensure that the user has selected a source option
        function checkSourceSelection() {

            //added condition to check to see Selected Tags is Resource
            if ($("#criteriaListTabs ul").find("li[aria-selected='true']").find('a').html() == 'Resources') {

            var dialogSelectedText = $('#divToolTipTarget_Source_Selected_Text');
                var dialogSelectedTextSchoolGradName = $("#divToolTipTarget_SchoolGradeName_Selected_Text");

                if ($('#divSchoolGradeName').css('display') != 'none') {
                    if ($(dialogSelectedText).text() == '' || $(dialogSelectedTextSchoolGradName).text() == '' || $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaSchool").val() == 0 || $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaGrades").val() == 0 || $("#MainContent_SearchControl_SchoolGradeName_SchoolsGradesNames_ddlCriteriaNames").val() == 0) {
                        window.radalert("Please review required fields (<span style=\"color:red\">*</span>) and ensure all entries are made before selecting Search.", 350, 47);
                        if ($("#hdnIsTeacherClick").val() == 'YES') { $("#divToolTipSource_SchoolGradeName").trigger("click"); }
                        return false;
                    }
                }
                else {
                    if ($('#divSchoolGradeName').css('display') == 'none' && $(dialogSelectedText).text() == '') {
                        window.radalert("Please review required fields (<span style=\"color:red\">*</span>) and ensure all entries are made before selecting Search.", 350, 47);
                return false;
            }
                }

            if ($(dialogSelectedText).text() == NationalLearningRegistry && !doesAnyNLRCriteriaExist()) {
                window.radalert("Please select at least one additional criteria", 320, 47);
                return false;
            }
            return true;
        }
        }

        function doesAnyNLRCriteriaExist() {

            if ($('#divToolTipTarget_Text_Selected_Text') != undefined && $('#divToolTipTarget_Text_Selected_Text').text() != "") {
                return true;
            }
            if ($('#divToolTipTarget_Keywords_Selected_Text') != undefined && $('#divToolTipTarget_Keywords_Selected_Text').text() != "") {
                return true;
            }

            if ($('#divToolTipTarget_EndUser_Selected_Text') != undefined && $('#divToolTipTarget_EndUser_Selected_Text').text() != "") {
                return true;
            }
            if ($('#divToolTipTarget_Creator_Selected_Text') != undefined && $('#divToolTipTarget_Creator_Selected_Text').text() != "") {
                return true;
            }

            if ($('#divToolTipTarget_Publisher_Selected_Text') != undefined && $('#divToolTipTarget_Publisher_Selected_Text').text() != "") {
                return true;
            }
            if ($('#divToolTipTarget_UsageRights_Selected_Text') != undefined && $('#divToolTipTarget_UsageRights_Selected_Text').text() != "") {
                return true;
            }

            if ($('#divToolTipTarget_Language_Selected_Text') != undefined && $('#divToolTipTarget_Language_Selected_Text').text() != "") {
                return true;
            }
            if ($('#divToolTipTarget_AgeAppropriate_Selected_Text') != undefined && $('#divToolTipTarget_AgeAppropriate_Selected_Text').text() != "") {
                return true;
            }

            return false;
        }

        function AddItemToMyDocs(title, description, url) {
            alert("Title: " + title + "\n\r" + "Description: " + description + "\n\r" + "URL: " + url + "\n\r");
        }

        // Process user selecting a document link
        function Resource_onClick(pSource, pUrl, pNodeAliasPath) {
            if (pSource == NationalLearningRegistry) {
                window.open(pUrl, "_blank");
            } else
                if (pSource == PBSLearningMedia) {
                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/GetExternalResource',
                    data: "{'url':'" + pUrl + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function () {
                        // alert(errorThrown);
                    },
                    success: function (result) {
                        window.open(result.d, "_blank");
                    }
                });
            } else {
                var aliasPath = '<%:KenticoHelper.KenticoVirtualFolderRelative%>' + pNodeAliasPath + ".aspx?viewmode=3";
                window.open(aliasPath, '_blank');
            }
        }

        // end change TFS-2887
        function populatesubtype(type, controlID) {
            var typeval = type.Value.Key;
            $.ajax({
                type: "POST",
                url: '<%=ResolveUrl("~/Services/KenticoServices/KenticoWebServices.aspx")%>/getsubtype',
                data: "{'type':'" + typeval + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (textStatus) {
                    // alert(errorThrown);
                },
                success: function (result) {
                    if (controlID && controlID.ControlID) {
                        var sValue = $("#" + controlID.ControlID).val();
                        $("#" + controlID.ControlID).html(result.d);
                        $("#" + controlID.ControlID).val(sValue);
                    }
                }
            });
        }

        function getSubjectList(standardSet, gradeIn, controlID) {
            var stdset = standardSet.Value;
            var grade = gradeIn.Value;
            $.ajax({
                type: "POST",
                url: "Services/WebServices/btWebServices.aspx/getStandardSetGradeSubject",
                data: "{'standardSet':'" + stdset + "', 'grade':'" + grade + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (xmlHttpRequest, textStatus, errorThrown) {
                    alert(errorThrown);
                },
                success: function (result) {
                    if (controlID && controlID.ControlID) {
                        var sValue = $("#" + controlID.ControlID).val();
                        $("#" + controlID.ControlID).html(result.d);
                        $("#" + controlID.ControlID).val(sValue);
                    }
                }
            });
        }

        function getCourseList(standardSet, gradeIn, subjectIn, controlID) {
            var stdset = standardSet.Value;
            var grade = gradeIn.Value;
            var subject = subjectIn.Value;
            $.ajax({
                type: "POST",
                url: "Services/WebServices/btWebServices.aspx/getStandardSetGradeSubjectCourse",
                data: "{'standardSet':'" + stdset + "', 'grade':'" + grade + "', 'subject':'" + subject + "'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function () {
                },
                success: function (result) {
                    if (controlID && controlID.ControlID) {
                        var sValue = $("#" + controlID.ControlID).val();
                        $("#" + controlID.ControlID).html(result.d);
                        $("#" + controlID.ControlID).val(sValue);
                    }
                }
            });
        }

        var discriptionColumnWidth = 0;
        function ResizeColumn(index) {
            var maxWidth = 700;
            var grid = window.$find("<%= radGridResults.ClientID %>");            
            var MasterTable = grid.get_masterTableView();
            var indexValue = MasterTable.getColumnByUniqueName(index).get_element().cellIndex;
            MasterTable.resizeColumn(indexValue, maxWidth);
            return false;
        }
        
        function toggleSidebar() {

            var $leftColumn = $('#leftColumn');
            var currentState = $leftColumn.attr('state');
            if (!currentState) {
                currentState = 'opened';
                $leftColumn.attr('state', currentState);
                $leftColumn.attr('orig_width', $leftColumn.width());
            }
            if (currentState == 'opened') {
                $leftColumn.css('overflow', 'hidden');
                $leftColumn.css('overflow-y', 'hidden');
                $leftColumn.stop().animate({ width: 0 }, {
                    duration: 500,
                    complete: function () {
                        $leftColumn.css('overflow', 'hidden');
                        $leftColumn.css('overflow-y', 'hidden');
                        $('#columnExpanderHandleImage').attr('src', '../../Images/arrow_gray_right.gif');
                    }, step: function (now) {
                        $('#rightColumn').css('margin-left', now + 10);
                        $('#<%= radGridResults.ClientID %>').css('width', 680 + (200 - now));
                    }
                });
                currentState = 'closed';
            } else {
                $leftColumn.css('overflow-y', 'auto');
                $leftColumn.stop().animate({ width: $leftColumn.attr('orig_width') }, {
                    duration: 500,
                    complete: function () {
                        $leftColumn.css('overflow-y', 'auto');
                        $('#columnExpanderHandleImage').attr('src', '../../Images/arrow_gray_left.gif');
                        $leftColumn.css('overflow', 'visible');
                    }, step: function (now) {
                        $('#rightColumn').css('margin-left', now + 10);
                        $('#<%= radGridResults.ClientID %>').css('width', 880 - now);
                    }
                });
                currentState = 'opened';
            }
            $leftColumn.attr('state', currentState);
        }

        function checkOpenerIdentityIsSearchPage() {
            // This is an empty function required to prevent the Resource Search Page from refreshing on closing a Plan
            // The function is referred in the RPage web parts in the Page Templates in Kentico.
        }


        //function OpenManageAssignment(DocumentID,GroupID,ClassID)
        //{
        //    var urlstring = "..//InstructionMaterial/ManageAssignments.aspx?DocumentID=" + DocumentID + "&GroupID="+ GroupID + "&ClassID=" + ClassID;
        //    parent.customDialog({ url: urlstring, name: "Manage Assignments", maximize: true, maxwidth: 935, maxheight: 900, title: 'Manage Assignments' });
        //}

        function ManageAssignment(DocumentID, GroupID, ClassID, isExpired, expirationdate) {
                    
         
            if (isExpired == 'True') {
                alert("Please extend the Usage Right Expiration Date before assigning Instruction Material to Students!");
                return false;
            }

            var ua = navigator.userAgent.toLowerCase();
            var yPos = 16;
            if (ua.indexOf(' chrome/') != -1) {
                yPos = 50;
            }
            else if (ua.indexOf(' safari/') != -1) {
                yPos = 50;
            }
            var urlstring = '../Controls/InstructionMaterial/ManageAssignments.aspx?DocumentNodeID=' + DocumentID + '&GroupID=' + GroupID + '&ClassID=' + ClassID;
            parent.customDialog({ url: urlstring, name: "Manage Assignments", maximize: true, xPos: 90, yPos: yPos, maxwidth: 1100, maxheight: 700, title: 'Manage Assignments' });
        }

      
        <%-- $(document).ready(function () {

           

            var IM = '<%= isInstructionMaterial %>';
            if(IM==true)
            {
                var wnd = getCurrentCustomDialog();
                wnd.title = "Instruction Search";
            }
        }
        );--%>


    </script> 
    
</asp:Content>



