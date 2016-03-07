<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImprovementPlanCoverPageTemplate.ascx.cs" Inherits="Thinkgate.ImprovementPlan.ImprovementPlanCoverPageTemplate" %>


<style type="text/css">
    .viewModeCss {
       
       border:none;
       background-color:transparent;
       overflow:hidden;
       vertical-align:middle;

    }

      .tableImprovementPlan {
        width: 100%;
    }
         
      .text-area-height {
        height: 50px;
      }
         
       .width-99per {
        width: 99%;
      }

    .font-bold {

        font-weight: bold;
        font-size: 14px;

    }


    /*td {
        padding-top: 10px;
        padding-left: 2%;
    }*/

    .tdFirstColumn {
            width: 200px;
        }

        .inputtextbox {
            width: 100%;
            padding-top: 5px;
            padding-bottom: 5px;
        }
            .font-family {
        font-family: Segoe UI;
    }


    .tdSecondColumn {
            width: 50%;
        padding-right: 5%;
        }

        .text-area-heigth {
            height: 50px;
        }

                .width-90per {
            width: 90%;
        }

    .tdthirdColumn {
            width: 15%;
        padding-right: 2%;
        padding-left: 2%;
        }

        .inputButtons {
            width: 30%;
            height: 30px;
            margin-left: 20px;
        background: cornflowerblue;
            color: white;
            font-weight: bold;
        }

         .full-width {
        width: 100%;
    }

             .border-2px {
        border: 2px solid black;
    }

             
    .centered {
        margin: 0 auto;
        text-align: left;
        width: 98%;
    }


    </style>

<script type="text/javascript">
    function inProgressTileRefresh() {
        var btnSpan = parent.document.getElementById('refreshTile');
        var btnTrigger = btnSpan && typeof (btnSpan) != 'undefined' && btnSpan.childNodes.length > 0 ? btnSpan.childNodes[0] : null;
        if (btnTrigger)
            btnTrigger.click();
    }
    
    function setHeight(txtdesc) {
        txtdesc.style.height = txtdesc.scrollHeight + "px";
    }
</script>
<div id="dvCoverPage" style="background:lavender" runat="server" class="centered border-2px font-family">

 <div style="float:left">
            <img alt="Paulding Country School District" longdesc="Focused on Learning" src="../Images/ClientLogos/GAPauldinglogo.jpg" runat="server" ID="imgLogo"/>
            </div><div style="text-align: center; padding-left: 100px">
            <h4><br/><b>
                    <asp:Label ID="lblPageTitle" runat="server" ></asp:Label> </b></h4>
          
        </div>
    <div style="clear: both" />
    
        <table class="tableImprovementPlan">
            <tr>
                <td class="tdFirstColumn">School District:</td>
                <td class="tdSecondColumn">
                    <asp:TextBox ID="txtDistrictName" runat="server" class="inputtextbox" ReadOnly="true"></asp:TextBox>
                </td>
                <td class="tdthirdColumn">School Year:</td>
                <td style="padding-left: 1%; padding-right: 1%">
                    <asp:TextBox ID="txtSchoolYear" runat="server" class="inputtextbox" ReadOnly="true"></asp:TextBox>
                </td>
            </tr>
            
            <tr>
                <td class="tdFirstColumn">Name of Superintendent:
        </td>
                <td class="tdSecondColumn">
                    <asp:TextBox ID="txtSuperintendent" runat="server" class="inputtextbox" MaxLength="50" /></td>
                
            </tr>
            <tr>
                <td class="tdFirstColumn">
                    <asp:Label ID="lblSchoolNameLabel" runat="server" Text="Name of School:" /></td>
                <td class="tdSecondColumn">
                    
                    <asp:TextBox ID="txtSchoolName" runat="server" class="inputtextbox" ReadOnly="true" /></td>
    
            </tr>
            <tr>
                <td class="tdFirstColumn">
                    <asp:Label ID="lblPrincipalNameLabel" runat="server" Text="Name of Principal:" /></td>
                <td class="tdSecondColumn">
                    <asp:TextBox ID="txtPrincipal" runat="server" class="inputtextbox" MaxLength="50" />
      
                </td>
               
            </tr>
        </table>
        <br />
       <table id="tbStrategicGoals" class="width-99per">
                        <tr>
                <td class="pad-left-1">District Strategic Goals:
                    <asp:Button ID="btnAddStrategicGoal" runat="server" Text="Add Strategic Goal" Width="150px" CssClass="button-css" OnClick="btnAddStrategicGoal_Click" /></td>
                        </tr>
                        <tr>
                           <td>

                                <asp:Repeater ID="rptStrategicGoal" runat="server" OnItemDataBound="rptStrategicGoal_ItemDataBound">                                  
                                    <ItemTemplate>
                                        <br />
                            <table class="width-99per">
                                
                                <tr>
                                    <td style="width: 5%"/>
                                    <td>
                                          <asp:Label ID="lblStrategicGoal" runat="server" CssClass="font-bold">District Strategic Goal <%# string.Format("{0} -", Container.ItemIndex + 1)  %></asp:Label> </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 5%"/>
                                        <td>
                                           <asp:TextBox ID="txtStrategicGoal" runat="server" CssClass="full-width text-area-heigth"
                                            TextMode="MultiLine"                                                                                  
                                            StrategicGoalID='<%# DataBinder.Eval(Container.DataItem, "ID") %>'
                                            ImprovementPlanID='<%# DataBinder.Eval(Container.DataItem, "ImprovementPlanID") %>'
                                            Text='<%# DataBinder.Eval(Container.DataItem, "StrategicGoal") %>'                                           
                                            InitialValue='<%# DataBinder.Eval(Container.DataItem, "StrategicGoal") %>' 
                                               onkeyup="setHeight(this);" ></asp:TextBox>
                                            <asp:Label ID="lblStrategicGoalReadOnly" CssClass="width-99per text-area-heigth" runat="server"
                                                Visible="false"></asp:Label>
                               </td>
                              </tr>
                            </table>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </td>
                        </tr>
                    </table>
        <div style="text-align: center">
            <asp:Label ID="lblDisplayForSIP" runat="server" Style="font-size: x-small;" Text="The following document outlines the school's plan for improvement as aligned to the district's goals and priorities."></asp:Label>
        </div>
        <br />
        <br />
        <div style="float: left;">
        <asp:CheckBox ID="chkFinalized" runat="server"/> Finalized    
            <br/>
        </div>
            <div style="float: right;">
            Date: <telerik:RadDatePicker ID="rdpCreateDateCoverPage" runat="server" Width="140px" AutoPostBack="false" Skin="Vista"
                DateInput-EmptyMessage="Select Date" MinDate="01/01/1000" MaxDate="01/01/3000">
                 <Calendar runat="server" EnableKeyboardNavigation="true" />
                    </telerik:RadDatePicker>
                <br/>
        </div>
       
        <br/>
        <div style="float: left; width: 70%">
            
        <asp:Button id="btnCoverPageSave" Text="Save" class="inputButtons" onclick="btnSave_Click" runat="server"/>
        <asp:Button id="btnCoverPageSaveContinue" Text="Save and Continue" class="inputButtons" onclick="btnSave_Click"  runat="server"/>

        </div>

    
    <asp:HiddenField runat="server" ID="hiddenImpPlanAction" />
    <asp:HiddenField runat="server" ID="hiddenImprovementPlanID" />
    
    </div>

