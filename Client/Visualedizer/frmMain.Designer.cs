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
            tabPageBasicControl = new TabPage();
            gbGradient = new GroupBox();
            rbGradient = new RadioButton();
            ucHueMinMaxGradient = new Visualedizer.UcHueMinMax();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            gbSolidColor = new GroupBox();
            rbSolid = new RadioButton();
            ucHueSolid = new Visualedizer.UcHue();
            label1 = new Label();
            trackBrightnessBasic = new TrackBar();
            lblSaturation = new Label();
            trackSaturationBasic = new TrackBar();
            tabPageAcVolume = new TabPage();
            lblAudioDevice = new Label();
            cbAudioDevices = new ComboBox();
            lblLevel = new Label();
            trackBarNormalizationLevel = new TrackBar();
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
            tabPageScreenCapture = new TabPage();
            chbReverse = new CheckBox();
            chbShowGuide = new CheckBox();
            lblScreenRowCapturePreview = new Label();
            tabPageAcSpectralAnalysis = new TabPage();
            tabPageOtherDevices = new TabPage();
            gbLaser = new GroupBox();
            numLaserColorY = new NumericUpDown();
            numLaserColorX = new NumericUpDown();
            lblLaserColor = new Label();
            numLaserPatternY = new NumericUpDown();
            numLaserPatternX = new NumericUpDown();
            lblLaserPattern = new Label();
            numLaserTriggerY = new NumericUpDown();
            numLaserTriggerX = new NumericUpDown();
            lblLaserTrigger = new Label();
            gbStrobe = new GroupBox();
            numStrobeY = new NumericUpDown();
            numStrobeX = new NumericUpDown();
            lblStrobeTrigger = new Label();
            panel1 = new Panel();
            dgvDevices = new DataGridView();
            colEnabled = new DataGridViewCheckBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            colScene = new DataGridViewComboBoxColumn();
            colHost = new DataGridViewTextBoxColumn();
            colPort = new DataGridViewTextBoxColumn();
            colLedCount = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            btnRemoveDevice = new Button();
            btnAddDevice = new Button();
            lblDevices = new Label();
            lblRefreshRate = new Label();
            lblDelay = new Label();
            numDelay = new NumericUpDown();
            colorBackground = new ColorDialog();
            timerRotate = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numScreenRow).BeginInit();
            pnlScreenRowSelector.SuspendLayout();
            statusStrip.SuspendLayout();
            tabControl.SuspendLayout();
            tabPageBasicControl.SuspendLayout();
            gbGradient.SuspendLayout();
            gbSolidColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBrightnessBasic).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackSaturationBasic).BeginInit();
            tabPageAcVolume.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarNormalizationLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarRotate).BeginInit();
            gbBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBgBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            tabPageScreenCapture.SuspendLayout();
            tabPageOtherDevices.SuspendLayout();
            gbLaser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numLaserColorY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserColorX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserPatternY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserPatternX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserTriggerY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserTriggerX).BeginInit();
            gbStrobe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numStrobeY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStrobeX).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDevices).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).BeginInit();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(8, 411);
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
            numScreenRow.ValueChanged += numScreenRow_ValueChanged;
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
            statusStrip.Location = new Point(0, 714);
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
            tabControl.Controls.Add(tabPageBasicControl);
            tabControl.Controls.Add(tabPageAcVolume);
            tabControl.Controls.Add(tabPageScreenCapture);
            tabControl.Controls.Add(tabPageAcSpectralAnalysis);
            tabControl.Controls.Add(tabPageOtherDevices);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 240);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(807, 474);
            tabControl.TabIndex = 11;
            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
            // 
            // tabPageBasicControl
            // 
            tabPageBasicControl.Controls.Add(gbGradient);
            tabPageBasicControl.Controls.Add(label5);
            tabPageBasicControl.Controls.Add(gbSolidColor);
            tabPageBasicControl.Controls.Add(trackBrightnessBasic);
            tabPageBasicControl.Controls.Add(lblSaturation);
            tabPageBasicControl.Controls.Add(trackSaturationBasic);
            tabPageBasicControl.Location = new Point(4, 24);
            tabPageBasicControl.Name = "tabPageBasicControl";
            tabPageBasicControl.Size = new Size(799, 446);
            tabPageBasicControl.TabIndex = 3;
            tabPageBasicControl.Text = "Basic control";
            tabPageBasicControl.UseVisualStyleBackColor = true;
            // 
            // gbGradient
            // 
            gbGradient.Controls.Add(rbGradient);
            gbGradient.Controls.Add(ucHueMinMaxGradient);
            gbGradient.Controls.Add(label3);
            gbGradient.Controls.Add(label4);
            gbGradient.Location = new Point(3, 106);
            gbGradient.Name = "gbGradient";
            gbGradient.Size = new Size(793, 138);
            gbGradient.TabIndex = 28;
            gbGradient.TabStop = false;
            gbGradient.Text = "Gradient";
            // 
            // rbGradient
            // 
            rbGradient.AutoSize = true;
            rbGradient.BackColor = SystemColors.ControlLight;
            rbGradient.Location = new Point(6, 0);
            rbGradient.Name = "rbGradient";
            rbGradient.Size = new Size(70, 19);
            rbGradient.TabIndex = 33;
            rbGradient.TabStop = true;
            rbGradient.Text = "Gradient";
            rbGradient.UseVisualStyleBackColor = false;
            rbGradient.CheckedChanged += rbBasic_CheckedChanged;
            // 
            // ucHueMinMaxGradient
            // 
            ucHueMinMaxGradient.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueMinMaxGradient.HueMax = 360;
            ucHueMinMaxGradient.HueMin = 0;
            ucHueMinMaxGradient.Location = new Point(251, 37);
            ucHueMinMaxGradient.Name = "ucHueMinMaxGradient";
            ucHueMinMaxGradient.Size = new Size(536, 59);
            ucHueMinMaxGradient.TabIndex = 28;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(734, 19);
            label3.Name = "label3";
            label3.Size = new Size(52, 15);
            label3.TabIndex = 27;
            label3.Text = "Max hue";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(251, 99);
            label4.Name = "label4";
            label4.Size = new Size(51, 15);
            label4.TabIndex = 26;
            label4.Text = "Min hue";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(254, 338);
            label5.Name = "label5";
            label5.Size = new Size(62, 15);
            label5.TabIndex = 28;
            label5.Text = "Brightness";
            // 
            // gbSolidColor
            // 
            gbSolidColor.Controls.Add(rbSolid);
            gbSolidColor.Controls.Add(ucHueSolid);
            gbSolidColor.Controls.Add(label1);
            gbSolidColor.Location = new Point(3, 3);
            gbSolidColor.Name = "gbSolidColor";
            gbSolidColor.Size = new Size(793, 97);
            gbSolidColor.TabIndex = 27;
            gbSolidColor.TabStop = false;
            gbSolidColor.Text = "Solid color";
            // 
            // rbSolid
            // 
            rbSolid.AutoSize = true;
            rbSolid.BackColor = SystemColors.ControlLight;
            rbSolid.Checked = true;
            rbSolid.Location = new Point(6, 0);
            rbSolid.Name = "rbSolid";
            rbSolid.Size = new Size(81, 19);
            rbSolid.TabIndex = 29;
            rbSolid.TabStop = true;
            rbSolid.Text = "Solid color";
            rbSolid.UseVisualStyleBackColor = false;
            rbSolid.CheckedChanged += rbBasic_CheckedChanged;
            // 
            // ucHueSolid
            // 
            ucHueSolid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueSolid.Hue = 0;
            ucHueSolid.Location = new Point(251, 35);
            ucHueSolid.MaxVal = 360;
            ucHueSolid.MinVal = 0;
            ucHueSolid.Name = "ucHueSolid";
            ucHueSolid.Size = new Size(536, 45);
            ucHueSolid.TabIndex = 23;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(251, 17);
            label1.Name = "label1";
            label1.Size = new Size(36, 15);
            label1.TabIndex = 26;
            label1.Text = "Color";
            // 
            // trackBrightnessBasic
            // 
            trackBrightnessBasic.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBrightnessBasic.Location = new Point(254, 356);
            trackBrightnessBasic.Maximum = 100;
            trackBrightnessBasic.Name = "trackBrightnessBasic";
            trackBrightnessBasic.Size = new Size(537, 45);
            trackBrightnessBasic.TabIndex = 27;
            trackBrightnessBasic.Value = 50;
            // 
            // lblSaturation
            // 
            lblSaturation.AutoSize = true;
            lblSaturation.Location = new Point(254, 262);
            lblSaturation.Name = "lblSaturation";
            lblSaturation.Size = new Size(61, 15);
            lblSaturation.TabIndex = 25;
            lblSaturation.Text = "Saturation";
            // 
            // trackSaturationBasic
            // 
            trackSaturationBasic.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackSaturationBasic.Location = new Point(254, 280);
            trackSaturationBasic.Maximum = 100;
            trackSaturationBasic.Name = "trackSaturationBasic";
            trackSaturationBasic.Size = new Size(537, 45);
            trackSaturationBasic.TabIndex = 24;
            trackSaturationBasic.Value = 100;
            // 
            // tabPageAcVolume
            // 
            tabPageAcVolume.Controls.Add(lblAudioDevice);
            tabPageAcVolume.Controls.Add(cbAudioDevices);
            tabPageAcVolume.Controls.Add(lblLevel);
            tabPageAcVolume.Controls.Add(trackBarNormalizationLevel);
            tabPageAcVolume.Controls.Add(trackBarRotate);
            tabPageAcVolume.Controls.Add(chbRotate);
            tabPageAcVolume.Controls.Add(rbBrightness);
            tabPageAcVolume.Controls.Add(chbHueRevers);
            tabPageAcVolume.Controls.Add(chbRevers);
            tabPageAcVolume.Controls.Add(ucHueMinMax);
            tabPageAcVolume.Controls.Add(chbWhite);
            tabPageAcVolume.Controls.Add(gbBackground);
            tabPageAcVolume.Controls.Add(lblHueMax);
            tabPageAcVolume.Controls.Add(lblHueMin);
            tabPageAcVolume.Controls.Add(lblBrightness);
            tabPageAcVolume.Controls.Add(trackBarBrightness);
            tabPageAcVolume.Controls.Add(rbModeMidToOutPoint);
            tabPageAcVolume.Controls.Add(rbModeColorPush);
            tabPageAcVolume.Controls.Add(rbModeMidToOut);
            tabPageAcVolume.Controls.Add(rbModeEndToStart);
            tabPageAcVolume.Controls.Add(rbModeStartToEnd);
            tabPageAcVolume.Controls.Add(lblPreview);
            tabPageAcVolume.Controls.Add(progressBar);
            tabPageAcVolume.Location = new Point(4, 24);
            tabPageAcVolume.Name = "tabPageAcVolume";
            tabPageAcVolume.Padding = new Padding(3);
            tabPageAcVolume.Size = new Size(799, 446);
            tabPageAcVolume.TabIndex = 0;
            tabPageAcVolume.Text = "Volume";
            tabPageAcVolume.UseVisualStyleBackColor = true;
            // 
            // lblAudioDevice
            // 
            lblAudioDevice.AutoSize = true;
            lblAudioDevice.Location = new Point(225, 9);
            lblAudioDevice.Name = "lblAudioDevice";
            lblAudioDevice.Size = new Size(76, 15);
            lblAudioDevice.TabIndex = 31;
            lblAudioDevice.Text = "Audio device";
            // 
            // cbAudioDevices
            // 
            cbAudioDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAudioDevices.FormattingEnabled = true;
            cbAudioDevices.Location = new Point(225, 27);
            cbAudioDevices.Name = "cbAudioDevices";
            cbAudioDevices.Size = new Size(340, 23);
            cbAudioDevices.TabIndex = 9;
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.Location = new Point(227, 73);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(34, 15);
            lblLevel.TabIndex = 30;
            lblLevel.Text = "Level";
            // 
            // trackBarNormalizationLevel
            // 
            trackBarNormalizationLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarNormalizationLevel.Location = new Point(225, 93);
            trackBarNormalizationLevel.Maximum = 100;
            trackBarNormalizationLevel.Minimum = 1;
            trackBarNormalizationLevel.Name = "trackBarNormalizationLevel";
            trackBarNormalizationLevel.Size = new Size(554, 45);
            trackBarNormalizationLevel.TabIndex = 29;
            trackBarNormalizationLevel.Value = 10;
            trackBarNormalizationLevel.Scroll += trackBarNormalizationLevel_Scroll;
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
            rbBrightness.CheckedChanged += rbMode_CheckedChanged;
            // 
            // chbHueRevers
            // 
            chbHueRevers.AutoSize = true;
            chbHueRevers.Location = new Point(652, 307);
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
            ucHueMinMax.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueMinMax.HueMax = 360;
            ucHueMinMax.HueMin = 0;
            ucHueMinMax.Location = new Point(225, 242);
            ucHueMinMax.Name = "ucHueMinMax";
            ucHueMinMax.Size = new Size(554, 59);
            ucHueMinMax.TabIndex = 25;
            // 
            // chbWhite
            // 
            chbWhite.AutoSize = true;
            chbWhite.Location = new Point(718, 307);
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
            gbBackground.Location = new Point(227, 372);
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
            ucHueBg.MaxVal = 360;
            ucHueBg.MinVal = 0;
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
            // lblHueMax
            // 
            lblHueMax.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblHueMax.AutoSize = true;
            lblHueMax.Location = new Point(726, 224);
            lblHueMax.Name = "lblHueMax";
            lblHueMax.Size = new Size(52, 15);
            lblHueMax.TabIndex = 13;
            lblHueMax.Text = "Max hue";
            // 
            // lblHueMin
            // 
            lblHueMin.AutoSize = true;
            lblHueMin.Location = new Point(227, 311);
            lblHueMin.Name = "lblHueMin";
            lblHueMin.Size = new Size(51, 15);
            lblHueMin.TabIndex = 12;
            lblHueMin.Text = "Min hue";
            // 
            // lblBrightness
            // 
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(225, 151);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(62, 15);
            lblBrightness.TabIndex = 10;
            lblBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarBrightness.Location = new Point(225, 169);
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
            lblPreview.Location = new Point(8, 393);
            lblPreview.Name = "lblPreview";
            lblPreview.Size = new Size(48, 15);
            lblPreview.TabIndex = 4;
            lblPreview.Text = "Preview";
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
            tabPageScreenCapture.Size = new Size(799, 574);
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
            // tabPageAcSpectralAnalysis
            // 
            tabPageAcSpectralAnalysis.Location = new Point(4, 24);
            tabPageAcSpectralAnalysis.Name = "tabPageAcSpectralAnalysis";
            tabPageAcSpectralAnalysis.Size = new Size(799, 574);
            tabPageAcSpectralAnalysis.TabIndex = 2;
            tabPageAcSpectralAnalysis.Text = "Spectral analysis";
            // 
            // tabPageOtherDevices
            // 
            tabPageOtherDevices.Controls.Add(gbLaser);
            tabPageOtherDevices.Controls.Add(gbStrobe);
            tabPageOtherDevices.Location = new Point(4, 24);
            tabPageOtherDevices.Name = "tabPageOtherDevices";
            tabPageOtherDevices.Size = new Size(799, 574);
            tabPageOtherDevices.TabIndex = 4;
            tabPageOtherDevices.Text = "Other devices";
            tabPageOtherDevices.UseVisualStyleBackColor = true;
            // 
            // gbLaser
            // 
            gbLaser.Controls.Add(numLaserColorY);
            gbLaser.Controls.Add(numLaserColorX);
            gbLaser.Controls.Add(lblLaserColor);
            gbLaser.Controls.Add(numLaserPatternY);
            gbLaser.Controls.Add(numLaserPatternX);
            gbLaser.Controls.Add(lblLaserPattern);
            gbLaser.Controls.Add(numLaserTriggerY);
            gbLaser.Controls.Add(numLaserTriggerX);
            gbLaser.Controls.Add(lblLaserTrigger);
            gbLaser.Location = new Point(8, 93);
            gbLaser.Name = "gbLaser";
            gbLaser.Size = new Size(236, 135);
            gbLaser.TabIndex = 7;
            gbLaser.TabStop = false;
            gbLaser.Text = "Laser";
            // 
            // numLaserColorY
            // 
            numLaserColorY.Location = new Point(159, 91);
            numLaserColorY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserColorY.Name = "numLaserColorY";
            numLaserColorY.Size = new Size(54, 23);
            numLaserColorY.TabIndex = 9;
            numLaserColorY.ValueChanged += numLaserColorY_ValueChanged;
            // 
            // numLaserColorX
            // 
            numLaserColorX.Location = new Point(99, 91);
            numLaserColorX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserColorX.Name = "numLaserColorX";
            numLaserColorX.Size = new Size(54, 23);
            numLaserColorX.TabIndex = 8;
            numLaserColorX.ValueChanged += numLaserColorX_ValueChanged;
            // 
            // lblLaserColor
            // 
            lblLaserColor.AutoSize = true;
            lblLaserColor.Location = new Point(17, 93);
            lblLaserColor.Name = "lblLaserColor";
            lblLaserColor.Size = new Size(64, 15);
            lblLaserColor.TabIndex = 10;
            lblLaserColor.Text = "Color [x, y]";
            // 
            // numLaserPatternY
            // 
            numLaserPatternY.Location = new Point(159, 62);
            numLaserPatternY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserPatternY.Name = "numLaserPatternY";
            numLaserPatternY.Size = new Size(54, 23);
            numLaserPatternY.TabIndex = 6;
            numLaserPatternY.ValueChanged += numLaserPatternY_ValueChanged;
            // 
            // numLaserPatternX
            // 
            numLaserPatternX.Location = new Point(99, 62);
            numLaserPatternX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserPatternX.Name = "numLaserPatternX";
            numLaserPatternX.Size = new Size(54, 23);
            numLaserPatternX.TabIndex = 5;
            numLaserPatternX.ValueChanged += numLaserPatternX_ValueChanged;
            // 
            // lblLaserPattern
            // 
            lblLaserPattern.AutoSize = true;
            lblLaserPattern.Location = new Point(17, 64);
            lblLaserPattern.Name = "lblLaserPattern";
            lblLaserPattern.Size = new Size(73, 15);
            lblLaserPattern.TabIndex = 7;
            lblLaserPattern.Text = "Pattern [x, y]";
            // 
            // numLaserTriggerY
            // 
            numLaserTriggerY.Location = new Point(159, 33);
            numLaserTriggerY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserTriggerY.Name = "numLaserTriggerY";
            numLaserTriggerY.Size = new Size(54, 23);
            numLaserTriggerY.TabIndex = 1;
            numLaserTriggerY.ValueChanged += numLaserTriggerY_ValueChanged;
            // 
            // numLaserTriggerX
            // 
            numLaserTriggerX.Location = new Point(99, 33);
            numLaserTriggerX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserTriggerX.Name = "numLaserTriggerX";
            numLaserTriggerX.Size = new Size(54, 23);
            numLaserTriggerX.TabIndex = 0;
            numLaserTriggerX.ValueChanged += numLaserTriggerX_ValueChanged;
            // 
            // lblLaserTrigger
            // 
            lblLaserTrigger.AutoSize = true;
            lblLaserTrigger.Location = new Point(17, 35);
            lblLaserTrigger.Name = "lblLaserTrigger";
            lblLaserTrigger.Size = new Size(72, 15);
            lblLaserTrigger.TabIndex = 4;
            lblLaserTrigger.Text = "Trigger [x, y]";
            // 
            // gbStrobe
            // 
            gbStrobe.Controls.Add(numStrobeY);
            gbStrobe.Controls.Add(numStrobeX);
            gbStrobe.Controls.Add(lblStrobeTrigger);
            gbStrobe.Location = new Point(8, 3);
            gbStrobe.Name = "gbStrobe";
            gbStrobe.Size = new Size(236, 84);
            gbStrobe.TabIndex = 6;
            gbStrobe.TabStop = false;
            gbStrobe.Text = "Strobe";
            // 
            // numStrobeY
            // 
            numStrobeY.Location = new Point(161, 35);
            numStrobeY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numStrobeY.Name = "numStrobeY";
            numStrobeY.Size = new Size(54, 23);
            numStrobeY.TabIndex = 3;
            numStrobeY.ValueChanged += numStrobeY_ValueChanged;
            // 
            // numStrobeX
            // 
            numStrobeX.Location = new Point(101, 35);
            numStrobeX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numStrobeX.Name = "numStrobeX";
            numStrobeX.Size = new Size(54, 23);
            numStrobeX.TabIndex = 2;
            numStrobeX.ValueChanged += numStrobeX_ValueChanged;
            // 
            // lblStrobeTrigger
            // 
            lblStrobeTrigger.AutoSize = true;
            lblStrobeTrigger.Location = new Point(20, 37);
            lblStrobeTrigger.Name = "lblStrobeTrigger";
            lblStrobeTrigger.Size = new Size(72, 15);
            lblStrobeTrigger.TabIndex = 5;
            lblStrobeTrigger.Text = "Trigger [x, y]";
            // 
            // panel1
            // 
            panel1.Controls.Add(dgvDevices);
            panel1.Controls.Add(btnRemoveDevice);
            panel1.Controls.Add(btnAddDevice);
            panel1.Controls.Add(lblDevices);
            panel1.Controls.Add(lblRefreshRate);
            panel1.Controls.Add(lblDelay);
            panel1.Controls.Add(numDelay);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(807, 240);
            panel1.TabIndex = 12;
            // 
            // dgvDevices
            // 
            dgvDevices.AllowUserToAddRows = false;
            dgvDevices.AllowUserToDeleteRows = false;
            dgvDevices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvDevices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDevices.Columns.AddRange(new DataGridViewColumn[] { colEnabled, colName, colScene, colHost, colPort, colLedCount, colStatus });
            dgvDevices.Location = new Point(12, 97);
            dgvDevices.Name = "dgvDevices";
            dgvDevices.RowHeadersVisible = false;
            dgvDevices.Size = new Size(783, 132);
            dgvDevices.TabIndex = 12;
            // 
            // colEnabled
            // 
            colEnabled.DataPropertyName = "Enabled";
            colEnabled.HeaderText = "Enabled";
            colEnabled.Name = "colEnabled";
            colEnabled.Width = 60;
            // 
            // colName
            // 
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colName.DataPropertyName = "Name";
            colName.FillWeight = 20F;
            colName.HeaderText = "Name";
            colName.Name = "colName";
            // 
            // colScene
            // 
            colScene.DataPropertyName = "Scene";
            colScene.HeaderText = "Scene";
            colScene.Name = "colScene";
            colScene.Resizable = DataGridViewTriState.True;
            colScene.SortMode = DataGridViewColumnSortMode.Automatic;
            colScene.Width = 110;
            // 
            // colHost
            // 
            colHost.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colHost.DataPropertyName = "Host";
            colHost.FillWeight = 25F;
            colHost.HeaderText = "Host";
            colHost.Name = "colHost";
            // 
            // colPort
            // 
            colPort.DataPropertyName = "Port";
            colPort.HeaderText = "Port";
            colPort.Name = "colPort";
            colPort.Width = 60;
            // 
            // colLedCount
            // 
            colLedCount.DataPropertyName = "LedCount";
            colLedCount.HeaderText = "LEDs";
            colLedCount.Name = "colLedCount";
            colLedCount.Width = 60;
            // 
            // colStatus
            // 
            colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colStatus.DataPropertyName = "Status";
            colStatus.FillWeight = 20F;
            colStatus.HeaderText = "Status";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // btnRemoveDevice
            // 
            btnRemoveDevice.Location = new Point(106, 68);
            btnRemoveDevice.Name = "btnRemoveDevice";
            btnRemoveDevice.Size = new Size(108, 23);
            btnRemoveDevice.TabIndex = 11;
            btnRemoveDevice.Text = "Remove selected";
            btnRemoveDevice.UseVisualStyleBackColor = true;
            btnRemoveDevice.Click += btnRemoveDevice_Click;
            // 
            // btnAddDevice
            // 
            btnAddDevice.Location = new Point(12, 68);
            btnAddDevice.Name = "btnAddDevice";
            btnAddDevice.Size = new Size(88, 23);
            btnAddDevice.TabIndex = 10;
            btnAddDevice.Text = "Add device";
            btnAddDevice.UseVisualStyleBackColor = true;
            btnAddDevice.Click += btnAddDevice_Click;
            // 
            // lblDevices
            // 
            lblDevices.AutoSize = true;
            lblDevices.Location = new Point(12, 47);
            lblDevices.Name = "lblDevices";
            lblDevices.Size = new Size(47, 15);
            lblDevices.TabIndex = 9;
            lblDevices.Text = "Devices";
            // 
            // lblRefreshRate
            // 
            lblRefreshRate.AutoSize = true;
            lblRefreshRate.Location = new Point(160, 16);
            lblRefreshRate.Name = "lblRefreshRate";
            lblRefreshRate.Size = new Size(38, 15);
            lblRefreshRate.TabIndex = 4;
            lblRefreshRate.Text = "(5 Hz)";
            // 
            // lblDelay
            // 
            lblDelay.AutoSize = true;
            lblDelay.Location = new Point(12, 14);
            lblDelay.Name = "lblDelay";
            lblDelay.Size = new Size(63, 15);
            lblDelay.TabIndex = 3;
            lblDelay.Text = "Delay (ms)";
            // 
            // numDelay
            // 
            numDelay.Location = new Point(92, 12);
            numDelay.Name = "numDelay";
            numDelay.Size = new Size(60, 23);
            numDelay.TabIndex = 2;
            numDelay.Value = new decimal(new int[] { 20, 0, 0, 0 });
            numDelay.ValueChanged += numDelay_ValueChanged;
            // 
            // timerRotate
            // 
            timerRotate.Interval = 20000;
            timerRotate.Tick += timerRotate_Tick;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(807, 736);
            Controls.Add(tabControl);
            Controls.Add(panel1);
            Controls.Add(statusStrip);
            Name = "FrmMain";
            Text = "Visualedizer";
            FormClosing += FrmMain_FormClosing;
            Load += frmMain_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)numScreenRow).EndInit();
            pnlScreenRowSelector.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            tabControl.ResumeLayout(false);
            tabPageBasicControl.ResumeLayout(false);
            tabPageBasicControl.PerformLayout();
            gbGradient.ResumeLayout(false);
            gbGradient.PerformLayout();
            gbSolidColor.ResumeLayout(false);
            gbSolidColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBrightnessBasic).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackSaturationBasic).EndInit();
            tabPageAcVolume.ResumeLayout(false);
            tabPageAcVolume.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarNormalizationLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarRotate).EndInit();
            gbBackground.ResumeLayout(false);
            gbBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBgBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            tabPageScreenCapture.ResumeLayout(false);
            tabPageScreenCapture.PerformLayout();
            tabPageOtherDevices.ResumeLayout(false);
            gbLaser.ResumeLayout(false);
            gbLaser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numLaserColorY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserColorX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserPatternY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserPatternX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserTriggerY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserTriggerX).EndInit();
            gbStrobe.ResumeLayout(false);
            gbStrobe.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numStrobeY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStrobeX).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDevices).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).EndInit();
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
        private ToolStripStatusLabel statLblConnection;
        private TabControl tabControl;
        private TabPage tabPageAcVolume;
        private TabPage tabPageAcSpectralAnalysis;
        private TabPage tabPageScreenCapture;
        private Panel panel1;
        private DataGridView dgvDevices;
        private DataGridViewCheckBoxColumn colEnabled;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewComboBoxColumn colScene;
        private DataGridViewTextBoxColumn colHost;
        private DataGridViewTextBoxColumn colPort;
        private DataGridViewTextBoxColumn colLedCount;
        private DataGridViewTextBoxColumn colStatus;
        private Button btnRemoveDevice;
        private Button btnAddDevice;
        private Label lblDevices;
        private NumericUpDown numDelay;
        private Label lblRefreshRate;
        private Label lblDelay;
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
        public StatusStrip statusStrip;
        public TrackBar trackBarNormalizationLevel;
        private TabPage tabPageBasicControl;
        private Label label1;
        public Visualedizer.UcHue ucHueSolid;
        public TrackBar trackSaturationBasic;
        private Label lblSaturation;
        private GroupBox gbGradient;
        public Visualedizer.UcHueMinMax ucHueMinMaxGradient;
        private Label label3;
        private Label label4;
        private GroupBox gbSolidColor;
        public ComboBox cbAudioDevices;
        private Label lblAudioDevice;
        private Label label5;
        public TrackBar trackBrightnessBasic;
        public RadioButton rbSolid;
        public RadioButton rbGradient;
        private TabPage tabPageOtherDevices;
        private Label lblLaserTrigger;
        private NumericUpDown numLaserTriggerY;
        private NumericUpDown numLaserTriggerX;
        private Label lblStrobeTrigger;
        private NumericUpDown numStrobeY;
        private NumericUpDown numStrobeX;
        private GroupBox gbLaser;
        private GroupBox gbStrobe;
        private NumericUpDown numLaserColorY;
        private NumericUpDown numLaserColorX;
        private Label lblLaserColor;
        private NumericUpDown numLaserPatternY;
        private NumericUpDown numLaserPatternX;
        private Label lblLaserPattern;
    }
}
