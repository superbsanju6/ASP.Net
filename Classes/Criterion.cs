using Thinkgate.Classes.Search;

namespace Thinkgate.Classes
{
    using System;
    using System.Collections.Generic;

    using Thinkgate.Base.Enums;

    [Serializable]
    public class Criterion
    {
        #region Constructors

        public Criterion()
        {
            DataTextField = "ListVal";
            DataValueField = "ID";
            IsHeader = true;
            Object = null;
            Visible = true;
            StandardSelection = new StandardSelections(null, null, null, null, null);
            CurriculumSelection = new CurriculumSelections(null, null, null, null);
            SchoolGradeNameSelection = new SchoolGradeNameSelections(null, null, null, null);
        }

        public Criterion(
            string header,
            string key,
            string description,
            string type,
            object obj,
            string reportStringKey,
            string reportStringVal,
            string databaseID,
            bool empty,
            bool locked,
            object dataSource,
            string dataType,
            UIType uiType,
            bool removable)
        {
            Key = key;
            Description = description;
            Type = type;
            Object = obj;
            ReportStringKey = reportStringKey;
            ReportStringVal = reportStringVal;
            DatabaseID = databaseID;
            Empty = empty;
            Header = header;
            Locked = locked;
            DataSource = dataSource;
            DataType = dataType;
            UIType = uiType;
            Removable = removable;
            Visible = true;
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
        /// Gets or sets the description of the criterion object.
        /// </summary>
        public string Description { get; set; }

        public string Type { get; set; }

        public object Object { get; set; }

        /// <summary>
        /// Gets or sets the title to be displayed.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the path of the service url to be called.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets any data that needs to be sent to the service (Ex: json data for a POST).
        /// </summary>
        public string ServiceData { get; set; }

        /// <summary>
        /// Gets or sets the javascript callback for the data to be returned to.
        /// </summary>
        public string ServiceOnSuccess { get; set; }

        /// <summary>
        /// Gets or sets an unique identifier.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets which other criterion object(s) depend on this object. Only the criterion key should be used.
        /// </summary>
        public KeyValuePair<string, string>[] Dependencies { get; set; }

        public string DatabaseID { get; set; }
        public string ReportStringKey { get; set; }
        public string ReportStringVal { get; set; }

        public bool Empty
        {
            get
            {
                return Object == null && string.IsNullOrEmpty(ReportStringVal);
            }

            set
            {
                if (value)
                {
                    Object = null;
                    ReportStringVal = string.Empty;
                }
            }
        }

        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the data to bind to this criterion object.
        /// </summary>
        public object DataSource { get; set; }
        public object StandardSetDataSource { get; set; }
        public object GradesDataSource { get; set; }
        public object SubjectsDataSource { get; set; }
        public object CourseDataSource { get; set; }
        public object StandardsDataSource { get; set; }

        public string ChildHeader { get; set; }
        public object ChildDataSource { get; set; }
        public string ChildDataValueField { get; set; }
        public string ChildDataTextField { get; set; }
        public string ChildDataParentField { get; set; } //Set to column that gives you the tie back to DataValueField of "parent" data source
        public StandardSelections StandardSelection { get; set; }
        public CurriculumSelections CurriculumSelection { get; set; }
        public bool AutoHide { get; set; }

        public EditMaskType EditMask { get; set; }
        public int DecimalPositions { get; set; }
        public string DataType { get; set; }
        public object SchoolDataSource { get; set; }
        public object TeacherNameDataSource { get; set; }
        public SchoolGradeNameSelections SchoolGradeNameSelection { get; set; }

        //public SchoolGradeNameSelections SchoolGradeNameSelection { get; set; }
        /// <summary>
        /// Gets or sets the type of control to be used for this criterion object.
        /// </summary>
        public UIType UIType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to remove the Object from the Criterion object.
        /// </summary>
        public bool Removable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this Criterion object is a header object.
        /// </summary>
        public bool IsHeader { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether this Criterion object is required to use.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a string value for the default value of the Criterion object.
        /// </summary>
        public string DefaultValue { get; set; }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Helper function to create a dependency.
        /// </summary>
        /// <param name="key">The key used to reference other criterion objects.</param>
        /// <param name="value">This is the mapping to the data contract object for a web service. If this is empty or null, value will be set to the key passed in.</param>
        /// <returns>A dependency object.</returns>
        public static KeyValuePair<string, string> CreateDependency(string key, string value = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                value = key;
            }

            return new KeyValuePair<string, string>(key, value);
        }

        #endregion

        #region Public Methods

        public void Clear()
        {
            if (!Locked)
            {
                Empty = true;
            }
        }

        /// <summary>
        /// Creates a clone of the current criterion object. NOTE: It does not clone the DataSource.
        /// </summary>
        /// <returns>Cloned Criterion object.</returns>
        public Criterion Clone()
        {
            return new Criterion
            {
                DataSource = null,
                DataTextField = this.DataTextField,
                DataType = this.DataType,
                DataValueField = this.DataValueField,
                DatabaseID = this.DatabaseID,
                Description = this.Description,
                Empty = this.Empty,
                Header = this.Header,
                IsHeader = this.IsHeader,
                Key = this.Key,
                Locked = this.Locked,
                Object = this.Object,
                Removable = this.Removable,
                ReportStringKey = this.ReportStringKey,
                ReportStringVal = this.ReportStringVal,
                Type = this.Type,
                UIType = this.UIType,
                DefaultValue = this.DefaultValue
            };
        }

        #endregion
    }
}
