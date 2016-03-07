<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Folders.ascx.cs" Inherits="Thinkgate.Controls.Folders" %>
<script type="text/javascript">
    function folderClick(text) {
        var pos = window.location.href.indexOf("&folder");
        if (pos > -1)
            window.location.href = window.location.href.substring(0, pos) + "&folder=" + text;
        else
            window.location.href = window.location.href + "&folder=" + text;

        return false;
    }
</script>
<asp:Repeater runat="server" ID="foldersRepeater" ClientIDMode="Static" 
    onitemdatabound="foldersRepeater_ItemDataBound">
    <HeaderTemplate>
        <div style='clear: both;'>
    </HeaderTemplate>
    <ItemTemplate>
        <div id="folderDiv" class="tileFolderDiv" runat="server">
<%--            <script type="text/javascript">
                var folderText = '<%# Eval("Text") %>';
                var selectedFolderText = '<%# Request.QueryString["folder"] %>';

                if (selectedFolderText != null) {
                    if (folderText == selectedFolderText) {
                        //alert($("#folderIcon"));
                        $("#folderIcon").attr("selected", "selected");
                    }
                }

            </script>--%>
            <a id="folderIcon" runat="server" CommandArgument='<%# Eval("Text") %>' class="FolderIconImageButton" href="javascript:void(0)">
                <%# Eval("Text") %>
            </a>
            <div id="folderDividerDiv" runat="server">
                &nbsp; | &nbsp;</div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
</asp:Repeater>
