using System.ComponentModel;
using static Ledqualizer.AcVolume;

namespace Ledqualizer
{
    public partial class FrmMain : Form
    {
        private sealed class DeviceRunEntry
        {
            public DeviceRunEntry(DeviceConfig config, string configSignature, RunController controller)
            {
                Config = config;
                ConfigSignature = configSignature;
                Controller = controller;
            }

            public DeviceConfig Config { get; }
            public string ConfigSignature { get; }
            public RunController Controller { get; }
        }

        private sealed class SceneTypeOption
        {
            public SceneTypeOption(SceneType value)
            {
                Value = value;
                Display = SceneTypeNames.GetDisplayName(value);
            }

            public SceneType Value { get; }
            public string Display { get; }
        }

        private readonly AppConfig appConfig = new();
        private readonly BindingList<DeviceGridRow> deviceRows = new();
        private readonly BindingList<SceneGridRow> sceneRows = new();
        private readonly BindingSource sceneLookupBindingSource = new();
        private readonly BindingSource sceneGridBindingSource = new();
        private readonly Dictionary<string, DeviceRunEntry> deviceRuns = new();
        private readonly Dictionary<SceneType, Form> sceneEditors = new();
        private readonly List<SceneTypeOption> sceneTypeOptions = Enum.GetValues<SceneType>().Select(type => new SceneTypeOption(type)).ToList();
        private readonly DeviceMetadataService deviceMetadataService = new();
        private readonly Dictionary<string, Task> metadataRefreshTasks = new();
        private readonly Font deviceGroupFont;

        private bool isLoading;
        private bool reconcileInProgress;
        private bool reconcileRequested;
        private bool closingInProgress;
        private bool shutdownCompleted;
        private bool syncingAudioDeviceSelection;
        private string? selectedAudioDeviceId;
        private OtherDevicesForm? otherDevicesForm;
        private FormOverlay? frmOverlay;

        public sealed class FormOverlay : Form
        {
            private readonly Rectangle highlightRectangle;

            public FormOverlay(Rectangle rectangle)
            {
                FormBorderStyle = FormBorderStyle.None;
                BackColor = Color.Black;
                TransparencyKey = Color.Black;
                Opacity = 1;
                TopMost = true;
                StartPosition = FormStartPosition.Manual;
                Location = rectangle.Location;
                Size = rectangle.Size;
                ShowInTaskbar = false;
                highlightRectangle = new Rectangle(0, 0, rectangle.Width, rectangle.Height);

                Paint += (sender, e) =>
                {
                    using Pen whitePen = new(Color.White, 1);
                    e.Graphics.DrawRectangle(
                        whitePen,
                        highlightRectangle.X,
                        highlightRectangle.Y,
                        Math.Max(0, highlightRectangle.Width - 1),
                        Math.Max(0, highlightRectangle.Height - 1));
                };
            }
        }

        public FrmMain()
        {
            InitializeComponent();
            deviceGroupFont = new Font(dgvDevices.Font, FontStyle.Bold);
            InitializeSceneEditors();
            ConfigureDeviceGrid();
            ConfigureSceneGrid();
        }

        private void InitializeSceneEditors()
        {
            var solidColorEditor = new SolidColorSceneEditorForm();
            var gradientEditor = new GradientSceneEditorForm();
            var volumeEditor = new VolumeReactiveSceneEditorForm();
            var screenRowEditor = new ScreenRowCaptureSceneEditorForm();
            var spectralEditor = new SpectralAnalysisSceneEditorForm();

            solidColorEditor.SceneChanged += Editor_SceneChanged;
            gradientEditor.SceneChanged += Editor_SceneChanged;
            volumeEditor.SceneChanged += Editor_SceneChanged;
            volumeEditor.SelectedAudioDeviceChanged += VolumeEditor_SelectedAudioDeviceChanged;
            screenRowEditor.SceneChanged += Editor_SceneChanged;
            screenRowEditor.GuideChanged += ScreenRowEditor_GuideChanged;
            screenRowEditor.CaptureRowChanged += ScreenRowEditor_CaptureRowChanged;
            spectralEditor.SceneChanged += Editor_SceneChanged;
            spectralEditor.SelectedAudioDeviceChanged += SpectralEditor_SelectedAudioDeviceChanged;

            sceneEditors[SceneType.SolidColor] = solidColorEditor;
            sceneEditors[SceneType.Gradient] = gradientEditor;
            sceneEditors[SceneType.VolumeReactive] = volumeEditor;
            sceneEditors[SceneType.ScreenRowCapture] = screenRowEditor;
            sceneEditors[SceneType.SpectralAnalysis] = spectralEditor;

            foreach (Form editor in sceneEditors.Values)
            {
                editor.Visible = false;
                panelSceneEditorHost.Controls.Add(editor);
                editor.Show();
            }
        }

        private void ConfigureDeviceGrid()
        {
            dgvDevices.AutoGenerateColumns = false;
            dgvDevices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDevices.MultiSelect = true;
            dgvDevices.DataSource = deviceRows;
            dgvDevices.CurrentCellDirtyStateChanged += dgvDevices_CurrentCellDirtyStateChanged;
            dgvDevices.CellValueChanged += dgvDevices_CellValueChanged;
            dgvDevices.CellEndEdit += dgvDevices_CellEndEdit;
            dgvDevices.CellBeginEdit += dgvDevices_CellBeginEdit;
            dgvDevices.CellValidating += dgvDevices_CellValidating;
            dgvDevices.CellFormatting += dgvDevices_CellFormatting;
            dgvDevices.DataError += dgvDevices_DataError;

            colAssignedScene.DataSource = sceneLookupBindingSource;
            colAssignedScene.DisplayMember = nameof(SceneGridRow.Name);
            colAssignedScene.ValueMember = nameof(SceneGridRow.Id);
            colAssignedScene.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
        }

        private void ConfigureSceneGrid()
        {
            sceneLookupBindingSource.DataSource = sceneRows;
            sceneGridBindingSource.DataSource = sceneRows;

            dgvScenes.AutoGenerateColumns = false;
            dgvScenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvScenes.MultiSelect = false;
            dgvScenes.DataSource = sceneGridBindingSource;
            dgvScenes.CurrentCellDirtyStateChanged += dgvScenes_CurrentCellDirtyStateChanged;
            dgvScenes.CellValueChanged += dgvScenes_CellValueChanged;
            dgvScenes.CellEndEdit += dgvScenes_CellEndEdit;
            dgvScenes.CellValidating += dgvScenes_CellValidating;
            dgvScenes.DataError += dgvScenes_DataError;
            dgvScenes.SelectionChanged += dgvScenes_SelectionChanged;

            colSceneType.DataSource = sceneTypeOptions;
            colSceneType.DisplayMember = nameof(SceneTypeOption.Display);
            colSceneType.ValueMember = nameof(SceneTypeOption.Value);
            colSceneType.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            isLoading = true;
            try
            {
                appConfig.LoadFromIni();
                ApplyConfigToUi();
                LoadAudioDevices();
                CountHz();
            }
            finally
            {
                isLoading = false;
            }

            await ReconcileDeviceRunsAsync();
        }

        private void ApplyConfigToUi()
        {
            numDelay.Value = Math.Max(numDelay.Minimum, Math.Min(numDelay.Maximum, appConfig.Delay));

            sceneRows.Clear();
            foreach (SceneConfig scene in appConfig.Scenes)
            {
                sceneRows.Add(SceneGridRow.FromSceneConfig(scene));
            }

            deviceRows.Clear();
            foreach (DeviceConfig device in appConfig.Devices)
            {
                foreach (DeviceGridRow row in BuildDeviceRows(device))
                {
                    deviceRows.Add(row);
                }
            }

            sceneLookupBindingSource.ResetBindings(false);
            sceneGridBindingSource.ResetBindings(false);

            if (sceneRows.Count > 0)
            {
                dgvScenes.ClearSelection();
                dgvScenes.Rows[0].Selected = true;
                ShowSelectedSceneEditor();
            }

            UpdateConnectionSummary();
        }

        private void SyncConfigFromUi()
        {
            appConfig.Delay = (int)numDelay.Value;
            appConfig.Devices = GetRootDeviceRows()
                .Select(BuildDeviceConfigFromRows)
                .ToList();
        }

        private IEnumerable<DeviceGridRow> BuildDeviceRows(DeviceConfig device)
        {
            yield return DeviceGridRow.FromDeviceConfig(device);

            if (device.StripCount < 2)
            {
                yield break;
            }

            foreach (DeviceStripConfig strip in device.Strips.OrderBy(strip => strip.StripIndex))
            {
                yield return DeviceGridRow.FromStripConfig(device, strip);
            }
        }

        private IEnumerable<DeviceGridRow> GetRootDeviceRows()
        {
            return deviceRows.Where(row => row.Kind == DeviceRowKind.Device);
        }

        private DeviceGridRow? FindRootDeviceRow(string deviceId)
        {
            return deviceRows.FirstOrDefault(row => row.Kind == DeviceRowKind.Device && string.Equals(row.Id, deviceId, StringComparison.Ordinal));
        }

        private List<DeviceGridRow> GetStripRows(string deviceId)
        {
            return deviceRows
                .Where(row => row.Kind == DeviceRowKind.Strip && string.Equals(row.ParentDeviceId, deviceId, StringComparison.Ordinal))
                .OrderBy(row => row.StripIndex)
                .ToList();
        }

        private DeviceConfig BuildDeviceConfigFromRows(DeviceGridRow rootRow)
        {
            return new DeviceConfig
            {
                Id = rootRow.Id,
                Name = rootRow.Name,
                Host = rootRow.Host,
                Port = rootRow.Port,
                LedCount = rootRow.LedCount,
                StripCount = rootRow.StripCount,
                Enabled = rootRow.Enabled,
                AssignedSceneId = rootRow.AssignedSceneId,
                Strips = GetStripRows(rootRow.Id)
                    .Select(row => new DeviceStripConfig
                    {
                        StripIndex = row.StripIndex,
                        LedCount = row.LedCount,
                        Enabled = row.Enabled,
                        AssignedSceneId = row.AssignedSceneId
                    })
                    .ToList()
            };
        }

        private void LoadAudioDevices()
        {
            syncingAudioDeviceSelection = true;
            try
            {
                if (sceneEditors[SceneType.VolumeReactive] is VolumeReactiveSceneEditorForm volumeEditor)
                {
                    volumeEditor.LoadAudioDevices(selectedAudioDeviceId);
                    selectedAudioDeviceId ??= volumeEditor.GetSelectedAudioDeviceId();
                }

                if (sceneEditors[SceneType.SpectralAnalysis] is SpectralAnalysisSceneEditorForm spectralEditor)
                {
                    spectralEditor.LoadAudioDevices(selectedAudioDeviceId);
                    selectedAudioDeviceId ??= spectralEditor.GetSelectedAudioDeviceId();
                }
            }
            finally
            {
                syncingAudioDeviceSelection = false;
            }
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
                    await RefreshMetadataForEnabledDevicesAsync();
                    UpdateInvalidDeviceStatuses();

                    Dictionary<string, DeviceConfig> desiredDevices = GetRootDeviceRows()
                        .Select(BuildDeviceConfigFromRows)
                        .Where(HasActiveTargets)
                        .ToDictionary(device => device.Id, device => device);

                    foreach (string deviceId in deviceRuns.Keys.ToList())
                    {
                        if (!desiredDevices.TryGetValue(deviceId, out DeviceConfig? desiredDevice))
                        {
                            await StopDeviceRunAsync(deviceId);
                            continue;
                        }

                        DeviceRunEntry current = deviceRuns[deviceId];
                        if (RequiresRestart(current, desiredDevice))
                        {
                            await StopDeviceRunAsync(deviceId);
                            await StartDeviceRunAsync(desiredDevice);
                        }
                    }

                    foreach (DeviceConfig desiredDevice in desiredDevices.Values)
                    {
                        if (!deviceRuns.ContainsKey(desiredDevice.Id))
                        {
                            await StartDeviceRunAsync(desiredDevice);
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

        private void UpdateInvalidDeviceStatuses()
        {
            foreach (DeviceGridRow row in deviceRows)
            {
                DeviceGridRow rootRow = row.Kind == DeviceRowKind.Device
                    ? row
                    : FindRootDeviceRow(row.ParentDeviceId) ?? row;
                bool anyTargetEnabled = HasAnyEnabledTarget(rootRow.Id);
                bool rowEnabled = row.Kind == DeviceRowKind.Device ? anyTargetEnabled : row.Enabled;

                if (!rowEnabled)
                {
                    row.Status = "Disconnected";
                    continue;
                }

                if (!IsValidDeviceRow(rootRow))
                {
                    row.Status = "Invalid";
                    continue;
                }

                if (row.LedCount <= 0 || rootRow.LedCount <= 0)
                {
                    row.Status = "Metadata unavailable";
                    continue;
                }

                bool rowHasTarget = row.Kind == DeviceRowKind.Device ? row.Enabled : row.Enabled;
                if (rowHasTarget && FindSceneById(row.AssignedSceneId) == null)
                {
                    row.Status = "Scene missing";
                }
                else if (!deviceRuns.ContainsKey(rootRow.Id) && row.Status.StartsWith("Offline", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else if (!deviceRuns.ContainsKey(rootRow.Id))
                {
                    row.Status = "Pending";
                }
            }

            dgvDevices.Refresh();
        }

        private async Task StartDeviceRunAsync(DeviceConfig config)
        {
            if (config.LedCount <= 0)
            {
                return;
            }

            RunController controller = new();
            controller.DeviceStatusChanged += RunController_DeviceStatusChanged;
            deviceRuns[config.Id] = new DeviceRunEntry(config, BuildRunSignature(config), controller);
            await controller.StartAsync(new[] { config }, CreateCompositeSceneRunner(config.Id));
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
            foreach (string deviceId in deviceRuns.Keys.ToList())
            {
                await StopDeviceRunAsync(deviceId);
            }
        }

        private static bool RequiresRestart(DeviceRunEntry current, DeviceConfig desired)
        {
            return !string.Equals(current.Config.Host, desired.Host, StringComparison.OrdinalIgnoreCase)
                || current.Config.Port != desired.Port
                || current.Config.LedCount != desired.LedCount
                || current.Config.StripCount != desired.StripCount
                || !string.Equals(current.ConfigSignature, BuildRunSignature(desired), StringComparison.Ordinal);
        }

        private bool HasActiveTargets(DeviceConfig device)
        {
            if (!IsValidDeviceConfig(device) || device.LedCount <= 0)
            {
                return false;
            }

            if (device.Enabled && FindSceneById(device.AssignedSceneId) != null)
            {
                return true;
            }

            return device.Strips.Any(strip => strip.Enabled && strip.LedCount > 0 && FindSceneById(strip.AssignedSceneId) != null);
        }

        private bool HasAnyEnabledTarget(string deviceId)
        {
            DeviceGridRow? rootRow = FindRootDeviceRow(deviceId);
            if (rootRow == null)
            {
                return false;
            }

            return rootRow.Enabled || GetStripRows(deviceId).Any(row => row.Enabled);
        }

        private string ResolveActiveStatus(string deviceId)
        {
            return deviceRuns.ContainsKey(deviceId) ? "Effect active" : "Online";
        }

        private string ResolvePostMetadataStatus(string deviceId)
        {
            DeviceGridRow? rootRow = FindRootDeviceRow(deviceId);
            if (rootRow == null || !HasAnyEnabledTarget(deviceId))
            {
                return "Disconnected";
            }

            return rootRow.Status switch
            {
                "Connecting" => "Connecting",
                string status when status.StartsWith("Offline", StringComparison.OrdinalIgnoreCase) => status,
                "Invalid" => "Invalid",
                "Metadata unavailable" => "Metadata unavailable",
                "Scene missing" => "Scene missing",
                _ => ResolveActiveStatus(deviceId)
            };
        }

        private static bool IsValidDeviceConfig(DeviceConfig device)
        {
            return !string.IsNullOrWhiteSpace(device.Host)
                && device.Port > 0
                && device.Port <= 65535;
        }

        private static string BuildRunSignature(DeviceConfig config)
        {
            string stripSignature = string.Join("|", config.Strips
                .OrderBy(strip => strip.StripIndex)
                .Select(strip => $"{strip.StripIndex}:{strip.LedCount}:{strip.Enabled}:{strip.AssignedSceneId}"));
            return $"{config.Host}|{config.Port}|{config.LedCount}|{config.StripCount}|{config.Enabled}|{config.AssignedSceneId}|{stripSignature}";
        }

        private ISceneRunner CreateCompositeSceneRunner(string deviceId)
        {
            return new CompositeSceneRunner(
                () => GetDeviceSceneAssignments(deviceId),
                () => selectedAudioDeviceId,
                () => ReadUi(() => (int)numDelay.Value),
                UpdatePreview,
                UpdateVolumeProgress,
                UpdateSpectralProgress,
                UpdateRate);
        }

        private IReadOnlyList<DeviceSceneAssignment> GetDeviceSceneAssignments(string deviceId)
        {
            DeviceGridRow? rootRow = FindRootDeviceRow(deviceId);
            if (rootRow == null)
            {
                return Array.Empty<DeviceSceneAssignment>();
            }

            var assignments = new List<DeviceSceneAssignment>();
            int offset = 0;

            if (rootRow.Enabled)
            {
                SceneConfig? rootScene = FindSceneById(rootRow.AssignedSceneId);
                if (rootScene != null && rootRow.LedCount > 0)
                {
                    assignments.Add(new DeviceSceneAssignment
                    {
                        Scene = rootScene,
                        LedCount = rootRow.LedCount,
                        StartIndex = 0
                    });
                }
            }

            foreach (DeviceGridRow stripRow in GetStripRows(deviceId))
            {
                if (stripRow.Enabled)
                {
                    SceneConfig? stripScene = FindSceneById(stripRow.AssignedSceneId);
                    if (stripScene != null && stripRow.LedCount > 0)
                    {
                        assignments.Add(new DeviceSceneAssignment
                        {
                            Scene = stripScene,
                            LedCount = stripRow.LedCount,
                            StartIndex = offset
                        });
                    }
                }

                offset += Math.Max(stripRow.LedCount, 0);
            }

            return assignments;
        }

        private SolidColorSceneSettings GetSolidColorSceneSettings(string sceneId)
        {
            SceneConfig scene = RequireScene(sceneId);
            return ReadUi(() => new SolidColorSceneSettings
            {
                Hue = scene.SolidColor.Hue,
                MinHue = scene.SolidColor.MinHue,
                MaxHue = scene.SolidColor.MaxHue,
                SaturationValue = scene.SolidColor.Saturation,
                BrightnessValue = scene.SolidColor.Brightness,
                Delay = (int)numDelay.Value
            });
        }

        private GradientSceneSettings GetGradientSceneSettings(string sceneId)
        {
            SceneConfig scene = RequireScene(sceneId);
            return ReadUi(() => new GradientSceneSettings
            {
                HueMin = scene.Gradient.HueMin,
                HueMax = scene.Gradient.HueMax,
                SaturationValue = scene.Gradient.Saturation,
                BrightnessValue = scene.Gradient.Brightness,
                Delay = (int)numDelay.Value
            });
        }

        private VolumeSceneSettings GetVolumeSceneSettings(string sceneId)
        {
            SceneConfig scene = RequireScene(sceneId);
            return ReadUi(() => new VolumeSceneSettings
            {
                Mode = scene.VolumeReactive.Mode,
                Delay = (int)numDelay.Value,
                BrightnessValue = scene.VolumeReactive.Brightness,
                NormalizationValue = scene.VolumeReactive.Normalization,
                Reverse = scene.VolumeReactive.Reverse,
                HueReverse = scene.VolumeReactive.HueReverse,
                White = scene.VolumeReactive.White,
                BackgroundWhite = scene.VolumeReactive.BackgroundWhite,
                BackgroundBrightnessValue = scene.VolumeReactive.BackgroundBrightness,
                BackgroundHue = scene.VolumeReactive.BackgroundHue,
                HueMin = scene.VolumeReactive.HueMin,
                HueMax = scene.VolumeReactive.HueMax
            });
        }

        private SpectralSceneSettings GetSpectralSceneSettings(string sceneId)
        {
            SceneConfig scene = RequireScene(sceneId);
            return ReadUi(() => new SpectralSceneSettings
            {
                Mode = scene.SpectralAnalysis.Mode,
                Delay = (int)numDelay.Value,
                BrightnessValue = scene.SpectralAnalysis.Brightness,
                NormalizationValue = scene.SpectralAnalysis.Normalization,
                Reverse = scene.SpectralAnalysis.Reverse,
                HueReverse = scene.SpectralAnalysis.HueReverse,
                White = scene.SpectralAnalysis.White,
                BackgroundWhite = scene.SpectralAnalysis.BackgroundWhite,
                BackgroundBrightnessValue = scene.SpectralAnalysis.BackgroundBrightness,
                BackgroundHue = scene.SpectralAnalysis.BackgroundHue,
                HueMin = scene.SpectralAnalysis.HueMin,
                HueMax = scene.SpectralAnalysis.HueMax,
                FrequencyLowHz = scene.SpectralAnalysis.FrequencyLowHz,
                FrequencyHighHz = scene.SpectralAnalysis.FrequencyHighHz,
                LevelLowDb = scene.SpectralAnalysis.LevelLowDb,
                LevelHighDb = scene.SpectralAnalysis.LevelHighDb
            });
        }

        private ScreenCaptureSceneSettings GetScreenCaptureSceneSettings(string sceneId)
        {
            SceneConfig scene = RequireScene(sceneId);
            return ReadUi(() => new ScreenCaptureSceneSettings
            {
                Delay = (int)numDelay.Value,
                MonitorIndex = scene.ScreenRowCapture.MonitorIndex,
                CaptureY = scene.ScreenRowCapture.CaptureY,
                Reverse = scene.ScreenRowCapture.Reverse
            });
        }

        private void UpdatePreview(IReadOnlyList<Color> colors)
        {
            if (sceneEditors[SceneType.ScreenRowCapture] is ScreenRowCaptureSceneEditorForm screenRowEditor)
            {
                screenRowEditor.UpdatePreview(colors);
            }
        }

        private void UpdateVolumeProgress(int value)
        {
            if (sceneEditors[SceneType.VolumeReactive] is VolumeReactiveSceneEditorForm volumeEditor)
            {
                volumeEditor.UpdateProgress(value);
            }
        }

        private void UpdateSpectralProgress(int value)
        {
            if (sceneEditors[SceneType.SpectralAnalysis] is SpectralAnalysisSceneEditorForm spectralEditor)
            {
                spectralEditor.UpdateProgress(value);
            }
        }

        private void UpdateRate(string text)
        {
            SafeUi(() => statLblRate.Text = text);
        }

        private void RunController_DeviceStatusChanged(string deviceId, ConnectionState state, string? detail)
        {
            SafeUi(() =>
            {
                DeviceGridRow? rootRow = FindRootDeviceRow(deviceId);
                if (rootRow == null)
                {
                    return;
                }

                string status = state switch
                {
                    ConnectionState.Connecting => "Connecting",
                    ConnectionState.Connected => ResolveActiveStatus(deviceId),
                    ConnectionState.Faulted => string.IsNullOrWhiteSpace(detail) ? "Offline" : $"Offline: {detail}",
                    _ => HasAnyEnabledTarget(deviceId) ? "Pending" : "Disconnected"
                };

                rootRow.Status = status;
                foreach (DeviceGridRow stripRow in GetStripRows(deviceId))
                {
                    stripRow.Status = stripRow.Enabled ? status : "Disconnected";
                }

                dgvDevices.Refresh();
                UpdateConnectionSummary();
            });

            if (state == ConnectionState.Connected)
            {
                _ = RefreshMetadataForDeviceAsync(deviceId, force: true);
            }
        }

        private void UpdateConnectionSummary()
        {
            int enabledCount = GetRootDeviceRows()
                .Select(BuildDeviceConfigFromRows)
                .Count(HasActiveTargets);
            int connectedCount = GetRootDeviceRows().Count(item => item.Status is "Online" or "Effect active");
            int offlineCount = GetRootDeviceRows().Count(item => item.Enabled && item.Status.StartsWith("Offline", StringComparison.OrdinalIgnoreCase));

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

        private void dgvDevices_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvDevices.IsCurrentCellDirty && IsImmediateCommitCell(dgvDevices))
            {
                dgvDevices.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvDevices_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dgvDevices.Rows[e.RowIndex].DataBoundItem is not DeviceGridRow row)
            {
                return;
            }

            string propertyName = dgvDevices.Columns[e.ColumnIndex].DataPropertyName;
            if (row.Kind == DeviceRowKind.Strip
                && propertyName is nameof(DeviceGridRow.Name) or nameof(DeviceGridRow.Host) or nameof(DeviceGridRow.Port))
            {
                e.Cancel = true;
            }
        }

        private void dgvDevices_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvDevices.Rows[e.RowIndex].DataBoundItem is not DeviceGridRow row)
            {
                return;
            }

            string propertyName = dgvDevices.Columns[e.ColumnIndex].DataPropertyName;
            if (propertyName == nameof(DeviceGridRow.Name))
            {
                e.Value = row.Kind == DeviceRowKind.Strip ? $"    Strip {row.StripIndex}" : row.Name;
                e.FormattingApplied = true;
            }
            else if (propertyName == nameof(DeviceGridRow.StripCount))
            {
                e.Value = row.Kind == DeviceRowKind.Device
                    ? $"Count: {row.StripCount}"
                    : $"Idx: {row.StripIndex}";
                e.FormattingApplied = true;
            }
            else if (propertyName == nameof(DeviceGridRow.LedCount))
            {
                e.Value = row.LedCount.ToString();
                e.FormattingApplied = true;
            }
            else if (row.Kind == DeviceRowKind.Strip && propertyName is nameof(DeviceGridRow.Host) or nameof(DeviceGridRow.Port) or nameof(DeviceGridRow.StripCount))
            {
                e.Value = string.Empty;
                e.FormattingApplied = true;
            }

            DataGridViewCellStyle style = dgvDevices.Rows[e.RowIndex].DefaultCellStyle;
            if (row.Kind == DeviceRowKind.Device)
            {
                style.Font = deviceGroupFont;
            }
            else
            {
                style.Font = dgvDevices.Font;
            }
        }

        private async void dgvDevices_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (!ShouldHandleValueChangedImmediately(dgvDevices, e))
            {
                return;
            }

            await HandleDeviceCellChangedAsync(e.RowIndex, e.ColumnIndex);
        }

        private async void dgvDevices_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (IsImmediateCommitColumn(dgvDevices, e.ColumnIndex))
            {
                return;
            }

            await HandleDeviceCellChangedAsync(e.RowIndex, e.ColumnIndex);
        }

        private async Task HandleDeviceCellChangedAsync(int rowIndex, int columnIndex)
        {
            if (isLoading || rowIndex < 0 || columnIndex < 0)
            {
                return;
            }

            string propertyName = dgvDevices.Columns[columnIndex].DataPropertyName;
            if (dgvDevices.Rows[rowIndex].DataBoundItem is not DeviceGridRow row)
            {
                return;
            }

            if (row.Kind == DeviceRowKind.Device
                && (propertyName == nameof(DeviceGridRow.Host) || propertyName == nameof(DeviceGridRow.Port)))
            {
                row.Name = "Device";
                row.StripCount = 0;
                row.LedCount = 0;
                row.Status = row.Enabled ? "Pending metadata" : "Disconnected";
                foreach (DeviceGridRow stripRow in GetStripRows(row.Id))
                {
                    stripRow.Host = row.Host;
                    stripRow.Port = row.Port;
                    stripRow.LedCount = 0;
                    stripRow.Status = row.Status;
                }
                dgvDevices.Refresh();
            }
            else if (row.Kind == DeviceRowKind.Device)
            {
                foreach (DeviceGridRow stripRow in GetStripRows(row.Id))
                {
                    stripRow.Host = row.Host;
                    stripRow.Port = row.Port;
                }
            }

            await ReconcileDeviceRunsAsync();
        }

        private void dgvDevices_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            string propertyName = dgvDevices.Columns[e.ColumnIndex].DataPropertyName;
            string formattedValue = e.FormattedValue?.ToString() ?? string.Empty;

            if (propertyName == nameof(DeviceGridRow.Host))
            {
                if (dgvDevices.Rows[e.RowIndex].DataBoundItem is DeviceGridRow row && row.Kind == DeviceRowKind.Strip)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(formattedValue))
                {
                    e.Cancel = true;
                    dgvDevices.Rows[e.RowIndex].ErrorText = $"{propertyName} is required.";
                }
            }

            if (propertyName == nameof(DeviceGridRow.Port))
            {
                if (dgvDevices.Rows[e.RowIndex].DataBoundItem is DeviceGridRow row && row.Kind == DeviceRowKind.Strip)
                {
                    return;
                }

                if (!int.TryParse(formattedValue, out int port) || port <= 0 || port > 65535)
                {
                    e.Cancel = true;
                    dgvDevices.Rows[e.RowIndex].ErrorText = "Port must be between 1 and 65535.";
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

        private void dgvScenes_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvScenes.IsCurrentCellDirty && IsImmediateCommitCell(dgvScenes))
            {
                dgvScenes.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private async void dgvScenes_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (!ShouldHandleValueChangedImmediately(dgvScenes, e))
            {
                return;
            }

            await HandleSceneCellChangedAsync(e.RowIndex);
        }

        private async void dgvScenes_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (IsImmediateCommitColumn(dgvScenes, e.ColumnIndex))
            {
                return;
            }

            await HandleSceneCellChangedAsync(e.RowIndex);
        }

        private async Task HandleSceneCellChangedAsync(int rowIndex)
        {
            if (isLoading || rowIndex < 0)
            {
                return;
            }

            SceneGridRow? row = dgvScenes.Rows[rowIndex].DataBoundItem as SceneGridRow;
            if (row == null)
            {
                return;
            }

            SceneConfig? scene = FindSceneById(row.Id);
            if (scene == null)
            {
                return;
            }

            bool typeChanged = scene.Type != row.Type;
            scene.Name = string.IsNullOrWhiteSpace(row.Name) ? scene.Name : row.Name.Trim();
            scene.Type = row.Type;
            row.Name = scene.Name;
            row.Summary = SceneSummaryBuilder.Build(scene);

            sceneLookupBindingSource.ResetBindings(false);
            sceneGridBindingSource.ResetBindings(false);
            dgvDevices.Refresh();

            if (GetSelectedSceneConfig()?.Id == row.Id)
            {
                ShowSelectedSceneEditor();
            }

            if (typeChanged)
            {
                await ReconcileDeviceRunsAsync();
            }
        }

        private static bool IsImmediateCommitCell(DataGridView grid)
        {
            DataGridViewCell? cell = grid.CurrentCell;
            return cell is DataGridViewCheckBoxCell or DataGridViewComboBoxCell;
        }

        private static bool IsImmediateCommitColumn(DataGridView grid, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= grid.Columns.Count)
            {
                return false;
            }

            DataGridViewColumn column = grid.Columns[columnIndex];
            return column is DataGridViewCheckBoxColumn or DataGridViewComboBoxColumn;
        }

        private static bool ShouldHandleValueChangedImmediately(DataGridView grid, DataGridViewCellEventArgs e)
        {
            return e.RowIndex >= 0 && IsImmediateCommitColumn(grid, e.ColumnIndex);
        }

        private void dgvScenes_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            string propertyName = dgvScenes.Columns[e.ColumnIndex].DataPropertyName;
            if (propertyName == nameof(SceneGridRow.Name) && string.IsNullOrWhiteSpace(e.FormattedValue?.ToString()))
            {
                e.Cancel = true;
                dgvScenes.Rows[e.RowIndex].ErrorText = "Scene name is required.";
                return;
            }

            dgvScenes.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        private void dgvScenes_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvScenes_SelectionChanged(object? sender, EventArgs e)
        {
            if (!isLoading)
            {
                ShowSelectedSceneEditor();
            }
        }

        private void ShowSelectedSceneEditor()
        {
            SceneConfig? scene = GetSelectedSceneConfig();
            lblEditorTitle.Text = scene == null ? "Scene Settings" : $"{scene.Name} Settings";

            foreach (Form editor in sceneEditors.Values)
            {
                editor.Visible = false;
            }

            if (scene == null || !sceneEditors.TryGetValue(scene.Type, out Form? editorToShow))
            {
                CloseOverlayForm();
                return;
            }

            editorToShow.Visible = true;
            editorToShow.BringToFront();
            ((ISceneEditorForm)editorToShow).LoadScene(scene);

            if (scene.Type == SceneType.ScreenRowCapture)
            {
                SyncOverlayToScreenEditor();
            }
            else
            {
                CloseOverlayForm();
            }
        }

        private SceneConfig? GetSelectedSceneConfig()
        {
            if (dgvScenes.CurrentRow?.DataBoundItem is SceneGridRow row)
            {
                return FindSceneById(row.Id);
            }

            return null;
        }

        private SceneConfig? FindSceneById(string sceneId)
        {
            return appConfig.Scenes.FirstOrDefault(scene => string.Equals(scene.Id, sceneId, StringComparison.Ordinal));
        }

        private SceneConfig RequireScene(string sceneId)
        {
            SceneConfig? scene = FindSceneById(sceneId);
            if (scene == null)
            {
                throw new InvalidOperationException($"Scene '{sceneId}' was not found.");
            }

            return scene;
        }

        private async void btnAddDevice_Click(object? sender, EventArgs e)
        {
            string defaultSceneId = appConfig.Scenes.FirstOrDefault()?.Id ?? string.Empty;
            DeviceConfig device = new()
            {
                Id = Guid.NewGuid().ToString("N"),
                Enabled = false,
                Name = "Device",
                Host = "127.0.0.1",
                Port = 81,
                LedCount = 0,
                StripCount = 0,
                AssignedSceneId = defaultSceneId
            };

            foreach (DeviceGridRow row in BuildDeviceRows(device))
            {
                deviceRows.Add(row);
            }

            await ReconcileDeviceRunsAsync();
        }

        private async void btnRemoveDevice_Click(object? sender, EventArgs e)
        {
            List<string> deviceIdsToRemove = dgvDevices.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(row => row.DataBoundItem as DeviceGridRow)
                .Where(row => row != null)
                .Cast<DeviceGridRow>()
                .Select(row => row.Kind == DeviceRowKind.Device ? row.Id : row.ParentDeviceId)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            foreach (string deviceId in deviceIdsToRemove)
            {
                foreach (DeviceGridRow row in deviceRows
                    .Where(item => string.Equals(item.Id, deviceId, StringComparison.Ordinal)
                        || string.Equals(item.ParentDeviceId, deviceId, StringComparison.Ordinal))
                    .ToList())
                {
                    deviceRows.Remove(row);
                }
            }

            await ReconcileDeviceRunsAsync();
        }

        private void btnOtherDevices_Click(object? sender, EventArgs e)
        {
            if (otherDevicesForm == null || otherDevicesForm.IsDisposed)
            {
                otherDevicesForm = new OtherDevicesForm();
                otherDevicesForm.SettingsChanged += OtherDevicesForm_SettingsChanged;
            }

            otherDevicesForm.LoadSettings(new OtherDevicesSceneSettings
            {
                StrobeTriggerX = appConfig.StrobeTriggerX,
                StrobeTriggerY = appConfig.StrobeTriggerY,
                LaserTriggerX = appConfig.LaserTriggerX,
                LaserTriggerY = appConfig.LaserTriggerY,
                LaserPatternX = appConfig.LaserPatternX,
                LaserPatternY = appConfig.LaserPatternY,
                LaserColorX = appConfig.LaserColorX,
                LaserColorY = appConfig.LaserColorY
            });

            otherDevicesForm.Show(this);
            otherDevicesForm.BringToFront();
        }

        private void OtherDevicesForm_SettingsChanged(object? sender, OtherDevicesSettingsChangedEventArgs e)
        {
            appConfig.StrobeTriggerX = e.Settings.StrobeTriggerX;
            appConfig.StrobeTriggerY = e.Settings.StrobeTriggerY;
            appConfig.LaserTriggerX = e.Settings.LaserTriggerX;
            appConfig.LaserTriggerY = e.Settings.LaserTriggerY;
            appConfig.LaserPatternX = e.Settings.LaserPatternX;
            appConfig.LaserPatternY = e.Settings.LaserPatternY;
            appConfig.LaserColorX = e.Settings.LaserColorX;
            appConfig.LaserColorY = e.Settings.LaserColorY;
        }

        private void btnAddScene_Click(object? sender, EventArgs e)
        {
            SceneConfig scene = SceneConfig.CreateDefault(SceneType.SolidColor, sceneRows.Count + 1);
            appConfig.Scenes.Add(scene);
            SceneGridRow row = SceneGridRow.FromSceneConfig(scene);
            sceneRows.Add(row);
            sceneLookupBindingSource.ResetBindings(false);
            sceneGridBindingSource.ResetBindings(false);
            dgvScenes.ClearSelection();
            int rowIndex = sceneRows.Count - 1;
            if (rowIndex >= 0)
            {
                dgvScenes.Rows[rowIndex].Selected = true;
                dgvScenes.CurrentCell = dgvScenes.Rows[rowIndex].Cells[0];
            }
        }

        private void btnDuplicateScene_Click(object? sender, EventArgs e)
        {
            SceneConfig? selectedScene = GetSelectedSceneConfig();
            if (selectedScene == null)
            {
                return;
            }

            SceneConfig clone = selectedScene.Clone();
            clone.Name = $"{selectedScene.Name} Copy";
            appConfig.Scenes.Add(clone);
            sceneRows.Add(SceneGridRow.FromSceneConfig(clone));
            sceneLookupBindingSource.ResetBindings(false);
            sceneGridBindingSource.ResetBindings(false);
        }

        private void btnRemoveScene_Click(object? sender, EventArgs e)
        {
            SceneConfig? selectedScene = GetSelectedSceneConfig();
            if (selectedScene == null)
            {
                return;
            }

            bool isAssigned = deviceRows.Any(device => string.Equals(device.AssignedSceneId, selectedScene.Id, StringComparison.Ordinal));
            if (isAssigned)
            {
                MessageBox.Show(this, "This scene is still assigned to one or more devices.", "Scene In Use", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            appConfig.Scenes.RemoveAll(scene => string.Equals(scene.Id, selectedScene.Id, StringComparison.Ordinal));
            SceneGridRow? row = sceneRows.FirstOrDefault(item => item.Id == selectedScene.Id);
            if (row != null)
            {
                sceneRows.Remove(row);
            }

            sceneLookupBindingSource.ResetBindings(false);
            sceneGridBindingSource.ResetBindings(false);
            ShowSelectedSceneEditor();
        }

        private void Editor_SceneChanged(object? sender, EventArgs e)
        {
            SceneConfig? selectedScene = GetSelectedSceneConfig();
            if (selectedScene == null)
            {
                return;
            }

            SceneGridRow? row = sceneRows.FirstOrDefault(item => item.Id == selectedScene.Id);
            if (row != null)
            {
                row.Summary = SceneSummaryBuilder.Build(selectedScene);
                sceneGridBindingSource.ResetBindings(false);
            }
        }

        private void VolumeEditor_SelectedAudioDeviceChanged(object? sender, EventArgs e)
        {
            if (syncingAudioDeviceSelection || sender is not VolumeReactiveSceneEditorForm volumeEditor)
            {
                return;
            }

            selectedAudioDeviceId = volumeEditor.GetSelectedAudioDeviceId();
            SyncAudioDeviceEditors(excludeVolume: true);
        }

        private void SpectralEditor_SelectedAudioDeviceChanged(object? sender, EventArgs e)
        {
            if (syncingAudioDeviceSelection || sender is not SpectralAnalysisSceneEditorForm spectralEditor)
            {
                return;
            }

            selectedAudioDeviceId = spectralEditor.GetSelectedAudioDeviceId();
            SyncAudioDeviceEditors(excludeSpectral: true);
        }

        private void SyncAudioDeviceEditors(bool excludeVolume = false, bool excludeSpectral = false)
        {
            syncingAudioDeviceSelection = true;
            try
            {
                if (!excludeVolume && sceneEditors[SceneType.VolumeReactive] is VolumeReactiveSceneEditorForm volumeEditor)
                {
                    volumeEditor.SelectAudioDevice(selectedAudioDeviceId);
                }

                if (!excludeSpectral && sceneEditors[SceneType.SpectralAnalysis] is SpectralAnalysisSceneEditorForm spectralEditor)
                {
                    spectralEditor.SelectAudioDevice(selectedAudioDeviceId);
                }
            }
            finally
            {
                syncingAudioDeviceSelection = false;
            }
        }

        private void ScreenRowEditor_GuideChanged(object? sender, EventArgs e)
        {
            SyncOverlayToScreenEditor();
        }

        private void ScreenRowEditor_CaptureRowChanged(object? sender, EventArgs e)
        {
            SyncOverlayToScreenEditor();
        }

        private void SyncOverlayToScreenEditor()
        {
            if (sceneEditors[SceneType.ScreenRowCapture] is not ScreenRowCaptureSceneEditorForm screenEditor || !screenEditor.Visible)
            {
                CloseOverlayForm();
                return;
            }

            if (screenEditor.ShowGuide)
            {
                ShowOverlayForm(screenEditor.MonitorIndex, screenEditor.CaptureRow);
            }
            else
            {
                CloseOverlayForm();
            }
        }

        public void ShowOverlayForm(int monitorIndex, int y)
        {
            Screen[] screens = Screen.AllScreens;
            Screen screen = (monitorIndex >= 0 && monitorIndex < screens.Length)
                ? screens[monitorIndex]
                : (Screen.PrimaryScreen ?? screens[0]);
            Rectangle bounds = screen.Bounds;
            Rectangle captureArea = new(bounds.Left, bounds.Top + Math.Max(0, y - 1), bounds.Width, 3);
            if (frmOverlay != null)
            {
                frmOverlay.Location = captureArea.Location;
                frmOverlay.Size = captureArea.Size;
                frmOverlay.Invalidate();
                return;
            }

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

        private bool IsValidDeviceRow(DeviceGridRow row)
        {
            DeviceGridRow rootRow = row.Kind == DeviceRowKind.Device
                ? row
                : FindRootDeviceRow(row.ParentDeviceId) ?? row;
            return !string.IsNullOrWhiteSpace(rootRow.Host)
                && rootRow.Port > 0
                && rootRow.Port <= 65535;
        }

        private void CountHz()
        {
            if (numDelay.Value > 0)
            {
                lblRefreshRate.Text = $"({1000m / numDelay.Value:F1} Hz)";
            }
        }

        private void numDelay_ValueChanged(object? sender, EventArgs e)
        {
            appConfig.Delay = (int)numDelay.Value;
            CountHz();
        }

        private async Task RefreshMetadataForEnabledDevicesAsync()
        {
            List<DeviceGridRow> rows = GetRootDeviceRows()
                .Where(row => HasAnyEnabledTarget(row.Id) && IsValidDeviceRow(row) && row.LedCount <= 0)
                .ToList();

            foreach (DeviceGridRow row in rows)
            {
                try
                {
                    await RefreshMetadataForRowAsync(row, force: false).ConfigureAwait(false);
                }
                catch
                {
                    // The row status is updated in the refresh helper.
                }
            }
        }

        private Task RefreshMetadataForDeviceAsync(string deviceId, bool force)
        {
            lock (metadataRefreshTasks)
            {
                if (!force && metadataRefreshTasks.TryGetValue(deviceId, out Task? existingTask))
                {
                    return existingTask;
                }
            }

            DeviceGridRow? row = ReadUi(() => deviceRows.FirstOrDefault(item => item.Id == deviceId));
            if (row == null)
            {
                return Task.CompletedTask;
            }

            Task refreshTask = RefreshMetadataForRowAsync(row, force);
            lock (metadataRefreshTasks)
            {
                metadataRefreshTasks[deviceId] = refreshTask;
            }

            return refreshTask.ContinueWith(task =>
            {
                lock (metadataRefreshTasks)
                {
                    metadataRefreshTasks.Remove(deviceId);
                }
            }, TaskScheduler.Default);
        }

        private async Task RefreshMetadataForRowAsync(DeviceGridRow row, bool force)
        {
            if (row.Kind != DeviceRowKind.Device)
            {
                row = FindRootDeviceRow(row.ParentDeviceId) ?? row;
            }

            if (!force && row.LedCount > 0)
            {
                return;
            }

            SafeUi(() =>
            {
                if (HasAnyEnabledTarget(row.Id))
                {
                    row.Status = "Loading metadata";
                    dgvDevices.Refresh();
                }
            });

            try
            {
                DeviceMetadata metadata = await deviceMetadataService.FetchAsync(row.Host, row.Port, CancellationToken.None).ConfigureAwait(false);
                bool requiresRestart = false;

                SafeUi(() =>
                {
                    requiresRestart = row.LedCount > 0 && (row.LedCount != metadata.TotalLedCount || row.StripCount != metadata.StripCount);
                    row.Name = metadata.Name;
                    row.LedCount = metadata.TotalLedCount;
                    row.StripCount = metadata.StripCount;

                    Dictionary<int, DeviceGridRow> existingStripRows = GetStripRows(row.Id)
                        .ToDictionary(stripRow => stripRow.StripIndex, stripRow => stripRow);

                    if (metadata.StripCount >= 2)
                    {
                        foreach (DeviceStripMetadata stripMetadata in metadata.Strips)
                        {
                            if (!existingStripRows.TryGetValue(stripMetadata.Index, out DeviceGridRow? stripRow))
                            {
                                stripRow = new DeviceGridRow
                                {
                                    Id = $"{row.Id}:strip:{stripMetadata.Index}",
                                    ParentDeviceId = row.Id,
                                    Kind = DeviceRowKind.Strip,
                                    StripIndex = stripMetadata.Index,
                                    Enabled = false,
                                    Name = $"Strip {stripMetadata.Index}",
                                    Host = row.Host,
                                    Port = row.Port,
                                    AssignedSceneId = row.AssignedSceneId,
                                    Status = row.Enabled ? row.Status : "Disconnected"
                                };

                                int insertIndex = deviceRows.IndexOf(row) + 1 + deviceRows.Count(item => item.Kind == DeviceRowKind.Strip && string.Equals(item.ParentDeviceId, row.Id, StringComparison.Ordinal) && item.StripIndex < stripMetadata.Index);
                                deviceRows.Insert(insertIndex, stripRow);
                            }

                            stripRow.Host = row.Host;
                            stripRow.Port = row.Port;
                            stripRow.LedCount = stripMetadata.LedCount;
                            stripRow.Status = stripRow.Enabled ? ResolvePostMetadataStatus(row.Id) : "Disconnected";
                        }
                    }

                    foreach (DeviceGridRow obsoleteStrip in existingStripRows.Values.Where(stripRow => metadata.Strips.All(strip => strip.Index != stripRow.StripIndex)).ToList())
                    {
                        deviceRows.Remove(obsoleteStrip);
                    }

                    if (metadata.StripCount < 2)
                    {
                        foreach (DeviceGridRow obsoleteStrip in existingStripRows.Values.ToList())
                        {
                            deviceRows.Remove(obsoleteStrip);
                        }
                    }

                    DeviceConfig? config = appConfig.Devices.FirstOrDefault(device => device.Id == row.Id);
                    if (config != null)
                    {
                        config.Name = metadata.Name;
                        config.LedCount = metadata.TotalLedCount;
                        config.StripCount = metadata.StripCount;
                        config.Strips = metadata.Strips
                            .Select(strip => new DeviceStripConfig
                            {
                                StripIndex = strip.Index,
                                LedCount = strip.LedCount,
                                Enabled = config.Strips.FirstOrDefault(existing => existing.StripIndex == strip.Index)?.Enabled ?? false,
                                AssignedSceneId = config.Strips.FirstOrDefault(existing => existing.StripIndex == strip.Index)?.AssignedSceneId ?? config.AssignedSceneId
                            })
                            .ToList();
                    }

                    if (!HasAnyEnabledTarget(row.Id))
                    {
                        row.Status = "Disconnected";
                    }
                    else
                    {
                        row.Status = ResolvePostMetadataStatus(row.Id);
                        foreach (DeviceGridRow stripRow in GetStripRows(row.Id))
                        {
                            stripRow.Status = stripRow.Enabled ? row.Status : "Disconnected";
                        }
                    }

                    dgvDevices.Refresh();
                });

                if (requiresRestart && ReadUi(() => deviceRuns.ContainsKey(row.Id)))
                {
                    await ReadUi(() => ReconcileDeviceRunsAsync()).ConfigureAwait(false);
                }
            }
            catch
            {
                SafeUi(() =>
                {
                    if (HasAnyEnabledTarget(row.Id) && row.LedCount <= 0)
                    {
                        row.Status = "Metadata unavailable";
                        dgvDevices.Refresh();
                    }
                });

                throw;
            }
        }

        private async void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (shutdownCompleted)
            {
                return;
            }

            if (closingInProgress)
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = true;
            closingInProgress = true;

            CloseOverlayForm();
            if (otherDevicesForm != null && !otherDevicesForm.IsDisposed)
            {
                otherDevicesForm.Close();
            }

            try
            {
                await StopAllDeviceRunsAsync();
                SyncConfigFromUi();
                appConfig.SaveToIni();
                shutdownCompleted = true;
                Close();
            }
            finally
            {
                closingInProgress = false;
            }
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
