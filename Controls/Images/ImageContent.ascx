<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ImageContent.ascx.cs"
    Inherits="Thinkgate.Controls.Images.ImageContent" %>
<div class="identificationTile-DivAroundTable">
    <script type="text/javascript">
        function hlImagePreviewClick() {
            var hdnImageName = $('#ImageContentTile_HdnImageName')[0].value;
            var hdnImageContent = $('#ImageContentTile_HdnImageContent')[0].value;
            customDialog(
                        { title: 'Image Preview',
                            content: '<table><tr><td><div class="contentTile-PreviewLabel">' + hdnImageName +
                                     '</div></td></tr><tr><td><div class="contentTile-PreviewContent"><img src="' + hdnImageContent + '" alt="preview image" />' + 
                                     '</div></td></tr><tr><td style="text-align:right"><a onclick="RenderPDF()" href="#" ><img src="<% =ResolveUrl("../../Images/mag_glass.png") %>" height="20px" />Print</a></td></tr></table>'
                        }
                    );
        }

        function RenderPDF() {
            var hdnPrintPreview = $('#ImageContentTile_hdnPrintPreviewUrl')[0].value;
            customDialog(
                           { title: 'Print Preview',
                           maximize: true, maxwidth: 600, maxheight: 900,
                               content: '<iframe height="800px"; width="550px"; style="overflow:none; " src="' + hdnPrintPreview + '"></iframe>'
                           }
                        );
        }
    </script>
    <table width="100%" class="fieldValueTable">
        <tr>
            <td height="205" style="text-align: center; vertical-align: middle;">
                <img style="max-height: 200px;max-width: 300px" id="imgThumbNail" class="thumbImage" runat="server" />
            </td>
             </tr>
    </table>
              <div style="float:right;padding-right:10px;">
                    <a id="hlImagePreview" href="" runat="server" class="previewLink" >
                        <img id="ImgPreview" runat="server" src='../../Images/mag_glass.png' />                      
                        Preview </a>
               
               </div>
<input runat="server" type="hidden" id="ImageContentTile_HdnImageName" ClientIDMode="Static"/>
<input runat="server" type="hidden" id="ImageContentTile_HdnImageContent" ClientIDMode="Static"/>
<input runat="server" type="hidden" id="ImageContentTile_hdnPrintPreviewUrl" ClientIDMode="Static"/>

