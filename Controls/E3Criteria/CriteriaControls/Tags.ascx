<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tags.ascx.cs" Inherits="Thinkgate.Controls.E3Criteria.CriteriaControls.Tags" %>
<%@ Import Namespace="Thinkgate.Classes" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Register  Src="~/Controls/E3Criteria/CriteriaControls/StandSetList.ascx" TagPrefix="e3" TagName="StandardSetList" %>

<!-- Add jQuery library -->
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8/jquery.min.js"></script>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js"></script>
<script type="text/javascript" src="<%:KenticoHelper.KenticoVirtualFolderRelative%>/cmsscripts/jquery/jquery-1.9.1.min.js"></script>
<script type="text/javascript" src="<%:KenticoHelper.KenticoVirtualFolderRelative%>/cmsscripts/jquery/jquery-migrate-1.1.0.min.js"></script>
<script type="text/javascript" src="<%:KenticoHelper.KenticoVirtualFolderRelative%>/cmsscripts/jquery/jquery-core.js"></script>
<script type="text/javascript" src="<%:KenticoHelper.KenticoVirtualFolderRelative%>/cmsscripts/jquery/jquery-ui/js/jquery-ui-1.10.0.custom.min.js"></script>
<script type="text/javascript" src="<%:KenticoHelper.KenticoVirtualFolderRelative%>/cmsscripts/jquery/jquery-cookie.js"></script>
<script type="text/javascript" src="<%:KenticoHelper.KenticoVirtualFolderRelative%>/cmsscripts/jquery/DataTables/js/jquery.dataTables.js"></script>
<script type="text/javascript" src="<%:KenticoHelper.KenticoVirtualFolderRelative%>/CMSScripts/Custom/tgDivTools.js"></script>
<script type="text/javascript" src="<%:KenticoHelper.KenticoVirtualFolderRelative%>/CMSScripts/Custom/addNewDocument.js"></script>
<script type="text/javascript" src=<%=Page.ResolveUrl("~/Scripts/jquery-1.9.1.js")%>></script>
<script type="text/javascript" src=<%=Page.ResolveUrl("~/Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js")%> ></Script>   
<script type="text/javascript" src=<%=Page.ResolveUrl("~/Scripts/jquery.inputmask.js")%> ></script>   
<link rel="stylesheet" href="<%=Page.ResolveUrl("~/Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css")%>"/> 

<style type="text/css">
    ::-ms-clear {
         display: none;
        }
     body, form {
         font-family: Tahoma;
         font-size: 12px;
     }

    .inputLabel {
        font-weight: bold;
        vertical-align: middle;
        width: 200px;
    }

    .inputLabel_h {
        font-weight: bold;
        vertical-align: middle;
        width: 200px;
        text-decoration: underline;
    }
    .addStandardIconDiv {
        position: relative;
        border: solid 1px gray;
        border-radius: 15px;
        padding: 3px;
        cursor: pointer;
        margin-top: 5px;
        width: 110px;
    }

    .addStandardIcon {
        top: 5px;
        width: 16px;
        height: 16px;
        float: left;
        margin-left: 5px;
        margin-right: 5px;
        cursor: pointer;
    }

    .alignmentLabel {
        font-weight: normal;
    }

    .alignmentContent {
        font-weight: normal;
        font-size: 8pt;
    }

    .eduAlignmentLabel {
        font-weight: bold;
        text-decoration: underline;
    }

    input[type=text], select {
        border-radius: 5px;
    }

    .greenButton {
        background-color: rgb(91, 183, 91);
        border: 1px solid lightgray;
        border-width: 1px;
        color: black;
        cursor: pointer;
        display: inline-block;
        font-family: Verdana,Arial,sans-serif;
        font-size: 14px;
        line-height: 15px;
        text-decoration: none;
        padding: 12px;
        padding-top: 5px;
        padding-bottom: 5px;
        border-radius: 5px;
    }
    .redButton {
        background-color: rgb(218, 79, 73);
        border: 1px solid lightgray;
        border-width: 1px;
        color: white;
        cursor: pointer;
        display: inline-block;
        font-family: Verdana,Arial,sans-serif;
        font-size: 14px;
        line-height: 15px;
        text-decoration: none;
        padding: 12px;
        padding-top: 5px;
        padding-bottom: 5px;
        border-radius: 5px;
    }

    .okbuttonImage {
        cursor: pointer;
        display: inline-block;
        color: #fff;
        margin-top: 1px;
        width: 14px;
        height: 14px;
        line-height: 14px;
        text-align: center;
        vertical-align: text-top;
    }

    .cancelbuttonImage {
        cursor: pointer;
        display: inline-block;
        color: #fff;
        margin-top: 1px;
        width: 14px;
        height: 14px;
        line-height: 14px;
        text-align: center;
        vertical-align: text-top;
    }

    #divMain {
        font-family: Tahoma, "Trebuchet MS", Arial;
        font-size: 11px;
        background-color: white;
    }

    #trEducationalAlignment {
        line-height: 10px;
        height: 10px;
    }

    table tr {
        line-height: 15px;
    }

    .ui-widget-content {
        background: #ccc!important;
    }

    .tagsSelected {
        font-size: 12px;
        font-weight: bold;
        text-decoration: underline;
        cursor: pointer;
    }

    .toolTipSelected {
        width: 90%;
        margin-left: 2%;
        line-height: 25px;
        height: 25px;
        display: none;
        background-color: transparent;
        padding-left: 2px;
    }
    .chkboxList {
        height: 5px;
    }
    .scroll_checkboxes
    {
        height: 120px;
        width: 200px;
        padding: 5px;
        overflow: auto;
        border: 1px solid #ccc;
        z-index: 100;
        position:absolute;
        opacity: 100;
        background-color: whitesmoke
    }

    .FormText
    {
        FONT-SIZE: 11px;
        FONT-FAMILY: tahoma,sans-serif
    }
    .StandSetLists {      
        cursor: pointer;
        vertical-align: middle;
        margin-left: 10px;
    }
    .RadCalendar {
        z-index: 0;
    }

    .ui-datepicker-trigger
    {
        padding:0px;
        padding-left:5px;
        vertical-align:baseline;

        position:relative;
        top:4px;
        height:18px;
    }
    .infoIcon{
        background-repeat: no-repeat;
        display: inline-table;
        height: 16px;
        vertical-align: middle;
        width: 16px;
    }

</style>

<script type="text/javascript">

    $(function () {
        $('#addStandardsDialog').dialog({
            autoOpen: false,
            modal: true,
            width: 900,
            height: 500,           
            title: 'Tag - Standards Association',
            close: function (event, ui) {
            }
        });
    });

    function openModalDialog(displayControlId, inputControlId) {
        currentDisplayControl = displayControlId;
        currentInputControl = inputControlId;
        if (currentInputControl) {
            oldselectedItems = $get(currentInputControl).value;
        }

        var url = '<%=Page.ResolveUrl("~/Controls/StdAddNewAssoc.aspx") %>' + '?parentnodeid=' + '<%=ObjectId.ToString(CultureInfo.InvariantCulture) %>' + '&ReturnURL=LRMI&LRMIItemID=' + $("#hdnLRMIItemId").val();
        $('#addStandardsFrame').attr('src', url);
        $('#addStandardsDialog').dialog("open");
    }

    function CloseDialog() {
        $('#addStandardsDialog').dialog("close");
    }

    function closeModalDialog(standardIds, standardName) {
        //get the transferred arguments
        if (currentInputControl) {
            $get(currentInputControl).value += standardIds;
            selectedItems = $get(currentInputControl).value;
        }
        if (currentDisplayControl) {
            $get(currentDisplayControl).innerHTML += standardName;
        }
        }
    function displayCreativeCommonIcon(Id) {

        $("#UsageRightUrlTxt").hide();
        $("#UsageRightUrl").hide();
        if (jsonCreativeCommon[Id.selectedIndex].ID != 0) {
            $("#UsageRightsImage").attr('src', jsonCreativeCommon[Id.selectedIndex].IconUrl);
            $("#UsageRightsImage").attr('alt', jsonCreativeCommon[Id.selectedIndex].Description);
            $("#UsageRightsImage").show();
            if (jsonCreativeCommon[Id.selectedIndex].SelectDescription != "Custom") {
                $("#UsageRightUrl").text(jsonCreativeCommon[Id.selectedIndex].Description);
                $("#UsageRightUrl").attr('href', jsonCreativeCommon[Id.selectedIndex].DescriptionUrl);
                $("#UsageRightUrl").attr('target', '_blank');
                $("#UsageRightUrl").show();
            } else {
                $("#UsageRightUrlTxt").show();
            }
        } else {
            $("#UsageRightsImage").hide();
            $("#UsageRightUrl").text("");
            $("#UsageRightUrl").attr('href', "");
            $("#UsageRightUrl").attr('target', '_blank');
        }
        }
    function DisableCheckBox(checkBoxListId) {
        $("[id^=" + checkBoxListId + "_]").attr("disabled", true);
        }
    function DisplayStandards(standardsets) {
    $('#StandSetListPanel').toggle();
        }
</script>
<div>
    <e3:StandardSetList ID="StandSetListDiv" ClientIDMode="Static" runat="server"></e3:StandardSetList>
            </div>
<div>
	<div style="padding-left: 5px; padding-top: 5px; width: 800px !important;" class="selectNew4">
		<div id="divMain" style="width: 795px; height: 100%; border: solid 1px black; padding: 2px;">
			
			<div style="overflow: auto;">
                <table id="tblEducationalAlignment" runat="server" width="100%">
                    <tr id="trEducationalAlignment" runat="server" style="height: 10px; line-height: 10px;">
                        <td class="inputLabel_h">Educational Alignment:
                        </td>
                    </tr>
                </table>
				<table>
                            <tr>
					<td style="vertical-align: top">
						<div style="float: left;">
						<table>
							<tr>
								<td><asp:Label ID="lblAssessedText" runat="server">Assesses:</asp:Label></td>
								<td>
									<asp:Label ID="btnAddAssessed" Width="110px" runat="server">
                                        <div id="addStandardIconDiv1" class="addStandardIconDiv" title="Add Standards" runat="server" clientidmode="Static">
											<span id="addStandardIcon1" class="addStandardIcon" runat="server" clientidmode="Static"></span>Add Standards                         
                                        </div>
                                    </asp:Label>
                                    <asp:Label ID="lblNoStandardsAssessed" style="display:none" runat="server">No Standards Aligned</asp:Label>
                                     <asp:ImageButton ID="AssessedIcon" runat="server" ToolTip="View Standards" ImageUrl="~/Images/ViewPageSmall.png" OnClientClick="DisplayStandards('Assessed'); return true;"  OnClick="BuildStandardSet_OnClick" CssClass="StandSetLists" />                                 
									<asp:HiddenField ID="hdnAssessedIds" runat="server"></asp:HiddenField>
                                </td>
                            </tr>
                            <tr>
								<td><asp:Label ID="lblTeachesText" runat="server">Teaches:</asp:Label></td>
								<td>
									<asp:Label ID="btnAddTeaches" Width="110px" runat="server">
                                        <div id="addStandardIconDiv2" class="addStandardIconDiv" title="Add Standards" runat="server" clientidmode="Static">
											<span id="addStandardIcon2" class="addStandardIcon" runat="server" clientidmode="Static"></span>Add Standards
                                        </div>
                                    </asp:Label>
                                    <asp:Label ID="lblNoStandardsTeaches" style="display:none" runat="server">No Standards Aligned</asp:Label>
                                    <asp:ImageButton ID="TeachesIcon" runat="server" ToolTip="View Standards" ImageUrl="~/Images/ViewPageSmall.png" OnClientClick="DisplayStandards('Teaches'); return true;" OnClick="BuildStandardSet_OnClick" CssClass="StandSetLists" />
									<asp:HiddenField ID="hdnTeachesIds" runat="server"></asp:HiddenField>
                                </td>
                            </tr>
                            <tr>
								<td><asp:Label ID="lblRequiresText" runat="server">Requires:</asp:Label></td>
								<td>
									<asp:Label ID="btnAddRequires" Width="110px" runat="server">
                                        <div id="addStandardIconDiv3" class="addStandardIconDiv" title="Add Standards" runat="server" clientidmode="Static">
											<span id="addStandardIcon3" class="addStandardIcon" runat="server" clientidmode="Static"></span>Add Standards
                                        </div>
                                    </asp:Label>
                                    <asp:Label ID="lblNoStandardsRequires" style="display:none" runat="server">No Standards Aligned</asp:Label>
                                    <asp:ImageButton ID="RequiresIcon" runat="server" ToolTip="View Standards" ImageUrl="~/Images/ViewPageSmall.png" OnClientClick="DisplayStandards('Requires'); return true;" OnClick="BuildStandardSet_OnClick" CssClass="StandSetLists" />
									<asp:HiddenField ID="hdnRequiresIds" runat="server"></asp:HiddenField>
                                </td>
                            </tr>
                            <tr>
								<td>Reading Level:</td>
								<td>
									<asp:TextBox ID="ReadingLevel" runat="server" MaxLength="50" Skin="Web20" Text="" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; " Height="16px" Width="195px"></asp:TextBox>
								</td>
                            </tr>
							<tr>
								<td>&nbsp;</td>
								<td>&nbsp;</td>
							</tr>
                        </table>
                    </div>
					</td>
					<td style="vertical-align: top">
						<div style="margin: 0px; margin-top: 5px;">
					
                    <div style="float: left;">
                        <table style="">
						   
                            <tr>
								<td>Educational Subject:</td>
                                <td class="alignmentContent" style="width: 225px;">
								    <asp:TextBox ID="EducationalSubjectSelections" runat="server" ReadOnly="True" Style="border-radius: 5px;  border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; " width="175px" onclick="$('#SubjectDiv').show();$('#SubjectDiv').focus();"></asp:TextBox>
                                    <asp:Image ID="EducationalSubjectImage" runat="server" ImageUrl="~/Images/down_arrow.png" onclick="$('#SubjectDiv').show();$('#SubjectDiv').focus();"/>
								    <div id="SubjectDiv" class="scroll_checkboxes" style="display:none">
									    <asp:CheckBoxList ID="ddlEducationalSubject" CssClass="FormText" runat="server" RepeatDirection="Vertical" RepeatColumns="1" BorderWidth="0" Datafield="SubjectText" DataValueField="SubjectValue"></asp:CheckBoxList>
								        <asp:Image ImageUrl="~/Images/ok.png" onclick="$('#SubjectDiv').hide();" runat="server"/>
                                         </div>
                                </td>
                            </tr>
                            <tr>
								<td>Text Complexity:</td>
                                <td class="alignmentContent" style="width: 225px;">
									<asp:TextBox ID="TextComplexity" runat="server" MaxLength="50" Text="" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; " Width="195px"></asp:TextBox>
                                </td>
                            </tr>
						</table>
					</div>
				</div>
                                </td>
					<td style="vertical-align: top">
						<div style="float: left;">
						<table style="">
                            <tr>
								<td class="alignmentLabel">Grade:</td>
								<td class="alignmentContent">
								    <asp:TextBox ID="GradeSelections" runat="server" ReadOnly="True" Style="border-radius: 5px;  border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; " width="85px" onclick="$('#GradeDiv').show();$('#GradeDiv').focus();"></asp:TextBox>
                                    <asp:Image ID="GradeImage" runat="server" ImageUrl="~/Images/down_arrow.png" onclick="$('#GradeDiv').show();$('#GradeDiv').focus();"/>
								    <div id="GradeDiv" class="scroll_checkboxes" style="display:none; width: 85px" >
									    <asp:CheckBoxList ID="GradeCheckboxList" CssClass="FormText" runat="server" RepeatDirection="Vertical" RepeatColumns="1" BorderWidth="0" Datafield="SubjectText" DataValueField="SubjectValue"></asp:CheckBoxList>
								        <asp:Image ImageUrl="~/Images/ok.png" onclick="$('#GradeDiv').hide();" runat="server"/>
                                         </div>
                                </td>
                            </tr>
                        </table>
                    </div>
					</td>
					</tr>
			</table>
				
				
                <table style="clear: both;">
                    <tr>
						<td class="eduAlignmentLabel">Learning Resource Type</td>
						<td class="eduAlignmentLabel">Educational Use</td>
						<td class="eduAlignmentLabel">End User</td>
						<td class="eduAlignmentLabel">Media Type</td>
						<td class="eduAlignmentLabel">Interactivity Type</td>						
						
                    </tr>
                    <tr>
						<td class="alignmentContent" style="width: 160px; vertical-align: top;">
							<asp:CheckBoxList  CellPadding="0" CellSpacing="0" ID="chkListLearningResourceType" DataTextField="Description" DataValueField="Enum" runat="server" RepeatDirection="Vertical" RepeatLayout="Table" >
							</asp:CheckBoxList>
						</td>
						<td class="alignmentContent" style="width: 150px; vertical-align: top;">
							<asp:CheckBoxList CellPadding="0" CellSpacing="0" ID="chkEducationalUse" DataTextField="Description" DataValueField="Enum" runat="server" RepeatDirection="Vertical" RepeatLayout="Table">
                            </asp:CheckBoxList>
                        </td>
						<td class="alignmentContent" style="width: 160px; vertical-align: top;">
							<asp:CheckBoxList CellPadding="0" CellSpacing="0" ID="chkListTargetAudience" DataTextField="Description" DataValueField="Enum" runat="server" RepeatDirection="Vertical" RepeatLayout="Table">
                            </asp:CheckBoxList>
						</td>
						<td class="alignmentContent" style="width: 140px; vertical-align: top;">
							<asp:CheckBoxList CellPadding="0" CellSpacing="0" ID="chkListInstructionTypes" DataTextField="Description" DataValueField="Enum" runat="server" RepeatDirection="Vertical" RepeatLayout="Table">
                            </asp:CheckBoxList>
                        </td>
						<td class="alignmentContent" style="width: 100px; vertical-align: top;">
							<asp:CheckBoxList CellPadding="0" CellSpacing="0" ID="chkListActivityType" DataTextField="Description" DataValueField="Enum" runat="server" RepeatDirection="Vertical" RepeatLayout="Table">
                            </asp:CheckBoxList>
                        </td>
						
                    </tr>
                </table>

                <table>
                    <tr>
						<td id="rtbDurationLbl" class="inputLabel">Time Required:
						</td>
						<td>
							<asp:TextBox runat="server" ID="rtbDuration" MaxLength="3" onblur="Value365(this);"  ToolTip="Enter Time in Days" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px;  text-align:center; " Width="23px" />
							&nbsp;Days&nbsp;<asp:DropDownList ID="DurationHours" runat="server" ToolTip="Enter Time in Hours" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; " Width="55px" />&nbsp;Hours&nbsp;<asp:DropDownList ID="DurationMinutes" runat="server" ToolTip="Enter Time in Minutes" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; " Width="55px" />&nbsp;Minutes
                        </td>
						<td>Creator:</td>
                        <td>
							<asp:TextBox runat="server" ID="Creator" MaxLength="200" ToolTip="Creator" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; width: 220px;" />
                        </td>
                    </tr>
                    <tr>
                        <td class="inputLabel">Age Appropriateness:
                        </td>
						<td>
									<asp:DropDownList ID="ddlAgeAppropriate" runat="server" ToolTip="Age Appropriate" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 3px; padding-left: 5px; padding-right: 3px; width: 180px;"></asp:DropDownList>
						</td>
						<td>Date Created:</td>
						<td><asp:TextBox ID="DateCreatedtxt" runat="server"  Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; width: 80px;"  ></asp:TextBox>

						</td>
					</tr>
					<tr>
						<td class="inputLabel">Usage Rights: <div id="infoIconImage" class="infoIcon"></div></td>
						<td>
						    <asp:DropDownList ID="ddlUsageRights" ToolTip="Usage Rights" onchange="displayCreativeCommonIcon(this);" runat="server" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 3px; padding-left: 5px; padding-right: 3px; width: 180px;"></asp:DropDownList>
						</td>
						<td>Publisher</td>
						<td>
							<asp:TextBox runat="server" ID="Publisher" MaxLength="200" ToolTip="Publisher" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; width: 220px;" />
						</td>
					</tr>
					<tr>
						<td></td>
						<td>
						    <asp:Image ID="UsageRightsImage" ClientIDMode="Static" style="display:none" runat="server"></asp:Image>
							</td>
						<td>Language:</td>
                        <td>
							<asp:DropDownList ID="ddlLanguage" runat="server" ToolTip="Language" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 3px; padding-left: 5px; padding-right: 3px; width: 180px;"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="inputLabel">Usage Right URL:
                        </td>
						<td colspan="3">
							<asp:HyperLink ID="UsageRightUrl" ClientIDMode="Static" runat="server"></asp:HyperLink>
                            <asp:TextBox ID="UsageRightUrlTxt" ClientIDMode="Static" runat="server" Style="display: none; border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; width: 220px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="inputLabel">Original 3rd Party URL:
                        </td>
						<td colspan="3">
                            <asp:TextBox runat="server" ID="rtbOriginalThirdPartyURL" MaxLength="200" ToolTip="Enter Original 3rd Party URL" Style="border: solid 1px #6788be; color: #333; font-size: 11.5px; font-family: 'segoe ui', arial, sans-serif; padding-bottom: 3px; padding-top: 2px; padding-left: 5px; padding-right: 5px; width: 220px;" />
                        </td>
                    </tr>
                </table>
            </div>
            <div style="margin: 0px;">
                <hr />
                <table id="tblLRMIDetails" runat="server" width="100%">
                    <tr>
						<td style="text-align: right; padding-right: 5px;" colspan="3">
							<asp:LinkButton ID="RadButtonSave" runat="server" CssClass="greenButton" OnClick="RadButtonSave_Click"><i class="okbuttonImage"></i>Save Tags&nbsp;</asp:LinkButton>&nbsp;
							<asp:LinkButton ID="RadButtonCancel" runat="server" CssClass="redButton" OnClientClick="return confirmCancel();" OnClick="RadButtonCancel_Click" ><i class="cancelbuttonImage"></i>Cancel&nbsp;</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField runat="server" ID="hdnLRMIItemId" Value="0" />
            </div>
            <div id="addStandardsDialog" style="display: none;">
                <iframe id="addStandardsFrame" width="99%" height="99%" style="margin-left: 3px; margin-top: 3px;" src="" frameborder="0">No frames</iframe>
            </div>
        </div>
    </div>

        <asp:HiddenField ClientIDMode="Static" ID="Tags_SelectedTagValues" runat="server" Value="{}" />
    </div>
<script type="text/javascript">
   

    var pagePath = window.location.pathname.substr(0, window.location.pathname.indexOf("/", 1));
    $("#addStandardIcon1").css("background-image", 'url(' + pagePath + "/Images/add.gif" + ')');
    $("#addStandardIcon2").css("background-image", 'url(' + pagePath + "/Images/add.gif" + ')');
    $("#addStandardIcon3").css("background-image", 'url(' + pagePath + "/Images/add.gif" + ')');
    $("#infoIconImage").css("background-image", 'url(' + pagePath + "/Images/Information-icon.png" + ')');

    if($("#UsageRightsImage").attr('src') != "") 
        $("#UsageRightsImage").show();

    $('#SubjectDiv').focusout(function () {
        var focusElement = String($(":focus").attr("id"));
        if (focusElement.indexOf("ddlEducationalSubject") == -1 &&
            focusElement.indexOf("SubjectDiv") == -1 &&
            focusElement != "undefined") {
            $('#SubjectDiv').hide();
        }
            
        if (focusElement == "undefined")
            $('#SubjectDiv').focus();
    });

    $('#GradeDiv').focusout(function () {
        var focusElement = String($(":focus").attr("id"));
        if (focusElement.indexOf("GradeCheckboxList") == -1 &&
            focusElement.indexOf("GradeDiv") == -1 &&
            focusElement != "undefined") {
            $('#GradeDiv').hide();
        }
        
        if (focusElement == "undefined")
            $('#GradeDiv').focus();
    });

    $(function () {
        $('#addStandardsDialog').dialog({
            autoOpen: false,
            modal: true,
            width: 900,
            height: 500,
            position: [25,25],
            title: 'Tag - Standards Association',
            close: function (event, ui) {
            }
        });
    });

    $("[id^=ddlEducationalSubject_]").attr("onclick", "UpdateSubjectText(this);");
    $("[id^=GradeCheckboxList_]").attr("onclick", "UpdateGradeText(this);");


    $(document).ready(function () {
        $("#DateCreatedtxt").datepicker({ showOn: 'button', buttonImageOnly: true, buttonImage: pagePath + '/images/calendar_small.gif', showButtonPanel: true });
        $("#DateCreatedtxt").inputmask("mm/dd/yyyy", { "placeholder": "__/__/____" });
    });
    jQuery.datepicker._gotoToday = function (id) {
        var today = new Date();
        var dateRef = jQuery("<td><a>" + today.getDate() + "</a></td>");
        this._selectDay(id, today.getMonth(), today.getFullYear(), dateRef);
    };

    function UpdateGradeText(elementControl) {
        if ($('#GradeSelections').val() == "--Select--")
            $('#GradeSelections').val(elementControl.value);
        else {
            var numberChecked = 0;
            $('[id^=GradeCheckboxList_]').each(function () {
                
                if (this.checked == true) numberChecked += 1;
                $('#GradeSelections').val(numberChecked + " items selected.");
            });
            if (numberChecked == 0) $('#GradeSelections').val("--Select--");
        }
    }
    function UpdateSubjectText(elementControl) {
        if ($('#EducationalSubjectSelections').val() == "--Select--")
            $('#EducationalSubjectSelections').val(elementControl.value);
        else {
            var numberChecked = 0;
            $('[id^=ddlEducationalSubject_]').each(function () {

                if (this.checked == true) numberChecked += 1;
                $('#EducationalSubjectSelections').val(numberChecked + " items selected.");
            });
            if (numberChecked == 0) $('#EducationalSubjectSelections').val("--Select--");
        }
    }

    function openModalDialog(displayControlId, inputControlId) {
        currentDisplayControl = displayControlId;
        currentInputControl = inputControlId;
        if (currentInputControl) {
            oldselectedItems = $get(currentInputControl).value;
        }

        var url = pagePath + '/Controls/StdAddNewAssoc.aspx?parentnodeid=' +
            <%=ObjectId.ToString(CultureInfo.InvariantCulture) %> +
            '&ReturnURL=LRMI&LRMIItemID=' +
            $("#hdnLRMIItemId").val();
        $('#addStandardsFrame').attr('src', url);
        $('#addStandardsDialog').dialog("open");
    }

    function CloseDialog() {
        $('#addStandardsDialog').dialog("close");
    }

    function closeModalDialog(standardIds, standardName) {
        //get the transferred arguments
        if (currentInputControl) {
            $get(currentInputControl).value += standardIds;
            selectedItems = $get(currentInputControl).value;

            if ($('#hdnAssessedIds').val() != "") $('#AssessedIcon').show();
            else $('#AssessedIcon').hide();
            if ($('#hdnTeachesIds').val() != "") $('#TeachesIcon').show();
            else $('#TeachesIcon').hide();
            if ($('#hdnRequiresIds').val() != "") $('#RequiresIcon').show();
            else $('#RequiesIcon').hide();
        }
    }

   
    $(document).ready(function () {        
        $('#rtbDuration').keydown(function (event) {            
            if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9
                || event.keyCode == 27 || event.keyCode == 13
                || (event.keyCode == 65 && event.ctrlKey === true)
                || (event.keyCode >= 35 && event.keyCode <= 39)) {
                return;
            } else {                
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        });

    });
    function confirmCancel() {
        return confirm("Are you sure you wish to close this window? Any changes made will be lost.");
    }
    function Value365(sender) {
        var duration = $('#' + sender.id);
        if (parseInt(duration.val()) > 365) duration.val("365");
    }
</script>

