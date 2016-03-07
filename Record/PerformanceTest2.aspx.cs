using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using System.Data;

namespace Thinkgate.Record
{
    public partial class PerformanceTest2 : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            drGeneric_String_String dr1 = new drGeneric_String_String();
            stopwatch.Start();
            for (var x = 0; x < 100; x++)
            {
                //CourseList classCourses = CourseMasterList.GetClassCoursesForUser(SessionObject.LoggedInUser);
                //DataTable temp = ThinkgateDataAccess.FetchDataTable("E3_WesTempTest", new object[] { classCourses.ToSql() });
                //dr1 = ThinkgateDataAccess.wesTest();
            
            }
            /*foreach (Course c in classCourses)
            {
                Console.WriteLine(c.Subject);
            }*/
            stopwatch.Stop();
            sb.Append("X: " + dr1[1].Value + "<br/>");
            sb.Append("A: " + stopwatch.Elapsed + "<br/>");

            drGeneric_String_String dr2 = new drGeneric_String_String();
            stopwatch.Reset(); stopwatch.Start();
            for (var y = 0; y < 100; y++)
            {
                /*DataTable classCoursesX = CourseMasterList2.GetClassCoursesForUser(SessionObject.LoggedInUser);
                dtGeneric_Int x1 = new dtGeneric_Int();
                foreach (DataRow row in classCoursesX.Rows)
                {
                    x1.Add(DataIntegrity.ConvertToInt(row["ID"]));
                }
                DataTable temp4 = ThinkgateDataAccess.FetchDataTable("E3_WesTempTest", new object[] { x1.ToSql() });
            */
                //dr2 = ThinkgateDataAccess.wesTest2();
            
            }
            stopwatch.Stop();
            sb.Append("X: " + dr2[1].Value + "<br/>");
            

            sb.Append("G: " + stopwatch.Elapsed + "<br/>");





            dtGeneric_String_String dt1 = new dtGeneric_String_String();
            stopwatch.Reset(); stopwatch.Start();
            for (var y = 0; y < 100; y++)
            {
                /*DataTable classCoursesX = CourseMasterList2.GetClassCoursesForUser(SessionObject.LoggedInUser);
                dtGeneric_Int x1 = new dtGeneric_Int();
                foreach (DataRow row in classCoursesX.Rows)
                {
                    x1.Add(DataIntegrity.ConvertToInt(row["ID"]));
                }
                DataTable temp4 = ThinkgateDataAccess.FetchDataTable("E3_WesTempTest", new object[] { x1.ToSql() });
            */
                //dt1 = ThinkgateDataAccess.wesTest3();

            }
            stopwatch.Stop();
            sb.Append("X: " + dt1.Rows[1]["Value"].ToString() + "<br/>");
            sb.Append("H: " + stopwatch.Elapsed + "<br/>");


            resultDiv.InnerHtml = sb.ToString();

        }
    }
}