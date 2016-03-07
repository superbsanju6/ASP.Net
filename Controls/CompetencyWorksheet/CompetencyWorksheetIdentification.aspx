<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/AddNew.Master" CodeBehind="CompetencyWorksheetIdentification.aspx.cs" Inherits="Thinkgate.Controls.CompetencyWorksheet.CompetencyWorksheetIdentification"
    Title="Identification" %>

<asp:Content ID="Content2" ContentPlaceHolderID="headContent" runat="server">

    <link href="../../Scripts/reset-min.css" rel="stylesheet" />
    <%-- <link href="../../Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />--%>
    <link href="../../Scripts/DataTables/css/site_jui.ccss" rel="stylesheet" />
    <link href="../../Scripts/DataTables/css/demo_table_jui.css" rel="stylesheet" />
    <link href="../../Scripts/jquery-ui/css/smoothness/jquery-ui-1.10.0.custom.css" rel="stylesheet" />

    <script type="text/javascript" src="../../Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-migrate-1.1.0.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-ui/js/jquery-ui-1.10.0.custom.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery.cookie.js"></script>
    <script type="text/javascript" src="../../Scripts/DataTables/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../../Scripts/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="../../Scripts/Custom/addNewDocument.js"></script>
    <script type="text/javascript" src="../../Scripts/Custom/tgDivTools.js"></script>
    <script type="text/javascript" src="../../Scripts/master.js"></script>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">


            var is500 = false;
            var isCancel = false;
            function getStandardsDataTableBygrade(sender, args) {
                if (args != null) {
                    //StudentForClass();
                    var Grade = args._item._parent._value;
                    var txtDescription = $("#<%= txtDescription.ClientID %>").val();
                    txtDescription = $.trim(txtDescription);
                    var name = $("#<%= txtName.ClientID %>").val();
                    var rubric = $("#<%= cmbRubric.ClientID %>").val();
                    var subjectDropdown = $("#<%= subjectDropdown.ClientID %>").val();
                    var courseDropdown = $("#<%= courseDropdown.ClientID %>").val();
                    name = $.trim(name);
                    if (subjectDropdown && courseDropdown && txtDescription && name && rubric && args._item._parent._value != "") {
                        if (is500 == false) {
                            $("#btnContinue").prop('disabled', false).prop('refresh');
                            $("#btnSave").prop('disabled', false).prop('refresh');
                        }
                    }
                    else {
                        $("#btnContinue").prop('disabled', true).prop('refresh');
                        $("#btnSave").prop('disabled', true).prop('refresh');
                    }

                    $.ajax({
                        type: "POST",
                        url: "CompetencyWorksheetIdentification.aspx/LoadSubjectsButtonFilterByGrade",
                        data: "{'gradeVal':'" + Grade + "'}",

                        contentType: "application/json; charset=utf-8",
                        dataType: "json",

                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            //alert(textStatus + "\n" + errorThrown);
                        },
                        success: function (result) {
                            var senderElement = sender.get_element();
                            var dropdownObject = $find('subjectDropdown');
                            clearAllDropdownItems(dropdownObject);
                            var results = result.d;
                            for (var i = 0; i < results.length; i++) {
                                addDropdownItem(dropdownObject, results[i], results[i]);
                            }

                            if (results.length == 1) {
                                dropdownObject.get_items().getItem(0).select();
                                results = null;
                            }
                            else if (dropdownObject._emptyMessage) {
                                dropdownObject.clearSelection();
                            }
                        }
                    });
                    // StudentForClass();
                    args = null;
                }
            }
            function addDropdownItem(dropdownObject, itemValue, itemText) {
                if (!dropdownObject || !itemText || !itemValue) {
                    return false;
                }
                /*indicates that client-side changes are going to be made and 
                these changes are supposed to be persisted after postback.*/
                dropdownObject.trackChanges();

                //Instantiate a new client item
                var item = new Telerik.Web.UI.RadComboBoxItem();

                //Set its text and add the item
                item.set_text(itemText);
                item.set_value(itemValue);
                dropdownObject.get_items().add(item);

                //submit the changes to the server.
                dropdownObject.commitChanges();
            }

            function clearAllDropdownItems(dropdownObject) {
                var allItems = dropdownObject.get_items().get_count();
                if (allItems < 1) {
                    return false;
                }

                /*indicates that client-side changes are going to be made and 
                these changes are supposed to be persisted after postback.*/
                dropdownObject.trackChanges();

                //clear all items
                dropdownObject.get_items().clear();

                //submit the changes to the server.
                dropdownObject.commitChanges();

                return false;
            }

            function getStandardsDataTableBySubject(sender, args) {
                if (args != null) {
                    StudentForClass();
                    var name = $("#<%= txtName.ClientID %>").val();
                    var rubric = $("#<%= cmbRubric.ClientID %>").val();
                    var txtDescription = $("#<%= txtDescription.ClientID %>").val();
                    txtDescription = $.trim(txtDescription);
                    var gradeDropdown = $("#<%= gradeDropdown.ClientID %>").val();
                    var courseDropdown = $("#<%= courseDropdown.ClientID %>").val();
                    var subjectDropdown = args._item._parent._value;
                    name = $.trim(name);
                    if (gradeDropdown && courseDropdown && txtDescription && name && rubric && args._item._parent._value != "") {
                        if (is500 == false) {
                            $("#btnContinue").prop('disabled', false).prop('refresh');
                            $("#btnSave").prop('disabled', false).prop('refresh');
                        }
                    }
                    else {
                        $("#btnContinue").prop('disabled', true).prop('refresh');
                        $("#btnSave").prop('disabled', true).prop('refresh');
                    }

                    $.ajax({
                        type: "POST",
                        url: "CompetencyWorksheetIdentification.aspx/LoadCoursesButtonFilterBySubject",
                        data: "{'gradeVal':'" + gradeDropdown + "','subject':'" + subjectDropdown + "'}",

                        contentType: "application/json; charset=utf-8",
                        dataType: "json",

                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            //alert(textStatus + "\n" + errorThrown);
                        },
                        success: function (result) {
                            var senderElement = sender.get_element();
                            var dropdownObject = $find('courseDropdown');
                            clearAllDropdownItems(dropdownObject);
                            var results = result.d;
                            for (var i = 0; i < results.length; i++) {
                                var finalvalue = results[i].split('/');
                                addDropdownItem(dropdownObject, finalvalue[0], finalvalue[1]);
                            }

                            if (results.length == 1) {
                                dropdownObject.get_items().getItem(0).select();
                            }
                            else if (dropdownObject._emptyMessage) {
                                dropdownObject.clearSelection();
                            }
                        }
                    });
                    // StudentForClass();
                    args = null;
                }
            }

            function getStandardsDataTableByCourse(sender, args) {
                     
                StudentForClass();

                var name = $("#<%= txtName.ClientID %>").val();
                var rubric = $("#<%= cmbRubric.ClientID %>").val();
                var txtDescription = $("#<%= txtDescription.ClientID %>").val();
                txtDescription = $.trim(txtDescription);
                var gradeDropdown = $("#<%= gradeDropdown.ClientID %>").val();
                var subjectDropdown = $("#<%= subjectDropdown.ClientID %>").val();
                name = $.trim(name);

                courseVal = args._item._parent._value;

                if (gradeDropdown && subjectDropdown && txtDescription && name && rubric && args._item._parent._value != "") {
                    if (is500 == false) {
                        $("#btnContinue").prop('disabled', false).prop('refresh');
                        $("#btnSave").prop('disabled', false).prop('refresh');
                    }
                }
                else {
                    $("#btnContinue").prop('disabled', true).prop('refresh');
                    $("#btnSave").prop('disabled', true).prop('refresh');
                }
             
                $.ajax({
                    type: "POST",
                    url: "CompetencyWorksheetIdentification.aspx/LoadRubricsFilterByGradeSubjectCourse",
                    data: "{'gradeVal':'" + gradeDropdown + "','subjectVal':'" + subjectDropdown + "','courseVal':'" + courseVal + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(XMLHttpRequest.responseText + "\n" + errorThrown);
                    },
                    success: function (result) {
                        var senderElement = sender.get_element();
                        var dropdownObject = $find('cmbRubric');
                        clearAllDropdownItems(dropdownObject);
                        var results = result.d;
                        for (var i = 0; i < results.length; i++) {
                            var finalvalue = results[i].split('/');
                            addDropdownItem(dropdownObject, finalvalue[0], finalvalue[1]);
                        }

                        if (results.length == 1) {
                            dropdownObject.get_items().getItem(0).select();
                        }
                        else if (dropdownObject._emptyMessage) {
                            dropdownObject.clearSelection();
                        }
                    }
                });
                //   StudentForClass();

            }


            function ValueChanged(sender, args) {
                var rubric = $("#<%= cmbRubric.ClientID %>").val();
                var gradeDropdown = $("#<%= gradeDropdown.ClientID %>").val();
                var subjectDropdown = $("#<%= subjectDropdown.ClientID %>").val();
                var courseDropdown = $("#<%= courseDropdown.ClientID %>").val();

                var txtDescription = $("#<%= txtDescription.ClientID %>").val();
                txtDescription = $.trim(txtDescription);

                var name = args._newValue;
                name = $.trim(name);

                if (txtDescription && name && rubric && gradeDropdown && subjectDropdown && courseDropdown) {
                    if (is500 == false) {
                        $("#btnContinue").prop('disabled', false).prop('refresh');
                        $("#btnSave").prop('disabled', false).prop('refresh');
                    }
                }
                else {
                    $("#btnContinue").prop('disabled', true).prop('refresh');
                    $("#btnSave").prop('disabled', true).prop('refresh');
                }
                // StudentForClass();
            }

            function DescriptionValueChanged(sender, args) {
                var rubric = $("#<%= cmbRubric.ClientID %>").val();
                var gradeDropdown = $("#<%= gradeDropdown.ClientID %>").val();
                var subjectDropdown = $("#<%= subjectDropdown.ClientID %>").val();
                var courseDropdown = $("#<%= courseDropdown.ClientID %>").val();

                var name = $("#<%= txtName.ClientID %>").val();
                name = $.trim(name);
                var txtDescription = $("#<%= txtDescription.ClientID %>").val();
                txtDescription = $.trim(txtDescription);

                if (txtDescription && name && rubric && gradeDropdown && subjectDropdown && courseDropdown) {
                    if (is500 == false) {
                        $("#btnContinue").prop('disabled', false).prop('refresh');
                        $("#btnSave").prop('disabled', false).prop('refresh');
                    }
                }
                else {
                    $("#btnContinue").prop('disabled', true).prop('refresh');
                    $("#btnSave").prop('disabled', true).prop('refresh');
                }
                //     StudentForClass();
            }

            function OnClientSelectedIndexChanged(sender, args) {
                var txtDescription = $("#<%= txtDescription.ClientID %>").val();
                txtDescription = $.trim(txtDescription);

                var name = $("#<%= txtName.ClientID %>").val();
                var gradeDropdown = $("#<%= gradeDropdown.ClientID %>").val();
                var subjectDropdown = $("#<%= subjectDropdown.ClientID %>").val();
                var courseDropdown = $("#<%= courseDropdown.ClientID %>").val();

                name = $.trim(name);
                if (args._item._parent._value != "" && txtDescription && name && gradeDropdown && subjectDropdown && courseDropdown) {
                    if (is500 == false) {
                        $("#btnContinue").prop('disabled', false).prop('refresh');
                        $("#btnSave").prop('disabled', false).prop('refresh');
                    }
                }
                else {
                    $("#btnContinue").prop('disabled', true).prop('refresh');
                    $("#btnSave").prop('disabled', true).prop('refresh');
                }
                // StudentForClass();
            }

            function closeCmsDialogOnCancel() {
                var rubric = $("#<%= cmbRubric.ClientID %>").val();
                var name = $("#<%= txtName.ClientID %>").val();
                var gradeDropdown = $("#<%= gradeDropdown.ClientID %>").val();
                var subjectDropdown = $("#<%= subjectDropdown.ClientID %>").val();
                var courseDropdown = $("#<%= courseDropdown.ClientID %>").val();

                var txtDescription = $("#<%= txtDescription.ClientID %>").val();
                txtDescription = $.trim(txtDescription);
                name = $.trim(name);

                if (rubric != "" || txtDescription || name || gradeDropdown || subjectDropdown || courseDropdown) {
                    if (confirm("Are you sure you want to cancel?")) {
                        isCancel = true;
                        $("#<%= hdnCancelClickCheck.ClientID %>").val('1');
                        var oWnd = getCurrentCustomDialog();
                        oWnd.close();
                    }
                }
                else {// if (confirm("Are you sure you want to cancel?")) {
                    var oWnd = getCurrentCustomDialog();
                    oWnd.close();
                }
                return false;
            }

            function copyMessage(encrptid, classid) {
                var msg = "This is copy of previously viewed worksheet";
                window.parent.jQuery('#divCWCopyDialog').dialog('close');
                top.location.href = "../../CompetencyWorksheetPreview.aspx?xID=" + encrptid + "&classid=" + classid;
                sessionStorage.reloadAfterPageLoad = true;


            }

            function DuplicateRecords() {
                alert("Duplicate Records: Work sheet already exists.");
            }


            function CloseDialog(type) {
                isCancel = true;
                if (type == "edit")
                    window.parent.jQuery('#divCWEditDialog').dialog('close');
                else if (type == "copy")
                    window.parent.jQuery('#divCWCopyDialog').dialog('close');
                else if (type == "save") {
                    window.parent.jQuery('#divCWCopyDialog').dialog('close');
                    top.location.href = top.location.href;
                    window.parent.opener.location.href = window.parent.opener.location.href;
                }
                else if (type == "update") {
                    window.close();
                    top.location.href = top.location.href;
                }
                $("#<%= hdnCancelClickCheck.ClientID %>").val('1');
            }
                     
            function CallPage(worksheetid, isNew) {
                parent.customDialog({ url: "../Controls/CompetencyWorksheet/CompetencyStdAddNewAssoc.aspx?parentnodeid=" + worksheetid + "&prevpage=CompetencyWorksheetIdentification.aspx&IsNew=" + isNew, title: "Standards Associations", name: 'CompetencyStdAddNewAssoc', width: 1020, height: 650, destroyOnClose: true, closeMode: false });
                var win = getCurrentCustomDialog();
                if (win) setTimeout(function () { win.close() }, 0);
            }
            
            $(document).on({
                ajaxStart: function () {
                    $("#tbldiv").addClass("modal");
                },
                ajaxStop: function () {
                    $("#tbldiv").removeClass("modal");
                }
            });

            $(window).load(function () {
                var rubric = $("#<%= cmbRubric.ClientID %>").val();
                var name = $("#<%= txtName.ClientID %>").val();
                var gradeDropdown = $("#<%= gradeDropdown.ClientID %>").val();
                var subjectDropdown = $("#<%= subjectDropdown.ClientID %>").val();
                var courseDropdown = $("#<%= courseDropdown.ClientID %>").val();
                var description = $("#<%= txtDescription.ClientID %>").val();
                description = $.trim(description);
                name = $.trim(name);

                if ((typeof rubric != 'undefined' && rubric != "") && (typeof description != 'undefined' && description != "") && (typeof name != 'undefined' && name != "") && (typeof gradeDropdown != 'undefined' && gradeDropdown != "") && (typeof subjectDropdown != 'undefined' && subjectDropdown != "") && (typeof courseDropdown != 'undefined' && courseDropdown != "")) {
                    $("#btnContinue").prop('disabled', false).prop('refresh');
                    $("#btnSave").prop('disabled', false).prop('refresh');
                }
                else {
                    $("#btnContinue").prop('disabled', true).prop('refresh');
                    $("#btnSave").prop('disabled', false).prop('refresh');
                }
                if (isCancel == false && $("#<%= hdnCancelClickCheck.ClientID %>").val() != '1') {
                    StudentForClass();
                    $("#<%= hdnCancelClickCheck.ClientID %>").val('');
                }


            });

            function StudentForClass() {
                is500 = false;
                var gradeDropdown = $("#<%= gradeDropdown.ClientID %>").val();
                var subjectDropdown = $("#<%= subjectDropdown.ClientID %>").val();
                var courseDropdown = $("#<%= courseDropdown.ClientID %>").val();

                $.ajax({
                    type: "POST",
                    url: "CompetencyWorksheetIdentification.aspx/StudentForClass",
                    data: "{'Grade':'" + gradeDropdown + "','Subject':'" + subjectDropdown + "','currCourseName':'" + courseDropdown + "'}",

                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert(textStatus + "\n" + errorThrown);
                    },
                    success: function (result) {
                        if (result && result.d) {
                            count = JSON.parse(result.d)[0].totalRow;

                            if (count > 500) {
                                is500 = true;
                                $("#btnContinue").prop('disabled', true).prop('refresh');
                                $("#btnSave").prop('disabled', true).prop('refresh');
                                // return false;
                                alert("A worksheet may not contain more than 500 students. The class or group selected contains more than 500 students. Please reduce the number of students or make a different selection.");
                                return false;
                            }
                        }
                    }
                });
            }

            function Wait() {
                $("#tbldiv").addClass("modal");
                return false;
            };


        </script>
    </telerik:RadCodeBlock>


</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <style>
        .tableDiv {
            display: table;
            width: 92%;
            top: 10px;
        }

        .row {
            display: table-row;
        }

        .cellLabel {
            display: table-cell;
            width: 38%;
            height: 40px;
            text-align: left;
            font-size: 11pt;
            padding-top: 5px;
            padding-left: 5px;
            padding-right: 5px;
            vertical-align: top;
            font-family: 'Arial Rounded MT';
        }

        .modal {
            display: block;
            position: fixed;
            z-index: 1000;
            top: -1px;
            left: -1px;
            right: -1px;
            height: 100%;
            width: auto;
            background: rgba( 255, 255, 255, .8 ) url('../../Styles/Thinkgate_Window/Common/loading.gif') 50% 50% no-repeat;
        }

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

        .radDropdownBtn {
            font-weight: bold;
            font-size: 11pt;
            top: -1px;
            left: 1px;
        }

        .roundButtons {
            color: #00F;
            font-weight: bold;
            font-size: 12pt;
            padding: 2px;
            display: inline;
            position: relative;
            border: solid 1px #000;
            border-bottom: solid 1px #000;
            border-radius: 50px;
            float: right;
            margin-left: 10px;
            cursor: pointer;
            background-color: #FFF;
        }

        .rbPrimaryIcon {
            top: 5px !important;
        }

        #assignToLeft {
            display: inline;
            float: left;
        }

        #assignToRight {
            display: inline;
            float: right;
        }
    </style>
    <div>
        <asp:HiddenField ID="hdnCancelClickCheck" runat="server" />
        <asp:HiddenField ID="hdnWorksheetId" runat="server" />
        <asp:HiddenField ID="hdnType" runat="server" />
        <asp:HiddenField ID="hdnNodeId" runat="server" />
        <div class="tableDiv">

            <div class="row">
                <div class="cellLabel">
                    <b>Grade:<span style="color: red;"> *</span></b>
                </div>
                <div class="cellValue">
                    <telerik:RadComboBox ID="gradeDropdown" Skin="Web20" ClientIDMode="Static" runat="server" HighlightTemplatedItems="true"
                        EmptyMessage="&lt;Select One&gt;" teacherID="" OnClientSelectedIndexChanged="getStandardsDataTableBygrade" level=""
                        AutoPostBack="false" Width="200" CssClass="radDropdownBtn" />
                    <input type="hidden" runat="server" id="initGrade" clientidmode="Static" />
                </div>
            </div>

            <div class="row">
                <div class="cellLabel">
                    <b>Subject:<span style="color: red;"> *</span></b>
                </div>
                <div class="cellValue">
                    <telerik:RadComboBox ID="subjectDropdown" Skin="Web20" ClientIDMode="Static" runat="server" HighlightTemplatedItems="true"
                        EmptyMessage="&lt;Select One&gt;" teacherID="" level="" OnClientSelectedIndexChanged="getStandardsDataTableBySubject" 
                        AutoPostBack="false" Width="200" CssClass="radDropdownBtn" />
                    <input type="hidden" runat="server" id="initSubject" clientidmode="Static" />
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">
                    <b>Course:<span style="color: red;"> *</span></b>
                </div>
                <div class="cellValue">
                    <telerik:RadComboBox ID="courseDropdown" Skin="Web20" ClientIDMode="Static" runat="server" HighlightTemplatedItems="true"
                        EmptyMessage="&lt;Select One&gt;" teacherID="" level="" OnClientSelectedIndexChanged="getStandardsDataTableByCourse"
                        AutoPostBack="false" Width="200" CssClass="radDropdownBtn" />
                    <input type="hidden" runat="server" id="initCourse" clientidmode="Static" />
                </div>
            </div>

            <div class="row">
                <div class="cellLabel">
                    <b>Name:<span style="color: red;"> *</span></b>
                </div>
                <div class="cellValue">
                    <telerik:RadTextBox runat="server" ID="txtName" Width="100%" Height="23px" BorderStyle="Solid" BorderColor="black" BorderWidth="1px" MaxLength="100">
                        <ClientEvents OnValueChanging="ValueChanged" />
                    </telerik:RadTextBox>
                </div>
            </div>
            <div class="row">
                <div class="cellLabel">
                    <b>Description:<span style="color: red;"> *</span></b>
                </div>
                <div class="cellValue">
                    <telerik:RadTextBox runat="server" ID="txtDescription" Width="100%" Height="50px" BorderStyle="Solid" BorderColor="black" BorderWidth="1px" TextMode="MultiLine" MaxLength="200">
                        <ClientEvents OnValueChanging="DescriptionValueChanged" />
                    </telerik:RadTextBox>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="cellLabel">
                    <b>Rubric:<span style="color: red;"> *</span></b>
                </div>
                <div class="cellValue">
                    <telerik:RadComboBox runat="server" ClientIDMode="Static" ID="cmbRubric" Skin="Web20" HighlightTemplatedItems="true" CssClass="radDropdownBtn" AutoPostBack="false" Width="200"
                        EmptyMessage="&lt;Select One&gt;" OnClientSelectedIndexChanged="OnClientSelectedIndexChanged">
                        <%-- <ItemTemplate><span><%# Eval("CmbText")%></span></ItemTemplate>--%>
                    </telerik:RadComboBox>
                    <input type="hidden" runat="server" id="intRubric" clientidmode="Static" />
                </div>
            </div>
            <%--  <div class="row">
                <div class="cellLabel">
                    <b>Assign to:</b>
                </div>
                <div class="cellValue">
                    <table>
                            <tr>
                                <td>
                                    <telerik:RadButton ID="rdbClass" runat="server" Width="60px" Text="Class  " ButtonType="ToggleButton" AutoPostBack="true"
                            ToggleType="Radio" Font-Size="12pt" Skin="Web20" OnCheckedChanged="rdbClass_CheckedChanged" Checked="True" Height="23px">
                        </telerik:RadButton>
                                </td>
                                <td rowspan="2" style="vertical-align:top;">
                    <div id="assignToRightClassName" clientidmode="Static" runat="server">
                        <telerik:RadComboBox runat="server" ID="CmbClassName" Skin="Web20"
                            Text="" CssClass="radDropdownBtn" Width="150" EmptyMessage="Select One"  HighlightTemplatedItems="true" OnClientSelectedIndexChanged="getStandardsDataTableByClass">
                            <ItemTemplate><span><%# Eval("CmbText")%></span></ItemTemplate>
                        </telerik:RadComboBox>
                        <telerik:RadComboBox runat="server" ID="CmbGroupName" Skin="Web20" Visible="false"
                            Text="" CssClass="radDropdownBtn" Width="150" EmptyMessage="Select One"  HighlightTemplatedItems="true" OnClientSelectedIndexChanged="getStandardsDataTableByGroup">
                            <ItemTemplate><span><%# Eval("CmbText")%></span></ItemTemplate>
                        </telerik:RadComboBox>
                    </div>
                                </td>
                            </tr>
                             <tr>
                                <td> <telerik:RadButton ID="rdbGroup" runat="server" Width="65px" Text="Group  " ButtonType="ToggleButton" AutoPostBack="true"
                            ToggleType="Radio" Font-Size="12pt" Skin="Web20" OnCheckedChanged="rdbGroup_CheckedChanged" Height="23px">
                        </telerik:RadButton></td>
                                
                            </tr>
                        <tr><td><br id="hideBR" runat="server"/></td></tr>
                        </table>

                   
                </div>
            </div>--%>
            <br />
            <div class="row">
                <div class="cellLabel"></div>
                <div class="cellValue">
                    <asp:Button runat="server" ID="btnContinue" UseSubmitBehaviour="false" OnClientClick="Wait();return StudentForClass();" ClientIDMode="Static" CssClass="roundButtons" Text="  Continue  " OnClick="btnContinue_Click" />
                    <asp:Button runat="server" ID="btnSave" Visible="false" ClientIDMode="Static" CssClass="roundButtons" UseSubmitBehaviour="false" OnClientClick="Wait();return StudentForClass();" Text="  Save  " OnClick="btnSave_Click" />
                    &nbsp;
                    <asp:Button runat="server" OnClientClick="closeCmsDialogOnCancel();" ID="btnCancel" ClientIDMode="Static" CssClass="roundButtons" Text="  Cancel  " />
                </div>
            </div>
        </div>
    </div>
    <div id="tbldiv"></div>
</asp:Content>

