<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContentEditor_RubricItem.aspx.cs"
    Inherits="Thinkgate.Controls.Assessment.ContentEditor.ContentEditor_RubricItem" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link id="Link1" href="~/Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=4" rel="stylesheet" type="text/css" runat="server" />
</head>
<body>
    <asp:PlaceHolder runat="server">
        <script type="text/javascript">
            function GetEquationEditorURL() {
                var _EquationEditorURL = '<%=ConfigurationManager.AppSettings["EquationEditorURL"].ToString() %>';
                return _EquationEditorURL;
            }
        </script>
   </asp:PlaceHolder>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            <asp:ScriptReference Path="~/Scripts/ckeditor/ckeditor.js"/>
            <asp:ScriptReference Path="~/Scripts/ckeditor/adapters/jquery.js"/>
            <asp:ScriptReference Path="~/Scripts/master.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <script type="text/javascript">
        var $ = $telerik.$;
        var winEditRubricItem = getCurrentCustomDialog();
        winEditRubricItem.addConfirmDialog({ title: 'Cancel Rubric Item Edit', text: 'Proceeding with Cancel discards any entries made and does not update the rubric.  Select OK to continue with the Cancel action or Close to return to the current window.' }, window);

        $(document).ready(function () {
            if (typeof CKEDITOR != "undefined") {
                CKEDITOR.replace("CkEditorRubricItemEdit", { toolbar: 'rubricEditor', height: "490px" });
            }
            setup_rubric_item_edit();
         });
        function setup_rubric_item_edit() {
            var id = getURLParm('xID');
            var content = '';
            if (parent && parent.document.getElementById(id)) {
                if (parent.document.getElementById(id).innerHTML != null) {
                    content = parent.document.getElementById(id).innerHTML;
                }
            }
            if (parent.document.getElementById(id).innerText != 'Click to Edit') {
                CKEDITOR.instances.CkEditorRubricItemEdit.setData(content);
            }
        }
        function SetContent() {
            var id = getURLParm('xID');
            var content = CKEDITOR.instances.CkEditorRubricItemEdit.getData();
            if (parent && parent.document.getElementById(id)) {
                if (parent.document.getElementById(id).innerHTML != null) {
                    parent.document.getElementById(id).innerHTML = content;
                    parent.SaveRubricItem(id);
                }
            }
            //Since the user clicked Save, we do not want to confirm before closing our window.
            winEditRubricItem.remove_beforeClose(winEditRubricItem.confirmBeforeClose);
            closeCurrentCustomDialog();
        }

        function getURLParm(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regexS = "[\\?&]" + name + "=([^&#]*)";
            var regex = new RegExp(regexS);
            var results = regex.exec(window.location.href);
            if (results == null)
                return "";
            else
                return decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        String.prototype.trimString = function() {
            return this.replace(/^\s+|\s+$/g, "");
        };

        function CancelContent() {
            closeCurrentCustomDialog();
        }
    </script>
    <div style="max-width:550px;">
        <div id="CkEditorRubricItemEdit" style="width: 100%;">
        </div>
        
    </div>
    <input type="button" onclick="SetContent();" value="Save" />
    <input type="button" onclick="CancelContent();" value="Cancel" />
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false" Skin="Thinkgate_Window" EnableEmbeddedSkins="false" >
     <ConfirmTemplate>
            <div class="rwDialogPopup radconfirm">
                <div class="rwDialogText">
                    {1}
                </div>
                <div>
                    <a onclick="$find('{0}').close(true);" class="rwPopupButton" href="javascript:void(0);">
                        <span class="rwOuterSpan"><span class="rwInnerSpan">##LOC[OK]##</span></span></a>
                    <a onclick="$find('{0}').close(false);" class="rwPopupButton" href="javascript:void(0);">
                        <span class="rwOuterSpan"><span class="rwInnerSpan">##LOC[Close]##</span></span></a>
                </div>
            </div>
        </ConfirmTemplate>    
    </telerik:RadWindowManager>

    </form>
</body>
</html>
