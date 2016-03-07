using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes.Serializers
{
    public class RubricMinSerializer : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new Type[] { typeof(Rubric) }); }
        }

        public override IDictionary<string, object> Serialize(object obj,
            JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var rubric = (Rubric)obj;
            if (rubric != null)
            {
                result.Add("ID", rubric.ID);
                result.Add("Name", rubric.Name);
                result.Add("Content", rubric.Content);            
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