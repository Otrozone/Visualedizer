namespace Ledqualizer
{
    public partial class ScreenRowCaptureSceneEditorForm : Form, ISceneEditorForm
    {
        private bool isLoading;

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
            hsbScreenRowSelector.Maximum = Math.Max(Screen.PrimaryScreen?.Bounds.Height ?? 1, 1);
            numScreenRow.Maximum = hsbScreenRowSelector.Maximum;
            pictureBoxPreview.Paint += PictureBoxPreview_Paint;
        }

        public bool ShowGuide => chbShowGuide.Checked;

        public int CaptureRow => (int)numScreenRow.Value;

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
