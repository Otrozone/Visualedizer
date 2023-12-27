using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ledqualizer
{
    internal class AudioCaptureVolume
    {
        private LedSync ledSync;
        private AudioCaptureVolumeMode mode;
        private FrmMain frmMain;

        public enum AudioCaptureVolumeMode
        {
            ModeStartToEnd = 0,
            ModeEndToStart = 1,
            ModeMidToOut = 2,
            ModeColorPush = 3,
            ModeMidToOut_Point = 4,
            ModeBrightness = 5
        }

        /*private static AudioCaptureVolume instance;

        public static AudioCaptureVolume GetInstance()
        {
            if (instance == null)
            {
                instance = new AudioCaptureVolume();
            }

            return instance;
        }*/

        delegate void ComputeColorsDelegate(byte[] ledConfigArray, int ledCount, float vol);

        public AudioCaptureVolume(FrmMain frmMain, LedSync ledSync, AudioCaptureVolumeMode mode)
        {
            this.ledSync = ledSync;
            this.mode = mode;
            this.frmMain = frmMain;

            frmMain.progressBar.Invoke((MethodInvoker)delegate { frmMain.progressBar.Value = 0; });
        }

        private Color GetBgColor()
        {
            double saturation = frmMain.chbBgWhite.Checked ? 0 : 1.0;
            double brightness = (double)frmMain.trackBarBgBrightness.Value / 100;
            return Common.HSVToRGB(frmMain.ucHueBg.Hue, saturation, brightness);
        }

        private void Postprocess(byte[] ledConfigArray)
        {
            if (frmMain.chbRevers.Checked)
            {
                Array.Reverse(ledConfigArray);
            }
        }

        private void ComputeColors_StarToEnd(byte[] ledConfigArray, int ledCount, float vol)
        {
            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                if (i < Math.Round(ledCount * vol))
                {
                    double hue = 360 * ((float)i / ledCount);
                    hue = Common.MapValue(hue, 0, 360, frmMain.ucHueMinMax.HueMin, frmMain.ucHueMinMax.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, 1.0, (double)frmMain.trackBarBrightness.Value / frmMain.trackBarBrightness.Maximum);

                    ledConfigArray[idx] = rgbColor.R;
                    ledConfigArray[idx + 1] = rgbColor.G;
                    ledConfigArray[idx + 2] = rgbColor.B;
                }
                else
                {
                    Color bgColor = GetBgColor();
                    ledConfigArray[idx] = bgColor.R;
                    ledConfigArray[idx + 1] = bgColor.G;
                    ledConfigArray[idx + 2] = bgColor.B;
                }
            }

            Postprocess(ledConfigArray);
        }

        private void ComputeColors_EndToStart(byte[] ledConfigArray, int ledCount, float vol)
        {
            for (int i = ledCount - 1; i > 0; i--)
            {
                int idx= i * 3;
                if (i > Math.Round(ledCount * (1 - vol)))
                {
                    double hue = 360 - (360 * ((float)i / ledCount));
                    hue = Common.MapValue(hue, 0, 360, frmMain.ucHueMinMax.HueMin, frmMain.ucHueMinMax.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, 1.0, (double)frmMain.trackBarBrightness.Value / frmMain.trackBarBrightness.Maximum);

                    ledConfigArray[idx] = rgbColor.R;
                    ledConfigArray[idx + 1] = rgbColor.G;
                    ledConfigArray[idx + 2] = rgbColor.B;
                }
                else
                {
                    Color bgColor = GetBgColor();
                    ledConfigArray[idx] = bgColor.R;
                    ledConfigArray[idx + 1] = bgColor.G;
                    ledConfigArray[idx + 2] = bgColor.B;
                }
            }

            Postprocess(ledConfigArray);
        }

        private void ComputeColors_MidToOut(byte[] ledConfigArray, int ledCount, float vol)
        {
            int center = ledCount / 2;

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = (float)distance / center;
                float adjustedVol = vol * (1.0f - distanceFactor);

                if (vol > distanceFactor)
                {
                    double hue = 360 * (frmMain.chbHueRevers.Checked ? 1 - distanceFactor : distanceFactor);
                    double saturation = frmMain.chbWhite.Checked ? 1 : 0;
                    hue = Common.MapValue(hue, 0, 360, frmMain.ucHueMinMax.HueMin, frmMain.ucHueMinMax.HueMax);

                    Color rgbColor = Common.HSVToRGB(hue, 1.0, (double)frmMain.trackBarBrightness.Value / frmMain.trackBarBrightness.Maximum);

                    ledConfigArray[idx] = rgbColor.R;
                    ledConfigArray[idx + 1] = rgbColor.G;
                    ledConfigArray[idx + 2] = rgbColor.B;
                }
                else
                {
                    Color bgColor = GetBgColor();
                    ledConfigArray[idx] = bgColor.R;
                    ledConfigArray[idx + 1] = bgColor.G;
                    ledConfigArray[idx + 2] = bgColor.B;
                }
            }

            Postprocess(ledConfigArray);
        }

        private void ComputeColors_MidToOut_Point(byte[] ledConfigArray, int ledCount, float vol)
        {
            int center = ledCount / 2;
            int pointSize = 10; // half of it 

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = (float)distance / center;
                // float adjustedVol = vol * (1.0f - distanceFactor);

                if (Math.Round(vol * center) > distance - pointSize && Math.Round(vol * center) < distance + pointSize)
                {
                    double hue = 360 * (frmMain.chbHueRevers.Checked ? 1 - distanceFactor : distanceFactor);
                    hue = Common.MapValue(hue, 0, 360, frmMain.ucHueMinMax.HueMin, frmMain.ucHueMinMax.HueMax);
                    Color rgbColor = Common.HSVToRGB(hue, 1.0, (double)frmMain.trackBarBrightness.Value / frmMain.trackBarBrightness.Maximum);

                    ledConfigArray[idx] = rgbColor.R;
                    ledConfigArray[idx + 1] = rgbColor.G;
                    ledConfigArray[idx + 2] = rgbColor.B;
                }
                else
                {
                    Color bgColor = GetBgColor();
                    ledConfigArray[idx] = bgColor.R;
                    ledConfigArray[idx + 1] = bgColor.G;
                    ledConfigArray[idx + 2] = bgColor.B;
                }
            }

            Postprocess(ledConfigArray);
        }


        private void ComputeColors_ColorPush(byte[] ledConfigArray, int ledCount, float vol)
        {
            int center = ledCount / 2;

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = (float)distance / center;
                float adjustedVol = vol * (1.0f - distanceFactor);

                Color rgbColor = Common.HSVToRGB(360 * adjustedVol, 1.0, (double)frmMain.trackBarBrightness.Value / frmMain.trackBarBrightness.Maximum);

                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }

            Postprocess(ledConfigArray);
        }

        private void ComputeColors_Brightness(byte[] ledConfigArray, int ledCount, float vol)
        {
            int center = ledCount / 2;

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                int distance = Math.Abs(i - center);
                float distanceFactor = (float)distance / center;
                float adjustedVol = vol * (1.0f - distanceFactor);
                
                double hue = 360 * (frmMain.chbHueRevers.Checked ? 1 - distanceFactor : distanceFactor);
                hue = Common.MapValue(hue, 0, 360, frmMain.ucHueMinMax.HueMin, frmMain.ucHueMinMax.HueMax);
                Color rgbColor = Common.HSVToRGB(hue, 1.0, adjustedVol * ((double)frmMain.trackBarBrightness.Value / frmMain.trackBarBrightness.Maximum));

                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }

            Postprocess(ledConfigArray);
        }

        public async Task CaptureAudioAsync(CancellationToken token)
        {
            int ledCount = ledSync.config.ledCount;

            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            while (!token.IsCancellationRequested)
            {
                float vol = defaultDevice.AudioMeterInformation.MasterPeakValue;
                int percVol = (int)Math.Round(vol * 100);

                if (frmMain.progressBar != null && !frmMain.progressBar.IsDisposed && !frmMain.progressBar.Disposing)
                {
                    frmMain.progressBar.Invoke((MethodInvoker)delegate { frmMain.progressBar.Value = percVol; });
                }

                byte[] ledConfigArray = new byte[ledCount * 3];

                ComputeColorsDelegate computeColorDelegate = null;
                if (frmMain.rbModeStartToEnd.Checked)
                {
                    computeColorDelegate = ComputeColors_StarToEnd;
                } 
                else if (frmMain.rbModeEndToStart.Checked) 
                {
                    computeColorDelegate = ComputeColors_EndToStart;
                }
                else if (frmMain.rbModeMidToOut.Checked)
                {
                    computeColorDelegate = ComputeColors_MidToOut;
                }
                else if (frmMain.rbModeColorPush.Checked)
                {
                    computeColorDelegate = ComputeColors_ColorPush;
                }
                else if (frmMain.rbModeMidToOutPoint.Checked)
                {
                    computeColorDelegate = ComputeColors_MidToOut_Point;
                }
                else if (frmMain.rbBrightness.Checked)
                {
                    computeColorDelegate = ComputeColors_Brightness;
                }

                    if (computeColorDelegate != null)
                {
                    computeColorDelegate(ledConfigArray, ledCount, vol);
                }

                await ledSync.SendDataAsync(ledConfigArray);

                await Task.Delay(ledSync.config.delay);
            }
        }

    }
}
