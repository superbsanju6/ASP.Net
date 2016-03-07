using System;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;
using System.Data;

namespace Thinkgate.Controls.Teacher
{
	public partial class StaffEvaluation : TileControlBase
	{
		private DataTable _evalTbl = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Tile == null) return;

			if (!IsPostBack)
				BuildUI();
		}

		private void BuildUI()
		{
			if (EvalTbl.Rows.Count > 0)
			{
				grdEval.DataSource = EvalTbl;
				grdEval.DataBind();
			}
		}


		/// <summary>
		/// Get the evaluations table. There are three user types that may have evaluations, Classroom Teacher (CI), Non-Classroom Teacher (NCI),
		/// and School-Based Administrator (SA).
		///
		/// It has columns:
		/// Int32 ID	(evaluation id)
		/// String Type (One of EvaluationTypes as string).
		/// String EvalName (text for first line)
		/// </summary>
		private DataTable EvalTbl
		{
			get
			{
				if(_evalTbl == null)
				{
					_evalTbl = Thinkgate.Base.Classes.Staff.GetStaffEvaluationsByID(SessionObject.LoggedInUser.Page);
					_evalTbl = Standpoint.Core.Classes.Encryption.EncryptDataTableColumn(_evalTbl, "ID", "ID_Encrypted");
					DataColumn saCol = _evalTbl.Columns.Add("IsSA", typeof(Boolean));
					foreach(DataRow row in _evalTbl.Rows)
						row[saCol] = (String.Compare(row["Type"].ToString(), EvaluationTypes.Administrator.ToString(), true) == 0);
				}
				_evalTbl.DefaultView.Sort = "EvalName";
				return _evalTbl;
			}
		}
	}
}