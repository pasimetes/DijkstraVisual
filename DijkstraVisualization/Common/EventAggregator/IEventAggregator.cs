namespace DijkstraVisualization.Common
{
    public interface IEventAggregator
    {
        void Publish<TEventType>(TEventType eventToPublish);

        void Subsribe(object subscriber);
    }
}