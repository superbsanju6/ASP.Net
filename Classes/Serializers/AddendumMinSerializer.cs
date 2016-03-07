using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes.Serializers
{
    public class AddendumMinSerializer : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new Type[] { typeof(Addendum) }); }
        }

        public override IDictionary<string, object> Serialize(object obj,
            JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var addendum = (Addendum)obj;
            if (addendum != null)
            {
                result.Add("ID", addendum.ID);
                result.Add("Addendum_Name", addendum.Addendum_Name);
                result.Add("Addendum_Text", addendum.Addendum_Text);             
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