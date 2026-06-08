using NAudio.Dsp;
using System.Diagnostics;

namespace Ledqualizer
{
    // https://github.com/SjB/NAudio/blob/master/NAudioWpfDemo/AudioPlaybackDemo/SampleAggregator.cs
    // https://copyprogramming.com/howto/naudio-fft-result-gives-intensity-on-all-frequencies-c
    class SampleAggregator
    {
        // FFT
        // public event EventHandler FftCalculated;
        public event EventHandler<FftEventArgs> FftCalculated;
        public bool PerformFFT { get; set; }
        private Complex[] fftBuffer;
        private int fftPos;
        private int fftLength;
        private int m;

        public SampleAggregator(int fftLength)
        {
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            this.m = (int)Math.Log(fftLength, 2.0);
            this.fftLength = fftLength;
            this.fftBuffer = new Complex[fftLength];
        }

        bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public void Add(double value)
        {
            Add((float)value);
        }

        public void Add(float value)
        {
            var fftCalculated = FftCalculated;
            if (!PerformFFT || fftCalculated == null)
            {
                return;
            }

            fftBuffer[fftPos].X = (float)(value * FastFourierTransform.HammingWindow(fftPos, fftLength));
            fftBuffer[fftPos].Y = 0;
            fftPos++;
            if (fftPos >= fftLength)
            {
                fftPos = 0;

                var fftFrame = new Complex[fftLength];
                Array.Copy(fftBuffer, fftFrame, fftLength);
                FastFourierTransform.FFT(true, m, fftFrame);

                fftCalculated(this, new FftEventArgs(fftFrame));
            }
        }
    }
    public class FftEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public FftEventArgs(Complex[] result)
        {
            this.Result = result;
        }
        public Complex[] Result { get; private set; }
    }
}
