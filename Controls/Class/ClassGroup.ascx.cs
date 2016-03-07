using System;
using System.Data;
using System.Web.UI.WebControls;
using Thinkgate.Classes;
using System.Collections.Generic;

namespace Thinkgate.Controls.Class
{
    public partial class ClassGroup : TileControlBase
    {        
        //public event EventHandler PanelClickEventHandler;

        //Parms Dictionary includes: classes

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Tile == null) return;

            if (Tile.TileParms.GetParm("classes") != null) BindCheckBoxList();
        }

        private void BindCheckBoxList()
        {
            List<string> c = new List<string>();
            string grade;

            var classes = (List<Thinkgate.Base.Classes.Class>) Tile.TileParms.GetParm("classes");

            foreach (Thinkgate.Base.Classes.Class classInstance in classes)
            {
                grade = Standpoint.Core.Utilities.NumericHelpers.GetOrdinal(Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(classInstance.Grade.DisplayText));
                var text = ("   " + grade + " Grade " + classInstance.ClassName + " - Period " + classInstance.Period);
                chkClasses.Items.Add(new ListItem(text, classInstance.ID.ToString()));
            }
        }
    }
}