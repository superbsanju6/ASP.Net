using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thinkgate.Classes.Search
{
    public class KeyValueComparer: IEqualityComparer<KeyValuePair>
    {
        public bool Equals(KeyValuePair x, KeyValuePair y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(KeyValuePair obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}