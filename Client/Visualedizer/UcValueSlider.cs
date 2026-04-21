using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Visualedizer
{
    public class UcValueSlider : UserControl
    {
        private const int ThumbWidth = 12;
        private const int ThumbHeight = 20;
        private const int RailHeight = 12;
        private const int PaddingHorizontal = 10;
        private const int PaddingVertical = 5;

        private int minimum;
        private int maximum = 100;
        private int sliderValue = 100;
        private bool isDragging;
        private Color startColor = Color.FromArgb(80, 80, 80);
        private Color endColor = Color.White;

        public event EventHandler? ValueChanged;

        public UcValueSlider()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                | ControlStyles.Selectable, true);

            DoubleBuffered = true;
            MinimumSize = new Size(80, 28);
            Size = new Size(200, 30);
            TabStop = true;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(0)]
        public int Minimum
        {
            get => minimum;
            set
            {
                minimum = value;
                if (maximum < minimum)
                {
                    maximum = minimum;
                }

                Value = ClampValue(sliderValue);
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(100)]
        public int Maximum
        {
            get => maximum;
            set
            {
                maximum = value;
                if (minimum > maximum)
                {
                    minimum = maximum;
                }

                Value = ClampValue(sliderValue);
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(100)]
        public int Value
        {
            get => sliderValue;
            set
            {
                int clamped = ClampValue(value);
                if (sliderValue == clamped)
                {
                    Invalidate();
                    return;
                }

                sliderValue = clamped;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "80, 80, 80")]
        public Color StartColor
        {
            get => startColor;
            set
            {
                if (startColor == value)
                {
                    return;
                }

                startColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color EndColor
        {
            get => endColor;
            set
            {
                if (endColor == value)
                {
                    return;
                }

                endColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(BackColor);

            Rectangle railBounds = GetRailBounds();
            DrawRail(e.Graphics, railBounds);
            DrawThumb(e.Graphics, railBounds, Focused || isDragging);
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

            SetValue(sliderValue + delta, true);
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
            SetValue(PositionToValue(pointerX), raiseEvent);
        }

        private void SetValue(int newValue, bool raiseEvent)
        {
            int clamped = ClampValue(newValue);
            bool changed = sliderValue != clamped;
            sliderValue = clamped;
            Invalidate();

            if (changed && raiseEvent)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int ClampValue(int value)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private Rectangle GetRailBounds()
        {
            int width = Math.Max(Width - (PaddingHorizontal * 2), 20);
            int x = (Width - width) / 2;
            int y = Math.Max((Height - RailHeight) / 2, PaddingVertical);
            return new Rectangle(x, y, width, RailHeight);
        }

        private int ValueToPosition(Rectangle railBounds, int value)
        {
            if (railBounds.Width <= 1 || maximum == minimum)
            {
                return railBounds.Left;
            }

            float ratio = (float)(value - minimum) / (maximum - minimum);
            return railBounds.Left + (int)Math.Round(ratio * (railBounds.Width - 1));
        }

        private int PositionToValue(int pointerX)
        {
            Rectangle railBounds = GetRailBounds();
            int clampedX = Math.Max(railBounds.Left, Math.Min(railBounds.Right - 1, pointerX));
            float ratio = railBounds.Width <= 1
                ? 0
                : (float)(clampedX - railBounds.Left) / (railBounds.Width - 1);
            return ClampValue((int)Math.Round(minimum + (ratio * (maximum - minimum))));
        }

        private Rectangle GetThumbBounds(Rectangle railBounds)
        {
            int centerX = ValueToPosition(railBounds, sliderValue);
            return new Rectangle(centerX - (ThumbWidth / 2), railBounds.Top + (railBounds.Height - ThumbHeight) / 2, ThumbWidth, ThumbHeight);
        }

        private void DrawRail(Graphics graphics, Rectangle railBounds)
        {
            using LinearGradientBrush brush = new(railBounds, startColor, endColor, LinearGradientMode.Horizontal);
            graphics.FillRectangle(brush, railBounds);

            using Pen borderPen = new(Color.FromArgb(90, 90, 90));
            graphics.DrawRectangle(borderPen, railBounds);
        }

        private void DrawThumb(Graphics graphics, Rectangle railBounds, bool emphasize)
        {
            Rectangle thumbBounds = GetThumbBounds(railBounds);
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
