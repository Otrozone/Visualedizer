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

                List<Color> pixelColors = new(bounds.Width);
                for (int x = 0; x < bounds.Width; x++)
                {
                    pixelColors.Add(screenCapture.GetPixel(x, 0));
                }

                if (settings.Reverse)
                {
                    pixelColors.Reverse();
                }

                bool previewUpdated = false;
                bool anySent = false;
                foreach (DeviceTarget device in devices)
                {
                    List<Color> reducedColors = ReducePixels(pixelColors, device.Config.LedCount, bounds.Width);
                    anySent |= await device.Session.SendFrameAsync(ColorListToByteArray(reducedColors), token).ConfigureAwait(false);

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

    internal sealed class CompositeSceneRunner : ISceneRunner
    {
        private readonly Func<IReadOnlyList<DeviceSceneAssignment>> assignmentsProvider;
        private readonly Func<string?> audioDeviceIdProvider;
        private readonly Func<int> delayProvider;
        private readonly Action<IReadOnlyList<Color>> previewUpdater;
        private readonly Action<int> volumeProgressReporter;
        private readonly Action<int> spectralProgressReporter;
        private readonly Action<string> rateReporter;

        public CompositeSceneRunner(
            Func<IReadOnlyList<DeviceSceneAssignment>> assignmentsProvider,
            Func<string?> audioDeviceIdProvider,
            Func<int> delayProvider,
            Action<IReadOnlyList<Color>> previewUpdater,
            Action<int> volumeProgressReporter,
            Action<int> spectralProgressReporter,
            Action<string> rateReporter)
        {
            this.assignmentsProvider = assignmentsProvider;
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

            while (!token.IsCancellationRequested)
            {
                IReadOnlyList<DeviceSceneAssignment> assignments = assignmentsProvider();
                if (assignments.Count == 0)
                {
                    return;
                }

                bool needsSpectral = assignments.Any(assignment => assignment.Scene.Type == SceneType.SpectralAnalysis);
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
                bool previewUpdated = false;
                double? lastSpectralDb = null;

                foreach (DeviceTarget device in devices)
                {
                    byte[] composedFrame = new byte[device.Config.LedCount * 3];
                    foreach (DeviceSceneAssignment assignment in assignments)
                    {
                        byte[] segmentFrame = BuildAssignmentFrame(
                            assignment,
                            audioDeviceIdProvider,
                            ref sharedVolume,
                            spectralAnalysis,
                            ref lastSpectralDb,
                            previewUpdated ? null : previewUpdater,
                            volumeProgressReporter,
                            spectralProgressReporter);
                        OverlayFrame(composedFrame, segmentFrame, assignment.StartIndex);
                        previewUpdated = previewUpdated || assignment.Scene.Type == SceneType.ScreenRowCapture;
                    }

                    bool sent = await device.Session.SendFrameAsync(composedFrame, token).ConfigureAwait(false);
                    if (!sent)
                    {
                        return;
                    }
                }

                iterations++;
                if (stopwatch.ElapsedMilliseconds >= 1000)
                {
                    string rateText = needsSpectral && lastSpectralDb.HasValue
                        ? $"Rate: {iterations} | Band: {lastSpectralDb.Value:F1} dB"
                        : $"Rate: {iterations}";
                    rateReporter(rateText);
                    iterations = 0;
                    stopwatch.Restart();
                }

                await Task.Delay(Math.Max(delayProvider(), 1), token).ConfigureAwait(false);
            }
        }

        private static byte[] BuildAssignmentFrame(
            DeviceSceneAssignment assignment,
            Func<string?> audioDeviceIdProvider,
            ref float? sharedVolume,
            AcSpectralAnalysis spectralAnalysis,
            ref double? lastSpectralDb,
            Action<IReadOnlyList<Color>>? previewUpdater,
            Action<int> volumeProgressReporter,
            Action<int> spectralProgressReporter)
        {
            return assignment.Scene.Type switch
            {
                SceneType.Gradient => BuildGradientFrame(assignment.LedCount, assignment.Scene.Gradient),
                SceneType.VolumeReactive => BuildVolumeFrame(assignment.Scene, assignment.LedCount, audioDeviceIdProvider, ref sharedVolume, volumeProgressReporter),
                SceneType.ScreenRowCapture => BuildScreenCaptureFrame(assignment.Scene.ScreenRowCapture, assignment.LedCount, previewUpdater),
                SceneType.SpectralAnalysis => BuildSpectralFrame(assignment.Scene, assignment.LedCount, spectralAnalysis, ref lastSpectralDb, spectralProgressReporter),
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

        private static byte[] BuildScreenCaptureFrame(ScreenRowCaptureSceneConfig config, int ledCount, Action<IReadOnlyList<Color>>? previewUpdater)
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

            List<Color> pixelColors = new(bounds.Width);
            for (int x = 0; x < bounds.Width; x++)
            {
                pixelColors.Add(screenCapture.GetPixel(x, 0));
            }

            if (config.Reverse)
            {
                pixelColors.Reverse();
            }

            List<Color> reducedColors = ReducePixels(pixelColors, ledCount, bounds.Width);
            previewUpdater?.Invoke(reducedColors);
            return ColorListToByteArray(reducedColors);
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
}
