using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Thinkgate.Classes.Search;

namespace Thinkgate.Classes
{
    [Serializable]
    public class Criteria
    {
        public List<Criterion> CriterionList = new List<Criterion>();

        public string ToJsonString()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var obj = from p in CriterionList
                      select new
                      {
                          p.Key,
                          p.UIType,
                          p.Header,
                          p.DataTextField,
                          p.DataValueField,
                          p.Dependencies,
                          p.DefaultValue,
                          Object = p.DefaultValue != null ? new Object() : p.Object,
                          p.Locked,
                          p.IsRequired,
                          p.Visible,
                          p.AutoHide,
                          p.EditMask,
                          p.DecimalPositions,
                          p.StandardSelection,
                          p.CurriculumSelection,
                          p.SchoolGradeNameSelection,
                      };
            return serializer.Serialize(obj);
        }

        public void Add(Criterion criterion)
        {
            CriterionList.Add(criterion);
        }

        public bool RemoveCriterion(string key)
        {
            if (CriterionList.Exists(r => r.Key == key)) return false;    
            CriterionList.Remove((Criterion)CriterionList.Where(r => r.Key == key));
            return true;
        }

        public bool RemoveCriterionSearchParm(string criterionKey, string searchParmKey)
        {
            if (CriterionList.Exists(r => r.Key == criterionKey && r.Type == "SearchParms")) return false;
            var criterion = (Criterion)CriterionList.Where(r => r.Key == criterionKey);
            var searchParm = ((SearchParms)criterion.Object).GetParm((searchParmKey));
            if (searchParm == null) return false;
            ((SearchParms)criterion.Object).DeleteParm(searchParmKey);
            return true;
        }

        public string GetCriteriaOverrideString()
        {
            var criteriaList = new List<string>();

            if (CriterionList == null) return null;

            var count = 0;
            var criteriaList2 = CriterionList.Where(c => !String.IsNullOrEmpty(c.ReportStringKey) && !c.Empty);

            foreach (Criterion criterion in criteriaList2)
            {
                count++;

                switch (criterion.Type)
                {
                    case "String":
                        criteriaList.Add(criterion.ReportStringKey + "=" + criterion.ReportStringVal);
                        break;

                    //case "SearchParms":
                    //    var parms = (SearchParms)criterion.Object;

                    //    foreach (var parm in parms.Parms)
                    //    {
                    //        criteriaList.Add(criterion.ReportStringKey + "=" + criterion.ReportStringVal);
                    //    }

                    //    break;

                    //case "List<string>":
                    //    var list = (List<string>)criterion.Object;

                    //    if (list == null) break;

                    //    foreach (string str in (List<string>)criterion.Object)
                    //    {

                    //    }

                    //    break;
                }
            }

            var critString = "@@";
            critString += string.Join("@@", criteriaList);
            critString += "@@";

            return critString;
        }

        public Criterion GetLevelCriterion(string level)
        {
            var criterion = CriterionList.FindLast(c => c.Key == level && c.ReportStringVal != null);

            if (criterion == null) return null;

            return criterion;
        }

        public Criterion GetAssessmentCriterion()
        {
            return CriterionList.FindLast(c => c.Key == "Assessment");
        }

        public Criterion FindCriterionHeaderByText(string text)
        {
            return CriterionList.Find(c => c.IsHeader && c.Header == text);
        }
    }
}
