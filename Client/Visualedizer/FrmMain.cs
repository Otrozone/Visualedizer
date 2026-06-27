using System.ComponentModel;
using static Ledqualizer.AcVolume;

namespace Ledqualizer
{
    public partial class FrmMain : Form
    {
        private const int MinCollectionAutoSelectionPeriodSeconds = 1;
        private const int MaxCollectionAutoSelectionPeriodSeconds = 3600;

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

        private sealed class SceneAssignmentOption
        {
            public SceneAssignmentOption(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public string Id { get; }
            public string Name { get; }
        }

        private readonly AppConfig appConfig = new();
        private readonly BindingList<DeviceGridRow> deviceRows = new();
        private readonly BindingList<SceneGridRow> sceneRows = new();
        private readonly BindingList<CollectionGridRow> collectionRows = new();
        private readonly BindingSource ledSceneLookupBindingSource = new();
        private readonly BindingSource laserSceneLookupBindingSource = new();
        private readonly BindingSource strobeSceneLookupBindingSource = new();
        private readonly BindingSource sceneGridBindingSource = new();
        private readonly BindingSource collectionGridBindingSource = new();
        private readonly Dictionary<string, DeviceRunEntry> deviceRuns = new();
        private readonly Dictionary<SceneType, Form> sceneEditors = new();
        private readonly List<SceneTypeOption> sceneTypeOptions = Enum.GetValues<SceneType>().Select(type => new SceneTypeOption(type)).ToList();
        private readonly BindingList<SceneAssignmentOption> ledSceneOptions = new();
        private readonly BindingList<SceneAssignmentOption> laserSceneOptions = new();
        private readonly BindingList<SceneAssignmentOption> strobeSceneOptions = new();
        private readonly DeviceMetadataService deviceMetadataService = new();
        private readonly Dictionary<string, Task> metadataRefreshTasks = new();
        private readonly Dictionary<string, LaserDmxRuntimeState> manualLaserStates = new();
        private readonly System.Windows.Forms.Timer collectionAutoSelectionTimer = new();
        private readonly Random collectionAutoSelectionRandom = new();
        private readonly Font deviceGroupFont;

        private bool isLoading;
        private bool reconcileInProgress;
        private bool reconcileRequested;
        private bool closingInProgress;
        private bool shutdownCompleted;
        private bool syncingAudioDeviceSelection;
        private bool syncingCollectionAutoSelectionControls;
        private bool collectionAutoSelectionTickInProgress;
        private bool collectionOverrideActive;
        private bool collectionOverrideChanging;
        private string? selectedAudioDeviceId;
        private string? activeCollectionId;
        private CollectionActivationMode? activeCollectionMode;
        private KeyboardShortcutConfig? activeHoldShortcut;
        private OtherDevicesForm? otherDevicesForm;
        private FormOverlay? frmOverlay;
        private GlobalShortcutManager? globalShortcutManager;

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
            btnOtherDevices.Visible = false;
            deviceGroupFont = new Font(dgvDevices.Font, FontStyle.Bold);
            InitializeSceneEditors();
            ConfigureDeviceGrid();
            ConfigureSceneGrid();
            ConfigureCollectionGrid();
            InitializeGridReordering();
            InitializeGlobalShortcutManager();
        }

        private void InitializeSceneEditors()
        {
            var solidColorEditor = new SolidColorSceneEditorForm();
            var gradientEditor = new GradientSceneEditorForm();
            var volumeEditor = new VolumeReactiveSceneEditorForm();
            var screenRowEditor = new ScreenRowCaptureSceneEditorForm();
            var spectralEditor = new SpectralAnalysisSceneEditorForm();
            var spectralSegmentsEditor = new SpectralAnalysisSegmentsSceneEditorForm();
            var imageRowEditor = new ImageRowCaptureSceneEditorForm();
            var sparkleAndFlashEditor = new SparkleAndFlashSceneEditorForm();
            var laserDmxEditor = new LaserDmxSceneEditorForm();
            var strobeEditor = new StrobeSceneEditorForm();
            var ledStrobeEditor = new LedStrobeSceneEditorForm();

            solidColorEditor.SceneChanged += Editor_SceneChanged;
            gradientEditor.SceneChanged += Editor_SceneChanged;
            volumeEditor.SceneChanged += Editor_SceneChanged;
            volumeEditor.SelectedAudioDeviceChanged += VolumeEditor_SelectedAudioDeviceChanged;
            screenRowEditor.SceneChanged += Editor_SceneChanged;
            screenRowEditor.GuideChanged += ScreenRowEditor_GuideChanged;
            screenRowEditor.CaptureRowChanged += ScreenRowEditor_CaptureRowChanged;
            spectralEditor.SceneChanged += Editor_SceneChanged;
            spectralEditor.SelectedAudioDeviceChanged += SpectralEditor_SelectedAudioDeviceChanged;
            spectralSegmentsEditor.SceneChanged += Editor_SceneChanged;
            spectralSegmentsEditor.SelectedAudioDeviceChanged += SpectralSegmentsEditor_SelectedAudioDeviceChanged;
            imageRowEditor.SceneChanged += Editor_SceneChanged;
            sparkleAndFlashEditor.SceneChanged += Editor_SceneChanged;
            laserDmxEditor.SceneChanged += Editor_SceneChanged;
            strobeEditor.SceneChanged += Editor_SceneChanged;
            ledStrobeEditor.SceneChanged += Editor_SceneChanged;
            ledStrobeEditor.SelectedAudioDeviceChanged += LedStrobeEditor_SelectedAudioDeviceChanged;
            laserDmxEditor.SendRequested += LaserDmxEditor_SendRequested;
            strobeEditor.TestRequested += StrobeEditor_TestRequested;

            sceneEditors[SceneType.SolidColor] = solidColorEditor;
            sceneEditors[SceneType.Gradient] = gradientEditor;
            sceneEditors[SceneType.VolumeReactive] = volumeEditor;
            sceneEditors[SceneType.ScreenRowCapture] = screenRowEditor;
            sceneEditors[SceneType.SpectralAnalysis] = spectralEditor;
            sceneEditors[SceneType.SpectralAnalysisSegments] = spectralSegmentsEditor;
            sceneEditors[SceneType.ImageRowCapture] = imageRowEditor;
            sceneEditors[SceneType.SparkleAndFlash] = sparkleAndFlashEditor;
            sceneEditors[SceneType.LaserDmx] = laserDmxEditor;
            sceneEditors[SceneType.Strobe] = strobeEditor;
            sceneEditors[SceneType.LedStrobe] = ledStrobeEditor;

            foreach (Form editor in sceneEditors.Values)
            {
                editor.Visible = false;
                panelSceneEditorHost.Controls.Add(editor);
                editor.Show();
            }
        }

        private void ConfigureDeviceGrid()
        {
            ApplyCompactRowHeight(dgvDevices);
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

            ledSceneLookupBindingSource.DataSource = ledSceneOptions;
            laserSceneLookupBindingSource.DataSource = laserSceneOptions;
            strobeSceneLookupBindingSource.DataSource = strobeSceneOptions;

            colAssignedScene.DataSource = ledSceneLookupBindingSource;
            colAssignedScene.DisplayMember = nameof(SceneAssignmentOption.Name);
            colAssignedScene.ValueMember = nameof(SceneAssignmentOption.Id);
            colAssignedScene.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;

            colAssignedLaserScene.DataSource = laserSceneLookupBindingSource;
            colAssignedLaserScene.DisplayMember = nameof(SceneAssignmentOption.Name);
            colAssignedLaserScene.ValueMember = nameof(SceneAssignmentOption.Id);
            colAssignedLaserScene.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;

            colAssignedStrobeScene.DataSource = strobeSceneLookupBindingSource;
            colAssignedStrobeScene.DisplayMember = nameof(SceneAssignmentOption.Name);
            colAssignedStrobeScene.ValueMember = nameof(SceneAssignmentOption.Id);
            colAssignedStrobeScene.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
        }

        private void ConfigureSceneGrid()
        {
            sceneGridBindingSource.DataSource = sceneRows;

            ApplyCompactRowHeight(dgvScenes);
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

        private void ConfigureCollectionGrid()
        {
            collectionGridBindingSource.DataSource = collectionRows;

            ApplyCompactRowHeight(dgvCollections);
            dgvCollections.AutoGenerateColumns = false;
            dgvCollections.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCollections.MultiSelect = false;
            dgvCollections.DataSource = collectionGridBindingSource;
            dgvCollections.CurrentCellDirtyStateChanged += dgvCollections_CurrentCellDirtyStateChanged;
            dgvCollections.CellValueChanged += dgvCollections_CellValueChanged;
            dgvCollections.CellEndEdit += dgvCollections_CellEndEdit;
            dgvCollections.CellMouseClick += dgvCollections_CellMouseClick;
            dgvCollections.CellValidating += dgvCollections_CellValidating;
            dgvCollections.DataError += dgvCollections_DataError;

            colCollectionMode.DataSource = Enum.GetValues<CollectionActivationMode>();
            colCollectionMode.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            cmbCollectionAutoMode.DataSource = Enum.GetValues<CollectionAutoSelectionMode>();
            cmbCollectionAutoMode.SelectedIndexChanged += cmbCollectionAutoMode_SelectedIndexChanged;
            numCollectionAutoPeriod.Minimum = MinCollectionAutoSelectionPeriodSeconds;
            numCollectionAutoPeriod.Maximum = MaxCollectionAutoSelectionPeriodSeconds;
            numCollectionAutoPeriod.ValueChanged += numCollectionAutoPeriod_ValueChanged;
            collectionAutoSelectionTimer.Tick += collectionAutoSelectionTimer_Tick;
        }

        private static void ApplyCompactRowHeight(DataGridView grid)
        {
            int compactHeight = Math.Max(1, (int)Math.Round(grid.RowTemplate.Height * 0.8, MidpointRounding.AwayFromZero));
            grid.RowTemplate.Height = compactHeight;
        }

        private void InitializeGlobalShortcutManager()
        {
            try
            {
                globalShortcutManager = new GlobalShortcutManager();
                globalShortcutManager.ShortcutKeyDown += GlobalShortcutManager_ShortcutKeyDown;
                globalShortcutManager.ShortcutKeyUp += GlobalShortcutManager_ShortcutKeyUp;
            }
            catch
            {
                globalShortcutManager = null;
            }
        }

        private void RefreshSceneAssignmentOptions()
        {
            RebuildSceneAssignmentOptions(
                ledSceneOptions,
                appConfig.Scenes.Where(scene => SceneTypeRules.SupportsStripAssignment(scene.Type)),
                includeNone: true);
            RebuildSceneAssignmentOptions(
                laserSceneOptions,
                appConfig.Scenes.Where(scene => SceneTypeRules.IsLaser(scene.Type)),
                includeNone: true);
            RebuildSceneAssignmentOptions(
                strobeSceneOptions,
                appConfig.Scenes.Where(scene => SceneTypeRules.IsStrobe(scene.Type)),
                includeNone: true);

            ledSceneLookupBindingSource.ResetBindings(false);
            laserSceneLookupBindingSource.ResetBindings(false);
            strobeSceneLookupBindingSource.ResetBindings(false);
        }

        private static void RebuildSceneAssignmentOptions(
            BindingList<SceneAssignmentOption> target,
            IEnumerable<SceneConfig> scenes,
            bool includeNone)
        {
            target.Clear();
            if (includeNone)
            {
                target.Add(new SceneAssignmentOption(string.Empty, "(None)"));
            }

            foreach (SceneConfig scene in scenes.OrderBy(scene => scene.Name, StringComparer.CurrentCultureIgnoreCase))
            {
                target.Add(new SceneAssignmentOption(scene.Id, scene.Name));
            }
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            isLoading = true;
            CollectionAutoSelectionMode autoSelectionMode = CollectionAutoSelectionMode.Off;
            try
            {
                appConfig.Load();
                ApplyConfigToUi();
                autoSelectionMode = appConfig.CollectionAutoSelection.Mode;
                LoadAudioDevices();
                CountHz();
            }
            finally
            {
                isLoading = false;
            }

            if (autoSelectionMode != CollectionAutoSelectionMode.Off
                && await StartCollectionAutoSelectionAsync(autoSelectionMode, showMessage: false))
            {
                return;
            }

            await ReconcileDeviceRunsAsync();
        }

        private void ApplyConfigToUi()
        {
            numDelay.Value = Math.Max(numDelay.Minimum, Math.Min(numDelay.Maximum, appConfig.Delay));
            ApplyCollectionAutoSelectionSettingsToUi();

            sceneRows.Clear();
            foreach (SceneConfig scene in appConfig.Scenes)
            {
                sceneRows.Add(SceneGridRow.FromSceneConfig(scene));
            }
            RefreshSceneAssignmentOptions();

            deviceRows.Clear();
            foreach (DeviceConfig device in appConfig.Devices)
            {
                foreach (DeviceGridRow row in BuildDeviceRows(device))
                {
                    deviceRows.Add(row);
                }
            }

            RefreshCollectionRows();
            lblResetShortcut.Text = $"Reset shortcut: {FormatShortcut(appConfig.ResetShortcut)}";

            sceneGridBindingSource.ResetBindings(false);
            collectionGridBindingSource.ResetBindings(false);
            ledSceneLookupBindingSource.ResetBindings(false);
            laserSceneLookupBindingSource.ResetBindings(false);
            strobeSceneLookupBindingSource.ResetBindings(false);

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
            appConfig.CollectionAutoSelection.Mode = GetSelectedCollectionAutoSelectionMode();
            appConfig.CollectionAutoSelection.PeriodSeconds = (int)numCollectionAutoPeriod.Value;
            appConfig.Devices = GetRootDeviceRows()
                .Select(BuildDeviceConfigFromRows)
                .ToList();
            SyncCollectionsFromRows();
        }

        private void ApplyCollectionAutoSelectionSettingsToUi()
        {
            syncingCollectionAutoSelectionControls = true;
            try
            {
                CollectionAutoSelectionSettings settings = appConfig.CollectionAutoSelection ?? new CollectionAutoSelectionSettings();
                cmbCollectionAutoMode.SelectedItem = Enum.IsDefined(typeof(CollectionAutoSelectionMode), settings.Mode)
                    ? settings.Mode
                    : CollectionAutoSelectionMode.Off;
                numCollectionAutoPeriod.Value = Math.Max(
                    numCollectionAutoPeriod.Minimum,
                    Math.Min(numCollectionAutoPeriod.Maximum, settings.PeriodSeconds));
                UpdateCollectionAutoSelectionTimerInterval();
            }
            finally
            {
                syncingCollectionAutoSelectionControls = false;
            }
        }

        private void RefreshCollectionRows()
        {
            collectionRows.Clear();
            foreach (ConfigurationCollection collection in appConfig.Collections)
            {
                collectionRows.Add(CollectionGridRow.FromCollection(collection, IsCollectionActive(collection.Id)));
            }
        }

        private void RefreshCollectionRow(ConfigurationCollection collection)
        {
            CollectionGridRow? row = collectionRows.FirstOrDefault(item => item.Id == collection.Id);
            if (row == null)
            {
                collectionRows.Add(CollectionGridRow.FromCollection(collection, IsCollectionActive(collection.Id)));
                return;
            }

            int rowIndex = collectionRows.IndexOf(row);
            CollectionGridRow refreshed = CollectionGridRow.FromCollection(collection, IsCollectionActive(collection.Id));
            row.Name = refreshed.Name;
            row.ActivationMode = refreshed.ActivationMode;
            row.IncludedInAutoSelection = refreshed.IncludedInAutoSelection;
            row.ShortcutText = refreshed.ShortcutText;
            row.TargetSummary = refreshed.TargetSummary;
            row.StatusText = refreshed.StatusText;
            if (rowIndex >= 0)
            {
                collectionGridBindingSource.ResetItem(rowIndex);
            }
        }

        private void RefreshCollectionStatuses()
        {
            foreach (CollectionGridRow row in collectionRows)
            {
                int rowIndex = collectionRows.IndexOf(row);
                row.StatusText = IsCollectionActive(row.Id) ? "Active" : "Inactive";
                if (rowIndex >= 0)
                {
                    collectionGridBindingSource.ResetItem(rowIndex);
                }
            }
        }

        private bool IsCollectionActive(string collectionId)
        {
            return collectionOverrideActive && string.Equals(activeCollectionId, collectionId, StringComparison.Ordinal);
        }

        private void SyncCollectionsFromRows()
        {
            foreach (CollectionGridRow row in collectionRows)
            {
                ConfigurationCollection? collection = FindCollectionById(row.Id);
                if (collection == null)
                {
                    continue;
                }

                collection.Name = string.IsNullOrWhiteSpace(row.Name) ? collection.Name : row.Name.Trim();
                collection.ActivationMode = row.ActivationMode;
                collection.IncludedInAutoSelection = row.IncludedInAutoSelection;
            }
        }

        private ConfigurationCollection? FindCollectionById(string collectionId)
        {
            return appConfig.Collections.FirstOrDefault(collection => string.Equals(collection.Id, collectionId, StringComparison.Ordinal));
        }

        private ConfigurationCollection? GetSelectedCollection()
        {
            return dgvCollections.CurrentRow?.DataBoundItem is CollectionGridRow row
                ? FindCollectionById(row.Id)
                : null;
        }

        private static string FormatShortcut(KeyboardShortcutConfig? shortcut)
        {
            string text = shortcut?.ToString() ?? string.Empty;
            return string.IsNullOrWhiteSpace(text) ? "none" : text;
        }

        private CollectionAutoSelectionMode GetSelectedCollectionAutoSelectionMode()
        {
            return cmbCollectionAutoMode.SelectedItem is CollectionAutoSelectionMode mode
                && Enum.IsDefined(typeof(CollectionAutoSelectionMode), mode)
                ? mode
                : CollectionAutoSelectionMode.Off;
        }

        private void SetCollectionAutoSelectionMode(CollectionAutoSelectionMode mode)
        {
            appConfig.CollectionAutoSelection.Mode = mode;
            syncingCollectionAutoSelectionControls = true;
            try
            {
                cmbCollectionAutoMode.SelectedItem = mode;
            }
            finally
            {
                syncingCollectionAutoSelectionControls = false;
            }
        }

        private bool IsCollectionAutoSelectionEnabled()
        {
            return collectionAutoSelectionTimer.Enabled
                || appConfig.CollectionAutoSelection.Mode != CollectionAutoSelectionMode.Off;
        }

        private void UpdateCollectionAutoSelectionTimerInterval()
        {
            int periodSeconds = Math.Max(
                MinCollectionAutoSelectionPeriodSeconds,
                Math.Min(MaxCollectionAutoSelectionPeriodSeconds, appConfig.CollectionAutoSelection.PeriodSeconds));
            appConfig.CollectionAutoSelection.PeriodSeconds = periodSeconds;

            bool restartTimer = collectionAutoSelectionTimer.Enabled;
            if (restartTimer)
            {
                collectionAutoSelectionTimer.Stop();
            }

            collectionAutoSelectionTimer.Interval = periodSeconds * 1000;
            if (restartTimer)
            {
                collectionAutoSelectionTimer.Start();
            }
        }

        private async Task<bool> StartCollectionAutoSelectionAsync(CollectionAutoSelectionMode mode, bool showMessage)
        {
            if (mode == CollectionAutoSelectionMode.Off)
            {
                await StopCollectionAutoSelectionAsync(resumeDefault: true);
                return false;
            }

            bool wasAutoSelectionEnabled = IsCollectionAutoSelectionEnabled();
            SyncCollectionsFromRows();
            if (GetEligibleAutoSelectionCollections().Count == 0)
            {
                collectionAutoSelectionTimer.Stop();
                SetCollectionAutoSelectionMode(CollectionAutoSelectionMode.Off);
                if (wasAutoSelectionEnabled)
                {
                    await StopCollectionOverrideAsync(resumeDefault: true);
                }

                if (showMessage)
                {
                    MessageBox.Show(this, "Select at least one collection in the Auto column before enabling automatic collection switching.", "No Automatic Collections", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return false;
            }

            SetCollectionAutoSelectionMode(mode);
            UpdateCollectionAutoSelectionTimerInterval();
            if (!await SwitchToNextAutomaticCollectionAsync(isInitial: true))
            {
                return false;
            }

            collectionAutoSelectionTimer.Start();
            return true;
        }

        private async Task StopCollectionAutoSelectionAsync(bool resumeDefault)
        {
            collectionAutoSelectionTimer.Stop();
            SetCollectionAutoSelectionMode(CollectionAutoSelectionMode.Off);
            await StopCollectionOverrideAsync(resumeDefault);
        }

        private async Task HandleCollectionAutoSelectionEligibilityChangedAsync()
        {
            if (!IsCollectionAutoSelectionEnabled())
            {
                return;
            }

            List<ConfigurationCollection> eligibleCollections = GetEligibleAutoSelectionCollections();
            if (eligibleCollections.Count == 0)
            {
                await StopCollectionAutoSelectionAsync(resumeDefault: true);
                return;
            }

            bool activeCollectionStillEligible = !string.IsNullOrWhiteSpace(activeCollectionId)
                && eligibleCollections.Any(collection => string.Equals(collection.Id, activeCollectionId, StringComparison.Ordinal));
            if (!collectionOverrideActive || !activeCollectionStillEligible)
            {
                await SwitchToNextAutomaticCollectionAsync(isInitial: false);
            }
        }

        private async Task<bool> SwitchToNextAutomaticCollectionAsync(bool isInitial)
        {
            CollectionAutoSelectionMode mode = appConfig.CollectionAutoSelection.Mode;
            if (mode == CollectionAutoSelectionMode.Off)
            {
                collectionAutoSelectionTimer.Stop();
                return false;
            }

            SyncCollectionsFromRows();
            ConfigurationCollection? collection = GetNextAutomaticCollection(mode, isInitial);
            if (collection == null)
            {
                await StopCollectionAutoSelectionAsync(resumeDefault: true);
                return false;
            }

            if (collectionOverrideActive && string.Equals(activeCollectionId, collection.Id, StringComparison.Ordinal))
            {
                return true;
            }

            await StartCollectionOverrideAsync(collection, CollectionActivationMode.Toggle, holdShortcut: null);
            return true;
        }

        private ConfigurationCollection? GetNextAutomaticCollection(CollectionAutoSelectionMode mode, bool isInitial)
        {
            List<ConfigurationCollection> eligibleCollections = GetEligibleAutoSelectionCollections();
            if (eligibleCollections.Count == 0)
            {
                return null;
            }

            return mode switch
            {
                CollectionAutoSelectionMode.Random => GetRandomAutomaticCollection(eligibleCollections),
                CollectionAutoSelectionMode.Ascending => GetSequentialAutomaticCollection(eligibleCollections, ascending: true, isInitial: isInitial),
                CollectionAutoSelectionMode.Descending => GetSequentialAutomaticCollection(eligibleCollections, ascending: false, isInitial: isInitial),
                _ => null
            };
        }

        private List<ConfigurationCollection> GetEligibleAutoSelectionCollections()
        {
            return collectionRows
                .Select(row => FindCollectionById(row.Id))
                .Where(collection => collection != null && collection.IncludedInAutoSelection && collection.HasTargets())
                .Cast<ConfigurationCollection>()
                .ToList();
        }

        private ConfigurationCollection GetRandomAutomaticCollection(List<ConfigurationCollection> eligibleCollections)
        {
            List<ConfigurationCollection> candidates = eligibleCollections;
            if (eligibleCollections.Count > 1 && !string.IsNullOrWhiteSpace(activeCollectionId))
            {
                List<ConfigurationCollection> nonActiveCandidates = eligibleCollections
                    .Where(collection => !string.Equals(collection.Id, activeCollectionId, StringComparison.Ordinal))
                    .ToList();
                if (nonActiveCandidates.Count > 0)
                {
                    candidates = nonActiveCandidates;
                }
            }

            return candidates[collectionAutoSelectionRandom.Next(candidates.Count)];
        }

        private ConfigurationCollection GetSequentialAutomaticCollection(List<ConfigurationCollection> eligibleCollections, bool ascending, bool isInitial)
        {
            if (isInitial || string.IsNullOrWhiteSpace(activeCollectionId))
            {
                return ascending ? eligibleCollections[0] : eligibleCollections[^1];
            }

            int activeEligibleIndex = eligibleCollections.FindIndex(collection => string.Equals(collection.Id, activeCollectionId, StringComparison.Ordinal));
            if (activeEligibleIndex >= 0)
            {
                int nextEligibleIndex = ascending
                    ? (activeEligibleIndex + 1) % eligibleCollections.Count
                    : (activeEligibleIndex - 1 + eligibleCollections.Count) % eligibleCollections.Count;
                return eligibleCollections[nextEligibleIndex];
            }

            int activeRowIndex = collectionRows
                .Select((row, index) => new { row.Id, Index = index })
                .FirstOrDefault(item => string.Equals(item.Id, activeCollectionId, StringComparison.Ordinal))
                ?.Index ?? -1;
            if (activeRowIndex >= 0)
            {
                for (int offset = 1; offset <= collectionRows.Count; offset++)
                {
                    int candidateIndex = ascending
                        ? (activeRowIndex + offset) % collectionRows.Count
                        : (activeRowIndex - offset + collectionRows.Count) % collectionRows.Count;
                    ConfigurationCollection? candidate = FindCollectionById(collectionRows[candidateIndex].Id);
                    if (candidate != null && candidate.IncludedInAutoSelection && candidate.HasTargets())
                    {
                        return candidate;
                    }
                }
            }

            return ascending ? eligibleCollections[0] : eligibleCollections[^1];
        }

        private IEnumerable<DeviceGridRow> BuildDeviceRows(DeviceConfig device)
        {
            yield return DeviceGridRow.FromDeviceConfig(device);

            if (device.StripCount <= 0)
            {
                yield break;
            }

            foreach (DeviceStripConfig strip in device.Strips)
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
                AssignedLaserSceneId = rootRow.AssignedLaserSceneId,
                AssignedStrobeSceneId = rootRow.AssignedStrobeSceneId,
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

                if (sceneEditors[SceneType.SpectralAnalysisSegments] is SpectralAnalysisSegmentsSceneEditorForm spectralSegmentsEditor)
                {
                    spectralSegmentsEditor.LoadAudioDevices(selectedAudioDeviceId);
                    selectedAudioDeviceId ??= spectralSegmentsEditor.GetSelectedAudioDeviceId();
                }

                if (sceneEditors[SceneType.LedStrobe] is LedStrobeSceneEditorForm ledStrobeEditor)
                {
                    ledStrobeEditor.LoadAudioDevices(selectedAudioDeviceId);
                    selectedAudioDeviceId ??= ledStrobeEditor.GetSelectedAudioDeviceId();
                }
            }
            finally
            {
                syncingAudioDeviceSelection = false;
            }
        }

        private async Task ReconcileDeviceRunsAsync()
        {
            if (isLoading || collectionOverrideChanging)
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

                    Dictionary<string, DeviceConfig> desiredDevices = GetDesiredRunDevices();

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

        private Dictionary<string, DeviceConfig> GetDesiredRunDevices()
        {
            if (collectionOverrideActive)
            {
                return GetActiveCollectionSnapshots()
                    .Select(BuildDeviceConfigFromSnapshot)
                    .Where(config => config.LedCount > 0 && IsValidDeviceConfig(config))
                    .ToDictionary(config => config.Id, config => config, StringComparer.Ordinal);
            }

            return GetRootDeviceRows()
                .Select(BuildDeviceConfigFromRows)
                .Where(HasActiveTargets)
                .ToDictionary(device => device.Id, device => device, StringComparer.Ordinal);
        }

        private void UpdateInvalidDeviceStatuses()
        {
            if (collectionOverrideActive)
            {
                UpdateCollectionOverrideDeviceStatuses();
                return;
            }

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

                bool rowHasTarget = row.Enabled;
                SceneConfig? assignedScene = FindSceneById(row.AssignedSceneId);
                bool requiresLedScene = row.Kind == DeviceRowKind.Strip && rowHasTarget;
                if (requiresLedScene && assignedScene == null)
                {
                    row.Status = "Scene missing";
                }
                else if (row.Kind == DeviceRowKind.Strip
                    && requiresLedScene
                    && assignedScene != null
                    && !SceneTypeRules.SupportsStripAssignment(assignedScene.Type))
                {
                    row.Status = "Scene incompatible";
                }
                else if (row.Kind == DeviceRowKind.Device
                    && row.Enabled
                    && !IsValidAuxiliarySceneAssignment(row.AssignedLaserSceneId, SceneTypeRules.IsLaser, out string? laserStatus))
                {
                    row.Status = laserStatus!;
                }
                else if (row.Kind == DeviceRowKind.Device
                    && row.Enabled
                    && !IsValidAuxiliarySceneAssignment(row.AssignedStrobeSceneId, SceneTypeRules.IsStrobe, out string? strobeStatus))
                {
                    row.Status = strobeStatus!;
                }
                else if (!deviceRuns.ContainsKey(rootRow.Id) && row.Status.StartsWith("Offline", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else if (!deviceRuns.ContainsKey(rootRow.Id))
                {
                    row.Status = "Pending";
                }
                else
                {
                    row.Status = ResolvePostMetadataStatus(rootRow.Id);
                }
            }

            dgvDevices.Refresh();
        }

        private void UpdateCollectionOverrideDeviceStatuses()
        {
            foreach (DeviceGridRow row in deviceRows)
            {
                DeviceGridRow rootRow = row.Kind == DeviceRowKind.Device
                    ? row
                    : FindRootDeviceRow(row.ParentDeviceId) ?? row;
                CollectionDeviceSnapshot? snapshot = GetActiveCollectionSnapshot(rootRow.Id);
                bool rowHasTarget = snapshot != null
                    && (row.Kind == DeviceRowKind.Device
                        ? HasSnapshotTargets(snapshot)
                        : IsCollectionStripActive(rootRow.Id, row.StripIndex));

                if (!rowHasTarget)
                {
                    row.Status = "Disconnected";
                    continue;
                }

                DeviceConfig snapshotConfig = BuildDeviceConfigFromSnapshot(snapshot!);
                if (!IsValidDeviceConfig(snapshotConfig))
                {
                    row.Status = "Invalid";
                }
                else if (snapshotConfig.LedCount <= 0)
                {
                    row.Status = "Metadata unavailable";
                }
                else if (deviceRuns.ContainsKey(rootRow.Id))
                {
                    row.Status = ResolvePostMetadataStatus(rootRow.Id);
                }
                else if (row.Status.StartsWith("Offline", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else
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

        private async Task StartCollectionOverrideAsync(ConfigurationCollection collection, CollectionActivationMode activationMode, KeyboardShortcutConfig? holdShortcut)
        {
            if (collectionOverrideChanging)
            {
                return;
            }

            collectionOverrideChanging = true;
            try
            {
                collectionOverrideActive = true;
                activeCollectionId = collection.Id;
                activeCollectionMode = activationMode;
                activeHoldShortcut = holdShortcut?.Clone();
                RefreshCollectionStatuses();
            }
            finally
            {
                collectionOverrideChanging = false;
            }

            await ReconcileDeviceRunsAsync();
        }

        private async Task StopCollectionOverrideAsync(bool resumeDefault)
        {
            if (!collectionOverrideActive)
            {
                return;
            }

            collectionOverrideChanging = true;
            try
            {
                collectionOverrideActive = false;
                activeCollectionId = null;
                activeCollectionMode = null;
                activeHoldShortcut = null;
                RefreshCollectionStatuses();
            }
            finally
            {
                collectionOverrideChanging = false;
            }

            if (resumeDefault)
            {
                await ReconcileDeviceRunsAsync();
            }
        }

        private static bool HasSnapshotTargets(CollectionDeviceSnapshot snapshot)
        {
            return snapshot.LaserScene != null
                || snapshot.StrobeScene != null
                || snapshot.Strips.Any(strip => strip.Scene != null && strip.LedCount > 0);
        }

        private IEnumerable<CollectionDeviceSnapshot> GetActiveCollectionSnapshots()
        {
            if (!collectionOverrideActive || string.IsNullOrWhiteSpace(activeCollectionId))
            {
                return Enumerable.Empty<CollectionDeviceSnapshot>();
            }

            ConfigurationCollection? collection = FindCollectionById(activeCollectionId);
            return collection?.Devices.Where(HasSnapshotTargets) ?? Enumerable.Empty<CollectionDeviceSnapshot>();
        }

        private CollectionDeviceSnapshot? GetActiveCollectionSnapshot(string deviceId)
        {
            return GetActiveCollectionSnapshots()
                .FirstOrDefault(snapshot => string.Equals(snapshot.DeviceId, deviceId, StringComparison.Ordinal));
        }

        private bool IsCollectionDeviceActive(string deviceId)
        {
            return GetActiveCollectionSnapshot(deviceId) != null;
        }

        private bool IsCollectionStripActive(string deviceId, int stripIndex)
        {
            CollectionDeviceSnapshot? snapshot = GetActiveCollectionSnapshot(deviceId);
            return snapshot?.Strips.Any(strip =>
                strip.StripIndex == stripIndex
                && strip.Scene != null
                && strip.LedCount > 0) == true;
        }

        private DeviceConfig BuildDeviceConfigFromSnapshot(CollectionDeviceSnapshot snapshot)
        {
            return new DeviceConfig
            {
                Id = snapshot.DeviceId,
                Name = snapshot.Name,
                Host = snapshot.Host,
                Port = snapshot.Port,
                LedCount = snapshot.LedCount,
                StripCount = snapshot.StripCount,
                Enabled = snapshot.LaserScene != null || snapshot.StrobeScene != null,
                AssignedSceneId = string.Empty,
                AssignedLaserSceneId = snapshot.LaserScene?.Id ?? string.Empty,
                AssignedStrobeSceneId = snapshot.StrobeScene?.Id ?? string.Empty,
                Strips = snapshot.Strips
                    .Select(strip => new DeviceStripConfig
                    {
                        StripIndex = strip.StripIndex,
                        LedCount = strip.LedCount,
                        Enabled = strip.Scene != null,
                        AssignedSceneId = strip.Scene?.Id ?? string.Empty
                    })
                    .ToList()
            };
        }

        private static IReadOnlyList<DeviceSceneAssignment> GetSnapshotSceneAssignments(CollectionDeviceSnapshot snapshot)
        {
            return snapshot.Strips
                .Where(strip => strip.Scene != null && strip.LedCount > 0)
                .Select(strip => new DeviceSceneAssignment
                {
                    Scene = strip.Scene!,
                    StartIndex = strip.StartIndex,
                    LedCount = strip.LedCount
                })
                .ToList();
        }

        private bool RequiresRestart(DeviceRunEntry current, DeviceConfig desired)
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

            if (device.Enabled && (IsLaserSceneAssigned(device.AssignedLaserSceneId) || IsStrobeSceneAssigned(device.AssignedStrobeSceneId)))
            {
                return true;
            }

            return device.Strips.Any(strip =>
            {
                SceneConfig? scene = FindSceneById(strip.AssignedSceneId);
                return strip.Enabled
                    && strip.LedCount > 0
                    && scene != null
                    && SceneTypeRules.SupportsStripAssignment(scene.Type);
            });
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

        private bool HasActiveRuntimeTarget(string deviceId)
        {
            return collectionOverrideActive
                ? IsCollectionDeviceActive(deviceId)
                : HasAnyEnabledTarget(deviceId);
        }

        private string ResolveActiveStatus(string deviceId)
        {
            return IsCollectionDeviceActive(deviceId)
                ? "Collection active"
                : deviceRuns.ContainsKey(deviceId) ? "Effect active" : "Online";
        }

        private string ResolvePostMetadataStatus(string deviceId)
        {
            DeviceGridRow? rootRow = FindRootDeviceRow(deviceId);
            if (rootRow == null || !HasActiveRuntimeTarget(deviceId))
            {
                return "Disconnected";
            }

            return rootRow.Status switch
            {
                "Connecting" => "Connecting",
                string status when status.StartsWith("Offline", StringComparison.OrdinalIgnoreCase) => status,
                "Invalid" when !collectionOverrideActive => "Invalid",
                "Metadata unavailable" when !collectionOverrideActive => "Metadata unavailable",
                "Scene missing" when !collectionOverrideActive => "Scene missing",
                "Scene incompatible" when !collectionOverrideActive => "Scene incompatible",
                _ => ResolveActiveStatus(deviceId)
            };
        }

        private static bool IsValidDeviceConfig(DeviceConfig device)
        {
            return !string.IsNullOrWhiteSpace(device.Host)
                && device.Port > 0
                && device.Port <= 65535;
        }

        private string BuildRunSignature(DeviceConfig config)
        {
            string stripSignature = string.Join("|", config.Strips
                .OrderBy(strip => strip.StripIndex)
                .Select(strip => $"{strip.StripIndex}:{strip.LedCount}"));
            return $"{config.Host}|{config.Port}|{config.LedCount}|{config.StripCount}|{stripSignature}";
        }

        private ISceneRunner CreateCompositeSceneRunner(string deviceId)
        {
            return new CompositeSceneRunner(
                () => GetActiveSceneAssignments(deviceId),
                () => GetActiveLaserScene(deviceId),
                () => GetActiveStrobeScene(deviceId),
                () => selectedAudioDeviceId,
                () => ReadUi(() => (int)numDelay.Value),
                UpdatePreview,
                UpdateVolumeProgress,
                UpdateSpectralProgress,
                UpdateRate);
        }

        private IReadOnlyList<DeviceSceneAssignment> GetActiveSceneAssignments(string deviceId)
        {
            if (collectionOverrideActive)
            {
                CollectionDeviceSnapshot? snapshot = GetActiveCollectionSnapshot(deviceId);
                return snapshot == null ? Array.Empty<DeviceSceneAssignment>() : GetSnapshotSceneAssignments(snapshot);
            }

            return GetDeviceSceneAssignments(deviceId);
        }

        private SceneConfig? GetActiveLaserScene(string deviceId)
        {
            if (collectionOverrideActive)
            {
                return GetActiveCollectionSnapshot(deviceId)?.LaserScene;
            }

            return GetAssignedLaserScene(deviceId);
        }

        private SceneConfig? GetActiveStrobeScene(string deviceId)
        {
            if (collectionOverrideActive)
            {
                return GetActiveCollectionSnapshot(deviceId)?.StrobeScene;
            }

            return GetAssignedStrobeScene(deviceId);
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

            foreach (DeviceGridRow stripRow in GetStripRows(deviceId))
            {
                if (stripRow.Enabled)
                {
                    SceneConfig? stripScene = FindSceneById(stripRow.AssignedSceneId);
                    if (stripScene != null && stripRow.LedCount > 0 && SceneTypeRules.SupportsStripAssignment(stripScene.Type))
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

        private SceneConfig? GetAssignedLaserScene(string deviceId)
        {
            DeviceGridRow? rootRow = FindRootDeviceRow(deviceId);
            if (rootRow == null || !rootRow.Enabled)
            {
                return null;
            }

            SceneConfig? scene = FindSceneById(rootRow.AssignedLaserSceneId);
            return scene != null && SceneTypeRules.IsLaser(scene.Type) ? scene : null;
        }

        private SceneConfig? GetAssignedStrobeScene(string deviceId)
        {
            DeviceGridRow? rootRow = FindRootDeviceRow(deviceId);
            if (rootRow == null || !rootRow.Enabled)
            {
                return null;
            }

            SceneConfig? scene = FindSceneById(rootRow.AssignedStrobeSceneId);
            return scene != null && SceneTypeRules.IsStrobe(scene.Type) ? scene : null;
        }

        private bool IsLedSceneAssigned(string sceneId)
        {
            SceneConfig? scene = FindSceneById(sceneId);
            return scene != null && SceneTypeRules.SupportsStripAssignment(scene.Type);
        }

        private bool IsLaserSceneAssigned(string sceneId)
        {
            SceneConfig? scene = FindSceneById(sceneId);
            return scene != null && SceneTypeRules.IsLaser(scene.Type);
        }

        private bool IsStrobeSceneAssigned(string sceneId)
        {
            SceneConfig? scene = FindSceneById(sceneId);
            return scene != null && SceneTypeRules.IsStrobe(scene.Type);
        }

        private bool IsValidAuxiliarySceneAssignment(string sceneId, Func<SceneType, bool> matcher, out string? status)
        {
            status = null;
            if (string.IsNullOrWhiteSpace(sceneId))
            {
                return true;
            }

            SceneConfig? scene = FindSceneById(sceneId);
            if (scene == null)
            {
                status = "Scene missing";
                return false;
            }

            if (!matcher(scene.Type))
            {
                status = "Scene incompatible";
                return false;
            }

            return true;
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
                SaturationValue = scene.VolumeReactive.Saturation,
                NormalizationValue = scene.VolumeReactive.Normalization,
                Reverse = scene.VolumeReactive.Reverse,
                HueReverse = scene.VolumeReactive.HueReverse,
                White = scene.VolumeReactive.White,
                BackgroundWhite = scene.VolumeReactive.BackgroundWhite,
                BackgroundBrightnessValue = scene.VolumeReactive.BackgroundBrightness,
                BackgroundSaturationValue = scene.VolumeReactive.BackgroundSaturation,
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
                SaturationValue = scene.SpectralAnalysis.Saturation,
                NormalizationValue = scene.SpectralAnalysis.Normalization,
                Reverse = scene.SpectralAnalysis.Reverse,
                HueReverse = scene.SpectralAnalysis.HueReverse,
                White = scene.SpectralAnalysis.White,
                BackgroundWhite = scene.SpectralAnalysis.BackgroundWhite,
                BackgroundBrightnessValue = scene.SpectralAnalysis.BackgroundBrightness,
                BackgroundSaturationValue = scene.SpectralAnalysis.BackgroundSaturation,
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

        private void UpdatePreview(CaptureScenePreview preview)
        {
            SceneConfig? scene = FindSceneById(preview.SceneId);
            if (scene == null)
            {
                return;
            }

            if (scene.Type == SceneType.ScreenRowCapture
                && sceneEditors[SceneType.ScreenRowCapture] is ScreenRowCaptureSceneEditorForm screenRowEditor)
            {
                screenRowEditor.UpdatePreview(preview);
            }
            else if (scene.Type == SceneType.ImageRowCapture
                && sceneEditors[SceneType.ImageRowCapture] is ImageRowCaptureSceneEditorForm imageRowEditor)
            {
                imageRowEditor.UpdatePreview(preview);
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

            if (sceneEditors[SceneType.SpectralAnalysisSegments] is SpectralAnalysisSegmentsSceneEditorForm spectralSegmentsEditor)
            {
                spectralSegmentsEditor.UpdateProgress(value);
            }
        }

        private void UpdateRate(string text)
        {
            SafeUi(() => statLblRate.Text = text);
        }

        private async void LaserDmxEditor_SendRequested(object? sender, LaserDmxSendRequestedEventArgs e)
        {
            List<DeviceGridRow> rows = GetActiveAssignedDeviceRows(
                row => string.Equals(row.AssignedLaserSceneId, e.SceneId, StringComparison.Ordinal));
            foreach (DeviceGridRow row in rows)
            {
                await SendAuxiliaryPayloadToDevicesAsync(
                    new[] { BuildDeviceConfigFromRows(row) },
                    BuildAuxiliaryPayloadForRow(row, row.AssignedLaserSceneId, null)).ConfigureAwait(false);
            }
        }

        private async void StrobeEditor_TestRequested(object? sender, StrobeTestRequestedEventArgs e)
        {
            List<DeviceGridRow> rows = GetActiveAssignedDeviceRows(
                row => string.Equals(row.AssignedStrobeSceneId, e.SceneId, StringComparison.Ordinal));
            foreach (DeviceGridRow row in rows)
            {
                await SendAuxiliaryPayloadToDevicesAsync(
                    new[] { BuildDeviceConfigFromRows(row) },
                    BuildAuxiliaryPayloadForRow(row, row.AssignedLaserSceneId, true)).ConfigureAwait(false);
            }

            List<string> activeDeviceIds = rows.Select(row => row.Id).ToList();
            if (activeDeviceIds.Count > 0)
            {
                _ = RestoreStrobeAfterTestAsync(activeDeviceIds);
            }
        }

        private async Task RestoreStrobeAfterTestAsync(IReadOnlyList<string> activeDeviceIds)
        {
            await Task.Delay(500).ConfigureAwait(false);

            foreach (string deviceId in activeDeviceIds)
            {
                DeviceGridRow? row = ReadUi(() => FindRootDeviceRow(deviceId));
                if (row == null || !row.Enabled)
                {
                    continue;
                }

                SceneConfig? scene = ReadUi(() => FindSceneById(row.AssignedStrobeSceneId));
                if (scene?.Type != SceneType.Strobe)
                {
                    continue;
                }

                DeviceConfig config = ReadUi(() => BuildDeviceConfigFromRows(row));
                byte[] payload = ReadUi(() => BuildAuxiliaryPayloadForRow(row, row.AssignedLaserSceneId, false));
                await SendAuxiliaryPayloadToDevicesAsync(
                    new[] { config },
                    payload).ConfigureAwait(false);
            }
        }

        private List<DeviceGridRow> GetActiveAssignedDeviceRows(Func<DeviceGridRow, bool> predicate)
        {
            return ReadUi(() => GetRootDeviceRows()
                .Where(row => row.Enabled
                    && predicate(row)
                    && deviceRuns.ContainsKey(row.Id))
                .ToList());
        }

        private byte[] BuildAuxiliaryPayloadForRow(DeviceGridRow row, string? laserSceneIdOverride, bool? strobeEnabledOverride)
        {
            string laserSceneId = laserSceneIdOverride ?? row.AssignedLaserSceneId;
            SceneConfig? laserScene = FindSceneById(laserSceneId);
            bool hasRuntimeState = AuxiliaryRuntimeRegistry.TryGet(row.Id, out byte[] currentPayload, out bool laserActive, out bool strobeActive);
            bool strobeEnabled = strobeEnabledOverride ?? strobeActive;
            List<(int Channel, int Value)> currentChannels = new();
            AuxiliaryDmxTransmissionOptions transmissionOptions = AuxiliaryPayloadBuilder.DefaultTransmissionOptions;
            bool parsedCurrentPayload = hasRuntimeState
                && AuxiliaryPayloadBuilder.TryParsePayload(currentPayload, out currentChannels, out _, out transmissionOptions);

            if (laserScene?.Type == SceneType.LaserDmx)
            {
                if (!manualLaserStates.TryGetValue(laserScene.Id, out LaserDmxRuntimeState? runtimeState))
                {
                    runtimeState = new LaserDmxRuntimeState();
                    manualLaserStates[laserScene.Id] = runtimeState;
                }

                return AuxiliaryPayloadBuilder.BuildLaserPayload(
                    laserScene.LaserDmx,
                    runtimeState,
                    true,
                    null,
                    strobeEnabled);
            }

            if (laserActive && parsedCurrentPayload)
            {
                return AuxiliaryPayloadBuilder.BuildExplicitPayload(currentChannels, strobeEnabled, transmissionOptions);
            }

            return strobeEnabled
                ? AuxiliaryPayloadBuilder.BuildStrobePayload(true, transmissionOptions)
                : AuxiliaryPayloadBuilder.BuildOffPayload(transmissionOptions);
        }

        private async Task SendAuxiliaryPayloadToDevicesAsync(IReadOnlyList<DeviceConfig> devices, byte[] payload)
        {
            foreach (DeviceConfig device in devices)
            {
                var session = new DeviceSession(device, (_, _, _) => { });
                try
                {
                    if (await session.ConnectAsync(CancellationToken.None).ConfigureAwait(false))
                    {
                        await session.SendFrameAsync(payload, CancellationToken.None).ConfigureAwait(false);
                    }
                }
                finally
                {
                    await session.DisconnectAsync(CancellationToken.None).ConfigureAwait(false);
                }
            }
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
                    _ => HasActiveRuntimeTarget(deviceId) ? "Pending" : "Disconnected"
                };

                rootRow.Status = status;
                foreach (DeviceGridRow stripRow in GetStripRows(deviceId))
                {
                    bool stripActive = collectionOverrideActive
                        ? IsCollectionStripActive(deviceId, stripRow.StripIndex)
                        : stripRow.Enabled;
                    stripRow.Status = stripActive ? status : "Disconnected";
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
            if (collectionOverrideActive)
            {
                int enabledCollectionCount = GetActiveCollectionSnapshots()
                    .Select(BuildDeviceConfigFromSnapshot)
                    .Count(config => config.LedCount > 0 && IsValidDeviceConfig(config));
                int connectedCollectionCount = GetRootDeviceRows()
                    .Count(item => IsCollectionDeviceActive(item.Id)
                        && item.Status is "Collection active" or "Online" or "Effect active");
                statLblConnection.Text = enabledCollectionCount == 0
                    ? "Collection active: no targets"
                    : $"Collection: {connectedCollectionCount}/{enabledCollectionCount} online";
                return;
            }

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

        private async void btnAddCollection_Click(object? sender, EventArgs e)
        {
            ConfigurationCollection? collection = CreateSnapshotCollection();
            if (collection == null)
            {
                MessageBox.Show(this, "There are no enabled LED strips, laser scenes, or strobe scenes to capture.", "No Enabled Targets", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            appConfig.Collections.Add(collection);
            collectionRows.Add(CollectionGridRow.FromCollection(collection, IsCollectionActive(collection.Id)));
            int rowIndex = collectionRows.Count - 1;
            if (rowIndex >= 0)
            {
                dgvCollections.ClearSelection();
                dgvCollections.Rows[rowIndex].Selected = true;
                dgvCollections.CurrentCell = dgvCollections.Rows[rowIndex].Cells[0];
            }

            await Task.CompletedTask;
        }

        private async void btnRemoveCollection_Click(object? sender, EventArgs e)
        {
            ConfigurationCollection? collection = GetSelectedCollection();
            if (collection == null)
            {
                return;
            }

            bool removedActiveCollection = string.Equals(activeCollectionId, collection.Id, StringComparison.Ordinal);
            bool autoSelectionEnabled = IsCollectionAutoSelectionEnabled();

            appConfig.Collections.Remove(collection);
            CollectionGridRow? row = collectionRows.FirstOrDefault(item => item.Id == collection.Id);
            if (row != null)
            {
                collectionRows.Remove(row);
            }

            if (autoSelectionEnabled)
            {
                await HandleCollectionAutoSelectionEligibilityChangedAsync();
            }
            else if (removedActiveCollection)
            {
                await StopCollectionOverrideAsync(resumeDefault: true);
            }
        }

        private void btnAssignCollectionShortcut_Click(object? sender, EventArgs e)
        {
            ConfigurationCollection? collection = GetSelectedCollection();
            if (collection == null)
            {
                return;
            }

            KeyboardShortcutConfig? shortcut = CaptureShortcut(collection.Shortcut, "Collection Shortcut");
            if (shortcut == null)
            {
                return;
            }

            if (!ValidateShortcut(shortcut, collection.Id, out string error))
            {
                MessageBox.Show(this, error, "Shortcut Conflict", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            collection.Shortcut = shortcut;
            RefreshCollectionRow(collection);
        }

        private void btnClearCollectionShortcut_Click(object? sender, EventArgs e)
        {
            ConfigurationCollection? collection = GetSelectedCollection();
            if (collection == null)
            {
                return;
            }

            collection.Shortcut = KeyboardShortcutConfig.Empty();
            RefreshCollectionRow(collection);
        }

        private void btnSetResetShortcut_Click(object? sender, EventArgs e)
        {
            KeyboardShortcutConfig? shortcut = CaptureShortcut(appConfig.ResetShortcut, "Reset Shortcut");
            if (shortcut == null)
            {
                return;
            }

            if (!ValidateShortcut(shortcut, excludedCollectionId: null, out string error))
            {
                MessageBox.Show(this, error, "Shortcut Conflict", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            appConfig.ResetShortcut = shortcut;
            lblResetShortcut.Text = $"Reset shortcut: {FormatShortcut(appConfig.ResetShortcut)}";
            collectionGridBindingSource.ResetBindings(false);
        }

        private async void btnStopCollection_Click(object? sender, EventArgs e)
        {
            await StopCollectionAutoSelectionAsync(resumeDefault: true);
        }

        private async void cmbCollectionAutoMode_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isLoading || syncingCollectionAutoSelectionControls)
            {
                return;
            }

            CollectionAutoSelectionMode mode = GetSelectedCollectionAutoSelectionMode();
            if (mode == CollectionAutoSelectionMode.Off)
            {
                await StopCollectionAutoSelectionAsync(resumeDefault: true);
                return;
            }

            await StartCollectionAutoSelectionAsync(mode, showMessage: true);
        }

        private void numCollectionAutoPeriod_ValueChanged(object? sender, EventArgs e)
        {
            if (isLoading || syncingCollectionAutoSelectionControls)
            {
                return;
            }

            appConfig.CollectionAutoSelection.PeriodSeconds = (int)numCollectionAutoPeriod.Value;
            UpdateCollectionAutoSelectionTimerInterval();
        }

        private async void collectionAutoSelectionTimer_Tick(object? sender, EventArgs e)
        {
            if (collectionAutoSelectionTickInProgress)
            {
                return;
            }

            collectionAutoSelectionTickInProgress = true;
            try
            {
                await SwitchToNextAutomaticCollectionAsync(isInitial: false);
            }
            finally
            {
                collectionAutoSelectionTickInProgress = false;
            }
        }

        private KeyboardShortcutConfig? CaptureShortcut(KeyboardShortcutConfig? current, string title)
        {
            using var form = new ShortcutCaptureForm(title, current);
            return form.ShowDialog(this) == DialogResult.OK ? form.Shortcut : null;
        }

        private bool ValidateShortcut(KeyboardShortcutConfig shortcut, string? excludedCollectionId, out string error)
        {
            error = string.Empty;
            if (shortcut.IsEmpty)
            {
                return true;
            }

            if (!shortcut.IsUsable)
            {
                error = "Shortcut must include a non-modifier key.";
                return false;
            }

            if (appConfig.ResetShortcut != null
                && !shortcut.IsEmpty
                && shortcut.Matches(appConfig.ResetShortcut)
                && excludedCollectionId != null)
            {
                error = "This shortcut is already assigned as the reset shortcut.";
                return false;
            }

            foreach (ConfigurationCollection collection in appConfig.Collections)
            {
                if (excludedCollectionId != null && string.Equals(collection.Id, excludedCollectionId, StringComparison.Ordinal))
                {
                    continue;
                }

                if (collection.Shortcut != null && !collection.Shortcut.IsEmpty && shortcut.Matches(collection.Shortcut))
                {
                    error = $"This shortcut is already assigned to '{collection.Name}'.";
                    return false;
                }
            }

            return true;
        }

        private ConfigurationCollection? CreateSnapshotCollection()
        {
            SyncConfigFromUi();
            var collection = new ConfigurationCollection
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"Snapshot {appConfig.Collections.Count + 1}",
                ActivationMode = CollectionActivationMode.Toggle,
                CreatedUtc = DateTime.UtcNow
            };

            foreach (DeviceGridRow rootRow in GetRootDeviceRows())
            {
                CollectionDeviceSnapshot? snapshot = CreateDeviceSnapshot(rootRow);
                if (snapshot != null)
                {
                    collection.Devices.Add(snapshot);
                }
            }

            return collection.HasTargets() ? collection : null;
        }

        private CollectionDeviceSnapshot? CreateDeviceSnapshot(DeviceGridRow rootRow)
        {
            var snapshot = new CollectionDeviceSnapshot
            {
                DeviceId = rootRow.Id,
                Name = rootRow.Name,
                Host = rootRow.Host,
                Port = rootRow.Port,
                LedCount = rootRow.LedCount,
                StripCount = rootRow.StripCount
            };

            if (rootRow.Enabled)
            {
                SceneConfig? laserScene = FindSceneById(rootRow.AssignedLaserSceneId);
                if (laserScene != null && SceneTypeRules.IsLaser(laserScene.Type))
                {
                    snapshot.LaserScene = laserScene.Clone();
                }

                SceneConfig? strobeScene = FindSceneById(rootRow.AssignedStrobeSceneId);
                if (strobeScene != null && SceneTypeRules.IsStrobe(strobeScene.Type))
                {
                    snapshot.StrobeScene = strobeScene.Clone();
                }
            }

            int offset = 0;
            foreach (DeviceGridRow stripRow in GetStripRows(rootRow.Id))
            {
                if (stripRow.Enabled)
                {
                    SceneConfig? scene = FindSceneById(stripRow.AssignedSceneId);
                    if (scene != null && SceneTypeRules.SupportsStripAssignment(scene.Type) && stripRow.LedCount > 0)
                    {
                        snapshot.Strips.Add(new CollectionStripSnapshot
                        {
                            StripIndex = stripRow.StripIndex,
                            StartIndex = offset,
                            LedCount = stripRow.LedCount,
                            Scene = scene.Clone()
                        });
                    }
                }

                offset += Math.Max(0, stripRow.LedCount);
            }

            return HasSnapshotTargets(snapshot) ? snapshot : null;
        }

        private async void dgvCollections_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.Button != MouseButtons.Left || (ModifierKeys & Keys.Control) != Keys.Control)
            {
                return;
            }

            if (dgvCollections.Rows[e.RowIndex].DataBoundItem is not CollectionGridRow row)
            {
                return;
            }

            ConfigurationCollection? collection = FindCollectionById(row.Id);
            if (collection != null)
            {
                await StartCollectionOverrideAsync(collection, CollectionActivationMode.Toggle, holdShortcut: null);
            }
        }

        private void dgvCollections_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvCollections.IsCurrentCellDirty && IsImmediateCommitCell(dgvCollections))
            {
                dgvCollections.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private async void dgvCollections_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (!ShouldHandleValueChangedImmediately(dgvCollections, e))
            {
                return;
            }

            await HandleCollectionCellChangedAsync(e.RowIndex);
        }

        private async void dgvCollections_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (IsImmediateCommitColumn(dgvCollections, e.ColumnIndex))
            {
                return;
            }

            await HandleCollectionCellChangedAsync(e.RowIndex);
        }

        private async Task HandleCollectionCellChangedAsync(int rowIndex)
        {
            if (isLoading || rowIndex < 0)
            {
                return;
            }

            if (dgvCollections.Rows[rowIndex].DataBoundItem is not CollectionGridRow row)
            {
                return;
            }

            ConfigurationCollection? collection = FindCollectionById(row.Id);
            if (collection == null)
            {
                return;
            }

            bool includedChanged = collection.IncludedInAutoSelection != row.IncludedInAutoSelection;
            collection.Name = string.IsNullOrWhiteSpace(row.Name) ? collection.Name : row.Name.Trim();
            collection.ActivationMode = row.ActivationMode;
            collection.IncludedInAutoSelection = row.IncludedInAutoSelection;
            RefreshCollectionRow(collection);
            if (includedChanged)
            {
                await HandleCollectionAutoSelectionEligibilityChangedAsync();
            }
        }

        private void dgvCollections_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            string propertyName = dgvCollections.Columns[e.ColumnIndex].DataPropertyName;
            if (propertyName == nameof(CollectionGridRow.Name) && string.IsNullOrWhiteSpace(e.FormattedValue?.ToString()))
            {
                e.Cancel = true;
                dgvCollections.Rows[e.RowIndex].ErrorText = "Collection name is required.";
                return;
            }

            dgvCollections.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        private void dgvCollections_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void GlobalShortcutManager_ShortcutKeyDown(object? sender, GlobalShortcutEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }

            SafeUi(() => _ = HandleGlobalShortcutDownAsync(e.Shortcut));
        }

        private void GlobalShortcutManager_ShortcutKeyUp(object? sender, GlobalShortcutEventArgs e)
        {
            SafeUi(() => _ = HandleGlobalShortcutUpAsync());
        }

        private async Task HandleGlobalShortcutDownAsync(KeyboardShortcutConfig shortcut)
        {
            if (shortcut.IsEmpty || !shortcut.IsUsable)
            {
                return;
            }

            if (appConfig.ResetShortcut != null && !appConfig.ResetShortcut.IsEmpty && shortcut.Matches(appConfig.ResetShortcut))
            {
                await StopCollectionAutoSelectionAsync(resumeDefault: true);
                return;
            }

            ConfigurationCollection? collection = appConfig.Collections
                .FirstOrDefault(item => item.Shortcut != null && !item.Shortcut.IsEmpty && shortcut.Matches(item.Shortcut));
            if (collection == null)
            {
                return;
            }

            if (string.Equals(activeCollectionId, collection.Id, StringComparison.Ordinal)
                && activeCollectionMode == collection.ActivationMode)
            {
                return;
            }

            KeyboardShortcutConfig? holdShortcut = collection.ActivationMode == CollectionActivationMode.Hold
                ? shortcut
                : null;
            await StartCollectionOverrideAsync(collection, collection.ActivationMode, holdShortcut);
        }

        private async Task HandleGlobalShortcutUpAsync()
        {
            if (!collectionOverrideActive || activeCollectionMode != CollectionActivationMode.Hold || activeHoldShortcut == null)
            {
                return;
            }

            if (globalShortcutManager == null || !globalShortcutManager.IsShortcutPressed(activeHoldShortcut))
            {
                await StopCollectionOverrideAsync(resumeDefault: true);
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
            if (row.Kind == DeviceRowKind.Device
                && propertyName == nameof(DeviceGridRow.AssignedSceneId))
            {
                e.Cancel = true;
                return;
            }

            if (row.Kind == DeviceRowKind.Strip
                && propertyName is nameof(DeviceGridRow.Name)
                    or nameof(DeviceGridRow.Host)
                    or nameof(DeviceGridRow.Port)
                    or nameof(DeviceGridRow.AssignedLaserSceneId)
                    or nameof(DeviceGridRow.AssignedStrobeSceneId))
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
            else if (row.Kind == DeviceRowKind.Device
                && propertyName == nameof(DeviceGridRow.AssignedSceneId))
            {
                e.Value = string.Empty;
                e.FormattingApplied = true;
            }
            else if (row.Kind == DeviceRowKind.Strip
                && propertyName is nameof(DeviceGridRow.Host)
                    or nameof(DeviceGridRow.Port)
                    or nameof(DeviceGridRow.StripCount)
                    or nameof(DeviceGridRow.AssignedLaserSceneId)
                    or nameof(DeviceGridRow.AssignedStrobeSceneId))
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

            RefreshSceneAssignmentOptions();
            if (typeChanged)
            {
                NormalizeSceneAssignments();
            }

            ledSceneLookupBindingSource.ResetBindings(false);
            laserSceneLookupBindingSource.ResetBindings(false);
            strobeSceneLookupBindingSource.ResetBindings(false);
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

        private void NormalizeSceneAssignments()
        {
            string fallbackLedSceneId = appConfig.Scenes
                .FirstOrDefault(scene => SceneTypeRules.SupportsStripAssignment(scene.Type))?.Id
                ?? string.Empty;

            foreach (DeviceGridRow row in deviceRows)
            {
                if (row.Kind == DeviceRowKind.Strip && !IsLedSceneAssigned(row.AssignedSceneId))
                {
                    row.AssignedSceneId = fallbackLedSceneId;
                }

                if (row.Kind == DeviceRowKind.Device)
                {
                    if (!IsLaserSceneAssigned(row.AssignedLaserSceneId))
                    {
                        row.AssignedLaserSceneId = string.Empty;
                    }

                    if (!IsStrobeSceneAssigned(row.AssignedStrobeSceneId))
                    {
                        row.AssignedStrobeSceneId = string.Empty;
                    }
                }
                else
                {
                    row.AssignedLaserSceneId = string.Empty;
                    row.AssignedStrobeSceneId = string.Empty;
                }
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
            string defaultSceneId = appConfig.Scenes.FirstOrDefault(scene => SceneTypeRules.SupportsStripAssignment(scene.Type))?.Id
                ?? string.Empty;
            DeviceConfig device = new()
            {
                Id = Guid.NewGuid().ToString("N"),
                Enabled = false,
                Name = "Device",
                Host = "127.0.0.1",
                Port = 81,
                LedCount = 0,
                StripCount = 0,
                AssignedSceneId = defaultSceneId,
                AssignedLaserSceneId = string.Empty,
                AssignedStrobeSceneId = string.Empty
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
            SceneConfig scene = SceneConfig.CreateDefault(ConsumePendingNewSceneType(SceneType.SolidColor), sceneRows.Count + 1);
            appConfig.Scenes.Add(scene);
            SceneGridRow row = SceneGridRow.FromSceneConfig(scene);
            sceneRows.Add(row);
            RefreshSceneAssignmentOptions();
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
            RefreshSceneAssignmentOptions();
            sceneGridBindingSource.ResetBindings(false);
        }

        private void btnRemoveScene_Click(object? sender, EventArgs e)
        {
            SceneConfig? selectedScene = GetSelectedSceneConfig();
            if (selectedScene == null)
            {
                return;
            }

            bool isAssigned = deviceRows.Any(device =>
                (device.Kind == DeviceRowKind.Strip && string.Equals(device.AssignedSceneId, selectedScene.Id, StringComparison.Ordinal))
                || (device.Kind == DeviceRowKind.Device
                    && (string.Equals(device.AssignedLaserSceneId, selectedScene.Id, StringComparison.Ordinal)
                        || string.Equals(device.AssignedStrobeSceneId, selectedScene.Id, StringComparison.Ordinal))));
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

            RefreshSceneAssignmentOptions();
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
                int rowIndex = sceneRows.IndexOf(row);
                if (rowIndex >= 0)
                {
                    sceneGridBindingSource.ResetItem(rowIndex);
                }
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

        private void SpectralSegmentsEditor_SelectedAudioDeviceChanged(object? sender, EventArgs e)
        {
            if (syncingAudioDeviceSelection || sender is not SpectralAnalysisSegmentsSceneEditorForm spectralSegmentsEditor)
            {
                return;
            }

            selectedAudioDeviceId = spectralSegmentsEditor.GetSelectedAudioDeviceId();
            SyncAudioDeviceEditors(excludeSpectralSegments: true);
        }

        private void LedStrobeEditor_SelectedAudioDeviceChanged(object? sender, EventArgs e)
        {
            if (syncingAudioDeviceSelection || sender is not LedStrobeSceneEditorForm ledStrobeEditor)
            {
                return;
            }

            selectedAudioDeviceId = ledStrobeEditor.GetSelectedAudioDeviceId();
            SyncAudioDeviceEditors(excludeLedStrobe: true);
        }

        private void SyncAudioDeviceEditors(
            bool excludeVolume = false,
            bool excludeSpectral = false,
            bool excludeSpectralSegments = false,
            bool excludeLedStrobe = false)
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

                if (!excludeSpectralSegments && sceneEditors[SceneType.SpectralAnalysisSegments] is SpectralAnalysisSegmentsSceneEditorForm spectralSegmentsEditor)
                {
                    spectralSegmentsEditor.SelectAudioDevice(selectedAudioDeviceId);
                }

                if (!excludeLedStrobe && sceneEditors[SceneType.LedStrobe] is LedStrobeSceneEditorForm ledStrobeEditor)
                {
                    ledStrobeEditor.SelectAudioDevice(selectedAudioDeviceId);
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
                    requiresRestart = row.LedCount > 0
                        && (row.LedCount != metadata.TotalLedCount || row.StripCount != metadata.StripCount);
                    row.Name = metadata.Name;
                    row.LedCount = metadata.TotalLedCount;
                    row.StripCount = metadata.StripCount;

                    Dictionary<int, DeviceGridRow> existingStripRows = GetStripRows(row.Id)
                        .ToDictionary(stripRow => stripRow.StripIndex, stripRow => stripRow);

                    if (metadata.StripCount >= 1)
                    {
                        int insertIndex = deviceRows.IndexOf(row) + 1 + GetStripRows(row.Id).Count;
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

                                deviceRows.Insert(insertIndex, stripRow);
                                insertIndex++;
                            }

                            stripRow.Host = row.Host;
                            stripRow.Port = row.Port;
                            stripRow.LedCount = stripMetadata.LedCount;
                            bool stripActive = collectionOverrideActive
                                ? IsCollectionStripActive(row.Id, stripRow.StripIndex)
                                : stripRow.Enabled;
                            stripRow.Status = stripActive ? ResolvePostMetadataStatus(row.Id) : "Disconnected";
                        }
                    }

                    foreach (DeviceGridRow obsoleteStrip in existingStripRows.Values.Where(stripRow => metadata.Strips.All(strip => strip.Index != stripRow.StripIndex)).ToList())
                    {
                        deviceRows.Remove(obsoleteStrip);
                    }

                    DeviceConfig? config = appConfig.Devices.FirstOrDefault(device => device.Id == row.Id);
                    if (config != null)
                    {
                        config.Name = metadata.Name;
                        config.LedCount = metadata.TotalLedCount;
                        config.StripCount = metadata.StripCount;
                        Dictionary<int, DeviceStripConfig> existingConfigStrips = config.Strips
                            .GroupBy(strip => strip.StripIndex)
                            .ToDictionary(group => group.Key, group => group.First());
                        var refreshedStrips = new List<DeviceStripConfig>();

                        foreach (DeviceStripConfig existingStrip in config.Strips)
                        {
                            DeviceStripMetadata? metadataStrip = metadata.Strips.FirstOrDefault(strip => strip.Index == existingStrip.StripIndex);
                            if (metadataStrip == null)
                            {
                                continue;
                            }

                            refreshedStrips.Add(new DeviceStripConfig
                            {
                                StripIndex = metadataStrip.Index,
                                LedCount = metadataStrip.LedCount,
                                Enabled = existingStrip.Enabled,
                                AssignedSceneId = existingStrip.AssignedSceneId
                            });
                        }

                        foreach (DeviceStripMetadata metadataStrip in metadata.Strips)
                        {
                            if (refreshedStrips.Any(strip => strip.StripIndex == metadataStrip.Index))
                            {
                                continue;
                            }

                            refreshedStrips.Add(new DeviceStripConfig
                            {
                                StripIndex = metadataStrip.Index,
                                LedCount = metadataStrip.LedCount,
                                Enabled = existingConfigStrips.TryGetValue(metadataStrip.Index, out DeviceStripConfig? existingStrip) && existingStrip.Enabled,
                                AssignedSceneId = existingConfigStrips.TryGetValue(metadataStrip.Index, out existingStrip)
                                    ? existingStrip.AssignedSceneId
                                    : config.AssignedSceneId
                            });
                        }

                        config.Strips = refreshedStrips;
                    }

                    if (!HasActiveRuntimeTarget(row.Id))
                    {
                        row.Status = "Disconnected";
                    }
                    else
                    {
                        row.Status = ResolvePostMetadataStatus(row.Id);
                        foreach (DeviceGridRow stripRow in GetStripRows(row.Id))
                        {
                            bool stripActive = collectionOverrideActive
                                ? IsCollectionStripActive(row.Id, stripRow.StripIndex)
                                : stripRow.Enabled;
                            stripRow.Status = stripActive ? row.Status : "Disconnected";
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
                    if (HasActiveRuntimeTarget(row.Id) && row.LedCount <= 0)
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
                collectionAutoSelectionTimer.Stop();
                await StopCollectionOverrideAsync(resumeDefault: false);
                await StopAllDeviceRunsAsync();
                SyncConfigFromUi();
                appConfig.Save();
                collectionAutoSelectionTimer.Dispose();
                globalShortcutManager?.Dispose();
                globalShortcutManager = null;
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
