using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using System.Data;

namespace Thinkgate.Classes.Serializers
{
    public class ToStringSerializer : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new Type[] { typeof(ItemWeight), typeof(Rigor), typeof(DifficultyIndex), typeof(dtItemBank) }); }
        }

        public override IDictionary<string, object> Serialize(object obj,
            JavaScriptSerializer serializer)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (obj != null)
            {
                if (obj is Rigor)
                {
                    result.Add("Text", ((Rigor)obj).ToString());
                }
                else if (obj is ItemWeight)
                {
                    result.Add("Text", ((ItemWeight)obj).ToString());
                }
                else if (obj is DifficultyIndex)
                {
                    result.Add("Text", ((DifficultyIndex)obj).ToString());
                }
                else if (obj is dtItemBank)
                {
                    List<ItemBank> itemBankList = new List<ItemBank>();
                    foreach (DataRow row in ((dtItemBank)obj).Rows)
                    {
                        var itembank = new ItemBank(row["Target"].ToString(), Standpoint.Core.Utilities.DataIntegrity.ConvertToShort(row["TargetType"]), row["ApprovalSource"].ToString(), row["Label"].ToString());
                        itemBankList.Add(itembank);
                    }

                    result.Add("ItemBanks", itemBankList);
                }

            }

            return result;
        }

        public override object Deserialize(IDictionary<string, object> dictionary,
            Type type, JavaScriptSerializer serializer)
        {
            object obj = null;

            if (dictionary.ContainsKey("ItemBanks"))
            {
                var itemBankDt = new dtItemBank();
                var itemBankList = ((List<ItemBank>)dictionary["ItemBanks"]);

                foreach (ItemBank ib in itemBankList)
                {
                    itemBankDt.Add(Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(ib.TargetType.ToString()), ib.Target,
                        Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(ib.ApprovalSource), ib.Label);

                }

                obj = itemBankDt;

            }

            return obj;
        }
    }
}