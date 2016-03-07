<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThinkgateUniversity.ascx.cs"
    Inherits="Thinkgate.Controls.Teacher.ThinkgateUniversity" %>
<telerik:RadAjaxPanel runat="server" ID="thinkgateUniversityPanel" LoadingPanelID="thinkgateUnivLoadingPanel">
    <telerik:RadComboBox ID="cmbCourseType" runat="server" ToolTip="Select a type" Skin="Web20"
        Width="125" OnSelectedIndexChanged="cmbCourseType_SelectedIndexChanged" AutoPostBack="true"
        CausesValidation="False" HighlightTemplatedItems="true">
    </telerik:RadComboBox>
    <telerik:RadComboBox ID="cmbEnrollmentType" runat="server" ToolTip="Select an enrollment type" Skin="Web20"
        Width="125" OnSelectedIndexChanged="cmbEnrollmentType_SelectedIndexChanged" AutoPostBack="true"
        CausesValidation="False" HighlightTemplatedItems="true">
    </telerik:RadComboBox>
    <telerik:RadMultiPage runat="server" ID="RadMultiPageThinkgateUniversity" SelectedIndex="0"
        Height="185px" Width="300px" CssClass="multiPage">
        <telerik:RadPageView runat="server" ID="radPageRegistered">
           <div class="listView" id="divRegisteredList" runat="server">
				<asp:Panel ID="registeredNoResults" runat="server" Visible="false" Height="185px">
					<div style="width: 100%; text-align: center;">No courses found.</div>
				</asp:Panel>
				<telerik:RadListBox runat="server" ID="lbxRegisteredList" Width="100%" Height="185px" OnItemDataBound="lbxRegisteredList_ItemDataBound">
				<ItemTemplate>					
                    <table width="100%">
                        <tr>
                            <td>                                
                                <asp:Image ID="imgCourseType" runat="server" AlternateText='<%# Eval("CourseType")%>'/>
                            </td>
                            <td style="vertical-align: top">
                                <asp:HyperLink ID="lnkCourseName" runat="server" NavigateUrl="" Target="_blank"><%# Eval("CourseName")%></asp:HyperLink><br />
                                <asp:Image ID="imgCourseRequired" runat="server" AlternateText="Required" ImageUrl="~/Images/alert.png" Visible="false" />
                                <asp:ImageButton ID="imgBtnLaunch" runat="server" AlternateText="Launch Course" ImageUrl="~/Images/go.png" Visible="false" />
                            </td>
                        </tr>
                    </table>									
				</ItemTemplate>
			</telerik:RadListBox>
		</div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewInProgress">
           
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="radPageViewCompleted">
           
        </telerik:RadPageView>
         <telerik:RadPageView runat="server" ID="radPageViewNew">
           
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <telerik:RadTabStrip runat="server" ID="thinkgateUniversityRadTabStrip" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageThinkgateUniversity" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left" OnClientTabSelected="toggleView_RadTab_SmallTile">
        <Tabs>
            <telerik:RadTab Text="Registered" Width="75px">
            </telerik:RadTab>
            <telerik:RadTab Text="In Progress" Width="75px">
            </telerik:RadTab>
            <telerik:RadTab Text="Completed" Width="75px">
            </telerik:RadTab>
             <telerik:RadTab Text="New" Width="70px">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="thinkgateUnivLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>
