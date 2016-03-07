<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AddNew.Master" CodeBehind="CredentialAlignments.aspx.cs" Inherits="Thinkgate.Controls.Credentials.CredentialAlignments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <telerik:RadCodeBlock ID="jsRadCodeBlock1" runat="server">
        <script type="text/javascript" src="../../Scripts/jquery-1.9.1.js"></script>
        <script type="text/javascript" src="../../Scripts/bootstrap/js/bootstrap.js"></script>
        <script type="text/javascript">

            $(document).ready(function () {

                $("div.container-alignment").find("input[type=checkbox]").each(function (index, checkbox) {
                    $(checkbox).click(function (event) {
                        event.stopPropagation();
                    });
                });

            });
            
            function toggleState(rootPanel) {
                var isCollapsed = $(rootPanel).hasClass("collapsed");
                if (isCollapsed === true) {
                    var target = $(rootPanel).find("span.icon-chevron-right");
                    target.removeClass("expandIcon");
                    target.removeClass("icon-chevron-right");
                    target.addClass("collapseIcon");
                    target.addClass("icon-chevron-down");
                    $(rootPanel).find("span.rootLabel").css({ "color": "white" });
                }
                else {
                    var target = $(rootPanel).find("span.icon-chevron-down");
                    target.removeClass("collapseIcon");
                    target.removeClass("icon-chevron-down");
                    target.addClass("expandIcon");
                    target.addClass("icon-chevron-right");
                    $(rootPanel).find("span.rootLabel").css({ "color": "#333333" });
                }
            }

            function updateAndClose() {

                var addCredentialFrameWnd = null;
                var selectedAlignments = getSelectedAlignments();
                var topWnd = top;
                
                if (parent.location.pathname.indexOf("District.aspx") > -1)
                {
                    topWnd = top;
                }
                else if (parent.location.pathname.indexOf("EditCredentials.aspx") > -1)
                {
                    topWnd = top.frames[0];
                }

                for (var i = 0; i < topWnd.frames.length; i++)
                {
                    if (topWnd.frames[i].location.pathname.indexOf("AddCredential.aspx") > -1)
                    {
                        addCredentialFrameWnd = topWnd.frames[i];
                        addCredentialFrameWnd.setAlignmentsModified();
                        break;
                    }
                }
                $('[id$="hdnAlignments"]', addCredentialFrameWnd.document).val(selectedAlignments);
                closeWindow();
            }

            function closeWindow() {
                var oWnd = getCurrentCustomDialog();
                setTimeout(function () {
                    oWnd.close();
                }, 100)
            }

            function getSelectedAlignments()
            {
                var selectedAlignments = "";
                $("div.container-alignment").find("span[nodeKind=saveTarget]").each(function (i, span) {
                    $(span).children("input[type=checkbox]").each(function (index, checkbox) {
                        if (checkbox.checked === true)
                        {
                            selectedAlignments += $(span).attr("nodeValue") + ",";
                        }
                    });
                });
                return selectedAlignments;
            }
        </script>

    </telerik:RadCodeBlock>

    <link rel="stylesheet" type="text/css" href="styles.css" />
    <link href="../../Scripts/bootstrap/css/bootstrap.css" rel="stylesheet" />
    <style type="text/css">

        body
        {
            font-family: 'Trebuchet MS';
            font-size: 12px;
        }
        .container-alignment
        {
            width: 400px;
            margin-left: 10px;
            margin-top: 15px;
            border: 1px solid gray;
            color: #333333;
        }

        .divPanelRoot {
            width: 100%;
            height: 32px;
            line-height: 32px;
            cursor: pointer;
            border-bottom: 1px solid gray;
             border-right: 1px solid gray;
            background-color: gray;
        }

        .divPanelChild {
            width: 100%;
            height: 32px;
            line-height: 32px;
            cursor: pointer;
            border-bottom: 1px solid gray;
            border-right: 1px solid gray;
            background-color: white;
            color: black;
        }

        .collapsed {
            width: 100%;
            height: 32px;
            line-height: 32px;
            cursor: pointer;
            border-bottom: 1px solid gray;
            background-color: white;
        }

        span.expandIcon {
            margin-top: 10px;
            margin-right: 10px;
            height: 12px;
            width: 12px;
        }

        span.collapseIcon {
            margin-top: 10px;
            margin-right: 10px;
            height: 12px;
            width: 12px;
        }

        .rootLabel
        {
            margin-left: 5px;
        }

        .childLabel
        {
            margin-left: 5px;
        }

        .check {
            margin-left: 5px;
        }

        .childCheck {
            margin-left: 40px;
        }

        input
        {
            vertical-align: baseline;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:HiddenField ID="hdnSavedAlignments" runat="server" ClientIDMode="Static" />
    <div class="container-alignment" style="height: 410px; overflow: auto; overflow-x: hidden">
        <asp:Repeater ID="rptAlignments" runat="server" OnItemDataBound="rptAccordion_ItemDataBound">
            <ItemTemplate>
                <div style="width: 100%;">
                    <div class="divPanelRoot" id="divRootItem" runat="server" clientidmode="Static">
                        <div class="collapsed" data-toggle="collapse" data-target='#<%#Eval("ID") %>' onclick="toggleState(this);">
                            <asp:CheckBox ID="chkRoot" runat="server" class="check" nodeKind="saveTarget" nodeValue='<%#Eval("ID") %>' />
                            <span class="rootLabel" title='<%#Eval("CredentialAlignment") %>'><%#Eval("CredentialAlignment") %></span>
                            <span class="pull-right expandIcon icon-chevron-right"></span>
                        </div>
                    </div>
                    <div id='<%#Eval("ID") %>' class="collapse" nodeKind="children" style="border-right: 1px solid gray; width: 100%;">
                        <asp:Repeater ID="rptChildAlignments" runat="server">
                            <ItemTemplate>
                                <div class="divPanelChild">
                                    <asp:CheckBox ID="chkChild" runat="server" class="childCheck" nodeKind="saveTarget" nodeValue='<%#Eval("ID") %>' />
                                    <span class="childLabel" title='<%#Eval("CredentialAlignment") %>'><%#Eval("CredentialAlignment") %></span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div style="float: right; line-height: 30px; margin-top: 5px;">
        <telerik:RadButton ID="UpdateButton" runat="server" Text="Update" Skin="Web20" AutoPostBack="false" OnClientClicked="updateAndClose" UseSubmitBehavior="false"></telerik:RadButton>
        <telerik:RadButton ID="CancelButton" runat="server" Text="Cancel" Skin="Web20" AutoPostBack="false" OnClientClicked="closeWindow" UseSubmitBehavior="false"></telerik:RadButton>
    </div>
</asp:Content>
