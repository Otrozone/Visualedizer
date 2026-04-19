using NAudio.CoreAudioApi;
using System.ComponentModel;
using Visualedizer;
using static Ledqualizer.AcVolume;
using static Ledqualizer.ScreenCapture;

namespace Ledqualizer
{
    public partial class FrmMain : Form
    {
        private sealed class DeviceRunEntry
        {
            public DeviceConfig Config { get; }
            public RunController Controller { get; }

            public DeviceRunEntry(DeviceConfig config, RunController controller)
            {
                Config = config;
                Controller = controller;
            }
        }

        private readonly AppConfig appConfig = new();
        private readonly BindingList<DeviceGridRow> deviceRows = new();
        private readonly Dictionary<string, DeviceRunEntry> deviceRuns = new();
        private readonly List<Color> previewColors = new();

        private bool isLoading;
        private bool suppressTabRestart;
        private bool reconcileInProgress;
        private bool reconcileRequested;
        private bool syncingAudioDeviceSelection;
        private int rotateIdx;
        private RadioButton[] rotateModeRadios = Array.Empty<RadioButton>();
        private string? selectedAudioDeviceId;
        private AcVolume.AudioCaptureVolumeMode audioCaptureVolumeMode;
        private FormOverlay? frmOverlay;
        private ComboBox cbAudioDevicesSpectral = null!;
        private ProgressBar progressBarSpectral = null!;
        private TrackBar trackBarSpectralFrequencyLow = null!;
        private TrackBar trackBarSpectralFrequencyHigh = null!;
        private TrackBar trackBarSpectralLevelLow = null!;
        private TrackBar trackBarSpectralLevelHigh = null!;
        private Label lblSpectralFrequencyLowValue = null!;
        private Label lblSpectralFrequencyHighValue = null!;
        private Label lblSpectralLevelLowValue = null!;
        private Label lblSpectralLevelHighValue = null!;

        public class FormOverlay : Form
        {
            public FormOverlay(Rectangle rectangle)
            {
                FormBorderStyle = FormBorderStyle.None;
                BackColor = Color.WhiteSmoke;
                Opacity = 0.5;
                TopMost = true;
                StartPosition = FormStartPosition.Manual;
                Location = rectangle.Location;
                Size = rectangle.Size;

                Paint += (sender, e) =>
                {
                    using Pen redPen = new Pen(Color.Red, 1);
                    e.Graphics.DrawRectangle(redPen, new Rectangle(0, 17, Width - 1, 3));
                };
            }
        }

        public FrmMain()
        {
            InitializeComponent();
            InitializeSpectralAnalysisUi();
            ConfigureDevicePanel();
            pictureBox.Paint += PictureBox_Paint;
            cbAudioDevices.SelectedIndexChanged += CbAudioDevices_SelectedIndexChanged;
            cbAudioDevicesSpectral.SelectedIndexChanged += CbAudioDevices_SelectedIndexChanged;
        }

        private void InitializeSpectralAnalysisUi()
        {
            Label lblHint = new()
            {
                AutoSize = true,
                Location = new Point(16, 16),
                Text = "Band detection drives the same LED modes and colors configured on the Volume tab."
            };

            Label lblAudioDeviceSpectral = new()
            {
                AutoSize = true,
                Location = new Point(16, 52),
                Text = "Audio device"
            };

            cbAudioDevicesSpectral = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(16, 70),
                Size = new Size(520, 23)
            };

            Label lblFrequencyLow = new()
            {
                AutoSize = true,
                Location = new Point(16, 112),
                Text = "Frequency low (Hz)"
            };

            trackBarSpectralFrequencyLow = new TrackBar
            {
                AutoSize = false,
                Location = new Point(16, 132),
                Minimum = 20,
                Maximum = 20000,
                TickFrequency = 1000,
                LargeChange = 200,
                SmallChange = 10,
                Size = new Size(264, 32),
                Value = 60
            };

            lblSpectralFrequencyLowValue = new Label
            {
                AutoSize = true,
                Location = new Point(290, 137),
                Text = "60 Hz"
            };

            Label lblFrequencyHigh = new()
            {
                AutoSize = true,
                Location = new Point(16, 176),
                Text = "Frequency high (Hz)"
            };

            trackBarSpectralFrequencyHigh = new TrackBar
            {
                AutoSize = false,
                Location = new Point(16, 196),
                Minimum = 20,
                Maximum = 20000,
                TickFrequency = 1000,
                LargeChange = 200,
                SmallChange = 10,
                Size = new Size(264, 32),
                Value = 250
            };

            lblSpectralFrequencyHighValue = new Label
            {
                AutoSize = true,
                Location = new Point(290, 201),
                Text = "250 Hz"
            };

            Label lblLevelLow = new()
            {
                AutoSize = true,
                Location = new Point(16, 240),
                Text = "Level low (dB)"
            };

            trackBarSpectralLevelLow = new TrackBar
            {
                AutoSize = false,
                Location = new Point(16, 260),
                Minimum = -90,
                Maximum = 0,
                TickFrequency = 10,
                LargeChange = 5,
                SmallChange = 1,
                Size = new Size(264, 32),
                Value = -60
            };

            lblSpectralLevelLowValue = new Label
            {
                AutoSize = true,
                Location = new Point(290, 265),
                Text = "-60 dB"
            };

            Label lblLevelHigh = new()
            {
                AutoSize = true,
                Location = new Point(16, 304),
                Text = "Level high (dB)"
            };

            trackBarSpectralLevelHigh = new TrackBar
            {
                AutoSize = false,
                Location = new Point(16, 324),
                Minimum = -90,
                Maximum = 0,
                TickFrequency = 10,
                LargeChange = 5,
                SmallChange = 1,
                Size = new Size(264, 32),
                Value = -20
            };

            lblSpectralLevelHighValue = new Label
            {
                AutoSize = true,
                Location = new Point(290, 329),
                Text = "-20 dB"
            };

            Label lblLevelHelp = new()
            {
                AutoSize = true,
                Location = new Point(16, 370),
                Text = "Below low the strip stays idle; at high and above it reaches full response."
            };

            progressBarSpectral = new ProgressBar
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(8, 542),
                Name = "progressBarSpectral",
                Size = new Size(785, 24),
                Style = ProgressBarStyle.Continuous
            };

            tabPageAcSpectralAnalysis.Controls.Add(lblHint);
            tabPageAcSpectralAnalysis.Controls.Add(lblAudioDeviceSpectral);
            tabPageAcSpectralAnalysis.Controls.Add(cbAudioDevicesSpectral);
            tabPageAcSpectralAnalysis.Controls.Add(lblFrequencyLow);
            tabPageAcSpectralAnalysis.Controls.Add(trackBarSpectralFrequencyLow);
            tabPageAcSpectralAnalysis.Controls.Add(lblSpectralFrequencyLowValue);
            tabPageAcSpectralAnalysis.Controls.Add(lblFrequencyHigh);
            tabPageAcSpectralAnalysis.Controls.Add(trackBarSpectralFrequencyHigh);
            tabPageAcSpectralAnalysis.Controls.Add(lblSpectralFrequencyHighValue);
            tabPageAcSpectralAnalysis.Controls.Add(lblLevelLow);
            tabPageAcSpectralAnalysis.Controls.Add(trackBarSpectralLevelLow);
            tabPageAcSpectralAnalysis.Controls.Add(lblSpectralLevelLowValue);
            tabPageAcSpectralAnalysis.Controls.Add(lblLevelHigh);
            tabPageAcSpectralAnalysis.Controls.Add(trackBarSpectralLevelHigh);
            tabPageAcSpectralAnalysis.Controls.Add(lblSpectralLevelHighValue);
            tabPageAcSpectralAnalysis.Controls.Add(lblLevelHelp);
            tabPageAcSpectralAnalysis.Controls.Add(progressBarSpectral);

            trackBarSpectralFrequencyLow.ValueChanged += SpectralTrackBar_ValueChanged;
            trackBarSpectralFrequencyHigh.ValueChanged += SpectralTrackBar_ValueChanged;
            trackBarSpectralLevelLow.ValueChanged += SpectralTrackBar_ValueChanged;
            trackBarSpectralLevelHigh.ValueChanged += SpectralTrackBar_ValueChanged;
            UpdateSpectralTrackBarLabels();
        }

        private void ConfigureDevicePanel()
        {
            dgvDevices.AutoGenerateColumns = false;
            dgvDevices.RowHeadersVisible = false;
            dgvDevices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDevices.MultiSelect = true;
            dgvDevices.DataSource = deviceRows;
            dgvDevices.CurrentCellDirtyStateChanged += dgvDevices_CurrentCellDirtyStateChanged;
            dgvDevices.CellValidating += dgvDevices_CellValidating;
            dgvDevices.DataError += dgvDevices_DataError;
            dgvDevices.CellValueChanged += dgvDevices_CellValueChanged;

            if (dgvDevices.Columns[nameof(colScene)] is DataGridViewComboBoxColumn sceneColumn)
            {
                sceneColumn.DataSource = Enum.GetValues(typeof(SceneKind));
            }
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            isLoading = true;

            hsbScreenRowSelector.Maximum = ScreenCapture.GetScreenHeight();
            numScreenRow.Maximum = hsbScreenRowSelector.Maximum;
            rotateModeRadios = new[] { rbModeColorPush, rbModeEndToStart, rbModeMidToOut, rbModeMidToOutPoint, rbModeStartToEnd };
            audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeStartToEnd;

            appConfig.LoadFromIni();
            ApplyConfigToUi();

            if (tabControl.SelectedTab == tabPageAcVolume || tabControl.SelectedTab == tabPageAcSpectralAnalysis)
            {
                LoadAudioDevices();
            }

            CountHz();
            isLoading = false;

            await ReconcileDeviceRunsAsync();
        }

        private void ApplyConfigToUi()
        {
            numDelay.Value = Math.Max(numDelay.Minimum, Math.Min(numDelay.Maximum, (decimal)appConfig.Delay));
            numScreenRow.Value = Math.Max(numScreenRow.Minimum, Math.Min(numScreenRow.Maximum, (decimal)appConfig.ScreenCaptureRow));
            trackBarBrightness.Value = Math.Max(trackBarBrightness.Minimum, Math.Min(trackBarBrightness.Maximum, (int)Math.Round(appConfig.Brightness * 100)));
            trackBarNormalizationLevel.Value = Math.Max(trackBarNormalizationLevel.Minimum, Math.Min(trackBarNormalizationLevel.Maximum, (int)Math.Round(appConfig.NormalizationLevel * 10)));
            trackBarSpectralFrequencyLow.Value = Math.Max(trackBarSpectralFrequencyLow.Minimum, Math.Min(trackBarSpectralFrequencyLow.Maximum, appConfig.SpectralFrequencyLow));
            trackBarSpectralFrequencyHigh.Value = Math.Max(trackBarSpectralFrequencyHigh.Minimum, Math.Min(trackBarSpectralFrequencyHigh.Maximum, appConfig.SpectralFrequencyHigh));
            trackBarSpectralLevelLow.Value = Math.Max(trackBarSpectralLevelLow.Minimum, Math.Min(trackBarSpectralLevelLow.Maximum, appConfig.SpectralLevelLowDb));
            trackBarSpectralLevelHigh.Value = Math.Max(trackBarSpectralLevelHigh.Minimum, Math.Min(trackBarSpectralLevelHigh.Maximum, appConfig.SpectralLevelHighDb));
            UpdateSpectralTrackBarLabels();

            numStrobeX.Value = appConfig.StrobeTriggerX;
            numStrobeY.Value = appConfig.StrobeTriggerY;
            numLaserTriggerX.Value = appConfig.LaserTriggerX;
            numLaserTriggerY.Value = appConfig.LaserTriggerY;
            numLaserPatternX.Value = appConfig.LaserPatternX;
            numLaserPatternY.Value = appConfig.LaserPatternY;
            numLaserColorX.Value = appConfig.LaserColorX;
            numLaserColorY.Value = appConfig.LaserColorY;

            deviceRows.Clear();
            foreach (DeviceConfig device in appConfig.Devices)
            {
                deviceRows.Add(DeviceGridRow.FromDeviceConfig(device));
            }
        }

        private void SyncConfigFromUi()
        {
            appConfig.Delay = (int)numDelay.Value;
            appConfig.ScreenCaptureRow = (int)numScreenRow.Value;
            appConfig.NormalizationLevel = trackBarNormalizationLevel.Value / 10.0f;
            appConfig.Brightness = trackBarBrightness.Value / (float)Math.Max(trackBarBrightness.Maximum, 1);
            appConfig.SpectralFrequencyLow = trackBarSpectralFrequencyLow.Value;
            appConfig.SpectralFrequencyHigh = trackBarSpectralFrequencyHigh.Value;
            appConfig.SpectralLevelLowDb = trackBarSpectralLevelLow.Value;
            appConfig.SpectralLevelHighDb = trackBarSpectralLevelHigh.Value;

            appConfig.StrobeTriggerX = (int)numStrobeX.Value;
            appConfig.StrobeTriggerY = (int)numStrobeY.Value;
            appConfig.LaserTriggerX = (int)numLaserTriggerX.Value;
            appConfig.LaserTriggerY = (int)numLaserTriggerY.Value;
            appConfig.LaserPatternX = (int)numLaserPatternX.Value;
            appConfig.LaserPatternY = (int)numLaserPatternY.Value;
            appConfig.LaserColorX = (int)numLaserColorX.Value;
            appConfig.LaserColorY = (int)numLaserColorY.Value;

            appConfig.Devices = deviceRows.Select(row => row.ToDeviceConfig()).ToList();
        }

        private async Task ReconcileDeviceRunsAsync()
        {
            if (isLoading)
            {
                return;
            }

            if (reconcileInProgress)
            {
                reconcileRequested = true;
                return;
            }

            reconcileInProgress = true;

            try
            {
                do
                {
                    reconcileRequested = false;

                    Dictionary<string, DeviceConfig> desiredDevices = deviceRows
                        .Where(row => row.Enabled && IsValidDeviceRow(row))
                        .Select(row => row.ToDeviceConfig())
                        .ToDictionary(device => device.Id, device => device);

                    List<string> activeIds = deviceRuns.Keys.ToList();
                    foreach (string deviceId in activeIds)
                    {
                        if (!desiredDevices.TryGetValue(deviceId, out DeviceConfig? desiredConfig))
                        {
                            await StopDeviceRunAsync(deviceId);
                            continue;
                        }

                        DeviceRunEntry current = deviceRuns[deviceId];
                        if (RequiresRestart(current.Config, desiredConfig))
                        {
                            await StopDeviceRunAsync(deviceId);
                            await StartDeviceRunAsync(desiredConfig);
                        }
                    }

                    foreach (DeviceConfig desiredConfig in desiredDevices.Values)
                    {
                        if (!deviceRuns.ContainsKey(desiredConfig.Id))
                        {
                            await StartDeviceRunAsync(desiredConfig);
                        }
                    }

                    UpdateConnectionSummary();
                }
                while (reconcileRequested);
            }
            finally
            {
                reconcileInProgress = false;
            }
        }

        private async Task StartDeviceRunAsync(DeviceConfig config)
        {
            var controller = new RunController();
            controller.DeviceStatusChanged += RunController_DeviceStatusChanged;
            deviceRuns[config.Id] = new DeviceRunEntry(config, controller);

            await controller.StartAsync(new[] { config }, CreateSceneRunner(config.Scene));
        }

        private async Task StopDeviceRunAsync(string deviceId)
        {
            if (!deviceRuns.TryGetValue(deviceId, out DeviceRunEntry? entry))
            {
                return;
            }

            deviceRuns.Remove(deviceId);
            entry.Controller.DeviceStatusChanged -= RunController_DeviceStatusChanged;
            await entry.Controller.StopAsync();
        }

        private async Task StopAllDeviceRunsAsync()
        {
            List<string> deviceIds = deviceRuns.Keys.ToList();
            foreach (string deviceId in deviceIds)
            {
                await StopDeviceRunAsync(deviceId);
            }
        }

        private static bool RequiresRestart(DeviceConfig current, DeviceConfig desired)
        {
            return !string.Equals(current.Host, desired.Host, StringComparison.OrdinalIgnoreCase)
                || current.Port != desired.Port
                || current.LedCount != desired.LedCount
                || current.Scene != desired.Scene;
        }

        private ISceneRunner CreateSceneRunner(SceneKind scene)
        {
            return scene switch
            {
                SceneKind.Volume => new VolumeSceneRunner(
                    GetVolumeSceneSettings,
                    () => selectedAudioDeviceId,
                    UpdateProgress,
                    UpdateRate),
                SceneKind.SpectralAnalysis => new SpectralSceneRunner(
                    GetSpectralSceneSettings,
                    () => selectedAudioDeviceId,
                    UpdateSpectralProgress,
                    UpdateRate),
                SceneKind.ScreenCapture => new ScreenCaptureSceneRunner(GetScreenCaptureSceneSettings, UpdatePreview),
                SceneKind.OtherDevices => new OtherDevicesSceneRunner(GetOtherDevicesSceneSettings),
                _ => new BasicSceneRunner(GetBasicSceneSettings),
            };
        }

        private BasicSceneSettings GetBasicSceneSettings()
        {
            return ReadUi(() => new BasicSceneSettings
            {
                Solid = rbSolid.Checked,
                SolidHue = ucHueSolid.Hue,
                SolidMinHue = ucHueSolid.MinVal,
                SolidMaxHue = ucHueSolid.MaxVal,
                SaturationValue = trackSaturationBasic.Value,
                SaturationMinimum = trackSaturationBasic.Minimum,
                SaturationMaximum = trackSaturationBasic.Maximum,
                BrightnessValue = trackBrightnessBasic.Value,
                BrightnessMaximum = trackBrightnessBasic.Maximum,
                GradientHueMin = ucHueMinMaxGradient.HueMin,
                GradientHueMax = ucHueMinMaxGradient.HueMax,
                Delay = (int)numDelay.Value
            });
        }

        private VolumeSceneSettings GetVolumeSceneSettings()
        {
            return ReadUi(() => new VolumeSceneSettings
            {
                Mode = audioCaptureVolumeMode,
                Delay = (int)numDelay.Value,
                BrightnessValue = trackBarBrightness.Value,
                BrightnessMaximum = trackBarBrightness.Maximum,
                NormalizationValue = trackBarNormalizationLevel.Value,
                Reverse = chbRevers.Checked,
                HueReverse = chbHueRevers.Checked,
                White = chbWhite.Checked,
                BackgroundWhite = chbBgWhite.Checked,
                BackgroundBrightnessValue = trackBarBgBrightness.Value,
                BackgroundHue = ucHueBg.Hue,
                HueMin = ucHueMinMax.HueMin,
                HueMax = ucHueMinMax.HueMax
            });
        }

        private SpectralSceneSettings GetSpectralSceneSettings()
        {
            return ReadUi(() => new SpectralSceneSettings
            {
                Mode = audioCaptureVolumeMode,
                Delay = (int)numDelay.Value,
                BrightnessValue = trackBarBrightness.Value,
                BrightnessMaximum = trackBarBrightness.Maximum,
                NormalizationValue = trackBarNormalizationLevel.Value,
                Reverse = chbRevers.Checked,
                HueReverse = chbHueRevers.Checked,
                White = chbWhite.Checked,
                BackgroundWhite = chbBgWhite.Checked,
                BackgroundBrightnessValue = trackBarBgBrightness.Value,
                BackgroundHue = ucHueBg.Hue,
                HueMin = ucHueMinMax.HueMin,
                HueMax = ucHueMinMax.HueMax,
                FrequencyLowHz = trackBarSpectralFrequencyLow.Value,
                FrequencyHighHz = trackBarSpectralFrequencyHigh.Value,
                LevelLowDb = trackBarSpectralLevelLow.Value,
                LevelHighDb = trackBarSpectralLevelHigh.Value
            });
        }

        private void SpectralTrackBar_ValueChanged(object? sender, EventArgs e)
        {
            UpdateSpectralTrackBarLabels();
        }

        private void UpdateSpectralTrackBarLabels()
        {
            lblSpectralFrequencyLowValue.Text = $"{trackBarSpectralFrequencyLow.Value} Hz";
            lblSpectralFrequencyHighValue.Text = $"{trackBarSpectralFrequencyHigh.Value} Hz";
            lblSpectralLevelLowValue.Text = $"{trackBarSpectralLevelLow.Value} dB";
            lblSpectralLevelHighValue.Text = $"{trackBarSpectralLevelHigh.Value} dB";
        }

        private ScreenCaptureSceneSettings GetScreenCaptureSceneSettings()
        {
            return ReadUi(() => new ScreenCaptureSceneSettings
            {
                Delay = (int)numDelay.Value,
                CaptureY = (int)numScreenRow.Value,
                Reverse = chbReverse.Checked
            });
        }

        private OtherDevicesSceneSettings GetOtherDevicesSceneSettings()
        {
            return ReadUi(() => new OtherDevicesSceneSettings
            {
                Delay = (int)numDelay.Value,
                StrobeTriggerX = (int)numStrobeX.Value,
                StrobeTriggerY = (int)numStrobeY.Value,
                LaserTriggerX = (int)numLaserTriggerX.Value,
                LaserTriggerY = (int)numLaserTriggerY.Value,
                LaserPatternX = (int)numLaserPatternX.Value,
                LaserPatternY = (int)numLaserPatternY.Value,
                LaserColorX = (int)numLaserColorX.Value,
                LaserColorY = (int)numLaserColorY.Value
            });
        }

        private void UpdatePreview(IReadOnlyList<Color> colors)
        {
            SafeUi(() =>
            {
                previewColors.Clear();
                previewColors.AddRange(colors);
                pictureBox.Invalidate();
            });
        }

        private void UpdateProgress(int value)
        {
            SafeUi(() => progressBar.Value = Math.Max(progressBar.Minimum, Math.Min(progressBar.Maximum, value)));
        }

        private void UpdateSpectralProgress(int value)
        {
            SafeUi(() => progressBarSpectral.Value = Math.Max(progressBarSpectral.Minimum, Math.Min(progressBarSpectral.Maximum, value)));
        }

        private void UpdateRate(string text)
        {
            SafeUi(() => statusStrip.Items[0].Text = text);
        }

        private void PictureBox_Paint(object? sender, PaintEventArgs e)
        {
            if (previewColors.Count == 0 || sender is not PictureBox previewBox)
            {
                return;
            }

            int pictureBoxWidth = previewBox.Width;
            int segmentCount = previewColors.Count;
            int segmentWidth = Math.Max(pictureBoxWidth / segmentCount, 1);

            for (int i = 0; i < segmentCount; i++)
            {
                using Brush brush = new SolidBrush(previewColors[i]);
                int x = i * segmentWidth;
                int width = i == segmentCount - 1 ? pictureBoxWidth - (i * segmentWidth) : segmentWidth;
                e.Graphics.FillRectangle(brush, x, 0, width, previewBox.Height);
            }
        }

        private void RunController_DeviceStatusChanged(string deviceId, ConnectionState state, string? detail)
        {
            SafeUi(() =>
            {
                DeviceGridRow? row = deviceRows.FirstOrDefault(item => item.Id == deviceId);
                if (row == null)
                {
                    return;
                }

                row.Status = state switch
                {
                    ConnectionState.Connecting => "Connecting",
                    ConnectionState.Connected => "Connected",
                    ConnectionState.Faulted => string.IsNullOrWhiteSpace(detail) ? "Offline" : $"Offline: {detail}",
                    _ => "Disconnected"
                };

                dgvDevices.Refresh();
                UpdateConnectionSummary();
            });
        }

        private void UpdateConnectionSummary()
        {
            int connectedCount = deviceRows.Count(item => item.Status == "Connected");
            int enabledCount = deviceRows.Count(item => item.Enabled && IsValidDeviceRow(item));
            int offlineCount = deviceRows.Count(item => item.Enabled && item.Status.StartsWith("Offline", StringComparison.OrdinalIgnoreCase));

            if (enabledCount == 0)
            {
                statLblConnection.Text = "No enabled devices";
                return;
            }

            statLblConnection.Text = $"{connectedCount}/{enabledCount} online";
            if (offlineCount > 0)
            {
                statLblConnection.Text += $" ({offlineCount} offline)";
            }
        }

        private void LoadAudioDevices()
        {
            suppressTabRestart = true;
            try
            {
                AcVolume.LoadAudioDevicesToComboBox(cbAudioDevices);
                AcVolume.LoadAudioDevicesToComboBox(cbAudioDevicesSpectral);

                if (!string.IsNullOrWhiteSpace(selectedAudioDeviceId))
                {
                    TrySelectAudioDevice(cbAudioDevices, selectedAudioDeviceId);
                    TrySelectAudioDevice(cbAudioDevicesSpectral, selectedAudioDeviceId);
                }

                selectedAudioDeviceId = (cbAudioDevices.SelectedItem as DeviceDescriptor)?.DeviceId
                    ?? (cbAudioDevicesSpectral.SelectedItem as DeviceDescriptor)?.DeviceId;
            }
            finally
            {
                suppressTabRestart = false;
            }
        }

        private void CbAudioDevices_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (syncingAudioDeviceSelection)
            {
                return;
            }

            if (sender is not ComboBox comboBox)
            {
                return;
            }

            selectedAudioDeviceId = (comboBox.SelectedItem as DeviceDescriptor)?.DeviceId;

            syncingAudioDeviceSelection = true;
            try
            {
                if (!ReferenceEquals(comboBox, cbAudioDevices))
                {
                    TrySelectAudioDevice(cbAudioDevices, selectedAudioDeviceId);
                }

                if (!ReferenceEquals(comboBox, cbAudioDevicesSpectral))
                {
                    TrySelectAudioDevice(cbAudioDevicesSpectral, selectedAudioDeviceId);
                }
            }
            finally
            {
                syncingAudioDeviceSelection = false;
            }
        }

        private static void TrySelectAudioDevice(ComboBox comboBox, string? deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is DeviceDescriptor descriptor && string.Equals(descriptor.DeviceId, deviceId, StringComparison.Ordinal))
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }
        }

        private async void btnAddDevice_Click(object? sender, EventArgs e)
        {
            int deviceNumber = deviceRows.Count + 1;
            deviceRows.Add(new DeviceGridRow
            {
                Id = Guid.NewGuid().ToString("N"),
                Enabled = false,
                Name = $"Device {deviceNumber}",
                Host = "127.0.0.1",
                Port = 81,
                LedCount = 218,
                Scene = GetCurrentSceneKind(),
                Status = "Disconnected"
            });

            await ReconcileDeviceRunsAsync();
        }

        private async void btnRemoveDevice_Click(object? sender, EventArgs e)
        {
            var rowsToRemove = dgvDevices.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(row => row.DataBoundItem as DeviceGridRow)
                .Where(row => row != null)
                .ToList();

            foreach (DeviceGridRow? row in rowsToRemove)
            {
                if (row != null)
                {
                    deviceRows.Remove(row);
                }
            }

            await ReconcileDeviceRunsAsync();
        }

        private bool IsValidDeviceRow(DeviceGridRow row)
        {
            return !string.IsNullOrWhiteSpace(row.Name)
                && !string.IsNullOrWhiteSpace(row.Host)
                && row.Port > 0
                && row.Port <= 65535
                && row.LedCount > 0;
        }

        private void dgvDevices_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvDevices.IsCurrentCellDirty)
            {
                dgvDevices.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private async void dgvDevices_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (isLoading || e.RowIndex < 0)
            {
                return;
            }

            await ReconcileDeviceRunsAsync();
        }

        private void dgvDevices_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            string propertyName = dgvDevices.Columns[e.ColumnIndex].DataPropertyName;
            string formattedValue = e.FormattedValue?.ToString() ?? string.Empty;

            if (propertyName == nameof(DeviceGridRow.Name) || propertyName == nameof(DeviceGridRow.Host))
            {
                if (string.IsNullOrWhiteSpace(formattedValue))
                {
                    e.Cancel = true;
                    dgvDevices.Rows[e.RowIndex].ErrorText = $"{propertyName} is required.";
                }
            }

            if (propertyName == nameof(DeviceGridRow.Port))
            {
                if (!int.TryParse(formattedValue, out int port) || port <= 0 || port > 65535)
                {
                    e.Cancel = true;
                    dgvDevices.Rows[e.RowIndex].ErrorText = "Port must be between 1 and 65535.";
                }
            }

            if (propertyName == nameof(DeviceGridRow.LedCount))
            {
                if (!int.TryParse(formattedValue, out int ledCount) || ledCount <= 0)
                {
                    e.Cancel = true;
                    dgvDevices.Rows[e.RowIndex].ErrorText = "LED count must be greater than zero.";
                }
            }

            if (!e.Cancel)
            {
                dgvDevices.Rows[e.RowIndex].ErrorText = string.Empty;
            }
        }

        private void dgvDevices_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void CountHz()
        {
            if (numDelay.Value > 0)
            {
                lblRefreshRate.Text = (1000m / numDelay.Value).ToString("F1") + " Hz";
            }
        }

        private void chbShowGuide_CheckedChanged(object sender, EventArgs e)
        {
            if (chbShowGuide.Checked)
            {
                ShowOverlayForm((int)numScreenRow.Value);
            }
            else
            {
                CloseOverlayForm();
            }
        }

        public void ShowOverlayForm(int y)
        {
            Rectangle captureArea = new Rectangle(0, y - 18, GetScreenWidth() - 1, 3);
            frmOverlay = new FormOverlay(captureArea);
            frmOverlay.Show();
        }

        public void CloseOverlayForm()
        {
            if (frmOverlay == null)
            {
                return;
            }

            frmOverlay.Close();
            frmOverlay.Dispose();
            frmOverlay = null;
        }

        private void hsbScreenRowSelector_Scroll(object sender, ScrollEventArgs e)
        {
            numScreenRow.Value = hsbScreenRowSelector.Value;

            if (frmOverlay != null)
            {
                frmOverlay.Location = new Point(0, (int)numScreenRow.Value);
            }
        }

        private void numDelay_ValueChanged(object sender, EventArgs e)
        {
            if (isLoading)
            {
                return;
            }

            appConfig.Delay = (int)numDelay.Value;
            CountHz();
        }

        private void rbMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbModeStartToEnd.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeStartToEnd;
            }
            if (rbModeEndToStart.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeEndToStart;
            }
            if (rbModeMidToOut.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeMidToOut;
            }
            if (rbModeColorPush.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeColorPush;
            }
            if (rbModeMidToOutPoint.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeMidToOut_Point;
            }
            if (rbBrightness.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeBrightness;
            }
        }

        private void chbWhite_CheckedChanged(object sender, EventArgs e)
        {
            ucHueMinMax.Enabled = !chbWhite.Checked;
        }

        private void chbRotate_CheckedChanged(object sender, EventArgs e)
        {
            timerRotate.Enabled = chbRotate.Checked;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            timerRotate.Interval = trackBarRotate.Value * 1000;
        }

        private void timerRotate_Tick(object sender, EventArgs e)
        {
            if (rotateIdx >= rotateModeRadios.Length)
            {
                rotateIdx = 0;
            }

            rotateModeRadios[rotateIdx].Checked = true;
            rotateIdx++;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressTabRestart)
            {
                return;
            }

            if (tabControl.SelectedTab == tabPageAcVolume || tabControl.SelectedTab == tabPageAcSpectralAnalysis)
            {
                LoadAudioDevices();
            }
        }

        private void rbBasic_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton[] radioButtonGroup = { rbSolid, rbGradient };

            if (sender is not RadioButton changedRadioButton || !changedRadioButton.Checked)
            {
                return;
            }

            foreach (RadioButton radioButton in radioButtonGroup)
            {
                if (radioButton != changedRadioButton)
                {
                    radioButton.Checked = false;
                }
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAllDeviceRunsAsync().GetAwaiter().GetResult();
            SyncConfigFromUi();
            appConfig.SaveToIni();
        }

        private void numScreenRow_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.ScreenCaptureRow = (int)numScreenRow.Value;
            }
        }

        private void numStrobeX_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.StrobeTriggerX = (int)numStrobeX.Value;
            }
        }

        private void numStrobeY_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.StrobeTriggerY = (int)numStrobeY.Value;
            }
        }

        private void numLaserTriggerX_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.LaserTriggerX = (int)numLaserTriggerX.Value;
            }
        }

        private void numLaserTriggerY_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.LaserTriggerY = (int)numLaserTriggerY.Value;
            }
        }

        private void numLaserPatternX_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.LaserPatternX = (int)numLaserPatternX.Value;
            }
        }

        private void numLaserPatternY_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.LaserPatternY = (int)numLaserPatternY.Value;
            }
        }

        private void numLaserColorX_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.LaserColorX = (int)numLaserColorX.Value;
            }
        }

        private void numLaserColorY_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                appConfig.LaserColorY = (int)numLaserColorY.Value;
            }
        }

        private SceneKind GetCurrentSceneKind()
        {
            if (tabControl.SelectedTab == tabPageAcVolume)
            {
                return SceneKind.Volume;
            }

            if (tabControl.SelectedTab == tabPageAcSpectralAnalysis)
            {
                return SceneKind.SpectralAnalysis;
            }

            if (tabControl.SelectedTab == tabPageScreenCapture)
            {
                return SceneKind.ScreenCapture;
            }

            if (tabControl.SelectedTab == tabPageOtherDevices)
            {
                return SceneKind.OtherDevices;
            }

            return SceneKind.Basic;
        }

        private void textIpAddress_TextChanged(object sender, EventArgs e)
        {
        }

        private void numLedCount_ValueChanged(object sender, EventArgs e)
        {
        }

        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
        }

        private void trackBarNormalizationLevel_Scroll(object sender, EventArgs e)
        {
        }

        private void pnlBackgroundColor_Click(object sender, EventArgs e)
        {
        }

        private T ReadUi<T>(Func<T> action)
        {
            if (InvokeRequired)
            {
                return (T)Invoke(action);
            }

            return action();
        }

        private void SafeUi(Action action)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(action);
                return;
            }

            action();
        }
    }
}
