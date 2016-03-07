<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitHaloOrder.aspx.cs" Inherits="Thinkgate.SubmitHaloOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        body {
        }

        table {
            border-spacing: 0px;
            border: 0px solid black;
            width: auto;
            font-family: verdana;
            font-size: 12px;
            text-align: left;
        }

        td {
            padding-top: 5px;
            padding-bottom: 5px;
            padding-left: 20px;
            padding-right: 10px;
        }

        .header {
            border-spacing: 5px;
            padding: 5px;
            border-bottom: 1px solid black;
            width: auto;
            font-family: verdana;
            font-size: 15px;
            font-weight: bold;
            text-align: center;
            background-color: whitesmoke;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="radScriptManager" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery-1.9.1.min.js" />
                <asp:ScriptReference Path="~/scripts/jquery-ui/js/jquery-ui-1.10.0.custom.min.js" />
                <asp:ScriptReference Path="~/Scripts/master.js" />
            </Scripts>
        </telerik:RadScriptManager>

        <div align="center" style="border: 1px solid black; height: 625px">
            <div class="header">Order Halo Forms</div>
            <br />
            <table>
                <tr>
                    <td>Requested By:
                    </td>
                    <td>
                        <asp:Label ID="lblUserName" runat="server" Font-Bold="true"></asp:Label>
                    </td>
                    <td>Date:
                    </td>
                    <td>
                        <asp:Label ID="lblOrderDate" runat="server" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>District:
                    </td>
                    <td>
                        <asp:Label ID="lblDistrict" runat="server" Font-Bold="true"></asp:Label>
                    </td>
                    <td>PO Number:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radPONumber" runat="server" Skin="Web20" MaxLength="15"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td>School:
                    </td>
                    <td>
                        <telerik:RadComboBox ID="radSchool" runat="server" Skin="Web20" EmptyMessage="<Select>" DropDownWidth="250"></telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>Phone:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radPhone" runat="server" Skin="Web20" MaxLength="20"></telerik:RadTextBox>
                    </td>
                    <td colspan="2" style="color: maroon;"><b>Minimum order - 5,000 forms</b>
                    </td>
                </tr>
                <tr>
                    <td>Email:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radEmail" runat="server" Skin="Web20" MaxLength="70"></telerik:RadTextBox>
                    </td>
                    <td>Quantity:
                    </td>
                    <td>
                        <telerik:RadComboBox ID="radQuantity" runat="server" Skin="Web20" EmptyMessage="<Select>" OnClientSelectedIndexChanged="CalculateCosts">
                            <Items>
                                <telerik:RadComboBoxItem Value="5000" Text="5,000" />
                                <telerik:RadComboBoxItem Value="10000" Text="10,000" />
                                <telerik:RadComboBoxItem Value="15000" Text="15,000" />
                                <telerik:RadComboBoxItem Value="20000" Text="20,000" />
                                <telerik:RadComboBoxItem Value="25000" Text="25,000" />
                                <telerik:RadComboBoxItem Value="30000" Text="30,000" />
                                <telerik:RadComboBoxItem Value="35000" Text="35,000" />
                                <telerik:RadComboBoxItem Value="40000" Text="40,000" />
                                <telerik:RadComboBoxItem Value="45000" Text="45,000" />
                                <telerik:RadComboBoxItem Value="50000" Text="50,000" />
                                <telerik:RadComboBoxItem Value="55000" Text="55,000" />
                                <telerik:RadComboBoxItem Value="60000" Text="60,000" />
                                <telerik:RadComboBoxItem Value="65000" Text="65,000" />
                                <telerik:RadComboBoxItem Value="70000" Text="70,000" />
                                <telerik:RadComboBoxItem Value="75000" Text="75,000" />
                                <telerik:RadComboBoxItem Value="80000" Text="80,000" />
                                <telerik:RadComboBoxItem Value="85000" Text="85,000" />
                                <telerik:RadComboBoxItem Value="90000" Text="90,000" />
                                <telerik:RadComboBoxItem Value="95000" Text="95,000" />
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>Email Confirmation:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radEmailConfirmation" runat="server" Skin="Web20" MaxLength="70">
                            <ClientEvents OnBlur="checkForEmailConf" />
                        </telerik:RadTextBox>
                    </td>
                    <td>Date Needed:
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="radDateNeeded" runat="server" Skin="Web20"></telerik:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <b>Billing Address</b>
                    </td>
                </tr>
                <tr>
                    <td>ATTN:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radBillingPOC" runat="server" Skin="Web20" MaxLength="30"></telerik:RadTextBox>
                    </td>
                    <td>Halo Forms:  $
                    </td>
                    <td style="text-align: right; padding-right: 180px">
                        <asp:Label ID="FormCost" runat="server"></asp:Label>
                        <asp:HiddenField runat="server" ID="FormCostHidden"/>
                        <asp:HiddenField runat="server" ID="HaloCostPerBox"/>
                        <asp:HiddenField runat="server" ID="HaloFormsPerBox"/>
                        <asp:HiddenField runat="server" ID="HaloShippingCost"/>
                        <asp:HiddenField runat="server" ID="HaloShippingCostFlatRate"/>
                    </td>
                </tr>
                <tr>
                    <td>Street:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radBillingStreet" runat="server" Skin="Web20" MaxLength="50"></telerik:RadTextBox>
                    </td>
                    <td>Shipping Costs:  $
                    </td>
                    <td style="text-align: right; padding-right: 180px">
                        <asp:Label ID="ShippingCost" runat="server"></asp:Label>
                        <asp:HiddenField runat="server" ID="ShippingCostHidden"/>
                    </td>
                </tr>
                <tr>
                    <td>City, State:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radBillingCity" runat="server" Skin="Web20" MaxLength="30"></telerik:RadTextBox>
                    </td>
                    <td>Total: $
                    </td>
                    <td style="text-align: right; padding-right: 180px">
                        <asp:Label ID="TotalCost" runat="server"></asp:Label>
                        <asp:HiddenField runat="server" ID="TotalCostHidden"/>
                    </td>
                </tr>
                <tr>
                    <td>Zip Code:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radBillingZip" runat="server" Skin="Web20" MaxLength="15"></telerik:RadTextBox>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <b>Shipping Address</b>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkSameAsBilling" runat="server" Text="Same as Billing" onclick="LoadShpAddrFields(this)"></asp:CheckBox>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>ATTN:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radShippingPOC" runat="server" Skin="Web20" MaxLength="30"></telerik:RadTextBox>
                    </td>
                    <td rowspan="4">Comments:
                        <br />
                        (170 char max)
                    </td>
                    <td rowspan="4">
                        <telerik:RadTextBox ID="radComments" runat="server" Skin="Web20" MaxLength="170" TextMode="MultiLine" Height="80" Width="300"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td>Street:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radShippingStreet" runat="server" Skin="Web20" MaxLength="50"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td>City, State:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radShippingState" runat="server" Skin="Web20" MaxLength="30"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td>Zip Code:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radShippingZip" runat="server" Skin="Web20" MaxLength="15"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="3">
                        <telerik:RadButton ID="radSubmit" runat="server" Skin="Web20" Text="Submit" Width="75" OnClientClicked="Validate" OnClick="radSubmit_Click" />
                    </td>
                </tr>
                <tr>
                    <td style="font-weight: bold; color: maroon;">*** REMINDERS ***
                    </td>
                    <td colspan="3" style="color: maroon;">Please plan for four (4) weeks delivery time
                        <br />
                        All orders have a minimum quantity of 5000 forms
                    </td>
                </tr>
            </table>
        </div>

        <script type="text/javascript">
            function MessageAlert(message) {
                parent.customDialog({ title: 'Submit Halo Order', maximize: true, maxwidth: 500, maxheight: 100, animation: 'None', dialog_style: 'alert', content: message }, [{ title: 'OK' }]);;
            }

            function Validate(sender, args) {
                if (document.getElementById('radPONumber').value &&
                	document.getElementById('radSchool').value &&
                    document.getElementById('radPhone').value &&
                	document.getElementById('radEmail').value &&
                	document.getElementById('radEmailConfirmation').value &&
                	document.getElementById('radQuantity').value &&
                	document.getElementById('radDateNeeded').value &&
                	document.getElementById('radBillingPOC').value &&
                	document.getElementById('radBillingStreet').value &&
                	document.getElementById('radBillingCity').value &&
                	document.getElementById('radBillingZip').value &&
                	document.getElementById('radShippingPOC').value &&
                	document.getElementById('radShippingStreet').value &&
                	document.getElementById('radShippingState').value &&
                	document.getElementById('radShippingZip').value) {

                    sender.set_autoPostBack(true);
                }
                else {
                    alert('All fields are mandatory.');
                    sender.set_autoPostBack(false);
                }
            }

            function CalculateCosts(sender, eventArgs) {
                var ordQuantity = Number(eventArgs.get_item().get_value());
                var pricePerBox = document.getElementById("HaloCostPerBox").value;
                var haloFormsPerBox = document.getElementById("HaloFormsPerBox").value;
                var shippingCost = Number(document.getElementById("HaloShippingCost").value);
                var isFlatRate = document.getElementById("HaloShippingCostFlatRate").value;

                var formCost = Number(ordQuantity / haloFormsPerBox * pricePerBox);
                var formCostStr = String(FormatNumber(formCost, 2, false, false, true));
                document.getElementById('FormCost').innerHTML = formCostStr;
                document.getElementById('FormCostHidden').value = formCostStr;

                shippingCost = isFlatRate == 'Yes' ? shippingCost : Number(ordQuantity / haloFormsPerBox * shippingCost);
                var shippingCostStr = String(FormatNumber(shippingCost, 2, false, false, true));
                document.getElementById('ShippingCost').innerHTML = shippingCostStr;
                document.getElementById('ShippingCostHidden').value = shippingCostStr;


                var totalCost = formCost + shippingCost;
                var totalCostStr = String(FormatNumber(totalCost, 2, false, false, true));
                document.getElementById('TotalCost').innerHTML = totalCostStr;
                document.getElementById('TotalCostHidden').value = totalCostStr;
            }

            function FormatNumber(num, decimalNum, bolLeadingZero, bolParens, bolCommas)
                /**********************************************************************
                    IN:
                        num - the number to format
                        decimalNum - the number of decimal places to format the number to
                        bolLeadingZero - true / false - display a leading zero for numbers between -1 and 1
                        bolParens - true / false - use parenthesis around negative numbers
                        bolCommas - put commas as number separators.
                 
                    RETVAL:
                        The formatted number!
                 **********************************************************************/ {
                if (isNaN(parseInt(num))) return "NaN";

                var tmpNum = num;
                var iSign = num < 0 ? -1 : 1;		// Get sign of number

                // Adjust number so only the specified number of numbers after
                // the decimal point are shown.
                tmpNum *= Math.pow(10, decimalNum);
                tmpNum = Math.round(Math.abs(tmpNum));
                tmpNum /= Math.pow(10, decimalNum);
                tmpNum *= iSign;					// Readjust for sign

                // Create a string object to do our formatting on
                var tmpNumStr = new String(tmpNum);

                // See if we need to strip out the leading zero or not.
                if (!bolLeadingZero && num < 1 && num > -1 && num != 0)
                    if (num > 0)
                        tmpNumStr = tmpNumStr.substring(1, tmpNumStr.length);
                    else
                        tmpNumStr = "-" + tmpNumStr.substring(2, tmpNumStr.length);

                // See if we need to put in the commas
                if (bolCommas && (num >= 1000 || num <= -1000)) {
                    var iStart = tmpNumStr.indexOf(".");
                    if (iStart < 0)
                        iStart = tmpNumStr.length;

                    iStart -= 3;
                    while (iStart >= 1) {
                        tmpNumStr = tmpNumStr.substring(0, iStart) + "," + tmpNumStr.substring(iStart, tmpNumStr.length)
                        iStart -= 3;
                    }
                }

                // See if we need to use parenthesis
                if (bolParens && num < 0)
                    tmpNumStr = "(" + tmpNumStr.substring(1, tmpNumStr.length) + ")";


                //Add trailing zeros if we need to
                var chartest = tmpNumStr.substr(tmpNumStr.length - 3);
                if (chartest.indexOf('.') == 1) {
                    tmpNumStr = tmpNumStr + '0';
                }
                else {
                    if (chartest.indexOf('.') == -1) {
                        tmpNumStr = tmpNumStr + '.00';
                    }
                }


                return tmpNumStr;		// Return our formatted string!
            }

            function checkForEmailConf(sender, args) {
                textboxEmail = $find('<%= radEmail.ClientID %>');
                textboxEmailConf = $find('<%= radEmailConfirmation.ClientID %>');

                if (textboxEmail.get_value() == textboxEmailConf.get_value() || textboxEmailConf.get_value() == "") {
                    true;
                }
                else {
                    alert('Email confirmation does not match email. Please re-enter');
                    textboxEmailConf.set_value("");
                    textboxEmailConf.focus();
                }
            }

            function LoadShpAddrFields(ctrl) {

                var textboxShipping;
                var textboxBilling;

                if (ctrl.checked) {
                    textboxShipping = $find('<%= radShippingPOC.ClientID %>');
                    textboxBilling = $find('<%= radBillingPOC.ClientID %>');
                    textboxShipping.set_value(textboxBilling.get_value());

                    textboxShipping = $find('<%= radShippingStreet.ClientID %>');
                    textboxBilling = $find('<%= radBillingStreet.ClientID %>');
                    textboxShipping.set_value(textboxBilling.get_value());

                    textboxShipping = $find('<%= radShippingState.ClientID %>');
                    textboxBilling = $find('<%= radBillingCity.ClientID %>');
                    textboxShipping.set_value(textboxBilling.get_value());

                    textboxShipping = $find('<%= radShippingZip.ClientID %>');
                    textboxBilling = $find('<%= radBillingZip.ClientID %>');
                    textboxShipping.set_value(textboxBilling.get_value());
                }
                else {
                    textboxShipping = $find('<%= radShippingPOC.ClientID %>');
                    textboxShipping.set_value("");

                    textboxShipping = $find('<%= radShippingStreet.ClientID %>');
                    textboxShipping.set_value("");

                    textboxShipping = $find('<%= radShippingState.ClientID %>');
                    textboxShipping.set_value("");

                    textboxShipping = $find('<%= radShippingZip.ClientID %>');
                    textboxShipping.set_value("");
                }
            }
        </script>

    </form>
</body>
</html>
