<%@ Page Title="Results Analysis" Language="C#" AutoEventWireup="true" CodeBehind="AssessmentResults_ExpandedV2.aspx.cs" 
Inherits="Thinkgate.Controls.Assessment.AssessmentResults_ExpandedV2" MasterPageFile="~/Search.Master" %>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx"  %>
<%@ Register TagPrefix="e3" TagName="CheckBoxList" Src="~/Controls/E3Criteria/CheckboxList.ascx" %>
<%@ Register TagPrefix="e3" TagName="DropDownList" Src="~/Controls/E3Criteria/DropDownList.ascx" %>
<%@ Register TagPrefix="e3" TagName="Demographics" Src="~/Controls/E3Criteria/Demographics.ascx" %>

<%@ Register TagPrefix="e3" TagName="GradeSubjectCourseStandardSet" Src="~/Controls/E3Criteria/GradeSubjectCourseStandardSet.ascx"  %>
<%@ MasterType virtualpath="~/Search.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">   
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    <!-- criteria -->
    <e3:GradeSubjectCourseStandardSet runat="server" ID="ctrlGradeSubjectCourseStandardSet" CriteriaName="GradeSubjectCourseStandardSet"/>
    <e3:CheckBoxList ID="cblTerm" CriteriaName="Term" runat="server" Text="Term"/>
    <e3:DropDownList ID="cmbCategory" CriteriaName="Category" runat="server" Text="Category" EmptyMessage="Select Category" onchange="if(comboBox._selectedItem != null) FillTypes(comboBox._selectedItem._text);"/>
    <e3:CheckBoxList ID="cmbType" CriteriaName="Type" runat="server" Text="Type"/>
    <e3:CheckBoxList ID="cblYear" CriteriaName="Year" runat="server" Text="Year" DataTextField="Year" DataValueField="Year"/>
    <e3:Demographics ID="demographics" CriteriaName="Demographics" runat="server" Text="Demographics"/>
    <e3:DropDownList ID="cblGrps" CriteriaName="Groups" runat="server" Text="Groups" EmptyMessage="Select Group" Height="200px"/>
   
    <telerik:RadXmlHttpPanel runat="server" ID="reportListXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/ReportsWCF.svc"
        WcfServiceMethod="GetReportListByLevel" RenderMode="Block" OnClientResponseEnding="loadReportList">
    </telerik:RadXmlHttpPanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <input type="hidden" runat="server" id="lockedTestIDs" ClientIDMode="Static" value=""/>
    <input type="hidden" runat="server" id="portalType" ClientIDMode="Static" value="Teacher"/>
    <input type="hidden" runat="server" id="yearList" ClientIDMode="Static" value=""/>
    <input type="hidden" runat="server" id="districtYear" ClientIDMode="Static" value=""/>
    <input type="hidden" runat="server" id="IsStudentResponseVisible" ClientIDMode="Static" value="true"/> 
    <input type="hidden" runat="server" id="IsSuggestedResourcesVisible" ClientIDMode="Static" value=""/>         
    <div runat="server" ClientIDMode="Static" id="initMessage" style="text-align:center;height:300px;padding-top:25%;font-size:14pt;">
        <span style="font-weight:bold;">Please select criteria</span><br/>
        (Curriculum and grade are required fields)
    </div>
    <telerik:RadAjaxPanel ID="resultsPanel" runat="server" LoadingPanelID="resultsLoadingPanel" Visible="false">
        <div id="topDiv">
            <div align="left" style="background-color:#FFF; padding-left: 100px;">
                <br />
                <div runat="server" id="performanceLevelDiv">
                </div>
                <br />
            </div>
            <table width="100%" style="background-color:#FFF;">
                <tr>
                    <td style="text-align: left; padding-left: 40px; width: 350px;">
                        <input type="checkbox" runat="server" ID="chkPerformanceLevels" ClientIDMode="Static" checked="checked" onclick="togglePerformanceLevels(this.checked);" />
                        To hide or show performance level colors, click here.
                    </td>
                    <td style="text-align: left;">
                        <input type="checkbox" runat="server" ID="chkUnlockedColumns" checked="checked" onclick="toggleLockedColumnDisplay(this.checked);" />
                        To hide or show unlocked columns, click here.
                    </td>
                </tr>
            </table>
            <div style="height:10px; background-color: #FFF;"></div>
        </div>
        <%--There is a corresponding style to override some height issues telerik has with scrolling a rtl. 
        Search for rtlDataDiv if you adjust the height of the treeList below--%>
        <div align="center" style="height: 900px; overflow: hidden;">
            <telerik:RadTreeList ID="radTreeResults" runat="server"  OnNeedDataSource="radTreeResults_NeedDataSource"
                OnItemDataBound="radTreeResults_ItemDataBound" ParentDataKeyNames="ParentConcatKey" Skin="Office2010Silver"
                DataKeyNames="ConcatKey" AutoGenerateColumns="false" HeaderStyle-Width="130px" HeaderStyle-HorizontalAlign="Center" 
                HeaderStyle-VerticalAlign="Top" CssClass="rtlAssessResults">
                <ClientSettings Scrolling-AllowScroll="true" Scrolling-UseStaticHeaders="true">
                    <Resizing ResizeMode="AllowScroll" AllowColumnResize="true" EnableRealTimeResize="true" />
                </ClientSettings>
            </telerik:RadTreeList>
        </div>
                               
        <telerik:RadWindowManager ID="RadWindowManager2" runat="server" EnableShadow="true"
            AutoSize="true" Skin="Thinkgate_Window" EnableEmbeddedSkins="False" VisibleStatusbar="False">
            <Windows>
                <telerik:RadWindow ID="UserListDialog" runat="server" Title="Report Selection"
                     ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true" Behaviors="Close">
                    <ContentTemplate>
                        <div id="contentDiv" clientidmode="Static" style="width: 350px; background-color: #f0f0f0; padding-top: 20px; padding-left: 20px; padding-bottom: 20px;">
                            <table style="width:100%">
                                <tr>
                                    <div id="Div1"></div>
                                </tr>
                                <tr>
                                    <td id="list1">
                                        <div id="Div2"></div>
                                    </td>
                                    <td id="list2" valign="top">
                                        <div id="Div3"></div>
                                    </td>
                                </tr>
                            </table>
                         </div>
                    </ContentTemplate>
                </telerik:RadWindow>
            </Windows>
        </telerik:RadWindowManager>
        <telerik:RadWindow ID="criteriaWindow" runat="server" EnableShadow="true" Skin="Office2010Silver"
            Behaviors="Close" VisibleOnPageLoad="false" Animation="Slide"
            Modal="true" AutoSize="true" ClientIDMode="Static" VisibleStatusbar="false">
            <ContentTemplate>
                <div id="chooseReportDiv" clientidmode="Static" style="width: 450px; height: 350px;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        background-color: #f0f0f0; display: none;">
                    <h1>
                        Groups</h1>
                    <table width="100%">
                        <tr>
                            <td width="33%">
                                At Risk
                            </td>
                            <td width="33%">
                                <a href="#" onclick="openReport();">Analysis</a>
                            </td>
                            <td width="33%">
                                My Group 2
                            </td>
                        </tr>
                        <tr>
                            <td width="33%">
                                Mastery
                            </td>
                            <td width="33%">
                                Report Cards
                            </td>
                            <td width="33%">
                                My Group 3
                            </td>
                        </tr>
                        <tr>
                            <td width="33%">
                                Results
                            </td>
                            <td width="33%">
                                Progress Reports
                            </td>
                            <td width="33%">
                            </td>
                        </tr>
                        <tr>
                            <td width="33%">
                                Proficiency
                            </td>
                            <td width="33%">
                                My Group 1
                            </td>
                            <td width="33%">
                            </td>
                        </tr>
                    </table>
                    <br />
                    <h1>
                        Reports</h1>
                    <table width="100%">
                        <tr>
                            <td width="50%">
                                At Risk Students by Standard
                            </td>
                            <td width="50%">
                                Distractor Analysis
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                At Risk Students by Test
                            </td>
                            <td width="50%">
                                Item Analysis
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                At Risk Students by Domain
                            </td>
                            <td width="50%">
                                Score Analysis
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                At Risk Standards by Student
                            </td>
                            <td width="50%">
                                Standard Analysis
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                At Risk Subgroups by Domain
                            </td>
                            <td width="50%">
                                Test Summary
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                Mastery Students by Standard
                            </td>
                            <td width="50%">
                                Progress Report by Student
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                Mastery Students by Test
                            </td>
                            <td width="50%">
                                Progress Report by Demographic
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                Mastery Students by Domain
                            </td>
                            <td width="50%">
                                Report Card by Standard
                            </td>
                        </tr>
                        <tr>
                            <td width="50%">
                                Mastery Standards by Student
                            </td>
                            <td width="50%">
                                Mastery Subgroups by Domain
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="resultsLoadingPanel" runat="server" />        
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>
    

   
<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .RadTreeList_Transparent .rtlR td[class*="noShade"]
        {
            background-image: none;
        }
            
        .RadTreeList_Transparent .rtlA td[class*="noShade"]
        {
            background-image: none;
        }
        
        .rbl input[type="radio"]
        {
            margin-left: 10px;
            margin-right: 1px;
        }
        
    </style>
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        
        function showWindow() {
            $find('<%= criteriaWindow.ClientID %>').show();
        }

        function showReportWindow() {
            var oWnd = $find("<%= UserListDialog.ClientID %>");
            oWnd.show();
            oWnd.autoSize();
        }

        function toggleColumnLock(imageObject, testID) {
            var radTreeResultsData = $('.rtlDataDiv');
            var cellPos = imageObject.parentNode.parentNode.parentNode.parentNode.parentNode.cellIndex - 2;

            if (imageObject.src.indexOf('unlock.gif') > -1) {
                imageObject.src = '../../Images/lock.gif';

                $('table tr', radTreeResultsData).each(function () {
                    //$('td[scoreCell]', $(this))[cellPos].removeAttribute('unlockedCell'); Commented for bug id 9030 as not working in IE 10
                    $('td[scoreCell]', $(this)).eq(cellPos).removeAttr('unlockedCell');
                });

                updateLockedTestIDsList(testID, true);
            }
            else {
                imageObject.src = '../../Images/unlock.gif';

                $('table tr', radTreeResultsData).each(function () {
                    // $('td[scoreCell]', $(this))[cellPos].setAttribute('unlockedCell', 'yes'); Commented for bug id 9030 as not working in IE 10
                    $('td[scoreCell]', $(this)).eq(cellPos).attr('unlockedCell', 'yes');
                });

                updateLockedTestIDsList(testID, false);
            }
        }

        function updateLockedTestIDsList(testID, add) {
            var lockedTestIDs = $('#lockedTestIDs').attr('value');
            var lockedTestIDsArr = lockedTestIDs.split(',');

            if (add) {
                var testIDsString = ',' + lockedTestIDs + ',';
                if (testIDsString.indexOf(',' + testID + ',') == -1) {
                    lockedTestIDsArr.push(testID);
                    $('#lockedTestIDs').attr('value', lockedTestIDsArr.join(','));
                }
            }
            else {
                for (var i = 0; i < lockedTestIDsArr.length; i++) {
                    if (lockedTestIDsArr[i] == testID) {
                        lockedTestIDsArr.splice(i, 1);
                        break;
                    }
                }

                $('#lockedTestIDs').attr('value', lockedTestIDsArr.join(','));
            }
        }

        function togglePerformanceLevels(show) {
            var radTreeResults = $('.RadTreeList');
            if (show) {
                $('td[bgStyle]', radTreeResults).each(function () {
                    $(this).css('background-color', $(this).attr('bgStyle'));
                    $(this).addClass('noShade');
                });
            }
            else {
                $('td[bgStyle]', radTreeResults).css('background-color', '').removeClass('noShade');
            }
        }

        function toggleLockedColumnDisplay(show) {
            var radTreeResultsHeader = $('.rtlHeader');
            var radTreeResultsData = $('.rtlDataDiv');
            if (show) {
                $('table th', radTreeResultsHeader).each(function () {
                    if ($('img[src*="unlock.gif"]', $(this)).length == 1) {
                        //var currCell = $(this)[0].cellIndex;
                        $(this).css('display', '');
                    }
                });

                $('table td[unlockedCell="yes"]', radTreeResultsData).css('display', '');
            }
            else {
                $('table th', radTreeResultsHeader).each(function () {
                    if ($('img[src*="unlock.gif"]', $(this)).length == 1) {
                        //var currCell = $(this)[0].cellIndex;
                        $(this).css('display', 'none');
                    }
                });

                $('table td[unlockedCell="yes"]', radTreeResultsData).css('display', 'none');
            }
            setTimeout(resizeContent, 1);
        }

        function resizeContent() {
            var radTreeResults = $('.RadTreeList');
            var radTreeHeader = $('.rtlHeader', radTreeResults);
            var radDataDiv = $('.rtlDataDiv', radTreeResults);

            if (radTreeResults.length == 0 || radTreeHeader.length == 0 || radDataDiv.length == 0)
                return;
            //radDataDiv.css('cssText', 'height: ' + (radTreeResults.height() - radTreeHeader.height()-20) + 'px !important; overflow: auto;');
            //radDataDiv.height($("#AjaxPanelResults").height() - $("#topDiv").height() - -radTreeHeader.height());

            //radDataDiv.css('cssText', 'height: 400px !important; overflow: auto !important;');
            var windowHeight = $(window).height();
            var ajaxPanelResultsHeight = $("#AjaxPanelResults").height();
            if (windowHeight <= ajaxPanelResultsHeight) {
                radDataDiv.css('cssText', 'height: ' + (windowHeight - $("#topDiv").height() - radTreeHeader.height() - 5) + 'px !important; overflow: auto;');
            }
            else {
                radDataDiv.css('cssText', 'height: ' + (ajaxPanelResultsHeight - $("#topDiv").height() - radTreeHeader.height() - 10) + 'px !important; overflow: auto;');
            }
        }

    </script>
  
	 
      <script type="text/javascript" language="javascript">
          var typeElements;         
          var statusOnLoad = 1;
          jQuery(document).ready(function () {             
              var lst = '<%= ListTypes %>';              
              typeElements = lst.split('^');
          });
          function FillTypes(catName) {
              if (statusOnLoad == 0) {
                  var Category;                            
                  var array = [];
                  array.push('All');
                  for (i = 0; i < typeElements.length; i++) {
                      category = typeElements[i].split('~');
                      if (catName == category[1]) {
                          array.push(category[0]);
                      }
                  }
                  if (array.length == 1)
                      array = [];
                  populateTypesOnCall(array, catName);                  
              }
              else {
                  statusOnLoad = 0;
              }
          }

          function populateTypesOnCall(arry) {             
              TypeController.PopulateList(arry);              
          }
      </script>
</asp:Content>
