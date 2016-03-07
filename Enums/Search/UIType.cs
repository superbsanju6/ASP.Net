
using System;

namespace Thinkgate.Enums.Search
{
    [Serializable]
    public enum UIType
    {
        None = 0,   // do not render. may use in background
        CheckBoxList,
        DropDownList,
        TextBox,
        DatePicker,
        Tags,
        UITable,
        GradeSubjectCurricula,
        GradeSubjectStandards,
        Documents,
        TextBoxEdit,
        TextBoxEditRange
    }
}