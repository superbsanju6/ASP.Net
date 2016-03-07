<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupStudent.ascx.cs" Inherits="Thinkgate.Controls.Groups.GroupStudent" %>

<telerik:RadGrid ID="radGridStudentGroups" runat="server" Width="300px" Height="190px" 
    AutoGenerateColumns="false" OnNeedDataSource="LoadGroupsForCurrentStudent" Style="background-image: url(../../Images/transparent_bkgd.png);">
    <MasterTableView>
        <Columns>
            <telerik:GridBoundColumn DataField="Name" HeaderText="Name" UniqueName="GroupNameColumn">
                <HeaderStyle Width="150"></HeaderStyle>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="StudentCount" HeaderText="# in Group" UniqueName="StudentCount">
                <HeaderStyle Width="75"></HeaderStyle>
            </telerik:GridBoundColumn>    
            <telerik:GridBoundColumn DataField="CreatedDate" HeaderText="Added" UniqueName="CreatedDateColumn" DataFormatString="{0:M/d/yy}">
                <HeaderStyle Width="75"></HeaderStyle>
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
    <ClientSettings>
        <Scrolling AllowScroll="True" />
    </ClientSettings>
</telerik:RadGrid>
