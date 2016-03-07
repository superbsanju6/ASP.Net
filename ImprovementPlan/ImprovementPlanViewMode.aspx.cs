using GemBox.Spreadsheet;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Enums;
using Thinkgate.Enums.ImprovementPlan;
using Thinkgate.Classes;
using System.IO;
using System.Text;
using Thinkgate.Services.Contracts.ImprovementPlanService;
using ExtensionHelper = Thinkgate.Services.Contracts.ImprovementPlanService.ExtensionHelper;

namespace Thinkgate.ImprovementPlan
{
    public partial class ImprovementPlanViewMode : BasePage
    {
        #region Properties
        private int ImprovementID { get; set; }
        private string DistrictName { get; set; }
        private string ClientID { get; set; }
        private EventTargets eventTargetEnum;
        private ImprovementPlanOutput improvementPlanOutput;
        #endregion Properties

        #region Private Methods

        private void LoadStrategyData()
        {
            this.improvementPlanOutput = new ImprovementPlanProxy().GetImprovementPlanByPlanID(this.ImprovementID, new List<ImprovementPlanActions> { 
                ImprovementPlanActions.StrategyPlan, ImprovementPlanActions.SmartGoal, ImprovementPlanActions.ImprovementPlan,
            ImprovementPlanActions.ActionStep}, this.ClientID);

            this.rptAllStrategy.DataSource = this.improvementPlanOutput.ImprovementPlanStrategy;
            this.rptAllStrategy.DataBind();
        }

        /// <summary>
        /// Get the client id from the district params
        /// </summary>
        private void GetClientID()
        {
            DistrictParms districtParams = DistrictParms.LoadDistrictParms();
            if (districtParams != default(DistrictParms))
            {
                this.ClientID = districtParams.ClientID;
                this.DistrictName = districtParams.ImprovementPlanDistrictDisplayName;
            }
        }

        /// <summary>
        /// Get the improvement plan id and action type from query string.
        /// If those are not supplied the page will redirect with an error message.
        /// </summary>
        private void GetQueryStringData()
        {
            if (Request.QueryString["impID"] != null)
                this.ImprovementID = Convert.ToInt32(Request.QueryString["impID"]);


            if (Request.QueryString["isPDF"] != null && Request.QueryString["isPDF"] == "Yes")
            {
                imgDelete.Visible = false;
                imgExcel.Visible = false;
                imgPDF.Visible = false;
                impPlanCPCtrl.IsPDF = true;

            }


//#if DEBUG
//            this.ImprovementID = 1;
//#endif
        }

        /// <summary>
        /// Format the Cell
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="workSheet"></param>
        /// <param name="vertical"></param>
        /// <param name="horizontal"></param>
        /// <param name="weight"></param>
        /// <param name="isWrap"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="isTextVertical"></param>
        private void GetFormatedCell(int row, int col, ref ExcelWorksheet workSheet,
                                    VerticalAlignmentStyle vertical = VerticalAlignmentStyle.Center,
                                    HorizontalAlignmentStyle horizontal = HorizontalAlignmentStyle.Center,
                                    int weight = ExcelFont.NormalWeight, bool isWrap = default(bool), Color backgroundColor = default(Color),
                                    bool isTextVertical = default(bool), Color foregroundColor = default(Color), int rotation = default(int))
        {
            workSheet.Cells[row, col].Style.VerticalAlignment = vertical;
            workSheet.Cells[row, col].Style.HorizontalAlignment = horizontal;
            workSheet.Cells[row, col].Style.Font.Weight = weight;
            workSheet.Cells[row, col].Style.IsTextVertical = isTextVertical;
            //if (backgroundColor != default(Color) || foregroundColor != default(Color))
            //    workSheet.Cells[row, col].Style.FillPattern.SetPattern(FillPatternStyle.None, foregroundColor, backgroundColor);
            if (isWrap)
                workSheet.Cells[row, col].Style.WrapText = true;
            else
                workSheet.Columns[col].AutoFit();

            if (rotation != default(int))
                workSheet.Cells[row, col].Style.Rotation = rotation;
        }

        /// <summary>
        /// Create the Main Work Sheet
        /// </summary>
        /// <param name="improvementPlanDetails"></param>
        /// <param name="excelFile"></param>
        private void CreateMainWorkSheet(ImprovementPlanOutput improvementPlanDetails, ref ExcelFile excelFile)
        {
            ExcelWorksheet mainWorkSheet = excelFile.Worksheets.Add("District Goals");
            mainWorkSheet.PrintOptions.PrintGridlines = true;

            if (this.improvementPlanOutput.ImprovementPlanInfo != null)
            {
                mainWorkSheet.Cells[1, 0].Value = string.Format("ANNUAL {0} ACADEMIC IMPROVEMENT PLAN", ((ImprovementPlanType)improvementPlanOutput.ImprovementPlanInfo.ImprovementPlanType).ToString().ToUpper());
                mainWorkSheet.Cells.GetSubrangeAbsolute(1, 0, 2, 8).Merged = true;
                GetFormatedCell(1, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight);
                mainWorkSheet.Rows[1].Height = 1900;
            }

            #region Add Image
            AnchorCell topLeft = new AnchorCell(mainWorkSheet.Columns[0], mainWorkSheet.Rows[1], true);
            mainWorkSheet.Pictures.Add(Server.MapPath("../Images/ClientLogos/GAPauldinglogo.jpg"), PositioningMode.Move, topLeft);
            #endregion


            #region School District Label And Value
            mainWorkSheet.Cells[4, 0].Value = "School District: ";
            GetFormatedCell(4, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

            mainWorkSheet.Cells[4, 1].Value = this.DistrictName;
            mainWorkSheet.Cells.GetSubrangeAbsolute(4, 1, 4, 4).Merged = true;
            mainWorkSheet.Cells[4, 1].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
            GetFormatedCell(4, 1, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
            #endregion School District Label And Value

            #region School Year Label And Value
            mainWorkSheet.Cells[4, 6].Value = "School Year:";
            GetFormatedCell(4, 6, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

            mainWorkSheet.Cells[4, 7].Value = improvementPlanDetails.ImprovementPlanInfo == null ? default(string) : improvementPlanDetails.ImprovementPlanInfo.ImprovementPlanYear;
            mainWorkSheet.Cells.GetSubrangeAbsolute(4, 7, 4, 8).Merged = true;
            mainWorkSheet.Cells[4, 7].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
            GetFormatedCell(4, 7, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
            #endregion School Year Label And Value

            #region Name of Superintendent Label And Value
            mainWorkSheet.Cells[6, 0].Value = "Name of Superintendent: ";
            GetFormatedCell(6, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

            mainWorkSheet.Cells[6, 1].Value = improvementPlanDetails.ImprovementPlanInfo == null ? default(string) : improvementPlanDetails.ImprovementPlanInfo.Superintendent;
            mainWorkSheet.Cells.GetSubrangeAbsolute(6, 1, 6, 4).Merged = true;
            mainWorkSheet.Cells[6, 1].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
            GetFormatedCell(6, 1, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
            #endregion Name of Superintendent Label And Value


            if (this.improvementPlanOutput.ImprovementPlanInfo != null && improvementPlanDetails.ImprovementPlanInfo.PlanType == ImprovementPlanType.School)
            {
                #region Name of School Label And Value
                mainWorkSheet.Cells[8, 0].Value = "Name of School: ";
                GetFormatedCell(8, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

                mainWorkSheet.Cells[8, 1].Value = improvementPlanDetails.ImprovementPlanInfo == null ? default(string) : improvementPlanDetails.ImprovementPlanInfo.SchoolName;
                mainWorkSheet.Cells.GetSubrangeAbsolute(8, 1, 8, 3).Merged = true;
                mainWorkSheet.Cells[8, 1].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
                GetFormatedCell(8, 1, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
                #endregion Name of School Label And Value

                #region Name of Principal Label And Value
                mainWorkSheet.Cells[10, 0].Value = "Name of Principal ";
                GetFormatedCell(10, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

                mainWorkSheet.Cells[10, 1].Value = improvementPlanDetails.ImprovementPlanInfo == null ? default(string) : improvementPlanDetails.ImprovementPlanInfo.Principal;
                mainWorkSheet.Cells.GetSubrangeAbsolute(10, 1, 10, 3).Merged = true;
                mainWorkSheet.Cells[10, 1].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
                GetFormatedCell(10, 1, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
                #endregion Name of Principal Label And Value

                mainWorkSheet.Cells[12, 0].Value = "District Strategic Goals ";
                GetFormatedCell(12, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);
            }
            else
            {
                mainWorkSheet.Cells[8, 0].Value = "District Strategic Goals";
                GetFormatedCell(8, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);
            }


            if (improvementPlanDetails != null)
            {
                if (improvementPlanDetails.ImprovementPlanStrategicGoal != null)
                {
                    int rowCount = default(int);
                    int strategyRow = mainWorkSheet.Rows.Count();
                    ExtensionHelper.ForEach(improvementPlanDetails.ImprovementPlanStrategicGoal, strategy =>
                    {
                        rowCount++;
                        int currentRow = (strategyRow + rowCount);

                        mainWorkSheet.Cells[currentRow, 0].Value = string.Format("District Strategic Goal {0} -", rowCount);
                        GetFormatedCell(currentRow, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Right, weight: ExcelFont.BoldWeight);

                        mainWorkSheet.Cells[currentRow, 1].Value = strategy.StrategicGoal;
                        mainWorkSheet.Cells.GetSubrangeAbsolute(currentRow, 1, currentRow, 8).Merged = true;
                        GetFormatedCell(currentRow, 1, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);

                        if (strategy.StrategicGoal.Length > 83)
                        {
                            mainWorkSheet.Rows[currentRow].Height = 500 * ((strategy.StrategicGoal.Length) / 83);
                        }

                    });
                }
            }

            int footerRow = mainWorkSheet.Rows.Count() + 1;

            if (this.improvementPlanOutput.ImprovementPlanInfo != null && improvementPlanDetails.ImprovementPlanInfo.PlanType == ImprovementPlanType.School)
            {
                mainWorkSheet.Cells[footerRow, 0].Value = "The following document outlines the school's plan for improvement as aligned to the district's goals and priorities";
                mainWorkSheet.Cells.GetSubrangeAbsolute(footerRow, 0, footerRow, 8).Merged = true;
                GetFormatedCell(footerRow, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);
            }


            footerRow = mainWorkSheet.Rows.Count() + 1;

            #region Principal's Signature Label And Value

            mainWorkSheet.Cells[footerRow, 0].Value = (improvementPlanDetails.ImprovementPlanInfo != null && improvementPlanDetails.ImprovementPlanInfo.PlanType == ImprovementPlanType.School) ? "Principal's Signature:" : "Superintendent's Signature:";
            GetFormatedCell(footerRow, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

            mainWorkSheet.Cells[footerRow, 1].Value = "";
            mainWorkSheet.Cells.GetSubrangeAbsolute(footerRow, 1, footerRow, 3).Merged = true;
            mainWorkSheet.Cells[footerRow, 1].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
            GetFormatedCell(footerRow, 1, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
            #endregion Principal's Signature Label And Value

            #region Date Label And Value
            mainWorkSheet.Cells[footerRow, 5].Value = "Date:";
            GetFormatedCell(footerRow, 5, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);


            mainWorkSheet.Cells[footerRow, 6].Value = (improvementPlanDetails.ImprovementPlanInfo == null || improvementPlanDetails.ImprovementPlanInfo.SignedDate == default(DateTime?)) ? default(string) : DataIntegrity.ConvertToDate(improvementPlanDetails.ImprovementPlanInfo.SignedDate).ToShortDateString();
            mainWorkSheet.Cells.GetSubrangeAbsolute(footerRow, 6, footerRow, 7).Merged = true;
            mainWorkSheet.Cells[footerRow, 6].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
            GetFormatedCell(footerRow, 6, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
            #endregion Date Label And Value

            #region Finalize

            footerRow++;
            mainWorkSheet.Cells[footerRow, 0].Value = "Finalize:";
            GetFormatedCell(footerRow, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

            mainWorkSheet.Cells[footerRow, 1].Value = (improvementPlanDetails.ImprovementPlanInfo != null && improvementPlanDetails.ImprovementPlanInfo.IsFinalized) ? "Yes" : "No";
            mainWorkSheet.Cells[footerRow, 1].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
            GetFormatedCell(footerRow, 1, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
            #endregion Finalize

            CreateBorder(ref mainWorkSheet);


            footerRow = mainWorkSheet.Rows.Count() + 3;

            mainWorkSheet.Cells[footerRow, 0].Value = @"The vision of the Paulding County School District is to provide a safe, healthy, supportive environment focused on learning and committed to high academic achievement. Throught the shared reponsiblity of all stakeholders, students will be prepared as lifelong learners and as participating, contributing members of our dynamic and diverse community.";
            mainWorkSheet.Cells.GetSubrangeAbsolute(footerRow, 0, footerRow, 8).Merged = true;
            GetFormatedCell(footerRow, 0, ref mainWorkSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.NormalWeight, isWrap: true);
            mainWorkSheet.Rows[footerRow].Height = 1200;

        }

        /// <summary>
        /// Create border around the data
        /// </summary>
        /// <param name="mainWorkSheet"></param>
        public void CreateBorder(ref ExcelWorksheet mainWorkSheet)
        {
            int lastRow = mainWorkSheet.Rows.Count();
            int lastCol = mainWorkSheet.Columns.Count();

            //Top Margin
            mainWorkSheet.Cells.GetSubrangeAbsolute(0, 0, 0, lastCol + 1).Merged = true;
            mainWorkSheet.Rows[0].Cells[0].SetBorders(MultipleBorders.Top, Color.Black, LineStyle.Medium);
            mainWorkSheet.Cells.GetSubrangeAbsolute(0, 0, 0, lastCol + 1).Merged = false;

            //Bottom Margin
            mainWorkSheet.Cells.GetSubrangeAbsolute(lastRow + 1, 0, lastRow + 1, lastCol + 1).Merged = true;
            mainWorkSheet.Rows[lastRow + 1].Cells[0].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
            mainWorkSheet.Cells.GetSubrangeAbsolute(lastRow + 1, 0, lastRow + 1, lastCol + 1).Merged = false;

            //Right Margin
            mainWorkSheet.Cells.GetSubrangeAbsolute(0, lastCol + 1, lastRow + 1, lastCol + 1).Merged = true;
            mainWorkSheet.Rows[0].Cells[lastCol + 1].SetBorders(MultipleBorders.Right, Color.Black, LineStyle.Medium);
            //mainWorkSheet.Cells.GetSubrangeAbsolute(0, lastCol + 1, lastRow + 1, lastCol + 1).Merged = false;

        }

        /// <summary>
        /// Generate the Strategy work sheets
        /// </summary>
        /// <param name="improvementPlanDetails"></param>
        /// <param name="excelFile"></param>
        public void CreateWorkSheet(ImprovementPlanOutput improvementPlanDetails, ref ExcelFile excelFile)
        {
            int strategyCount = default(int);


            if (improvementPlanDetails.ImprovementPlanStrategy != null)
            {
                foreach (ImprovementPlanStrategy planStrategy in improvementPlanDetails.ImprovementPlanStrategy)
                {
                    strategyCount++;

                    ExcelWorksheet workSheet = excelFile.Worksheets.Add(string.Format("Strategy {0}", strategyCount));
                    workSheet.PrintOptions.PrintGridlines = true;

                    if (this.improvementPlanOutput.ImprovementPlanInfo != null)
                    {
                        workSheet.Cells[1, 0].Value = string.Format("Annual {0} Academic Improvement Plan", improvementPlanOutput.ImprovementPlanInfo.PlanType.ToString());
                        workSheet.Cells.GetSubrangeAbsolute(1, 0, 2, 9).Merged = true;
                        GetFormatedCell(1, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight);
                    }

                    #region School District Label And Value
                    workSheet.Cells[4, 0].Value = "School District: ";
                    GetFormatedCell(4, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

                    workSheet.Cells[4, 1].Value = this.DistrictName;
                    workSheet.Cells.GetSubrangeAbsolute(4, 1, 4, 3).Merged = true;
                    workSheet.Cells[4, 1].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
                    GetFormatedCell(4, 1, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
                    #endregion School District Label And Value

                    #region School Year Label And Value
                    workSheet.Cells[4, 7].Value = "School Year:";
                    GetFormatedCell(4, 7, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

                    workSheet.Cells[4, 8].Value = improvementPlanDetails.ImprovementPlanInfo == null ? default(string) : improvementPlanDetails.ImprovementPlanInfo.ImprovementPlanYear;
                    workSheet.Cells.GetSubrangeAbsolute(4, 8, 4, 9).Merged = true;
                    workSheet.Cells[4, 8].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
                    GetFormatedCell(4, 8, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
                    #endregion School Year Label And Value

                    #region School Label And Value
                    if (improvementPlanOutput.ImprovementPlanInfo.PlanType == ImprovementPlanType.School)
                    {
                        workSheet.Cells[6, 0].Value = "School: ";
                        GetFormatedCell(6, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

                        workSheet.Cells[6, 1].Value = improvementPlanDetails.ImprovementPlanInfo == null ? default(string) : improvementPlanDetails.ImprovementPlanInfo.SchoolName;
                        workSheet.Cells.GetSubrangeAbsolute(6, 1, 6, 3).Merged = true;
                        workSheet.Cells[6, 1].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
                        GetFormatedCell(6, 1, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
                    }
                    #endregion School Label And Value

                    #region Strategy Name
                    workSheet.Cells[8, 0].Value = "Strategy: ";
                    workSheet.Cells.GetSubrangeAbsolute(8, 0, 8, 8).Merged = true;
                    GetFormatedCell(8, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight, backgroundColor: Color.Maroon, foregroundColor: Color.White);

                    workSheet.Cells[9, 0].Value = planStrategy.StrategyName;
                    workSheet.Cells.GetSubrangeAbsolute(9, 0, 9, 8).Merged = true;
                    GetFormatedCell(9, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, backgroundColor: Color.Maroon, foregroundColor: Color.White);
                    #endregion Strategy Name

                    #region Person Responsible
                    workSheet.Cells[8, 9].Value = "Person(s) Responsible: ";
                    GetFormatedCell(8, 9, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

                    workSheet.Cells[9, 9].Value = planStrategy.PersonResponsible;
                    GetFormatedCell(9, 9, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);
                    #endregion Person Responsible

                    #region Smart Goal Label
                    workSheet.Cells[11, 0].Value = "SMART Goals: ";
                    GetFormatedCell(11, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);
                    workSheet.Cells[11, 0].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    workSheet.Cells.GetSubrangeAbsolute(11, 1, 11, 8).Merged = true;
                    #endregion Smart Goal Label

                    #region Smart Goals
                    int currentRow = workSheet.Rows.Count() - 1;

                    if (improvementPlanDetails.ImprovementPlanSmartGoal != null && improvementPlanDetails.ImprovementPlanSmartGoal.Any(any => any.StrategyID == planStrategy.ID))
                    {
                        int smartGoalCount = 1;
                        foreach (ImprovementPlanSmartGoal smartGoal in improvementPlanDetails.ImprovementPlanSmartGoal.Where(filter => filter.StrategyID == planStrategy.ID))
                        {
                            currentRow++;
                            workSheet.Cells[currentRow, 0].Value = string.Format("SMART Goal {0}-", smartGoalCount);
                            GetFormatedCell(currentRow, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Right, weight: ExcelFont.BoldWeight);

                            workSheet.Cells[currentRow, 1].Value = smartGoal.SmartGoal;
                            GetFormatedCell(currentRow, 1, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);                            
                            workSheet.Cells.GetSubrangeAbsolute(currentRow, 1, currentRow, 9).Merged = true;
                            
                            if (smartGoal.SmartGoal.Length > 109)
                            {
                                workSheet.Rows[currentRow].Height = 500 * (smartGoal.SmartGoal.Length / 109);
                            }
                            smartGoalCount++;
                        }
                    }
                    #endregion Smart Goals

                    #region Action Step Header

                    currentRow = workSheet.Rows.Count() + 1;

                    workSheet.Rows[currentRow].Height = 1450;

                    GetFormatedCell(currentRow, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight, isWrap: true, backgroundColor: Color.LightGray, rotation: 90);
                    workSheet.Cells[currentRow, 0].Value = "District Strategic Goal";
                    workSheet.Cells[currentRow, 0].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    workSheet.Cells[currentRow, 1].Value = "Action Steps";
                    workSheet.Cells.GetSubrangeAbsolute(currentRow, 1, currentRow, 3).Merged = true;
                    GetFormatedCell(currentRow, 1, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight, isWrap: true, backgroundColor: Color.LightGray);
                    workSheet.Cells[currentRow, 1].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    GetFormatedCell(currentRow, 4, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight, isWrap: true, backgroundColor: Color.LightGray, rotation: 90);
                    workSheet.Cells[currentRow, 4].Value = "Person(s) Responsible";
                    workSheet.Cells[currentRow, 4].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    GetFormatedCell(currentRow, 5, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight, isWrap: true, backgroundColor: Color.LightGray, rotation: 90);
                    workSheet.Cells[currentRow, 5].Value = "Start Date";
                    workSheet.Cells[currentRow, 5].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    GetFormatedCell(currentRow, 6, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight, isWrap: true, backgroundColor: Color.LightGray, rotation: 90);
                    workSheet.Cells[currentRow, 6].Value = "Finish Date";
                    workSheet.Cells[currentRow, 6].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    GetFormatedCell(currentRow, 7, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight, isWrap: true, backgroundColor: Color.LightGray, rotation: 90);
                    workSheet.Cells[currentRow, 7].Value = "Status (See Key Below)";
                    workSheet.Cells[currentRow, 7].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    GetFormatedCell(currentRow, 8, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight, isWrap: true, backgroundColor: Color.LightGray, rotation: 90);
                    workSheet.Cells[currentRow, 8].Value = "Resources/Cost";
                    workSheet.Cells[currentRow, 8].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    workSheet.Cells[currentRow, 9].Value = "Expected Results";
                    GetFormatedCell(currentRow, 9, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.BoldWeight, isWrap: true, backgroundColor: Color.LightGray);
                    workSheet.Cells[currentRow, 9].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                    #endregion Action Step Header

                    #region Action Steps
                    currentRow = workSheet.Rows.Count() - 1;

                    if (improvementPlanDetails.ImprovementPlanActionStep != null && improvementPlanDetails.ImprovementPlanActionStep.Any(any => any.StrategyID == planStrategy.ID))
                    {
                        foreach (ImprovementPlanActionStep actionStep in improvementPlanDetails.ImprovementPlanActionStep.Where(filter => filter.StrategyID == planStrategy.ID))
                        {
                            currentRow++;
                            string strategyGoal = default(string);

                            if (improvementPlanDetails.ImprovementPlanStrategicGoal != null && improvementPlanDetails.ImprovementPlanStrategicGoal.Any(any => any.ID == actionStep.StrategicGoalID))
                            {
                                strategyGoal = improvementPlanDetails.ImprovementPlanStrategicGoal.FirstOrDefault(find => find.ID == actionStep.StrategicGoalID).StrategicGoal;
                            }

                            string strategyName = default(string);

                            if (actionStep.StrategicGoalID != default(int?) && improvementPlanDetails.ImprovementPlanStrategicGoal != null &&
                                improvementPlanDetails.ImprovementPlanStrategicGoal.Any(any => any.ID == actionStep.StrategicGoalID))
                            {
                                ImprovementPlanStrategicGoal strategyGoalDetails = improvementPlanDetails.ImprovementPlanStrategicGoal.FirstOrDefault(first => first.ID == actionStep.StrategicGoalID);
                                strategyName = string.Format("Strategic Goal {0}-", improvementPlanDetails.ImprovementPlanStrategicGoal.ToList().IndexOf(strategyGoalDetails) + 1);
                            }

                            workSheet.Cells[currentRow, 0].Value = strategyName;
                            GetFormatedCell(currentRow, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);
                            workSheet.Cells[currentRow, 0].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                            workSheet.Cells[currentRow, 1].Value = actionStep.ActionStep;
                            workSheet.Cells.GetSubrangeAbsolute(currentRow, 1, currentRow, 3).Merged = true;
                            GetFormatedCell(currentRow, 1, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);
                            workSheet.Cells[currentRow, 1].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                            if (actionStep.ActionStep.Length > 34)
                            {
                                workSheet.Rows[currentRow].Height = 500 * (actionStep.ActionStep.Length / 34);
                            }


                            workSheet.Cells[currentRow, 4].Value = actionStep.PersonResponsible;
                            GetFormatedCell(currentRow, 4, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);
                            workSheet.Cells[currentRow, 4].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                            workSheet.Cells[currentRow, 5].Value = actionStep.StartDate != default(DateTime?) ? Convert.ToDateTime(actionStep.StartDate).ToShortDateString() : default(string);
                            GetFormatedCell(currentRow, 5, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
                            workSheet.Cells[currentRow, 5].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);


                            workSheet.Cells[currentRow, 6].Value = actionStep.FinishDate != default(DateTime?) ? Convert.ToDateTime(actionStep.FinishDate).ToShortDateString() : default(string);
                            GetFormatedCell(currentRow, 6, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight);
                            workSheet.Cells[currentRow, 6].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                            workSheet.Cells[currentRow, 7].Value = actionStep.StatusDescription;
                            GetFormatedCell(currentRow, 7, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);
                            workSheet.Cells[currentRow, 7].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                            workSheet.Cells[currentRow, 8].Value = actionStep.ResourceCosts;
                            GetFormatedCell(currentRow, 8, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);
                            workSheet.Cells[currentRow, 8].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);

                            workSheet.Cells[currentRow, 9].Value = actionStep.ExpectedResults;
                            GetFormatedCell(currentRow, 9, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.NormalWeight, isWrap: true);
                            workSheet.Cells[currentRow, 9].SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Medium);
                        }
                    }
                    #endregion Action Steps


                    #region Strategy Footer

                    currentRow = workSheet.Rows.Count() + 1;

                    workSheet.Cells[currentRow, 0].Value = " The vision  of the Paulding County School District is to provide a safe, healthy, supportive environment focused on learning and committed to high academic achievement. Through the shared responsibility of all stakeholders, students will be prepared as lifelong learners and as participating, contributing members of our dynamic and diverse community.";
                    workSheet.Cells.GetSubrangeAbsolute(currentRow, 0, currentRow, 9).Merged = true;
                    GetFormatedCell(currentRow, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Center, weight: ExcelFont.NormalWeight, isWrap: true);
                    workSheet.Rows[currentRow].Height = 1200;

                    currentRow = workSheet.Rows.Count() + 1;
                    workSheet.Cells[currentRow, 0].Value = "Status Key:";
                    workSheet.Cells.GetSubrangeAbsolute(currentRow, 0, currentRow, 9).Merged = true;
                    foreach (ImprovementPlanStatusKey statusKey in improvementPlanDetails.ImprovementPlanStatusKey)
                    {
                        workSheet.Cells[currentRow, 0].Value = string.Concat(workSheet.Cells[currentRow, 0].Value, string.Format(" {0}={1} ", statusKey.StatusKey, statusKey.Description));
                    }
                    GetFormatedCell(currentRow, 0, ref workSheet, horizontal: HorizontalAlignmentStyle.Left, weight: ExcelFont.BoldWeight);

                    #endregion Strategy Footer

                    int lastRow = workSheet.Rows.Count();
                    int lastCol = workSheet.Columns.Count();

                    //Top Margin
                    workSheet.Cells.GetSubrangeAbsolute(0, 0, 0, lastCol).Merged = true;
                    workSheet.Rows[0].Cells[0].SetBorders(MultipleBorders.Top, Color.Black, LineStyle.Medium);
                    workSheet.Cells.GetSubrangeAbsolute(0, 0, 0, lastCol).Merged = false;

                    //Bottom Margin
                    workSheet.Cells.GetSubrangeAbsolute(lastRow + 1, 0, lastRow + 1, lastCol).Merged = true;
                    workSheet.Rows[lastRow + 1].Cells[0].SetBorders(MultipleBorders.Bottom, Color.Black, LineStyle.Medium);
                    workSheet.Cells.GetSubrangeAbsolute(lastRow + 1, 0, lastRow + 1, lastCol).Merged = false;

                    //Right Margin
                    workSheet.Cells.GetSubrangeAbsolute(0, lastCol, lastRow + 1, lastCol).Merged = true;
                    workSheet.Rows[0].Cells[lastCol].SetBorders(MultipleBorders.Right, Color.Black, LineStyle.Medium);
                    //mainWorkSheet.Cells.GetSubrangeAbsolute(0, lastCol + 1, lastRow + 1, lastCol + 1).Merged = false;


                }
            }
        }

        /// <summary>
        /// Export the data to excel
        /// </summary>
        private void ExportToExcel()
        {
            ImprovementPlanOutput improvementPlanDetails = default(ImprovementPlanOutput);

            improvementPlanDetails = new ImprovementPlanProxy().GetImprovementPlanByPlanID(this.ImprovementID, new List<ImprovementPlanActions> 
                                                                                            { ImprovementPlanActions.StrategicGoal, 
                                                                                              ImprovementPlanActions.ActionStep, 
                                                                                              ImprovementPlanActions.SmartGoal, 
                                                                                              ImprovementPlanActions.ImprovementPlan, 
                                                                                              ImprovementPlanActions.Status,
                                                                                            ImprovementPlanActions.StrategyPlan},
                                                                                              this.ClientID);

            ExcelFile excelFile = new ExcelFile();
            excelFile.DefaultFontName = "Calibri";

            LoadStrategyData();

            //Create the cover page work sheet
            CreateMainWorkSheet(improvementPlanDetails, ref excelFile);

            //Create the template work sheets
            CreateWorkSheet(improvementPlanDetails, ref excelFile);

            excelFile.Save(Response, GetExcelFileName(improvementPlanDetails.ImprovementPlanInfo.PlanType,
                                                      improvementPlanDetails.ImprovementPlanInfo.SchoolName,
                                                      improvementPlanDetails.ImprovementPlanInfo.ImprovementPlanYear));
        }

        /// <summary>
        /// Get the excel file name based on plantype, schoolname and year
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="schoolName"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private string GetExcelFileName(ImprovementPlanType planType, string schoolName, string year)
        {
            StringBuilder excelFileName = new StringBuilder();

            //Add year to the file name
            if (!string.IsNullOrEmpty(year))
            {
                excelFileName.Append(year);
                excelFileName.Append("-");
            }

            //Add district or school plan type to file name
            if (planType != ImprovementPlanType.None)
                excelFileName.Append(planType.ToString());

            //Add improvement plan to file name
            excelFileName.Append("ImprovementPlan");

            //Add school name to file name
            if (planType == ImprovementPlanType.School)
                excelFileName.Append(schoolName);

            //Add extension
            excelFileName.Append(".xlsx");

            return excelFileName.ToString();
        }

        private void ExportToPDF()
        {
            Response.Redirect("../ImprovementPlan/ImprovementPlanPDFView.aspx?impID=" + ImprovementID);
        }

        private void GetData()
        {
            ImprovementPlanOutput improvementPlanDetails = default(ImprovementPlanOutput);

            improvementPlanDetails = new ImprovementPlanProxy().GetImprovementPlanByPlanID(this.ImprovementID,
                                                                                            new List<ImprovementPlanActions> 
                                                                                            {                                                                                              
                                                                                              ImprovementPlanActions.ImprovementPlan
                                                                                            }, this.ClientID);
        }

        private void BindCoverPage()
        {

        }

        /// <summary>
        /// Delete the improvement plan
        /// </summary>
        private void DeleteImprovement()
        {
            new ImprovementPlanProxy().DeleteImprovementPlan(this.ImprovementID, this.ClientID);
            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "Alert",
                        "refreshOpenerPage()", true);
        }

        private void SetIconVisibility()
        {
            if (improvementPlanOutput != null && improvementPlanOutput.ImprovementPlanInfo != null)
                if (improvementPlanOutput.ImprovementPlanInfo.ImprovementPlanType != null)
                    imgDelete.Visible = (ImprovementPlanType)improvementPlanOutput.ImprovementPlanInfo.ImprovementPlanType == ImprovementPlanType.District ? UserHasPermission(Permission.Icon_Delete_DistrictImprovementPlan)
                        : (ImprovementPlanType)improvementPlanOutput.ImprovementPlanInfo.ImprovementPlanType == ImprovementPlanType.School ? UserHasPermission(Permission.Icon_Delete_SchoolImprovementPlan) : true;

            if (Request.QueryString["isPDF"] != null && Request.QueryString["isPDF"] == "Yes")
            {
                imgDelete.Visible = false;
                imgExcel.Visible = false;
                imgPDF.Visible = false;
            }
        }

        /// <summary>
        /// Raise the event based on the autopost back
        /// </summary>
        /// <param name="eventTargets"></param>
        private void RaiseEventByTarget(string eventTargets)
        {
            Enum.TryParse(eventTargets, true, out eventTargetEnum);

            switch (eventTargetEnum)
            {
                case EventTargets.btnDelete:
                    DeleteImprovement();
                    break;

                case EventTargets.btnExcel:
                    ExportToExcel();
                    break;

                case EventTargets.btnPDF:
                    ExportToPDF();
                    break;

            }
        }
        #endregion Private Methods

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            GetClientID();
            GetQueryStringData();

            if (!Page.IsPostBack)
            {
                GetData();
                BindCoverPage();

                LoadStrategyData();
                SetIconVisibility();

            }

            if (Request.Form["__EVENTTARGET"] != null && !string.IsNullOrEmpty(Request.Form["__EVENTTARGET"].ToString()))
            {
                RaiseEventByTarget(Request.Form["__EVENTTARGET"].ToString());
            }
        }

        protected void rptAllStrategy_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                ImprovementPlanStrategy strategy = e.Item.DataItem as ImprovementPlanStrategy;

                if (strategy != null)
                {

                    if (this.improvementPlanOutput.ImprovementPlanInfo != null)
                    {
                        Label districtLblCtrl = e.Item.Controls[1].FindControl("lblDistrict") as Label;
                        Label schoolLblCtrls = e.Item.Controls[1].FindControl("lblSchool") as Label;
                        Label schoolLblCtrl = e.Item.Controls[1].FindControl("lblSchoolVal") as Label;
                        Label yearLblCtrl = e.Item.Controls[1].FindControl("lblSchoolYear") as Label;
                        Label pageTitleLblCtrl = e.Item.Controls[1].FindControl("lblPageTitle") as Label;

                        if (districtLblCtrl != null)
                            districtLblCtrl.Text = this.DistrictName;

                        if (schoolLblCtrl != null)
                            schoolLblCtrl.Text = this.improvementPlanOutput.ImprovementPlanInfo.SchoolName;

                        if (yearLblCtrl != null)
                            yearLblCtrl.Text = this.improvementPlanOutput.ImprovementPlanInfo.ImprovementPlanYear;

                        if (pageTitleLblCtrl != null)
                            pageTitleLblCtrl.Text = string.Format("ANNUAL {0} ACADEMIC IMPROVEMENT PLAN", ((ImprovementPlanType)improvementPlanOutput.ImprovementPlanInfo.ImprovementPlanType).ToString().ToUpper());

                        if (this.improvementPlanOutput.ImprovementPlanInfo.PlanType == ImprovementPlanType.District)
                        {
                            schoolLblCtrls.Visible = false;
                            schoolLblCtrl.Visible = false;
                        }
                    }


                    TextBox strategyCtrl = e.Item.Controls[1].FindControl("txtStrategy") as TextBox;
                    TextBox personResponsibleCtrl = e.Item.Controls[1].FindControl("txtPersonResponsibles") as TextBox;

                    Repeater rptSmartGoal = e.Item.Controls[1].FindControl("rptSmartGoal") as Repeater;

                    DataGrid rptActions = e.Item.Controls[1].FindControl("dgActions") as DataGrid;

                    if (strategyCtrl != null)
                    {
                        strategyCtrl.Text = strategy.StrategyName;
                    }

                    if (personResponsibleCtrl != null)
                    {
                        personResponsibleCtrl.Text = strategy.PersonResponsible;
                    }

                    if (rptSmartGoal != null)
                    {
                        rptSmartGoal.DataSource = this.improvementPlanOutput.ImprovementPlanSmartGoal.Where(filter => filter.StrategyID == strategy.ID).ToList();
                        rptSmartGoal.DataBind();
                    }

                    if (rptActions != null)
                    {
                        IEnumerable<ImprovementPlanActionStep> actionStep = this.improvementPlanOutput.ImprovementPlanActionStep.Where(filter => filter.StrategyID == strategy.ID).ToList();

                        rptActions.DataSource = (actionStep != null && actionStep.Count() > 0) ? actionStep : new List<ImprovementPlanActionStep> { new ImprovementPlanActionStep { } };
                        rptActions.DataBind();
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        #endregion Events
    }
}