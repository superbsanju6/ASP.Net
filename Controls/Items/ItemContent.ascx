<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ItemContent.ascx.cs"
    Inherits="Thinkgate.Controls.Items.ItemContent" %>
<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">
    // TODO: change this to use ItemThumbnailFolder
    var bad_thumb_path = '../Images/thumb_none.png';

    function setDefaultImage(source) {
        var badImg = new Image();
        badImg.src = bad_thumb_path;
        var cpyImg = new Image();
        cpyImg.src = source.src;

        if (!cpyImg.width) {
            source.src = badImg.src;
        }
    }

    function onImgError(source) {
        source.src = bad_thumb_path;
        source.onerror = "";
        return true;
    }

    $(document).ready(function() {

    });

    function onLinePreviewClick() {

       
        customDialog(
            { name: 'OnlinePreview',
            maximize:true,
                title: 'Online Preview',
            resizable: true,
            movable: true,
            url: document.getElementById('ItemContentTile_lbl_OTCUrl').value + "?assessmentID=0&studentID=0&itemID=" + document.getElementById('ItemContentTile_itemID').value + "&itemType=" + document.getElementById('ItemContentTile_itemType').value,
                dialog_style: 'alert'
            }
        );

      
    }

    function displayAddendumClick() {
//        var hdnAddendumInfo = $('#ItemContentTile_hdnAddendumInfo')[0];

        var contentHTML = $('#ItemContentTile_tblDisplayAddendum')[0];        
//        var contentHTML = '<table><tr><td><div class="contentTile-PreviewLabel">' + hdnAddendumInfo.name +
//                          '</div></td></tr><tr><td><div class="contentTile-PreviewContent">' + hdnAddendumInfo.value +
//                          '</div></td></tr></table>';

        customDialog(
                        {title: 'Addendum Preview', 
                         content: contentHTML.outerHTML,
                         autosize: true}
        );
    }

    function printPreviewClick() {
        var hdnPrintPreview = $('#ItemContentTile_hdnPrintPreviewInfo')[0];
        customDialog(
                        {title: 'Print Preview',
                        maximize: true, maxwidth: 600, maxheight: 900,
                        content: '<iframe height="800px"; width="550px"; style="overflow:none; " src="' + hdnPrintPreview.value + '"></iframe>'
                    }
        );
    }
    function execInitialJavaScript() {
        setDefaultImage($('#ItemContentTile_imgThumbnail')[0]);
    }

</script>
<div>
    <table width="100%">
        <tr>
            <td style="height: 170px; width: 100%; text-align: center !important; vertical-align: middle;">
                <div class="thumbInstance" id="div1" >
                    <img runat="server" id="ItemContentTile_imgThumbnail" style="max-height: 170px; max-width: 200px;" src="" alt="Item Thumbnail" clientidmode="Static" />
                </div>
            </td>
        </tr>
        <tr>
            <td style="height: 30px; width: 100%; text-align: left; vertical-align: top;">
                <div runat="server" id="divDisplayAddendum" style="visibility: hidden;">
                    <a id="hlDisplayAddendum" href="javascript:void(0);" onclick="" onerror="onImgError(this);"
                        runat="server"></a>
                </div>
            </td>
        </tr>
    </table>
    <table  width="100%" >
        <tr>
            <td height="30px"style="text-align: right; vertical-align: bottom;">
                <a id="hlPreviewOnline" class="previewLink" href="javascript:void(0);" onclick="onLinePreviewClick();" onerror="onImgError(this);" runat="server">
                    <img runat="server" id="imgPreviewIcon" src='' />
                    Online Preview
                </a> 
                &nbsp;&nbsp;&nbsp;&nbsp; 
                <a id="hlPrintPreview" class="previewLink" href="javascript:void(0);" onclick="printPreviewClick()" onerror="onImgError(this);" runat="server">
                    <img runat="server" id="imgPrintPreviewIcon" src='' />
                    Print Preview
                </a>
            </td>
        </tr>
    </table>
</div>
<div style="display:none">
    <table id="ItemContentTile_tblDisplayAddendum" clientidmode="Static">
        <tr style="display:none;">
            <td>
                <div class="contentTile-PreviewLabel">
                    <a runat="server" id="hlLaunchAddendumPage" href="javascript:void(0);" onclick=""></a>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div runat="server" id="divAddendumContent" class="contentTile-PreviewContent">
                </div>
            </td>
        </tr>
    </table>
</div>
<input runat="server" type="hidden" id="ItemContentTile_hdnThumbnailURL" ClientIDMode="Static"/>
<input runat="server" type="hidden" id="ItemContentTile_hdnAddendumInfo" ClientIDMode="Static"/>
<input runat="server" type="hidden" id="ItemContentTile_hdnPrintPreviewInfo" ClientIDMode="Static"/>
<input id="ItemContentTile_lbl_OTCUrl" type="hidden" runat="server" clientidmode="Static" />  
<input id="ItemContentTile_itemID" type="hidden" runat="server" clientidmode="Static" />  
<input id="ItemContentTile_itemType" type="hidden" runat="server" clientidmode="Static" />  

