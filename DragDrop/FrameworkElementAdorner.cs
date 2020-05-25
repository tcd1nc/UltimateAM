
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace AssetManager.DragDrop
{
    class FrameworkElementAdorner : Adorner
    {
        private AdornerLayer adornerLayer;

        public FrameworkElementAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.adornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
            this.adornerLayer.Add(this);
        }

        internal void Update()
        {
            this.adornerLayer.Update(this.AdornedElement);
            this.Visibility = Visibility.Visible;
        }

        public void Remove()
        {

            this.Visibility = Visibility.Collapsed;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Red)
            {
                Opacity = 0.5
            };
            Pen renderPen = new Pen(new SolidColorBrush(Colors.White), 1.5);
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
            

            renderPen = new Pen(new SolidColorBrush(Colors.Red), 1.5);
            drawingContext.DrawLine(renderPen,adornedElementRect.TopLeft, adornedElementRect.TopRight);
            drawingContext.DrawLine(renderPen, adornedElementRect.BottomLeft, adornedElementRect.BottomRight);
            drawingContext.DrawLine(renderPen, adornedElementRect.TopLeft, adornedElementRect.BottomLeft);
            drawingContext.DrawLine(renderPen, adornedElementRect.TopRight, adornedElementRect.BottomRight);
        }

    }
}
