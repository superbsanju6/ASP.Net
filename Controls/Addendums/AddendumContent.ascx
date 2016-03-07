<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="AddendumContent.ascx.cs"
    Inherits="Thinkgate.Controls.Addendums.AddendumContent" %>
<div style="height: 100%">
    <script type="text/javascript">
        function hlAddendumContentClick() {
            var hdnAddendumName = $('#AddendumContentTile_HdnAddendumName')[0].value;
            var hdnAddendumContent = $('#AddendumContentTile_HdnAddendumContent')[0].value;
            customDialog(
                        { title: 'Addendum Preview',
                            content: '<table><tr><td><div class="contentTile-PreviewLabel">' + hdnAddendumName + '</div></td></tr><tr><td><div class="contentTile-PreviewContent">' + hdnAddendumContent + '</div></td></tr><tr><td style="text-align:right"><a onclick="RenderPDF()" href="#" ><img src="<% =ResolveUrl("../../Images/mag_glass.png") %>" height="20px" />Print</a></td></tr></table>'
                        }
                    );
         }

        function RenderPDF() {
            var hdnPrintPreview = $('#AddendumContentTile_hdnPrintPreviewUrl')[0].value;
            customDialog(
                           {title: 'Print Preview',
                            maxwidth: 600,
                            maxheight: 900,
                            maximize: true,
                            content: '<iframe height="800px"; width="550px"; style="overflow:none; " src="' + hdnPrintPreview + '"></iframe>'}
                        );
        }
    </script>
    <table width="100%" height="100%" class="fieldValueTable">
        <tr>
            <td style="text-align: center; vert-align:middle; font-size:large;color:blue;text-decoration: underline;">
                <a id="hlAddendumContent" href="" runat="server">Click to view Addendum</a>
            </td>
        </tr>   
    </table>
</div>
<input runat="server" type="hidden" id="AddendumContentTile_HdnAddendumName" ClientIDMode="Static"/>
<input runat="server" type="hidden" id="AddendumContentTile_HdnAddendumContent" ClientIDMode="Static"/>
<input runat="server" type="hidden" id="AddendumContentTile_hdnPrintPreviewUrl" ClientIDMode="Static"/>

