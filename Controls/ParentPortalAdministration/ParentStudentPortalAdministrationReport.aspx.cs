using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Classes;
using Thinkgate.Controls.E3Criteria;
using System.Collections.Generic;
using System.ComponentModel;

namespace Thinkgate.Controls.ParentPortalAdministration
{
    public partial class ParentStudentPortalAdministrationReport : BasePage
    {
        public string StudentId { get; set; }

        public bool IsParentGridInEditMode { get; set; }

        public System.Data.DataTable ParentData { get; set; }

        private ParentStudentAdminReportVM ViewModel
        {
            get;
            set;
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            this.ViewModel = new ParentStudentAdminReportVM();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Master.Search += SearchHandler;
            RegisterScript();

            //btnCheckDuplicateEmail.Attributes["style"] = "display: none";

            ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
            if (scriptManager != null)
            {
                scriptManager.RegisterPostBackControl(imgExport);
            }

            switch (Request.Form["__EVENTTARGET"])
            {
                case "radSaveParentGrid":
                    radSaveParentGrid_Click(this, new EventArgs());
                    break;
                case "imgExport":
                    //BindParentGrid();
                    imgExport_Click(this, new EventArgs());
                    break;
                case "btnCheckDuplicateEmail":
                    btnCheckDuplicateEmail_Click(this, new EventArgs());
                    break;
            }
        }
        //private const string CompetencyTrackingReport = "Competency Tracking Report";
        //private const string CompetencyTrackingReport_NoSpaces = "CompetencyTrackingReport";

        #region search criteria event

        protected void ddlSchoolType_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.ddlSchoolType.DataSource = null;
            this.ddlSchoolType.DataBind();

            if (this.SessionObject != null && this.SessionObject.LoggedInUser != null)
            {
                var loggedInUser = this.SessionObject.LoggedInUser;
                if (loggedInUser.Roles != null && loggedInUser.Roles.Any())
                {
                    this.ddlSchoolType.DataSource = this.ViewModel.GetAllSchoolType(loggedInUser).Select(c => new { Text = string.Format("{0}", c.Name), Value = c.Name });
                    this.ddlSchoolType.DataBind();
                }
            }
        }

        protected void ddlSchool_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.ddlSchool.DataSource = null;
            this.ddlSchool.DataBind();

            if (this.SessionObject != null && this.SessionObject.LoggedInUser != null)
            {
                var loggedInUser = this.SessionObject.LoggedInUser;
                if (loggedInUser.Roles != null && loggedInUser.Roles.Any())
                {
                    this.ddlSchool.DataSource = this.ViewModel.GetAllSchoolByType(Convert.ToString(e.Context["SchoolType"]), loggedInUser);
                    this.ddlSchool.DataBind();
                }
            }
        }

        protected void ddlStudentGrade_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.ddlStudentGrade.DataSource = this.ViewModel.GetGrades(Convert.ToString(e.Context["SchoolType"]));
            this.ddlStudentGrade.DataBind();
        }

        protected void ddlStudentId_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.ddlStudentId.ComboBox.Items.Clear();
            e.NumberOfItems = (e.NumberOfItems == 0 ? 1 : e.NumberOfItems) * 10;

            if (!string.IsNullOrEmpty(Convert.ToString(e.Context["School"])))
                this.ddlStudentId.DataSource = this.ViewModel.GetAllStundentIDBySchool(Convert.ToString(e.Context["SchoolType"]), e.Text, Convert.ToString(e.Context["Grade"]), Convert.ToInt32(e.Context["School"]), string.Empty, e.NumberOfItems / 10);
            else
                this.ddlStudentId.DataSource = this.ViewModel.GetAllStundentIDBySchool(Convert.ToString(e.Context["SchoolType"]), e.Text, Convert.ToString(e.Context["Grade"]), 0, string.Empty, e.NumberOfItems / 10);

            this.ddlStudentId.DataBind();
        }

        protected void ddlStundentName_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            this.ddlStudentName.ComboBox.Items.Clear();

            e.NumberOfItems = (e.NumberOfItems == 0 ? 1 : e.NumberOfItems) * 10;

            if (!string.IsNullOrEmpty(Convert.ToString(e.Context["School"])))
                this.ddlStudentName.DataSource = this.ViewModel.GetAllStundentBySchool(Convert.ToString(e.Context["SchoolType"]), e.Text, Convert.ToString(e.Context["Grade"]), Convert.ToInt32(e.Context["School"]), Convert.ToString(e.Context["StudentId"]), e.NumberOfItems / 10);
            else
                this.ddlStudentName.DataSource = this.ViewModel.GetAllStundentBySchool(Convert.ToString(e.Context["SchoolType"]), e.Text, Convert.ToString(e.Context["Grade"]), 0, Convert.ToString(e.Context["StudentId"]), e.NumberOfItems / 10);

            this.ddlStudentName.DataBind();
        }

        #endregion

        #region export event

        protected void imgExport_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterClientScriptBlock(
            //       this,
            //       typeof (Page),
            //       "anything",
            //       "alert",
            //       true);

            string schoolType = Convert.ToString(ViewState["schoolType"]);
            string schoolId = Convert.ToString(ViewState["schoolId"]);
            string grade = Convert.ToString(ViewState["grade"]);
            string studentId = Convert.ToString(ViewState["studentId"]);
            string studentName = Convert.ToString(ViewState["studentName"]);

            var dtSource = this.ViewModel.GetParentStudentPortalAdministrationReport(
                schoolType,
                schoolId,
                grade,
                studentId,
                studentName);

            DataTable dtExport = new DataTable("ExcelExport");
            dtExport.Columns.Add("SchoolType");
            dtExport.Columns.Add("SchoolName");
            dtExport.Columns.Add("Grade");
            dtExport.Columns.Add("StudentId");
            dtExport.Columns.Add("Student Last Name");
            dtExport.Columns.Add("Student first Name");
            dtExport.Columns.Add("StudentEmail");
            dtExport.Columns.Add("Parent Last Name");
            dtExport.Columns.Add("Parent first Name");
            dtExport.Columns.Add("Email");
            dtExport.Columns.Add("ParentGuardianIndicator");
            dtExport.Columns.Add("ParentGuardianAccessEnabledValue");

            foreach (DataRow sourcerow in dtSource.Rows)
            {
                if (Convert.ToString(sourcerow["IsDeleted"]) != "True")
                {
                    DataRow destRow = dtExport.NewRow();
                    destRow["SchoolType"] = sourcerow["SchoolType"];
                    destRow["SchoolName"] = sourcerow["SchoolName"];
                    destRow["Grade"] = sourcerow["Grade"];
                    destRow["StudentId"] = sourcerow["StudentId"];
                    destRow["Student Last Name"] = sourcerow["Student Last Name"];
                    destRow["Student first Name"] = sourcerow["Student first Name"];
                    destRow["StudentEmail"] = sourcerow["StudentEmail"];
                    destRow["Parent Last Name"] = sourcerow["Parent Last Name"];
                    destRow["Parent first Name"] = sourcerow["Parent first Name"];
                    destRow["Email"] = sourcerow["Email"];
                    destRow["ParentGuardianIndicator"] = sourcerow["ParentGuardianIndicator"];
                    destRow["ParentGuardianAccessEnabledValue"] =
                        sourcerow["ParentGuardianAccessEnabledValue"];
                    dtExport.Rows.Add(destRow);
                }
            }

            HttpContext.Current.Response.Charset = "utf-8";

            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");

            //sets font

            HttpContext.Current.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");

            const string attach = "attachment;filename=StudentReport.xls";
            Response.ClearContent();

            Response.AddHeader("content-disposition", attach);
            //Response.ContentType = "application/ms-excel";
            Response.ContentType = "application/vnd.ms-excel";

            if (dtExport != null)
            {
                try
                {
                    foreach (DataColumn dc in dtExport.Columns)
                    {
                        Response.Write(dc.ColumnName + "\t");
                    }

                    Response.Write(Environment.NewLine);

                    foreach (DataRow dr in dtExport.Rows)
                    {
                        for (int i = 0; i < dtExport.Columns.Count; i++)
                        {
                            Response.Write(Convert.ToString(dr[i]) + "\t");
                        }

                        Response.Write("\n");
                    }

                    Response.End();
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Error: System.Exception: Excel Rendering Extension")
                    {
                        imgExport.Visible = false;
                    }
                    else
                    {
                        imgExport.Visible = true;
                    }
                }
            }
        }

        protected void btnCheckDuplicateEmail_Click(object sender, EventArgs e)
        {
            string strStudentID = Convert.ToString(ViewState["StudentID"]);
            string strUserId = Convert.ToString(ViewState["UserId"]);
            string strParentFirstName = Convert.ToString(ViewState["ParentFirstName"]);
            string strParentLastName = Convert.ToString(ViewState["ParentLastName"]);
            string strEmail = Convert.ToString(ViewState["Email"]);
            string strUserRole = Convert.ToString(ViewState["UserRole"]);
            int parentGuardianAccessEnabledValue = Convert.ToInt16(ViewState["parentGuardianAccessEnabledValue"]);

            DataTable dtCreateParentGuardian = this.ViewModel.CreateParentGuardian(strStudentID, strUserId, strParentFirstName, strParentLastName, strEmail, strUserRole, parentGuardianAccessEnabledValue, false);

            if (dtCreateParentGuardian != null && dtCreateParentGuardian.Rows.Count > 0)
            {
                if (Convert.ToString(dtCreateParentGuardian.Rows[0][0]).Equals("0"))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "EmailAlreadyExistAlert", "EmailAlreadyExistAlert();", true);
                }
            }

            // radParentStudentAccess.Rebind();
            BindParentGrid();


            radParentStudentAccess.MasterTableView.IsItemInserted = false;

            radParentStudentAccess.MasterTableView.ClearEditItems();


        }
        #endregion

        #region parent grid event

        protected void radParentGrid_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "EditParentAccessInformation")
            {
                //if (hdnCheckValueflag.Value == "0")
                //{
                string studentID = Convert.ToString(e.CommandArgument);
                radParentStudentAccess.MasterTableView.IsItemInserted = false;
                ViewState["StudentID"] = studentID;

                if (e.Item is GridDataItem)
                {
                    GridDataItem dataBoundItem = e.Item as GridDataItem;
                    ViewState["UserID"] = dataBoundItem["UserId"].Text;
                    ViewState["StudentName"] = dataBoundItem["StudentName"].Text;
                    // ViewState["ParentGuardianIndicator"] = dataBoundItem["ParentGuardianIndicator"].Text;
                    IsParentGridInEditMode = true;
                }

                DataTable data = this.ViewModel.GetParentStudentAccess(studentID);
                BindRadGridParentStudentAccess(data, studentID);

                DataTable studentData = this.ViewModel.GetStudentInformation(studentID);
                BindStudentGrid(studentData, studentID);
                //}
            }
        }

        protected void radParentGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                var item = e.Item as GridHeaderItem;
                if (item != null)
                {
                    GridHeaderItem hItem = item;
                    CheckBox chk1 = (CheckBox)hItem.FindControl("checkAll");

                    if (chk1 != null)
                        hdnField.Value = Convert.ToString(chk1.Checked);
                }
            }
        }

        protected void radParentGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Header)
            {
                var dataitem = e.Item as GridHeaderItem;

                var parentGuardianAccessEnabledValueAllCheck = false;

                if (ParentData != null)
                {
                    DataTable data = ParentData;

                    parentGuardianAccessEnabledValueAllCheck =
                        data.AsEnumerable().Where(x => !string.IsNullOrWhiteSpace(x.Field<string>("UserId"))).All(x => x.Field<Boolean>("ParentGuardianAccessEnabledValue"));

                    var chk1 = (CheckBox)dataitem.FindControl("checkAll");
                    chk1.Checked = parentGuardianAccessEnabledValueAllCheck;
                }



                //if (dataitem != null)
                //{
                //    GridHeaderItem hItem = dataitem;
                //    CheckBox chk1 = (CheckBox)hItem.FindControl("checkAll");

                //    switch (hdnchk.Value)
                //    {
                //        case "true":
                //            {

                //                chk1.Checked = true;

                //            }
                //            break;
                //        case "false":
                //            {

                //                chk1.Checked = false;
                //            }
                //            break;
                //        default:
                //            chk1.Checked = (parentGuardianAccessEnabledValueAllCheck) ? true : false;
                //            break;
                //    }
                //}
            }

            if (e.Item is GridPagerItem)
            {
                RadComboBox pageSizeCombo = (RadComboBox)e.Item.FindControl("PageSizeComboBox");
                pageSizeCombo.Items.Clear();

                int i = 10;
                while (i <= 50)
                {
                    pageSizeCombo.Items.Add(new RadComboBoxItem(Convert.ToString(i)));
                    pageSizeCombo.FindItemByText(Convert.ToString(i)).Attributes.Add("ownerTableViewId", radParentGrid.MasterTableView.ClientID);
                    i = i + 10;
                }
                pageSizeCombo.FindItemByText(Convert.ToString(e.Item.OwnerTableView.PageSize)).Selected = true;
            }
        }

        protected void radSaveParentGrid_Click(object sender, EventArgs e)
        {
            string strStudentId = string.Empty;
            string strAccess = string.Empty;
            string strUserIds = string.Empty;

            if (string.IsNullOrEmpty(hdnchk.Value))
            {
                foreach (GridDataItem di in radParentGrid.Items)
                {
                    Label lbl = (Label)(di.FindControl("lblStudentID"));
                    CheckBox chkBoxd = (CheckBox)(di.FindControl("cboxSelect"));

                    strStudentId += lbl.Text + ",";

                    string userId = !string.IsNullOrWhiteSpace(di["UserId"].Text) ? di["UserId"].Text.Replace("&nbsp;", "null") : "null";
                    strUserIds += userId + ",";

                    switch (hdnchk.Value)
                    {
                        case "true":
                            {
                                chkBoxd.Checked = true;
                            }
                            break;
                        case "false":
                            {
                                chkBoxd.Checked = false;
                            }
                            break;
                    }

                    string strAccessValue = Convert.ToString(chkBoxd.Checked);

                    if (strAccessValue != "True")
                        strAccessValue = "0";
                    else
                        strAccessValue = "1";

                    strAccess += strAccessValue + ",";
                }
                this.ViewModel.UpdateParentGuardianAccess(
                  strStudentId.TrimEnd(','),
                  strAccess.TrimEnd(','),
                  strUserIds.TrimEnd(','), false);
            }
            else
            {

                this.ViewModel.UpdateParentGuardianAccess("", "", "", Convert.ToBoolean(hdnchk.Value));
            }
        }

        protected void radParentGrid_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            radParentGrid.CurrentPageIndex = e.NewPageIndex;
            BindParentGrid();
        }

        protected void radParentGrid_OnPageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
        {
            if (hdnCheckValueflag.Value == "0")
            {
                radParentGrid.CurrentPageIndex = 0;
                BindParentGrid();
            }
        }

        protected void radParentGrid_OnPreRender(object sender, EventArgs e)
        {
            radParentGrid.MasterTableView.GetColumn("UserId").Visible = true;
            radParentGrid.MasterTableView.GetColumn("UserId").Display = false;

            foreach (GridDataItem item in radParentGrid.MasterTableView.Items)
            {
                Label stdId = (Label)item.FindControl("lblStudentID");
                ImageButton imgEdit = (ImageButton)item.FindControl("imgEditParentAccessInformation");
                var stdItemID = stdId.Text;

                string parentGuardianName = !string.IsNullOrWhiteSpace(item["ParentGuardianName"].Text) ? item["ParentGuardianName"].Text.Replace("&nbsp;", "") : null;

                if (string.IsNullOrWhiteSpace(parentGuardianName))
                {
                    var checkboxItem = ((GridEditableItem)(item)).FindControl("cboxSelect");

                    if (checkboxItem != null)
                        checkboxItem.Visible = false;
                }
                else
                {
                    CheckBox chkBoxd = (CheckBox)(item.FindControl("cboxSelect"));

                    switch (hdnchk.Value)
                    {
                        case "true":
                            {
                                chkBoxd.Checked = true;
                            }
                            break;
                        case "false":
                            {
                                chkBoxd.Checked = false;
                            }
                            break;
                    }
                }

                if (string.IsNullOrWhiteSpace(StudentId) || StudentId != stdItemID)
                {
                    StudentId = stdItemID;
                    stdId.Style.Add("display", "block");
                }
                else
                {
                    stdId.Style.Add("display", "None");
                    item["StudentName"].Text = "";
                    item["SchoolName"].Text = "";
                    item["Grade"].Text = "";
                    item["StudentEmail"].Text = "";
                }

            }
        }

        #endregion

        #region child grid event

        protected void radParentStudentAccess_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                string strRegex = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                 + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                 + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                 + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

                string strRegesxAlpaNumeric = @"^[a-zA-Z0-9 ]*$";

                GridEditableItem item = e.Item as GridEditableItem;

                GridTextBoxColumnEditor parentFirstName = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("ParentFirstName");
                TableCell cellParentFirstName = (TableCell)parentFirstName.TextBoxControl.Parent;
                RegularExpressionValidator validateParentFirstName = new RegularExpressionValidator();
                validateParentFirstName.ControlToValidate = parentFirstName.TextBoxControl.ID;
                validateParentFirstName.ValidationExpression = strRegesxAlpaNumeric;
                validateParentFirstName.ErrorMessage = "Enter Alphanumeric Text Only";
                validateParentFirstName.ForeColor = Color.Red;
                validateParentFirstName.Font.Size = 9;
                cellParentFirstName.Controls.Add(validateParentFirstName);

                GridTextBoxColumnEditor parentLastName = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("ParentLastName");
                TableCell cellParentLastName = (TableCell)parentLastName.TextBoxControl.Parent;
                RegularExpressionValidator validateParentLastName = new RegularExpressionValidator();
                validateParentLastName.ControlToValidate = parentLastName.TextBoxControl.ID;
                validateParentLastName.ValidationExpression = strRegesxAlpaNumeric;
                validateParentLastName.ErrorMessage = "Enter Alphanumeric Text Only";
                validateParentLastName.ForeColor = Color.Red;
                validateParentLastName.Font.Size = 9;
                cellParentLastName.Controls.Add(validateParentLastName);

                GridTextBoxColumnEditor editor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("Email");
                TableCell cell = (TableCell)editor.TextBoxControl.Parent;
                RegularExpressionValidator validate = new RegularExpressionValidator();
                validate.ControlToValidate = editor.TextBoxControl.ID;
                validate.ValidationExpression = strRegex;
                validate.ErrorMessage = "Invalid Email Format";
                validate.ForeColor = Color.Red;
                validate.Font.Size = 9;
                cell.Controls.Add(validate);

                if (e.Item is GridDataItem)
                {
                    GridDataItem dataItem = e.Item as GridDataItem;
                    ImageButton lnkbtn = (ImageButton)dataItem.FindControl("imgDelete");
                    lnkbtn.Style.Add("display", "none");
                }
            }
        }

        int index = -1;
        bool _isValidForm = false;
        bool _isDuplicateEmail = false;

        protected void radParentStudentAccess_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            if (item == null) return;

            Hashtable newValues = new Hashtable();
            item.OwnerTableView.ExtractValuesFromItem(newValues, item);

            GridEditableItem update = (GridEditableItem)e.Item;
            RadComboBox combo = (RadComboBox)update.FindControl("ParentGuardianIndicator");

            string strStudentID = Convert.ToString(newValues["StudentID"]);
            string strUserId = newValues["UserId"] == null ? string.Empty : Convert.ToString(newValues["UserId"]).ToLower();
            string strParentFirstName = newValues["ParentFirstName"] == null ? string.Empty : Convert.ToString(newValues["ParentFirstName"]).Trim();
            string strParentLastName = newValues["ParentLastName"] == null ? string.Empty : Convert.ToString(newValues["ParentLastName"]).Trim();
            string strEmail = newValues["Email"] == null ? string.Empty : Convert.ToString(newValues["Email"]).Trim();
            string strUserRole = string.IsNullOrEmpty(combo.Text) ? string.Empty : Convert.ToString(combo.Text).ToLower();
            bool boolParentGuardianAccessEnabledValue = Convert.ToBoolean(newValues["ParentGuardianAccessEnabledValue"]);

            ViewState["ParentFirstName"] = Convert.ToString(strParentFirstName);
            ViewState["ParentLastName"] = Convert.ToString(strParentLastName);
            ViewState["Email"] = Convert.ToString(strEmail);

            if (string.IsNullOrEmpty(Convert.ToString(strParentFirstName)))
            {
                TextBox dataField = (TextBox)item["ParentFirstName"].Controls[0];
                dataField.Focus();
                _isValidForm = true;
            }
            if (string.IsNullOrEmpty(Convert.ToString(strParentLastName)))
            {
                TextBox dataField = (TextBox)item["ParentLastName"].Controls[0];
                dataField.Focus();
                _isValidForm = true;
            }
            if (string.IsNullOrEmpty(Convert.ToString(strEmail)))
            {
                TextBox dataField = (TextBox)item["Email"].Controls[0];
                dataField.Focus();
                _isValidForm = true;
            }

            if (_isValidForm)
            {
                //var msg =
                //    @"Please confirm that the first Name, Last Name, Email and Parent/Guardian indicator fields are all completed for the defined users.";
                //ScriptManager.RegisterStartupScript(
                //    this,
                //    typeof(Page),
                //    "Message2",
                //    "alert('" + msg + "');",
                //    true);
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationMessageforTextControls", "ValidationMessageforTextControls();", true);
                index = update.ItemIndex;
                return;
            }

            strUserRole = (strUserRole.Equals("p")) ? Convert.ToString(Enums.ParentStudentPortalAccess.Parent) : Convert.ToString(Enums.ParentStudentPortalAccess.Guardian);
            int parentGuardianAccessEnabledValue = (boolParentGuardianAccessEnabledValue == false) ? 0 : 1;

            DataTable dt = this.ViewModel.CreateParentGuardian(strStudentID, strUserId, strParentFirstName, strParentLastName, strEmail, strUserRole, parentGuardianAccessEnabledValue, false);

            if (dt != null && dt.Rows.Count > 0)
            {
                if (Convert.ToString(dt.Rows[0][0]).Equals("0"))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "alert", "alert('The email entered matches an existing entry.');", true);
                }
            }


            IsParentGridInEditMode = false;

            //radParentStudentAccess.Rebind();
            BindParentGrid();
        }

        protected void radParentStudentAccess_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode && e.Item is GridEditableItem)
            {
                if (e.Item.ItemIndex == -1)
                {
                    // insert new record
                    GridDataItem dataItem = (GridDataItem)e.Item;
                    dataItem.BackColor = Color.LimeGreen;
                }
                if (e.Item.ItemIndex != -1)
                {
                    GridEditableItem item = (GridEditableItem)e.Item;
                    if (item != null)
                    {
                        RadComboBox ddlParentGuardianIndicator = (RadComboBox)item.FindControl("ParentGuardianIndicator");
                        ddlParentGuardianIndicator.SelectedValue =
                            Convert.ToString(item.GetDataKeyValue("ParentGuardianIndicator"));

                    }
                }
            }

        }

        protected void radParentStudentAccess_ItemCommand(object sender, GridCommandEventArgs e)
        {

            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {

                if (String.Compare(e.CommandName, "MessageDelete", StringComparison.Ordinal) == 0)
                {
                    var strUserId = string.Empty;

                    string strStudentId = Convert.ToString(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["StudentID"]);

                    if (e.Item is GridDataItem)
                    {
                        GridDataItem dataBoundItem = e.Item as GridDataItem;

                        strUserId = dataBoundItem["UserId"].Text;
                    }

                    this.ViewModel.DeleteParentGuardianAccess(strStudentId, strUserId);
                    //radParentStudentAccess.Rebind();

                    BindParentGrid();
                }
            }

            if (e.CommandName == "PerformInsert")
            {
                GridDataInsertItem editItem = (GridDataInsertItem)radParentStudentAccess.MasterTableView.GetInsertItem();
                RadComboBox list = (RadComboBox)editItem.FindControl("ParentGuardianIndicator");
                CheckBox chk = editItem["GridCheckBoxAccessEnabled"].Controls[0] as CheckBox;

                string strStudentID = Convert.ToString(ViewState["StudentID"]);
                string strUserId = (editItem["UserId"].Controls[0] as TextBox).Text.Trim();
                string strParentFirstName = (editItem["ParentFirstName"].Controls[0] as TextBox).Text.Trim();
                string strParentLastName = (editItem["ParentLastName"].Controls[0] as TextBox).Text.Trim();
                string strEmail = (editItem["Email"].Controls[0] as TextBox).Text.Trim();
                string strUserRole = (list != null) ? list.Text : "P";
                strUserRole = (strUserRole.Equals("P")) ? "Parent" : "Guardian";

                int parentGuardianAccessEnabledValue = (chk.Checked == false) ? 0 : 1;

                ViewState["UserId"] = strUserId;
                ViewState["ParentFirstName"] = strParentFirstName;
                ViewState["ParentLastName"] = strParentLastName;
                ViewState["Email"] = strEmail;
                ViewState["UserRole"] = strUserRole;
                ViewState["parentGuardianAccessEnabledValue"] = parentGuardianAccessEnabledValue;

                if (string.IsNullOrEmpty(Convert.ToString(strParentFirstName)) || string.IsNullOrEmpty((Convert.ToString(strParentLastName))) || string.IsNullOrEmpty(Convert.ToString(strEmail)))
                {
                    TextBox dataField = (TextBox)editItem["ParentFirstName"].Controls[0];
                    dataField.Focus();

                    //var msg = @"Please confirm that the first Name, Last Name, Email and Parent/Guardian indicator fields are all completed for the defined users.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ValidationMessageforTextControls", "ValidationMessageforTextControls();", true);

                    index = editItem.ItemIndex;
                    return;
                }

                DataTable dt = this.ViewModel.CheckDuplicateEmail(strEmail);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0][0]).Equals("1"))
                    {
                        _isDuplicateEmail = true;
                        ScriptManager.RegisterStartupScript(this, GetType(), "CheckDuplicateEmailAlert", string.Format("CheckDuplicateEmailAlert('{0}','{1}','{2}');", strParentFirstName, strParentLastName, strEmail), true);
                        e.Canceled = true;
                    }

                    else
                    {
                        DataTable dtCreateParentGuardian = this.ViewModel.CreateParentGuardian(strStudentID, strUserId, strParentFirstName, strParentLastName, strEmail, strUserRole, parentGuardianAccessEnabledValue, false);
                        _isDuplicateEmail = false;
                        if (dtCreateParentGuardian != null && dtCreateParentGuardian.Rows.Count > 0)
                        {
                            if (Convert.ToString(dtCreateParentGuardian.Rows[0][0]).Equals("0"))
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "EmailAlreadyExistAlert", "EmailAlreadyExistAlert();", true);
                            }
                        }
                    }
                }

                //radParentStudentAccess.Rebind();
                BindParentGrid();
            }


            if (e.CommandName == RadGrid.EditCommandName)
            {
                radParentStudentAccess.MasterTableView.IsItemInserted = false;
            }
            if (e.CommandName == RadGrid.InitInsertCommandName)
            {
                radParentStudentAccess.MasterTableView.ClearEditItems();
            }

        }

        protected void radParentStudentAccess_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            string studentID = Convert.ToString(ViewState["StudentID"]);
            System.Data.DataTable data = this.ViewModel.GetParentStudentAccess(studentID);


            this.divChildGrid.Visible = true;
            this.radParentStudentAccess.Visible = true;

            if (_isValidForm)
            {
                // DataTable dt = new DataTable();
                foreach (DataRow row in data.Rows)
                {
                    foreach (DataColumn c in data.Columns)
                    {
                        row["ParentFirstName"] = Convert.ToString(ViewState["ParentFirstName"]);
                        row["ParentLastName"] = Convert.ToString(ViewState["ParentLastName"]);
                        row["Email"] = Convert.ToString(ViewState["Email"]);
                    }
                }

                data.AcceptChanges();

            }

            this.radParentStudentAccess.DataSource = data;

            lblStudentID.Text = Extensions.Description(Enums.ParentStudentPortalAccess.HeaderText) + "  " + Convert.ToString(ViewState["StudentName"]) + " " + "( " + Convert.ToString(ViewState["StudentID"]) + " )";
            if (_isDuplicateEmail)
            {
                radParentStudentAccess.MasterTableView.IsItemInserted = true;
                //radParentStudentAccess.Rebind();
            }

            this.radParentStudentAccess.Visible = true;
            this.divChildGrid.Visible = true;
        }

        protected void radParentStudentAccess_PreRender(object sender, EventArgs e)
        {
            if (!_isDuplicateEmail)
            {
                radParentStudentAccess.MasterTableView.GetColumn("UserId").Visible = true;
                radParentStudentAccess.MasterTableView.GetColumn("UserId").Display = false;
                radParentStudentAccess.MasterTableView.GetColumn("StudentID").Visible = true;
                radParentStudentAccess.MasterTableView.GetColumn("StudentID").Display = false;

                if (index > -1)
                {
                    radParentStudentAccess.MasterTableView.Items[index].Edit = true;
                    radParentStudentAccess.MasterTableView.Rebind();
                }

                if (IsParentGridInEditMode)
                {
                    foreach (GridItem item in radParentStudentAccess.MasterTableView.Items)
                    {
                        if (item is GridEditableItem)
                        {
                            GridEditableItem editableItem = item as GridDataItem;

                            if (editableItem != null)
                            {
                                if (
                                    Convert.ToString(ViewState["UserID"])
                                           .Equals(Convert.ToString(editableItem["UserId"].Text)))
                                {
                                    editableItem.Edit = true;
                                }
                                else
                                {
                                    editableItem.Edit = false;
                                }
                            }
                        }
                    }
                }

                radParentStudentAccess.Rebind();
            }
        }

        protected void imgCloseChildGrid_Click(object sender, ImageClickEventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, typeof(Page), "confirm", "confirm('Would you like to discard your changes and return to th einterface ?\\nSelect OK to proceed or Cancel to resume the in progress update.');", true);
            // Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenConfirm", "confirm('Confirm Password Change?')", true);

            //DialogResult dr = MessageBox.Show("Message.", "Title", MessageBoxButtons.YesNoCancel,
            //MessageBoxIcon.Information);

            //if (dr == DialogResult.Yes)
            //{
            //    // Do something
            //}
            this.divChildGrid.Visible = false;
        }

        #endregion

        #region Student grid event

        protected void radStudentInformation_ItemCreated(object sender, GridItemEventArgs e)
        {

            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                string strRegex = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                 + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                 + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                 + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

                GridEditableItem item = e.Item as GridEditableItem;

                GridTextBoxColumnEditor editor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor("StudentEmail");
                TableCell cell = (TableCell)editor.TextBoxControl.Parent;
                RegularExpressionValidator validate = new RegularExpressionValidator();
                System.Web.UI.WebControls.TextBox dataField = (System.Web.UI.WebControls.TextBox)item["StudentEmail"].Controls[0];
                dataField.Focus();
                validate.ControlToValidate = editor.TextBoxControl.ID;
                validate.ValidationExpression = strRegex;
                validate.ErrorMessage = "Invalid Email Format";
                validate.ForeColor = Color.Red;
                validate.Font.Size = 9;
                cell.Controls.Add(validate);
            }
        }

        protected void radStudentInformation_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            string studentID = Convert.ToString(ViewState["StudentID"]);
            System.Data.DataTable studentData = this.ViewModel.GetStudentInformation(studentID);

            if (studentData != null && studentData.Rows.Count > 0)
            {

                this.divStudentGrid.Visible = true;
                this.radStudentInformation.Visible = true;

                this.radStudentInformation.DataSource = studentData;
            }
            else
            {
                this.radStudentInformation.Visible = false;
                this.divStudentGrid.Visible = false;
            }
        }

        protected void radStudentInformation_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            if (item == null) return;

            Hashtable newValues = new Hashtable();
            item.OwnerTableView.ExtractValuesFromItem(newValues, item);

            var userId = Convert.ToString(item.GetDataKeyValue("UserId"));

            string strUserId = newValues["UserId"] == null ? string.Empty : Convert.ToString(newValues["UserId"]).ToLower();

            String strStudentID = Convert.ToString(newValues["StudentID"]);
            string strEmail = newValues["StudentEmail"] == null ? string.Empty : Convert.ToString(newValues["StudentEmail"]);



            System.Data.DataTable dt = this.ViewModel.UpdateStudentInformation(strStudentID, strEmail, userId);

            if (dt != null && dt.Rows.Count > 0)
            {
                if (Convert.ToString(dt.Rows[0][0]).Equals("0"))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(System.Web.UI.Page), "alert", "alert('The email entered matches an existing entry.');", true);
                }
            }

            radStudentInformation.Rebind();
            BindParentGrid();

        }

        protected void radStudentInformation_OnPreRender(object sender, EventArgs e)
        {
            string studentID = Convert.ToString(ViewState["StudentID"]);
            System.Data.DataTable studentData = this.ViewModel.GetStudentInformation(studentID);

            if (studentData != null && studentData.Rows.Count > 0)
                lblStudentInformation.Text = Extensions.Description(Thinkgate.Enums.ParentStudentPortalAccess.StudentHeaderText) + " " + Convert.ToString(studentData.Rows[0]["StudentName"]) + " ( " + studentID + " )";

            radStudentInformation.MasterTableView.GetColumn("StudentID").Visible = true;
            radStudentInformation.MasterTableView.GetColumn("StudentID").Display = false;

            radStudentInformation.Rebind();
        }

        protected void imgCloseStudentGrid_Click(object sender, ImageClickEventArgs e)
        {
            this.divStudentGrid.Visible = false;
        }

        #endregion

        #region User defined function

        protected void SearchHandler(object sender, CriteriaController model)
        {
            imgExport.Visible = true;

            if (hdnCheckValueflag.Value == "0")
                BindRadGridResults(model);
        }

        public void BindRadGridResults(CriteriaController model)
        {
            this.dvEmpty.Visible = false;

            this.radParentStudentAccess.Visible = false;
            this.divChildGrid.Visible = false;
            radStudentInformation.Visible = false;
            divStudentGrid.Visible = false;

            string schoolType = string.Join(",", model.ParseCriteria<E3Criteria.RadDropDownList.ValueObject>(this.ddlSchoolType.CriteriaName).Select(x => x.Value));
            string schoolId = string.Join(",", model.ParseCriteria<E3Criteria.RadDropDownList.ValueObject>(this.ddlSchool.CriteriaName).Select(x => x.Value));
            string grade = string.Join(",", model.ParseCriteria<E3Criteria.RadDropDownList.ValueObject>(this.ddlStudentGrade.CriteriaName).Select(x => x.Value));
            string studentId = string.Join(",", model.ParseCriteria<E3Criteria.RadDropDownList.ValueObject>(this.ddlStudentId.CriteriaName).Select(x => x.Value));
            string studentName = string.Join(",", model.ParseCriteria<E3Criteria.RadDropDownList.ValueObject>(this.ddlStudentName.CriteriaName).Select(x => x.Text));

            ViewState["schoolType"] = schoolType;
            ViewState["schoolId"] = schoolId;
            ViewState["grade"] = grade;
            ViewState["studentId"] = studentId;

            if (ViewState["studentName"] != null)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(ViewState["studentName"])))
                {
                    if (Convert.ToString(ViewState["studentName"]) == studentName)
                    {
                        this.radParentStudentAccess.Visible = true;
                        this.divChildGrid.Visible = true;
                        radStudentInformation.Visible = true;
                        divStudentGrid.Visible = true;
                    }
                }
            }

            ViewState["studentName"] = studentName;

            var data = this.ViewModel.GetParentStudentPortalAdministrationReport(schoolType, schoolId, grade, studentId, studentName);

            var sb = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser).Select(x => x.Name).Distinct().ToList();

            if (data != null & data.Rows.Count > 0)
            {
                var filteredDataforloggedInUser =
                    data.AsEnumerable()
                        .Where(x => sb.Contains(Convert.ToString(x["SchoolName"])))
                        .CopyToDataTable();


                if (filteredDataforloggedInUser != null
                    && filteredDataforloggedInUser.Rows.Count > 0)
                {
                    ParentData = filteredDataforloggedInUser;
                    lblNoResult.Visible = false;
                    this.radParentGrid.Visible = true;
                    this.divsave.Visible = true;

                    radParentGrid.CurrentPageIndex = 0;

                    this.radParentGrid.DataSource = filteredDataforloggedInUser;
                    this.radParentGrid.DataBind();
                }
                else
                {
                    lblNoResult.Visible = true;
                    this.radParentGrid.Visible = false;
                    this.divsave.Visible = false;
                    this.divChildGrid.Visible = false;
                    this.divStudentGrid.Visible = false;
                    this.imgExport.Visible = false;
                }
            }
            else
            {
                lblNoResult.Visible = true;
                this.radParentGrid.Visible = false;
                this.divsave.Visible = false;
                this.divChildGrid.Visible = false;
                this.divStudentGrid.Visible = false;
                this.imgExport.Visible = false;

            }
        }

        public void BindParentGrid()
        {
            string schoolType = Convert.ToString(ViewState["schoolType"]);
            string schoolId = Convert.ToString(ViewState["schoolId"]);
            string grade = Convert.ToString(ViewState["grade"]);
            string studentId = Convert.ToString(ViewState["studentId"]);
            string studentName = Convert.ToString(ViewState["studentName"]);

            var data = this.ViewModel.GetParentStudentPortalAdministrationReport(schoolType, schoolId, grade, studentId, studentName);

            var sb = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser).Select(x => x.Name).Distinct().ToList();

            if (data != null && data.Rows.Count > 0)
            {
                var filteredDataforloggedInUser =
                    data.AsEnumerable()
                        .Where(x => sb.Contains(Convert.ToString(x["SchoolName"])))
                        .CopyToDataTable();

                if (filteredDataforloggedInUser != null
                   && filteredDataforloggedInUser.Rows.Count > 0)
                {
                    lblNoResult.Visible = false;
                    this.radParentGrid.Visible = true;
                    this.divsave.Visible = true;

                    this.radParentGrid.DataSource = filteredDataforloggedInUser;
                    ParentData = filteredDataforloggedInUser;
                    this.radParentGrid.DataBind();
                }
                else
                {
                    lblNoResult.Visible = true;
                    this.radParentGrid.Visible = false;
                    this.divsave.Visible = false;
                    this.radParentStudentAccess.Visible = false;
                    this.divChildGrid.Visible = false;
                    radStudentInformation.Visible = false;
                    divStudentGrid.Visible = false;
                    this.imgExport.Visible = false;
                }
            }
            else 
            {
                lblNoResult.Visible = true;
                this.radParentGrid.Visible = false;
                this.divsave.Visible = false;
                this.divChildGrid.Visible = false;
                this.divStudentGrid.Visible = false;
                this.imgExport.Visible = false;

            }
        }

        public void BindRadGridParentStudentAccess(System.Data.DataTable data, string studentID)
        {

            this.divChildGrid.Visible = true;
            this.radParentStudentAccess.Visible = true;

            this.radParentStudentAccess.DataSource = data;
            this.radParentStudentAccess.DataBind();

            lblStudentID.Text = Extensions.Description(Enums.ParentStudentPortalAccess.HeaderText) + "  " + Convert.ToString(ViewState["StudentName"]) + " " + "( " + Convert.ToString(ViewState["StudentID"]) + " )";

            this.radParentStudentAccess.Visible = true;
            this.divChildGrid.Visible = true;
        }

        public void BindStudentGrid(System.Data.DataTable studentData, string studentID)
        {
            if (studentData != null && studentData.Rows.Count > 0)
            {

                lblStudentInformation.Text = Extensions.Description(Enums.ParentStudentPortalAccess.StudentHeaderText) + " " + Convert.ToString(studentData.Rows[0]["StudentName"]) + "  " + "( " + studentID + " )";
                this.divStudentGrid.Visible = true;
                this.radStudentInformation.Visible = true;

                this.radStudentInformation.DataSource = studentData;
                this.radStudentInformation.DataBind();
            }
            else
            {
                this.radStudentInformation.Visible = false;
                this.divStudentGrid.Visible = false;
            }
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

            if (hdnCheckValueflag.Value == "1")
                ScriptManager.RegisterClientScriptBlock(this, typeof(string), "MandatorytoSaveAfterChangeInCheckEvent", "MandatorytoSaveAfterChangeInCheckEvent()", true);
        }

        #endregion
    }
}
