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
            ucGradient.ValueChanged += ControlValueChanged;
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                ucGradient.HueStart = (int)Math.Round(scene.Gradient.HueMin);
                ucGradient.HueEnd = (int)Math.Round(scene.Gradient.HueMax);
                ucGradient.Saturation = scene.Gradient.Saturation;
                ucGradient.Brightness = scene.Gradient.Brightness;
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

            CurrentScene.Gradient.HueMin = ucGradient.HueStart;
            CurrentScene.Gradient.HueMax = ucGradient.HueEnd;
            CurrentScene.Gradient.Saturation = ucGradient.Saturation;
            CurrentScene.Gradient.Brightness = ucGradient.Brightness;
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
