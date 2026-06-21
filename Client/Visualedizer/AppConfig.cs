using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ledqualizer
{
    internal sealed class AppConfig
    {
        private const int CurrentConfigVersion = 2;
        private const string JsonFileName = "config.json";
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public int ConfigVersion { get; set; } = CurrentConfigVersion;
        public List<DeviceConfig> Devices { get; set; } = new();
        public List<SceneConfig> Scenes { get; set; } = new();
        public List<ConfigurationCollection> Collections { get; set; } = new();
        public KeyboardShortcutConfig ResetShortcut { get; set; } = new();

        public int Delay { get; set; } = 20;

        public int StrobeTriggerX { get; set; }
        public int StrobeTriggerY { get; set; }
        public int LaserTriggerX { get; set; }
        public int LaserTriggerY { get; set; }
        public int LaserPatternX { get; set; }
        public int LaserPatternY { get; set; }
        public int LaserColorY { get; set; }
        public int LaserColorX { get; set; }

        public void Load()
        {
            Devices.Clear();
            Scenes.Clear();
            Collections.Clear();
            ResetShortcut = new KeyboardShortcutConfig();

            if (File.Exists(JsonFileName))
            {
                string json = File.ReadAllText(JsonFileName);
                AppConfig? loaded = JsonSerializer.Deserialize<AppConfig>(json, JsonOptions);
                if (loaded != null)
                {
                    CopyFrom(loaded);
                    if (ConfigVersion < CurrentConfigVersion)
                    {
                        MigrateDeviceLevelLedAssignmentsToStrips();
                    }

                    EnsureDefaults();
                    ConfigVersion = CurrentConfigVersion;
                    return;
                }
            }

            EnsureDefaults();
            ConfigVersion = CurrentConfigVersion;
        }

        public void Save()
        {
            EnsureDefaults();
            ConfigVersion = CurrentConfigVersion;
            string json = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(JsonFileName, json);
        }

        private void CopyFrom(AppConfig loaded)
        {
            ConfigVersion = loaded.ConfigVersion;
            Devices = loaded.Devices ?? new List<DeviceConfig>();
            Scenes = loaded.Scenes ?? new List<SceneConfig>();
            Collections = loaded.Collections ?? new List<ConfigurationCollection>();
            ResetShortcut = loaded.ResetShortcut ?? new KeyboardShortcutConfig();
            Delay = loaded.Delay;
            StrobeTriggerX = loaded.StrobeTriggerX;
            StrobeTriggerY = loaded.StrobeTriggerY;
            LaserTriggerX = loaded.LaserTriggerX;
            LaserTriggerY = loaded.LaserTriggerY;
            LaserPatternX = loaded.LaserPatternX;
            LaserPatternY = loaded.LaserPatternY;
            LaserColorX = loaded.LaserColorX;
            LaserColorY = loaded.LaserColorY;
        }

        private void EnsureDefaults()
        {
            Collections ??= new List<ConfigurationCollection>();
            ResetShortcut ??= new KeyboardShortcutConfig();

            if (Scenes.Count == 0)
            {
                Scenes.Add(SceneConfig.CreateDefault(SceneType.SolidColor, 1));
            }

            if (Devices.Count == 0)
            {
                string defaultLedSceneId = Scenes.FirstOrDefault(scene => SceneTypeRules.SupportsStripAssignment(scene.Type))?.Id
                    ?? string.Empty;
                Devices.Add(new DeviceConfig
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "Device 1",
                    Host = "127.0.0.1",
                    Port = 81,
                    LedCount = 0,
                    StripCount = 0,
                    Enabled = true,
                    AssignedSceneId = defaultLedSceneId,
                    AssignedLaserSceneId = string.Empty,
                    AssignedStrobeSceneId = string.Empty
                });
            }

            string fallbackLedSceneId = Scenes
                .FirstOrDefault(scene => SceneTypeRules.SupportsStripAssignment(scene.Type))?.Id
                ?? string.Empty;
            foreach (DeviceConfig device in Devices)
            {
                if (device.StripCount <= 0 && device.LedCount > 0)
                {
                    device.StripCount = 1;
                }

                if (string.IsNullOrWhiteSpace(device.AssignedSceneId)
                    || !IsLedSceneAssignment(device.AssignedSceneId))
                {
                    device.AssignedSceneId = fallbackLedSceneId;
                }

                if (!string.IsNullOrWhiteSpace(device.AssignedLaserSceneId)
                    && !IsLaserSceneAssignment(device.AssignedLaserSceneId))
                {
                    device.AssignedLaserSceneId = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(device.AssignedStrobeSceneId)
                    && !IsStrobeSceneAssignment(device.AssignedStrobeSceneId))
                {
                    device.AssignedStrobeSceneId = string.Empty;
                }

                device.Strips ??= new List<DeviceStripConfig>();
                EnsureStripDefaults(device, fallbackLedSceneId, FindSceneType);
            }

            foreach (ConfigurationCollection collection in Collections)
            {
                if (string.IsNullOrWhiteSpace(collection.Id))
                {
                    collection.Id = Guid.NewGuid().ToString("N");
                }

                if (string.IsNullOrWhiteSpace(collection.Name))
                {
                    collection.Name = "Collection";
                }

                if (collection.CreatedUtc == default)
                {
                    collection.CreatedUtc = DateTime.UtcNow;
                }

                collection.Shortcut ??= new KeyboardShortcutConfig();
                collection.Devices ??= new List<CollectionDeviceSnapshot>();
                foreach (CollectionDeviceSnapshot device in collection.Devices)
                {
                    device.Strips ??= new List<CollectionStripSnapshot>();
                }
            }
        }

        private void MigrateDeviceLevelLedAssignmentsToStrips()
        {
            string fallbackLedSceneId = Scenes
                .FirstOrDefault(scene => SceneTypeRules.SupportsStripAssignment(scene.Type))?.Id
                ?? string.Empty;

            foreach (DeviceConfig device in Devices)
            {
                bool oldDeviceEnabled = device.Enabled;
                bool oldDeviceHadLedScene = IsLedSceneAssignment(device.AssignedSceneId);
                bool oldDeviceHadAuxiliary = IsLaserSceneAssignment(device.AssignedLaserSceneId)
                    || IsStrobeSceneAssignment(device.AssignedStrobeSceneId);

                EnsureStripDefaults(device, fallbackLedSceneId, FindSceneType);

                if (oldDeviceEnabled && oldDeviceHadLedScene)
                {
                    foreach (DeviceStripConfig strip in device.Strips)
                    {
                        strip.Enabled = true;
                        if (string.IsNullOrWhiteSpace(strip.AssignedSceneId)
                            || string.Equals(strip.AssignedSceneId, fallbackLedSceneId, StringComparison.Ordinal))
                        {
                            strip.AssignedSceneId = device.AssignedSceneId;
                        }
                    }
                }

                device.Enabled = oldDeviceEnabled && oldDeviceHadAuxiliary;
            }
        }

        private static void EnsureStripDefaults(DeviceConfig device, string fallbackSceneId, Func<string, SceneType?> findSceneType)
        {
            var normalized = new List<DeviceStripConfig>();
            var activeStrips = new List<DeviceStripConfig>();
            var extraStrips = new List<DeviceStripConfig>();
            var seenActiveIndices = new HashSet<int>();
            int fallbackLedCount = device.StripCount > 0 ? Math.Max(device.LedCount / Math.Max(device.StripCount, 1), 0) : 0;

            foreach (DeviceStripConfig strip in device.Strips.Where(strip => strip.StripIndex >= 0))
            {
                if (strip.StripIndex < device.StripCount)
                {
                    if (seenActiveIndices.Add(strip.StripIndex))
                    {
                        activeStrips.Add(strip);
                    }
                }
                else
                {
                    extraStrips.Add(strip);
                }
            }

            foreach (DeviceStripConfig strip in activeStrips)
            {
                NormalizeStrip(strip, fallbackSceneId, fallbackLedCount, findSceneType);
                normalized.Add(strip);
            }

            for (int i = 0; i < device.StripCount; i++)
            {
                if (seenActiveIndices.Contains(i))
                {
                    continue;
                }

                var strip = new DeviceStripConfig
                {
                    StripIndex = i,
                    AssignedSceneId = fallbackSceneId
                };
                NormalizeStrip(strip, fallbackSceneId, fallbackLedCount, findSceneType);
                normalized.Add(strip);
            }

            foreach (DeviceStripConfig extraStrip in extraStrips)
            {
                NormalizeStrip(extraStrip, fallbackSceneId, fallbackLedCount, findSceneType);
                normalized.Add(extraStrip);
            }

            device.Strips = normalized;
        }

        private static void NormalizeStrip(DeviceStripConfig strip, string fallbackSceneId, int fallbackLedCount, Func<string, SceneType?> findSceneType)
        {
            if (string.IsNullOrWhiteSpace(strip.AssignedSceneId))
            {
                strip.AssignedSceneId = fallbackSceneId;
            }
            else
            {
                SceneType? sceneType = findSceneType(strip.AssignedSceneId);
                if (sceneType == null || !SceneTypeRules.SupportsStripAssignment(sceneType.Value))
                {
                    strip.AssignedSceneId = fallbackSceneId;
                }
            }

            if (strip.LedCount <= 0)
            {
                strip.LedCount = fallbackLedCount;
            }
        }

        private SceneType? FindSceneType(string sceneId)
        {
            return Scenes.FirstOrDefault(scene => string.Equals(scene.Id, sceneId, StringComparison.Ordinal))?.Type;
        }

        private bool IsLedSceneAssignment(string sceneId)
        {
            SceneType? sceneType = FindSceneType(sceneId);
            return sceneType != null && SceneTypeRules.SupportsStripAssignment(sceneType.Value);
        }

        private bool IsLaserSceneAssignment(string sceneId)
        {
            SceneType? sceneType = FindSceneType(sceneId);
            return sceneType != null && SceneTypeRules.IsLaser(sceneType.Value);
        }

        private bool IsStrobeSceneAssignment(string sceneId)
        {
            SceneType? sceneType = FindSceneType(sceneId);
            return sceneType != null && SceneTypeRules.IsStrobe(sceneType.Value);
        }

    }
}
