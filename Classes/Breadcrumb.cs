namespace Thinkgate.Classes
{
    public class Breadcrumb
    {
        public string DisplayText {get; set;}
        public string ControlSection { get; set; }
        public TileParms TileParms { get; set; }

        public Breadcrumb(string displayText, string controlSection, TileParms tileParms)
        {
            DisplayText = displayText;
            ControlSection = controlSection;
            TileParms = tileParms;
        }
    }
}
