using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Classes.Data;

namespace Thinkgate.Controls.Assessment
{
    public partial class SetPerformanceLevelCut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            SetPerformanceLevel pLevel = new SetPerformanceLevel();

            pLevel.LevelAbbr = txtLevelAbbr.Value;
            pLevel.LevelColor = txtLevelColor.Value;
            pLevel.LevelDescription = txtLevelDesc.Value;
            pLevel.LevelEquivalent = Convert.ToInt32( txtLevelEquivalent.Value);
            pLevel.LevelFlag = txtLevelFlag.Value;
            pLevel.LevelIndex = Convert.ToInt32( txtLevelIndex.Value);
            pLevel.LevelMaxScore = Convert.ToInt32(txtLevelMaxScore.Value);
            pLevel.LevelMinScore = Convert.ToInt32(txtLevelMinScore.Value);
            pLevel.LevelText = txtLevelText.Value;
            pLevel.SetDescription = setDescriptionInput.Value;

            pLevel.SetName = setNameInput.Value;
            pLevel.SetMaxScore = Convert.ToInt32(txtMaxScore.Value);
            pLevel.SetMinScore = Convert.ToInt32(txtMinScore.Value);

            pLevel.SetScoreCalc = txtSetScoreCalc.Value;

            pLevel.SetScoreType = rdoScore.Checked ? rdoScore.Value : rdoPerformance.Value;

            pLevel.ID = hdnID.Value== "" ? 0 : Convert.ToInt32( hdnID.Value);

            int PerformanceLevelID =  pLevel.AddPerformanceLevelSet();



        }
    }
}