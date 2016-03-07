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
    public class UsageGridGroupDetail
    {
        public string GroupTitle { get; set; }
        public int ColumnSpan { get; set; }
    }
}
