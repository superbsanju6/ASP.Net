using System;
using Thinkgate.Classes;
using Thinkgate.Base.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Thinkgate.Controls.Standards
{
    public partial class StandardsIdentification : TileControlBase
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadData();
        }

        private void LoadData()
        {
            var oStd = (Thinkgate.Base.Classes.Standards)Tile.TileParms.GetParm("standards");
            if (oStd == null) return;

            lblStandard_Set.Text = oStd.Subject;
            lblGrade.Text = oStd.Grade;
            lblSubject.Text = oStd.Subject;
            lblCourse.Text = oStd.Course;
            lblLevel.Text = oStd.Level;
            lblStateNbr.Text = oStd.StateNbr;
            lblKeywords.Text = oStd.Keywords;
            lblKeywords.Text = oStd.Keywords;
            lblYear.Text = oStd.Year;
        }
    }
}