using System;

namespace Thinkgate.Classes.Search
{
     [Serializable]
    public class CurriculumSelections
    {
         public CurriculumSelections()
        { }
         public CurriculumSelections(string key, string grade, string subject, string course)
            : this()
        {
            Key = key;
            Grade = grade;
            Subject = subject;
            Course = course;
        }

        public string Key { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string Course { get; set; }
    }
}