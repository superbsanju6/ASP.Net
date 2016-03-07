using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes.Serializers
{
    public class BankQuestionSerializerForAssessmentPage : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new Type[] { typeof(BankQuestion) }); }
        }

        public override IDictionary<string, object> Serialize(object obj,
            JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var bq = (BankQuestion)obj;
            if (bq != null)
            {
                result.Add("ID", bq.ID);
                result.Add("EncryptedID", bq.EncryptedID);
                result.Add("OnAnyTest", bq.OnAnyTest);
                result.Add("ThumbnailName", bq.ThumbnailName);
                result.Add("Standards", bq.Standards);
                result.Add("StandardName", bq.StandardName);
                result.Add("StandardID", bq.StandardID);
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