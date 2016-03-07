using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Thinkgate.Services;

namespace Thinkgate.Interfaces
{
    [ServiceContract(Namespace = "")]
    public interface IStandardsWCF
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat= WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, string> RequestStandardsCourseList(StandardsWCFVariables newObject);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, string> RequestStandardsSubjectList(StandardsWCFVariables newObject);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> LoadStandardsSubjectList(StandardsWCFVariables standardsVars);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, List<string>> LoadStandardsGradeSubjectCourseList(StandardsWCFVariables standardsVars);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, List<string>> LoadStandardsSubjectCourseList(StandardsWCFVariables standardsVars);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> LoadStandardsCourseList(StandardsWCFVariables standardsVars);
    }
}
