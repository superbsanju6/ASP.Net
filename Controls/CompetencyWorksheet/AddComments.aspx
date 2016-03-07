<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddComments.aspx.cs" Inherits="Thinkgate.Controls.CompetencyWorksheet.AddComments" Title="Add Comments" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .tableDiv
        {
            display: table;
        }

        .row
        {
            display: table-row;
        }

        .cellLabel
        {
            display: table-cell;
            width: 40%;
            height: 40px;
            text-align: left;
            font-size: 13pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }

        .cellValue
        {
            display: table-cell;
            height: 40px;
            text-align: left;
            font-size: 12pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
        }

        .textBox
        {
            border: solid 1px #000;
            padding: 2px;
            width: 250px;
        }

        .smallText
        {
            font-size: 9pt;
        }

        .radDropdownBtn
        {
            font-weight: bold;
            font-size: 11pt;
        }

        .labelForValidation
        {
            color: Red;
            display: block;
            font-size: 10pt;
        }

        .fieldLabel
        {
            font-weight: bold;
            font-size: 10pt;
            white-space: nowrap;
            text-align: left;
            width: 110px;
            height: 30px;
            vertical-align: top;
        }

        .fieldEntry
        {
            vertical-align: top;
            text-align: left;
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

        .auto-style1
        {
            height: 76px;
        }
    </style>
       <script type="text/javascript">

           function closeWindow() {
               window.close();
               parent.location.reload();
           }
            </script> 
</head>
<body>
    <form id="form1" runat="server">
      <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/Scripts/jquery-migrate-1.1.0.min.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
                <asp:ScriptReference Path="~/Scripts/EditSubmitResultPagesWithinCustomDialog.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="False" Modal="True" InitialBehaviors="Close"
            Animation="None">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Web20">
        </telerik:RadSkinManager>
        <div>
            <div style="overflow: hidden;">
             
                    <asp:Panel runat="server" ID="addPanel" ClientIDMode="Static">
                        <div class="tableDiv">
                            <div class="row">
                                <table border="0" style="width: 95%">
                                    <tr>
                                        <td colspan="2">
                                           
                                           <telerik:RadTextBox ID="txtComments" runat="server" MaxLength="250" Height="100px" Width="400px" TextMode="MultiLine" style="overflow: hidden; padding: 0px 0px 0px 0px; width:400px !important;" Wrap="true"></telerik:RadTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-size:small">
                                             <asp:Label ID="Label1" ForeColor="Red" runat="server"> Maximum comment entry is 250 characters</asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Button ID="btnSave" runat="server" Text="Save" Enabled="false" ClientIDMode="Static" OnClick="btnSave_Click" />
                                            &nbsp;
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel"  OnClientClick="CloseConfirmation(); return false;"/>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </asp:Panel>
                     <asp:Panel ID="resultPanel" runat="server" Visible="false" ClientIDMode="Static" >
                        <asp:Label runat="server" ID="lblResultMessage" Text="" />
                        <br />
                        <telerik:RadButton runat="server" ID="RadButtonOK" Text="OK" AutoPostBack="False" OnClientClicked="closeWindow"  />
                     </asp:Panel>
            
            </div>
        </div>
    </form>
</body>
</html>

<script type="text/javascript">

    //function closeWindow() {
    //    window.close();
    //    parent.location.reload();
    //}

    $(document).ready(function () {

        window.parent.jQuery('#divCommentAddDialog').dialog("option", "height", "230");
        window.parent.jQuery('#divCommentAddDialog').dialog("option", "width", "480");
        window.parent.jQuery('#divCommentAddDialog').dialog("option", "position", "center");
        window.parent.jQuery('#divCommentAddDialog').dialog("open");

    });
    function RefreshParentPage() {

        window.opener.document.getElementById('btnAddComment').click();
    }
    $('#txtComments').bind("keyup", function () {

        var comment = $('#txtComments').val();
        comment = $.trim(comment);

        if (comment != "") {
            $('#btnSave').removeAttr('disabled');
        }
        else {
            $('#btnSave').attr('disabled', 'disabled');
        }
    });

    $("#txtComments").mouseout(function () {
       
        var comment = $('#txtComments').val();
        comment = $.trim(comment);

        if (comment != "") {
            $('#btnSave').removeAttr('disabled');
        }
        else {
            $('#btnSave').attr('disabled', 'disabled');
        }
    });

    function CloseConfirmation() {
        window.parent.jQuery('#divCommentAddDialog').dialog('close');
        //var comment = $('#txtComments').val();
        //if (comment != "") {
        //    if (confirm("Are you sure you want to cancel. Any updates will be lost")) {
        //        window.open('', '_self', '');
        //        window.parent.jQuery('#divCommentAddDialog').dialog('close');
        //    }
        //}
        //else {
        //   // window.open('', '_self', '');
        //    window.parent.jQuery('#divCommentAddDialog').dialog('close');
        //}
    }
    
    </script>
