namespace Ledqualizer
{
    public partial class VolumeReactiveSceneEditorForm
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
        private Label lblColorRange;
        private Label lblNormalization;
        private TrackBar trackBarNormalizationLevel;
        private Visualedizer.UcHueRangeSaturationBrightness ucColorRange;
        private CheckBox chbReverse;
        private CheckBox chbHueReverse;
        private CheckBox chbWhite;
        private CheckBox chbBackgroundEnabled;
        private Panel pnlBackgroundSettings;
        private Visualedizer.UcHueSaturationBrightness ucBackgroundSettings;
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
            lblColorRange = new Label();
            lblNormalization = new Label();
            trackBarNormalizationLevel = new TrackBar();
            ucColorRange = new Visualedizer.UcHueRangeSaturationBrightness();
            chbReverse = new CheckBox();
            chbHueReverse = new CheckBox();
            chbWhite = new CheckBox();
            chbBackgroundEnabled = new CheckBox();
            pnlBackgroundSettings = new Panel();
            ucBackgroundSettings = new Visualedizer.UcHueSaturationBrightness();
            progressBar = new ProgressBar();
            timerRotate = new System.Windows.Forms.Timer(components);
            gbModes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarRotate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarNormalizationLevel).BeginInit();
            pnlBackgroundSettings.SuspendLayout();
            SuspendLayout();
            // 
            // lblAudioDevice
            // 
            lblAudioDevice.AutoSize = true;
            lblAudioDevice.Location = new Point(12, 10);
            lblAudioDevice.Name = "lblAudioDevice";
            lblAudioDevice.Size = new Size(76, 15);
            lblAudioDevice.TabIndex = 0;
            lblAudioDevice.Text = "Audio device";
            // 
            // cbAudioDevices
            // 
            cbAudioDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAudioDevices.Location = new Point(12, 26);
            cbAudioDevices.Name = "cbAudioDevices";
            cbAudioDevices.Size = new Size(269, 23);
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
            gbModes.Location = new Point(12, 58);
            gbModes.Name = "gbModes";
            gbModes.Size = new Size(626, 58);
            gbModes.TabIndex = 2;
            gbModes.TabStop = false;
            gbModes.Text = "Mode";
            // 
            // rbBrightness
            // 
            rbBrightness.AutoSize = true;
            rbBrightness.Location = new Point(520, 24);
            rbBrightness.Name = "rbBrightness";
            rbBrightness.Size = new Size(80, 19);
            rbBrightness.TabIndex = 5;
            rbBrightness.TabStop = true;
            rbBrightness.Text = "Brightness";
            rbBrightness.UseVisualStyleBackColor = true;
            rbBrightness.CheckedChanged += ControlValueChanged;
            // 
            // rbModeColorPush
            // 
            rbModeColorPush.AutoSize = true;
            rbModeColorPush.Location = new Point(420, 24);
            rbModeColorPush.Name = "rbModeColorPush";
            rbModeColorPush.Size = new Size(83, 19);
            rbModeColorPush.TabIndex = 4;
            rbModeColorPush.TabStop = true;
            rbModeColorPush.Text = "Color push";
            rbModeColorPush.UseVisualStyleBackColor = true;
            rbModeColorPush.CheckedChanged += ControlValueChanged;
            // 
            // rbModeMidToOutPoint
            // 
            rbModeMidToOutPoint.AutoSize = true;
            rbModeMidToOutPoint.Location = new Point(314, 24);
            rbModeMidToOutPoint.Name = "rbModeMidToOutPoint";
            rbModeMidToOutPoint.Size = new Size(91, 19);
            rbModeMidToOutPoint.TabIndex = 3;
            rbModeMidToOutPoint.TabStop = true;
            rbModeMidToOutPoint.Text = "Center point";
            rbModeMidToOutPoint.UseVisualStyleBackColor = true;
            rbModeMidToOutPoint.CheckedChanged += ControlValueChanged;
            // 
            // rbModeMidToOut
            // 
            rbModeMidToOut.AutoSize = true;
            rbModeMidToOut.Location = new Point(216, 24);
            rbModeMidToOut.Name = "rbModeMidToOut";
            rbModeMidToOut.Size = new Size(81, 19);
            rbModeMidToOut.TabIndex = 2;
            rbModeMidToOut.TabStop = true;
            rbModeMidToOut.Text = "Center out";
            rbModeMidToOut.UseVisualStyleBackColor = true;
            rbModeMidToOut.CheckedChanged += ControlValueChanged;
            // 
            // rbModeEndToStart
            // 
            rbModeEndToStart.AutoSize = true;
            rbModeEndToStart.Location = new Point(114, 24);
            rbModeEndToStart.Name = "rbModeEndToStart";
            rbModeEndToStart.Size = new Size(85, 19);
            rbModeEndToStart.TabIndex = 1;
            rbModeEndToStart.TabStop = true;
            rbModeEndToStart.Text = "End to start";
            rbModeEndToStart.UseVisualStyleBackColor = true;
            rbModeEndToStart.CheckedChanged += ControlValueChanged;
            // 
            // rbModeStartToEnd
            // 
            rbModeStartToEnd.AutoSize = true;
            rbModeStartToEnd.Location = new Point(12, 24);
            rbModeStartToEnd.Name = "rbModeStartToEnd";
            rbModeStartToEnd.Size = new Size(86, 19);
            rbModeStartToEnd.TabIndex = 0;
            rbModeStartToEnd.TabStop = true;
            rbModeStartToEnd.Text = "Start to end";
            rbModeStartToEnd.UseVisualStyleBackColor = true;
            rbModeStartToEnd.CheckedChanged += ControlValueChanged;
            // 
            // chbRotate
            // 
            chbRotate.AutoSize = true;
            chbRotate.Location = new Point(12, 124);
            chbRotate.Name = "chbRotate";
            chbRotate.Size = new Size(99, 19);
            chbRotate.TabIndex = 3;
            chbRotate.Text = "Rotate modes";
            chbRotate.UseVisualStyleBackColor = true;
            chbRotate.CheckedChanged += chbRotate_CheckedChanged;
            // 
            // trackBarRotate
            // 
            trackBarRotate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarRotate.Location = new Point(120, 116);
            trackBarRotate.Maximum = 60;
            trackBarRotate.Minimum = 1;
            trackBarRotate.Name = "trackBarRotate";
            trackBarRotate.Size = new Size(518, 45);
            trackBarRotate.TabIndex = 4;
            trackBarRotate.Value = 20;
            trackBarRotate.ValueChanged += ControlValueChanged;
            // 
            // lblColorRange
            // 
            lblColorRange.AutoSize = true;
            lblColorRange.Location = new Point(12, 154);
            lblColorRange.Name = "lblColorRange";
            lblColorRange.Size = new Size(36, 15);
            lblColorRange.TabIndex = 5;
            lblColorRange.Text = "Color";
            // 
            // lblNormalization
            // 
            lblNormalization.AutoSize = true;
            lblNormalization.Location = new Point(12, 258);
            lblNormalization.Name = "lblNormalization";
            lblNormalization.Size = new Size(109, 15);
            lblNormalization.TabIndex = 7;
            lblNormalization.Text = "Normalization level";
            // 
            // trackBarNormalizationLevel
            // 
            trackBarNormalizationLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarNormalizationLevel.Location = new Point(12, 274);
            trackBarNormalizationLevel.Maximum = 30;
            trackBarNormalizationLevel.Minimum = 1;
            trackBarNormalizationLevel.Name = "trackBarNormalizationLevel";
            trackBarNormalizationLevel.Size = new Size(626, 45);
            trackBarNormalizationLevel.TabIndex = 8;
            trackBarNormalizationLevel.Value = 10;
            trackBarNormalizationLevel.ValueChanged += ControlValueChanged;
            // 
            // ucColorRange
            // 
            ucColorRange.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucColorRange.Location = new Point(12, 170);
            ucColorRange.Margin = new Padding(0);
            ucColorRange.MinimumSize = new Size(120, 84);
            ucColorRange.Name = "ucColorRange";
            ucColorRange.Size = new Size(626, 84);
            ucColorRange.TabIndex = 6;
            // 
            // chbReverse
            // 
            chbReverse.AutoSize = true;
            chbReverse.Location = new Point(12, 330);
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
            chbHueReverse.Location = new Point(130, 330);
            chbHueReverse.Name = "chbHueReverse";
            chbHueReverse.Size = new Size(89, 19);
            chbHueReverse.TabIndex = 12;
            chbHueReverse.Text = "Reverse hue";
            chbHueReverse.UseVisualStyleBackColor = true;
            chbHueReverse.CheckedChanged += ControlValueChanged;
            // 
            // chbWhite
            // 
            chbWhite.AutoSize = true;
            chbWhite.Location = new Point(238, 330);
            chbWhite.Name = "chbWhite";
            chbWhite.Size = new Size(93, 19);
            chbWhite.TabIndex = 13;
            chbWhite.Text = "White center";
            chbWhite.UseVisualStyleBackColor = true;
            chbWhite.CheckedChanged += ControlValueChanged;
            // 
            // chbBackgroundEnabled
            // 
            chbBackgroundEnabled.AutoSize = true;
            chbBackgroundEnabled.Location = new Point(12, 356);
            chbBackgroundEnabled.Name = "chbBackgroundEnabled";
            chbBackgroundEnabled.Size = new Size(120, 19);
            chbBackgroundEnabled.TabIndex = 14;
            chbBackgroundEnabled.Text = "Background color";
            chbBackgroundEnabled.UseVisualStyleBackColor = true;
            chbBackgroundEnabled.CheckedChanged += chbBackgroundEnabled_CheckedChanged;
            // 
            // pnlBackgroundSettings
            // 
            pnlBackgroundSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlBackgroundSettings.Controls.Add(ucBackgroundSettings);
            pnlBackgroundSettings.Location = new Point(12, 381);
            pnlBackgroundSettings.Name = "pnlBackgroundSettings";
            pnlBackgroundSettings.Size = new Size(626, 84);
            pnlBackgroundSettings.TabIndex = 15;
            // 
            // ucBackgroundSettings
            // 
            ucBackgroundSettings.Dock = DockStyle.Fill;
            ucBackgroundSettings.Location = new Point(0, 0);
            ucBackgroundSettings.Margin = new Padding(0);
            ucBackgroundSettings.MinimumSize = new Size(120, 84);
            ucBackgroundSettings.Name = "ucBackgroundSettings";
            ucBackgroundSettings.Size = new Size(626, 84);
            ucBackgroundSettings.TabIndex = 0;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.Location = new Point(300, 30);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(338, 16);
            progressBar.TabIndex = 16;
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
            ClientSize = new Size(654, 506);
            Controls.Add(progressBar);
            Controls.Add(pnlBackgroundSettings);
            Controls.Add(chbBackgroundEnabled);
            Controls.Add(chbWhite);
            Controls.Add(chbHueReverse);
            Controls.Add(chbReverse);
            Controls.Add(ucColorRange);
            Controls.Add(trackBarNormalizationLevel);
            Controls.Add(lblNormalization);
            Controls.Add(lblColorRange);
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
            ((System.ComponentModel.ISupportInitialize)trackBarNormalizationLevel).EndInit();
            pnlBackgroundSettings.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
