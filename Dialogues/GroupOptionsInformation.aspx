<%@ Page Title="" Language="C#" MasterPageFile="~/Dialogue.Master" AutoEventWireup="true" CodeBehind="GroupOptionsInformation.aspx.cs" Inherits="Thinkgate.Dialogues.GroupOptionsInformation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseDialog() {
            GetRadWindow().close();
        }
    </script>
    <style type="text/css">
        /*body {
            background-color: white;
        }*/

        #container {
            width: 375px;
            height: 100%;
            margin-left: auto;
            margin-right: auto;
            margin-top: 30px;
        }

        div.columnLeft {
            float: left;
            width: 60px;
        }

        div.columnRight {
            float: right;
            width: 315px;
        }

        label.term {
            font-weight: bold;
        }

        div.buttonGroup {
            float: right;
            margin-right: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="container">
        <div class="columnLeft">
            <label class="term">Private:</label>
        </div>
        <div class="columnRight">
            <label>All users are able to create private groups.  Only the user who creates the group is able to search for, view and modify the group.</label>
        </div>
        <br style="clear: both;" />
        <br/><br/>
        <div class="columnLeft">
            <label class="term">Public:</label>
        </div>
        <div class="columnRight">
            <label>
                Public groups are targeted to either the district or one or more schools.  Targeting options available are controlled by the school(s) a user is tied to.  Targeting controls who has access to the group.
            </label>
            <br/><br/>
            <label>
                <span style="text-decoration: underline">Read Only</span>
                <br />
                When a group is marked as Read-Only, targets are only allowed to view the group, but cannot add or remove students.  When a group is NOT marked as Read-Only, targets are able to search for, view and add/remove students.
            </label>
        </div>
        <br style="clear: both;" />
        <br/><br/>
        <div class="buttonGroup">
            <telerik:RadButton runat="server" ID="btnClose" Text="Close" Skin="Web20" AutoPostBack="False" OnClientClicked="CloseDialog" CausesValidation="False" />
        </div>
    </div>

</asp:Content>
