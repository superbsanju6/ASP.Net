<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Demographics.ascx.cs" Inherits="Thinkgate.CriteriaControls.Demographics" %>
<asp:Repeater ID="repeaterDemographics" runat="server"></asp:Repeater>
<br />

Race:
<telerik:RadComboBox ID="radComboRace" runat="server" Skin="Web20" EmptyMessage="All"></telerik:RadComboBox>