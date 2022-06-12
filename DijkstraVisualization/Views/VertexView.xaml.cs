using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.Misc;
using DijkstraVisualization.Models.Enums;
using DijkstraVisualization.Models.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DijkstraVisualization.Views
{
    public partial class VertexView : UserControl,
        IHandle<OnConnectVertexMessage>,
        IHandle<OnCreateEdgeMessage>,
        IHandle<OnCancelConnectMessage>,
        IHandle<OnStartedAnimationMessage>,
        IHandle<OnCompletedAnimationMessage>,
        IHandle<OnResetMessage>
    {
        public static readonly DependencyProperty IsHoveredProperty =
            DependencyProperty.Register(
                nameof(IsHovered),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public static readonly DependencyProperty IsConnectorProperty =
            DependencyProperty.Register(
                nameof(IsConnector),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public static readonly DependencyProperty IsSubjectProperty =
            DependencyProperty.Register(
                nameof(IsSubject),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public static readonly DependencyProperty IsVisitingProperty =
            DependencyProperty.Register(
                nameof(IsVisiting),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public static readonly DependencyProperty IsVisitedProperty =
            DependencyProperty.Register(
                nameof(IsVisited),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public static readonly DependencyProperty HasConnectorProperty =
            DependencyProperty.Register(
                nameof(HasConnector),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public static readonly DependencyProperty IsSourceProperty =
            DependencyProperty.Register(
                nameof(IsSource),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public static readonly DependencyProperty IsInPathProperty =
            DependencyProperty.Register(
                nameof(IsInPath),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public static readonly DependencyProperty IsMarkedProperty =
            DependencyProperty.Register(
                nameof(IsMarked),
                typeof(bool),
                typeof(VertexView),
                new PropertyMetadata());

        public bool IsMarked
        {
            get => (bool)GetValue(IsMarkedProperty);
            set => SetValue(IsMarkedProperty, value);
        }

        public bool IsInPath
        {
            get => (bool)GetValue(IsInPathProperty);
            set => SetValue(IsInPathProperty, value);
        }

        public bool IsSource
        {
            get => (bool)GetValue(IsSourceProperty);
            set => SetValue(IsSourceProperty, value);
        }

        public bool IsVisiting
        {
            get => (bool)GetValue(IsVisitingProperty);
            set => SetValue(IsVisitingProperty, value);
        }

        public bool IsVisited
        {
            get => (bool)GetValue(IsVisitedProperty);
            set => SetValue(IsVisitedProperty, value);
        }

        public bool IsSubject
        {
            get => (bool)GetValue(IsSubjectProperty);
            set => SetValue(IsSubjectProperty, value);
        }

        public bool IsHovered
        {
            get => (bool)GetValue(IsHoveredProperty);
            set => SetValue(IsHoveredProperty, value);
        }

        public bool IsConnector
        {
            get => (bool)GetValue(IsConnectorProperty);
            set => SetValue(IsConnectorProperty, value);
        }

        public bool HasConnector
        {
            get => (bool)GetValue(HasConnectorProperty);
            set => SetValue(HasConnectorProperty, value);
        }

        private readonly VertexViewModel _context;

        private IList<PieMenuItem> _algorithmChoices;
        private IList<PieMenuItem> _actionChoices;
        private bool _preventInteraction;

        public VertexView(string id, int row, int column)
        {
            EventAggregator.Instance.Subsribe(this);
            DataContext = _context = new VertexViewModel(id, row, column);
            Row = row;
            Column = column;

            InitializeComponent();
        }

        public string Id => _context.Id;

        public int Row { get; }

        public int Column { get; }

        public void Handle(OnCreateEdgeMessage message)
        {
            IsConnector = false;
            HasConnector = false;
        }

        public void Handle(OnConnectVertexMessage message)
        {
            HasConnector = true;
            VertexMenu.Visibility = Visibility.Collapsed;
        }

        public void Handle(OnCancelConnectMessage message)
        {
            IsConnector = false;
            HasConnector = false;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_preventInteraction)
            {
                EventAggregator.Instance.Publish(new OnFindShortestRouteMessage(Id));
                return;
            }

            IsHovered = true;

            EventAggregator.Instance.Publish(new OnHoverVertex(Id));

            if (!HasConnector)
            {
                VertexMenu.ClearSelection();
                VertexMenu.Visibility = Visibility.Visible;
                return;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            IsHovered = false;
            EventAggregator.Instance.Publish(new OnLeaveVertex(Id));
            VertexMenu.Visibility = Visibility.Collapsed;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_preventInteraction)
                return;

            if (HasConnector && !IsConnector)
            {
                EventAggregator.Instance.Publish(new OnConnectVertexMessage(Id));
                return;
            }

            EventAggregator.Instance.Publish(new OnCancelConnectMessage());
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            EventAggregator.Instance.Publish(new OnDeleteVertexMessage(Id));
        }

        private void OnConnectClick(object sender, RoutedEventArgs e)
        {
            IsConnector = true;
            HasConnector = true;
            EventAggregator.Instance.Publish(new OnConnectVertexMessage(Id));
        }

        private void OnRunDijkstraClick(object sender, RoutedEventArgs e)
        {
            EventAggregator.Instance.Publish(new OnRunAlgorithmMessage(Id, Algorithm.Dijkstra));
        }

        public void Handle(OnCompletedAnimationMessage message)
        {
            if (IsSource)
            {
                if (_algorithmChoices == null || !_algorithmChoices.Any())
                    _algorithmChoices = AlgorithmsMenu.Items
                        .OfType<PieMenuItem>()
                        .ToList();

                AlgorithmsMenu.Header = "Reset";
                AlgorithmsMenu.Items.Clear();
                return;
            }
            _preventInteraction = true;

            AlgorithmsMenu.Header = "Run";
        }

        public void Handle(OnStartedAnimationMessage message)
        {
            if (IsSource)
            {
                if (_algorithmChoices == null || !_algorithmChoices.Any())
                    _algorithmChoices = AlgorithmsMenu.Items
                        .OfType<PieMenuItem>()
                        .ToList();

                _actionChoices = new List<PieMenuItem>
                {
                    DeleteMenuOption,
                    ConnectMenuOption,
                    OtherMenuOption
                };

                AlgorithmsMenu.Header = "Cancel";
                AlgorithmsMenu.Items.Clear();

                VertexMenu.Items.Remove(DeleteMenuOption);
                VertexMenu.Items.Remove(ConnectMenuOption);
                VertexMenu.Items.Remove(OtherMenuOption);
                VertexMenu.MenuSector /= 3;
                VertexMenu.Rotation += 75;
                return;
            }
            _preventInteraction = true;

            AlgorithmsMenu.Header = "Run";
        }

        private void OnAlgorithmsClick(object sender, RoutedEventArgs e)
        {
            if (IsSource)
            {
                AlgorithmsMenu.Header = "Run";

                foreach (var algorithm in _algorithmChoices)
                    AlgorithmsMenu.Items.Add(algorithm);

                VertexMenu.ClearSelection();
                VertexMenu.Items.Insert(0, _actionChoices[1]);
                VertexMenu.Items.Add(_actionChoices[0]);
                VertexMenu.Items.Add(_actionChoices[2]);
                VertexMenu.MenuSector *= 3;
                VertexMenu.Rotation -= 75;
                IsSource = false;

                EventAggregator.Instance.Publish(new OnResetMessage());
            }
        }

        public void Handle(OnResetMessage message)
        {
            _preventInteraction = false;
        }
    }
}
