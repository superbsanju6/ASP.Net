using Telerik.Web.UI;
using Thinkgate.Controls.E3Criteria;
using System;
using System.Web.Security;
using Thinkgate.Classes;
using System.Web.UI;
using System.Web.Helpers;
using ClosedXML.Excel;
using System.Data;
using System.Collections.Generic;

namespace Thinkgate
{

    public partial class SearchMaster : System.Web.UI.MasterPage
    {
        #region Properties

        public delegate void SearchHandler(object sender, CriteriaController criteriaController);
        public event SearchHandler Search;
        public static string ImageWebFolder;

        public SessionObject SessionObject { get; set; }
        
        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            SessionObject = (SessionObject)Session["SessionObject"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ImageWebFolder = (Request.ApplicationPath.Equals("/") ? "" : Request.ApplicationPath) + "/Images/";
            if (!Page.User.Identity.IsAuthenticated || Session == null || Session.IsNewSession || Session["SessionObject"] == null)
            {
                Services.Service2.KillSession();
            }
            if (!IsPostBack)
            {
                RegisterScript();
            }

            ScriptManager.RegisterStartupScript(this, typeof(string), "SessionBridgeVariable", " var SessionBridge='" + ResolveUrl("~/SessionBridge.aspx?ReturnURL=") + "';", true);
        }

        public void RegisterScript()
        {
            bool firstOne = true;
            string enumStr = "CriteriaController.RestrictValueOptions = {";
            foreach (int option in Enum.GetValues(typeof(CriteriaBase.RestrictValueOptions)))
            {
                if (!firstOne) enumStr += ",";
                enumStr += "\"" + Enum.GetName(typeof(CriteriaBase.RestrictValueOptions), option) + "\" : " + option;
                firstOne = false;
            }
            enumStr += "};";
            ScriptManager.RegisterStartupScript(this, typeof(string), "SearchEnums", enumStr, true);
        }

        public void NotifyOfMissingCritiera()
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(Page), Guid.NewGuid().ToString(), "alert('Please make sure all required criteria is selected before updating search.');", true);
        }

        protected void AjaxPanelResults_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            var criteriaController = Json.Decode(e.Argument,typeof(CriteriaController));

            if (!VerifyRequiredCriteria(criteriaController))
            {
                NotifyOfMissingCritiera();
                return; 
            }
           
            ViewState["CriteriaController"] = e.Argument;

            if (Search != null)
                Search(sender, criteriaController);
            //ScriptManager.RegisterStartupScript(this, typeof(string), "SearchComplete", "CriteriaController.SearchFinished();", true);
            //ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "SearchComplete", "CriteriaController.SearchFinished();", true);
        }

        protected bool VerifyRequiredCriteria(CriteriaController criteriaController, Control root=null)
        {
            if (root == null) root = FindControl("LeftColumnContentPlaceHolder");

            foreach (Control ctrl in root.Controls)
            {
                var obj = ctrl as Thinkgate.Controls.E3Criteria.CriteriaBase;
                if (obj != null)
                {
                    if (obj.Required && !criteriaController.ValueGiven(obj.CriteriaName))
                        return false;
                }
                var subCheck = VerifyRequiredCriteria(criteriaController, ctrl);
                if (!subCheck) return false;
            }
            return true;
        }

       public CriteriaController CurrentCriteria()
       {
           if (ViewState["CriteriaController"] == null)  throw new CriteriaNotLoaded("Criteria not yet loaded.");
          
               var criteriaString = ViewState["CriteriaController"].ToString();
               if (String.IsNullOrEmpty(criteriaString)) return null;
               return Json.Decode(criteriaString, typeof(CriteriaController));
         

       }

       public void ChangeCriteriaOrder(ref List<DateRange.ValueObject> dateSelected)
       {
           if (dateSelected.Count > 1 && dateSelected[0].Type == "End" && dateSelected[1].Type == "Start")
           {
               var tempDate = dateSelected[0];
               dateSelected[0] = dateSelected[1];
               dateSelected[1] = tempDate;
           }
       }

       public XLWorkbook ConvertDataTableToSingleSheetWorkBook(DataTable dt, string sheet = "")
       {
           XLWorkbook workbook = new XLWorkbook();

           if (dt.Rows.Count > 0)
           {
               workbook.Worksheets.Add(sheet ?? "Sheet1");

               int colCount;
               //Write second rows first so that we can calculate width of for the headers
               var rowCount = 2;
               foreach (DataRow row in dt.Rows) // Loop over the rows.
               {
                   colCount = 1;
                   foreach (var item in row.ItemArray) // Loop over the items.
                   {
                       workbook.Worksheet(1).Cell(rowCount, colCount).Value = item.ToString();
                       colCount++;
                   }

                   rowCount++;
               }

               colCount = 1; //reset columns
               foreach (DataColumn column in dt.Columns)
               {
                   workbook.Worksheet(1).Cell(1, colCount).Value = column.ColumnName;
                   workbook.Worksheet(1).Cell(1, colCount).Style.Font.SetBold();
                   workbook.Worksheet(1).Column(colCount).AdjustToContents();
                   colCount++;
               }


               return workbook;
           }
           else
           {
               return null;
           }
       }

    }

    public class CriteriaNotLoaded : System.Exception
    {
        // The default constructor needs to be defined
        // explicitly now since it would be gone otherwise.

        public CriteriaNotLoaded()
        {
        }

        public CriteriaNotLoaded(string message)
            : base(message)
        {
        }
    }
}
