namespace Ledqualizer
{
    public partial class ImageRowCaptureSceneEditorForm : Form, ISceneEditorForm
    {
        private sealed class DirectionItem
        {
            public required ImageScanDirection Value { get; init; }
            public required string Display { get; init; }
            public override string ToString() => Display;
        }

        private sealed class SourceModeItem
        {
            public required ImageSourceMode Value { get; init; }
            public required string Display { get; init; }
            public override string ToString() => Display;
        }

        private static readonly DirectionItem[] DirectionItems =
        {
            new() { Value = ImageScanDirection.TopToBottom, Display = "Top -> Bottom" },
            new() { Value = ImageScanDirection.BottomToTop, Display = "Bottom -> Top" },
            new() { Value = ImageScanDirection.LeftToRight, Display = "Left -> Right" },
            new() { Value = ImageScanDirection.RightToLeft, Display = "Right -> Left" },
            new() { Value = ImageScanDirection.Random, Display = "Random" }
        };

        private static readonly SourceModeItem[] SourceModeItems =
        {
            new() { Value = ImageSourceMode.SingleImage, Display = "Single Image" },
            new() { Value = ImageSourceMode.Folder, Display = "Folder" }
        };

        private bool isLoading;
        private readonly List<Color> previewColors = new();
        private Bitmap? previewImage;
        private string? previewImagePath;
        private int sampleIndex = -1;
        private Size sourceSize = Size.Empty;
        private ImageScanDirection previewDirection = ImageScanDirection.TopToBottom;
        private bool hasResolvedPreviewDirection;

        public ImageRowCaptureSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            cbSourceMode.DataSource = SourceModeItems;
            cbSourceMode.DisplayMember = nameof(SourceModeItem.Display);
            cbSourceMode.ValueMember = nameof(SourceModeItem.Value);

            cbDirection.DataSource = DirectionItems;
            cbDirection.DisplayMember = nameof(DirectionItem.Display);
            cbDirection.ValueMember = nameof(DirectionItem.Value);

            pictureBoxStripPreview.Paint += PictureBoxStripPreview_Paint;
            pictureBoxImagePreview.Paint += PictureBoxImagePreview_Paint;
            pictureBoxImagePreview.MouseClick += PictureBoxImagePreview_MouseClick;
            Disposed += ImageRowCaptureSceneEditorForm_Disposed;
        }

        public SceneType SceneType => SceneType.ImageRowCapture;

        protected SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                ImageRowCaptureSceneConfig config = scene.ImageRowCapture;
                SelectSourceMode(config.SourceMode);
                txtImagePath.Text = config.ImagePath;
                txtFolderPath.Text = config.FolderPath;
                chbRecursive.Checked = config.Recursive;
                chbLoop.Checked = config.Loop;
                SelectDirection(config.Direction);
                numSpeedMin.Value = ClampDecimal(config.SpeedMin, numSpeedMin);
                numSpeedMax.Value = ClampDecimal(Math.Max(config.SpeedMin, config.SpeedMax), numSpeedMax);
                if (numSpeedMax.Value < numSpeedMin.Value)
                {
                    numSpeedMax.Value = numSpeedMin.Value;
                }

                hasResolvedPreviewDirection = config.Direction != ImageScanDirection.Random;
                previewDirection = config.Direction == ImageScanDirection.Random
                    ? ImageScanDirection.TopToBottom
                    : config.Direction;
            }
            finally
            {
                isLoading = false;
            }

            UpdateControlStates();
            UpdatePauseUi();
            UpdatePreviewImageFromScene();
            UpdateCurrentFileLabel();
            pictureBoxImagePreview.Invalidate();
            pictureBoxStripPreview.Invalidate();
        }

        internal void UpdatePreview(CaptureScenePreview preview)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<CaptureScenePreview>(UpdatePreview), preview);
                return;
            }

            previewColors.Clear();
            previewColors.AddRange(preview.Colors);
            sampleIndex = preview.SampleIndex;
            sourceSize = preview.SourceSize;
            previewDirection = preview.Direction;
            hasResolvedPreviewDirection = true;

            if (!string.Equals(previewImagePath, preview.SourcePath, StringComparison.OrdinalIgnoreCase))
            {
                LoadPreviewImage(preview.SourcePath);
            }

            UpdateCurrentFileLabel(preview.SourcePath);
            UpdatePauseUi();
            pictureBoxImagePreview.Invalidate();
            pictureBoxStripPreview.Invalidate();
        }

        private static decimal ClampDecimal(double value, NumericUpDown control)
        {
            decimal decimalValue = (decimal)value;
            return Math.Max(control.Minimum, Math.Min(control.Maximum, decimalValue));
        }

        private void SelectSourceMode(ImageSourceMode mode)
        {
            for (int i = 0; i < SourceModeItems.Length; i++)
            {
                if (SourceModeItems[i].Value == mode)
                {
                    cbSourceMode.SelectedIndex = i;
                    return;
                }
            }

            cbSourceMode.SelectedIndex = 0;
        }

        private void SelectDirection(ImageScanDirection direction)
        {
            for (int i = 0; i < DirectionItems.Length; i++)
            {
                if (DirectionItems[i].Value == direction)
                {
                    cbDirection.SelectedIndex = i;
                    return;
                }
            }

            cbDirection.SelectedIndex = 0;
        }

        private void UpdateControlStates()
        {
            bool folderMode = SelectedSourceMode == ImageSourceMode.Folder;
            txtImagePath.Enabled = !folderMode;
            btnBrowseImage.Enabled = !folderMode;
            txtFolderPath.Enabled = folderMode;
            btnBrowseFolder.Enabled = folderMode;
            chbRecursive.Enabled = folderMode;
        }

        private void UpdatePauseUi()
        {
            bool isPaused = CurrentScene?.ImageRowCapture.IsPaused ?? false;
            btnPausePlay.Text = isPaused ? "Play" : "Pause";
            pictureBoxImagePreview.Cursor = CanSeekByClick() ? Cursors.Cross : Cursors.Default;
        }

        private void UpdateSceneFromControls()
        {
            if (CurrentScene == null || isLoading)
            {
                return;
            }

            ImageRowCaptureSceneConfig config = CurrentScene.ImageRowCapture;
            config.SourceMode = SelectedSourceMode;
            config.ImagePath = txtImagePath.Text.Trim();
            config.FolderPath = txtFolderPath.Text.Trim();
            config.Recursive = chbRecursive.Checked;
            config.Loop = chbLoop.Checked;
            config.Direction = SelectedDirection;
            config.SpeedMin = (double)numSpeedMin.Value;
            config.SpeedMax = Math.Max(config.SpeedMin, (double)numSpeedMax.Value);
            config.RequestedSampleIndex = -1;
            config.RequestedSeekRevision = 0;

            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private ImageSourceMode SelectedSourceMode =>
            (cbSourceMode.SelectedItem as SourceModeItem)?.Value ?? ImageSourceMode.SingleImage;

        private ImageScanDirection SelectedDirection =>
            (cbDirection.SelectedItem as DirectionItem)?.Value ?? ImageScanDirection.TopToBottom;

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (sender == numSpeedMin && numSpeedMax.Value < numSpeedMin.Value)
            {
                numSpeedMax.Value = numSpeedMin.Value;
            }

            if (sender == numSpeedMax && numSpeedMax.Value < numSpeedMin.Value)
            {
                numSpeedMax.Value = numSpeedMin.Value;
            }

            UpdateControlStates();
            UpdatePauseUi();
            UpdateSceneFromControls();
            UpdatePreviewImageFromScene();
            UpdateCurrentFileLabel();
        }

        private void btnBrowseImage_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new()
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*",
                FileName = txtImagePath.Text
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                txtImagePath.Text = dialog.FileName;
            }
        }

        private void btnBrowseFolder_Click(object? sender, EventArgs e)
        {
            using FolderBrowserDialog dialog = new()
            {
                SelectedPath = Directory.Exists(txtFolderPath.Text) ? txtFolderPath.Text : string.Empty,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                txtFolderPath.Text = dialog.SelectedPath;
            }
        }

        private void UpdatePreviewImageFromScene()
        {
            string? previewPath = ResolvePreviewImagePath();
            LoadPreviewImage(previewPath);
            if (string.IsNullOrWhiteSpace(previewPath))
            {
                sampleIndex = -1;
                sourceSize = previewImage?.Size ?? Size.Empty;
            }

            UpdatePauseUi();
            pictureBoxImagePreview.Invalidate();
        }

        private string? ResolvePreviewImagePath()
        {
            if (CurrentScene == null)
            {
                return null;
            }

            ImageRowCaptureSceneConfig config = CurrentScene.ImageRowCapture;
            if (config.SourceMode == ImageSourceMode.SingleImage)
            {
                return File.Exists(config.ImagePath) ? config.ImagePath : null;
            }

            if (string.IsNullOrWhiteSpace(config.FolderPath) || !Directory.Exists(config.FolderPath))
            {
                return null;
            }

            SearchOption searchOption = config.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (string file in Directory.EnumerateFiles(config.FolderPath, "*.*", searchOption))
            {
                if (IsSupportedImageFile(file))
                {
                    return file;
                }
            }

            return null;
        }

        private void LoadPreviewImage(string? path)
        {
            if (string.Equals(previewImagePath, path, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            previewImage?.Dispose();
            previewImage = null;
            previewImagePath = path;
            sourceSize = Size.Empty;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            try
            {
                using Image image = Image.FromFile(path);
                previewImage = new Bitmap(image);
                sourceSize = previewImage.Size;
            }
            catch
            {
                previewImage?.Dispose();
                previewImage = null;
                previewImagePath = null;
                sourceSize = Size.Empty;
            }
        }

        private void UpdateCurrentFileLabel(string? pathOverride = null)
        {
            string? path = pathOverride;
            if (string.IsNullOrWhiteSpace(path))
            {
                path = CurrentScene == null ? null : ResolvePreviewImagePath();
            }

            lblCurrentFileValue.Text = string.IsNullOrWhiteSpace(path) ? "No file selected" : Path.GetFileName(path);
        }

        private static bool IsSupportedImageFile(string path)
        {
            string extension = Path.GetExtension(path);
            return extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".gif", StringComparison.OrdinalIgnoreCase);
        }

        private void btnPausePlay_Click(object? sender, EventArgs e)
        {
            if (CurrentScene == null)
            {
                return;
            }

            CurrentScene.ImageRowCapture.IsPaused = !CurrentScene.ImageRowCapture.IsPaused;
            UpdatePauseUi();
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private void PictureBoxStripPreview_Paint(object? sender, PaintEventArgs e)
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
                int width = i == previewColors.Count - 1 ? previewBox.Width - x : segmentWidth;
                e.Graphics.FillRectangle(brush, x, 0, width, previewBox.Height);
            }
        }

        private void PictureBoxImagePreview_MouseClick(object? sender, MouseEventArgs e)
        {
            if (!CanSeekByClick() || CurrentScene == null || previewImage == null)
            {
                return;
            }

            Rectangle imageBounds = GetScaledImageBounds(pictureBoxImagePreview.ClientRectangle, previewImage.Size);
            if (!imageBounds.Contains(e.Location))
            {
                return;
            }

            ImageScanDirection direction = GetEffectiveSeekDirection();
            int? requestedSampleIndex = direction switch
            {
                ImageScanDirection.TopToBottom or ImageScanDirection.BottomToTop => MapPreviewClickToSampleIndex(
                    e.Y,
                    imageBounds.Top,
                    imageBounds.Height,
                    sourceSize.Height),
                ImageScanDirection.LeftToRight or ImageScanDirection.RightToLeft => MapPreviewClickToSampleIndex(
                    e.X,
                    imageBounds.Left,
                    imageBounds.Width,
                    sourceSize.Width),
                _ => null
            };

            if (!requestedSampleIndex.HasValue)
            {
                return;
            }

            ImageRowCaptureSceneConfig config = CurrentScene.ImageRowCapture;
            config.RequestedSampleIndex = requestedSampleIndex.Value;
            config.RequestedSeekRevision++;

            ApplyLocalSeekPreview(requestedSampleIndex.Value, direction);
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }

        private void PictureBoxImagePreview_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not PictureBox previewBox)
            {
                return;
            }

            e.Graphics.Clear(SystemColors.ControlDarkDark);
            if (previewImage == null || previewBox.Width <= 0 || previewBox.Height <= 0)
            {
                return;
            }

            Rectangle imageBounds = GetScaledImageBounds(previewBox.ClientRectangle, previewImage.Size);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImage(previewImage, imageBounds);

            if (sampleIndex < 0 || sourceSize.Width <= 0 || sourceSize.Height <= 0)
            {
                return;
            }

            using Pen overlayPen = new(Color.LimeGreen, 2);
            switch (previewDirection)
            {
                case ImageScanDirection.TopToBottom:
                case ImageScanDirection.BottomToTop:
                    if (sourceSize.Height > 0)
                    {
                        float y = imageBounds.Top + (sampleIndex / (float)Math.Max(sourceSize.Height - 1, 1) * imageBounds.Height);
                        e.Graphics.DrawLine(overlayPen, imageBounds.Left, y, imageBounds.Right, y);
                    }
                    break;
                case ImageScanDirection.LeftToRight:
                case ImageScanDirection.RightToLeft:
                    if (sourceSize.Width > 0)
                    {
                        float x = imageBounds.Left + (sampleIndex / (float)Math.Max(sourceSize.Width - 1, 1) * imageBounds.Width);
                        e.Graphics.DrawLine(overlayPen, x, imageBounds.Top, x, imageBounds.Bottom);
                    }
                    break;
            }
        }

        private static Rectangle GetScaledImageBounds(Rectangle bounds, Size imageSize)
        {
            if (imageSize.Width <= 0 || imageSize.Height <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return Rectangle.Empty;
            }

            float ratio = Math.Min(bounds.Width / (float)imageSize.Width, bounds.Height / (float)imageSize.Height);
            int width = Math.Max(1, (int)Math.Round(imageSize.Width * ratio));
            int height = Math.Max(1, (int)Math.Round(imageSize.Height * ratio));
            int x = bounds.Left + ((bounds.Width - width) / 2);
            int y = bounds.Top + ((bounds.Height - height) / 2);
            return new Rectangle(x, y, width, height);
        }

        private bool CanSeekByClick()
        {
            if (CurrentScene == null || previewImage == null || sourceSize.Width <= 0 || sourceSize.Height <= 0)
            {
                return false;
            }

            return CurrentScene.ImageRowCapture.IsPaused
                && (CurrentScene.ImageRowCapture.Direction != ImageScanDirection.Random || hasResolvedPreviewDirection);
        }

        private ImageScanDirection GetEffectiveSeekDirection()
        {
            if (CurrentScene == null)
            {
                return previewDirection;
            }

            ImageScanDirection configuredDirection = CurrentScene.ImageRowCapture.Direction;
            return configuredDirection == ImageScanDirection.Random ? previewDirection : configuredDirection;
        }

        private static int? MapPreviewClickToSampleIndex(int coordinate, int start, int length, int sampleLength)
        {
            if (length <= 0 || sampleLength <= 0)
            {
                return null;
            }

            double relative = (coordinate - start) / (double)Math.Max(length, 1);
            relative = Math.Clamp(relative, 0.0, 0.999999);
            return Math.Clamp((int)Math.Round(relative * Math.Max(sampleLength - 1, 0)), 0, sampleLength - 1);
        }

        private void ApplyLocalSeekPreview(int requestedSampleIndex, ImageScanDirection direction)
        {
            sampleIndex = Math.Clamp(
                requestedSampleIndex,
                0,
                direction is ImageScanDirection.TopToBottom or ImageScanDirection.BottomToTop
                    ? Math.Max(sourceSize.Height - 1, 0)
                    : Math.Max(sourceSize.Width - 1, 0));
            previewDirection = direction;
            hasResolvedPreviewDirection = true;

            if (previewImage != null && previewColors.Count > 0)
            {
                int ledCount = previewColors.Count;
                List<Color> sampledPixels = direction is ImageScanDirection.TopToBottom or ImageScanDirection.BottomToTop
                    ? PixelFrameHelpers.GetBitmapRowColors(previewImage, sampleIndex)
                    : PixelFrameHelpers.GetBitmapColumnColors(previewImage, sampleIndex);
                previewColors.Clear();
                previewColors.AddRange(PixelFrameHelpers.ReducePixels(sampledPixels, ledCount));
            }

            pictureBoxImagePreview.Invalidate();
            pictureBoxStripPreview.Invalidate();
            UpdatePauseUi();
        }

        private void ImageRowCaptureSceneEditorForm_Disposed(object? sender, EventArgs e)
        {
            previewImage?.Dispose();
            previewImage = null;
        }
    }
}
