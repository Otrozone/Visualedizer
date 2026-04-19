using IniParser;
using IniParser.Model;
using System.Globalization;

namespace Ledqualizer
{
    internal sealed class AppConfig
    {
        private enum LegacySceneKind
        {
            Basic,
            Volume,
            SpectralAnalysis,
            ScreenCapture,
            OtherDevices
        }

        private const string IniFileName = "config.ini";
        private const string IniSectionSettings = "Settings";
        private const string IniSectionScreenCaptureOther = "ScreenCaptureOther";
        private const string IniSectionDevicePrefix = "Device:";
        private const string IniSectionScenePrefix = "Scene:";

        public List<DeviceConfig> Devices { get; set; } = new();
        public List<SceneConfig> Scenes { get; set; } = new();

        public int Delay { get; set; } = 20;

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
            Scenes.Clear();

            if (!File.Exists(IniFileName))
            {
                EnsureDefaults();
                return;
            }

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(IniFileName);

            Delay = ParseInt(GetValue(data, IniSectionSettings, "delay"), Delay);

            StrobeTriggerX = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "strobeTriggerX"), StrobeTriggerX);
            StrobeTriggerY = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "strobeTriggerY"), StrobeTriggerY);
            LaserTriggerX = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserTriggerX"), LaserTriggerX);
            LaserTriggerY = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserTriggerY"), LaserTriggerY);
            LaserPatternX = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserPatternX"), LaserPatternX);
            LaserPatternY = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserPatternY"), LaserPatternY);
            LaserColorX = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserColorX"), LaserColorX);
            LaserColorY = ParseInt(GetValue(data, IniSectionScreenCaptureOther, "laserColorY"), LaserColorY);

            List<SectionData> deviceSections = data.Sections
                .Where(section => section.SectionName.StartsWith(IniSectionDevicePrefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (SectionData section in data.Sections)
            {
                if (!section.SectionName.StartsWith(IniSectionScenePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                SceneConfig? scene = LoadScene(section);
                if (scene != null)
                {
                    Scenes.Add(scene);
                }
            }

            var legacySceneMap = new Dictionary<LegacySceneKind, string>();
            foreach (SectionData section in deviceSections)
            {
                DeviceConfig? device = LoadDevice(section, legacySceneMap, data);
                if (device != null)
                {
                    Devices.Add(device);
                }
            }

            if (Devices.Count == 0)
            {
                DeviceConfig? migrated = TryMigrateLegacySingleDevice(data, legacySceneMap);
                if (migrated != null)
                {
                    Devices.Add(migrated);
                }
            }

            EnsureDefaults();
        }

        public void SaveToIni()
        {
            EnsureDefaults();

            var parser = new FileIniDataParser();
            IniData data = new();

            data[IniSectionSettings]["delay"] = Delay.ToString(CultureInfo.InvariantCulture);

            data[IniSectionScreenCaptureOther]["strobeTriggerX"] = StrobeTriggerX.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["strobeTriggerY"] = StrobeTriggerY.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserTriggerX"] = LaserTriggerX.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserTriggerY"] = LaserTriggerY.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserPatternX"] = LaserPatternX.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserPatternY"] = LaserPatternY.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserColorX"] = LaserColorX.ToString(CultureInfo.InvariantCulture);
            data[IniSectionScreenCaptureOther]["laserColorY"] = LaserColorY.ToString(CultureInfo.InvariantCulture);

            for (int i = 0; i < Scenes.Count; i++)
            {
                SceneConfig scene = Scenes[i];
                string sectionName = $"{IniSectionScenePrefix}{i}";
                SaveSceneSection(data[sectionName], scene);
            }

            for (int i = 0; i < Devices.Count; i++)
            {
                DeviceConfig device = Devices[i];
                string sectionName = $"{IniSectionDevicePrefix}{i}";
                data[sectionName]["id"] = device.Id;
                data[sectionName]["name"] = device.Name;
                data[sectionName]["host"] = device.Host;
                data[sectionName]["port"] = device.Port.ToString(CultureInfo.InvariantCulture);
                data[sectionName]["ledCount"] = device.LedCount.ToString(CultureInfo.InvariantCulture);
                data[sectionName]["stripCount"] = device.StripCount.ToString(CultureInfo.InvariantCulture);
                data[sectionName]["enabled"] = device.Enabled.ToString(CultureInfo.InvariantCulture);
                data[sectionName]["assignedSceneId"] = device.AssignedSceneId;
            }

            parser.WriteFile(IniFileName, data);
        }

        private void SaveSceneSection(KeyDataCollection section, SceneConfig scene)
        {
            section["id"] = scene.Id;
            section["name"] = scene.Name;
            section["type"] = scene.Type.ToString();

            section["solidHue"] = scene.SolidColor.Hue.ToString(CultureInfo.InvariantCulture);
            section["solidMinHue"] = scene.SolidColor.MinHue.ToString(CultureInfo.InvariantCulture);
            section["solidMaxHue"] = scene.SolidColor.MaxHue.ToString(CultureInfo.InvariantCulture);
            section["solidSaturation"] = scene.SolidColor.Saturation.ToString(CultureInfo.InvariantCulture);
            section["solidBrightness"] = scene.SolidColor.Brightness.ToString(CultureInfo.InvariantCulture);

            section["gradientHueMin"] = scene.Gradient.HueMin.ToString(CultureInfo.InvariantCulture);
            section["gradientHueMax"] = scene.Gradient.HueMax.ToString(CultureInfo.InvariantCulture);
            section["gradientSaturation"] = scene.Gradient.Saturation.ToString(CultureInfo.InvariantCulture);
            section["gradientBrightness"] = scene.Gradient.Brightness.ToString(CultureInfo.InvariantCulture);

            SaveAudioReactiveSection(section, scene.VolumeReactive, "volume");
            SaveAudioReactiveSection(section, scene.SpectralAnalysis, "spectral");

            section["spectralFrequencyLow"] = scene.SpectralAnalysis.FrequencyLowHz.ToString(CultureInfo.InvariantCulture);
            section["spectralFrequencyHigh"] = scene.SpectralAnalysis.FrequencyHighHz.ToString(CultureInfo.InvariantCulture);
            section["spectralLevelLowDb"] = scene.SpectralAnalysis.LevelLowDb.ToString(CultureInfo.InvariantCulture);
            section["spectralLevelHighDb"] = scene.SpectralAnalysis.LevelHighDb.ToString(CultureInfo.InvariantCulture);

            section["screenCaptureRow"] = scene.ScreenRowCapture.CaptureY.ToString(CultureInfo.InvariantCulture);
            section["screenCaptureMonitorIndex"] = scene.ScreenRowCapture.MonitorIndex.ToString(CultureInfo.InvariantCulture);
            section["screenCaptureReverse"] = scene.ScreenRowCapture.Reverse.ToString(CultureInfo.InvariantCulture);
        }

        private static void SaveAudioReactiveSection(KeyDataCollection section, AudioReactiveSceneConfig config, string prefix)
        {
            section[$"{prefix}Mode"] = config.Mode.ToString();
            section[$"{prefix}Brightness"] = config.Brightness.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}Normalization"] = config.Normalization.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}Reverse"] = config.Reverse.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}HueReverse"] = config.HueReverse.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}White"] = config.White.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}BackgroundWhite"] = config.BackgroundWhite.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}BackgroundBrightness"] = config.BackgroundBrightness.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}BackgroundHue"] = config.BackgroundHue.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}HueMin"] = config.HueMin.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}HueMax"] = config.HueMax.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}RotateModes"] = config.RotateModes.ToString(CultureInfo.InvariantCulture);
            section[$"{prefix}RotateIntervalSeconds"] = config.RotateIntervalSeconds.ToString(CultureInfo.InvariantCulture);
        }

        private SceneConfig? LoadScene(SectionData section)
        {
            string name = section.Keys["name"];
            string typeValue = section.Keys["type"];

            if (string.IsNullOrWhiteSpace(name) || !Enum.TryParse(typeValue, true, out SceneType sceneType))
            {
                return null;
            }

            SceneConfig scene = new()
            {
                Id = string.IsNullOrWhiteSpace(section.Keys["id"]) ? Guid.NewGuid().ToString("N") : section.Keys["id"],
                Name = name,
                Type = sceneType
            };

            scene.SolidColor.Hue = ParseDouble(section.Keys["solidHue"], scene.SolidColor.Hue);
            scene.SolidColor.MinHue = ParseDouble(section.Keys["solidMinHue"], scene.SolidColor.MinHue);
            scene.SolidColor.MaxHue = ParseDouble(section.Keys["solidMaxHue"], scene.SolidColor.MaxHue);
            scene.SolidColor.Saturation = ParseInt(section.Keys["solidSaturation"], scene.SolidColor.Saturation);
            scene.SolidColor.Brightness = ParseInt(section.Keys["solidBrightness"], scene.SolidColor.Brightness);

            scene.Gradient.HueMin = ParseDouble(section.Keys["gradientHueMin"], scene.Gradient.HueMin);
            scene.Gradient.HueMax = ParseDouble(section.Keys["gradientHueMax"], scene.Gradient.HueMax);
            scene.Gradient.Saturation = ParseInt(section.Keys["gradientSaturation"], scene.Gradient.Saturation);
            scene.Gradient.Brightness = ParseInt(section.Keys["gradientBrightness"], scene.Gradient.Brightness);

            LoadAudioReactiveSection(section, scene.VolumeReactive, "volume");
            LoadAudioReactiveSection(section, scene.SpectralAnalysis, "spectral");

            scene.SpectralAnalysis.FrequencyLowHz = ParseDouble(section.Keys["spectralFrequencyLow"], scene.SpectralAnalysis.FrequencyLowHz);
            scene.SpectralAnalysis.FrequencyHighHz = ParseDouble(section.Keys["spectralFrequencyHigh"], scene.SpectralAnalysis.FrequencyHighHz);
            scene.SpectralAnalysis.LevelLowDb = ParseDouble(section.Keys["spectralLevelLowDb"], scene.SpectralAnalysis.LevelLowDb);
            scene.SpectralAnalysis.LevelHighDb = ParseDouble(section.Keys["spectralLevelHighDb"], scene.SpectralAnalysis.LevelHighDb);

            scene.ScreenRowCapture.CaptureY = ParseInt(section.Keys["screenCaptureRow"], scene.ScreenRowCapture.CaptureY);
            scene.ScreenRowCapture.MonitorIndex = ParseInt(section.Keys["screenCaptureMonitorIndex"], scene.ScreenRowCapture.MonitorIndex);
            scene.ScreenRowCapture.Reverse = ParseBool(section.Keys["screenCaptureReverse"], scene.ScreenRowCapture.Reverse);

            return scene;
        }

        private static void LoadAudioReactiveSection(SectionData section, AudioReactiveSceneConfig config, string prefix)
        {
            config.Mode = ParseAudioMode(section.Keys[$"{prefix}Mode"], config.Mode);
            config.Brightness = ParseInt(section.Keys[$"{prefix}Brightness"], config.Brightness);
            config.Normalization = ParseInt(section.Keys[$"{prefix}Normalization"], config.Normalization);
            config.Reverse = ParseBool(section.Keys[$"{prefix}Reverse"], config.Reverse);
            config.HueReverse = ParseBool(section.Keys[$"{prefix}HueReverse"], config.HueReverse);
            config.White = ParseBool(section.Keys[$"{prefix}White"], config.White);
            config.BackgroundWhite = ParseBool(section.Keys[$"{prefix}BackgroundWhite"], config.BackgroundWhite);
            config.BackgroundBrightness = ParseInt(section.Keys[$"{prefix}BackgroundBrightness"], config.BackgroundBrightness);
            config.BackgroundHue = ParseDouble(section.Keys[$"{prefix}BackgroundHue"], config.BackgroundHue);
            config.HueMin = ParseDouble(section.Keys[$"{prefix}HueMin"], config.HueMin);
            config.HueMax = ParseDouble(section.Keys[$"{prefix}HueMax"], config.HueMax);
            config.RotateModes = ParseBool(section.Keys[$"{prefix}RotateModes"], config.RotateModes);
            config.RotateIntervalSeconds = ParseInt(section.Keys[$"{prefix}RotateIntervalSeconds"], config.RotateIntervalSeconds);
        }

        private DeviceConfig? LoadDevice(SectionData section, IDictionary<LegacySceneKind, string> legacySceneMap, IniData data)
        {
            string host = section.Keys["host"];
            int port = ParseInt(section.Keys["port"], 81);
            int ledCount = ParseInt(section.Keys["ledCount"], 0);
            int stripCount = ParseInt(section.Keys["stripCount"], 0);
            if (string.IsNullOrWhiteSpace(host) || port <= 0)
            {
                return null;
            }

            string assignedSceneId = section.Keys["assignedSceneId"];
            if (string.IsNullOrWhiteSpace(assignedSceneId))
            {
                assignedSceneId = MigrateLegacySceneAssignment(section.Keys["scene"], legacySceneMap, data);
            }

            return new DeviceConfig
            {
                Id = string.IsNullOrWhiteSpace(section.Keys["id"]) ? Guid.NewGuid().ToString("N") : section.Keys["id"],
                Name = string.IsNullOrWhiteSpace(section.Keys["name"]) ? "Device" : section.Keys["name"],
                Host = host,
                Port = port,
                LedCount = ledCount,
                StripCount = stripCount,
                Enabled = ParseBool(section.Keys["enabled"], true),
                AssignedSceneId = assignedSceneId
            };
        }

        private DeviceConfig? TryMigrateLegacySingleDevice(IniData data, IDictionary<LegacySceneKind, string> legacySceneMap)
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
                StripCount = ledCount > 0 ? 1 : 0,
                Enabled = true,
                AssignedSceneId = GetOrCreateLegacyScene(LegacySceneKind.Basic, legacySceneMap, data)
            };
        }

        private string MigrateLegacySceneAssignment(string sceneValue, IDictionary<LegacySceneKind, string> legacySceneMap, IniData data)
        {
            if (Enum.TryParse(sceneValue, true, out LegacySceneKind kind))
            {
                return GetOrCreateLegacyScene(kind, legacySceneMap, data);
            }

            return GetOrCreateLegacyScene(LegacySceneKind.Basic, legacySceneMap, data);
        }

        private string GetOrCreateLegacyScene(LegacySceneKind kind, IDictionary<LegacySceneKind, string> legacySceneMap, IniData data)
        {
            if (legacySceneMap.TryGetValue(kind, out string existingId))
            {
                return existingId;
            }

            SceneConfig scene = CreateMigratedScene(kind, data);
            Scenes.Add(scene);
            legacySceneMap[kind] = scene.Id;
            return scene.Id;
        }

        private SceneConfig CreateMigratedScene(LegacySceneKind kind, IniData data)
        {
            SceneConfig scene = SceneConfig.CreateDefault(MapLegacySceneType(kind), Scenes.Count + 1);
            scene.Name = kind switch
            {
                LegacySceneKind.Basic => "Migrated Solid Color",
                LegacySceneKind.Volume => "Migrated Volume Reactive",
                LegacySceneKind.SpectralAnalysis => "Migrated Spectral Analysis",
                LegacySceneKind.ScreenCapture => "Migrated Screen Row Capture",
                _ => "Migrated Solid Color"
            };

            scene.SolidColor.Brightness = ParseInt(GetValue(data, IniSectionSettings, "brightness"), scene.SolidColor.Brightness);
            scene.VolumeReactive.Brightness = ParseInt(GetValue(data, IniSectionSettings, "brightness"), scene.VolumeReactive.Brightness);
            scene.VolumeReactive.Normalization = ParseInt(GetValue(data, IniSectionSettings, "normalizationLevel"), scene.VolumeReactive.Normalization);
            scene.SpectralAnalysis.Brightness = scene.VolumeReactive.Brightness;
            scene.SpectralAnalysis.Normalization = scene.VolumeReactive.Normalization;
            scene.SpectralAnalysis.FrequencyLowHz = ParseInt(GetValue(data, "SpectralAnalysis", "frequencyLow"), (int)scene.SpectralAnalysis.FrequencyLowHz);
            scene.SpectralAnalysis.FrequencyHighHz = ParseInt(GetValue(data, "SpectralAnalysis", "frequencyHigh"), (int)scene.SpectralAnalysis.FrequencyHighHz);
            scene.SpectralAnalysis.LevelLowDb = ParseInt(GetValue(data, "SpectralAnalysis", "levelLowDb"), (int)scene.SpectralAnalysis.LevelLowDb);
            scene.SpectralAnalysis.LevelHighDb = ParseInt(GetValue(data, "SpectralAnalysis", "levelHighDb"), (int)scene.SpectralAnalysis.LevelHighDb);
            scene.ScreenRowCapture.CaptureY = ParseInt(GetValue(data, "ScreenCapture", "screenCaptureRow"), scene.ScreenRowCapture.CaptureY);
            scene.ScreenRowCapture.MonitorIndex = ParseInt(GetValue(data, "ScreenCapture", "screenCaptureMonitorIndex"), scene.ScreenRowCapture.MonitorIndex);

            return scene;
        }

        private static SceneType MapLegacySceneType(LegacySceneKind kind)
        {
            return kind switch
            {
                LegacySceneKind.Volume => SceneType.VolumeReactive,
                LegacySceneKind.SpectralAnalysis => SceneType.SpectralAnalysis,
                LegacySceneKind.ScreenCapture => SceneType.ScreenRowCapture,
                _ => SceneType.SolidColor
            };
        }

        private void EnsureDefaults()
        {
            if (Scenes.Count == 0)
            {
                Scenes.Add(SceneConfig.CreateDefault(SceneType.SolidColor, 1));
            }

            if (Devices.Count == 0)
            {
                Devices.Add(new DeviceConfig
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "Device 1",
                    Host = "127.0.0.1",
                    Port = 81,
                    LedCount = 0,
                    StripCount = 0,
                    Enabled = true,
                    AssignedSceneId = Scenes[0].Id
                });
            }

            string fallbackSceneId = Scenes[0].Id;
            foreach (DeviceConfig device in Devices)
            {
                if (string.IsNullOrWhiteSpace(device.AssignedSceneId))
                {
                    device.AssignedSceneId = fallbackSceneId;
                }
            }
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

        private static double ParseDouble(string value, double defaultValue)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed) ? parsed : defaultValue;
        }

        private static bool ParseBool(string value, bool defaultValue)
        {
            return bool.TryParse(value, out bool parsed) ? parsed : defaultValue;
        }

        private static AcVolume.AudioCaptureVolumeMode ParseAudioMode(string value, AcVolume.AudioCaptureVolumeMode defaultValue)
        {
            return Enum.TryParse(value, true, out AcVolume.AudioCaptureVolumeMode parsed) ? parsed : defaultValue;
        }
    }
}
