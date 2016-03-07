using System.Drawing;

namespace Thinkgate.Controls.Items
{
    public class LabelState
    {
        public string Text { get; set; }
        public Color ForegroundColor { get; set; }

        public LabelState()
        {
            Text = string.Empty;
            ForegroundColor = Color.White;
        }
    }
}