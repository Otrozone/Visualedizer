using Ledqualizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualedizer
{
    internal class Basic
    {
        private LedSync ledSync;
        private FrmMain frmMain;

        public Basic(FrmMain frmMain, LedSync ledSync)
        {
            this.ledSync = ledSync;
            this.frmMain = frmMain;
        }

        private void SetColorSolid(byte[] ledConfigArray, int ledCount)
        {
            double hue = Common.MapValue(frmMain.ucHueSolid.Hue, 0, 360, frmMain.ucHueSolid.MinVal, frmMain.ucHueSolid.MaxVal);
            double saturation = Common.MapValue(frmMain.trackSaturationBasic.Value, frmMain.trackSaturationBasic.Minimum, frmMain.trackSaturationBasic.Maximum, 0, 1.0);
            double brightness = (double)frmMain.trackBrightnessBasic.Value / frmMain.trackBarBrightness.Maximum;
            Color rgbColor = Common.HSVToRGB(hue, saturation, brightness);

            for (int i = 0; i < ledCount; i++)
            {
                int idx = i * 3;
                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }
        }

        private void SetColorGradient(byte[] ledConfigArray, int ledCount)
        {
            double saturation = Common.MapValue(frmMain.trackSaturationBasic.Value, frmMain.trackSaturationBasic.Minimum, frmMain.trackSaturationBasic.Maximum, 0, 1.0);
            double brightness = (double)frmMain.trackBrightnessBasic.Value / frmMain.trackBarBrightness.Maximum;
            for (int i = 0; i < ledCount; i++)
            {
                double hue = Common.MapValue(i, 0, ledCount, frmMain.ucHueMinMaxGradient.HueMin, frmMain.ucHueMinMaxGradient.HueMax);
                Color rgbColor = Common.HSVToRGB(hue, saturation, brightness);
                int idx = i * 3;
                ledConfigArray[idx] = rgbColor.R;
                ledConfigArray[idx + 1] = rgbColor.G;
                ledConfigArray[idx + 2] = rgbColor.B;
            }
        }

        public async Task BasicOperations(CancellationToken token)
        {
            int ledCount = ledSync.config.ledCount;

            while (!token.IsCancellationRequested)
            {
                byte[] ledConfigArray = new byte[ledCount * 3];
                if (frmMain.rbSolid.Checked)
                {
                    SetColorSolid(ledConfigArray, ledCount);
                }

                if (frmMain.rbGradient.Checked)
                {
                    SetColorGradient(ledConfigArray, ledCount);
                }

                await ledSync.SendDataAsync(ledConfigArray);
                await Task.Delay(ledSync.config.delay);
            }
        }
    }
}
