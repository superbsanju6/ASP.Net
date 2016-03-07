<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomErr401403.aspx.cs" Inherits="Thinkgate.ErrorPages.CustomErr401403" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/Error.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main">
           <div class="mainMiddle">
               <div class="tileHeader"></div>
               <div class="tileMain">
                   <div class="tileContent">
                       <img class="tgMiddle2" src="../Images/ErrorImage/error_image_noauth.png" />
                    </div>
               </div>
           </div>
        </div>
</asp:Content>
