
using System;
using System.ComponentModel;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Linq;
using System.Data;


namespace Thinkgate.Controls.Student
{
    public partial class StudentChecklistTile : TileControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetGradeData();

                var studentinformation = (Thinkgate.Base.Classes.Student)Tile.TileParms.GetParm("student");

                if (studentinformation != null)
                    Session["studentId"] = studentinformation.ID;
            }
        }

        #region Events

        protected void cmbGrade_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (e.Value == "-1" || e.Value.Length < 0)
                return;

            msgStudentGrade.Visible = false;
            msgStudentChecklist.Visible = true;

            int grade = Convert.ToInt16(e.Text.Substring(0, e.Text.IndexOf('t')));

            switch (grade)
            {
                case (int)Thinkgate.Enums.StudentChecklistEnum.Eight:
                    lblStudentChecklist.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.EightGrade);
                    break;
                case (int)Thinkgate.Enums.StudentChecklistEnum.Ninth:
                    lblStudentChecklist.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.NinthGrade);
                    break;
                case (int)Thinkgate.Enums.StudentChecklistEnum.Tenth:
                    lblStudentChecklist.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.TenthGrade);
                    break;
                case (int)Thinkgate.Enums.StudentChecklistEnum.Eleventh:
                    lblStudentChecklist.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.EleventhGrade);
                    break;
                case (int)Thinkgate.Enums.StudentChecklistEnum.Twelfth:
                    lblStudentChecklist.Text = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.TwelfthGrade);
                    break;
                default:
                    lblStudentChecklist.Text = string.Empty;
                    msgStudentGrade.Visible = true;
                    msgStudentChecklist.Visible = false;
                    break;
            }

            Session["grade"] = e.Text;

            string StudentGrade = Extensions.Description(Thinkgate.Enums.CheckListLabelDescription.StudentGrade);

            if (!e.Value.Equals("-1"))
            {
                if (cmbGrade.Items.FindItemByText(StudentGrade) != null)
                    cmbGrade.Items.Remove(cmbGrade.Items.FindItemByText(StudentGrade));
            }

        }

        #endregion

        #region Private Methods

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
                        foreach (string gradeLevel in arrGrade.Skip(0))
                        {
                            cmbGrade.Items.Add(new RadComboBoxItem(gradeLevel));
                            DataRow row = dtGrade.NewRow();
                            row["GradeText"] = gradeLevel;
                            dtGrade.Rows.Add(row);
                        }
                    }
                }

                dtGrade.Columns.Add(GradeValue, typeof(String));

                int i = 0;
                foreach (DataRow row in dtGrade.Rows)
                {
                    row["GradeValue"] = i;
                    i++;
                }

                cmbGrade.DataTextField = "GradeText";
                cmbGrade.DataValueField = "GradeValue";
                cmbGrade.DataSource = dtGrade;
                cmbGrade.DataBind();

            }
        }


        #endregion
    }
}


public static class Extensions
{
    public static string Description(this Enum o)
    {
        var attribute = o.GetType()
            .GetField(o.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() as DescriptionAttribute;
        return attribute == null ? o.ToString() : attribute.Description;
    }
}