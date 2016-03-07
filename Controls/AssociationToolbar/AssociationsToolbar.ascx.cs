using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

/* Note to control users:
 * This toolbar control was designed to be created in the codebehind file of its parent. Here is sample code:
 	protected void Page_Load(object sender, EventArgs e)
	{
		AssociationInfo[] assoc = new AssociationInfo[] { 
							new AssociationInfo() { Text = "Schedules", NumberOfAssociations = 1, EditorPath = "PopupForm.aspx" },
							new AssociationInfo() { Text = "Standards", NumberOfAssociations = 2, EditorPath = "PopupForm.aspx" },
							new AssociationInfo() { Text = "Curricula", NumberOfAssociations = 0, EditorPath = "PopupForm.aspx" },
							new AssociationInfo() { Text = "Docs & Resources", NumberOfAssociations = 10, EditorPath = "PopupForm.aspx" }
						};

		AssociationsToolbar tb1 = (AssociationsToolbar)LoadControl("AssociationsToolbar.ascx");
		tb1.AssocInfo = assoc;
		tb1.DocumentId = "1234";
		tb1.DocumentType = "InstructionalPlan";
		tb1.EncryptedUserId = "UE1ncWxHRW5xaHg2eWRLa2x5UHhPdz09";
		ToolbarPlaceholder1.Controls.Add(tb1);

		AssociationsToolbar tb2 = (AssociationsToolbar)LoadControl("AssociationsToolbar.ascx");
		tb2.AssocInfo = assoc;
		tb2.DocumentId = "5678";
		tb2.DocumentType = "UnitPlan";
		tb2.EncryptedUserId = "UE1ncWxHRW5xaHg2eWRLa2x5UHhPdz09";
		ToolbarPlaceholder2.Controls.Add(tb2);
	}

 * In this example the toolbars are inserted into placeholder controls like this:
 			<asp:PlaceHolder runat="server" ID="ToolbarPlaceholder1" />

*/

public struct AssociationInfo
	{
		// Text to appear on the button.
		public String Text;
		// Number of associations to show on the icon.
		public Int32 NumberOfAssociations;
		// The relative uri path of the editor control associated with the button.
		public String EditorPath;
	}

	public partial class AssociationsToolbar : System.Web.UI.UserControl
	{
		#region Private members
		// Button specific info for each toolbar button.
		private AssociationInfo[] assocInfo = null;
		// General toolbar information.
		private String documentType, documentId, encryptedUserId;
		#endregion Private members

		/// <summary>
		/// Default constructor required.
		/// </summary>
		public AssociationsToolbar()
		{
		}
		 
		protected void Page_Load(object sender, EventArgs e)
		{
			// Create the buttons if initial load.
			if(!IsPostBack)
				CreateButtons();
		}

		private void CreateButtons()
		{
			// Can't create buttons if we don't have information for them.
			if(AssocInfo == null)
				return;

			// Create each button.
			for (Int32 i = 0; i < AssocInfo.Length; i++)
			{
				AssociationInfo info = AssocInfo[i];
				// Our button is just a RadMenuItem.
				RadMenuItem mi = new RadMenuItem();
				mi.Text = info.Text;
				// Create a uri to show in the popup window. We pass the userID, document type, and document id.
				mi.Value = String.Format(@"{0}?xID={1}&docType={2}&docID={3}", info.EditorPath, EncryptedUserId, DocumentType, DocumentId);
				// The number badge.
				mi.ImageUrl = String.Format(@"../Images/AssociationToolbar/Badge{0}.png", (info.NumberOfAssociations == 0) ? "Blank" : (info.NumberOfAssociations > 9) ? "Gt9" : info.NumberOfAssociations.ToString());
				AssociationsRadMenu.Items.Add(mi);
			}
		}

		#region Properties
		public AssociationInfo[] AssocInfo
		{
			get { return this.assocInfo; }
			set { this.assocInfo = value; }
		}

		public String DocumentType
		{
			get { return documentType; }
			set { documentType = value; }
		}

		public String DocumentId
		{
			get { return documentId; }
			set { documentId = value; }
		}

		public String EncryptedUserId
		{
			get { return encryptedUserId; }
			set { encryptedUserId = value; }
		}
		#endregion Properties

	}

