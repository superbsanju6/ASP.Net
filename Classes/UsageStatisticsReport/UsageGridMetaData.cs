using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Enums;

namespace Thinkgate.Classes.UsageStatisticsReport
{
    public class UsageGridMetaData
    {
        public string data { get; set; }
        public string title { get; set; }
        public bool visible { get; set; }
    }
}
