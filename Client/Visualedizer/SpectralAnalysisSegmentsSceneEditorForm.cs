using System.ComponentModel;
using static Ledqualizer.AcVolume;

namespace Ledqualizer
{
    public sealed partial class SpectralAnalysisSegmentsSceneEditorForm : Form, ISceneEditorForm
    {
        private readonly BindingList<SpectralSegmentRowViewModel> rows = new();
        private bool isLoading;
        private bool isCommitting;

        public SpectralAnalysisSegmentsSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            InitializeGrid();
        }

        public SceneType SceneType => SceneType.SpectralAnalysisSegments;

        public SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public event EventHandler? SelectedAudioDeviceChanged;

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
                scene.SpectralAnalysisSegments.EnsureSegmentDefaults();
                rows.Clear();
                for (int i = 0; i < scene.SpectralAnalysisSegments.Segments.Count; i++)
                {
                    rows.Add(SpectralSegmentRowViewModel.FromModel(scene.SpectralAnalysisSegments.Segments[i], i));
                }
            }
            finally
            {
                isLoading = false;
            }
        }

        private void InitializeGrid()
        {
            dgvSegments.AutoGenerateColumns = false;
            dgvSegments.AllowUserToAddRows = false;
            dgvSegments.AllowUserToDeleteRows = false;
            dgvSegments.DataSource = rows;
            dgvSegments.RowHeadersVisible = false;
            dgvSegments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSegments.MultiSelect = true;

            dgvSegments.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.Enabled),
                HeaderText = "On",
                Width = 42
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.Name),
                HeaderText = "Name",
                Width = 130
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.RatioDenominator),
                HeaderText = "Ratio 1/n",
                Width = 72
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.SegmentIndex),
                HeaderText = "Index",
                Width = 58
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.FrequencyLowHz),
                HeaderText = "Low Hz",
                Width = 78
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.FrequencyHighHz),
                HeaderText = "High Hz",
                Width = 78
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.ThresholdDb),
                HeaderText = "Threshold dB",
                Width = 92
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.LightUpMs),
                HeaderText = "Hold ms",
                Width = 72
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.FadeOutMs),
                HeaderText = "Fade ms",
                Width = 72
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.Brightness),
                HeaderText = "Bright %",
                Width = 72
            });
            dgvSegments.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.BrightnessMode),
                HeaderText = "Bright mode",
                DataSource = Enum.GetValues<SpectralSegmentBrightnessMode>(),
                Width = 112
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.BrightnessLow),
                HeaderText = "Bright low %",
                Width = 90
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.LevelLowDb),
                HeaderText = "Level low dB",
                Width = 92
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.LevelHighDb),
                HeaderText = "Level high dB",
                Width = 96
            });
            dgvSegments.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.SizeMode),
                HeaderText = "Size mode",
                DataSource = Enum.GetValues<SpectralSegmentSizeMode>(),
                Width = 105
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.CenterPointWidthPercent),
                HeaderText = "Point width %",
                Width = 92
            });
            dgvSegments.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.HueMode),
                HeaderText = "Hue mode",
                DataSource = Enum.GetValues<SpectralSegmentHueMode>(),
                Width = 118
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.HueStart),
                HeaderText = "Hue start",
                Width = 75
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.HueEnd),
                HeaderText = "Hue end",
                Width = 75
            });
            dgvSegments.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.SaturationMode),
                HeaderText = "Sat mode",
                DataSource = Enum.GetValues<SpectralSegmentSaturationMode>(),
                Width = 105
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.Saturation),
                HeaderText = "Sat %",
                Width = 62
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.SaturationMin),
                HeaderText = "Sat min",
                Width = 68
            });
            dgvSegments.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SpectralSegmentRowViewModel.SaturationMax),
                HeaderText = "Sat max",
                Width = 68
            });

            dgvSegments.CurrentCellDirtyStateChanged += dgvSegments_CurrentCellDirtyStateChanged;
            dgvSegments.CellValueChanged += dgvSegments_CellValueChanged;
            dgvSegments.CellEndEdit += dgvSegments_CellEndEdit;
            dgvSegments.DataError += dgvSegments_DataError;
        }

        private void cmbAudioDevices_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SelectedAudioDeviceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void btnAddRow_Click(object? sender, EventArgs e)
        {
            int bandIndex = rows.Count(row => row.RatioDenominator == 5) % 5;
            rows.Add(SpectralSegmentRowViewModel.FromModel(
                SpectralAnalysisSegmentsSceneConfig.CreateDefaultBandSegment(bandIndex),
                rows.Count));
            CommitRowsToScene();
            SelectRow(rows.Count - 1);
        }

        private void btnRemoveRow_Click(object? sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvSegments.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(row => row.Index))
            {
                if (row.DataBoundItem is SpectralSegmentRowViewModel item)
                {
                    rows.Remove(item);
                }
            }

            CommitRowsToScene();
        }

        private void btnMoveUp_Click(object? sender, EventArgs e)
        {
            MoveSelectedRow(-1);
        }

        private void btnMoveDown_Click(object? sender, EventArgs e)
        {
            MoveSelectedRow(1);
        }

        private void btnResetDefaults_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Reset spectral segment rows to defaults?", "Reset Defaults", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            rows.Clear();
            foreach (SpectralAnalysisSegmentConfig segment in SpectralAnalysisSegmentsSceneConfig.CreateDefaultSegments())
            {
                rows.Add(SpectralSegmentRowViewModel.FromModel(segment, rows.Count));
            }

            CommitRowsToScene();
        }

        private void dgvSegments_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvSegments.IsCurrentCellDirty)
            {
                dgvSegments.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvSegments_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            CommitRowsToScene();
        }

        private void dgvSegments_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            CommitRowsToScene();
        }

        private void dgvSegments_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void MoveSelectedRow(int delta)
        {
            if (dgvSegments.CurrentRow?.DataBoundItem is not SpectralSegmentRowViewModel item)
            {
                return;
            }

            int oldIndex = rows.IndexOf(item);
            int newIndex = Math.Clamp(oldIndex + delta, 0, rows.Count - 1);
            if (oldIndex < 0 || oldIndex == newIndex)
            {
                return;
            }

            rows.RemoveAt(oldIndex);
            rows.Insert(newIndex, item);
            CommitRowsToScene();
            SelectRow(newIndex);
        }

        private void SelectRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvSegments.Rows.Count)
            {
                return;
            }

            dgvSegments.ClearSelection();
            dgvSegments.Rows[rowIndex].Selected = true;
            if (dgvSegments.Columns.Count > 0)
            {
                dgvSegments.CurrentCell = dgvSegments.Rows[rowIndex].Cells[0];
            }
        }

        private void CommitRowsToScene()
        {
            if (isLoading || isCommitting || CurrentScene == null)
            {
                return;
            }

            isCommitting = true;
            try
            {
                List<SpectralAnalysisSegmentConfig> models = rows
                    .Select(row => row.ToModel())
                    .Select((segment, index) => SpectralAnalysisSegmentsSceneConfig.NormalizeSegment(segment, index))
                    .ToList();

                CurrentScene.SpectralAnalysisSegments.Segments = models;
                for (int i = 0; i < rows.Count && i < models.Count; i++)
                {
                    rows[i].LoadFromModel(models[i], i);
                }

                dgvSegments.Refresh();
            }
            finally
            {
                isCommitting = false;
            }

            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private sealed class SpectralSegmentRowViewModel
        {
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public bool Enabled { get; set; } = true;
            public string Name { get; set; } = "Segment";
            public int RatioDenominator { get; set; } = 5;
            public int SegmentIndex { get; set; }
            public double FrequencyLowHz { get; set; } = 20;
            public double FrequencyHighHz { get; set; } = 20000;
            public double ThresholdDb { get; set; } = -35;
            public int LightUpMs { get; set; } = 70;
            public int FadeOutMs { get; set; } = 1000;
            public SpectralSegmentBrightnessMode BrightnessMode { get; set; } = SpectralSegmentBrightnessMode.Fixed;
            public int BrightnessLow { get; set; } = 20;
            public int Brightness { get; set; } = 100;
            public double LevelLowDb { get; set; } = -35;
            public double LevelHighDb { get; set; } = -15;
            public SpectralSegmentSizeMode SizeMode { get; set; } = SpectralSegmentSizeMode.Full;
            public int CenterPointWidthPercent { get; set; } = 10;
            public SpectralSegmentHueMode HueMode { get; set; } = SpectralSegmentHueMode.FixedRange;
            public double HueStart { get; set; }
            public double HueEnd { get; set; } = 360;
            public SpectralSegmentSaturationMode SaturationMode { get; set; } = SpectralSegmentSaturationMode.Fixed;
            public int Saturation { get; set; } = 100;
            public int SaturationMin { get; set; } = 100;
            public int SaturationMax { get; set; } = 100;

            public SpectralAnalysisSegmentConfig ToModel()
            {
                return new SpectralAnalysisSegmentConfig
                {
                    Id = Id,
                    Enabled = Enabled,
                    Name = string.IsNullOrWhiteSpace(Name) ? "Segment" : Name.Trim(),
                    RatioDenominator = RatioDenominator,
                    SegmentIndex = SegmentIndex,
                    FrequencyLowHz = FrequencyLowHz,
                    FrequencyHighHz = FrequencyHighHz,
                    ThresholdDb = ThresholdDb,
                    LightUpMs = LightUpMs,
                    FadeOutMs = FadeOutMs,
                    BrightnessMode = BrightnessMode,
                    BrightnessLow = BrightnessLow,
                    Brightness = Brightness,
                    LevelLowDb = LevelLowDb,
                    LevelHighDb = LevelHighDb,
                    SizeMode = SizeMode,
                    CenterPointWidthPercent = CenterPointWidthPercent,
                    HueMode = HueMode,
                    HueStart = HueStart,
                    HueEnd = HueEnd,
                    SaturationMode = SaturationMode,
                    Saturation = Saturation,
                    SaturationMin = SaturationMin,
                    SaturationMax = SaturationMax
                };
            }

            public void LoadFromModel(SpectralAnalysisSegmentConfig model, int fallbackIndex)
            {
                SpectralAnalysisSegmentConfig normalized = SpectralAnalysisSegmentsSceneConfig.NormalizeSegment(model, fallbackIndex);
                Id = normalized.Id;
                Enabled = normalized.Enabled;
                Name = normalized.Name;
                RatioDenominator = normalized.RatioDenominator;
                SegmentIndex = normalized.SegmentIndex;
                FrequencyLowHz = normalized.FrequencyLowHz;
                FrequencyHighHz = normalized.FrequencyHighHz;
                ThresholdDb = normalized.ThresholdDb;
                LightUpMs = normalized.LightUpMs;
                FadeOutMs = normalized.FadeOutMs;
                BrightnessMode = normalized.BrightnessMode;
                BrightnessLow = normalized.BrightnessLow;
                Brightness = normalized.Brightness;
                LevelLowDb = normalized.LevelLowDb;
                LevelHighDb = normalized.LevelHighDb;
                SizeMode = normalized.SizeMode;
                CenterPointWidthPercent = normalized.CenterPointWidthPercent;
                HueMode = normalized.HueMode;
                HueStart = normalized.HueStart;
                HueEnd = normalized.HueEnd;
                SaturationMode = normalized.SaturationMode;
                Saturation = normalized.Saturation;
                SaturationMin = normalized.SaturationMin;
                SaturationMax = normalized.SaturationMax;
            }

            public static SpectralSegmentRowViewModel FromModel(SpectralAnalysisSegmentConfig model, int fallbackIndex)
            {
                var row = new SpectralSegmentRowViewModel();
                row.LoadFromModel(model, fallbackIndex);
                return row;
            }
        }
    }
}
