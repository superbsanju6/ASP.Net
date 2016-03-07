using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Thinkgate.Classes
{
		public class Container : UserControl
		{
				public SessionObject SessionObject { get; set; }
				public string ContainerID { get; set; }
				public string RadDockZoneID { get; set; }
				public int NumberOfTilesPerPage { get; set; }
				public event EventHandler OnTileSort;


				public Container()
				{
						if (Context.Session["SessionObject"] == null) return;
						SessionObject = (SessionObject)Context.Session["SessionObject"];
				}

				public Container(string containerID, int numberOfTilesPerPage)
				{
						if (Context.Session["SessionObject"] == null) return;
						SessionObject = (SessionObject)Context.Session["SessionObject"];

						ContainerID = containerID;
						NumberOfTilesPerPage = numberOfTilesPerPage;
				}

				protected void DockPositionChanged(object sender, DockLayoutEventArgs e)
				{
						if (OnTileSort != null)
						{
								OnTileSort(sender, e);
						}
				}

				public void BuildDockZone()
				{
						RadDockZoneID = "radDockZone" + ContainerID;
						var zone = FindControl("RadDockZone1");
						if (zone == null) return;
						zone.ID = RadDockZoneID;

						for (var i = 0; i < NumberOfTilesPerPage; i++)
						{
								var dock = FindControl("tileContainerDiv" + (i + 1));
								if (dock == null) continue;
								((RadDock)dock).AllowedZones = new string[] { RadDockZoneID };
						}
				}


				public List<Tile> GetTiles()
				{
						var tiles = new List<Tile>();
						var layout = (RadDockLayout)FindControl("radDockLayout");

						if (layout == null)
						{
								return null;
						}

						var dockStates = layout.GetRegisteredDocksState();

						dockStates.Sort(DockStateIndex);

						foreach (DockState ds in dockStates)
						{
								var dock = (RadDock)FindControl(ds.UniqueName);
								if (dock.ContentContainer.Controls.Count > 0)
								{
										var ctlTile = (TileControlBase)dock.ContentContainer.Controls[0];
										tiles.Add(ctlTile.Tile);
								}
						}

						return tiles;
				}

				private static int DockStateIndex(DockState s, DockState t)
				{
						return s.Index.CompareTo(t.Index);
				}

				public void LoadControlsToContent(List<Tile> tiles)
				{
						if (tiles == null) return;

						var counter = 1;
						
						foreach (Tile t in tiles)
						{
								try
								{
                                    //REFACTOR : WSH - investigate removing this permission check now that we've added check to RecordPage to prevent chunking
										if (t.TilePermission != null)
										{
												var permission = t.TilePermission ?? Thinkgate.Base.Enums.Permission.HasAccess;

												if (SessionObject == null && Session["SessionObject"] != null) SessionObject = (SessionObject)Session["SessionObject"];
												var isTrue = SessionObject.LoggedInUser.HasPermission(permission);

												if (!isTrue) continue;
										}

                    var widget = (TileControlBase) LoadControl(t.ControlPath);

										t.ParentContainer = this;

										//Control must inherit from TileControlBase in order to have Tile property
										//((TileControlBase)widget).Tile = t;
								        widget.Tile = t;

										var destination = FindControl("tileContainerDiv" + counter);

										if (destination == null) continue;

										RadDock dock = (RadDock)destination;

										if (t.Title.Contains("@@dockID"))
										{
												t.Title = t.Title.Replace("@@dockID", dock.ClientID);
										}

										dock.ContentContainer.Controls.Clear();
										dock.ContentContainer.Controls.Add(widget);

										var titlePanel = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
										titlePanel.Attributes["class"] = "tileTitleRow";
										titlePanel.InnerHtml = t.Title;
										dock.TitlebarContainer.Controls.Add(titlePanel);

										var extraCommandPanel = new System.Web.UI.WebControls.Panel();
										extraCommandPanel.ID = "extraCommandPanel";
										extraCommandPanel.CssClass = "extraCommandContainer";

										if (t.ToggleView || !String.IsNullOrEmpty(t.HelpJSFunction))
										{
												var extraCommandButtonPanel = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
												extraCommandButtonPanel.ID = "extraCommandButtonPanel";

												var tileGuid = Guid.NewGuid().ToString().Replace("-", string.Empty);

												if (t.ToggleView)
												{
													ImageButton extraCommandButton_ListViewToggle = new ImageButton()
													{
															ImageUrl = "~/Images/list_view.png"
													};

													string listViewImgURL = extraCommandButton_ListViewToggle.ResolveClientUrl(extraCommandButton_ListViewToggle.ImageUrl);
													string listViewClass = t.ToggleDefault == "list" ? "toggleViewButtons_selected" : "toggleViewButtons";
											
													ImageButton extraCommandButton_GraphicalViewToggle = new ImageButton
													{
															ImageUrl = "~/Images/graphical_view.png"
													};

													string graphicalViewImgURL = extraCommandButton_GraphicalViewToggle.ResolveClientUrl(extraCommandButton_GraphicalViewToggle.ImageUrl);
													string graphicalViewClass = t.ToggleDefault == "graphical" ? "toggleViewButtons_selected" : "toggleViewButtons";

													var listViewImgButton = new Image()
													{
															ID = destination.ID + "_ListView" + tileGuid,
															ImageUrl = listViewImgURL,
															AlternateText = "List View",
															ToolTip = "List View",
															CssClass = listViewClass,
															ClientIDMode = System.Web.UI.ClientIDMode.Static
													};

													var graphViewImgButton = new Image()
													{
															ID = destination.ID + "_GraphicalView" + tileGuid,
															ImageUrl = graphicalViewImgURL,
															AlternateText = "Graphical View",
															ToolTip = "Graphical View",
															CssClass = graphicalViewClass,
															ClientIDMode = System.Web.UI.ClientIDMode.Static
													};

													listViewImgButton.Attributes.Add("OnClick", "toggleView_SmallTile2(this, '"
																														+ destination.ID + "', 'listView','" + graphViewImgButton.ClientID + "','" + listViewImgButton.ClientID + "')");
													graphViewImgButton.Style.Add(HtmlTextWriterStyle.Display, "none");
													graphViewImgButton.Attributes.Add("OnClick", "toggleView_SmallTile2(this, '"
																														+ destination.ID + "', 'graphicalView','" + listViewImgButton.ClientID + "','" + graphViewImgButton.ClientID + "')");   


													extraCommandButtonPanel.Controls.Add(listViewImgButton);
													extraCommandButtonPanel.Controls.Add(graphViewImgButton);

												//string toggleButtons = "<img src=\"" + listViewImgURL + "\" alt=\"List View\" title='Select List View' class=\""
												//        + listViewClass + "\" onclick=\"toggleView_SmallTile(this, '"
												//        + destination.ID + "', 'listView');\" />"
												//        + "<img src=\"" + graphicalViewImgURL + "\" alt=\"Graphical View\" title='Select Graphical View' class=\""
												//        + graphicalViewClass + "\" onclick=\"toggleView_SmallTile(this, '"
												//        + destination.ID + "', 'graphicalView');\" />";

												//extraCommandButtonPanel.InnerHtml = toggleButtons;
												}
												// Some tiles have a help icon.
												else
												{
													ImageButton extraCommandButton_Help = new ImageButton()	{	ImageUrl = "~/Images/help_small.png" };
													string helpImgURL = extraCommandButton_Help.ResolveClientUrl(extraCommandButton_Help.ImageUrl);
													Image helpImgButton = new Image()
													{
														ID = destination.ID + "_Help" + tileGuid,
														ImageUrl = helpImgURL,
														AlternateText = "Help",
														CssClass = "tileHelpButton",
														ClientIDMode = System.Web.UI.ClientIDMode.Static
													};
													helpImgButton.Attributes.Add("OnClick", t.HelpJSFunction + "()");
													extraCommandButtonPanel.Controls.Add(helpImgButton);
												}
												extraCommandPanel.Controls.Add(extraCommandButtonPanel);
										}
										dock.TitlebarContainer.Controls.Add(extraCommandPanel);

										dock.Command += RadDock_Command;
										t.ParentDock = dock;

										//BJC: Adding new class to the title bar in order to set the height to 50 pixels and create two rows.
										dock.TitlebarContainer.CssClass = "rdTitleBarTwoRows";

										if (t.DockClickMethod != null) dock.Commands.Add(GetDockCommand("Select Class", "changeCommand"));
										if (!String.IsNullOrEmpty(t.ExpandedControlPath)) dock.Commands.Add(GetDockCommand("Expand", "expandCommand", t.ExpandedControlPath, t.ExpandJSFunctionOverride));
										if (!String.IsNullOrEmpty(t.EditControlPath)) dock.Commands.Add(GetDockCommand("Edit", "editCommand", t.EditControlPath, t.EditJSFunctionOverride));
                                        if (!String.IsNullOrEmpty(t.PerformanceLevelControlPath)) dock.Commands.Add(GetDockCommand("PerformanceLevel", "performanceLevel", t.PerformanceLevelControlPath, t.PerformanceLevelJSFunctionOverride));
                                        if (!String.IsNullOrEmpty(t.ExportToExcelJSFunctionOverride)) dock.Commands.Add(GetJSDockCommand("Export to Excel", "exportToExcelCommand", string.Empty, t.ExportToExcelJSFunctionOverride));
                                        if (!String.IsNullOrEmpty(t.ReportJSFunctionOverride)) dock.Commands.Add(GetDockCommand("Assessment Report Card", "reportCommand", t.ReportControlPath, t.ReportJSFunctionOverride));
										foreach (var command in widget.AdditionalCommands())
										{
											dock.Commands.Add(command);
										}

										if (SessionObject.TileClicked != null
														&& SessionObject.Elements_ActiveFolder.Equals(SessionObject.LastElementsFolder_TileClicked))
										{
												if (SessionObject.TileClicked.IndexOf(dock.ClientID) > -1 && dock.ClientID.IndexOf("rotator1") > -1)
												{
														//dock.TitlebarContainer.CssClass = dock.TitlebarContainer.CssClass.ToString() + " rdTitleBar" + counter;
														//SessionObject.selectedRDTitleBarClass = " rdTitleBar" + counter;
														dock.CssClass = string.Empty;
												}
												else if (dock.ClientID.IndexOf("rotator2") > -1)
												{
														//dock.TitlebarContainer.CssClass = dock.TitlebarContainer.CssClass.ToString() + SessionObject.selectedRDTitleBarClass;
														dock.CssClass = string.Empty;
												}
												else
												{
														var browserVersion = System.Web.HttpContext.Current.Request.UserAgent.ToLower();
//														dock.CssClass = browserVersion.Contains("msie 8.") ? "tileContainerDiv_dimmed_IE8" : "tileContainerDiv_dimmed";
														String cs = browserVersion.Contains("msie 8.") ? "tileContainerDiv_dimmed_IE8" : "tileContainerDiv_dimmed";
														dock.ContentContainer.CssClass = dock.ContentContainer.CssClass + " " + cs;
														extraCommandPanel.CssClass = extraCommandPanel.CssClass + " " + cs;
														dock.TitlebarContainer.CssClass = "rdTitleBarTwoRowsLight";
														foreach(var cmd in dock.Commands)
															cmd.CssClass = cmd.CssClass + " " + cs;
												}
										}

								}
								catch (Exception)
								{
								}

								counter++;
						}

						HideUnusedTiles(counter);
				}

				private static DockCommand GetJSDockCommand(string commandText, string cssClass, string url, string overrideFunction)
				{
						var cmd = new DockCommand();
						cmd.AutoPostBack = false;
						cmd.OnClientCommand = (!String.IsNullOrEmpty(overrideFunction)) ? overrideFunction : "openExpandEditRadWindow";
						cmd.Name = url;
						cmd.Text = commandText;
						cmd.CssClass = cssClass;

						return cmd;
				}

				public static DockCommand GetDockCommand(string commandText, string cssClass, string path = "", string overrideFunction = "")
				{
						if (path.Contains(".aspx") || overrideFunction.Contains("OpenURL")) return GetJSDockCommand(commandText, cssClass, path, overrideFunction);

						var cmd = new DockCommand();
						cmd.AutoPostBack = true;
						cmd.Name = commandText;
						cmd.Text = commandText;
						cmd.CssClass = cssClass;

						return cmd;
				}

				protected void RadDock_Command(object sender, DockCommandEventArgs e)
				{
						var command = e.Command.Name;
						var tile = ((TileControlBase)((RadDock)sender).ContentContainer.Controls[0]).Tile;

						switch (command)
						{
								//Should go through JS command instead and not this
								case "Edit":
                                case "PerformanceLevel":
								case "Expand":
										ExpandTile(tile, command);
										break;

								case "Select Class":
										SessionObject.TileClicked = ((RadDock)sender).ClientID;
										SessionObject.LastElementsFolder_TileClicked = SessionObject.Elements_ActiveFolder;
										ChangeTile(tile);
										break;
						}
				}

				public void ChangeTile(Tile tile)
				{
						if (tile.DockClickMethod == null)
						{
								return;
						}

						tile.DockClickMethod(tile.TileParms);
				}


				public void ExpandTile(Tile tile, string expandMode)
				{
						SessionObject.ExpandingTileMode = expandMode;
						SessionObject.ExpandingTile_ControlTile = tile;
				}

				private void HideUnusedTiles(int firstTileIndexToHide)
				{
						for (var i = firstTileIndexToHide; i <= NumberOfTilesPerPage; i++)
						{
								var containterDiv = FindControl("tileContainerDiv" + i);
								if (containterDiv == null)
								{
										continue;
								}

								containterDiv.Visible = false;
						}
				}
		}
}