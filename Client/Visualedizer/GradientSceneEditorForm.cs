namespace Ledqualizer
{
    internal partial class GradientSceneEditorForm : SceneEditorFormBase
    {
        public override SceneType SceneType => SceneType.Gradient;

        public GradientSceneEditorForm()
        {
            InitializeComponent();
            ucHueMinMaxGradient.ValueChanged += ControlValueChanged;
        }

        protected override void OnLoadScene(SceneConfig scene)
        {
            ucHueMinMaxGradient.HueMin = (int)Math.Round(scene.Gradient.HueMin);
            ucHueMinMaxGradient.HueMax = (int)Math.Round(scene.Gradient.HueMax);
            trackBarSaturation.Value = Math.Max(trackBarSaturation.Minimum, Math.Min(trackBarSaturation.Maximum, scene.Gradient.Saturation));
            trackBarBrightness.Value = Math.Max(trackBarBrightness.Minimum, Math.Min(trackBarBrightness.Maximum, scene.Gradient.Brightness));
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (CurrentScene == null || IsLoadingScene)
            {
                return;
            }

            CurrentScene.Gradient.HueMin = ucHueMinMaxGradient.HueMin;
            CurrentScene.Gradient.HueMax = ucHueMinMaxGradient.HueMax;
            CurrentScene.Gradient.Saturation = trackBarSaturation.Value;
            CurrentScene.Gradient.Brightness = trackBarBrightness.Value;
            NotifySceneChanged();
        }
    }
}
