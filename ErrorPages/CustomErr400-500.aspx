<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomErr400-500.aspx.cs" Inherits="Thinkgate.ErrorPages.CustomErr400_500" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Error.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main">
        <div class="mainMiddle">
            <div class="tileHeader">
            </div>
            <div class="tileMain">
                <div class="tileContent">
                    <img class="tgMiddle" src="../Images/ErrorImage/error_image_400_500.png" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>