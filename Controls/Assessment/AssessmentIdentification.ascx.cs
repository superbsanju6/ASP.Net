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

namespace Thinkgate.Controls.Assessment
{
	public partial class AssessmentIdentification : TileControlBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Tile == null) return;

			Thinkgate.Base.Classes.Assessment assessment = (Thinkgate.Base.Classes.Assessment)Tile.TileParms.GetParm("assessment");

			lblGrade.Text = assessment.Grade;
			lblSubject.Text = assessment.Subject;
			lblCourse.Text = assessment.Course;
			lblType.Text = assessment.TestType;
			lblTerm.Text = assessment.Term;
			lblDescription.Text = assessment.Description;
			lblAuthor.Text = assessment.CreatedByName;
			lblCreated.Text = assessment.DateCreated.ToShortDateString();
		}
	}
}