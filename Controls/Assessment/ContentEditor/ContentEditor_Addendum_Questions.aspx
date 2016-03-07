<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor_Addendum_Questions.aspx.cs"
    Inherits="Thinkgate.Controls.Assessment.ContentEditor.ContentEditor_Addendum_Questions" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="PageTitle" runat="server"></title>
    <style>
        .divcenter
        {
            height:300px;
            width:600px;
            margin:0px;
            min-width:600px;
            min-height:300px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server" />

     <div class="divcenter">
        <telerik:RadGrid ID="RadGrid1" AllowFilteringByColumn="false" runat="server" AllowPaging="false"
            AutoGenerateColumns="false" ShowStatusBar="false" AllowSorting="false" OnNeedDataSource="RadGrid1_NeedDataSource" OnItemDataBound="RadGrid1_ItemDataBound1">
            <MasterTableView PagerStyle-AlwaysVisible="false">
                <Columns>
                    <telerik:GridBoundColumn DataField="Question_Text" HeaderText="Item" UniqueName="Item">
                        <ItemStyle Width="120px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridMaskedColumn HeaderText="Distractors" UniqueName="Distractors" >
                        <ItemStyle Width="120px" />
                    </telerik:GridMaskedColumn>
                    <telerik:GridBoundColumn DataField="SourceName" HeaderText="District" UniqueName="SourceName">
                        <ItemStyle Width="120px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CreatedByName" HeaderText="Created By" UniqueName="CreatedByName">
                        <ItemStyle Width="120px" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
    </form>
</body>
</html>
