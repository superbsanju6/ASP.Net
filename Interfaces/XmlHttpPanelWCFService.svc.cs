using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace Thinkgate.Interfaces
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class XmlHttpPanelWCFService : IXmlHttpPanelWCFService
    {
        public string RequestStandardsCourseList(string standardsCourse)
        {
            return standardsCourse;
        }
    }
}
