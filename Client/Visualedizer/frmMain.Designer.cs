namespace Ledqualizer
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            progressBar = new ProgressBar();
            pictureBox = new PictureBox();
            hsbScreenRowSelector = new HScrollBar();
            lblScreenRow = new Label();
            numScreenRow = new NumericUpDown();
            pnlScreenRowSelector = new Panel();
            statusStrip = new StatusStrip();
            statLblConnection = new ToolStripStatusLabel();
            tabControl = new TabControl();
            tabPageAudioCaptureVolume = new TabPage();
            trackBarRotate = new TrackBar();
            chbRotate = new CheckBox();
            rbBrightness = new RadioButton();
            chbHueRevers = new CheckBox();
            chbRevers = new CheckBox();
            ucHueMinMax = new Visualedizer.UcHueMinMax();
            chbWhite = new CheckBox();
            gbBackground = new GroupBox();
            lblBgHue = new Label();
            chbBgWhite = new CheckBox();
            ucHueBg = new Visualedizer.UcHue();
            trackBarBgBrightness = new TrackBar();
            lblBgBrightness = new Label();
            lblBackgroundColor = new Label();
            pnlBackgroundColor = new Panel();
            lblHueMax = new Label();
            lblHueMin = new Label();
            lblBrightness = new Label();
            trackBarBrightness = new TrackBar();
            rbModeMidToOutPoint = new RadioButton();
            rbModeColorPush = new RadioButton();
            rbModeMidToOut = new RadioButton();
            rbModeEndToStart = new RadioButton();
            rbModeStartToEnd = new RadioButton();
            lblPreview = new Label();
            tabPageAudioCaptureEqualizer = new TabPage();
            tabPageScreenCapture = new TabPage();
            chbReverse = new CheckBox();
            chbShowGuide = new CheckBox();
            lblScreenRowCapturePreview = new Label();
            panel1 = new Panel();
            btnTerminate = new Button();
            btnInitiate = new Button();
            numLedCount = new NumericUpDown();
            lblLedCount = new Label();
            lblRefreshRate = new Label();
            lblDelay = new Label();
            numDelay = new NumericUpDown();
            textIpAddress = new TextBox();
            lblHostname = new Label();
            colorBackground = new ColorDialog();
            timerRotate = new System.Windows.Forms.Timer(components);
            trackBarLevel = new TrackBar();
            lblLevel = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numScreenRow).BeginInit();
            pnlScreenRowSelector.SuspendLayout();
            statusStrip.SuspendLayout();
            tabControl.SuspendLayout();
            tabPageAudioCaptureVolume.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarRotate).BeginInit();
            gbBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBgBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            tabPageScreenCapture.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numLedCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLevel).BeginInit();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(8, 551);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(785, 25);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 3;
            // 
            // pictureBox
            // 
            pictureBox.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.Location = new Point(8, 142);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(638, 28);
            pictureBox.TabIndex = 4;
            pictureBox.TabStop = false;
            // 
            // hsbScreenRowSelector
            // 
            hsbScreenRowSelector.Dock = DockStyle.Fill;
            hsbScreenRowSelector.Location = new Point(0, 0);
            hsbScreenRowSelector.Name = "hsbScreenRowSelector";
            hsbScreenRowSelector.Size = new Size(565, 22);
            hsbScreenRowSelector.TabIndex = 6;
            hsbScreenRowSelector.Scroll += hsbScreenRowSelector_Scroll;
            // 
            // lblScreenRow
            // 
            lblScreenRow.AutoSize = true;
            lblScreenRow.Location = new Point(8, 12);
            lblScreenRow.Name = "lblScreenRow";
            lblScreenRow.Size = new Size(65, 15);
            lblScreenRow.TabIndex = 7;
            lblScreenRow.Text = "Screen row";
            // 
            // numScreenRow
            // 
            numScreenRow.Location = new Point(8, 30);
            numScreenRow.Name = "numScreenRow";
            numScreenRow.ReadOnly = true;
            numScreenRow.Size = new Size(65, 23);
            numScreenRow.TabIndex = 8;
            // 
            // pnlScreenRowSelector
            // 
            pnlScreenRowSelector.BorderStyle = BorderStyle.FixedSingle;
            pnlScreenRowSelector.Controls.Add(hsbScreenRowSelector);
            pnlScreenRowSelector.Location = new Point(79, 30);
            pnlScreenRowSelector.Name = "pnlScreenRowSelector";
            pnlScreenRowSelector.Size = new Size(567, 24);
            pnlScreenRowSelector.TabIndex = 9;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statLblConnection });
            statusStrip.Location = new Point(0, 726);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(807, 22);
            statusStrip.TabIndex = 10;
            statusStrip.Text = "statusStrip1";
            // 
            // statLblConnection
            // 
            statLblConnection.Name = "statLblConnection";
            statLblConnection.Size = new Size(79, 17);
            statLblConnection.Text = "Disconnected";
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPageAudioCaptureVolume);
            tabControl.Controls.Add(tabPageAudioCaptureEqualizer);
            tabControl.Controls.Add(tabPageScreenCapture);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 112);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(807, 614);
            tabControl.TabIndex = 11;
            // 
            // tabPageAudioCaptureVolume
            // 
            tabPageAudioCaptureVolume.Controls.Add(lblLevel);
            tabPageAudioCaptureVolume.Controls.Add(trackBarLevel);
            tabPageAudioCaptureVolume.Controls.Add(trackBarRotate);
            tabPageAudioCaptureVolume.Controls.Add(chbRotate);
            tabPageAudioCaptureVolume.Controls.Add(rbBrightness);
            tabPageAudioCaptureVolume.Controls.Add(chbHueRevers);
            tabPageAudioCaptureVolume.Controls.Add(chbRevers);
            tabPageAudioCaptureVolume.Controls.Add(ucHueMinMax);
            tabPageAudioCaptureVolume.Controls.Add(chbWhite);
            tabPageAudioCaptureVolume.Controls.Add(gbBackground);
            tabPageAudioCaptureVolume.Controls.Add(pnlBackgroundColor);
            tabPageAudioCaptureVolume.Controls.Add(lblHueMax);
            tabPageAudioCaptureVolume.Controls.Add(lblHueMin);
            tabPageAudioCaptureVolume.Controls.Add(lblBrightness);
            tabPageAudioCaptureVolume.Controls.Add(trackBarBrightness);
            tabPageAudioCaptureVolume.Controls.Add(rbModeMidToOutPoint);
            tabPageAudioCaptureVolume.Controls.Add(rbModeColorPush);
            tabPageAudioCaptureVolume.Controls.Add(rbModeMidToOut);
            tabPageAudioCaptureVolume.Controls.Add(rbModeEndToStart);
            tabPageAudioCaptureVolume.Controls.Add(rbModeStartToEnd);
            tabPageAudioCaptureVolume.Controls.Add(lblPreview);
            tabPageAudioCaptureVolume.Controls.Add(progressBar);
            tabPageAudioCaptureVolume.Location = new Point(4, 24);
            tabPageAudioCaptureVolume.Name = "tabPageAudioCaptureVolume";
            tabPageAudioCaptureVolume.Padding = new Padding(3);
            tabPageAudioCaptureVolume.Size = new Size(799, 586);
            tabPageAudioCaptureVolume.TabIndex = 0;
            tabPageAudioCaptureVolume.Text = "Audio capture volume";
            tabPageAudioCaptureVolume.UseVisualStyleBackColor = true;
            // 
            // trackBarRotate
            // 
            trackBarRotate.Location = new Point(7, 233);
            trackBarRotate.Maximum = 300;
            trackBarRotate.Minimum = 1;
            trackBarRotate.Name = "trackBarRotate";
            trackBarRotate.Size = new Size(155, 45);
            trackBarRotate.SmallChange = 10;
            trackBarRotate.TabIndex = 9;
            trackBarRotate.Value = 20;
            trackBarRotate.ValueChanged += trackBar1_ValueChanged;
            // 
            // chbRotate
            // 
            chbRotate.AutoSize = true;
            chbRotate.Location = new Point(8, 213);
            chbRotate.Name = "chbRotate";
            chbRotate.Size = new Size(60, 19);
            chbRotate.TabIndex = 28;
            chbRotate.Text = "Rotate";
            chbRotate.UseVisualStyleBackColor = true;
            chbRotate.CheckedChanged += chbRotate_CheckedChanged;
            // 
            // rbBrightness
            // 
            rbBrightness.AutoSize = true;
            rbBrightness.Location = new Point(8, 172);
            rbBrightness.Name = "rbBrightness";
            rbBrightness.Size = new Size(80, 19);
            rbBrightness.TabIndex = 13;
            rbBrightness.TabStop = true;
            rbBrightness.Text = "Brightness";
            rbBrightness.UseVisualStyleBackColor = true;
            // 
            // chbHueRevers
            // 
            chbHueRevers.AutoSize = true;
            chbHueRevers.Location = new Point(664, 244);
            chbHueRevers.Name = "chbHueRevers";
            chbHueRevers.Size = new Size(60, 19);
            chbHueRevers.TabIndex = 27;
            chbHueRevers.Text = "Revers";
            chbHueRevers.UseVisualStyleBackColor = true;
            // 
            // chbRevers
            // 
            chbRevers.AutoSize = true;
            chbRevers.Location = new Point(8, 131);
            chbRevers.Name = "chbRevers";
            chbRevers.Size = new Size(60, 19);
            chbRevers.TabIndex = 26;
            chbRevers.Text = "Revers";
            chbRevers.UseVisualStyleBackColor = true;
            // 
            // ucHueMinMax
            // 
            ucHueMinMax.HueMax = 360;
            ucHueMinMax.HueMin = 0;
            ucHueMinMax.Location = new Point(237, 179);
            ucHueMinMax.Name = "ucHueMinMax";
            ucHueMinMax.Size = new Size(554, 59);
            ucHueMinMax.TabIndex = 25;
            // 
            // chbWhite
            // 
            chbWhite.AutoSize = true;
            chbWhite.Location = new Point(730, 244);
            chbWhite.Name = "chbWhite";
            chbWhite.Size = new Size(57, 19);
            chbWhite.TabIndex = 24;
            chbWhite.Text = "White";
            chbWhite.UseVisualStyleBackColor = true;
            chbWhite.CheckedChanged += chbWhite_CheckedChanged;
            // 
            // gbBackground
            // 
            gbBackground.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbBackground.Controls.Add(lblBgHue);
            gbBackground.Controls.Add(chbBgWhite);
            gbBackground.Controls.Add(ucHueBg);
            gbBackground.Controls.Add(trackBarBgBrightness);
            gbBackground.Controls.Add(lblBgBrightness);
            gbBackground.Controls.Add(lblBackgroundColor);
            gbBackground.Location = new Point(239, 309);
            gbBackground.Name = "gbBackground";
            gbBackground.Size = new Size(554, 159);
            gbBackground.TabIndex = 21;
            gbBackground.TabStop = false;
            gbBackground.Text = "Background";
            // 
            // lblBgHue
            // 
            lblBgHue.AutoSize = true;
            lblBgHue.Location = new Point(17, 22);
            lblBgHue.Name = "lblBgHue";
            lblBgHue.Size = new Size(36, 15);
            lblBgHue.TabIndex = 22;
            lblBgHue.Text = "Color";
            // 
            // chbBgWhite
            // 
            chbBgWhite.AutoSize = true;
            chbBgWhite.Location = new Point(491, 84);
            chbBgWhite.Name = "chbBgWhite";
            chbBgWhite.Size = new Size(57, 19);
            chbBgWhite.TabIndex = 21;
            chbBgWhite.Text = "White";
            chbBgWhite.UseVisualStyleBackColor = true;
            // 
            // ucHueBg
            // 
            ucHueBg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueBg.Hue = 0;
            ucHueBg.Location = new Point(12, 40);
            ucHueBg.Name = "ucHueBg";
            ucHueBg.Size = new Size(536, 45);
            ucHueBg.TabIndex = 9;
            // 
            // trackBarBgBrightness
            // 
            trackBarBgBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarBgBrightness.Location = new Point(16, 110);
            trackBarBgBrightness.Maximum = 100;
            trackBarBgBrightness.Name = "trackBarBgBrightness";
            trackBarBgBrightness.Size = new Size(532, 45);
            trackBarBgBrightness.TabIndex = 19;
            // 
            // lblBgBrightness
            // 
            lblBgBrightness.AutoSize = true;
            lblBgBrightness.Location = new Point(17, 88);
            lblBgBrightness.Name = "lblBgBrightness";
            lblBgBrightness.Size = new Size(62, 15);
            lblBgBrightness.TabIndex = 20;
            lblBgBrightness.Text = "Brightness";
            // 
            // lblBackgroundColor
            // 
            lblBackgroundColor.AutoSize = true;
            lblBackgroundColor.Location = new Point(12, 42);
            lblBackgroundColor.Name = "lblBackgroundColor";
            lblBackgroundColor.Size = new Size(36, 15);
            lblBackgroundColor.TabIndex = 15;
            lblBackgroundColor.Text = "Color";
            // 
            // pnlBackgroundColor
            // 
            pnlBackgroundColor.BorderStyle = BorderStyle.FixedSingle;
            pnlBackgroundColor.Location = new Point(8, 343);
            pnlBackgroundColor.Name = "pnlBackgroundColor";
            pnlBackgroundColor.Size = new Size(41, 39);
            pnlBackgroundColor.TabIndex = 14;
            pnlBackgroundColor.Click += pnlBackgroundColor_Click;
            // 
            // lblHueMax
            // 
            lblHueMax.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblHueMax.AutoSize = true;
            lblHueMax.Location = new Point(738, 161);
            lblHueMax.Name = "lblHueMax";
            lblHueMax.Size = new Size(53, 15);
            lblHueMax.TabIndex = 13;
            lblHueMax.Text = "Max hue";
            // 
            // lblHueMin
            // 
            lblHueMin.AutoSize = true;
            lblHueMin.Location = new Point(239, 248);
            lblHueMin.Name = "lblHueMin";
            lblHueMin.Size = new Size(51, 15);
            lblHueMin.TabIndex = 12;
            lblHueMin.Text = "Min hue";
            // 
            // lblBrightness
            // 
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(237, 88);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(62, 15);
            lblBrightness.TabIndex = 10;
            lblBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarBrightness.Location = new Point(237, 106);
            trackBarBrightness.Maximum = 100;
            trackBarBrightness.Minimum = 1;
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new Size(554, 45);
            trackBarBrightness.TabIndex = 9;
            trackBarBrightness.Value = 30;
            trackBarBrightness.Scroll += trackBarBrightness_Scroll;
            // 
            // rbModeMidToOutPoint
            // 
            rbModeMidToOutPoint.AutoSize = true;
            rbModeMidToOutPoint.Location = new Point(8, 106);
            rbModeMidToOutPoint.Name = "rbModeMidToOutPoint";
            rbModeMidToOutPoint.Size = new Size(116, 19);
            rbModeMidToOutPoint.TabIndex = 9;
            rbModeMidToOutPoint.TabStop = true;
            rbModeMidToOutPoint.Text = "Mid-to-out point";
            rbModeMidToOutPoint.UseVisualStyleBackColor = true;
            // 
            // rbModeColorPush
            // 
            rbModeColorPush.AutoSize = true;
            rbModeColorPush.Location = new Point(8, 81);
            rbModeColorPush.Name = "rbModeColorPush";
            rbModeColorPush.Size = new Size(83, 19);
            rbModeColorPush.TabIndex = 8;
            rbModeColorPush.TabStop = true;
            rbModeColorPush.Text = "Color push";
            rbModeColorPush.UseVisualStyleBackColor = true;
            rbModeColorPush.CheckedChanged += rbMode_CheckedChanged;
            // 
            // rbModeMidToOut
            // 
            rbModeMidToOut.AutoSize = true;
            rbModeMidToOut.Location = new Point(8, 56);
            rbModeMidToOut.Name = "rbModeMidToOut";
            rbModeMidToOut.Size = new Size(85, 19);
            rbModeMidToOut.TabIndex = 7;
            rbModeMidToOut.Text = "Mid-to-out";
            rbModeMidToOut.UseVisualStyleBackColor = true;
            rbModeMidToOut.CheckedChanged += rbMode_CheckedChanged;
            // 
            // rbModeEndToStart
            // 
            rbModeEndToStart.AutoSize = true;
            rbModeEndToStart.Location = new Point(8, 31);
            rbModeEndToStart.Name = "rbModeEndToStart";
            rbModeEndToStart.Size = new Size(89, 19);
            rbModeEndToStart.TabIndex = 6;
            rbModeEndToStart.Text = "End-to-start";
            rbModeEndToStart.UseVisualStyleBackColor = true;
            rbModeEndToStart.CheckedChanged += rbMode_CheckedChanged;
            // 
            // rbModeStartToEnd
            // 
            rbModeStartToEnd.AutoSize = true;
            rbModeStartToEnd.Checked = true;
            rbModeStartToEnd.Location = new Point(8, 6);
            rbModeStartToEnd.Name = "rbModeStartToEnd";
            rbModeStartToEnd.Size = new Size(90, 19);
            rbModeStartToEnd.TabIndex = 5;
            rbModeStartToEnd.TabStop = true;
            rbModeStartToEnd.Text = "Start-to-end";
            rbModeStartToEnd.UseVisualStyleBackColor = true;
            rbModeStartToEnd.CheckedChanged += rbMode_CheckedChanged;
            // 
            // lblPreview
            // 
            lblPreview.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblPreview.AutoSize = true;
            lblPreview.Location = new Point(8, 533);
            lblPreview.Name = "lblPreview";
            lblPreview.Size = new Size(48, 15);
            lblPreview.TabIndex = 4;
            lblPreview.Text = "Preview";
            // 
            // tabPageAudioCaptureEqualizer
            // 
            tabPageAudioCaptureEqualizer.Location = new Point(4, 24);
            tabPageAudioCaptureEqualizer.Name = "tabPageAudioCaptureEqualizer";
            tabPageAudioCaptureEqualizer.Size = new Size(799, 443);
            tabPageAudioCaptureEqualizer.TabIndex = 2;
            tabPageAudioCaptureEqualizer.Text = "Audio capture equalizer";
            tabPageAudioCaptureEqualizer.UseVisualStyleBackColor = true;
            // 
            // tabPageScreenCapture
            // 
            tabPageScreenCapture.Controls.Add(chbReverse);
            tabPageScreenCapture.Controls.Add(chbShowGuide);
            tabPageScreenCapture.Controls.Add(lblScreenRowCapturePreview);
            tabPageScreenCapture.Controls.Add(pictureBox);
            tabPageScreenCapture.Controls.Add(pnlScreenRowSelector);
            tabPageScreenCapture.Controls.Add(lblScreenRow);
            tabPageScreenCapture.Controls.Add(numScreenRow);
            tabPageScreenCapture.Location = new Point(4, 24);
            tabPageScreenCapture.Name = "tabPageScreenCapture";
            tabPageScreenCapture.Padding = new Padding(3);
            tabPageScreenCapture.Size = new Size(799, 443);
            tabPageScreenCapture.TabIndex = 1;
            tabPageScreenCapture.Text = "Screen row capture";
            tabPageScreenCapture.UseVisualStyleBackColor = true;
            // 
            // chbReverse
            // 
            chbReverse.AutoSize = true;
            chbReverse.Location = new Point(6, 89);
            chbReverse.Name = "chbReverse";
            chbReverse.Size = new Size(60, 19);
            chbReverse.TabIndex = 12;
            chbReverse.Text = "Revers";
            chbReverse.UseVisualStyleBackColor = true;
            // 
            // chbShowGuide
            // 
            chbShowGuide.AutoSize = true;
            chbShowGuide.Location = new Point(6, 59);
            chbShowGuide.Name = "chbShowGuide";
            chbShowGuide.Size = new Size(88, 19);
            chbShowGuide.TabIndex = 11;
            chbShowGuide.Text = "Show guide";
            chbShowGuide.UseVisualStyleBackColor = true;
            chbShowGuide.CheckedChanged += chbShowGuide_CheckedChanged;
            // 
            // lblScreenRowCapturePreview
            // 
            lblScreenRowCapturePreview.AutoSize = true;
            lblScreenRowCapturePreview.Location = new Point(8, 124);
            lblScreenRowCapturePreview.Name = "lblScreenRowCapturePreview";
            lblScreenRowCapturePreview.Size = new Size(48, 15);
            lblScreenRowCapturePreview.TabIndex = 10;
            lblScreenRowCapturePreview.Text = "Preview";
            // 
            // panel1
            // 
            panel1.Controls.Add(btnTerminate);
            panel1.Controls.Add(btnInitiate);
            panel1.Controls.Add(numLedCount);
            panel1.Controls.Add(lblLedCount);
            panel1.Controls.Add(lblRefreshRate);
            panel1.Controls.Add(lblDelay);
            panel1.Controls.Add(numDelay);
            panel1.Controls.Add(textIpAddress);
            panel1.Controls.Add(lblHostname);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(807, 112);
            panel1.TabIndex = 12;
            // 
            // btnTerminate
            // 
            btnTerminate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTerminate.Location = new Point(720, 40);
            btnTerminate.Name = "btnTerminate";
            btnTerminate.Size = new Size(75, 23);
            btnTerminate.TabIndex = 8;
            btnTerminate.Text = "Terminate";
            btnTerminate.UseVisualStyleBackColor = true;
            btnTerminate.Click += btnTerminate_Click;
            // 
            // btnInitiate
            // 
            btnInitiate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnInitiate.Location = new Point(720, 11);
            btnInitiate.Name = "btnInitiate";
            btnInitiate.Size = new Size(75, 23);
            btnInitiate.TabIndex = 7;
            btnInitiate.Text = "Initiate";
            btnInitiate.UseVisualStyleBackColor = true;
            btnInitiate.Click += btnInitiate_Click;
            // 
            // numLedCount
            // 
            numLedCount.Location = new Point(149, 38);
            numLedCount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLedCount.Name = "numLedCount";
            numLedCount.Size = new Size(60, 23);
            numLedCount.TabIndex = 6;
            numLedCount.Value = new decimal(new int[] { 218, 0, 0, 0 });
            // 
            // lblLedCount
            // 
            lblLedCount.AutoSize = true;
            lblLedCount.Location = new Point(11, 40);
            lblLedCount.Name = "lblLedCount";
            lblLedCount.Size = new Size(60, 15);
            lblLedCount.TabIndex = 5;
            lblLedCount.Text = "Led count";
            // 
            // lblRefreshRate
            // 
            lblRefreshRate.AutoSize = true;
            lblRefreshRate.Location = new Point(216, 69);
            lblRefreshRate.Name = "lblRefreshRate";
            lblRefreshRate.Size = new Size(38, 15);
            lblRefreshRate.TabIndex = 4;
            lblRefreshRate.Text = "(5 Hz)";
            // 
            // lblDelay
            // 
            lblDelay.AutoSize = true;
            lblDelay.Location = new Point(12, 69);
            lblDelay.Name = "lblDelay";
            lblDelay.Size = new Size(63, 15);
            lblDelay.TabIndex = 3;
            lblDelay.Text = "Delay (ms)";
            // 
            // numDelay
            // 
            numDelay.Location = new Point(150, 67);
            numDelay.Name = "numDelay";
            numDelay.Size = new Size(60, 23);
            numDelay.TabIndex = 2;
            numDelay.Value = new decimal(new int[] { 20, 0, 0, 0 });
            numDelay.ValueChanged += numDelay_ValueChanged;
            // 
            // textIpAddress
            // 
            textIpAddress.Location = new Point(149, 9);
            textIpAddress.Name = "textIpAddress";
            textIpAddress.Size = new Size(132, 23);
            textIpAddress.TabIndex = 1;
            textIpAddress.Text = "10.0.1.11";
            // 
            // lblHostname
            // 
            lblHostname.AutoSize = true;
            lblHostname.Location = new Point(11, 12);
            lblHostname.Name = "lblHostname";
            lblHostname.Size = new Size(132, 15);
            lblHostname.TabIndex = 0;
            lblHostname.Text = "Hostname or IP address";
            // 
            // timerRotate
            // 
            timerRotate.Interval = 20000;
            timerRotate.Tick += timerRotate_Tick;
            // 
            // trackBarLevel
            // 
            trackBarLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarLevel.Location = new Point(237, 30);
            trackBarLevel.Maximum = 50;
            trackBarLevel.Minimum = -50;
            trackBarLevel.Name = "trackBarLevel";
            trackBarLevel.Size = new Size(554, 45);
            trackBarLevel.TabIndex = 29;
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.Location = new Point(239, 10);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(38, 15);
            lblLevel.TabIndex = 30;
            lblLevel.Text = "label1";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(807, 748);
            Controls.Add(tabControl);
            Controls.Add(panel1);
            Controls.Add(statusStrip);
            Name = "FrmMain";
            Text = "Visualedizer";
            Load += frmMain_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)numScreenRow).EndInit();
            pnlScreenRowSelector.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            tabControl.ResumeLayout(false);
            tabPageAudioCaptureVolume.ResumeLayout(false);
            tabPageAudioCaptureVolume.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarRotate).EndInit();
            gbBackground.ResumeLayout(false);
            gbBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBgBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            tabPageScreenCapture.ResumeLayout(false);
            tabPageScreenCapture.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numLedCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLevel).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnTest;
        private PictureBox pictureBox;
        private HScrollBar hsbScreenRowSelector;
        private Label lblScreenRow;
        private NumericUpDown numScreenRow;
        private Panel pnlScreenRowSelector;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statLblConnection;
        private TabControl tabControl;
        private TabPage tabPageAudioCaptureVolume;
        private TabPage tabPageAudioCaptureEqualizer;
        private TabPage tabPageScreenCapture;
        private Panel panel1;
        private NumericUpDown numDelay;
        private TextBox textIpAddress;
        private Label lblHostname;
        private Label lblRefreshRate;
        private Label lblDelay;
        private NumericUpDown numLedCount;
        private Label lblLedCount;
        private Button btnInitiate;
        private Button btnTerminate;
        private Label lblPreview;
        private Label lblScreenRowCapturePreview;
        private CheckBox chbShowGuide;
        private Label lblBrightness;
        private CheckBox chbReverse;
        public TrackBar trackBarHueMin;
        public ProgressBar progressBar;
        private Label lblHueMax;
        private Label lblHueMin;
        public TrackBar trackBarBrightness;
        private Label lblBackgroundColor;
        private ColorDialog colorBackground;
        public Panel pnlBackgroundColor;
        private GroupBox gbBackground;
        private Label lblBgBrightness;
        public TrackBar trackBarBgBrightness;
        private Label lblBgHue;
        public Visualedizer.UcHue ucHueBg;
        public CheckBox chbBgWhite;
        public RadioButton rbModeMidToOut;
        public RadioButton rbModeEndToStart;
        public RadioButton rbModeStartToEnd;
        public RadioButton rbModeColorPush;
        public RadioButton rbModeMidToOutPoint;
        public Visualedizer.UcHueMinMax ucHueMinMax;
        public CheckBox chbRevers;
        public CheckBox chbHueRevers;
        public CheckBox chbWhite;
        public RadioButton rbBrightness;
        private TrackBar trackBarRotate;
        private CheckBox chbRotate;
        private System.Windows.Forms.Timer timerRotate;
        private Label lblLevel;
        private TrackBar trackBarLevel;
    }
}