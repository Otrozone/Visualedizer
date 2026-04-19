using NAudio.CoreAudioApi;
using NAudio.Dsp;
using NAudio.Wave;

namespace Ledqualizer
{
    internal sealed class AcSpectralAnalysis : IDisposable
    {
        private const int FftLength = 2048;
        private const double SilenceFloorDb = -90.0;
        private const double LevelEpsilon = 1e-12;

        private readonly SampleAggregator sampleAggregator = new(FftLength);
        private readonly object settingsLock = new();

        private WasapiLoopbackCapture? capture;
        private int sampleRate;
        private float currentStrength;
        private double currentBandLevelDb = SilenceFloorDb;
        private SpectralBandSettings bandSettings = SpectralBandSettings.Default;

        public AcSpectralAnalysis()
        {
            sampleAggregator.PerformFFT = true;
            sampleAggregator.FftCalculated += SampleAggregator_FftCalculated;
        }

        public void UpdateBandSettings(SpectralBandSettings settings)
        {
            lock (settingsLock)
            {
                bandSettings = settings.Normalize();
            }
        }

        public void Start(string? deviceId)
        {
            Stop();
            currentStrength = 0.0f;
            currentBandLevelDb = SilenceFloorDb;

            try
            {
                using MMDeviceEnumerator deviceEnumerator = new();
                MMDevice device = ResolveAudioDevice(deviceEnumerator, deviceId);

                capture = new WasapiLoopbackCapture(device);
                sampleRate = capture.WaveFormat.SampleRate;
                capture.DataAvailable += Capture_DataAvailable;
                capture.RecordingStopped += Capture_RecordingStopped;
                capture.StartRecording();
            }
            catch
            {
                capture?.Dispose();
                capture = null;
            }
        }

        public float GetCurrentStrength() => currentStrength;

        public double GetCurrentBandLevelDb() => currentBandLevelDb;

        public void Stop()
        {
            if (capture == null)
            {
                return;
            }

            capture.DataAvailable -= Capture_DataAvailable;
            capture.RecordingStopped -= Capture_RecordingStopped;

            try
            {
                capture.StopRecording();
            }
            catch
            {
                capture.Dispose();
            }

            capture = null;
        }

        public void Dispose()
        {
            Stop();
            sampleAggregator.FftCalculated -= SampleAggregator_FftCalculated;
        }

        private void Capture_DataAvailable(object? sender, WaveInEventArgs e)
        {
            WasapiLoopbackCapture? activeCapture = capture;
            if (activeCapture == null || e.BytesRecorded <= 0)
            {
                return;
            }

            WaveFormat format = activeCapture.WaveFormat;
            int bytesPerFrame = format.BlockAlign;
            if (bytesPerFrame <= 0)
            {
                return;
            }

            for (int frameOffset = 0; frameOffset + bytesPerFrame <= e.BytesRecorded; frameOffset += bytesPerFrame)
            {
                sampleAggregator.Add(ReadMonoSample(e.Buffer, frameOffset, format));
            }
        }

        private static float ReadMonoSample(byte[] buffer, int frameOffset, WaveFormat format)
        {
            int channels = Math.Max(format.Channels, 1);
            int bytesPerChannel = Math.Max(format.BitsPerSample / 8, 1);
            double sum = 0.0;

            for (int channel = 0; channel < channels; channel++)
            {
                int sampleOffset = frameOffset + (channel * bytesPerChannel);
                switch (format.Encoding)
                {
                    case WaveFormatEncoding.IeeeFloat when format.BitsPerSample == 32:
                        sum += BitConverter.ToSingle(buffer, sampleOffset);
                        break;
                    case WaveFormatEncoding.Pcm when format.BitsPerSample == 16:
                        sum += BitConverter.ToInt16(buffer, sampleOffset) / 32768.0;
                        break;
                    case WaveFormatEncoding.Pcm when format.BitsPerSample == 24:
                        int sample24 = buffer[sampleOffset] | (buffer[sampleOffset + 1] << 8) | (buffer[sampleOffset + 2] << 16);
                        if ((sample24 & 0x800000) != 0)
                        {
                            sample24 |= unchecked((int)0xFF000000);
                        }

                        sum += sample24 / 8388608.0;
                        break;
                    default:
                        return 0.0f;
                }
            }

            return (float)(sum / channels);
        }

        private void SampleAggregator_FftCalculated(object? sender, FftEventArgs e)
        {
            if (sampleRate <= 0)
            {
                return;
            }

            SpectralBandSettings settings;
            lock (settingsLock)
            {
                settings = bandSettings;
            }

            int usableBins = e.Result.Length / 2;
            if (usableBins <= 0)
            {
                currentStrength = 0.0f;
                currentBandLevelDb = SilenceFloorDb;
                return;
            }

            double binWidth = sampleRate / (double)FftLength;
            int startIndex = Math.Max(1, (int)Math.Floor(settings.FrequencyLowHz / binWidth));
            int endIndex = Math.Min(usableBins - 1, (int)Math.Ceiling(settings.FrequencyHighHz / binWidth));
            if (endIndex < startIndex)
            {
                currentStrength = 0.0f;
                currentBandLevelDb = SilenceFloorDb;
                return;
            }

            double power = 0.0;
            int count = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                double magnitude = Math.Sqrt((e.Result[i].X * e.Result[i].X) + (e.Result[i].Y * e.Result[i].Y));
                power += magnitude * magnitude;
                count++;
            }

            double rmsMagnitude = count == 0 ? 0.0 : Math.Sqrt(power / count);
            double bandLevelDb = 20.0 * Math.Log10(rmsMagnitude + LevelEpsilon);
            bandLevelDb = Math.Max(SilenceFloorDb, Math.Min(0.0, bandLevelDb));

            float targetStrength = (float)Math.Clamp(
                Common.MapValue(bandLevelDb, settings.LevelLowDb, settings.LevelHighDb, 0.0, 1.0),
                0.0,
                1.0);

            float previous = currentStrength;
            float smoothing = targetStrength >= previous ? 0.45f : 0.20f;
            currentStrength = previous + ((targetStrength - previous) * smoothing);
            currentBandLevelDb = bandLevelDb;
        }

        private void Capture_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            capture?.Dispose();
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

    internal readonly struct SpectralBandSettings
    {
        private const double MinimumDb = -90.0;

        public static SpectralBandSettings Default => new(60, 250, -60, -20);

        public SpectralBandSettings(double frequencyLowHz, double frequencyHighHz, double levelLowDb, double levelHighDb)
        {
            FrequencyLowHz = frequencyLowHz;
            FrequencyHighHz = frequencyHighHz;
            LevelLowDb = levelLowDb;
            LevelHighDb = levelHighDb;
        }

        public double FrequencyLowHz { get; }
        public double FrequencyHighHz { get; }
        public double LevelLowDb { get; }
        public double LevelHighDb { get; }

        public SpectralBandSettings Normalize()
        {
            double lowHz = Math.Clamp(Math.Min(FrequencyLowHz, FrequencyHighHz), 20.0, 20000.0);
            double highHz = Math.Clamp(Math.Max(FrequencyLowHz, FrequencyHighHz), 20.0, 20000.0);
            double lowDb = Math.Clamp(Math.Min(LevelLowDb, LevelHighDb), MinimumDb, 0.0);
            double highDb = Math.Clamp(Math.Max(LevelLowDb, LevelHighDb), MinimumDb, 0.0);

            if (Math.Abs(highDb - lowDb) < 0.1)
            {
                highDb = Math.Min(0.0, lowDb + 0.1);
            }

            return new SpectralBandSettings(lowHz, highHz, lowDb, highDb);
        }
    }
}
