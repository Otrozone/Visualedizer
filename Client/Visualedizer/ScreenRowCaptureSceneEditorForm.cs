namespace Ledqualizer
{
    internal partial class ScreenRowCaptureSceneEditorForm : SceneEditorFormBase
    {
        public override SceneType SceneType => SceneType.ScreenRowCapture;

        public event EventHandler? GuideChanged;
        public event EventHandler? CaptureRowChanged;

        private readonly List<Color> previewColors = new();

        public ScreenRowCaptureSceneEditorForm()
        {
            InitializeComponent();
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

        protected override void OnLoadScene(SceneConfig scene)
        {
            int captureY = Math.Max((int)numScreenRow.Minimum, Math.Min((int)numScreenRow.Maximum, scene.ScreenRowCapture.CaptureY));
            numScreenRow.Value = captureY;
            hsbScreenRowSelector.Value = Math.Max(hsbScreenRowSelector.Minimum, Math.Min(hsbScreenRowSelector.Maximum, captureY));
            chbReverse.Checked = scene.ScreenRowCapture.Reverse;
        }

        private void hsbScreenRowSelector_Scroll(object? sender, ScrollEventArgs e)
        {
            numScreenRow.Value = Math.Max(numScreenRow.Minimum, Math.Min(numScreenRow.Maximum, hsbScreenRowSelector.Value));
        }

        private void numScreenRow_ValueChanged(object? sender, EventArgs e)
        {
            hsbScreenRowSelector.Value = Math.Max(hsbScreenRowSelector.Minimum, Math.Min(hsbScreenRowSelector.Maximum, (int)numScreenRow.Value));
            if (CurrentScene != null && !IsLoadingScene)
            {
                CurrentScene.ScreenRowCapture.CaptureY = (int)numScreenRow.Value;
                NotifySceneChanged();
            }

            CaptureRowChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (CurrentScene != null && !IsLoadingScene)
            {
                CurrentScene.ScreenRowCapture.Reverse = chbReverse.Checked;
                NotifySceneChanged();
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
