namespace Ledqualizer
{
    internal sealed class OtherDevicesSettingsChangedEventArgs : EventArgs
    {
        public OtherDevicesSettingsChangedEventArgs(OtherDevicesSceneSettings settings)
        {
            Settings = settings;
        }

        public OtherDevicesSceneSettings Settings { get; }
    }

    internal partial class OtherDevicesForm : Form
    {
        private bool isLoading;

        public event EventHandler<OtherDevicesSettingsChangedEventArgs>? SettingsChanged;

        public OtherDevicesForm()
        {
            InitializeComponent();
        }

        public void LoadSettings(OtherDevicesSceneSettings settings)
        {
            isLoading = true;
            try
            {
                numStrobeX.Value = settings.StrobeTriggerX;
                numStrobeY.Value = settings.StrobeTriggerY;
                numLaserTriggerX.Value = settings.LaserTriggerX;
                numLaserTriggerY.Value = settings.LaserTriggerY;
                numLaserPatternX.Value = settings.LaserPatternX;
                numLaserPatternY.Value = settings.LaserPatternY;
                numLaserColorX.Value = settings.LaserColorX;
                numLaserColorY.Value = settings.LaserColorY;
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ControlValueChanged(object? sender, EventArgs e)
        {
            if (isLoading)
            {
                return;
            }

            SettingsChanged?.Invoke(this, new OtherDevicesSettingsChangedEventArgs(new OtherDevicesSceneSettings
            {
                StrobeTriggerX = (int)numStrobeX.Value,
                StrobeTriggerY = (int)numStrobeY.Value,
                LaserTriggerX = (int)numLaserTriggerX.Value,
                LaserTriggerY = (int)numLaserTriggerY.Value,
                LaserPatternX = (int)numLaserPatternX.Value,
                LaserPatternY = (int)numLaserPatternY.Value,
                LaserColorX = (int)numLaserColorX.Value,
                LaserColorY = (int)numLaserColorY.Value
            }));
        }
    }
}
