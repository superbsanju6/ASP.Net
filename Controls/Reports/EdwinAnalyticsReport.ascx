<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EdwinAnalyticsReport.ascx.cs" Inherits="Thinkgate.Controls.Reports.EdwinAnalyticsReport" %>
<%@ Import Namespace="Telerik.Web.UI" %>

<% if (Links != null && Links.Count > 0) { %>
    <style type="text/css">
        .reportLink > div
        {
            padding: 3px;
        }
    
        .reportLink > div > a
        {        
            font-size: 14px;
        }
    </style>
    <div id="tileContainerEdwinAnalytics" style="text-align: left; padding-top: 15px;height:85%;overflow-y:scroll;" class="reportLink" runat="server">

                <% foreach (var link in Links)
                   { %>
                    <div>
                        <a target="_blank" href='<%= link.LinkUrl %>'><%= link.LinkName %></a>
                    </div>
                <% } %>
    </div>
<% } %>
