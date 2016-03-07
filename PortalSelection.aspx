<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PortalSelection.aspx.cs" Inherits="Thinkgate.PortalSelection" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderImageContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FoldersContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            resizeBackground();
        });
        

      

        // Handles the window resizing so the portals will always be in the center.
        $(window).resize(function () {
            resizeBackground();
        });

        function clamp(value, min, max) {
            return Math.min(Math.max(value, min), max);
        };

        function ImageButtonClick(portalId, redirectUrl, schoolid) {
            
            LoginService.UpdateLegacyRole(portalId, schoolid);
            window.location.href = redirectUrl;
            
        }

        function resizeBackground() {
            var bodyDiv = $('#mainBody');

            var headerHeight = $('#headerDiv').height();
            var documentHeight = $(document).height();

            var newBodyHeightHalf = (documentHeight / 2) - headerHeight;

            if (newBodyHeightHalf <= headerHeight) {
                newBodyHeightHalf = headerHeight;
            }

            bodyDiv.css('margin-top', clamp(newBodyHeightHalf - (bodyDiv.height() / 2), 0, Number.MAX_VALUE) + "px");
        }
    </script>
    <div id="mainBody">
        <div runat="server" id="PortalIcons">
        </div>
        <div runat="server" id="DemoPortalIcons" style="display: none;">
            <ul id="mainNavMenu2">
                <li>
                    <asp:ImageButton ID="stateImgBtn" runat="server" ImageUrl="~/Images/new/state.png" OnClick="stateImgBtn_Click" />
                    <br />
                    <a style="color: black;">State</a> </li>
                <li>
                    <asp:ImageButton ID="districtImgBtn" runat="server" ImageUrl="~/Images/new/district_alt.png"
                        OnClick="districtImgBtn_Click" />
                    <br />
                    <a style="color: black;">District</a> </li>
                <li>
                    <asp:ImageButton ID="schoolImgBtn" runat="server" ImageUrl="~/Images/new/school.png"
                        OnClick="schoolImgBtn_Click" />
                    <br />
                    <a style="color: black;">School</a> </li>
                <li>
                    <asp:ImageButton ID="teacherImgBtn" runat="server" ImageUrl="~/Images/new/male_teacher.png"
                        OnClick="teacherImgBtn_Click" />
                    <br />
                    <a style="color: black;">Teacher</a> </li>
                <li>
                    <asp:ImageButton ID="classImgBtn" runat="server" ImageUrl="~/Images/new/class.png"
                        OnClick="classImgBtn_Click" />
                    <br />
                    <a style="color: black;">Class</a> </li>
                <li>
                    <asp:ImageButton ID="studentImgBtn" runat="server" ImageUrl="~/Images/new/female_student.png"
                        OnClick="studentImgBtn_Click" />
                    <br />
                    <a style="color: black;">Student</a> </li>
                <li>
                    <asp:ImageButton ID="btnIMC" runat="server" ImageUrl="~/Images/new/female_student.png"
                        OnClick="btnLCO_Click" />
                    <br />
                    <a style="color: black;">IMC</a> </li>
                <li>
                    <asp:ImageButton ID="btnRegional" runat="server" ImageUrl="~/Images/new/female_student.png"
                        OnClick="btnLCO_Click" />
                    <br />
                    <a style="color: black;">Regional Coordinator</a> </li>
                <li>
                    <asp:ImageButton ID="btnSection" runat="server" ImageUrl="~/Images/new/female_student.png"
                        OnClick="btnLCO_Click" />
                    <br />
                    <a style="color: black;">Section Chief</a> </li>
                 <li>
                    <asp:ImageButton ID="btnLCOAdmin" runat="server" ImageUrl="~/Images/new/female_student.png"
                        OnClick="btnLCO_Click" />
                    <br />
                    <a style="color: black;">LCO Admin</a> </li>
            </ul>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageFooter" runat="server">
</asp:Content>
