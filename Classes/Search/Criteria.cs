
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Thinkgate.Enums.Search;

namespace Thinkgate.Classes.Search
{
    public class Criteria
    {
        #region Constructors

        public Criteria()
        {
            Criterias = new List<Criterion>();
            ValidationResults = new List<ValidationResult>();
        }

        #endregion

        #region Properties

        public List<Criterion> Criterias { get; set; }
        public List<ValidationResult> ValidationResults { get; set; }

        #endregion

        #region Public Methods

        public string ToJsonString()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var obj = from p in Criterias
                      select new
                      {
                          p.Key,
                          p.UIType,
                          p.Header,
                          p.DataTextField,
                          p.DataValueField,
                          p.Dependencies,
                          p.DefaultValue,
                          Value = p.DefaultValue != null ? new KeyValuePair(p.DefaultValue.Key, p.DefaultValue.Value) : p.Value,
                          p.Locked,
                          p.HandlerName,
                          p.IsRequired,
                          p.IsUpdatedByUser,
                          p.Visible,
                          p.AutoHide,
                          p.EditMask,
                          p.DecimalPositions,
                          p.ToValue,
                          p.StandardSelection,
                          p.CurriculumSelection,
                          p.SchoolGradeNameSelection
                      };
            return serializer.Serialize(obj);
        }

        public bool IsEmpty()
        {
            if (Criterias == null)
                return true;
            if (Criterias.Count <= 0)
                return true;

            bool isEmpty = true;
            foreach (Criterion criteria in Criterias)
            {
                if (criteria.Value == null)
                {
                    return true;
                }
                if (!string.IsNullOrWhiteSpace(criteria.Value.Key))
                {
                    isEmpty = false;
                    break;
                }
                if (!string.IsNullOrWhiteSpace(criteria.Value.Value))
                {
                    isEmpty = false;
                    break;
                }
            }
            return isEmpty;
        }

        public List<ValidationResult> Validate()
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (Criterias != null && Criterias.Count > 0)
            {
                foreach (var criteria in Criterias)
                {
                    if (criteria.IsRequired && string.IsNullOrWhiteSpace(criteria.Value.Value))
                    {
                        results.Add(new ValidationResult(criteria.Header, "Criteria '" + criteria.Header + "' is required and must be selected.", ValidationType.MissingRequiredCriteria));
                    }
                }
            }
            else
            {
                results.Add(new ValidationResult("No Criteria", "No criteria available.", ValidationType.EmptyCriteria));
            }
            return results;
        }

        #endregion
    }
}