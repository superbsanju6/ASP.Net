<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StandardsFilterEdit.aspx.cs"
    Inherits="Thinkgate.Controls.Standards.StandardsFilterEdit" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<%--This height of 100% helps elements fill the page--%>
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css" rel="stylesheet" type="text/css" runat="server" />
    <title></title>
    <base target="_self" />
    <telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
    </telerik:RadStyleSheetManager>
    <script type='text/javascript' src='../../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../../Scripts/jquery-migrate-1.1.0.min.js'></script>
    <script type='text/javascript' src='../../Scripts/jquery.scrollTo.js'></script>
    <script type="text/javascript">        var $j = jQuery.noConflict();</script>
    <style>
        .radButtonFloatRight 
        {
            float: right;
        }
        
        .tableHeader 
        {
            table-layout: fixed;   
        }
        
        .tableHeader th 
        {
            padding: 3px;
            font-weight: bold;
        }
        
        .tableBody
        {
            table-layout: fixed;
        }
        
        .tableBody td
        {
            padding: 3px;
        }
        
        .tableContainer
        {
            border-top: solid 1px #000;
        }
        
        .tableScroll
        {
            height: 290px;
            overflow: auto;
        }
        
        .fullText
        {
            display: none;
            position: absolute;
            z-index: 9999;
            background-color: #FFF;
            border-top: solid 1px #D0D0D0;
            border-left: solid 1px #D0D0D0;
            border-right: solid 2px #A0A0A0;
            border-bottom: solid 2px #A0A0A0;
            filter: progid:DXImageTransform.Microsoft.Shadow(color='#969696',  Direction=135, Strength=3);
            width: 99%;
            top: 0px;
            left: 0px;
            padding: 2px;
        }        
        
        .alignCellCenter
        {
            text-align: center;
        }
        
        .roundButtons
        {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }
        
        a.standardTextLink
        {
            color: #000;
            text-decoration: none;
        }
        a.standardTextLink:active
        {
            color: #000;
            text-decoration: none;
        }
        a.standardTextLink:hover
        {
            color: #000;
            text-decoration: underline;
        }
        a.standardTextLink:visited
        {
            color: #000;
            text-decoration: none;
        }
        
        .searchFilterText 
        {
            color: #000;
            font-weight: bold;
            text-align: center;
            text-decoration: underline;
            width: 150px;
            position: relative;
            float: left;
            margin: 20px;
        }
        
        #searchStandardsButton 
        {
            display:inline; 
            margin-top: 28px;
        }
        
        .standardFilterNameHeader
        {
            font-size: 1.5em;
            color: #666666;
            font-variant: small-caps;
            text-transform: none;
            font-weight: 200;
            margin-bottom: 0px;
            background: transparent;
            border: none;
            cursor: default;
        }
        
        .standardFilterNameHeader_edit
        {
            font-size: 1.5em;
            color: #666666;
            font-variant: small-caps;
            text-transform: none;
            font-weight: 200;
            margin-bottom: 0px;
        }
        
        .divOverflowHidden 
        {
            white-space:nowrap; 
            overflow:hidden;
            text-overflow: ellipsis;
        }
        
        .defaultMessage 
        {
            height: 130px;
            padding-top: 10%;
            text-align: center;   
            font-size: 12pt;
        }
        
        #newFilterNameDiv 
        {
            background-color: #FFF;
            width: 250px;
            height: 170px;
            padding: 0px;
            border-right: solid 1px #888;
            border-bottom: solid 1px #888;
            position: absolute;
            z-index: 999;
            margin-top: 20%;
            margin-left: 39%;
            font-size: 11pt;
            text-align: center;
            font-weight: bold;
            filter: progid:DXImageTransform.Microsoft.Shadow(color='#969696',  Direction=135, Strength=3);
        }
        
        .grayOut 
        {
            background-color: #555;
            filter: alpha(opacity=30);
            opacity: .3;
            position: absolute;
            z-index: 998;
            left: 0px;
            top: 0px;
        }
        
        html { 
            overflow: auto; 
            -webkit-overflow-scrolling: touch; 
        }
        
        body {
            height: 100%;
            overflow: auto; 
            -webkit-overflow-scrolling: touch;
        }
    </style>
</head>
<body>
    <form id="form2" runat="server" style="height: 100%;">
    <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    </telerik:RadAjaxManager>
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
    </telerik:RadSkinManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
        Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
        Animation="None">
    </telerik:RadWindowManager>
    <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel" Width="100%">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/under-construction-sign.png" Visible="false" />

        <div style="padding:5px;">
            <div class="grayOut"></div>
            <div id="newFilterNameDiv" style="display:none;">
                <div style="background-color: #2B77AD; height: 20px; color: #FFF; font-size: 10pt; font-weight: bold; text-align:left; padding: 3px;">New Standard Filter</div><br/>
                Please enter a name for this filter.<br/><br/>
                <telerik:RadTextBox onchange="enableContinueButton(this);" runat="server" ID="newFilterName" ClientIDMode="Static" Skin="Web20" Width="210"></telerik:RadTextBox><br/><br/><br/>
                <div style="height:25px; padding-right: 10px;">
                <asp:Button runat="server" ID="newFilterNameContinueButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Continue&nbsp;&nbsp;" disabled="true" OnClientClick="newFilterNameContinue_Click(); return false;" />
                <asp:Button runat="server" ID="newFilterNameCancelButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="cancelChanges(); return false;" />
                </div>
            </div>
            <input type="text" class="standardFilterNameHeader" id="standardFilterName" ClientIDMode="Static" runat="server" value="" onfocus="blur();" /> 
            (<a href="javascript:void(0);" onclick="editFilterName(this); return false;" style="font-size:11pt; font-family:Arial, sans-serif;">Edit</a>)
            <telerik:RadButton runat="server" ID="clearFilterRadButton" ClientIDMode="Static" Skin="Web20" Text="Clear Filter" AutoPostBack="false" CssClass="radButtonFloatRight" OnClientClicked="clearFilter" />
            <br/><br/>
            <div class="defaultMessage" id="defaultMessageDiv" ClientIDMode="Static" runat="server">&lt;&lt; Use the criteria below to search and add standards to the filter &gt;&gt;</div>
            <div class="tableContainer" runat="server" ClientIDMode="Static" id="tableContainerDiv">
                <asp:Table BorderWidth="1" GridLines="Both" ID="standardsFilterHeader" CssClass="tableHeader" ClientIDMode="Static" runat="server" Width="100%" CellPadding="2" CellSpacing="0" BackColor="White">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell Width="50">Remove</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Name</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="250">Text</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="50">Grade</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Subject</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Course</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Level</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
                <div class="tableScroll">
                    <asp:Table ID="standardsFilterTable" CssClass="tableBody" ClientIDMode="Static" BorderWidth="1" CellPadding="2" CellSpacing="0" GridLines="Both" runat="server" Width="100%" BackColor="White">
                    </asp:Table>
                </div>
            </div>
            <br/>
            <div style="height: 25px;">
            <asp:Button runat="server" ID="updateButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Update Filter&nbsp;&nbsp;" OnClick="UpdateFilter_Click" />
            <asp:Button runat="server" ID="cancelButton" ClientIDMode="Static" CssClass="roundButtons"
                Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="cancelChanges(); return false;" />
            </div>

            <hr style="border:solid 1px #000; margin-top: 15px; margin-bottom: 15px;"/>

            <h1 style="text-decoration: underline; display: inline;">Search Standards</h1><span style="font-style:italic;"> - Select criteria below and click GO.</span>

            <div style="overflow:auto;">
                <div class="searchFilterText">
                    Standard Set<br/><br/>
                    <telerik:RadComboBox ID="standardsSetDropdown" ClientIDMode="Static" runat="server" Skin="Web20" EmptyMessage="Select" OnClientSelectedIndexChanged="requestFilter" 
                    xmlHttpPanelID="standardsFilterGradeSubjectCourseXmlHttpPanel"></telerik:RadComboBox>
                </div>
                <div class="searchFilterText">
                    Grade<br/><br/>
                    <telerik:RadComboBox ID="gradeDropdown" ClientIDMode="Static" runat="server" Skin="Web20" OnClientSelectedIndexChanged="requestFilter" 
                    EmptyMessage="Select" xmlHttpPanelID="standardsFilterSubjectCourseXmlHttpPanel"></telerik:RadComboBox>
                </div>
                <div class="searchFilterText">
                    Subject<br/><br/>
                    <telerik:RadComboBox ID="subjectDropdown" ClientIDMode="Static" runat="server" Skin="Web20" OnClientSelectedIndexChanged="requestFilter" 
                    xmlHttpPanelID="standardsFilterCourseXmlHttpPanel" EmptyMessage="Select"></telerik:RadComboBox>
                </div>
                <div class="searchFilterText" style="width: 275px;">
                    Standard Course<br/><br/>
                    <telerik:RadComboBox ID="courseDropdown" ClientIDMode="Static" runat="server" Skin="Web20" Width="275" MaxHeight="250" EmptyMessage="Select"></telerik:RadComboBox>
                </div>
                <div class="searchFilterText">
                    Text<br/><br/>
                    <telerik:RadTextBox runat="server" ID="textInput" ClientIDMode="Static" Skin="Web20" Width="150"></telerik:RadTextBox>
                </div>
                <div class="searchFilterText" style="width:10px; margin-left:0px;">
                    <asp:ImageButton runat="server" ImageUrl="~/Images/go.png" ID="searchStandardsButton" ClientIDMode="Static" AlternateText="Search Standards" 
                    OnClientClick="verifyCriteria(); return false;" />
                    <span style="display:none;"><asp:Button runat="server" ID="searchStandardsButtonTrigger" ClientIDMode="Static" OnClick="LoadStandardsSearchTable_Click" /></span>
                </div>
            </div>

            <div class="tableContainer">
                <asp:Table BorderWidth="1" GridLines="Both" ID="standardsSearchHeader" CssClass="tableHeader" ClientIDMode="Static" runat="server" Width="100%" CellPadding="2" CellSpacing="0" 
                BackColor="White" Visible="false">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell Width="50">Add</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Name</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="250">Text</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="50">Grade</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Subject</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Course</asp:TableHeaderCell>
                        <asp:TableHeaderCell Width="100">Level</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
                <div class="tableScroll">
                    <asp:Table ID="standardsSearchTable" CssClass="tableBody" ClientIDMode="Static" BorderWidth="1" CellPadding="2" CellSpacing="0" GridLines="Both" runat="server" Width="100%" 
                    BackColor="White" Visible="false">
                    </asp:Table>
                </div>
            </div>
        </div>
        <input type="hidden" runat="server" ClientIDMode="Static" id="filterIDs" value=""/>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    <telerik:RadXmlHttpPanel runat="server" ID="standardsFilterGradeSubjectCourseXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
        WcfServiceMethod="LoadStandardsGradeSubjectCourseList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
        objectToLoadID="gradeDropdown">
    </telerik:RadXmlHttpPanel>
    <telerik:RadXmlHttpPanel runat="server" ID="standardsFilterSubjectCourseXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
        WcfServiceMethod="LoadStandardsSubjectCourseList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
        objectToLoadID="subjectDropdown">
    </telerik:RadXmlHttpPanel>
    <telerik:RadXmlHttpPanel runat="server" ID="standardsFilterCourseXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
        WcfServiceMethod="LoadStandardsCourseList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
        objectToLoadID="courseDropdown">
    </telerik:RadXmlHttpPanel>
    </form>

    <script type="text/javascript">
        var modalWin = parent.$find('RadWindow1Url');
        var displayAlertMessage = false;
        prepModalWindow();
        
        function prepModalWindow() {
            if (modalWin && modalWin.isVisible()) {
                modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
                modalWin.addConfirmDialog({ title: 'Cancel Changes', text: 'Are you sure you want to cancel? Any changes that have been made will be lost.' }, window);
            }

            if ($('#standardFilterName').attr('value') == '') {
                setTimeout(function() {
                    $('.grayOut').css('display', '');

                    if (document.compatMode == 'CSS1Compat') {
                        $('.grayOut').css('width', document.documentElement.clientWidth + 'px');
                        $('.grayOut').css('height', document.documentElement.clientHeight + 'px');
                    }
                    else {
                        $('.grayOut').css('width', document.body.clientWidth + 'px');
                        $('.grayOut').css('height', document.body.clientHeight + 'px');
                    }

                    $('#newFilterNameDiv').css('display', '');
                }, 100);
            }
        }

        function cancelChanges() {
            if (!displayAlertMessage) modalWin.remove_beforeClose(modalWin.confirmBeforeClose);
            setTimeout(function () { modalWin.close(); }, 0);
        }
        
        function displayFullDescription(obj) {
            var spanContainer = obj.parentNode.parentNode;
            var fullTextSpan = $('.fullText', obj.parentNode.parentNode);
            fullTextSpan.css('display', 'inline');
        }

        function requestFilter(sender, args) {
            var senderElement = sender.get_element();
            var itemText = args.get_item().get_text();
            var panelID = senderElement.getAttribute('xmlHttpPanelID');
            var panel = $find(panelID);

            switch (panelID) {
                case 'standardsFilterGradeSubjectCourseXmlHttpPanel':
                    var standardsSetValue = itemText == 'All' ? null : '"' + itemText + '"';
                    panel.set_value('{"StandardsSet":' + standardsSetValue + '}');
                    break;
                case 'standardsFilterSubjectCourseXmlHttpPanel':
                    var courseDropdown = $find('courseDropdown');
                    var standardsSetDropdown = $find('standardsSetDropdown');
                    var standardsSet = standardsSetDropdown.get_selectedItem() ? standardsSetDropdown.get_selectedItem().get_value() : '';
                    var standardsSetValue = itemText == 'All' ? null : '"' + standardsSet + '"';
                    clearAllDropdownItems(courseDropdown);
                    panel.set_value('{"Grade":"' + itemText + '", "StandardsSet":' + standardsSetValue + '}');
                    break;
                case 'standardsFilterCourseXmlHttpPanel':
                    var gradeDropdown = $find('gradeDropdown');
                    var grade = gradeDropdown.get_selectedItem() ? gradeDropdown.get_selectedItem().get_value() : '';
                    var standardsSetDropdown = $find('standardsSetDropdown');
                    var standardsSet = standardsSetDropdown.get_selectedItem().get_value();
                    var standardsSetValue = itemText == 'All' ? null : '"' + standardsSet + '"';
                    panel.set_value('{"Grade":"' + grade + '","Subject":"' + itemText + '", "StandardsSet":' + standardsSetValue + '}');
                    break;
            }
            
        }

        function loadChildFilter(sender, args) {
            //load panel
            var senderElement = sender.get_element();
            var results = args.get_content();
            var dropdownObjectID = senderElement.getAttribute('objectToLoadID');
            var dropdownObject = $find(dropdownObjectID);

            if (results[0] && results[0].Value) {
                for (var x = 0; x < results.length; x++) {
                    dropdownObject = $find(results[x].Key + 'Dropdown');

                    //Clear all context menu items
                    clearAllDropdownItems(dropdownObject);

                    /*Add each new context menu item
                    results[i].Key = dropdown keyword
                    results[i].Value = value and display text
                    */
                    for (var i = 0; i < results[x].Value.length; i++) {
                        addDropdownItem(dropdownObject, results[x].Value[i]);
                    }

                    if (results[x].Value.length == 1) {
                        dropdownObject.get_items().getItem(0).select();
                    }
                    else if (dropdownObject._emptyMessage) {
                        dropdownObject.clearSelection();
                    }
                }
            }
            else {
                //Clear all context menu items
                clearAllDropdownItems(dropdownObject);

                /*Add each new context menu item
                results[i].Key(value) = ID
                results[i].Value(text) = display text
                */
                for (var i = 0; i < results.length; i++) {
                    addDropdownItem(dropdownObject, results[i].Key ? results[i].Key : results[i], results[i].Value);
                }

                if (results.length == 1) {
                    dropdownObject.get_items().getItem(0).select();
                }
                else if (dropdownObject._emptyMessage) {
                    dropdownObject.clearSelection();
                }
            }
        }

        function addDropdownItem(dropdownObject, itemTextandValue, value) {
            if (!dropdownObject || !itemTextandValue) {
                return false;
            }

            /*indicates that client-side changes are going to be made and 
            these changes are supposed to be persisted after postback.*/
            dropdownObject.trackChanges();

            //Instantiate a new client item
            var item = new Telerik.Web.UI.RadComboBoxItem();

            //Set its text and add the item
            if (typeof(value) == 'undefined') {
                item.set_text(itemTextandValue);
                item.set_value(itemTextandValue);
            }
            else {
                item.set_text(value);
                item.set_value(itemTextandValue);
            }
            dropdownObject.get_items().add(item);

            //submit the changes to the server.
            dropdownObject.commitChanges();
        }

        function clearAllDropdownItems(dropdownObject) {
            var allItems = dropdownObject.get_items().get_count();
            if (allItems < 1) {
                return false;
            }

            /*indicates that client-side changes are going to be made and 
            these changes are supposed to be persisted after postback.*/
            dropdownObject.trackChanges();

            //clear all items
            dropdownObject.get_items().clear();

            //submit the changes to the server.
            dropdownObject.commitChanges();

            return false;
        }
        
        function verifyCriteria() {
            var grade = $find('gradeDropdown').get_text;
            var subject = $find('subjectDropdown').get_text();
            var course = $find('courseDropdown').get_text();

            if (grade == '' || grade == 'Select' || subject == '' || subject == 'Select' || course == '' || course == 'Select') {
                var wnd = window.radalert('Please choose a grade, subject, and course.', 400, 150, 'Alert');
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            }
            else {
                $('#searchStandardsButtonTrigger').click();
            }
        }
        
        function editFilterName(obj) {
            standardFilterName = document.getElementById('standardFilterName');          
            switch(obj.innerHTML) {
                case 'Edit':
                    standardFilterName.className = 'standardFilterNameHeader_edit';
                    standardFilterName.onfocus = null;
                    obj.innerHTML = 'Done';
                    displayAlertMessage = true;
                    break;
                default:
                    if (standardFilterName.value == '') {
                        var wnd = window.radalert('Filter name cannot be blank.', 400, 150, 'Alert');
                        wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                    }
                    else {
                        standardFilterName.className = 'standardFilterNameHeader';
                        standardFilterName.style.width = (standardFilterName.value.length * 11) + 'px';
                        standardFilterName.onfocus = function() { this.blur(); };
                        obj.innerHTML = 'Edit';
                    }
                    break;
            }
        }

        function addToFilter(obj) {
            var filterIDs = document.getElementById('filterIDs');
            var updateButton = document.getElementById('updateButton');
            var standardsSearchTable = $('#standardsSearchTable');
            var standardsFilterTable = $('#standardsFilterTable');
            var standardID = obj.getAttribute('standardID');
            var lastCellWidth = '';

            if($('#tableContainerDiv').css('display') == 'none') {
                lastCellWidth = '100px';
            }
            
            filterIDs.value = filterIDs.value + obj.getAttribute('standardID') + ',';
            updateButton.disabled = false;
            $('#tableContainerDiv').css('display', 'block');
            $('#defaultMessageDiv').css('display', 'none');
            
            $('tr[standardID="' + standardID + '"]', standardsSearchTable).appendTo('#standardsFilterTable');
            $('tr[standardID="' + standardID + '"] td', standardsFilterTable).css('background-color', '#FCC');
            $('tr[standardID="' + standardID + '"] td input[id*="standardsSearchCheckbox_"]', standardsFilterTable).each(function () {
                $(this).attr('name', 'standardsFilterCheckbox_' + standardID);
                $(this).attr('id', 'standardsFilterCheckbox_' + standardID);
                $(this).attr('onclick', 'removeFromFilter(this, this.checked);');
            });
            
            if(lastCellWidth.length > 0) {
                $('tr[standardID="' + standardID + '"] td:last', standardsFilterTable).css('width', lastCellWidth);
            }

            displayAlertMessage = true;
        }

        function removeFromFilter(obj, remove) {
            var filterIDs = document.getElementById('filterIDs');
            var filterIDsArray = filterIDs.value.replace( /,$/ , '').split(',');
            var standardID = obj.getAttribute('standardID');

            if (remove) {
                for (var i = 0; i < filterIDsArray.length; i++) {
                    if (filterIDsArray[i] == standardID) {
                        filterIDsArray.splice(i, 1);
                        break;
                    }
                }

                filterIDs.value = filterIDsArray.join(',');
            }
            else {
                filterIDs.value = filterIDs.value + standardID + ',';
            }

            displayAlertMessage = true;
        }
        
        function clearFilter(sender, args) {
            var filterIDs = document.getElementById('filterIDs');

            $('#standardsFilterTable').empty();
            
            filterIDs.value = '';
            $('#tableContainerDiv').css('display', 'none');
            $('#defaultMessageDiv').css('display', 'block');
            displayAlertMessage = true;
        }
        
        function enableContinueButton(obj) {
            if(obj.value == '') {
                $('#newFilterNameContinueButton').attr('disabled', true);
                $('#newFilterNameContinueButton').addClass('aspNetDisabled');
            }
            else {
                $('#newFilterNameContinueButton').attr('disabled', false);
                $('#newFilterNameContinueButton').removeClass('aspNetDisabled');
            }
        }
        
        function newFilterNameContinue_Click() {
            var standardFilterName = document.getElementById('standardFilterName');
            var newFilterName = $find('newFilterName');
            
            standardFilterName.value = newFilterName._enteredText;
            standardFilterName.style.width = (standardFilterName.value.length * 11) + 'px';
            $('#newFilterNameDiv').css('display', 'none');
            $('.grayOut').css('display', 'none');
            displayAlertMessage = true;
        }
    </script>
</body>
</html>
