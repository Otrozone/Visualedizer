using NAudio.CoreAudioApi;
using System.Threading;
using System.Windows.Forms;
using static Ledqualizer.ScreenCapture;
// https://github.com/naudio/NAudio

namespace Ledqualizer
{
    public partial class FrmMain : Form
    {
        private Task taskMain;
        private CancellationTokenSource ctsMain;

        private Task taskScreenDraw;
        private CancellationTokenSource ctsScreenDraw;

        private LedSync ledSync;

        private bool isDragging = false;
        private int scrollValue = 0;
        private AudioCaptureVolume.AudioCaptureVolumeMode audioCaptureVolumeMode;

        Config config = new Config();

        private int count = 0;

        private int rotateIdx = 0;
        RadioButton[] rotateModeRadios;

        public class FormOverlay : Form
        {
            public FormOverlay(Rectangle rectangle)
            {
                FormBorderStyle = FormBorderStyle.None;
                BackColor = Color.WhiteSmoke;
                Opacity = 0.5;
                TopMost = true;
                StartPosition = FormStartPosition.Manual;
                Location = rectangle.Location;
                Size = rectangle.Size;

                // Draw the red rectangle
                Paint += (sender, e) =>
                {
                    using (Pen redPen = new Pen(Color.Red, 1))
                    {
                        e.Graphics.DrawRectangle(redPen, new Rectangle(0, 17, Width - 1, 3));
                    }
                };
            }
        }
        private FormOverlay frmOverlay;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            hsbScreenRowSelector.Maximum = ScreenCapture.GetScreenHeight();
            numScreenRow.Maximum = hsbScreenRowSelector.Maximum;
            CountHz();

            pnlBackgroundColor.BackColor = Color.Black;
            rotateModeRadios = new RadioButton[] { rbModeColorPush, rbModeEndToStart, rbModeMidToOut, rbModeMidToOutPoint, rbModeStartToEnd };
        }

        private async void btnCaptureStart_Click(object sender, EventArgs e)
        {
        }

        private void btnScreenCapture_Click(object sender, EventArgs e)
        {

            /// screenCaptureCts = new CancellationTokenSource();
            // screenCaptureTask = Task.Run(() => ScreenCapture.Capture(screenCaptureCts.Token, ledCount, pictureBox, (int)numScreenRow.Value));
        }


        private async void DrawRectangle(int y)
        {
            /*ctsScreenDraw.Cancel();
            ctsScreenDraw = new CancellationTokenSource();
            await ScreenDrawing.DrawRect(ctsScreenDraw.Token);*/
        }

        private async void hsbScreenRowSelector_Scroll(object sender, ScrollEventArgs e)
        {
            numScreenRow.Value = hsbScreenRowSelector.Value;

            if (frmOverlay != null)
            {
                frmOverlay.Location = new Point(0, (int)numScreenRow.Value);
            }

            /*
            if (e.Type == ScrollEventType.ThumbTrack)
            {
                int y = hsbScreenRowSelector.Value;
                ScreenDrawing.left = 1;
                ScreenDrawing.right = ScreenCapture.GetScreenWidth() - 1;
                ScreenDrawing.top = y - 1;
                ScreenDrawing.bottom = y + 1;

                if (taskScreenDraw == null)
                {
                    count++;
                    statLblConnection.Text = count.ToString();

                    ctsScreenDraw = new CancellationTokenSource();
                    taskScreenDraw = Task.Run(() => ScreenDrawing.DrawLine(ctsScreenDraw.Token));
                }
            } 
            else if (e.Type == ScrollEventType.EndScroll)
            {
                if (ctsScreenDraw != null)
                {
                    ctsScreenDraw.Cancel();
                }
                if (taskScreenDraw != null)
                {
                }
            }
            
            if (taskScreenDraw == null)
            {
                int y = hsbScreenRowSelector.Value;
                ScreenDrawing.left = 1;
                ScreenDrawing.right = ScreenCapture.GetScreenWidth() - 1;
                ScreenDrawing.top = y - 1;
                ScreenDrawing.bottom = y + 1;
            }*/
        }

        private Config GetConfig()
        {
            config = new Config()
            {
                delay = (int)numDelay.Value,
                ledCount = (int)numLedCount.Value,
                ipAddress = textIpAddress.Text,
                brightness = (float)trackBarBrightness.Value / trackBarBrightness.Maximum
            };

            return config;
        }

        private async void btnInitiate_Click(object sender, EventArgs e)
        {
            GetConfig();
            ctsMain = new CancellationTokenSource();

            // LedSync ledSync = LedSync.GetInstance(config);
            LedSync ledSync = new LedSync(config);
            await ledSync.ConnectAsync();

            switch (tabControl.SelectedIndex)
            {
                case 0:
                    // AudioCaptureVolume
                    AudioCaptureVolume audioCaptureVolume = new AudioCaptureVolume(this, ledSync, audioCaptureVolumeMode);
                    await audioCaptureVolume.CaptureAudioAsync(ctsMain.Token);
                    break;
                case 1:
                    AudioCaptureEqualizer audioCaptureEqualizer = new AudioCaptureEqualizer(ledSync);
                    await audioCaptureEqualizer.Capture(ctsMain.Token);
                    break;
                case 2:
                    ScreenCapture screenCapture = new ScreenCapture(ledSync, pictureBox, numScreenRow, chbReverse);
                    await screenCapture.Capture(ctsMain.Token);
                    break;
            }
            await ledSync.DisconnectAsync();
        }

        private void btnTerminate_Click(object sender, EventArgs e)
        {
            if (ctsMain != null)
            {
                ctsMain.Cancel();
            }
        }

        public void ShowOverlayForm(int y)
        {
            Rectangle captureArea = new Rectangle(0, y - 18, GetScreenWidth() - 1, 3);
            frmOverlay = new FormOverlay(captureArea);
            frmOverlay.Show();
        }

        public void CloseOverlayForm()
        {
            if (frmOverlay != null)
            {
                frmOverlay.Close();
                frmOverlay.Dispose();
                frmOverlay = null;
            }
        }

        private void chbShowGuide_CheckedChanged(object sender, EventArgs e)
        {
            if (chbShowGuide.Checked)
            {
                ShowOverlayForm((int)numScreenRow.Value);
            }
            else
            {
                CloseOverlayForm();
            }
        }

        private void CountHz()
        {
            lblRefreshRate.Text = (1000 / numDelay.Value).ToString("F1") + " Hz";
        }

        private void numDelay_ValueChanged(object sender, EventArgs e)
        {
            CountHz();
        }

        private void rbMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbModeStartToEnd.Checked)
            {
                audioCaptureVolumeMode = AudioCaptureVolume.AudioCaptureVolumeMode.ModeStartToEnd;
            }
            if (rbModeEndToStart.Checked)
            {
                audioCaptureVolumeMode = AudioCaptureVolume.AudioCaptureVolumeMode.ModeEndToStart;
            }
            if (rbModeMidToOut.Checked)
            {
                audioCaptureVolumeMode = AudioCaptureVolume.AudioCaptureVolumeMode.ModeMidToOut;
            }
            if (rbModeColorPush.Checked)
            {
                audioCaptureVolumeMode = AudioCaptureVolume.AudioCaptureVolumeMode.ModeColorPush;
            }
            if (rbModeMidToOutPoint.Checked)
            {
                audioCaptureVolumeMode = AudioCaptureVolume.AudioCaptureVolumeMode.ModeMidToOut_Point;
            }
        }

        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
            config = GetConfig();
        }

        private void pnlBackgroundColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.AllowFullOpen = true;
            colorDialog.AnyColor = true;

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pnlBackgroundColor.BackColor = colorDialog.Color;
            }
        }

        private void chbWhite_CheckedChanged(object sender, EventArgs e)
        {
            ucHueMinMax.Enabled = !chbWhite.Checked;
        }

        private void chbRotate_CheckedChanged(object sender, EventArgs e)
        {
            timerRotate.Enabled = chbRotate.Checked;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            timerRotate.Interval = trackBarRotate.Value * 1000;
        }

        private void timerRotate_Tick(object sender, EventArgs e)
        {
            if (rotateIdx >= rotateModeRadios.Length)
            {
                rotateIdx = 0;
            }
            rotateModeRadios[rotateIdx].Checked = true;
            rotateIdx++;
        }
    }
}