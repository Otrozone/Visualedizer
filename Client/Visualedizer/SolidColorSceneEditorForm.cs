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
            ucColor.ValueChanged += ControlValueChanged;
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                ucColor.Hue = (int)Math.Round(scene.SolidColor.Hue);
                ucColor.MinHue = (int)Math.Round(scene.SolidColor.MinHue);
                ucColor.MaxHue = (int)Math.Round(scene.SolidColor.MaxHue);
                ucColor.Saturation = scene.SolidColor.Saturation;
                ucColor.Brightness = scene.SolidColor.Brightness;
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

            CurrentScene.SolidColor.Hue = ucColor.Hue;
            CurrentScene.SolidColor.MinHue = ucColor.MinHue;
            CurrentScene.SolidColor.MaxHue = ucColor.MaxHue;
            CurrentScene.SolidColor.Saturation = ucColor.Saturation;
            CurrentScene.SolidColor.Brightness = ucColor.Brightness;
            SceneChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
