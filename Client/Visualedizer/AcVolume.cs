using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ledqualizer
{
    internal class AcVolume
    {
        private LedSync ledSync;
        private AudioCaptureVolumeMode mode;
        private FrmMain frmMain;

        public MMDevice audioDevice { get; set; }

        public enum AudioCaptureVolumeMode
        {
            ModeStartToEnd = 0,
            ModeEndToStart = 1,
            ModeMidToOut = 2,
            ModeColorPush = 3,
            ModeMidToOut_Point = 4,
            ModeBrightness = 5
        }

        public class DeviceDescriptor
        {
            public MMDevice Device { get; set; }
            public string Text { get; set; }
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

        public AcVolume(FrmMain frmMain, LedSync ledSync, AudioCaptureVolumeMode mode)
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

        private void ComputeColors_StartToEnd(byte[] ledConfigArray, int ledCount, float vol)
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

        public void CbAudioDevices_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            if (sender is ComboBox comboBox)
            {
                var selectedDeviceItem = comboBox.SelectedItem as DeviceDescriptor;
                var selectedDevice = selectedDeviceItem?.Device;
                if (selectedDevice != null)
                {
                    audioDevice = selectedDevice;
                }
            }
        }

        public static void LoadAudioDevicesToComboBox(ComboBox comboBox)
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            var defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            comboBox.Items.Clear();
            foreach (var device in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                var item = new DeviceDescriptor
                {
                    Device = device,
                    Text = device.FriendlyName
                };

                if (device.ID.Equals(defaultDevice.ID))
                {
                    item.Text += " [default]";
                }

                int idx = comboBox.Items.Add(item);

                if (device.ID.Equals(defaultDevice.ID))
                {
                    comboBox.SelectedIndex = idx;
                }
            }

            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Device";
            
        }

        public async Task CaptureAudioAsync(CancellationToken token)
        {
            int ledCount = ledSync.config.ledCount;

            // MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            // MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            // var selectedDeviceItem = frmMain.cbAudioDevices.SelectedItem as DeviceDescriptor;
            // var selectedDevice = selectedDeviceItem?.Device;
            
            int tick = Environment.TickCount;
            int iterations = 0;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!token.IsCancellationRequested && audioDevice != null)
            {
                float vol = audioDevice.AudioMeterInformation.MasterPeakValue;
                int percVol = (int)Math.Round(vol * 100);

                if (frmMain.progressBar != null && !frmMain.progressBar.IsDisposed && !frmMain.progressBar.Disposing)
                {
                    frmMain.progressBar.Invoke((MethodInvoker)delegate { frmMain.progressBar.Value = percVol; });
                }

                byte[] ledConfigArray = new byte[ledCount * 3];

                ComputeColorsDelegate computeColorDelegate = null;
                if (frmMain.rbModeStartToEnd.Checked)
                {
                    computeColorDelegate = ComputeColors_StartToEnd;
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
                    float normalizedVol = ((float)frmMain.trackBarNormalizationLevel.Value / 10) * vol;
                    computeColorDelegate(ledConfigArray, ledCount, normalizedVol);
                }

                await ledSync.SendDataAsync(ledConfigArray);

                tick = Environment.TickCount;
                iterations++;

                if (stopwatch.ElapsedMilliseconds >= 1000)
                {
                    Console.WriteLine($"Rate: {iterations}");
                    frmMain.statusStrip.Items[0].Text = $"Rate: {iterations}";
                    iterations = 0;
                    stopwatch.Restart();
                }

                await Task.Delay(ledSync.config.delay);
            }
        }

    }
}
