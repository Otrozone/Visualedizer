namespace Ledqualizer
{
    partial class StrobeSceneEditorForm
    {
        private System.ComponentModel.IContainer? components = null;
        private TableLayoutPanel layoutRoot = null!;
        private Label lblInfo = null!;
        private TableLayoutPanel layoutTriggerHeader = null!;
        private Label lblEventType = null!;
        private Label lblRetriggerMode = null!;
        private Label lblOnDuration = null!;
        private TableLayoutPanel layoutTriggerPanels = null!;
        private TableLayoutPanel layoutVolume = null!;
        private Label lblVolumeAudioDevice = null!;
        private Label lblVolumeThreshold = null!;
        private TableLayoutPanel layoutSpectral = null!;
        private Label lblSpectralAudioDevice = null!;
        private Label lblSpectralLowHz = null!;
        private Label lblSpectralHighHz = null!;
        private Label lblSpectralThresholdDb = null!;
        private TableLayoutPanel layoutScreenContainer = null!;
        private TableLayoutPanel layoutScreen = null!;
        private Label lblScreenMonitor = null!;
        private Label lblScreenX = null!;
        private Label lblScreenY = null!;
        private Label lblScreenWidth = null!;
        private Label lblScreenHeight = null!;
        private FlowLayoutPanel panelScreenSecondRow = null!;
        private Label lblScreenBrightness = null!;
        private FlowLayoutPanel panelActions = null!;
        private ComboBox cbEventType = null!;
        private ComboBox cbRetriggerMode = null!;
        private NumericUpDown numOnDurationMs = null!;
        private Panel pnlVolume = null!;
        private ComboBox cbVolumeAudioDevice = null!;
        private NumericUpDown numVolumeThreshold = null!;
        private Panel pnlSpectral = null!;
        private ComboBox cbSpectralAudioDevice = null!;
        private NumericUpDown numSpectralLowHz = null!;
        private NumericUpDown numSpectralHighHz = null!;
        private NumericUpDown numSpectralThresholdDb = null!;
        private Panel pnlScreen = null!;
        private ComboBox cbScreenMonitor = null!;
        private NumericUpDown numScreenX = null!;
        private NumericUpDown numScreenY = null!;
        private NumericUpDown numScreenWidth = null!;
        private NumericUpDown numScreenHeight = null!;
        private NumericUpDown numScreenBrightnessThreshold = null!;
        private Button btnPickArea = null!;
        private Button btnTest = null!;

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
            layoutTriggerHeader = new TableLayoutPanel();
            lblEventType = new Label();
            cbEventType = new ComboBox();
            lblRetriggerMode = new Label();
            cbRetriggerMode = new ComboBox();
            lblOnDuration = new Label();
            numOnDurationMs = new NumericUpDown();
            layoutTriggerPanels = new TableLayoutPanel();
            pnlVolume = new Panel();
            layoutVolume = new TableLayoutPanel();
            lblVolumeAudioDevice = new Label();
            cbVolumeAudioDevice = new ComboBox();
            lblVolumeThreshold = new Label();
            numVolumeThreshold = new NumericUpDown();
            pnlSpectral = new Panel();
            layoutSpectral = new TableLayoutPanel();
            lblSpectralAudioDevice = new Label();
            cbSpectralAudioDevice = new ComboBox();
            lblSpectralLowHz = new Label();
            numSpectralLowHz = new NumericUpDown();
            lblSpectralHighHz = new Label();
            numSpectralHighHz = new NumericUpDown();
            lblSpectralThresholdDb = new Label();
            numSpectralThresholdDb = new NumericUpDown();
            pnlScreen = new Panel();
            layoutScreenContainer = new TableLayoutPanel();
            layoutScreen = new TableLayoutPanel();
            lblScreenMonitor = new Label();
            cbScreenMonitor = new ComboBox();
            btnPickArea = new Button();
            lblScreenX = new Label();
            numScreenX = new NumericUpDown();
            lblScreenY = new Label();
            numScreenY = new NumericUpDown();
            lblScreenWidth = new Label();
            numScreenWidth = new NumericUpDown();
            lblScreenHeight = new Label();
            panelScreenSecondRow = new FlowLayoutPanel();
            numScreenHeight = new NumericUpDown();
            lblScreenBrightness = new Label();
            numScreenBrightnessThreshold = new NumericUpDown();
            panelActions = new FlowLayoutPanel();
            btnTest = new Button();
            layoutRoot.SuspendLayout();
            layoutTriggerHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMs).BeginInit();
            layoutTriggerPanels.SuspendLayout();
            pnlVolume.SuspendLayout();
            layoutVolume.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numVolumeThreshold).BeginInit();
            pnlSpectral.SuspendLayout();
            layoutSpectral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSpectralLowHz).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSpectralHighHz).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSpectralThresholdDb).BeginInit();
            pnlScreen.SuspendLayout();
            layoutScreenContainer.SuspendLayout();
            layoutScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numScreenX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numScreenY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numScreenWidth).BeginInit();
            panelScreenSecondRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numScreenHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numScreenBrightnessThreshold).BeginInit();
            panelActions.SuspendLayout();
            SuspendLayout();
            // 
            // layoutRoot
            // 
            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Controls.Add(lblInfo, 0, 0);
            layoutRoot.Controls.Add(layoutTriggerHeader, 0, 1);
            layoutRoot.Controls.Add(layoutTriggerPanels, 0, 2);
            layoutRoot.Controls.Add(panelActions, 0, 3);
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Location = new Point(0, 0);
            layoutRoot.Name = "layoutRoot";
            layoutRoot.Padding = new Padding(12);
            layoutRoot.RowCount = 4;
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.Size = new Size(860, 520);
            layoutRoot.TabIndex = 0;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(15, 12);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(686, 15);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "Configure what should trigger the strobe. The output turns on when the trigger fires and turns off when the trigger window ends.";
            // 
            // layoutTriggerHeader
            // 
            layoutTriggerHeader.AutoSize = true;
            layoutTriggerHeader.ColumnCount = 6;
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle());
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle());
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle());
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle());
            layoutTriggerHeader.Controls.Add(lblEventType, 0, 0);
            layoutTriggerHeader.Controls.Add(cbEventType, 1, 0);
            layoutTriggerHeader.Controls.Add(lblRetriggerMode, 2, 0);
            layoutTriggerHeader.Controls.Add(cbRetriggerMode, 3, 0);
            layoutTriggerHeader.Controls.Add(lblOnDuration, 4, 0);
            layoutTriggerHeader.Controls.Add(numOnDurationMs, 5, 0);
            layoutTriggerHeader.Dock = DockStyle.Fill;
            layoutTriggerHeader.Location = new Point(15, 30);
            layoutTriggerHeader.Name = "layoutTriggerHeader";
            layoutTriggerHeader.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            layoutTriggerHeader.Size = new Size(830, 20);
            layoutTriggerHeader.TabIndex = 1;
            // 
            // lblEventType
            // 
            lblEventType.Anchor = AnchorStyles.Left;
            lblEventType.AutoSize = true;
            lblEventType.Location = new Point(3, 2);
            lblEventType.Name = "lblEventType";
            lblEventType.Size = new Size(36, 15);
            lblEventType.TabIndex = 0;
            lblEventType.Text = "Event";
            // 
            // cbEventType
            // 
            cbEventType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEventType.FormattingEnabled = true;
            cbEventType.Location = new Point(45, 3);
            cbEventType.Name = "cbEventType";
            cbEventType.Size = new Size(240, 23);
            cbEventType.TabIndex = 1;
            cbEventType.SelectedIndexChanged += TriggerControlChanged;
            // 
            // lblRetriggerMode
            // 
            lblRetriggerMode.Anchor = AnchorStyles.Left;
            lblRetriggerMode.AutoSize = true;
            lblRetriggerMode.Location = new Point(336, 2);
            lblRetriggerMode.Name = "lblRetriggerMode";
            lblRetriggerMode.Size = new Size(55, 15);
            lblRetriggerMode.TabIndex = 2;
            lblRetriggerMode.Text = "Retrigger";
            // 
            // cbRetriggerMode
            // 
            cbRetriggerMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRetriggerMode.FormattingEnabled = true;
            cbRetriggerMode.Location = new Point(397, 3);
            cbRetriggerMode.Name = "cbRetriggerMode";
            cbRetriggerMode.Size = new Size(240, 23);
            cbRetriggerMode.TabIndex = 3;
            cbRetriggerMode.SelectedIndexChanged += TriggerControlChanged;
            // 
            // lblOnDuration
            // 
            lblOnDuration.Anchor = AnchorStyles.Left;
            lblOnDuration.AutoSize = true;
            lblOnDuration.Location = new Point(688, 2);
            lblOnDuration.Name = "lblOnDuration";
            lblOnDuration.Size = new Size(42, 15);
            lblOnDuration.TabIndex = 4;
            lblOnDuration.Text = "On ms";
            // 
            // numOnDurationMs
            // 
            numOnDurationMs.Location = new Point(736, 3);
            numOnDurationMs.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            numOnDurationMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numOnDurationMs.Name = "numOnDurationMs";
            numOnDurationMs.Size = new Size(90, 23);
            numOnDurationMs.TabIndex = 5;
            numOnDurationMs.Value = new decimal(new int[] { 250, 0, 0, 0 });
            numOnDurationMs.ValueChanged += TriggerControlChanged;
            // 
            // layoutTriggerPanels
            // 
            layoutTriggerPanels.AutoSize = true;
            layoutTriggerPanels.ColumnCount = 1;
            layoutTriggerPanels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutTriggerPanels.Controls.Add(pnlVolume, 0, 0);
            layoutTriggerPanels.Controls.Add(pnlSpectral, 0, 1);
            layoutTriggerPanels.Controls.Add(pnlScreen, 0, 2);
            layoutTriggerPanels.Dock = DockStyle.Fill;
            layoutTriggerPanels.Location = new Point(15, 56);
            layoutTriggerPanels.Name = "layoutTriggerPanels";
            layoutTriggerPanels.RowCount = 3;
            layoutTriggerPanels.RowStyles.Add(new RowStyle());
            layoutTriggerPanels.RowStyles.Add(new RowStyle());
            layoutTriggerPanels.RowStyles.Add(new RowStyle());
            layoutTriggerPanels.Size = new Size(830, 119);
            layoutTriggerPanels.TabIndex = 2;
            // 
            // pnlVolume
            // 
            pnlVolume.AutoSize = true;
            pnlVolume.Controls.Add(layoutVolume);
            pnlVolume.Dock = DockStyle.Fill;
            pnlVolume.Location = new Point(3, 3);
            pnlVolume.Name = "pnlVolume";
            pnlVolume.Size = new Size(824, 20);
            pnlVolume.TabIndex = 0;
            // 
            // layoutVolume
            // 
            layoutVolume.AutoSize = true;
            layoutVolume.ColumnCount = 4;
            layoutVolume.ColumnStyles.Add(new ColumnStyle());
            layoutVolume.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutVolume.ColumnStyles.Add(new ColumnStyle());
            layoutVolume.ColumnStyles.Add(new ColumnStyle());
            layoutVolume.Controls.Add(lblVolumeAudioDevice, 0, 0);
            layoutVolume.Controls.Add(cbVolumeAudioDevice, 1, 0);
            layoutVolume.Controls.Add(lblVolumeThreshold, 2, 0);
            layoutVolume.Controls.Add(numVolumeThreshold, 3, 0);
            layoutVolume.Dock = DockStyle.Fill;
            layoutVolume.Location = new Point(0, 0);
            layoutVolume.Name = "layoutVolume";
            layoutVolume.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            layoutVolume.Size = new Size(824, 20);
            layoutVolume.TabIndex = 0;
            // 
            // lblVolumeAudioDevice
            // 
            lblVolumeAudioDevice.Anchor = AnchorStyles.Left;
            lblVolumeAudioDevice.AutoSize = true;
            lblVolumeAudioDevice.Location = new Point(3, 2);
            lblVolumeAudioDevice.Name = "lblVolumeAudioDevice";
            lblVolumeAudioDevice.Size = new Size(77, 15);
            lblVolumeAudioDevice.TabIndex = 0;
            lblVolumeAudioDevice.Text = "Audio Device";
            // 
            // cbVolumeAudioDevice
            // 
            cbVolumeAudioDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVolumeAudioDevice.FormattingEnabled = true;
            cbVolumeAudioDevice.Location = new Point(86, 3);
            cbVolumeAudioDevice.Name = "cbVolumeAudioDevice";
            cbVolumeAudioDevice.Size = new Size(360, 23);
            cbVolumeAudioDevice.TabIndex = 1;
            cbVolumeAudioDevice.SelectedIndexChanged += TriggerControlChanged;
            // 
            // lblVolumeThreshold
            // 
            lblVolumeThreshold.Anchor = AnchorStyles.Left;
            lblVolumeThreshold.AutoSize = true;
            lblVolumeThreshold.Location = new Point(662, 2);
            lblVolumeThreshold.Name = "lblVolumeThreshold";
            lblVolumeThreshold.Size = new Size(73, 15);
            lblVolumeThreshold.TabIndex = 2;
            lblVolumeThreshold.Text = "Threshold %";
            // 
            // numVolumeThreshold
            // 
            numVolumeThreshold.Location = new Point(741, 3);
            numVolumeThreshold.Name = "numVolumeThreshold";
            numVolumeThreshold.Size = new Size(80, 23);
            numVolumeThreshold.TabIndex = 3;
            numVolumeThreshold.Value = new decimal(new int[] { 65, 0, 0, 0 });
            numVolumeThreshold.ValueChanged += TriggerControlChanged;
            // 
            // pnlSpectral
            // 
            pnlSpectral.AutoSize = true;
            pnlSpectral.Controls.Add(layoutSpectral);
            pnlSpectral.Dock = DockStyle.Fill;
            pnlSpectral.Location = new Point(3, 29);
            pnlSpectral.Name = "pnlSpectral";
            pnlSpectral.Size = new Size(824, 20);
            pnlSpectral.TabIndex = 1;
            // 
            // layoutSpectral
            // 
            layoutSpectral.AutoSize = true;
            layoutSpectral.ColumnCount = 8;
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.Controls.Add(lblSpectralAudioDevice, 0, 0);
            layoutSpectral.Controls.Add(cbSpectralAudioDevice, 1, 0);
            layoutSpectral.Controls.Add(lblSpectralLowHz, 2, 0);
            layoutSpectral.Controls.Add(numSpectralLowHz, 3, 0);
            layoutSpectral.Controls.Add(lblSpectralHighHz, 4, 0);
            layoutSpectral.Controls.Add(numSpectralHighHz, 5, 0);
            layoutSpectral.Controls.Add(lblSpectralThresholdDb, 6, 0);
            layoutSpectral.Controls.Add(numSpectralThresholdDb, 7, 0);
            layoutSpectral.Dock = DockStyle.Fill;
            layoutSpectral.Location = new Point(0, 0);
            layoutSpectral.Name = "layoutSpectral";
            layoutSpectral.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            layoutSpectral.Size = new Size(824, 20);
            layoutSpectral.TabIndex = 0;
            // 
            // lblSpectralAudioDevice
            // 
            lblSpectralAudioDevice.Anchor = AnchorStyles.Left;
            lblSpectralAudioDevice.AutoSize = true;
            lblSpectralAudioDevice.Location = new Point(3, 2);
            lblSpectralAudioDevice.Name = "lblSpectralAudioDevice";
            lblSpectralAudioDevice.Size = new Size(77, 15);
            lblSpectralAudioDevice.TabIndex = 0;
            lblSpectralAudioDevice.Text = "Audio Device";
            // 
            // cbSpectralAudioDevice
            // 
            cbSpectralAudioDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSpectralAudioDevice.FormattingEnabled = true;
            cbSpectralAudioDevice.Location = new Point(86, 3);
            cbSpectralAudioDevice.Name = "cbSpectralAudioDevice";
            cbSpectralAudioDevice.Size = new Size(250, 23);
            cbSpectralAudioDevice.TabIndex = 1;
            cbSpectralAudioDevice.SelectedIndexChanged += TriggerControlChanged;
            // 
            // lblSpectralLowHz
            // 
            lblSpectralLowHz.Anchor = AnchorStyles.Left;
            lblSpectralLowHz.AutoSize = true;
            lblSpectralLowHz.Location = new Point(378, 2);
            lblSpectralLowHz.Name = "lblSpectralLowHz";
            lblSpectralLowHz.Size = new Size(46, 15);
            lblSpectralLowHz.TabIndex = 2;
            lblSpectralLowHz.Text = "Low Hz";
            // 
            // numSpectralLowHz
            // 
            numSpectralLowHz.Location = new Point(430, 3);
            numSpectralLowHz.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
            numSpectralLowHz.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numSpectralLowHz.Name = "numSpectralLowHz";
            numSpectralLowHz.Size = new Size(80, 23);
            numSpectralLowHz.TabIndex = 3;
            numSpectralLowHz.Value = new decimal(new int[] { 60, 0, 0, 0 });
            numSpectralLowHz.ValueChanged += TriggerControlChanged;
            // 
            // lblSpectralHighHz
            // 
            lblSpectralHighHz.Anchor = AnchorStyles.Left;
            lblSpectralHighHz.AutoSize = true;
            lblSpectralHighHz.Location = new Point(516, 2);
            lblSpectralHighHz.Name = "lblSpectralHighHz";
            lblSpectralHighHz.Size = new Size(50, 15);
            lblSpectralHighHz.TabIndex = 4;
            lblSpectralHighHz.Text = "High Hz";
            // 
            // numSpectralHighHz
            // 
            numSpectralHighHz.Location = new Point(572, 3);
            numSpectralHighHz.Maximum = new decimal(new int[] { 20000, 0, 0, 0 });
            numSpectralHighHz.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numSpectralHighHz.Name = "numSpectralHighHz";
            numSpectralHighHz.Size = new Size(80, 23);
            numSpectralHighHz.TabIndex = 5;
            numSpectralHighHz.Value = new decimal(new int[] { 250, 0, 0, 0 });
            numSpectralHighHz.ValueChanged += TriggerControlChanged;
            // 
            // lblSpectralThresholdDb
            // 
            lblSpectralThresholdDb.Anchor = AnchorStyles.Left;
            lblSpectralThresholdDb.AutoSize = true;
            lblSpectralThresholdDb.Location = new Point(658, 2);
            lblSpectralThresholdDb.Name = "lblSpectralThresholdDb";
            lblSpectralThresholdDb.Size = new Size(77, 15);
            lblSpectralThresholdDb.TabIndex = 6;
            lblSpectralThresholdDb.Text = "Threshold dB";
            // 
            // numSpectralThresholdDb
            // 
            numSpectralThresholdDb.Location = new Point(741, 3);
            numSpectralThresholdDb.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            numSpectralThresholdDb.Minimum = new decimal(new int[] { 90, 0, 0, int.MinValue });
            numSpectralThresholdDb.Name = "numSpectralThresholdDb";
            numSpectralThresholdDb.Size = new Size(80, 23);
            numSpectralThresholdDb.TabIndex = 7;
            numSpectralThresholdDb.Value = new decimal(new int[] { 30, 0, 0, int.MinValue });
            numSpectralThresholdDb.ValueChanged += TriggerControlChanged;
            // 
            // pnlScreen
            // 
            pnlScreen.AutoSize = true;
            pnlScreen.Controls.Add(layoutScreenContainer);
            pnlScreen.Dock = DockStyle.Fill;
            pnlScreen.Location = new Point(3, 55);
            pnlScreen.Name = "pnlScreen";
            pnlScreen.Size = new Size(824, 61);
            pnlScreen.TabIndex = 2;
            // 
            // layoutScreenContainer
            // 
            layoutScreenContainer.AutoSize = true;
            layoutScreenContainer.ColumnCount = 1;
            layoutScreenContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutScreenContainer.Controls.Add(layoutScreen, 0, 0);
            layoutScreenContainer.Controls.Add(panelScreenSecondRow, 0, 1);
            layoutScreenContainer.Dock = DockStyle.Fill;
            layoutScreenContainer.Location = new Point(0, 0);
            layoutScreenContainer.Name = "layoutScreenContainer";
            layoutScreenContainer.RowCount = 2;
            layoutScreenContainer.RowStyles.Add(new RowStyle());
            layoutScreenContainer.RowStyles.Add(new RowStyle());
            layoutScreenContainer.Size = new Size(824, 61);
            layoutScreenContainer.TabIndex = 0;
            // 
            // layoutScreen
            // 
            layoutScreen.AutoSize = true;
            layoutScreen.ColumnCount = 10;
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.ColumnStyles.Add(new ColumnStyle());
            layoutScreen.Controls.Add(lblScreenMonitor, 0, 0);
            layoutScreen.Controls.Add(cbScreenMonitor, 1, 0);
            layoutScreen.Controls.Add(btnPickArea, 2, 0);
            layoutScreen.Controls.Add(lblScreenX, 3, 0);
            layoutScreen.Controls.Add(numScreenX, 4, 0);
            layoutScreen.Controls.Add(lblScreenY, 5, 0);
            layoutScreen.Controls.Add(numScreenY, 6, 0);
            layoutScreen.Controls.Add(lblScreenWidth, 7, 0);
            layoutScreen.Controls.Add(numScreenWidth, 8, 0);
            layoutScreen.Controls.Add(lblScreenHeight, 9, 0);
            layoutScreen.Dock = DockStyle.Fill;
            layoutScreen.Location = new Point(3, 3);
            layoutScreen.Name = "layoutScreen";
            layoutScreen.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            layoutScreen.Size = new Size(818, 20);
            layoutScreen.TabIndex = 0;
            // 
            // lblScreenMonitor
            // 
            lblScreenMonitor.Anchor = AnchorStyles.Left;
            lblScreenMonitor.AutoSize = true;
            lblScreenMonitor.Location = new Point(3, 2);
            lblScreenMonitor.Name = "lblScreenMonitor";
            lblScreenMonitor.Size = new Size(50, 15);
            lblScreenMonitor.TabIndex = 0;
            lblScreenMonitor.Text = "Monitor";
            // 
            // cbScreenMonitor
            // 
            cbScreenMonitor.DropDownStyle = ComboBoxStyle.DropDownList;
            cbScreenMonitor.FormattingEnabled = true;
            cbScreenMonitor.Location = new Point(59, 3);
            cbScreenMonitor.Name = "cbScreenMonitor";
            cbScreenMonitor.Size = new Size(250, 23);
            cbScreenMonitor.TabIndex = 1;
            cbScreenMonitor.SelectedIndexChanged += TriggerControlChanged;
            // 
            // btnPickArea
            // 
            btnPickArea.AutoSize = true;
            btnPickArea.Location = new Point(418, 3);
            btnPickArea.Name = "btnPickArea";
            btnPickArea.Size = new Size(68, 14);
            btnPickArea.TabIndex = 2;
            btnPickArea.Text = "Pick Area";
            btnPickArea.UseVisualStyleBackColor = true;
            btnPickArea.Click += btnPickArea_Click;
            // 
            // lblScreenX
            // 
            lblScreenX.Anchor = AnchorStyles.Left;
            lblScreenX.AutoSize = true;
            lblScreenX.Location = new Point(492, 2);
            lblScreenX.Name = "lblScreenX";
            lblScreenX.Size = new Size(14, 15);
            lblScreenX.TabIndex = 3;
            lblScreenX.Text = "X";
            // 
            // numScreenX
            // 
            numScreenX.Location = new Point(512, 3);
            numScreenX.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numScreenX.Name = "numScreenX";
            numScreenX.Size = new Size(75, 23);
            numScreenX.TabIndex = 4;
            numScreenX.ValueChanged += TriggerControlChanged;
            // 
            // lblScreenY
            // 
            lblScreenY.Anchor = AnchorStyles.Left;
            lblScreenY.AutoSize = true;
            lblScreenY.Location = new Point(593, 2);
            lblScreenY.Name = "lblScreenY";
            lblScreenY.Size = new Size(14, 15);
            lblScreenY.TabIndex = 5;
            lblScreenY.Text = "Y";
            // 
            // numScreenY
            // 
            numScreenY.Location = new Point(613, 3);
            numScreenY.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numScreenY.Name = "numScreenY";
            numScreenY.Size = new Size(75, 23);
            numScreenY.TabIndex = 6;
            numScreenY.ValueChanged += TriggerControlChanged;
            // 
            // lblScreenWidth
            // 
            lblScreenWidth.Anchor = AnchorStyles.Left;
            lblScreenWidth.AutoSize = true;
            lblScreenWidth.Location = new Point(694, 2);
            lblScreenWidth.Name = "lblScreenWidth";
            lblScreenWidth.Size = new Size(18, 15);
            lblScreenWidth.TabIndex = 7;
            lblScreenWidth.Text = "W";
            // 
            // numScreenWidth
            // 
            numScreenWidth.Location = new Point(718, 3);
            numScreenWidth.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numScreenWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numScreenWidth.Name = "numScreenWidth";
            numScreenWidth.Size = new Size(75, 23);
            numScreenWidth.TabIndex = 8;
            numScreenWidth.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numScreenWidth.ValueChanged += TriggerControlChanged;
            // 
            // lblScreenHeight
            // 
            lblScreenHeight.Anchor = AnchorStyles.Left;
            lblScreenHeight.AutoSize = true;
            lblScreenHeight.Location = new Point(799, 2);
            lblScreenHeight.Name = "lblScreenHeight";
            lblScreenHeight.Size = new Size(16, 15);
            lblScreenHeight.TabIndex = 9;
            lblScreenHeight.Text = "H";
            // 
            // panelScreenSecondRow
            // 
            panelScreenSecondRow.AutoSize = true;
            panelScreenSecondRow.Controls.Add(numScreenHeight);
            panelScreenSecondRow.Controls.Add(lblScreenBrightness);
            panelScreenSecondRow.Controls.Add(numScreenBrightnessThreshold);
            panelScreenSecondRow.Dock = DockStyle.Fill;
            panelScreenSecondRow.Location = new Point(3, 29);
            panelScreenSecondRow.Name = "panelScreenSecondRow";
            panelScreenSecondRow.Size = new Size(818, 29);
            panelScreenSecondRow.TabIndex = 1;
            // 
            // numScreenHeight
            // 
            numScreenHeight.Location = new Point(3, 3);
            numScreenHeight.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numScreenHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numScreenHeight.Name = "numScreenHeight";
            numScreenHeight.Size = new Size(75, 23);
            numScreenHeight.TabIndex = 0;
            numScreenHeight.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numScreenHeight.ValueChanged += TriggerControlChanged;
            // 
            // lblScreenBrightness
            // 
            lblScreenBrightness.AutoSize = true;
            lblScreenBrightness.Location = new Point(93, 6);
            lblScreenBrightness.Margin = new Padding(12, 6, 0, 0);
            lblScreenBrightness.Name = "lblScreenBrightness";
            lblScreenBrightness.Size = new Size(75, 15);
            lblScreenBrightness.TabIndex = 1;
            lblScreenBrightness.Text = "Brightness %";
            // 
            // numScreenBrightnessThreshold
            // 
            numScreenBrightnessThreshold.Location = new Point(171, 3);
            numScreenBrightnessThreshold.Name = "numScreenBrightnessThreshold";
            numScreenBrightnessThreshold.Size = new Size(75, 23);
            numScreenBrightnessThreshold.TabIndex = 2;
            numScreenBrightnessThreshold.Value = new decimal(new int[] { 70, 0, 0, 0 });
            numScreenBrightnessThreshold.ValueChanged += TriggerControlChanged;
            // 
            // panelActions
            // 
            panelActions.AutoSize = true;
            panelActions.Controls.Add(btnTest);
            panelActions.Dock = DockStyle.Fill;
            panelActions.Location = new Point(15, 181);
            panelActions.Name = "panelActions";
            panelActions.Size = new Size(830, 324);
            panelActions.TabIndex = 3;
            // 
            // btnTest
            // 
            btnTest.AutoSize = true;
            btnTest.Location = new Point(3, 3);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(43, 25);
            btnTest.TabIndex = 0;
            btnTest.Text = "Test";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // StrobeSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(860, 520);
            Controls.Add(layoutRoot);
            Name = "StrobeSceneEditorForm";
            Text = "Strobe";
            layoutRoot.ResumeLayout(false);
            layoutRoot.PerformLayout();
            layoutTriggerHeader.ResumeLayout(false);
            layoutTriggerHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMs).EndInit();
            layoutTriggerPanels.ResumeLayout(false);
            layoutTriggerPanels.PerformLayout();
            pnlVolume.ResumeLayout(false);
            pnlVolume.PerformLayout();
            layoutVolume.ResumeLayout(false);
            layoutVolume.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numVolumeThreshold).EndInit();
            pnlSpectral.ResumeLayout(false);
            pnlSpectral.PerformLayout();
            layoutSpectral.ResumeLayout(false);
            layoutSpectral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSpectralLowHz).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSpectralHighHz).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSpectralThresholdDb).EndInit();
            pnlScreen.ResumeLayout(false);
            pnlScreen.PerformLayout();
            layoutScreenContainer.ResumeLayout(false);
            layoutScreenContainer.PerformLayout();
            layoutScreen.ResumeLayout(false);
            layoutScreen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numScreenX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numScreenY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numScreenWidth).EndInit();
            panelScreenSecondRow.ResumeLayout(false);
            panelScreenSecondRow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numScreenHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)numScreenBrightnessThreshold).EndInit();
            panelActions.ResumeLayout(false);
            panelActions.PerformLayout();
            ResumeLayout(false);
        }
    }
}
