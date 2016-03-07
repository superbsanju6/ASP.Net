using System;
using System.Collections.Generic;
using System.Web.UI;
using Thinkgate.Base.Enums;
using Thinkgate.Controls;

namespace Thinkgate.Classes
{
    public class RecordPage : BasePage
    {
        #region Variables

        public List<Folder> Folders;
        public List<Tile> Rotator1Tiles = new List<Tile>();
        public List<Tile> Rotator2Tiles = new List<Tile>();
        public Folders FoldersControl;
        private DoubleScrollPanel _doublePanelControl;
        private Interfaces.IRotatorControl _doubleCarousel;

        #endregion

        #region Constants

        private const string DEFAULT_RESOURCE_NAME = "thinkgate.resource";
        private const string TILE_MODEL_CURRICULUM_TITLE_DEFAULT_VALUE = "Model Curriculum Unit";
        private const string TILE_INSTRUCTIONAL_PLAN_TITLE_DEFAULT_VALUE = "Instructional Plan";
        private const string TILE_UNIT_PLAN_TITLE_DEFAULT_VALUE = "Unit Plan";
        private const string TILE_LESSON_PLAN_TITLE_DEFAULT_VALUE = "Lesson Plan";
        private const string TILE_RESOURCES_TITLE_DEFAULT_VALUE = "Resources";
        private const string TILE_RTI_FORM_TITLE_DEFAULT_VALUE = "RTI Forms";
        private const string TILE_RTI_ANALYSIS_TITLE_DEFAULT_VALUE = "RTI Analysis";
        private const string TILE_RTI_INTERVENTIONS_TITLE_DEFAULT_VALUE = "RTI Interventions";

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the base initialization for any child of RecordPage and handles calling LoadRecordObject()
        /// which loads the child objects data object and puts it into a shared Cahce so the child page can access it
        /// when needed.
        /// </summary>
        /// <param name="folders">Honestly, this parameter should not be passed into the base class.  It needs
        /// to be refactored so that it stays in the child classes.</param>
        /// <param name="doubleScrollPanel">Honestly, this parameter should not be passed into the base class.  It needs
        /// to be refactored so that it stays in the child classes.</param>
        /// <param name="sender">Object that started this event chain.  It is passed down to BasePages Page_Init eventhandler</param>
        /// <param name="e">EventArgs passed down to BasePages Page_Init eventhandler</param>
        public void InitPage(Folders folders, DoubleScrollPanel doubleScrollPanel, object sender, EventArgs e)
        {
            FoldersControl = folders;
            _doublePanelControl = doubleScrollPanel;
            Page_Init(sender, e);
            LoadRecordObject();
        }

        #endregion

        #region Protected

        /// <summary>
        /// This needs to be broken up into two methods.  Currently it identifies which folder
        /// needs to have tiles loaded into it, then calls the methods to load the tiles into that Folder.
        /// It should be broken out into two methods, one that identifies the folder and another that
        /// loads the tiles
        /// </summary>
        protected void LoadDefaultFolderTiles()
        {
            var folderText = Request.QueryString["folder"] ?? string.Empty;
            var folder = Folders.Find(t => t.Text == folderText);

            if (Folders != null && Folders.Count > 0 && folder == null)
            {
                SessionObject.Elements_ActiveFolder = Folders[0].Text;
                ReloadTilesControl(Folders[0].Text);
                FoldersControl.HighlightFolder(Folders[0].Text);
            }
            else if (!string.IsNullOrEmpty(folderText))
            {
                ReloadTilesControl(folderText);
                FoldersControl.HighlightFolder(folderText);
            }
        }

        /// <summary>
        /// Reloads all tiles for a given "section" i.e. Folder
        /// </summary>
        /// <param name="section"></param>
        protected void ReloadTilesControl(string section)
        {
            Reload(section, _doublePanelControl);
        }

        /// <summary>
        /// Loads Tiles for the active folder that the current user has permission to view.
        /// </summary>
        /// <param name="ctlRotator">The rotator control to load tiles into</param>
        /// <param name="rotatorIndex">The index to load the tiles into on the rotator control</param>
        /// <param name="containerControlPath">The path of the container to load tiles into</param>
        /// <param name="numberTilesPerPage">The number of tiles to display within the container</param>
        /// <param name="tiles">The list of tiles to load into the container</param>
        protected void LoadTilesAndContainer(Interfaces.IRotatorControl ctlRotator,
                int rotatorIndex, string containerControlPath, int numberTilesPerPage,
                List<Tile> tiles)
        {
            var counter = 0;
            var containerCount = 0;
            var chunk = new List<Tile>();

            for (int i = 0; i < tiles.Count; i++)
            {
                // check permission before adding to chunk so prevent tile from taking up space and allowing other tiles to slide left into place
                if (tiles[i].TilePermission != null)
                {
                    var permission = tiles[i].TilePermission;

                    if (SessionObject == null)
                    {
                        SessionObject = (SessionObject)Session["SessionObject"];
                    }

                    var isTrue = SessionObject.LoggedInUser.HasPermission((Permission)permission);

                    if (!isTrue) continue;
                }

                chunk.Add(tiles[i]);
                counter++;

                if (counter == numberTilesPerPage)
                {
                    LoadContainer(ctlRotator, rotatorIndex, containerControlPath, chunk, numberTilesPerPage, containerCount);
                    counter = 0;
                    chunk = new List<Tile>();
                }

                containerCount++;
            }

            if (chunk.Count > 0)
                LoadContainer(ctlRotator, rotatorIndex, containerControlPath, chunk, numberTilesPerPage, containerCount);
        }

        /// <summary>
        /// Loads individual tiles into their container
        /// </summary>
        /// <param name="ctlRotator">The paging control to display the tile in</param>
        /// <param name="rotatorIndex">The index of the Rotator to insert the tile into</param>
        /// <param name="containerControlPath">The path of the control to load tiles into</param>
        /// <param name="chunk">The list of Tiles to be added into the RotatorControl</param>
        /// <param name="numberTilesPerPage">The number of Tiles to display on a given page</param>
        /// <param name="counter">Used to set the ID of the rotator panel</param>
        protected void LoadContainer(Interfaces.IRotatorControl ctlRotator, int rotatorIndex, string containerControlPath, List<Tile> chunk, int numberTilesPerPage, int counter)
        {
            var container = (Container)LoadControl(containerControlPath);
            container.NumberOfTilesPerPage = numberTilesPerPage;
            container.ID = "rotator" + rotatorIndex + "_container" + counter;
            container.ClientIDMode = ClientIDMode.Static;
            container.ContainerID = "container" + rotatorIndex;
            container.OnTileSort += TileSort;
            container.BuildDockZone();
            container.LoadControlsToContent(chunk);

            if (rotatorIndex == 1)
                ctlRotator.AddItemToRotator1(container);
            else if (rotatorIndex == 2)
                ctlRotator.AddItemToRotator2(container);
        }

        #region Get Tile Titles

        /// <summary>
        /// Returns the Title text displayed on the RTIFormTile
        /// </summary>
        /// <returns>The Tile_TRI_Form_Title property of our DistrictParms object if it has a value; 
        /// TileRtiFormTitleDefaultValue otherwise</returns>
        protected string GetRtiFormTileTitle()
        {
            var rTiFormParmValue = DistrictParms.Tile_RTI_Form_Title;
            return string.IsNullOrWhiteSpace(rTiFormParmValue) ? TILE_RTI_FORM_TITLE_DEFAULT_VALUE : rTiFormParmValue;
        }

        /// <summary>
        /// Returns the Title text displayed on the TRIAnalysisTile
        /// </summary>
        /// <returns>The Tile_RTI_Analysis_Title property of our DistrictParms object if it has a value; 
        /// TileRtiAnalysisTitleDefaultValue otherwise</returns>
        protected string GetRtiAnalysisTileTitle()
        {
            var rTiAnalysisParmValue = DistrictParms.Tile_RTI_Analysis_Title;
            return string.IsNullOrWhiteSpace(rTiAnalysisParmValue) ? TILE_RTI_ANALYSIS_TITLE_DEFAULT_VALUE : rTiAnalysisParmValue;
        }

        /// <summary>
        /// Returns the Title text displayed on the TRIInterventionsTile
        /// </summary>
        /// <returns>The Tile_RTI_Interventions_Title property of our DistrictParms object if it has a value; 
        /// TileRtiInterventionsTitleDefaultValue otherwise</returns>
        protected string GetRtiInterventionsTileTitle()
        {
            var rTiInterventionsParmValue = DistrictParms.Tile_RTI_Analysis_Title;
            return string.IsNullOrWhiteSpace(rTiInterventionsParmValue) ? TILE_RTI_INTERVENTIONS_TITLE_DEFAULT_VALUE : rTiInterventionsParmValue;
        }

        /// <summary>
        /// Returns the Title text displayed on the ModelCirriculum Tile
        /// </summary>
        /// <returns>The Tile_Model_Curriculum_Title property of our DistrictParms object if it has a value; 
        /// TileModelCurriculumTitleDefaultValue otherwise</returns>
        protected string GetModelCurriculumTileTitle()
        {
            var modelCurriculumParmValue = DistrictParms.Tile_Model_Curriculum_Title;
            return string.IsNullOrWhiteSpace(modelCurriculumParmValue) ? TILE_MODEL_CURRICULUM_TITLE_DEFAULT_VALUE : modelCurriculumParmValue;
        }

        /// <summary>
        /// Returns the Title text displayed on the InstructionalPlan Tile
        /// </summary>
        /// <returns>The Tile_Instructional_Plan_Title property of our DistrictParms object if it has a value; 
        /// TileInstructionalPlanTitleDefaultValue otherwise</returns>
        protected string GetInstructionalPlanTileTitle()
        {
            var instructionalPlanParmValue = DistrictParms.Tile_Instructional_Plan_Title;
            return string.IsNullOrWhiteSpace(instructionalPlanParmValue) ? TILE_INSTRUCTIONAL_PLAN_TITLE_DEFAULT_VALUE : instructionalPlanParmValue;
        }

        /// <summary>
        /// Returns the Title text displayed on the Unit Plan Tile
        /// </summary>
        /// <returns>The Tile_Unit_Plan_Title property of our DistrictParms object if it has a value; 
        /// TileUnitPlanTitleDefaultValue otherwise</returns>
        protected string GetUnitPlanTileTitle()
        {
            var unitPlanParmValue = DistrictParms.Tile_Unit_Plan_Title;
            return string.IsNullOrWhiteSpace(unitPlanParmValue) ? TILE_UNIT_PLAN_TITLE_DEFAULT_VALUE : unitPlanParmValue;
        }

        /// <summary>
        /// Returns the Title text displayed on the Lesson Plan Tile
        /// </summary>
        /// <returns>The Tile_Lesson_Plan_Title property of our DistrictParms object if it has a value; 
        /// TileLessonPlanTitleDefaultValue otherwise</returns>
        protected string GetLessonPlanTileTitle()
        {
            var lessonPlanParmValue = DistrictParms.Tile_Lesson_Plan_Title;
            return string.IsNullOrWhiteSpace(lessonPlanParmValue) ? TILE_LESSON_PLAN_TITLE_DEFAULT_VALUE : lessonPlanParmValue;
        }

        /// <summary>
        /// Returns the Title text displayed on the Resource Tile
        /// </summary>
        /// <returns>The Tile_Resources_Title property of our DistrictParms object if it has a value; 
        /// TileResourcesTitleDefaultValue otherwise</returns>
        protected string GetResourceTileTitle()
        {
            var resourcesParmValue = DistrictParms.Tile_Resources_Title;
            return string.IsNullOrWhiteSpace(resourcesParmValue) ? TILE_RESOURCES_TITLE_DEFAULT_VALUE : resourcesParmValue;
        }

        /// <summary>
        /// Returns Resource name of a Kentico Resource Tile
        /// </summary>
        /// <returns>DefaultResourceName</returns>
        protected string GetKenticoResourceTileResourceTypeName()
        {
            return DEFAULT_RESOURCE_NAME;
        }

        #endregion

        #endregion

        #region Private

        //TODO:  I think we can get rid of this method completly, it seems to serve no purpose.  Only reason I am not pulling it out now is because of the build tonight
        /// <summary>
        /// The purpose of this method is unclear, I think It should be removed
        /// </summary>
        /// <param name="div">I believe this method should be removed from the code</param>
        private void CatalogRotatorTiles(Control div)
        {
            if (div != null)
            {
                List<Tile> tiles = new List<Tile>();

                foreach (Control i in div.Controls)
                {
                    try
                    {
                        if (i == null)
                        {
                            continue;
                        }

                        var list = ((Container)i).GetTiles();
                        if (list == null)
                        {
                            return;
                        }

                        tiles.AddRange(list);
                    }
                    catch (Exception)
                    {
                        // TODO: Handle errors on sort drop
                    }
                }
            }
        }

        /// <summary>
        /// Reloads all tiles a given "Folder" and a given rotator control in that Folder
        /// </summary>
        /// <param name="section">The name of the Folder to Reload tiles for</param>
        /// <param name="ctlRotator">The RotatorControl inside of the folder to reload tiles for</param>
        private void Reload(string section, Interfaces.IRotatorControl ctlRotator)
        {
            SessionObject.Elements_ActiveFolder = section;
            _doubleCarousel = ctlRotator;

            Rotator1Tiles.Clear();
            Rotator2Tiles.Clear();
            ctlRotator.ClearRotators();

            //Find folder in folderList
            var folder = Folders.Find(t => t.Text == section);
            if (folder == null) return;
            folder.LoadTilesMethod();

            ctlRotator.ChangeCarouselTwoVisibility(folder.DoubleRotator);

            LoadTilesAndContainer(ctlRotator, 1, folder.Container1Path, folder.NumberOfTiles1, Rotator1Tiles);

            if (folder.DoubleRotator)
                LoadTilesAndContainer(ctlRotator, 2, folder.Container2Path, folder.NumberOfTiles1, Rotator2Tiles);

            ctlRotator.AddNavigationButtons(folder);
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Eventhandler that takes care of sorting the Tiles displayed on our child RecordPages
        /// </summary>
        /// <param name="sender">The sending object that originated this event</param>
        /// <param name="e">EventArgs passed to the Eventhandler</param>
        protected void TileSort(object sender, EventArgs e)
        {
            if (_doubleCarousel != null)
            {
                CatalogRotatorTiles(_doubleCarousel.GetRotator1());
                CatalogRotatorTiles(_doubleCarousel.GetRotator2());
            }
        }

        #endregion
    }
}
