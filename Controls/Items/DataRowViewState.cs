namespace Thinkgate.Controls.Items
{
    public class DataRowViewState
    {
        public LabelState DescriptionLabelState { get; set; }
        public LabelState DistractorLabelState { get; set; }
        public LabelState ValueLableState { get; set; }

        public DataRowViewState()
        {
            DescriptionLabelState = new LabelState();
            DistractorLabelState = new LabelState();
            ValueLableState = new LabelState();
        }
    }
}