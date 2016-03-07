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

namespace Thinkgate.Controls.LCO
{
    public partial class CourseIdentification : TileControlBase
    {
        protected Thinkgate.Base.Classes.LCO _lco;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            _lco = ((Thinkgate.Base.Classes.LCO)Tile.TileParms.GetParm("LCO"));

            lblGrade.Text = _lco.Grade;
            lblSubject.Text = _lco.ProgramArea;
            lblCourse.Text = _lco.Course;
            lblCourseNumber.Text = _lco.CourseNumber;
            lblAuthor.Text = _lco.IMCname;
            lblLEA.Text = _lco.LEAName;
            lblDateCreated.Text = DataIntegrity.ConvertToDate(_lco.DateCreated).ToShortDateString();
            lblDateApproved.Text = (_lco.IsApproved ? DataIntegrity.ConvertToDate(_lco.DateApproved).ToShortDateString() : "Pending");
          //  lblImplementationDate.Text = _lco.ImplementationYear;
        }
    }
}