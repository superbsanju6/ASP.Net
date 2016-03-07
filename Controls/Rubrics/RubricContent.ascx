<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="RubricContent.ascx.cs"
    Inherits="Thinkgate.Controls.Rubrics.RubricContent" %>
<div style="height: 100%">
     <script type="text/javascript">
         function hlRubricContentClick() {
             var hdnRubricName = $('#RubricContentTile_HdnRubricName')[0].value;
             var hdnRubricContent = $('#RubricContentTile_HdnRubricContent')[0].value;
             customDialog(
                        { title: 'Rubric Preview',
                            content: '<table><tr><td><div class="contentTile-PreviewLabel">' + hdnRubricName + '</div></td></tr><tr><td><div class="contentTile-PreviewContent">' + hdnRubricContent + '</div></td></tr><tr><td style="text-align:right"><a onclick="RenderPDF()" href="#" ><img src="<% =ResolveUrl("../../Images/mag_glass.png") %>" height="20px" />Print</a></td></tr></table>'
                        }
                    );
         }

         function RenderPDF() {
             var hdnPrintPreview = $('#RubricContentTile_hdnPrintPreviewUrl')[0].value;
             customDialog(
                           { title: 'Print Preview',
                           maximize: true, maxwidth: 600, maxheight: 900,
                               content: '<iframe height="800px"; width="550px"; style="overflow:none; " src="' + hdnPrintPreview + '"></iframe>'
                           }
                        );
         }
    </script>
   <table width="100%" height="100%" class="fieldValueTable">
        <tr>
            <td style="text-align: center; vert-align:middle; font-size:large;color:blue;text-decoration: underline;">
                <a id="hlRubricContent" href="" onclick="" runat="server">Click to view Rubric</a>
            </td>
        </tr>
    </table>
</div>
<input runat="server" type="hidden" id="RubricContentTile_HdnRubricName" ClientIDMode="Static"/>
<input runat="server" type="hidden" id="RubricContentTile_HdnRubricContent" ClientIDMode="Static"/>
<input runat="server" type="hidden" id="RubricContentTile_hdnPrintPreviewUrl" ClientIDMode="Static"/>