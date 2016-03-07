using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thinkgate.Controls.Assessment.ContentEditor
{
    public class ItemKeyValueCollection
    {
        public string EncAssessmentID;

        public string ItemID;

        public IEnumerable<ItemInfoKeyVal> Attributes;

        public class ItemInfoKeyVal
        {
            public string Key;
            public string Val;
        }
    }
}