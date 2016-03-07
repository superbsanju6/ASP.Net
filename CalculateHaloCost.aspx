<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalculateHaloCost.aspx.cs" Inherits="Thinkgate.CalculateHaloCost" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        body {
        }

        table {
            border-spacing: 0px;
            border: 1px solid black;
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
            <div class="header">Calculate Halo Form Costs</div>
            <br />
            <br />
            <br />
            <br />
            <table>
                <tr>
                    <td></td>
                    <td style="color: maroon;"><b>Minimum order - 5,000 forms</b>
                    </td>
                </tr>
                <tr>
                    <td>Quantity Requested:
                    </td>
                    <td>
                        <telerik:RadComboBox ID="radComboBoxQty" runat="server" Skin="Web20" EmptyMessage="<Select>" OnClientSelectedIndexChanged="CalculateCosts">
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
                    <td>Halo Forms: $
                    </td>
                    <td style="text-align: right; padding-right: 100px">
                        <asp:Label ID="FormCost" runat="server"></asp:Label>
                        <asp:HiddenField runat="server" ID="FormCostHidden"/>
                        <asp:HiddenField runat="server" ID="HaloCostPerBox"/>
                        <asp:HiddenField runat="server" ID="HaloFormsPerBox"/>
                        <asp:HiddenField runat="server" ID="HaloShippingCost"/>
                        <asp:HiddenField runat="server" ID="HaloShippingCostFlatRate"/>
                    </td>
                </tr>
                <tr>
                    <td>Shipping Costs: $
                    </td>
                    <td style="text-align: right; padding-right: 100px">
                        <asp:Label ID="ShippingCost" runat="server"></asp:Label>
                        <asp:HiddenField runat="server" ID="ShippingCostHidden"/>
                    </td>
                </tr>
                <tr>
                    <td>Total: $
                    </td>
                    <td style="text-align: right; padding-right: 100px">
                        <asp:Label ID="TotalCost" runat="server"></asp:Label>
                        <asp:HiddenField runat="server" ID="TotalCostHidden"/>
                    </td>
                </tr>
                <tr>
                    <td>Email 1:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radTextBoxEmail1" runat="server" Skin="Web20" Width="250"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td>Email 2:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="radTextBoxEmail2" runat="server" Skin="Web20" Width="250"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <telerik:RadButton ID="radButtonEmail" runat="server" Skin="Web20" Text="Email" Width="75" OnClientClicked="Validate" OnClick="radButtonEmail_Click"></telerik:RadButton>
                    </td>
                </tr>
            </table>
        </div>

        <script type="text/javascript">
            function MessageAlert(message) {
                parent.customDialog({ title: 'Calculate Halo Costs', height: 100, width: 500, animation: 'None', dialog_style: 'alert', content: message }, [{ title: 'OK' }]);;
            }

            function Validate(sender, args) {
                if (document.getElementById('radComboBoxQty').value &&
                    (document.getElementById('radTextBoxEmail1').value || document.getElementById('radTextBoxEmail2').value)) {

                    sender.set_autoPostBack(true);
                }
                else {
                    alert('Quantity/Email fields are mandatory.');

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
        </script>

    </form>
</body>
</html>
