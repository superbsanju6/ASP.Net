<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewBlueprints.ascx.cs" 
    Inherits="Thinkgate.Controls.Assessment.ViewBlueprints" %>

<style>
    .rbSkinnedButton {
        vertical-align:top;
    }
    .rbDecorated {
        padding-right:10px !important;
    }
</style>


 <telerik:RadAjaxPanel ID="viewBlueprintAjaxPanel" runat="server" LoadingPanelID="viewBlueprintLoadingPanel">
    <!-- Filter combo boxes -->
 
        <div style="z-index: 1; height: 24px; margin-left: 4px; margin-top: 1px;">
        <telerik:RadComboBox ID="cmbYear" runat="server" ToolTip="Select a year" 
			Skin="Web20" Width="60"
			OnSelectedIndexChanged="cmbYear_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
            <ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>

         </telerik:RadComboBox>
        	<telerik:RadComboBox ID="cmbGrade" runat="server" ToolTip="Select a grade" 
			Skin="Web20" Width="60"
			OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
			<ItemTemplate>
				<span><%# Eval("Grade") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>

        <telerik:RadComboBox ID="cmbSubject" runat="server" ToolTip="Select a subject" 
			Skin="Web20" Width="60"
			OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true" DropDownWidth="200px">
			<ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>

        <telerik:RadComboBox ID="cmbStatus" runat="server" ToolTip="Select a test status" 
			Skin="Web20" Width="75"
			OnSelectedIndexChanged="cmbStatus_SelectedIndexChanged" AutoPostBack="true" 
			CausesValidation="False" HighlightTemplatedItems="true">
			<ItemTemplate>
				<span><%# Eval("DropdownText") %></span>
			</ItemTemplate>
		</telerik:RadComboBox>  
            
           

            <telerik:RadButton ID="btnOk" runat="server" ToolTip="Filter blueprints" Skin="Web20" Width="30" Text="OK"
                OnClick="btnOk_OnClick"></telerik:RadButton>
         
        </div>

    <!-- Pages -->
    <!-- Graphic View -->
    <div class="graphicalView" id="divGraphicalView" runat="server">
        <asp:Panel ID="pnlGraphicNoResults" runat="server" Visible="false" Height="190px">
         <div style="width: 100%; text-align: center;">No blueprints found for selected criteria.</div>
		</asp:Panel>

        <asp:Panel ID="pnlFilterSelection" runat="server" Visible="false" Height="190px">
         <div style="width: 100%; text-align: center;">Please select search criteria.</div>
        </asp:Panel>

        <telerik:RadListBox runat="server" ID="lbxGraphic" Width="100%" Height="190px" OnItemDataBound="lbxList_ItemDataBound">
            <ItemTemplate>
                <!-- Hidden fields to pass parameters to javascript. -->
                <asp:HiddenField ID="inpGraphicAssessmentID" runat="server" Value='<%# Eval("PacingGuideID")%>'/>
                <!-- Blueprint icon -->
                <asp:Image ID="testImg" Style="float: left; padding: 2px;" Width="47" Height="56"
						ImageUrl='~/Images/editable.png' runat="server" />

                <!-- blueprint name, Line 1 -->
                <div>
                    <asp:Label ID="lblGrade" runat="server" ToolTip='<%# Eval("Grade")%>'><%# Eval("Grade")%></asp:Label>
                    <asp:Label ID="lblSubject" runat="server" ToolTip='<%# Eval("Subject")%>'><%# Eval("Subject")%></asp:Label>
                    <asp:Label ID="lblCourse" runat="server" ToolTip='<%# Eval("Course")%>'><%# Eval("Course")%></asp:Label>
               </div>

                <!-- More Results line -->
				<asp:Panel ID="graphicMore" runat="server" Visible="False">
					<a href="#" id="lnkGraphicMore" runat="server" onclick="">More Results...</a>
				</asp:Panel>

                <!-- Line 2 -->
                <asp:Panel ID="graphicLine2Summary" runat="server" Visible="True">
                    <asp:HyperLink ID="lnkGraphicBlueprint" runat="server" NavigateUrl="" Target="_blank" Visible="True" ForeColor="Blue">Blueprint</asp:HyperLink>
                    <asp:HyperLink ID="lnkGraphicGuide" runat="server" NavigateUrl="" Target="_blank" Visible="True" ForeColor="Blue">Guide</asp:HyperLink>
                    <asp:HyperLink ID="lnkGraphicSummary" runat="server" NavigateUrl="" Target="_blank" Visible="True" ForeColor="Blue">Summary</asp:HyperLink>

                    
                </asp:Panel>

               
                </ItemTemplate>
            </telerik:RadListBox>
    </div>
    </telerik:RadAjaxPanel>
<%--<telerik:RadAjaxLoadingPanel ClientIDMode="Static" ID="viewBlueprintLoadingPanel"
runat="server" />--%>
