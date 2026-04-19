namespace Ledqualizer
{
    internal partial class SolidColorSceneEditorForm : SceneEditorFormBase
    {
        public override SceneType SceneType => SceneType.SolidColor;

        public SolidColorSceneEditorForm()
        {
            InitializeComponent();
            ucHueSolid.ValueChanged += ControlValueChanged;
        }

        protected override void OnLoadScene(SceneConfig scene)
        {
            ucHueSolid.Hue = (int)Math.Round(scene.SolidColor.Hue);
            ucHueSolid.MinVal = (int)Math.Round(scene.SolidColor.MinHue);
            ucHueSolid.MaxVal = (int)Math.Round(scene.SolidColor.MaxHue);
            trackBarSaturation.Value = Math.Max(trackBarSaturation.Minimum, Math.Min(trackBarSaturation.Maximum, scene.SolidColor.Saturation));
            trackBarBrightness.Value = Math.Max(trackBarBrightness.Minimum, Math.Min(trackBarBrightness.Maximum, scene.SolidColor.Brightness));
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (CurrentScene == null || IsLoadingScene)
            {
                return;
            }

            CurrentScene.SolidColor.Hue = ucHueSolid.Hue;
            CurrentScene.SolidColor.MinHue = ucHueSolid.MinVal;
            CurrentScene.SolidColor.MaxHue = ucHueSolid.MaxVal;
            CurrentScene.SolidColor.Saturation = trackBarSaturation.Value;
            CurrentScene.SolidColor.Brightness = trackBarBrightness.Value;
            NotifySceneChanged();
        }
    }
}
