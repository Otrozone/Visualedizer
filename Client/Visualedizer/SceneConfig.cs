namespace Ledqualizer
{
    public enum SceneType
    {
        SolidColor,
        Gradient,
        VolumeReactive,
        ScreenRowCapture,
        SpectralAnalysis
    }

    public sealed class SceneConfig
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "Scene";
        public SceneType Type { get; set; } = SceneType.SolidColor;
        public SolidColorSceneConfig SolidColor { get; set; } = new();
        public GradientSceneConfig Gradient { get; set; } = new();
        public VolumeReactiveSceneConfig VolumeReactive { get; set; } = new();
        public ScreenRowCaptureSceneConfig ScreenRowCapture { get; set; } = new();
        public SpectralAnalysisSceneConfig SpectralAnalysis { get; set; } = new();

        public SceneConfig Clone()
        {
            return new SceneConfig
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = Name,
                Type = Type,
                SolidColor = SolidColor.Clone(),
                Gradient = Gradient.Clone(),
                VolumeReactive = VolumeReactive.Clone(),
                ScreenRowCapture = ScreenRowCapture.Clone(),
                SpectralAnalysis = SpectralAnalysis.Clone()
            };
        }

        public static SceneConfig CreateDefault(SceneType type, int index)
        {
            return new SceneConfig
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = $"{SceneTypeNames.GetDisplayName(type)} {index}",
                Type = type
            };
        }
    }

    public sealed class SolidColorSceneConfig
    {
        public double Hue { get; set; }
        public double MinHue { get; set; }
        public double MaxHue { get; set; } = 360;
        public int Saturation { get; set; } = 100;
        public int Brightness { get; set; } = 50;

        public SolidColorSceneConfig Clone()
        {
            return (SolidColorSceneConfig)MemberwiseClone();
        }
    }

    public sealed class GradientSceneConfig
    {
        public double HueMin { get; set; }
        public double HueMax { get; set; } = 360;
        public int Saturation { get; set; } = 100;
        public int Brightness { get; set; } = 50;

        public GradientSceneConfig Clone()
        {
            return (GradientSceneConfig)MemberwiseClone();
        }
    }

    public class AudioReactiveSceneConfig
    {
        public AcVolume.AudioCaptureVolumeMode Mode { get; set; } = AcVolume.AudioCaptureVolumeMode.ModeStartToEnd;
        public int Brightness { get; set; } = 100;
        public int Saturation { get; set; } = 100;
        public int Normalization { get; set; } = 10;
        public bool Reverse { get; set; }
        public bool HueReverse { get; set; }
        public bool White { get; set; }
        public bool BackgroundWhite { get; set; }
        public int BackgroundBrightness { get; set; }
        public int BackgroundSaturation { get; set; } = -1;
        public double BackgroundHue { get; set; }
        public double HueMin { get; set; }
        public double HueMax { get; set; } = 360;
        public bool RotateModes { get; set; }
        public int RotateIntervalSeconds { get; set; } = 20;
    }

    public sealed class VolumeReactiveSceneConfig : AudioReactiveSceneConfig
    {
        public VolumeReactiveSceneConfig Clone()
        {
            return new VolumeReactiveSceneConfig
            {
                Mode = Mode,
                Brightness = Brightness,
                Saturation = Saturation,
                Normalization = Normalization,
                Reverse = Reverse,
                HueReverse = HueReverse,
                White = White,
                BackgroundWhite = BackgroundWhite,
                BackgroundBrightness = BackgroundBrightness,
                BackgroundSaturation = BackgroundSaturation,
                BackgroundHue = BackgroundHue,
                HueMin = HueMin,
                HueMax = HueMax,
                RotateModes = RotateModes,
                RotateIntervalSeconds = RotateIntervalSeconds
            };
        }
    }

    public sealed class SpectralAnalysisSceneConfig : AudioReactiveSceneConfig
    {
        public double FrequencyLowHz { get; set; } = 60;
        public double FrequencyHighHz { get; set; } = 250;
        public double LevelLowDb { get; set; } = -60;
        public double LevelHighDb { get; set; } = -20;

        public SpectralAnalysisSceneConfig Clone()
        {
            return new SpectralAnalysisSceneConfig
            {
                Mode = Mode,
                Brightness = Brightness,
                Saturation = Saturation,
                Normalization = Normalization,
                Reverse = Reverse,
                HueReverse = HueReverse,
                White = White,
                BackgroundWhite = BackgroundWhite,
                BackgroundBrightness = BackgroundBrightness,
                BackgroundSaturation = BackgroundSaturation,
                BackgroundHue = BackgroundHue,
                HueMin = HueMin,
                HueMax = HueMax,
                RotateModes = RotateModes,
                RotateIntervalSeconds = RotateIntervalSeconds,
                FrequencyLowHz = FrequencyLowHz,
                FrequencyHighHz = FrequencyHighHz,
                LevelLowDb = LevelLowDb,
                LevelHighDb = LevelHighDb
            };
        }
    }

    public sealed class ScreenRowCaptureSceneConfig
    {
        public int MonitorIndex { get; set; }
        public int CaptureY { get; set; }
        public bool Reverse { get; set; }

        public ScreenRowCaptureSceneConfig Clone()
        {
            return (ScreenRowCaptureSceneConfig)MemberwiseClone();
        }
    }

    internal sealed class DeviceConfig
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "Device";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 81;
        public int LedCount { get; set; }
        public int StripCount { get; set; }
        public bool Enabled { get; set; } = true;
        public string AssignedSceneId { get; set; } = string.Empty;
        public List<DeviceStripConfig> Strips { get; set; } = new();
    }

    internal sealed class DeviceStripConfig
    {
        public int StripIndex { get; set; }
        public int LedCount { get; set; }
        public bool Enabled { get; set; }
        public string AssignedSceneId { get; set; } = string.Empty;
    }

    internal enum DeviceRowKind
    {
        Device,
        Strip
    }

    internal sealed class DeviceGridRow
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ParentDeviceId { get; set; } = string.Empty;
        public DeviceRowKind Kind { get; set; } = DeviceRowKind.Device;
        public int StripIndex { get; set; } = -1;
        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Device";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 81;
        public int LedCount { get; set; }
        public int StripCount { get; set; }
        public string AssignedSceneId { get; set; } = string.Empty;
        public string Status { get; set; } = "Disconnected";

        public DeviceConfig ToDeviceConfig()
        {
            return new DeviceConfig
            {
                Id = Id,
                Enabled = Enabled,
                Name = Name,
                Host = Host,
                Port = Port,
                LedCount = LedCount,
                StripCount = StripCount,
                AssignedSceneId = AssignedSceneId
            };
        }

        public static DeviceGridRow FromDeviceConfig(DeviceConfig device)
        {
            return new DeviceGridRow
            {
                Id = device.Id,
                ParentDeviceId = device.Id,
                Kind = DeviceRowKind.Device,
                StripIndex = -1,
                Enabled = device.Enabled,
                Name = device.Name,
                Host = device.Host,
                Port = device.Port,
                LedCount = device.LedCount,
                StripCount = device.StripCount,
                AssignedSceneId = device.AssignedSceneId,
                Status = "Disconnected"
            };
        }

        public static DeviceGridRow FromStripConfig(DeviceConfig device, DeviceStripConfig strip)
        {
            return new DeviceGridRow
            {
                Id = $"{device.Id}:strip:{strip.StripIndex}",
                ParentDeviceId = device.Id,
                Kind = DeviceRowKind.Strip,
                StripIndex = strip.StripIndex,
                Enabled = strip.Enabled,
                Name = $"Strip {strip.StripIndex}",
                Host = device.Host,
                Port = device.Port,
                LedCount = strip.LedCount,
                StripCount = 1,
                AssignedSceneId = strip.AssignedSceneId,
                Status = "Disconnected"
            };
        }
    }

    internal sealed class SceneGridRow
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "Scene";
        public SceneType Type { get; set; } = SceneType.SolidColor;
        public string Summary { get; set; } = string.Empty;

        public static SceneGridRow FromSceneConfig(SceneConfig scene)
        {
            return new SceneGridRow
            {
                Id = scene.Id,
                Name = scene.Name,
                Type = scene.Type,
                Summary = SceneSummaryBuilder.Build(scene)
            };
        }
    }

    internal static class SceneTypeNames
    {
        public static string GetDisplayName(SceneType type)
        {
            return type switch
            {
                SceneType.SolidColor => "Solid Color",
                SceneType.Gradient => "Gradient",
                SceneType.VolumeReactive => "Volume Reactive",
                SceneType.ScreenRowCapture => "Screen Row Capture",
                SceneType.SpectralAnalysis => "Spectral Analysis",
                _ => type.ToString()
            };
        }
    }

    internal static class SceneSummaryBuilder
    {
        public static string Build(SceneConfig scene)
        {
            return scene.Type switch
            {
                SceneType.SolidColor => $"Hue {scene.SolidColor.Hue:F0}, Sat {scene.SolidColor.Saturation}%, Bright {scene.SolidColor.Brightness}%",
                SceneType.Gradient => $"Hue {scene.Gradient.HueMin:F0}-{scene.Gradient.HueMax:F0}, Sat {scene.Gradient.Saturation}%, Bright {scene.Gradient.Brightness}%",
                SceneType.VolumeReactive => $"Mode {scene.VolumeReactive.Mode}, Sat {scene.VolumeReactive.Saturation}%, Norm {scene.VolumeReactive.Normalization}",
                SceneType.ScreenRowCapture => $"Display {scene.ScreenRowCapture.MonitorIndex + 1}, Row {scene.ScreenRowCapture.CaptureY}" + (scene.ScreenRowCapture.Reverse ? ", Reversed" : string.Empty),
                SceneType.SpectralAnalysis => $"{scene.SpectralAnalysis.FrequencyLowHz:F0}-{scene.SpectralAnalysis.FrequencyHighHz:F0} Hz, Sat {scene.SpectralAnalysis.Saturation}%",
                _ => string.Empty
            };
        }
    }
}
