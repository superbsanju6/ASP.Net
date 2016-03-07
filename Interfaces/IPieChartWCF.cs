using System.ServiceModel;
using System.ServiceModel.Web;
using Thinkgate.Services;

namespace Thinkgate.Interfaces
{
    [ServiceContract(Namespace = "")]
    public interface IPieChartWCF
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat= WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string OpenExpandedWindow(PieChartWCFVariables assessmentVars);
    }
}
