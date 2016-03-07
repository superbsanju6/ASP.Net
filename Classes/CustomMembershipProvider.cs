using System;
using System.Linq;
using System.Reflection;
using System.Web.Security;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes
{
    public class CustomMembershipProvider : SqlMembershipProvider
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            config["connectionStringName"] = AppSettings.ConnectionStringName;
            base.Initialize(name, config);
            //string connectionString = AppSettings.ConnectionString;
            // Set private property of Membership provider. 
            //FieldInfo connectionStringField = GetType().BaseType.GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
            //connectionStringField.SetValue(this, connectionString); 
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                DistrictParms DParms = DistrictParms.LoadDistrictParms();
                if (DParms.PasswordConfigurationRequired == "Yes")
                {
                    if (!string.IsNullOrEmpty(DParms.PasswordMinLength))
                        return int.Parse(DParms.PasswordMinLength);
                    else
                        return base.MinRequiredPasswordLength;
                }
                else
                    return base.MinRequiredPasswordLength;
            }

        }
    }
}