<%@ Page Title="" Language="C#" MasterPageFile="~/Dialogue.Master" AutoEventWireup="true" CodeBehind="DeleteGroup.aspx.cs" Inherits="Thinkgate.Dialogues.DeleteGroup" %>

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
        function OkClicking() {
            var btnOk = window.$find('<%= btnOK.ClientID %>');
            btnOk.set_enabled(false);
        }
        
    </script>
    <style type="text/css">
        #container {
            width: 95%;
            margin-left: auto;
            margin-right: auto;
            margin-top: 30px;
        }
                   
        .prompt {
            font-weight: bold;
            font-size:medium;
        }

        .header {
            font-size:small medium;
            margin-left: auto;
            margin-right: auto;
            width: 98%;
        }

        .optionPanel {
            margin-bottom: 30px;
            margin-left: auto;
            margin-right: auto;
            margin-top: 50px;
            width: 50px;
        }

        .buttonPanel {
            float: right;
            margin-right: 10px;
        }

        .RadGrid_Default th.rgHeader {
            background-image: none;
            background-color: white;
        }
        .groupDetail {
            margin-bottom: 30px;
            margin-left: auto;
            margin-right: auto;
            width: 98%;
        }
       
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="container">
        <div class="groupDetail">
            <telerik:RadGrid runat="server" ID="grdDetail" OnNeedDataSource="PopulateGroupDetail" BorderStyle="None" OnItemDataBound="ApplyToolTips">
                <MasterTableView AutoGenerateColumns="False" AllowSorting="False" DataKeyNames="DisplayName,Description">
                    <HeaderStyle BorderStyle="None" />
                    <Columns>
                        <telerik:GridBoundColumn DataField="DisplayNameShort" HeaderText="Name" UniqueName="DisplayNameShortColumn">
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DescriptionShort" HeaderText="Description"  UniqueName="DescriptionShortColumn">
                            <ItemStyle BorderStyle="None"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="CreatedByUserFullName" HeaderText="Created By">
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="CreatedDate" HeaderText="Created Date">
                            <ItemStyle BorderStyle="None" />
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings>
                    <Scrolling AllowScroll="False"></Scrolling>
                </ClientSettings>
            </telerik:RadGrid>
        </div>
        <div class="header">
            <label class="prompt">Are you sure you want to DELETE this group?</label>
            <br /><br />
            <label >There may be assignments and resources using this group that are affected by this group's removal.</label>
        </div>
        <br /><br />
        <div class="optionPanel">
            <telerik:RadButton runat="server" AutoPostBack="False" Font-Size="14" ButtonType="ToggleButton" ToggleType="Radio" Text="No" ID="optNo" GroupName="Delete" Checked="True" />
            <br />
            <telerik:RadButton runat="server" AutoPostBack="False" Font-Size="14" ButtonType="ToggleButton" ToggleType="Radio" Text="Yes" ID="optYes" GroupName="Delete" />
        </div>
        <div class="buttonPanel">
            <telerik:RadButton runat="server" ID="btnOK" Text="OK" OnClick="OkClick" AutoPostBack="True" OnClientClicking="OkClicking" UseSubmitBehavior="False"/>
        </div>
    </div>
</asp:Content>
