using System.ServiceModel;
using System.ServiceModel.Web;
using Thinkgate.Services;

namespace Thinkgate.Interfaces
{
    [ServiceContract(Namespace = "")]
    public interface IRubricsWCF
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat= WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        int CopyRubric(RubricsWCFVariables rubricVars);
    }
}
