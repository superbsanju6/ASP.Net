using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using CMS.GlobalHelper;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Utilities;

namespace Thinkgate
{
    public partial class CompetencyWorksheetPreview : System.Web.UI.Page
    {
        private string _WorkSheetIDEncrypted;
        private SessionObject session;
        public int WorksheetID;
        public int ClassID;

        protected void Page_Load(object sender, EventArgs e)
        {
            session = (SessionObject)Session["SessionObject"];

            if (Request.QueryString["xID"] == null)
            {
                Response.Redirect("~/PortalSelection.aspx", true);
            }
            else
            {
                _WorkSheetIDEncrypted = Request.QueryString["xID"];
                WorksheetID = Cryptography.GetDecryptedID(session.LoggedInUser.CipherKey);
            }

            if (Request.QueryString["classid"] != null)
            {
                ClassID = Convert.ToInt32(Request.QueryString["classid"].ToString().Trim());
            }
            try
            {
                if (Session["CheckForWorksheetCopied"].ToString() == "copied")
                {
                    ScriptManager.RegisterStartupScript(this.Page, typeof(string), "reloadClassTile", "window.opener.location.href = window.opener.location.href;", true);
                    Session["CheckForWorksheetCopied"] = null;
                }
            }
            catch { }
        }

        [System.Web.Services.WebMethod]
        public static string GetViewByStandardGrid(int WorksheetId, int ClassID)
        {
            DataSet dsWorksheet = CompetencyWorkSheet.GetCompetencyWorksheetViewByStandard(WorksheetId, ClassID);

            if (dsWorksheet != null && dsWorksheet.Tables.Count > 0)
            {
                dsWorksheet.Tables[0].AsEnumerable().ToList().ForEach(F => F["EncryptedStandardID"] = Standpoint.Core.Classes.Encryption.EncryptInt(Convert.ToInt32(F["StandardID"].ToString())));
            }

            return dsWorksheet.ToJSON(false);
        }

        [System.Web.Services.WebMethod]
        public static string GetViewByStudentGrid(int WorksheetId, int ClassID)
        {
            DataSet dsWorksheet = CompetencyWorkSheet.GetCompetencyWorksheetViewByStudent(WorksheetId, ClassID);

            if (dsWorksheet != null && dsWorksheet.Tables.Count > 2)
            {
                dsWorksheet.Tables[1].AsEnumerable().ToList().ForEach(F => F["EncryptedStandardID"] = Standpoint.Core.Classes.Encryption.EncryptInt(Convert.ToInt32(F["StandardID"].ToString())));
            }

            return dsWorksheet.ToJSON(false);
        }

        [System.Web.Services.WebMethod]
        public static string SaveWorksheetHistory(string History, string HistoryRubricItems)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jSerializer = 
                new System.Web.Script.Serialization.JavaScriptSerializer();

            var lstHistory = jSerializer.Deserialize<IEnumerable<HistoryType>>(History);
            var lstHistoryRubricItems = jSerializer.Deserialize<IEnumerable<HistoryRubricItemType>>(HistoryRubricItems);
            string result = string.Empty;

            DataTable dtCWHistory = new DataTable();

            dtCWHistory.Columns.Add("WorksheetID", typeof(int));
            dtCWHistory.Columns.Add("StudentID", typeof(int));
            dtCWHistory.Columns.Add("StandardID", typeof(int));
            dtCWHistory.Columns.Add("Teacher", typeof(Guid));
            dtCWHistory.Columns.Add("CompetencyRubricID", typeof(int));

            DataTable dtCWHistoryRI = new DataTable();
            dtCWHistoryRI.Columns.Add("WorkSheetHistoryID", typeof(int));
            dtCWHistoryRI.Columns.Add("ScoreDate", typeof(DateTime));
            dtCWHistoryRI.Columns.Add("CompetencyRubricItemID", typeof(int));
            dtCWHistoryRI.Columns.Add("WorksheetID", typeof(int));
            dtCWHistoryRI.Columns.Add("StudentID", typeof(int));
            dtCWHistoryRI.Columns.Add("StandardID", typeof(int));
            dtCWHistoryRI.Columns.Add("Teacher", typeof(Guid));

            if (lstHistory != null && lstHistory.Count() > 0)
            {
                foreach (HistoryType hItem in lstHistory)
                {
                    DataRow dr = dtCWHistory.NewRow();
                    dr["WorksheetID"] = hItem.WorksheetID;
                    dr["StudentID"] = hItem.StudentID;
                    dr["StandardID"] = hItem.StandardID;
                    dr["Teacher"] = hItem.TeacherID;
                    dr["CompetencyRubricID"] = hItem.RubricID;
                    dtCWHistory.Rows.Add(dr);
                }
            }

            if (lstHistoryRubricItems != null && lstHistoryRubricItems.Count() > 0)
            {
                foreach (HistoryRubricItemType hrItem in lstHistoryRubricItems)
                {
                    DataRow dr = dtCWHistoryRI.NewRow();
                    dr["WorkSheetHistoryID"] = 0;
                    dr["ScoreDate"] = DateTime.Now;
                    dr["CompetencyRubricItemID"] = hrItem.CompetencyRubricItemID;
                    dr["WorksheetID"] = hrItem.WorksheetID;
                    dr["StudentID"] = hrItem.StudentID;
                    dr["StandardID"] = hrItem.StandardID;
                    dr["Teacher"] = hrItem.TeacherID;
                    dtCWHistoryRI.Rows.Add(dr);
                }
            }

            if (dtCWHistory.Rows.Count > 0 && dtCWHistoryRI.Rows.Count > 0)
            {
                CompetencyWorkSheet.SaveCompetencyWorksheetHistory(dtCWHistory, dtCWHistoryRI);
            }

            return result;
        }

        [System.Web.Services.WebMethod]
        public static void DeleteWorksheet(int WorksheetId)
        {
            CompetencyWorkSheet.DeleteWorksheet(Convert.ToInt32(WorksheetId));
            CompetencyWorksheetPreview obj_CWSPrev = new CompetencyWorksheetPreview();
            obj_CWSPrev.checkForDelete();
        }

        private void checkForDelete()
        {
            if (Session["SessionObject"] != null)
            {
                session = (SessionObject)Session["SessionObject"];
                session.CheckNewWorksheetCreated = "1";
            }
        }
    }

    public class HistoryType
    {
        public int WorksheetID { get; set; }
        public int StudentID { get; set; }
        public int StandardID { get; set; }
        public int RubricID { get; set; }
        public Guid TeacherID { get; set; }
    }

    public class HistoryRubricItemType
    {
        public int StudentID { get; set; }
        public int StandardID { get; set; }
        public int WorkSheetHistoryID { get; set; }
        //public DateTime ScoreDate {get;set;}
        public int CompetencyRubricItemID { get; set; }
        public int WorksheetID { get; set; }
        public Guid TeacherID { get; set; }
    }
}