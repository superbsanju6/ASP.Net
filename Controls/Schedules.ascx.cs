using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using Thinkgate.Base;
using Thinkgate.Base.Classes;

namespace Thinkgate.Controls
{
    public partial class Schedules : System.Web.UI.UserControl
    {
        public object JSonDataSource;
		public List<Scheduling.Schedule> DataSource;
        public int DocumentID;
        public int DocumentTypeID;
        public int ScheduleTypeID;

		protected void Page_Init(object sender, EventArgs e)
		{
			//
			// Was looking for a RadGrid event to populate Datasource, but could not find any firing before 
			// this control was rendered, so I am using the Page_Init and searching up the Parent chain to 
			// get the info we need to display the control.
			//
			//var assessmentSchedules = ((Thinkgate.Controls.Assessment.AssessmentScheduling)this.Parent.TemplateControl).AssessmentSchedules;
			//var dataItem = (Thinkgate.Base.Classes.AssessmentSchedule)((Telerik.Web.UI.GridDataItem)this.Parent.Parent).DataItem;
			//DataSource = assessmentSchedules.FindAll(x => x.schedID == dataItem.ID);
		}

        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
			{
			}
			
		}

        public void RenderSchedTable()
        {
            if (DataSource != null)
            {
                //
                // Populate the control using datasource provided to codebehind
                //

                // Table Header
				var sbSchedTable = new StringBuilder("<table  class=\"Schedules\" ><tr class=\"hdr\" ><th style=\"width:53px;\" ></th><th style=\"width:80px;\" >Beginning</th><th style=\"width:80px;\" >Ending</th><th style=\"width:50px;\" >Status</th><th>Notes</th style=\"width:200px;\"  ></tr>");

                // Other table rows...

				//var firstTimeThrough = true;
                foreach (var sched in DataSource)
                {
                    sbSchedTable.Append("<tr" + (string.IsNullOrWhiteSpace(sched.CssStyle) ? "" : " class=\"" + sched.CssStyle + "\" ") + "><td class=\" Schedules " + sched.ScheduleTypeName + "\" >" + sched.Lock_Label +
									   "</td><td class=\" Schedules " + sched.ScheduleTypeName + " Begin \" >" + sched.Lock_Begin +
									   "</td><td class=\" Schedules " + sched.ScheduleTypeName + " End \" >" + sched.Lock_End +
									   "</td><td class=\" Schedules " + sched.ScheduleTypeName + " Locked \" value=\"" + (sched.Locked == 1).ToString() + "\" >" + (sched.Locked == 1 ? sched.Lock_Active : sched.Lock_Inactive) + "</td>");

					if (sched.LevelLabel != sched.ParentLevelLabel)
					{
						sbSchedTable.Append("</td><td class=\" Schedules " + sched.ScheduleTypeName + " Notes \" > Currently " +
											(sched.Lock_Inherited ?
												sched.Lock_Active :
												sched.Lock_Inactive)
											+ " at " + sched.ParentLevelLabel +
											(!sched.Lock_Inherited && !string.IsNullOrEmpty(sched.Lock_EffectiveBegin + sched.Lock_EffectiveEnd)
												? " level from " + sched.Lock_EffectiveBegin + " to " + sched.Lock_EffectiveEnd + ".</td>"
												: " level.</td>"));
					}
					//if (firstTimeThrough)
					//{
					//	sbSchedTable.Append("<td rowspan=\"" + DataSource.Count.ToString() + "\" class=\"" + sched.ScheduleTypeName + " Notes \"  >Currently " +
					//						(sched.Lock_Inherited ? sched.Lock_Active : sched.Lock_Inactive) +
					//						" at " + sched.ParentLevelLabel);

					//	sbSchedTable.Append((sched.Lock_Inherited && !string.IsNullOrEmpty(sched.Lock_EffectiveBegin + sched.Lock_EffectiveEnd))
					//						? " level from " + sched.Lock_EffectiveBegin + " to " + sched.Lock_EffectiveEnd + ".</td>"
					//						: " level.</td>");

					//	firstTimeThrough = false;
					//}

					sbSchedTable.Append("</tr>");
                }

				sbSchedTable.Append("</table>");
                divSchedTable.InnerHtml = sbSchedTable.ToString();
            }

			else divSchedTable.InnerHtml = "<table><tr><td>No schedules found.</td></tr></table>";
        }
    }
}