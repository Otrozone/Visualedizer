using IniParser;
using IniParser.Model;
using System.Globalization;

namespace Ledqualizer
{
    internal class AppConfig
    {
        private const string IniFileName = "config.ini";
        private const string IniSectionSettings = "Settings";
        private const string IniSectionScreenCapture = "ScreenCapture";
        private const string IniSectionScreenCaptureOther = "ScreenCaptureOther";
        private const string IniSectionDevicePrefix = "Device:";

        public List<DeviceConfig> Devices { get; set; } = new();

        public int Delay { get; set; } = 20;
        public float Brightness { get; set; } = 1.0f;
        public float NormalizationLevel { get; set; } = 1.0f;
        public int ScreenCaptureRow { get; set; }

        public int StrobeTriggerX { get; set; }
        public int StrobeTriggerY { get; set; }
        public int LaserTriggerX { get; set; }
        public int LaserTriggerY { get; set; }
        public int LaserPatternX { get; set; }
        public int LaserPatternY { get; set; }
        public int LaserColorY { get; set; }
        public int LaserColorX { get; set; }

        public void LoadFromIni()
        {
            Devices.Clear();

            if (!File.Exists(IniFileName))
            {
                EnsureAtLeastOneDevice();
                return;
            }

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(IniFileName);

            Delay = ParseInt(GetValue(data, IniSectionSettings, "delay"), Delay);
            Brightness = ParseFloat(GetValue(data, IniSectionSettings, "brightness"), Brightness);
            NormalizationLevel = ParseFloat(GetValue(data, IniSectionSettings, "normalizationLevel"), NormalizationLevel);

            ScreenCaptureRow = ParseInt(GetValue(data, IniSectionScreenCapture, "screenCaptureRow"), ScreenCaptureRow);

            StrobeTriggerX = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "strobeTriggerX"), StrobeTriggerX);
            StrobeTriggerY = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "strobeTriggerY"), StrobeTriggerY);
            LaserTriggerX = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserTriggerX"), LaserTriggerX);
            LaserTriggerY = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserTriggerY"), LaserTriggerY);
            LaserPatternX = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserPatternX"), LaserPatternX);
            LaserPatternY = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserPatternY"), LaserPatternY);
            LaserColorX = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserColorX"), LaserColorX);
            LaserColorY = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserColorY"), LaserColorY);

            foreach (SectionData section in data.Sections)
            {
                if (!section.SectionName.StartsWith(IniSectionDevicePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                DeviceConfig? device = LoadDevice(section);
                if (device != null)
                {
                    Devices.Add(device);
                }
            }

            if (Devices.Count == 0)
            {
                DeviceConfig? migrated = TryMigrateLegacyDevice(data);
                if (migrated != null)
                {
                    Devices.Add(migrated);
                }
            }

            EnsureAtLeastOneDevice();
        }

        public void SaveToIni()
        {
            EnsureAtLeastOneDevice();

            var parser = new FileIniDataParser();
            IniData data = new IniData();

            data[IniSectionSettings]["delay"] = Delay.ToString(CultureInfo.InvariantCulture);
            data[IniSectionSettings]["brightness"] = Brightness.ToString(CultureInfo.InvariantCulture);
            data[IniSectionSettings]["normalizationLevel"] = NormalizationLevel.ToString(CultureInfo.InvariantCulture);

            data[IniSectionScreenCapture]["screenCaptureRow"] = ScreenCaptureRow.ToString(CultureInfo.InvariantCulture);

            data[IniSectionScreenCaptureOther]["strobeTriggerX"] = StrobeTriggerX.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["strobeTriggerY"] = StrobeTriggerY.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserTriggerX"] = LaserTriggerX.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserTriggerY"] = LaserTriggerY.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserPatternX"] = LaserPatternX.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserPatternY"] = LaserPatternY.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserColorX"] = LaserColorX.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserColorY"] = LaserColorY.ToString(CultureInfo.InvariantCulture);

            for (int i = 0; i < Devices.Count; i++)
            {
                string sectionName = $"{IniSectionDevicePrefix}{i}";
                DeviceConfig device = Devices[i];

                data[sectionName]["id"] = device.Id;
                data[sectionName]["name"] = device.Name;
                data[sectionName]["host"] = device.Host;
                data[sectionName]["port"] = device.Port.ToString(CultureInfo.InvariantCulture);
                data[sectionName]["ledCount"] = device.LedCount.ToString(CultureInfo.InvariantCulture);
                data[sectionName]["enabled"] = device.Enabled.ToString(CultureInfo.InvariantCulture);
                data[sectionName]["scene"] = device.Scene.ToString();
            }

            parser.WriteFile(IniFileName, data);
        }

        private DeviceConfig? LoadDevice(SectionData section)
        {
            string host = section.Keys["host"];
            int port = ParseInt(section.Keys["port"], 81);
            int ledCount = ParseInt(section.Keys["ledCount"], 0);

            if (string.IsNullOrWhiteSpace(host) || port <= 0 || ledCount <= 0)
            {
                return null;
            }

            return new DeviceConfig
            {
                Id = string.IsNullOrWhiteSpace(section.Keys["id"]) ? Guid.NewGuid().ToString("N") : section.Keys["id"],
                Name = string.IsNullOrWhiteSpace(section.Keys["name"]) ? "Device" : section.Keys["name"],
                Host = host,
                Port = port,
                LedCount = ledCount,
                Enabled = ParseBool(section.Keys["enabled"], true),
                Scene = ParseSceneKind(section.Keys["scene"], SceneKind.Basic)
            };
        }

        private DeviceConfig? TryMigrateLegacyDevice(IniData data)
        {
            string host = GetValue(data, IniSectionSettings, "ipAddress");
            int port = ParseInt(GetValue(data, IniSectionSettings, "port"), 81);
            int ledCount = ParseInt(GetValue(data, IniSectionSettings, "ledCount"), 0);

            if (string.IsNullOrWhiteSpace(host) || port <= 0 || ledCount <= 0)
            {
                return null;
            }

            return new DeviceConfig
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Device 1",
                Host = host,
                Port = port,
                LedCount = ledCount,
                Enabled = true,
                Scene = SceneKind.Basic
            };
        }

        private void EnsureAtLeastOneDevice()
        {
            if (Devices.Count > 0)
            {
                return;
            }

            Devices.Add(new DeviceConfig
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Device 1",
                Host = "127.0.0.1",
                Port = 81,
                LedCount = 218,
                Enabled = true,
                Scene = SceneKind.Basic
            });
        }

        private static string GetValue(IniData data, string section, string key)
        {
            if (!data.Sections.ContainsSection(section))
            {
                return string.Empty;
            }

            return data[section][key] ?? string.Empty;
        }

        private static int ParseInt(string value, int defaultValue)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) ? parsed : defaultValue;
        }

        private static float ParseFloat(string value, float defaultValue)
        {
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) ? parsed : defaultValue;
        }

        private static bool ParseBool(string value, bool defaultValue)
        {
            return bool.TryParse(value, out bool parsed) ? parsed : defaultValue;
        }

        private static SceneKind ParseSceneKind(string value, SceneKind defaultValue)
        {
            return Enum.TryParse(value, ignoreCase: true, out SceneKind parsed) ? parsed : defaultValue;
        }
    }
}
