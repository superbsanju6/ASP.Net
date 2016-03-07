<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="TGLogin.aspx.cs" Inherits="Thinkgate.TGLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <link href="~/Styles/Login.css" rel="stylesheet" type="text/css" />
    <link href="Scripts/Jquery/themes/smoothness/jquery-ui.min.css" rel="stylesheet" />
    <style>
        .myLogin {
            margin-left: auto;
            margin-right: auto;
            width: 200px;
        }
    </style>
</head>
<body runat="server" id="bodyTag">
    <img id="bgimg" runat="server" clientidmode="Static" src="Images/bgs/1024768.png" style="visibility: hidden;" alt="" />
    <div id="realBody">
        <form id="loginForm" runat="server">
            <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
                <Scripts>
                    <%--Needed for JavaScript IntelliSense in VS2010--%>
                    <%--For VS2008 replace RadScriptManager with ScriptManager--%>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                    <asp:ScriptReference Path="~/Scripts/fullscreenBackground.js" />
                    <asp:ScriptReference Path="~/Scripts/jquery-1.9.1.min.js" />
                    <asp:ScriptReference Path="~/Scripts/jquery.cookie.js" />
                    <asp:ScriptReference Path="~/Scripts/DataTables/js/jquery.dataTables.js" />
                    <asp:ScriptReference Path="~/Scripts/bootstrap/js/bootstrap.min.js" />
                    <asp:ScriptReference Path="~/Scripts/Custom/addNewDocument.js" />
                    <asp:ScriptReference Path="~/Scripts/Custom/tgDivTools.js" />
                    <asp:ScriptReference Path="~/Scripts/master.js" />
                    <asp:ScriptReference Path="~/Scripts/jquery-migrate-1.1.0.min.js" />
                    <asp:ScriptReference Path="~/Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.js" />

                    <asp:ScriptReference Path="Scripts/Jquery/ui/jquery-ui.min.js" />
                    <asp:ScriptReference Path="~/Scripts/cookieContols.js" />
                </Scripts>
            </telerik:RadScriptManager>
            <script type="text/javascript">
                $(function () {
                    populateFieldValuesFromCookies();
                    resizeBackground();
                    $('#loginForm').attr('autocomplete', 'off');
                    //$("#myLogin_LoginButton").on("click", function () {
                    //    $("#hdnClientId").val($("#myLogin_txtClientID").val());
                    //})

                });

                // Handles the window resizing so the login information will always be in the center.
                $(window).resize(function () {
                    resizeBackground();
                });

                function clamp(value, min, max) {
                    return Math.min(Math.max(value, min), max);
                };

                function resizeBackground() {
                    fullscreenBackground('bgimg');

                    var bodyDiv = $('#mainBody');
                    var documentHeight = $(document).height();

                    var centerY = (documentHeight / 2) - (bodyDiv.height() / 2);

                    if (centerY <= 0) {
                        centerY = 0;
                    }

                    bodyDiv.css('margin-top', '40' + "px");
                }

                function DoLogin(message) {
                    if (window != window.top) {
                        if (message.length > 3) {
                            //have error
                            //alert('Error');
                            if (message == 'ResetPassword') {
                                parent.startscreen(message);
                            } else {
                                parent.setError(message);
                            }

                        }
                        else {
                            //no errors
                            //alert('No Error');
                            parent.startscreen();
                        }

                    }
                }
                function popitup(url) {

                    var $dialogreset = $('<div></div>')
                    .html('<iframe style="border: 0px; " src="' + url + '" width="100%" height="100%"></iframe>')
                    .dialog({
                        autoOpen: false,
                        modal: true,
                        resizable: false,
                        height: 400,
                        width: 700,
                        title: "Thinkgate Password Reset Request"
                    });
                    //$("#dvReset").dialog('open');
                    $dialogreset.dialog('open');
                    return false;

                }
                function closePopitupDialog() {
                    parent.$('.ui-icon-closethick').click();
                }

                function setRememberMeCookie(isCheckboxChecked) {
                    var expdate = new Date();
                    if (isCheckboxChecked) {
                        //set cookie here
                        expdate.setTime(expdate.getTime() + 20 * 24 * 60 * 60 * 1000);
                        setCookie('signinuid', $("#myLogin_UserName").val(), expdate);
                        $('#myLogin_Password').attr("autocomplete", "off");
                        setCookie('signinpass', $("#myLogin_Password").val(), expdate);
                        //setCookie('signincid', $("#myLogin_txtClientID").val(), expdate);
                    } else {
                        deleteCookie('signinuid');
                        deleteCookie('signinpass');
                        // deleteCookie('signincid');
                    }
                }
                function populateFieldValuesFromCookies() {
                    var username = getCookie('signinuid');
                    var password = getCookie('');
                    //var clientId = getCookie('signincid');

                    if (username && username != "") {
                        $("#myLogin_UserName").val(username);
                    }
                    if (password && password != "") {
                        $("#myLogin_Password").val(password);
                    }
                    //if (clientId && clientId != "") {
                    //    $("#myLogin_txtClientID").val(clientId);
                    //}

                    //if ((clientId && clientId != "") || (username && username != "") || (password && password != "")) {
                    if ((username && username != "") || (password && password != "")) {
                        $("#rememberMeCheckbox").prop('checked', true);
                    }
                }

                var specialKeys = new Array();               
                specialKeys.push(8);
                specialKeys.push(9);
                specialKeys.push(13);
                specialKeys.push(37);
                specialKeys.push(32);
                specialKeys.push(222);
                function IsAlphaNumeric(e) {
                    var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
                    var ret = ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (specialKeys.indexOf(e.keyCode) != -1 || keyCode == 32 || keyCode == 39 && e.charCode != e.keyCode));
                    if (e.char == '%' || e.char == '$' || e.char == '#' || (e.shiftKey == true && e.keyCode == 37)) {
                        ret = false;
                    }
                    else if (($.browser.mozilla == true && keyCode == 46) || ($.browser.chrome == true && keyCode == 46) || ($.browser.msie == true && e.keyCode == 46) || ($.browser.safari == true && keyCode == 46)) {
                        ret = true;
                    }
                    else if (($.browser.msie == true && e.keyCode == 39) || ($.browser.chrome == true && e.keyCode == 39)) {
                        ret = true;
                    }
                    return ret;
                }
                function IsAlphaNumeric_CheckID(e) {
                    var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
                    var ret = ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (specialKeys.indexOf(e.keyCode) != -1 || keyCode == 32 && e.charCode != e.keyCode));
                    if (e.char == '%' || e.char == '$' || e.char == '#' || e.char == "'" || (e.shiftKey == true && e.keyCode == 37)) {
                        ret = false;
                    }
                    else if (($.browser.mozilla == true && keyCode == 46) || ($.browser.chrome == true && keyCode == 46) || ($.browser.msie == true && e.keyCode == 46) || ($.browser.safari == true && keyCode == 46)) {
                        ret = true;
                    }
                    return ret;
                }

                function CheckAllowedCharacters(e) {
                    var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
                    var keyPressedValue = String.fromCharCode(keyCode);
                    var allowedCharacters = new RegExp("[\w_@.'-]", "g");
                    return allowedCharacters.test(keyPressedValue);
                }

                $(document).ready(function () {
                    $('#myLogin_UserName').bind('keypress', function (e) {
                        var chk = IsAlphaNumeric(e);
                        var isAllowedChararacter = CheckAllowedCharacters(e);
                        if (chk == false && isAllowedChararacter == false) {
                            e.preventDefault();
                        }
                    });
                    $('#myLogin_UserName').bind('paste', function () {
                        setTimeout(function () {
                            var data = $('#myLogin_UserName').val();
                            var dataFull = data.replace(/[^\w\s\'\@\_\.]/gi, '');
                            $('#myLogin_UserName').val(dataFull);
                        });
                    });
                });

            </script>
            <div id="mainBody">
                <%--  <asp:HiddenField ID="hdnClientId" runat="server" Value="" />--%>
                <%
                    string logoUrl = "./images/thinkgateLogo.png";

                    //if an ./images/lealogo.png file exitst we will use that for the logo - if the file doesn't exist or fails to load show the default Thinkgate logo
                    if (System.IO.File.Exists(Server.MapPath("./images/lealogo.png")))
                    {
                        logoUrl = "./images/lealogo.png";
                    }

                    if (System.IO.File.Exists(Server.MapPath("./images/ClientLogos/" + DParms.ClientID + "logo.png")))
                    {
                        logoUrl = "./images/ClientLogos/" + DParms.ClientID + "logo.png";
                    }

                %>

                <div style="padding-bottom: 1em; text-align: center">
                    <%
                        Response.Write("<img src='");
                        Response.Write(logoUrl);
                        Response.Write("' onerror='this.src = \"./images/thinkgateLogo.png\"'>");
                    %>
                </div>

                <asp:Login ID="myLogin" runat="server" ForeColor="White" RememberMeSet="False" TitleText="" UserNameLabelText="Username:" OnAuthenticate="myLogin_Authenticate" OnLoginError="myLogin_LoginError" CssClass="myLogin">
                    <LayoutTemplate>

                        <div id="content">
                            <section class="login-modal" style="opacity: 1; height: 180px!important;">
                                                       <div class="field email-field">
                                         <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" CssClass="text">Username:</asp:Label>
                                    <asp:TextBox ID="UserName" runat="server" CssClass="text" MaxLength="256"></asp:TextBox>
                                                           <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                                                  ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="myLogin"
                                                                  ForeColor="red">* Username is required</asp:RequiredFieldValidator>                                             
                                                       </div>
                                                       <div class="field password-field">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="text" >Password:</asp:Label>
                                    <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="text" autocomplete="off"></asp:TextBox>
                                                           <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                                                  ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="myLogin"
                                                                  ForeColor="Red">* Password is required</asp:RequiredFieldValidator>
                                                       </div
                              
                                                   <%--    <div class="field email-field">
                                                              <asp:Label CssClass="text" ID="ClientIDLabel" runat="server" AssociatedControlID="txtClientID">Client ID:</asp:Label>
                                                              <asp:TextBox ID="txtClientID" runat="server" CssClass="text"></asp:TextBox>
                                                              <asp:RequiredFieldValidator ID="ClientIDRequired" runat="server" ControlToValidate="txtClientID"
                                                                     ErrorMessage="Client ID is required." ToolTip="Client ID is required." ValidationGroup="myLogin"
                                                                     ForeColor="red">* Client ID is required</asp:RequiredFieldValidator>                                                           
                                                       </div>--%>
                               
                                                       <div class="footer">
                                    <asp:Button ID="LoginButton" runat="server" class="button primary-button" style="width:100%;" CommandName="Login" Text="Log In" ValidationGroup="myLogin" />
                                  </div>
                                                </section>

                            <div class="footerDiv">
                                <div class="alignleft" style="color: #747474;">
                                    <input type="checkbox" id="rememberMeCheckbox" onclick="setRememberMeCookie(this.checked)">
                                    Remember Me
                                </div>

                                <div class=" alignright" style="color: #747474;" id="resetPasswordDiv"
                                    onmouseover="this.style.cursor='pointer';this.style.textDecoration='underline';"
                                    onmouseout="this.style.textDecoration='none';"
                                    onclick="return popitup('Account/Reset.aspx')">
                                    Forgot Password
                                </div>
                            </div>
                            <div id="LoginFailureDiv" class="errorText" style="color: red; width: 420px;">
                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False" ClientIDMode="Static"></asp:Literal>
                            </div>
                            <%-- <div class="errorSection">
                                <asp:Literal ID="Literal1" runat="server" EnableViewState="False" ClientIDMode="Static"></asp:Literal>
                                <asp:Label ID="ErrorLocation" CssClass="errorText" runat="server" Text="" Visible="False"></asp:Label>
                            </div>--%>
                        </div>

                    </LayoutTemplate>
                </asp:Login>

                <div id="ssoErrorMessageDiv" runat="server" style="width: 100%; text-align: center;" Visible="false">
                    <div id="ssoErrorTextDiv" class="errorText">
                        Error encountered accessing this application using the current Single Sign On credentials.
                        <asp:Label ID="lblSsoErrorMessage" runat="server"></asp:Label>
                    </div>
                    <div id="ssoErrorInfoDiv" style="color: white!important;">
                        <br> If access to this application should be allowed using these SSO credentials, please contact the support team.
                        <br> If you wish to access this application using non-SSO credentials, you may enter them here.
                    </div>
                </div>

                <div id="messageDiv" style="width: 100%; text-align: center;">
                    <asp:Label ID="lblMessage" runat="server" Font-Size="20" Visible="false"></asp:Label>
                </div>
                <div class="versionDiv" style="text-align: right; padding-right: 1em; color: white!important;">
                    <asp:Label ID="lblVersion" runat="server"></asp:Label>
                </div>
                <div class="versionDiv" style="text-align: right; padding-right: 1em; color: white!important;">
                    <asp:Label ID="lblModifiedDate" runat="server"></asp:Label>
                </div>
                <div class="versionDiv" style="text-align: right; padding-right: 1em;">
                    <asp:Label ID="lblServerName" runat="server"></asp:Label>
                </div>
            </div>
        </form>
    </div>
    <link href="<%=ResolveUrl(ClientSiteCss)%>" rel="stylesheet" type="text/css" />
    <link href="<%=ResolveUrl(ClientLoginCss)%>" rel="stylesheet" type="text/css" />
</body>
</html>
