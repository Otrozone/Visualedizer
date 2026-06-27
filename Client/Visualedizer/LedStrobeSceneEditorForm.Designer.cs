namespace Ledqualizer
{
    partial class LedStrobeSceneEditorForm
    {
        private System.ComponentModel.IContainer? components = null;
        private TableLayoutPanel layoutRoot = null!;
        private Label lblInfo = null!;
        private GroupBox gbOperation = null!;
        private TableLayoutPanel layoutOperation = null!;
        private Label lblOperationMode = null!;
        private ComboBox cbOperationMode = null!;
        private Label lblAudioDevice = null!;
        private ComboBox cbAudioDevices = null!;
        private Label lblVolumeThreshold = null!;
        private NumericUpDown numVolumeThreshold = null!;
        private Label lblVolumeChance = null!;
        private NumericUpDown numVolumeChance = null!;
        private Label lblBandLowHz = null!;
        private NumericUpDown numBandLowHz = null!;
        private Label lblBandHighHz = null!;
        private NumericUpDown numBandHighHz = null!;
        private Label lblBandThresholdDb = null!;
        private NumericUpDown numBandThresholdDb = null!;
        private Label lblBandChance = null!;
        private NumericUpDown numBandChance = null!;
        private GroupBox gbOnTiming = null!;
        private TableLayoutPanel layoutOnTiming = null!;
        private Label lblOnDurationMode = null!;
        private ComboBox cbOnDurationMode = null!;
        private Label lblOnDurationMs = null!;
        private NumericUpDown numOnDurationMs = null!;
        private Label lblOnDurationMinMs = null!;
        private NumericUpDown numOnDurationMinMs = null!;
        private Label lblOnDurationMaxMs = null!;
        private NumericUpDown numOnDurationMaxMs = null!;
        private GroupBox gbOffTiming = null!;
        private TableLayoutPanel layoutOffTiming = null!;
        private Label lblOffDurationMode = null!;
        private ComboBox cbOffDurationMode = null!;
        private Label lblOffDurationMs = null!;
        private NumericUpDown numOffDurationMs = null!;
        private Label lblOffDurationMinMs = null!;
        private NumericUpDown numOffDurationMinMs = null!;
        private Label lblOffDurationMaxMs = null!;
        private NumericUpDown numOffDurationMaxMs = null!;
        private GroupBox gbColor = null!;
        private TableLayoutPanel layoutColor = null!;
        private Label lblHueMode = null!;
        private ComboBox cbHueMode = null!;
        private Label lblHue = null!;
        private Visualedizer.UcHue ucHue = null!;
        private Label lblHueRange = null!;
        private Visualedizer.UcHueMinMax ucHueRange = null!;
        private Label lblSaturation = null!;
        private NumericUpDown numSaturation = null!;
        private Label lblBrightness = null!;
        private NumericUpDown numBrightness = null!;

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
            layoutRoot = new TableLayoutPanel();
            lblInfo = new Label();
            gbOperation = new GroupBox();
            layoutOperation = new TableLayoutPanel();
            lblOperationMode = new Label();
            cbOperationMode = new ComboBox();
            lblAudioDevice = new Label();
            cbAudioDevices = new ComboBox();
            lblVolumeThreshold = new Label();
            numVolumeThreshold = new NumericUpDown();
            lblVolumeChance = new Label();
            numVolumeChance = new NumericUpDown();
            lblBandLowHz = new Label();
            numBandLowHz = new NumericUpDown();
            lblBandHighHz = new Label();
            numBandHighHz = new NumericUpDown();
            lblBandThresholdDb = new Label();
            numBandThresholdDb = new NumericUpDown();
            lblBandChance = new Label();
            numBandChance = new NumericUpDown();
            gbOnTiming = new GroupBox();
            layoutOnTiming = new TableLayoutPanel();
            lblOnDurationMode = new Label();
            cbOnDurationMode = new ComboBox();
            lblOnDurationMs = new Label();
            numOnDurationMs = new NumericUpDown();
            lblOnDurationMinMs = new Label();
            numOnDurationMinMs = new NumericUpDown();
            lblOnDurationMaxMs = new Label();
            numOnDurationMaxMs = new NumericUpDown();
            gbOffTiming = new GroupBox();
            layoutOffTiming = new TableLayoutPanel();
            lblOffDurationMode = new Label();
            cbOffDurationMode = new ComboBox();
            lblOffDurationMs = new Label();
            numOffDurationMs = new NumericUpDown();
            lblOffDurationMinMs = new Label();
            numOffDurationMinMs = new NumericUpDown();
            lblOffDurationMaxMs = new Label();
            numOffDurationMaxMs = new NumericUpDown();
            gbColor = new GroupBox();
            layoutColor = new TableLayoutPanel();
            lblHueMode = new Label();
            cbHueMode = new ComboBox();
            lblHue = new Label();
            ucHue = new Visualedizer.UcHue();
            lblHueRange = new Label();
            ucHueRange = new Visualedizer.UcHueMinMax();
            lblSaturation = new Label();
            numSaturation = new NumericUpDown();
            lblBrightness = new Label();
            numBrightness = new NumericUpDown();
            layoutRoot.SuspendLayout();
            gbOperation.SuspendLayout();
            layoutOperation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numVolumeThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVolumeChance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBandLowHz).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBandHighHz).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBandThresholdDb).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBandChance).BeginInit();
            gbOnTiming.SuspendLayout();
            layoutOnTiming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMinMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMaxMs).BeginInit();
            gbOffTiming.SuspendLayout();
            layoutOffTiming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMinMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMaxMs).BeginInit();
            gbColor.SuspendLayout();
            layoutColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSaturation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBrightness).BeginInit();
            SuspendLayout();
            //
            // layoutRoot
            //
            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Controls.Add(lblInfo, 0, 0);
            layoutRoot.Controls.Add(gbOperation, 0, 1);
            layoutRoot.Controls.Add(gbOnTiming, 0, 2);
            layoutRoot.Controls.Add(gbOffTiming, 0, 3);
            layoutRoot.Controls.Add(gbColor, 0, 4);
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Location = new Point(0, 0);
            layoutRoot.Name = "layoutRoot";
            layoutRoot.Padding = new Padding(12);
            layoutRoot.RowCount = 5;
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.Size = new Size(920, 470);
            layoutRoot.TabIndex = 0;
            //
            // lblInfo
            //
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(15, 12);
            lblInfo.Margin = new Padding(3, 0, 3, 8);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(718, 15);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "Configure timed or audio-triggered LED strip strobe flashes. Random ranges are sampled at each phase or flash transition.";
            //
            // gbOperation
            //
            gbOperation.AutoSize = true;
            gbOperation.Controls.Add(layoutOperation);
            gbOperation.Dock = DockStyle.Fill;
            gbOperation.Location = new Point(15, 38);
            gbOperation.Name = "gbOperation";
            gbOperation.Size = new Size(890, 106);
            gbOperation.TabIndex = 1;
            gbOperation.TabStop = false;
            gbOperation.Text = "Operation";
            //
            // layoutOperation
            //
            layoutOperation.AutoSize = true;
            layoutOperation.ColumnCount = 8;
            layoutOperation.ColumnStyles.Add(new ColumnStyle());
            layoutOperation.ColumnStyles.Add(new ColumnStyle());
            layoutOperation.ColumnStyles.Add(new ColumnStyle());
            layoutOperation.ColumnStyles.Add(new ColumnStyle());
            layoutOperation.ColumnStyles.Add(new ColumnStyle());
            layoutOperation.ColumnStyles.Add(new ColumnStyle());
            layoutOperation.ColumnStyles.Add(new ColumnStyle());
            layoutOperation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutOperation.Controls.Add(lblOperationMode, 0, 0);
            layoutOperation.Controls.Add(cbOperationMode, 1, 0);
            layoutOperation.Controls.Add(lblAudioDevice, 2, 0);
            layoutOperation.Controls.Add(cbAudioDevices, 3, 0);
            layoutOperation.Controls.Add(lblVolumeThreshold, 0, 1);
            layoutOperation.Controls.Add(numVolumeThreshold, 1, 1);
            layoutOperation.Controls.Add(lblVolumeChance, 2, 1);
            layoutOperation.Controls.Add(numVolumeChance, 3, 1);
            layoutOperation.Controls.Add(lblBandLowHz, 0, 2);
            layoutOperation.Controls.Add(numBandLowHz, 1, 2);
            layoutOperation.Controls.Add(lblBandHighHz, 2, 2);
            layoutOperation.Controls.Add(numBandHighHz, 3, 2);
            layoutOperation.Controls.Add(lblBandThresholdDb, 4, 2);
            layoutOperation.Controls.Add(numBandThresholdDb, 5, 2);
            layoutOperation.Controls.Add(lblBandChance, 6, 2);
            layoutOperation.Controls.Add(numBandChance, 7, 2);
            layoutOperation.Dock = DockStyle.Fill;
            layoutOperation.Location = new Point(3, 19);
            layoutOperation.Name = "layoutOperation";
            layoutOperation.Padding = new Padding(8);
            layoutOperation.RowCount = 3;
            layoutOperation.RowStyles.Add(new RowStyle());
            layoutOperation.RowStyles.Add(new RowStyle());
            layoutOperation.RowStyles.Add(new RowStyle());
            layoutOperation.Size = new Size(884, 84);
            layoutOperation.TabIndex = 0;
            //
            // lblOperationMode
            //
            lblOperationMode.Anchor = AnchorStyles.Left;
            lblOperationMode.AutoSize = true;
            lblOperationMode.Location = new Point(11, 15);
            lblOperationMode.Name = "lblOperationMode";
            lblOperationMode.Size = new Size(38, 15);
            lblOperationMode.TabIndex = 0;
            lblOperationMode.Text = "Mode";
            //
            // cbOperationMode
            //
            cbOperationMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOperationMode.FormattingEnabled = true;
            cbOperationMode.Location = new Point(101, 11);
            cbOperationMode.Name = "cbOperationMode";
            cbOperationMode.Size = new Size(190, 23);
            cbOperationMode.TabIndex = 1;
            cbOperationMode.SelectedIndexChanged += ControlValueChanged;
            //
            // lblAudioDevice
            //
            lblAudioDevice.Anchor = AnchorStyles.Left;
            lblAudioDevice.AutoSize = true;
            lblAudioDevice.Location = new Point(297, 15);
            lblAudioDevice.Name = "lblAudioDevice";
            lblAudioDevice.Size = new Size(77, 15);
            lblAudioDevice.TabIndex = 2;
            lblAudioDevice.Text = "Audio device";
            //
            // cbAudioDevices
            //
            cbAudioDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAudioDevices.FormattingEnabled = true;
            cbAudioDevices.Location = new Point(431, 11);
            cbAudioDevices.Name = "cbAudioDevices";
            cbAudioDevices.Size = new Size(360, 23);
            cbAudioDevices.TabIndex = 3;
            cbAudioDevices.SelectedIndexChanged += cbAudioDevices_SelectedIndexChanged;
            layoutOperation.SetColumnSpan(cbAudioDevices, 5);
            //
            // lblVolumeThreshold
            //
            lblVolumeThreshold.Anchor = AnchorStyles.Left;
            lblVolumeThreshold.AutoSize = true;
            lblVolumeThreshold.Location = new Point(11, 44);
            lblVolumeThreshold.Name = "lblVolumeThreshold";
            lblVolumeThreshold.Size = new Size(84, 15);
            lblVolumeThreshold.TabIndex = 4;
            lblVolumeThreshold.Text = "Threshold %";
            //
            // numVolumeThreshold
            //
            numVolumeThreshold.Location = new Point(101, 40);
            numVolumeThreshold.Maximum = 100;
            numVolumeThreshold.Name = "numVolumeThreshold";
            numVolumeThreshold.Size = new Size(90, 23);
            numVolumeThreshold.TabIndex = 5;
            numVolumeThreshold.Value = 65;
            numVolumeThreshold.ValueChanged += ControlValueChanged;
            //
            // lblVolumeChance
            //
            lblVolumeChance.Anchor = AnchorStyles.Left;
            lblVolumeChance.AutoSize = true;
            lblVolumeChance.Location = new Point(297, 44);
            lblVolumeChance.Name = "lblVolumeChance";
            lblVolumeChance.Size = new Size(58, 15);
            lblVolumeChance.TabIndex = 6;
            lblVolumeChance.Text = "Chance %";
            //
            // numVolumeChance
            //
            numVolumeChance.Location = new Point(431, 40);
            numVolumeChance.Maximum = 100;
            numVolumeChance.Name = "numVolumeChance";
            numVolumeChance.Size = new Size(90, 23);
            numVolumeChance.TabIndex = 7;
            numVolumeChance.Value = 100;
            numVolumeChance.ValueChanged += ControlValueChanged;
            //
            // lblBandLowHz
            //
            lblBandLowHz.Anchor = AnchorStyles.Left;
            lblBandLowHz.AutoSize = true;
            lblBandLowHz.Location = new Point(11, 73);
            lblBandLowHz.Name = "lblBandLowHz";
            lblBandLowHz.Size = new Size(45, 15);
            lblBandLowHz.TabIndex = 8;
            lblBandLowHz.Text = "Low Hz";
            //
            // numBandLowHz
            //
            numBandLowHz.Increment = 10;
            numBandLowHz.Location = new Point(101, 69);
            numBandLowHz.Maximum = 20000;
            numBandLowHz.Minimum = 20;
            numBandLowHz.Name = "numBandLowHz";
            numBandLowHz.Size = new Size(90, 23);
            numBandLowHz.TabIndex = 9;
            numBandLowHz.Value = 60;
            numBandLowHz.ValueChanged += ControlValueChanged;
            //
            // lblBandHighHz
            //
            lblBandHighHz.Anchor = AnchorStyles.Left;
            lblBandHighHz.AutoSize = true;
            lblBandHighHz.Location = new Point(297, 73);
            lblBandHighHz.Name = "lblBandHighHz";
            lblBandHighHz.Size = new Size(47, 15);
            lblBandHighHz.TabIndex = 10;
            lblBandHighHz.Text = "High Hz";
            //
            // numBandHighHz
            //
            numBandHighHz.Increment = 10;
            numBandHighHz.Location = new Point(431, 69);
            numBandHighHz.Maximum = 20000;
            numBandHighHz.Minimum = 20;
            numBandHighHz.Name = "numBandHighHz";
            numBandHighHz.Size = new Size(90, 23);
            numBandHighHz.TabIndex = 11;
            numBandHighHz.Value = 250;
            numBandHighHz.ValueChanged += ControlValueChanged;
            //
            // lblBandThresholdDb
            //
            lblBandThresholdDb.Anchor = AnchorStyles.Left;
            lblBandThresholdDb.AutoSize = true;
            lblBandThresholdDb.Location = new Point(527, 73);
            lblBandThresholdDb.Name = "lblBandThresholdDb";
            lblBandThresholdDb.Size = new Size(79, 15);
            lblBandThresholdDb.TabIndex = 12;
            lblBandThresholdDb.Text = "Threshold dB";
            //
            // numBandThresholdDb
            //
            numBandThresholdDb.DecimalPlaces = 1;
            numBandThresholdDb.Increment = 0.5M;
            numBandThresholdDb.Location = new Point(612, 69);
            numBandThresholdDb.Maximum = 0;
            numBandThresholdDb.Minimum = -90;
            numBandThresholdDb.Name = "numBandThresholdDb";
            numBandThresholdDb.Size = new Size(80, 23);
            numBandThresholdDb.TabIndex = 13;
            numBandThresholdDb.Value = -30;
            numBandThresholdDb.ValueChanged += ControlValueChanged;
            //
            // lblBandChance
            //
            lblBandChance.Anchor = AnchorStyles.Left;
            lblBandChance.AutoSize = true;
            lblBandChance.Location = new Point(698, 73);
            lblBandChance.Name = "lblBandChance";
            lblBandChance.Size = new Size(58, 15);
            lblBandChance.TabIndex = 14;
            lblBandChance.Text = "Chance %";
            //
            // numBandChance
            //
            numBandChance.Location = new Point(762, 69);
            numBandChance.Maximum = 100;
            numBandChance.Name = "numBandChance";
            numBandChance.Size = new Size(80, 23);
            numBandChance.TabIndex = 15;
            numBandChance.Value = 100;
            numBandChance.ValueChanged += ControlValueChanged;
            //
            // gbOnTiming
            //
            gbOnTiming.AutoSize = true;
            gbOnTiming.Controls.Add(layoutOnTiming);
            gbOnTiming.Dock = DockStyle.Fill;
            gbOnTiming.Location = new Point(15, 150);
            gbOnTiming.Name = "gbOnTiming";
            gbOnTiming.Size = new Size(890, 72);
            gbOnTiming.TabIndex = 2;
            gbOnTiming.TabStop = false;
            gbOnTiming.Text = "On time";
            //
            // layoutOnTiming
            //
            layoutOnTiming.AutoSize = true;
            layoutOnTiming.ColumnCount = 8;
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutOnTiming.Controls.Add(lblOnDurationMode, 0, 0);
            layoutOnTiming.Controls.Add(cbOnDurationMode, 1, 0);
            layoutOnTiming.Controls.Add(lblOnDurationMs, 2, 0);
            layoutOnTiming.Controls.Add(numOnDurationMs, 3, 0);
            layoutOnTiming.Controls.Add(lblOnDurationMinMs, 4, 0);
            layoutOnTiming.Controls.Add(numOnDurationMinMs, 5, 0);
            layoutOnTiming.Controls.Add(lblOnDurationMaxMs, 6, 0);
            layoutOnTiming.Controls.Add(numOnDurationMaxMs, 7, 0);
            layoutOnTiming.Dock = DockStyle.Fill;
            layoutOnTiming.Location = new Point(3, 19);
            layoutOnTiming.Name = "layoutOnTiming";
            layoutOnTiming.Padding = new Padding(8);
            layoutOnTiming.RowCount = 1;
            layoutOnTiming.RowStyles.Add(new RowStyle());
            layoutOnTiming.Size = new Size(884, 50);
            layoutOnTiming.TabIndex = 0;
            //
            // lblOnDurationMode
            //
            lblOnDurationMode.Anchor = AnchorStyles.Left;
            lblOnDurationMode.AutoSize = true;
            lblOnDurationMode.Location = new Point(11, 17);
            lblOnDurationMode.Name = "lblOnDurationMode";
            lblOnDurationMode.Size = new Size(38, 15);
            lblOnDurationMode.TabIndex = 0;
            lblOnDurationMode.Text = "Mode";
            //
            // cbOnDurationMode
            //
            cbOnDurationMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOnDurationMode.FormattingEnabled = true;
            cbOnDurationMode.Location = new Point(55, 11);
            cbOnDurationMode.Name = "cbOnDurationMode";
            cbOnDurationMode.Size = new Size(130, 23);
            cbOnDurationMode.TabIndex = 1;
            cbOnDurationMode.SelectedIndexChanged += ControlValueChanged;
            //
            // lblOnDurationMs
            //
            lblOnDurationMs.Anchor = AnchorStyles.Left;
            lblOnDurationMs.AutoSize = true;
            lblOnDurationMs.Location = new Point(191, 17);
            lblOnDurationMs.Name = "lblOnDurationMs";
            lblOnDurationMs.Size = new Size(22, 15);
            lblOnDurationMs.TabIndex = 2;
            lblOnDurationMs.Text = "ms";
            //
            // numOnDurationMs
            //
            numOnDurationMs.Increment = 10;
            numOnDurationMs.Location = new Point(219, 11);
            numOnDurationMs.Maximum = 600000;
            numOnDurationMs.Minimum = 1;
            numOnDurationMs.Name = "numOnDurationMs";
            numOnDurationMs.Size = new Size(90, 23);
            numOnDurationMs.TabIndex = 3;
            numOnDurationMs.Value = 80;
            numOnDurationMs.ValueChanged += ControlValueChanged;
            //
            // lblOnDurationMinMs
            //
            lblOnDurationMinMs.Anchor = AnchorStyles.Left;
            lblOnDurationMinMs.AutoSize = true;
            lblOnDurationMinMs.Location = new Point(315, 17);
            lblOnDurationMinMs.Name = "lblOnDurationMinMs";
            lblOnDurationMinMs.Size = new Size(45, 15);
            lblOnDurationMinMs.TabIndex = 4;
            lblOnDurationMinMs.Text = "Min ms";
            //
            // numOnDurationMinMs
            //
            numOnDurationMinMs.Increment = 10;
            numOnDurationMinMs.Location = new Point(366, 11);
            numOnDurationMinMs.Maximum = 600000;
            numOnDurationMinMs.Minimum = 1;
            numOnDurationMinMs.Name = "numOnDurationMinMs";
            numOnDurationMinMs.Size = new Size(90, 23);
            numOnDurationMinMs.TabIndex = 5;
            numOnDurationMinMs.Value = 80;
            numOnDurationMinMs.ValueChanged += ControlValueChanged;
            //
            // lblOnDurationMaxMs
            //
            lblOnDurationMaxMs.Anchor = AnchorStyles.Left;
            lblOnDurationMaxMs.AutoSize = true;
            lblOnDurationMaxMs.Location = new Point(462, 17);
            lblOnDurationMaxMs.Name = "lblOnDurationMaxMs";
            lblOnDurationMaxMs.Size = new Size(47, 15);
            lblOnDurationMaxMs.TabIndex = 6;
            lblOnDurationMaxMs.Text = "Max ms";
            //
            // numOnDurationMaxMs
            //
            numOnDurationMaxMs.Increment = 10;
            numOnDurationMaxMs.Location = new Point(515, 11);
            numOnDurationMaxMs.Maximum = 600000;
            numOnDurationMaxMs.Minimum = 1;
            numOnDurationMaxMs.Name = "numOnDurationMaxMs";
            numOnDurationMaxMs.Size = new Size(90, 23);
            numOnDurationMaxMs.TabIndex = 7;
            numOnDurationMaxMs.Value = 160;
            numOnDurationMaxMs.ValueChanged += ControlValueChanged;
            //
            // gbOffTiming
            //
            gbOffTiming.AutoSize = true;
            gbOffTiming.Controls.Add(layoutOffTiming);
            gbOffTiming.Dock = DockStyle.Fill;
            gbOffTiming.Location = new Point(15, 228);
            gbOffTiming.Name = "gbOffTiming";
            gbOffTiming.Size = new Size(890, 72);
            gbOffTiming.TabIndex = 3;
            gbOffTiming.TabStop = false;
            gbOffTiming.Text = "Off time";
            //
            // layoutOffTiming
            //
            layoutOffTiming.AutoSize = true;
            layoutOffTiming.ColumnCount = 8;
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutOffTiming.Controls.Add(lblOffDurationMode, 0, 0);
            layoutOffTiming.Controls.Add(cbOffDurationMode, 1, 0);
            layoutOffTiming.Controls.Add(lblOffDurationMs, 2, 0);
            layoutOffTiming.Controls.Add(numOffDurationMs, 3, 0);
            layoutOffTiming.Controls.Add(lblOffDurationMinMs, 4, 0);
            layoutOffTiming.Controls.Add(numOffDurationMinMs, 5, 0);
            layoutOffTiming.Controls.Add(lblOffDurationMaxMs, 6, 0);
            layoutOffTiming.Controls.Add(numOffDurationMaxMs, 7, 0);
            layoutOffTiming.Dock = DockStyle.Fill;
            layoutOffTiming.Location = new Point(3, 19);
            layoutOffTiming.Name = "layoutOffTiming";
            layoutOffTiming.Padding = new Padding(8);
            layoutOffTiming.RowCount = 1;
            layoutOffTiming.RowStyles.Add(new RowStyle());
            layoutOffTiming.Size = new Size(884, 50);
            layoutOffTiming.TabIndex = 0;
            //
            // lblOffDurationMode
            //
            lblOffDurationMode.Anchor = AnchorStyles.Left;
            lblOffDurationMode.AutoSize = true;
            lblOffDurationMode.Location = new Point(11, 17);
            lblOffDurationMode.Name = "lblOffDurationMode";
            lblOffDurationMode.Size = new Size(38, 15);
            lblOffDurationMode.TabIndex = 0;
            lblOffDurationMode.Text = "Mode";
            //
            // cbOffDurationMode
            //
            cbOffDurationMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOffDurationMode.FormattingEnabled = true;
            cbOffDurationMode.Location = new Point(55, 11);
            cbOffDurationMode.Name = "cbOffDurationMode";
            cbOffDurationMode.Size = new Size(130, 23);
            cbOffDurationMode.TabIndex = 1;
            cbOffDurationMode.SelectedIndexChanged += ControlValueChanged;
            //
            // lblOffDurationMs
            //
            lblOffDurationMs.Anchor = AnchorStyles.Left;
            lblOffDurationMs.AutoSize = true;
            lblOffDurationMs.Location = new Point(191, 17);
            lblOffDurationMs.Name = "lblOffDurationMs";
            lblOffDurationMs.Size = new Size(22, 15);
            lblOffDurationMs.TabIndex = 2;
            lblOffDurationMs.Text = "ms";
            //
            // numOffDurationMs
            //
            numOffDurationMs.Increment = 10;
            numOffDurationMs.Location = new Point(219, 11);
            numOffDurationMs.Maximum = 600000;
            numOffDurationMs.Minimum = 1;
            numOffDurationMs.Name = "numOffDurationMs";
            numOffDurationMs.Size = new Size(90, 23);
            numOffDurationMs.TabIndex = 3;
            numOffDurationMs.Value = 80;
            numOffDurationMs.ValueChanged += ControlValueChanged;
            //
            // lblOffDurationMinMs
            //
            lblOffDurationMinMs.Anchor = AnchorStyles.Left;
            lblOffDurationMinMs.AutoSize = true;
            lblOffDurationMinMs.Location = new Point(315, 17);
            lblOffDurationMinMs.Name = "lblOffDurationMinMs";
            lblOffDurationMinMs.Size = new Size(45, 15);
            lblOffDurationMinMs.TabIndex = 4;
            lblOffDurationMinMs.Text = "Min ms";
            //
            // numOffDurationMinMs
            //
            numOffDurationMinMs.Increment = 10;
            numOffDurationMinMs.Location = new Point(366, 11);
            numOffDurationMinMs.Maximum = 600000;
            numOffDurationMinMs.Minimum = 1;
            numOffDurationMinMs.Name = "numOffDurationMinMs";
            numOffDurationMinMs.Size = new Size(90, 23);
            numOffDurationMinMs.TabIndex = 5;
            numOffDurationMinMs.Value = 40;
            numOffDurationMinMs.ValueChanged += ControlValueChanged;
            //
            // lblOffDurationMaxMs
            //
            lblOffDurationMaxMs.Anchor = AnchorStyles.Left;
            lblOffDurationMaxMs.AutoSize = true;
            lblOffDurationMaxMs.Location = new Point(462, 17);
            lblOffDurationMaxMs.Name = "lblOffDurationMaxMs";
            lblOffDurationMaxMs.Size = new Size(47, 15);
            lblOffDurationMaxMs.TabIndex = 6;
            lblOffDurationMaxMs.Text = "Max ms";
            //
            // numOffDurationMaxMs
            //
            numOffDurationMaxMs.Increment = 10;
            numOffDurationMaxMs.Location = new Point(515, 11);
            numOffDurationMaxMs.Maximum = 600000;
            numOffDurationMaxMs.Minimum = 1;
            numOffDurationMaxMs.Name = "numOffDurationMaxMs";
            numOffDurationMaxMs.Size = new Size(90, 23);
            numOffDurationMaxMs.TabIndex = 7;
            numOffDurationMaxMs.Value = 160;
            numOffDurationMaxMs.ValueChanged += ControlValueChanged;
            //
            // gbColor
            //
            gbColor.AutoSize = true;
            gbColor.Controls.Add(layoutColor);
            gbColor.Dock = DockStyle.Fill;
            gbColor.Location = new Point(15, 306);
            gbColor.Name = "gbColor";
            gbColor.Size = new Size(890, 122);
            gbColor.TabIndex = 4;
            gbColor.TabStop = false;
            gbColor.Text = "Color";
            //
            // layoutColor
            //
            layoutColor.AutoSize = true;
            layoutColor.ColumnCount = 6;
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.Controls.Add(lblHueMode, 0, 0);
            layoutColor.Controls.Add(cbHueMode, 1, 0);
            layoutColor.Controls.Add(lblSaturation, 4, 0);
            layoutColor.Controls.Add(numSaturation, 5, 0);
            layoutColor.Controls.Add(lblHue, 0, 1);
            layoutColor.Controls.Add(ucHue, 1, 1);
            layoutColor.Controls.Add(lblBrightness, 4, 1);
            layoutColor.Controls.Add(numBrightness, 5, 1);
            layoutColor.Controls.Add(lblHueRange, 0, 2);
            layoutColor.Controls.Add(ucHueRange, 1, 2);
            layoutColor.Dock = DockStyle.Fill;
            layoutColor.Location = new Point(3, 19);
            layoutColor.Name = "layoutColor";
            layoutColor.Padding = new Padding(8);
            layoutColor.RowCount = 3;
            layoutColor.RowStyles.Add(new RowStyle());
            layoutColor.RowStyles.Add(new RowStyle());
            layoutColor.RowStyles.Add(new RowStyle());
            layoutColor.Size = new Size(884, 100);
            layoutColor.TabIndex = 0;
            //
            // lblHueMode
            //
            lblHueMode.Anchor = AnchorStyles.Left;
            lblHueMode.AutoSize = true;
            lblHueMode.Location = new Point(11, 17);
            lblHueMode.Name = "lblHueMode";
            lblHueMode.Size = new Size(38, 15);
            lblHueMode.TabIndex = 0;
            lblHueMode.Text = "Mode";
            //
            // cbHueMode
            //
            cbHueMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbHueMode.FormattingEnabled = true;
            cbHueMode.Location = new Point(58, 11);
            cbHueMode.Name = "cbHueMode";
            cbHueMode.Size = new Size(130, 23);
            cbHueMode.TabIndex = 1;
            cbHueMode.SelectedIndexChanged += ControlValueChanged;
            //
            // lblHue
            //
            lblHue.Anchor = AnchorStyles.Left;
            lblHue.AutoSize = true;
            lblHue.Location = new Point(11, 45);
            lblHue.Name = "lblHue";
            lblHue.Size = new Size(29, 15);
            lblHue.TabIndex = 4;
            lblHue.Text = "Hue";
            //
            // ucHue
            //
            ucHue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            layoutColor.SetColumnSpan(ucHue, 3);
            ucHue.Hue = 0;
            ucHue.Location = new Point(58, 40);
            ucHue.MaxVal = 360;
            ucHue.MinVal = 0;
            ucHue.Name = "ucHue";
            ucHue.Size = new Size(580, 26);
            ucHue.TabIndex = 5;
            ucHue.ValueChanged += ControlValueChanged;
            //
            // lblHueRange
            //
            lblHueRange.Anchor = AnchorStyles.Left;
            lblHueRange.AutoSize = true;
            lblHueRange.Location = new Point(11, 73);
            lblHueRange.Name = "lblHueRange";
            lblHueRange.Size = new Size(41, 15);
            lblHueRange.TabIndex = 8;
            lblHueRange.Text = "Range";
            //
            // ucHueRange
            //
            ucHueRange.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            layoutColor.SetColumnSpan(ucHueRange, 3);
            ucHueRange.HueEnd = 360;
            ucHueRange.HueStart = 0;
            ucHueRange.Location = new Point(58, 68);
            ucHueRange.Name = "ucHueRange";
            ucHueRange.Size = new Size(580, 26);
            ucHueRange.TabIndex = 9;
            ucHueRange.ValueChanged += ControlValueChanged;
            //
            // lblSaturation
            //
            lblSaturation.Anchor = AnchorStyles.Left;
            lblSaturation.AutoSize = true;
            lblSaturation.Location = new Point(644, 17);
            lblSaturation.Name = "lblSaturation";
            lblSaturation.Size = new Size(72, 15);
            lblSaturation.TabIndex = 2;
            lblSaturation.Text = "Saturation %";
            //
            // numSaturation
            //
            numSaturation.Location = new Point(722, 11);
            numSaturation.Maximum = 100;
            numSaturation.Name = "numSaturation";
            numSaturation.Size = new Size(80, 23);
            numSaturation.TabIndex = 3;
            numSaturation.Value = 100;
            numSaturation.ValueChanged += ControlValueChanged;
            //
            // lblBrightness
            //
            lblBrightness.Anchor = AnchorStyles.Left;
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(644, 45);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(74, 15);
            lblBrightness.TabIndex = 6;
            lblBrightness.Text = "Brightness %";
            //
            // numBrightness
            //
            numBrightness.Location = new Point(722, 40);
            numBrightness.Maximum = 100;
            numBrightness.Name = "numBrightness";
            numBrightness.Size = new Size(80, 23);
            numBrightness.TabIndex = 7;
            numBrightness.Value = 100;
            numBrightness.ValueChanged += ControlValueChanged;
            //
            // LedStrobeSceneEditorForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(920, 470);
            Controls.Add(layoutRoot);
            Name = "LedStrobeSceneEditorForm";
            Text = "Strobe";
            layoutRoot.ResumeLayout(false);
            layoutRoot.PerformLayout();
            gbOperation.ResumeLayout(false);
            gbOperation.PerformLayout();
            layoutOperation.ResumeLayout(false);
            layoutOperation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numVolumeThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVolumeChance).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBandLowHz).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBandHighHz).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBandThresholdDb).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBandChance).EndInit();
            gbOnTiming.ResumeLayout(false);
            gbOnTiming.PerformLayout();
            layoutOnTiming.ResumeLayout(false);
            layoutOnTiming.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMinMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMaxMs).EndInit();
            gbOffTiming.ResumeLayout(false);
            gbOffTiming.PerformLayout();
            layoutOffTiming.ResumeLayout(false);
            layoutOffTiming.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMinMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMaxMs).EndInit();
            gbColor.ResumeLayout(false);
            gbColor.PerformLayout();
            layoutColor.ResumeLayout(false);
            layoutColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSaturation).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBrightness).EndInit();
            ResumeLayout(false);
        }
    }
}
