using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Data;
using Standpoint.Core.Utilities;

namespace Thinkgate.Classes
{
    [Serializable]

    /// <summary>
    /// Creates a Dictionary for storing Standard ID's, Rigor Levels, and Rigor Level counts. 
    /// KEY is the Standard ID, VALUE is a Dictionary of Rigor Levels and RigorLevelCounts objects.
    /// Rigor Level KEY is the Rigor Level name, VALUE is the RigorLevelCounts object.
    /// </summary>
    public class StandardRigorLevels
    {
        private Dictionary<int, Dictionary<string, RigorLevelCounts>> _standardRigorLevel;
        private Dictionary<int, ArrayList> _standardItemTotals;
        private Dictionary<int, string> _standardItemNames;
        private Dictionary<int, int> _blankItemCounts;
        public Dictionary<int, Dictionary<string, RigorLevelCounts>> StandardRigorLevel { get { return _standardRigorLevel; } }
        public Dictionary<int, ArrayList> StandardItemTotals { get { return _standardItemTotals; } }
        public Dictionary<int, string> StandardItemNames { get { return _standardItemNames; } }
        public Dictionary<int, int> BlankItemCounts { get { return _blankItemCounts; } }

        public StandardRigorLevels()
        {
            _standardRigorLevel = new Dictionary<int, Dictionary<string, RigorLevelCounts>>();
            _standardItemTotals = new Dictionary<int, ArrayList>();
            _standardItemNames = new Dictionary<int, string>();
            _blankItemCounts = new Dictionary<int, int>();
        }

        /// <summary>
        /// Adds a new Standard Dictionary or updates an existing one. Clears existing rigor level and rigor level counts before adding new data.
        /// </summary>
        /// <param name="standardID">Int value to be used as the KEY</param>
        /// <param name="rigorLevel">String value to be used as the Rigor Level Dictionary KEY</param>
        /// <param name="multiplechoiceCount">Int value to be set to the MultipleChoiceCount property of the RigorLevelCounts object</param>
        /// <param name="shortanswerCount">Int value to be set to the ShortAnswerCount property of the RigorLevelCounts object</param>
        /// <param name="essayCount">Int value to be set to the EssayCount property of the RigorLevelCounts object</param>
        /// <param name="truefalseCount">Int value to be set to the TrueFalseChoiceCount property of the RigorLevelCounts object</param>
        /// <param name="naCount">Int value to be set to the NACount property of the RigorLevelCounts object</param>
        public void AddLevel(int standardID, string rigorLevel, int multiplechoice3Count = 0, int multiplechoice4Count = 0, int multiplechoice5Count = 0, int shortanswerCount = 0, int essayCount = 0, int truefalseCount = 0, int blueprintCount = 0)
        {
            RigorLevelCounts newRigorCounts = new RigorLevelCounts(multiplechoice3Count, multiplechoice4Count, multiplechoice5Count, shortanswerCount, essayCount, truefalseCount, blueprintCount); //Create object to hold counts
            Dictionary<string, RigorLevelCounts> newRigorLevel = new Dictionary<string, RigorLevelCounts>(); //Create rigor level object

            newRigorLevel.Add(rigorLevel, newRigorCounts); //Add counts to rigor level


            if (_standardRigorLevel.ContainsKey(standardID))
            {
                _standardRigorLevel[standardID].Remove(rigorLevel); //Remove possible existing rigor level
                _standardRigorLevel[standardID].Add(rigorLevel, newRigorCounts); //Add new rigor level and counts to standard
            }
            else
            {
                _standardRigorLevel.Add(standardID, newRigorLevel); //Add new standard, rigor level, and counts
            }

            //Calculate total item count and store in the StandardItemTotals dictionary
            //int totalItemCount = multiplechoiceCount + shortanswerCount + essayCount + truefalseCount;
            //AddStandardItemTotal(standardID, totalItemCount);
        }

        public void AddBlankItemCount(int standardID, int count)
        {
            if(_blankItemCounts.ContainsKey(standardID))
            {
                _blankItemCounts[standardID] = count;
            }
            else
            {
                _blankItemCounts.Add(standardID, count);
            }
        }

        /// <summary>
        /// Removes a Rigor Level from the Standard Dictionary. The Standard will also be removed if it's rigor level count goes to zero.
        /// </summary>
        /// <param name="standardID"></param>
        /// <param name="rigorLevel"></param>
        public void RemoveLevel(int standardID, string rigorLevel)
        {
            if (_standardRigorLevel.ContainsKey(standardID))
            {
                _standardRigorLevel[standardID].Remove(rigorLevel);

                if (_standardRigorLevel[standardID].Count == 0)
                {
                    _standardRigorLevel.Remove(standardID);
                }
            }
        }

        /// <summary>
        /// Adds or updates a new standard item total to the StandardItemTotals dictionary
        /// </summary>
        /// <param name="standardID">Int value to be used as the dictionary KEY</param>
        /// <param name="total">Int value to be used as the dictionary VALUE</param>
        public void AddStandardItemTotal(int standardID, string standardSet, int total) {
            if (_standardItemTotals.ContainsKey(standardID))
            {
                int num;
                if (_standardItemTotals[standardID][1] != null && Int32.TryParse(_standardItemTotals[standardID][1].ToString(), out num))
                {
                    _standardItemTotals[standardID][0] = standardSet;
                    _standardItemTotals[standardID][1] = DataIntegrity.ConvertToInt(_standardItemTotals[standardID][1]) + total;
                }
            }
            else
            {
                ArrayList standardArray = new ArrayList();
                standardArray.Add(standardSet);
                standardArray.Add(total);
                _standardItemTotals.Add(standardID, standardArray);
            }
        }

        /// <summary>
        /// Adds or updates a new standard item name to the StandardItemNames dictionary
        /// </summary>
        /// <param name="standardID">Int value to be used as the dictionary KEY</param>
        /// <param name="total">String value to be used as the dictionary VALUE</param>
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

        /// <summary>
        /// Clears any value set for the selected standard item total in the StandardItemTotals dictionary and resets it back to 0
        /// </summary>
        /// <param name="standardID">Int value to be used as the dictionary KEY reference</param>
        public void ClearStandardItemTotal(int standardID)
        {
            if (_standardItemTotals.ContainsKey(standardID))
            {
                _standardItemTotals.Remove(standardID);
            }
        }

        /// <summary>
        /// Clears any value set for the selected standard item name in the StandardItemNames dictionary
        /// </summary>
        /// <param name="standardID">Int value to be used as the dictionary KEY reference</param>
        public void ClearStandardItemName(int standardID)
        {
            if (_standardItemNames.ContainsKey(standardID))
            {
                _standardItemNames.Remove(standardID);
            }
        }

        public void ClearBlankItemCount(int standardID)
        {
            if(_blankItemCounts.ContainsKey(standardID))
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

        public void ClearAllStandardRigorLevel()
        {
            _standardRigorLevel.Clear();
        }

        public Thinkgate.Base.DataAccess.dtStandardsRigorLevels BuildDataTable()
        {
            var standardRigorLevelTable = new Thinkgate.Base.DataAccess.dtStandardsRigorLevels();
            
            foreach(KeyValuePair<int, Dictionary<string, RigorLevelCounts>> item in _standardRigorLevel) {
                if (item.Value.Count > 0)
                {
                    foreach (KeyValuePair<string, RigorLevelCounts> level in item.Value)
                    {
                        standardRigorLevelTable.Add(item.Key, level.Key.ToString(), level.Value.MultipleChoice3Count,
                                                    level.Value.MultipleChoice4Count,level.Value.MultipleChoice5Count,
                                                    level.Value.ShortAnswerCount, level.Value.EssayCount,
                                                    level.Value.TrueFalseCount, level.Value.BlueprintCount); //BluePrint count = Not Specified by default
                    }
                }
            }

            return standardRigorLevelTable;
        }

        public DataTable BuildRigorDistributionTable()
        {
            //DataTable rigorList = Thinkgate.Base.Classes.Assessment.GetRigorList();
            DataTable rigorList = Thinkgate.Base.Classes.Rigor.ListAsDataTable();
            DataColumn distributionCol = new DataColumn("Distribution");
            distributionCol.DataType = System.Type.GetType("System.Int32");
            rigorList.Columns.Add(distributionCol);

            DataColumn multipleChoice3Col = new DataColumn("MultipleChoice3");
            multipleChoice3Col.DataType = System.Type.GetType("System.Int32");
            rigorList.Columns.Add(multipleChoice3Col);

            DataColumn multipleChoice4Col = new DataColumn("MultipleChoice4");
            multipleChoice4Col.DataType = System.Type.GetType("System.Int32");
            rigorList.Columns.Add(multipleChoice4Col);

            DataColumn multipleChoice5Col = new DataColumn("MultipleChoice5");
            multipleChoice5Col.DataType = System.Type.GetType("System.Int32");
            rigorList.Columns.Add(multipleChoice5Col);

            DataColumn shortAnswerCol = new DataColumn("ShortAnswer");
            shortAnswerCol.DataType = System.Type.GetType("System.Int32");
            rigorList.Columns.Add(shortAnswerCol);

            DataColumn essayCol = new DataColumn("Essay");
            essayCol.DataType = System.Type.GetType("System.Int32");
            rigorList.Columns.Add(essayCol);

            DataColumn trueFalseCol = new DataColumn("TrueFalse");
            trueFalseCol.DataType = System.Type.GetType("System.Int32");
            rigorList.Columns.Add(trueFalseCol);

            DataColumn bluePrintCol = new DataColumn("BPCount");
            bluePrintCol.DataType = System.Type.GetType("System.Int32");
            rigorList.Columns.Add(bluePrintCol);

            int totalItemCount = 0;
            int totalRigorItemsCount = 0;
            int totalBlankItemsCount = 0;

            foreach (KeyValuePair<int, ArrayList> item in _standardItemTotals)
            {
                totalItemCount += DataIntegrity.ConvertToInt(item.Value[1]);

                //Remove blank item count from item total for this standard
                if(_blankItemCounts.ContainsKey(item.Key))
                {
                    totalItemCount -= _blankItemCounts[item.Key];
                }

                if (_standardRigorLevel.ContainsKey(item.Key))
                {
                    foreach(DataRow level in rigorList.Rows) {
                        
                        if (level["Text"].ToString() == "(None)")
                        { level.SetField("Text", "Not Specified"); }

                        if (_standardRigorLevel[item.Key].ContainsKey(level["Text"].ToString()))
                        {
                            int levelItemsCount = 
                                _standardRigorLevel[item.Key][level["Text"].ToString()].MultipleChoice3Count +
                                _standardRigorLevel[item.Key][level["Text"].ToString()].MultipleChoice4Count +
                                _standardRigorLevel[item.Key][level["Text"].ToString()].MultipleChoice5Count +
                                _standardRigorLevel[item.Key][level["Text"].ToString()].ShortAnswerCount +
                                _standardRigorLevel[item.Key][level["Text"].ToString()].EssayCount +
                                _standardRigorLevel[item.Key][level["Text"].ToString()].TrueFalseCount +
                                _standardRigorLevel[item.Key][level["Text"].ToString()].BlueprintCount
                                ;

                            totalRigorItemsCount += levelItemsCount;
                            level[distributionCol] = DataIntegrity.ConvertToInt(level[distributionCol]) + levelItemsCount;

                            level[multipleChoice3Col] = DataIntegrity.ConvertToInt(level[multipleChoice3Col]) + _standardRigorLevel[item.Key][level["Text"].ToString()].MultipleChoice3Count;
                            level[multipleChoice4Col] = DataIntegrity.ConvertToInt(level[multipleChoice4Col]) + _standardRigorLevel[item.Key][level["Text"].ToString()].MultipleChoice4Count;
                            level[multipleChoice5Col] = DataIntegrity.ConvertToInt(level[multipleChoice5Col]) + _standardRigorLevel[item.Key][level["Text"].ToString()].MultipleChoice5Count;
                            level[shortAnswerCol] = DataIntegrity.ConvertToInt(level[shortAnswerCol]) + _standardRigorLevel[item.Key][level["Text"].ToString()].ShortAnswerCount;
                            level[essayCol] = DataIntegrity.ConvertToInt(level[essayCol]) + _standardRigorLevel[item.Key][level["Text"].ToString()].EssayCount;
                            level[trueFalseCol] = DataIntegrity.ConvertToInt(level[trueFalseCol]) + _standardRigorLevel[item.Key][level["Text"].ToString()].TrueFalseCount;
                            level[bluePrintCol] = DataIntegrity.ConvertToInt(level[bluePrintCol]) + _standardRigorLevel[item.Key][level["Text"].ToString()].BlueprintCount;
                        }
                    }
                }
            }

            if (totalItemCount > totalRigorItemsCount)
            {
                int distributionTotal = DataIntegrity.ConvertToInt(rigorList.Rows[rigorList.Rows.Count-1][distributionCol]);
                int unspecifiedTotal = totalItemCount - totalRigorItemsCount;
                rigorList.Rows[rigorList.Rows.Count - 1][distributionCol] = distributionTotal + unspecifiedTotal;
            }

            //Get blank item counts total
            foreach(KeyValuePair<int, int> count in _blankItemCounts)
            {
                totalBlankItemsCount += count.Value;
            }

            //Add blank items row
            DataRow blankItemsRow = rigorList.NewRow();
            blankItemsRow["Text"] = "Blank Items";
          blankItemsRow[distributionCol] = totalBlankItemsCount;
          blankItemsRow[multipleChoice4Col] = totalBlankItemsCount;

            rigorList.Rows.Add(blankItemsRow);

            return rigorList;
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

            foreach (KeyValuePair<int, ArrayList> item in _standardItemTotals) 
            {
                totalItemCount += DataIntegrity.ConvertToInt(item.Value[1]);
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

                    decimal percentage = (DataIntegrity.ConvertToDecimal(_standardItemTotals[key][1]) / DataIntegrity.ConvertToDecimal(totalItemCount)) * 100;
                    percentage = Math.Round(percentage, 2);
                    row[percentageCol] = percentage.ToString() + "%";

                    row[itemsCol] = _standardItemTotals[key][1].ToString();

                    standardTable.Rows.Add(row);
                }
            }
            /*
            foreach (KeyValuePair<int, ArrayList> item in _standardItemTotals)
            {
                if (_standardItemNames.ContainsKey(item.Key))
                {
                    DataRow row = standardTable.NewRow();
                    row[standardNameCol] = linkAccess ? "<a href='#'>" + _standardItemNames[item.Key].ToString() + "</a>" : _standardItemNames[item.Key].ToString();

                    decimal percentage = (DataIntegrity.ConvertToDecimal(item.Value[1]) / DataIntegrity.ConvertToDecimal(totalItemCount)) * 100;
                    percentage = Math.Round(percentage, 2);
                    row[percentageCol] = percentage.ToString() + "%";

                    row[itemsCol] = item.Value[1].ToString();

                    standardTable.Rows.Add(row);
                }
            }*/

            return standardTable;
        }

        public DataTable BuildStandardItemTotalsTable()
        {
            DataTable standardItemTotalsTable = new DataTable();
            DataColumn standardIDColumn = new DataColumn("StandardID");
            standardIDColumn.DataType = System.Type.GetType("System.Int32");
            standardItemTotalsTable.Columns.Add(standardIDColumn);

            DataColumn countColumn = new DataColumn("Count");
            countColumn.DataType = System.Type.GetType("System.Int32");
            standardItemTotalsTable.Columns.Add(countColumn);

            foreach (KeyValuePair<int, ArrayList> item in _standardItemTotals)
            {
                DataRow row = standardItemTotalsTable.NewRow();
                row[standardIDColumn] = item.Key;
                row[countColumn] = DataIntegrity.ConvertToInt(item.Value[1]);

                standardItemTotalsTable.Rows.Add(row);
            }

            return standardItemTotalsTable;
        }

        public Thinkgate.Base.DataAccess.dtGeneric_Int_Int BuildStandardItemTotalsSQLTable()
        {
            var standardItemTotalsTable = new Thinkgate.Base.DataAccess.dtGeneric_Int_Int();
            
            foreach (KeyValuePair<int, ArrayList> item in _standardItemTotals)
            {
                standardItemTotalsTable.Add(item.Key, DataIntegrity.ConvertToInt(item.Value[1]));
            }

            if (_standardItemTotals.Count == 0)
            {
                standardItemTotalsTable.Add(0,0);
            }

            return standardItemTotalsTable;
        }
    }
}