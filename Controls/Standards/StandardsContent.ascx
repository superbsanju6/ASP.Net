<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StandardsContent.ascx.cs" Inherits="Thinkgate.Controls.Standards.StandardsContent" %>
<link href="../../Styles/Site.css" rel="stylesheet" type="text/css" />
<style type="text/css">
    .stdTitleForPage
    {
        font-size: larger;
        font-weight: bold;
    }
    
    .stdTextForPage
    {
        font-size: larger;
        font-weight: bold;
    }
    
    .stdParent
    {
        color: #454543;
    }
    
    .stdChildren
    {
        color: #454543;
    }
    
    .stdTitle
    {
        font-weight: bold;
        font-style: italic;
    }
    
    .stdText
    {
        font-style: italic;
    }
    
    .hideCtrl
    {
        display: none;
    }

    .btnColumn {
        width: 11px;
    }
    
    #tblStdContent td {
        text-align: left; 
        vertical-align: top;
        /*border: 1px solid blue;*/
    }

    #tblStdContent div {
        text-align: left; 
        vertical-align: top;
        /*border: 1px solid purple;*/
        display:table-cell;
    }

</style>
<div  class="identificationTile-DivAroundTable">
    <script type="text/javascript">
        function toggleStdDetail(sender) {
            if (sender.id == "StdsPage_StdsContent_ImgExpandStdDetail") {

                /* Reveal detail sections of our standard */
                tdStdParent.style.display = "block";
                tdStdChildren.style.display = "block";
                tdStdTitle.style.display = "block";

                /* Display our collapse image */
                imgCollapseStdDetail.style.display = "block";
                imgExpandStdDetail.style.display = "none";
            }
            else {
                /* hide detail sections of our standard */
                tdStdParent.style.display = "none";
                tdStdChildren.style.display = "none";
                tdStdTitle.style.display = "none";

                /* Display our expand image */
                imgCollapseStdDetail.style.display = "none";
                imgExpandStdDetail.style.display = "block";
            }
        }
    </script>
    <table width="100%" class="fieldValueTable" runat="server" id="tblStdContent" clientidmode="Static" >
        <tr>
            <td id="StdsPage_StdsContent_tdStdParent" class="stdParent" clientidmode="Static" >
                <div runat="server" id="StdsPage_StdsContent_DivStdParentTitle" class="stdTitle" clientidmode="Static" style="display:block;">
                </div>
                <div runat="server" id="StdsPage_StdsContent_DivStdParentText" class="stdText" clientidmode="Static" style="display:block;">
                </div>
            </td>
        </tr>
        <tr>
            <td id="StdsPage_StdsContent_tdStdTitle" clientidmode="Static" style="padding-bottom: 0px;" >
                <div style="width:20px;"></div>
                <div runat="server" id="StdsPage_StdsContent_DivStdTitle" class="stdTitleForPage" clientidmode="Static" >
                </div>
            </td>
        </tr>
        <tr>
            <td style="padding-top: 0px;">
                <div style="width:20px;">
                    <img runat="server" id="StdsPage_StdsContent_ImgExpandStdDetail" onclick="toggleStdDetail(this)" alt="expand" src="../../Images/BtnExpandPlus.png" clientidmode="Static" />
                    <img runat="server" id="StdsPage_StdsContent_ImgCollapseStdDetail" onclick="toggleStdDetail(this)" alt="collapse" src="../../Images/BtnCollapseMinus.png" clientidmode="Static" />
                </div>
                <div runat="server" id="StdsPage_StdsContent_DivStdText" class="stdTextForPage" clientidmode="Static"  style="display:table-cell">
                </div>
            </td>
        </tr>
        <tr>
            <td class="stdChildren"  id="StdsPage_StdsContent_tdStdChildren" clientidmode="Static" >
                <div style="width: 40px;" ></div>
                <div runat="server" id="StdsPage_StdsContent_divStdChildren" clientidmode="Static" >
                </div>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        /*********************************************
        Map local elements to variables for easier
        javascript coding.
        *********************************************/

        //this.divStdParentTitle = $('#StdsPage_StdsContent_DivStdParentTitle')[0];
        //this.divStdParentText = $('#StdsPage_StdsContent_DivStdParentText')[0];
        tdStdParent = $('#StdsPage_StdsContent_tdStdParent')[0];
        

        this.imgExpandStdDetail = $('#StdsPage_StdsContent_ImgExpandStdDetail')[0];
        this.imgCollapseStdDetail = $('#StdsPage_StdsContent_ImgCollapseStdDetail')[0];
        this.divStdName = $('#StdsPage_StdsContent_DivStdName')[0];

        //this.divStdTitle = $('#StdsPage_StdsContent_DivStdTitle')[0];
        this.tdStdTitle = $('#StdsPage_StdsContent_tdStdTitle')[0];

        //this.divStdChildren = $('#StdsPage_StdsContent_divStdChildren')[0];
        this.tdStdChildren = $('#StdsPage_StdsContent_tdStdChildren')[0];

        this.imgCollapseStdDetail = $('#StdsPage_StdsContent_ImgCollapseStdDetail')[0];
        imgCollapseStdDetail.click();
        
    </script>
</div>
