using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using Thinkgate.Domain.Classes;
using Thinkgate.Domain;


namespace Thinkgate.Controls.Reports
{
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    using Thinkgate.Base.Classes;

    public partial class ReportCriteria : System.Web.UI.UserControl
    {
        public event EventHandler ReloadReport;

        #region Properties

        public Criteria Criteria
        {
            get { return (Criteria)Session["Criteria_" + Guid]; }
            set { Session["Criteria_" + Guid] = value; }
        }

        public SessionObject SessionObject
        {
            get { return (SessionObject)Session["SessionObject"]; }
            set { Session["SessionObject"] = value; }
        }

        public string Guid { get; set; }

        public bool FirstTimeLoaded { get; set; }

        public string InitialButtonText { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Guid))
            {
                return;
            }

            if (FirstTimeLoaded)
            {
                LoadExtraCriterion();
                if (!string.IsNullOrEmpty(InitialButtonText))
                {
                    btnUpdateCriteria.Text = InitialButtonText;
                    hiddenGradeListSelected.Text = String.Empty;
                    hiddenSubjectListSelected.Text = String.Empty;
                    hiddenFieldsSelected.Text = String.Empty;
                    hiddenSchoolTypeListSelected.Text = String.Empty;
                }
            }
            else
            {
                btnUpdateCriteria.Text = "Update Results";
            }

            WriteSearchParms();
        }

        public void SetAllCriterionVisible(bool isVisible = true)
        {
            foreach (var c in Criteria.CriterionList)
                c.Visible = isVisible;
        }

        public void HideCriterionSection(string headerText)
        {
            var criterion = Criteria.CriterionList.FindLast(r => r.IsHeader && r.Header == headerText);
            if (criterion == null) return;
            criterion.Visible = false;
        }

        private void KillReport(string message)
        {
            SessionObject.RedirectMessage = message;
            Response.Redirect("~/PortalSelection.aspx", true);
        }

        private HtmlGenericControl CreateHeaderDiv(Criterion criterion)
        {
            var containerDiv = new HtmlGenericControl("div");

            var headerDiv = new HtmlGenericControl("div");
            headerDiv.Attributes["class"] = "criteriaHeaderDiv";

            var headerDivLbl = new HtmlGenericControl("div");
            headerDivLbl.Attributes["class"] = "left";

            var headerDivExpand = new HtmlGenericControl("div");
            headerDivExpand.Attributes["class"] = "right";

            var adjustedID = StripString(criterion.Key);

            headerDivExpand.ID = "expand_" + adjustedID;
            headerDivExpand.Style.Add("overflow", "hidden");

            var requiredAsterik = "";

            // Add tooltip
            if (!criterion.Locked && (criterion.UIType != UIType.None))
            {
                var tooltip = new RadToolTip
                {
                    Height = 55,
                    Width = 205,
                    TargetControlID = headerDivExpand.ID,
                    Position = ToolTipPosition.MiddleRight,
                    RelativeTo = ToolTipRelativeDisplay.Element,
                    HideEvent = ToolTipHideEvent.Default,
                    AutoCloseDelay = 20000,
                    Skin = "Black",
                    ShowEvent = ToolTipShowEvent.OnClick,
                    EnableShadow = true
                };

                var contentChunk = new HtmlGenericControl("div");
                contentChunk.Style.Add("position", "relative");

                if (criterion.IsRequired) //BJC - 6/11/2012: If this criterion object is required
                {
                    requiredFields.Value += criterion.Key + ","; //Add Key to the requiredFields hidden input value.
                    requiredAsterik = "<span style=\"font-weight:bold;color:#F00;\">*</span>";
                }

                // Add appropriate control to tooltip
                switch (criterion.UIType)
                {
                    case UIType.DropDownList:
                        var cmb = CreateDropDownList(criterion, adjustedID);

                        if (criterion.Object != null && criterion.ReportStringVal != null)
                        {
                            var selectedItemIndex = cmb.FindItemIndexByValue(adjustedID + "_" + criterion.ReportStringVal, true);
                            if (selectedItemIndex > 0) cmb.SelectedIndex = selectedItemIndex;
                            if (requiredFieldsSelected.Value.IndexOf(criterion.Key) == -1)
                            {
                                requiredFieldsSelected.Value += criterion.Key + ",";
                            }
                        }

                        contentChunk.Controls.Add(new HtmlGenericControl("div") { InnerText = criterion.Header });
                        contentChunk.Controls.Add(cmb);

                        tooltip.Attributes["dropDownListID"] = cmb.ID;
                        tooltip.OnClientShow = "onClientShowToolTipDropDownList";

                        break;

                    //case UIType.RadioButton:
                    //    {
                    //        var radioButtons = new RadButton()
                    //        {
                    //            ID = "RadRadioButtonCriteriaRadioButtonList",
                    //            GroupName = criterion.Key,
                    //            ToggleType = ButtonToggleType.Radio,
                    //            //AutoPostBack = false,
                    //            //OnClientToggleStateChanged = "OnClientToggleStateChanged"

                    //        };

                    //        if (criterion.DataSource == null)
                    //        {
                    //            radioButtons.ToggleStates.Add(new RadButtonToggleState("No data supplied", string.Format(adjustedID + "_" + "0")));
                    //        }
                    //        else
                    //        {
                    //            radioButtons.ToggleStates.Add(new RadButtonToggleState("All", string.Format(adjustedID + "_" + "0")));


                    //            foreach (DataRow row in ((DataTable)criterion.DataSource).Rows)
                    //            {
                    //                var rbItem = new RadButtonToggleState(row[criterion.DataTextField].ToString(),
                    //                                                      adjustedID + "_" +
                    //                                                      row[criterion.DataValueField]);

                    //                radioButtons.ToggleStates.Add(rbItem);
                    //            }
                    //        }

                    //        contentChunk.Controls.Add(new System.Web.UI.HtmlControls.HtmlGenericControl("div") { InnerText = criterion.Header });
                    //        contentChunk.Controls.Add(radioButtons);


                    //        break;
                    //    }

                    case UIType.CheckBoxList:
                        {
                            tooltip.Width = (criterion.ChildDataSource != null) ? 450 : 205;

                            var listBox = this.CreateCheckBoxList(criterion, adjustedID);
                            tooltip.Attributes.Add("lstBoxID", listBox.ClientID);

                            tooltip.OnClientShow = "setListBoxMaxHeight";

                            if (criterion.Object != null && criterion.ReportStringVal != null)
                            {
                                if (requiredFieldsSelected.Value.IndexOf(criterion.Key) == -1)
                                {
                                    requiredFieldsSelected.Value += criterion.Key + ",";
                                }
                            }

                            contentChunk.Controls.Add(new HtmlGenericControl("div") { InnerText = criterion.Header });
                            contentChunk.Controls.Add(listBox);

                            if (!string.IsNullOrEmpty(criterion.ChildHeader))
                            {
                                tooltip.OnClientShow = "TooltipOnClientShow_DisplayChildCheckboxItems";
                                tooltip.Attributes["ParentListID"] = "RadCombobBoxCriteriaCheckBoxList";
                                tooltip.Attributes["ChildListID"] = "RadCombobBoxCriteriaCheckBoxList2";

                                listBox.Attributes.Add("ChildCheckBoxList", "RadCombobBoxCriteriaCheckBoxList2");

                                var listBox2 = new RadListBox
                                {
                                    ID = "RadCombobBoxCriteriaCheckBoxList2",
                                    AutoPostBack = false,
                                    CheckBoxes = true,
                                    Skin = "Vista",
                                    OnClientItemChecked = "onItemChecked"
                                };

                                foreach (DataRow row in ((DataTable)criterion.ChildDataSource).Rows)
                                {
                                    var listBoxValue = string.Format("{0}_{1}", adjustedID, StripString(row[criterion.ChildDataValueField].ToString()));
                                    var listBoxItem = new RadListBoxItem(row[criterion.ChildDataTextField].ToString(), listBoxValue);
                                    listBoxItem.Attributes["parentValue"] = string.Format("{0}_{1}", adjustedID, StripString(row[criterion.ChildDataParentField].ToString()));
                                    listBoxItem.Attributes["checkBoxID"] = string.Format("{0}_RadCombobBoxCriteriaCheckBoxList2_CheckBox", listBoxValue);

                                    Criterion tempCriterion = null;
                                    foreach (Criterion c in Criteria.CriterionList)
                                    {
                                        if (c.Key == listBoxValue)
                                        {
                                            tempCriterion = c;
                                            break;
                                        }
                                    }

                                    if (tempCriterion != null)
                                    {
                                        listBoxItem.Checked = !tempCriterion.Empty;
                                    }

                                    listBox2.Items.Add(listBoxItem);
                                }

                                contentChunk.Controls.Add(new HtmlGenericControl("span") { InnerText = criterion.ChildHeader });
                                contentChunk.Controls.Add(listBox2);
                            }

                            break;
                        }

                    case UIType.TextBox:
                        {
                            var textBox = CreateTextBox(criterion, adjustedID);

                            if (criterion.Object != null && criterion.ReportStringVal != null)
                            {
                                if (requiredFieldsSelected.Value.IndexOf(criterion.Key) == -1)
                                {
                                    requiredFieldsSelected.Value += criterion.Key + ",";
                                }
                            }

                            contentChunk.Controls.Add(new HtmlGenericControl("div") { InnerText = criterion.Header });
                            contentChunk.Controls.Add(textBox);

                            tooltip.OnClientShow = "onClientShowToolTipTextBox";
                            tooltip.Attributes["textBoxID"] = textBox.ID;

                            break;
                        }

                    case UIType.DatePicker:
                        {
                            tooltip.Width = 330;

                            var startCriterion = Criteria.CriterionList.Find(c => c.Header == criterion.Header && c.IsHeader == false && c.Key.Contains("Start"));
                            var endCriterion = Criteria.CriterionList.Find(c => c.Header == criterion.Header && c.IsHeader == false && c.Key.Contains("End"));

                            var wrapperDiv = CreateDatePicker(adjustedID, startCriterion, endCriterion);

                            if (criterion.Object != null && criterion.ReportStringVal != null)
                            {
                                if (requiredFieldsSelected.Value.IndexOf(criterion.Key) == -1)
                                {
                                    requiredFieldsSelected.Value += criterion.Key + ",";
                                }
                            }

                            contentChunk.Controls.Add(new HtmlGenericControl("div") { InnerText = criterion.Header });
                            contentChunk.Controls.Add(wrapperDiv);

                            break;
                        }

                    case UIType.AssessmentTextSearch:
                        {
                            tooltip.Width = 400;

                            var wrapperDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                            wrapperDiv.Style.Add("width", "350px");

                            var textBox = new RadTextBox
                            {
                                ID = "RadTextBoxAssessmentTextSearch",
                                AutoPostBack = false,
                                Skin = "Vista"
                            };

                            textBox.ClientEvents.OnBlur = "onInputBlur";
                            textBox.Attributes["updateMessageHeader"] = adjustedID;
                            textBox.Attributes["comboBoxDivID"] = "cmbBoxDiv";

                            //ADD DIV TO contentChunk
                            var textBoxDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div") { ID = "textBoxDiv" };
                            textBoxDiv.Controls.Add(textBox);


                            var textSearchCmb = new Telerik.Web.UI.RadComboBox
                            {
                                ID = "RadComboBoxAssessmentTextSearch",
                                AutoPostBack = false,
                                MarkFirstMatch = true,
                                AllowCustomText = false,
                                ZIndex = 8005,
                                OnClientSelectedIndexChanged = "onSelectedIndexChanged",
                                Skin = "Vista"
                            };
                            textSearchCmb.Attributes["textBoxDivID"] = "textBoxDiv";


                            if (criterion.Object != null)
                            {
                                var textSearchObjectArray = criterion.Object.ToString().Split(':');
                                textBox.Text = textSearchObjectArray[1].Trim();

                                var selectedItemIndex = textSearchCmb.FindItemIndexByText(textSearchObjectArray[0].Trim(), true);

                                textSearchCmb.SelectedIndex = selectedItemIndex;

                                if (requiredFieldsSelected.Value.IndexOf(criterion.Key) == -1)
                                {
                                    requiredFieldsSelected.Value += criterion.Key + ",";
                                }
                            }

                            var textSearchCmbDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div") { ID = "cmbBoxDiv" };
                            textSearchCmbDiv.Controls.Add(textSearchCmb);

                            if (criterion.DataSource == null)
                            {
                                textSearchCmb.Items.Add(new RadComboBoxItem("No data supplied", adjustedID + "_" + "0"));
                            }
                            else
                            {
                                foreach (DataRow row in ((DataTable)criterion.DataSource).Rows)
                                {
                                    textSearchCmb.Items.Add(new RadComboBoxItem(row[criterion.DataTextField].ToString(), adjustedID + "_" + row[criterion.DataValueField]));
                                }
                            }

                            contentChunk.Controls.Add(new System.Web.UI.HtmlControls.HtmlGenericControl("div") { InnerText = criterion.Header });

                            var wrapperDivLeft = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                            wrapperDivLeft.Style.Add("float", "left");
                            wrapperDivLeft.Style.Add("width", "149px");
                            wrapperDivLeft.Controls.Add(textBoxDiv);

                            var wrapperDivRight = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                            wrapperDivRight.Style.Add("float", "right");
                            wrapperDivRight.Style.Add("width", "149px");
                            wrapperDivRight.Controls.Add(textSearchCmbDiv);

                            wrapperDiv.Controls.Add(wrapperDivLeft);
                            wrapperDiv.Controls.Add(wrapperDivRight);

                            contentChunk.Controls.Add(wrapperDiv);

                            break;
                        }

                    case UIType.Demographics:
                        {
                            tooltip.Width = 390;

                            contentChunk.Controls.Add(new HtmlGenericControl("div") { InnerText = criterion.Header });
                            contentChunk.Controls.Add(CreateDemographics(criterion, adjustedID));

                            break;
                        }

                    case UIType.RTI:
                        {
                            contentChunk.Controls.Add(new HtmlGenericControl("div") { InnerText = criterion.Header });
                            contentChunk.Controls.Add(CreateRTI(criterion, adjustedID));

                            break;
                        }
                }

                tooltip.Controls.Add(contentChunk);
                headerDiv.Controls.Add(tooltip);
            }

            if (criterion.Locked == false)
            {
                headerDivExpand.Controls.Add(new Image { ImageUrl = "~/Images/commands/expand_bubble.png", Width = 16, Height = 16 });
            }
            else if (FirstTimeLoaded)
            {
                criterion.ReportStringVal = criterion.DefaultValue;
            }

            headerDivLbl.InnerHtml = criterion.Header + ":" + requiredAsterik;

            headerDiv.Controls.Add(headerDivLbl);
            headerDiv.Controls.Add(headerDivExpand);

            var updateMessageDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            updateMessageDiv.Attributes["class"] = "criteriaUpdateMessageDiv";
            updateMessageDiv.Attributes["id"] = adjustedID + "_updateMessage";

            containerDiv.Controls.Add(headerDiv);
            containerDiv.Controls.Add(updateMessageDiv);

            return containerDiv;
        }

        private string DependencyString(Criterion criterion)
        {
            var dependencies = new List<string>();

            foreach (var dependency in criterion.Dependencies)
            {


                var tempCriterion = Criteria.CriterionList.Find(c => c.IsHeader && c.Key == dependency.Key);
                if (tempCriterion == null)
                {
                    continue;
                }

                var innerContent = string.Format(
                    "\"key\": \"{0}\", \"type\": \"{1}\", \"value\": \"{2}\"",
                    tempCriterion.Key,
                    EnumUtils.stringValueOf(tempCriterion.UIType),
                    dependency.Value);
                dependencies.Add("{" + innerContent + "}");
            }

            return "[" + string.Join(",", dependencies) + "]";
        }

        #region Create Control Helper Methods

        private static HtmlGenericControl CreateDatePicker(string adjustedID, Criterion startCriterion, Criterion endCriterion)
        {
            var radMinDate = new RadDatePicker
            {
                ID = "RadMinDatePicker",
                AutoPostBack = false,
                Skin = "Vista",
                Width = System.Web.UI.WebControls.Unit.Pixel(140),
                MinDate = DateTime.Parse("01/01/1000"),
                MaxDate = DateTime.Parse("01/01/3000"),
                ZIndex = 9999
            };

            radMinDate.ClientEvents.OnDateSelected = "DateSelected";
            radMinDate.Attributes.Add("DateRangePosition", startCriterion.Key);
            radMinDate.Attributes["updateMessageHeader"] = adjustedID;
            radMinDate.Calendar.SpecialDays.Add(
                new RadCalendarDay { Repeatable = Telerik.Web.UI.Calendar.RecurringEvents.Today });

            radMinDate.DateInput.EmptyMessage = "Start...";

            var radMaxDate = new RadDatePicker { ID = "RadMaxDatePicker", AutoPostBack = false, Skin = "Vista", ZIndex = 9999 };

            radMaxDate.ClientEvents.OnDateSelected = "DateSelected";
            radMaxDate.Attributes.Add("DateRangePosition", endCriterion.Key);
            radMaxDate.Attributes["updateMessageHeader"] = adjustedID;
            radMaxDate.DateInput.EmptyMessage = "End...";
            radMaxDate.Calendar.SpecialDays.Add(
                new RadCalendarDay { Repeatable = Telerik.Web.UI.Calendar.RecurringEvents.Today });

            //textBox.ClientEvents.OnBlur = "onInputBlur";
            //textBox.Attributes["updateMessageHeader"] = adjustedID;
            //textBox.Text = criterion.Object == null ? string.Empty : criterion.Object.ToString();
            var wrapperDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");

            var wrapperDivLeft = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            wrapperDivLeft.Style.Add("float", "left");
            wrapperDivLeft.Style.Add("width", "149px");
            wrapperDivLeft.Controls.Add(radMinDate);

            var wrapperDivRight = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            wrapperDivRight.Style.Add("float", "right");
            wrapperDivRight.Style.Add("width", "149px");
            wrapperDivRight.Controls.Add(radMaxDate);

            wrapperDiv.Controls.Add(wrapperDivLeft);
            wrapperDiv.Controls.Add(wrapperDivRight);
            return wrapperDiv;
        }



        private RadComboBox CreateDropDownList(Criterion criterion, string adjustedID)
        {
            var comboBox = new RadComboBox
            {
                ID = "RadCombobBoxCriteriaDropDownList" + adjustedID,
                AutoPostBack = false,
                MarkFirstMatch = true,
                AllowCustomText = false,
                ZIndex = 9002,
                OnClientSelectedIndexChanged = "onSelectedIndexChanged",
                //OnClientSelectedIndexChanging = "OnClientSelectedIndexChanging",
                Skin = "Vista",
                Width = 180
            };
            if (criterion.Dependencies != null && criterion.Dependencies.Length > 0)
            {


                //CALL DEP METHOD
                comboBox.Attributes["dependencies"] = DependencyString(criterion);
            }

            if (!string.IsNullOrEmpty(criterion.ServiceUrl))
            {
                comboBox.Attributes["serviceurl"] = criterion.ServiceUrl;
            }

            if (!string.IsNullOrEmpty(criterion.ServiceOnSuccess))
            {
                comboBox.Attributes["successcallback"] = criterion.ServiceOnSuccess;
            }

            if (criterion.DataSource == null)
            {
                comboBox.Items.Add(new RadComboBoxItem("No data supplied", adjustedID + "_" + "0"));
            }
            else
            {

                if (criterion.Header.ToLower() == "groups") comboBox.Items.Add(new RadComboBoxItem("All", adjustedID + "_"));
                else comboBox.Items.Add(new RadComboBoxItem("Select a " + criterion.Header, "0"));



                foreach (DataRow row in ((DataTable)criterion.DataSource).Rows)
                {
                    comboBox.Items.Add(
                        new RadComboBoxItem(
                            row[criterion.DataTextField].ToString(), adjustedID + "_" + StripString(row[criterion.DataValueField].ToString())));
                }

                if (criterion.Object != null && !string.IsNullOrEmpty(criterion.Object.ToString()))
                {
                    comboBox.SelectedIndex = comboBox.Items.FindItemIndexByText(criterion.Object.ToString(), true);
                }
                else if (FirstTimeLoaded && !String.IsNullOrEmpty(criterion.DefaultValue))
                {
                    comboBox.SelectedIndex = comboBox.Items.FindItemIndexByValue(criterion.Key + "_" + StripString(criterion.DefaultValue), true);
                    criterion.Object = comboBox.SelectedItem.Text;
                    criterion.ReportStringVal = criterion.DefaultValue;
                }

                if (IsPostBack && !string.IsNullOrEmpty(criterion.ServiceUrl))
                {
                    var javascript = "function () { serviceControlsList['" + adjustedID + "'].loaded = true; serviceControlsList['" + adjustedID + "'].callback = function () { loadServiceData('" + adjustedID + "', 'DropDownList'); } }";
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "StartupService" + adjustedID, "addServiceControl('" + adjustedID + "');", true);
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "CheckServiceControls", "loadingInterval = window.setInterval('checkServiceControlsFullyLoaded()', 200);", true);

                    comboBox.OnClientLoad = javascript;
                }
            }
            return comboBox;
        }

        private static RadTextBox CreateTextBox(Criterion criterion, string adjustedID)
        {
            var textBox = new RadTextBox { ID = "RadTextBoxCriteria" + adjustedID, AutoPostBack = false, Skin = "Vista" };

            textBox.ClientEvents.OnBlur = "onInputBlur";
            textBox.Attributes["updateMessageHeader"] = adjustedID;
            textBox.Text = criterion.Object == null ? string.Empty : criterion.Object.ToString();
            return textBox;
        }

        private static HtmlGenericControl CreateRTI(Criterion criterion, string adjustedID)
        {
            var outerDiv = new HtmlGenericControl("div");
            outerDiv.Style.Add("background-color", "white");
            outerDiv.Style.Add("border", "1px solid #979797;");

            var rtiSerializer = new JavaScriptSerializer();
            RTIJsonObject rtiObject = null;

            if (!string.IsNullOrEmpty(criterion.ReportStringVal))
            {
                rtiObject = rtiSerializer.Deserialize<RTIJsonObject>(criterion.ReportStringVal);
            }

            var tiers = RTI.GetAllTiers();

            foreach (var tier in tiers)
            {
                var containerDiv = new HtmlGenericControl("div");
                containerDiv.Style.Add("padding", "3px");

                var image = new HtmlGenericControl("img");

                switch (tier)
                {
                    case "Former Year":
                        image.Attributes["src"] = "../../Images/rti_gray.png";
                        image.Attributes["alt"] = "Gray";
                        break;

                    case "Current Tier 1":
                        image.Attributes["src"] = "../../Images/rti_green.png";
                        image.Attributes["alt"] = "Green";
                        break;

                    case "Current Tier 2":
                        image.Attributes["src"] = "../../Images/rti_yellow.png";
                        image.Attributes["alt"] = "Yellow";
                        break;

                    case "Current Tier 3":
                        image.Attributes["src"] = "../../Images/rti_red.png";
                        image.Attributes["alt"] = "Red";
                        break;
                }

                image.Style.Add("width", "26px");
                image.Style.Add("height", "26px");
                image.Style.Add("vertical-align", "middle");
                image.Style.Add("margin-left", "3px");

                var checkBox = new RadButton
                {
                    AutoPostBack = false,
                    ToggleType = ButtonToggleType.CheckBox,
                    ButtonType = RadButtonType.ToggleButton,
                    Text = tier,
                    Value = tier,
                    Skin = "Vista",
                    OnClientCheckedChanged = "onRTICheckedChanged"
                };

                if (rtiObject != null)
                {
                    var rti = rtiObject.items.Find(r => r.text == tier);
                    if (rti != null)
                    {
                        checkBox.Checked = true;
                    }
                }

                checkBox.Style.Add("vertical-align", "middle");

                containerDiv.Controls.Add(checkBox);
                containerDiv.Controls.Add(image);
                outerDiv.Controls.Add(containerDiv);
            }

            return outerDiv;
        }

        private static HtmlGenericControl CreateDemographics(Criterion criterion, string adjustedID)
        {
            // Get all the demographic information and break it up into categories
            var demographics = Demographic.GetListOfDemographics();
            var raceDemographics = demographics.FindAll(r => r.Label == "Race");
            var genderDemographics = demographics.FindAll(r => r.Label == "Gender");
            var otherDemographics = demographics.FindAll(r => r.Label != "Race" && r.Label != "Gender" && r.DemoField > 0);

            var serializer = new JavaScriptSerializer();
            DemographicJsonObject demographicObject = null;

            if (!string.IsNullOrEmpty(criterion.ReportStringVal))
            {
                demographicObject = serializer.Deserialize<DemographicJsonObject>(criterion.ReportStringVal);
            }

            var outerDiv = new HtmlGenericControl("div");
            outerDiv.Style.Add("background-color", "white");
            outerDiv.Style.Add("border", "1px solid #979797;");

            var raceComboBoxItems = raceDemographics.Select(demographic => new RadComboBoxItem(demographic.Value, string.Format("{0}_{1}_{2}", "Race", demographic.Value, demographic.DemoField)));
            var genderComboBoxItems = genderDemographics.Select(demographic => new RadComboBoxItem(demographic.Abbreviation, string.Format("{0}_{1}_{2}", "Gender", demographic.Value, demographic.DemoField)));

            var selectedGenderText = string.Empty;
            var selectedRaceText = string.Empty;

            if (demographicObject != null)
            {
                var selectedGender = demographicObject.items.Find(d => d.groupName == "Gender");
                if (selectedGender != null)
                {
                    selectedGenderText += string.Format("{0}_{1}_{2}", "Gender", selectedGender.value, selectedGender.demoField);
                }

                var selectedRace = demographicObject.items.Find(d => d.groupName == "Race");
                if (selectedRace != null)
                {
                    selectedRaceText += string.Format("{0}_{1}_{2}", "Race", selectedRace.value, selectedRace.demoField);
                }
            }

            outerDiv.Controls.Add(CreateDemographicComboBox("Gender", genderComboBoxItems, selectedGenderText));
            outerDiv.Controls.Add(CreateDemographicComboBox("Race", raceComboBoxItems, selectedRaceText));

            foreach (var demographic in otherDemographics)
            {
                var groupName = StripString(demographic.Label);
                var value = string.Format("{0}_{1}", demographic.DemoField, demographic.Abbreviation);

                var allRadioButton = CreateRadioButton("All", value, groupName);
                allRadioButton.Checked = true;

                var yesRadioButton = CreateRadioButton("Yes", value, groupName);
                yesRadioButton.Style.Add("margin-left", "10px");

                var noRadioButton = CreateRadioButton("No", value, groupName);
                noRadioButton.Style.Add("margin-left", "10px");

                if (demographicObject != null)
                {
                    var jsonObject = demographicObject.items.Find(d => d.abbreviation == demographic.Abbreviation);
                    if (jsonObject != null)
                    {
                        allRadioButton.Checked = false;
                        yesRadioButton.Checked = false;
                        noRadioButton.Checked = false;

                        switch (jsonObject.text)
                        {
                            case "All":
                                allRadioButton.Checked = true;
                                break;

                            case "Yes":
                                yesRadioButton.Checked = true;
                                break;

                            case "No":
                                noRadioButton.Checked = true;
                                break;
                        }
                    }
                }

                // Add the text and 3 radio buttons to the outer div
                var itemDiv = new HtmlGenericControl("div");
                itemDiv.Style.Add("margin-left", "5px");
                itemDiv.Style.Add("margin-top", "5px");

                var radioButtonsGroupTextSpan = new HtmlGenericControl("span") { InnerText = demographic.Label + ":" };
                radioButtonsGroupTextSpan.Style.Add("float", "left");
                radioButtonsGroupTextSpan.Style.Add("width", "200px");

                var radioButtonsDiv = new HtmlGenericControl("div");
                radioButtonsDiv.Style.Add("display", "inline");

                radioButtonsDiv.Controls.Add(allRadioButton);
                radioButtonsDiv.Controls.Add(yesRadioButton);
                radioButtonsDiv.Controls.Add(noRadioButton);

                itemDiv.Controls.Add(radioButtonsGroupTextSpan);
                itemDiv.Controls.Add(radioButtonsDiv);

                outerDiv.Controls.Add(itemDiv);
            }

            return outerDiv;
        }

        private static RadButton CreateRadioButton(string text, string value, string groupName = "")
        {
            var id = "RadButtonCriteriaDemographicsRadioButton" + groupName + StripString(text);

            return new RadButton
            {
                AutoPostBack = false,
                ID = id,
                ToggleType = ButtonToggleType.Radio,
                ButtonType = RadButtonType.ToggleButton,
                GroupName = groupName,
                Text = text,
                Value = value,
                Skin = "Web20",
                OnClientCheckedChanged = "onDemographicCheckedChanged"
            };
        }

        private static HtmlGenericControl CreateDemographicComboBox(string text, IEnumerable<RadComboBoxItem> items = null, string selectedItemValue = "")
        {
            var id = "RadButtonCriteriaDemographicsComboBox" + StripString(text);

            var comboBox = new RadComboBox
            {
                ID = id,
                AutoPostBack = false,
                AllowCustomText = false,
                MarkFirstMatch = true,
                Skin = "Vista",
                ZIndex = 8500,
                OnClientSelectedIndexChanged = "onDemographicSelectedIndexChanged"
            };

            if (items != null)
            {
                comboBox.Items.Add(new RadComboBoxItem("Select a " + text, string.Format("{0}_-1", text)));

                foreach (var item in items)
                {
                    comboBox.Items.Add(item);
                }
            }

            if (!string.IsNullOrEmpty(selectedItemValue))
            {
                comboBox.SelectedValue = selectedItemValue;
            }

            var itemDiv = new HtmlGenericControl("div");
            itemDiv.Style.Add("margin-left", "5px");
            itemDiv.Style.Add("margin-top", "5px");

            var comboBoxSpan = new HtmlGenericControl("span") { InnerText = text + ":" };
            comboBoxSpan.Style.Add("float", "left");
            comboBoxSpan.Style.Add("width", "200px");

            itemDiv.Controls.Add(comboBoxSpan);
            itemDiv.Controls.Add(comboBox);

            return itemDiv;
        }

        private RadListBox CreateCheckBoxList(Criterion criterion, string adjustedID)
        {
            var listBox = new RadListBox
            {
                ID = "RadCombobBoxCriteriaCheckBoxList" + adjustedID,
                AutoPostBack = false,
                CheckBoxes = true,
                Skin = "Vista",
                OnClientItemChecked = "onItemChecked",
                CssClass = "noWrapRadListBox"
            };

            if (criterion.Dependencies != null && criterion.Dependencies.Length > 0)
            {


                listBox.Attributes["dependencies"] = DependencyString(criterion);
            }

            if (!string.IsNullOrEmpty(criterion.ServiceUrl))
            {
                listBox.Attributes["serviceurl"] = criterion.ServiceUrl;
            }

            if (!string.IsNullOrEmpty(criterion.ServiceOnSuccess))
            {
                listBox.Attributes["successcallback"] = criterion.ServiceOnSuccess;
            }

            if (string.IsNullOrEmpty(listBox.Attributes["ListBoxIdentifier"]))
            {
                listBox.Attributes["ListBoxIdentifier"] = listBox.ClientID;
            }

            listBox.Style.Add(HtmlTextWriterStyle.Overflow, "auto !important");
            listBox.Style.Add(HtmlTextWriterStyle.Height, "100%");
            listBox.Style.Add(HtmlTextWriterStyle.Width, "100%");

            if (criterion.DataSource == null)
            {
                listBox.Items.Add(new RadListBoxItem("No data supplied", string.Format("{0}_0", adjustedID)));
                listBox.CheckBoxes = false;
            }
            else
            {
                foreach (DataRow row in ((DataTable)criterion.DataSource).Rows)
                {
                    var listBoxValue = string.Format(
                        "{0}_{1}", adjustedID, StripString(row[criterion.DataValueField].ToString()));
                    var listBoxItem = new RadListBoxItem(row[criterion.DataTextField].ToString(), listBoxValue);
                    listBoxItem.Attributes["checkBoxID"] =
                        string.Format("{0}_RadCombobBoxCriteriaCheckBoxList{1}_CheckBox", listBoxValue, adjustedID);

                    // This fails with an exception if no First item.
                    //var tempCriterion = Criteria.CriterionList.First(r => r.Key == listBoxValue);
                    // Do it so that no exceptions are generated.
                    Criterion tempCriterion = null;
                    foreach (Criterion c in this.Criteria.CriterionList)
                    {
                        if (c.Key == listBoxValue)
                        {
                            tempCriterion = c;
                            break;
                        }
                    }

                    if (tempCriterion != null)
                    {
                        if (FirstTimeLoaded)
                        {
                            if (!String.IsNullOrEmpty(tempCriterion.DefaultValue) &&
                                tempCriterion.DefaultValue.Contains(StripString(row[criterion.DataValueField].ToString())))
                            {
                                tempCriterion.Object = listBoxItem.Text;
                                tempCriterion.ReportStringVal = tempCriterion.DefaultValue;
                                listBoxItem.Checked = true;
                            }
                            else
                            {
                                listBoxItem.Checked = !tempCriterion.Empty;
                            }
                        }
                        else
                        {
                            if (adjustedID=="SchoolType")
                            {
                                listBoxItem.Checked = hiddenSchoolTypeListSelected.Text.Contains(listBoxItem.Value);
                            }
                            else
                            {
                                listBoxItem.Checked = hiddenGradeListSelected.Text.Contains(listBoxItem.Value);
                            }
                           
                        }
                    }

                    listBox.Items.Add(listBoxItem);
                }

                if (IsPostBack && !string.IsNullOrEmpty(criterion.ServiceUrl))
                {
                    var javascript = "function () { serviceControlsList['" + adjustedID + "'].loaded = true; serviceControlsList['" + adjustedID + "'].callback = function () { loadServiceData('" + adjustedID + "', 'CheckBoxList'); } }";
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "StartupService" + adjustedID, "addServiceControl('" + adjustedID + "');", true);
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "CheckServiceControls", "loadingInterval = window.setInterval('checkServiceControlsFullyLoaded()', 200);", true);

                    listBox.OnClientLoad = javascript;
                }
            }

            return listBox;
        }

        #endregion

        private static System.Web.UI.HtmlControls.HtmlGenericControl CreateRowDiv(Criterion criterion, int itemsToDisplay = 1)
        {
            // Locked criteria will not get an X beside them. 
            var key = criterion.Key;
            var value = criterion.Object.ToString();

            var newRowDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");

            if (string.IsNullOrEmpty(value))
            {
                return newRowDiv;
            }

            newRowDiv.Attributes["class"] = "criteriaItemDiv";

            // Remove spaces, parenthesis and dashes
            var adjustedKey = StripString(key);

            newRowDiv.Attributes["id"] = "key_" + adjustedKey;

            var newRowDivImg = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            if (criterion.Removable)
            {
                var onClickJavascript = string.Format("removeCriterion(this,'{0}');", adjustedKey);

                switch (criterion.UIType)
                {
                    case UIType.CheckBoxList:
                        onClickJavascript += string.Format("uncheckTooltipCheckBox('{0}_RadCombobBoxCriteriaCheckBoxList{1}_CheckBox');", adjustedKey, criterion.Header);
                        break;
                }

                newRowDivImg.Attributes["onclick"] = onClickJavascript;
                newRowDivImg.Attributes["id"] = adjustedKey + "_RemoveCriterionButton";

                var image = new Image { ImageUrl = "~/Images/close_x.gif" };
                image.Attributes["id"] = adjustedKey + "_Image";

                newRowDivImg.Controls.Add(image);
            }

            newRowDivImg.Attributes["class"] = "left";

            var newRowDivLbl = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            newRowDivLbl.Attributes["id"] = adjustedKey + "_Text";
            newRowDivLbl.Attributes["class"] = "right";
            newRowDivLbl.InnerHtml = itemsToDisplay == 2 ? key + ": " + value : value;

            newRowDiv.Controls.Add(newRowDivImg);
            newRowDiv.Controls.Add(newRowDivLbl);

            return newRowDiv;
        }

        private static string StripString(string oldValue)
        {
            // awful kludge here but ultimately, this older criteria control should be replaced with updated version that isn't encoding values with underscores
            return oldValue.Replace("(", "-lp-").Replace(")", "-rp-").Replace(" ", "-s-").Replace(".", "-p-").Replace(",", "-c-").Replace("9_12", "9-12").Replace("6_12", "6-12").Replace("K_12", "K-12");
        }

        private static string UnStripString(string oldValue)
        {
            // awful kludge here but ultimately, this older criteria control should be replaced with updated version that isn't encoding values with underscores
            return oldValue.Replace("-lp-", "(").Replace("-rp-", ")").Replace("-s-", " ").Replace("-p-", ".").Replace("-c-", ",").Replace("9-12", "9_12").Replace("6-12", "6_12").Replace("K-12", "K_12");
        }

        private void WriteSearchParms()
        {
            if (string.IsNullOrEmpty(Guid))
            {
                KillReport("Could not write searchparms. <br/> No Guid provided in ReportCriteria control.");
            }

            var criteriaHeaders = (from criterion in Criteria.CriterionList.OrderBy(c => c.Locked.ToString())
                               select criterion.Header).Distinct();        
            criteriaRepeater.DataSource = criteriaHeaders;
            criteriaRepeater.DataBind();
        }

        protected Boolean CheckSelectedFields(string adjustedKey)
        {
            if (hiddenFieldsSelected.Text.Contains(adjustedKey))
            {
                return true;
            }

            hiddenFieldsSelected.Text += adjustedKey + ",";
            return false;
        }

        protected void BtnUpdateCriteriaClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Guid))
            {
                return;
            }
            var tempCriteria = Criteria;

            var deleteString = hiddenTextField.Value;
            var updateString = changedSelections.Value;
            

            // HANDLE DELETES
            var deletes = deleteString.Split(";".ToCharArray());
            foreach (var delete in deletes)
            {
                if (string.IsNullOrEmpty(delete))
                {
                    continue;
                }

                var criterion = tempCriteria.CriterionList.First(r => r.Key == delete);
                if (criterion != null)
                {
                    criterion.Empty = true;
                }
            }

            // HANDLE UPDATES
            var updates = updateString.Split(";".ToCharArray());
            var listOfKeys = updates.Where(x => x.ToString() != string.Empty).Select(x => x.Substring(0, x.IndexOf("~"))).ToList();


            foreach (var update in updates)
            {
                if (listOfKeys.Count > 0)
                {
                    listOfKeys.RemoveAt(0);
                }

                if (string.IsNullOrEmpty(update))
                {
                    continue;
                }

                var parse = update.Split("~".ToCharArray());
                if (parse.Length != 2)
                {
                    continue;
                }

                var adjustedParseKey = parse[0];
                var adjustedParseID = parse[1];

                if (adjustedParseID == "0" || ((listOfKeys.Count > 0 ? listOfKeys.Contains(adjustedParseKey) : false) && adjustedParseKey == "TextSearch"))
                {
                    continue;
                }
                var elementId = update.Replace("~", "_");
                if (CheckSelectedFields(elementId))
                {
                    continue;
                }

                //var matches = updates.Where(u => u.Contains(adjustedParseKey) && !u.Equals(update));

                string objectName = string.Empty;
                var convertedAjustedParseID = DataIntegrity.ConvertToInt(adjustedParseID);

                switch (adjustedParseKey)
                {
                    case "Student":
                        objectName = Base.Classes.Data.StudentDB.GetStudentByID(convertedAjustedParseID).StudentName;
                        break;

                    case "Teacher":
                    case "TeacherName":
                        objectName = Thinkgate.Base.Classes.Data.TeacherDB.GetTeacherByPage(convertedAjustedParseID).TeacherName;
                        break;

                    case "Class":
                        objectName = Thinkgate.Base.Classes.Class.GetClassByID(convertedAjustedParseID).ClassName;
                        break;
                    case "StandardCourse":
                    case "StandardCourseFilter":
                        var standardCourse = Thinkgate.Base.Classes.CourseMasterList.GetStandardCourseById(convertedAjustedParseID);

                        objectName = standardCourse != null
                        ? standardCourse.CourseName
                        : "Not set!";
                        break;

                    case "Assessment":
                        objectName = Thinkgate.Base.Classes.Assessment.GetAssessmentByID(convertedAjustedParseID).TestName;
                        break;

                    case "School":
                        objectName = Thinkgate.Base.Classes.School.GetSchoolByID(convertedAjustedParseID).Name;
                        break;

                    case "TextSearchFilter":
                    case "TextSearchType":
                    case "StandardFilter":                  
                    case "Block":
                    case "UserFullName":
                    case "UserID":
                    case "TestType":
                    case "AssessmentAssignmentTeacher":
                    case "SchoolType":
                        objectName = UnStripString(adjustedParseID);
                        break;
                        case "RoleName":
                        objectName =UnStripString(adjustedParseID);
                        break;
                    case "Section":
                    case "SchoolName":
                    case "Cluster":
                    case "Name":
                    case "StudentID":
                    case "LoginID":
                    case "SchoolID":
                    case "Term":
                    case "Terms":
                    case "Status":
                    case "StandardSet":
                    case "ItemBank":
                    case "RubricType":
                    case "Type":
                    case "Year":
                    case "Category":
                    case "Subject":
                    case "CreatedDateStart":
                    case "CreatedDateEnd":
                    case "Grades":
                    case "Subtype":
                    case "Period":
                    case "Semester":
                    case "TestID":
                    case "Grade":
                    case "Enrollment":
                        objectName = UnStripString(adjustedParseID);
                        break;

                    case "Curriculum":
                        var course = Thinkgate.Base.Classes.CourseMasterList.GetCurrCourseById(convertedAjustedParseID);

                        objectName = course != null
                                        ? course.Grade + "-" + course.CourseName
                                        : "Not set!";

                        break;
                    case "ClassCourse":
                    case "ClassCourses":

                        var classCourse = Thinkgate.Base.Classes.CourseMasterList.GetClassCourseById(convertedAjustedParseID);

                        objectName = classCourse != null
                                        ? classCourse.CourseName
                                        : "Not set!";

                        break;
                    case "TextSearch":
                        var cmbSelection = adjustedParseID.Substring(0, adjustedParseID.IndexOf("||"));
                        var textInput = adjustedParseID.Substring(adjustedParseID.IndexOf("||") + 2, adjustedParseID.Length - adjustedParseID.IndexOf("||") - 2);
                        objectName = cmbSelection + ":" + textInput;

                        break;

                    case "Demographics":
                        var serializer = new JavaScriptSerializer();
                        var demographicObject = serializer.Deserialize<DemographicJsonObject>(adjustedParseID);
                        if (demographicObject == null)
                        {
                            break;
                        }

                        objectName = demographicObject.items.Aggregate(
                            string.Empty,
                            (current, item) => current + string.Format("<div><span style='font-weight: bold;'>{0}:</span> {1}</div>", UnStripString(item.abbreviation), item.text));

                        break;

                    case "RTI":
                        var rtiSerializer = new JavaScriptSerializer();
                        var rtiObject = rtiSerializer.Deserialize<RTIJsonObject>(adjustedParseID);
                        if (rtiObject == null)
                        {
                            break;
                        }

                        objectName = rtiObject.items.Aggregate(
                            string.Empty,
                            (current, item) => current + string.Format("<div>{0}</div>", item.text));

                        break;
                    default:
                        objectName = "Not set!";
                        break;
                }

                var allCriterionNodes = tempCriteria.CriterionList.FindAll(r => r.Key.Contains(adjustedParseKey));
                var wasFound = false;

                if (allCriterionNodes.Count > 1)
                {
                    foreach (var criterion in allCriterionNodes.Where(criterion => !criterion.IsHeader && criterion.Object == null && criterion.Key.Contains(adjustedParseID)))
                    {
                        criterion.Object = objectName;
                        criterion.ReportStringVal = UnStripString(adjustedParseID);
                        wasFound = true;
                        break;
                    }
                }

                if (adjustedParseKey.Contains("CreatedDate"))
                {
                    if (allCriterionNodes.Count == 1 || !wasFound)
                    {
                        var criterion = allCriterionNodes.First(r => !r.IsHeader && r.Key == adjustedParseKey);
                        criterion.Object = adjustedParseKey.Substring("CreatedDate".Length) + ": " + objectName;
                        criterion.ReportStringVal = adjustedParseID;
                    }
                }
                else
                {
                    if (allCriterionNodes.Count == 1 || !wasFound)
                    {
                        var criterion = allCriterionNodes.First(r => r.IsHeader && r.Key == adjustedParseKey);
                        criterion.Object = objectName;
                        criterion.ReportStringVal = adjustedParseID;
                    }
                }
            }

            Criteria = tempCriteria;

            WriteSearchParms();

            hiddenTextField.Value = string.Empty;
            changedSelections.Value = string.Empty;

            if (ReloadReport != null)
            {
                ReloadReport(null, new EventArgs());
            }
        }

        protected void LoadCriteriaItems(object sender, RepeaterItemEventArgs e)
        {
            if (string.IsNullOrEmpty(Guid))
            {
                return;
            }

            var criterionHeader = e.Item;

            if (criterionHeader == null)
            {
                return;
            }

            if ((criterionHeader.ItemType == ListItemType.Item) || (criterionHeader.ItemType == ListItemType.AlternatingItem))
            {
                var placeholderHeader = criterionHeader.FindControl("phCriterionHeader");
                var placeholderItem = criterionHeader.FindControl("phCriterionItems");

                if (placeholderHeader == null || placeholderItem == null)
                {
                    return;
                }

                var criterion = Criteria.CriterionList.FindLast(r => r.IsHeader && r.Header == criterionHeader.DataItem.ToString());

                if (criterion == null || !criterion.Visible) //Visible Override function above
                {
                    return;
                }

                placeholderHeader.Controls.Add(CreateHeaderDiv(criterion));

                foreach (var instance in Criteria.CriterionList.Where(r => !r.Empty && r.Header == criterionHeader.DataItem.ToString()))
                {
                    placeholderItem.Controls.Add(CreateRowDiv(instance));
                }
            }
        }

        /// <summary>
        /// Certain custom controls need to add extra criterion objects below the header. This is where that happens.
        /// </summary>
        private void LoadExtraCriterion()
        {
            var insertCriterion = new Dictionary<int, List<Criterion>>();
            var criteriaHeaders = (from criterion in Criteria.CriterionList select criterion).Where(r => r.IsHeader).Distinct();

            foreach (var criteriaHeader in criteriaHeaders)
            {
                switch (criteriaHeader.UIType)
                {
                    case UIType.CheckBoxList:
                        {
                            if (criteriaHeader.DataSource == null)
                            {
                                break;
                            }

                            var checkBoxCriteria = new List<Criterion>();

                            foreach (DataRow row in ((DataTable)criteriaHeader.DataSource).Rows)
                            {
                                var clonedCriterion = criteriaHeader.Clone();
                                clonedCriterion.IsHeader = false;
                                clonedCriterion.Key = string.Format("{0}_{1}", criteriaHeader.Key,  StripString(row[criteriaHeader.DataValueField].ToString()));

                                checkBoxCriteria.Add(clonedCriterion);
                            }

                            var index = Criteria.CriterionList.FindIndex(r => r.Header == criteriaHeader.Header && r.IsHeader);
                            insertCriterion.Add(index, checkBoxCriteria);

                            break;
                        }
                }
            }

            var adjustedIndex = 0;

            // Actually insert the ranges into the criteria object so we don't get a collection modified exception.
            foreach (var criterion in insertCriterion)
            {
                var insertIndex = criterion.Key + adjustedIndex;

                if ((Criteria.CriterionList.Count - 1) == insertIndex)
                {
                    Criteria.CriterionList.AddRange(criterion.Value);
                }
                else
                {
                    Criteria.CriterionList.InsertRange(insertIndex + 1, criterion.Value);
                }

                adjustedIndex += criterion.Value.Count;
            }
        }

        protected void RadButtonClear_Click(object sender, EventArgs e)
        {

            foreach (var criterion in Criteria.CriterionList)
            {
                criterion.Clear();
            }
            //if (ReloadReport != null)
            //{
            //    ReloadReport(null, new EventArgs());
            //}
            //BJC - 6/13/2012: Clear the required fields hidden inputs so that the WriteSearchParms method doesn't add duplicate required keys.
            requiredFields.Value = String.Empty;
            requiredFieldsSelected.Value = String.Empty;
            hiddenGradeListSelected.Text = String.Empty;
            hiddenSubjectListSelected.Text = String.Empty;
            hiddenFieldsSelected.Text = String.Empty;

            WriteSearchParms();
        }

        // TODO: Needs to move into its own file
        public class DemographicJsonObject
        {
            public List<Item> items { get; set; }

            public class Item
            {
                public string abbreviation { get; set; }

                public string demoField { get; set; }

                public string groupName { get; set; }

                public string text { get; set; }

                public string value { get; set; }
            }
        }

        // TODO: Needs to move into its own file
        public class RTIJsonObject
        {
            public List<Item> items { get; set; }

            public class Item
            {
                public string text { get; set; }

                public string value { get; set; }
            }
        }
    }
}
