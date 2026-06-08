namespace Ledqualizer
{
    public enum SceneType
    {
        SolidColor,
        Gradient,
        VolumeReactive,
        ScreenRowCapture,
        SpectralAnalysis,
        ImageRowCapture,
        LaserDmxLive,
        StrobeLive
    }

    public enum ImageSourceMode
    {
        SingleImage,
        Folder
    }

    public enum ImageScanDirection
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft,
        Random
    }

    public enum LaserDmxValueMode
    {
        Constant,
        RandomRange,
        ValueList,
        RandomValueFromList
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
        public ImageRowCaptureSceneConfig ImageRowCapture { get; set; } = new();
        public LaserDmxLiveSceneConfig LaserDmxLive { get; set; } = new();
        public StrobeLiveSceneConfig StrobeLive { get; set; } = new();

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
                SpectralAnalysis = SpectralAnalysis.Clone(),
                ImageRowCapture = ImageRowCapture.Clone(),
                LaserDmxLive = LaserDmxLive.Clone(),
                StrobeLive = StrobeLive.Clone()
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

    public sealed class ImageRowCaptureSceneConfig
    {
        public ImageSourceMode SourceMode { get; set; } = ImageSourceMode.SingleImage;
        public string ImagePath { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;
        public bool Recursive { get; set; }
        public bool Loop { get; set; } = true;
        public ImageScanDirection Direction { get; set; } = ImageScanDirection.TopToBottom;
        public double SpeedMin { get; set; } = 1.0;
        public double SpeedMax { get; set; } = 1.0;
        public bool IsPaused { get; set; }
        public int RequestedSampleIndex { get; set; } = -1;
        public int RequestedSeekRevision { get; set; }

        public ImageRowCaptureSceneConfig Clone()
        {
            return new ImageRowCaptureSceneConfig
            {
                SourceMode = SourceMode,
                ImagePath = ImagePath,
                FolderPath = FolderPath,
                Recursive = Recursive,
                Loop = Loop,
                Direction = Direction,
                SpeedMin = SpeedMin,
                SpeedMax = SpeedMax
            };
        }
    }

    public sealed class LaserDmxLiveSceneConfig
    {
        public List<LaserDmxChannelRow> Channels { get; set; } = new();

        public LaserDmxLiveSceneConfig Clone()
        {
            return new LaserDmxLiveSceneConfig
            {
                Channels = Channels.Select(channel => channel.Clone()).ToList()
            };
        }
    }

    public sealed class LaserDmxChannelRow
    {
        public int Channel { get; set; } = 1;
        public LaserDmxValueMode Mode { get; set; } = LaserDmxValueMode.Constant;
        public int ConstantValue { get; set; }
        public int RangeMin { get; set; }
        public int RangeMax { get; set; } = 255;
        public List<int> Values { get; set; } = new();
        public bool RefreshEnabled { get; set; }
        public double RefreshIntervalSeconds { get; set; } = 1.0;

        public LaserDmxChannelRow Clone()
        {
            return new LaserDmxChannelRow
            {
                Channel = Channel,
                Mode = Mode,
                ConstantValue = ConstantValue,
                RangeMin = RangeMin,
                RangeMax = RangeMax,
                Values = new List<int>(Values),
                RefreshEnabled = RefreshEnabled,
                RefreshIntervalSeconds = RefreshIntervalSeconds
            };
        }
    }

    public sealed class StrobeLiveSceneConfig
    {
        public int TriggerX { get; set; }
        public int TriggerY { get; set; }

        public StrobeLiveSceneConfig Clone()
        {
            return (StrobeLiveSceneConfig)MemberwiseClone();
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
                SceneType.ImageRowCapture => "Image Row Capture",
                SceneType.LaserDmxLive => "Laser DMX",
                SceneType.StrobeLive => "Strobe",
                _ => type.ToString()
            };
        }
    }

    internal static class SceneTypeRules
    {
        public static bool IsAuxiliary(SceneType type)
        {
            return type is SceneType.LaserDmxLive or SceneType.StrobeLive;
        }

        public static bool SupportsStripAssignment(SceneType type)
        {
            return !IsAuxiliary(type);
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
                SceneType.ImageRowCapture => BuildImageRowSummary(scene.ImageRowCapture),
                SceneType.LaserDmxLive => BuildLaserDmxSummary(scene.LaserDmxLive),
                SceneType.StrobeLive => "Activation on/off",
                _ => string.Empty
            };
        }

        private static string BuildImageRowSummary(ImageRowCaptureSceneConfig config)
        {
            string source = config.SourceMode == ImageSourceMode.Folder ? "Folder" : "Image";
            string speed = Math.Abs(config.SpeedMin - config.SpeedMax) < 0.0001
                ? $"{config.SpeedMin:F2} row/tick"
                : $"{config.SpeedMin:F2}-{config.SpeedMax:F2} row/tick";
            string loop = config.Loop ? ", Loop" : string.Empty;
            return $"{source}, {config.Direction}, {speed}{loop}";
        }

        private static string BuildLaserDmxSummary(LaserDmxLiveSceneConfig config)
        {
            int refreshCount = config.Channels.Count(channel => channel.RefreshEnabled);
            return $"{config.Channels.Count} channels" + (refreshCount > 0 ? $", {refreshCount} refreshing" : string.Empty);
        }
    }
}
