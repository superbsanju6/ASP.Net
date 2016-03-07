using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Thinkgate.Services;

namespace Thinkgate.Interfaces
{
    [ServiceContract(Namespace = "")]
    public interface ITeacherWCF
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat= WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, string> RequestSubjectList(TeacherWCFVariables newObject);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, string> RequestCourseList(TeacherWCFVariables newObject);
    }
}
