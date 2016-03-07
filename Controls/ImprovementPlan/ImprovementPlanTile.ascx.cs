using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Enums.ImprovementPlan;
using Thinkgate.Services.Contracts.ImprovementPlanService;

namespace Thinkgate.Controls.ImprovementPlan
{
  public partial class ImprovementPlanTile : TileControlBase
  {

      protected ImprovementPlanType ImpPlanType;
      protected int? SchoolID;
      private DistrictParms _districtParms;
      private ImprovementPlanProxy _improvementPlanService;
      private List<ImprovementPlanTileOutput> _improvementPlanList;

      private SessionObject sessionObject;

        protected new void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            sessionObject = (SessionObject)Session["SessionObject"];
            ImpPlanType = (ImprovementPlanType)Tile.TileParms.GetParm("improvementPlanType");
            if (Tile.TileParms.Parms.ContainsKey("schoolID"))
            {
                SchoolID = (int)Tile.TileParms.GetParm("schoolID");
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            _districtParms = DistrictParms.LoadDistrictParms();
            _improvementPlanService = new ImprovementPlanProxy();
            String postBackControlID = GetControlThatCausedPostBack(Parent.Page);
            bool isPostBack = !String.IsNullOrEmpty(postBackControlID) && !postBackControlID.StartsWith("folder") && !postBackControlID.StartsWith("tileContainer");
            
            SetFilterVisibility();
            DisableBtnAdd();

            if (!isPostBack)
            {
                LoadDropDownYear();
                LoadDropDownSchool();
               
            }

            LoadImprovementPlanList();
        }

        protected void SetFilterVisibility()
        {
            cmbSchool.Visible = ImpPlanType == ImprovementPlanType.School;
            cmbYearSchool.Visible = ImpPlanType == ImprovementPlanType.School;
            cmbYearDistrict.Visible = ImpPlanType == ImprovementPlanType.District; 
            if (cmbSchool.Visible && SchoolID != default(int?)) cmbSchool.SelectedValue = SchoolID.ToString();
        }

        private void LoadDropDownYear()
        {

            List<String> yearList = _improvementPlanService.GetImprovementPlanYearByClientId(_districtParms.ClientID).ToList();
            yearList.Insert(0,"Year");
            cmbYearDistrict.DataSource = yearList;
            cmbYearDistrict.DataBind();
            cmbYearSchool.DataSource = yearList;
            cmbYearSchool.DataBind();
        }

        private void LoadDropDownSchool()
        {

            var schoolsForLooggedInUser = SchoolMasterList.GetSchoolsForUser(SessionObject.LoggedInUser);
            schoolsForLooggedInUser = SchoolID != default(int?) ? schoolsForLooggedInUser.Where(e => e.ID == SchoolID).ToList() : schoolsForLooggedInUser.Where(e => e.Name != "District Office").ToList();
            if (schoolsForLooggedInUser.Count > 1)
            {
                Base.Classes.School firstSchool = new Base.Classes.School {Name = "School", ID = 0};
                schoolsForLooggedInUser.Insert(0,firstSchool);
            }

            cmbSchool.DataTextField = "Name";
            cmbSchool.DataValueField = "ID";
            cmbSchool.DataSource = schoolsForLooggedInUser;
            cmbSchool.DataBind();
        }

        private void LoadImprovementPlanList()
        {
            var inputParamater = new ImprovementPlanTileInput {SchoolID = SchoolID, PlanType = ImpPlanType};
            _improvementPlanList = _improvementPlanService.GetImprovementPlanByPlanType(inputParamater, _districtParms.ClientID).ToList();

           
           
            string year = null;
            if (ImpPlanType == ImprovementPlanType.District)
            {
                if (cmbYearDistrict.SelectedValue != "Year") year = cmbYearDistrict.SelectedValue;
        }
            else
            {
                if (cmbYearSchool.SelectedValue != "Year") year = cmbYearSchool.SelectedValue;
                if (cmbSchool.SelectedValue != "0")
                    SchoolID = Convert.ToInt32(cmbSchool.SelectedValue);
            }

            FilterAndDisplayResult(ImpPlanType, SchoolID, year);

        }

      private void FilterAndDisplayResult(ImprovementPlanType type, int? school, string year = null)
      {

          var improvementPlanDisplayList =
              _improvementPlanList.Where(
                  x =>
                      (x.PlanType == type) && (school == default(int?) || (x.School != null && x.School.ID == school)) &&
                      (string.IsNullOrEmpty(year) || x.Year == year)).ToList();
          if (improvementPlanDisplayList.Count <= 0)
          {
              pnlNoResults.Visible = true;
              lbxImprovementPlanList.Visible = false;
              if (!string.IsNullOrEmpty(year) &&
                  (ImpPlanType == ImprovementPlanType.District || (school != default(int?))))
              {
                  EnableBtnAdd(year,school);
              }
              else
              {
                  DisableBtnAdd();
              }
          }
          else
          {
              improvementPlanDisplayList = improvementPlanDisplayList.OrderBy(order => order.Year).ToList();

              pnlNoResults.Visible = false;
              lbxImprovementPlanList.Visible = true;
              DisableBtnAdd();

              lbxImprovementPlanList.DataSource = improvementPlanDisplayList;
              lbxImprovementPlanList.DataBind();
          }
      }

      protected void cmbYear_OnSelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
      {
          string year = ImpPlanType== ImprovementPlanType.District ? cmbYearDistrict.SelectedValue : cmbYearSchool.SelectedValue;
          if (year == "Year") year = null;
          string school = ImpPlanType== ImprovementPlanType.District ? null : cmbSchool.SelectedValue;
          SchoolID = (string.IsNullOrEmpty(school) || school == "0") ? default(int?) : Convert.ToInt32(school);
          FilterAndDisplayResult(ImpPlanType, SchoolID, year);
      }

      protected void cmbSchool_OnSelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
      {
          string school = cmbSchool.SelectedValue;
          SchoolID = (string.IsNullOrEmpty(school) || school == "0") ? default(int?) : Convert.ToInt32(school);
          string year = ImpPlanType == ImprovementPlanType.District ? cmbYearDistrict.SelectedValue : cmbYearSchool.SelectedValue;
          if (year == "Year") year = null;
          FilterAndDisplayResult(ImpPlanType, SchoolID, year);

      }

      protected void lbxList_ItemDataBound(object sender, RadListBoxItemEventArgs e)
      {
          RadListBoxItem listBoxItem = e.Item;
          var row = (ImprovementPlanTileOutput)(listBoxItem).DataItem;
          string schoolname = string.Empty;
          switch (row.PlanType)
          {
              case ImprovementPlanType.District:
                  break;
              case ImprovementPlanType.School:
                  schoolname = row.School.SchoolName;
                  break;
          }
          HyperLink hlkTestname = (HyperLink)listBoxItem.FindControl("lnkImpPlanName");
          hlkTestname.Text = row.Year + " " + row.PlanType + " Improvement Plan " + schoolname;
         // hlkTestname.NavigateUrl = "~/ImprovementPlan/ImprovementPlanViewMode.aspx?impID=" + Cryptography.EncryptInt(row.ImprovementPlanID, sessionObject.LoggedInUser.CipherKey) + "&actType=" + Cryptography.EncryptString(ActionType.View.ToString(), sessionObject.LoggedInUser.CipherKey);
          hlkTestname.NavigateUrl = "~/ImprovementPlan/ImprovementPlanViewMode.aspx?impID=" +row.ImprovementPlanID + "&actType=" + ActionType.View;

          HyperLink hlkEdit = (HyperLink)listBoxItem.FindControl("hlkEdit");
          hlkEdit.Visible = (ImprovementPlanType) row.PlanType == ImprovementPlanType.District ? UserHasPermission(Permission.Icon_Edit_DistrictImprovementPlan)
              : UserHasPermission(Permission.Icon_Edit_SchoolImprovementPlan);
          //hlkEdit.NavigateUrl = "~/ImprovementPlan/ImprovementCoverPage.aspx?impID=" + Cryptography.EncryptInt(row.ImprovementPlanID, sessionObject.LoggedInUser.CipherKey) + "&actType=" + Cryptography.EncryptString(ActionType.Edit.ToString(), sessionObject.LoggedInUser.CipherKey);
          hlkEdit.NavigateUrl = "~/ImprovementPlan/ImprovementCoverPage.aspx?impID=" + row.ImprovementPlanID + "&actType=" + ActionType.Edit;
          
      }

      protected void DisableBtnAdd()
      {

          BtnAdd.Attributes["onclick"] = "return false;";
          BtnAdd.Attributes["style"] = "cursor:default; margin-top: -1px;";
          BtnAddSpan.Attributes["style"] = "opacity:.5; filter:progid:DXImageTransform.Alpha(opacity=50); filter:alpha(opacity=50);";
          BtnAddDiv.Attributes["style"] = "padding: 0; opacity:.5; filter:progid:DXImageTransform.Alpha(opacity=50); filter:alpha(opacity=50);"; 
      
      }

      protected void EnableBtnAdd(string year, int? school)
      {
          if ((ImpPlanType == ImprovementPlanType.District && UserHasPermission(Permission.Icon_Edit_DistrictImprovementPlan) )
              || (ImpPlanType == ImprovementPlanType.School && UserHasPermission(Permission.Icon_Edit_SchoolImprovementPlan)))
          {
              BtnAdd.Attributes["onclick"] = "customDialog({url: '../ImprovementPlan/ImprovementCoverPage.aspx?year=" +
                                           year + "&schoolID=" + school + "&plantype=" + ImpPlanType + "&actType=" + ActionType.Edit + "', title: 'Add New Improvement Plan',maximize: true}); inProgressTileRefresh();";

              BtnAdd.Attributes["style"] = "cursor: pointer; margin-top: -1px;";

              if (Request.Browser.Type.ToUpper().Contains("IE") && Request.Browser.MajorVersion < 9)
              {
                  BtnAddSpan.Attributes["style"] =
                      "";
                  BtnAddDiv.Attributes["style"] =
                      "";
              }
              else
              {
              BtnAddSpan.Attributes["style"] =
                  "opacity:1; filter:progid:DXImageTransform.Alpha(opacity=1); filter:alpha(opacity=1);";
              BtnAddDiv.Attributes["style"] =
                  "padding: 0; opacity:1; filter:progid:DXImageTransform.Alpha(opacity=1); filter:alpha(opacity=1);";
           
                  
              }
          }

      }

      protected void btnrefreshTile_Click(object sender, EventArgs e)
      {
          string school = cmbSchool.SelectedValue;
          SchoolID = (string.IsNullOrEmpty(school) || school == "0") ? default(int?) : Convert.ToInt32(school);
          string year = ImpPlanType == ImprovementPlanType.District ? cmbYearDistrict.SelectedValue : cmbYearSchool.SelectedValue;
          if (year == "Year") year = null;
          FilterAndDisplayResult(ImpPlanType, SchoolID, year);
      }

  }
}