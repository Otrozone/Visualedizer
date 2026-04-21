namespace Ledqualizer
{
    public partial class SpectralAnalysisSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblAudioDevice;
        private ComboBox cmbAudioDevices;
        private GroupBox grpResponseMode;
        private RadioButton rdoBrightnessMode;
        private RadioButton rdoColorPushMode;
        private RadioButton rdoCenterPointMode;
        private RadioButton rdoCenterOutMode;
        private RadioButton rdoEndToStartMode;
        private RadioButton rdoStartToEndMode;
        private Label lblColorSettings;
        private Label lblNormalizationLevel;
        private TrackBar trkNormalizationLevel;
        private Visualedizer.UcHueRangeSaturationBrightness ucPrimaryColor;
        private CheckBox chkReverseOutput;
        private CheckBox chkReverseHue;
        private CheckBox chkWhiteCenter;
        private CheckBox chkBackgroundEnabled;
        private Panel pnlBackgroundSettings;
        private Visualedizer.UcHueSaturationBrightness ucBackgroundSettings;
        private Label lblLowFrequency;
        private TrackBar trkLowFrequency;
        private NumericUpDown nudLowFrequency;
        private Label lblHighFrequency;
        private TrackBar trkHighFrequency;
        private NumericUpDown nudHighFrequency;
        private Label lblLowLevel;
        private TrackBar trkLowLevel;
        private NumericUpDown nudLowLevel;
        private Label lblHighLevel;
        private TrackBar trkHighLevel;
        private NumericUpDown nudHighLevel;
        private ProgressBar prgAudioLevel;

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
            lblAudioDevice = new Label();
            cmbAudioDevices = new ComboBox();
            grpResponseMode = new GroupBox();
            rdoBrightnessMode = new RadioButton();
            rdoColorPushMode = new RadioButton();
            rdoCenterPointMode = new RadioButton();
            rdoCenterOutMode = new RadioButton();
            rdoEndToStartMode = new RadioButton();
            rdoStartToEndMode = new RadioButton();
            lblColorSettings = new Label();
            lblNormalizationLevel = new Label();
            trkNormalizationLevel = new TrackBar();
            ucPrimaryColor = new Visualedizer.UcHueRangeSaturationBrightness();
            chkReverseOutput = new CheckBox();
            chkReverseHue = new CheckBox();
            chkWhiteCenter = new CheckBox();
            chkBackgroundEnabled = new CheckBox();
            pnlBackgroundSettings = new Panel();
            ucBackgroundSettings = new Visualedizer.UcHueSaturationBrightness();
            lblLowFrequency = new Label();
            trkLowFrequency = new TrackBar();
            nudLowFrequency = new NumericUpDown();
            lblHighFrequency = new Label();
            trkHighFrequency = new TrackBar();
            nudHighFrequency = new NumericUpDown();
            lblLowLevel = new Label();
            trkLowLevel = new TrackBar();
            nudLowLevel = new NumericUpDown();
            lblHighLevel = new Label();
            trkHighLevel = new TrackBar();
            nudHighLevel = new NumericUpDown();
            prgAudioLevel = new ProgressBar();
            tblFrequencyRange = new TableLayoutPanel();
            pnlLowFrequency = new Panel();
            pnlHighFrequency = new Panel();
            pnlHighLevel = new Panel();
            pnlLowLevel = new Panel();
            tblLevelRange = new TableLayoutPanel();
            grpResponseMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkNormalizationLevel).BeginInit();
            pnlBackgroundSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trkLowFrequency).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLowFrequency).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkHighFrequency).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHighFrequency).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkLowLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLowLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trkHighLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHighLevel).BeginInit();
            tblFrequencyRange.SuspendLayout();
            pnlLowFrequency.SuspendLayout();
            pnlHighFrequency.SuspendLayout();
            pnlHighLevel.SuspendLayout();
            pnlLowLevel.SuspendLayout();
            tblLevelRange.SuspendLayout();
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
            // cmbAudioDevices
            // 
            cmbAudioDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAudioDevices.Location = new Point(12, 26);
            cmbAudioDevices.Name = "cmbAudioDevices";
            cmbAudioDevices.Size = new Size(297, 23);
            cmbAudioDevices.TabIndex = 1;
            cmbAudioDevices.SelectedIndexChanged += cbAudioDevices_SelectedIndexChanged;
            // 
            // grpResponseMode
            // 
            grpResponseMode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpResponseMode.Controls.Add(rdoBrightnessMode);
            grpResponseMode.Controls.Add(rdoColorPushMode);
            grpResponseMode.Controls.Add(rdoCenterPointMode);
            grpResponseMode.Controls.Add(rdoCenterOutMode);
            grpResponseMode.Controls.Add(rdoEndToStartMode);
            grpResponseMode.Controls.Add(rdoStartToEndMode);
            grpResponseMode.Location = new Point(14, 145);
            grpResponseMode.Name = "grpResponseMode";
            grpResponseMode.Size = new Size(626, 58);
            grpResponseMode.TabIndex = 2;
            grpResponseMode.TabStop = false;
            grpResponseMode.Text = "Response mode";
            // 
            // rdoBrightnessMode
            // 
            rdoBrightnessMode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rdoBrightnessMode.AutoSize = true;
            rdoBrightnessMode.Location = new Point(520, 24);
            rdoBrightnessMode.Name = "rdoBrightnessMode";
            rdoBrightnessMode.Size = new Size(80, 19);
            rdoBrightnessMode.TabIndex = 5;
            rdoBrightnessMode.TabStop = true;
            rdoBrightnessMode.Text = "Brightness";
            rdoBrightnessMode.UseVisualStyleBackColor = true;
            rdoBrightnessMode.CheckedChanged += ControlValueChanged;
            // 
            // rdoColorPushMode
            // 
            rdoColorPushMode.AutoSize = true;
            rdoColorPushMode.Location = new Point(420, 24);
            rdoColorPushMode.Name = "rdoColorPushMode";
            rdoColorPushMode.Size = new Size(83, 19);
            rdoColorPushMode.TabIndex = 4;
            rdoColorPushMode.TabStop = true;
            rdoColorPushMode.Text = "Color push";
            rdoColorPushMode.UseVisualStyleBackColor = true;
            rdoColorPushMode.CheckedChanged += ControlValueChanged;
            // 
            // rdoCenterPointMode
            // 
            rdoCenterPointMode.AutoSize = true;
            rdoCenterPointMode.Location = new Point(314, 24);
            rdoCenterPointMode.Name = "rdoCenterPointMode";
            rdoCenterPointMode.Size = new Size(91, 19);
            rdoCenterPointMode.TabIndex = 3;
            rdoCenterPointMode.TabStop = true;
            rdoCenterPointMode.Text = "Center point";
            rdoCenterPointMode.UseVisualStyleBackColor = true;
            rdoCenterPointMode.CheckedChanged += ControlValueChanged;
            // 
            // rdoCenterOutMode
            // 
            rdoCenterOutMode.AutoSize = true;
            rdoCenterOutMode.Location = new Point(216, 24);
            rdoCenterOutMode.Name = "rdoCenterOutMode";
            rdoCenterOutMode.Size = new Size(81, 19);
            rdoCenterOutMode.TabIndex = 2;
            rdoCenterOutMode.TabStop = true;
            rdoCenterOutMode.Text = "Center out";
            rdoCenterOutMode.UseVisualStyleBackColor = true;
            rdoCenterOutMode.CheckedChanged += ControlValueChanged;
            // 
            // rdoEndToStartMode
            // 
            rdoEndToStartMode.AutoSize = true;
            rdoEndToStartMode.Location = new Point(114, 24);
            rdoEndToStartMode.Name = "rdoEndToStartMode";
            rdoEndToStartMode.Size = new Size(85, 19);
            rdoEndToStartMode.TabIndex = 1;
            rdoEndToStartMode.TabStop = true;
            rdoEndToStartMode.Text = "End to start";
            rdoEndToStartMode.UseVisualStyleBackColor = true;
            rdoEndToStartMode.CheckedChanged += ControlValueChanged;
            // 
            // rdoStartToEndMode
            // 
            rdoStartToEndMode.AutoSize = true;
            rdoStartToEndMode.Location = new Point(12, 24);
            rdoStartToEndMode.Name = "rdoStartToEndMode";
            rdoStartToEndMode.Size = new Size(86, 19);
            rdoStartToEndMode.TabIndex = 0;
            rdoStartToEndMode.TabStop = true;
            rdoStartToEndMode.Text = "Start to end";
            rdoStartToEndMode.UseVisualStyleBackColor = true;
            rdoStartToEndMode.CheckedChanged += ControlValueChanged;
            // 
            // lblColorSettings
            // 
            lblColorSettings.AutoSize = true;
            lblColorSettings.Location = new Point(14, 211);
            lblColorSettings.Name = "lblColorSettings";
            lblColorSettings.Size = new Size(36, 15);
            lblColorSettings.TabIndex = 3;
            lblColorSettings.Text = "Color";
            // 
            // lblNormalizationLevel
            // 
            lblNormalizationLevel.AutoSize = true;
            lblNormalizationLevel.Location = new Point(14, 411);
            lblNormalizationLevel.Name = "lblNormalizationLevel";
            lblNormalizationLevel.Size = new Size(109, 15);
            lblNormalizationLevel.TabIndex = 5;
            lblNormalizationLevel.Text = "Normalization level";
            // 
            // trkNormalizationLevel
            // 
            trkNormalizationLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkNormalizationLevel.Location = new Point(14, 427);
            trkNormalizationLevel.Maximum = 30;
            trkNormalizationLevel.Minimum = 1;
            trkNormalizationLevel.Name = "trkNormalizationLevel";
            trkNormalizationLevel.Size = new Size(623, 45);
            trkNormalizationLevel.TabIndex = 6;
            trkNormalizationLevel.Value = 10;
            trkNormalizationLevel.ValueChanged += ControlValueChanged;
            // 
            // ucPrimaryColor
            // 
            ucPrimaryColor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucPrimaryColor.Location = new Point(14, 227);
            ucPrimaryColor.Margin = new Padding(0);
            ucPrimaryColor.MinimumSize = new Size(120, 84);
            ucPrimaryColor.Name = "ucPrimaryColor";
            ucPrimaryColor.Size = new Size(623, 84);
            ucPrimaryColor.TabIndex = 4;
            // 
            // chkReverseOutput
            // 
            chkReverseOutput.AutoSize = true;
            chkReverseOutput.Location = new Point(14, 479);
            chkReverseOutput.Name = "chkReverseOutput";
            chkReverseOutput.Size = new Size(105, 19);
            chkReverseOutput.TabIndex = 9;
            chkReverseOutput.Text = "Reverse output";
            chkReverseOutput.UseVisualStyleBackColor = true;
            chkReverseOutput.CheckedChanged += ControlValueChanged;
            // 
            // chkReverseHue
            // 
            chkReverseHue.AutoSize = true;
            chkReverseHue.Location = new Point(132, 479);
            chkReverseHue.Name = "chkReverseHue";
            chkReverseHue.Size = new Size(89, 19);
            chkReverseHue.TabIndex = 10;
            chkReverseHue.Text = "Reverse hue";
            chkReverseHue.UseVisualStyleBackColor = true;
            chkReverseHue.CheckedChanged += ControlValueChanged;
            // 
            // chkWhiteCenter
            // 
            chkWhiteCenter.AutoSize = true;
            chkWhiteCenter.Location = new Point(240, 479);
            chkWhiteCenter.Name = "chkWhiteCenter";
            chkWhiteCenter.Size = new Size(93, 19);
            chkWhiteCenter.TabIndex = 11;
            chkWhiteCenter.Text = "White center";
            chkWhiteCenter.UseVisualStyleBackColor = true;
            chkWhiteCenter.CheckedChanged += ControlValueChanged;
            // 
            // chkBackgroundEnabled
            // 
            chkBackgroundEnabled.AutoSize = true;
            chkBackgroundEnabled.Location = new Point(14, 505);
            chkBackgroundEnabled.Name = "chkBackgroundEnabled";
            chkBackgroundEnabled.Size = new Size(118, 19);
            chkBackgroundEnabled.TabIndex = 12;
            chkBackgroundEnabled.Text = "Background color";
            chkBackgroundEnabled.UseVisualStyleBackColor = true;
            chkBackgroundEnabled.CheckedChanged += chkBackgroundEnabled_CheckedChanged;
            // 
            // pnlBackgroundSettings
            // 
            pnlBackgroundSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlBackgroundSettings.Controls.Add(ucBackgroundSettings);
            pnlBackgroundSettings.Location = new Point(14, 530);
            pnlBackgroundSettings.Name = "pnlBackgroundSettings";
            pnlBackgroundSettings.Size = new Size(623, 84);
            pnlBackgroundSettings.TabIndex = 13;
            // 
            // ucBackgroundSettings
            // 
            ucBackgroundSettings.Dock = DockStyle.Fill;
            ucBackgroundSettings.Location = new Point(0, 0);
            ucBackgroundSettings.Margin = new Padding(0);
            ucBackgroundSettings.MinimumSize = new Size(120, 84);
            ucBackgroundSettings.Name = "ucBackgroundSettings";
            ucBackgroundSettings.Size = new Size(623, 84);
            ucBackgroundSettings.TabIndex = 0;
            // 
            // lblLowFrequency
            // 
            lblLowFrequency.AutoSize = true;
            lblLowFrequency.Location = new Point(0, 8);
            lblLowFrequency.Name = "lblLowFrequency";
            lblLowFrequency.Size = new Size(101, 15);
            lblLowFrequency.TabIndex = 14;
            lblLowFrequency.Text = "Frequency low Hz";
            // 
            // trkLowFrequency
            // 
            trkLowFrequency.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkLowFrequency.Location = new Point(0, 24);
            trkLowFrequency.Maximum = 20000;
            trkLowFrequency.Minimum = 20;
            trkLowFrequency.Name = "trkLowFrequency";
            trkLowFrequency.Size = new Size(218, 45);
            trkLowFrequency.TabIndex = 15;
            trkLowFrequency.TickFrequency = 1000;
            trkLowFrequency.Value = 60;
            trkLowFrequency.ValueChanged += trackBarFrequencyLow_ValueChanged;
            // 
            // nudLowFrequency
            // 
            nudLowFrequency.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            nudLowFrequency.Location = new Point(224, 35);
            nudLowFrequency.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
            nudLowFrequency.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            nudLowFrequency.Name = "nudLowFrequency";
            nudLowFrequency.Size = new Size(80, 23);
            nudLowFrequency.TabIndex = 16;
            nudLowFrequency.Value = new decimal(new int[] { 60, 0, 0, 0 });
            nudLowFrequency.ValueChanged += numFrequencyLow_ValueChanged;
            // 
            // lblHighFrequency
            // 
            lblHighFrequency.AutoSize = true;
            lblHighFrequency.Location = new Point(0, 8);
            lblHighFrequency.Name = "lblHighFrequency";
            lblHighFrequency.Size = new Size(106, 15);
            lblHighFrequency.TabIndex = 17;
            lblHighFrequency.Text = "Frequency high Hz";
            // 
            // trkHighFrequency
            // 
            trkHighFrequency.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkHighFrequency.Location = new Point(0, 24);
            trkHighFrequency.Maximum = 20000;
            trkHighFrequency.Minimum = 20;
            trkHighFrequency.Name = "trkHighFrequency";
            trkHighFrequency.Size = new Size(217, 45);
            trkHighFrequency.TabIndex = 18;
            trkHighFrequency.TickFrequency = 1000;
            trkHighFrequency.Value = 250;
            trkHighFrequency.ValueChanged += trackBarFrequencyHigh_ValueChanged;
            // 
            // nudHighFrequency
            // 
            nudHighFrequency.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            nudHighFrequency.Location = new Point(223, 35);
            nudHighFrequency.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
            nudHighFrequency.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            nudHighFrequency.Name = "nudHighFrequency";
            nudHighFrequency.Size = new Size(78, 23);
            nudHighFrequency.TabIndex = 19;
            nudHighFrequency.Value = new decimal(new int[] { 250, 0, 0, 0 });
            nudHighFrequency.ValueChanged += numFrequencyHigh_ValueChanged;
            // 
            // lblLowLevel
            // 
            lblLowLevel.AutoSize = true;
            lblLowLevel.Location = new Point(0, 8);
            lblLowLevel.Name = "lblLowLevel";
            lblLowLevel.Size = new Size(73, 15);
            lblLowLevel.TabIndex = 20;
            lblLowLevel.Text = "Level low dB";
            // 
            // trkLowLevel
            // 
            trkLowLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkLowLevel.Location = new Point(0, 24);
            trkLowLevel.Maximum = 0;
            trkLowLevel.Minimum = -90;
            trkLowLevel.Name = "trkLowLevel";
            trkLowLevel.Size = new Size(218, 45);
            trkLowLevel.TabIndex = 21;
            trkLowLevel.TickFrequency = 10;
            trkLowLevel.Value = -60;
            trkLowLevel.ValueChanged += trackBarLevelLow_ValueChanged;
            // 
            // nudLowLevel
            // 
            nudLowLevel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            nudLowLevel.Location = new Point(224, 35);
            nudLowLevel.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            nudLowLevel.Minimum = new decimal(new int[] { 90, 0, 0, int.MinValue });
            nudLowLevel.Name = "nudLowLevel";
            nudLowLevel.Size = new Size(80, 23);
            nudLowLevel.TabIndex = 22;
            nudLowLevel.Value = new decimal(new int[] { 60, 0, 0, int.MinValue });
            nudLowLevel.ValueChanged += numLevelLow_ValueChanged;
            // 
            // lblHighLevel
            // 
            lblHighLevel.AutoSize = true;
            lblHighLevel.Location = new Point(0, 8);
            lblHighLevel.Name = "lblHighLevel";
            lblHighLevel.Size = new Size(78, 15);
            lblHighLevel.TabIndex = 23;
            lblHighLevel.Text = "Level high dB";
            // 
            // trkHighLevel
            // 
            trkHighLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trkHighLevel.Location = new Point(0, 24);
            trkHighLevel.Maximum = 0;
            trkHighLevel.Minimum = -90;
            trkHighLevel.Name = "trkHighLevel";
            trkHighLevel.Size = new Size(217, 45);
            trkHighLevel.TabIndex = 24;
            trkHighLevel.TickFrequency = 10;
            trkHighLevel.Value = -20;
            trkHighLevel.ValueChanged += trackBarLevelHigh_ValueChanged;
            // 
            // nudHighLevel
            // 
            nudHighLevel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            nudHighLevel.Location = new Point(223, 35);
            nudHighLevel.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            nudHighLevel.Minimum = new decimal(new int[] { 90, 0, 0, int.MinValue });
            nudHighLevel.Name = "nudHighLevel";
            nudHighLevel.Size = new Size(78, 23);
            nudHighLevel.TabIndex = 25;
            nudHighLevel.Value = new decimal(new int[] { 20, 0, 0, int.MinValue });
            nudHighLevel.ValueChanged += numLevelHigh_ValueChanged;
            // 
            // prgAudioLevel
            // 
            prgAudioLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            prgAudioLevel.Location = new Point(326, 28);
            prgAudioLevel.Name = "prgAudioLevel";
            prgAudioLevel.Size = new Size(313, 21);
            prgAudioLevel.TabIndex = 25;
            // 
            // tblFrequencyRange
            // 
            tblFrequencyRange.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tblFrequencyRange.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tblFrequencyRange.ColumnCount = 2;
            tblFrequencyRange.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblFrequencyRange.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblFrequencyRange.Controls.Add(pnlLowFrequency, 0, 0);
            tblFrequencyRange.Controls.Add(pnlHighFrequency, 1, 0);
            tblFrequencyRange.Location = new Point(13, 56);
            tblFrequencyRange.Margin = new Padding(0);
            tblFrequencyRange.Name = "tblFrequencyRange";
            tblFrequencyRange.RowCount = 1;
            tblFrequencyRange.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            tblFrequencyRange.Size = new Size(623, 82);
            tblFrequencyRange.TabIndex = 26;
            // 
            // pnlLowFrequency
            // 
            pnlLowFrequency.Controls.Add(trkLowFrequency);
            pnlLowFrequency.Controls.Add(lblLowFrequency);
            pnlLowFrequency.Controls.Add(nudLowFrequency);
            pnlLowFrequency.Dock = DockStyle.Fill;
            pnlLowFrequency.Location = new Point(1, 1);
            pnlLowFrequency.Margin = new Padding(0, 0, 6, 0);
            pnlLowFrequency.Name = "pnlLowFrequency";
            pnlLowFrequency.Size = new Size(304, 80);
            pnlLowFrequency.TabIndex = 27;
            // 
            // pnlHighFrequency
            // 
            pnlHighFrequency.Controls.Add(lblHighFrequency);
            pnlHighFrequency.Controls.Add(trkHighFrequency);
            pnlHighFrequency.Controls.Add(nudHighFrequency);
            pnlHighFrequency.Dock = DockStyle.Fill;
            pnlHighFrequency.Location = new Point(318, 1);
            pnlHighFrequency.Margin = new Padding(6, 0, 0, 0);
            pnlHighFrequency.Name = "pnlHighFrequency";
            pnlHighFrequency.Size = new Size(304, 80);
            pnlHighFrequency.TabIndex = 28;
            // 
            // pnlHighLevel
            // 
            pnlHighLevel.Controls.Add(lblHighLevel);
            pnlHighLevel.Controls.Add(trkHighLevel);
            pnlHighLevel.Controls.Add(nudHighLevel);
            pnlHighLevel.Dock = DockStyle.Fill;
            pnlHighLevel.Location = new Point(318, 1);
            pnlHighLevel.Margin = new Padding(6, 0, 0, 0);
            pnlHighLevel.Name = "pnlHighLevel";
            pnlHighLevel.Size = new Size(304, 80);
            pnlHighLevel.TabIndex = 27;
            // 
            // pnlLowLevel
            // 
            pnlLowLevel.Controls.Add(lblLowLevel);
            pnlLowLevel.Controls.Add(trkLowLevel);
            pnlLowLevel.Controls.Add(nudLowLevel);
            pnlLowLevel.Dock = DockStyle.Fill;
            pnlLowLevel.Location = new Point(1, 1);
            pnlLowLevel.Margin = new Padding(0, 0, 6, 0);
            pnlLowLevel.Name = "pnlLowLevel";
            pnlLowLevel.Size = new Size(304, 80);
            pnlLowLevel.TabIndex = 29;
            // 
            // tblLevelRange
            // 
            tblLevelRange.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tblLevelRange.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tblLevelRange.ColumnCount = 2;
            tblLevelRange.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblLevelRange.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblLevelRange.Controls.Add(pnlLowLevel, 0, 0);
            tblLevelRange.Controls.Add(pnlHighLevel, 1, 0);
            tblLevelRange.Location = new Point(14, 319);
            tblLevelRange.Margin = new Padding(0);
            tblLevelRange.Name = "tblLevelRange";
            tblLevelRange.RowCount = 1;
            tblLevelRange.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            tblLevelRange.Size = new Size(623, 82);
            tblLevelRange.TabIndex = 30;
            // 
            // SpectralAnalysisSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(654, 641);
            Controls.Add(tblLevelRange);
            Controls.Add(tblFrequencyRange);
            Controls.Add(prgAudioLevel);
            Controls.Add(pnlBackgroundSettings);
            Controls.Add(chkBackgroundEnabled);
            Controls.Add(chkWhiteCenter);
            Controls.Add(chkReverseHue);
            Controls.Add(chkReverseOutput);
            Controls.Add(ucPrimaryColor);
            Controls.Add(trkNormalizationLevel);
            Controls.Add(lblNormalizationLevel);
            Controls.Add(lblColorSettings);
            Controls.Add(grpResponseMode);
            Controls.Add(cmbAudioDevices);
            Controls.Add(lblAudioDevice);
            Name = "SpectralAnalysisSceneEditorForm";
            Text = "Spectral Analysis";
            grpResponseMode.ResumeLayout(false);
            grpResponseMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trkNormalizationLevel).EndInit();
            pnlBackgroundSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)trkLowFrequency).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLowFrequency).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkHighFrequency).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHighFrequency).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkLowLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLowLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)trkHighLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHighLevel).EndInit();
            tblFrequencyRange.ResumeLayout(false);
            pnlLowFrequency.ResumeLayout(false);
            pnlLowFrequency.PerformLayout();
            pnlHighFrequency.ResumeLayout(false);
            pnlHighFrequency.PerformLayout();
            pnlHighLevel.ResumeLayout(false);
            pnlHighLevel.PerformLayout();
            pnlLowLevel.ResumeLayout(false);
            pnlLowLevel.PerformLayout();
            tblLevelRange.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private TableLayoutPanel tblFrequencyRange;
        private Panel pnlLowFrequency;
        private Panel pnlHighFrequency;
        private Panel pnlHighLevel;
        private Panel pnlLowLevel;
        private TableLayoutPanel tblLevelRange;
    }
}
