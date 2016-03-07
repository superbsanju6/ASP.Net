
using Thinkgate.Enums.Search;

namespace Thinkgate.Classes.Search
{
    public class ValidationResult
    {
        #region Properties

        public ValidationResult()
        { }

        public ValidationResult(string message, string description, ValidationType validationType)
            : this()
        {
            this.Message = message;
            this.Description = description;
            this.ValidationType = validationType;
        }

        public string Message { get; set; }
        public string Description { get; set; }
        public ValidationType ValidationType { get; set; }

        #endregion
    }
}