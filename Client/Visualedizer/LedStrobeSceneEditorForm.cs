namespace Ledqualizer
{
    public sealed partial class LedStrobeSceneEditorForm : Form, ISceneEditorForm
    {
        private bool isLoading;
        private bool isAdjustingRanges;

        public LedStrobeSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            cbOnDurationMode.DataSource = Enum.GetValues<StrobeTimingMode>();
            cbOffDurationMode.DataSource = Enum.GetValues<StrobeTimingMode>();
            cbHueMode.DataSource = Enum.GetValues<StrobeHueMode>();
            UpdateControlStates();
        }

        public SceneType SceneType => SceneType.LedStrobe;

        public SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                LedStrobeSceneConfig config = scene.LedStrobe;
                cbOnDurationMode.SelectedItem = EnsureEnum(config.OnDurationMode, StrobeTimingMode.Constant);
                numOnDurationMs.Value = ClampDecimal(config.OnDurationMs, numOnDurationMs);
                SetRangeValues(numOnDurationMinMs, numOnDurationMaxMs, config.OnDurationMinMs, config.OnDurationMaxMs);

                cbOffDurationMode.SelectedItem = EnsureEnum(config.OffDurationMode, StrobeTimingMode.Constant);
                numOffDurationMs.Value = ClampDecimal(config.OffDurationMs, numOffDurationMs);
                SetRangeValues(numOffDurationMinMs, numOffDurationMaxMs, config.OffDurationMinMs, config.OffDurationMaxMs);

                cbHueMode.SelectedItem = EnsureEnum(config.HueMode, StrobeHueMode.Constant);
                ucHue.Hue = (int)Math.Round(Math.Clamp(config.Hue, 0.0, 360.0));
                ucHueRange.HueStart = (int)Math.Round(Math.Clamp(config.HueMin, 0.0, 360.0));
                ucHueRange.HueEnd = (int)Math.Round(Math.Clamp(config.HueMax, 0.0, 360.0));
                numSaturation.Value = ClampDecimal(config.Saturation, numSaturation);
                numBrightness.Value = ClampDecimal(config.Brightness, numBrightness);
            }
            finally
            {
                isLoading = false;
            }

            UpdateControlStates();
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (isLoading || isAdjustingRanges)
            {
                return;
            }

            isAdjustingRanges = true;
            try
            {
                EnsureRangeOrder(numOnDurationMinMs, numOnDurationMaxMs, sender);
                EnsureRangeOrder(numOffDurationMinMs, numOffDurationMaxMs, sender);
            }
            finally
            {
                isAdjustingRanges = false;
            }

            UpdateControlStates();
            UpdateSceneFromControls();
        }

        private void UpdateControlStates()
        {
            bool onRandom = GetSelectedTimingMode(cbOnDurationMode) == StrobeTimingMode.RandomRange;
            lblOnDurationMs.Enabled = !onRandom;
            numOnDurationMs.Enabled = !onRandom;
            lblOnDurationMinMs.Enabled = onRandom;
            numOnDurationMinMs.Enabled = onRandom;
            lblOnDurationMaxMs.Enabled = onRandom;
            numOnDurationMaxMs.Enabled = onRandom;

            bool offRandom = GetSelectedTimingMode(cbOffDurationMode) == StrobeTimingMode.RandomRange;
            lblOffDurationMs.Enabled = !offRandom;
            numOffDurationMs.Enabled = !offRandom;
            lblOffDurationMinMs.Enabled = offRandom;
            numOffDurationMinMs.Enabled = offRandom;
            lblOffDurationMaxMs.Enabled = offRandom;
            numOffDurationMaxMs.Enabled = offRandom;

            bool hueRandom = GetSelectedHueMode() == StrobeHueMode.RandomRange;
            lblHue.Enabled = !hueRandom;
            ucHue.Enabled = !hueRandom;
            lblHueRange.Enabled = hueRandom;
            ucHueRange.Enabled = hueRandom;
        }

        private void UpdateSceneFromControls()
        {
            if (CurrentScene == null || isLoading)
            {
                return;
            }

            LedStrobeSceneConfig config = CurrentScene.LedStrobe;
            config.OnDurationMode = GetSelectedTimingMode(cbOnDurationMode);
            config.OnDurationMs = Math.Max(1, (int)numOnDurationMs.Value);
            config.OnDurationMinMs = Math.Max(1, (int)numOnDurationMinMs.Value);
            config.OnDurationMaxMs = Math.Max(config.OnDurationMinMs, (int)numOnDurationMaxMs.Value);
            config.OffDurationMode = GetSelectedTimingMode(cbOffDurationMode);
            config.OffDurationMs = Math.Max(1, (int)numOffDurationMs.Value);
            config.OffDurationMinMs = Math.Max(1, (int)numOffDurationMinMs.Value);
            config.OffDurationMaxMs = Math.Max(config.OffDurationMinMs, (int)numOffDurationMaxMs.Value);
            config.HueMode = GetSelectedHueMode();
            config.Hue = ucHue.Hue;
            config.HueMin = Math.Min(ucHueRange.HueStart, ucHueRange.HueEnd);
            config.HueMax = Math.Max(ucHueRange.HueStart, ucHueRange.HueEnd);
            config.Saturation = (int)numSaturation.Value;
            config.Brightness = (int)numBrightness.Value;
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private static StrobeTimingMode GetSelectedTimingMode(ComboBox comboBox)
        {
            return comboBox.SelectedItem is StrobeTimingMode mode ? mode : StrobeTimingMode.Constant;
        }

        private StrobeHueMode GetSelectedHueMode()
        {
            return cbHueMode.SelectedItem is StrobeHueMode mode ? mode : StrobeHueMode.Constant;
        }

        private static T EnsureEnum<T>(T value, T fallback)
            where T : struct, Enum
        {
            return Enum.IsDefined(value) ? value : fallback;
        }

        private static void SetRangeValues(NumericUpDown minControl, NumericUpDown maxControl, int configuredMin, int configuredMax)
        {
            int normalizedMin = Math.Max((int)minControl.Minimum, Math.Min(configuredMin, configuredMax));
            int normalizedMax = Math.Max(normalizedMin, Math.Max(configuredMin, configuredMax));
            minControl.Value = ClampDecimal(normalizedMin, minControl);
            maxControl.Value = ClampDecimal(normalizedMax, maxControl);
            if (maxControl.Value < minControl.Value)
            {
                maxControl.Value = minControl.Value;
            }
        }

        private static void EnsureRangeOrder(NumericUpDown minControl, NumericUpDown maxControl, object? sender)
        {
            if (maxControl.Value >= minControl.Value)
            {
                return;
            }

            if (ReferenceEquals(sender, maxControl))
            {
                minControl.Value = maxControl.Value;
            }
            else
            {
                maxControl.Value = minControl.Value;
            }
        }

        private static decimal ClampDecimal(double value, NumericUpDown control)
        {
            decimal decimalValue = (decimal)value;
            return Math.Max(control.Minimum, Math.Min(control.Maximum, decimalValue));
        }
    }
}
