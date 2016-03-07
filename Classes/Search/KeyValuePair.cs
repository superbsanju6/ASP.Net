
using System;

namespace Thinkgate.Classes.Search
{
    [Serializable]
    public class KeyValuePair
    {
        #region Properties

        public KeyValuePair()
        { }

        public KeyValuePair(string key, string value)
            : this()
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }

        #endregion
    }
}