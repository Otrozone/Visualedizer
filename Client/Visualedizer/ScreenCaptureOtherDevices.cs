using Ledqualizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Visualedizer
{
    internal class ScreenCaptureOtherDevices
    {
        private LedSync ledSync;
        /*private NumericUpDown numStrobeX;
        private NumericUpDown numStrobeY;
        private NumericUpDown numLaserX;
        private NumericUpDown numLaserY;
        private NumericUpDown numLaserPatternX;
        private NumericUpDown numLaserPatternY;
        private NumericUpDown numLaserColorX;
        private NumericUpDown numLaserColorY;*/

        private int strobeTriggerX { get; set; }
        private int strobeTriggerY { get; set; }
        private int laserTriggerX { get; set; }
        private int laserTriggerY { get; set; }
        private int laserColorX { get; set; }
        private int laserPatternX { get; set; }
        private int laserPatternY { get; set; }
        private int laserColorY { get; set; }


        private const int TRIGGER_THRESHOLD = 100;

        public ScreenCaptureOtherDevices(LedSync ledSync) // , NumericUpDown numLaserX, NumericUpDown numLaserY, NumericUpDown numStrobeX, NumericUpDown numStrobeY
        {
            this.ledSync = ledSync;

            Config config = ledSync.config;

            /*this.numLaserX = numLaserX;
            this.numLaserY = numLaserY;
            this.numStrobeX = numStrobeX;
            this.numStrobeY = numStrobeY;*/

            this.strobeTriggerX = config.strobeTriggerX;
            this.strobeTriggerY = config.strobeTriggerY;
            this.laserTriggerX = config.laserTriggerX;
            this.laserTriggerY = config.laserTriggerY;
            this.laserPatternX = config.laserPatternX;
            this.laserPatternY = config.laserPatternY;
            this.laserColorY = config.laserColorY;
            this.laserColorX = config.laserColorX;
        }

        private Color getPixelColor(Bitmap screenCapture, Graphics graphics, int x, int y)
        {
            Color pixelColor = new Color();
            graphics.CopyFromScreen(x, y, 0, 0, new Size(1, 1));
            pixelColor = screenCapture.GetPixel(0, 0);

            return pixelColor;
        }

        public async Task Capture(CancellationToken token)
        {
            Bitmap screenCapture = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(screenCapture);

            while (!token.IsCancellationRequested)
            {
                byte[] data = new byte[7];

                // Laser
                Color pixelColor = new Color();
                pixelColor = getPixelColor(screenCapture, graphics, laserTriggerX, laserTriggerY);

                // Laser trigger
                if (pixelColor.R + pixelColor.G + pixelColor.B > TRIGGER_THRESHOLD)
                {
                    data[0] = (byte)1;
                }
                else
                {
                    data[0] = (byte)0;
                }

                data[1] = (byte)0;
                data[2] = (byte)240;

                // Laser pattern
                pixelColor = getPixelColor(screenCapture, graphics, laserPatternX, laserPatternY);
                byte avgVal = (byte)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                
                data[3] = avgVal;
                // data[3] = (byte)220;

                // Laser color
                pixelColor = getPixelColor(screenCapture, graphics, laserColorX, laserColorY);

                data[4] = (byte)0;
                data[5] = (byte)0;

                // Strobe
                pixelColor = getPixelColor(screenCapture, graphics, strobeTriggerX, strobeTriggerY);

                // byte[] dataStrobe = new byte[1];
                // dataStrobe[0] = (byte)255;

                if (pixelColor.R + pixelColor.G + pixelColor.B > TRIGGER_THRESHOLD)
                {
                    data[6] = (byte)1;
                } 
                else
                {
                    data[6] = (byte)0;
                }

                await ledSync.SendDataAsync(data);

                await Task.Delay(20);
            }
        }
    }
}
