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
            ucPrimaryColor.ValueChanged += ControlValueChanged;
            ucBackgroundSettings.ValueChanged += ControlValueChanged;
        }

        public void LoadAudioDevices(string? selectedDeviceId)
        {
            AcVolume.LoadAudioDevicesToComboBox(cmbAudioDevices);
            SelectAudioDevice(selectedDeviceId);
        }

        public void SelectAudioDevice(string? deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            for (int i = 0; i < cmbAudioDevices.Items.Count; i++)
            {
                if (cmbAudioDevices.Items[i] is DeviceDescriptor descriptor && string.Equals(descriptor.DeviceId, deviceId, StringComparison.Ordinal))
                {
                    cmbAudioDevices.SelectedIndex = i;
                    return;
                }
            }
        }

        public string? GetSelectedAudioDeviceId()
        {
            return (cmbAudioDevices.SelectedItem as DeviceDescriptor)?.DeviceId;
        }

        public void UpdateProgress(int value)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateProgress), value);
                return;
            }

            prgAudioLevel.Value = Math.Max(prgAudioLevel.Minimum, Math.Min(prgAudioLevel.Maximum, value));
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                SpectralAnalysisSceneConfig config = scene.SpectralAnalysis;
                SelectMode(config.Mode);
                ucPrimaryColor.Brightness = config.Brightness;
                ucPrimaryColor.Saturation = config.Saturation;
                trkNormalizationLevel.Value = Math.Max(trkNormalizationLevel.Minimum, Math.Min(trkNormalizationLevel.Maximum, config.Normalization));
                ucPrimaryColor.HueStart = (int)Math.Round(config.HueMin);
                ucPrimaryColor.HueEnd = (int)Math.Round(config.HueMax);
                chkReverseOutput.Checked = config.Reverse;
                chkReverseHue.Checked = config.HueReverse;
                chkWhiteCenter.Checked = config.White;
                chkBackgroundEnabled.Checked = config.BackgroundBrightness > 0;
                ucBackgroundSettings.Hue = (int)Math.Round(config.BackgroundHue);
                ucBackgroundSettings.Saturation = config.BackgroundWhite
                    ? 0
                    : (config.BackgroundSaturation >= 0 ? config.BackgroundSaturation : config.Saturation);
                ucBackgroundSettings.Brightness = Math.Max(0, config.BackgroundBrightness > 0 ? config.BackgroundBrightness : 25);
                trkLowFrequency.Value = ClampTrackBar(trkLowFrequency, (int)Math.Round(config.FrequencyLowHz));
                trkHighFrequency.Value = ClampTrackBar(trkHighFrequency, (int)Math.Round(config.FrequencyHighHz));
                trkLowLevel.Value = ClampTrackBar(trkLowLevel, (int)Math.Round(config.LevelLowDb));
                trkHighLevel.Value = ClampTrackBar(trkHighLevel, (int)Math.Round(config.LevelHighDb));
                SyncNumericWithTrackBar(nudLowFrequency, trkLowFrequency);
                SyncNumericWithTrackBar(nudHighFrequency, trkHighFrequency);
                SyncNumericWithTrackBar(nudLowLevel, trkLowLevel);
                SyncNumericWithTrackBar(nudHighLevel, trkHighLevel);
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
            config.Brightness = ucPrimaryColor.Brightness;
            config.Saturation = ucPrimaryColor.Saturation;
            config.Normalization = trkNormalizationLevel.Value;
            config.HueMin = ucPrimaryColor.HueStart;
            config.HueMax = ucPrimaryColor.HueEnd;
            config.Reverse = chkReverseOutput.Checked;
            config.HueReverse = chkReverseHue.Checked;
            config.White = chkWhiteCenter.Checked;
            config.BackgroundWhite = chkBackgroundEnabled.Checked && ucBackgroundSettings.Saturation == 0;
            config.BackgroundHue = ucBackgroundSettings.Hue;
            config.BackgroundSaturation = ucBackgroundSettings.Saturation;
            config.BackgroundBrightness = chkBackgroundEnabled.Checked ? ucBackgroundSettings.Brightness : 0;
            config.FrequencyLowHz = trkLowFrequency.Value;
            config.FrequencyHighHz = trkHighFrequency.Value;
            config.LevelLowDb = trkLowLevel.Value;
            config.LevelHighDb = trkHighLevel.Value;
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private void chkBackgroundEnabled_CheckedChanged(object? sender, EventArgs e)
        {
            pnlBackgroundSettings.Enabled = chkBackgroundEnabled.Checked;
            ControlValueChanged(sender, e);
        }

        private static int ClampTrackBar(TrackBar trackBar, int value)
        {
            return Math.Max(trackBar.Minimum, Math.Min(trackBar.Maximum, value));
        }

        private void trackBarFrequencyLow_ValueChanged(object? sender, EventArgs e)
        {
            SyncNumericWithTrackBar(nudLowFrequency, trkLowFrequency);
            ControlValueChanged(sender, e);
        }

        private void trackBarFrequencyHigh_ValueChanged(object? sender, EventArgs e)
        {
            SyncNumericWithTrackBar(nudHighFrequency, trkHighFrequency);
            ControlValueChanged(sender, e);
        }

        private void trackBarLevelLow_ValueChanged(object? sender, EventArgs e)
        {
            SyncNumericWithTrackBar(nudLowLevel, trkLowLevel);
            ControlValueChanged(sender, e);
        }

        private void trackBarLevelHigh_ValueChanged(object? sender, EventArgs e)
        {
            SyncNumericWithTrackBar(nudHighLevel, trkHighLevel);
            ControlValueChanged(sender, e);
        }

        private void numFrequencyLow_ValueChanged(object? sender, EventArgs e)
        {
            SyncTrackBarWithNumeric(trkLowFrequency, nudLowFrequency);
            ControlValueChanged(sender, e);
        }

        private void numFrequencyHigh_ValueChanged(object? sender, EventArgs e)
        {
            SyncTrackBarWithNumeric(trkHighFrequency, nudHighFrequency);
            ControlValueChanged(sender, e);
        }

        private void numLevelLow_ValueChanged(object? sender, EventArgs e)
        {
            SyncTrackBarWithNumeric(trkLowLevel, nudLowLevel);
            ControlValueChanged(sender, e);
        }

        private void numLevelHigh_ValueChanged(object? sender, EventArgs e)
        {
            SyncTrackBarWithNumeric(trkHighLevel, nudHighLevel);
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
            if (rdoEndToStartMode.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeEndToStart;
            }

            if (rdoCenterOutMode.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeMidToOut;
            }

            if (rdoColorPushMode.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeColorPush;
            }

            if (rdoCenterPointMode.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeMidToOut_Point;
            }

            if (rdoBrightnessMode.Checked)
            {
                return AcVolume.AudioCaptureVolumeMode.ModeBrightness;
            }

            return AcVolume.AudioCaptureVolumeMode.ModeStartToEnd;
        }

        private void SelectMode(AcVolume.AudioCaptureVolumeMode mode)
        {
            rdoStartToEndMode.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeStartToEnd;
            rdoEndToStartMode.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeEndToStart;
            rdoCenterOutMode.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeMidToOut;
            rdoColorPushMode.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeColorPush;
            rdoCenterPointMode.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeMidToOut_Point;
            rdoBrightnessMode.Checked = mode == AcVolume.AudioCaptureVolumeMode.ModeBrightness;
        }
    }
}
