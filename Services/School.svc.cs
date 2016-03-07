namespace Thinkgate.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web;
    using System;
    using System.Reflection;
    
    using Thinkgate.Base.Classes;
    using Thinkgate.Classes;
    using Thinkgate.ExceptionHandling;

    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class School
    {
        #region Properties

        public SessionObject SessionObject
        {
            get
            {
                if (HttpContext.Current.Session == null)
                {
                    return null;
                }

                if (HttpContext.Current.Session["SessionObject"] == null)
                {
                    return null;
                }

                return HttpContext.Current.Session["SessionObject"] as SessionObject;
            }
        }

        #endregion

        #region Operation Contracts

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        public SchoolAndSchoolTypeContainer GetAllSchoolsFromSchoolTypes(SchoolAndSchoolTypeContainer container)
        {
            if (SessionObject == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var schoolsForLoggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return CreateSchoolAndSchoolTypeContainer(schoolsForLoggedInUser, container);
        }

        #endregion

        #region Private Static Methods

        private static SchoolAndSchoolTypeContainer CreateSchoolAndSchoolTypeContainer(IEnumerable<Base.Classes.School> schoolList, SchoolAndSchoolTypeContainer container)
        {
            if (schoolList == null)
            {
                return null;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            var result = new SchoolAndSchoolTypeContainer();
            var selectedSchoolTypes = container.SchoolTypes.FindAll(st => st.Selected);

            if (selectedSchoolTypes.Count == 0)
            {
                foreach (var school in schoolList)
                {
                    result.Schools.Add(new GenericControl
                    {
                        DisplayText = school.Name,
                        Selected = false,
                        Value = string.Format("{0}_{1}", container.SchoolsKey, school.ID)
                    });
                }
            }
            else
            {
                foreach (var school in from school in schoolList from selectedSchoolType in selectedSchoolTypes where school.Type == selectedSchoolType.DisplayText select school)
                {
                    result.Schools.Add(new GenericControl
                        {
                            DisplayText = school.Name,
                            Selected = false,
                            Value = string.Format("{0}_{1}", container.SchoolsKey, school.ID)
                        });
                }
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return result;
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request SchoolWCF", "SchoolWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end SchoolWCF", "SchoolWCF"); }
        }

        #endregion
    }

    [DataContract]
    public class SchoolAndSchoolTypeContainer
    {
        public SchoolAndSchoolTypeContainer()
        {
            Schools = new List<GenericControl>();
            SchoolTypes = new List<GenericControl>();
        }

        [DataMember]
        public List<GenericControl> Schools { get; set; }

        [DataMember]
        public string SchoolsKey { get; set; }

        [DataMember]
        public List<GenericControl> SchoolTypes { get; set; }

        [DataMember]
        public string SchoolTypesKey { get; set; }
    }
}
