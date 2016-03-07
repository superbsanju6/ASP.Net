using System;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;

namespace Thinkgate
{
	public partial class SecurityTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			SessionObject sessionObject = (SessionObject)Session["SessionObject"];
            ThinkgateUser user = sessionObject.LoggedInUser;
			/*user.HasPermission("foo");
			user.HasPermissionAtLevel("foo", ThinkgatePermission.PermissionLevelValues.Read);
			user.HasPermissionAtOrAboveLevel("foo", ThinkgatePermission.PermissionLevelValues.Read);*/

		}
	}
}