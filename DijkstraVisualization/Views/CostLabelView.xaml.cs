using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DijkstraVisualization.Views
{
    public partial class CostLabelView : UserControl,
        IHandle<OnConnectVertexMessage>,
        IHandle<OnCancelConnectMessage>,
        IHandle<OnCreateEdgeMessage>,
        IHandle<OnStartedAnimationMessage>,
        IHandle<OnResetMessage>
    {
        public static readonly DependencyProperty IsHoveredProperty =
            DependencyProperty.Register(
                nameof(IsHovered),
                typeof(bool),
                typeof(CostLabelView),
                new PropertyMetadata());

        public bool IsHovered
        {
            get => (bool)GetValue(IsHoveredProperty);
            set => SetValue(IsHoveredProperty, value);
        }

        private readonly CostLabelViewModel _context;

        private bool _canInteract;

        public CostLabelView(VertexView source, VertexView destination, int cost)
        {
            EventAggregator.Instance.Subsribe(this);

            Source = source;
            Destination = destination;
            DataContext = _context = new CostLabelViewModel(source.Id, destination.Id, cost);
            InitializeComponent();
        }

        public VertexView Source { get; }

        public VertexView Destination { get; }

        public void Handle(OnConnectVertexMessage message)
        {
            _canInteract = false;
        }

        public void Handle(OnCancelConnectMessage message)
        {
            _canInteract = true;
        }

        public void Handle(OnCreateEdgeMessage message)
        {
            _canInteract = true;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_canInteract)
                return;

            IsHovered = true;
            EventAggregator.Instance.Publish(new OnHoverCostLabel(Source.Id, Destination.Id));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_canInteract)
                return;

            IsHovered = false;
            EventAggregator.Instance.Publish(new OnLeaveCostLabel());
        }

        public void Handle(OnStartedAnimationMessage message)
        {
            _canInteract = false;
        }

        public void Handle(OnResetMessage message)
        {
            _canInteract = true;
        }
    }
}
