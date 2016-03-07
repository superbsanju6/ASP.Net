using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using Standpoint.Core.Utilities;
using Telerik.Web;
using Telerik.Charting;
using Telerik.Web.UI;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using System.Text;

namespace Thinkgate.Controls.Assessment
{
	public partial class AssessmentContent : TileControlBase
	{
		private Int32 _assessmentID, _userID;
		private DataSet _ds;
		private Thinkgate.Base.Classes.Assessment _assessment;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Tile == null) return;

			_assessmentID = (Int32)Tile.TileParms.GetParm("assessmentID");
			_assessment = (Thinkgate.Base.Classes.Assessment)Tile.TileParms.GetParm("assessment");
			_userID = SessionObject.LoggedInUser.Page;

			_ds = ThinkgateDataAccess.FetchDataSet("E3_Assessment_GetByID", new object[] { _assessmentID, _userID });
			_ds.Tables[0].TableName = "Summary";
			_ds.Tables[1].TableName = "StandardQuestionCounts";
			_ds.Tables[2].TableName = "RigorCounts";

			DataTable summaryTbl = _ds.Tables["Summary"];
			DataTable standardQuestionCountsTbl = _ds.Tables["StandardQuestionCounts"];
			DataTable rigorCountsTbl = _ds.Tables["RigorCounts"];
			Int32 totalRigorLevelCount = 0;
				Int32 totalRigorCount = 0;

			foreach(DataRow row in rigorCountsTbl.Rows)
			{
								if (String.Compare("Blank", row["Text"].ToString(), true) != 0 &&
                                         (Int32)row["ItemCount"] > 0 && String.Compare("Not Specified", row["Text"].ToString(), true) != 0)
								{
										totalRigorLevelCount++;
										totalRigorCount += DataIntegrity.ConvertToInt(row["ItemCount"]);
								}
			}

						LoadSummaryView(summaryTbl, standardQuestionCountsTbl, totalRigorLevelCount, _assessment);
			LoadStandardView(standardQuestionCountsTbl);
						LoadRigorView(rigorCountsTbl, totalRigorCount);
		}

		private void LoadSummaryView(DataTable summaryTbl, DataTable standardQuestionCountsTbl,
																																 Int32 totalRigorLevelCount, Thinkgate.Base.Classes.Assessment assessment)
		{
			DataRow summaryRow = summaryTbl.Rows[0];

			lblStatus.Text = (DataIntegrity.ConvertToBool(summaryRow["TestProofed"])) ? "Proofed" : "Unproofed";
			lblStandards.Text = standardQuestionCountsTbl.Rows.Count.ToString();
						lblRigorLevels.Text = totalRigorLevelCount.ToString();

                        
			//lblitems.Text = assessment.Items.Count.ToString();
		    lblitems.Text = standardQuestionCountsTbl.Rows.Count != 0 ? standardQuestionCountsTbl.Rows[0]["TotalItemCount"].ToString() : "0";
			lblAddendums.Text = assessment.AddendumCount.ToString(); 
			lblRubrics.Text = summaryRow["RubricCount"].ToString();
			DateTime? edited = DataIntegrity.ConvertToNullableDate(summaryRow["DateUpdated"]);
			DateTime? created = DataIntegrity.ConvertToNullableDate(summaryRow["DateCreated"]);
			lblLastEdited.Text = (edited.HasValue) ? edited.Value.ToShortDateString() : (created.HasValue) ? created.Value.ToShortDateString() : String.Empty;

#if false // Original content.
			lblContType.Text = attrRow["Content_Type"].ToString();
			lblFieldTest.Text = attrRow["FieldTest"].ToString();
			lblNumForms.Text = DataIntegrity.ConvertToInt(attrRow["FormCount"]).ToString();
			lblFormSequence.Text = attrRow["FormSeq"].ToString();
			lblScoreType.Text = attrRow["ScoreType"].ToString();
			if(String.IsNullOrEmpty(lblScoreType.Text))
				lblScoreType.Text = "Percent";
			lblDistractors.Text = attrRow["AlternatingDistractorValue"].ToString();
			DateTime? edited = DataIntegrity.ConvertToNullableDate(attrRow["DateUpdated"]);
			DateTime? created = DataIntegrity.ConvertToNullableDate(attrRow["DateCreated"]);
			lblLastEdited.Text = (edited.HasValue) ? edited.Value.ToShortDateString() : (created.HasValue) ? created.Value.ToShortDateString() : String.Empty;
#endif
		}

		private void LoadStandardView(DataTable standardQuestionCountsTbl)
		{
			standardQuestionCountsTbl.Columns.Add("PercentAssessment", typeof(String));
			foreach (DataRow row in standardQuestionCountsTbl.Rows)
			{
				Int32 stdQuestionCount = 0, totalItemCount = 0;
				Int32.TryParse(row["StandardQuestionCount"].ToString(), out stdQuestionCount);
				Int32.TryParse(row["TotalItemCount"].ToString(), out totalItemCount);
				row["PercentAssessment"] = (stdQuestionCount <= 0 || totalItemCount <= 0) ? "0%" : Math.Round(100.0 * stdQuestionCount / totalItemCount).ToString() + "%";

			}
			grdStdDist.DataSource = standardQuestionCountsTbl;
			grdStdDist.DataBind();
		}

		private void LoadRigorView(DataTable rigorCountsTbl, Int32 totalRigorCount)
		{
			DataRow row;
			StringBuilder sb = new StringBuilder();
			Int32 countIdx = rigorCountsTbl.Columns.IndexOf("ItemCount");
			Int32 textIdx = rigorCountsTbl.Columns.IndexOf("Text");
			Int32 mc3Idx = rigorCountsTbl.Columns.IndexOf("MultipleChoice3");
            Int32 mc4Idx = rigorCountsTbl.Columns.IndexOf("MultipleChoice4");
            Int32 mc5Idx = rigorCountsTbl.Columns.IndexOf("MultipleChoice5");
			Int32 saIdx = rigorCountsTbl.Columns.IndexOf("ShortAnswer");
			Int32 essayIdx = rigorCountsTbl.Columns.IndexOf("Essay");
			Int32 tfIdx = rigorCountsTbl.Columns.IndexOf("TrueFalse");
			Int32 count, mc3,mc4,mc5, sa, essay, tf;

			// Table header.
			sb.AppendFormat(@"<table border='1' style=cellpadding:0;' width='100%'>
										<tr>
											<th class='cellCommon cellHeader'>Distribution</th>
											<th class='cellCommon cellHeader'>{0}</th>
											<th class='cellCommon cellHeader' colspan='6'>Item Criteria Summary</th>
										</tr>", rigorCountsTbl.Rows[0]["RigorType"].ToString());

			// Top row.
			sb.AppendFormat(@"<tr>
													<td class='cellCommon cellContent'>{0}</td>
													<td class='cellCommon cellContent'></td>
													<th class='cellCommon cellHeader'>MC3</th>
                                                    <th class='cellCommon cellHeader'>MC4</th>
                                                    <th class='cellCommon cellHeader'>MC5</th>
													<th class='cellCommon cellHeader'>S/A</th>
													<th class='cellCommon cellHeader'>Essay</th>
													<th class='cellCommon cellHeader'>T/F</th>
												</tr>", totalRigorCount);

						foreach(DataRow currRow in rigorCountsTbl.Rows)
						{
								if (currRow["Text"].ToString() == "Not Specified" || currRow["Text"].ToString() == "Blank") continue;

								count = (Int32) currRow[countIdx];
								mc3 = (Int32) currRow[mc3Idx];
                                mc4 = (Int32)currRow[mc4Idx];
                                mc5 = (Int32)currRow[mc5Idx];
								sa = (Int32) currRow[saIdx];
								essay = (Int32) currRow[essayIdx];
								tf = (Int32) currRow[tfIdx];
								sb.AppendFormat(
                                        @"<tr>
												<td class='cellCommon cellContent'>{0}</td>
												<td class='cellCommon cellContent'>{13}</td>
												<td class='cellCommon {1}'>{2}</td>
												<td class='cellCommon {3}'>{4}</td>
												<td class='cellCommon {5}'>{6}</td>
												<td class='cellCommon {7}'>{8}</td>
                                                <td class='cellCommon {9}'>{10}</td>
                                                <td class='cellCommon {11}'>{12}</td>
										</tr>",
										Content(count),
										ContentStyle(mc3), Content(mc3),
                                        ContentStyle(mc4), Content(mc4),
                                        ContentStyle(mc5), Content(mc5),
										ContentStyle(sa), Content(sa),
										ContentStyle(essay), Content(essay),
										ContentStyle(tf), Content(tf), currRow["Text"].ToString());
						}

						//Not Specified
						row = (from r in rigorCountsTbl.AsEnumerable() where String.Compare(r.Field<String>("Text"), "Not Specified", true) == 0 select r).First();
                        count = (Int32)row[countIdx]; mc3 = (Int32)row[mc3Idx]; mc4 = (Int32)row[mc4Idx]; mc5 = (Int32)row[mc5Idx]; sa = (Int32)row[saIdx]; essay = (Int32)row[essayIdx]; tf = (Int32)row[tfIdx];
						sb.AppendFormat(@"<tr>
													<td class='cellCommon cellContent'>{0}</td>
													<td class='cellCommon cellContent'>Not Specified</td>
													<td class='cellCommon {1}'>{2}</td>
													<td class='cellCommon {3}'>{4}</td>
													<td class='cellCommon {5}'>{6}</td>
													<td class='cellCommon {7}'>{8}</td>
                                                    <td class='cellCommon {9}'>{10}</td>
                                                    <td class='cellCommon {11}'>{12}</td>
											</tr>", Content(count),
                                                                                        ContentStyle(mc3), Content(mc3),
                                                                                        ContentStyle(mc4), Content(mc4),
                                                                                        ContentStyle(mc5), Content(mc5),
																						ContentStyle(sa), Content(sa),
																						ContentStyle(essay), Content(essay),
																						ContentStyle(tf), Content(tf));


						//Blank Items
						row = (from r in rigorCountsTbl.AsEnumerable() where String.Compare(r.Field<String>("Text"), "Blank", true) == 0 select r).First();
                        count = (Int32)row[countIdx]; mc3 = (Int32)row[mc3Idx]; mc4 = (Int32)row[mc4Idx]; mc5 = (Int32)row[mc5Idx]; sa = (Int32)row[saIdx]; essay = (Int32)row[essayIdx]; tf = (Int32)row[tfIdx];
						sb.AppendFormat(@"<tr>
													<td class='cellCommon cellContent'>{0}</td>
													<td class='cellCommon cellContent'>Blank Items</td>
													<td class='cellCommon {1}'>{2}</td>
													<td class='cellCommon {3}'>{4}</td>
													<td class='cellCommon {5}'>{6}</td>
													<td class='cellCommon {7}'>{8}</td>
                                                    <td class='cellCommon {9}'>{10}</td>
                                                    <td class='cellCommon {11}'>{12}</td>
											</tr>", Content(count),
                                                                                        ContentStyle(mc3), Content(mc3),
                                                                                        ContentStyle(mc4), Content(mc4),
                                                                                        ContentStyle(mc5), Content(mc5),
																						ContentStyle(sa), Content(sa),
																						ContentStyle(essay), Content(essay),
																						ContentStyle(tf), Content(tf));

			// End table
			sb.Append(@"</table>");
			litRigorTbl.Text = sb.ToString();
		}

		private String ContentStyle(Int32 count)
		{
			return (count > 0) ? "cellContent" : "";
		}		

		private String Content(Int32 count)
		{
			return (count > 0) ? count.ToString() : "";
		}
	}
}