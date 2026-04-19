using Ledqualizer;
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
    public partial class UcHue : UserControl
    {
        public event EventHandler? ValueChanged;

        public UcHue()
        {
            InitializeComponent();
        }

        private void pictureHue_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                float xRelativeToImage = (float)e.X / pictureBox.Width;
                double hue = xRelativeToImage * 360; // Common.MapValue(xRelativeToImage, 0, 1, 0, 360);

                trackBarHue.Value = (int)Math.Round(hue);
            }
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("Hue value (0 - 360).")]
        public int Hue
        {
            get { return trackBarHue.Value; }
            set
            {
                trackBarHue.Value = value;
            }
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("Min value.")]
        public int MinVal
        {
            get { return trackBarHue.Minimum; }
            set
            {
                trackBarHue.Minimum = value;
            }
        }

        [Browsable(true)]
        [Category("Color")]
        [Description("Max value.")]
        public int MaxVal
        {
            get { return trackBarHue.Maximum; }
            set
            {
                trackBarHue.Maximum = value;
            }
        }

        public int getHueVal()
        {
            return trackBarHue.Value;
        }

        private void trackBarHue_ValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}
