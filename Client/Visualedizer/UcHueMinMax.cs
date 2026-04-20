using System.ComponentModel;
using System.Drawing.Drawing2D;
using Ledqualizer;

namespace Visualedizer
{
    public partial class UcHueMinMax : UserControl
    {
        private const int HueMinimum = 0;
        private const int HueMaximum = 360;
        private const int ThumbWidth = 12;
        private const int ThumbHeight = 20;
        private const int RailHeight = 12;
        private const int PaddingHorizontal = 10;
        private const int PaddingVertical = 5;

        private int hueStart;
        private int hueEnd = HueMaximum;
        private DragTarget dragTarget = DragTarget.None;

        public event EventHandler? ValueChanged;

        public UcHueMinMax()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.Selectable, true);

            MinimumSize = new Size(80, 28);
            TabStop = true;
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("Start hue value (0 - 360).")]
        public int HueStart
        {
            get => hueStart;
            set
            {
                int clamped = ClampHue(value);
                if (hueStart == clamped)
                {
                    Invalidate();
                    return;
                }

                hueStart = clamped;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("End hue value (0 - 360).")]
        public int HueEnd
        {
            get => hueEnd;
            set
            {
                int clamped = ClampHue(value);
                if (hueEnd == clamped)
                {
                    Invalidate();
                    return;
                }

                hueEnd = clamped;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(BackColor);

            Rectangle railBounds = GetRailBounds();
            DrawHueRail(e.Graphics, railBounds);
            DrawSelectionOverlay(e.Graphics, railBounds);
            DrawThumb(e.Graphics, railBounds, hueStart, true, Focused || dragTarget == DragTarget.Start);
            DrawThumb(e.Graphics, railBounds, hueEnd, false, Focused || dragTarget == DragTarget.End);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            Rectangle railBounds = GetRailBounds();
            Rectangle startThumbBounds = GetThumbBounds(railBounds, hueStart, true);
            Rectangle endThumbBounds = GetThumbBounds(railBounds, hueEnd, false);

            dragTarget = ResolveDragTarget(e.Location, startThumbBounds, endThumbBounds, railBounds);
            UpdateFromPointer(e.Location.X, raiseEvent: true);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragTarget == DragTarget.None || e.Button != MouseButtons.Left)
            {
                return;
            }

            UpdateFromPointer(e.Location.X, raiseEvent: true);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragTarget = DragTarget.None;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (dragTarget == DragTarget.None)
            {
                Invalidate();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData is Keys.Left or Keys.Right or Keys.ShiftKey || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            int delta = e.KeyCode switch
            {
                Keys.Left => -1,
                Keys.Right => 1,
                _ => 0
            };

            if (delta == 0)
            {
                return;
            }

            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                SetHueRange(hueStart, hueEnd + delta, true);
            }
            else
            {
                SetHueRange(hueStart + delta, hueEnd, true);
            }

            e.Handled = true;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        private void UpdateFromPointer(int pointerX, bool raiseEvent)
        {
            int hue = PositionToHue(pointerX);
            switch (dragTarget)
            {
                case DragTarget.Start:
                    SetHueRange(hue, hueEnd, raiseEvent);
                    break;
                case DragTarget.End:
                    SetHueRange(hueStart, hue, raiseEvent);
                    break;
            }
        }

        private void SetHueRange(int newStart, int newEnd, bool raiseEvent)
        {
            int clampedStart = ClampHue(newStart);
            int clampedEnd = ClampHue(newEnd);
            bool changed = hueStart != clampedStart || hueEnd != clampedEnd;
            hueStart = clampedStart;
            hueEnd = clampedEnd;
            Invalidate();

            if (changed && raiseEvent)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private Rectangle GetRailBounds()
        {
            int width = Math.Max(Width - (PaddingHorizontal * 2), 20);
            int x = (Width - width) / 2;
            int y = Math.Max((Height - RailHeight) / 2, PaddingVertical);
            return new Rectangle(x, y, width, RailHeight);
        }

        private Rectangle GetThumbBounds(Rectangle railBounds, int hue, bool alignLeft)
        {
            int centerX = HueToPosition(railBounds, hue);
            int x = alignLeft ? centerX - ThumbWidth : centerX;
            int y = railBounds.Top + (railBounds.Height - ThumbHeight) / 2;
            return new Rectangle(x, y, ThumbWidth, ThumbHeight);
        }

        private int HueToPosition(Rectangle railBounds, int hue)
        {
            if (railBounds.Width <= 1)
            {
                return railBounds.Left;
            }

            float ratio = (float)(hue - HueMinimum) / (HueMaximum - HueMinimum);
            return railBounds.Left + (int)Math.Round(ratio * (railBounds.Width - 1));
        }

        private int PositionToHue(int pointerX)
        {
            Rectangle railBounds = GetRailBounds();
            int clampedX = Math.Max(railBounds.Left, Math.Min(railBounds.Right - 1, pointerX));
            float ratio = railBounds.Width <= 1
                ? 0
                : (float)(clampedX - railBounds.Left) / (railBounds.Width - 1);
            return ClampHue((int)Math.Round(ratio * HueMaximum));
        }

        private static int ClampHue(int value)
        {
            return Math.Max(HueMinimum, Math.Min(HueMaximum, value));
        }

        private DragTarget ResolveDragTarget(Point location, Rectangle startThumbBounds, Rectangle endThumbBounds, Rectangle railBounds)
        {
            if (startThumbBounds.Contains(location))
            {
                return DragTarget.Start;
            }

            if (endThumbBounds.Contains(location))
            {
                return DragTarget.End;
            }

            if (!railBounds.Contains(location))
            {
                return Math.Abs(location.X - startThumbBounds.Left) <= Math.Abs(location.X - endThumbBounds.Right)
                    ? DragTarget.Start
                    : DragTarget.End;
            }

            int startDistance = Math.Abs(location.X - (startThumbBounds.Left + startThumbBounds.Width / 2));
            int endDistance = Math.Abs(location.X - (endThumbBounds.Left + endThumbBounds.Width / 2));
            return startDistance <= endDistance ? DragTarget.Start : DragTarget.End;
        }

        private void DrawHueRail(Graphics graphics, Rectangle railBounds)
        {
            for (int x = 0; x < railBounds.Width; x++)
            {
                double hue = railBounds.Width <= 1
                    ? 0
                    : (double)x / (railBounds.Width - 1) * HueMaximum;
                using Pen pen = new(Common.HSVToRGB(hue, 1.0, 1.0));
                graphics.DrawLine(pen, railBounds.Left + x, railBounds.Top, railBounds.Left + x, railBounds.Bottom - 1);
            }

            using Pen borderPen = new(Color.FromArgb(90, 90, 90));
            graphics.DrawRectangle(borderPen, railBounds);
        }

        private void DrawSelectionOverlay(Graphics graphics, Rectangle railBounds)
        {
            int left = Math.Min(HueToPosition(railBounds, hueStart), HueToPosition(railBounds, hueEnd));
            int right = Math.Max(HueToPosition(railBounds, hueStart), HueToPosition(railBounds, hueEnd));
            int width = Math.Max(right - left, 1);
            Rectangle selectionBounds = new(left, railBounds.Top, width, railBounds.Height);

            using Brush outsideBrush = new SolidBrush(Color.FromArgb(120, BackColor));
            if (selectionBounds.Left > railBounds.Left)
            {
                graphics.FillRectangle(outsideBrush, railBounds.Left, railBounds.Top, selectionBounds.Left - railBounds.Left, railBounds.Height);
            }

            if (selectionBounds.Right < railBounds.Right)
            {
                graphics.FillRectangle(outsideBrush, selectionBounds.Right, railBounds.Top, railBounds.Right - selectionBounds.Right, railBounds.Height);
            }

            using Pen selectionPen = new(Color.White, 1);
            graphics.DrawRectangle(selectionPen, selectionBounds);
        }

        private void DrawThumb(Graphics graphics, Rectangle railBounds, int hue, bool alignLeft, bool emphasize)
        {
            Rectangle thumbBounds = GetThumbBounds(railBounds, hue, alignLeft);
            using GraphicsPath path = CreateThumbPath(thumbBounds);
            using Brush fillBrush = new SolidBrush(emphasize ? Color.White : Color.FromArgb(245, 245, 245));
            using Pen borderPen = new(Color.FromArgb(60, 60, 60));
            graphics.FillPath(fillBrush, path);
            graphics.DrawPath(borderPen, path);
        }

        private static GraphicsPath CreateThumbPath(Rectangle bounds)
        {
            var path = new GraphicsPath();
            int radius = Math.Min(bounds.Width / 2, 4);

            path.AddArc(bounds.Left, bounds.Top, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Top, radius * 2, radius * 2, 270, 90);
            path.AddLine(bounds.Right, bounds.Top + radius, bounds.Right, bounds.Bottom - 5);
            path.AddLine(bounds.Right, bounds.Bottom - 5, bounds.Left + bounds.Width / 2, bounds.Bottom);
            path.AddLine(bounds.Left + bounds.Width / 2, bounds.Bottom, bounds.Left, bounds.Bottom - 5);
            path.AddLine(bounds.Left, bounds.Bottom - 5, bounds.Left, bounds.Top + radius);
            path.CloseFigure();
            return path;
        }

        private enum DragTarget
        {
            None,
            Start,
            End
        }
    }
}
