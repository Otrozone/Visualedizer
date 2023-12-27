using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ledqualizer
{
    internal class AudioCaptureEqualizer
    {
        // NAudio.Wave.WaveOut waveOut;

        private LedSync ledSync;

        // private static int fftLength = 8192;
        private static int fftLength = 16;
        private SampleAggregator sampleAggregator = new SampleAggregator(fftLength);

        int sampleRate;
        int bytesPerSample;

        double minFrequency = 20.0;
        double maxFrequency = 20000.0;


        public AudioCaptureEqualizer(LedSync ledSync)
        {
            this.ledSync = ledSync;
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (e.BytesRecorded > 0)
            {
                int bytesRecorded = e.BytesRecorded;

                for (int i = 0; i < bytesRecorded; i += bytesPerSample)
                {
                    if (bytesPerSample == 4)
                    {
                        float sample32 = BitConverter.ToSingle(e.Buffer, i);
                        sampleAggregator.Add(sample32);
                    }
                    else if (bytesPerSample == 8)
                    {
                        double sample64 = BitConverter.ToDouble(e.Buffer, i);
                        sampleAggregator.Add(sample64);
                    }
                }
            }
        }

        private Color CalculateColor(double intensity, double segmentStartFrequency, double segmentEndFrequency)
        {
            // Now it's done in normalization
            double minIntensity = 0.0;
            double maxIntensity = 1.0;

            double brightness = Common.MapValue(intensity, minIntensity, maxIntensity, 0.0, 1.0);
            // double brightness = Math.Min(intensity * 100, 1);

            double hue = Common.MapValue(segmentStartFrequency, minFrequency, maxFrequency, 0.0, 360.0);
            float saturation = 1.0f;

            Color rgbColor = Common.HSVToRGB((float)hue, saturation, (float)brightness);
            // Color rgbColor = Color.FromArgb(hsvColor.R, hsvColor.G, hsvColor.B);

            return rgbColor;
        }


        private double[] SegmentMagnitudeSpectrum(double[] magnitudeSpectrum)
        {
            // int segmentCount = ledSync.config.ledCount;
            int segmentCount = 3;
            double[] segmentedMagnitude = new double[segmentCount];
            double segmentSize = (maxFrequency - minFrequency) / segmentCount;

            for (int segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++)
            {
                double segmentMinFrequency = minFrequency + segmentIndex * segmentSize;
                double segmentMaxFrequency = segmentMinFrequency + segmentSize;

                // Get the frequency indexies
                int startIndex = (int)Math.Floor(segmentMinFrequency * fftLength / sampleRate); 
                int endIndex = (int)Math.Ceiling(segmentMaxFrequency * fftLength / sampleRate);
                startIndex = Math.Max(0, Math.Min(startIndex, fftLength - 1));
                endIndex = Math.Max(0, Math.Min(endIndex, fftLength - 1));

                double totalMagnitude = 0.0;
                int count = 0;

                for (int i = startIndex; i <= endIndex; i++)
                {
                    totalMagnitude += magnitudeSpectrum[i];
                    count++;
                }

                double averageMagnitude = count > 0 ? totalMagnitude / count : 0.0;

                // averageMagnitude = Math.Min(averageMagnitude, 0.0000002);
                // segmentedMagnitude[segmentIndex] = MapValue(averageMagnitude, 0, 0.0000002, 0.0, 1.0);
                // Math.Floor(Math.Log10(Math.Abs(averageMagnitude)));
                segmentedMagnitude[segmentIndex] = Math.Floor(Math.Log10(Math.Abs(averageMagnitude * 1000)));
                // Debug.WriteLine($"AvgMag: [{segmentIndex}]: {averageMagnitude} ({Math.Floor(Math.Log10(Math.Abs(averageMagnitude)))})");
            }

            return segmentedMagnitude;
        }

        private void NormalizeMagnitudeSpectrum(double[] magnitudeSpectrum)
        {
            double maxMagnitude = magnitudeSpectrum.Max();
            for (int i = 0; i < magnitudeSpectrum.Length; i++)
            {
                double normalizedMagnitude = magnitudeSpectrum[i] / maxMagnitude;
                magnitudeSpectrum[i] = double.IsNaN(normalizedMagnitude) ? 0 : (double.IsInfinity(normalizedMagnitude) ? double.MaxValue : normalizedMagnitude);
            }
        }

        private void FftCalculated(object sender, FftEventArgs e)
        {
            double[] magnitudeSpectrum = new double[e.Result.Length];
            for (int i = 0; i < e.Result.Length; i++)
            {
                magnitudeSpectrum[i] = Math.Sqrt(e.Result[i].X * e.Result[i].X + e.Result[i].Y * e.Result[i].Y);
            }
            NormalizeMagnitudeSpectrum(magnitudeSpectrum);

            double[] segmentedMagnitude = SegmentMagnitudeSpectrum(magnitudeSpectrum);
// fail?
            int segmentCount = segmentedMagnitude.Length;
            double segmentSize = (maxFrequency - minFrequency) / segmentCount;

            Color[] colors = new Color[segmentedMagnitude.Length];

            for (int i = 0; i < segmentedMagnitude.Length; i++)
            {
                // Debug.WriteLine($"Segment {i}: Average Magnitude = {segmentedMagnitude[i]}");
                
                double segmentStartFreq = minFrequency + (i * segmentSize);
                double segmentEndFreq = segmentStartFreq + segmentSize;

                colors[i] = CalculateColor(segmentedMagnitude[i], segmentStartFreq, segmentEndFreq);
            }

            _ = ledSync.SendDataAsync(ColorArrayToByteArray(colors));
        }

        private byte[] ColorArrayToByteArray(Color[] colors)
        {
            byte[] bytes = new byte[colors.Length * 3];

            for (int i = 0; i < colors.Length; i++)
            {
                bytes[i * 3] = colors[i].R;
                bytes[i * 3 + 1] = colors[i].G;
                bytes[i * 3 + 2] = colors[i].B;
            }

            return bytes;
        }

        public async Task Capture(CancellationToken token)
        {
            sampleAggregator.FftCalculated += new EventHandler<FftEventArgs>(FftCalculated);
            sampleAggregator.PerformFFT = true;

            var capture = new WasapiLoopbackCapture();
            // capture.WaveFormat = new WaveFormat(44100, 16, 2);

            sampleRate = capture.WaveFormat.SampleRate;
            bytesPerSample = capture.WaveFormat.BlockAlign;

            capture.DataAvailable += OnDataAvailable;

            capture.RecordingStopped += (s, a) =>
            {
                capture.Dispose();
            };

            capture.StartRecording();

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(ledSync.config.delay);
            }

            if (token.IsCancellationRequested)
            {
                capture.StopRecording();
            }
        }
    }
}
