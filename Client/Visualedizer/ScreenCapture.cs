using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ledqualizer
{
    internal class ScreenCapture
    {
        private LedSync ledSync;
        private PictureBox pictureBox;
        private NumericUpDown numScreenRow;
        private CheckBox chbRevers;

        static List<Color> reducedPixelColors = new List<Color>();

        public ScreenCapture(LedSync ledSync, PictureBox pictureBox, NumericUpDown numScreenRow, CheckBox chbRevers)
        {
            this.ledSync = ledSync;
            this.pictureBox = pictureBox;
            this.numScreenRow = numScreenRow;
            this.chbRevers = chbRevers;
        }

        public static int GetScreenHeight()
        {
            return Screen.PrimaryScreen.Bounds.Height;
        }

        public static int GetScreenWidth()
        {
            return Screen.PrimaryScreen.Bounds.Width;
        }

        private void PaintPixels(object sender, PaintEventArgs e)
        {
            if (reducedPixelColors.Count == 0)
                return;

            PictureBox pictureBox = (PictureBox)sender;
            int pictureBoxWidth = pictureBox.Width;
            int segmentCount = reducedPixelColors.Count;
            int segmentWidth = pictureBoxWidth / segmentCount;

            for (int i = 0; i < segmentCount; i++)
            {
                Color segmentColor = reducedPixelColors[i];
                Brush brush = new SolidBrush(segmentColor);

                int x = i * segmentWidth;
                int width = segmentWidth;

                // If this is the last segment, adjust the width to fill the remaining space
                if (i == segmentCount - 1)
                {
                    width = pictureBoxWidth - (i * segmentWidth);
                }

                e.Graphics.FillRectangle(brush, x, 0, width, pictureBox.Height);
            }

            pictureBox.Width = segmentWidth * segmentCount;
        }

        private Color CalculateAverageColor(List<Color> colors)
        {
            int totalRed = 0;
            int totalGreen = 0;
            int totalBlue = 0;

            foreach (var color in colors)
            {
                totalRed += color.R;
                totalGreen += color.G;
                totalBlue += color.B;
            }

            int averageRed = totalRed / colors.Count;
            int averageGreen = totalGreen / colors.Count;
            int averageBlue = totalBlue / colors.Count;

            return Color.FromArgb(averageRed, averageGreen, averageBlue);
        }

        private byte[] ColorListToByteArray(List<Color> colorList)
        {
            List<byte> byteList = new List<byte>();

            foreach (Color color in colorList)
            {
                byteList.Add(color.R);
                byteList.Add(color.G);
                byteList.Add(color.B);
            }

            byte[] byteArray = byteList.ToArray();

            return byteArray;
        }

        public async Task Capture(CancellationToken token)
        {
            int ledCount = ledSync.config.ledCount;
            int captureY = (int)numScreenRow.Value;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            Bitmap screenCapture = new Bitmap(screenWidth, 1);
            Graphics graphics = Graphics.FromImage(screenCapture);

            pictureBox.Paint += new PaintEventHandler(PaintPixels);

            while (!token.IsCancellationRequested)
            {
                graphics.CopyFromScreen(0, captureY, 0, 0, new Size(screenWidth, 1));

                List<Color> pixelColors = new List<Color>();
                for (int x = 0; x < screenWidth; x++)
                {
                    Color pixelColor = screenCapture.GetPixel(x, 0);
                    pixelColors.Add(pixelColor);
                }

                if (chbRevers.Checked)
                {
                    pixelColors.Reverse();
                }

                int segmentSize = screenWidth / ledCount;
                reducedPixelColors.Clear();

                for (int i = 0; i < ledCount; i++)
                {
                    int startIndex = i * segmentSize;
                    int endIndex = (i + 1) * segmentSize;

                    if (endIndex >= screenWidth)
                    {
                        endIndex = screenWidth - 1;
                    }

                    Color segmentColor = CalculateAverageColor(pixelColors.GetRange(startIndex, endIndex - startIndex + 1));
                    reducedPixelColors.Add(segmentColor);
                }

                pictureBox.Invalidate();

                await ledSync.SendDataAsync(ColorListToByteArray(reducedPixelColors));

                await Task.Delay(20);
            }
        }
    }
}
