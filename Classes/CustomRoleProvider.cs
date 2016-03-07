using System;
using System.Linq;
using System.Web.Security;
using System.Reflection;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes
{
    public class CustomRoleProvider : SqlRoleProvider
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            config["connectionStringName"] = AppSettings.ConnectionStringName;
            base.Initialize(name, config);
            //string connectionString = AppSettings.ConnectionString;
            //var roleField = Roles.Provider.GetType().GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
            //if (roleField != null)
            //    roleField.SetValue(Roles.Provider, connectionString);
            //_sqlConnectionString = connectionString;

        }
    }
}