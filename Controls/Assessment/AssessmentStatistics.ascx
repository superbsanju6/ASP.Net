<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentStatistics.ascx.cs" Inherits="Thinkgate.Controls.Assessment.AssessmentStatistics" %>
<link href="~/Styles/Site.css" rel="stylesheet" type="text/css"/>



    <telerik:RadlistBox runat="server"  ID="lbxOriginalStatistics" Width="100%" Height="210px" OnItemDataBound="lbxList_ItemDataBound" BackColor="White">
        <ItemTemplate>
            <asp:Label runat="server" ID="lblDesc" Width="135px" style="text-align: left;" BackColor="White"></asp:Label>
            <asp:Label runat="server" ID="lblValue" Width="135px" style="text-align: right;" BackColor="White"></asp:Label>
        </ItemTemplate>
    </telerik:RadlistBox>



