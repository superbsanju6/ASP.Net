using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using System.Data.SqlClient;
using Standpoint.Core.Utilities;
using Telerik.Charting;
using Thinkgate.Base.Classes.Data;
using Thinkgate.Domain.Classes;
using Thinkgate.Utilities;

namespace Thinkgate.RosterPortal
{
    public partial class RosterPortletPage : Page
    {
        /* List objects I will need */
        protected List<Class> teacherClasses = new List<Class>();

        /* objects for sessions */
        protected SessionObject _sessionObject;

        /* Modular Int32 for page level scope Ids */
        protected Int32 _classId;
        protected Int32 _userId;

        /* Int32 for Students.ID_Encrypted */
        protected Int32 idEncrypted = new Int32();

        /* Per Jason, this is part of the RadChart logic I am ripping out of StudentSearch.ascx  */
        public static DataTable countData;
        public static EntityTypes _level;
        public static int _levelID;

        /* SP that grabs the count for SchoolTypes */
        private void GetStudentCounts()
        {
            // asynchronous task.
            countData = Base.Classes.Data.StudentDB.GetStudentCountsDT(_levelID, _level);
        }

        private void LoadDistrictParms()
        {
            var parms = Base.Classes.DistrictParms.LoadDistrictParms();
            if (parms.TilePieCharts_Clickable.ToLower() == "yes") studentCountChart.Click += pieChart_Click;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                /* grab the session object to evaluate */
                _sessionObject = (SessionObject)Session["SessionObject"];

                /* obtain the userId */
                _userId = _sessionObject.LoggedInUser.Page;

                /* Didn't inherit from TileControlBase, I can't use the TileParms for level 
                 * Have to use RolePortal for _level. MA has 1 district ID (22936). 
                 * I have to get it from the logged in user */
                var rolePortal = (RolePortal)_sessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection);

                /* Check rolePortal, set level and levelId */
                switch (rolePortal)
                {
                    case RolePortal.School:
                    case RolePortal.District:
                        if (Request.QueryString["levelID"] != null && !String.IsNullOrEmpty(Request.QueryString["levelID"]))
                        {
                            _levelID = Cryptography.GetDecryptedID(_sessionObject.LoggedInUser.CipherKey);
                        }
                        else
                        {
                            _levelID = _sessionObject.LoggedInUser.District;
                            _level = rolePortal == RolePortal.District ? EntityTypes.District : EntityTypes.School;
                        }
                        SetUpNonTeacherUi(); // Setup UI
                        LoadPieCharts(); // Load charts
                        break;
                    case RolePortal.Teacher:
                        {
                            _level = EntityTypes.Teacher; // Set _level
                            SetUpTeacherUi(); // Setup UI
                            break;
                        }
                }
            }
        }

        private void SetUpNonTeacherUi()
        {
            /* Hide the list box and the combo box */
            studentList.Visible = false;
            classComboBox.Visible = false;
        }

        private void SetUpTeacherUi()
        {
            /* Hide the pie chart div */
            studentChart.Visible = false;

            /* load the teacherClasses list up using the session object rather than calling DB again */
            teacherClasses = _sessionObject.LoggedInUser.Classes;

            /* Some LINQ to get the first class ID from the list */
            var initialClassLoad = teacherClasses.Select(c => c.ID).FirstOrDefault();

            /* Methods to load up students, encrypt and data bind */
            EncryptStudentIdAndDataBind(GetStudentsFromClassId(initialClassLoad, _userId));

            /* Load the teacher's classes into the drop down */
            LoadClassesIntoDropDown();
        }

        #region Public Methods and initial Page_Load events to setup the page
        /// <summary>
        /// Create list of type Student with Student_Name/ID pairs 
        /// Displays the Student's Name as link that's associated with an ID
        /// </summary>
        /// <param name="classId">The Class.ID</param>
        /// <param name="userId">The user logged into the session</param>
        /// <returns>New list of Student_Name/ID for that class</returns>
        public static List<Student> GetStudentsFromClassId(int classId, int userId)
        {
            // Create a list instance of Students
            var students = new List<Student>();

            // 'using' statement to release resources from data reader after we have the data
            using (var conn = new SqlConnection(AppSettings.ConnectionString))
            {
                String name = string.Empty; // Student's Name
                String value = string.Empty;  // Students ID

                conn.Open(); // open connection

                var cmd = new SqlCommand() // Setup commands
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "E3_StudentNameAndId_GetByClassId",
                    Connection = conn
                };

                // Setup parameters
                var param = new SqlParameter { ParameterName = "classPageId", Value = classId };
                cmd.Parameters.Add(param);
                param = new SqlParameter { ParameterName = "userId", Value = userId };
                cmd.Parameters.Add(param);

                // Create a generic datatable
                var dt = new DataTable();

                // Add two columns to datatable for Name and Value
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                // Create instance of SQLDataReader and execute the command we just created
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    // Now read the data.
                    while (reader.Read())
                    {
                        // Set the name/value pair and add a row to the datatable
                        name = reader["Student_Name"].ToString();
                        value = reader["ID"].ToString();
                        dt.Rows.Add(name, value);
                    }
                }

                // Pass the datatable row to be converted from a   
                // data row to a list item then append it to the list
                students.AddRange(from DataRow r in dt.Rows select GetNameFromDataRow(r));

                // Return the list
                return students;
            }
        }


        /// <summary>
        /// Creates a new student object from a data row and passes the object back 
        /// with all the properties of student ready to be set. Each ID_Encrypted 
        /// property will be set for the student in EncryptStudentIdAndDataBind.
        /// </summary>
        /// <param name="row">dt.Row comes from the GetStudentsFromClassId</param>
        /// <returns>newStudentObj</returns>
        public static Student GetNameFromDataRow(DataRow row)
        {
            // new instance of a Student with the Name/Value pairs
            var newStudentObj = new Student((row["Name"].ToString()), Convert.ToInt32(row["Value"]));
            return newStudentObj; // return Name/Value fields
        }


        /// <summary>
        /// Fires from Page_Load event to load up the 
        /// classes into the dropdown list box on student
        /// list page for MA Roster Portlet
        /// </summary>
        protected void LoadClassesIntoDropDown()
        {
            // Sort the list of classes by Name
            teacherClasses = (from tc in teacherClasses orderby tc.ClassName select tc).ToList();

            // Identify the DataSource, ValueField, and Text Shown then bind the data
            dropDownClasses.DataSource = teacherClasses;
            dropDownClasses.DataValueField = "ID";
            dropDownClasses.DataTextField = "FriendlyName";
            dropDownClasses.DataBind();
        }

        protected void pieChart_Click(object sender, ChartClickEventArgs args)
        {
            if (args.SeriesItem != null)
            {
                var pieChartValue = string.Empty;
                switch (_level)
                {
                    case EntityTypes.School:
                        pieChartValue = args.SeriesItem.XValue.ToString();
                        break;
                    case EntityTypes.District:
                        pieChartValue = args.SeriesItem.Name;
                        break;
                }

                _sessionObject.StudentSearchParms.DeleteParm("PieChartValue");
                _sessionObject.StudentSearchParms.AddParm("PieChartValue", pieChartValue);

                #region  Student PieChart XmlHttpPanel - Depricated for the moment, but we should keep here for future enhancements
                //studentPieChartXmlHttpPanel.Attributes["level"] = studentsearch_hiddenLevel.Value;
                //studentPieChartXmlHttpPanel.Attributes["levelID"] = studentsearch_hiddenLevelID.Value;
                //studentPieChartXmlHttpPanel.Attributes["searchTileName"] = "students";
                //studentPieChartXmlHttpPanel.Attributes["controlURL"] = "../Controls/Student/StudentSearch_Expanded.aspx";
                //studentPieChartXmlHttpPanel.Value = "{\"EmptyString\":\"\"}";
                #endregion
            }
        }

        protected void LoadPieCharts()
        {
            if (_levelID <= 0) return;


            var taskList = new List<AsyncPageTask>
            {
                new AsyncPageTask(GetStudentCounts),
                new AsyncPageTask(LoadDistrictParms)
            };

            foreach (AsyncPageTask page in taskList)
            {
                PageAsyncTask newTask = new PageAsyncTask(page.OnBegin, page.OnEnd, page.OnTimeout, "StudentSearch", true);
                Page.RegisterAsyncTask(newTask);
            }
            taskList = null;
            Page.ExecuteRegisteredAsyncTasks();

            studentCountChart.DataSource = countData;
            studentCountChart.ChartTitle.Visible = false;
            studentCountChart.DataBind();

            //studentPieChartXmlHttpPanel.Value = "";   Don't need this particularly at the moment

            //Handle Legend Labels
            studentCountChart.Series.Clear();
            var nSeries = new ChartSeries();

            studentCountChart.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            studentCountChart.PlotArea.Appearance.FillStyle.FillType = Telerik.Charting.Styles.FillType.Solid;
            studentCountChart.PlotArea.Appearance.Border.Color = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");

            studentCountChart.PlotArea.XAxis.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 6);
            studentCountChart.PlotArea.YAxis.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 6);
            studentCountChart.PlotArea.XAxis.Appearance.LabelAppearance.Position.AlignedPosition = Telerik.Charting.Styles.AlignedPositions.TopRight;
            nSeries.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.Black;
            nSeries.Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 6);
            nSeries.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;

            var count = 0;

            //change to foreach for easier reading
            var gradeList = new List<string>();
            foreach (DataRow dr in countData.Rows)
            {
                var value = DataIntegrity.ConvertToDouble(dr["StudentCount"]);
                var xValue = 0;
                switch (_level)
                {
                    case EntityTypes.Teacher:
                        xValue = dr.Table.Columns.Contains("ClassID") ? DataIntegrity.ConvertToInt(dr["ClassID"]) : 0;
                        break;
                    case EntityTypes.School:
                        gradeList.Add(dr.Table.Columns.Contains("GradeNumber") ? dr["GradeNumber"].ToString() : string.Empty);
                        xValue = gradeList.Count - 1;
                        break;
                }

                var myItem = new ChartSeriesItem(value) { Name = dr["Grade"].ToString(), XValue = xValue };
                myItem.Appearance.FillStyle.MainColor = System.Drawing.ColorTranslator.FromHtml(StyleController.GetPieChartColor(dr["Grade"].ToString(), count++)); ;
                myItem.Appearance.FillStyle.FillType = Telerik.Charting.Styles.FillType.Solid;

                if (value <= 0) myItem.Label.Visible = false;

                nSeries.Type = ChartSeriesType.Pie;
                nSeries.AddItem(myItem);
                studentCountChart.Series.Add(nSeries);

            }
        }

        /// <summary>
        /// Encrypts the Student's ID and 
        /// finishes up the data binding process
        /// </summary>
        /// <param name="studentRoster"></param>
        protected void EncryptStudentIdAndDataBind(List<Student> studentRoster)
        {
            /* iterate through students and set their ID_Encrypted
             * property for navigation to Student.aspx page     */

            foreach (var student in studentRoster)
            {
                student.IDEncrypted = student.ID.ToString();
            }

            // Sort
            studentRoster = (from sr in studentRoster orderby sr.StudentName select sr).ToList();

            //load list view
            rosterGrid.DataSource = studentRoster;
            rosterGrid.DataBind();
        }

        #endregion

        #region Events from front-end
        /// <summary>
        /// Fires from front-end when a data bind happens
        /// It's fires off logic that needs to be performed
        /// while the data is being bound.
        /// </summary>
        /// <param name="sender">the object</param>
        /// <param name="e">the objects GridItemEvent argument</param>
        protected void SetRtiImage(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var item = (GridDataItem)e.Item;
            }
            else if (e.Item is GridEditFormItem)
            {
                var item = (GridEditFormItem)e.Item;
            }
        }

        /// <summary>
        /// This will fire when the user selects a different class id from dropdown.
        /// </summary>
        /// <param name="sender">the object this event fires for</param>
        /// <param name="e">a specific attribute of the object's event argument</param>
        protected void dropDownClasses_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string classLookUpId = e.Value;
            var newStudentList = new List<Student>();
            newStudentList = GetStudentsFromClassId(Convert.ToInt32(classLookUpId), _userId);
            if (newStudentList != null) { EncryptStudentIdAndDataBind(newStudentList); }
        }

        /// <summary>
        /// OnItemDataBound event. Basic template that we might need
        /// to fire during an objects DataBinding event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SetClassList(object sender, RadComboBoxItemEventArgs e)
        {
            if (e.Item is RadComboBoxItem)
            {

                var item = e.Item;
            }
        }
        #endregion
    }
}