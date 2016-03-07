using System;

namespace Thinkgate.Classes.Search
{
     [Serializable]
    public class StandardSelections
    {
        #region Properties

         public StandardSelections()
        { }

        public StandardSelections(string key, string standardSet, string grade, string subject, string course )
            : this()
        {
            Key = key;
            StandardSet = standardSet;
            Grade = grade;
            Subject = subject;
            Course = course;
        }

        public string Key { get; set; }
        public string StandardSet { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Course { get; set; }

        #endregion
    }
}