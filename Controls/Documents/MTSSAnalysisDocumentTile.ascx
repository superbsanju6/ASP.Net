<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MTSSAnalysisDocumentTile.ascx.cs" Inherits="Thinkgate.Controls.Documents.MTSSDiagnosticsDocumentTile" %>

<telerik:RadAjaxLoadingPanel ID="MTSSAnalysisRadAjaxLoadingPanel" runat="server" />

<script type="text/javascript">
    function showForms(wnd, hl, fn) {
        var hyperlinks = hl.split(",");
        var fname = fn.split(",");
        $(".divStudentLinksDialog").empty();
        $(".divStudentLinksDialog").append('Select the name hyperlink to view the form.</br>');
        $.each(hyperlinks, function (i, val) {
            $(".divStudentLinksDialog").append('</br> <a href="' + val + '" target="_blank">' + fname[i] + ' </a> </br>');
        });
        wnd.show();
    }

</script>

<telerik:RadWindowManager ID="wndWindowManager" runat="server">
    <Windows>
        <telerik:RadWindow runat="server" ID="wndAddDocument" 
            Title="Student Forms" 
            ShowContentDuringLoad="False"
            Behavior="Close" 
            ReloadOnShow="True" 
            Modal="True" 
            Skin="Web20" 
            VisibleStatusbar="False" 
            AutoSize="False" 
            AutoSizeBehaviors="Default"
            DestroyOnClose="true"
            Height="300px"
            Width="300px">
            <ContentTemplate>
                <div runat="server" id="links" class="divStudentLinksDialog" visible="True" style="margin-left: auto; margin-right: auto;font-size: 10pt; height:100%; width:100%" >
                    Select the name hyperlink to view the form. <br />
                </div>
        </ContentTemplate>
    </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>

<telerik:RadAjaxPanel ID="MTSSAnalysisAjaxPanel" runat="server" LoadingPanelID="MTSSAnalysisRadAjaxLoadingPanel" Height="209px">
    <!--Filters-->
    <div>
        <telerik:RadComboBox ID="MTSSAnalysiscmbSchool" 
            runat="server"
            ToolTip="School"
            Skin="Web20"
            Width="67"
            CausesValidation="False"
            HighlightTemplatedItems="true"
            AutoPostBack="True"
            DropDownWidth="300px"
            AppendDataBoundItems="true"
            OnSelectedIndexChanged="cmbSchool_SelectedIndexChanged">
            <Items>
                <telerik:RadComboBoxItem Text="School" />
            </Items>
            <ItemTemplate>
                <span>
                    <%# Eval("Name")%>
                </span>
            </ItemTemplate>
        </telerik:RadComboBox>

        <telerik:RadComboBox ID="MTSSAnalysiscmbGrade" runat="server"
            ToolTip="Select a grade"
            Skin="Web20"
            Width="67"
            AutoPostBack="true"
            CausesValidation="False"
            OnSelectedIndexChanged="cmbGrade_SelectedIndexChanged"
            HighlightTemplatedItems="true">
            <ItemTemplate>
                <span>
                    <%# Eval("Grade") %>
                </span>
            </ItemTemplate>
        </telerik:RadComboBox>

        <telerik:RadComboBox ID="MTSSAnalysiscmbSubject" runat="server"
            ToolTip="Select a subject"
            Skin="Web20"
            Width="67"
            AutoPostBack="true"
            CausesValidation="False"
            OnSelectedIndexChanged="cmbSubject_SelectedIndexChanged"
            HighlightTemplatedItems="true"
            DropDownWidth="200px">
            <ItemTemplate>
                <span>
                    <%# Eval("Subject") %>
                </span>
            </ItemTemplate>
        </telerik:RadComboBox>

        <telerik:RadComboBox ID="MTSSAnalysiscmbStudents" runat="server"
        ToolTip="Select a course"
        Skin="Web20"
        Width="67"
        AutoPostBack="true"
        CausesValidation="False"
        OnSelectedIndexChanged="cmbStudents_SelectedIndexChanged"
        HighlightTemplatedItems="true"
        DropDownWidth="200px">
        <ItemTemplate>
            <span>
                <%# Eval("StudentName") %>
            </span>
        </ItemTemplate>
    </telerik:RadComboBox>

    <div style="float:right">
       <asp:ImageButton runat="server" ID="MTSSAnalysisRefreshTile" OnClick="refreshTile" ImageUrl="~/Images/refresh.png" Height="23px" Width="23px"/>
    </div>
 </div>
        <!--Pages-->
    <telerik:RadMultiPage runat="server" ID="MTSSAnalysisRadMultiPage" SelectedIndex="0" Height="96%" Width="100%"  CssClass="multiPage">  
         <telerik:RadPageView runat="server" ID="MTSSAnalysisActive" BackColor="#aeaeac">
            <div class="" style="height: auto; padding: 0px;">
                <asp:Panel ID="Panel1" runat="server" Visible="false" Height="182px" Width="97%" Style="margin-left:10px;margin-right:10px">
                    <div style="width: 100%; text-align: center;">
                        No results found.
                    </div>
                </asp:Panel>
                <div class="" style="height: auto; padding: 0px;">
                    <asp:Panel runat="server" ID="Panel2" ScrollBars="Auto" Height="182px" Width="97%" Style="margin-left:10px;margin-right:10px">
                        <asp:PlaceHolder runat="server" ID="AcademicPlaceholder"></asp:PlaceHolder>
                    </asp:Panel>
                </div>
           </div>
           </telerik:RadPageView>
         <telerik:RadPageView runat="server" ID="MTSSAnalysisBehavioral" BackColor="#aeaeac">
           <div class="" style="height: auto; padding: 0px;">
                <asp:Panel runat="server" ID="Panel4" ScrollBars="Auto" Height="182px" Width="97%" Style="margin-left:10px;margin-right:10px">
                    <asp:PlaceHolder runat="server" ID="BehavioralPlaceholder"></asp:PlaceHolder>
                </asp:Panel>
         </div>
     </telerik:RadPageView>
         <telerik:RadPageView runat="server" ID="MTSSAnalysisInactive" BackColor="#aeaeac">
           <div class="" style="height: auto; padding: 0px;">
                <asp:Panel runat="server" ID="Panel3" ScrollBars="Auto" Height="182px" Width="97%" Style="margin-left:10px;margin-right:10px">
                    <asp:PlaceHolder runat="server" ID="InactivePlaceholder"></asp:PlaceHolder>
                </asp:Panel>
         </div>
     </telerik:RadPageView>
  </telerik:RadMultiPage>
    </telerik:RadAjaxPanel>

        <telerik:RadTabStrip runat="server" ID="MTSSAnalysisRadTabStrip" Orientation="HorizontalBottom" SelectedIndex="0" MultiPageID="MTSSAnalysisRadMultiPage" 
                             Skin="Thinkgate_Blue" EnableEmbeddedSkins="False" Style="float: left">
            <Tabs>
            <telerik:RadTab Text="Academic" runat="server" ID="MTSSAnalysisAcademicView" Font-Size="X-Small" Visible="false"></telerik:RadTab>
            <telerik:RadTab Text="Behavioral" runat="server" ID="MTSSAnalysisBehavioralView" Font-Size="X-Small" Visible="false"></telerik:RadTab>
            <telerik:RadTab Text="Inactive" runat="server" ID="MTSSAnalysisInactiveView" Font-Size="X-Small" Visible="false"></telerik:RadTab>
             </Tabs>
        </telerik:RadTabStrip>

    <div class="tabsAndButtonsWrapper_smallTile">
        <div runat="server" id="btnAdd" visible="true" class="searchTileBtnDiv_smallTile" title="Add New" style="margin-top: 1px;float:right;" >
            <span class="searchTileBtnDiv_smallTile_icon searchTileBtnDiv_smallTile_addIcon"></span>
            <div style="padding: 0;">Add New</div>
        </div>
    </div>



