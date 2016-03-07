using System;
using System.Collections.Generic;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Reports
{
	public partial class CompetencyTrackingPortalPage : RecordPage
	{
		#region Properties

		public SearchParms SearchParms;
		public Criteria ReportCriteria;
		private string _guid;

		#endregion

		#region Page Events

		protected new void Page_Init(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				var siteMaster = this.Master as SiteMaster;
				if (siteMaster != null)
				{
					siteMaster.BannerType = BannerType.ObjectScreen;
				}
			}			
			
			base.Page_Init(sender, e);
			SetupFolders();
			InitPage(ctlFolders, ctlDoublePanel, sender, e);

		}

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadPageTitle();

			if (!string.IsNullOrEmpty(hiddenTxtBox.Text))
			{
				var guid = hiddenTxtBox.Text;
			}
			else
			{
				LoadSearchCriteria();
			}

			if (!IsPostBack)
			{
			}

			LoadDefaultFolderTiles();
		}

		protected void Page_LoadComplete()
		{
			var siteMaster = this.Master as SiteMaster;
			if (siteMaster != null)
			{
				siteMaster.BannerControl.HideContextMenu(Classes.Banner.ContextMenu.Actions);
			}
		}

		private void LoadPageTitle()
		{
			pageTitleLabel.Text = "Competency Tracking Report Portal";
		}

		#endregion

		#region Folder Methods

		private void SetupFolders()
		{
			Folders = new List<Folder>
			{
				new Folder("Competency Tracking Report", "~/Images/new/folder_data_analysis.png", LoadCompetencyTrackingReport, "~/ContainerControls/TileReportContainer_1_1.ascx", 1)
			};

			ctlFolders.BindFolderList(Folders);
		}

		#endregion

		#region Tile Methods

		private void LoadReportTiles(string tilePath, TileParms tileParms = null, string reportName = "Results")
		{
			if (tileParms == null) tileParms = new TileParms();

			tileParms.AddParm("CriteriaGUID", hiddenTxtBox.Text);
			//tileParms.AddParm("multiTestIDs", _multiTestIDs);

			Rotator1Tiles.Add(new Tile(reportName, tilePath, false, tileParms));
		}

		private void LoadCompetencyTrackingReport()
		{
			var tileParms = new TileParms();
			tileParms.AddParm("guid", _guid);
			LoadReportTiles("~/Controls/Reports/CompetencyTrackingReport.ascx", tileParms, "Competency Tracking Report");
		}
		#endregion

		private void LoadSearchCriteria()
		{
			_guid = Guid.NewGuid().ToString();
			hiddenTxtBox.Text = _guid;
			
		}
	}
}