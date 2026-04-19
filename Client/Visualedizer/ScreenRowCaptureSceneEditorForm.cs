namespace Ledqualizer
{
    public partial class ScreenRowCaptureSceneEditorForm : Form, ISceneEditorForm
    {
        private sealed class MonitorItem
        {
            public int Index { get; init; }
            public string Display { get; init; } = string.Empty;
            public override string ToString() => Display;
        }

        private bool isLoading;
        private readonly List<MonitorItem> monitors = new();

        public SceneType SceneType => SceneType.ScreenRowCapture;

        protected SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public event EventHandler? GuideChanged;
        public event EventHandler? CaptureRowChanged;

        private readonly List<Color> previewColors = new();

        public ScreenRowCaptureSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            LoadMonitors();
            pictureBoxPreview.Paint += PictureBoxPreview_Paint;
        }

        public bool ShowGuide => chbShowGuide.Checked;

        public int CaptureRow => (int)numScreenRow.Value;

        public int MonitorIndex => (cbMonitors.SelectedItem as MonitorItem)?.Index ?? 0;

        public void UpdatePreview(IReadOnlyList<Color> colors)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IReadOnlyList<Color>>(UpdatePreview), colors);
                return;
            }

            previewColors.Clear();
            previewColors.AddRange(colors);
            pictureBoxPreview.Invalidate();
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                LoadMonitors();
                SelectMonitor(scene.ScreenRowCapture.MonitorIndex);
                int captureY = Math.Max((int)numScreenRow.Minimum, Math.Min((int)numScreenRow.Maximum, scene.ScreenRowCapture.CaptureY));
                numScreenRow.Value = captureY;
                hsbScreenRowSelector.Value = Math.Max(hsbScreenRowSelector.Minimum, Math.Min(hsbScreenRowSelector.Maximum, captureY));
                chbReverse.Checked = scene.ScreenRowCapture.Reverse;
            }
            finally
            {
                isLoading = false;
            }
        }

        private void LoadMonitors()
        {
            Screen[] screens = Screen.AllScreens;
            monitors.Clear();
            for (int i = 0; i < screens.Length; i++)
            {
                Rectangle bounds = screens[i].Bounds;
                monitors.Add(new MonitorItem
                {
                    Index = i,
                    Display = $"Display {i + 1} ({bounds.Width}x{bounds.Height})"
                });
            }

            cbMonitors.DataSource = null;
            cbMonitors.DataSource = monitors;
            cbMonitors.DisplayMember = nameof(MonitorItem.Display);
            cbMonitors.ValueMember = nameof(MonitorItem.Index);

            if (monitors.Count == 0)
            {
                UpdateCaptureRange(null);
            }
            else if (cbMonitors.SelectedItem is MonitorItem selected)
            {
                UpdateCaptureRange(GetScreen(selected.Index));
            }
        }

        private void SelectMonitor(int monitorIndex)
        {
            for (int i = 0; i < monitors.Count; i++)
            {
                if (monitors[i].Index == monitorIndex)
                {
                    cbMonitors.SelectedIndex = i;
                    return;
                }
            }

            if (monitors.Count > 0)
            {
                cbMonitors.SelectedIndex = 0;
            }
        }

        private static Screen? GetScreen(int monitorIndex)
        {
            Screen[] screens = Screen.AllScreens;
            if (monitorIndex >= 0 && monitorIndex < screens.Length)
            {
                return screens[monitorIndex];
            }

            return Screen.PrimaryScreen ?? screens.FirstOrDefault();
        }

        private void UpdateCaptureRange(Screen? screen)
        {
            int maxRow = Math.Max(screen?.Bounds.Height ?? 1, 1);
            int largeChange = Math.Max(Math.Min(maxRow / 10, 50), 1);

            numScreenRow.Minimum = 0;
            numScreenRow.Maximum = maxRow;
            hsbScreenRowSelector.Minimum = 0;
            hsbScreenRowSelector.Maximum = maxRow + largeChange - 1;
            hsbScreenRowSelector.LargeChange = largeChange;
            hsbScreenRowSelector.SmallChange = 1;
        }

        private void cbMonitors_SelectedIndexChanged(object? sender, EventArgs e)
        {
            MonitorItem? selected = cbMonitors.SelectedItem as MonitorItem;
            UpdateCaptureRange(selected == null ? null : GetScreen(selected.Index));
            numScreenRow.Value = Math.Max(numScreenRow.Minimum, Math.Min(numScreenRow.Maximum, numScreenRow.Value));
            hsbScreenRowSelector.Value = Math.Max(hsbScreenRowSelector.Minimum, Math.Min(hsbScreenRowSelector.Maximum, (int)numScreenRow.Value));

            if (CurrentScene != null && !isLoading && selected != null)
            {
                CurrentScene.ScreenRowCapture.MonitorIndex = selected.Index;
                SceneChanged?.Invoke(this, EventArgs.Empty);
            }

            CaptureRowChanged?.Invoke(this, EventArgs.Empty);
            GuideChanged?.Invoke(this, EventArgs.Empty);
        }

        private void hsbScreenRowSelector_Scroll(object? sender, ScrollEventArgs e)
        {
            numScreenRow.Value = Math.Max(numScreenRow.Minimum, Math.Min(numScreenRow.Maximum, hsbScreenRowSelector.Value));
        }

        private void numScreenRow_ValueChanged(object? sender, EventArgs e)
        {
            hsbScreenRowSelector.Value = Math.Max(hsbScreenRowSelector.Minimum, Math.Min(hsbScreenRowSelector.Maximum, (int)numScreenRow.Value));
            if (CurrentScene != null && !isLoading)
            {
                CurrentScene.ScreenRowCapture.CaptureY = (int)numScreenRow.Value;
                SceneChanged?.Invoke(this, EventArgs.Empty);
            }

            CaptureRowChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (CurrentScene != null && !isLoading)
            {
                CurrentScene.ScreenRowCapture.Reverse = chbReverse.Checked;
                SceneChanged?.Invoke(this, EventArgs.Empty);
            }

            GuideChanged?.Invoke(this, EventArgs.Empty);
        }

        private void PictureBoxPreview_Paint(object? sender, PaintEventArgs e)
        {
            if (previewColors.Count == 0 || sender is not PictureBox previewBox)
            {
                return;
            }

            int segmentWidth = Math.Max(previewBox.Width / previewColors.Count, 1);
            for (int i = 0; i < previewColors.Count; i++)
            {
                using Brush brush = new SolidBrush(previewColors[i]);
                int x = i * segmentWidth;
                int width = i == previewColors.Count - 1 ? previewBox.Width - (i * segmentWidth) : segmentWidth;
                e.Graphics.FillRectangle(brush, x, 0, width, previewBox.Height);
            }
        }
    }
}
