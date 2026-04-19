namespace Ledqualizer
{
    public partial class SolidColorSceneEditorForm : Form, ISceneEditorForm
    {
        private bool isLoading;

        public SceneType SceneType => SceneType.SolidColor;

        protected SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public SolidColorSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            ucHueSolid.ValueChanged += ControlValueChanged;
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
            ucHueSolid.Hue = (int)Math.Round(scene.SolidColor.Hue);
            ucHueSolid.MinVal = (int)Math.Round(scene.SolidColor.MinHue);
            ucHueSolid.MaxVal = (int)Math.Round(scene.SolidColor.MaxHue);
            trackBarSaturation.Value = Math.Max(trackBarSaturation.Minimum, Math.Min(trackBarSaturation.Maximum, scene.SolidColor.Saturation));
            trackBarBrightness.Value = Math.Max(trackBarBrightness.Minimum, Math.Min(trackBarBrightness.Maximum, scene.SolidColor.Brightness));
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (CurrentScene == null || isLoading)
            {
                return;
            }

            CurrentScene.SolidColor.Hue = ucHueSolid.Hue;
            CurrentScene.SolidColor.MinHue = ucHueSolid.MinVal;
            CurrentScene.SolidColor.MaxHue = ucHueSolid.MaxVal;
            CurrentScene.SolidColor.Saturation = trackBarSaturation.Value;
            CurrentScene.SolidColor.Brightness = trackBarBrightness.Value;
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
