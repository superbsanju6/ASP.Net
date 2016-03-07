
using System;
using System.Data;
using System.Linq;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using System.Drawing;
using Thinkgate.Classes;

namespace Thinkgate.Controls.Student
{
    public partial class StudentChecklist : BasePage
    {
        public string StudentId
        {
            get
            {
                return Request.QueryString["studentId"] ?? Convert.ToString(Session["studentId"]);
            }
        }

        public string GradeLevel
        {
            get
            {
                if (Request.QueryString["grade"] != null)
                    return Request.QueryString["grade"] + " grade";
                else
                    return Convert.ToString(Session["grade"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = this.StudentId;
                string gradeLevel = this.GradeLevel;


                if ((id != null) && (gradeLevel != null || gradeLevel.Length != 0))
                {

                    System.Drawing.ColorConverter colConvert = new ColorConverter();
                    cmbGrade.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#99CCFF");

                    GetGradeData();

                    if (gradeLevel.Substring(0, 1) == "0")
                        gradeLevel = gradeLevel.Remove(0, 1);

                    RadComboBoxItem item = cmbGrade.FindItemByValue(Convert.ToString(gradeLevel.Substring(0, gradeLevel.IndexOf('t'))));
                    if (item != null)
                        item.Selected = true;

                    string StudentGrade = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.StudentGrade);

                    if (cmbGrade.Items.FindItemByText(StudentGrade) != null)
                        cmbGrade.Items.Remove(cmbGrade.Items.FindItemByText(StudentGrade));

                    GetCheckListData(Convert.ToInt32(id), gradeLevel);
                    GetIntroductionData(id, gradeLevel);
                }
            }
        }


        # region Events

        protected void cmbGrade_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (e.Value == "-1" || e.Value.Length < 0)
                return;

            string id = this.StudentId;
            string gradeLevel = e.Text;

            if (id != null && gradeLevel != null && gradeLevel.Length != 0)
            {
                Initialization();
                GetIntroductionData(id, gradeLevel);
                GetCheckListData(Convert.ToInt32(id), gradeLevel);

                string StudentGrade = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.StudentGrade);

                if (!e.Value.Equals("-1"))
                {
                    if (cmbGrade.Items.FindItemByText(StudentGrade) != null)
                        cmbGrade.Items.Remove(cmbGrade.Items.FindItemByText(StudentGrade));
                }
            }
        }

        # endregion

        # region Private Methods

        private void GetGradeData()
        {
            cmbGrade.Items.Clear();

            string GradeText = Convert.ToString(Thinkgate.Enums.CheckListLabelDescription.GradeText);
            string GradeValue = Convert.ToString(Thinkgate.Enums.CheckListLabelDescription.GradeValue);

            DataTable dtGrade = new DataTable();
            DataColumn gradeCol = dtGrade.Columns.Add(GradeText, typeof(String));

            var DParmsNew = DistrictParms.LoadDistrictParms();

            if (DParmsNew != null)
            {
                string grade = DParmsNew.GAPauldingGrades;

                if (grade != null)
                {
                    string[] arrGrade = grade.Split(',');

                    if (arrGrade != null || arrGrade.Length > 0)
                    {
                        foreach (string gradeLevel in arrGrade)
                        {
                            cmbGrade.Items.Add(new RadComboBoxItem(gradeLevel));
                            DataRow row = dtGrade.NewRow();
                            row["GradeText"] = gradeLevel;
                            dtGrade.Rows.Add(row);
                        }
                    }
                }


                dtGrade.Columns.Add(GradeValue, typeof(String));

                int i = 8;
                foreach (DataRow row in dtGrade.Rows)
                {
                    row["GradeValue"] = i;
                    i++;
                }


                cmbGrade.DataTextField = "GradeText";
                cmbGrade.DataValueField = "GradeValue";
                cmbGrade.DataSource = dtGrade;
                cmbGrade.DataBind();

                string StudentGrade = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.StudentGrade);

                if (cmbGrade.Items.FindItemByText(StudentGrade) != null)
                    cmbGrade.Items.Remove(cmbGrade.Items.FindItemByText(StudentGrade));

            }
        }

        private void GetIntroductionData(string id, string gradeLevel)
        {
            DataTable dtStudentChecklistInformation = Base.Classes.Student.GetStudentChecklistInformation(id);

            if (dtStudentChecklistInformation != null && dtStudentChecklistInformation.Rows.Count > 0)
            {
                lblStudentNameDisplay.Text = dtStudentChecklistInformation.Rows[0][0].ToString();
                lblSchoolNameDisplay.Text = dtStudentChecklistInformation.Rows[0][1].ToString();
                lblCounselorNameDisplay.Text = dtStudentChecklistInformation.Rows[0][2].ToString() + ' ' + dtStudentChecklistInformation.Rows[0][3].ToString();
            }
            DataTable getGradeIntroduction = Base.Classes.Student.GetGradeIntroduction();

            if (getGradeIntroduction != null && getGradeIntroduction.Rows.Count > 0)
            {
                int grade = Convert.ToInt16(gradeLevel.Substring(0, gradeLevel.IndexOf('t')));

                switch (grade)
                {
                    case (int)Thinkgate.Enums.StudentChecklistEnum.Eight:
                        lblGradeName.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.EightGrade);
                        lblIntroduction.Text = Convert.ToString(getGradeIntroduction.Rows[0][1]);
                        break;
                    case (int)Thinkgate.Enums.StudentChecklistEnum.Ninth:
                        lblGradeName.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.NinthGrade);
                        lblIntroduction.Text = Convert.ToString(getGradeIntroduction.Rows[1][1]);
                        break;
                    case (int)Thinkgate.Enums.StudentChecklistEnum.Tenth:
                        lblGradeName.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.TenthGrade);
                        lblIntroduction.Text = Convert.ToString(getGradeIntroduction.Rows[2][1]);
                        break;
                    case (int)Thinkgate.Enums.StudentChecklistEnum.Eleventh:
                        lblGradeName.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.EleventhGrade);
                        lblIntroduction.Text = Convert.ToString(getGradeIntroduction.Rows[3][1]);
                        break;
                    case (int)Thinkgate.Enums.StudentChecklistEnum.Twelfth:
                        lblGradeName.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.TwelfthGrade);
                        lblIntroduction.Text = Convert.ToString(getGradeIntroduction.Rows[4][1]);
                        break;
                    default:
                        lblIntroduction.Text = string.Empty;
                        break;
                }
            }
        }


        public void GetCheckListData(int id, string gradeLevel)
        {
            if (gradeLevel != Convert.ToString(Thinkgate.Enums.CheckListLabelDescription.StudentGrade))
            {
                int grade = Convert.ToInt16(gradeLevel.Substring(0, gradeLevel.IndexOf('t')));

                var studentchecklist = Base.Classes.Student.GetStudentChecklistInfo(id, grade);

                var queryCustomersChecklistJuly = from checklist in studentchecklist.AsEnumerable()
                                                  where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.July))
                                                  select checklist;

                var queryCustomersChecklistAugust = from checklist in studentchecklist.AsEnumerable()
                                                    where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.August))
                                                    select checklist;

                var queryCustomersChecklistSeptember = from checklist in studentchecklist.AsEnumerable()
                                                       where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.September))
                                                       select checklist;

                var queryCustomersChecklistOctober = from checklist in studentchecklist.AsEnumerable()
                                                     where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.October))
                                                     select checklist;

                var queryCustomersChecklistNovember = from checklist in studentchecklist.AsEnumerable()
                                                      where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.November))
                                                      select checklist;

                var queryCustomersChecklistDecember = from checklist in studentchecklist.AsEnumerable()
                                                      where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.December))
                                                      select checklist;

                var queryCustomersChecklistJanuary = from checklist in studentchecklist.AsEnumerable()
                                                     where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.January))
                                                     select checklist;

                var queryCustomersChecklistFebruary = from checklist in studentchecklist.AsEnumerable()
                                                      where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.February))
                                                      select checklist;

                var queryCustomersChecklistMarch = from checklist in studentchecklist.AsEnumerable()
                                                   where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.March))
                                                   select checklist;

                var queryCustomersChecklistApril = from checklist in studentchecklist.AsEnumerable()
                                                   where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.April))
                                                   select checklist;

                var queryCustomersChecklistMay = from checklist in studentchecklist.AsEnumerable()
                                                 where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.May))
                                                 select checklist;

                var queryCustomersChecklistJune = from checklist in studentchecklist.AsEnumerable()
                                                  where checklist.Field<string>("Month").Equals(Convert.ToString(Thinkgate.Enums.Month.June))
                                                  select checklist;

                // show/hide is handled in the GridBind
                this.GridBind(this.rgStudentChecklistJuly, queryCustomersChecklistJuly);
                this.GridBind(this.rgStudentChecklistAugust, queryCustomersChecklistAugust);
                this.GridBind(this.rgStudentChecklistSeptember, queryCustomersChecklistSeptember);
                this.GridBind(this.rgStudentChecklistOctober, queryCustomersChecklistOctober);
                this.GridBind(this.rgStudentChecklistNovember, queryCustomersChecklistNovember);
                this.GridBind(this.rgStudentChecklistDecember, queryCustomersChecklistDecember);
                this.GridBind(this.rgStudentChecklistJanuary, queryCustomersChecklistJanuary);
                this.GridBind(this.rgStudentChecklistFebruary, queryCustomersChecklistFebruary);
                this.GridBind(this.rgStudentChecklistMarch, queryCustomersChecklistMarch);
                this.GridBind(this.rgStudentChecklistApril, queryCustomersChecklistApril);
                this.GridBind(this.rgStudentChecklistMay, queryCustomersChecklistMay);
                this.GridBind(this.rgStudentChecklistJune, queryCustomersChecklistJune);
            }
        }

        public void GridBind(RadGrid rgv, EnumerableRowCollection<DataRow> queryCustomersChecklist)
        {
            if (queryCustomersChecklist.Count() != 0)
            {
                DataTable dtCustomersChecklist = queryCustomersChecklist.CopyToDataTable();
                if (dtCustomersChecklist != null && dtCustomersChecklist.Rows.Count > 0)
                    rgv.DataSource = dtCustomersChecklist;
            }
            else
                rgv.DataSource = null;

            rgv.DataBind();
            rgv.Parent.Visible = rgv.DataSource != null;
        }


        public void Initialization()
        {
            lblGradeName.Text = string.Empty;
            lblStudentNameDisplay.Text = string.Empty;
            lblCounselorNameDisplay.Text = string.Empty;
            lblSchoolNameDisplay.Text = string.Empty;
            lblIntroduction.Text = string.Empty;
            rgStudentChecklistAugust.DataSource = new string[] { };
            rgStudentChecklistSeptember.DataSource = new string[] { };
            rgStudentChecklistOctober.DataSource = new string[] { };
            rgStudentChecklistNovember.DataSource = new string[] { };
            rgStudentChecklistDecember.DataSource = new string[] { };
            rgStudentChecklistJanuary.DataSource = new string[] { };
            rgStudentChecklistFebruary.DataSource = new string[] { };
            rgStudentChecklistMarch.DataSource = new string[] { };
            rgStudentChecklistApril.DataSource = new string[] { };
            rgStudentChecklistMay.DataSource = new string[] { };
            rgStudentChecklistJune.DataSource = new string[] { };

        }
        # endregion


    }
}



