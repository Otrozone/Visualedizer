using System.ComponentModel;
using Ledqualizer;

namespace Visualedizer
{
    public class UcHueSaturationBrightness : UserControl
    {
        private readonly TableLayoutPanel layoutPanel;
        private readonly Label lblHue;
        private readonly Label lblSaturation;
        private readonly Label lblBrightness;
        private readonly UcHue hueControl;
        private readonly UcValueSlider saturationControl;
        private readonly UcValueSlider brightnessControl;

        public event EventHandler? ValueChanged;

        public UcHueSaturationBrightness()
        {
            layoutPanel = new TableLayoutPanel();
            lblHue = CreateLabel("Hue");
            lblSaturation = CreateLabel("Sat");
            lblBrightness = CreateLabel("Bri");
            hueControl = new UcHue();
            saturationControl = new UcValueSlider();
            brightnessControl = new UcValueSlider();

            SuspendLayout();

            layoutPanel.ColumnCount = 2;
            layoutPanel.RowCount = 3;
            layoutPanel.Dock = DockStyle.Fill;
            layoutPanel.Margin = Padding.Empty;
            layoutPanel.Padding = Padding.Empty;
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 38F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

            hueControl.Dock = DockStyle.Fill;
            hueControl.Margin = Padding.Empty;
            hueControl.MinVal = 0;
            hueControl.MaxVal = 360;
            hueControl.ValueChanged += ChildValueChanged;

            saturationControl.Dock = DockStyle.Fill;
            saturationControl.Margin = Padding.Empty;
            saturationControl.Minimum = 0;
            saturationControl.Maximum = 100;
            saturationControl.Value = 100;
            saturationControl.StartColor = Color.FromArgb(90, 90, 90);
            saturationControl.EndColor = Color.FromArgb(255, 106, 0);
            saturationControl.ValueChanged += ChildValueChanged;

            brightnessControl.Dock = DockStyle.Fill;
            brightnessControl.Margin = Padding.Empty;
            brightnessControl.Minimum = 0;
            brightnessControl.Maximum = 100;
            brightnessControl.Value = 100;
            brightnessControl.StartColor = Color.Black;
            brightnessControl.EndColor = Color.White;
            brightnessControl.ValueChanged += ChildValueChanged;

            layoutPanel.Controls.Add(lblHue, 0, 0);
            layoutPanel.Controls.Add(hueControl, 1, 0);
            layoutPanel.Controls.Add(lblSaturation, 0, 1);
            layoutPanel.Controls.Add(saturationControl, 1, 1);
            layoutPanel.Controls.Add(lblBrightness, 0, 2);
            layoutPanel.Controls.Add(brightnessControl, 1, 2);

            Controls.Add(layoutPanel);

            AutoScaleMode = AutoScaleMode.Font;
            DoubleBuffered = true;
            Margin = Padding.Empty;
            MinimumSize = new Size(120, 84);
            Name = nameof(UcHueSaturationBrightness);
            Size = new Size(300, 84);
            UpdateSliderColors();

            ResumeLayout(false);
        }

        [Browsable(true)]
        [Category("Color")]
        [DefaultValue(0)]
        public int Hue
        {
            get => hueControl.Hue;
            set => hueControl.Hue = value;
        }

        [Browsable(true)]
        [Category("Color")]
        [DefaultValue(0)]
        public int MinHue
        {
            get => hueControl.MinVal;
            set => hueControl.MinVal = value;
        }

        [Browsable(true)]
        [Category("Color")]
        [DefaultValue(360)]
        public int MaxHue
        {
            get => hueControl.MaxVal;
            set => hueControl.MaxVal = value;
        }

        [Browsable(true)]
        [Category("Color")]
        [DefaultValue(100)]
        public int Saturation
        {
            get => saturationControl.Value;
            set => saturationControl.Value = value;
        }

        [Browsable(true)]
        [Category("Color")]
        [DefaultValue(100)]
        public int Brightness
        {
            get => brightnessControl.Value;
            set => brightnessControl.Value = value;
        }

        private void ChildValueChanged(object? sender, EventArgs e)
        {
            UpdateSliderColors();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateSliderColors()
        {
            Color hueColor = Common.HSVToRGB(hueControl.Hue, 1.0, 1.0);
            saturationControl.EndColor = hueColor;
            brightnessControl.EndColor = Common.HSVToRGB(hueControl.Hue, saturationControl.Value / 100.0, 1.0);
            saturationControl.Invalidate();
            brightnessControl.Invalidate();
        }

        private static Label CreateLabel(string text)
        {
            return new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = text
            };
        }
    }
}
