using DijkstraVisualization.Common;
using DijkstraVisualization.Events;
using DijkstraVisualization.Models;
using DijkstraVisualization.Util;
using DijkstraVisualization.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DijkstraVisualization.Views
{
    public partial class GraphView : UserControl,
        IHandle<OnDeleteVertexMessage>,
        IHandle<OnConnectVertexMessage>,
        IHandle<OnCancelConnectMessage>,
        IHandle<OnHoverVertex>,
        IHandle<OnLeaveVertex>,
        IHandle<OnHoverCostLabel>,
        IHandle<OnLeaveCostLabel>,
        IHandle<OnRemoveEdgeMessage>,
        IHandle<OnLoadGraphMessage>,
        IHandle<OnCompletedDijkstraMessage>,
        IHandle<OnResetMessage>,
        IHandle<OnFindShortestRouteMessage>
    {
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                nameof(Rows),
                typeof(int),
                typeof(GraphView),
                new PropertyMetadata());

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                nameof(Columns),
                typeof(int),
                typeof(GraphView),
                new PropertyMetadata());

        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register(
                nameof(CellSize),
                typeof(int),
                typeof(GraphView),
                new PropertyMetadata());

        public int CellSize
        {
            get => (int)GetValue(CellSizeProperty);
            set => SetValue(CellSizeProperty, value);
        }

        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        private bool _hasSource;
        private bool _vertexMenuOpen;
        private string _connectingVertexId;
        private IDictionary<string, List<string>> _recentVisitLog;
        private IDictionary<string, int> _recentDistances;
        private IDictionary<string, string> _recentPaths;

        protected readonly GraphViewModel Context;

        public GraphView()
        {
            EventAggregator.Instance.Subsribe(this);
            DataContext = Context = new GraphViewModel();
            Connections = new ObservableCollection<ConnectionArrow>();
            CostLabels = new ObservableCollection<CostLabelView>();

            InitializeComponent();
        }

        public ObservableCollection<ConnectionArrow> Connections { get; }

        public ObservableCollection<CostLabelView> CostLabels { get; }

        public void Handle(OnDeleteVertexMessage message)
        {
            var elementToRemove = DrawCanvas.Children
                .OfType<VertexView>()
                .Single(v => v.Id == message.Id);

            RemoveConnectionArrows(elementToRemove);
            RemoveCostLabels(elementToRemove);

            DrawCanvas.Children.Remove(elementToRemove);

            for (int i = 0; i < Connections.Count; i++)
                if (Connections[i].Source == elementToRemove || Connections[i].Destination == elementToRemove)
                {
                    Connections.Remove(Connections[i]);
                    i--;
                }
        }

        public void Handle(OnConnectVertexMessage message)
        {
            var source = GetSource();
            if (source != null)
            {
                _hasSource = true;
                CreateTempArrow(source);
            }

            if (string.IsNullOrEmpty(_connectingVertexId) || message.Id == _connectingVertexId)
            {
                _connectingVertexId = message.Id;
                return;
            }

            var destination = TryGetVertexById(message.Id);
            CreatePersistentArrow(source, destination, 1);

            EventAggregator.Instance.Publish(
                new OnCreateEdgeMessage(
                    _connectingVertexId,
                    message.Id,
                    1));

            _hasSource = false;
            _connectingVertexId = default;

            ConnectionArrow.Visibility = Visibility.Collapsed;
        }

        public void Handle(OnCancelConnectMessage message)
        {
            _connectingVertexId = default;
            _hasSource = false;
            ConnectionArrow.Visibility = Visibility.Collapsed;
        }

        private void DrawCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_hasSource)
            {
                var pos = e.GetPosition(DrawCanvas);
                ConnectionArrow.X2 = pos.X;
                ConnectionArrow.Y2 = pos.Y;
            }
        }

        private void DrawCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _hasSource || _vertexMenuOpen)
                return;

            var position = e.GetPosition(this);
            foreach (var label in DrawCanvas.Children.OfType<CostLabelView>())
            {
                var relativeLocation = label.TranslatePoint(new Point(0, 0), DrawCanvas);
                if (position.X > relativeLocation.X && position.X <= relativeLocation.X + label.ActualWidth &&
                    position.Y > relativeLocation.Y && position.Y <= relativeLocation.Y + label.ActualHeight)
                {
                    return;
                }
            }

            var location = GetCellLocation();
            CreateVertex(RandomString(4), location.Row, location.Column);
        }

        private void CreateVertex(string id, int row, int column)
        {
            if (!HasEnoughSpace(row, column))
                return;

            CreateVertexUIElement(id, row, column);

            EventAggregator.Instance.Publish(
                new OnCreateVertexMessage(
                    id,
                    row,
                    column));
        }

        private void CreateVertexUIElement(string id, int row, int column)
        {
            var element = new VertexView(id, row, column)
            {
                Width = CellSize,
                Height = CellSize
            };

            Canvas.SetTop(element, row * CellSize);
            Canvas.SetLeft(element, column * CellSize);

            DrawCanvas.Children.Add(element);
        }

        private bool HasEnoughSpace(int row, int column)
        {
            var vertices = DrawCanvas.Children
                .OfType<VertexView>()
                .Select(v => v.DataContext)
                .Cast<dynamic>();

            if (vertices.Any(n => n.Row == row && n.Column == column) ||
                vertices.Any(n => n.Row + 1 == row && n.Column == column) ||
                vertices.Any(n => n.Row == row && n.Column + 1 == column) ||
                vertices.Any(n => n.Row - 1 == row && n.Column == column) ||
                vertices.Any(n => n.Row == row && n.Column - 1 == column))
                return false;

            return true;
        }

        private Location GetCellLocation()
        {
            var position = Mouse.GetPosition(DrawCanvas);
            var row = Math.Min((int)(position.Y / CellSize), Rows - 1);
            var column = Math.Min((int)(position.X / CellSize), Columns - 1);

            return new Location
            {
                Row = row,
                Column = column
            };
        }

        private VertexView GetSource()
        {
            return DrawCanvas.Children
                .OfType<VertexView>()
                .SingleOrDefault(v => v.IsConnector);
        }

        public VertexView TryGetVertexById(string id)
        {
            return DrawCanvas.Children
                .OfType<VertexView>()
                .SingleOrDefault(v => v.Id == id);
        }

        private void CreateTempArrow(VertexView fromVertex)
        {
            var relativeLocation = fromVertex.TranslatePoint(new Point(0, 0), DrawCanvas);
            ConnectionArrow.X1 = relativeLocation.X + CellSize / 2;
            ConnectionArrow.Y1 = relativeLocation.Y + CellSize / 2;
            ConnectionArrow.X2 = relativeLocation.X + CellSize / 2;
            ConnectionArrow.Y2 = relativeLocation.Y + CellSize / 2;
            ConnectionArrow.Visibility = Visibility.Visible;
        }

        private void CreatePersistentArrow(VertexView fromVertex, VertexView toVertex, int cost)
        {
            var destinationPosition = new Point(toVertex.Column * CellSize, toVertex.Row * CellSize);
            var sourcePosition = new Point(fromVertex.Column * CellSize, fromVertex.Row * CellSize);
            sourcePosition.X += CellSize / 2;
            sourcePosition.Y += CellSize / 2;

            var endingPoint = new Point(destinationPosition.X + CellSize / 2, destinationPosition.Y + CellSize / 2);

            var finalDestinationPoint = GeometryHelper.CalculatePoint(
                sourcePosition,
                endingPoint,
                GeometryHelper.GetDistance(sourcePosition, endingPoint) - CellSize / 2);

            var finalSourcePoint = GeometryHelper.CalculatePoint(
                endingPoint,
                sourcePosition,
                GeometryHelper.GetDistance(endingPoint, sourcePosition) - CellSize / 2);

            var arrowToPersist = new Line
            {
                X1 = finalSourcePoint.X,
                Y1 = finalSourcePoint.Y,
                X2 = finalDestinationPoint.X,
                Y2 = finalDestinationPoint.Y,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = ConnectionArrow.StrokeThickness,
                IsHitTestVisible = false
            };

            var connection = new ConnectionArrow(
                arrowToPersist,
                fromVertex,
                toVertex);

            Connections.Add(connection);
            DrawCanvas.Children.Insert(0, connection.Line);

            var center = new Point(
                (finalSourcePoint.X + finalDestinationPoint.X) / 2,
                (finalSourcePoint.Y + finalDestinationPoint.Y) / 2);

            var costLabel = new CostLabelView(fromVertex, toVertex, cost);
            costLabel.Measure(DrawCanvas.RenderSize);

            Canvas.SetLeft(costLabel, center.X - costLabel.DesiredSize.Width / 2);
            Canvas.SetTop(costLabel, center.Y - costLabel.DesiredSize.Height / 2);

            CostLabels.Add(costLabel);
            DrawCanvas.Children.Insert(DrawCanvas.Children.OfType<Line>().Count(), costLabel);
        }

        private void RemoveConnectionArrows(VertexView fromVertex)
        {
            foreach (var arrowToRemove in Connections)
                if (arrowToRemove.Source == fromVertex || arrowToRemove.Destination == fromVertex)
                {
                    DrawCanvas.Children.Remove(arrowToRemove.Line);
                }
        }

        private void RemoveCostLabels(VertexView fromVertex)
        {
            foreach (var label in CostLabels)
                if (label.Source == fromVertex || label.Destination == fromVertex)
                {
                    DrawCanvas.Children.Remove(label);
                }
        }

        private static string RandomString(int length)
        {
            var chars = "qwertyuiopasdfghjklzxcvbnm0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[new Random(Guid.NewGuid().GetHashCode()).Next(s.Length)]).ToArray());
        }

        public void Handle(OnHoverVertex message)
        {
            if (_hasSource)
                return;

            _vertexMenuOpen = true;

            var hoveredVertex = TryGetVertexById(message.Id);

            Panel.SetZIndex(hoveredVertex, 1);

            if (!Connections.Any(c => c.Source.Id == message.Id || c.Destination.Id == message.Id))
                return;

            var whiteList = new List<FrameworkElement>();

            foreach (var connection in Connections.Where(c => c.Source == hoveredVertex || c.Destination == hoveredVertex))
            {
                whiteList.Add(connection.Source);
                whiteList.Add(connection.Destination);
                whiteList.Add(connection.Line);
            }

            foreach (var label in CostLabels.Where(c => c.Source == hoveredVertex || c.Destination == hoveredVertex))
                whiteList.Add(label);

            BlurElements(whiteList);
        }

        public void Handle(OnLeaveVertex message)
        {
            _vertexMenuOpen = false;

            var hoveredVertex = TryGetVertexById(message.Id);
            if (hoveredVertex != null)
                Panel.SetZIndex(hoveredVertex, 0);

            ResetElements();
            ClearHighlightedPath();
        }

        public void Handle(OnHoverCostLabel message)
        {
            if (_hasSource)
                return;

            var sourceVertex = TryGetVertexById(message.SourceId);
            var destinationVertex = TryGetVertexById(message.DestinationId);

            var connection = Connections.Where(c
                => (c.Source == sourceVertex && c.Destination == destinationVertex)
                || (c.Source == destinationVertex && c.Destination == sourceVertex))
                .Single();

            var label = CostLabels.Where(c
                => (c.Source == sourceVertex && c.Destination == destinationVertex)
                || (c.Source == destinationVertex && c.Destination == sourceVertex))
                .Single();

            var whiteList = new List<FrameworkElement>();
            whiteList.Add(sourceVertex);
            whiteList.Add(destinationVertex);
            whiteList.Add(connection.Line);
            whiteList.Add(label);

            BlurElements(whiteList);
        }

        public void Handle(OnLeaveCostLabel message)
        {
            ResetElements();
        }

        public void Handle(OnRemoveEdgeMessage message)
        {
            var connection = Connections.Single(c
                => c.Source.Id == message.SourceId
                && c.Destination.Id == message.DestinationId);

            var label = CostLabels.Single(l
                => l.Source.Id == message.SourceId
                && l.Destination.Id == message.DestinationId);

            Connections.Remove(connection);
            CostLabels.Remove(label);
            DrawCanvas.Children.Remove(connection.Line);
            DrawCanvas.Children.Remove(label);
        }

        private void BlurElements(IEnumerable<FrameworkElement> whiteList)
        {
            foreach (var element in DrawCanvas.Children.OfType<FrameworkElement>())
            {
                if (whiteList.Contains(element))
                    continue;

                var animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0.15,
                    Duration = TimeSpan.FromSeconds(0.4),
                };
                element.IsHitTestVisible = false;
                element.BeginAnimation(FrameworkElement.OpacityProperty, animation);
            }
        }

        private void ResetElements()
        {
            foreach (var element in DrawCanvas.Children.OfType<FrameworkElement>())
            {
                var animation = new DoubleAnimation
                {
                    From = element.Opacity,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.4),
                };
                element.IsHitTestVisible = true;
                element.BeginAnimation(FrameworkElement.OpacityProperty, animation);
            }
        }

        public void Handle(OnLoadGraphMessage message)
        {
            foreach (var vertex in message.Data.Vertices)
                CreateVertex(vertex.Id, vertex.Row, vertex.Column);

            foreach (var src in message.Data.AdjacencyList)
            {
                var srcVertex = TryGetVertexById(src.Key);
                foreach (var dest in src.Value)
                {
                    var destinationVertex = TryGetVertexById(dest.Item1);

                    if (Connections.Any(c
                        => (c.Source == srcVertex && c.Destination == destinationVertex)
                        || (c.Source == destinationVertex && c.Destination == srcVertex)))
                    {
                        continue;
                    }

                    CreatePersistentArrow(srcVertex, destinationVertex, dest.Item2);

                    EventAggregator.Instance.Publish(
                        new OnCreateEdgeMessage(
                            srcVertex.Id,
                            destinationVertex.Id,
                            dest.Item2));
                }
            }
        }

        public async void Handle(OnCompletedDijkstraMessage message)
        {
            _recentVisitLog = message.Result.VisitLog;
            _recentDistances = message.Result.Distances;
            _recentPaths = message.Result.Paths;

            var src = TryGetVertexById(message.Result.VisitLog.Keys.First());
            src.IsSource = true;

            EventAggregator.Instance.Publish(new OnStartedAnimationMessage());

            foreach (var kvp in message.Result.VisitLog)
            {
                var subject = TryGetVertexById(kvp.Key);
                subject.IsSubject = true;

                await Task.Delay(150);

                foreach (var vertexId in kvp.Value)
                {
                    var visitingVertex = TryGetVertexById(vertexId);
                    //if (visitingVertex.IsVisited)
                    //    continue;

                    visitingVertex.IsVisiting = true;

                    var connection = Connections.Single(c
                        => (c.Source == subject && c.Destination == visitingVertex)
                        || (c.Destination == subject && c.Source == visitingVertex));

                    connection.Line.StrokeThickness = 3.5;
                    connection.Line.Stroke = new SolidColorBrush(Colors.Lime);

                    await Task.Delay(100);

                    visitingVertex.IsVisiting = false;
                    connection.Line.StrokeThickness = 0.6;
                    connection.Line.Stroke = new SolidColorBrush(Colors.Black);
                }

                subject.IsSubject = false;
                subject.IsVisited = true;
            }

            EventAggregator.Instance.Publish(new OnCompletedAnimationMessage());
        }

        public void Handle(OnResetMessage message)
        {
            foreach (var vertex in DrawCanvas.Children.OfType<VertexView>())
            {
                vertex.IsVisited = false;
                vertex.IsSource = false;
            }
        }

        public void Handle(OnFindShortestRouteMessage message)
        {
            var sourceVertex = DrawCanvas.Children.OfType<VertexView>().Single(v => v.IsSource);
            var destinationVertex = TryGetVertexById(message.DestinationId);
            var elementsToHighlight = new List<VertexView>
            {
                destinationVertex,
            };

            var currentId = destinationVertex.Id;
            while (currentId != sourceVertex.Id)
            {
                var previousVertex = _recentPaths[currentId];
                elementsToHighlight.Add(TryGetVertexById(previousVertex));
                currentId = previousVertex;
            }

            Debug.WriteLine(_recentDistances[destinationVertex.Id]);

            var whitelist = new List<FrameworkElement>();

            for (int i = 0; i < elementsToHighlight.Count - 1; i++)
            {
                var from = TryGetVertexById(elementsToHighlight[i].Id);
                var to = TryGetVertexById(elementsToHighlight[i + 1].Id);

                var connection = Connections.Where(c
                    => (c.Source == from && c.Destination == to)
                    || (c.Source == to && c.Destination == from))
                    .Single();

                whitelist.Add(connection.Source);
                whitelist.Add(connection.Destination);
                whitelist.Add(connection.Line);

                var label = CostLabels.Single(l
                    => (l.Source.Id == from.Id
                    && l.Destination.Id == to.Id)
                    || (l.Source.Id == to.Id
                    && l.Destination.Id == from.Id));

                whitelist.Add(label);

                //connection.Line.Stroke = new SolidColorBrush(Colors.Yellow);

                //if (connection.Source != sourceVertex)
                //{
                connection.Source.IsMarked = true;
                connection.Destination.IsMarked = true;
                //}

                var animation = new DoubleAnimation
                {
                    From = connection.Line.StrokeThickness,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.4),
                };
                connection.Line.BeginAnimation(Line.StrokeThicknessProperty, animation);
            }

            BlurElements(whitelist);
        }

        private void ClearHighlightedPath()
        {
            foreach (var connectionLine in Connections)
            {
                connectionLine.Line.Stroke = new SolidColorBrush(Colors.Black);

                connectionLine.Source.IsMarked = false;
                connectionLine.Destination.IsMarked = false;

                var animation = new DoubleAnimation
                {
                    From = connectionLine.Line.StrokeThickness,
                    To = 0.6,
                    Duration = TimeSpan.FromSeconds(0.2),
                };
                connectionLine.Line.BeginAnimation(Line.StrokeThicknessProperty, animation);
            }
        }
    }
}
