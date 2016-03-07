<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RosterPortletPage.aspx.cs" Inherits="Thinkgate.RosterPortal.RosterPortletPage" %>

<!DOCTYPE html>

<%--<html xmlns="http://www.w3.org/1999/xhtml">--%>

<!--Title to Indicate this is MA's "Roster Portlet" that bridges over from MA's EOE domain site-->
<head runat="server">
    <title>EOE to E3 Roster Portal Page</title>
</head>
<form id="RosterPortal" title="Roster Portal" runat="server">
    <h2 style="color: olive; width: 425px;">EOE to E3 Mass Roster Portlet</h2>
    <!--Div for RadComboBox-->
    <div id="classComboBox" runat="server">
        <telerik:RadComboBox runat="server" ID="dropDownClasses" Skin="Web20" AutoPostBack="true" Width="307" ShowDropDownOnTextboxClick="true" EmptyMessage="Select a Class"
            OnSelectedIndexChanged="dropDownClasses_SelectedIndexChanged">
            <ItemTemplate>
                <span><%# Eval("FriendlyName")%></span>
            </ItemTemplate>
        </telerik:RadComboBox>
    </div>
    <div style="width: 300px; height: 230px; overflow: auto; padding: 3px;">
        <!--This is div for RadChart - studentChart-->
        <div id="studentChart" runat="server" onclick="window.open('../PortalSelection.aspx')">
            <telerik:RadChart ID="studentCountChart" runat="server" Width="300px" Height="210px"
                DefaultType="Pie" AutoLayout="true" AutoTextWrap="False" CreateImageMap="true">

                <Appearance TextQuality="AntiAlias"></Appearance>

                <PlotArea>
                    <Appearance Dimensions-Margins="18%, 24%, 22%, 10%"></Appearance>

                    <XAxis LayoutMode="Inside" AutoScale="false">
                        <Appearance>
                            <LabelAppearance RotationAngle="45" Position-AlignedPosition="Top"></LabelAppearance>
                        </Appearance>
                    </XAxis>

                    <YAxis IsZeroBased="false"></YAxis>
                </PlotArea>
            </telerik:RadChart>
        </div>
        <!--div for studentList-->
        <div id="studentList" runat="server">

            <div class="listView" style="display: block;">
                <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
                    <Scripts>
                        <asp:scriptreference assembly="Telerik.Web.UI" name="Telerik.Web.UI.Common.Core.js" />
                        <asp:scriptreference assembly="Telerik.Web.UI" name="Telerik.Web.UI.Common.jQuery.js" />
                        <asp:scriptreference assembly="Telerik.Web.UI" name="Telerik.Web.UI.Common.jQueryInclude.js" />
                        <asp:scriptreference path="~/Scripts/fullscreenBackground.js" />
                        <asp:scriptreference path="~/Scripts/master.js" />
                        <asp:scriptreference path="https://www.google.com/jsapi" />
                    </Scripts>
                </telerik:RadScriptManager>
                <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
                </telerik:RadAjaxManager>

                <!-- RadGrid -->
                <div>
                    <telerik:RadGrid runat="server" ID="rosterGrid" ClientIDMode="Static" AutoGenerateColumns="false"
                        Width="100%" AllowFilteringByColumn="false" OnItemDataBound="SetRtiImage">
                        <MasterTableView AllowFilteringByColumn="true" AutoGenerateColumns="false" ShowHeader="false" ShowHeadersWhenNoRecords="false">
                            <Columns>
                                <telerik:GridTemplateColumn InitializeTemplatesFirst="false" ItemStyle-CssClass="searchResults_smallTile" ItemStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <a href="../Record/Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>" onclick="window.open('../Record/Student.aspx?childPage=yes&xID=<%# Eval("ID_Encrypted") %>'); return false;"><%# Eval("StudentName") %></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </div>
        </div>
    </div>
    <%--    <span style="display: none;">   This is not needed for the time being.  In case it is needed, I will leave it here.
        <telerik:RadXmlHttpPanel runat="server" ID="studentPieChartXmlHttpPanel" ClientIDMode="Static"
            Value="" WcfRequestMethod="POST" WcfServicePath="~/Services/PieChartWCF.svc"
            WcfServiceMethod="OpenExpandedWindow" RenderMode="Block" OnClientResponseEnding="openPieChartExpandedWindow">
        </telerik:RadXmlHttpPanel>
    </span>--%>
</form>



