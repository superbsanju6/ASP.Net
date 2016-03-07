<%@ Page Title="Item Search" Language="C#" AutoEventWireup="true" CodeBehind="SearchTemplate.aspx.cs" Inherits="Thinkgate.Controls.SearchTemplate" MasterPageFile="~/Search.Master"%>
<%@ Register TagPrefix="e3" TagName="SearchAndClear" Src="~/Controls/E3Criteria/SearchAndClear.ascx"  %>
<%@ MasterType virtualpath="~/Search.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="RightColumnHeaderContentPlaceHolder" runat="server">
    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="LeftColumnHeaderContentPlaceHolder" runat="server">
    <e3:SearchAndClear ID="SearchAndClear" runat="server" StarterText="Search" AfterSearchText="Update Results"/>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="LeftColumnContentPlaceHolder" runat="server">
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="RightColumnContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="gridResultsPanel">
        <asp:PlaceHolder ID="initialDisplayText" runat="server"></asp:PlaceHolder>
    </asp:Panel>
   
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="EndOfFormContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ScriptPlaceHolder" runat="server">
    <script type="text/javascript">
        function alterPageComponents(state) {

            $('.rgHeaderDiv').each(function () {
                this.style.width = '';
            });

        }

        window.onload = function() {
            alterPageComponents('moot');
        };
        
    </script>
</asp:Content>



<asp:Content ID="Content7" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        
        #rightColumn
        {
            background-color: white;
        }
        
    </style>
</asp:Content>