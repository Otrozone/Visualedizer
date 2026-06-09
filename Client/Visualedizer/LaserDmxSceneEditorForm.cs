using System.ComponentModel;

namespace Ledqualizer
{
    public sealed partial class LaserDmxSceneEditorForm : Form, ISceneEditorForm
    {
        private readonly BindingList<LaserChannelRowViewModel> rows = new();
        private bool isLoading;
        private bool selectorDataLoaded;

        public LaserDmxSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            InitializeGrid();
            EnsureSelectorDataLoaded();
            UpdateTriggerPanelVisibility();
        }

        public event EventHandler? SceneChanged;
        public event EventHandler<LaserDmxSendRequestedEventArgs>? SendRequested;

        public SceneType SceneType => SceneType.LaserDmx;

        public SceneConfig? CurrentScene { get; private set; }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                EnsureSelectorDataLoaded();
                LoadTrigger(scene.LaserDmx.Trigger);

                rows.Clear();
                foreach (LaserDmxChannelRow channel in scene.LaserDmx.Channels)
                {
                    rows.Add(LaserChannelRowViewModel.FromModel(channel));
                }
            }
            finally
            {
                isLoading = false;
            }

            UpdateTriggerPanelVisibility();
        }

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

        private void InitializeGrid()
        {
            dgvChannels.AutoGenerateColumns = false;
            dgvChannels.AllowUserToAddRows = false;
            dgvChannels.AllowUserToDeleteRows = false;
            dgvChannels.Dock = DockStyle.Fill;
            dgvChannels.DataSource = rows;
            dgvChannels.RowHeadersVisible = false;
            dgvChannels.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvChannels.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(LaserChannelRowViewModel.Channel),
                HeaderText = "Channel",
                Width = 70
            });

            dgvChannels.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(LaserChannelRowViewModel.Mode),
                HeaderText = "Mode",
                Width = 150,
                DataSource = Enum.GetValues<LaserDmxValueMode>()
            });

            dgvChannels.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(LaserChannelRowViewModel.ConstantValue),
                HeaderText = "Constant",
                Width = 75
            });

            dgvChannels.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(LaserChannelRowViewModel.RangeMin),
                HeaderText = "Min",
                Width = 55
            });

            dgvChannels.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(LaserChannelRowViewModel.RangeMax),
                HeaderText = "Max",
                Width = 55
            });

            dgvChannels.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(LaserChannelRowViewModel.ValuesText),
                HeaderText = "Values",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvChannels.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(LaserChannelRowViewModel.RefreshEnabled),
                HeaderText = "Refresh",
                Width = 65
            });

            dgvChannels.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(LaserChannelRowViewModel.RefreshIntervalSeconds),
                HeaderText = "Refresh s",
                Width = 80
            });

            dgvChannels.CellValueChanged += dgvChannels_CellValueChanged;
            dgvChannels.CurrentCellDirtyStateChanged += dgvChannels_CurrentCellDirtyStateChanged;
            dgvChannels.DataError += (_, _) => { };
        }

        private void btnAddRow_Click(object? sender, EventArgs e)
        {
            rows.Add(new LaserChannelRowViewModel());
            CommitRowsToScene();
        }

        private void btnRemoveRow_Click(object? sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvChannels.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(row => row.Index))
            {
                if (row.DataBoundItem is LaserChannelRowViewModel item)
                {
                    rows.Remove(item);
                }
            }

            CommitRowsToScene();
        }

        private void btnSend_Click(object? sender, EventArgs e)
        {
            CommitAll();
            if (!string.IsNullOrWhiteSpace(CurrentScene?.Id))
            {
                SendRequested?.Invoke(this, new LaserDmxSendRequestedEventArgs(CurrentScene.Id));
            }
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

        private void dgvChannels_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvChannels.IsCurrentCellDirty)
            {
                dgvChannels.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvChannels_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            CommitRowsToScene();
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

        private void CommitAll()
        {
            CommitTriggerToScene();
            CommitRowsToScene();
        }

        private void CommitTriggerToScene()
        {
            if (isLoading || CurrentScene == null)
            {
                return;
            }

            AuxiliaryTriggerConfig trigger = CurrentScene.LaserDmx.Trigger;
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

        private void CommitRowsToScene()
        {
            if (isLoading || CurrentScene == null)
            {
                return;
            }

            CurrentScene.LaserDmx.Channels = rows.Select(row => row.ToModel()).ToList();
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private static decimal ClampToNumeric(NumericUpDown numeric, double value)
        {
            decimal decimalValue = (decimal)value;
            return Math.Max(numeric.Minimum, Math.Min(numeric.Maximum, decimalValue));
        }

        private sealed class LaserChannelRowViewModel
        {
            public int Channel { get; set; } = 1;
            public LaserDmxValueMode Mode { get; set; } = LaserDmxValueMode.Constant;
            public int ConstantValue { get; set; }
            public int RangeMin { get; set; }
            public int RangeMax { get; set; } = 255;
            public string ValuesText { get; set; } = string.Empty;
            public bool RefreshEnabled { get; set; }
            public double RefreshIntervalSeconds { get; set; } = 1.0;

            public LaserDmxChannelRow ToModel()
            {
                List<int> values = ValuesText
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(item => int.TryParse(item, out int parsed) ? Math.Clamp(parsed, 0, 255) : -1)
                    .Where(item => item >= 0)
                    .ToList();

                int rangeMin = Math.Clamp(RangeMin, 0, 255);
                int rangeMax = Math.Clamp(RangeMax, 0, 255);
                if (rangeMax < rangeMin)
                {
                    (rangeMin, rangeMax) = (rangeMax, rangeMin);
                }

                return new LaserDmxChannelRow
                {
                    Channel = Math.Clamp(Channel, 1, 512),
                    Mode = Mode,
                    ConstantValue = Math.Clamp(ConstantValue, 0, 255),
                    RangeMin = rangeMin,
                    RangeMax = rangeMax,
                    Values = values,
                    RefreshEnabled = RefreshEnabled,
                    RefreshIntervalSeconds = Math.Max(0.1, RefreshIntervalSeconds)
                };
            }

            public static LaserChannelRowViewModel FromModel(LaserDmxChannelRow model)
            {
                return new LaserChannelRowViewModel
                {
                    Channel = model.Channel,
                    Mode = model.Mode,
                    ConstantValue = model.ConstantValue,
                    RangeMin = model.RangeMin,
                    RangeMax = model.RangeMax,
                    ValuesText = string.Join(", ", model.Values),
                    RefreshEnabled = model.RefreshEnabled,
                    RefreshIntervalSeconds = model.RefreshIntervalSeconds
                };
            }
        }
    }

    public sealed class LaserDmxSendRequestedEventArgs : EventArgs
    {
        public LaserDmxSendRequestedEventArgs(string sceneId)
        {
            SceneId = sceneId;
        }

        public string SceneId { get; }
    }
}
