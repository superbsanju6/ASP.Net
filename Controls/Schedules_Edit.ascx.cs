using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Thinkgate.Base.Classes;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;

namespace Thinkgate.Controls
{
	public partial class Schedules_Edit : System.Web.UI.UserControl
	{
		protected const Int32 cbxUnchecked = 0;
		protected const Int32 cbxChecked = 1;
		protected const Int32 cbxFilled = 2;

		public List<Scheduling.ScheduleType> ScheduleTypes;
		public string SaveHandler;
		public string CancelHandler;
		protected void Page_Load(object sender, EventArgs e)
		{

			lblResultMessage.Text = "";
			if (!IsPostBack)
			{
			}

			if (Request.Form["__EVENTTARGET"] == "RadButtonSave")
			{
				//rbSaveClick(this, new EventArgs());
			}
		}

		/// <summary>
		/// The Schedules_Edit control is made up of a set of date windows, each with a begin date, end date, and lock toggle. The set of date 
		/// windows is passed/assigned to this control by the page or control hosting us. To keep this control generic, we don't know how many 
		/// date windows we'll get to display, so we'll assume the set is handed to us and we can iterate through them one by one. Given that, 
		/// the configuration of each date window is the same - consisting of a radDatePicker control for the begin date, a radDatePicker 
		/// control for the end date, and a radButton made to look like a check box for the lock/unlock toggle. Iterating through the set of 
		/// date windows, we will build and configure the controls for each window here in the code-behind.  
		/// </summary>
		public void RenderEditControl()
		{
			if (ScheduleTypes != null)
			{
				int i = 1; //intentionally set to 1 so as to create row and insert after the header row.
				foreach (Scheduling.ScheduleType schedType in ScheduleTypes)
				{
					HtmlTableRow row = new HtmlTableRow();

					HtmlTableCell cell;
					//
					// Create the label - this label describes this date window is for.
					//
					Label lbl = new Label();
					lbl.Text = schedType.TypeName;
					lbl.CssClass += " Schedules_Edit_Control label " + schedType.TypeName;
					lbl.Attributes.Add("ScheduleTypeID", schedType.TypeID.ToString());
					lbl.Attributes.Add("DocTypeID", schedType.DocTypeID.ToString());
					lbl.Attributes.Add("DefaultValues", "");
					cell = new HtmlTableCell();
					cell.Controls.Add(lbl);
					row.Controls.Add(cell);

					//
					// Create the Begin Date Control
					//
					RadDatePicker ctrlBegDate = new RadDatePicker();
					ctrlBegDate.ID = schedType.TypeName + "_ctrlBeginDate";
					ctrlBegDate.CssClass += " Schedules_Edit_Control BeginDate " + schedType.TypeName;

					//Styling for our calendar control
					ctrlBegDate.Skin = "Vista";		//just one of Telerik's styles - If we don't specify, then what you get is a ghost-like transparency.

					//Styling - Make current date stand out (but not selected).
					RadCalendarDay rcd = new RadCalendarDay();
					rcd.Repeatable = RecurringEvents.Today;
					rcd.ItemStyle.BackColor = System.Drawing.Color.Bisque;
					ctrlBegDate.Calendar.SpecialDays.Add(rcd);

					ctrlBegDate.MinDate = new DateTime(1980, 1, 1);
					ctrlBegDate.MaxDate = new DateTime(3000, 12, 31);

					// If a default date is provided, then a) set our control to that date, and b) store off date as an attribute
					if (!string.IsNullOrEmpty(schedType.DefaultStart))
					{
						ctrlBegDate.SelectedDate = DataIntegrity.ConvertToDate(schedType.DefaultStart);
						lbl.Attributes["DefaultValues"] = schedType.DefaultStart;
					}
					else
						lbl.Attributes["DefaultValues"] = DateTime.Now.ToString();
					// Add control to the row.
					cell = new HtmlTableCell();
					cell.Controls.Add(ctrlBegDate);
					row.Controls.Add(cell);

					//
					// Create the End Date Control
					//
					RadDatePicker ctrlEndDate = new Telerik.Web.UI.RadDatePicker();
					ctrlEndDate.ID = schedType.TypeName + "_ctrlEndDate";
					ctrlEndDate.CssClass += " Schedules_Edit_Control EndDate " + schedType.TypeName;

					//Styling for our calendar control
					ctrlEndDate.Skin = "Vista";		//just one of Telerik's styles - If we don't specify, then what you get is a ghost-like transparency.

					//Styling - Make current date stand out (but not selected).  rcd was created up above for use in ctrlBegDate.
					ctrlEndDate.Calendar.SpecialDays.Add(rcd);
					ctrlEndDate.MinDate = new DateTime(1980, 1, 1);
					ctrlEndDate.MaxDate = new DateTime(3000, 12, 31);

					// If a default date is provided, then a) set our control to that date, and b) store off date as an attribute
					if (!string.IsNullOrEmpty(schedType.DefaultEnd))
					{
						ctrlEndDate.SelectedDate = DataIntegrity.ConvertToDate(schedType.DefaultEnd);
						lbl.Attributes["DefaultValues"] += "|" + schedType.DefaultEnd;
					}
					else
						lbl.Attributes["DefaultValues"] = "|" + DateTime.Now.ToString();
					// Add control to the row.
					cell = new HtmlTableCell();
					cell.Controls.Add(ctrlEndDate);
					row.Controls.Add(cell);

					//
					// Create the Clear Dates image
					//
					HtmlImage img = new HtmlImage();
					img.ID = schedType.TypeName + "_ctrlClearDates";
					img.Attributes.Add("class", " Schedules_Edit_Control ClearDates " + schedType.TypeName);
					img.Src = "~/Images/Eraser_Small.png";
					img.Attributes.Add("OnClick", "Schedules_Edit_Control.ClearDatesByType(\"" + schedType.TypeName + "\")");
					// Add control to the row.
					cell = new HtmlTableCell();
					cell.Controls.Add(img);
					row.Controls.Add(cell);

					//
					// Create Lock/Unlock check box.
					//
					RadButton ctrlRadButton = new Telerik.Web.UI.RadButton();
					ctrlRadButton.ButtonType = RadButtonType.ToggleButton;
					ctrlRadButton.ToggleType = ButtonToggleType.CustomToggle;
					ctrlRadButton.ID = schedType.TypeName + "_btnToggle";
					ctrlRadButton.CssClass += " Schedules_Edit_Control Toggle " + schedType.TypeName;
					ctrlRadButton.AutoPostBack = false;
					// Add to our button the toggle states
					RadButtonToggleState tglState = new RadButtonToggleState();
					tglState.PrimaryIconCssClass = "rbToggleCheckbox";
					ctrlRadButton.ToggleStates.Add(tglState);
					
					tglState = new RadButtonToggleState();
					tglState.PrimaryIconCssClass = "rbToggleCheckboxChecked";
					ctrlRadButton.ToggleStates.Add(tglState);

					tglState = new RadButtonToggleState();
					tglState.PrimaryIconCssClass = "rbToggleCheckboxFilled";
					ctrlRadButton.ToggleStates.Add(tglState);

					// Set default value for our check box
					// The check boxes in this control are a three-state checkbox - 
					//	 - not checked,
					//	 - checked,
					//	 - heterogeneous (a state representing multiple schedules some of which are checked and others of which are not.
					//
					// The initial state of the checkboxes should be set to "Heterogeneous" (numeric value of 2) because the schedule 
					// control is not even displayed until user clicks on a schedule in the grid.  At that point, there is javascript 
					// logic in AssessmentScheduling.ascx that will populate the checkboxes and dates within this control according to 
					// business logic there.  By setting the checkbox to 2, the logic in AssessmentScheduling.ascx will interpret this 
					// to mean that the checkbox has not been set to any value yet.
					//
					
					if (schedType.DefaultLock.HasValue)
					{
						ctrlRadButton.SelectedToggleStateIndex = (schedType.DefaultLock.Value ? cbxChecked : cbxUnchecked);
						lbl.Attributes["DefaultValues"] += "|" + (schedType.DefaultLock.Value ? cbxChecked.ToString() : cbxUnchecked.ToString());
					}
					else
					{
						ctrlRadButton.SelectedToggleStateIndex = cbxFilled;
						lbl.Attributes["DefaultValues"] += "|" + cbxFilled.ToString();
					}
					// Add our button to the row
					cell = new HtmlTableCell();
					cell.Controls.Add(ctrlRadButton);
					row.Controls.Add(cell);

					// Add our row to the table.
					SchedControlsTable.Rows.Insert(i, row);
					i++;
				}
				Schedules_Edit_EditPanel.Visible = true;
				Schedules_Edit_ResultPanel.Visible = false;
			}
			else
			{
				lblResultMessage.Text = "No scheduling purposes found.";
				Schedules_Edit_EditPanel.Visible = false;
				Schedules_Edit_ResultPanel.Visible = true;
			}
		}

	}
}