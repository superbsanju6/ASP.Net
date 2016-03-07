using System;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Enums;
using Thinkgate.Utilities;

namespace Thinkgate.Classes
{
    [Obsolete]
    public class ExpandedPage : BasePage
    {
        //public string NoResultsMessage = "No results found.";

        //public object LoadRecordObject(EntityTypes RecordType, string encryptedID)
        //{
        //    var xID = Cryptography.DecryptionToInt(encryptedID, SessionObject.LoggedInUser.CipherKey);
        //    if (xID == 0)
        //    {
        //        xID = Standpoint.Core.Classes.Encryption.DecryptStringToInt(Request.QueryString["xID"]);
        //        if (xID == 0)
        //        {
        //            RedirectToHome("Invalid entity ID provided.");
        //        }
        //    }
        //    string key = string.Empty;
        //    object obj = new object();
        //    var absoluteRetirement = DateTime.Now.AddHours(1);

        //    switch (RecordType)
        //    {
        //        case EntityTypes.District:
        //            key = "District_" + xID.ToString();
        //            break;

        //        case EntityTypes.Student:
        //            key = "Student_" + xID.ToString();
        //            break;

        //        case EntityTypes.Class:
        //            key = "Class_" + xID.ToString();
        //            break;

        //        case EntityTypes.Teacher:
        //            key = "Teacher_" + xID.ToString();
        //            break;

        //        case EntityTypes.School:
        //            key = "School_" + xID.ToString();
        //            break;

        //        case EntityTypes.Assessment:
        //            key = "Assessment_" + xID.ToString();
        //            break;
        //    }

        //    if (RecordExistsInCache(key)) return Base.Classes.Cache.Get(key);

        //    switch (RecordType)
        //    {
        //        case EntityTypes.District:
        //            obj = Base.Classes.District.GetDistrictByID(DataIntegrity.ConvertToInt(xID)); // TODO: Pass UserID
        //            break;

        //        case EntityTypes.Student:
        //            obj = Base.Classes.Data.StudentDB.GetStudentByID(DataIntegrity.ConvertToInt(xID)); // TODO: Pass UserID
        //            break;

        //        case EntityTypes.Class:
        //            obj = Base.Classes.Class.GetClassByID(DataIntegrity.ConvertToInt(xID)); // TODO: Pass UserID
        //            break;

        //        case EntityTypes.Teacher:
        //            obj = Thinkgate.Base.Classes.Data.TeacherDB.GetTeacherByPage(DataIntegrity.ConvertToInt(xID)); // TODO: Pass UserID
        //            break;

        //        case EntityTypes.School:
        //            obj = Base.Classes.School.GetSchoolByID(DataIntegrity.ConvertToInt(xID)); // TODO: Pass UserID
        //            break;

        //        case EntityTypes.Assessment:
        //            obj = Base.Classes.Assessment.GetAssessmentByID(DataIntegrity.ConvertToInt(xID)); // TODO: Pass UserID
        //            break;

        //        /*case EntityTypes.Staff:
        //            obj = Base.Classes.Staff.GetStaffByID(DataIntegrity.ConvertToInt(xID)); // TODO: Pass UserID
        //            break;*/
        //    }

        //    // Updates every hour
        //    if (obj != null)
        //    {
        //        Base.Classes.Cache.Insert(key, obj, null, absoluteRetirement, TimeSpan.Zero);
        //    }

        //    return obj;
        //}
    }
}
