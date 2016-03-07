using System;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Standpoint.Core.Utilities;
using System.Data;
using System.Web.UI.HtmlControls;
using Standpoint.Core.Classes;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.UI;

namespace Thinkgate.Controls.Class
{
    public partial class ClassSummary_Expanded : BasePage
    {
        private Base.Classes.Class _selectedClass;
        public string PageTitle;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            LoadRecordObject();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadClass();

            if (_selectedClass == null)
            {
                SessionObject.RedirectMessage = "No entity ID provided in URL.";
                Response.Redirect("~/PortalSelection.aspx", true);
            }

            _selectedClass.LoadStudents();
            rosterGrid.DataSource = _selectedClass.Students;
            rosterGrid.DataBind();

            studentCountSpan.InnerHtml = _selectedClass.Students.Count.ToString() + " students";

            PopulateProfileLabels(_selectedClass);
            BindDemographicData(_selectedClass);
            LoadCurriculumTable();
            LoadTeachers();

            Page.Title = _selectedClass.PrimaryTeacher + ": " + _selectedClass.Grade.GetFriendlyName() + " Grade " + _selectedClass.Subject.DisplayText + " - Period " + _selectedClass.Period;
        }

        private void LoadClass()
        {
            if (IsQueryStringMissingParameter(X_ID))
            {
                RedirectToPortalSelectionScreen();
            }
            else
            {
                if (RecordExistsInCache(Key))
                {
                    _selectedClass = (Base.Classes.Class)Base.Classes.Cache.Get(Key);
                }               
            }
        }

        private void BindDemographicData(Base.Classes.Class course)
        {
            var demographicDataSource = course.GetDemographicData();
            string rawData = "[";
            string labels = "[";

            //Load gender data to hidden inputs
            foreach(DataRow row in demographicDataSource.Tables["Gender"].Rows)
            {
                rawData += "['" + row["Gender"].ToString() + "', " + row["Percentage"].ToString() + ", " +
                           row["Count"].ToString() + "],";

                labels += "'" + row["Gender"].ToString() + "',";
            }
            genderRawData.Value = (rawData == "[" ? rawData : rawData.Substring(0, rawData.LastIndexOf(","))) + "]";
            genderLabels.Value = (labels == "[" ? labels : labels.Substring(0, labels.LastIndexOf(","))) + "]";
            //***********************************************************************

            //Reset rawData and labels
            rawData = "[";
            labels = "[";

            //Load race data to hidden inputs
            foreach (DataRow row in demographicDataSource.Tables["Races"].Rows)
            {
                if (DataIntegrity.ConvertToInt(row["Count"]) == 0) continue;

                rawData += "['" + row["Race"].ToString() + "', " + row["Percentage"].ToString() + ", " +
                           row["Count"].ToString() + "],";

                labels += "'" + row["Race"].ToString() + "',";
            }
            raceRawData.Value = (rawData == "[" ? rawData : rawData.Substring(0, rawData.LastIndexOf(","))) + "]";
            raceLabels.Value = (labels == "[" ? labels : labels.Substring(0, labels.LastIndexOf(","))) + "]";
            //***********************************************************************

            //Reset rawData and labels
            rawData = "[";
            labels = "[";

            //Load demographic subgroup data to hidden inputs
            foreach (DataRow row in demographicDataSource.Tables["Other"].Rows)
            {
                if (DataIntegrity.ConvertToInt(row["Count"]) == 0) continue;
                
                rawData += "['" + row["Demographic"].ToString() + "', " + row["Percentage"].ToString() + ", " +
                           row["Count"].ToString() + ", " + row["DemoField"].ToString() + "],";

                labels += "'" + row["Demographic"].ToString() + "',";
            }
            subgroupRawData.Value = (rawData == "[" ? rawData : rawData.Substring(0, rawData.LastIndexOf(","))) + "]";
            subgroupLabels.Value = (labels == "[" ? labels : labels.Substring(0, labels.LastIndexOf(","))) + "]";
            //***********************************************************************
        }

        private void PopulateProfileLabels(Thinkgate.Base.Classes.Class c)
        {
            lblGrade.Text = c.Grade.DisplayText;
            lblSubject.Text = c.Subject.DisplayText;
            lblCourse.Text = c.Course.CourseName;
            lblSection.Text = c.Section;
            lblPeriod.Text = c.Period;
            lblSemester.Text = c.Semester;
            lblYear.Text = c.Year;
            lblBlock.Text = c.Block;
            lblSchoolName.Text = c.School.Name;
        }

        private void LoadCurriculumTable()
        {
            DataTable curriculumDataTable = _selectedClass.GetCurriculumDataTable();
            int rowCount = 0;

            foreach (DataRow row in curriculumDataTable.Rows)
            {
                HtmlTableRow newRow = new HtmlTableRow();
                HtmlTableCell cell1 = new HtmlTableCell();
                //HtmlTableCell cell2 = new HtmlTableCell(); Removed cell2 data (Instructional Pacing) as per David

                cell1.InnerHtml = row["Grade"].ToString() + " " + row["Subject"].ToString() + "-" + row["Course"].ToString();
                /*
                string pacingGuide = (row["PacingGuide1"].ToString() == "" ? "" : row["PacingGuide1"].ToString() + ", ") +
                                     (row["PacingGuide2"].ToString() == "" ? "" : row["PacingGuide2"].ToString() + ", ") +
                                     (row["PacingGuide3"].ToString() == "" ? "" : row["PacingGuide3"].ToString() + ", ") +
                                     (row["PacingGuide4"].ToString() == "" ? "" : row["PacingGuide4"].ToString() + ", ") +
                                     (row["PacingGuide5"].ToString() == "" ? "" : row["PacingGuide5"].ToString() + ", ") +
                                     (row["PacingGuide6"].ToString() == "" ? "" : row["PacingGuide6"].ToString() + ", ") +
                                     (row["PacingGuide7"].ToString() == "" ? "" : row["PacingGuide7"].ToString() + ", ") +
                                     (row["PacingGuide8"].ToString() == "" ? "" : row["PacingGuide8"].ToString() + ", ") +
                                     (row["PacingGuide9"].ToString() == "" ? "" : row["PacingGuide9"].ToString() + ", ") +
                                     (row["PacingGuide10"].ToString() == "" ? "" : row["PacingGuide10"].ToString() + ", ") +
                                     (row["PacingGuide11"].ToString() == "" ? "" : row["PacingGuide11"].ToString() + ", ") +
                                     (row["PacingGuide12"].ToString() == "" ? "" : row["PacingGuide12"].ToString() + ", ") +
                                     (row["PacingGuideAll"].ToString() == "" ? "" : row["PacingGuideAll"].ToString());
                 

                cell2.Attributes["style"] = "color: #980000; font-weight:bold;";
                cell2.InnerHtml = pacingGuide.Substring(pacingGuide.Length - 2, 2) == ", "
                    ? pacingGuide.Substring(0, pacingGuide.Length - 2) : pacingGuide;
                */
                newRow.Cells.Add(cell1);
                //newRow.Cells.Add(cell2);

                curriculumTable.Rows.Add(newRow);
                rowCount++;

                if (rowCount == 4) break;
            }

            if(curriculumDataTable.Rows.Count > 4)
            {
                HtmlTableRow moreTextRow = new HtmlTableRow();
                HtmlTableCell moreTextCell1 = new HtmlTableCell()
                {
                    ColSpan = 2,
                    InnerHtml = "<a href=\"javascript:void(0);\" onclick=\"customDialog({ url: '../../ControlHost/PreviewCurriculums.aspx?xID=" + Request.QueryString["xID"].ToString() +
                        "', title: 'Curriculums', maximize: true, maxwidth: 300, maxheight: 300} ); return false;\" style=\"font-style:italic;\">See More...</a>"
                };

                moreTextRow.Cells.Add(moreTextCell1);
                curriculumTable.Rows.Add(moreTextRow);
            }

            curriculumTable.DataBind();
        }

        private void LoadTeachers()
        {
            if(_selectedClass.Teachers.Count == 0) _selectedClass.LoadTeachers();

            HtmlImage summaryImage = new HtmlImage();
            summaryImage.Src = "~/Images/summary.png";
            string summaryImageSrc = summaryImage.ResolveClientUrl(summaryImage.Src);

            HyperLink teacherOnclickLink = new HyperLink();
            string teacherOnclickLinkString;
            teacherOnclickLink.NavigateUrl = "~/Record/Teacher.aspx?childPage=yes&xID=";
            teacherOnclickLinkString = teacherOnclickLink.ResolveClientUrl(teacherOnclickLink.NavigateUrl);

            foreach (Thinkgate.Base.Classes.Teacher teacher in _selectedClass.Teachers)
            {
                HtmlTableRow newRow = new HtmlTableRow();
                HtmlTableCell cell1 = new HtmlTableCell();
                string xID = Encryption.EncryptInt(teacher.PersonID);

                if (UserHasPermission(Permission.Hyperlink_Teacher_ClassSummaryExpanded))
                {
                    /* User has permission so display Teacher Name as hyperlink. */
                    cell1.InnerHtml = "<img src=\"" + summaryImageSrc + "\" class=\"summaryImgButton\" style=\"display:none; cursor:pointer; margin-right:5px;\"/><a href=\"javascript:void();\" onclick=\"window.open('" +
                        teacherOnclickLinkString + xID + "'); return false;\">" + teacher.FirstName + " " + teacher.LastName + "</a> " + (teacher.IsPrimary ? "(Primary)" : "");
                }
                else
                {
                    /* User does not have permission so display Teacher Name as label. */
                    cell1.InnerHtml = "<img src=\"" + summaryImageSrc + "\" class=\"summaryImgButton\" style=\"display:none; cursor:pointer; margin-right:5px;\"/><label>" + 
                                      teacher.FirstName + " " + teacher.LastName + "</label> " + (teacher.IsPrimary ? "(Primary)" : "");
                }

                newRow.Cells.Add(cell1);
                teachersTable.Rows.Add(newRow);
            }

            teachersTable.DataBind();
        }

        protected void Roster_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                HyperLink studentNameLink = (HyperLink) item.FindControl("studentLink");
                Image summaryIcon = (Image)item.FindControl("summaryIcon");
                Image rtiImage = (Image)item.FindControl("RTIImage");
                Image accommodationsImage = (Image)item.FindControl("AccommodationsImage");
                Thinkgate.Base.Classes.Student student = (Thinkgate.Base.Classes.Student)item.DataItem;
                string rtiImageURL = (String.IsNullOrEmpty(student.RtiImage) ? "" : student.RtiImage);
                string accommodationsImageURL = (String.IsNullOrEmpty(student.AccommodationsImage) ? "" : student.AccommodationsImage);
                HyperLink link = (HyperLink)item["name"].Controls[0];
                link.ForeColor = System.Drawing.Color.Blue;
                link.NavigateUrl = "~/Record/Student.aspx?childPage=yes&xID=" + Encryption.EncryptInt(student.ID);  

               
                item.Attributes["gender"] = (student.Gender == Standpoint.Core.Enums.Gender.Female ? "Female" : "Male");
                item.Attributes["race"] = student.Race;
                item.Attributes["demoField1"] = student.DemoField1;
                item.Attributes["demoField2"] = student.DemoField2;
                item.Attributes["demoField3"] = student.DemoField3;
                item.Attributes["demoField4"] = student.DemoField4;
                item.Attributes["demoField5"] = student.DemoField5;
                item.Attributes["demoField6"] = student.DemoField6;
                item.Attributes["demoField7"] = student.DemoField7;
                item.Attributes["demoField8"] = student.DemoField8;
                item.Attributes["demoField9"] = student.DemoField9;
                item.Attributes["demoField10"] = student.DemoField10;

                if(studentNameLink != null)
                {
                    if (UserHasPermission(Base.Enums.Permission.Hyperlink_Student_ClassSummaryExpanded))
                    {
                        string xID = Encryption.EncryptInt(student.ID);
                        studentNameLink.NavigateUrl = "~/Record/Student.aspx?childPage=yes&xID=" + xID;
                        studentNameLink.Attributes["onclick"] = "window.open('" + studentNameLink.ResolveClientUrl(studentNameLink.NavigateUrl) + "'); return false;";
                        studentNameLink.Attributes["style"] = "color:#00F;";
                    }
                    studentNameLink.Text = student.StudentName;
                }

                if (summaryIcon != null && !UserHasPermission(Base.Enums.Permission.Icon_Summary_Student))
                {
                    summaryIcon.Attributes["style"] = "display: none;";
                }

                if (rtiImage != null && rtiImageURL.Length > 0 && rtiImageURL.IndexOf("blank") == -1)
                {
                    rtiImage.ImageUrl = "~/Images/" + rtiImageURL;
                }
                
                if(accommodationsImage != null && accommodationsImageURL.Length > 0 && accommodationsImageURL.IndexOf("blank") == -1)
                {
                    accommodationsImage.ImageUrl = "~/Images/" + accommodationsImageURL;
                }
            }
            else if (e.Item is GridEditFormItem)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
            }
        }

        protected void btnPrintStudentRoster_Click(object sender, ImageClickEventArgs e)
        {
            rosterGrid.ExportSettings.OpenInNewWindow = true;
            rosterGrid.ExportSettings.ExportOnlyData = true;
            foreach (GridColumn column in rosterGrid.MasterTableView.Columns)
            {
                column.HeaderStyle.Width = 150;
                if (column.UniqueName != "name" && column.UniqueName != "studentid")
                {
                    column.Visible = false;
                }
            }

            foreach (GridHeaderItem header in rosterGrid.MasterTableView.GetItems(GridItemType.Header))
            {
                foreach (TableCell cell in header.Cells)
                {
                    cell.Style.Add("text-align", "left");
                }
            }

            rosterGrid.MasterTableView.ExportToPdf();
        }

        protected override object LoadRecord(int xId)
        {
            return Base.Classes.Class.GetClassByID(xId);
        }

        #region Overrides of BasePage

        protected override string TypeKey
        {
            get { return EntityTypes.Class + "_"; }
        }

        #endregion
    }
}