using System;
using Telerik.Web.UI;
namespace Thinkgate.Classes
{
		public delegate void DockClickMethod(TileParms tileParms);

		[Serializable]
		public class Tile
		{
				public string ID {get; set;}
				public string Title { get; set; }
				public string ControlPath { get; set; }
				public string ExpandedControlPath { get; set; }
				public string EditControlPath { get; set; }
                public string PerformanceLevelControlPath { get; set; }

                public string PerformanceLevelJSFunctionOverride { get; set; }
				public bool ToggleView { get; set; }
				public string ToggleDefault { get; set; }
				public string ExpandJSFunctionOverride { get; set; }
				public string EditJSFunctionOverride { get; set; }
				public string HelpJSFunction { get; set; }
                public string ExportToExcelJSFunctionOverride { get; set; }
                public string ReportControlPath { get; set; }
                public string ReportJSFunctionOverride { get; set; }
				public Thinkgate.Base.Enums.Permission? TilePermission { get; set; }

				public TileParms TileParms;
				public DockClickMethod DockClickMethod; // Click of anywhere in dock?
				public Container ParentContainer;
				public RadDock ParentDock;

				public bool IsIteration { get; set; }

				public Tile(string title, string controlPath, bool isIteration = false,
										TileParms tileParms = null, DockClickMethod dockClickMethod = null, string expandedControlPath = "",
                                        string editControlPath = "", bool toggleView = false, string toggleDefault = "list", string expandJSFunctionOverride = "",
                                        string editJSFunctionOverride = "", string helpJSFunction = "", string exportToExcelJSFunctionOverride = "", string reportControlPath = "", string reportJSFunctionOverride = "")
				{
						Title = title;
						ControlPath = controlPath;            
						IsIteration = isIteration;
						TileParms = tileParms;
						DockClickMethod = dockClickMethod;
						ExpandedControlPath = expandedControlPath;
						EditControlPath = editControlPath;
                       
						ToggleView = toggleView;
						ToggleDefault = toggleDefault;
						ExpandJSFunctionOverride = expandJSFunctionOverride;
						EditJSFunctionOverride = editJSFunctionOverride;
						HelpJSFunction = helpJSFunction;
                        ExportToExcelJSFunctionOverride = exportToExcelJSFunctionOverride;
                        ReportControlPath = reportControlPath;
                        ReportJSFunctionOverride = reportJSFunctionOverride;
				}

                public Tile(Thinkgate.Base.Enums.Permission tilePermission, string title, string controlPath, bool isIteration = false,
                        TileParms tileParms = null, DockClickMethod dockClickMethod = null, string expandedControlPath = "",
                        string editControlPath = "", bool toggleView = false, string toggleDefault = "list", string expandJSFunctionOverride = "",
                        string editJSFunctionOverride = "", string helpJSFunction = "", string exportToExcelJSFunctionOverride = "", string reportControlPath = "", string reportJSFunctionOverride = ""
                    , string performanceLevelControlPath = "", string performanceLevelJSFunctionOverride = ""
                    )
                {
                    TilePermission = tilePermission;
                    Title = title;
                    ControlPath = controlPath;
                    IsIteration = isIteration;
                    TileParms = tileParms;
                    DockClickMethod = dockClickMethod;
                    ExpandedControlPath = expandedControlPath;
                    EditControlPath = editControlPath;
                    ToggleView = toggleView;
                    ToggleDefault = toggleDefault;
                    ExpandJSFunctionOverride = expandJSFunctionOverride;
                    EditJSFunctionOverride = editJSFunctionOverride;
                    HelpJSFunction = helpJSFunction;
                    ExportToExcelJSFunctionOverride = exportToExcelJSFunctionOverride;
                    ReportControlPath = reportControlPath;
                    ReportJSFunctionOverride = reportJSFunctionOverride;
                    PerformanceLevelControlPath = performanceLevelControlPath;
                    PerformanceLevelJSFunctionOverride = performanceLevelJSFunctionOverride;
                }

                

				public string GetXML()
				{
						/*
						string xml = @"<Tile ID='" + ID + "'>"
											 + @"<Title>" + Title + "</Title>"
											 + @"<ControlPath>" + ControlPath + "</ControlPath>"
											 + @"<ExpandedControlPath>" + ControlPath + "</ExpandedControlPath>"
											 + @"<EditControlPath>" + ControlPath + "</EditControlPath>"
											 + TileParms.GetXML()
											 + @"</Tile>";
								
						return xml;
						 */
						return string.Empty;
				}
		}
}
