using Standpoint.Core.Utilities;
//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using Thinkgate.Base.Classes;

namespace Thinkgate.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LoginService
    {

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public void UpdateLegacyRole(string portalId, string schoolId)
        {
            Classes.SessionObject obj = (Thinkgate.Classes.SessionObject)HttpContext.Current.Session["SessionObject"];
            DistrictParms districtParms = DistrictParms.LoadDistrictParms();
           
                if (districtParms.MultiRoleEnabled == "Yes" && obj.LoggedInUser.Roles != null)
                {
                    for (int i = 0; i < DataIntegrity.ConvertToInt(obj.LoggedInUser.Roles.Count); i++)
                    {
                        if (obj.LoggedInUser.Roles[i].RolePortalSelection == DataIntegrity.ConvertToInt(portalId))
                        {
                            ThinkgateRole.UpdateLegacyRoleBasedOnPortalSelection(obj.LoggedInUser.Page, obj.LoggedInUser.Roles[i].RoleName.ToLower(), DataIntegrity.ConvertToInt(schoolId));
                            break;
                        }
                           
                    }
                   
               
                
            }
        }

    }
}
