using static Ledqualizer.AcVolume;

namespace Ledqualizer
{
    public partial class VolumeReactiveSceneEditorForm : Form, ISceneEditorForm
    {
        private readonly RadioButton[] rotateModes;
        private int rotateIdx;
        private bool isLoading;

        public SceneType SceneType => SceneType.VolumeReactive;

        protected SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public event EventHandler? SelectedAudioDeviceChanged;

        public VolumeReactiveSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            rotateModes = new[] { rbModeColorPush, rbModeEndToStart, rbModeMidToOut, rbModeMidToOutPoint, rbModeStartToEnd };
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
                VolumeReactiveSceneConfig config = scene.VolumeReactive;
                SelectMode(config.Mode);
                chbRotate.Checked = config.RotateModes;
                trackBarRotate.Value = Math.Max(trackBarRotate.Minimum, Math.Min(trackBarRotate.Maximum, config.RotateIntervalSeconds));
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
                timerRotate.Interval = trackBarRotate.Value * 1000;
                timerRotate.Enabled = chbRotate.Checked;
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

        private void chbRotate_CheckedChanged(object? sender, EventArgs e)
        {
            timerRotate.Enabled = chbRotate.Checked;
            ControlValueChanged(sender, e);
        }

        private void timerRotate_Tick(object? sender, EventArgs e)
        {
            if (rotateIdx >= rotateModes.Length)
            {
                rotateIdx = 0;
            }

            rotateModes[rotateIdx].Checked = true;
            rotateIdx++;
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (CurrentScene == null || isLoading)
            {
                return;
            }

            VolumeReactiveSceneConfig config = CurrentScene.VolumeReactive;
            config.Mode = GetSelectedMode();
            config.RotateModes = chbRotate.Checked;
            config.RotateIntervalSeconds = trackBarRotate.Value;
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
            timerRotate.Interval = trackBarRotate.Value * 1000;
            SceneChanged?.Invoke(this, EventArgs.Empty);
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
