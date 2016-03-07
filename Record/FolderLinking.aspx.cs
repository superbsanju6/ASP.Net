using System;
using Thinkgate.Base.Enums;
using Thinkgate.Classes;

namespace Thinkgate.Record
{
    public partial class FolderLinking : RecordPage
    {
        protected new void Page_Init(object sender, EventArgs e)
        {
            CreateTiles();
        }

        private void CreateTiles()
        {
            if (UserHasPermission(Permission.Folder_Linking))
            {
                Rotator1Tiles.Add(new Tile(Permission.Folder_Linking, "Groups", "~/Controls/Groups/GroupSingleUser.ascx"));
            }
        }

    }
}