using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace Ledqualizer
{
    internal sealed class SolidColorSceneSettings
    {
        public double Hue { get; set; }
        public double MinHue { get; set; }
        public double MaxHue { get; set; } = 360;
        public int SaturationValue { get; set; }
        public int BrightnessValue { get; set; }
        public int Delay { get; set; }
    }

    internal sealed class GradientSceneSettings
    {
        public double HueMin { get; set; }
        public double HueMax { get; set; } = 360;
        public int SaturationValue { get; set; }
        public int BrightnessValue { get; set; }
        public int Delay { get; set; }
    }

    internal class AudioReactiveSceneSettings
    {
        public AcVolume.AudioCaptureVolumeMode Mode { get; set; }
        public int Delay { get; set; }
        public int BrightnessValue { get; set; }
        public int SaturationValue { get; set; } = 100;
        public int BrightnessMaximum { get; set; } = 100;
        public int NormalizationValue { get; set; }
        public bool Reverse { get; set; }
        public bool HueReverse { get; set; }
        public bool White { get; set; }
        public bool BackgroundWhite { get; set; }
        public int BackgroundBrightnessValue { get; set; }
        public int BackgroundSaturationValue { get; set; } = -1;
        public double BackgroundHue { get; set; }
        public double HueMin { get; set; }
        public double HueMax { get; set; }
    }

    internal sealed class VolumeSceneSettings : AudioReactiveSceneSettings
    {
    }

    internal sealed class SpectralSceneSettings : AudioReactiveSceneSettings
    {
        public double FrequencyLowHz { get; set; }
        public double FrequencyHighHz { get; set; }
        public double LevelLowDb { get; set; }
        public double LevelHighDb { get; set; }
    }

    internal sealed class ScreenCaptureSceneSettings
    {
        public int Delay { get; set; }
        public int MonitorIndex { get; set; }
        public int CaptureY { get; set; }
        public bool Reverse { get; set; }
    }

    internal sealed class OtherDevicesSceneSettings
    {
        public int Delay { get; set; }
        public int StrobeTriggerX { get; set; }
        public int StrobeTriggerY { get; set; }
        public int LaserTriggerX { get; set; }
        public int LaserTriggerY { get; set; }
        public int LaserPatternX { get; set; }
        public int LaserPatternY { get; set; }
        public int LaserColorX { get; set; }
        public int LaserColorY { get; set; }
    }

    internal sealed class DeviceSceneAssignment
    {
        public required SceneConfig Scene { get; init; }
        public int StartIndex { get; init; }
        public int LedCount { get; init; }
    }

    internal sealed class SolidColorSceneRunner : ISceneRunner
    {
        private readonly Func<SolidColorSceneSettings> settingsProvider;

        public SolidColorSceneRunner(Func<SolidColorSceneSettings> settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                SolidColorSceneSettings settings = settingsProvider();
                bool anySent = false;
                foreach (DeviceTarget device in devices)
                {
                    anySent |= await device.Session.SendFrameAsync(BuildFrame(device.Config.LedCount, settings), token).ConfigureAwait(false);
                }

                if (!anySent)
                {
                    return;
                }

                await Task.Delay(Math.Max(settings.Delay, 1), token).ConfigureAwait(false);
            }
        }

        private static byte[] BuildFrame(int ledCount, SolidColorSceneSettings settings)
        {
            byte[] ledConfigArray = new byte[ledCount * 3];
            double hue = Common.MapValue(settings.Hue, 0, 360, settings.MinHue, settings.MaxHue);
            double saturation = settings.SaturationValue / 100.0;
            double brightness = settings.BrightnessValue / 100.0;
            Color rgbColor = Common.HSVToRGB(hue, saturation, brightness);

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }

            return ledConfigArray;
        }
    }

    internal sealed class GradientSceneRunner : ISceneRunner
    {
        private readonly Func<GradientSceneSettings> settingsProvider;

        public GradientSceneRunner(Func<GradientSceneSettings> settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                GradientSceneSettings settings = settingsProvider();
                bool anySent = false;
                foreach (DeviceTarget device in devices)
                {
                    anySent |= await device.Session.SendFrameAsync(BuildFrame(device.Config.LedCount, settings), token).ConfigureAwait(false);
                }

                if (!anySent)
                {
                    return;
                }

                await Task.Delay(Math.Max(settings.Delay, 1), token).ConfigureAwait(false);
            }
        }

        private static byte[] BuildFrame(int ledCount, GradientSceneSettings settings)
        {
            byte[] ledConfigArray = new byte[ledCount * 3];
            double saturation = settings.SaturationValue / 100.0;
            double brightness = settings.BrightnessValue / 100.0;
            for (int i = 0; i < ledCount; i++)
            {
                double hue = Common.MapValue(i, 0, Math.Max(ledCount, 1), settings.HueMin, settings.HueMax);
                Color rgbColor = Common.HSVToRGB(hue, saturation, brightness);
                int idx = i * 3;
                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }

            return ledConfigArray;
        }
    }

    internal sealed class VolumeSceneRunner : ISceneRunner
    {
        private readonly Func<VolumeSceneSettings> settingsProvider;
        private readonly Func<string?> audioDeviceIdProvider;
        private readonly Action<int> progressReporter;
        private readonly Action<string> rateReporter;

        public VolumeSceneRunner(
            Func<VolumeSceneSettings> settingsProvider,
            Func<string?> audioDeviceIdProvider,
            Action<int> progressReporter,
            Action<string> rateReporter)
        {
            this.settingsProvider = settingsProvider;
            this.audioDeviceIdProvider = audioDeviceIdProvider;
            this.progressReporter = progressReporter;
            this.rateReporter = rateReporter;
        }

        public async Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token)
        {
            int iterations = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!token.IsCancellationRequested)
            {
                VolumeSceneSettings settings = settingsProvider();
                float volume = ReadVolume(audioDeviceIdProvider());
                progressReporter((int)Math.Round(volume * 100));

                bool anySent = false;
                foreach (DeviceTarget device in devices)
                {
                    anySent |= await device.Session.SendFrameAsync(AudioReactiveFrameBuilder.BuildFrame(device.Config.LedCount, volume, settings), token).ConfigureAwait(false);
                }

                if (!anySent)
                {
                    return;
                }

                iterations++;
                if (stopwatch.ElapsedMilliseconds >= 1000)
                {
                    rateReporter($"Rate: {iterations}");
                    iterations = 0;
                    stopwatch.Restart();
                }

                await Task.Delay(Math.Max(settings.Delay, 1), token).ConfigureAwait(false);
            }
        }

        internal static float ReadVolume(string? deviceId)
        {
            try
            {
                using MMDeviceEnumerator deviceEnumerator = new();
                using MMDevice audioDevice = ResolveAudioDevice(deviceEnumerator, deviceId);
                return audioDevice.AudioMeterInformation.MasterPeakValue;
            }
            catch
            {
                return 0.0f;
            }
        }

        private static MMDevice ResolveAudioDevice(MMDeviceEnumerator deviceEnumerator, string? deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }

            try
            {
                return deviceEnumerator.GetDevice(deviceId);
            }
            catch
            {
                return deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
        }
    }

    internal sealed class SpectralSceneRunner : ISceneRunner
    {
        private readonly Func<SpectralSceneSettings> settingsProvider;
        private readonly Func<string?> audioDeviceIdProvider;
        private readonly Action<int> progressReporter;
        private readonly Action<string> rateReporter;

        public SpectralSceneRunner(
            Func<SpectralSceneSettings> settingsProvider,
            Func<string?> audioDeviceIdProvider,
            Action<int> progressReporter,
            Action<string> rateReporter)
        {
            this.settingsProvider = settingsProvider;
            this.audioDeviceIdProvider = audioDeviceIdProvider;
            this.progressReporter = progressReporter;
            this.rateReporter = rateReporter;
        }

        public async Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token)
        {
            string? activeDeviceId = null;
            int iterations = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            using AcSpectralAnalysis spectralAnalysis = new();

            while (!token.IsCancellationRequested)
            {
                SpectralSceneSettings settings = settingsProvider();
                string? requestedDeviceId = audioDeviceIdProvider();
                if (!string.Equals(activeDeviceId, requestedDeviceId, StringComparison.Ordinal))
                {
                    spectralAnalysis.Start(requestedDeviceId);
                    activeDeviceId = requestedDeviceId;
                }

                spectralAnalysis.UpdateBandSettings(new SpectralBandSettings(
                    settings.FrequencyLowHz,
                    settings.FrequencyHighHz,
                    settings.LevelLowDb,
                    settings.LevelHighDb));

                float strength = spectralAnalysis.GetCurrentStrength();
                progressReporter((int)Math.Round(strength * 100.0f));

                bool anySent = false;
                foreach (DeviceTarget device in devices)
                {
                    anySent |= await device.Session.SendFrameAsync(AudioReactiveFrameBuilder.BuildFrame(device.Config.LedCount, strength, settings), token).ConfigureAwait(false);
                }

                if (!anySent)
                {
                    return;
                }

                iterations++;
                if (stopwatch.ElapsedMilliseconds >= 1000)
                {
                    rateReporter($"Rate: {iterations} | Band: {spectralAnalysis.GetCurrentBandLevelDb():F1} dB");
                    iterations = 0;
                    stopwatch.Restart();
                }

                await Task.Delay(Math.Max(settings.Delay, 1), token).ConfigureAwait(false);
            }
        }
    }

    internal static class AudioReactiveFrameBuilder
    {
        public static byte[] BuildFrame(int ledCount, float inputLevel, AudioReactiveSceneSettings settings)
        {
            byte[] ledConfigArray = new byte[ledCount * 3];
            float normalizedLevel = Math.Max(0.0f, (settings.NormalizationValue / 10.0f) * inputLevel);

            switch (settings.Mode)
            {
                case AcVolume.AudioCaptureVolumeMode.ModeEndToStart:
                    ComputeColorsEndToStart(ledConfigArray, ledCount, normalizedLevel, settings);
                    break;
                case AcVolume.AudioCaptureVolumeMode.ModeMidToOut:
                    ComputeColorsMidToOut(ledConfigArray, ledCount, normalizedLevel, settings);
                    break;
                case AcVolume.AudioCaptureVolumeMode.ModeColorPush:
                    ComputeColorsColorPush(ledConfigArray, ledCount, normalizedLevel, settings);
                    break;
                case AcVolume.AudioCaptureVolumeMode.ModeMidToOut_Point:
                    ComputeColorsMidToOutPoint(ledConfigArray, ledCount, normalizedLevel, settings);
                    break;
                case AcVolume.AudioCaptureVolumeMode.ModeBrightness:
                    ComputeColorsBrightness(ledConfigArray, ledCount, normalizedLevel, settings);
                    break;
                default:
                    ComputeColorsStartToEnd(ledConfigArray, ledCount, normalizedLevel, settings);
                    break;
            }

            if (settings.Reverse)
            {
                Array.Reverse(ledConfigArray);
            }

            return ledConfigArray;
        }

        private static void ComputeColorsStartToEnd(byte[] ledConfigArray, int ledCount, float vol, AudioReactiveSceneSettings settings)
        {
            double saturation = GetActiveSaturation(settings);
            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                if (i < Math.Round(ledCount * vol))
                {
                    double hue = 360 * ((float)i / ledCount);
                    hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, saturation, settings.BrightnessValue / (double)settings.BrightnessMaximum);
                    ledConfigArray[idx] = rgbColor.R;
                    ledConfigArray[idx + 1] = rgbColor.G;
                    ledConfigArray[idx + 2] = rgbColor.B;
                }
                else
                {
                    ApplyBackground(ledConfigArray, idx, settings);
                }
            }
        }

        private static void ComputeColorsEndToStart(byte[] ledConfigArray, int ledCount, float vol, AudioReactiveSceneSettings settings)
        {
            double saturation = GetActiveSaturation(settings);
            for (int i = ledCount - 1; i >= 0; i--)
            {
                int idx = i * 3;
                if (i > Math.Round(ledCount * (1 - vol)))
                {
                    double hue = 360 - (360 * ((float)i / ledCount));
                    hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, saturation, settings.BrightnessValue / (double)settings.BrightnessMaximum);
                    ledConfigArray[idx] = rgbColor.R;
                    ledConfigArray[idx + 1] = rgbColor.G;
                    ledConfigArray[idx + 2] = rgbColor.B;
                }
                else
                {
                    ApplyBackground(ledConfigArray, idx, settings);
                }
            }
        }

        private static void ComputeColorsMidToOut(byte[] ledConfigArray, int ledCount, float vol, AudioReactiveSceneSettings settings)
        {
            int center = ledCount / 2;
            double saturation = GetActiveSaturation(settings);
            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = center == 0 ? 0 : (float)distance / center;
                if (vol > distanceFactor)
                {
                    double hue = 360 * (settings.HueReverse ? 1 - distanceFactor : distanceFactor);
                    hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, saturation, settings.BrightnessValue / (double)settings.BrightnessMaximum);
                    ledConfigArray[idx] = rgbColor.R;
                    ledConfigArray[idx + 1] = rgbColor.G;
                    ledConfigArray[idx + 2] = rgbColor.B;
                }
                else
                {
                    ApplyBackground(ledConfigArray, idx, settings);
                }
            }
        }

        private static void ComputeColorsMidToOutPoint(byte[] ledConfigArray, int ledCount, float vol, AudioReactiveSceneSettings settings)
        {
            int center = ledCount / 2;
            const int pointSize = 10;
            double saturation = GetActiveSaturation(settings);
            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = center == 0 ? 0 : (float)distance / center;
                if (Math.Round(vol * center) > distance - pointSize && Math.Round(vol * center) < distance + pointSize)
                {
                    double hue = 360 * (settings.HueReverse ? 1 - distanceFactor : distanceFactor);
                    hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, saturation, settings.BrightnessValue / (double)settings.BrightnessMaximum);
                    ledConfigArray[idx] = rgbColor.R;
                    ledConfigArray[idx + 1] = rgbColor.G;
                    ledConfigArray[idx + 2] = rgbColor.B;
                }
                else
                {
                    ApplyBackground(ledConfigArray, idx, settings);
                }
            }
        }

        private static void ComputeColorsColorPush(byte[] ledConfigArray, int ledCount, float vol, AudioReactiveSceneSettings settings)
        {
            int center = ledCount / 2;
            double saturation = GetActiveSaturation(settings);
            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = center == 0 ? 0 : (float)distance / center;
                float adjustedVol = vol * (1.0f - distanceFactor);
                Color rgbColor = Common.HSVToRGB(360 * adjustedVol, saturation, settings.BrightnessValue / (double)settings.BrightnessMaximum);
                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }
        }

        private static void ComputeColorsBrightness(byte[] ledConfigArray, int ledCount, float vol, AudioReactiveSceneSettings settings)
        {
            int center = ledCount / 2;
            double saturation = GetActiveSaturation(settings);
            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = center == 0 ? 0 : (float)distance / center;
                float adjustedVol = vol * (1.0f - distanceFactor);
                double hue = 360 * (settings.HueReverse ? 1 - distanceFactor : distanceFactor);
                hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                Color rgbColor = Common.HSVToRGB(hue, saturation, adjustedVol * (settings.BrightnessValue / (double)settings.BrightnessMaximum));
                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }
        }

        private static void ApplyBackground(byte[] ledConfigArray, int idx, AudioReactiveSceneSettings settings)
        {
            double saturation = settings.BackgroundWhite
                ? 0
                : (settings.BackgroundSaturationValue >= 0 ? settings.BackgroundSaturationValue : settings.SaturationValue) / 100.0;
            double brightness = settings.BackgroundBrightnessValue / 100.0;
            Color bgColor = Common.HSVToRGB(settings.BackgroundHue, saturation, brightness);
            ledConfigArray[idx] = bgColor.R;
            ledConfigArray[idx + 1] = bgColor.G;
            ledConfigArray[idx + 2] = bgColor.B;
        }

        private static double GetActiveSaturation(AudioReactiveSceneSettings settings)
        {
            return settings.White ? 0 : settings.SaturationValue / 100.0;
        }
    }

    internal static class PixelFrameHelpers
    {
        public static List<Color> ReducePixels(IReadOnlyList<Color> pixelColors, int ledCount)
        {
            var reducedPixelColors = new List<Color>(Math.Max(ledCount, 0));
            if (ledCount <= 0)
            {
                return reducedPixelColors;
            }

            if (pixelColors.Count == 0)
            {
                return CreateBlackPixels(ledCount);
            }

            for (int i = 0; i < ledCount; i++)
            {
                double start = i * pixelColors.Count / (double)ledCount;
                double end = (i + 1) * pixelColors.Count / (double)ledCount;
                int startIndex = Math.Clamp((int)Math.Floor(start), 0, pixelColors.Count - 1);
                int endExclusive = Math.Clamp((int)Math.Ceiling(end), startIndex + 1, pixelColors.Count);
                reducedPixelColors.Add(CalculateAverageColor(pixelColors, startIndex, endExclusive));
            }

            return reducedPixelColors;
        }

        public static List<Color> CreateBlackPixels(int ledCount)
        {
            var colors = new List<Color>(Math.Max(ledCount, 0));
            for (int i = 0; i < ledCount; i++)
            {
                colors.Add(Color.Black);
            }

            return colors;
        }

        public static byte[] ColorListToByteArray(IReadOnlyList<Color> colorList)
        {
            byte[] bytes = new byte[colorList.Count * 3];
            for (int i = 0; i < colorList.Count; i++)
            {
                bytes[i * 3] = colorList[i].R;
                bytes[i * 3 + 1] = colorList[i].G;
                bytes[i * 3 + 2] = colorList[i].B;
            }

            return bytes;
        }

        public static List<Color> GetBitmapRowColors(Bitmap bitmap, int rowIndex)
        {
            int y = Math.Clamp(rowIndex, 0, Math.Max(bitmap.Height - 1, 0));
            var colors = new List<Color>(bitmap.Width);
            for (int x = 0; x < bitmap.Width; x++)
            {
                colors.Add(bitmap.GetPixel(x, y));
            }

            return colors;
        }

        public static List<Color> GetBitmapColumnColors(Bitmap bitmap, int columnIndex)
        {
            int x = Math.Clamp(columnIndex, 0, Math.Max(bitmap.Width - 1, 0));
            var colors = new List<Color>(bitmap.Height);
            for (int y = 0; y < bitmap.Height; y++)
            {
                colors.Add(bitmap.GetPixel(x, y));
            }

            return colors;
        }

        private static Color CalculateAverageColor(IReadOnlyList<Color> colors, int startIndex, int endExclusive)
        {
            int totalRed = 0;
            int totalGreen = 0;
            int totalBlue = 0;
            int count = 0;

            for (int i = startIndex; i < endExclusive; i++)
            {
                Color color = colors[i];
                totalRed += color.R;
                totalGreen += color.G;
                totalBlue += color.B;
                count++;
            }

            return count == 0
                ? Color.Black
                : Color.FromArgb(totalRed / count, totalGreen / count, totalBlue / count);
        }
    }

    internal sealed class ScreenCaptureSceneRunner : ISceneRunner
    {
        private readonly Func<ScreenCaptureSceneSettings> settingsProvider;
        private readonly Action<IReadOnlyList<Color>> previewUpdater;

        public ScreenCaptureSceneRunner(Func<ScreenCaptureSceneSettings> settingsProvider, Action<IReadOnlyList<Color>> previewUpdater)
        {
            this.settingsProvider = settingsProvider;
            this.previewUpdater = previewUpdater;
        }

        public async Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                ScreenCaptureSceneSettings settings = settingsProvider();
                Screen targetScreen = ResolveScreen(settings.MonitorIndex);
                Rectangle bounds = targetScreen.Bounds;
                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    return;
                }

                int captureY = Math.Max(0, Math.Min(bounds.Height - 1, settings.CaptureY));

                using Bitmap screenCapture = new(bounds.Width, 1);
                using Graphics graphics = Graphics.FromImage(screenCapture);
                graphics.CopyFromScreen(bounds.Left, bounds.Top + captureY, 0, 0, new Size(bounds.Width, 1));

                List<Color> pixelColors = PixelFrameHelpers.GetBitmapRowColors(screenCapture, 0);

                if (settings.Reverse)
                {
                    pixelColors.Reverse();
                }

                bool previewUpdated = false;
                bool anySent = false;
                foreach (DeviceTarget device in devices)
                {
                    List<Color> reducedColors = PixelFrameHelpers.ReducePixels(pixelColors, device.Config.LedCount);
                    anySent |= await device.Session.SendFrameAsync(PixelFrameHelpers.ColorListToByteArray(reducedColors), token).ConfigureAwait(false);

                    if (!previewUpdated)
                    {
                        previewUpdater(reducedColors);
                        previewUpdated = true;
                    }
                }

                if (!anySent)
                {
                    return;
                }

                await Task.Delay(Math.Max(settings.Delay, 1), token).ConfigureAwait(false);
            }
        }

        private static Screen ResolveScreen(int monitorIndex)
        {
            Screen[] screens = Screen.AllScreens;
            if (screens.Length == 0)
            {
                return Screen.PrimaryScreen ?? throw new InvalidOperationException("No screens are available.");
            }

            if (monitorIndex >= 0 && monitorIndex < screens.Length)
            {
                return screens[monitorIndex];
            }

            return Screen.PrimaryScreen ?? screens[0];
        }

        private static List<Color> ReducePixels(List<Color> pixelColors, int ledCount, int screenWidth)
        {
            var reducedPixelColors = new List<Color>(ledCount);
            int segmentSize = Math.Max(screenWidth / Math.Max(ledCount, 1), 1);
            for (int i = 0; i < ledCount; i++)
            {
                int startIndex = i * segmentSize;
                if (startIndex >= pixelColors.Count)
                {
                    reducedPixelColors.Add(Color.Black);
                    continue;
                }

                int endIndex = Math.Min((i + 1) * segmentSize, pixelColors.Count) - 1;
                reducedPixelColors.Add(CalculateAverageColor(pixelColors.GetRange(startIndex, endIndex - startIndex + 1)));
            }

            return reducedPixelColors;
        }

        private static Color CalculateAverageColor(List<Color> colors)
        {
            int totalRed = 0;
            int totalGreen = 0;
            int totalBlue = 0;
            foreach (Color color in colors)
            {
                totalRed += color.R;
                totalGreen += color.G;
                totalBlue += color.B;
            }

            return Color.FromArgb(totalRed / colors.Count, totalGreen / colors.Count, totalBlue / colors.Count);
        }

        private static byte[] ColorListToByteArray(List<Color> colorList)
        {
            byte[] bytes = new byte[colorList.Count * 3];
            for (int i = 0; i < colorList.Count; i++)
            {
                bytes[i * 3] = colorList[i].R;
                bytes[i * 3 + 1] = colorList[i].G;
                bytes[i * 3 + 2] = colorList[i].B;
            }

            return bytes;
        }
    }

    internal sealed class OtherDevicesSceneRunner : ISceneRunner
    {
        private const int TriggerThreshold = 100;
        private readonly Func<OtherDevicesSceneSettings> settingsProvider;

        public OtherDevicesSceneRunner(Func<OtherDevicesSceneSettings> settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token)
        {
            using Bitmap screenCapture = new(1, 1);
            using Graphics graphics = Graphics.FromImage(screenCapture);

            while (!token.IsCancellationRequested)
            {
                byte[] data = BuildFrame(screenCapture, graphics, settingsProvider());
                bool anySent = false;
                foreach (DeviceTarget device in devices)
                {
                    anySent |= await device.Session.SendFrameAsync(data, token).ConfigureAwait(false);
                }

                if (!anySent)
                {
                    return;
                }

                await Task.Delay(Math.Max(settingsProvider().Delay, 1), token).ConfigureAwait(false);
            }
        }

        private static byte[] BuildFrame(Bitmap screenCapture, Graphics graphics, OtherDevicesSceneSettings settings)
        {
            byte[] data = new byte[7];

            Color pixelColor = GetPixelColor(screenCapture, graphics, settings.LaserTriggerX, settings.LaserTriggerY);
            data[0] = (byte)(pixelColor.R + pixelColor.G + pixelColor.B > TriggerThreshold ? 1 : 0);
            data[1] = 0;
            data[2] = 240;

            pixelColor = GetPixelColor(screenCapture, graphics, settings.LaserPatternX, settings.LaserPatternY);
            data[3] = (byte)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);

            GetPixelColor(screenCapture, graphics, settings.LaserColorX, settings.LaserColorY);
            data[4] = 0;
            data[5] = 0;

            pixelColor = GetPixelColor(screenCapture, graphics, settings.StrobeTriggerX, settings.StrobeTriggerY);
            data[6] = (byte)(pixelColor.R + pixelColor.G + pixelColor.B > TriggerThreshold ? 1 : 0);

            return data;
        }

        private static Color GetPixelColor(Bitmap screenCapture, Graphics graphics, int x, int y)
        {
            graphics.CopyFromScreen(x, y, 0, 0, new Size(1, 1));
            return screenCapture.GetPixel(0, 0);
        }
    }

    internal sealed class LaserDmxRuntimeState
    {
        private readonly Dictionary<int, int> listIndexes = new();
        private readonly Dictionary<int, int> currentValues = new();

        public Dictionary<int, DateTime> NextRefreshByChannel { get; } = new();
        public string Signature { get; set; } = string.Empty;

        public int GetOrResolveValue(LaserDmxChannelRow row, bool forceRefresh)
        {
            if (!forceRefresh && currentValues.TryGetValue(row.Channel, out int storedValue))
            {
                return storedValue;
            }

            int value = row.Mode switch
            {
                LaserDmxValueMode.RandomRange => ResolveRandomRange(row),
                LaserDmxValueMode.ValueList => ResolveSequentialList(row),
                LaserDmxValueMode.RandomValueFromList => ResolveRandomList(row),
                _ => Math.Clamp(row.ConstantValue, 0, 255)
            };
            currentValues[row.Channel] = value;
            return value;
        }

        public void Reset()
        {
            listIndexes.Clear();
            currentValues.Clear();
            NextRefreshByChannel.Clear();
            Signature = string.Empty;
        }

        private static int ResolveRandomRange(LaserDmxChannelRow row)
        {
            int min = Math.Clamp(Math.Min(row.RangeMin, row.RangeMax), 0, 255);
            int max = Math.Clamp(Math.Max(row.RangeMin, row.RangeMax), 0, 255);
            return Random.Shared.Next(min, max + 1);
        }

        private int ResolveSequentialList(LaserDmxChannelRow row)
        {
            List<int> values = NormalizeValues(row.Values);
            if (values.Count == 0)
            {
                return 0;
            }

            int index = listIndexes.TryGetValue(row.Channel, out int currentIndex) ? currentIndex : 0;
            int resolved = values[index % values.Count];
            listIndexes[row.Channel] = (index + 1) % values.Count;
            return resolved;
        }

        private static int ResolveRandomList(LaserDmxChannelRow row)
        {
            List<int> values = NormalizeValues(row.Values);
            return values.Count == 0 ? 0 : values[Random.Shared.Next(values.Count)];
        }

        private static List<int> NormalizeValues(List<int> values)
        {
            return values.Select(value => Math.Clamp(value, 0, 255)).ToList();
        }
    }

    internal sealed class AuxiliaryTriggerRuntimeState
    {
        public bool LastConditionHigh { get; set; }
        public bool Armed { get; set; } = true;
        public bool OutputActive { get; set; }
        public DateTime OnUntilUtc { get; set; } = DateTime.MinValue;

        public void Reset()
        {
            LastConditionHigh = false;
            Armed = true;
            OutputActive = false;
            OnUntilUtc = DateTime.MinValue;
        }
    }

    internal readonly record struct AuxiliaryTriggerEvaluation(bool Triggered, bool StateChanged, bool OutputActive);

    internal readonly record struct AuxiliaryDmxTransmissionOptions(bool SendFullPacket, int RequestedChannelCount);

    internal static class AuxiliaryPayloadBuilder
    {
        private static readonly byte[] Magic = { (byte)'V', (byte)'A', (byte)'U', (byte)'X' };
        private const byte LegacyVersion = 1;
        private const byte Version = 2;
        private const byte StrobeEnabledFlag = 0x01;
        private const byte SendFullPacketFlag = 0x02;
        private const int LegacyHeaderLength = 7;
        private const int HeaderLength = 9;
        private const int MaxDmxChannelCount = 512;
        public const int DefaultCompactChannelCount = 6;

        public static AuxiliaryDmxTransmissionOptions DefaultTransmissionOptions { get; } =
            new(false, DefaultCompactChannelCount);

        public static byte[] BuildLaserPayload(
            LaserDmxSceneConfig config,
            LaserDmxRuntimeState state,
            bool refreshAll,
            ISet<int>? refreshChannels = null)
        {
            return BuildLaserPayload(config, state, refreshAll, refreshChannels, false);
        }

        public static byte[] BuildLaserPayload(
            LaserDmxSceneConfig config,
            LaserDmxRuntimeState state,
            bool refreshAll,
            ISet<int>? refreshChannels,
            bool strobeEnabled)
        {
            string signature = BuildLaserSignature(config);
            if (!string.Equals(signature, state.Signature, StringComparison.Ordinal))
            {
                state.Reset();
                state.Signature = signature;
                refreshAll = true;
            }

            List<(int Channel, int Value)> resolvedChannels = config.Channels
                .Select(channel =>
                {
                    bool shouldRefresh = refreshAll
                        || refreshChannels == null
                        || refreshChannels.Contains(channel.Channel);
                    return (Channel: Math.Clamp(channel.Channel, 1, 512), Value: state.GetOrResolveValue(channel, shouldRefresh));
                })
                .OrderBy(item => item.Channel)
                .ToList();

            return BuildPayload(resolvedChannels, strobeEnabled, GetTransmissionOptions(config));
        }

        public static byte[] BuildStrobePayload(bool enabled)
        {
            return BuildStrobePayload(enabled, DefaultTransmissionOptions);
        }

        public static byte[] BuildStrobePayload(bool enabled, AuxiliaryDmxTransmissionOptions transmissionOptions)
        {
            return BuildPayload(Array.Empty<(int Channel, int Value)>(), enabled, transmissionOptions);
        }

        public static byte[] BuildExplicitPayload(IReadOnlyList<(int Channel, int Value)> channels, bool strobeEnabled)
        {
            return BuildExplicitPayload(channels, strobeEnabled, DefaultTransmissionOptions);
        }

        public static byte[] BuildExplicitPayload(
            IReadOnlyList<(int Channel, int Value)> channels,
            bool strobeEnabled,
            AuxiliaryDmxTransmissionOptions transmissionOptions)
        {
            return BuildPayload(channels, strobeEnabled, transmissionOptions);
        }

        public static byte[] BuildOffPayload()
        {
            return BuildOffPayload(DefaultTransmissionOptions);
        }

        public static byte[] BuildOffPayload(AuxiliaryDmxTransmissionOptions transmissionOptions)
        {
            return BuildPayload(Array.Empty<(int Channel, int Value)>(), false, transmissionOptions);
        }

        public static AuxiliaryDmxTransmissionOptions GetTransmissionOptions(LaserDmxSceneConfig? config)
        {
            if (config == null)
            {
                return DefaultTransmissionOptions;
            }

            if (config.SendFullDmxPacket)
            {
                return new AuxiliaryDmxTransmissionOptions(true, MaxDmxChannelCount);
            }

            int highestConfiguredChannel = config.Channels.Count == 0
                ? 0
                : config.Channels.Max(channel => Math.Clamp(channel.Channel, 1, MaxDmxChannelCount));
            return new AuxiliaryDmxTransmissionOptions(
                false,
                Math.Max(DefaultCompactChannelCount, highestConfiguredChannel));
        }

        public static bool TryParsePayload(byte[] payload, out List<(int Channel, int Value)> channels, out bool strobeEnabled)
        {
            return TryParsePayload(payload, out channels, out strobeEnabled, out _);
        }

        public static bool TryParsePayload(
            byte[] payload,
            out List<(int Channel, int Value)> channels,
            out bool strobeEnabled,
            out AuxiliaryDmxTransmissionOptions transmissionOptions)
        {
            channels = new List<(int Channel, int Value)>();
            strobeEnabled = false;
            transmissionOptions = DefaultTransmissionOptions;
            if (payload.Length < LegacyHeaderLength
                || payload[0] != Magic[0]
                || payload[1] != Magic[1]
                || payload[2] != Magic[2]
                || payload[3] != Magic[3])
            {
                return false;
            }

            byte version = payload[4];
            int headerLength;
            int channelCount;
            if (version == LegacyVersion)
            {
                headerLength = LegacyHeaderLength;
                strobeEnabled = payload[5] != 0;
                channelCount = payload[6];
                transmissionOptions = new AuxiliaryDmxTransmissionOptions(true, MaxDmxChannelCount);
            }
            else if (version == Version)
            {
                if (payload.Length < HeaderLength)
                {
                    return false;
                }

                headerLength = HeaderLength;
                byte flags = payload[5];
                strobeEnabled = (flags & StrobeEnabledFlag) != 0;
                channelCount = payload[6];
                int requestedChannelCount = payload[7] | (payload[8] << 8);
                transmissionOptions = new AuxiliaryDmxTransmissionOptions(
                    (flags & SendFullPacketFlag) != 0,
                    requestedChannelCount);
            }
            else
            {
                return false;
            }

            int expectedLength = headerLength + (channelCount * 3);
            if (payload.Length < expectedLength)
            {
                return false;
            }

            for (int i = 0; i < channelCount; i++)
            {
                int offset = headerLength + (i * 3);
                int channel = payload[offset] | (payload[offset + 1] << 8);
                int value = payload[offset + 2];
                channels.Add((channel, value));
            }

            int highestPairChannel = channels.Count == 0
                ? 0
                : channels.Max(channel => Math.Clamp(channel.Channel, 1, MaxDmxChannelCount));
            transmissionOptions = NormalizeTransmissionOptions(transmissionOptions, highestPairChannel);
            return true;
        }

        private static byte[] BuildPayload(
            IReadOnlyList<(int Channel, int Value)> channels,
            bool strobeEnabled,
            AuxiliaryDmxTransmissionOptions transmissionOptions)
        {
            List<(int Channel, int Value)> normalizedChannels = channels
                .Take(byte.MaxValue)
                .Select(channel => (
                    Channel: Math.Clamp(channel.Channel, 1, MaxDmxChannelCount),
                    Value: Math.Clamp(channel.Value, 0, 255)))
                .ToList();
            int highestPairChannel = normalizedChannels.Count == 0
                ? 0
                : normalizedChannels.Max(channel => channel.Channel);
            transmissionOptions = NormalizeTransmissionOptions(transmissionOptions, highestPairChannel);

            int channelCount = normalizedChannels.Count;
            byte[] payload = new byte[HeaderLength + channelCount * 3];
            Array.Copy(Magic, payload, Magic.Length);
            payload[4] = Version;
            payload[5] = (byte)((strobeEnabled ? StrobeEnabledFlag : 0)
                | (transmissionOptions.SendFullPacket ? SendFullPacketFlag : 0));
            payload[6] = (byte)channelCount;
            payload[7] = (byte)(transmissionOptions.RequestedChannelCount & 0xFF);
            payload[8] = (byte)((transmissionOptions.RequestedChannelCount >> 8) & 0xFF);

            for (int i = 0; i < channelCount; i++)
            {
                int offset = HeaderLength + i * 3;
                payload[offset] = (byte)(normalizedChannels[i].Channel & 0xFF);
                payload[offset + 1] = (byte)((normalizedChannels[i].Channel >> 8) & 0xFF);
                payload[offset + 2] = (byte)normalizedChannels[i].Value;
            }

            return payload;
        }

        private static AuxiliaryDmxTransmissionOptions NormalizeTransmissionOptions(
            AuxiliaryDmxTransmissionOptions transmissionOptions,
            int highestPairChannel)
        {
            if (transmissionOptions.SendFullPacket)
            {
                return new AuxiliaryDmxTransmissionOptions(true, MaxDmxChannelCount);
            }

            int requestedChannelCount = Math.Clamp(transmissionOptions.RequestedChannelCount, 1, MaxDmxChannelCount);
            requestedChannelCount = Math.Max(requestedChannelCount, DefaultCompactChannelCount);
            requestedChannelCount = Math.Max(requestedChannelCount, Math.Clamp(highestPairChannel, 0, MaxDmxChannelCount));
            return new AuxiliaryDmxTransmissionOptions(false, requestedChannelCount);
        }

        private static string BuildLaserSignature(LaserDmxSceneConfig config)
        {
            string channelSignature = string.Join("|", config.Channels.Select(channel =>
                $"{channel.Channel}:{channel.Mode}:{channel.ConstantValue}:{channel.RangeMin}:{channel.RangeMax}:{string.Join(",", channel.Values)}:{channel.RefreshEnabled}:{channel.RefreshIntervalSeconds:F3}"));
            return $"{config.SendFullDmxPacket}:{channelSignature}";
        }
    }

    internal static class AuxiliaryRuntimeRegistry
    {
        private sealed class Snapshot
        {
            public byte[] Payload { get; init; } = Array.Empty<byte>();
            public bool LaserActive { get; init; }
            public bool StrobeActive { get; init; }
        }

        private static readonly object SyncRoot = new();
        private static readonly Dictionary<string, Snapshot> Snapshots = new(StringComparer.Ordinal);

        public static void Update(string deviceId, byte[] payload, bool laserActive, bool strobeActive)
        {
            lock (SyncRoot)
            {
                Snapshots[deviceId] = new Snapshot
                {
                    Payload = payload.ToArray(),
                    LaserActive = laserActive,
                    StrobeActive = strobeActive
                };
            }
        }

        public static void Clear(string deviceId)
        {
            lock (SyncRoot)
            {
                Snapshots.Remove(deviceId);
            }
        }

        public static bool TryGet(string deviceId, out byte[] payload, out bool laserActive, out bool strobeActive)
        {
            lock (SyncRoot)
            {
                if (Snapshots.TryGetValue(deviceId, out Snapshot? snapshot))
                {
                    payload = snapshot.Payload.ToArray();
                    laserActive = snapshot.LaserActive;
                    strobeActive = snapshot.StrobeActive;
                    return true;
                }
            }

            payload = Array.Empty<byte>();
            laserActive = false;
            strobeActive = false;
            return false;
        }
    }

    internal sealed class CompositeSceneRunner : ISceneRunner
    {
        private sealed class ImageScenePlaybackState
        {
            public object SyncRoot { get; } = new();
            public string ConfigSignature { get; set; } = string.Empty;
            public List<string> Files { get; set; } = new();
            public int CurrentFileIndex { get; set; }
            public string? CurrentFilePath { get; set; }
            public Bitmap? CurrentBitmap { get; set; }
            public double ScanPosition { get; set; }
            public ImageScanDirection ActiveDirection { get; set; } = ImageScanDirection.TopToBottom;
            public double ActiveSpeed { get; set; } = 1.0;
            public bool IsStopped { get; set; }
            public int LastAppliedSeekRevision { get; set; }
            public DateTime LastAdvanceUtc { get; set; } = DateTime.MinValue;

            public void DisposeBitmap()
            {
                CurrentBitmap?.Dispose();
                CurrentBitmap = null;
                CurrentFilePath = null;
            }
        }

        private readonly Func<IReadOnlyList<DeviceSceneAssignment>> assignmentsProvider;
        private readonly Func<SceneConfig?> laserSceneProvider;
        private readonly Func<SceneConfig?> strobeSceneProvider;
        private readonly Func<string?> audioDeviceIdProvider;
        private readonly Func<int> delayProvider;
        private readonly Action<CaptureScenePreview> previewUpdater;
        private readonly Action<int> volumeProgressReporter;
        private readonly Action<int> spectralProgressReporter;
        private readonly Action<string> rateReporter;
        private readonly LaserDmxRuntimeState auxiliaryLaserRuntimeState = new();
        private readonly AuxiliaryTriggerRuntimeState laserTriggerState = new();
        private readonly AuxiliaryTriggerRuntimeState strobeTriggerState = new();
        private readonly Dictionary<string, AcSpectralAnalysis> auxiliarySpectralMeters = new(StringComparer.Ordinal);
        private double? latestAuxiliarySpectralDb;
        private bool auxiliaryWasActive;
        private bool laserOutputActive;
        private bool strobeOutputActive;
        private string? lastLaserSceneId;
        private string? lastStrobeSceneId;
        private AuxiliaryDmxTransmissionOptions lastAuxiliaryTransmissionOptions = AuxiliaryPayloadBuilder.DefaultTransmissionOptions;
        private readonly HashSet<string> registeredImageSceneIds = new(StringComparer.Ordinal);
        private static readonly object SharedImageSceneStatesLock = new();
        private static readonly Dictionary<string, ImageScenePlaybackState> SharedImageSceneStates = new(StringComparer.Ordinal);
        private static readonly Dictionary<string, int> SharedImageSceneUsageCounts = new(StringComparer.Ordinal);

        public CompositeSceneRunner(
            Func<IReadOnlyList<DeviceSceneAssignment>> assignmentsProvider,
            Func<SceneConfig?> laserSceneProvider,
            Func<SceneConfig?> strobeSceneProvider,
            Func<string?> audioDeviceIdProvider,
            Func<int> delayProvider,
            Action<CaptureScenePreview> previewUpdater,
            Action<int> volumeProgressReporter,
            Action<int> spectralProgressReporter,
            Action<string> rateReporter)
        {
            this.assignmentsProvider = assignmentsProvider;
            this.laserSceneProvider = laserSceneProvider;
            this.strobeSceneProvider = strobeSceneProvider;
            this.audioDeviceIdProvider = audioDeviceIdProvider;
            this.delayProvider = delayProvider;
            this.previewUpdater = previewUpdater;
            this.volumeProgressReporter = volumeProgressReporter;
            this.spectralProgressReporter = spectralProgressReporter;
            this.rateReporter = rateReporter;
        }

        public async Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token)
        {
            if (devices.Count == 0)
            {
                return;
            }

            string? activeAudioDeviceId = null;
            int iterations = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            using AcSpectralAnalysis spectralAnalysis = new();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    IReadOnlyList<DeviceSceneAssignment> assignments = assignmentsProvider();
                    SceneConfig? laserScene = laserSceneProvider();
                    SceneConfig? strobeScene = strobeSceneProvider();
                    bool hasAuxiliaryScene = laserScene != null || strobeScene != null;
                    if (assignments.Count == 0 && !hasAuxiliaryScene)
                    {
                        return;
                    }

                    CleanupImageSceneStates(assignments);

                    bool needsSpectral = assignments.Any(assignment => assignment.Scene.Type == SceneType.SpectralAnalysis)
                        || laserScene?.LaserDmx.Trigger.EventType == AuxiliaryTriggerEventType.SpectralAnalysis
                        || strobeScene?.Strobe.Trigger.EventType == AuxiliaryTriggerEventType.SpectralAnalysis;
                    if (needsSpectral)
                    {
                        string? requestedDeviceId = audioDeviceIdProvider();
                        if (!string.Equals(activeAudioDeviceId, requestedDeviceId, StringComparison.Ordinal))
                        {
                            spectralAnalysis.Start(requestedDeviceId);
                            activeAudioDeviceId = requestedDeviceId;
                        }
                    }

                    float? sharedVolume = null;
                    double? lastSpectralDb = null;

                    for (int deviceIndex = 0; deviceIndex < devices.Count; deviceIndex++)
                    {
                        DeviceTarget device = devices[deviceIndex];
                        byte[] composedFrame = new byte[device.Config.LedCount * 3];
                        Action<CaptureScenePreview>? devicePreviewUpdater = deviceIndex == 0 ? previewUpdater : null;

                        foreach (DeviceSceneAssignment assignment in assignments)
                        {
                            byte[] segmentFrame = BuildAssignmentFrame(
                                assignment,
                                audioDeviceIdProvider,
                                ref sharedVolume,
                                spectralAnalysis,
                                ref lastSpectralDb,
                                devicePreviewUpdater,
                                volumeProgressReporter,
                                spectralProgressReporter);
                            OverlayFrame(composedFrame, segmentFrame, assignment.StartIndex);
                        }

                        bool sent = await device.Session.SendFrameAsync(composedFrame, token).ConfigureAwait(false);
                        if (!sent)
                        {
                            return;
                        }
                    }

                    bool auxiliarySent = await SendAuxiliaryFrameAsync(
                        devices,
                        token,
                        laserScene,
                        strobeScene).ConfigureAwait(false);
                    if (hasAuxiliaryScene && !auxiliarySent)
                    {
                        return;
                    }

                    iterations++;
                    if (stopwatch.ElapsedMilliseconds >= 1000)
                    {
                        double? displayedSpectralDb = lastSpectralDb ?? latestAuxiliarySpectralDb;
                        string rateText = needsSpectral && displayedSpectralDb.HasValue
                            ? $"Rate: {iterations} | Band: {displayedSpectralDb.Value:F1} dB"
                            : $"Rate: {iterations}";
                        rateReporter(rateText);
                        iterations = 0;
                        stopwatch.Restart();
                    }

                    await Task.Delay(Math.Max(delayProvider(), 1), token).ConfigureAwait(false);
                }
            }
            finally
            {
                if (auxiliaryWasActive || laserOutputActive || strobeOutputActive)
                {
                    byte[] offFrame = AuxiliaryPayloadBuilder.BuildOffPayload(lastAuxiliaryTransmissionOptions);
                    foreach (DeviceTarget device in devices)
                    {
                        await device.Session.SendFrameAsync(offFrame, CancellationToken.None).ConfigureAwait(false);
                        AuxiliaryRuntimeRegistry.Clear(device.Config.Id);
                    }
                }

                foreach (AcSpectralAnalysis meter in auxiliarySpectralMeters.Values)
                {
                    meter.Dispose();
                }

                auxiliarySpectralMeters.Clear();

                foreach (string sceneId in registeredImageSceneIds.ToList())
                {
                    ReleaseSharedImageSceneState(sceneId);
                }

                registeredImageSceneIds.Clear();
            }
        }

        private async Task<bool> SendAuxiliaryFrameAsync(
            IReadOnlyList<DeviceTarget> devices,
            CancellationToken token,
            SceneConfig? laserScene,
            SceneConfig? strobeScene)
        {
            bool hasLaserScene = laserScene?.Type == SceneType.LaserDmx;
            bool hasStrobeScene = strobeScene?.Type == SceneType.Strobe;
            bool hasAuxiliaryScene = hasLaserScene || hasStrobeScene;
            AuxiliaryDmxTransmissionOptions currentTransmissionOptions = hasLaserScene && laserScene != null
                ? AuxiliaryPayloadBuilder.GetTransmissionOptions(laserScene.LaserDmx)
                : lastAuxiliaryTransmissionOptions;
            bool transmissionOptionsChanged = hasLaserScene
                && !currentTransmissionOptions.Equals(lastAuxiliaryTransmissionOptions);

            if (!hasAuxiliaryScene)
            {
                if (!auxiliaryWasActive)
                {
                    return true;
                }

                auxiliaryWasActive = false;
                lastLaserSceneId = null;
                lastStrobeSceneId = null;
                auxiliaryLaserRuntimeState.Reset();
                laserTriggerState.Reset();
                strobeTriggerState.Reset();
                laserOutputActive = false;
                strobeOutputActive = false;

                byte[] offFrame = AuxiliaryPayloadBuilder.BuildOffPayload(lastAuxiliaryTransmissionOptions);
                bool offFrameSent = false;
                foreach (DeviceTarget device in devices)
                {
                    offFrameSent |= await device.Session.SendFrameAsync(offFrame, token).ConfigureAwait(false);
                    AuxiliaryRuntimeRegistry.Clear(device.Config.Id);
                }

                lastAuxiliaryTransmissionOptions = AuxiliaryPayloadBuilder.DefaultTransmissionOptions;
                return offFrameSent;
            }

            bool laserSceneChanged = !string.Equals(lastLaserSceneId, laserScene?.Id, StringComparison.Ordinal);
            bool strobeSceneChanged = !string.Equals(lastStrobeSceneId, strobeScene?.Id, StringComparison.Ordinal);
            bool previousLaserOutputActive = laserOutputActive;
            bool previousStrobeOutputActive = strobeOutputActive;

            if (laserSceneChanged)
            {
                auxiliaryLaserRuntimeState.Reset();
                laserTriggerState.Reset();
                laserOutputActive = false;
            }

            if (strobeSceneChanged)
            {
                strobeTriggerState.Reset();
                strobeOutputActive = false;
            }

            DateTime nowUtc = DateTime.UtcNow;
            AuxiliaryTriggerEvaluation laserEvaluation = default;
            AuxiliaryTriggerEvaluation strobeEvaluation = default;
            if (hasLaserScene && laserScene != null)
            {
                lastAuxiliaryTransmissionOptions = currentTransmissionOptions;
            }

            if (hasLaserScene && laserScene != null)
            {
                bool conditionHigh = EvaluateAuxiliaryTriggerCondition(laserScene.LaserDmx.Trigger);
                laserEvaluation = EvaluateAuxiliaryTrigger(laserScene.LaserDmx.Trigger, laserTriggerState, conditionHigh, nowUtc);
                laserOutputActive = laserEvaluation.OutputActive;
            }
            else
            {
                laserOutputActive = false;
            }

            if (hasStrobeScene && strobeScene != null)
            {
                bool conditionHigh = EvaluateAuxiliaryTriggerCondition(strobeScene.Strobe.Trigger);
                strobeEvaluation = EvaluateAuxiliaryTrigger(strobeScene.Strobe.Trigger, strobeTriggerState, conditionHigh, nowUtc);
                strobeOutputActive = strobeEvaluation.OutputActive;
            }
            else
            {
                strobeOutputActive = false;
            }

            HashSet<int> refreshChannels = new();
            bool refreshAll = laserSceneChanged
                || laserEvaluation.Triggered
                || (laserOutputActive && !previousLaserOutputActive);
            if (laserOutputActive && hasLaserScene && laserScene != null)
            {
                foreach (LaserDmxChannelRow channel in laserScene.LaserDmx.Channels.Where(channel => channel.RefreshEnabled))
                {
                    DateTime nextRefresh = auxiliaryLaserRuntimeState.NextRefreshByChannel.TryGetValue(channel.Channel, out DateTime stored)
                        ? stored
                        : DateTime.MinValue;
                    if (refreshAll || nextRefresh == DateTime.MinValue || nowUtc >= nextRefresh)
                    {
                        refreshChannels.Add(channel.Channel);
                        auxiliaryLaserRuntimeState.NextRefreshByChannel[channel.Channel] = nowUtc.AddSeconds(Math.Max(0.1, channel.RefreshIntervalSeconds));
                    }
                }
            }

            bool anyAuxiliaryActive = laserOutputActive || strobeOutputActive;
            bool shouldSend = laserEvaluation.StateChanged
                || strobeEvaluation.StateChanged
                || refreshAll
                || refreshChannels.Count > 0
                || laserSceneChanged
                || strobeSceneChanged
                || transmissionOptionsChanged
                || (laserOutputActive != previousLaserOutputActive)
                || (strobeOutputActive != previousStrobeOutputActive);

            lastLaserSceneId = laserScene?.Id;
            lastStrobeSceneId = strobeScene?.Id;

            if (!anyAuxiliaryActive)
            {
                if (!auxiliaryWasActive)
                {
                    return true;
                }

                auxiliaryWasActive = false;
                byte[] offFrame = AuxiliaryPayloadBuilder.BuildOffPayload(currentTransmissionOptions);
                bool anyOffSent = false;
                foreach (DeviceTarget device in devices)
                {
                    anyOffSent |= await device.Session.SendFrameAsync(offFrame, token).ConfigureAwait(false);
                    AuxiliaryRuntimeRegistry.Clear(device.Config.Id);
                }

                return anyOffSent;
            }

            auxiliaryWasActive = true;
            if (!shouldSend)
            {
                return true;
            }

            byte[] payload;
            if (laserOutputActive && laserScene != null)
            {
                lastAuxiliaryTransmissionOptions = AuxiliaryPayloadBuilder.GetTransmissionOptions(laserScene.LaserDmx);
                payload = AuxiliaryPayloadBuilder.BuildLaserPayload(
                    laserScene.LaserDmx,
                    auxiliaryLaserRuntimeState,
                    refreshAll,
                    refreshChannels,
                    strobeOutputActive);
            }
            else
            {
                payload = AuxiliaryPayloadBuilder.BuildStrobePayload(strobeOutputActive, currentTransmissionOptions);
            }

            bool anySent = false;
            foreach (DeviceTarget device in devices)
            {
                anySent |= await device.Session.SendFrameAsync(payload, token).ConfigureAwait(false);
                AuxiliaryRuntimeRegistry.Update(device.Config.Id, payload, laserOutputActive, strobeOutputActive);
            }

            return anySent;
        }

        private bool EvaluateAuxiliaryTriggerCondition(AuxiliaryTriggerConfig trigger)
        {
            return trigger.EventType switch
            {
                AuxiliaryTriggerEventType.Volume => EvaluateVolumeCondition(trigger.Volume),
                AuxiliaryTriggerEventType.SpectralAnalysis => EvaluateSpectralCondition(trigger.SpectralAnalysis),
                AuxiliaryTriggerEventType.ScreenCapture => EvaluateScreenCaptureCondition(trigger.ScreenCapture),
                _ => false
            };
        }

        private bool EvaluateVolumeCondition(AuxiliaryVolumeTriggerConfig config)
        {
            float volume = VolumeSceneRunner.ReadVolume(config.AudioDeviceId);
            return (volume * 100.0f) >= Math.Clamp(config.ThresholdPercent, 0, 100);
        }

        private bool EvaluateSpectralCondition(AuxiliarySpectralTriggerConfig config)
        {
            string deviceId = config.AudioDeviceId ?? string.Empty;
            if (!auxiliarySpectralMeters.TryGetValue(deviceId, out AcSpectralAnalysis? spectralMeter))
            {
                spectralMeter = new AcSpectralAnalysis();
                spectralMeter.Start(config.AudioDeviceId);
                auxiliarySpectralMeters[deviceId] = spectralMeter;
            }

            spectralMeter.UpdateBandSettings(new SpectralBandSettings(
                config.FrequencyLowHz,
                config.FrequencyHighHz,
                -90.0,
                0.0));

            double bandLevelDb = spectralMeter.GetCurrentBandLevelDb();
            latestAuxiliarySpectralDb = bandLevelDb;
            return bandLevelDb >= config.ThresholdDb;
        }

        private static bool EvaluateScreenCaptureCondition(AuxiliaryScreenCaptureTriggerConfig config)
        {
            Screen targetScreen = ResolveScreen(config.MonitorIndex);
            Rectangle screenBounds = targetScreen.Bounds;
            int x = Math.Clamp(config.X, 0, Math.Max(0, screenBounds.Width - 1));
            int y = Math.Clamp(config.Y, 0, Math.Max(0, screenBounds.Height - 1));
            Rectangle relative = new(
                x,
                y,
                Math.Max(1, config.Width),
                Math.Max(1, config.Height));
            Rectangle absolute = new(
                screenBounds.Left + relative.X,
                screenBounds.Top + relative.Y,
                Math.Min(relative.Width, Math.Max(1, screenBounds.Width - relative.X)),
                Math.Min(relative.Height, Math.Max(1, screenBounds.Height - relative.Y)));

            if (absolute.Width <= 0 || absolute.Height <= 0)
            {
                return false;
            }

            using Bitmap capture = new(absolute.Width, absolute.Height);
            using Graphics graphics = Graphics.FromImage(capture);
            graphics.CopyFromScreen(absolute.Location, Point.Empty, absolute.Size);

            int stepX = Math.Max(1, absolute.Width / 32);
            int stepY = Math.Max(1, absolute.Height / 32);
            double brightnessTotal = 0.0;
            int sampleCount = 0;
            for (int sampleY = 0; sampleY < absolute.Height; sampleY += stepY)
            {
                for (int sampleX = 0; sampleX < absolute.Width; sampleX += stepX)
                {
                    Color pixel = capture.GetPixel(sampleX, sampleY);
                    brightnessTotal += (pixel.R + pixel.G + pixel.B) / (255.0 * 3.0);
                    sampleCount++;
                }
            }

            double brightnessPercent = sampleCount <= 0 ? 0.0 : (brightnessTotal / sampleCount) * 100.0;
            return brightnessPercent >= Math.Clamp(config.BrightnessThresholdPercent, 0, 100);
        }

        private static AuxiliaryTriggerEvaluation EvaluateAuxiliaryTrigger(
            AuxiliaryTriggerConfig config,
            AuxiliaryTriggerRuntimeState state,
            bool conditionHigh,
            DateTime nowUtc)
        {
            bool wasActive = state.OutputActive;
            bool triggered = false;

            switch (config.RetriggerMode)
            {
                case AuxiliaryTriggerRetriggerMode.HoldWhileHigh:
                    state.OutputActive = conditionHigh;
                    triggered = conditionHigh && !wasActive;
                    break;

                case AuxiliaryTriggerRetriggerMode.RepeatWhileHigh:
                    if (conditionHigh && (!state.OutputActive || nowUtc >= state.OnUntilUtc))
                    {
                        triggered = true;
                        state.OutputActive = true;
                        state.OnUntilUtc = nowUtc.AddMilliseconds(Math.Max(1, config.OnDurationMs));
                    }
                    else if (state.OutputActive && nowUtc >= state.OnUntilUtc && !conditionHigh)
                    {
                        state.OutputActive = false;
                    }
                    break;

                default:
                    if (!conditionHigh)
                    {
                        state.Armed = true;
                    }

                    if (conditionHigh && !state.LastConditionHigh && state.Armed)
                    {
                        triggered = true;
                        state.OutputActive = true;
                        state.OnUntilUtc = nowUtc.AddMilliseconds(Math.Max(1, config.OnDurationMs));
                        state.Armed = false;
                    }
                    else if (state.OutputActive && nowUtc >= state.OnUntilUtc)
                    {
                        state.OutputActive = false;
                    }
                    break;
            }

            state.LastConditionHigh = conditionHigh;
            return new AuxiliaryTriggerEvaluation(triggered, wasActive != state.OutputActive || triggered, state.OutputActive);
        }

        private void CleanupImageSceneStates(IReadOnlyList<DeviceSceneAssignment> assignments)
        {
            HashSet<string> activeIds = assignments
                .Where(assignment => assignment.Scene.Type == SceneType.ImageRowCapture)
                .Select(assignment => assignment.Scene.Id)
                .ToHashSet(StringComparer.Ordinal);

            foreach (string sceneId in registeredImageSceneIds.Where(sceneId => !activeIds.Contains(sceneId)).ToList())
            {
                ReleaseSharedImageSceneState(sceneId);
                registeredImageSceneIds.Remove(sceneId);
            }

            foreach (string sceneId in activeIds)
            {
                if (registeredImageSceneIds.Add(sceneId))
                {
                    AcquireSharedImageSceneState(sceneId);
                }
            }
        }

        private byte[] BuildAssignmentFrame(
            DeviceSceneAssignment assignment,
            Func<string?> audioDeviceIdProvider,
            ref float? sharedVolume,
            AcSpectralAnalysis spectralAnalysis,
            ref double? lastSpectralDb,
            Action<CaptureScenePreview>? capturePreviewUpdater,
            Action<int> volumeProgressReporter,
            Action<int> spectralProgressReporter)
        {
            return assignment.Scene.Type switch
            {
                SceneType.Gradient => BuildGradientFrame(assignment.LedCount, assignment.Scene.Gradient),
                SceneType.VolumeReactive => BuildVolumeFrame(assignment.Scene, assignment.LedCount, audioDeviceIdProvider, ref sharedVolume, volumeProgressReporter),
                SceneType.ScreenRowCapture => BuildScreenCaptureFrame(assignment.Scene.Id, assignment.Scene.ScreenRowCapture, assignment.LedCount, capturePreviewUpdater),
                SceneType.SpectralAnalysis => BuildSpectralFrame(assignment.Scene, assignment.LedCount, spectralAnalysis, ref lastSpectralDb, spectralProgressReporter),
                SceneType.ImageRowCapture => BuildImageCaptureFrame(assignment.Scene.Id, assignment.Scene.ImageRowCapture, assignment.LedCount, capturePreviewUpdater),
                SceneType.LaserDmx => new byte[assignment.LedCount * 3],
                SceneType.Strobe => new byte[assignment.LedCount * 3],
                _ => BuildSolidFrame(assignment.LedCount, assignment.Scene.SolidColor),
            };
        }

        private static byte[] BuildSolidFrame(int ledCount, SolidColorSceneConfig config)
        {
            byte[] frame = new byte[ledCount * 3];
            double hue = Common.MapValue(config.Hue, 0, 360, config.MinHue, config.MaxHue);
            Color color = Common.HSVToRGB(hue, config.Saturation / 100.0, config.Brightness / 100.0);
            for (int i = 0; i < ledCount; i++)
            {
                int index = i * 3;
                frame[index] = color.R;
                frame[index + 1] = color.G;
                frame[index + 2] = color.B;
            }

            return frame;
        }

        private static byte[] BuildGradientFrame(int ledCount, GradientSceneConfig config)
        {
            byte[] frame = new byte[ledCount * 3];
            for (int i = 0; i < ledCount; i++)
            {
                double hue = Common.MapValue(i, 0, Math.Max(ledCount, 1), config.HueMin, config.HueMax);
                Color color = Common.HSVToRGB(hue, config.Saturation / 100.0, config.Brightness / 100.0);
                int index = i * 3;
                frame[index] = color.R;
                frame[index + 1] = color.G;
                frame[index + 2] = color.B;
            }

            return frame;
        }

        private static byte[] BuildVolumeFrame(
            SceneConfig scene,
            int ledCount,
            Func<string?> audioDeviceIdProvider,
            ref float? sharedVolume,
            Action<int> progressReporter)
        {
            sharedVolume ??= VolumeSceneRunner.ReadVolume(audioDeviceIdProvider());
            progressReporter((int)Math.Round(sharedVolume.Value * 100));
            return AudioReactiveFrameBuilder.BuildFrame(ledCount, sharedVolume.Value, new VolumeSceneSettings
            {
                Mode = scene.VolumeReactive.Mode,
                BrightnessValue = scene.VolumeReactive.Brightness,
                SaturationValue = scene.VolumeReactive.Saturation,
                NormalizationValue = scene.VolumeReactive.Normalization,
                Reverse = scene.VolumeReactive.Reverse,
                HueReverse = scene.VolumeReactive.HueReverse,
                White = scene.VolumeReactive.White,
                BackgroundWhite = scene.VolumeReactive.BackgroundWhite,
                BackgroundBrightnessValue = scene.VolumeReactive.BackgroundBrightness,
                BackgroundSaturationValue = scene.VolumeReactive.BackgroundSaturation,
                BackgroundHue = scene.VolumeReactive.BackgroundHue,
                HueMin = scene.VolumeReactive.HueMin,
                HueMax = scene.VolumeReactive.HueMax
            });
        }

        private static byte[] BuildSpectralFrame(
            SceneConfig scene,
            int ledCount,
            AcSpectralAnalysis spectralAnalysis,
            ref double? lastSpectralDb,
            Action<int> progressReporter)
        {
            spectralAnalysis.UpdateBandSettings(new SpectralBandSettings(
                scene.SpectralAnalysis.FrequencyLowHz,
                scene.SpectralAnalysis.FrequencyHighHz,
                scene.SpectralAnalysis.LevelLowDb,
                scene.SpectralAnalysis.LevelHighDb));

            float strength = spectralAnalysis.GetCurrentStrength();
            progressReporter((int)Math.Round(strength * 100.0f));
            lastSpectralDb = spectralAnalysis.GetCurrentBandLevelDb();
            return AudioReactiveFrameBuilder.BuildFrame(ledCount, strength, new SpectralSceneSettings
            {
                Mode = scene.SpectralAnalysis.Mode,
                BrightnessValue = scene.SpectralAnalysis.Brightness,
                SaturationValue = scene.SpectralAnalysis.Saturation,
                NormalizationValue = scene.SpectralAnalysis.Normalization,
                Reverse = scene.SpectralAnalysis.Reverse,
                HueReverse = scene.SpectralAnalysis.HueReverse,
                White = scene.SpectralAnalysis.White,
                BackgroundWhite = scene.SpectralAnalysis.BackgroundWhite,
                BackgroundBrightnessValue = scene.SpectralAnalysis.BackgroundBrightness,
                BackgroundSaturationValue = scene.SpectralAnalysis.BackgroundSaturation,
                BackgroundHue = scene.SpectralAnalysis.BackgroundHue,
                HueMin = scene.SpectralAnalysis.HueMin,
                HueMax = scene.SpectralAnalysis.HueMax
            });
        }

        private static byte[] BuildScreenCaptureFrame(
            string sceneId,
            ScreenRowCaptureSceneConfig config,
            int ledCount,
            Action<CaptureScenePreview>? previewUpdater)
        {
            Screen targetScreen = ResolveScreen(config.MonitorIndex);
            Rectangle bounds = targetScreen.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return new byte[ledCount * 3];
            }

            int captureY = Math.Max(0, Math.Min(bounds.Height - 1, config.CaptureY));
            using Bitmap screenCapture = new(bounds.Width, 1);
            using Graphics graphics = Graphics.FromImage(screenCapture);
            graphics.CopyFromScreen(bounds.Left, bounds.Top + captureY, 0, 0, new Size(bounds.Width, 1));

            List<Color> pixelColors = PixelFrameHelpers.GetBitmapRowColors(screenCapture, 0);
            if (config.Reverse)
            {
                pixelColors.Reverse();
            }

            List<Color> reducedColors = PixelFrameHelpers.ReducePixels(pixelColors, ledCount);
            previewUpdater?.Invoke(new CaptureScenePreview
            {
                SceneId = sceneId,
                Colors = new List<Color>(reducedColors),
                SourceSize = new Size(bounds.Width, bounds.Height),
                SampleIndex = captureY,
                Direction = ImageScanDirection.TopToBottom
            });

            return PixelFrameHelpers.ColorListToByteArray(reducedColors);
        }

        private byte[] BuildImageCaptureFrame(
            string sceneId,
            ImageRowCaptureSceneConfig config,
            int ledCount,
            Action<CaptureScenePreview>? previewUpdater)
        {
            ImageScenePlaybackState state = GetOrCreateImageState(sceneId, config);
            lock (state.SyncRoot)
            {
                string signature = BuildImageSceneSignature(config);
                if (!string.Equals(state.ConfigSignature, signature, StringComparison.Ordinal))
                {
                    ResetImageState(state, config, signature);
                }

                if (!TryEnsureImageLoaded(state, config))
                {
                    List<Color> blackPixels = PixelFrameHelpers.CreateBlackPixels(ledCount);
                    previewUpdater?.Invoke(new CaptureScenePreview
                    {
                        SceneId = sceneId,
                        Colors = new List<Color>(blackPixels),
                        SampleIndex = -1,
                        Direction = state.ActiveDirection
                    });

                    return PixelFrameHelpers.ColorListToByteArray(blackPixels);
                }

                Bitmap bitmap = state.CurrentBitmap!;
                int scanLength = GetScanLength(bitmap, state.ActiveDirection);
                if (scanLength <= 0)
                {
                    return new byte[ledCount * 3];
                }

                ApplyPendingSeekRequest(state, config, scanLength);
                int progressIndex = Math.Clamp((int)Math.Floor(state.ScanPosition), 0, scanLength - 1);
                int sampleIndex = MapSampleIndex(progressIndex, scanLength, state.ActiveDirection);
                List<Color> sampledPixels = GetSampledPixels(bitmap, sampleIndex, state.ActiveDirection);
                List<Color> reducedColors = PixelFrameHelpers.ReducePixels(sampledPixels, ledCount);

                previewUpdater?.Invoke(new CaptureScenePreview
                {
                    SceneId = sceneId,
                    Colors = new List<Color>(reducedColors),
                    SourcePath = state.CurrentFilePath,
                    SourceSize = bitmap.Size,
                    SampleIndex = sampleIndex,
                    Direction = state.ActiveDirection
                });

                AdvanceImageState(state, config, scanLength, delayProvider());
                return PixelFrameHelpers.ColorListToByteArray(reducedColors);
            }
        }

        private ImageScenePlaybackState GetOrCreateImageState(string sceneId, ImageRowCaptureSceneConfig config)
        {
            lock (SharedImageSceneStatesLock)
            {
                if (!SharedImageSceneStates.TryGetValue(sceneId, out ImageScenePlaybackState? state))
                {
                    state = new ImageScenePlaybackState();
                    SharedImageSceneStates[sceneId] = state;
                }

                return state;
            }
        }

        private static void AcquireSharedImageSceneState(string sceneId)
        {
            lock (SharedImageSceneStatesLock)
            {
                SharedImageSceneUsageCounts[sceneId] = SharedImageSceneUsageCounts.TryGetValue(sceneId, out int count)
                    ? count + 1
                    : 1;
            }
        }

        private static void ReleaseSharedImageSceneState(string sceneId)
        {
            lock (SharedImageSceneStatesLock)
            {
                if (!SharedImageSceneUsageCounts.TryGetValue(sceneId, out int count))
                {
                    return;
                }

                if (count > 1)
                {
                    SharedImageSceneUsageCounts[sceneId] = count - 1;
                    return;
                }

                SharedImageSceneUsageCounts.Remove(sceneId);
                if (SharedImageSceneStates.TryGetValue(sceneId, out ImageScenePlaybackState? state))
                {
                    lock (state.SyncRoot)
                    {
                        state.DisposeBitmap();
                    }

                    SharedImageSceneStates.Remove(sceneId);
                }
            }
        }

        private static string BuildImageSceneSignature(ImageRowCaptureSceneConfig config)
        {
            return string.Join("|",
                config.SourceMode,
                config.ImagePath,
                config.FolderPath,
                config.Recursive,
                config.Loop,
                config.Direction,
                config.SpeedMin.ToString("F4", System.Globalization.CultureInfo.InvariantCulture),
                config.SpeedMax.ToString("F4", System.Globalization.CultureInfo.InvariantCulture));
        }

        private static void ResetImageState(ImageScenePlaybackState state, ImageRowCaptureSceneConfig config, string signature)
        {
            state.DisposeBitmap();
            state.ConfigSignature = signature;
            state.Files = ResolveImageFiles(config);
            state.CurrentFileIndex = 0;
            state.ScanPosition = 0;
            state.ActiveDirection = ResolvePassDirection(config.Direction);
            state.ActiveSpeed = ResolvePassSpeed(config);
            state.IsStopped = false;
            state.LastAppliedSeekRevision = config.RequestedSeekRevision;
            state.LastAdvanceUtc = DateTime.MinValue;
        }

        private static List<string> ResolveImageFiles(ImageRowCaptureSceneConfig config)
        {
            if (config.SourceMode == ImageSourceMode.SingleImage)
            {
                return IsSupportedImageFile(config.ImagePath) && File.Exists(config.ImagePath)
                    ? new List<string> { config.ImagePath }
                    : new List<string>();
            }

            if (string.IsNullOrWhiteSpace(config.FolderPath) || !Directory.Exists(config.FolderPath))
            {
                return new List<string>();
            }

            SearchOption searchOption = config.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.EnumerateFiles(config.FolderPath, "*.*", searchOption)
                .Where(IsSupportedImageFile)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static bool IsSupportedImageFile(string path)
        {
            string extension = Path.GetExtension(path);
            return extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase)
                || extension.Equals(".gif", StringComparison.OrdinalIgnoreCase);
        }

        private static ImageScanDirection ResolvePassDirection(ImageScanDirection configuredDirection)
        {
            return configuredDirection != ImageScanDirection.Random
                ? configuredDirection
                : Random.Shared.Next(4) switch
                {
                    0 => ImageScanDirection.TopToBottom,
                    1 => ImageScanDirection.BottomToTop,
                    2 => ImageScanDirection.LeftToRight,
                    _ => ImageScanDirection.RightToLeft
                };
        }

        private static double ResolvePassSpeed(ImageRowCaptureSceneConfig config)
        {
            double min = Math.Max(0.01, Math.Min(config.SpeedMin, config.SpeedMax));
            double max = Math.Max(min, Math.Max(config.SpeedMin, config.SpeedMax));
            return Math.Abs(max - min) < 0.0001
                ? min
                : min + (Random.Shared.NextDouble() * (max - min));
        }

        private static bool TryEnsureImageLoaded(ImageScenePlaybackState state, ImageRowCaptureSceneConfig config)
        {
            if (state.CurrentBitmap != null)
            {
                return true;
            }

            if (state.Files.Count == 0)
            {
                return false;
            }

            bool allowWrap = config.SourceMode == ImageSourceMode.SingleImage || config.Loop;
            return TryLoadAvailableBitmap(state, allowWrap);
        }

        private static bool TryLoadAvailableBitmap(ImageScenePlaybackState state, bool allowWrap)
        {
            if (state.Files.Count == 0)
            {
                return false;
            }

            int attempts = allowWrap ? state.Files.Count : Math.Max(state.Files.Count - state.CurrentFileIndex, 0);
            for (int offset = 0; offset < attempts; offset++)
            {
                int index = state.CurrentFileIndex + offset;
                if (allowWrap)
                {
                    index %= state.Files.Count;
                }
                else if (index >= state.Files.Count)
                {
                    break;
                }

                if (TryLoadBitmapAtIndex(state, index))
                {
                    return true;
                }
            }

            state.DisposeBitmap();
            return false;
        }

        private static bool TryLoadBitmapAtIndex(ImageScenePlaybackState state, int index)
        {
            if (index < 0 || index >= state.Files.Count)
            {
                return false;
            }

            try
            {
                using Image image = Image.FromFile(state.Files[index]);
                state.DisposeBitmap();
                state.CurrentBitmap = new Bitmap(image);
                state.CurrentFileIndex = index;
                state.CurrentFilePath = state.Files[index];
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static int GetScanLength(Bitmap bitmap, ImageScanDirection direction)
        {
            return direction is ImageScanDirection.TopToBottom or ImageScanDirection.BottomToTop
                ? bitmap.Height
                : bitmap.Width;
        }

        private static int MapSampleIndex(int progressIndex, int scanLength, ImageScanDirection direction)
        {
            return direction is ImageScanDirection.BottomToTop or ImageScanDirection.RightToLeft
                ? Math.Max(0, scanLength - 1 - progressIndex)
                : progressIndex;
        }

        private static List<Color> GetSampledPixels(Bitmap bitmap, int sampleIndex, ImageScanDirection direction)
        {
            return direction is ImageScanDirection.TopToBottom or ImageScanDirection.BottomToTop
                ? PixelFrameHelpers.GetBitmapRowColors(bitmap, sampleIndex)
                : PixelFrameHelpers.GetBitmapColumnColors(bitmap, sampleIndex);
        }

        private static void ApplyPendingSeekRequest(ImageScenePlaybackState state, ImageRowCaptureSceneConfig config, int scanLength)
        {
            if (config.RequestedSampleIndex < 0 || config.RequestedSeekRevision == state.LastAppliedSeekRevision)
            {
                return;
            }

            int clampedSampleIndex = Math.Clamp(config.RequestedSampleIndex, 0, scanLength - 1);
            int progressIndex = MapProgressIndex(clampedSampleIndex, scanLength, state.ActiveDirection);
            state.ScanPosition = progressIndex;
            state.IsStopped = false;
            state.LastAppliedSeekRevision = config.RequestedSeekRevision;
        }

        private static int MapProgressIndex(int sampleIndex, int scanLength, ImageScanDirection direction)
        {
            return direction is ImageScanDirection.BottomToTop or ImageScanDirection.RightToLeft
                ? Math.Max(0, scanLength - 1 - sampleIndex)
                : sampleIndex;
        }

        private static void AdvanceImageState(ImageScenePlaybackState state, ImageRowCaptureSceneConfig config, int scanLength, int delayMs)
        {
            if (config.IsPaused || state.IsStopped || scanLength <= 0)
            {
                return;
            }

            int effectiveDelayMs = Math.Max(delayMs, 1);
            DateTime nowUtc = DateTime.UtcNow;
            if (state.LastAdvanceUtc != DateTime.MinValue
                && (nowUtc - state.LastAdvanceUtc).TotalMilliseconds < effectiveDelayMs * 0.8)
            {
                return;
            }

            double maxProgress = Math.Max(scanLength - 1, 0);
            state.ScanPosition += state.ActiveSpeed;
            state.LastAdvanceUtc = nowUtc;
            if (state.ScanPosition <= maxProgress)
            {
                return;
            }

            if (config.SourceMode == ImageSourceMode.SingleImage)
            {
                CompleteSingleImagePass(state, config, maxProgress);
                return;
            }

            CompleteFolderImagePass(state, config, maxProgress);
        }

        private static void CompleteSingleImagePass(ImageScenePlaybackState state, ImageRowCaptureSceneConfig config, double maxProgress)
        {
            if (!config.Loop)
            {
                state.ScanPosition = maxProgress;
                state.IsStopped = true;
                return;
            }

            state.ScanPosition = 0;
            state.ActiveSpeed = ResolvePassSpeed(config);
            state.ActiveDirection = config.Direction == ImageScanDirection.Random
                ? ResolvePassDirection(config.Direction)
                : GetOppositeDirection(state.ActiveDirection);
        }

        private static void CompleteFolderImagePass(ImageScenePlaybackState state, ImageRowCaptureSceneConfig config, double maxProgress)
        {
            if (TryAdvanceToNextFolderImage(state, config))
            {
                state.ScanPosition = 0;
                state.IsStopped = false;
                state.ActiveSpeed = ResolvePassSpeed(config);
                state.ActiveDirection = ResolvePassDirection(config.Direction);
                return;
            }

            state.ScanPosition = maxProgress;
            state.IsStopped = true;
        }

        private static bool TryAdvanceToNextFolderImage(ImageScenePlaybackState state, ImageRowCaptureSceneConfig config)
        {
            if (state.Files.Count == 0)
            {
                return false;
            }

            int startIndex = state.CurrentFileIndex;
            int attempts = config.Loop ? state.Files.Count : Math.Max(state.Files.Count - startIndex - 1, 0);
            for (int step = 1; step <= attempts; step++)
            {
                int nextIndex = startIndex + step;
                if (config.Loop)
                {
                    nextIndex %= state.Files.Count;
                }
                else if (nextIndex >= state.Files.Count)
                {
                    break;
                }

                if (TryLoadBitmapAtIndex(state, nextIndex))
                {
                    return true;
                }
            }

            return false;
        }

        private static ImageScanDirection GetOppositeDirection(ImageScanDirection direction)
        {
            return direction switch
            {
                ImageScanDirection.TopToBottom => ImageScanDirection.BottomToTop,
                ImageScanDirection.BottomToTop => ImageScanDirection.TopToBottom,
                ImageScanDirection.LeftToRight => ImageScanDirection.RightToLeft,
                ImageScanDirection.RightToLeft => ImageScanDirection.LeftToRight,
                _ => ImageScanDirection.TopToBottom
            };
        }

        private static void OverlayFrame(byte[] composedFrame, byte[] segmentFrame, int startLedIndex)
        {
            int startByteIndex = Math.Max(0, startLedIndex) * 3;
            int copyLength = Math.Min(segmentFrame.Length, Math.Max(0, composedFrame.Length - startByteIndex));
            if (copyLength > 0)
            {
                Array.Copy(segmentFrame, 0, composedFrame, startByteIndex, copyLength);
            }
        }

        private static Screen ResolveScreen(int monitorIndex)
        {
            Screen[] screens = Screen.AllScreens;
            if (screens.Length == 0)
            {
                return Screen.PrimaryScreen ?? throw new InvalidOperationException("No screens are available.");
            }

            if (monitorIndex >= 0 && monitorIndex < screens.Length)
            {
                return screens[monitorIndex];
            }

            return Screen.PrimaryScreen ?? screens[0];
        }
    }
}
