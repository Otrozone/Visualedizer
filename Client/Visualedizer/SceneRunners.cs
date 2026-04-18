using NAudio.CoreAudioApi;
using System.Diagnostics;

namespace Ledqualizer
{
    internal sealed class BasicSceneSettings
    {
        public bool Solid { get; set; }
        public double SolidHue { get; set; }
        public double SolidMinHue { get; set; }
        public double SolidMaxHue { get; set; }
        public int SaturationValue { get; set; }
        public int SaturationMinimum { get; set; }
        public int SaturationMaximum { get; set; }
        public int BrightnessValue { get; set; }
        public int BrightnessMaximum { get; set; }
        public double GradientHueMin { get; set; }
        public double GradientHueMax { get; set; }
        public int Delay { get; set; }
    }

    internal sealed class VolumeSceneSettings
    {
        public AcVolume.AudioCaptureVolumeMode Mode { get; set; }
        public int Delay { get; set; }
        public int BrightnessValue { get; set; }
        public int BrightnessMaximum { get; set; }
        public int NormalizationValue { get; set; }
        public bool Reverse { get; set; }
        public bool HueReverse { get; set; }
        public bool White { get; set; }
        public bool BackgroundWhite { get; set; }
        public int BackgroundBrightnessValue { get; set; }
        public double BackgroundHue { get; set; }
        public double HueMin { get; set; }
        public double HueMax { get; set; }
    }

    internal sealed class ScreenCaptureSceneSettings
    {
        public int Delay { get; set; }
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

    internal sealed class BasicSceneRunner : ISceneRunner
    {
        private readonly Func<BasicSceneSettings> settingsProvider;

        public BasicSceneRunner(Func<BasicSceneSettings> settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task RunAsync(IReadOnlyList<DeviceTarget> devices, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                BasicSceneSettings settings = settingsProvider();
                bool anySent = false;

                foreach (DeviceTarget device in devices)
                {
                    byte[] frame = BuildFrame(device.Config.LedCount, settings);
                    anySent |= await device.Session.SendFrameAsync(frame, token).ConfigureAwait(false);
                }

                if (!anySent)
                {
                    return;
                }

                await Task.Delay(Math.Max(settings.Delay, 1), token).ConfigureAwait(false);
            }
        }

        private static byte[] BuildFrame(int ledCount, BasicSceneSettings settings)
        {
            byte[] ledConfigArray = new byte[ledCount * 3];

            if (settings.Solid)
            {
                double hue = Common.MapValue(settings.SolidHue, 0, 360, settings.SolidMinHue, settings.SolidMaxHue);
                double saturation = Common.MapValue(settings.SaturationValue, settings.SaturationMinimum, settings.SaturationMaximum, 0, 1.0);
                double brightness = (double)settings.BrightnessValue / settings.BrightnessMaximum;
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

            double gradientSaturation = Common.MapValue(settings.SaturationValue, settings.SaturationMinimum, settings.SaturationMaximum, 0, 1.0);
            double gradientBrightness = (double)settings.BrightnessValue / settings.BrightnessMaximum;
            for (int i = 0; i < ledCount; i++)
            {
                double hue = Common.MapValue(i, 0, ledCount, settings.GradientHueMin, settings.GradientHueMax);
                Color rgbColor = Common.HSVToRGB(hue, gradientSaturation, gradientBrightness);
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
                string? requestedDeviceId = audioDeviceIdProvider();
                float volume = ReadVolume(requestedDeviceId);
                int percentVolume = (int)Math.Round(volume * 100);

                progressReporter(percentVolume);
                bool anySent = false;

                foreach (DeviceTarget device in devices)
                {
                    byte[] frame = BuildFrame(device.Config.LedCount, volume, settings);
                    anySent |= await device.Session.SendFrameAsync(frame, token).ConfigureAwait(false);
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

        private static float ReadVolume(string? deviceId)
        {
            try
            {
                using MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
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

        private static byte[] BuildFrame(int ledCount, float volume, VolumeSceneSettings settings)
        {
            byte[] ledConfigArray = new byte[ledCount * 3];
            float normalizedVol = (settings.NormalizationValue / 10.0f) * volume;

            switch (settings.Mode)
            {
                case AcVolume.AudioCaptureVolumeMode.ModeEndToStart:
                    ComputeColorsEndToStart(ledConfigArray, ledCount, normalizedVol, settings);
                    break;
                case AcVolume.AudioCaptureVolumeMode.ModeMidToOut:
                    ComputeColorsMidToOut(ledConfigArray, ledCount, normalizedVol, settings);
                    break;
                case AcVolume.AudioCaptureVolumeMode.ModeColorPush:
                    ComputeColorsColorPush(ledConfigArray, ledCount, normalizedVol, settings);
                    break;
                case AcVolume.AudioCaptureVolumeMode.ModeMidToOut_Point:
                    ComputeColorsMidToOutPoint(ledConfigArray, ledCount, normalizedVol, settings);
                    break;
                case AcVolume.AudioCaptureVolumeMode.ModeBrightness:
                    ComputeColorsBrightness(ledConfigArray, ledCount, normalizedVol, settings);
                    break;
                default:
                    ComputeColorsStartToEnd(ledConfigArray, ledCount, normalizedVol, settings);
                    break;
            }

            if (settings.Reverse)
            {
                Array.Reverse(ledConfigArray);
            }

            return ledConfigArray;
        }

        private static void ComputeColorsStartToEnd(byte[] ledConfigArray, int ledCount, float vol, VolumeSceneSettings settings)
        {
            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                if (i < Math.Round(ledCount * vol))
                {
                    double hue = 360 * ((float)i / ledCount);
                    hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, 1.0, (double)settings.BrightnessValue / settings.BrightnessMaximum);
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

        private static void ComputeColorsEndToStart(byte[] ledConfigArray, int ledCount, float vol, VolumeSceneSettings settings)
        {
            for (int i = ledCount - 1; i > 0; i--)
            {
                int idx = i * 3;
                if (i > Math.Round(ledCount * (1 - vol)))
                {
                    double hue = 360 - (360 * ((float)i / ledCount));
                    hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, 1.0, (double)settings.BrightnessValue / settings.BrightnessMaximum);
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

        private static void ComputeColorsMidToOut(byte[] ledConfigArray, int ledCount, float vol, VolumeSceneSettings settings)
        {
            int center = ledCount / 2;
            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = center == 0 ? 0 : (float)distance / center;

                if (vol > distanceFactor)
                {
                    double hue = 360 * (settings.HueReverse ? 1 - distanceFactor : distanceFactor);
                    hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, settings.White ? 0 : 1.0, (double)settings.BrightnessValue / settings.BrightnessMaximum);
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

        private static void ComputeColorsMidToOutPoint(byte[] ledConfigArray, int ledCount, float vol, VolumeSceneSettings settings)
        {
            int center = ledCount / 2;
            const int pointSize = 10;

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = center == 0 ? 0 : (float)distance / center;

                if (Math.Round(vol * center) > distance - pointSize && Math.Round(vol * center) < distance + pointSize)
                {
                    double hue = 360 * (settings.HueReverse ? 1 - distanceFactor : distanceFactor);
                    hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, 1.0, (double)settings.BrightnessValue / settings.BrightnessMaximum);
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

        private static void ComputeColorsColorPush(byte[] ledConfigArray, int ledCount, float vol, VolumeSceneSettings settings)
        {
            int center = ledCount / 2;

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = center == 0 ? 0 : (float)distance / center;
                float adjustedVol = vol * (1.0f - distanceFactor);

                Color rgbColor = Common.HSVToRGB(360 * adjustedVol, 1.0, (double)settings.BrightnessValue / settings.BrightnessMaximum);
                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }
        }

        private static void ComputeColorsBrightness(byte[] ledConfigArray, int ledCount, float vol, VolumeSceneSettings settings)
        {
            int center = ledCount / 2;

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = center == 0 ? 0 : (float)distance / center;
                float adjustedVol = vol * (1.0f - distanceFactor);

                double hue = 360 * (settings.HueReverse ? 1 - distanceFactor : distanceFactor);
                hue = Common.MapValue(hue, 0, 360, settings.HueMin, settings.HueMax);
                Color rgbColor = Common.HSVToRGB(hue, 1.0, adjustedVol * ((double)settings.BrightnessValue / settings.BrightnessMaximum));
                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }
        }

        private static void ApplyBackground(byte[] ledConfigArray, int idx, VolumeSceneSettings settings)
        {
            double saturation = settings.BackgroundWhite ? 0 : 1.0;
            double brightness = settings.BackgroundBrightnessValue / 100.0;
            Color bgColor = Common.HSVToRGB(settings.BackgroundHue, saturation, brightness);
            ledConfigArray[idx] = bgColor.R;
            ledConfigArray[idx + 1] = bgColor.G;
            ledConfigArray[idx + 2] = bgColor.B;
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
            int screenWidth = Screen.PrimaryScreen?.Bounds.Width ?? 0;
            if (screenWidth <= 0)
            {
                return;
            }

            using Bitmap screenCapture = new Bitmap(screenWidth, 1);
            using Graphics graphics = Graphics.FromImage(screenCapture);

            while (!token.IsCancellationRequested)
            {
                ScreenCaptureSceneSettings settings = settingsProvider();
                graphics.CopyFromScreen(0, settings.CaptureY, 0, 0, new Size(screenWidth, 1));

                var pixelColors = new List<Color>(screenWidth);
                for (int x = 0; x < screenWidth; x++)
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
                    List<Color> reducedColors = ReducePixels(pixelColors, device.Config.LedCount, screenWidth);
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

        private static List<Color> ReducePixels(List<Color> pixelColors, int ledCount, int screenWidth)
        {
            var reducedPixelColors = new List<Color>(ledCount);
            int segmentSize = Math.Max(screenWidth / ledCount, 1);

            for (int i = 0; i < ledCount; i++)
            {
                int startIndex = i * segmentSize;
                if (startIndex >= pixelColors.Count)
                {
                    reducedPixelColors.Add(Color.Black);
                    continue;
                }

                int endIndex = Math.Min(((i + 1) * segmentSize), pixelColors.Count) - 1;
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
            using Bitmap screenCapture = new Bitmap(1, 1);
            using Graphics graphics = Graphics.FromImage(screenCapture);

            while (!token.IsCancellationRequested)
            {
                OtherDevicesSceneSettings settings = settingsProvider();
                byte[] data = BuildFrame(screenCapture, graphics, settings);
                bool anySent = false;

                foreach (DeviceTarget device in devices)
                {
                    anySent |= await device.Session.SendFrameAsync(data, token).ConfigureAwait(false);
                }

                if (!anySent)
                {
                    return;
                }

                await Task.Delay(Math.Max(settings.Delay, 1), token).ConfigureAwait(false);
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
}
