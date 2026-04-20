using System.ComponentModel;
using System.Drawing.Drawing2D;
using Ledqualizer;

namespace Visualedizer
{
    public partial class UcHue : UserControl
    {
        private const int DefaultMinimum = 0;
        private const int DefaultMaximum = 360;
        private const int ThumbWidth = 12;
        private const int ThumbHeight = 20;
        private const int RailHeight = 12;
        private const int PaddingHorizontal = 10;
        private const int PaddingVertical = 5;

        private int minVal = DefaultMinimum;
        private int maxVal = DefaultMaximum;
        private int hue;
        private bool isDragging;

        public event EventHandler? ValueChanged;

        public UcHue()
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
        [Description("Hue value.")]
        public int Hue
        {
            get => hue;
            set
            {
                int clamped = ClampValue(value, minVal, maxVal);
                if (hue == clamped)
                {
                    Invalidate();
                    return;
                }

                hue = clamped;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("Minimum hue value.")]
        public int MinVal
        {
            get => minVal;
            set
            {
                minVal = value;
                if (minVal > maxVal)
                {
                    maxVal = minVal;
                }

                hue = ClampValue(hue, minVal, maxVal);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("Maximum hue value.")]
        public int MaxVal
        {
            get => maxVal;
            set
            {
                maxVal = value;
                if (maxVal < minVal)
                {
                    minVal = maxVal;
                }

                hue = ClampValue(hue, minVal, maxVal);
                Invalidate();
            }
        }

        public int getHueVal()
        {
            return Hue;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(BackColor);

            Rectangle railBounds = GetRailBounds();
            DrawHueRail(e.Graphics, railBounds);
            DrawThumb(e.Graphics, railBounds, hue, Focused || isDragging);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            isDragging = true;
            UpdateFromPointer(e.X, raiseEvent: true);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!isDragging || e.Button != MouseButtons.Left)
            {
                return;
            }

            UpdateFromPointer(e.X, raiseEvent: true);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData is Keys.Left or Keys.Right || base.IsInputKey(keyData);
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

            SetHue(hue + delta, true);
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
            SetHue(PositionToHue(pointerX), raiseEvent);
        }

        private void SetHue(int newHue, bool raiseEvent)
        {
            int clamped = ClampValue(newHue, minVal, maxVal);
            bool changed = hue != clamped;
            hue = clamped;
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

        private Rectangle GetThumbBounds(Rectangle railBounds, int value)
        {
            int centerX = ValueToPosition(railBounds, value);
            return new Rectangle(centerX - (ThumbWidth / 2), railBounds.Top + (railBounds.Height - ThumbHeight) / 2, ThumbWidth, ThumbHeight);
        }

        private int ValueToPosition(Rectangle railBounds, int value)
        {
            if (railBounds.Width <= 1 || maxVal == minVal)
            {
                return railBounds.Left;
            }

            float ratio = (float)(value - minVal) / (maxVal - minVal);
            return railBounds.Left + (int)Math.Round(ratio * (railBounds.Width - 1));
        }

        private int PositionToHue(int pointerX)
        {
            Rectangle railBounds = GetRailBounds();
            int clampedX = Math.Max(railBounds.Left, Math.Min(railBounds.Right - 1, pointerX));
            float ratio = railBounds.Width <= 1
                ? 0
                : (float)(clampedX - railBounds.Left) / (railBounds.Width - 1);
            return ClampValue((int)Math.Round(minVal + (ratio * (maxVal - minVal))), minVal, maxVal);
        }

        private static int ClampValue(int value, int minimum, int maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private void DrawHueRail(Graphics graphics, Rectangle railBounds)
        {
            for (int x = 0; x < railBounds.Width; x++)
            {
                double hueValue = railBounds.Width <= 1
                    ? minVal
                    : minVal + ((double)x / (railBounds.Width - 1) * (maxVal - minVal));
                using Pen pen = new(Common.HSVToRGB(hueValue, 1.0, 1.0));
                graphics.DrawLine(pen, railBounds.Left + x, railBounds.Top, railBounds.Left + x, railBounds.Bottom - 1);
            }

            using Pen borderPen = new(Color.FromArgb(90, 90, 90));
            graphics.DrawRectangle(borderPen, railBounds);
        }

        private void DrawThumb(Graphics graphics, Rectangle railBounds, int value, bool emphasize)
        {
            Rectangle thumbBounds = GetThumbBounds(railBounds, value);
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
    }
}
