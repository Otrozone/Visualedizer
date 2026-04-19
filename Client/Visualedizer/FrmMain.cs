using System.ComponentModel;
using static Ledqualizer.AcVolume;

namespace Ledqualizer
{
    public partial class FrmMain : Form
    {
        private sealed class DeviceRunEntry
        {
            public DeviceRunEntry(DeviceConfig config, SceneType sceneType, RunController controller)
            {
                Config = config;
                SceneType = sceneType;
                Controller = controller;
            }

            public DeviceConfig Config { get; }
            public SceneType SceneType { get; }
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
                    using Pen redPen = new(Color.Red, 1);
                    e.Graphics.DrawRectangle(redPen, new Rectangle(0, 17, Width - 1, 3));
                };
            }
        }

        public FrmMain()
        {
            InitializeComponent();
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
            dgvDevices.CellValidating += dgvDevices_CellValidating;
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
                deviceRows.Add(DeviceGridRow.FromDeviceConfig(device));
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
            appConfig.Devices = deviceRows.Select(row => row.ToDeviceConfig()).ToList();
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

                    Dictionary<string, DeviceConfig> desiredDevices = deviceRows
                        .Where(row => row.Enabled && IsValidDeviceRow(row) && row.LedCount > 0 && FindSceneById(row.AssignedSceneId) != null)
                        .Select(row => row.ToDeviceConfig())
                        .ToDictionary(device => device.Id, device => device);

                    foreach (string deviceId in deviceRuns.Keys.ToList())
                    {
                        if (!desiredDevices.TryGetValue(deviceId, out DeviceConfig? desiredDevice))
                        {
                            await StopDeviceRunAsync(deviceId);
                            continue;
                        }

                        SceneConfig scene = FindSceneById(desiredDevice.AssignedSceneId)!;
                        DeviceRunEntry current = deviceRuns[deviceId];
                        if (RequiresRestart(current, desiredDevice, scene.Type))
                        {
                            await StopDeviceRunAsync(deviceId);
                            await StartDeviceRunAsync(desiredDevice, scene.Type);
                        }
                    }

                    foreach (DeviceConfig desiredDevice in desiredDevices.Values)
                    {
                        if (!deviceRuns.ContainsKey(desiredDevice.Id))
                        {
                            SceneConfig scene = FindSceneById(desiredDevice.AssignedSceneId)!;
                            await StartDeviceRunAsync(desiredDevice, scene.Type);
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
                if (!row.Enabled)
                {
                    row.Status = "Disconnected";
                    continue;
                }

                if (!IsValidDeviceRow(row))
                {
                    row.Status = "Invalid";
                    continue;
                }

                if (row.LedCount <= 0)
                {
                    row.Status = "Metadata unavailable";
                    continue;
                }

                if (FindSceneById(row.AssignedSceneId) == null)
                {
                    row.Status = "Scene missing";
                }
                else if (!deviceRuns.ContainsKey(row.Id) && row.Status.StartsWith("Offline", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else if (!deviceRuns.ContainsKey(row.Id))
                {
                    row.Status = "Pending";
                }
            }

            dgvDevices.Refresh();
        }

        private async Task StartDeviceRunAsync(DeviceConfig config, SceneType sceneType)
        {
            if (config.LedCount <= 0)
            {
                return;
            }

            RunController controller = new();
            controller.DeviceStatusChanged += RunController_DeviceStatusChanged;
            deviceRuns[config.Id] = new DeviceRunEntry(config, sceneType, controller);
            await controller.StartAsync(new[] { config }, CreateSceneRunner(config.AssignedSceneId, sceneType));
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

        private static bool RequiresRestart(DeviceRunEntry current, DeviceConfig desired, SceneType sceneType)
        {
            return !string.Equals(current.Config.Host, desired.Host, StringComparison.OrdinalIgnoreCase)
                || current.Config.Port != desired.Port
                || current.Config.LedCount != desired.LedCount
                || !string.Equals(current.Config.AssignedSceneId, desired.AssignedSceneId, StringComparison.Ordinal)
                || current.SceneType != sceneType;
        }

        private ISceneRunner CreateSceneRunner(string sceneId, SceneType sceneType)
        {
            return sceneType switch
            {
                SceneType.Gradient => new GradientSceneRunner(() => GetGradientSceneSettings(sceneId)),
                SceneType.VolumeReactive => new VolumeSceneRunner(() => GetVolumeSceneSettings(sceneId), () => selectedAudioDeviceId, UpdateVolumeProgress, UpdateRate),
                SceneType.ScreenRowCapture => new ScreenCaptureSceneRunner(() => GetScreenCaptureSceneSettings(sceneId), UpdatePreview),
                SceneType.SpectralAnalysis => new SpectralSceneRunner(() => GetSpectralSceneSettings(sceneId), () => selectedAudioDeviceId, UpdateSpectralProgress, UpdateRate),
                _ => new SolidColorSceneRunner(() => GetSolidColorSceneSettings(sceneId)),
            };
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
                    _ => row.Enabled ? "Pending" : "Disconnected"
                };

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
            int enabledCount = deviceRows.Count(item => item.Enabled && IsValidDeviceRow(item) && item.LedCount > 0 && FindSceneById(item.AssignedSceneId) != null);
            int connectedCount = deviceRows.Count(item => item.Status == "Connected");
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

        private void dgvDevices_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvDevices.IsCurrentCellDirty && IsImmediateCommitCell(dgvDevices))
            {
                dgvDevices.CommitEdit(DataGridViewDataErrorContexts.Commit);
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
            if (dgvDevices.Rows[rowIndex].DataBoundItem is DeviceGridRow row
                && (propertyName == nameof(DeviceGridRow.Host) || propertyName == nameof(DeviceGridRow.Port)))
            {
                row.Name = "Device";
                row.StripCount = 0;
                row.LedCount = 0;
                row.Status = row.Enabled ? "Pending metadata" : "Disconnected";
                dgvDevices.Refresh();
            }

            await ReconcileDeviceRunsAsync();
        }

        private void dgvDevices_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            string propertyName = dgvDevices.Columns[e.ColumnIndex].DataPropertyName;
            string formattedValue = e.FormattedValue?.ToString() ?? string.Empty;

            if (propertyName == nameof(DeviceGridRow.Host))
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
            int deviceNumber = deviceRows.Count + 1;
            deviceRows.Add(new DeviceGridRow
            {
                Id = Guid.NewGuid().ToString("N"),
                Enabled = false,
                Name = "Device",
                Host = "127.0.0.1",
                Port = 81,
                LedCount = 0,
                StripCount = 0,
                AssignedSceneId = defaultSceneId,
                Status = "Disconnected"
            });

            await ReconcileDeviceRunsAsync();
        }

        private async void btnRemoveDevice_Click(object? sender, EventArgs e)
        {
            List<DeviceGridRow> rowsToRemove = dgvDevices.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(row => row.DataBoundItem as DeviceGridRow)
                .Where(row => row != null)
                .Cast<DeviceGridRow>()
                .ToList();

            foreach (DeviceGridRow row in rowsToRemove)
            {
                deviceRows.Remove(row);
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
                ShowOverlayForm(screenEditor.CaptureRow);
            }
            else
            {
                CloseOverlayForm();
            }
        }

        public void ShowOverlayForm(int y)
        {
            int screenWidth = Screen.PrimaryScreen?.Bounds.Width ?? 1;
            Rectangle captureArea = new(0, y - 18, screenWidth - 1, 3);
            if (frmOverlay != null)
            {
                frmOverlay.Location = captureArea.Location;
                frmOverlay.Size = captureArea.Size;
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
            return !string.IsNullOrWhiteSpace(row.Host)
                && row.Port > 0
                && row.Port <= 65535;
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
            List<DeviceGridRow> rows = deviceRows
                .Where(row => row.Enabled && IsValidDeviceRow(row) && row.LedCount <= 0)
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
            if (!force && row.LedCount > 0)
            {
                return;
            }

            SafeUi(() =>
            {
                if (row.Enabled)
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

                    DeviceConfig? config = appConfig.Devices.FirstOrDefault(device => device.Id == row.Id);
                    if (config != null)
                    {
                        config.Name = metadata.Name;
                        config.LedCount = metadata.TotalLedCount;
                        config.StripCount = metadata.StripCount;
                    }

                    if (!row.Enabled)
                    {
                        row.Status = "Disconnected";
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
                    if (row.Enabled && row.LedCount <= 0)
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
