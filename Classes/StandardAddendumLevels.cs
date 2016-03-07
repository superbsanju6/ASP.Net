using Standpoint.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Thinkgate.Classes
{
    [Serializable()]
    public class StandardAddendumLevels
    {
        private Dictionary<int, Dictionary<int, int>> _standardAddendumLevel;
        private Dictionary<int, int> _standardItemTotals;
        private Dictionary<int, string> _standardItemNames;
        private Dictionary<int, ArrayList> _addendumCounts;
        private Dictionary<int, int> _blankItemCounts;
        public Dictionary<int, Dictionary<int, int>> StandardAddendumLevel { get { return _standardAddendumLevel; } }
        public Dictionary<int, int> StandardItemTotals { get { return _standardItemTotals; } }
        public Dictionary<int, string> StandardItemNames { get { return _standardItemNames; } }
        public Dictionary<int, ArrayList> AddendumCounts { get { return _addendumCounts; } }
        public Dictionary<int, int> BlankItemCounts { get { return _blankItemCounts; } }


        public StandardAddendumLevels()
        {
            _standardAddendumLevel = new Dictionary<int, Dictionary<int, int>>();
            _standardItemTotals = new Dictionary<int, int>();
            _standardItemNames = new Dictionary<int, string>();
            _addendumCounts = new Dictionary<int, ArrayList>();
            _blankItemCounts = new Dictionary<int, int>();
        }

        public void AddLevel(int standardID, int addendumLevel, int itemCount = 0)
        {
            //AddendumLevelCounts newAddendumCounts = new AddendumLevelCounts(standardcount); //Create object to hold counts
            Dictionary<int, int> newAddendumLevel = new Dictionary<int, int>(); //Create Addendum level object


            newAddendumLevel.Add(addendumLevel, itemCount); //Add counts to Addendum level


            if (_standardAddendumLevel.ContainsKey(standardID))
            {
                 //Remove possible existing Addendum level
                Dictionary<int, int> obj = _standardAddendumLevel[standardID].ToDictionary(x => x.Key, x => x.Value);
                _standardAddendumLevel.Remove(standardID);
                obj.Add(addendumLevel, itemCount);
                //_standardAddendumLevel[standardID].Add(standardID, obj);
                _standardAddendumLevel.Add(standardID, obj);//Add new rigor level and counts to standard
            }
            else
            {
                _standardAddendumLevel.Add(standardID, newAddendumLevel); //Add new standard, rigor level, and counts
            }

            //Calculate total item count and store in the StandardItemTotals dictionary
            //int totalItemCount = multiplechoiceCount + shortanswerCount + essayCount + truefalseCount;
            //AddStandardItemTotal(standardID, totalItemCount);
        }

        public void AddBlankItemCount(int standardID, int count)
        {
            if (_blankItemCounts.ContainsKey(standardID))
            {
                _blankItemCounts[standardID] = count;
            }
            else
            {
                _blankItemCounts.Add(standardID, count);
            }
        }

        public void RemoveLevel(int standardID, int addendumLevel)
        {
            if (_standardAddendumLevel.ContainsKey(standardID))
            {
                _standardAddendumLevel[standardID].Remove(addendumLevel);

                if (_standardAddendumLevel[standardID].Count == 0)
                {
                    _standardAddendumLevel.Remove(standardID);
                }
            }
        }
        public void AddStandardItemTotal(IEnumerable<Thinkgate.Dialogues.Assessment.StandardCountList> standardlist)
        {

            foreach (Thinkgate.Dialogues.Assessment.StandardCountList item in standardlist)
            {



                if (_standardItemTotals.ContainsKey(item.StandardID))
                {
                    int num;
                    if (_standardItemTotals[item.StandardID] != null && Int32.TryParse(_standardItemTotals[item.StandardID].ToString(), out num))
                    {
                        _standardItemTotals[item.StandardID] = item.Count;
                        // _standardItemTotals[item.StandardID][1] = DataIntegrity.ConvertToInt(_standardItemTotals[standardID][1]) + total;
                    }
                }
                else
                {
                    //ArrayList standardArray = new ArrayList();
                    //standardArray.Add(standardSet);
                    //standardArray.Add(total);
                    //_standardItemTotals.Add(standardID, standardArray);
                    _standardItemTotals.Add(item.StandardID, item.Count);
                }
            }

        }
        public void AddStandardItemName(int standardID, string name)
        {
            if (_standardItemNames.ContainsKey(standardID))
            {
                _standardItemNames[standardID] += name;
            }
            else
            {
                _standardItemNames.Add(standardID, name);
            }
        }
        public void ClearStandardItemTotal(int standardID)
        {
            if (_standardItemTotals.ContainsKey(standardID))
            {
                _standardItemTotals.Remove(standardID);
            }
        }

        public void ClearStandardItemName(int standardID)
        {
            if (_standardItemNames.ContainsKey(standardID))
            {
                _standardItemNames.Remove(standardID);
            }
        }
        public void ClearBlankItemCount(int standardID)
        {
            if (_blankItemCounts.ContainsKey(standardID))
            {
                _blankItemCounts.Remove(standardID);
            }
        }

        public void ClearAllStandardItemTotals()
        {
            _standardItemTotals.Clear();
        }

        public void ClearAllStandardItemNames()
        {
            _standardItemNames.Clear();
        }

        public void ClearAllBlankItemCounts()
        {
            _blankItemCounts.Clear();
        }

        public void ClearAllStandardAddendumLevel()
        {
            _standardAddendumLevel.Clear();
        }

        public void ClearAllAddendumCounts()
        {
            _addendumCounts.Clear();
        }

        public void ClearAddendumCount(int addendumID)
        {
            if (_addendumCounts.ContainsKey(addendumID))
            {
                _addendumCounts.Remove(addendumID);
            }
        }

        public void AddAddendumCount(int addendumID, string addendumName, int count)
        {
            ArrayList addendumArray = new ArrayList();
            addendumArray.Add(addendumName);
            addendumArray.Add(count);
            _addendumCounts.Add(addendumID, addendumArray);
        }

        public DataTable BuildAddendumDistributionTable()
        {
            DataTable addendumTable = new DataTable();

            DataColumn addendumIDCol = new DataColumn("AddendumID");
            addendumTable.Columns.Add(addendumIDCol);

            DataColumn addendumNameCol = new DataColumn("AddendumName");
            addendumTable.Columns.Add(addendumNameCol);

            DataColumn itemCountCol = new DataColumn("ItemCount");
            addendumTable.Columns.Add(itemCountCol);

            var list = _addendumCounts.Keys.ToList();
            list.Sort();
            foreach (var key in list)
            {
                if (_addendumCounts.ContainsKey(key))
                {
                    if (Convert.ToInt32(_addendumCounts[key][1]) > 0)
                    {
                        DataRow row = addendumTable.NewRow();

                        row[addendumIDCol] = key.ToString().Trim();

                        row[addendumNameCol] = _addendumCounts[key][0].ToString();

                        row[itemCountCol] = _addendumCounts[key][1].ToString();

                        addendumTable.Rows.Add(row);
                    }
                }
            }
            return addendumTable;
        }

        public DataTable BuildStandardDistributionTable()
        {
            DataTable standardTable = new DataTable();

            DataColumn standardIDCol = new DataColumn("StandardID");
            standardTable.Columns.Add(standardIDCol);

            DataColumn standardNameCol = new DataColumn("Standard");
            standardTable.Columns.Add(standardNameCol);

            DataColumn percentageCol = new DataColumn("Percentage");
            standardTable.Columns.Add(percentageCol);

            DataColumn itemsCol = new DataColumn("Items");
            standardTable.Columns.Add(itemsCol);

            decimal totalItemCount = 0;

            foreach (KeyValuePair<int, int> item in _standardItemTotals)
            {
                totalItemCount += DataIntegrity.ConvertToInt(item.Value);
            }

            var list = _standardItemTotals.Keys.ToList();
            list.Sort();
            foreach (var key in list)
            {
                if (_standardItemNames.ContainsKey(key))
                {
                    DataRow row = standardTable.NewRow();

                    row[standardIDCol] = key.ToString().Trim();

                    row[standardNameCol] = _standardItemNames[key].ToString();

                    decimal percentage = (DataIntegrity.ConvertToDecimal(_standardItemTotals[key]) / DataIntegrity.ConvertToDecimal(totalItemCount)) * 100;
                    percentage = Math.Round(percentage, 2);
                    row[percentageCol] = percentage.ToString() + "%";

                    row[itemsCol] = _standardItemTotals[key].ToString();

                    standardTable.Rows.Add(row);
                }
            }

            return standardTable;
        }

        public Thinkgate.Base.DataAccess.dtStandardsAddendumLevels BuildDataTable()
        {
            var standardAddendumLevelTable = new Thinkgate.Base.DataAccess.dtStandardsAddendumLevels();

            foreach (KeyValuePair<int, Dictionary<int, int>> item in _standardAddendumLevel)
            {
                if (item.Value.Count > 0)
                {
                    foreach (KeyValuePair<int, int> level in item.Value)
                    {
                        standardAddendumLevelTable.Add(level.Key, item.Key, level.Value); //BluePrint count = Not Specified by default
                    }
                }
            }

            return standardAddendumLevelTable;
        }

        public Thinkgate.Base.DataAccess.dtGeneric_Int_Int BuildStandardItemTotalsSQLTable()
        {
            var standardItemTotalsTable = new Thinkgate.Base.DataAccess.dtGeneric_Int_Int();

            foreach (KeyValuePair<int, int> item in _standardItemTotals)
            {
                standardItemTotalsTable.Add(item.Key, DataIntegrity.ConvertToInt(item.Value));
            }

            if (_standardItemTotals.Count == 0)
            {
                standardItemTotalsTable.Add(0, 0);
            }

            return standardItemTotalsTable;
        }

    }


}