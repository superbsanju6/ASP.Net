using System.ServiceModel;
using System.ServiceModel.Web;
using Thinkgate.Services;
using System.Collections.Generic;

namespace Thinkgate.Interfaces
{
    [ServiceContract(Namespace = "")]
    public interface IReportsWCF
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat= WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<object> GetReportListByLevel(ReportsWCFVariables reportVars);
    }
}
