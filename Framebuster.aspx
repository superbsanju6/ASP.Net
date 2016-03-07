<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Framebuster.aspx.cs" Inherits="Thinkgate.Framebuster" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        //This page MUST be passed, at a minimum, a querystring with the first key named "PageName".
        //Any additional keys will be parsed and passed on to that page.
        var qString = this.location.href.substring(this.location.href.indexOf('?') + 1);
        var array1 = qString.split('&');
        var qStringNew = '?killSession=false&';
        for (var i = 0; i < array1.length; i++) {
            var tmpArray = array1[i].split('=');
            if (tmpArray[0] == "PageName") {
                eval(tmpArray[0] + "=\"" + tmpArray[1] + "\"");
            } else {
                qStringNew += array1[i];
                if (i < array1.length - 1) {
                    qStringNew += "&";
                }
            }
        }

        var targetURL = top.location.href;

        if (location.href.lastIndexOf('/') != -1) {
            lastpos = targetURL.lastIndexOf('/');
            targetURL = targetURL.substring(0, lastpos);
        }
        
        targetURL = targetURL.replace("/TMNET", "");

        //top.location.href = targetURL + '/' + PageName + qStringNew;
        top.location.href = "http://tgdev/TM2011/display.asp";
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    </div>
    </form>
</body>
</html>
