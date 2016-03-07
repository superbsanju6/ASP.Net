using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Reflection;

using Thinkgate.Controls.Teacher;
using Standpoint.Core.Utilities;
using Thinkgate.ExceptionHandling;

namespace Thinkgate.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PieChartWCF : Interfaces.IPieChartWCF
    {
        public string OpenExpandedWindow(PieChartWCFVariables pieChartVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            string returnMessage = "open radwindow";
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);

            return returnMessage;
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request PieChartWCF", "PieChartWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end PieChartWCF", "PieChartWCF"); }
        }
    }
}
