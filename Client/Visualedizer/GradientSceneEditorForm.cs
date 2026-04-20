namespace Ledqualizer
{
    public partial class GradientSceneEditorForm : Form, ISceneEditorForm
    {
        private bool isLoading;

        public SceneType SceneType => SceneType.Gradient;

        protected SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        public GradientSceneEditorForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            ucHueMinMaxGradient.ValueChanged += ControlValueChanged;
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
            ucHueMinMaxGradient.HueStart = (int)Math.Round(scene.Gradient.HueMin);
            ucHueMinMaxGradient.HueEnd = (int)Math.Round(scene.Gradient.HueMax);
            trackBarSaturation.Value = Math.Max(trackBarSaturation.Minimum, Math.Min(trackBarSaturation.Maximum, scene.Gradient.Saturation));
            trackBarBrightness.Value = Math.Max(trackBarBrightness.Minimum, Math.Min(trackBarBrightness.Maximum, scene.Gradient.Brightness));
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

            CurrentScene.Gradient.HueMin = ucHueMinMaxGradient.HueStart;
            CurrentScene.Gradient.HueMax = ucHueMinMaxGradient.HueEnd;
            CurrentScene.Gradient.Saturation = trackBarSaturation.Value;
            CurrentScene.Gradient.Brightness = trackBarBrightness.Value;
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
