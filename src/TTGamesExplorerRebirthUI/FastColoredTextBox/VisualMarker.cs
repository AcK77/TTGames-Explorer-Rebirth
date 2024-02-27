namespace FastColoredTextBoxNS
{
    public class VisualMarker(Rectangle rectangle)
    {
        public readonly Rectangle Rectangle = rectangle;

        public virtual void Draw(Graphics gr, Pen pen)
        {
        }

        public virtual Cursor Cursor
        {
            get { return Cursors.Hand; }
        }
    }

    public class CollapseFoldingMarker(int iLine, Rectangle rectangle) : VisualMarker(rectangle)
    {
        public readonly int ILine = iLine;

        public void Draw(Graphics gr, Pen pen, Brush backgroundBrush, Pen forePen)
        {
            // Draw minus
            gr.FillRectangle(backgroundBrush, Rectangle);
            gr.DrawRectangle(pen, Rectangle);
            gr.DrawLine(forePen, Rectangle.Left + 2, Rectangle.Top + Rectangle.Height / 2, Rectangle.Right - 2, Rectangle.Top + Rectangle.Height / 2);
        }
    }

    public class ExpandFoldingMarker(int iLine, Rectangle rectangle) : VisualMarker(rectangle)
    {
        public readonly int ILine = iLine;

        public void Draw(Graphics gr, Pen pen,  Brush backgroundBrush, Pen forePen)
        {
            // Draw plus
            gr.FillRectangle(backgroundBrush, Rectangle);
            gr.DrawRectangle(pen, Rectangle);
            gr.DrawLine(forePen, Rectangle.Left + 2, Rectangle.Top + Rectangle.Height / 2, Rectangle.Right - 2, Rectangle.Top + Rectangle.Height / 2);
            gr.DrawLine(forePen, Rectangle.Left + Rectangle.Width / 2, Rectangle.Top + 2, Rectangle.Left + Rectangle.Width / 2, Rectangle.Bottom - 2);
        }
    }

    public class FoldedAreaMarker(int iLine, Rectangle rectangle) : VisualMarker(rectangle)
    {
        public readonly int ILine = iLine;

        public override void Draw(Graphics gr, Pen pen)
        {
            gr.DrawRectangle(pen, Rectangle);
        }
    }

    public class StyleVisualMarker(Rectangle rectangle, Style style) : VisualMarker(rectangle)
    {
        public Style Style { get; private set; } = style;
    }

    public class VisualMarkerEventArgs(Style style, StyleVisualMarker marker, MouseEventArgs args) : MouseEventArgs(args.Button, args.Clicks, args.X, args.Y, args.Delta)
    {
        public Style Style { get; private set; } = style;
        public StyleVisualMarker Marker { get; private set; } = marker;
    }
}