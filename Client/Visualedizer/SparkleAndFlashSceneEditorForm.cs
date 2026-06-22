namespace Ledqualizer
{
    public partial class SparkleAndFlashSceneEditorForm : Form, ISceneEditorForm
    {
        private bool isLoading;
        private bool isAdjustingRanges;

        public SparkleAndFlashSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
        }

        public SceneType SceneType => SceneType.SparkleAndFlash;

        public SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                SparkleAndFlashSceneConfig config = scene.SparkleAndFlash;
                SetRangeValues(numSegmentSizeMin, numSegmentSizeMax, config.SegmentSizeMin, config.SegmentSizeMax);
                numSegmentHoldMs.Value = ClampDecimal(config.SegmentHoldMs, numSegmentHoldMs);
                SetRangeValues(numSegmentIntervalMinMs, numSegmentIntervalMaxMs, config.SegmentIntervalMinMs, config.SegmentIntervalMaxMs);
                SetDecimalRangeValues(numSparkleHueMin, numSparkleHueMax, config.SparkleHueMin, config.SparkleHueMax);
                SetRangeValues(
                    numSparkleHueChangeIntervalMinMs,
                    numSparkleHueChangeIntervalMaxMs,
                    config.SparkleHueChangeIntervalMinMs,
                    config.SparkleHueChangeIntervalMaxMs);
                chkContinuousSparkleHueChange.Checked = config.ContinuousSparkleHueChange;
                chkSmoothFadeAndBlur.Checked = config.SmoothFadeAndBlur;
                numFadeDurationMs.Value = ClampDecimal(config.FadeDurationMs, numFadeDurationMs);
                numBlurRadius.Value = ClampDecimal(config.BlurRadius, numBlurRadius);
                numMaxActiveSparkles.Value = ClampDecimal(config.MaxActiveSparkles, numMaxActiveSparkles);
                chkFullStripFlashEnabled.Checked = config.FullStripFlashEnabled;
                numFullStripFlashHoldMs.Value = ClampDecimal(config.FullStripFlashHoldMs, numFullStripFlashHoldMs);
                chkFullStripSmoothFade.Checked = config.FullStripSmoothFade;
                numFullStripFadeDurationMs.Value = ClampDecimal(config.FullStripFadeDurationMs, numFullStripFadeDurationMs);
                SetRangeValues(
                    numFullStripFlashIntervalMinMs,
                    numFullStripFlashIntervalMaxMs,
                    config.FullStripFlashIntervalMinMs,
                    config.FullStripFlashIntervalMaxMs);
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
                EnsureRangeOrder(numSegmentSizeMin, numSegmentSizeMax, sender);
                EnsureRangeOrder(numSegmentIntervalMinMs, numSegmentIntervalMaxMs, sender);
                EnsureRangeOrder(numSparkleHueMin, numSparkleHueMax, sender);
                EnsureRangeOrder(numSparkleHueChangeIntervalMinMs, numSparkleHueChangeIntervalMaxMs, sender);
                EnsureRangeOrder(numFullStripFlashIntervalMinMs, numFullStripFlashIntervalMaxMs, sender);
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
            bool smoothEnabled = chkSmoothFadeAndBlur.Checked;
            lblFadeDurationMs.Enabled = smoothEnabled;
            numFadeDurationMs.Enabled = smoothEnabled;
            lblBlurRadius.Enabled = smoothEnabled;
            numBlurRadius.Enabled = smoothEnabled;

            bool fullStripEnabled = chkFullStripFlashEnabled.Checked;
            lblFullStripHoldMs.Enabled = fullStripEnabled;
            numFullStripFlashHoldMs.Enabled = fullStripEnabled;
            chkFullStripSmoothFade.Enabled = fullStripEnabled;
            lblFullStripFadeDurationMs.Enabled = fullStripEnabled && chkFullStripSmoothFade.Checked;
            numFullStripFadeDurationMs.Enabled = fullStripEnabled && chkFullStripSmoothFade.Checked;
            lblFullStripIntervalMinMs.Enabled = fullStripEnabled;
            numFullStripFlashIntervalMinMs.Enabled = fullStripEnabled;
            lblFullStripIntervalMaxMs.Enabled = fullStripEnabled;
            numFullStripFlashIntervalMaxMs.Enabled = fullStripEnabled;
        }

        private void UpdateSceneFromControls()
        {
            if (CurrentScene == null || isLoading)
            {
                return;
            }

            SparkleAndFlashSceneConfig config = CurrentScene.SparkleAndFlash;
            config.SegmentSizeMin = (int)numSegmentSizeMin.Value;
            config.SegmentSizeMax = (int)numSegmentSizeMax.Value;
            config.SegmentHoldMs = (int)numSegmentHoldMs.Value;
            config.SegmentIntervalMinMs = (int)numSegmentIntervalMinMs.Value;
            config.SegmentIntervalMaxMs = (int)numSegmentIntervalMaxMs.Value;
            config.SparkleHueMin = (double)numSparkleHueMin.Value;
            config.SparkleHueMax = (double)numSparkleHueMax.Value;
            config.SparkleHueChangeIntervalMinMs = (int)numSparkleHueChangeIntervalMinMs.Value;
            config.SparkleHueChangeIntervalMaxMs = (int)numSparkleHueChangeIntervalMaxMs.Value;
            config.ContinuousSparkleHueChange = chkContinuousSparkleHueChange.Checked;
            config.SmoothFadeAndBlur = chkSmoothFadeAndBlur.Checked;
            config.FadeDurationMs = (int)numFadeDurationMs.Value;
            config.BlurRadius = (int)numBlurRadius.Value;
            config.MaxActiveSparkles = (int)numMaxActiveSparkles.Value;
            config.FullStripFlashEnabled = chkFullStripFlashEnabled.Checked;
            config.FullStripFlashHoldMs = (int)numFullStripFlashHoldMs.Value;
            config.FullStripSmoothFade = chkFullStripSmoothFade.Checked;
            config.FullStripFadeDurationMs = (int)numFullStripFadeDurationMs.Value;
            config.FullStripFlashIntervalMinMs = (int)numFullStripFlashIntervalMinMs.Value;
            config.FullStripFlashIntervalMaxMs = (int)numFullStripFlashIntervalMaxMs.Value;
            SceneChanged?.Invoke(this, EventArgs.Empty);
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

        private static void SetDecimalRangeValues(NumericUpDown minControl, NumericUpDown maxControl, double configuredMin, double configuredMax)
        {
            double normalizedMin = Math.Max((double)minControl.Minimum, Math.Min(configuredMin, configuredMax));
            double normalizedMax = Math.Max(normalizedMin, Math.Max(configuredMin, configuredMax));
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
