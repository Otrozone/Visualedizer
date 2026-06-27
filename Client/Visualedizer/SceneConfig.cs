using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace Ledqualizer
{
    public enum SceneType
    {
        SolidColor,
        Gradient,
        VolumeReactive,
        ScreenRowCapture,
        SpectralAnalysis,
        SpectralAnalysisSegments,
        ImageRowCapture,
        SparkleAndFlash,
        LaserDmx,
        Strobe,
        LedStrobe
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

    public enum SpectralSegmentHueMode
    {
        FixedRange,
        RandomInRange,
        RandomRange
    }

    public enum SpectralSegmentSaturationMode
    {
        Fixed,
        RandomRange
    }

    public enum SpectralSegmentBrightnessMode
    {
        Fixed,
        LevelMapped
    }

    public enum SpectralSegmentSizeMode
    {
        Full,
        StartToEnd,
        EndToStart,
        CenterOut,
        CenterPoint
    }

    public enum LaserDmxValueMode
    {
        Constant,
        RandomRange,
        ValueList,
        RandomValueFromList
    }

    public enum StrobeTimingMode
    {
        Constant,
        RandomRange
    }

    public enum StrobeHueMode
    {
        Constant,
        RandomRange
    }

    public enum AuxiliaryTriggerEventType
    {
        Volume,
        SpectralAnalysis,
        ScreenCapture
    }

    public enum AuxiliaryTriggerRetriggerMode
    {
        OneShotUntilReset,
        RepeatWhileHigh,
        HoldWhileHigh
    }

    public enum CollectionActivationMode
    {
        Toggle,
        Hold
    }

    public sealed class KeyboardShortcutConfig
    {
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public bool Alt { get; set; }
        public Keys Key { get; set; } = Keys.None;

        [JsonIgnore]
        public bool IsEmpty => Key == Keys.None && !Control && !Shift && !Alt;

        [JsonIgnore]
        public bool IsUsable => Key != Keys.None && !IsModifierKey(Key);

        public KeyboardShortcutConfig Clone()
        {
            return new KeyboardShortcutConfig
            {
                Control = Control,
                Shift = Shift,
                Alt = Alt,
                Key = Key
            };
        }

        public bool Matches(KeyboardShortcutConfig? other)
        {
            return other != null
                && Control == other.Control
                && Shift == other.Shift
                && Alt == other.Alt
                && Key == other.Key;
        }

        public string GetSignature()
        {
            return $"{Control}:{Shift}:{Alt}:{Key}";
        }

        public override string ToString()
        {
            if (IsEmpty)
            {
                return string.Empty;
            }

            var parts = new List<string>();
            if (Control)
            {
                parts.Add("Ctrl");
            }

            if (Shift)
            {
                parts.Add("Shift");
            }

            if (Alt)
            {
                parts.Add("Alt");
            }

            if (Key != Keys.None)
            {
                parts.Add(FormatKey(Key));
            }

            return string.Join(" + ", parts);
        }

        public static KeyboardShortcutConfig Empty()
        {
            return new KeyboardShortcutConfig();
        }

        public static KeyboardShortcutConfig FromKeys(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            return new KeyboardShortcutConfig
            {
                Control = (keyData & Keys.Control) == Keys.Control,
                Shift = (keyData & Keys.Shift) == Keys.Shift,
                Alt = (keyData & Keys.Alt) == Keys.Alt,
                Key = NormalizeKey(key)
            };
        }

        public static KeyboardShortcutConfig FromKeyEvent(KeyEventArgs e)
        {
            return new KeyboardShortcutConfig
            {
                Control = e.Control,
                Shift = e.Shift,
                Alt = e.Alt,
                Key = NormalizeKey(e.KeyCode)
            };
        }

        public static bool IsModifierKey(Keys key)
        {
            return key is Keys.ControlKey
                or Keys.LControlKey
                or Keys.RControlKey
                or Keys.ShiftKey
                or Keys.LShiftKey
                or Keys.RShiftKey
                or Keys.Menu
                or Keys.LMenu
                or Keys.RMenu
                or Keys.Control
                or Keys.Shift
                or Keys.Alt;
        }

        private static Keys NormalizeKey(Keys key)
        {
            return key switch
            {
                Keys.ControlKey or Keys.LControlKey or Keys.RControlKey => Keys.None,
                Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey => Keys.None,
                Keys.Menu or Keys.LMenu or Keys.RMenu => Keys.None,
                _ => key
            };
        }

        private static string FormatKey(Keys key)
        {
            return key switch
            {
                Keys.Escape => "Esc",
                Keys.Return => "Enter",
                Keys.Space => "Space",
                Keys.Next => "Page Down",
                Keys.Prior => "Page Up",
                _ => key.ToString()
            };
        }
    }

    public sealed class ConfigurationCollection
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "Collection";
        public CollectionActivationMode ActivationMode { get; set; } = CollectionActivationMode.Toggle;
        public KeyboardShortcutConfig Shortcut { get; set; } = new();
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public List<CollectionDeviceSnapshot> Devices { get; set; } = new();

        public bool HasTargets()
        {
            return Devices.Any(device =>
                device.Strips.Any(strip => strip.Scene != null && strip.LedCount > 0)
                || device.LaserScene != null
                || device.StrobeScene != null);
        }
    }

    public sealed class CollectionDeviceSnapshot
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = "Device";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 81;
        public int LedCount { get; set; }
        public int StripCount { get; set; }
        public SceneConfig? LaserScene { get; set; }
        public SceneConfig? StrobeScene { get; set; }
        public List<CollectionStripSnapshot> Strips { get; set; } = new();
    }

    public sealed class CollectionStripSnapshot
    {
        public int StripIndex { get; set; }
        public int StartIndex { get; set; }
        public int LedCount { get; set; }
        public SceneConfig? Scene { get; set; }
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
        public SpectralAnalysisSegmentsSceneConfig SpectralAnalysisSegments { get; set; } = new();
        public ImageRowCaptureSceneConfig ImageRowCapture { get; set; } = new();
        public SparkleAndFlashSceneConfig SparkleAndFlash { get; set; } = new();
        public LaserDmxSceneConfig LaserDmx { get; set; } = new();
        public StrobeSceneConfig Strobe { get; set; } = new();
        public LedStrobeSceneConfig LedStrobe { get; set; } = new();

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
                SpectralAnalysisSegments = SpectralAnalysisSegments.Clone(),
                ImageRowCapture = ImageRowCapture.Clone(),
                SparkleAndFlash = SparkleAndFlash.Clone(),
                LaserDmx = LaserDmx.Clone(),
                Strobe = Strobe.Clone(),
                LedStrobe = LedStrobe.Clone()
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

    public sealed class SparkleAndFlashSceneConfig
    {
        public int SegmentSizeMin { get; set; } = 1;
        public int SegmentSizeMax { get; set; } = 3;
        public int SegmentHoldMs { get; set; } = 1000;
        public int SegmentIntervalMinMs { get; set; } = 250;
        public int SegmentIntervalMaxMs { get; set; } = 1500;
        public double SparkleHueMin { get; set; }
        public double SparkleHueMax { get; set; } = 360;
        public int SparkleHueChangeIntervalMinMs { get; set; } = 5000;
        public int SparkleHueChangeIntervalMaxMs { get; set; } = 15000;
        public bool ContinuousSparkleHueChange { get; set; }
        public bool SmoothFadeAndBlur { get; set; } = true;
        public int FadeDurationMs { get; set; } = 700;
        public int BlurRadius { get; set; } = 2;
        public int MaxActiveSparkles { get; set; } = 8;
        public bool FullStripFlashEnabled { get; set; } = true;
        public int FullStripFlashHoldMs { get; set; } = 1000;
        public bool FullStripSmoothFade { get; set; } = true;
        public int FullStripFadeDurationMs { get; set; } = 700;
        public int FullStripFlashIntervalMinMs { get; set; } = 15000;
        public int FullStripFlashIntervalMaxMs { get; set; } = 45000;

        public SparkleAndFlashSceneConfig Clone()
        {
            return (SparkleAndFlashSceneConfig)MemberwiseClone();
        }
    }

    public sealed class LedStrobeSceneConfig
    {
        public StrobeTimingMode OnDurationMode { get; set; } = StrobeTimingMode.Constant;
        public int OnDurationMs { get; set; } = 80;
        public int OnDurationMinMs { get; set; } = 80;
        public int OnDurationMaxMs { get; set; } = 160;
        public StrobeTimingMode OffDurationMode { get; set; } = StrobeTimingMode.Constant;
        public int OffDurationMs { get; set; } = 80;
        public int OffDurationMinMs { get; set; } = 40;
        public int OffDurationMaxMs { get; set; } = 160;
        public StrobeHueMode HueMode { get; set; } = StrobeHueMode.Constant;
        public double Hue { get; set; }
        public double HueMin { get; set; }
        public double HueMax { get; set; } = 360;
        public int Saturation { get; set; } = 100;
        public int Brightness { get; set; } = 100;

        public LedStrobeSceneConfig Clone()
        {
            return (LedStrobeSceneConfig)MemberwiseClone();
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

    public sealed class SpectralAnalysisSegmentsSceneConfig
    {
        public List<SpectralAnalysisSegmentConfig> Segments { get; set; } = CreateDefaultSegments();

        public SpectralAnalysisSegmentsSceneConfig Clone()
        {
            EnsureSegmentDefaults();
            return new SpectralAnalysisSegmentsSceneConfig
            {
                Segments = Segments.Select(segment => segment.Clone()).ToList()
            };
        }

        public void EnsureSegmentDefaults()
        {
            Segments ??= CreateDefaultSegments();
            for (int i = 0; i < Segments.Count; i++)
            {
                Segments[i] = NormalizeSegment(Segments[i], i);
            }
        }

        public static List<SpectralAnalysisSegmentConfig> CreateDefaultSegments()
        {
            var segments = new List<SpectralAnalysisSegmentConfig>
            {
                CreateFullStripAccentSegment()
            };

            for (int i = 0; i < 5; i++)
            {
                segments.Add(CreateDefaultBandSegment(i));
            }

            return segments;
        }

        public static SpectralAnalysisSegmentConfig CreateFullStripAccentSegment()
        {
            return new SpectralAnalysisSegmentConfig
            {
                Name = "Full strip accent",
                RatioDenominator = 1,
                SegmentIndex = 0,
                FrequencyLowHz = 20,
                FrequencyHighHz = 20000,
                ThresholdDb = -25,
                BrightnessMode = SpectralSegmentBrightnessMode.LevelMapped,
                BrightnessLow = 20,
                LevelLowDb = -25,
                LevelHighDb = -15,
                SizeMode = SpectralSegmentSizeMode.CenterOut,
                CenterPointWidthPercent = 10,
                HueMode = SpectralSegmentHueMode.RandomInRange,
                HueStart = 0,
                HueEnd = 360
            };
        }

        public static SpectralAnalysisSegmentConfig CreateDefaultBandSegment(int bandIndex)
        {
            double[] lows = { 20, 80, 316, 1260, 5012 };
            double[] highs = { 80, 316, 1260, 5012, 20000 };
            int normalizedIndex = Math.Clamp(bandIndex, 0, 4);
            return new SpectralAnalysisSegmentConfig
            {
                Name = $"Band {normalizedIndex + 1}",
                RatioDenominator = 5,
                SegmentIndex = normalizedIndex,
                FrequencyLowHz = lows[normalizedIndex],
                FrequencyHighHz = highs[normalizedIndex],
                ThresholdDb = -35,
                BrightnessMode = SpectralSegmentBrightnessMode.LevelMapped,
                BrightnessLow = 20,
                LevelLowDb = -35,
                LevelHighDb = -15,
                SizeMode = SpectralSegmentSizeMode.CenterOut,
                CenterPointWidthPercent = 10,
                HueMode = SpectralSegmentHueMode.FixedRange,
                HueStart = normalizedIndex * 72,
                HueEnd = (normalizedIndex + 1) * 72
            };
        }

        public static SpectralAnalysisSegmentConfig NormalizeSegment(SpectralAnalysisSegmentConfig? segment, int fallbackIndex)
        {
            SpectralAnalysisSegmentConfig normalized = segment?.Clone() ?? CreateDefaultBandSegment(fallbackIndex % 5);
            if (string.IsNullOrWhiteSpace(normalized.Id))
            {
                normalized.Id = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrWhiteSpace(normalized.Name))
            {
                normalized.Name = $"Segment {fallbackIndex + 1}";
            }

            normalized.RatioDenominator = Math.Clamp(normalized.RatioDenominator, 1, 1000);
            normalized.SegmentIndex = Math.Clamp(normalized.SegmentIndex, 0, normalized.RatioDenominator - 1);
            double lowHz = Math.Clamp(Math.Min(normalized.FrequencyLowHz, normalized.FrequencyHighHz), 20.0, 20000.0);
            double highHz = Math.Clamp(Math.Max(normalized.FrequencyLowHz, normalized.FrequencyHighHz), lowHz, 20000.0);
            normalized.FrequencyLowHz = lowHz;
            normalized.FrequencyHighHz = highHz;
            normalized.ThresholdDb = Math.Clamp(normalized.ThresholdDb, -90.0, 0.0);
            normalized.LightUpMs = Math.Clamp(normalized.LightUpMs, 1, 60000);
            normalized.FadeOutMs = Math.Clamp(normalized.FadeOutMs, 0, 60000);
            normalized.BrightnessLow = Math.Clamp(normalized.BrightnessLow, 0, 100);
            normalized.Brightness = Math.Clamp(normalized.Brightness, 0, 100);
            if (normalized.BrightnessLow > normalized.Brightness)
            {
                normalized.BrightnessLow = normalized.Brightness;
            }

            double levelLowDb = Math.Clamp(Math.Min(normalized.LevelLowDb, normalized.LevelHighDb), -90.0, 0.0);
            double levelHighDb = Math.Clamp(Math.Max(normalized.LevelLowDb, normalized.LevelHighDb), -90.0, 0.0);
            if (levelHighDb - levelLowDb < 0.1)
            {
                if (levelHighDb >= 0.0)
                {
                    levelLowDb = Math.Max(-90.0, levelHighDb - 0.1);
                }
                else
                {
                    levelHighDb = Math.Min(0.0, levelLowDb + 0.1);
                }
            }

            normalized.LevelLowDb = levelLowDb;
            normalized.LevelHighDb = levelHighDb;
            if (!Enum.IsDefined(normalized.SizeMode))
            {
                normalized.SizeMode = SpectralSegmentSizeMode.Full;
            }

            normalized.CenterPointWidthPercent = Math.Clamp(normalized.CenterPointWidthPercent, 1, 100);
            normalized.HueStart = Math.Clamp(normalized.HueStart, 0.0, 360.0);
            normalized.HueEnd = Math.Clamp(normalized.HueEnd, 0.0, 360.0);
            normalized.Saturation = Math.Clamp(normalized.Saturation, 0, 100);
            int saturationMin = Math.Clamp(Math.Min(normalized.SaturationMin, normalized.SaturationMax), 0, 100);
            int saturationMax = Math.Clamp(Math.Max(normalized.SaturationMin, normalized.SaturationMax), saturationMin, 100);
            normalized.SaturationMin = saturationMin;
            normalized.SaturationMax = saturationMax;
            return normalized;
        }
    }

    public sealed class SpectralAnalysisSegmentConfig
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public bool Enabled { get; set; } = true;
        public string Name { get; set; } = "Segment";
        public int RatioDenominator { get; set; } = 5;
        public int SegmentIndex { get; set; }
        public double FrequencyLowHz { get; set; } = 20;
        public double FrequencyHighHz { get; set; } = 20000;
        public double ThresholdDb { get; set; } = -35;
        public int LightUpMs { get; set; } = 70;
        public int FadeOutMs { get; set; } = 1000;
        public SpectralSegmentBrightnessMode BrightnessMode { get; set; } = SpectralSegmentBrightnessMode.Fixed;
        public int BrightnessLow { get; set; } = 20;
        public int Brightness { get; set; } = 100;
        public double LevelLowDb { get; set; } = -35;
        public double LevelHighDb { get; set; } = -15;
        public SpectralSegmentSizeMode SizeMode { get; set; } = SpectralSegmentSizeMode.Full;
        public int CenterPointWidthPercent { get; set; } = 10;
        public SpectralSegmentHueMode HueMode { get; set; } = SpectralSegmentHueMode.FixedRange;
        public double HueStart { get; set; }
        public double HueEnd { get; set; } = 360;
        public SpectralSegmentSaturationMode SaturationMode { get; set; } = SpectralSegmentSaturationMode.Fixed;
        public int Saturation { get; set; } = 100;
        public int SaturationMin { get; set; } = 100;
        public int SaturationMax { get; set; } = 100;

        public SpectralAnalysisSegmentConfig Clone()
        {
            return (SpectralAnalysisSegmentConfig)MemberwiseClone();
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

    public sealed class AuxiliaryTriggerConfig
    {
        public AuxiliaryTriggerEventType EventType { get; set; } = AuxiliaryTriggerEventType.Volume;
        public AuxiliaryTriggerRetriggerMode RetriggerMode { get; set; } = AuxiliaryTriggerRetriggerMode.OneShotUntilReset;
        public int OnDurationMs { get; set; } = 300;
        public AuxiliaryVolumeTriggerConfig Volume { get; set; } = new();
        public AuxiliarySpectralTriggerConfig SpectralAnalysis { get; set; } = new();
        public AuxiliaryScreenCaptureTriggerConfig ScreenCapture { get; set; } = new();

        public AuxiliaryTriggerConfig Clone()
        {
            return new AuxiliaryTriggerConfig
            {
                EventType = EventType,
                RetriggerMode = RetriggerMode,
                OnDurationMs = OnDurationMs,
                Volume = Volume.Clone(),
                SpectralAnalysis = SpectralAnalysis.Clone(),
                ScreenCapture = ScreenCapture.Clone()
            };
        }
    }

    public sealed class AuxiliaryVolumeTriggerConfig
    {
        public string AudioDeviceId { get; set; } = string.Empty;
        public int ThresholdPercent { get; set; } = 65;

        public AuxiliaryVolumeTriggerConfig Clone()
        {
            return (AuxiliaryVolumeTriggerConfig)MemberwiseClone();
        }
    }

    public sealed class AuxiliarySpectralTriggerConfig
    {
        public string AudioDeviceId { get; set; } = string.Empty;
        public double FrequencyLowHz { get; set; } = 60;
        public double FrequencyHighHz { get; set; } = 250;
        public double ThresholdDb { get; set; } = -30;

        public AuxiliarySpectralTriggerConfig Clone()
        {
            return (AuxiliarySpectralTriggerConfig)MemberwiseClone();
        }
    }

    public sealed class AuxiliaryScreenCaptureTriggerConfig
    {
        public int MonitorIndex { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
        public int BrightnessThresholdPercent { get; set; } = 70;

        public AuxiliaryScreenCaptureTriggerConfig Clone()
        {
            return (AuxiliaryScreenCaptureTriggerConfig)MemberwiseClone();
        }
    }

    public sealed class LaserDmxSceneConfig
    {
        public AuxiliaryTriggerConfig Trigger { get; set; } = new();
        public bool SendFullDmxPacket { get; set; }
        public List<LaserDmxChannelRow> Channels { get; set; } = new();

        public LaserDmxSceneConfig Clone()
        {
            return new LaserDmxSceneConfig
            {
                Trigger = Trigger.Clone(),
                SendFullDmxPacket = SendFullDmxPacket,
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

    public sealed class StrobeSceneConfig
    {
        public AuxiliaryTriggerConfig Trigger { get; set; } = new();

        public StrobeSceneConfig Clone()
        {
            return new StrobeSceneConfig
            {
                Trigger = Trigger.Clone()
            };
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
        public string AssignedLaserSceneId { get; set; } = string.Empty;
        public string AssignedStrobeSceneId { get; set; } = string.Empty;
        public List<DeviceStripConfig> Strips { get; set; } = new();

        public DeviceConfig Clone()
        {
            return new DeviceConfig
            {
                Id = Id,
                Name = Name,
                Host = Host,
                Port = Port,
                LedCount = LedCount,
                StripCount = StripCount,
                Enabled = Enabled,
                AssignedSceneId = AssignedSceneId,
                AssignedLaserSceneId = AssignedLaserSceneId,
                AssignedStrobeSceneId = AssignedStrobeSceneId,
                Strips = Strips.Select(strip => strip.Clone()).ToList()
            };
        }
    }

    internal sealed class DeviceStripConfig
    {
        public int StripIndex { get; set; }
        public int LedCount { get; set; }
        public bool Enabled { get; set; }
        public string AssignedSceneId { get; set; } = string.Empty;

        public DeviceStripConfig Clone()
        {
            return new DeviceStripConfig
            {
                StripIndex = StripIndex,
                LedCount = LedCount,
                Enabled = Enabled,
                AssignedSceneId = AssignedSceneId
            };
        }
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
        public string AssignedLaserSceneId { get; set; } = string.Empty;
        public string AssignedStrobeSceneId { get; set; } = string.Empty;
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
                AssignedSceneId = AssignedSceneId,
                AssignedLaserSceneId = AssignedLaserSceneId,
                AssignedStrobeSceneId = AssignedStrobeSceneId
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
                AssignedLaserSceneId = device.AssignedLaserSceneId,
                AssignedStrobeSceneId = device.AssignedStrobeSceneId,
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
                AssignedLaserSceneId = string.Empty,
                AssignedStrobeSceneId = string.Empty,
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

    internal sealed class CollectionGridRow
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "Collection";
        public CollectionActivationMode ActivationMode { get; set; } = CollectionActivationMode.Toggle;
        public string ShortcutText { get; set; } = string.Empty;
        public string TargetSummary { get; set; } = string.Empty;
        public string StatusText { get; set; } = "Inactive";

        public static CollectionGridRow FromCollection(ConfigurationCollection collection, bool isActive)
        {
            return new CollectionGridRow
            {
                Id = collection.Id,
                Name = collection.Name,
                ActivationMode = collection.ActivationMode,
                ShortcutText = collection.Shortcut?.ToString() ?? string.Empty,
                TargetSummary = CollectionSummaryBuilder.Build(collection),
                StatusText = isActive ? "Active" : "Inactive"
            };
        }
    }

    internal static class CollectionSummaryBuilder
    {
        public static string Build(ConfigurationCollection collection)
        {
            int deviceCount = collection.Devices.Count;
            int stripCount = collection.Devices.Sum(device => device.Strips.Count(strip => strip.Scene != null && strip.LedCount > 0));
            int laserCount = collection.Devices.Count(device => device.LaserScene != null);
            int strobeCount = collection.Devices.Count(device => device.StrobeScene != null);
            var parts = new List<string>();
            if (stripCount > 0)
            {
                parts.Add($"{stripCount} LED strip{(stripCount == 1 ? string.Empty : "s")}");
            }

            if (laserCount > 0)
            {
                parts.Add($"{laserCount} laser");
            }

            if (strobeCount > 0)
            {
                parts.Add($"{strobeCount} strobe");
            }

            string targetText = parts.Count == 0 ? "No targets" : string.Join(", ", parts);
            return $"{deviceCount} device{(deviceCount == 1 ? string.Empty : "s")}: {targetText}";
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
                SceneType.SpectralAnalysisSegments => "Spectral Analysis Segments",
                SceneType.ImageRowCapture => "Image Row Capture",
                SceneType.SparkleAndFlash => "Sparkle and Flash",
                SceneType.LaserDmx => "Laser DMX",
                SceneType.Strobe => "Auxiliary Strobe",
                SceneType.LedStrobe => "Strobe",
                _ => type.ToString()
            };
        }
    }

    internal static class SceneTypeRules
    {
        public static bool IsAuxiliary(SceneType type)
        {
            return type is SceneType.LaserDmx or SceneType.Strobe;
        }

        public static bool SupportsStripAssignment(SceneType type)
        {
            return !IsAuxiliary(type);
        }

        public static bool IsLaser(SceneType type)
        {
            return type == SceneType.LaserDmx;
        }

        public static bool IsStrobe(SceneType type)
        {
            return type == SceneType.Strobe;
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
                SceneType.SpectralAnalysisSegments => BuildSpectralAnalysisSegmentsSummary(scene.SpectralAnalysisSegments),
                SceneType.ImageRowCapture => BuildImageRowSummary(scene.ImageRowCapture),
                SceneType.SparkleAndFlash => BuildSparkleAndFlashSummary(scene.SparkleAndFlash),
                SceneType.LaserDmx => BuildLaserDmxSummary(scene.LaserDmx),
                SceneType.Strobe => $"{BuildTriggerSummary(scene.Strobe.Trigger)}, Auxiliary strobe output",
                SceneType.LedStrobe => BuildLedStrobeSummary(scene.LedStrobe),
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

        private static string BuildSpectralAnalysisSegmentsSummary(SpectralAnalysisSegmentsSceneConfig config)
        {
            config.EnsureSegmentDefaults();
            int total = config.Segments.Count;
            int enabled = config.Segments.Count(segment => segment.Enabled);
            if (total == 0)
            {
                return "0 segments";
            }

            double minHz = config.Segments.Min(segment => Math.Min(segment.FrequencyLowHz, segment.FrequencyHighHz));
            double maxHz = config.Segments.Max(segment => Math.Max(segment.FrequencyLowHz, segment.FrequencyHighHz));
            int holdMs = (int)Math.Round(config.Segments.Average(segment => Math.Max(1, segment.LightUpMs)));
            int fadeMs = (int)Math.Round(config.Segments.Average(segment => Math.Max(0, segment.FadeOutMs)));
            return $"{enabled}/{total} segments, {minHz:F0}-{maxHz:F0} Hz, hold {holdMs} ms, fade {fadeMs} ms";
        }

        private static string BuildSparkleAndFlashSummary(SparkleAndFlashSceneConfig config)
        {
            int segmentMin = Math.Max(1, Math.Min(config.SegmentSizeMin, config.SegmentSizeMax));
            int segmentMax = Math.Max(segmentMin, Math.Max(config.SegmentSizeMin, config.SegmentSizeMax));
            int intervalMin = Math.Max(1, Math.Min(config.SegmentIntervalMinMs, config.SegmentIntervalMaxMs));
            int intervalMax = Math.Max(intervalMin, Math.Max(config.SegmentIntervalMinMs, config.SegmentIntervalMaxMs));
            double hueMin = Math.Max(0, Math.Min(config.SparkleHueMin, config.SparkleHueMax));
            double hueMax = Math.Min(360, Math.Max(config.SparkleHueMin, config.SparkleHueMax));
            int hueIntervalMin = Math.Max(1, Math.Min(config.SparkleHueChangeIntervalMinMs, config.SparkleHueChangeIntervalMaxMs));
            int hueIntervalMax = Math.Max(hueIntervalMin, Math.Max(config.SparkleHueChangeIntervalMinMs, config.SparkleHueChangeIntervalMaxMs));
            string hueMode = config.ContinuousSparkleHueChange ? "continuous" : "random";
            string smooth = config.SmoothFadeAndBlur
                ? $", fade {Math.Max(0, config.FadeDurationMs)} ms, blur {Math.Max(0, config.BlurRadius)}"
                : ", hard on/off";
            string fullFlash = config.FullStripFlashEnabled
                ? $", full flash {Math.Max(1, config.FullStripFlashHoldMs)} ms" + (config.FullStripSmoothFade ? $", flash fade {Math.Max(0, config.FullStripFadeDurationMs)} ms" : ", hard flash")
                : ", no full flash";
            return $"Sparkles {segmentMin}-{segmentMax} LED every {intervalMin}-{intervalMax} ms, hue {hueMin:F0}-{hueMax:F0} {hueMode} {hueIntervalMin}-{hueIntervalMax} ms{smooth}{fullFlash}";
        }

        private static string BuildLedStrobeSummary(LedStrobeSceneConfig config)
        {
            string on = BuildStrobeDurationSummary(config.OnDurationMode, config.OnDurationMs, config.OnDurationMinMs, config.OnDurationMaxMs);
            string off = BuildStrobeDurationSummary(config.OffDurationMode, config.OffDurationMs, config.OffDurationMinMs, config.OffDurationMaxMs);
            string hue = BuildStrobeHueSummary(config);
            return $"On {on}, off {off}, hue {hue}, Sat {Math.Clamp(config.Saturation, 0, 100)}%, Bright {Math.Clamp(config.Brightness, 0, 100)}%";
        }

        private static string BuildStrobeDurationSummary(StrobeTimingMode mode, int constantMs, int minMs, int maxMs)
        {
            if (mode == StrobeTimingMode.RandomRange)
            {
                int min = Math.Max(1, Math.Min(minMs, maxMs));
                int max = Math.Max(min, Math.Max(minMs, maxMs));
                return min == max ? $"{min} ms" : $"{min}-{max} ms random";
            }

            return $"{Math.Max(1, constantMs)} ms";
        }

        private static string BuildStrobeHueSummary(LedStrobeSceneConfig config)
        {
            if (config.HueMode == StrobeHueMode.RandomRange)
            {
                double min = Math.Clamp(Math.Min(config.HueMin, config.HueMax), 0.0, 360.0);
                double max = Math.Clamp(Math.Max(config.HueMin, config.HueMax), min, 360.0);
                return Math.Abs(max - min) < 0.0001
                    ? $"{min:F0}"
                    : $"{min:F0}-{max:F0} random";
            }

            return $"{Math.Clamp(config.Hue, 0.0, 360.0):F0}";
        }

        private static string BuildLaserDmxSummary(LaserDmxSceneConfig config)
        {
            int refreshCount = config.Channels.Count(channel => channel.RefreshEnabled);
            string channelSummary = $"{config.Channels.Count} channels" + (refreshCount > 0 ? $", {refreshCount} refreshing" : string.Empty);
            string packetSummary = config.SendFullDmxPacket ? ", full DMX packet" : string.Empty;
            return $"{BuildTriggerSummary(config.Trigger)}, {channelSummary}{packetSummary}";
        }

        private static string BuildTriggerSummary(AuxiliaryTriggerConfig trigger)
        {
            string source = trigger.EventType switch
            {
                AuxiliaryTriggerEventType.Volume => $"Volume >= {Math.Clamp(trigger.Volume.ThresholdPercent, 0, 100)}%",
                AuxiliaryTriggerEventType.SpectralAnalysis => $"{Math.Min(trigger.SpectralAnalysis.FrequencyLowHz, trigger.SpectralAnalysis.FrequencyHighHz):F0}-{Math.Max(trigger.SpectralAnalysis.FrequencyLowHz, trigger.SpectralAnalysis.FrequencyHighHz):F0} Hz >= {trigger.SpectralAnalysis.ThresholdDb:F1} dB",
                AuxiliaryTriggerEventType.ScreenCapture => $"Screen {trigger.ScreenCapture.MonitorIndex + 1}, {Math.Max(1, trigger.ScreenCapture.Width)}x{Math.Max(1, trigger.ScreenCapture.Height)} >= {Math.Clamp(trigger.ScreenCapture.BrightnessThresholdPercent, 0, 100)}%",
                _ => trigger.EventType.ToString()
            };

            string mode = trigger.RetriggerMode switch
            {
                AuxiliaryTriggerRetriggerMode.OneShotUntilReset => "One-shot",
                AuxiliaryTriggerRetriggerMode.RepeatWhileHigh => "Repeat",
                AuxiliaryTriggerRetriggerMode.HoldWhileHigh => "Hold",
                _ => trigger.RetriggerMode.ToString()
            };

            return trigger.RetriggerMode == AuxiliaryTriggerRetriggerMode.HoldWhileHigh
                ? $"{source}, {mode}"
                : $"{source}, {mode}, {Math.Max(1, trigger.OnDurationMs)} ms";
        }
    }
}
