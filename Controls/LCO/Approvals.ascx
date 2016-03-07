<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Approvals.ascx.cs" Inherits="Thinkgate.Controls.LCO.Approvals" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="tk" %>
<script type="text/javascript">
    function showAddNewApprovalWindow() {
        customDialog({ url: '../Controls/LCO/AddCourse.aspx', maximize: true, maxwidth: 550, maxheight: 500 });
    }
</script>
<style>
    .ModalYesNo {
        background-color: #666699;
        filter: alpha(opacity=50);
        opacity: 0.7;
    }
</style>





<telerik:RadAjaxPanel ID="LCOApprovalAjaxPanel" runat="server" LoadingPanelID="LCOApprovalLoadingPanel">

<telerik:RadMultiPage runat="server" ID="RadMultiPageApprovals" SelectedIndex="0"
    Height="210px" Width="310px" CssClass="multiPage">
    <telerik:RadPageView runat="server" ID="RadPageViewApprovalsApproved">
       <div>
            <telerik:RadListBox Visible="false" runat="server" ID="RadListBoxApprovalsApproved" Width="100%" Height="210px">
                <ItemTemplate>
                    <div>
                        <asp:HyperLink ID="lnkLEA" runat="server" Target="_blank" Visible="True"><%# Eval("Text")%></asp:HyperLink>
                    </div>
                </ItemTemplate>
            </telerik:RadListBox>
        </div>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="RadPageViewApprovalsPending">
        <div>
            
            <telerik:RadListBox runat="server" ID="RadListBoxApprovalsPending" Width="100%" Height="210px">
            <ItemTemplate>
                <div>
                    <asp:HyperLink ID="lnkLEA" runat="server" Target="_blank" Visible="True"><%# Eval("Text")%></asp:HyperLink>
                </div>
            </ItemTemplate>
        </telerik:RadListBox>
          
        </div>
       
    </telerik:RadPageView>
</telerik:RadMultiPage>
<div class="tabsAndButtonsWrapper_smallTile">
    <telerik:RadTabStrip runat="server" ID="tabs" Orientation="HorizontalBottom"
        SelectedIndex="0" MultiPageID="RadMultiPageApprovals" Skin="Thinkgate_Blue"
        EnableEmbeddedSkins="false" Align="Left">
        <Tabs>
            <telerik:RadTab Text="Approved">
            </telerik:RadTab>
            <telerik:RadTab Text="Pending">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
   
</div>
 <div id="divAdd" runat="server" visible="false" class="searchTileBtnDiv_smallTile" title="Add New Course" style="margin-top: 1px; Float: right; width: 18%;">
            <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
            <asp:Button ID="BtnAdd" UseSubmitBehavior="true" runat="server" OnClick="btnAddNew_Click" Text="Add New" Style="background-color: transparent; border-width: 0px; margin-top: 1px; float: right; width: 95%;"></asp:Button>
        </div>
   
    <asp:Panel runat="server" ID="resultPanel" Style="display: none;" Width="250" Height="50" BackColor="ControlDark" ClientIDMode="Static" BorderWidth="3" BorderStyle="Solid">
        <asp:Label runat="server" ID="lblResultMessage" Text="Are you sure you want to add this course?" />
        <br />
        <div style="float: left; padding-left: 80px; padding-top: 10px;">
            <telerik:RadButton runat="server" ID="btnYes" Text="Yes" OnClick="btnYes_Click" />
            &nbsp;
        <telerik:RadButton runat="server" ID="btnNo" Text="No" />
        </div>
        <tk:ModalPopupExtender ID="modConfirm" runat="server" TargetControlID="BtnAdd" BackgroundCssClass="ModalYesNo" PopupControlID="resultPanel"></tk:ModalPopupExtender>
    </asp:Panel>

     
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ID="LCOApprovalLoadingPanel" runat="server" />










<%--


<telerik:RadAjaxPanel ID="LCOApprovalAjaxPanel" runat="server" LoadingPanelID="LCOApprovalLoadingPanel">
    
    <div runat="server">
        <telerik:RadListBox Visible="false" runat="server" ID="RadListBoxApprovalsApproved" Width="100%" Height="210px">
            <ItemTemplate>
                <div>
                    <asp:HyperLink ID="lnkLEA" runat="server" Target="_blank" Visible="True"><%# Eval("Text")%></asp:HyperLink>
                </div>
            </ItemTemplate>
        </telerik:RadListBox>
    </div>
    <div runat="server">
        <telerik:RadListBox runat="server" ID="RadListBoxApprovalsPending" Width="100%" Height="210px">
            <ItemTemplate>
                <div>
                    <asp:HyperLink ID="lnkLEA" runat="server" Target="_blank" Visible="True"><%# Eval("Text")%></asp:HyperLink>
                </div>
            </ItemTemplate>
        </telerik:RadListBox>
    </div>

    <div>
        <div style="float: left; width: 50%;">
            <telerik:RadTabStrip runat="server"
                ID="LCOApprovals_RadTabStrip" Orientation="HorizontalBottom"
                Skin="Thinkgate_Blue" CausesValidation="false" AutoPostBack="true" OnTabClick="LCOApprovals_RadTabStrip_TabClick"
                EnableEmbeddedSkins="False">
                <Tabs>
                    <telerik:RadTab runat="server" ID="radTabApproval" Text="Approved">
                    </telerik:RadTab>
                    <telerik:RadTab runat="server" ID="radTabPending" Text="Pending" Selected="true">
                    </telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
        </div>
        <div id="divAdd" runat="server" visible="false" class="searchTileBtnDiv_smallTile" title="Add New Course" style="margin-top: 1px; Float: right; width: 18%;">
            <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
            <asp:Button ID="btnAdd" UseSubmitBehavior="true" runat="server" OnClick="btnAddNew_Click" Text="Add New" Style="background-color: transparent; border-width: 0px; margin-top: 1px; float: right; width: 95%;"></asp:Button>
        </div>
    </div>
    <asp:Panel runat="server" ID="resultPanel" Style="display: none;" Width="250" Height="50" BackColor="ControlDark" ClientIDMode="Static" BorderWidth="3" BorderStyle="Solid">
        <asp:Label runat="server" ID="lblResultMessage" Text="Are you sure you want to add this course?" />
        <br />
        <div style="float: left; padding-left: 80px; padding-top: 10px;">
            <telerik:RadButton runat="server" ID="btnYes" Text="Yes" OnClick="btnYes_Click" />
            &nbsp;
        <telerik:RadButton runat="server" ID="btnNo" Text="No" />
        </div>
        <tk:ModalPopupExtender ID="modConfirm" runat="server" TargetControlID="btnAdd" BackgroundCssClass="ModalYesNo" PopupControlID="resultPanel"></tk:ModalPopupExtender>
    </asp:Panel>
</telerik:RadAjaxPanel>
<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="LCOApprovalLoadingPanel" runat="server" />--%>
