using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DijkstraVisualization.Views
{
    public partial class CameraView : ContentControl
    {
        private const double MaxZoom = 10f;
        private const double MinZoom = 0.5f;
        private const double Change = 0.25f;

        public static readonly DependencyProperty AreaWidthProperty =
            DependencyProperty.Register(
                nameof(AreaWidth),
                typeof(double),
                typeof(CameraView),
                new PropertyMetadata());

        public double AreaWidth
        {
            get => (double)GetValue(AreaWidthProperty);
            set => SetValue(AreaWidthProperty, value);
        }

        public static readonly DependencyProperty AreaHeightProperty =
            DependencyProperty.Register(
                nameof(AreaHeight),
                typeof(double),
                typeof(CameraView),
                new PropertyMetadata());

        public double AreaHeight
        {
            get => (double)GetValue(AreaHeightProperty);
            set => SetValue(AreaHeightProperty, value);
        }

        public static readonly DependencyProperty ZoomScaleProperty =
            DependencyProperty.Register(
                nameof(ZoomScale),
                typeof(double),
                typeof(CameraView),
                new PropertyMetadata());

        public double ZoomScale
        {
            get => (double)GetValue(ZoomScaleProperty);
            set => SetValue(ZoomScaleProperty, value);
        }

        public static readonly DependencyProperty ContentToDisplayProperty =
            DependencyProperty.Register(
                nameof(ContentToDisplay),
                typeof(object),
                typeof(CameraView),
                new PropertyMetadata());

        public object ContentToDisplay
        {
            get => GetValue(ContentToDisplayProperty);
            set => SetValue(ContentToDisplayProperty, value);
        }

        private bool _allowDrag;
        private double _offsetX;
        private double _offsetY;
        private Point _clickPosition;

        public CameraView()
        {
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed)
                return;

            var position = e.GetPosition(CameraPanel);

            _clickPosition = new Point(
                position.X - _offsetX,
                position.Y - _offsetY
                );

            _allowDrag = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_allowDrag || e.RightButton != MouseButtonState.Pressed)
                return;

            var position = e.GetPosition(CameraPanel);
            _offsetX = position.X - _clickPosition.X;
            _offsetY = position.Y - _clickPosition.Y;

            CameraCanvas.RenderTransform = new TranslateTransform(_offsetX, _offsetY);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _allowDrag = false;

        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && (ZoomScale + Change <= MaxZoom))
                ZoomScale += Change;

            else if (e.Delta < 0 && (ZoomScale - Change >= MinZoom))
                ZoomScale -= Change;
        }
    }
}
