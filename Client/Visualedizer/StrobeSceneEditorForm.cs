namespace Ledqualizer
{
    public sealed partial class StrobeSceneEditorForm : Form, ISceneEditorForm
    {
        private bool isLoading;
        private bool selectorDataLoaded;

        public StrobeSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            EnsureSelectorDataLoaded();
            UpdateTriggerPanelVisibility();
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                EnsureSelectorDataLoaded();
                LoadTrigger(scene.Strobe.Trigger);
            }
            finally
            {
                isLoading = false;
            }

            UpdateTriggerPanelVisibility();
        }

        public event EventHandler? SceneChanged;
        public event EventHandler<StrobeTestRequestedEventArgs>? TestRequested;

        public SceneType SceneType => SceneType.Strobe;

        public SceneConfig? CurrentScene { get; private set; }

        private void EnsureSelectorDataLoaded()
        {
            if (selectorDataLoaded)
            {
                return;
            }

            cbEventType.DataSource = Enum.GetValues<AuxiliaryTriggerEventType>();
            cbRetriggerMode.DataSource = Enum.GetValues<AuxiliaryTriggerRetriggerMode>();
            AuxiliaryTriggerEditorSupport.LoadAudioDevices(cbVolumeAudioDevice, null);
            AuxiliaryTriggerEditorSupport.LoadAudioDevices(cbSpectralAudioDevice, null);
            AuxiliaryTriggerEditorSupport.LoadMonitors(cbScreenMonitor);
            selectorDataLoaded = true;
        }

        private void LoadTrigger(AuxiliaryTriggerConfig trigger)
        {
            cbEventType.SelectedItem = trigger.EventType;
            cbRetriggerMode.SelectedItem = trigger.RetriggerMode;
            numOnDurationMs.Value = ClampToNumeric(numOnDurationMs, trigger.OnDurationMs);

            AuxiliaryTriggerEditorSupport.SelectAudioDevice(cbVolumeAudioDevice, trigger.Volume.AudioDeviceId);
            numVolumeThreshold.Value = ClampToNumeric(numVolumeThreshold, trigger.Volume.ThresholdPercent);

            AuxiliaryTriggerEditorSupport.SelectAudioDevice(cbSpectralAudioDevice, trigger.SpectralAnalysis.AudioDeviceId);
            numSpectralLowHz.Value = ClampToNumeric(numSpectralLowHz, trigger.SpectralAnalysis.FrequencyLowHz);
            numSpectralHighHz.Value = ClampToNumeric(numSpectralHighHz, trigger.SpectralAnalysis.FrequencyHighHz);
            numSpectralThresholdDb.Value = ClampToNumeric(numSpectralThresholdDb, trigger.SpectralAnalysis.ThresholdDb);

            AuxiliaryTriggerEditorSupport.SelectMonitor(cbScreenMonitor, trigger.ScreenCapture.MonitorIndex);
            numScreenX.Value = ClampToNumeric(numScreenX, trigger.ScreenCapture.X);
            numScreenY.Value = ClampToNumeric(numScreenY, trigger.ScreenCapture.Y);
            numScreenWidth.Value = ClampToNumeric(numScreenWidth, trigger.ScreenCapture.Width);
            numScreenHeight.Value = ClampToNumeric(numScreenHeight, trigger.ScreenCapture.Height);
            numScreenBrightnessThreshold.Value = ClampToNumeric(numScreenBrightnessThreshold, trigger.ScreenCapture.BrightnessThresholdPercent);
        }

        private void btnPickArea_Click(object? sender, EventArgs e)
        {
            Rectangle selected = AuxiliaryTriggerEditorSupport.PickScreenRectangle(
                this,
                AuxiliaryTriggerEditorSupport.GetSelectedMonitorIndex(cbScreenMonitor),
                new Rectangle((int)numScreenX.Value, (int)numScreenY.Value, (int)numScreenWidth.Value, (int)numScreenHeight.Value));

            numScreenX.Value = ClampToNumeric(numScreenX, selected.X);
            numScreenY.Value = ClampToNumeric(numScreenY, selected.Y);
            numScreenWidth.Value = ClampToNumeric(numScreenWidth, selected.Width);
            numScreenHeight.Value = ClampToNumeric(numScreenHeight, selected.Height);
        }

        private void btnTest_Click(object? sender, EventArgs e)
        {
            CommitTriggerToScene();
            if (!string.IsNullOrWhiteSpace(CurrentScene?.Id))
            {
                TestRequested?.Invoke(this, new StrobeTestRequestedEventArgs(CurrentScene.Id));
            }
        }

        private void TriggerControlChanged(object? sender, EventArgs e)
        {
            UpdateTriggerPanelVisibility();
            CommitTriggerToScene();
        }

        private void UpdateTriggerPanelVisibility()
        {
            AuxiliaryTriggerEventType eventType = cbEventType.SelectedItem is AuxiliaryTriggerEventType value
                ? value
                : AuxiliaryTriggerEventType.Volume;
            pnlVolume.Visible = eventType == AuxiliaryTriggerEventType.Volume;
            pnlSpectral.Visible = eventType == AuxiliaryTriggerEventType.SpectralAnalysis;
            pnlScreen.Visible = eventType == AuxiliaryTriggerEventType.ScreenCapture;
            bool holdMode = cbRetriggerMode.SelectedItem is AuxiliaryTriggerRetriggerMode mode
                && mode == AuxiliaryTriggerRetriggerMode.HoldWhileHigh;
            numOnDurationMs.Enabled = !holdMode;
        }

        private void CommitTriggerToScene()
        {
            if (isLoading || CurrentScene == null)
            {
                return;
            }

            AuxiliaryTriggerConfig trigger = CurrentScene.Strobe.Trigger;
            trigger.EventType = cbEventType.SelectedItem is AuxiliaryTriggerEventType eventType ? eventType : AuxiliaryTriggerEventType.Volume;
            trigger.RetriggerMode = cbRetriggerMode.SelectedItem is AuxiliaryTriggerRetriggerMode retriggerMode ? retriggerMode : AuxiliaryTriggerRetriggerMode.OneShotUntilReset;
            trigger.OnDurationMs = Math.Max(1, (int)numOnDurationMs.Value);

            trigger.Volume.AudioDeviceId = AuxiliaryTriggerEditorSupport.GetSelectedAudioDeviceId(cbVolumeAudioDevice) ?? string.Empty;
            trigger.Volume.ThresholdPercent = (int)numVolumeThreshold.Value;

            trigger.SpectralAnalysis.AudioDeviceId = AuxiliaryTriggerEditorSupport.GetSelectedAudioDeviceId(cbSpectralAudioDevice) ?? string.Empty;
            trigger.SpectralAnalysis.FrequencyLowHz = (double)numSpectralLowHz.Value;
            trigger.SpectralAnalysis.FrequencyHighHz = (double)numSpectralHighHz.Value;
            trigger.SpectralAnalysis.ThresholdDb = (double)numSpectralThresholdDb.Value;

            trigger.ScreenCapture.MonitorIndex = AuxiliaryTriggerEditorSupport.GetSelectedMonitorIndex(cbScreenMonitor);
            trigger.ScreenCapture.X = (int)numScreenX.Value;
            trigger.ScreenCapture.Y = (int)numScreenY.Value;
            trigger.ScreenCapture.Width = (int)numScreenWidth.Value;
            trigger.ScreenCapture.Height = (int)numScreenHeight.Value;
            trigger.ScreenCapture.BrightnessThresholdPercent = (int)numScreenBrightnessThreshold.Value;

            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private static void ConfigureNumeric(NumericUpDown numeric, decimal minimum, decimal maximum, decimal value, int width, int decimalPlaces = 0)
        {
            numeric.Minimum = minimum;
            numeric.Maximum = maximum;
            numeric.Value = Math.Max(minimum, Math.Min(maximum, value));
            numeric.Width = width;
            numeric.DecimalPlaces = decimalPlaces;
            numeric.Increment = decimalPlaces > 0 ? 0.5m : 1m;
        }

        private static decimal ClampToNumeric(NumericUpDown numeric, double value)
        {
            decimal decimalValue = (decimal)value;
            return Math.Max(numeric.Minimum, Math.Min(numeric.Maximum, decimalValue));
        }
    }

    public sealed class StrobeTestRequestedEventArgs : EventArgs
    {
        public StrobeTestRequestedEventArgs(string sceneId)
        {
            SceneId = sceneId;
        }

        public string SceneId { get; }
    }
}
