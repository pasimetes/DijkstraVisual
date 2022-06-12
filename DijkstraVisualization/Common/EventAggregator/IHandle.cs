namespace DijkstraVisualization.Common
{
    public interface IHandle<TEventType>
    {
        void Handle(TEventType message);
    }
}
