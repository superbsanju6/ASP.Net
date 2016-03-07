
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor_Addendum.aspx.cs"
    Inherits="Thinkgate.Controls.Assessment.ContentEditor.ContentEditor_Addendum" ValidateRequest ="false" %>

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
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4"
        rel="stylesheet" type="text/css" runat="server" />
    <link href="~/Thinkgate_Blue/TabStrip.Thinkgate_Blue.css?v=2" rel="stylesheet" type="text/css" />
    <title></title>
    <base target="_self" />
    <telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
    </telerik:RadStyleSheetManager>
    <script type='text/javascript' src='../../../Scripts/jquery-1.9.1.min.js'></script>
	<script type='text/javascript' src='../../../Scripts/jquery-migrate-1.1.0.min.js'></script>
    <script type='text/javascript' src='../../../Scripts/jquery.scrollTo.js'></script>
    <script type="text/javascript">        var $j = jQuery.noConflict();</script>
    <style>
        .bottomTextButton
        {
            vertical-align: middle;
        }
        .previewImg
        {
            width: 100%;
            height: 100%;
            margin: auto;
            vertical-align: middle;
            /*min-height: 600px;*/
        }
        table.ItemBankList td
        {
            width: 100%;
            padding: 10px;
        }
        .divUploadTemplate
        {
            background-color: White;
            width: 100%;
            height: 100%;
        }
        .center
        {
            text-align: center;
            width: 100%;
            margin: auto;
        }
        .IdentificationSubHeader
        {
            font-style: italic;
            color: Gray;
            line-height: 12px;
        }
        .ImageUploader
        {
            text-align: center;
            background-color: White;
            width: 100%;
            white-space: nowrap;
            font-weight: bold;
        }
        .LegalMessage
        {
            text-align: center;
            background-color: White;
            height: 100%;
            line-height: 12px;
        }
        .IconHeader
        {
            width: 100%;
            background-color: rgb(222, 226, 231);
        }
        
        .rpFirst .rpTemplate
        {
            position: relative;
            font-size: 0;
            line-height: 0;
        }
        
        div.RadPanelBar
        {
            margin: 0 auto 0;
        }
        
        .linksPanel
        {
            right: 30px;
            bottom: 30px;
            width: 200px;
            height: 30px;
            text-align: right;
        }
        
        .RadPanelBar .rpFirst .rpTemplate
        {
            zoom: 1;
            background: url('Images/tstore.jpg') no-repeat 0 0;
            width: 100%;
        }
        
        .linksPanel a
        {
            color: White;
        }
        
        /* styling of template contents */
        .rpTemplate .productList
        {
            padding: 0;
            width: 662px;
            margin: 0 auto;
            display: block;
        }
        
        .rpTemplate .productList:after
        {
            content: ".";
            display: block;
            visibility: hidden;
            font-size: 0;
            line-height: 0;
            overflow: hidden;
            height: 0;
            clear: both;
        }
        
        .rpTemplate .productList li
        {
            list-style-type: none;
            float: left;
            text-align: center;
            width: 220px;
            margin: 15px 0 0;
            padding: 0;
            border-right: 1px solid #bec7cb;
        }
        
        .rpTemplate .productList li.last
        {
            border-right-width: 0;
        }
        
        .rpTemplate .productList li div
        {
            font: normal 12px Arial,sans-serif;
            text-decoration: underline;
            color: #333;
        }
        
        div.RadPanelBar .rpTemplate
        {
            background: #EDF9FE;
            overflow: hidden !important;
            height: 100%;
        }
        .rpHeaderTemplate
        {
            overflow: hidden;
            /*height: 26px;*/
        }
        .RadPanelItemHeader
        {
            padding-left: 5px;
        }
    </style>
</head>
<body style="background-color: rgb(222, 226, 231);">
    <asp:PlaceHolder runat="server">
        <script type="text/javascript">
            function GetEquationEditorURL() {
                var _EquationEditorURL = '<%=ConfigurationManager.AppSettings["EquationEditorURL"].ToString() %>';
                return _EquationEditorURL;
            }
        </script>
   </asp:PlaceHolder>
    <form id="form_image" runat="server" style="min-width: 1000px;">
    <div runat="server" id="AddendumItem" style="display: none;">
    </div>
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			<asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            <asp:ScriptReference Path="~/Scripts/ckeditor/ckeditor.js"/>
            <asp:ScriptReference Path="~/Scripts/ckeditor/adapters/jquery.js"/>
			<asp:ScriptReference Path="~/Scripts/master.js?d=2" />
            <asp:ScriptReference Path="~/Scripts/master.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/Services/Service2.svc" />
        </Services>
    </telerik:RadScriptManager>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <script type="text/javascript">
            var $ = $telerik.$;
            var uploadsInProgress = 0;
            var cursorPosition = null;
            var hasAttachedEvent = false;
            var callInProgress = false;

            if (CKEDITOR) {
                window.onload = function() {
                    CKEDITOR.replace("CkEditorAddendumEdit");
                };
            }

            function onFileSelected(sender, args) {
                if (!uploadsInProgress)
                    $("#SaveButton").attr("disabled", "disabled");

                uploadsInProgress++;
            }

            function onFileUploaded(sender, args) {
                decrementUploadsInProgress();
            }

            function onUploadFailed(sender, args) {
                decrementUploadsInProgress();
            }

            function decrementUploadsInProgress() {
                uploadsInProgress--;

                if (!uploadsInProgress)
                    $("#SaveButton").removeAttr("disabled");
            }
            //<![CDATA[
            function validationFailed(sender, eventArgs) {
                $(".ErrorHolder").append("<p>Validation failed for '" + eventArgs.get_fileName() + "'.</p>").fadeIn("slow");
            }
            //]]>

            function mainToolBar_OnClientButtonClicked(sender, args) {
                var selected_button_value = args.get_item().get_value();
                switch (selected_button_value) {
                    case "DeleteAddendum":
                    customDialog({ title: "Delete Addendum?", maxheight: 100, maxwidth: 500, maximize: true, autoSize: false, content: "You are about to delete this Addendum. Do you wish to continue?<br/><br/>", dialog_style: 'confirm', destroyOnClose: true, Name: 'deleteAddendum' }, [{ title: "Cancel" }, { title: "Ok", callback: deleteAddendum_confirmCallback }]);
                        break;
                    default:
                        alert('Function is in development.');
                }
            }
            function deleteAddendum_confirmCallback() {
                var ID = document.getElementById('AddendumItem').innerHTML;
                Service2.DeleteAddendum(ID, deleteAddendum_onSuccess, deleteAddendum_onFailure);

            }
            function deleteAddendum_onSuccess() {
                closeCurrentCustomDialog();
            }
            function deleteAddendum_onFailure() {

            }


            function updateItemBanks(obj) {
	   
                var checks = $j("[type='checkbox'][checked='true'][targetType='5'].itemBankUpdate");
                var inBank = $j("[type='checkbox'][inBank='true'][targetType='5'].itemBankUpdate");
	            var itemBanks = '';
	
                if (obj.getAttribute('targetType') == '5') {
                    if (obj.checked) {
                        if (obj.getAttribute('multiBanks') == 'false') {
                            for (var i = 0; i < itemBankList.length; i++) {
                                document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][0] == '5' && obj.getAttribute('target') == itemBankList[i][1]) ? true : false;
                                document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = (itemBankList[i][0] == '5') ? false : true;
				            }
			            } else {
                            for (var i = 0; i < itemBankList.length; i++) {
                                if (itemBankList[i][0] != '5') {
                                    document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
                                    document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = true;
					            } else {
                                    if (itemBankList[i][3] == 'false') {
                                        document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
                                    } else {
                                        if (itemBankList[i][2] == 'true' || obj.getAttribute('target') == itemBankList[i][1]) {
                                            document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = true;
							            }
						            }
					            }
				            }
			            }
		            } else {
                        for (var i = 0; i < itemBankList.length; i++) {
                            if (checks.length != 0 || inBank.length != 0) {
                                if (itemBankList[i][0] < '5') {
                                    document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = false;
                                    document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = true;
					            } 
                                else {
                                    if (checks.length < 1) {
                                        document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][2] == 'true') ? true : false;
						            }
					            }
                            } else {
                                document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked = (itemBankList[i][2] == 'true') ? true : false;
                                document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).disabled = (itemBankList[i][0] == 2) ? true : false;
				            }
			            }
		            }
	            }
	
                for (var i = 0; i < itemBankList.length; i++) {
                    itemBanks += (document.getElementById('ItemBank_' + itemBankList[i][0] + '_' + itemBankList[i][1]).checked) ? ',' + itemBankList[i][0] + ':' + itemBankList[i][1] : '';
	            }
		        if (itemBanks != '') {
		            //alert(itemBanks.substring(1));
		            Addendum_changeItemBank();
	            }
            }

            function Addendum_changeItemBank() {
                var updatestring = '';
                var iBank = $j("[type='checkbox'][cbtype='ItemBank']");
                for (var x = 0; x < iBank.length; x++) {
                    if (iBank[x].checked) {
                        updatestring += '' + iBank[x].getAttribute('TargetType') + '@';
                        updatestring += '' + iBank[x].getAttribute('Target') + '|';
                    }

                }
                updatestring += ''
                if (updatestring.length > 5) {
                    //alert(updatestring);
                    var ID = document.getElementById('AddendumItem').innerHTML;
                    Service2.UpdateItem_ItemBank(ID, 2, updatestring, UpdateAddendum_ItemBank_onSuccess, UpdateAddendum_ItemBank_onFailure);
                }
            }

            function UpdateAddendum_ItemBank_onSuccess() {

            }
            function UpdateAddendum_ItemBank_onFailure() {

            }

            function Addendum_changeField_copyright(sender, args) {
                var value = sender._element.attributes['cbvalue'].value;
                if (value == 'Yes') {
                    //Show Source and credit
                    document.getElementById('trSource').style.display = '';
                    document.getElementById('trCredit').style.display = '';
                    document.getElementById('trExpiryDate').style.display = '';
                }
                else {
                    //Hide Source and credit
                    document.getElementById('trSource').style.display = 'none';
                    document.getElementById('trCredit').style.display = 'none';
                    document.getElementById('trExpiryDate').style.display = 'none';
                }
                Addendum_changeField(sender, args);
            }

            function Addendum_changeField_Type(sender, args) {

                var value = args.get_item().get_text();
                if (value == 'passage') {
                    document.getElementById('divcmbGenre').style.display = 'inline-block';
                }
                else {
                    document.getElementById('divcmbGenre').style.display = 'none';
                }
                Addendum_changeField(sender, args);
            }

          function Addendum_changeField(sender, args) {
                var field = sender._element.attributes['field'].value;
                var value = '';
                if (sender._element.nodeName == "INPUT")
                    value = args.get_newValue();
                else if (field == 'Copyright')
                    value = sender._element.attributes['cbvalue'].value;
                else
                    value = args.get_item().get_value();

                var ID = document.getElementById('AddendumItem').innerHTML;
                Service2.AddendumUpdateField(ID, field, value,
                    function (result_raw) {
                        result = jQuery.parseJSON(result_raw);
                        if (result.StatusCode > 0) {
                            Addendum_changeField_event_onFailure(null, result.Message);
                            return;
                        }
                        if (result.ExecOnReturn) {
                            eval(result.ExecOnReturn);
                        }
                    }, Addendum_changeField_event_onFailure);
            }
          function Addendum_changeField_event_onFailure() {

          }

            function upload_completed() {
                window.location.reload();
            }

            function Addendum_changeFieldValue(field, value) {
                callInProgress = true;
                var ID = document.getElementById('AddendumItem').innerHTML;
                Service2.AddendumUpdateField(ID, field, value,
                    function (result_raw) {
                        callInProgress = false;
                        result = jQuery.parseJSON(result_raw);
                        if (result.StatusCode == 0) {
                            currentDialog.remove_beforeClose(Addendum_Edit_save);
                            __doPostBack("<%= RadTabStrip1.UniqueID %>", "");
                        }

                        if (result.StatusCode > 0) {
                            Addendum_changeField_event_onFailure(null, result.Message);
                            return;
                        }
                        if (result.ExecOnReturn) {
                            eval(result.ExecOnReturn);
                        }
                    }, Addendum_changeField_event_onFailure);
            }

            function InsertAddendum(AddendumID, AddendumName, AddendumType, AddendumGenre) {

                var DialogueToSendResultsTo;
                DialogueToSendResultsTo = parent.getCurrentCustomDialogByName(getURLParm('NewAndReturnType'));
                try {
                    DialogueToSendResultsTo.get_contentFrame().contentWindow.InsertAddendum(AddendumID, AddendumName, AddendumType, AddendumGenre);
                }
                catch (e) {
                    try {
                        parent.InsertAddendum(AddendumID, AddendumName, AddendumType, AddendumGenre);
                    }
                    catch (e) {

                    }
                }

                closeCurrentCustomDialog();

            }


            function getURLParm(name) {
                name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
                var regexS = "[\\?&]" + name + "=([^&#]*)";
                var regex = new RegExp(regexS);
                var results = regex.exec(window.location.href);
                if (results == null)
                    return "";
                else
                    return decodeURIComponent(results[1].replace(/\+/g, " "));
            }

            function OpenAddendumItemCount() {
                if (document.getElementById('lblItemCount').innerHTML == '0')
                    return false;
                var ID = document.getElementById('AddendumItem').innerHTML;
                customDialog({ url: ('ContentEditor_Addendum_Questions.aspx?xID=' + ID), autoSize: true, name: 'ContentEditorAddendumQuestions', title: 'Items Found' });
            }

        </script>
    </telerik:RadScriptBlock>
    <div class="IconHeader">
        <telerik:RadToolBar ID="mainToolBar" runat="server" Style="" Width="722px" Skin="Sitefinity"
            EnableRoundedCorners="true" EnableShadows="true" OnClientButtonClicked="mainToolBar_OnClientButtonClicked">
            <Items>
                <telerik:RadToolBarButton ImageUrl="~/Images/Toolbars/delete.png" ToolTip="Delete Addendum"
                    Value="DeleteAddendum" />
            </Items>
        </telerik:RadToolBar>
        <asp:Image ID="FinishAndReturn" runat="server" Style="float: right; margin-top: 10px;
            margin-right: 10px; display: none;" ImageUrl="~/Images/finish_return.png" />
    </div>
    <br />
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
        SelectedIndex="0" Align="Justify" ReorderTabsOnSelect="true" Width="347px" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" OnClientTabSelected="Preview_OnClientTabSelected" AutoPostBack="false">
        <Tabs>
            <telerik:RadTab Text="Edit" Value="edit">
            </telerik:RadTab>
            <telerik:RadTab Text="Preview" Value="preview">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" Width="100%"
        CssClass="Web20">
        <telerik:RadPageView runat="server" ID="RadPageView1" CssClass="Web20">
            <telerik:RadPanelBar runat="server" ID="RadPanelBar1" Width="100%"
                Skin="Web20" ExpandMode="MultipleExpandedItems" OnClientLoad="onLoad">
                <Items>
                    <telerik:RadPanelItem Text="Identification" Expanded="true" runat="server" ID="Addendum_Identification">
                        <HeaderTemplate>
                            <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                            <span class="rpExpandHandle"></span>
                                        </td>
                                        <td style="width: 25%; vertical-align: top;">
                                            <div class="RadPanelItemHeader">
                                                Identification:</div>
                                        </td>
                                        <td style="width: 25%; vertical-align: top;">
                                            Name:&nbsp;&nbsp;<asp:Label runat="server" ID="lblNameEdit"></asp:Label>
                                        </td>
                                        <td style="width: 25%; vertical-align: top;">
                                            Type:&nbsp;&nbsp;<asp:Label runat="server" ID="lblTypeEdit"></asp:Label>
                                        </td>
                                        <td style="width: 25%; vertical-align: top;">
                                            Genre:&nbsp;&nbsp;<asp:Label runat="server" ID="lblGenreEdit"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 40%; border-right: 1px solid grey;">
                                        <!-- Column 1  -->
                                        <table width="100%" class="fieldValueTable">
                                            <tr>
                                                <td colspan="2" class="IdentificationSubHeader">
                                                    Complete the entries below with helpful information
                                                    <br />
                                                    to easily locate the addendum when searching.
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Name:
                                                </td>
                                                <td>
                                                    <telerik:RadTextBox runat="server" ID="txtName" field="Name" Skin="Web20" ClientEvents-OnValueChanged="Addendum_changeField" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="padding-left: 10px;">
                                                    Type:
                                                </td>
                                                <td class="fieldLabel" style="padding-left: 10px;">
                                                    <telerik:RadComboBox ID="cmbType" ClientIDMode="Static" runat="server" Skin="Web20"
                                                        OnClientSelectedIndexChanged="Addendum_changeField_Type" EmptyMessage="Select"
                                                        field="Type" Width="95">
                                                    </telerik:RadComboBox>
                                                    <div id="divcmbGenre" runat="server" clientidmode="Static" style="display: inline-block;">
                                                        &nbsp;&nbsp;&nbsp;&nbsp; Genre:
                                                        <telerik:RadComboBox ID="cmbGenre" ClientIDMode="Static" runat="server" Skin="Web20"
                                                            OnClientSelectedIndexChanged="Addendum_changeField" EmptyMessage="Select" field="Genre"
                                                            Width="95">
                                                        </telerik:RadComboBox>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Keywords:
                                                </td>
                                                <td>
                                                    <telerik:RadTextBox runat="server" ID="txtKeywords" Skin="Web20" field="Keywords"
                                                        ClientEvents-OnValueChanged="Addendum_changeField" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Items:
                                                </td>
                                                <td>
                                                    <a href="javascript:" onclick="OpenAddendumItemCount();">
                                                        <asp:Label runat="server" ID="lblItemCount" ClientIDMode="Static" /></a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Grade:
                                                </td>
                                                <td>
                                                    <telerik:RadComboBox ID="gradeDropdown" ClientIDMode="Static" runat="server" Skin="Web20"
                                                        OnClientSelectedIndexChanged="requestFilter" EmptyMessage="Select" xmlHttpPanelID="standardsFilterSubjectXmlHttpPanel"
                                                        field="Grade" ClientEvents-OnValueChanged="Addendum_changeField">
                                                    </telerik:RadComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Subject:
                                                </td>
                                                <td>
                                                    <telerik:RadComboBox ID="subjectDropdown" ClientIDMode="Static" runat="server" Skin="Web20"
                                                        OnClientSelectedIndexChanged="requestFilter" xmlHttpPanelID="standardsFilterCourseXmlHttpPanel"
                                                        EmptyMessage="Select" field="Subject" ClientEvents-OnValueChanged="Addendum_changeField">
                                                    </telerik:RadComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Course:
                                                </td>
                                                <td>
                                                    <telerik:RadComboBox ID="courseDropdown" ClientIDMode="Static" runat="server" Skin="Web20"
                                                        EmptyMessage="Select" field="Course" ClientEvents-OnValueChanged="Addendum_changeField"
                                                        OnClientSelectedIndexChanged="Addendum_changeField">
                                                    </telerik:RadComboBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 20%; border-right: 1px solid grey; vertical-align: top; padding: 0px 5px 0px 5px;">
                                        <!-- Column 2  -->
                                        <table style="width: 100%;">
                                            <tr>
                                                <td style="width: 40%; padding: 0px 5px 0px 5px;">
                                                    <hr />
                                                </td>
                                                <td style="font-weight: bold; white-space: nowrap;">
                                                    Item Banks
                                                </td>
                                                <td style="width: 40%; padding: 0px 5px 0px 5px;">
                                                    <hr />
                                                </td>
                                            </tr>
                                        </table>
                                        <div runat="server" id="ItemBankCheckBoxes" style="width: 100%;">
                                        </div>
                                    </td>
                                    <td style="width: 40%; vertical-align: top; padding: 0px 5px 0px 5px;" class="fieldValueTable">
                                        <!-- Column 3  -->
                                        <table style="width: 100%;">
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Author:
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblAuthor"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    District:
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblDistrict"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Copyright:
                                                </td>
                                                <td>
                                                    <telerik:RadButton ID="rbCopyRightYes" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
                                                        OnClientClicking="Addendum_changeField_copyright" field="Copyright" cbvalue="Yes"
                                                        GroupName="CopyRight" Text="Yes" AutoPostBack="false">
                                                    </telerik:RadButton>
                                                    &nbsp;&nbsp;&nbsp;
                                                    <telerik:RadButton ID="rbCopyRightNo" runat="server" ButtonType="ToggleButton" ToggleType="Radio"
                                                        OnClientClicking="Addendum_changeField_copyright" field="Copyright" cbvalue="No"
                                                        GroupName="CopyRight" Text="No" AutoPostBack="false">
                                                    </telerik:RadButton>
                                                </td>
                                            </tr>
                                                                                         
                                                <tr id="trExpiryDate" runat="server" clientidmode="Static">
                                                    <td class="fieldLabel" style="width: 95px; padding-left: 10px;">Expiration Date:
                                                    </td>
                                                    <td>

                                                        <asp:Label runat="server" ID="lblExpirationDate"></asp:Label>
                                                       

                                                    </td>
                                                </tr>

                                            <tr id="trSource" runat="server" clientidmode="Static">
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Source:
                                                </td>
                                                <td>
                                                    <telerik:RadTextBox runat="server" ID="txtSource" Skin="Web20" field="Source" ClientEvents-OnValueChanged="Addendum_changeField" />
                                                </td>
                                            </tr>
                                            <tr id="trCredit" runat="server" clientidmode="Static">
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Credit:
                                                </td>
                                                <td>
                                                    <telerik:RadTextBox runat="server" ID="txtCredit" Skin="Web20" field="Credit" ClientEvents-OnValueChanged="Addendum_changeField" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Date Created:
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblDateCreated"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldLabel" style="width: 95px; padding-left: 10px;">
                                                    Last Updated:
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblDateUpdated"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                    <telerik:RadPanelItem Expanded="true" runat="server" ID="Addendum_Image">
                        <HeaderTemplate>
                            <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                            <span class="rpExpandHandle"></span>
                                        </td>
                                        <td style="width: 15%;">
                                            <div class="RadPanelItemHeader">
                                                Image:</div>
                                        </td>
                                        <td style="width: 15%;">
                                            <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                <div style='background-image: url("../../../Images/find_btn.png");' id="Div1" class="bottomTextButton"
                                                    title="Search" onclick="stopPropagation(this); SearchItem('Image');">
                                                    Search</div>
                                            </div>
                                        </td>
                                        <td style="width: 15%;">
                                            <div onclick="stopPropagation(event)" style="" unselectable="on">
                                                <div style='background-image: url("../../../Images/add.gif");' id="btnAdd" class="bottomTextButton"
                                                    title="Add New" onclick="stopPropagation(this); addNewItem('Image');" unselectable="on">
                                                    Add New</div>
                                            </div>
                                        </td>
                                        <td style="width: 55%;">
                                            &nbsp;&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <div class="divUploadTemplate">
                                <div class="LegalMessage">
                                    <br />
                                    You must honor all copyright protection notifications on all material used within
                                    Elements&trade;. Only use content (Items, Distractors,<br />
                                    Images, Addendums, and Assessments) that your school system has purchased the rights
                                    to reproduce or that has been marked as public<br />
                                    domain. You are responsible for any copyright infringements. If in doubt about the
                                    content you wish to use, contact your central
                                    <br />
                                    office for permission clarification.
                                    <br />
                                    &nbsp;
                                </div>
                            </div>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                    <telerik:RadPanelItem Expanded="false" runat="server" ID="Addendum_Edit">
                        <HeaderTemplate>
                            <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 1%; margin: 0px; height: 24px; vertical-align: top;">
                                            <span class="rpExpandHandle"></span>
                                        </td>
                                        <td style="width: 99%;">
                                            <div class="RadPanelItemHeader">
                                                Addendum:</div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <div class="divUploadTemplate">
                                <div class="SKEditableBodyText">
                                    <textarea id="CkEditorAddendumEdit" name="CkEditorAddendumEdit" ClientIDMode="Static" runat="server" style="width: 100%; height: 550px;"></textarea>
                            </div>
                            </div>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                </Items>
            </telerik:RadPanelBar>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="RadPageView5" CssClass="Web20">
            <telerik:RadPanelBar runat="server" ID="RadPanelBar2s" Width="100%" Height="100%"
                Skin="Web20" ExpandMode="MultipleExpandedItems" OnClientLoad="onLoad">
                <Items>
                    <telerik:RadPanelItem Expanded="true">
                        <HeaderTemplate>
                            <div style="float: left; padding-top: 1px; z-index: 2800; width: 100%;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 25%;">
                                            <div class="RadPanelItemHeader">
                                                Identification:</div>
                                        </td>
                                        <td style="width: 25%;">
                                            Name:&nbsp;&nbsp;<asp:Label runat="server" ID="lblNamePreview"></asp:Label>
                                        </td>
                                        <td style="width: 25%;">
                                            Type:&nbsp;&nbsp;<asp:Label runat="server" ID="lblTypePreview"></asp:Label>
                                        </td>
                                        <td style="width: 25%;">
                                            Genre:&nbsp;&nbsp;<asp:Label runat="server" ID="lblGenrePreview"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <telerik:RadScriptBlock runat="server">
                                <script type="text/javascript" src="<asp:Literal runat='server' Text='<%$appSettings:MathJaxUrl%>' />/MathJax.js?config=TeX-AMS-MML_HTMLorMML,<asp:Literal runat='server' Text='<%$appSettings:MathJaxUrl%>' />/config/local/thinkgate-e3.js"></script>
                            </telerik:RadScriptBlock>
                            <div runat="server" id="previewAddendum" class="previewImg" clientidmode="Static">
                            </div>
                            <telerik:RadScriptBlock ClientIDMode="Static" ID="executeMathjax" Visible="False" runat="server">
                                <script type="text/javascript">
                                    MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
                                </script>
                            </telerik:RadScriptBlock>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                </Items>
            </telerik:RadPanelBar>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <%--    <telerik:RadWindow ID="RadWindow1" runat="server" Skin="Forest" Behaviors="Close, Move"
        Modal="true" Width="440px" Height="385px">
        <ContentTemplate>
            <img id="Img1" src="Images/discount.jpg" runat="server" alt="promotions" />
        </ContentTemplate>
    </telerik:RadWindow>--%>
    <script type="text/javascript">

        var panelBar;
        var panelBarProductsTab;
        var multiPage;
        var currentEditor;
        var currentEditableBody;
        var currentDialog = getCurrentCustomDialog();

        $(document).ready(function () {
        	try {
                CKEDITOR.on('instanceReady', function(event) {

                    event.editor.on('focus', function() {
                        setCursorPos(this);
                    });
                
                    event.editor.on('onkeyup', function () {
                        setCursorPos(this);
                        });
                });

                if (!hasAttachedEvent) {
                    currentDialog.add_beforeClose(Addendum_Edit_save);

                    hasAttachedEvent = true;
                }

        	}
        	catch (e) {

        	}
        });

        function onLoad(sender) {
            panelBar = sender;
            panelBarProductsTab = panelBar.get_items().getItem(1);
            //            multiPage = panelBar.get_items().getItem(1).findControl("RadMultiPage1");
           }

        function onItemClicked(sender, eventArgs) {
            if (!panelBarProductsTab.get_selected()) {
                panelBarProductsTab.expand();
                panelBarProductsTab.select();
            }

            var pageView = multiPage.get_pageViews().getPageView(
                eventArgs.get_item().get_index());

            pageView.set_selected(true);
        }


        function stopPropagation(e) {
            e.cancelBubble = true;

            if (e.stopPropagation)
                e.stopPropagation();
        }


        function addNewItem(itemType) {

            var appclient = '<%=AppClient() %>';
            //parent.customDialog({ url: ('../../../Controls/Assessment/ContentEditor/ContentEditor_Image.aspx?xID=a0pFaDN2aHh0RVNINHRCVkxpY3J2QT09&NewAndReturnID=a3o5YVh2TGtiZnJtQkVSQkhQV2VsQT09&NewAndReturnType=Addendum'), autoSize: true, name: 'ContentEditorIMAGE' });
            
            parent.customDialog({ url: (appclient + 'Dialogues/AddNewItem.aspx?xID=' + itemType + '&NewAndReturnID=' + document.getElementById('AddendumItem').innerHTML + '&NewAndReturnType=ContentEditorADDENDUM'), maximize: true, maxwidth: 450, maxheight: 250, autoSize: true, name: 'NewItem', title: ('Add New ' + itemType), destroyOnClose: true });

        }

        function SearchItem(itemType) {

            var appclient = '<%=AppClient() %>';
            parent.customDialog({ url: (appclient + 'Controls/Images/ImageSearch_ExpandedV2.aspx?NewAndReturnType=ContentEditorADDENDUM&ShowExpiredItems=No'), maxwidth: 900, maxheight: 675, maximize: true, name: ('Search' + itemType), title: ('Search ' + itemType), destroyOnClose: true });
        }

        function Preview_OnClientTabSelected(sender, eventArgs) {
            var tabValue = eventArgs.get_tab();
            var selectedTab = tabValue.get_text();
            if (selectedTab == "Preview") {
                Addendum_Edit_save();
            }
            currentDialog.remove_beforeClose(Addendum_Edit_save);
        }
        
        function Addendum_Edit_save(sender, args) {
            //args.set_cancel(true);

            var editor = CKEDITOR.instances.CkEditorAddendumEdit;
            if (editor.checkDirty()) {
                var editedContent = editor.getData();
                Addendum_changeFieldValue("Content", editedContent);
                document.getElementById('previewAddendum').innerHTML = editedContent;
            }

            var interval = null;
            if (sender != undefined && sender != null && sender._name == "ContentEditorADDENDUM")
            {
                interval = setInterval(function () {
                    $("#updPanelLoadingPanel").show();
                    if (callInProgress == false) {
                        if (args) {
                            currentDialog.remove_beforeClose(Addendum_Edit_save);
                            currentDialog.close();
                        }
                        clearInterval(interval);
                    }
                }, 500);
            }
        }

        function setCursorPos(editor) {
            try {
                currentEditor = editor;
                currentEditableBody = $(editor.element.$).parent();
            }
            catch (e) {

            }
        }

        function InsertImage(img) {
            if (currentEditor == null) setCursorPos(CKEDITOR.instances.CkEditorAddendumEdit);
            if (currentEditor !== null) {
                currentEditor.insertHtml(String.format("<img src='{0}' border='0' align='' alt='' /> ", img));
            }
        }

        var modalWin = parent.$find('RadWindow1Url');

        function displayFullDescription(obj) {
            var spanContainer = obj.parentNode;
            var fullTextSpan = $('.fullText', obj.parentNode);
            fullTextSpan.css('display', 'inline');
        }

        function requestFilter(sender, args) {
            var senderElement = sender.get_element();
            var itemText = args.get_item().get_text();
            var panelID = senderElement.getAttribute('xmlHttpPanelID');
            var panel = $find(panelID);

            switch (panelID) {
                case 'standardsFilterSubjectXmlHttpPanel':
                    var courseDropdown = $find('courseDropdown');
                    clearAllDropdownItems(courseDropdown);
                    panel.set_value('{"Grade":"' + itemText + '"}');
                    Addendum_changeField(sender, args);
                    break;
                case 'standardsFilterCourseXmlHttpPanel':
                    var gradeDropdown = $find('gradeDropdown');
                    var grade = gradeDropdown.get_selectedItem().get_value();
                    panel.set_value('{"Grade":"' + grade + '","Subject":"' + itemText + '"}');
                    Addendum_changeField(sender, args);
                    break;
            }

        }

        function loadChildFilter(sender, args) {
            //load panel
            var senderElement = sender.get_element();
            var results = args.get_content();
            var dropdownObjectID = senderElement.getAttribute('objectToLoadID');
            var dropdownObject = $find(dropdownObjectID);

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
            if (typeof (value) == 'undefined') {
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

        function editFilterName(obj) {
            standardFilterName = document.getElementById('standardFilterName');
            switch (obj.innerText) {
                case 'Edit':
                    standardFilterName.className = 'standardFilterNameHeader_edit';
                    standardFilterName.disabled = false;
                    obj.innerText = 'Done';
                    break;
                default:
                    standardFilterName.className = 'standardFilterNameHeader';
                    standardFilterName.disabled = true;
                    obj.innerText = 'Edit';
                    break;
            }
        }


    </script>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
        Skin="Thinkgate_Window" EnableEmbeddedSkins="false">
    </telerik:RadWindowManager>
    <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" style="position:absolute; top:0;left:0;right:0;bottom:0;margin:auto;" Width="100%" Height="100%" IsSticky="true"  runat="server"  ClientIDMode="Static" />
    <telerik:RadXmlHttpPanel runat="server" ID="standardsFilterSubjectXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
        WcfServiceMethod="LoadStandardsSubjectList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
        objectToLoadID="subjectDropdown">
    </telerik:RadXmlHttpPanel>
    <telerik:RadXmlHttpPanel runat="server" ID="standardsFilterCourseXmlHttpPanel" ClientIDMode="Static"
        Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/StandardsWCF.svc"
        WcfServiceMethod="LoadStandardsCourseList" RenderMode="Block" OnClientResponseEnding="loadChildFilter"
        objectToLoadID="courseDropdown">
    </telerik:RadXmlHttpPanel>
    </form>
</body>
</html>
