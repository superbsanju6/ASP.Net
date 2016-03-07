 <%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffIdentification.ascx.cs"
		Inherits="Thinkgate.Controls.Staff.StaffIdentification" %>

<style type="text/css">
	.bottomTextButton
	{
		position: relative;
		float: right;
		top: 0px;  
		left: 0px;
		height: 16px;
		cursor: pointer;
		background-repeat: no-repeat;
		background-position: left center;
		text-align: right;
		padding-left: 20px;
		padding-right: 10px;
	}    
	
	.fieldLabel 
	{
	    vertical-align: top;       
	}

    .overflowFix
    {
        overflow:inherit !important;
        min-height:100% !important;
    }

    .fieldValueTable {       
        width:310px;      
        height:210px;
    }
    
    .fieldValueTable td
    {
        padding:1px;
    }
   

</style>

<div>

	<table class="fieldValueTable" >
		<tr>
				<td class="fieldLabel">
						Name:
				</td>
				<td class="fieldLabel">
						<asp:Label runat="server" ID="lblName" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						User ID:
				</td>
				<td class="fieldLabel">
						<asp:Label runat="server" ID="lblUserID" />
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						School:
				</td>
				<td class="fieldLabel">
					<div style="overflow-y:auto !important;display:block !important;min-height:10px;height:auto !important; max-height:100px !important; max-width: 250px;">
                        <asp:Label runat="server" ID="lblSchool"/></div>
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						User Type:
				</td>
				<td class="fieldLabel">
                    <div style="overflow-y:auto !important;display:block !important;min-height:10px;height:auto !important; max-height:50px !important; max-width: 250px;">
						<asp:Label runat="server" ID="lblUserType" />
                        </div>
				</td>
		</tr>
		<tr>
				<td class="fieldLabel">
						Email:
				</td>
				<td class="fieldLabel">
						<a href="#" id="anchorEmail" runat="server"><asp:Label runat="server" ID="lblEmail" /></a>
				</td>
		</tr>
	</table>
    </div>
	<div runat="server" id="btnResetPassword" clientidmode="Static" class="bottomTextButton" style="background-image: url(../Images/Gears.png);" title="Reset Password" >Reset Password</div>

<script type="text/javascript">
    $("#ctl00_MainContent_ctlDoublePanel_rotator1_container1_tileContainerDiv1_C").addClass('overflowFix');
</script>