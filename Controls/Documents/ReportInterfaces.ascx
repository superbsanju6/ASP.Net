<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportInterfaces.ascx.cs"
    Inherits="Thinkgate.Controls.Documents.ReportInterfaces" %>

    <div class="btnWrap">
        <telerik:RadButton runat="server" ID="RadButton1" Width="90"
            Height="90" CssClass="tasks" Skin="Default">
            <ContentTemplate>
                <asp:Image ID="reportEngineImg" runat="server" ImageUrl="~/Images/gear.png" />
                <span class="btnText">Report Engine</span>
            </ContentTemplate>
        </telerik:RadButton>
        <telerik:RadButton runat="server" ID="RadButton2" Width="90"
            Height="90" CssClass="people" Skin="Default">
            <ContentTemplate>
                <asp:Image ID="reportEngineImg" runat="server" ImageUrl="~/Images/tools2.gif" />
                <span class="btnText">Report Builder</span>
            </ContentTemplate>
        </telerik:RadButton>
        <telerik:RadButton runat="server" ID="RadButton3" Width="90"
            Height="90" CssClass="people" Skin="Default">
            <ContentTemplate>
                <asp:Image ID="reportEngineImg" runat="server" ImageUrl="~/Images/Clipboard.png" />
                <span class="btnText" style="font-size:8pt;">Proficiency Analysis</span>
            </ContentTemplate>
        </telerik:RadButton>
        <telerik:RadButton runat="server" ID="RadButton4" Width="90"
            Height="90" CssClass="tasks" Skin="Default">
            <ContentTemplate>
                <asp:Image ID="reportEngineImg" runat="server" ImageUrl="~/Images/push_pin.png" />
                <span class="btnText">State Analysis</span>
            </ContentTemplate>
        </telerik:RadButton>
    </div>