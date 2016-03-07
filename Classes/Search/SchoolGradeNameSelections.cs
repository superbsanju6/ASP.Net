using System;

namespace Thinkgate.Classes.Search
{
     [Serializable]
    public class SchoolGradeNameSelections
    {
        #region Properties

         public SchoolGradeNameSelections()
        { }

        public SchoolGradeNameSelections(string key, string school, string grade, string name)
            : this()
        {
            Key = key;
            School = school;
            Grade = grade;
            Name = name;
        }

        public string Key { get; set; }
        public string Grade { get; set; }
        public string School { get; set; }
        public string Name { get; set; }

        #endregion
    }
}