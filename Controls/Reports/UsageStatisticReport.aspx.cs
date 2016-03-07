
using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.UsageStatisticsReport;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using Thinkgate.Classes.UsageStatisticsReport;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Domain.Classes;
using Thinkgate.Enums;
using Thinkgate.Services.Contracts.ServiceModel;
using Thinkgate.Services.Contracts.UsageStatistics;

namespace Thinkgate.Controls.Reports
{
    public partial class UsageStatisticReport : BasePage
    {
        private const string ROLE_DISTRICTADMIN = "District Administrator";
        private const string ROLE_SCHOOLADMIN = "School Administrator";
        private const string ROLE_TEACHER = "Teacher";
        private const string ROLE_SCHOOLSUPPORT = "School Support";
        private const string ROLE_NONADMINISTRATOR = "Non Administrator";

        static public ItemSearchModes SearchMode;
        static public ItemFilterModes FilterMode;
        protected string _Dok;
        private SessionObject _sessionObject;
        private string _clientID = string.Empty;
        private string _schoolName = default(string);
        

        protected new void Page_Init(object sender, EventArgs e)
        {
            /*
            bool hasPermission = UserHasPermission(Base.Enums.Permission.Access_UsageReport);
            if (!hasPermission)
            {
                Response.Redirect("~/UnauthorizedAccess.aspx", true);
                return;
            }
            */

            base.Page_Init(sender, e);
            Master.Search += (SearchHandler);

            DistrictParms parms = DistrictParms.LoadDistrictParms();
            _clientID = parms.ClientID;

            if (_sessionObject == null)
            {
                _sessionObject = (SessionObject)Session["SessionObject"];
            }

            if (!IsPostBack)
            {
                var rolePortal = (RolePortal)_sessionObject.LoggedInUser.Roles.Where(w => w.RolePortalSelection != 0).Min(m => m.RolePortalSelection);
                /* Check rolePortal */
                var itemlist = LevelItem();

                switch (rolePortal)
                {
                    case RolePortal.District:
                        {
                            cmbLevel.DataSource = itemlist;
                            string Level = itemlist.FirstOrDefault().Name.ToString();
                            cmbLevel.DefaultTexts = new List<string> { Level };
                            break;

                        }
                    case RolePortal.School:
                        {
                            var lst = from dd in itemlist
                                      where Convert.ToInt32(dd.Value) >= 2
                                      select dd;
                            cmbLevel.DataSource = lst;
                            string LevelName = lst.FirstOrDefault().Name.ToString();
                            cmbLevel.DefaultTexts = new List<string> { LevelName };
                            break;
                        }
                    case RolePortal.Teacher:
                        {
                            var lst = from dd in itemlist
                                      where Convert.ToInt32(dd.Value) == 3
                                      select dd;
                            cmbLevel.DataSource = lst;

                            if (lst.Count() == 1)
                            {
                                string Level = lst.LastOrDefault().Name.ToString();
                                cmbLevel.DefaultTexts = new List<string> { Level };
                                cmbLevel.ReadOnly = true;
                            }
                            break;
                        }
                }
                cmbLevel.DataBind();

                #region SchoolType/School

                var schoolList = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);

                DataTable dtSchoolList = new DataTable();
                dtSchoolList.Columns.Add("Value", typeof(int));

                foreach (Base.Classes.School school in schoolList)
                {
                    DataRow dr = dtSchoolList.NewRow();
                    dr["Value"] = school.ID;
                    dtSchoolList.Rows.Add(dr);
                }

                /* Add a 'All' SchoolType option */               
                var TypeCount = schoolList.Select(x => x.Type).Distinct().Count();
                if (TypeCount > 1)
                {
                    schoolList.Insert(0, new Base.Classes.School() { ID = 0, Name = string.Empty, Type = "All" });
                }

                string schooltypes = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(BuildJsonArray(schoolList));

                ctrlRegionSchoolTypeSchool.JsonDataSource = schooltypes;
               // ctrlRegionSchoolTypeSchool.CmbSchoolType.DefaultTexts = PossibleDefaultTexts(Request.QueryString["SchoolType"]);
               // ctrlRegionSchoolTypeSchool.CmbSchool.DefaultTexts = PossibleDefaultTexts(Request.QueryString["School"]);
                #endregion

                cblComponentType.DataSource = ComponentType();
                ddlView.DataSource = ReportView();
                ddlView.DataBind();
                if (ddlView.Items.Count > 0)
                {
                    ddlView.SelectedIndex = 1;
                }

                string SchoolType = ctrlRegionSchoolTypeSchool.CmbSchoolType.Text;
                int School = Convert.ToInt32(ctrlRegionSchoolTypeSchool.CmbSchool.DataValueField);

                DataTable dtResult = UsageStatisticMappings.GetSchoolTypeSchoolData(dtSchoolList);
                DataTable resultList = dtResult.Clone();

                if (_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_DISTRICTADMIN, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    resultList = dtResult;
                }
                else if (_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_SCHOOLADMIN, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    foreach (DataRow row in dtResult.Rows)
                    {
                        if (_sessionObject.LoggedInUser.Schools.Select(s => s.Id).ToList().Contains(int.Parse(row["School"].ToString())))
                        {
                            resultList.Rows.Add(row.ItemArray);
                        }
                    }
                }
                else if (_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_TEACHER, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    DataRow[] rows = dtResult.Select("UserId = '" + _sessionObject.LoggedInUser.UserId.ToString() + "' ");

                    foreach (DataRow drRow in rows)
                    {
                        resultList.Rows.Add(drRow.ItemArray);
                    }                   
                }

                ctrlUsageGradeSubjectCourse.JsonDataSource = (new JavaScriptSerializer()).Serialize(BuildGradeSubjectCourseJsonArray(resultList));
                ctrlUsageGradeSubjectCourse.CmbGrade.DefaultTexts = PossibleDefaultTexts(Request.QueryString["grade"]);
                ctrlUsageGradeSubjectCourse.CmbSubject.DefaultTexts = PossibleDefaultTexts(Request.QueryString["subject"]);
                ctrlUsageGradeSubjectCourse.CmbCourse.DefaultTexts = PossibleDefaultTexts(Request.QueryString["coursename"]);

                CalStartMonthYear.RadMonthYear.DateInput.ReadOnly = true;
                CalEndMonthYear.RadMonthYear.DateInput.ReadOnly = true;
            }
        }

        public ArrayList BuildJsonArray(List<Base.Classes.School> schools)
        {
            var arry = new ArrayList();
            foreach (var c in schools)
            {
                arry.Add(new object[]
                                 {
                                     c.Type, c.Name, c.ID
                                 });
            }
            return arry;
        }



        public ArrayList BuildGradeSubjectCourseJsonArray(DataTable resultSet)
        {
            var arry = new ArrayList();
            foreach (var c in resultSet.AsEnumerable())
            {
                arry.Add(new object[]
                                 {
                                     c["Grade"] , c["Subject"], c["Course"], c["Cluster"],c["SchoolType"],c["School"], c["SchoolName"] ,c["User_Full_Name"],c["UserId"]
                                 });
            }
            return arry;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["FileExport_Content_UsageStatisticReport"] != null)
                    Session["FileExport_Content_UsageStatisticReport"] = null;

            }
        }

        private List<String> PossibleDefaultTexts(object input)
        {
            if (input == null) return null;
            var list = new List<String>();
            list.AddRange(input.ToString().Split(','));
            return list;
        }

        /// <summary>
        /// Handler for the Search Button
        /// </summary>
        protected void SearchHandler(object sender, CriteriaController criteriaController)
        {
            UsageStatisticInputParameters parameters = BuildSearchCriteriaObject(SessionObject.LoggedInUser, criteriaController);
            string reportDataJSON = GetUsageStatistics(parameters);
            string reportDataJSONObject = "reportData = " + reportDataJSON + ";";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ReportData", reportDataJSONObject, true);
        }

        /// <summary>
        /// Builds the UsageStatisticReportCriteria. Used both from Search Button and from Service
        /// </summary>
        public UsageStatisticInputParameters BuildSearchCriteriaObject(ThinkgateUser user, CriteriaController criteriaController)
        {
            if (Session["FileExport_Content_UsageStatisticReport"] != null)
                Session["FileExport_Content_UsageStatisticReport"] = null;
            var level = criteriaController.ParseCriteria<DropDownList.ValueObject>("LevelFilter");

            drGeneric_String_String yearMonthStart = new drGeneric_String_String();
            foreach (var val in criteriaController.ParseCriteria<DateRange.ValueObject>("StartMonthYear"))
            {
                yearMonthStart.Add(val.Type == "Start" ? "YearMonthStart" : "YearMonthEnd", val.Date);
            }
            drGeneric_String_String yearMonthEnd = new drGeneric_String_String();
            foreach (var val in criteriaController.ParseCriteria<DateRange.ValueObject>("EndMonthYear"))
            {
                yearMonthEnd.Add(val.Type == "Start" ? "YearMonthStart" : "YearMonthEnd", val.Date);
            }
            var componentType = criteriaController.ParseCriteria<DropDownList.ValueObject>("ComponentType").OrderBy(c => c.Text);
            var region = criteriaController.ParseCriteria<DropDownList.ValueObject>("Region");
            var schoolType = criteriaController.ParseCriteria<DropDownList.ValueObject>("SchoolType");
            var school = criteriaController.ParseCriteria<DropDownList.ValueObject>("School");
            _schoolName = school.Any() ? school[0].Text : default(string);

            string userName = null;
            if (criteriaController.ParseCriteria<Text.ValueObject>("UserName").Count > 0)
            {
                userName = criteriaController.ParseCriteria<Text.ValueObject>("UserName")[0].Text;
            }
            if (_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_TEACHER, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                userName = _sessionObject.LoggedInUser.UserId.ToString();
            }

            /* Courses */
            var selectedGrades = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Grade").Select(x => x.Text).ToList();
            var selectedSubjects = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Subject").Select(x => x.Text).ToList();
            var selectedCourses = criteriaController.ParseCriteria<CheckBoxList.ValueObject>("Course").Select(x => x.Text).ToList();
            var selectedStandardSets = new drGeneric_String(criteriaController.ParseCriteria<CheckBoxList.ValueObject>("StandardSet").Select(x => x.Text));     // take straight to drGeneric_String because it's going to SQL

            /* Build criteria object to pass to SP */

            /*Parameters to be replaced with proper controls values*/
            var reportCriteria = new UsageStatisticInputParameters()
            {
                ClientID = _clientID,
                Year = DateTime.Now.Year.ToString(),
                Level = level.Count > 0 ? level[0].Text : null,
                ComponentType = string.Empty,
                StartYearMonth = yearMonthStart.Count > 0 ? String.Format("{0:yyyyMM}", Convert.ToDateTime(yearMonthStart[0].Value)) : null, /* Required parameter*/
                EndYearMonth = yearMonthEnd.Count > 0 ? String.Format("{0:yyyyMM}", Convert.ToDateTime(yearMonthEnd[0].Value)) : string.Format("{0:yyyyMM}", DateTime.Now),
                District = AppSettings.Demo_DistrictID.ToString(),
                Region = region.Count > 0 ? region[0].Value : null,
                SchoolType = schoolType.Count > 0 ? schoolType[0].Value : null,
                School = school.Count > 0 ? school[0].Value : null,
                User = !string.IsNullOrEmpty(userName) && level.Count > 0 && level[0].Text != "District" ? userName.Trim() : string.Empty,
                Grade = selectedGrades.Count > 0 ? selectedGrades[0] : null,
                Subject = selectedSubjects.Count > 0 ? selectedSubjects[0] : null,
                Course = selectedCourses.Count > 0 ? selectedCourses[0] : null,
            };
            SelectComponentTypeCriteria(componentType, reportCriteria);
            return reportCriteria;
        }

        private static void SelectComponentTypeCriteria(IOrderedEnumerable<DropDownList.ValueObject> componentType, UsageStatisticInputParameters reportCriteria)
        {
            foreach (DropDownList.ValueObject vo in componentType)
            {
                reportCriteria.ComponentType += vo.Text + ",";
            }
            if (reportCriteria.ComponentType.Trim().Length > 0)
            {
                reportCriteria.ComponentType = reportCriteria.ComponentType.Substring(0, reportCriteria.ComponentType.Length - 1);
            }
            if (reportCriteria.ComponentType.Trim().Length <= 0)
            {
                reportCriteria.ComponentType = "All";
            }
        }



        public string GetUsageStatistics(UsageStatisticInputParameters parameters)
        {
            var proxy =
                new UsageStatisticsProxy(new SamlSecurityTokenSettings
        {
            SamlSecurityTokenizerAction = SamlSecurityTokenizerAction.UseThreadPrincipalIdentity,
            ServiceCertificateStoreName = ConfigurationManager.AppSettings["ServiceCertificateStoreName"],
            ServiceCertificateThumbprint = ConfigurationManager.AppSettings["ServiceCertificateThumbprint"]
        });

           

            var udRaw = proxy.GetUsageStatistics(parameters);

            List<UsageStatisticData> lstUsageData = new List<UsageStatisticData>();

            if (udRaw != null && udRaw.Count() > 0)
            {

                if ((_sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_SCHOOLSUPPORT, StringComparison.InvariantCultureIgnoreCase)) != null ||
                    _sessionObject.LoggedInUser.Roles.Find(r => r.RoleName.Equals(ROLE_NONADMINISTRATOR, StringComparison.InvariantCultureIgnoreCase)) != null))
                /*If user role is school suport or non admin show only first record.*/
                {

                    lstUsageData.Add(udRaw.First());
                }
                else
                {
                    lstUsageData.AddRange(udRaw.Select(x => x));
                }
            }

            if (lstUsageData.Any())
            {
                lstUsageData.First().Level = parameters.Level;
                if (parameters.Level != "District")
                {
                    lstUsageData.First().School = _schoolName;
                }
            }


            UsageStatisticResultData objUsageStatistic = new UsageStatisticResultData();
            objUsageStatistic.PrepareReportData(parameters, lstUsageData);           
            ///*Prepares data for export when user clicks on export to excel*/
            CreateExcelStream(objUsageStatistic);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(objUsageStatistic);

        }
       

        /// <summary>
        /// Gets the value for query string parameter.
        /// </summary>
        /// <param name="parameterName">Name of query string parameter.</param>
        /// <returns>Returns a string containing the value of query string parameter.</returns>
        private static string GetQueryStringValue(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return null;
            }
            else
            {
                return HttpContext.Current.Request.QueryString.Get(parameterName);
            }
        }

        private static List<NameValue> LevelItem()
        {
            return new List<NameValue>
                {
                    new NameValue("District", "1"),
                    new NameValue("School", "2"),
                    new NameValue("User", "3")
                };
        }

        private static List<NameValue> ComponentType()
        {
            return new List<NameValue>
                {
                    new NameValue("All", "All"),
                    new NameValue("Profile", "Profile"),
                    new NameValue("Login","Login"),
                    new NameValue("Assessment","Assessment"),
                    new NameValue("Instruction","Instruction")
                };
        }

        private static List<NameValue> ReportView()
        {
            return new List<NameValue>
                {
                    new NameValue("Table", "Table"),
                    new NameValue("Graphical", "Graphical"),
                };
        }

        #region Export Grid to Excel

        protected void exportGridImgBtn_Click(object sender, ImageClickEventArgs e)
        {
            if (Session["FileExport_Content_UsageStatisticReport"] != null)
            {
                Byte[] fileExportContent = null;

                if (Session["FileExport_Content_UsageStatisticReport"] is Byte[])
                    fileExportContent = (Byte[])Session["FileExport_Content_UsageStatisticReport"];
                ExportByteArrayToExcel(fileExportContent);
            }

        }

        protected void ExportByteArrayToExcel(Byte[] byteArray)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=ExportData.xlsx");
            Response.BinaryWrite(byteArray);
            Response.End();
        }

        /// <summary>
        /// Prepare workbook and session  object to be exported based on data of usage statistics
        /// </summary>
        /// <param name="objUsageComplete"></param>
        public void CreateExcelStream(UsageStatisticResultData objUsageComplete)
        {
            XLWorkbook workbook = new XLWorkbook();
            workbook = GetWorkbook(objUsageComplete, "UsageStatisticsReport");
            if (workbook != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    Session["FileExport_Content_UsageStatisticReport"] = memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Preapare workbook object based on usage result list object
        /// </summary>
        /// <param name="objUsageComplete"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public XLWorkbook GetWorkbook(UsageStatisticResultData objUsageComplete, string sheet = "")
        {
            XLWorkbook workbook = new XLWorkbook();

            if (objUsageComplete != null && objUsageComplete.lstUsageData != null && objUsageComplete.lstUsageData.Count() > 0)
            {
                workbook.Worksheets.Add(sheet ?? "Sheet1");
                int iColumnStart = 2;
                int iRowStart = 4;
                int iHeaderColumnStart = 2;
                int iHeaderRowStart = 1;
                string InsertedOnDate = objUsageComplete.lstUsageData[0].InsertedOn;
                var UsageData = objUsageComplete.lstUsageData.Select(x => new { RowNumber = x.RowNumber, ClientID = x.ClientID, Level = x.Level, School = x.School, User = x.User, CurrentClasses = x.CurrentClasses, CurrentStudents = x.CurrentStudents, CurrentStaff = x.CurrentStaff, TotalLogins = x.TotalLogins, LoginUsers = x.LoginUsers, PctLoggedIn = x.PctLoggedIn, ClassroomTestCreated = x.ClassroomTestCreated, ClassroomTestAdministered = x.ClassroomTestAdministered, ClassroomTestResults = x.ClassroomTestResults, ClassroomPaperTestResults = x.ClassroomPaperTestResults, ClassroomOnlineTestResults = x.ClassroomOnlineTestResults, DistrictTestCreated = x.DistrictTestCreated, DistrictTestAdministered = x.DistrictTestAdministered, DistrictTestResults = x.DistrictTestResults, DistrictPaperTestResults = x.DistrictPaperTestResults, DistrictOnlineTestResults = x.DistrictOnlineTestResults, CurriculumMap = x.CurriculumMap, UnitPlan = x.UnitPlan, LessonPlan = x.LessonPlan, Other = x.Other }).ToList();
                /*Draw worksheet based on object list*/
                workbook.Worksheet(1).Cell(iRowStart, iColumnStart).InsertTable(UsageData.AsEnumerable(), false);
                workbook.Worksheet(1).Columns().AdjustToContents();

                if (objUsageComplete.lstMetaData != null && objUsageComplete.lstMetaData.Count() > 0)
                {
                    int i = objUsageComplete.lstMetaData.Count();

                    for (int cnt = 0; cnt < i; cnt++)
                    {
                        /*Delete any column not in meta-data list*/
                        if (!objUsageComplete.lstMetaData[cnt].data.Equals(workbook.Worksheet(1).Cell(iRowStart, cnt + iColumnStart).Value.ToString()))
                            workbook.Worksheet(1).Column(cnt + iColumnStart).Delete();
                        else
                            /*Rename title based on meta-data title*/
                            workbook.Worksheet(1).Cell(iRowStart, cnt + iColumnStart).Value = objUsageComplete.lstMetaData[cnt].title;
                    }

                    for (; i > 0; i--)
                    {
                        /*Delete any column not visible in meta-data list*/
                        if (!objUsageComplete.lstMetaData[i - 1].visible)
                            workbook.Worksheet(1).Column(i - 1 + iColumnStart).Delete();
                    }

                    i = objUsageComplete.lstMetaData.Where(column => column.visible == true).Count();
                    /*Set report date and format cells*/
                    workbook.Worksheet(1).Cell(iHeaderRowStart, iHeaderColumnStart).Value = "Results as of: " + InsertedOnDate;
                    workbook.Worksheet(1).Range(iHeaderRowStart, iHeaderColumnStart, iHeaderRowStart, i + iHeaderColumnStart - 1).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                }

                /*Add column group tiles and set color for group and columns title*/
                objUsageComplete.lstGridColumGroup.ForEach(oGroupTitle =>
                {
                    workbook.Worksheet(1).Cell(iRowStart - 1, iColumnStart).Value = oGroupTitle.GroupTitle;
                    workbook.Worksheet(1).Cell(iRowStart - 1, iColumnStart).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    workbook.Worksheet(1).Range(iRowStart - 1, iColumnStart, iRowStart - 1, iColumnStart + oGroupTitle.ColumnSpan - 1).Style.Fill.BackgroundColor = GetColor(oGroupTitle.GroupTitle, true);
                    workbook.Worksheet(1).Range(iRowStart, iColumnStart, iRowStart, iColumnStart + oGroupTitle.ColumnSpan - 1).Style.Fill.BackgroundColor = GetColor(oGroupTitle.GroupTitle, false);
                    workbook.Worksheet(1).Range(iRowStart - 1, iColumnStart, iRowStart, iColumnStart + oGroupTitle.ColumnSpan - 1).Style.Font.FontColor = (XLColor)XLColor.White;
                    workbook.Worksheet(1).Range(iRowStart - 1, iColumnStart, iRowStart - 1, iColumnStart + oGroupTitle.ColumnSpan - 1).Merge();
                    iColumnStart = iColumnStart + oGroupTitle.ColumnSpan;
                });

                return workbook;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return color object from HTML RGB color of each section based on  requirements
        /// </summary>
        /// <param name="strTitle"></param>
        /// <param name="isGroupTitle"></param>
        /// <returns></returns>

        private XLColor GetColor(string strTitle, bool isGroupTitle)
        {
            if (strTitle.Contains("Login")) return isGroupTitle ? (XLColor)XLColor.FromHtml("#000065") : (XLColor)XLColor.FromHtml("#366092");
            else if (strTitle.Contains("Classroom")) return isGroupTitle ? (XLColor)XLColor.FromHtml("#4e6128") : (XLColor)XLColor.FromHtml("#75923c");
            else if (strTitle.Contains("District")) return isGroupTitle ? (XLColor)XLColor.FromHtml("#964706") : (XLColor)XLColor.FromHtml("#e16b0a");
            else if (strTitle.Contains("Instruction")) return isGroupTitle ? (XLColor)XLColor.FromHtml("#963634") : (XLColor)XLColor.FromHtml("#c0504d");
            return (XLColor)XLColor.Gray;
        }


        #endregion
    }
}