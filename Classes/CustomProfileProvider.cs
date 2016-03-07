using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Profile;
using System.Reflection;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes
{
    public class CustomProfileProvider : SqlProfileProvider
    {
        public override void Initialize(string name, NameValueCollection config){
            config["connectionStringName"] = AppSettings.ConnectionStringName;
            base.Initialize(name, config);
            //string connectionString = AppSettings.ConnectionString;
            //var profileField = ProfileManager.Provider.GetType().GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
            //if (profileField != null)
            //    profileField.SetValue(ProfileManager.Provider, connectionString);
        }
    }
}