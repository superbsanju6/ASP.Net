
using System.Collections.Generic;
using Thinkgate.Base.Enums;
using Thinkgate.Enums.Search;

namespace Thinkgate.Classes.Search
{
    public class Criterion : System.Web.UI.UserControl
    {
        #region Constructors

        public Criterion()
        {
            DataTextField = string.Empty;
            DataValueField = string.Empty;
            Key = string.Empty;
            Value = new KeyValuePair(string.Empty, string.Empty);
            Dependencies = new List<KeyValuePair>();
            Header = string.Empty;
            Locked = false;
            // TODO:  MERGE ISSUE
            //UIType = UIType.None;
            HandlerName = string.Empty;
            DefaultValue = null;
            IsUpdatedByUser = false;
            Visible = true;
            AutoHide = true;
            ToValue = new KeyValuePair(string.Empty, string.Empty);
            StandardSelection = new StandardSelections(null, null, null, null, null);
            CurriculumSelection = new CurriculumSelections(null, null, null, null);
            SchoolGradeNameSelection = new SchoolGradeNameSelections(null, null, null, null);
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets an unique identifier.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the DataSource text field.
        /// </summary>
        public string DataTextField { get; set; }

        /// <summary>
        /// Gets or sets the DataSource value field.
        /// </summary>
        public string DataValueField { get; set; }

        /// <summary>
        /// Gets or sets the title to be displayed.
        /// </summary>
        public string Header { get; set; }

        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the data to bind to this criterion object.
        /// </summary>
        public object DataSource { get; set; }

        /// <summary>
        /// Gets or sets the type of control to be used for this criterion object.
        /// </summary>
        public Thinkgate.Base.Enums.UIType UIType { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether this Criterion object is required to use.
        /// </summary>
        public bool IsRequired { get; set; }

        public KeyValuePair Value { get; set; }

        public List<KeyValuePair> Dependencies { get; set; }

        public string HandlerName { get; set; }

        public KeyValuePair DefaultValue { get; set; }

        public bool IsUpdatedByUser { get; set; }

        public object GradesDataSource { get; set; }
        public object SubjectsDataSource { get; set; }
        public object CurriculaDataSource { get; set; }

        public object StandardSetDataSource { get; set; }

        public object CourseDataSource { get; set; }
        public object StandardsDataSource { get; set; }
        public new bool Visible { get; set; }
        public bool AutoHide { get; set; }

        public Thinkgate.Base.Enums.EditMaskType EditMask { get; set; }
        public int DecimalPositions { get; set; }
        public KeyValuePair ToValue { get; set; }
        public StandardSelections StandardSelection { get; set; }
        public CurriculumSelections CurriculumSelection { get; set; }
        public object SchoolDataSource { get; set; }
        public object TeacherNameDataSource { get; set; }
        public SchoolGradeNameSelections SchoolGradeNameSelection { get; set; }

        #endregion

        #region Public Static Methods

        public static KeyValuePair CreateDependency(string key, string value = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                value = key;
            }
            return new KeyValuePair(key, value);
        }

        #endregion
    }
}