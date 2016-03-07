using System;

namespace Thinkgate.Classes
{
    public delegate void LoadTilesMethod();

    public class Folder
    {
        public string Text { get; set; }
        public string Icon { get; set; }        
        public string Container1Path { get; set; }
        public string Container2Path { get; set; }
        public string Container1Label { get; set; }
        public string Container2Label { get; set; }
        public int NumberOfTiles1 { get; set; }
        public int NumberOfTiles2 { get; set; }
        public LoadTilesMethod LoadTilesMethod;

        public bool DoubleRotator { get { return NumberOfTiles2 > 0; } }

        public Folder()
        { }

        public Folder(string text, string icon, LoadTilesMethod loadTilesMethod,
                string container1Path, int numberOfTiles1, string container2Path = "", 
                int numberOfTiles2 = 0, string container1Label = "", string container2Label = "")
        {
            Text = text;
            Icon = icon;
            LoadTilesMethod = loadTilesMethod;
            Container1Path = container1Path;
            NumberOfTiles1 = numberOfTiles1;
            NumberOfTiles2 = numberOfTiles2;
            Container2Path = container2Path;
            Container1Label = container1Label;
            Container2Label = container2Label;
        }
    }
}