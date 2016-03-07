<%@ Page Title="Options to Create Assessment" Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master" CodeBehind="CreateAssessmentOptions.aspx.cs"
    Inherits="Thinkgate.Dialogues.Assessment.CreateAssessmentOptions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        body
        {
            font-family: Sans-Serif, Arial;
        }
        
        .containerDiv
        {
            width: 500px;
            margin: 10px;
        }
        
        p
        {
            font-style: italic;
            font-size: 12pt;
            font-weight: bold;
        }
        
        .menuSelections
        {
            font-weight: bold;
            font-size: 14pt;
            width: 250px;
            margin-left: 25%;
            margin-bottom: 10px;
            text-align: right;
            height: 65px;
            padding-top: 6%;
            cursor: pointer;
        }
        
        .quickImg
        {
            background: url(../../Images/lightningbolt.png) no-repeat left center;
        }
        
        .createOwnImg
        {
            background: url(../../Images/repairtool.png) no-repeat left center;
        }

        .TextLeft
        {
            margin-right: 14px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div class="containerDiv">
        <p>
            Please select an option:</p>
        <br />        
        <div class="menuSelections quickImg" onclick="makeItQuick();">
            <div class="TextLeft"> Make It Quick</div></div>
        <div class="menuSelections createOwnImg" onclick="createMyOwn();">
            Create My Own</div>
    </div>
    <script type="text/javascript">
        function openAddAssessmentWindow(url) {
            var win = parent.customDialog({
                name: 'RadWindowAddAssessment',
                url: url,
                title: 'Assessment Identification',
                maximize: true, maxwidth: 650, maxheight: 650
            });

            //win.addConfirmDialog({ title: 'Cancel Assessment', callback: parent.createAssessmentOptionsCallback }, window);
        }

        function makeItQuick() {
            openAddAssessmentWindow('../Dialogues/Assessment/CreateAssessmentIdentification.aspx?headerImg=lightningbolt&senderPage=new&showSecure=true');
        }

        function createMyOwn() {
            openAddAssessmentWindow('../Dialogues/Assessment/CreateAssessmentIdentification.aspx?headerImg=repairtool&showSecure=true&senderPage=new');
        }
    </script>
</asp:Content>
