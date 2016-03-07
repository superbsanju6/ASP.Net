using System.Collections.Generic;
using System;

namespace Thinkgate.Classes
{
    [Serializable]

    public class TileParms
    {
        private Dictionary<string, object> _parms;
        public Dictionary<string, object> Parms { get { return _parms; }  }

        public TileParms()
        {
            _parms = new Dictionary<string,object>();
        }

        public TileParms(Dictionary<string,object> parms)
        {
            _parms = parms;
        }

        public object GetParm(string key)
        {            
            return (_parms.ContainsKey(key)) ? _parms[key] : null;
        }

        public void AddParm(string key, object value)
        {
            if (GetParm(key) != null)
                UpdateParm(key, value);
            else
                _parms.Add(key, value);
        }

        public void UpdateParm(string key, object value)
        {
            if (GetParm(key) != null)
                _parms["key"] = value;
        }

        public void DeleteParm(string key)
        {
            if (GetParm(key) != null)
                _parms.Remove(key);
        }

        public string GetXML()
        {
            string xml = @"<TileParms>";

            foreach (var key in Parms.Keys)
                xml += @"<Parm key='" + key + "'>" + Parms[key].ToString() + "</Parm>";

            xml += @"</TileParms>";

            return xml;
        }
    }
}