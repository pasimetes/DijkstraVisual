namespace DijkstraVisualization.Events
{
    public class OnCostEditModeChangedMessage
    {
        public OnCostEditModeChangedMessage(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; }
    }
}
