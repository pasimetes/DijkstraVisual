using DijkstraVisualization.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DijkstraVisualization.Views
{
    public partial class AdjacencyMatrixView : UserControl
    {
        public static readonly DependencyProperty MatrixDataProperty =
            DependencyProperty.Register(
                nameof(MatrixData),
                typeof(ICollection<string>),
                typeof(AdjacencyMatrixView),
                new PropertyMetadata(
                    new PropertyChangedCallback((sender, arg) =>
                    {
                        var obj = (AdjacencyMatrixView)sender;
                        obj.DrawMatrix();
                    })));

        public ICollection<string> MatrixData
        {
            get => (ICollection<string>)GetValue(MatrixDataProperty);
            set => SetValue(MatrixDataProperty, value);
        }

        private readonly AdjacencyMatrixViewModel _context;

        private DrawingContext _drawing;

        public AdjacencyMatrixView()
        {
            DataContext = _context = new AdjacencyMatrixViewModel();

            SetBinding(MatrixDataProperty, new Binding("MatrixRepresentation"));
            InitializeComponent();
        }

        public void DrawMatrix()
        {
            //if (_drawing == null)
            //    return;

            //var rows = Math.Sqrt(MatrixData.Count);
            //var columns = rows;

            //int size = 12;
            //int offsetX = 0;
            //int offsetY = 0;

            ////using (var context = MatrixVisual.RenderOpen())
            ////{
            //    for (int i = 0; i < rows; i++)
            //    {
            //        context.DrawLine(
            //            new Pen(new SolidColorBrush(Colors.Red), 1),
            //            new Point(offsetX, offsetY),
            //            new Point(size * columns, offsetY));

            //        for (int j = 0; j < columns; j++)
            //        {
            //            context.DrawLine(
            //                new Pen(new SolidColorBrush(Colors.Red), 1),
            //                new Point(offsetX, offsetY),
            //                new Point(offsetX, j * size));

            //            offsetX += size;
            //        }

            //        offsetX = 0;
            //        offsetY += size;
            //    }
            //}
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            _drawing = drawingContext;

            DrawMatrix();
        }
    }
}
