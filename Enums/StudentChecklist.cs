
using System.ComponentModel;
using System;
using System.Configuration;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Web.UI;
using Standpoint.Core.Utilities;
using System.Web.Configuration;
using Thinkgate.Utilities;

namespace Thinkgate.Enums
{
    public enum StudentChecklistEnum
    {
      Eight = 8,
      Ninth = 9,
      Tenth = 10 ,
      Eleventh = 11,
      Twelfth =12
    };

    public enum Month
    {
        July,
        August,
        September,
        October,
        November,
        December,
        January,
        February,
        March,
        April,
        May,
        June
    };

    public enum CheckListLabelDescription
    {
        [Description("Student Grade")]
        StudentGrade,
        [Description("8th Grade Advisement Checklist for Parents")]
        EightGrade,
        [Description("Freshman Advisement Checklist for Parents")]
        NinthGrade,
        [Description("Sophomore Advisement Checklist for Parents")]
        TenthGrade,
        [Description("Junior Advisement Checklist for Parents")]
        EleventhGrade,
        [Description("Senior Advisement Checklist for Parents")]
        TwelfthGrade,
        GradeText,
        GradeValue
    };

    public enum ParentStudentPortalAccess
    {
        Parent,
        Guardian,        
        [Description("Edit Parent/Guardian Assignment for")]
        HeaderText,
        [Description("Student Information for")]
        StudentHeaderText,  
    };
}


