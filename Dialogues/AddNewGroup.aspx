<%@ Page Title="Add Group" Language="C#" MasterPageFile="~/Dialogue.Master" AutoEventWireup="true" CodeBehind="AddNewGroup.aspx.cs" Inherits="Thinkgate.Dialogues.AddNewGroup" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseDialog(arg) {
            var oWindow = GetRadWindow();
            setTimeout(function () {
                oWindow.Close(arg);
            }, 0);
        }

        function openInfoWindow() {
            window.radopen(null, "infoWindow");
        }

        function SaveClicking() {
            if (typeof (window.Page_ClientValidate) == 'function') {
                window.Page_ClientValidate();
            }
            if (window.Page_IsValid) {
                ButtonsSetEnabled(false);
                $("#ctl00_MainContent_btnCancel_input").addClass("disabledButton");
            }
        }

        function ButtonsSetEnabled(arg) {
            var btnOk = window.$find('<%= btnOK.ClientID %>');
            var btnCancel = window.$find('<%= btnCancel.ClientID %>');
            btnOk.set_enabled(arg);
            btnCancel.set_enabled(arg);
        }
    </script>
    <style type="text/css">
        
         #container {
             margin-left: 10px;
             margin-right: 10px;
         }
        
        div.columnLeft {
            float: left;
            width: 100px;            
        }

        div.columnRight {
            float: right;
            width: 440px;            
        }

        label.description {
            margin-left: 25px;
            display: inline-block;
        }

        div.buttonGroup {
            float: right;
        }

        .disabledButton {
            opacity: 1 !important;
            color: white !important;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
        <Windows>
            <telerik:RadWindow ID="infoWindow" runat="server" Title="Group Information" ShowContentDuringLoad="False" Behavior="Close" 
                NavigateUrl="GroupOptionsInformation.aspx" ReloadOnShow="True" Modal="True" Skin="Web20" VisibleStatusbar="False">
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel"/>
    <div id="container">
        <div style="width:540px; height:25px"></div>
        <div class="columnLeft">
            <label>Name:</label>
        </div>
        <div class="columnRight">
            <telerik:RadTextBox runat="server" ID="txtName" TabIndex="1" Width="100%" BorderStyle="Solid" BorderColor="black" BorderWidth="1px" MaxLength="50" />
            <asp:RequiredFieldValidator runat="server" ID="nameValidator" ControlToValidate="txtName" ValidationGroup="main" ErrorMessage="<span style='color: red;'>Group Name is Required</span>" />
            <asp:CustomValidator runat="server" ID="DupeValidator" OnServerValidate="CheckForDuplicateGroupName" ControlToValidate="txtName" Display="Dynamic" ValidationGroup="main" ErrorMessage="<span style='color: red;float: left'>This Group Name is already taken - Please choose another</span>" />
        </div>
        <br style="clear: both;" />
        <br />
        <div class="columnLeft">
            <label>Description:</label>
        </div>
        <div class="columnRight">
            <telerik:RadTextBox runat="server" ID="txtDescription" Width="100%" Height="200px" BorderStyle="Solid" BorderColor="black" BorderWidth="1px" TextMode="MultiLine" MaxLength="500" />
        </div>
        <br style="clear: both;" />
        <br style="clear: both;" />
        <div class="buttonGroup">
            <telerik:RadButton Skin="Web20" runat="server" ID="btnCancel" Text="Cancel" AutoPostBack="False" OnClientClicked="CloseDialog" />
            <telerik:RadButton Skin="Web20" runat="server" ID="btnOK" Text="OK" OnClick="Save" AutoPostBack="True" OnClientClicking="SaveClicking" CausesValidation="True" ValidationGroup="main" />
        </div>
    </div>
    <asp:HiddenField runat="server" ID="hdnTargetType" />
</asp:Content>
