using static Ledqualizer.AcVolume;

namespace Ledqualizer
{
    public partial class SpectralAnalysisSceneEditorForm : Form, ISceneEditorForm
    {
        private bool isLoading;

        public SceneType SceneType => SceneType.SpectralAnalysis;

        protected SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public event EventHandler? SelectedAudioDeviceChanged;

        public SpectralAnalysisSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            ucHueMinMax.ValueChanged += ControlValueChanged;
            ucHueBg.ValueChanged += ControlValueChanged;
        }

        public void LoadAudioDevices(string? selectedDeviceId)
        {
            AcVolume.LoadAudioDevicesToComboBox(cbAudioDevices);
            SelectAudioDevice(selectedDeviceId);
        }

        public void SelectAudioDevice(string? deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            for (int i = 0; i < cbAudioDevices.Items.Count; i++)
            {
                if (cbAudioDevices.Items[i] is DeviceDescriptor descriptor && string.Equals(descriptor.DeviceId, deviceId, StringComparison.Ordinal))
                {
                    cbAudioDevices.SelectedIndex = i;
                    return;
                }
            }
        }

        public string? GetSelectedAudioDeviceId()
        {
            return (cbAudioDevices.SelectedItem as DeviceDescriptor)?.DeviceId;
        }

        public void UpdateProgress(int value)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateProgress), value);
                return;
            }

            progressBar.Value = Math.Max(progressBar.Minimum, Math.Min(progressBar.Maximum, value));
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                SpectralAnalysisSceneConfig config = scene.SpectralAnalysis;
                SelectMode(config.Mode);
                trackBarBrightness.Value = Math.Max(trackBarBrightness.Minimum, Math.Min(trackBarBrightness.Maximum, config.Brightness));
                trackBarNormalizationLevel.Value = Math.Max(trackBarNormalizationLevel.Minimum, Math.Min(trackBarNormalizationLevel.Maximum, config.Normalization));
                ucHueMinMax.HueMin = (int)Math.Round(config.HueMin);
                ucHueMinMax.HueMax = (int)Math.Round(config.HueMax);
                chbReverse.Checked = config.Reverse;
                chbHueReverse.Checked = config.HueReverse;
                chbWhite.Checked = config.White;
                chbBgWhite.Checked = config.BackgroundWhite;
                ucHueBg.Hue = (int)Math.Round(config.BackgroundHue);
                trackBarBgBrightness.Value = Math.Max(trackBarBgBrightness.Minimum, Math.Min(trackBarBgBrightness.Maximum, config.BackgroundBrightness));
                trackBarFrequencyLow.Value = ClampTrackBar(trackBarFrequencyLow, (int)Math.Round(config.FrequencyLowHz));
                trackBarFrequencyHigh.Value = ClampTrackBar(trackBarFrequencyHigh, (int)Math.Round(config.FrequencyHighHz));
                trackBarLevelLow.Value = ClampTrackBar(trackBarLevelLow, (int)Math.Round(config.LevelLowDb));
                trackBarLevelHigh.Value = ClampTrackBar(trackBarLevelHigh, (int)Math.Round(config.LevelHighDb));
                SyncNumericWithTrackBar(numFrequencyLow, trackBarFrequencyLow);
                SyncNumericWithTrackBar(numFrequencyHigh, trackBarFrequencyHigh);
                SyncNumericWithTrackBar(numLevelLow, trackBarLevelLow);
                SyncNumericWithTrackBar(numLevelHigh, trackBarLevelHigh);
            }
            finally
            {
                isLoading = false;
            }
        }

        private void cbAudioDevices_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SelectedAudioDeviceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (CurrentScene == null || isLoading)
            {
                return;
            }

            SpectralAnalysisSceneConfig config = CurrentScene.SpectralAnalysis;
            config.Mode = GetSelectedMode();
            config.Brightness = trackBarBrightness.Value;
            config.Normalization = trackBarNormalizationLevel.Value;
            config.HueMin = ucHueMinMax.HueMin;
            config.HueMax = ucHueMinMax.HueMax;
            config.Reverse = chbReverse.Checked;
            config.HueReverse = chbHueReverse.Checked;
            config.White = chbWhite.Checked;
            config.BackgroundWhite = chbBgWhite.Checked;
            config.BackgroundHue = ucHueBg.Hue;
            config.BackgroundBrightness = trackBarBgBrightness.Value;
            config.FrequencyLowHz = trackBarFrequencyLow.Value;
            config.FrequencyHighHz = trackBarFrequencyHigh.Value;
            config.LevelLowDb = trackBarLevelLow.Value;
            config.LevelHighDb = trackBarLevelHigh.Value;
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private static int ClampTrackBar(TrackBar trackBar, int value)
        {
            return Math.Max(trackBar.Minimum, Math.Min(trackBar.Maximum, value));
        }

        private void trackBarFrequencyLow_ValueChanged(object? sender, EventArgs e)
        {
            SyncNumericWithTrackBar(numFrequencyLow, trackBarFrequencyLow);
            ControlValueChanged(sender, e);
        }

        private void trackBarFrequencyHigh_ValueChanged(object? sender, EventArgs e)
        {
            SyncNumericWithTrackBar(numFrequencyHigh, trackBarFrequencyHigh);
            ControlValueChanged(sender, e);
        }

        private void trackBarLevelLow_ValueChanged(object? sender, EventArgs e)
        {
            SyncNumericWithTrackBar(numLevelLow, trackBarLevelLow);
            ControlValueChanged(sender, e);
        }

        private void trackBarLevelHigh_ValueChanged(object? sender, EventArgs e)
        {
            SyncNumericWithTrackBar(numLevelHigh, trackBarLevelHigh);
            ControlValueChanged(sender, e);
        }

        private void numFrequencyLow_ValueChanged(object? sender, EventArgs e)
        {
            SyncTrackBarWithNumeric(trackBarFrequencyLow, numFrequencyLow);
            ControlValueChanged(sender, e);
        }

        private void numFrequencyHigh_ValueChanged(object? sender, EventArgs e)
        {
            SyncTrackBarWithNumeric(trackBarFrequencyHigh, numFrequencyHigh);
            ControlValueChanged(sender, e);
        }

        private void numLevelLow_ValueChanged(object? sender, EventArgs e)
        {
            SyncTrackBarWithNumeric(trackBarLevelLow, numLevelLow);
            ControlValueChanged(sender, e);
        }

        private void numLevelHigh_ValueChanged(object? sender, EventArgs e)
        {
            SyncTrackBarWithNumeric(trackBarLevelHigh, numLevelHigh);
            ControlValueChanged(sender, e);
        }

        private void SyncNumericWithTrackBar(NumericUpDown numericUpDown, TrackBar trackBar)
        {
            decimal value = trackBar.Value;
            if (numericUpDown.Value != value)
            {
                numericUpDown.Value = value;
            }
        }

        private void SyncTrackBarWithNumeric(TrackBar trackBar, NumericUpDown numericUpDown)
        {
            int value = Decimal.ToInt32(numericUpDown.Value);
            if (trackBar.Value != value)
            {
                trackBar.Value = ClampTrackBar(trackBar, value);
            }
        }

        private AcVolume.AudioCaptureVolumeMode GetSelectedMode()
        {
            if (rbModeEndToStart.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeEndToStart;
            }

            if (rbModeMidToOut.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeMidToOut;
            }

            if (rbModeColorPush.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeColorPush;
            }

            if (rbModeMidToOutPoint.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeMidToOut_Point;
            }

            if (rbBrightness.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeBrightness;
            }

            return AcVolume.AudioCaptureVolumeMode.ModeStartToEnd;
        }

        private void SelectMode(AcVolume.AudioCaptureVolumeMode mode)
        {
            rbModeStartToEnd.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeStartToEnd;
            rbModeEndToStart.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeEndToStart;
            rbModeMidToOut.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeMidToOut;
            rbModeColorPush.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeColorPush;
            rbModeMidToOutPoint.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeMidToOut_Point;
            rbBrightness.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeBrightness;
        }
    }
}
