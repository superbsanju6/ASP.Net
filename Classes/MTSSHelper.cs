using CMS.DocumentEngine;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;

namespace Thinkgate.Classes
{
    public class E3Student
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
    }

    public class KenticoMTSSFormData
    {
        public int StudentID { get; set; }
        public String StudentName { get; set; }
        public int InterventionID { get; set; }
        public String FormName { get; set; }
        public int FormID { get; set; }
        public int RecordID { get; set; }
        public String FormHyperLink { get; set; }
        public int FormCount { get; set; }
        public String ListHyperLinks { get; set; }
        public String ListFormNames { get; set; }
    }

    public class E3MTSSInterventions
    {
        public int InterventionID { get; set; }
        public String InterventionType { get; set; }
        public String InterventionName { get; set; }
        public String InterventionTier { get; set; }
        public bool InterventionActive { get; set; }
    }

    public class E3MTSSInterventionStudents
    {
        public int InterventionID { get; set; }
        public int StudentID { get; set; }
        public String StudentName { get; set; }
    }

    public class E3InterventionDataObject
    {
        public List<E3MTSSInterventions> InterventionsObject { get; set; }
        public List<E3MTSSInterventionStudents> StudentsObject { get; set; }
    }

    [Serializable]
    public class MTSSSessionObject
    {
        public int UserPage { get; set; }
        public int SchoolID { get; set; }
        public int StudentID { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public bool UserCrossCourses { get; set; }
    }

    [Serializable]
    public class MTSSComboBoxItemsWCF
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    [Serializable]
    public class MTSSComboBoxContextWCF
    {
        public string Text { get; set; }
        public int UserPage { get; set; }
        public string Tier { get; set; }
        public int NumberOfItems { get; set; }
    }

    public class MTSSHelper : KenticoHelper
    {

        public static Table GenerateHTMLTableByIntervention(string legacy, List<KenticoMTSSFormData> kentico, E3InterventionDataObject E3obj, Telerik.Web.UI.RadWindow wndw, string InterventionType, bool isActive, bool isFormColumnEnabled)
        {

            //HTML Table Setup
            Table tbl = new Table();
            TableHeaderRow thr = new TableHeaderRow();

            TableHeaderCell th1 = new TableHeaderCell();
            th1.Text = "Tier";
            th1.Style.Add("font-weight", "bold");
            th1.Height = 20;
            th1.Width = 50;
            th1.HorizontalAlign = HorizontalAlign.Center;

            TableHeaderCell th2 = new TableHeaderCell();
            th2.Text = "RTI/Students";
            th2.Style.Add("font-weight", "bold");
            th2.Height = 20;
            th2.Width = 600;
            th2.HorizontalAlign = HorizontalAlign.Center;

            TableHeaderCell th3 = new TableHeaderCell();
            th3.Text = "Forms";
            th3.Height = 20;
            th3.Style.Add("font-weight", "bold");
            if (!isFormColumnEnabled)
            {
                th3.Style.Add("display", "none");
            }

            thr.Cells.Add(th1);
            thr.Cells.Add(th2);
            thr.Cells.Add(th3);

            tbl.Rows.Add(thr);

            //Intervention Names from Database
            foreach (E3MTSSInterventions iname in E3obj.InterventionsObject.Where(x => x.InterventionActive == isActive && InterventionType.Contains(x.InterventionType)))
            {
                TableRow tblrw = new TableRow();

                TableCell tblcll1 = new TableCell(); //Icon
                tblcll1.Width = 100;
                tblcll1.HorizontalAlign = HorizontalAlign.Left;
                tblcll1.VerticalAlign = VerticalAlign.Top;

                TableCell tblcll2 = new TableCell(); //Student
                tblcll2.Width = 300;
                tblcll2.Style.Add("Padding-left", "10px");
                TableCell tblcll3 = new TableCell(); //Form Count
                tblcll3.Width = 0;
                if (!isFormColumnEnabled)
                {
                    tblcll3.Style.Add("display", "none");
                }

                //Determine correct image
                String imageSource = String.Empty;
                if (iname.InterventionTier.CompareTo("Tier 1") == 0)
                    imageSource = "../Images/MTSSTileCompassIcon-Tier1.png";
                else if (iname.InterventionTier.CompareTo("Tier 2") == 0)
                    imageSource = "../Images/MTSSTileCompassIcon-Tier2.png";
                else imageSource = "../Images/MTSSTileCompassIcon-Tier3.png";


                //Image for Rows
                Image img = new Image();
                img.Attributes["src"] = imageSource;
                img.Height = Unit.Pixel(50);
                img.Width = Unit.Pixel(50);
                tblcll1.Controls.Add(img);
                tblrw.Cells.Add(tblcll1);

                //Add Form Name as Hyperlink
                HyperLink formName = new HyperLink();
                formName.NavigateUrl = legacy + "?ReturnURL=display.asp?key=7275%26fo=basic display%26rm=page%26xID=" + iname.InterventionID.ToString();
                formName.Target = "_blank";
                formName.Text = iname.InterventionName + "</br>";
                tblcll2.Controls.Add(formName);
                HyperLink emptyCell = new HyperLink();
                emptyCell.Style.Add("Visibility", "hidden");
                emptyCell.NavigateUrl = legacy + "?ReturnURL=display.asp?key=7275%26fo=basic display%26rm=page%26xID=" + iname.InterventionID.ToString(); ;
                emptyCell.Text = "Test";
                tblcll3.Controls.Add(emptyCell);

                //Add List of Students for each Form + Kentico Form Counts (Left join)
                var studentList = (from e3 in E3obj.StudentsObject
                                   join kn in kentico on e3.StudentID equals kn.StudentID into lj
                                   from kn in lj.DefaultIfEmpty()
                                   where e3.InterventionID == iname.InterventionID
                                   select new
                                   {
                                       name = e3.StudentName,
                                       FormCount = (kn != null) ? kn.FormCount : 0,
                                       Hyperlink = (kn != null) ? kn.FormHyperLink : String.Empty,
                                       FormName = (kn != null) ? kn.FormName : String.Empty,
                                       FormNameList = (kn != null) ? kn.ListFormNames : String.Empty,
                                       HyperLinkList = (kn != null) ? kn.ListHyperLinks : String.Empty
                                   }).ToList();


                //Inner table for Student + Kentico Form Count
                Table innerTbl1 = new Table();
                Table innerTbl2 = new Table();

                //Add Students to list
                foreach (var student in studentList)
                {
                    TableRow innerTblrw1 = new TableRow();
                    TableRow innerTblrw2 = new TableRow();

                    //Name
                    TableCell innerCll1 = new TableCell();
                    innerCll1.Width = 600;
                    innerCll1.HorizontalAlign = HorizontalAlign.Left;
                    innerCll1.Style.Add("Padding-left", "20px");

                    //Count
                    TableCell innerCll2 = new TableCell();
                    innerCll2.Width = 50;
                    innerCll2.HorizontalAlign = HorizontalAlign.Center;

                    //Hyperlinks (Hidden)
                    TableCell innerCll3 = new TableCell();
                    innerCll3.Width = 0;
                    innerCll3.Style.Add("Display", "none");

                    //Student Name
                    Literal lt1 = new Literal();
                    lt1.Text = student.name;
                    innerCll1.Controls.Add(lt1);
                    innerTblrw1.Cells.Add(innerCll1);

                    //Number of Kentico Forms
                    if (student.FormCount == 0)
                    {
                        Literal lt2 = new Literal();
                        lt2.Text = student.FormCount.ToString();
                        innerCll2.Controls.Add(lt2);
                        innerTblrw2.Cells.Add(innerCll2);
                    }
                    else
                    {
                        HyperLink h1 = new HyperLink();
                        h1.Attributes["onclick"] = string.Format("javascript:showForms($find('{0}'), '{1}', '{2}');return false;", wndw.ClientID, student.HyperLinkList, student.FormNameList);
                        h1.NavigateUrl = "#";
                        h1.Text = student.FormCount.ToString();
                        innerCll2.Controls.Add(h1);
                        innerTblrw2.Cells.Add(innerCll2);
                        innerCll2.Style.Add("color", "blue");
                        innerCll2.Style.Add("font-weight", "bold");
                    }

                    //Add row to inner table
                    innerTbl1.Rows.Add(innerTblrw1);
                    innerTbl2.Rows.Add(innerTblrw2);

                }
                //Add inner table to cell, cell to row, and row to outer table
                tblcll2.Controls.Add(innerTbl1);
                tblcll3.Controls.Add(innerTbl2);
                tblrw.Cells.Add(tblcll2);
                tblrw.Cells.Add(tblcll3);
                tbl.Rows.Add(tblrw);

            }

            return tbl;

        }

        public static Table GenerateHTMLTableForMissingForms(List<KenticoMTSSFormData> kentico, E3InterventionDataObject E3obj)
        {

            //HTML Table Setup
            Table tbl = new Table();

            //List of Students with Missing Kentico Forms
            var InterventionList = (from e3 in E3obj.StudentsObject
                                    join kn in kentico on e3.StudentID equals kn.StudentID into lj1
                                    from kn in lj1.DefaultIfEmpty()
                                    join i3 in E3obj.InterventionsObject on e3.InterventionID equals i3.InterventionID into lj2
                                    from i3 in lj2.DefaultIfEmpty()
                                    where kn == null
                                    select new
                                    {
                                        InterventionName = i3.InterventionName,
                                        InterventionID = e3.InterventionID,
                                        date = "1/1/2013"
                                    }).Distinct();


            foreach (var intvn in InterventionList)
            {
                TableRow tblrw = new TableRow();
                TableCell tblcll1 = new TableCell(); //Student Name

                //Add Form Name as Hyperlink
                HyperLink formName = new HyperLink();
                formName.Text = intvn.InterventionName;
                formName.NavigateUrl = "~/SessionBridge.aspx?ReturnURL=display.asp?key=7275%26fo=basic display%26rm=page%26xID=" + intvn.InterventionID.ToString();
                formName.Target = "_blank";
                tblcll1.Controls.Add(formName);

                //Inner table for Student + Kentico Form Count
                Table innerTbl = new Table();

                var studentList = (from e in E3obj.StudentsObject
                                   where e.InterventionID == intvn.InterventionID
                                   select new
                                   {
                                       StudentName = e.StudentName,
                                       date = "1/1/2013"
                                   }).Distinct();

                //Add Students to list
                foreach (var student in studentList)
                {
                    TableRow innerTblrw = new TableRow();
                    TableCell innerCll1 = new TableCell(); //Name
                    TableCell innerCll2 = new TableCell(); //Date


                    //Student Name
                    Literal lt1 = new Literal();
                    lt1.Text = student.StudentName + "</br>";
                    innerCll1.Controls.Add(lt1);
                    innerTblrw.Cells.Add(innerCll1);

                    //Add row to inner table
                    innerTbl.Rows.Add(innerTblrw);

                }
                //Add inner table to cell, cell to row, and row to outer table
                tblcll1.Controls.Add(innerTbl);
                tblrw.Cells.Add(tblcll1);
                tbl.Rows.Add(tblrw);

            }

            return tbl;

        }

        public static Table GenerateHTMLTableForMissingInterventions(List<KenticoMTSSFormData> kentico, E3InterventionDataObject E3obj)
        {

            //HTML Table Setup
            Table tbl = new Table();

            //Add List of Students with Kentico Forms, but Missing Interventions
            var studentList = from k in kentico
                              join i in E3obj.InterventionsObject
                                  on k.InterventionID equals i.InterventionID into lj1
                              from i in lj1.DefaultIfEmpty()
                              where i == null
                              select new KenticoMTSSFormData
                              {
                                  StudentID = k.StudentID,
                                  FormName = k.FormName,
                                  FormID = k.FormID,
                                  RecordID = k.RecordID
                              };

            //Get Student Names from E3 Database
            List<E3Student> sname = getE3StudentNames(studentList.ToList());

            //Add Student Names
            var studentName = from st in studentList
                              join sn in sname
                                on st.StudentID equals sn.StudentID into lj1
                              from sn in lj1.DefaultIfEmpty()
                              select new KenticoMTSSFormData
                              {
                                  StudentID = st.StudentID,
                                  StudentName = (sn != null) ? sn.StudentName : " Missing Student Name (ID:" + st.StudentID.ToString() + ")",
                                  FormName = st.FormName,
                                  FormID = st.FormID,
                                  RecordID = st.RecordID
                              };

            //Intervention Names from Database
            foreach (var s in studentName.GroupBy(x => x.FormName).Select(y => y.First()))
            {
                TableRow tblrw = new TableRow();
                TableCell tblcll1 = new TableCell(); //Student Name
                //tblcll1.Style.Add("font-weight", "bold");

                //Student Name
                Literal lt1 = new Literal();
                lt1.Text = "<b>" + s.FormName + "</b></br>";
                tblcll1.Controls.Add(lt1);

                //Inner table for Forms
                Table innerTbl = new Table();

                //Form List
                var students = (from st in studentName
                                where st.StudentID == s.StudentID
                                select new
                                {
                                    StudentName = st.StudentName,
                                    Date = "1/1/2013"
                                }).Distinct();

                //Add Students to list
                foreach (var s1 in students)
                {
                    TableRow innerTblrw = new TableRow();
                    TableCell innerCll1 = new TableCell(); //Name
                    TableCell innerCll2 = new TableCell(); //Date

                    //Add Student Name as Hyperlink
                    HyperLink sName = new HyperLink();
                    sName.NavigateUrl = "/" + KenticoHelper.KenticoVirtualFolder + "/CMSModules/Thinkgate/Forms/BizForm_Edit.aspx?formID=" + s.FormID.ToString() + "&formRecordID=" + s.RecordID.ToString();
                    sName.Target = "_blank";
                    sName.Text = s1.StudentName + "</br>";
                    innerCll1.Controls.Add(sName);
                    innerTblrw.Cells.Add(innerCll1);

                    //Add row to inner table
                    innerTbl.Rows.Add(innerTblrw);

                }
                //Add inner table to cell, cell to row, and row to outer table
                tblcll1.Controls.Add(innerTbl);
                tblrw.Cells.Add(tblcll1);
                tbl.Rows.Add(tblrw);

            }

            return tbl;

        }

        public static E3InterventionDataObject getE3InterventionData(int userpage, bool usercrosscourses, int schoolid, string grade, string subject, int studentid, string type, string tier = "")
        {

            SqlParameterCollection sqlParms = new SqlCommand().Parameters;
            sqlParms.AddWithValue("@UserPage", userpage);
            sqlParms.AddWithValue("@UserCrossCourses", usercrosscourses);
            sqlParms.AddWithValue("@SchoolID", schoolid);
            sqlParms.AddWithValue("@Grade", grade);
            sqlParms.AddWithValue("@Subject", subject);
            sqlParms.AddWithValue("@StudentID", studentid);
            sqlParms.AddWithValue("@Type", type);
            sqlParms.AddWithValue("@Tier", tier);

            DataSet ds = Thinkgate.Base.DataAccess.ThinkgateDataAccess.FetchDataSet(AppSettings.ConnectionString,
                                                                        Thinkgate.Base.Classes.Data.StoredProcedures.E3_INTERVENTIONS_SELECT,
                                                                        CommandType.StoredProcedure,
                                                                        sqlParms);

            //Build MTSS Objects
            E3InterventionDataObject obj = new E3InterventionDataObject();
            obj.InterventionsObject = (List<E3MTSSInterventions>)ds.Tables[0].ToList<E3MTSSInterventions>();
            obj.StudentsObject = (List<E3MTSSInterventionStudents>)ds.Tables[1].ToList<E3MTSSInterventionStudents>();


            return obj;

        }

        public static DataTable getE3InterventionStudents(int userpage, bool usercrosscourses, int schoolid, string grade, string subject, string type, string tier = "")
        {
            SqlParameterCollection sqlParms = new SqlCommand().Parameters;
            sqlParms.AddWithValue("@UserPage", userpage);
            sqlParms.AddWithValue("@UserCrossCourses", usercrosscourses);
            sqlParms.AddWithValue("@SchoolID", schoolid);
            sqlParms.AddWithValue("@Grade", grade);
            sqlParms.AddWithValue("@Subject", subject);
            sqlParms.AddWithValue("@Type", type);
            sqlParms.AddWithValue("@Tier", tier);

            DataTable dt = Thinkgate.Base.DataAccess.ThinkgateDataAccess.FetchDataTable(AppSettings.ConnectionString,
                                                                        Thinkgate.Base.Classes.Data.StoredProcedures.E3_STUDENTS_SELECT_IN_RTI,
                                                                        CommandType.StoredProcedure,
                                                                        sqlParms);
            return dt;


        }

        public static List<E3Student> getE3StudentNames(List<KenticoMTSSFormData> studentList)
        {
            //Convert to DataTable for SP
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("StudentID");
            dt.Columns.Add(dc);

            foreach (var stid in studentList)
            {
                dt.Rows.Add(stid.StudentID.ToString());
            }

            //Call SP with DataTable list
            SqlParameterCollection sqlParms = new SqlCommand().Parameters;
            sqlParms.AddWithValue("@StudentIds", dt);

            DataTable rs = Thinkgate.Base.DataAccess.ThinkgateDataAccess.FetchDataTable(AppSettings.ConnectionString,
                                                                        Thinkgate.Base.Classes.Data.StoredProcedures.E3_STUDENTS_BY_STUDENT_ID,
                                                                        CommandType.StoredProcedure,
                                                                        sqlParms);
            //Convert to List using extension method (columns must match property names)
            List<E3Student> ls = (List<E3Student>)rs.ToList<E3Student>();
            return ls;

        }


    }
}

