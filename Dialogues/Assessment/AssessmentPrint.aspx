<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssessmentPrint.aspx.cs"
    Inherits="Thinkgate.Dialogues.Assessment.AssessmentPrint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="PRAGMA" content="NO-CACHE" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="CACHE-CONTROL" content="NO-STORE" />
    <link href="~/Styles/reset.css" rel="stylesheet" type="text/css" />
    <title>Print Assessment</title>
    <base target="_self" />
    <style type="text/css">
				.cellFont
				{
						font-size: 11pt;
						font-weight: bold;
				}
				
				.tbxPosn
				{
						margin-left: 30px;
				}
                .marginLeft0px
				{
						margin-left: 0px !important;
				}
				
				.cellPadding
				{
						padding-bottom: 8px;
				}
				
				.assessmentCol
				{
						width: 100px;
				}
				
				.answerKeyCol
				{
						width: 100px;
				}
				
				.rubricCol
				{
						width: 100px;
				}

                .instructionsCol
				{
						width: 200px;
				}
				
				.invisibleCol
				{
						visibility: hidden;                       
				}              
				
				.roundButtons
				{
						color: #00F;
						font-weight: bold;
						font-size: 12pt;
						padding: 2px;
						display: inline;
						position: relative;
						border: solid 1px #000;
						border-radius: 50px;
						float: right;
						margin-left: 10px;
						cursor: pointer;
						background-color: #FFF;
				}
                .grayText {
                    color: gray;
                }               

		</style>

    <link href="../../Thinkgate_Blue/TabStrip.Thinkgate_Blue.css?v=2" rel="stylesheet" type="text/css" />
    <link href="../../Styles/Site.css?v=2" rel="stylesheet" type="text/css" />
    <link href="../../Styles/Thinkgate_Window/Window.Thinkgate_Window.css?v=5" rel="stylesheet"
        type="text/css" runat="server" />
</head>
<body style="background-color: LightSteelBlue; font-family: Arial, Sans-Serif; font-size: 10pt;">
    <form runat="server" id="mainForm" method="post">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/scripts/master.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="false"
            Skin="Thinkgate_Window" EnableEmbeddedSkins="false" />
        <input id="inpAssessmentID" type="hidden" value="0" runat="server" />
        <input id="inpPrintOption" type="hidden" value="" runat="server" />
        <input id="inpShowAssessment" type="hidden" value="true" runat="server" />
        <input id="inpShowAnswerKey" type="hidden" value="true" runat="server" />
        <input id="inpShowRubrics" type="hidden" value="true" runat="server" />
        <input id="inpShowInstructions" type="hidden" value="true" runat="server" />
        <input id="inpContentType" type="hidden" runat="server" />        
        <asp:Panel ID="Panel1" runat="server">
            <div style="margin-top: 20px;">
                <table style="width: 480px; font-weight: bold; font-size: 11pt; color: Black; margin-left: 4px; margin-right: 4px;"
                    cellpadding="5" cellspacing="10">
                    <tr>
                        <td class="cellPadding" style="width: 80px;"></td>
                        <td class="cellPadding assessmentCol" align="center">
                            <asp:Label ID="lblAssessment" runat="server" />
                            <asp:ImageButton ID="imgInfoAssessment" runat="server" Width="16" Height="16"
                                ImageUrl="~/Images/QuestionIcon.png" ImageAlign="Middle"
                                OnClientClick="showDisabledMessage(); return false;" />
                        </td>
                        <td class="cellPadding answerKeyCol" align="center">
                            <span>Answer Key</span>
                            <asp:ImageButton ID="imgInfoAnswerKey" runat="server" Width="16" Height="16"
                                ImageUrl="~/Images/QuestionIcon.png" ImageAlign="Middle"
                                OnClientClick="showDisabledMessage(); return false;" />
                        </td>
                      
                        <td class="cellPadding rubricCol" align="center">
                            <span>Rubrics</span>
                            <asp:ImageButton ID="imgInfoRubrics" runat="server" Width="16" Height="16"
                                ImageUrl="~/Images/QuestionIcon.png" ImageAlign="Middle" Visible="false"
                                OnClientClick="showDisabledMessage(); return false;" />
                        </td>
                       
                        <td class="cellPadding instructionsCol" align="center">
                            <span>Display Administration Instructions</span>
                            <asp:ImageButton ID="imgInfoInstructions" runat="server" Width="16" Height="16"
                                ImageUrl="~/Images/QuestionIcon.png" ImageAlign="Middle" Visible="false"
                                OnClientClick="showDisabledMessage(); return false;" />
                        </td>
                    </tr>
                    <tr>
                        <td class="cellPadding">
                            <span>Select All</span>
                        </td>
                        <td class="cellPadding assessmentCol" align="center">
                            <telerik:RadButton ID="cbxAssessment" runat="server" ToggleType="CustomToggle" ButtonType="ToggleButton"
                                AutoPostBack="true" Skin="Web20" OnClick="cbxAssessment_Click">
                                <ToggleStates>
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckbox" />
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckboxFilled" />
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckboxChecked" CssClass="rbSkinnedButtonChecked" />
                                </ToggleStates>
                            </telerik:RadButton>
                        </td>
                        <td class="cellPadding answerKeyCol" align="center">
                            <telerik:RadButton ID="cbxAnswerKey" runat="server" ToggleType="CustomToggle" ButtonType="ToggleButton"
                                AutoPostBack="true" Skin="Web20" OnClick="cbxAnswerKey_Click">
                                <ToggleStates>
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckbox" />
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckboxFilled" />
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckboxChecked" CssClass="rbSkinnedButtonChecked" />
                                </ToggleStates>
                            </telerik:RadButton>
                        </td>
                     
                        <td class="cellPadding rubricCol" align="center">
                            <telerik:RadButton ID="cbxRubrics" runat="server" ToggleType="CustomToggle" ButtonType="ToggleButton"
                                AutoPostBack="false" Skin="Web20" OnClientToggleStateChanged="showRubricPrintMessage" >
                                <ToggleStates>
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckbox" />                                    
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckboxChecked" CssClass="rbSkinnedButtonChecked" />
                                </ToggleStates>
                            </telerik:RadButton>
                        </td>
                    
                        <td class="cellPadding instructionsCol" align="center">
                            <telerik:RadButton ID="cbxInstructions" runat="server" ToggleType="CustomToggle" ButtonType="ToggleButton"
                                AutoPostBack="true" Skin="Web20" OnClick="cbxInstructions_Click">
                                <ToggleStates>
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckbox" />
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckboxFilled" />
                                    <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleCheckboxChecked" CssClass="rbSkinnedButtonChecked" />
                                </ToggleStates>
                            </telerik:RadButton>

                        </td>
                    </tr>
                    <asp:Repeater ID="rptForms" runat="server">
                        <ItemTemplate>
                            <tr formid="<%# Eval("FormID") %>">
                                <td class="cellPadding">
                                    <%# Eval("FormIDDisplay") %>
                                </td>
                                <td class="cellPadding assessmentCol" align="center" >
                                    <asp:TextBox ID="tbxAssessment" Text='<%# Eval("NumAssessmentCopies") %>' runat="server"
                                        Width="24px" BackColor="White" Style="text-align: center;" CssClass="cellFont marginLeft0px"
                                        AutoPostBack="true" Enabled='<%# Eval("AssessmentEnabled") %>' col="0" onkeyup="check_print_enabled()" />
                                </td>
                                <td class="cellPadding answerKeyCol" align="center" >
                                    <asp:TextBox ID="tbxAnswerKey" Text='<%# Eval("NumAnswerKeyCopies") %>' runat="server"
                                        Width="24px" BackColor="White" Style="text-align: center;" CssClass="cellFont marginLeft0px"
                                        AutoPostBack="true" Enabled='<%# Eval("AnswerKeyEnabled") %>' col="1" onkeyup="check_print_enabled()" />
                                </td>                            
                                <td class="cellPadding rubricCol" align="center">
                                    <asp:TextBox ID="tbxRubrics" Text='<%# Eval("NumRubricsCopies") %>' runat="server"
                                        Width="24px" BackColor="White" Style="text-align: center;" CssClass="cellFont marginLeft0px"
                                        AutoPostBack="true" Enabled='<%# Eval("RubricsEnabled") %>' col="2" onkeyup="check_print_enabled()" />
                                </td>
                            
                                <td class="cellPadding instructionsCol" align="left"></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr style="height: 4px;">
                        <td class="cellPadding" colspan="5" style="border-bottom: 1px solid #000000;" />
                    </tr>
                    <tr>
                        <td class="cellPadding">
                            <span>Total</span>
                        </td>
                        <td class="cellPadding assessmentCol" align="center">
                            <asp:Label ID="lblTotalAssessment" CssClass="cellFont" Style="text-align: center;"
                                runat="server" Width="24px" Text='<%# ColumnTotal(numAssessmentCol) %>' />
                        </td>
                        <td class="cellPadding answerKeyCol" align="center">
                            <asp:Label ID="lblTotalAnswerKey" CssClass="cellFont" Style="text-align: center;"
                                runat="server" Width="24px" Text='<%# ColumnTotal(numAnswerKeyCol) %>' />
                        </td>
                      
                        <td class="cellPadding rubricCol" align="center">
                            <asp:Label ID="lblTotalRubric" CssClass="cellFont" Style="text-align: center;" runat="server"
                                Width="24px" Text='<%# ColumnTotal(numRubricsCol) %>' />
                        </td>                     
                        <td class="cellPadding instructionsCol" align="left"></td>
                    </tr>
                </table>
            </div>
            <br />
            <asp:Panel ID="Panel2" runat="server" Width="200px" Style="float: right; margin-right: 4px;">
                <!--	causes postback to run click event-->
                <!--
                        <asp:Button runat="server" ID="Button1" ClientIDMode="Static" CssClass="roundButtons"
								Text="&nbsp;&nbsp;&nbsp;&nbsp;Print&nbsp;&nbsp;&nbsp;&nbsp;" OnClick="btnPrint_Click" />
                                -->

                <asp:Button runat="server" ID="btnPrint" ClientIDMode="Static" CssClass="roundButtons"
                    Text="&nbsp;&nbsp;&nbsp;&nbsp;Print&nbsp;&nbsp;&nbsp;&nbsp;" OnClientClick="submit_print(); return false;" />

                <asp:Button runat="server" ID="btnCancel" ClientIDMode="Static" CssClass="roundButtons"
                    Text="&nbsp;&nbsp;Cancel&nbsp;&nbsp;" OnClientClick="closeCurrentCustomDialog(); return false;" />
            </asp:Panel>
        </asp:Panel>
        <script type="text/javascript">
            window.onload = function () {
                if (document.getElementById("inpShowAssessment").value == "false")
                    $(".assessmentCol").addClass("invisibleCol");
                if (document.getElementById("inpShowAnswerKey").value == "false")
                    $(".answerKeyCol").addClass("invisibleCol");
                if (document.getElementById("inpShowRubrics").value == "false")
                    $(".rubricCol").remove();
                   // $(".rubricCol").addClass("invisibleCol");
                if (document.getElementById("inpShowInstructions").value == "false") {
                    $(".instructionsCol").addClass("grayText");
                }

                check_print_enabled();
            };

            // Show the disabled content alert box.
            function showDisabledMessage() {
                var txt = 'A document has been uploaded for at least one form for this assessment. Disabled forms are missing an uploaded document';
                customDialog({ maximize: true, maxwidth: 300, maxheight: 200, title: 'Disabled Field Information', content: txt, dialog_style: 'alert' },
                                    [{ title: 'Cancel' }]);
            }

            // Show a generic under construction dialog.
            function ShowUnderConstructionDialog() {
                customDialog({ maximize: true, maxwidth: 300, maxheight: 200, resizable: false, title: 'Under Construction', content: 'Functionality is under construction', dialog_style: 'alert' },
                                    [{ title: 'Cancel' }]);
            }

            function submit_print() {
                //alert(PrintInfo);
                //alert(document.forms['printAction'].PrintInfo.value);
                var print_list = [];
                var print_lookup = {};
                $("input[type='text']").each(function (idx) {
                    var $this = $(this);
                    if ($this.attr('col') >= 0 && $this.attr('value') > 0) {
                        var formID = parseInt($this.closest('tr').attr('formID'));
                        var obj = print_lookup[formID];
                        if (!obj) {
                            print_list.push(obj = [formID, 0, 0, 0]);
                            print_lookup[formID] = obj;
                        }
                        obj[parseInt($this.attr('col')) + 1] = parseInt($this.attr('value'));
                    }
                });
                //document.forms['printAction'].PrintInfo.value = print_list;
                /*document.forms["printAction"].action = '../../Record/RenderAssessmentAsPDF.aspx?xID=' + getURLParm('xID') + '&print_list=' + print_list.join(',');
                document.forms["printAction"].target = '_blank';
                document.forms["printAction"].submit();*/
                // concerned about putting print_list in url, but don't want to spend time right now to build a form submit inside a popup iframe
                var isAdminInst = 0;
                if ($("#cbxInstructions").hasClass("rbSkinnedButtonChecked"))
                { isAdminInst = 1; }
                window.open('../../Record/RenderAssessmentAsPDF.aspx?xID=' + getURLParm('xID') + '&print_list=' + JSON.stringify(print_list) + '&isAdminInst=' + isAdminInst);
                //alert(JSON.stringify(print_lookup));
            }
            /*
            function stringify(arry) {
                var str = "[";
                for (var j = 0; j < arry.length; j++) {
                    str += (j > 0 ? ',' : '') + "[" + arry[j].join(',') + "]";
                }
                return str + "]";
            }
            */
            function nonZeroPresent() {
                var inputs = $("input[type='text']");
                for (var index = 0; index < inputs.length; index++) {
                    var $this = inputs.eq(index);
                    if ($this.attr('col') >= 0 && $this.attr('value') > 0) {
                        return true;
                    }
                }
                return false;
            }

            function check_print_enabled() {
                var columnCounts = [0, 0, 0];
                $("input[type='text']").each(function (idx) {
                    var $this = $(this);
                    if ($this.attr('col') >= 0 && $this.attr('value') > 0) {
                        columnCounts[$this.attr('col')] += parseInt($this.attr('value'));
                    }
                });
                var isEnabled = columnCounts.join('') != '000';
                $("#lblTotalAssessment").html(columnCounts[0]);
                $("#lblTotalAnswerKey").html(columnCounts[1]);
                $("#lblTotalRubric").html(columnCounts[2]);

                $btnPrint = $("#btnPrint");
                if (isEnabled) {
                    $btnPrint.removeClass = 'aspNetDisabled';                   
                    $btnPrint.removeAttr('disabled', 'disabled');
                    $("#btnPrint").css('cursor', 'pointer');
                } else {
                    $btnPrint.addClass = 'aspNetDisabled';                  
                    $btnPrint.attr('disabled', 'disabled');
                    $("#btnPrint").css('cursor', 'default');
                    
                }
            }


            function getURLParm(name) {
                name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
                var regexS = "[\\?&]" + name + "=([^&#]*)";
                var regex = new RegExp(regexS);
                var results = regex.exec(window.location.href);
                if (results == null)
                    return "";
                else
                    return decodeURIComponent(results[1].replace(/\+/g, " "));
            } 

            function showRubricPrintMessage(sender, args) {
                var currToggleState = $find("<%=cbxRubrics.ClientID %>").get_selectedToggleState().get_index();
                if (currToggleState == 1) {                    
                    $('#cbxRubrics span:nth-child(1)').removeClass("rbToggleCheckbox");
                    $('#cbxRubrics span:nth-child(1)').removeClass("rbToggleCheckboxChecked");
                    $('#cbxRubrics span:nth-child(1)').addClass('rbToggleCheckboxFilled');
                    if ($('#inpContentType').val() == "EXTERNAL") {
                        submitRubricCheckedEvent(1);
                    } else {
                        customDialog({
                            title: "Message",                            
                            height: 175,
                            width: 400,
                            autoSize: false,
                            onClosed: function (content) {                                
                                setRubricCbx(0);
                            },                            
                            content: "The Rubrics selection will result in all assessment Rubrics being printed on a separate print out. This will exclude the printing of individual item Rubrics below each designated assessment item. Select OK to continue or Cancel to return to the Print Assessment window.",
                            dialog_style: 'alert'
                        }, [{ title: "Cancel" },
                        { title: "OK", callback: submitRubricCheckedEvent, argArray: [1] }]);
                    }
                } else {
                    submitRubricCheckedEvent(0);
                    $('#cbxRubrics span:nth-child(1)').removeClass("rbToggleCheckboxFilled");
                    $('#cbxRubrics span:nth-child(1)').removeClass("rbToggleCheckboxChecked");
                    $('#cbxRubrics span:nth-child(1)').addClass('rbToggleCheckbox');
                }
            }
             
            function submitRubricCheckedEvent(arg) {  
                setRubricCbx(1);               
                __doPostBack('cbxRubrics', arg);
            }

            function resetRubricCheckbox() {
                setRubricCbx(0);
            }

            function setRubricCbx(val) {
                var btn = $find("<%=cbxRubrics.ClientID %>");
                btn.remove_toggleStateChanged(showRubricPrintMessage);                               
                btn.set_selectedToggleStateIndex(val);

                if (val == 0) {
                    $('#cbxRubrics span:nth-child(1)').removeClass("rbToggleCheckboxFilled");
                    $('#cbxRubrics span:nth-child(1)').removeClass("rbToggleCheckboxChecked");
                    $('#cbxRubrics span:nth-child(1)').addClass('rbToggleCheckbox'); 
                } else {
                    $('#cbxRubrics span:nth-child(1)').removeClass("rbToggleCheckbox");
                    $('#cbxRubrics span:nth-child(1)').removeClass("rbToggleCheckboxChecked");
                    $('#cbxRubrics span:nth-child(1)').addClass('rbToggleCheckboxFilled');
                }               

                btn.add_toggleStateChanged(showRubricPrintMessage);
            } 
             
            

        </script>
    </form>
    <form name="printAction" method="post">
        <input type="hidden" id="PrintInfo" />
    </form>
</body>
</html>
