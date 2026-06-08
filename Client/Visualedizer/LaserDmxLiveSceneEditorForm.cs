using System.ComponentModel;

namespace Ledqualizer
{
    public sealed class LaserDmxSendRequestedEventArgs : EventArgs
    {
        public LaserDmxSendRequestedEventArgs(string sceneId)
        {
            SceneId = sceneId;
        }

        public string SceneId { get; }
    }

    public sealed class LaserDmxLiveSceneEditorForm : Form, ISceneEditorForm
    {
        private readonly BindingList<LaserChannelRowViewModel> rows = new();
        private readonly DataGridView dgvChannels = new();
        private readonly Button btnAddRow = new();
        private readonly Button btnRemoveRow = new();
        private readonly Button btnSend = new();
        private bool isLoading;

        public LaserDmxLiveSceneEditorForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            TableLayoutPanel root = new()
            {
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                RowCount = 3
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label lblInfo = new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Text = "Configure explicit DMX channel rows. Values are sent when the scene activates, when refresh intervals elapse, and when you click Send."
            };
            root.Controls.Add(lblInfo, 0, 0);

            InitializeGrid();
            root.Controls.Add(dgvChannels, 0, 1);

            FlowLayoutPanel actions = new()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight
            };

            btnAddRow.AutoSize = true;
            btnAddRow.Text = "Add Row";
            btnAddRow.Click += btnAddRow_Click;
            actions.Controls.Add(btnAddRow);

            btnRemoveRow.AutoSize = true;
            btnRemoveRow.Text = "Remove Row";
            btnRemoveRow.Click += btnRemoveRow_Click;
            actions.Controls.Add(btnRemoveRow);

            btnSend.AutoSize = true;
            btnSend.Text = "Send";
            btnSend.Click += btnSend_Click;
            actions.Controls.Add(btnSend);

            root.Controls.Add(actions, 0, 2);
            Controls.Add(root);
        }

        public event EventHandler? SceneChanged;
        public event EventHandler<LaserDmxSendRequestedEventArgs>? SendRequested;

        public SceneType SceneType => SceneType.LaserDmxLive;

        public SceneConfig? CurrentScene { get; private set; }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                rows.Clear();
                foreach (LaserDmxChannelRow channel in scene.LaserDmxLive.Channels)
                {
                    rows.Add(LaserChannelRowViewModel.FromModel(channel));
                }
            }
            finally
            {
                isLoading = false;
            }
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
                Width = 140,
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
            CommitRowsToScene();
            if (!string.IsNullOrWhiteSpace(CurrentScene?.Id))
            {
                SendRequested?.Invoke(this, new LaserDmxSendRequestedEventArgs(CurrentScene.Id));
            }
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

        private void CommitRowsToScene()
        {
            if (isLoading || CurrentScene == null)
            {
                return;
            }

            CurrentScene.LaserDmxLive.Channels = rows.Select(row => row.ToModel()).ToList();
            SceneChanged?.Invoke(this, EventArgs.Empty);
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
}
