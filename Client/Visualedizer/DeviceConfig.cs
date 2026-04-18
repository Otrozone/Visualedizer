namespace Ledqualizer
{
    internal enum SceneKind
    {
        Basic,
        Volume,
        ScreenCapture,
        OtherDevices
    }

    internal class DeviceConfig
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "Device";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 81;
        public int LedCount { get; set; } = 60;
        public bool Enabled { get; set; } = true;
        public SceneKind Scene { get; set; } = SceneKind.Basic;
    }

    internal class DeviceGridRow
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Device";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 81;
        public int LedCount { get; set; } = 60;
        public SceneKind Scene { get; set; } = SceneKind.Basic;
        public string Status { get; set; } = "Disconnected";

        public DeviceConfig ToDeviceConfig()
        {
            return new DeviceConfig
            {
                Id = Id,
                Name = Name,
                Host = Host,
                Port = Port,
                LedCount = LedCount,
                Enabled = Enabled,
                Scene = Scene
            };
        }

        public static DeviceGridRow FromDeviceConfig(DeviceConfig device)
        {
            return new DeviceGridRow
            {
                Id = device.Id,
                Enabled = device.Enabled,
                Name = device.Name,
                Host = device.Host,
                Port = device.Port,
                LedCount = device.LedCount,
                Scene = device.Scene,
                Status = "Disconnected"
            };
        }
    }
}
