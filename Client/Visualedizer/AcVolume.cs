using NAudio.CoreAudioApi;

namespace Ledqualizer
{
    public static class AcVolume
    {
        public enum AudioCaptureVolumeMode
        {
            ModeStartToEnd = 0,
            ModeEndToStart = 1,
            ModeMidToOut = 2,
            ModeColorPush = 3,
            ModeMidToOut_Point = 4,
            ModeBrightness = 5
        }

        public sealed class DeviceDescriptor
        {
            public string DeviceId { get; set; } = string.Empty;
            public string Text { get; set; } = string.Empty;
        }

        public static void LoadAudioDevicesToComboBox(ComboBox comboBox)
        {
            using MMDeviceEnumerator deviceEnumerator = new();
            using MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            comboBox.Items.Clear();
            foreach (MMDevice device in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                DeviceDescriptor item = new()
                {
                    DeviceId = device.ID,
                    Text = device.ID.Equals(defaultDevice.ID, StringComparison.Ordinal) ? $"{device.FriendlyName} [default]" : device.FriendlyName
                };

                int idx = comboBox.Items.Add(item);
                if (device.ID.Equals(defaultDevice.ID, StringComparison.Ordinal))
                {
                    comboBox.SelectedIndex = idx;
                }
            }

            comboBox.DisplayMember = nameof(DeviceDescriptor.Text);
            comboBox.ValueMember = nameof(DeviceDescriptor.DeviceId);
        }
    }
}
