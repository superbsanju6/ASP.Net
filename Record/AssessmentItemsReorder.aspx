<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="AssessmentItemsReorder.aspx.cs"
    Inherits="Thinkgate.Record.AssessmentItemsReorder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="~/Thinkgate_Blue/TabStrip.Thinkgate_Blue.css" rel="stylesheet" type="text/css" />
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet"
        type="text/css" runat="server" />

    <style type="text/css">
        .tblContainer
        {
            display: table;
            padding: 0;
        }
        
        .tblRow
        {
            display: table-row;
        }
        
        .tblLeft, .tblRight, .tblMiddle
        {
            display: table-cell;
        }
        
        #sortable
        {
            margin: 0;
            padding: 0;
        }
        
        .thumbInstance
        {
            width: 230px;
            height: 190px;
            position: relative;
            float: left;
        }
        
        .flag_img
        {
            float: left;
            clear: left;
            margin-left: 2px;
            margin-top: 4px;
        }
        
        div.demo
        {
            position: relative;
            padding: 12px;
            font-family: "Trebuchet MS" , "Arial" , "Helvetica" , "Verdana" , "sans-serif";
            overflow: scroll;
        }
        
        .instructions
        {
            background-color: #DFE9F5;
            font-size: 8pt;
            font-style: italic;
            padding: 3px;
            vertical-align: bottom;
        }
        
        .sort_number
        {
            font-size: 8pt;
            font-weight: bold;
            float: left;
            width: 17px;
            text-align: center;
        }
        
        .questionPlaceholder
        {
            background-color: #CCCCCC;
            width: 223px;
            height: 160px;
            margin-bottom: 30px;
            float: left;
        }
        
        .moving_helper_text
        {
            text-align: center;
            position: absolute;
            padding: 3px;
            width: 160px;
            top: 75px;
            left: 40px;
            background-color: #F2B26A;
            font-weight: bold;
            font-size: 9pt;
        }
        
        
        .zoom_in_img
        {
            float: left;
            xmargin-top: 18px;
            cursor: pointer;
            clear: left;
        }
        
        .ItemStandard
        {
            position: absolute;
            top: 165px;
            left: 20px;
            width: 200px;
            font-family: arial;
            font-size: 8pt;
            background-color: #CCCCCC;
            text-align: center;
            overflow: hidden;
            padding-top: 2px;
            padding-bottom: 2px;
            height: 10px;
        }
         #radtab_Forms {
            position: absolute;
            top: 35px;
            right: 0px;
        }
        
        .RadTabStrip .rtsOut {
            padding-right: 9px !important;
            
        }
        .RadTabStrip .rtsIn {
            width: 100%;
        }
        
        .thumbImage_fieldTest_false, .thumbImage_fieldTest_true {
            position: absolute;
            top: 0px;
            left: 20px;
         }
        .thumbImage_fieldTest_False
        {
            border: 1px solid black;
        }
        
        .thumbImage_fieldTest_True {
            border: 2px solid #D3A200;
        }

    </style>
    
</head>
<body class="BODY" style="margin: 0px; overflow: hidden">
    <style type="text/css">
        .RadToolBar_Sitefinity .rtbMiddle
        {
            background-color: transparent;
        }
        .RadTabStrip .rtsLink
        {
            padding-left: 0px;
        }
        
        .RadTabStrip .rtsIn
        {
            width: 100%;
        }
        .RadTabStripTop_Thinkgate_Blue .rtsLevel
        {
            background-color: #DFE9F5;
        }
        .hiddenClass
        {
            display: none;
        }
    </style>
    <form id="form2" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
            <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
            <asp:ScriptReference Path="~/Scripts/jquery.ui.touch-punch.js" />
            <asp:ScriptReference Path="~/Scripts/master.js" />
            <asp:ScriptReference Path="~/Scripts/AssessmentItemsReorder.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/Services/Service2.svc" />
        </Services>
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false" Skin="Thinkgate_Window" EnableEmbeddedSkins="false">
       
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    <div id="topPortion">
        <div id="toolbar_div" style="background-color: #DFE9F5">
            <telerik:RadToolBar ID="mainToolBar" runat="server" Style="z-index: 90001;" Width=""
                Skin="Sitefinity" EnableRoundedCorners="true" EnableShadows="true" OnClientButtonClicked="mainToolBar_OnClientButtonClicked">
                <Items>
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/print.png" ToolTip="Print"
                        Value="Print" />
                    
                    <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/remove.png" ToolTip="Remove Items"
                        Value="RemoveItems" />
                </Items>
            </telerik:RadToolBar>
            <asp:Button runat="server" ID="btnShuffle" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Shuffle&nbsp;&nbsp;" OnClientClick="Shuffle(); return false;"
                Style="float: right; margin-top: 5px; margin-right: 10px" />
        </div>
        <div class="tblContainer" style="background-color: #DFE9F5; width: 100%; border-bottom: 1px solid black">
            <div class="tblRow">
                <div class="tblLeft instructions">
                    Click on item, hold down left mouse button and drag to desired location.
                </div>
                <div class="tblLeft" style="vertical-align: bottom">
                    <telerik:RadTabStrip runat="server" ID="radtab_Forms" Orientation="HorizontalTop"
                    SelectedIndex="0" MultiPageID="" Skin="Thinkgate_Blue" OnClientTabSelecting="formTabStrip_OnClientTabSelecting"  EnableEmbeddedSkins="False"
                        DataTextField="FormName" DataValueField="FormId" ScrollButtonsPosition="Right" Height="25"
                        ClientIDMode="Static" Style="float: right" PerTabScrolling="True" >
                       
                    </telerik:RadTabStrip>
                </div>
            </div>
        </div>
    </div>
    <div id="hiddenFieldTestHoldingArea" style="display: none"></div>
    
    <div style="position: absolute; top: -300px">
        <div id="multiDragHelper" style="width: 200px; height: 200px;">
        </div>
    </div>
    <div id="sortableGrid" class="demo" style="height: 100px;" runat="server">
        <asp:Repeater ID="rep_assessmentItems" runat="server">
            <HeaderTemplate>
                <div id="sortable">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="thumbInstance" id="<%# ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).ID %>"
                    addendumid="<%# ConditionalAddendum(Container.DataItem) %>" style="display: inline">
                    <span>
                        <div class="sort_number">
                            <%# ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).Sort %></div>
                        <input style="float: left; clear: left;" type="checkbox" isFieldTest="<%# ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).FieldTest %>" />
                        <img class="zoom_in_img" src="<%# ImageWebFolder %>commands/view_assessment_small.png"
                            onclick="previewBankQuestion('<%# ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).EncryptedID  %>')"></img>
                        <img id="SortIconImg" src="../Images/Items_Sort.png" alt="SortIcon" title="Use default sort order for this addendum"
                             onclick ="CheckDefaultSortOrder('<%# ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).ID  %>')"
                             class='<%# ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).ShowAddendumIcon ? "":"hiddenClass"%> zoom_in_img'></img>
                         </span>
                    <img class="thumbImage_fieldTest_<%# ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).FieldTest  %>" src='<%# ItemThumbnailWebPathDistrict + ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).ThumbnailName %>'
                        alt="Item Thumbnail" onerror="onImgError(this);" onload="setDefaultImage(this);" />
                    <span class="ItemStandard">
                        <div>
                            <%# ((Thinkgate.Base.Classes.TestQuestion)Container.DataItem).StandardName %></div>
                    </span>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
