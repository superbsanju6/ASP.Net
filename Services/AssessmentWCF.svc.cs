using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using Standpoint.Core.Utilities;
using Thinkgate.Base.DataAccess;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Thinkgate.Base.Enums;
using Thinkgate.ExceptionHandling;
using Thinkgate.Services.KenticoServices;

namespace Thinkgate.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AssessmentWCF : Interfaces.IAssessmentWCF
    {
        public string RequestNewAssessmentID(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            //return Standpoint.Core.Classes.Encryption.EncryptString("0");
            Thinkgate.Classes.SessionObject sessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            string newAssessmentID;
            int teacherID = sessionObject.LoggedInUser != null ? (sessionObject.LoggedInUser.Page > 0 ? sessionObject.LoggedInUser.Page : AppSettings.Demo_TeacherID) : AppSettings.Demo_TeacherID;
            string grade = !String.IsNullOrEmpty(assessmentVars.Grade) ? assessmentVars.Grade : (sessionObject.AssessmentBuildParms.ContainsKey("Grade") ? sessionObject.AssessmentBuildParms["Grade"] : "");
            string subject = !String.IsNullOrEmpty(assessmentVars.Subject) ? assessmentVars.Subject : (sessionObject.AssessmentBuildParms.ContainsKey("Subject") ? sessionObject.AssessmentBuildParms["Subject"] : "");
            int courseID = assessmentVars.CourseID > 0 ? assessmentVars.CourseID : (sessionObject.AssessmentBuildParms.ContainsKey("Course") ? DataIntegrity.ConvertToInt(sessionObject.AssessmentBuildParms["Course"]) : 0);
            string type = !String.IsNullOrEmpty(assessmentVars.Type) ? assessmentVars.Type : (sessionObject.AssessmentBuildParms.ContainsKey("Type") ? sessionObject.AssessmentBuildParms["Type"] : "");
            int term = assessmentVars.Term > 0 ? assessmentVars.Term : (sessionObject.AssessmentBuildParms.ContainsKey("Term") ? DataIntegrity.ConvertToInt(sessionObject.AssessmentBuildParms["Term"]) : 0);
            string content = !String.IsNullOrEmpty(assessmentVars.Content) ? assessmentVars.Content : (sessionObject.AssessmentBuildParms.ContainsKey("Content") ? sessionObject.AssessmentBuildParms["Content"] : "");
            string description = !String.IsNullOrEmpty(assessmentVars.Description) ? assessmentVars.Description : (sessionObject.AssessmentBuildParms.ContainsKey("Description") ? sessionObject.AssessmentBuildParms["Description"] : "");
            string testCategory = sessionObject.AssessmentBuildParms.ContainsKey("TestCategory") ? sessionObject.AssessmentBuildParms["TestCategory"] : "Classroom";
            string scoreType = assessmentVars.ScoreType;
            string performanceLevelSet = assessmentVars.PerformanceLevelSet;
            string keywords = assessmentVars.Keywords;
            string year = !String.IsNullOrEmpty(assessmentVars.Year) ? assessmentVars.Year : (sessionObject.AssessmentBuildParms.ContainsKey("Year") ? sessionObject.AssessmentBuildParms["Year"] : "");

            var tt = TestTypes.GetByName(type);
            bool SecureType = false;
            if (tt != null)
                SecureType= tt.Secure;

            bool hasPermission = sessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
            Dictionary<string, bool> dictionaryItem = Base.Classes.TestTypes.TypeWithSecureFlag(testCategory);
            bool isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            if (hasPermission && isSecuredFlag && SecureType)
                {
                    testCategory = type;
                }
       
            dtStandardsRigorLevels standardsRigorLevelTable = sessionObject.Standards_RigorLevels_ItemCounts.BuildDataTable();
            dtItemBank itemBankTable = ItemBankMasterList.GetItemBanksForStandardSearch(sessionObject.LoggedInUser, testCategory);
            dtItemBank itemBanksSQLTable = new dtItemBank();

            foreach(string item in sessionObject.ItemBanks)
            {
                DataRow itemBankRow = itemBankTable.Select("Label = '" + item + "'")[0];
                itemBanksSQLTable.Add(DataIntegrity.ConvertToInt(itemBankRow["TargetType"]),
                                      itemBankRow["Target"].ToString(),
                                      DataIntegrity.ConvertToInt(itemBankRow["ApprovalSource"]),
                                      itemBankRow["Label"].ToString());
            }

            if (assessmentVars.StandardIDs != null)
            {
                string returnVal = StoreStandardCounts(assessmentVars);
            }
            dtGeneric_Int_Int standardsCountsTable = sessionObject.Standards_RigorLevels_ItemCounts.BuildStandardItemTotalsSQLTable();

            #region Addendum
            dtStandardsAddendumLevels standardsAddendumLevelTable = sessionObject.Standards_Addendumevels_ItemCounts.BuildDataTable();
            if (sessionObject.Standards_Addendumevels_ItemCounts.StandardItemTotals.Count>0)
            standardsCountsTable = sessionObject.Standards_Addendumevels_ItemCounts.BuildStandardItemTotalsSQLTable();
            #endregion

            newAssessmentID = Base.Classes.Assessment.GenerateNewAssessment(grade, subject, courseID, type, term, content, description, itemBanksSQLTable, standardsCountsTable, "",
                                                                            teacherID, standardsRigorLevelTable, scoreType, performanceLevelSet, keywords, year, standardsAddendumLevelTable);
            newAssessmentID = newAssessmentID == "0" ? "0" : content == "No Items/Content" ? newAssessmentID.ToString() : Standpoint.Core.Classes.Encryption.EncryptString(newAssessmentID);

            //Clear any existing Session State objects from BluePrints
            sessionObject.Standards_RigorLevels_ItemCounts.ClearAllBlankItemCounts();
            sessionObject.Standards_RigorLevels_ItemCounts.ClearAllStandardItemNames();
            sessionObject.Standards_RigorLevels_ItemCounts.ClearAllStandardItemTotals();
            sessionObject.Standards_RigorLevels_ItemCounts.ClearAllStandardRigorLevel();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllBlankItemCounts();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardItemNames();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardItemTotals();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardAddendumLevel();
            sessionObject.Standards_Addendumevels_ItemCounts.ClearAllAddendumCounts();
            sessionObject.AssessmentBuildParms.Remove("BlueprintID");
            sessionObject.AssessmentBuildParms.Remove("StandardSet");

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return newAssessmentID;
        }

        public string StoreAssessmentIdentification(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            
            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            string standardSet = SessionObject.AssessmentBuildParms.ContainsKey("StandardSet")
                                     ? SessionObject.AssessmentBuildParms["StandardSet"]
                                     : "";
            string level = SessionObject.AssessmentBuildParms.ContainsKey("level")
                               ? SessionObject.AssessmentBuildParms["level"]
                               : "";
            string testCategory = SessionObject.AssessmentBuildParms.ContainsKey("TestCategory")
                                      ? SessionObject.AssessmentBuildParms["TestCategory"]
                                      : "";
            SessionObject.AssessmentBuildParms.Clear();
            SessionObject.AssessmentBuildParms.Add("Grade", assessmentVars.Grade);
            SessionObject.AssessmentBuildParms.Add("Subject", assessmentVars.Subject);
            SessionObject.AssessmentBuildParms.Add("Course", assessmentVars.CourseID.ToString());
            SessionObject.AssessmentBuildParms.Add("Type", assessmentVars.Type);
            SessionObject.AssessmentBuildParms.Add("Term", assessmentVars.Term.ToString());
            SessionObject.AssessmentBuildParms.Add("Content", assessmentVars.Content);
            SessionObject.AssessmentBuildParms.Add("AssessmentSelection", assessmentVars.AssessmentSelection);
            SessionObject.AssessmentBuildParms.Add("Description", assessmentVars.Description);
            SessionObject.AssessmentBuildParms.Add("Year", assessmentVars.Year);
            if(standardSet.Length > 0) SessionObject.AssessmentBuildParms.Add("StandardSet", standardSet);
            if(level.Length > 0) SessionObject.AssessmentBuildParms.Add("level", level);
            if(testCategory.Length > 0) SessionObject.AssessmentBuildParms.Add("TestCategory", testCategory);

            //Clear any existing Session State objects from BluePrints
            SessionObject.Standards_RigorLevels_ItemCounts.ClearAllBlankItemCounts();
            SessionObject.Standards_RigorLevels_ItemCounts.ClearAllStandardItemNames();
            SessionObject.Standards_RigorLevels_ItemCounts.ClearAllStandardItemTotals();
            SessionObject.Standards_RigorLevels_ItemCounts.ClearAllStandardRigorLevel();
            SessionObject.Standards_Addendumevels_ItemCounts.ClearAllBlankItemCounts();
            SessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardItemNames();
            SessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardItemTotals();
            SessionObject.Standards_Addendumevels_ItemCounts.ClearAllStandardAddendumLevel();
            SessionObject.Standards_Addendumevels_ItemCounts.ClearAllAddendumCounts();
            SessionObject.AssessmentBuildParms.Remove("BlueprintID");
            SessionObject.AssessmentBuildParms.Remove("StandardSet");

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return "assessment saved";
        }


        public string StoreStandardRigorLevels(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

                int itemCount = 0;

            //Set the StandardItemTotals value to 0 for the selected standard
            SessionObject.Standards_RigorLevels_ItemCounts.ClearStandardItemTotal(assessmentVars.StandardID);

            //Loop through all rigor levels and add item counts to the Standards_RigorLevels_ItemCounts dictionary
            for (int i = 0; i < assessmentVars.RigorLevels.Count; i++)
            {
                string rigorLevelName = assessmentVars.RigorLevels[i].ToString();
                int multipleChoice3Count = DataIntegrity.ConvertToInt(assessmentVars.MultipleChoice3Counts[i]);
                int multipleChoice4Count = DataIntegrity.ConvertToInt(assessmentVars.MultipleChoice4Counts[i]);
                int multipleChoice5Count = DataIntegrity.ConvertToInt(assessmentVars.MultipleChoice5Counts[i]);
                int shortAnswerCount = DataIntegrity.ConvertToInt(assessmentVars.ShortAnswerCounts[i]);
                int essayCount = DataIntegrity.ConvertToInt(assessmentVars.EssayCounts[i]);
                int trueFalseCount = DataIntegrity.ConvertToInt(assessmentVars.TrueFalseCounts[i]);
                int blueprintCount = DataIntegrity.ConvertToInt(assessmentVars.BlueprintCounts[i]);

                SessionObject.Standards_RigorLevels_ItemCounts.RemoveLevel(assessmentVars.StandardID, rigorLevelName);

                if (multipleChoice3Count > 0 || multipleChoice4Count > 0 || multipleChoice5Count > 0 || shortAnswerCount > 0 || essayCount > 0 || trueFalseCount > 0 || blueprintCount > 0)
                {
                    itemCount += multipleChoice3Count + multipleChoice4Count + multipleChoice5Count + shortAnswerCount + essayCount + trueFalseCount + blueprintCount;
                    SessionObject.Standards_RigorLevels_ItemCounts.AddLevel(assessmentVars.StandardID, rigorLevelName, multipleChoice3Count, multipleChoice4Count, multipleChoice5Count, shortAnswerCount, essayCount, trueFalseCount, blueprintCount);
                }
            }

            //If assessmentVars.TotalItemCount is greater than the rigor level itemCount, then add blank rows for the difference
                if (assessmentVars.TotalItemCount > itemCount)
            {
                int itemCountDifference = assessmentVars.TotalItemCount - itemCount;
                SessionObject.Standards_RigorLevels_ItemCounts.AddLevel(assessmentVars.StandardID, "", itemCountDifference);
            }

            //Store total item count for standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemTotal(assessmentVars.StandardID, assessmentVars.StandardSet, assessmentVars.TotalItemCount);

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return "standard rigor levels saved";
        }

        //User is able to enter any total value in the UI that may override blueprint counts for specific rigors. This function attempts to distribute evenly
        private void updateSingleRigorLevel(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            //Get Rigor Names from DB
            List<string> rigorNames = (from row in Base.Classes.Rigor.ListAsDataTable().AsEnumerable()
                                       select row.Field<string>("Text")).ToList();

            //Total entered by user in UI
            int requestedValue = assessmentVars.TotalItemCount;

            //Rigor Names from DB 
            int rigorLevels = rigorNames.Count - 1; //Off by 1 as "none" is included in DB , but not tracked in blueprint (special case in UI)

            //Blue Print Counts
            Int32 [] bpcount = (Int32 [])assessmentVars.RigorLevelCounts.ToArray(typeof(Int32));

            //New Distributed Values
            int [] newValues = new int [rigorLevels]; 
            Array.Clear(newValues, 0, newValues.Length);

            int counter = 0;

            //Distribute count evenly across BP rigor items
            while (requestedValue > 0 && (Int32)bpcount.Sum() > 0)
            {
                //Reset to array length
                if (counter >= rigorLevels)
                    counter = 0;
                //Preference original blue print items first
                if (requestedValue > 0 && (Int32)bpcount[counter] > 0)
                {
                    newValues[counter] += 1;
                    requestedValue -= 1;
                    bpcount[counter] -= 1; //Decrement bpcount to ensure we don't overload
                }
                counter++;
           }
            
            counter = 0;

            //Now fill in remaining randomly
            while (requestedValue > 0)
            {
                //Reset to array length
                if (counter >= rigorLevels)
                    counter = 0;
                //Assign in order
                if (requestedValue > 0)
                {
                    newValues[counter] += 1;
                    requestedValue -= 1;
                }
                counter++;
            }

                                              
            int standardid = assessmentVars.StandardID;
            int standardItemCount = 0;
            int totalItemCount = assessmentVars.TotalItemCount;

            //Set the StandardItemTotals value to 0 for the selected standard
            SessionObject.Standards_RigorLevels_ItemCounts.ClearStandardItemTotal(standardid);

            //Loop through all rigor levels and add item counts to the Standards_RigorLevels_ItemCounts dictionary
            for (int i = 0; i < rigorLevels; i++)
            {
                 
                string rigorLevelName = rigorNames[i].ToString();
                int multipleChoice3Count = 0;
                int multipleChoice4Count = 0;
                int multipleChoice5Count = 0;
                int shortAnswerCount = 0;
                int essayCount = 0;
                int trueFalseCount = 0;
                int blueprintCount = newValues[i];

                SessionObject.Standards_RigorLevels_ItemCounts.RemoveLevel(standardid, rigorLevelName);

                if (multipleChoice3Count > 0 || multipleChoice4Count > 0 || multipleChoice5Count > 0 || shortAnswerCount > 0 || essayCount > 0 || trueFalseCount > 0 || blueprintCount > 0)
                {
                    standardItemCount += multipleChoice3Count + multipleChoice4Count + multipleChoice5Count + shortAnswerCount + essayCount + trueFalseCount + blueprintCount;
                    SessionObject.Standards_RigorLevels_ItemCounts.AddLevel(standardid, rigorLevelName, multipleChoice3Count,multipleChoice4Count,multipleChoice5Count, shortAnswerCount, essayCount, trueFalseCount, blueprintCount);
                }

            }

            //If assessmentVars.TotalItemCount is greater than the rigor level itemCount, then add blank rows for the difference
            if (totalItemCount > standardItemCount)
            {
                int itemCountDifference = totalItemCount - standardItemCount;
                SessionObject.Standards_RigorLevels_ItemCounts.AddLevel(standardid, "", itemCountDifference);
            }

            //Store total item count for standard
            SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemTotal(standardid, "Blueprint", totalItemCount);

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);

         }

        public string StoreStandardCounts(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            for (int i = 0; i < assessmentVars.StandardIDs.Count; i++)
            {
                //Clear StandardItemTotal
                SessionObject.Standards_RigorLevels_ItemCounts.ClearStandardItemTotal(DataIntegrity.ConvertToInt(assessmentVars.StandardIDs[i]));

                //Set new total value for the selected standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemTotal(DataIntegrity.ConvertToInt(assessmentVars.StandardIDs[i]), assessmentVars.StandardSet, 
                    DataIntegrity.ConvertToInt(assessmentVars.StandardCounts[i]));
            }

            //Set Standard Set
            if(SessionObject.AssessmentBuildParms.ContainsKey("StandardSet"))
            {
                SessionObject.AssessmentBuildParms["StandardSet"] = assessmentVars.StandardSet;
            }
            else
            {
                SessionObject.AssessmentBuildParms.Add("StandardSet", assessmentVars.StandardSet);
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return "standard counts saved";
        }

        public string StoreStandardCount(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];


            //Clear StandardItemTotal
            SessionObject.Standards_RigorLevels_ItemCounts.ClearStandardItemTotal(assessmentVars.StandardID);

            //Clear StandardItemName
            SessionObject.Standards_RigorLevels_ItemCounts.ClearStandardItemName(assessmentVars.StandardID);

            //Clear BlankItemCount
            SessionObject.Standards_RigorLevels_ItemCounts.ClearBlankItemCount(assessmentVars.StandardID);

                //Special Case Blueprint
                if (assessmentVars.StandardSet == "Blueprint")
                {
                    updateSingleRigorLevel(assessmentVars);
                }
                else
                {
            if (assessmentVars.TotalItemCount > 0)
            {
                //Set new total value for the selected standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemTotal(DataIntegrity.ConvertToInt(assessmentVars.StandardID), assessmentVars.StandardSet, assessmentVars.TotalItemCount);

                //Set new name value for the selected standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemName(assessmentVars.StandardID, assessmentVars.StandardName);

                //Set new blank item count for the selected standard if value is greater than 0
                if (assessmentVars.BlankItemCount > 0)
                {
                    SessionObject.Standards_RigorLevels_ItemCounts.AddBlankItemCount(assessmentVars.StandardID, assessmentVars.BlankItemCount);
                }
            }
                }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return "standard count saved";
        }

        public string StoreStandardCountsAndNames(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            for (int i = 0; i < assessmentVars.StandardIDs.Count; i++)
            {
                //Clear StandardItemTotal
                SessionObject.Standards_RigorLevels_ItemCounts.ClearStandardItemTotal(DataIntegrity.ConvertToInt(assessmentVars.StandardIDs[i]));

                //Set new total value for the selected standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemTotal(DataIntegrity.ConvertToInt(assessmentVars.StandardIDs[i]), assessmentVars.StandardSet, 
                    DataIntegrity.ConvertToInt(assessmentVars.StandardCounts[i]));

                //Clear StandardItemName
                SessionObject.Standards_RigorLevels_ItemCounts.ClearStandardItemName(DataIntegrity.ConvertToInt(assessmentVars.StandardIDs[i]));

                //Set new name value for the selected standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddStandardItemName(DataIntegrity.ConvertToInt(assessmentVars.StandardIDs[i]), assessmentVars.StandardNames[i].ToString());
            }

            //Set Standard Set
            if (SessionObject.AssessmentBuildParms.ContainsKey("StandardSet"))
            {
                SessionObject.AssessmentBuildParms["StandardSet"] = assessmentVars.StandardSet;
            }
            else
            {
                SessionObject.AssessmentBuildParms.Add("StandardSet", assessmentVars.StandardSet);
            }

                foreach (string itemCount in assessmentVars.BlankItemCounts)
            {
                int standardID = DataIntegrity.ConvertToInt(itemCount.Split('|')[0]);
                int count = DataIntegrity.ConvertToInt(itemCount.Split('|')[1]);

                //Clear BlankItemCount
                SessionObject.Standards_RigorLevels_ItemCounts.ClearBlankItemCount(standardID);

                //Set new blank item count for selected standard
                SessionObject.Standards_RigorLevels_ItemCounts.AddBlankItemCount(standardID, count);
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return "standard counts and names saved";
        }

        public Dictionary<string, string> RequestGradeList(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            string sql = string.Format(@"Select DISTINCT cc.Grade  FROM Schools  S    
                                        INNER JOIN Classes  c ON S.ID =C.School AND  S.ID='{0}'       
                                        INNER JOIN ClassCourses  cc On cc.Id = c.ClassCourseId       
                                        INNER JOIN CTXref  ctx ON CTX.CLASSPAGE=C.ID and ctx.Type='primary'   
                                        INNER JOIN  aspnet_Users au ON au.Page = ctx.TeacherPage       
                                        where au.District= c.District Order by cc.Grade ASC", assessmentVars.SchoolID);

            DataTable GradeDataTable = GetDataTable(sql);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Dictionary<string, string> grades = new Dictionary<string, string>();
            if (GradeDataTable != null)
            {
                foreach (DataRow row in GradeDataTable.Rows)
                {
                    grades.Add(row["Grade"].ToString(), row["Grade"].ToString());
                }
            }
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return grades;
        }

        private DataTable GetDataTable (string selectSQL)
        {
            DataTable dataTable = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings[AppSettings.ConnectionStringName].ConnectionString;
            if (selectSQL != null)
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = new SqlCommand(selectSQL, sqlConnection);

                try
                {
                    sqlConnection.Open();
                }
                catch (SqlException ex)
                {
                    return dataTable;
                }
                try
                {
                    sqlDataAdapter.Fill(dataTable);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return dataTable;
        }

        public Dictionary<string, string> RequestSubjectList(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            CourseList curriculumCourses = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            var subjectsByCurriculumCourses = curriculumCourses.FilterByGrade(assessmentVars.Grade).GetSubjectList();

            Dictionary<string, string> subjects = new Dictionary<string, string>();

            foreach (var subjectText in subjectsByCurriculumCourses.Select(s => s.DisplayText).Distinct())
            {
                subjects.Add(subjectText, subjectText);
            }

            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemTotals.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemNames.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardRigorLevel.Clear();
            SessionObject.ItemBanks.Clear();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return subjects;
        }

        public Dictionary<int, string> RequestCourseList(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            CourseList curriculumCourses = CourseMasterList.GetCurrCoursesForUser(SessionObject.LoggedInUser);
            var coursesByCurriculumCourses = curriculumCourses.FilterByGradeAndSubject(assessmentVars.Grade, assessmentVars.Subject);
            coursesByCurriculumCourses.Sort((x, y) => string.Compare(x.CourseName, y.CourseName));

            Dictionary<int, string> courses = new Dictionary<int, string>();

            foreach (var c in coursesByCurriculumCourses)
            {
                courses.Add(c.ID, c.CourseName);
            }

            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemTotals.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemNames.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardRigorLevel.Clear();
            SessionObject.ItemBanks.Clear();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return courses;
        }

        public string ClearStandardsList(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemTotals.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardItemNames.Clear();
            SessionObject.Standards_RigorLevels_ItemCounts.StandardRigorLevel.Clear();
            SessionObject.ItemBanks.Clear();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return "standards cleared";
        }

        public Dictionary<string, string> RequestDistractorLabels(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            DataTable distractorLabelsTable = Base.Classes.Assessment.GetDistractorLabels(assessmentVars.NumberOfDistractors > 0 ? assessmentVars.NumberOfDistractors : 5, assessmentVars.UserID);
            Dictionary<string, string> distractorLabels = new Dictionary<string, string>();

            foreach(DataRow dr in distractorLabelsTable.Rows)
            {
                distractorLabels.Add(dr["Value"].ToString(), dr["DistractorLabel"].ToString());
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return distractorLabels;
        }

        public string StoreAssessmentItemBanksSelected(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

            if (SessionObject.ItemBankLabels.ContainsKey(assessmentVars.AssessmentID))
                SessionObject.ItemBankLabels.Remove(assessmentVars.AssessmentID);

            SessionObject.ItemBankLabels.Add(assessmentVars.AssessmentID, assessmentVars.ItemBankList);

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return "saved";
        }

        public string UpdateIdentification(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            string cacheKey = "Assessment_" + assessmentVars.EncryptedAssessmentID;
            var assessment = Base.Classes.Assessment.GetAssessmentAndQuestionsByID(assessmentVars.AssessmentID);
            
            assessment.Grade = assessmentVars.Grade;
            assessment.Subject = assessmentVars.Subject;
            assessment.currCourseID = assessmentVars.CourseID;
            Base.Classes.Course assessmentCourse = CourseMasterList.GetCurrCourseById(assessmentVars.CourseID);
            assessment.Course = assessmentCourse != null ? assessmentCourse.CourseName : "";
            assessment.TestType = assessmentVars.Type;
            assessment.Term = assessmentVars.Term.ToString();
            assessment.Description = assessmentVars.Description;
            assessment.Year = assessmentVars.Year;
            assessment.IsOTCNavigationRestricted = assessmentVars.AllowedOTCNavigation;


            Base.Classes.Assessment.SaveIdentificationInformation(assessment, SessionObject.LoggedInUser.Page);
            if (Cache.Get(cacheKey) == null)
            {
                Cache.Insert(cacheKey, assessment);
            }
            else
            {
                Cache.Remove(cacheKey);
                Cache.Insert(cacheKey, assessment);
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return "saved";
        }

        public string CopyAssessment(AssessmentWCFVariables assessmentVars)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            Thinkgate.Classes.SessionObject SessionObject = (Thinkgate.Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];
            int newId = Base.Classes.Assessment.CloneAssessment(assessmentVars.AssessmentID, assessmentVars.Term.ToString(), assessmentVars.Description, assessmentVars.Year);
						// We must make another call to a stored proc to set all the information. Ideally, the first stored proc should take these arguments.
            Base.Classes.Assessment.SaveIdentificationInformation(newId, assessmentVars.Grade, assessmentVars.Subject, assessmentVars.Course,
                                                                                                                        assessmentVars.Type, assessmentVars.Term.ToString(), assessmentVars.Description,
                                                                                                                        SessionObject.LoggedInUser.Page, assessmentVars.AllowedOTCNavigation == "1" ? true : false, assessmentVars.Year);
						string ret = Standpoint.Core.Classes.Encryption.EncryptInt(newId);
                        if (Assessment.IsContainsExpiredContent(assessmentVars.AssessmentID))
                            SessionObject.GenericPassThruParm = "CLONE:" + ret + ":ContainedExpiredContent";
                        else
            SessionObject.GenericPassThruParm = "CLONE:" + ret;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return ret;
        }

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request AssessmentWCF", "AssessmentWCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end AssessmentWCF", "AssessmentWCF"); }
        }
    }
}
