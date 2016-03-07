<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentCoverPageEditor.aspx.cs" Inherits="Thinkgate.ControlHost.AssessmentCoverPageEditor" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Thinkgate_Blue/TabStrip.Thinkgate_Blue.css?v=2" rel="stylesheet" type="text/css" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
        <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <title></title>
    <base target="_self" />
    <telerik:RadStyleSheetManager ID="radCSSManager" runat="server">
    </telerik:RadStyleSheetManager>
    <script type='text/javascript' src='../Scripts/jquery-1.9.1.min.js'></script>
    <script type='text/javascript' src='../Scripts/jquery.scrollTo.js'></script>
    <script type="text/javascript">var $j = jQuery.noConflict();</script>
    <style>
        .roundButtons
        {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            border: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }

        .hdnEditorContentCss {
            
        }
    </style>
    <script type="text/javascript">
        var modalWin = parent.$find('RadWindow1Url');
    </script>
</head>
<body style="background-image: none !important;">
    <asp:PlaceHolder runat="server">
        <script type="text/javascript">
            function GetEquationEditorURL() {
                var _EquationEditorURL = '<%=ConfigurationManager.AppSettings["EquationEditorURL"].ToString() %>';
                return _EquationEditorURL;
            }
        </script>
   </asp:PlaceHolder>
    <form id="form3" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/Scripts/master.js"/>
                <asp:ScriptReference Path="~/Scripts/ckeditor/ckeditor.js"/>
                <asp:ScriptReference Path="~/Scripts/ckeditor/adapters/jquery.js"/>
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadScriptBlock runat="server">
            <script type="text/javascript">
                $(document).ready(function() {
                    if (CKEDITOR !== 'undefined') {
                        CKEDITOR.replace("coverSheetCkEditor", { toolbar: 'assessmentCoverPageEditor', height: "500px" });
                    }
                });
                
            </script>
        </telerik:RadScriptBlock>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        </telerik:RadAjaxManager>
        <telerik:RadSkinManager ID="RadSkinManager1" runat="server" Skin="Transparent">
        </telerik:RadSkinManager>
        <telerik:RadAjaxPanel ID="updPanel" runat="server" LoadingPanelID="updPanelLoadingPanel" Width="100%" Height="100%">
            <input type="hidden" id="assessmentIDEncrypted" runat="server" ClientIDMode="Static"/>
           
            <telerik:RadTabStrip runat="server" ID="coverPageRadTabStrip" Orientation="HorizontalTop" OnClientTabSelected="viewPDF"
                SelectedIndex="0" MultiPageID="RadMultiPageCoverPage" Skin="Thinkgate_Blue" EnableEmbeddedSkins="false" Align="Left">
                <Tabs>
                <telerik:RadTab Text="Edit" >
                </telerik:RadTab>
                <telerik:RadTab Text="Preview" >
                </telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="RadMultiPageCoverPage" SelectedIndex="0" CssClass="multiPage">
                <telerik:RadPageView runat="server" ID="radPageViewEdit">
                    <div id="coverSheetCkEditorHolder">
                        <div id="coverSheetCkEditor" runat="server" ClientIDMode="Static">
                            <asp:Label runat="server" ID="lblCoverSheetCkeditor"></asp:Label>
                        </div>
					</div>
                    <div style="display:none;">
                        <asp:Button runat="server" ID="updateButton" ClientIDMode="Static" Text="Update" OnClick="UpdateCoverPage" />
                        <asp:Button ID="radTabSelectionTrigger" ClientIDMode="Static" OnClick="PDFView" runat="server" />
                        <asp:HiddenField runat="server" ID="hdnEditorContent"></asp:HiddenField>
                    </div>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="radPageViewPDF">
                    <div>
                        <iframe ClientIDMode="Static" runat="server" id="coverPagePDFFrame" style="width:965px; height:514px;"></iframe>
                    </div>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ID="updPanelLoadingPanel" runat="server" />
    </form>
    <script type="text/javascript">
        var modalWin = parent.$find('RadWindow1Url');
        prepModalWindow();
        
        function prepModalWindow() {
            if (modalWin && modalWin.isVisible()) {
                modalWin.add_beforeClose(coverPageCallbackFunction);
            }
        }
        
        function coverPageCallbackFunction(sender, args)
        {
            args.set_cancel(true);
            modalWin.remove_beforeClose(coverPageCallbackFunction);
            document.getElementById('updateButton').click();
        }

        function viewPDF(sender, args) {
            var editorContent;

            if (CKEDITOR.instances.coverSheetCkEditor) {
                editorContent = CKEDITOR.instances.coverSheetCkEditor.getData();
                $("#hdnEditorContent").val(escape(editorContent));
            }

            if (sender._selectedIndex == 1) {
                document.getElementById('radTabSelectionTrigger').click();
            }
            else {
                CKEDITOR.replace("coverSheetCkEditor", { toolbar: 'assessmentCoverPageEditor', height: "500px" });
                CKEDITOR.instances.coverSheetCkEditor.setData(unescape(editorContent));
                $('#coverPagePDFFrame').attr('src','');
            }
            
            return false;
        }
    </script>
</body>
</html>
