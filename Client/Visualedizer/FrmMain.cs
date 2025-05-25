using NAudio.CoreAudioApi;
using System.Threading;
using System.Windows.Forms;
using Visualedizer;
using static Ledqualizer.AcVolume;
using static Ledqualizer.ScreenCapture;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
        private AcVolume.AudioCaptureVolumeMode audioCaptureVolumeMode;

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

        private void InitConfFrmFields()
        {
            textIpAddress.Text = config.ipAddress;
            numLedCount.Value = config.ledCount;
            numDelay.Value = config.delay;

            numScreenRow.Value = config.screenCaptureRow;

            numStrobeX.Value = config.strobeTriggerX;
            numStrobeY.Value = config.strobeTriggerY;

            numLaserTriggerX.Value = config.laserTriggerX;
            numLaserTriggerY.Value = config.laserTriggerY;
            numLaserPatternX.Value = config.laserPatternX;
            numLaserPatternY.Value = config.laserPatternY;
            numLaserColorX.Value = config.laserColorX;
            numLaserColorY.Value = config.laserColorY;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            hsbScreenRowSelector.Maximum = ScreenCapture.GetScreenHeight();
            numScreenRow.Maximum = hsbScreenRowSelector.Maximum;
            CountHz();

            rotateModeRadios = new RadioButton[] { rbModeColorPush, rbModeEndToStart, rbModeMidToOut, rbModeMidToOutPoint, rbModeStartToEnd };

            tabControl.TabPages.Remove(tabPageAcSpectralAnalysis); // Not yet fully implemented

            config.LoadFromIni();
            InitConfFrmFields();
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

        /* private Config GetConfig()
        {
            config.delay = (int)numDelay.Value;
            config.ledCount = (int)numLedCount.Value;
            config.ipAddress = textIpAddress.Text;
            config.brightness = (float)trackBarBrightness.Value / trackBarBrightness.Maximum;
            config.normalizationLevel = (float)trackBarNormalizationLevel.Value;

            return config;
        }*/

        private async void btnInitiate_Click(object sender, EventArgs e)
        {
            // GetConfig();
            ctsMain = new CancellationTokenSource();

            // LedSync ledSync = LedSync.GetInstance(config);
            LedSync ledSync = new LedSync(config);
            await ledSync.ConnectAsync();

            if (tabControl.SelectedTab == tabPageBasicControl)
            {
                Basic basic = new Basic(this, ledSync);
                await basic.BasicOperations(ctsMain.Token);
            }
            else if (tabControl.SelectedTab == tabPageAcVolume)
            {
                // AudioCaptureVolume
                AcVolume audioCaptureVolume = new AcVolume(this, ledSync, audioCaptureVolumeMode);

                var selectedDeviceItem = cbAudioDevices.SelectedItem as DeviceDescriptor;
                var selectedDevice = selectedDeviceItem?.Device;
                if (selectedDevice != null)
                {
                    audioCaptureVolume.audioDevice = selectedDevice;
                }
                cbAudioDevices.SelectedIndexChanged += audioCaptureVolume.CbAudioDevices_SelectedIndexChanged;

                await audioCaptureVolume.CaptureAudioAsync(ctsMain.Token);
            }
            else if (tabControl.SelectedTab == tabPageScreenCapture)
            {
                ScreenCapture screenCapture = new ScreenCapture(ledSync, pictureBox, numScreenRow, chbReverse);
                await screenCapture.Capture(ctsMain.Token);
            }
            else if (tabControl.SelectedTab == tabPageAcSpectralAnalysis)
            {
                AcSpectralAnalysis audioCaptureEqualizer = new AcSpectralAnalysis(ledSync);
                await audioCaptureEqualizer.Capture(ctsMain.Token);
            }
            else if (tabControl.SelectedTab == tabPageOtherDevices)
            {
                ScreenCaptureOtherDevices screenCaptureOtherDevices =
                    new ScreenCaptureOtherDevices(ledSync);
                await screenCaptureOtherDevices.Capture(ctsMain.Token);
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
            if (numDelay.Value > 0)
            {
                lblRefreshRate.Text = (1000 / numDelay.Value).ToString("F1") + " Hz";
            }
        }

        private void numDelay_ValueChanged(object sender, EventArgs e)
        {
            config.delay = (int)numDelay.Value;
            CountHz();
        }

        private void rbMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbModeStartToEnd.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeStartToEnd;
            }
            if (rbModeEndToStart.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeEndToStart;
            }
            if (rbModeMidToOut.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeMidToOut;
            }
            if (rbModeColorPush.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeColorPush;
            }
            if (rbModeMidToOutPoint.Checked)
            {
                audioCaptureVolumeMode = AcVolume.AudioCaptureVolumeMode.ModeMidToOut_Point;
            }
        }

        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
            // config = GetConfig();
        }

        private void pnlBackgroundColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.AllowFullOpen = true;
            colorDialog.AnyColor = true;

            /*if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pnlBackgroundColor.BackColor = colorDialog.Color;
            }*/
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

        private void trackBarNormalizationLevel_Scroll(object sender, EventArgs e)
        {
            // config = GetConfig();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            config.SaveToIni();
        }

        private void textIpAddress_TextChanged(object sender, EventArgs e)
        {
            config.ipAddress = textIpAddress.Text;
        }

        private void numLedCount_ValueChanged(object sender, EventArgs e)
        {
            config.ledCount = (int)numLedCount.Value;
        }

        private void numScreenRow_ValueChanged(object sender, EventArgs e)
        {
            config.screenCaptureRow = (int)numScreenRow.Value;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnTerminate_Click(sender, e);

            if (tabControl.SelectedTab == tabPageAcVolume)
            {
                AcVolume.LoadAudioDevicesToComboBox(cbAudioDevices);
            }

            btnInitiate_Click(sender, e);
        }

        private void rbBasic_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton[] radioButtonGroup = { rbSolid, rbGradient };

            if (sender is RadioButton changedRadioButton)
            {
                if (changedRadioButton == null || !changedRadioButton.Checked)
                    return;

                foreach (var radioButton in radioButtonGroup)
                {
                    if (radioButton != changedRadioButton)
                    {
                        radioButton.Checked = false;
                    }
                }
            }
        }

        private void numStrobeX_ValueChanged(object sender, EventArgs e)
        {
            config.strobeTriggerX = (int)numStrobeX.Value;
        }

        private void numStrobeY_ValueChanged(object sender, EventArgs e)
        {
            config.strobeTriggerY = (int)numStrobeY.Value;
        }

        private void numLaserTriggerX_ValueChanged(object sender, EventArgs e)
        {
            config.laserTriggerX = (int)numLaserTriggerX.Value;
        }

        private void numLaserTriggerY_ValueChanged(object sender, EventArgs e)
        {
            config.laserTriggerY = (int)numLaserTriggerY.Value;
        }

        private void numLaserPatternX_ValueChanged(object sender, EventArgs e)
        {
            config.laserPatternX = (int)numLaserPatternX.Value;
        }

        private void numLaserPatternY_ValueChanged(object sender, EventArgs e)
        {
            config.laserPatternY = (int)numLaserPatternY.Value;
        }

        private void numLaserColorX_ValueChanged(object sender, EventArgs e)
        {
            config.laserColorX = (int)numLaserColorX.Value;
        }

        private void numLaserColorY_ValueChanged(object sender, EventArgs e)
        {
            config.laserColorY = (int)numLaserColorY.Value;
        }
    }
}