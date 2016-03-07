using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes.Serializers
{
    public class StandardsSerializer : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new Type[] { typeof(Standards) }); }
        }

        public override IDictionary<string, object> Serialize(object obj,
            JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var stand = (Standards)obj;
            if (stand != null)
            {
                result.Add("ID", stand.ID);
                result.Add("StandardName", stand.StandardName);
                result.Add("Desc", stand.Desc);                
            }

            return result;
        }

        public override object Deserialize(IDictionary<string, object> dictionary,
            Type type, JavaScriptSerializer serializer)
        {
            return null;
        }
    }
}