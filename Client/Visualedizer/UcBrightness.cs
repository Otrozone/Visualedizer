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
    public partial class UcBrightness : UserControl
    {
        public UcBrightness()
        {
            InitializeComponent();
        }

        private void pictureBrightness_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox pictureBox)
            {
                float xRelativeToImage = (float)e.X / pictureBox.Width;
                double brightness = xRelativeToImage * 100;

                trackBarBrightness.Value = (int)Math.Round(brightness);
            }
        }
    }
}
