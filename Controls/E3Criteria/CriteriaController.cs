using System;
using System.Collections.Generic;
using Thinkgate.Base.Classes.Extensions;

namespace Thinkgate.Controls.E3Criteria
{
    public class CriteriaController
    {
        public List<CriteriaNode> CriteriaNodes;

        public bool ValueGiven(string criteriaName)
        {
            foreach (var node in this.CriteriaNodes)
            {
                if (node.CriteriaName == criteriaName)
                {
                    foreach (var value in node.Values)
                    {
                        // if we ever change the code to update the criteria after a search as opposed to before, we'll need to remove this condition and just grab all items
                        if (value.Applied)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Look in the criteriaController for the specified criteria. Return a list of applied ValueObjects(T) where T will usually be the "[type of criteria control you are using].ValueObject"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteriaController">The criteria controller populated for your search.</param>
        /// <param name="criteriaName">Name of the criteria item you are working with.</param>
        /// <returns>List of T</returns>
        public List<T> ParseCriteria<T>(string criteriaName) where T : class, new()
        {
            var list = new List<T>();
            foreach (var node in this.CriteriaNodes)
            {
                if (node.CriteriaName == criteriaName)
                {
                    foreach (var value in node.Values)
                    {
                        // if we ever change the code to update the criteria after a search as opposed to before, we'll need to remove this condition and just grab all items
                        if (value.Applied == true)
                        {
                            list.Add(value.Value.ToObject<T>());
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Look in the criteriaController for the specified criteria. Return a list of Y, built from a specific public property of object T where T will usually be the "[type of criteria control you are using].ValueObject"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="criteriaController">The criteria controller.</param>
        /// <param name="criteriaName">Name of the criteria.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /*
        public static List<Y> ParseCriteriaToSingleValueList<T, Y>(CriteriaController criteriaController, string criteriaName, string propertyName) where T : class, new()
        {
            var list = new List<Y>();
            foreach (var node in criteriaController.CriteriaNodes)
            {
                if (node.CriteriaName == criteriaName)
                {
                    foreach (var value in node.Values)
                    {
                        // if we ever change the code to update the criteria after a search as opposed to before, we'll need to remove this condition and just grab all items
                        if (value.Applied == true)
                        {
                            dynamic obj = value.Value.ToObject<T>();
                            var targetProperty = obj.GetType().GetProperty(propertyName);
                            list.Add(targetProperty.GetValue(obj, null));
                        }
                    }
                }
            }
            return list;
        }*/
    }

    public class CriteriaNode
    {
        public string CriteriaName;
        public string CriteriaType;
        public List<SelectedValue> Values;
    }

    public class SelectedValue
    {
        public Dictionary<string, object> Value;
        public bool Applied;
        public bool CurrentlySelected;
    }


}