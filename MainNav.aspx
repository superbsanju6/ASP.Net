<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MainNav.aspx.cs" Inherits="Thinkgate.MainNav" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderImageContent" runat="server">
    <div style="height: 90px;">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageTitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FoldersContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        /*window.onresize = setTimeout('resizeBackgrounds();', 200); */

        $(document).ready = setTimeout('resizeBackground();', 300);
        function resizeBackground() {
            //fullscreenBackground('bgimg');

            var bodyDiv = document.getElementById('mainBody');
            var listNav = document.getElementById('mainNavMenu');

            if (bodyDiv == null) return false;
            if (listNav == null) return false;
            bodyDiv.style.height = (document.documentElement.clientHeight - 120) + "px";
            listNav.style.marginTop = (document.documentElement.clientHeight - 400) / 2 + "px";
        }

    </script>
    <div id="mainBody">
        <div>
            <ul id="mainNavMenu">
                <li>
                    <asp:Image ID="stateImgBtn" runat="server" ImageUrl="~/Images/new/state.png"
                        OnClick="districtImgBtn_Click" />
                    <br />
                    <a style="color: white;">State</a> </li>
                <li>
                    <asp:ImageButton ID="districtImgBtn" runat="server" ImageUrl="~/Images/new/district_alt.png"
                        OnClick="districtImgBtn_Click" />
                    <br />
                    <a style="color: white;">District</a> </li>
                <li>
                    <asp:ImageButton ID="schoolImgBtn" runat="server" ImageUrl="~/Images/new/school.png"
                        OnClick="schoolImgBtn_Click" />
                    <br />
                    <a style="color: white;">School</a> </li>
                <li>
                    <asp:ImageButton ID="teacherImgBtn" runat="server" ImageUrl="~/Images/new/male_teacher.png"
                        OnClick="teacherImgBtn_Click" />
                    <br />
                    <a style="color: white;">Teacher</a> </li>
                <li>
                    <asp:ImageButton ID="classImgBtn" runat="server" ImageUrl="~/Images/new/class.png"
                        OnClick="classImgBtn_Click" />
                    <br />
                    <a style="color: white;">Class</a> </li>
                <li>
                    <asp:ImageButton ID="studentImgBtn" runat="server" ImageUrl="~/Images/new/female_student.png"
                        OnClick="studentImgBtn_Click" />
                    <br />
                    <a style="color: white;">Student</a> </li>
                    
            </ul>
            
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageFooter" runat="server">
</asp:Content>
