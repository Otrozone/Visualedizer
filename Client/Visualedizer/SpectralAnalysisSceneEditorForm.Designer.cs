namespace Ledqualizer
{
    partial class SpectralAnalysisSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblAudioDevice;
        private ComboBox cbAudioDevices;
        private GroupBox gbModes;
        private RadioButton rbBrightness;
        private RadioButton rbModeColorPush;
        private RadioButton rbModeMidToOutPoint;
        private RadioButton rbModeMidToOut;
        private RadioButton rbModeEndToStart;
        private RadioButton rbModeStartToEnd;
        private Label lblBrightness;
        private TrackBar trackBarBrightness;
        private Label lblNormalization;
        private TrackBar trackBarNormalizationLevel;
        private Label lblHueRange;
        private Visualedizer.UcHueMinMax ucHueMinMax;
        private CheckBox chbReverse;
        private CheckBox chbHueReverse;
        private CheckBox chbWhite;
        private GroupBox gbBackground;
        private CheckBox chbBgWhite;
        private Label lblBgHue;
        private Visualedizer.UcHue ucHueBg;
        private Label lblBgBrightness;
        private TrackBar trackBarBgBrightness;
        private Label lblFrequencyLow;
        private TrackBar trackBarFrequencyLow;
        private Label lblFrequencyHigh;
        private TrackBar trackBarFrequencyHigh;
        private Label lblLevelLow;
        private TrackBar trackBarLevelLow;
        private Label lblLevelHigh;
        private TrackBar trackBarLevelHigh;
        private ProgressBar progressBar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblAudioDevice = new Label();
            cbAudioDevices = new ComboBox();
            gbModes = new GroupBox();
            rbBrightness = new RadioButton();
            rbModeColorPush = new RadioButton();
            rbModeMidToOutPoint = new RadioButton();
            rbModeMidToOut = new RadioButton();
            rbModeEndToStart = new RadioButton();
            rbModeStartToEnd = new RadioButton();
            lblBrightness = new Label();
            trackBarBrightness = new TrackBar();
            lblNormalization = new Label();
            trackBarNormalizationLevel = new TrackBar();
            lblHueRange = new Label();
            ucHueMinMax = new Visualedizer.UcHueMinMax();
            chbReverse = new CheckBox();
            chbHueReverse = new CheckBox();
            chbWhite = new CheckBox();
            gbBackground = new GroupBox();
            chbBgWhite = new CheckBox();
            lblBgHue = new Label();
            ucHueBg = new Visualedizer.UcHue();
            lblBgBrightness = new Label();
            trackBarBgBrightness = new TrackBar();
            lblFrequencyLow = new Label();
            trackBarFrequencyLow = new TrackBar();
            lblFrequencyHigh = new Label();
            trackBarFrequencyHigh = new TrackBar();
            lblLevelLow = new Label();
            trackBarLevelLow = new TrackBar();
            lblLevelHigh = new Label();
            trackBarLevelHigh = new TrackBar();
            progressBar = new ProgressBar();
            gbModes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarNormalizationLevel).BeginInit();
            gbBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBgBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFrequencyLow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFrequencyHigh).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLevelLow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLevelHigh).BeginInit();
            SuspendLayout();
            // 
            // lblAudioDevice
            // 
            lblAudioDevice.AutoSize = true;
            lblAudioDevice.Location = new Point(16, 16);
            lblAudioDevice.Name = "lblAudioDevice";
            lblAudioDevice.Size = new Size(76, 15);
            lblAudioDevice.TabIndex = 0;
            lblAudioDevice.Text = "Audio device";
            // 
            // cbAudioDevices
            // 
            cbAudioDevices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbAudioDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAudioDevices.Location = new Point(16, 34);
            cbAudioDevices.Name = "cbAudioDevices";
            cbAudioDevices.Size = new Size(620, 23);
            cbAudioDevices.TabIndex = 1;
            cbAudioDevices.SelectedIndexChanged += cbAudioDevices_SelectedIndexChanged;
            // 
            // gbModes
            // 
            gbModes.Controls.Add(rbBrightness);
            gbModes.Controls.Add(rbModeColorPush);
            gbModes.Controls.Add(rbModeMidToOutPoint);
            gbModes.Controls.Add(rbModeMidToOut);
            gbModes.Controls.Add(rbModeEndToStart);
            gbModes.Controls.Add(rbModeStartToEnd);
            gbModes.Location = new Point(16, 72);
            gbModes.Name = "gbModes";
            gbModes.Size = new Size(620, 72);
            gbModes.TabIndex = 2;
            gbModes.TabStop = false;
            gbModes.Text = "Response mode";
            // 
            // rbBrightness
            // 
            rbBrightness.AutoSize = true;
            rbBrightness.Location = new Point(510, 31);
            rbBrightness.Name = "rbBrightness";
            rbBrightness.Size = new Size(79, 19);
            rbBrightness.TabIndex = 5;
            rbBrightness.TabStop = true;
            rbBrightness.Text = "Brightness";
            rbBrightness.UseVisualStyleBackColor = true;
            rbBrightness.CheckedChanged += ControlValueChanged;
            // 
            // rbModeColorPush
            // 
            rbModeColorPush.AutoSize = true;
            rbModeColorPush.Location = new Point(414, 31);
            rbModeColorPush.Name = "rbModeColorPush";
            rbModeColorPush.Size = new Size(79, 19);
            rbModeColorPush.TabIndex = 4;
            rbModeColorPush.TabStop = true;
            rbModeColorPush.Text = "Color push";
            rbModeColorPush.UseVisualStyleBackColor = true;
            rbModeColorPush.CheckedChanged += ControlValueChanged;
            // 
            // rbModeMidToOutPoint
            // 
            rbModeMidToOutPoint.AutoSize = true;
            rbModeMidToOutPoint.Location = new Point(308, 31);
            rbModeMidToOutPoint.Name = "rbModeMidToOutPoint";
            rbModeMidToOutPoint.Size = new Size(89, 19);
            rbModeMidToOutPoint.TabIndex = 3;
            rbModeMidToOutPoint.TabStop = true;
            rbModeMidToOutPoint.Text = "Center point";
            rbModeMidToOutPoint.UseVisualStyleBackColor = true;
            rbModeMidToOutPoint.CheckedChanged += ControlValueChanged;
            // 
            // rbModeMidToOut
            // 
            rbModeMidToOut.AutoSize = true;
            rbModeMidToOut.Location = new Point(213, 31);
            rbModeMidToOut.Name = "rbModeMidToOut";
            rbModeMidToOut.Size = new Size(78, 19);
            rbModeMidToOut.TabIndex = 2;
            rbModeMidToOut.TabStop = true;
            rbModeMidToOut.Text = "Center out";
            rbModeMidToOut.UseVisualStyleBackColor = true;
            rbModeMidToOut.CheckedChanged += ControlValueChanged;
            // 
            // rbModeEndToStart
            // 
            rbModeEndToStart.AutoSize = true;
            rbModeEndToStart.Location = new Point(112, 31);
            rbModeEndToStart.Name = "rbModeEndToStart";
            rbModeEndToStart.Size = new Size(84, 19);
            rbModeEndToStart.TabIndex = 1;
            rbModeEndToStart.TabStop = true;
            rbModeEndToStart.Text = "End to start";
            rbModeEndToStart.UseVisualStyleBackColor = true;
            rbModeEndToStart.CheckedChanged += ControlValueChanged;
            // 
            // rbModeStartToEnd
            // 
            rbModeStartToEnd.AutoSize = true;
            rbModeStartToEnd.Location = new Point(12, 31);
            rbModeStartToEnd.Name = "rbModeStartToEnd";
            rbModeStartToEnd.Size = new Size(81, 19);
            rbModeStartToEnd.TabIndex = 0;
            rbModeStartToEnd.TabStop = true;
            rbModeStartToEnd.Text = "Start to end";
            rbModeStartToEnd.UseVisualStyleBackColor = true;
            rbModeStartToEnd.CheckedChanged += ControlValueChanged;
            // 
            // lblBrightness
            // 
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(16, 159);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(62, 15);
            lblBrightness.TabIndex = 3;
            lblBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarBrightness.Location = new Point(16, 177);
            trackBarBrightness.Maximum = 100;
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new Size(620, 45);
            trackBarBrightness.TabIndex = 4;
            trackBarBrightness.Value = 100;
            trackBarBrightness.ValueChanged += ControlValueChanged;
            // 
            // lblNormalization
            // 
            lblNormalization.AutoSize = true;
            lblNormalization.Location = new Point(16, 218);
            lblNormalization.Name = "lblNormalization";
            lblNormalization.Size = new Size(114, 15);
            lblNormalization.TabIndex = 5;
            lblNormalization.Text = "Normalization level";
            // 
            // trackBarNormalizationLevel
            // 
            trackBarNormalizationLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarNormalizationLevel.Location = new Point(16, 236);
            trackBarNormalizationLevel.Maximum = 30;
            trackBarNormalizationLevel.Minimum = 1;
            trackBarNormalizationLevel.Name = "trackBarNormalizationLevel";
            trackBarNormalizationLevel.Size = new Size(620, 45);
            trackBarNormalizationLevel.TabIndex = 6;
            trackBarNormalizationLevel.Value = 10;
            trackBarNormalizationLevel.ValueChanged += ControlValueChanged;
            // 
            // lblHueRange
            // 
            lblHueRange.AutoSize = true;
            lblHueRange.Location = new Point(16, 277);
            lblHueRange.Name = "lblHueRange";
            lblHueRange.Size = new Size(59, 15);
            lblHueRange.TabIndex = 7;
            lblHueRange.Text = "Hue range";
            // 
            // ucHueMinMax
            // 
            ucHueMinMax.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueMinMax.HueMax = 360;
            ucHueMinMax.HueMin = 0;
            ucHueMinMax.Location = new Point(16, 295);
            ucHueMinMax.Name = "ucHueMinMax";
            ucHueMinMax.Size = new Size(620, 59);
            ucHueMinMax.TabIndex = 8;
            // 
            // chbReverse
            // 
            chbReverse.AutoSize = true;
            chbReverse.Location = new Point(16, 360);
            chbReverse.Name = "chbReverse";
            chbReverse.Size = new Size(105, 19);
            chbReverse.TabIndex = 9;
            chbReverse.Text = "Reverse output";
            chbReverse.UseVisualStyleBackColor = true;
            chbReverse.CheckedChanged += ControlValueChanged;
            // 
            // chbHueReverse
            // 
            chbHueReverse.AutoSize = true;
            chbHueReverse.Location = new Point(141, 360);
            chbHueReverse.Name = "chbHueReverse";
            chbHueReverse.Size = new Size(90, 19);
            chbHueReverse.TabIndex = 10;
            chbHueReverse.Text = "Reverse hue";
            chbHueReverse.UseVisualStyleBackColor = true;
            chbHueReverse.CheckedChanged += ControlValueChanged;
            // 
            // chbWhite
            // 
            chbWhite.AutoSize = true;
            chbWhite.Location = new Point(251, 360);
            chbWhite.Name = "chbWhite";
            chbWhite.Size = new Size(95, 19);
            chbWhite.TabIndex = 11;
            chbWhite.Text = "White center";
            chbWhite.UseVisualStyleBackColor = true;
            chbWhite.CheckedChanged += ControlValueChanged;
            // 
            // gbBackground
            // 
            gbBackground.Controls.Add(chbBgWhite);
            gbBackground.Controls.Add(lblBgHue);
            gbBackground.Controls.Add(ucHueBg);
            gbBackground.Controls.Add(lblBgBrightness);
            gbBackground.Controls.Add(trackBarBgBrightness);
            gbBackground.Location = new Point(16, 392);
            gbBackground.Name = "gbBackground";
            gbBackground.Size = new Size(620, 152);
            gbBackground.TabIndex = 12;
            gbBackground.TabStop = false;
            gbBackground.Text = "Background";
            // 
            // chbBgWhite
            // 
            chbBgWhite.AutoSize = true;
            chbBgWhite.Location = new Point(16, 111);
            chbBgWhite.Name = "chbBgWhite";
            chbBgWhite.Size = new Size(102, 19);
            chbBgWhite.TabIndex = 4;
            chbBgWhite.Text = "White instead";
            chbBgWhite.UseVisualStyleBackColor = true;
            chbBgWhite.CheckedChanged += ControlValueChanged;
            // 
            // lblBgHue
            // 
            lblBgHue.AutoSize = true;
            lblBgHue.Location = new Point(16, 22);
            lblBgHue.Name = "lblBgHue";
            lblBgHue.Size = new Size(29, 15);
            lblBgHue.TabIndex = 0;
            lblBgHue.Text = "Hue";
            // 
            // ucHueBg
            // 
            ucHueBg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueBg.Hue = 0;
            ucHueBg.Location = new Point(16, 40);
            ucHueBg.MaxVal = 360;
            ucHueBg.MinVal = 0;
            ucHueBg.Name = "ucHueBg";
            ucHueBg.Size = new Size(586, 45);
            ucHueBg.TabIndex = 1;
            // 
            // lblBgBrightness
            // 
            lblBgBrightness.AutoSize = true;
            lblBgBrightness.Location = new Point(140, 112);
            lblBgBrightness.Name = "lblBgBrightness";
            lblBgBrightness.Size = new Size(62, 15);
            lblBgBrightness.TabIndex = 2;
            lblBgBrightness.Text = "Brightness";
            // 
            // trackBarBgBrightness
            // 
            trackBarBgBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarBgBrightness.Location = new Point(208, 100);
            trackBarBgBrightness.Maximum = 100;
            trackBarBgBrightness.Name = "trackBarBgBrightness";
            trackBarBgBrightness.Size = new Size(394, 45);
            trackBarBgBrightness.TabIndex = 3;
            trackBarBgBrightness.ValueChanged += ControlValueChanged;
            // 
            // lblFrequencyLow
            // 
            lblFrequencyLow.AutoSize = true;
            lblFrequencyLow.Location = new Point(16, 560);
            lblFrequencyLow.Name = "lblFrequencyLow";
            lblFrequencyLow.Size = new Size(102, 15);
            lblFrequencyLow.TabIndex = 13;
            lblFrequencyLow.Text = "Frequency low Hz";
            // 
            // trackBarFrequencyLow
            // 
            trackBarFrequencyLow.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarFrequencyLow.Location = new Point(16, 578);
            trackBarFrequencyLow.Maximum = 20000;
            trackBarFrequencyLow.Minimum = 20;
            trackBarFrequencyLow.Name = "trackBarFrequencyLow";
            trackBarFrequencyLow.Size = new Size(620, 45);
            trackBarFrequencyLow.TabIndex = 14;
            trackBarFrequencyLow.TickFrequency = 1000;
            trackBarFrequencyLow.Value = 60;
            trackBarFrequencyLow.ValueChanged += ControlValueChanged;
            // 
            // lblFrequencyHigh
            // 
            lblFrequencyHigh.AutoSize = true;
            lblFrequencyHigh.Location = new Point(16, 619);
            lblFrequencyHigh.Name = "lblFrequencyHigh";
            lblFrequencyHigh.Size = new Size(105, 15);
            lblFrequencyHigh.TabIndex = 15;
            lblFrequencyHigh.Text = "Frequency high Hz";
            // 
            // trackBarFrequencyHigh
            // 
            trackBarFrequencyHigh.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarFrequencyHigh.Location = new Point(16, 637);
            trackBarFrequencyHigh.Maximum = 20000;
            trackBarFrequencyHigh.Minimum = 20;
            trackBarFrequencyHigh.Name = "trackBarFrequencyHigh";
            trackBarFrequencyHigh.Size = new Size(620, 45);
            trackBarFrequencyHigh.TabIndex = 16;
            trackBarFrequencyHigh.TickFrequency = 1000;
            trackBarFrequencyHigh.Value = 250;
            trackBarFrequencyHigh.ValueChanged += ControlValueChanged;
            // 
            // lblLevelLow
            // 
            lblLevelLow.AutoSize = true;
            lblLevelLow.Location = new Point(16, 678);
            lblLevelLow.Name = "lblLevelLow";
            lblLevelLow.Size = new Size(76, 15);
            lblLevelLow.TabIndex = 17;
            lblLevelLow.Text = "Level low dB";
            // 
            // trackBarLevelLow
            // 
            trackBarLevelLow.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarLevelLow.Location = new Point(16, 696);
            trackBarLevelLow.Maximum = 0;
            trackBarLevelLow.Minimum = -90;
            trackBarLevelLow.Name = "trackBarLevelLow";
            trackBarLevelLow.Size = new Size(620, 45);
            trackBarLevelLow.TabIndex = 18;
            trackBarLevelLow.TickFrequency = 10;
            trackBarLevelLow.Value = -60;
            trackBarLevelLow.ValueChanged += ControlValueChanged;
            // 
            // lblLevelHigh
            // 
            lblLevelHigh.AutoSize = true;
            lblLevelHigh.Location = new Point(16, 737);
            lblLevelHigh.Name = "lblLevelHigh";
            lblLevelHigh.Size = new Size(79, 15);
            lblLevelHigh.TabIndex = 19;
            lblLevelHigh.Text = "Level high dB";
            // 
            // trackBarLevelHigh
            // 
            trackBarLevelHigh.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarLevelHigh.Location = new Point(16, 755);
            trackBarLevelHigh.Maximum = 0;
            trackBarLevelHigh.Minimum = -90;
            trackBarLevelHigh.Name = "trackBarLevelHigh";
            trackBarLevelHigh.Size = new Size(620, 45);
            trackBarLevelHigh.TabIndex = 20;
            trackBarLevelHigh.TickFrequency = 10;
            trackBarLevelHigh.Value = -20;
            trackBarLevelHigh.ValueChanged += ControlValueChanged;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(16, 807);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(620, 20);
            progressBar.TabIndex = 21;
            // 
            // SpectralAnalysisSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(654, 842);
            Controls.Add(progressBar);
            Controls.Add(trackBarLevelHigh);
            Controls.Add(lblLevelHigh);
            Controls.Add(trackBarLevelLow);
            Controls.Add(lblLevelLow);
            Controls.Add(trackBarFrequencyHigh);
            Controls.Add(lblFrequencyHigh);
            Controls.Add(trackBarFrequencyLow);
            Controls.Add(lblFrequencyLow);
            Controls.Add(gbBackground);
            Controls.Add(chbWhite);
            Controls.Add(chbHueReverse);
            Controls.Add(chbReverse);
            Controls.Add(ucHueMinMax);
            Controls.Add(lblHueRange);
            Controls.Add(trackBarNormalizationLevel);
            Controls.Add(lblNormalization);
            Controls.Add(trackBarBrightness);
            Controls.Add(lblBrightness);
            Controls.Add(gbModes);
            Controls.Add(cbAudioDevices);
            Controls.Add(lblAudioDevice);
            Name = "SpectralAnalysisSceneEditorForm";
            Text = "Spectral Analysis";
            gbModes.ResumeLayout(false);
            gbModes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarNormalizationLevel).EndInit();
            gbBackground.ResumeLayout(false);
            gbBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBgBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFrequencyLow).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFrequencyHigh).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLevelLow).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLevelHigh).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
