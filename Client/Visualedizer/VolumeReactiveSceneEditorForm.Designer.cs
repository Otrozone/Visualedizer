namespace Ledqualizer
{
    partial class VolumeReactiveSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblAudioDevice;
        private ComboBox cbAudioDevices;
        private GroupBox gbModes;
        private RadioButton rbModeStartToEnd;
        private RadioButton rbModeEndToStart;
        private RadioButton rbModeMidToOut;
        private RadioButton rbModeMidToOutPoint;
        private RadioButton rbModeColorPush;
        private RadioButton rbBrightness;
        private CheckBox chbRotate;
        private TrackBar trackBarRotate;
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
        private ProgressBar progressBar;
        private System.Windows.Forms.Timer timerRotate;

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
            chbRotate = new CheckBox();
            trackBarRotate = new TrackBar();
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
            progressBar = new ProgressBar();
            timerRotate = new System.Windows.Forms.Timer(components);
            gbModes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarRotate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarNormalizationLevel).BeginInit();
            gbBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBgBrightness).BeginInit();
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
            gbModes.Text = "Mode";
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
            // chbRotate
            // 
            chbRotate.AutoSize = true;
            chbRotate.Location = new Point(16, 159);
            chbRotate.Name = "chbRotate";
            chbRotate.Size = new Size(94, 19);
            chbRotate.TabIndex = 3;
            chbRotate.Text = "Rotate modes";
            chbRotate.UseVisualStyleBackColor = true;
            chbRotate.CheckedChanged += chbRotate_CheckedChanged;
            // 
            // trackBarRotate
            // 
            trackBarRotate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarRotate.Location = new Point(126, 150);
            trackBarRotate.Maximum = 60;
            trackBarRotate.Minimum = 1;
            trackBarRotate.Name = "trackBarRotate";
            trackBarRotate.Size = new Size(510, 45);
            trackBarRotate.TabIndex = 4;
            trackBarRotate.Value = 20;
            trackBarRotate.ValueChanged += ControlValueChanged;
            // 
            // lblBrightness
            // 
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(16, 196);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(62, 15);
            lblBrightness.TabIndex = 5;
            lblBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarBrightness.Location = new Point(16, 214);
            trackBarBrightness.Maximum = 100;
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new Size(620, 45);
            trackBarBrightness.TabIndex = 6;
            trackBarBrightness.Value = 100;
            trackBarBrightness.ValueChanged += ControlValueChanged;
            // 
            // lblNormalization
            // 
            lblNormalization.AutoSize = true;
            lblNormalization.Location = new Point(16, 255);
            lblNormalization.Name = "lblNormalization";
            lblNormalization.Size = new Size(114, 15);
            lblNormalization.TabIndex = 7;
            lblNormalization.Text = "Normalization level";
            // 
            // trackBarNormalizationLevel
            // 
            trackBarNormalizationLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarNormalizationLevel.Location = new Point(16, 273);
            trackBarNormalizationLevel.Maximum = 30;
            trackBarNormalizationLevel.Minimum = 1;
            trackBarNormalizationLevel.Name = "trackBarNormalizationLevel";
            trackBarNormalizationLevel.Size = new Size(620, 45);
            trackBarNormalizationLevel.TabIndex = 8;
            trackBarNormalizationLevel.Value = 10;
            trackBarNormalizationLevel.ValueChanged += ControlValueChanged;
            // 
            // lblHueRange
            // 
            lblHueRange.AutoSize = true;
            lblHueRange.Location = new Point(16, 314);
            lblHueRange.Name = "lblHueRange";
            lblHueRange.Size = new Size(59, 15);
            lblHueRange.TabIndex = 9;
            lblHueRange.Text = "Hue range";
            // 
            // ucHueMinMax
            // 
            ucHueMinMax.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueMinMax.HueMax = 360;
            ucHueMinMax.HueMin = 0;
            ucHueMinMax.Location = new Point(16, 332);
            ucHueMinMax.Name = "ucHueMinMax";
            ucHueMinMax.Size = new Size(620, 59);
            ucHueMinMax.TabIndex = 10;
            // 
            // chbReverse
            // 
            chbReverse.AutoSize = true;
            chbReverse.Location = new Point(16, 397);
            chbReverse.Name = "chbReverse";
            chbReverse.Size = new Size(105, 19);
            chbReverse.TabIndex = 11;
            chbReverse.Text = "Reverse output";
            chbReverse.UseVisualStyleBackColor = true;
            chbReverse.CheckedChanged += ControlValueChanged;
            // 
            // chbHueReverse
            // 
            chbHueReverse.AutoSize = true;
            chbHueReverse.Location = new Point(141, 397);
            chbHueReverse.Name = "chbHueReverse";
            chbHueReverse.Size = new Size(90, 19);
            chbHueReverse.TabIndex = 12;
            chbHueReverse.Text = "Reverse hue";
            chbHueReverse.UseVisualStyleBackColor = true;
            chbHueReverse.CheckedChanged += ControlValueChanged;
            // 
            // chbWhite
            // 
            chbWhite.AutoSize = true;
            chbWhite.Location = new Point(251, 397);
            chbWhite.Name = "chbWhite";
            chbWhite.Size = new Size(95, 19);
            chbWhite.TabIndex = 13;
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
            gbBackground.Location = new Point(16, 429);
            gbBackground.Name = "gbBackground";
            gbBackground.Size = new Size(620, 152);
            gbBackground.TabIndex = 14;
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
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(16, 587);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(620, 20);
            progressBar.TabIndex = 15;
            // 
            // timerRotate
            // 
            timerRotate.Interval = 20000;
            timerRotate.Tick += timerRotate_Tick;
            // 
            // VolumeReactiveSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(654, 622);
            Controls.Add(progressBar);
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
            Controls.Add(trackBarRotate);
            Controls.Add(chbRotate);
            Controls.Add(gbModes);
            Controls.Add(cbAudioDevices);
            Controls.Add(lblAudioDevice);
            Name = "VolumeReactiveSceneEditorForm";
            Text = "Volume Reactive";
            gbModes.ResumeLayout(false);
            gbModes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarRotate).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarNormalizationLevel).EndInit();
            gbBackground.ResumeLayout(false);
            gbBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarBgBrightness).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
