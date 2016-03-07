
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using Thinkgate.Classes.Search;
using Thinkgate.Controls.E3Criteria.CriteriaControls;
using Thinkgate.Enums.Search;

namespace Thinkgate.Controls.E3Criteria
{
    public partial class SearchControl : UserControl
    {
        #region Properties

        public Criteria Criteria { get; set; }
        public int ToolTipDelayHide { get; set; }  //in seconds

        #endregion

        #region Page Events

        protected override void OnInit(EventArgs e)
        {
            Criteria = new Criteria();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterCriteria();
                RenderCriteria();
            }
        }

        #endregion

        #region Public Methods

        public string GetCriteriaJson()
        {
            return hdnSearchControlSchema.Value;
        }

        public Criteria GetCriteria()
        {
            string criteriaString = hdnSearchControlSchema.Value;
            List<Criterion> criterion = (List<Criterion>)(new JavaScriptSerializer().Deserialize(criteriaString, typeof(List<Criterion>)));

            Criteria criteria = new Criteria();
            criteria.Criterias = criterion;

            return criteria;
        }

        #endregion

        #region Private Methods

        private void RegisterCriteria()
        {
            if (Criteria != null && Criteria.Criterias != null && Criteria.Criterias.Count > 0)
            {
                StringBuilder javaScriptBuilder = new StringBuilder();

                
                ClientScriptManager cs = Page.ClientScript;
                if (cs.IsClientScriptBlockRegistered(GetType(), "Thinkgate.Controls.Search"))
                {
                    javaScriptBuilder.Append("var searchControlSchemaLMRI = ");
                    javaScriptBuilder.Append(Criteria.ToJsonString());
                    javaScriptBuilder.Append("; ");
                    javaScriptBuilder.Append(
                        "searchControlSchema = JSON.parse(JSON.stringify(searchControlSchema) ).concat(JSON.parse(JSON.stringify(searchControlSchemaLMRI))); ");
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Thinkgate.Controls.SearchLMRI",
                        javaScriptBuilder.ToString(), true);
            }
                else
                {
                    javaScriptBuilder.Append("var searchControlSchema = ");
                    javaScriptBuilder.Append(Criteria.ToJsonString());
                    javaScriptBuilder.Append("; ");
                    hdnSearchControlSchema.Value = javaScriptBuilder.ToString();
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Thinkgate.Controls.Search",
                        javaScriptBuilder.ToString(), true);
        }
            }
        }

        private void RenderCriteria()
        {
            if (Criteria == null)
            {
                return;
            }
            if (Criteria.Criterias == null)
            {
                return;
            }
            if (Criteria.Criterias.Count == 0)
            {
                return;
            }

            hdnCriteriaToolTipDelayHide.Value = (ToolTipDelayHide * 1000).ToString(CultureInfo.InvariantCulture);
            foreach (Criterion criterion in Criteria.Criterias)
            {
                switch (criterion.UIType)
                {
                    case Base.Enums.UIType.DropDownList:
                        {
                            // The following dropdown control will replace existing E3 criteria dropdown.
                            Control dropdown = LoadControl("~/Controls/E3Criteria/CriteriaControls/DropDownList.ascx");

                            dropdown.ID = criterion.Key + "_DropdownList";
                            ((DropdownList)dropdown).Key = criterion.Key;
                            ((DropdownList)dropdown).UIType = criterion.UIType;
                            ((DropdownList)dropdown).Header = criterion.Header;
                            ((DropdownList)dropdown).DataTextField = criterion.DataTextField;
                            ((DropdownList)dropdown).DataValueField = criterion.DataValueField;
                            ((DropdownList)dropdown).DataSource = criterion.DataSource;
                            ((DropdownList)dropdown).DefaultValue = criterion.DefaultValue;
                            ((DropdownList)dropdown).Value = criterion.Value;
                            ((DropdownList)dropdown).Dependencies = criterion.Dependencies;
                            ((DropdownList)dropdown).Locked = criterion.Locked;
                            ((DropdownList)dropdown).IsRequired = criterion.IsRequired;
                            ((DropdownList)dropdown).HandlerName = criterion.HandlerName;
                            ((DropdownList)dropdown).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((DropdownList)dropdown).AutoHide = criterion.AutoHide;
                            dropdown.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(dropdown);
                            
                            break;
                        }
                    case Base.Enums.UIType.CheckBoxList:
                        {
                            // The following dropdown control will replace existing E3 criteria dropdown.
                            Control checkbox = LoadControl("~/Controls/E3Criteria/CriteriaControls/CheckBoxList.ascx");

                            checkbox.ID = criterion.Key + "_CheckBoxList";
                            ((CheckboxList)checkbox).Key = criterion.Key;
                            ((CheckboxList)checkbox).UIType = criterion.UIType;
                            ((CheckboxList)checkbox).Header = criterion.Header;
                            ((CheckboxList)checkbox).DataTextField = criterion.DataTextField;
                            ((CheckboxList)checkbox).DataValueField = criterion.DataValueField;
                            ((CheckboxList)checkbox).DataSource = criterion.DataSource;
                            ((CheckboxList)checkbox).DefaultValue = criterion.DefaultValue;
                            ((CheckboxList)checkbox).Value = criterion.Value;
                            ((CheckboxList)checkbox).Dependencies = criterion.Dependencies;
                            ((CheckboxList)checkbox).Locked = criterion.Locked;
                            ((CheckboxList)checkbox).IsRequired = criterion.IsRequired;
                            ((CheckboxList)checkbox).HandlerName = criterion.HandlerName;
                            ((CheckboxList)checkbox).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((CheckboxList)checkbox).AutoHide = criterion.AutoHide;
                            checkbox.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(checkbox);

                            break;
                        }

                    case Base.Enums.UIType.DateRange:
                        {
                            // The following dropdown control will replace existing E3 criteria dropdown.
                            Control dateRange = LoadControl("~/Controls/E3Criteria/CriteriaControls/DateRange.ascx");

                            dateRange.ID = criterion.Key + "_DateRange";
                            ((CriteriaControls.DateRange)dateRange).Key = criterion.Key;
                            ((CriteriaControls.DateRange)dateRange).UIType = criterion.UIType;
                            ((CriteriaControls.DateRange)dateRange).Header = criterion.Header;
                            ((CriteriaControls.DateRange)dateRange).DefaultValue = criterion.DefaultValue;
                            ((CriteriaControls.DateRange)dateRange).Value = criterion.Value;
                            ((CriteriaControls.DateRange)dateRange).Dependencies = criterion.Dependencies;
                            ((CriteriaControls.DateRange)dateRange).Locked = criterion.Locked;
                            ((CriteriaControls.DateRange)dateRange).IsRequired = criterion.IsRequired;
                            ((CriteriaControls.DateRange)dateRange).HandlerName = criterion.HandlerName;
                            ((CriteriaControls.DateRange)dateRange).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((CriteriaControls.DateRange)dateRange).AutoHide = criterion.AutoHide;
                            dateRange.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(dateRange);

                            break;
                        }

                    case Base.Enums.UIType.TextBox:
                        {
                            // The following dropdown control will replace existing E3 criteria dropdown.
                            Control text = LoadControl("~/Controls/E3Criteria/CriteriaControls/TextWithDropdown.ascx");

                            text.ID = criterion.Key + "_TextWithDropdown";
                            ((CriteriaControls.TextWithDropdown)text).Key = criterion.Key;
                            ((CriteriaControls.TextWithDropdown)text).UIType = criterion.UIType;
                            ((CriteriaControls.TextWithDropdown)text).Header = criterion.Header;
                            ((CriteriaControls.TextWithDropdown)text).DataTextField = criterion.DataTextField;
                            ((CriteriaControls.TextWithDropdown)text).DataValueField = criterion.DataValueField;
                            ((CriteriaControls.TextWithDropdown)text).DataSource = criterion.DataSource;
                            ((CriteriaControls.TextWithDropdown)text).DefaultValue = criterion.DefaultValue;
                            ((CriteriaControls.TextWithDropdown)text).Value = criterion.Value;
                            ((CriteriaControls.TextWithDropdown)text).Dependencies = criterion.Dependencies;
                            ((CriteriaControls.TextWithDropdown)text).Locked = criterion.Locked;
                            ((CriteriaControls.TextWithDropdown)text).IsRequired = criterion.IsRequired;
                            ((CriteriaControls.TextWithDropdown)text).HandlerName = criterion.HandlerName;
                            ((CriteriaControls.TextWithDropdown)text).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((CriteriaControls.TextWithDropdown)text).AutoHide = criterion.AutoHide;
                            text.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(text);

                            break;
                        }

                    case Base.Enums.UIType.TextBoxEdit:
                        {
                            // The following dropdown control will replace existing E3 criteria dropdown.
                            Control text = LoadControl("~/Controls/E3Criteria/CriteriaControls/TextBoxEdit.ascx");

                            text.ID = criterion.Key + "_TextBoxEdit";
                            ((TextBoxEdit)text).Key = criterion.Key;
                            ((TextBoxEdit)text).UIType = criterion.UIType;
                            ((TextBoxEdit)text).Header = criterion.Header;
                            ((TextBoxEdit)text).DataTextField = criterion.DataTextField;
                            ((TextBoxEdit)text).DataValueField = criterion.DataValueField;
                            ((TextBoxEdit)text).DataSource = criterion.DataSource;
                            ((TextBoxEdit)text).DefaultValue = criterion.DefaultValue;
                            ((TextBoxEdit)text).Value = criterion.Value;
                            ((TextBoxEdit)text).Dependencies = criterion.Dependencies;
                            ((TextBoxEdit)text).Locked = criterion.Locked;
                            ((TextBoxEdit)text).IsRequired = criterion.IsRequired;
                            ((TextBoxEdit)text).HandlerName = criterion.HandlerName;
                            ((TextBoxEdit)text).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((TextBoxEdit)text).AutoHide = criterion.AutoHide;
                            ((TextBoxEdit) text).EditMask = criterion.EditMask;
                            ((TextBoxEdit) text).DecimalPositions = criterion.DecimalPositions;
                            text.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(text);

                            break;
                        }
                    case Base.Enums.UIType.Duration:
                        {
                            // The following dropdown control will replace existing E3 criteria dropdown.
                            Control text = LoadControl("~/Controls/E3Criteria/CriteriaControls/DurationControl.ascx");

                            text.ID = criterion.Key + "_Duration";
                            ((DurationControl)text).Key = criterion.Key;
                            ((DurationControl)text).UIType = criterion.UIType;
                            ((DurationControl)text).Header = criterion.Header;
                            ((DurationControl)text).DataTextField = criterion.DataTextField;
                            ((DurationControl)text).DataValueField = criterion.DataValueField;
                            ((DurationControl)text).DataSource = criterion.DataSource;
                            ((DurationControl)text).DefaultValue = criterion.DefaultValue;
                            ((DurationControl)text).Value = criterion.Value;
                            ((DurationControl)text).Dependencies = criterion.Dependencies;
                            ((DurationControl)text).Locked = criterion.Locked;
                            ((DurationControl)text).IsRequired = criterion.IsRequired;
                            ((DurationControl)text).HandlerName = criterion.HandlerName;
                            ((DurationControl)text).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((DurationControl)text).AutoHide = criterion.AutoHide;
                            ((DurationControl)text).EditMask = criterion.EditMask;
                            ((DurationControl)text).DecimalPositions = criterion.DecimalPositions;
                            text.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(text);

                            break;
                        }
                    case Base.Enums.UIType.TextBoxEditRange:
                        {
                            // The following dropdown control will replace existing E3 criteria dropdown.
                            Control text = LoadControl("~/Controls/E3Criteria/CriteriaControls/TextBoxEditRange.ascx");

                            text.ID = criterion.Key + "_TextBoxEditRange";
                            ((TextBoxEditRange)text).Key = criterion.Key;
                            ((TextBoxEditRange)text).UIType = criterion.UIType;
                            ((TextBoxEditRange)text).Header = criterion.Header;
                            ((TextBoxEditRange)text).DataTextField = criterion.DataTextField;
                            ((TextBoxEditRange)text).DataValueField = criterion.DataValueField;
                            ((TextBoxEditRange)text).DataSource = criterion.DataSource;
                            ((TextBoxEditRange)text).DefaultValue = criterion.DefaultValue;
                            ((TextBoxEditRange)text).Value = criterion.Value;
                            ((TextBoxEditRange)text).Dependencies = criterion.Dependencies;
                            ((TextBoxEditRange)text).Locked = criterion.Locked;
                            ((TextBoxEditRange)text).IsRequired = criterion.IsRequired;
                            ((TextBoxEditRange)text).HandlerName = criterion.HandlerName;
                            ((TextBoxEditRange)text).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((TextBoxEditRange)text).AutoHide = criterion.AutoHide;
                            ((TextBoxEditRange)text).EditMask = criterion.EditMask;
                            ((TextBoxEditRange)text).DecimalPositions = criterion.DecimalPositions;
                            text.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(text);

                            break;
                        }
                    case Base.Enums.UIType.GradeSubjectCurricula:
                        {
                            Control currCtrl = LoadControl("~/Controls/E3Criteria/CriteriaControls/GradeSubjectCurriculum.ascx");

                            currCtrl.ID = criterion.Key + "_GradesSubjectsCurricula";
                            ((GradeSubjectCurriculum)currCtrl).Key = criterion.Key;
                            ((GradeSubjectCurriculum)currCtrl).UIType = criterion.UIType;
                            ((GradeSubjectCurriculum)currCtrl).Header = criterion.Header;
                            ((GradeSubjectCurriculum)currCtrl).GradesDataSource = criterion.GradesDataSource;
                            ((GradeSubjectCurriculum)currCtrl).SubjectsDataSource = criterion.SubjectsDataSource;
                            ((GradeSubjectCurriculum)currCtrl).CurriculaDataSource = criterion.CurriculaDataSource;
                            ((GradeSubjectCurriculum)currCtrl).DefaultValue = criterion.DefaultValue;
                            ((GradeSubjectCurriculum)currCtrl).Value = criterion.Value;
                            ((GradeSubjectCurriculum)currCtrl).Dependencies = criterion.Dependencies;
                            ((GradeSubjectCurriculum)currCtrl).Locked = criterion.Locked;
                            ((GradeSubjectCurriculum)currCtrl).IsRequired = criterion.IsRequired;
                            ((GradeSubjectCurriculum)currCtrl).HandlerName = criterion.HandlerName;
                            ((GradeSubjectCurriculum)currCtrl).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((GradeSubjectCurriculum)currCtrl).AutoHide = criterion.AutoHide;
                            currCtrl.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(currCtrl);
                            break;
                        }
                    case Base.Enums.UIType.GradeSubjectStandards:
                        {
                            Control standardCtrl = LoadControl("~/Controls/E3Criteria/CriteriaControls/GradeSubjectStandards.ascx");

                            standardCtrl.ID = criterion.Key + "_GradesSubjectsStandards";
                            ((GradeSubjectStandards)standardCtrl).Key = criterion.Key;
                            ((GradeSubjectStandards)standardCtrl).UIType = criterion.UIType;
                            ((GradeSubjectStandards)standardCtrl).Header = criterion.Header;
                            ((GradeSubjectStandards)standardCtrl).StandardSetDataSource = criterion.StandardSetDataSource;
                            ((GradeSubjectStandards)standardCtrl).GradesDataSource = criterion.GradesDataSource;
                            ((GradeSubjectStandards)standardCtrl).SubjectsDataSource = criterion.SubjectsDataSource;
                            ((GradeSubjectStandards)standardCtrl).CourseDataSource = criterion.CourseDataSource;
                            ((GradeSubjectStandards)standardCtrl).StandardsDataSource = criterion.StandardsDataSource;
                            ((GradeSubjectStandards)standardCtrl).DefaultValue = criterion.DefaultValue;
                            ((GradeSubjectStandards)standardCtrl).Value = criterion.Value;
                            ((GradeSubjectStandards)standardCtrl).Dependencies = criterion.Dependencies;
                            ((GradeSubjectStandards)standardCtrl).Locked = criterion.Locked;
                            ((GradeSubjectStandards)standardCtrl).IsRequired = criterion.IsRequired;
                            ((GradeSubjectStandards)standardCtrl).HandlerName = criterion.HandlerName;
                            ((GradeSubjectStandards)standardCtrl).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((GradeSubjectStandards)standardCtrl).AutoHide = criterion.AutoHide;
                            standardCtrl.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(standardCtrl);
                            break;
                        }
                     case Base.Enums.UIType.SchoolGradeName:
                        {
                            Control TeacherCtrl = LoadControl("~/Controls/E3Criteria/CriteriaControls/SchoolGradeName.ascx");

                            TeacherCtrl.ID = criterion.Key + "_SchoolsGradesNames";
                            ((SchoolGradeName)TeacherCtrl).Key = criterion.Key;
                            ((SchoolGradeName)TeacherCtrl).UIType = criterion.UIType;
                            ((SchoolGradeName)TeacherCtrl).Header = criterion.Header;
                            ((SchoolGradeName)TeacherCtrl).GradesDataSource = criterion.GradesDataSource;
                            ((SchoolGradeName)TeacherCtrl).SchoolDataSource = criterion.SchoolDataSource;
                            ((SchoolGradeName)TeacherCtrl).TeacherNameDataSource = criterion.TeacherNameDataSource;
                            ((SchoolGradeName)TeacherCtrl).DefaultValue = criterion.DefaultValue;
                            ((SchoolGradeName)TeacherCtrl).Value = criterion.Value;
                            ((SchoolGradeName)TeacherCtrl).Dependencies = criterion.Dependencies;
                            ((SchoolGradeName)TeacherCtrl).Locked = criterion.Locked;
                            ((SchoolGradeName)TeacherCtrl).IsRequired = criterion.IsRequired;
                            ((SchoolGradeName)TeacherCtrl).HandlerName = criterion.HandlerName;
                            ((SchoolGradeName)TeacherCtrl).IsUpdatedByUser = criterion.IsUpdatedByUser;
                            ((SchoolGradeName)TeacherCtrl).AutoHide = criterion.AutoHide;
                            TeacherCtrl.Visible = criterion.Visible;

                            divCriteriaPlaceHolder.Controls.Add(TeacherCtrl);
                            break;
                        }
                }
            }
        }

        #endregion
    }
}