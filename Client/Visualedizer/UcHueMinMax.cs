using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Visualedizer
{
    public partial class UcHueMinMax : UserControl
    {
        public UcHueMinMax()
        {
            InitializeComponent();
        }

        private void pictureHue_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                float xRelativeToImage = (float)e.X / pictureBox.Width;
                double hue = xRelativeToImage * 360; // Common.MapValue(xRelativeToImage, 0, 1, 0, 360);

                trackBarHueMin.Value = (int)Math.Round(hue);
            }
        }

        private void trackBarHueMax_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarHueMax.Value < trackBarHueMin.Value)
            {
                trackBarHueMin.Value = Math.Max(trackBarHueMax.Value - 1, trackBarHueMax.Minimum);
            }
        }

        private void trackBarHueMin_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarHueMin.Value > trackBarHueMax.Value)
            {
                trackBarHueMax.Value = Math.Min(trackBarHueMin.Value + 1, trackBarHueMin.Maximum);
            }
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("Hue value (0 - 360).")]
        public int HueMin
        {
            get { return trackBarHueMin.Value; }
            set
            {
                trackBarHueMin.Value = value;
            }
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("Hue value (0 - 360).")]
        public int HueMax
        {
            get { return trackBarHueMax.Value; }
            set
            {
                trackBarHueMax.Value = value;
            }
        }

    }
}
