<%@ Page Title="" Language="C#" MasterPageFile="~/Dialogues/Assessment/AssessmentDialog.Master" AutoEventWireup="true" CodeBehind="SetPerformanceLevelCut.aspx.cs" Inherits="Thinkgate.Controls.Assessment.SetPerformanceLevelCut" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <script type="text/javascript" src="../../Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js"></script>

    <link href="../../Scripts/jquery-ui/jquery.ui.all.css" rel="stylesheet" />

    <style type="text/css">
        body {
            font-family: Sans-Serif, Arial;
        }
        
        .containerDiv {
            width: 600px;
            height: 450px;
            margin: 10px;
        }
        
        .labels {
            font-size: 11pt;
            width: 120px;
            text-align: left;
            margin-right: 10px;
            position: relative;
            float: left;
        }
        
        .formElement {
            position: relative;
            float: left;
            margin-bottom: 30px !important;
            top: 0px;
            left: 0px;
        }
        
        input.formElement {
            width: 200px;
            padding: 3px;
            border: solid 1px #000;
        }
        
        .formContainer {
            width: 380px;
            text-align: center;
            margin-top: 60px;
        }
        
        .headerImg {
            position: absolute;
            left: 0;
            top: 0;
        }
        
        .roundButtons {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-radius: 50px;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }
        
        .contextMenuDiv {
            height: 490px;
            position: absolute;
            top: 0px;
            left: 0px;
        }

     
        .RadComboBox_Web20 .rcbInputCellLeft, .RadComboBox_Web20 .rcbInputCellRight, .RadComboBox_Web20 .rcbArrowCellLeft, .RadComboBox_Web20 .rcbArrowCellRight {
            /*background-image: url('../../Images/rcbSprite.png') !important; */           
        }
        
        .RadComboBox_Web20 .rcbHovered .rcbInputCell .rcbInput, .RadComboBox_Web20 .rcbFocused .rcbInputCell .rcbInput {
            color: #fff !important;            
        }
        
        .RadComboBox_Web20 .rcbMoreResults a {
            /*background-image: url('../../Images/rcbSprite.png') !important;*/
        }
        
        .hiddenUpdateButton {
            display: none;
        }

        span .rbText {
            margin-right: 47px;
        }
        .labels img{margin-top: -5px;}

         .cellValue {
            display: table-cell;
            height: 40px;
            text-align: left;
            font-size: 10pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
            font-family: Verdana;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <div class="containerDiv" align="center">
        <div class="formContainer">
           <%-- <div class="labels">
                Grade:
            </div>
               <telerik:RadComboBox ID="gradeDropdown" Skin="Web20" ClientIDMode="Static" runat="server"
                EmptyMessage="&lt;Select One&gt;" AutoPostBack="false" Width="200" CssClass="formElement"
                OnClientSelectedIndexChanged="requestSubjectFilter" teacherID="" level="" xmlHttpPanelID="subjectXmlHttpPanel" />
      --%>

            <div class="labels"><strong>Set Score Details </strong></div>
            <div class="cellValue" ></div>
         <div class="labels">Set Name:</div>
            <input runat="server" type="text" id="setNameInput" clientidmode="Static" name="setNameInput"
                class="formElement" />

         <div class="labels">Description:</div>
            <input runat="server" type="text" id="setDescriptionInput" clientidmode="Static" name="setDescriptionInput"
                class="formElement" />

           <div class="labels">Score Type:</div>
            <div class="cellValue">
            <input  type="radio" id="rdoScore" value="S" clientidmode="Static"  name="score" runat="server"  />Score
            <input  type="radio" id="rdoPerformance" clientidmode="Static" value="P" checked="true" name="score" runat="server"  />Performance
                </div>

              <div class="labels">Min Score:</div>
            <input type="number" min="0" step="1"  runat="server" clientidmode="Static" id="txtMinScore" name="txtMinScore" class="formElement" />

              <div class="labels">Max Score:</div>
            <input type="number" min="0" step="1" runat="server" id="txtMaxScore" clientidmode="Static" name="txtMaxScore" class="formElement"/>


               <div class="labels">Set Score Calc:</div>
            <input type="text" runat="server" id="txtSetScoreCalc" clientidmode="Static" name="txtSetScoreCalc" class="formElement"/>


            

            <div class="labels">Level Text:</div>
            <input type="text" runat="server" id="txtLevelText" clientidmode="Static" name="txtLevelText" class="formElement"/>


            <div class="labels">Level Description:</div>
            <input type="text" runat="server" id="txtLevelDesc" clientidmode="Static" name="txtLevelDesc" class="formElement"/>


            
            <div class="labels">Level Abbreviation:</div>
            <input type="text" runat="server" id="txtLevelAbbr" clientidmode="Static" name="txtLevelAbbr" class="formElement"/>

             <div class="labels">Level Index:</div>
            <input type="number" min="0" step="1" runat="server" clientidmode="Static" id="txtLevelIndex" name="txtLevelIndex" class="formElement"/>




              <div class="labels">Level Min Score:</div>
            <input type="number" min="0" step="1" runat="server" clientidmode="Static" id="txtLevelMinScore" name="txtLevelMinScore" class="formElement" />

              <div class="labels">Level Max Score:</div>
            <input type="number" min="0" step="1" runat="server" clientidmode="Static" id="txtLevelMaxScore" name="txtLevelMaxScore" class="formElement"/>

            <div class="labels">Level Color:</div>
            <input type="color"  runat="server" id="txtLevelColor" clientidmode="Static" name="txtLevelColor" class="formElement"/>


              <div class="labels">Level Equivalent:</div>
            <input type="number" min="0" step="1" runat="server" clientidmode="Static" id="txtLevelEquivalent" name="txtLevelEquivalent" class="formElement"/>


             <div class="labels">Level Flag:</div>
            <input type="text" runat="server" id="txtLevelFlag" clientidmode="Static" name="txtLevelFlag" class="formElement"/>

            
            <input type="hidden" runat="server" id="hdnID" clientidmode="Static" name="hdnID" />
            
             <asp:Button runat="server" ID="btnSave" ClientIDMode="Static" CssClass="roundButtons" CountAddendums="0"
                Text="&nbsp;&nbsp;Save&nbsp;&nbsp;" OnClick="btnSave_Click" />

             <asp:Button runat="server" ID="btnCancel" ClientIDMode="Static" CssClass="roundButtons" CountAddendums="0"
                Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" />




          </div>
        </div>


    <script type="text/javascript">
        $(document).ready(function () {


            var Id = 0;

            Id = GetParameterValues('ID');

            $.ajax({
                type: "POST",
                url: "SetPerformanceLevelList.aspx/GetPerofmranceLevel",
                data: "{'Id':'" + Id +"'}",

                contentType: "application/json; charset=utf-8",
                dataType: "json",

                error: function (xhr, textStatus, err) {
                    console.log("responseText: " + xhr.responseText);
                },
                success: function (result) {
                    var data = [];
                    if (result && result.d) {
                        data = JSON.parse(result.d);
                        populatePerformanceLevel(data);
                    }
                   
                }
            });


        });

        function populatePerformanceLevel(data) {
            var row = data[0];
            $("#hdnID").val(row.ID);
            $("#setNameInput").val(row.SetName);
            $("#setDescriptionInput").val(row.SetDescription);
            $("#txtMinScore").val(row.SetMinScore);
            $("#txtMaxScore").val(row.SetMaxScore);
            $("#txtSetScoreCalc").val(row.SetScoreCalc);
            $("#txtLevelText").val(row.LevelText);
            $("#txtLevelDesc").val(row.LevelDescription);
            $("#txtLevelAbbr").val(row.LevelAbbr);
            $("#txtLevelIndex").val(row.LevelIndex);
            $("#txtLevelMinScore").val(row.LevelMinScore);
            $("#txtLevelMaxScore").val(row.LevelMaxScore);
            $("#txtLevelColor").val(row.LevelColor);
            $("#txtLevelEquivalent").val(row.LevelEquivalent);
            $("#txtLevelFlag").val(row.LevelFlag);
            
        }


        function GetParameterValues(param) {
            var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < url.length; i++) {
                var urlparam = url[i].split('=');
                if (urlparam[0] == param) {
                    return urlparam[1];
                }
            }
        }

    </script>

</asp:Content>
