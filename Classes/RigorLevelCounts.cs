using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thinkgate.Classes
{
    [Serializable]
    /// <summary>
    /// Creates new object to store multiple choice, short answer, essay, true/false, and non-associated rigor level counts.
    /// </summary>
    public class RigorLevelCounts
    {
        private int _multipleChoice3Count;
        private int _multipleChoice4Count;
        private int _multipleChoice5Count; 
        private int _shortAnswerCount;
        private int _essayCount;
        private int _trueFalseCount;
        private int _blueprintCount;
        public int MultipleChoice3Count { get { return _multipleChoice3Count; } }
        public int MultipleChoice4Count { get { return _multipleChoice4Count; } }
        public int MultipleChoice5Count { get { return _multipleChoice5Count; } }
        public int ShortAnswerCount { get { return _shortAnswerCount; } }
        public int EssayCount { get { return _essayCount; } }
        public int TrueFalseCount { get { return _trueFalseCount; } }
        public int BlueprintCount { get { return _blueprintCount; } }

        public RigorLevelCounts(int multipleChoice3Count = 0, int multipleChoice4Count = 0, int multipleChoice5Count = 0, int shortAnswerCount = 0, int essayCount = 0, int trueFalseCount = 0, int blueprintCount = 0)
        {
            _multipleChoice3Count = multipleChoice3Count;
            _multipleChoice4Count = multipleChoice4Count;
            _multipleChoice5Count = multipleChoice5Count;
            _shortAnswerCount = shortAnswerCount;
            _essayCount = essayCount;
            _trueFalseCount = trueFalseCount;
            _blueprintCount = blueprintCount;
        }
    }
}