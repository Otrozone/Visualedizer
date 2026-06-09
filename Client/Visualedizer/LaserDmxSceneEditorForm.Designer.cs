namespace Ledqualizer
{
    partial class LaserDmxSceneEditorForm
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
        private DataGridView dgvChannels = null!;
        private Button btnAddRow = null!;
        private Button btnRemoveRow = null!;
        private Button btnSend = null!;

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
            dgvChannels = new DataGridView();
            panelActions = new FlowLayoutPanel();
            btnAddRow = new Button();
            btnRemoveRow = new Button();
            btnSend = new Button();

            SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)dgvChannels).BeginInit();
            panelActions.SuspendLayout();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "LaserDmxSceneEditorForm";
            Text = "Laser DMX";
            ClientSize = new Size(980, 620);

            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Padding = new Padding(12);
            layoutRoot.RowCount = 5;
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoot.RowStyles.Add(new RowStyle());

            lblInfo.AutoSize = true;
            lblInfo.Text = "Configure a trigger source and the DMX rows to send when this laser scene fires.";

            layoutTriggerHeader.AutoSize = true;
            layoutTriggerHeader.ColumnCount = 6;
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle());
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle());
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle());
            layoutTriggerHeader.ColumnStyles.Add(new ColumnStyle());
            layoutTriggerHeader.Dock = DockStyle.Fill;

            lblEventType.Anchor = AnchorStyles.Left;
            lblEventType.AutoSize = true;
            lblEventType.Text = "Event";
            cbEventType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEventType.SelectedIndexChanged += TriggerControlChanged;

            lblRetriggerMode.Anchor = AnchorStyles.Left;
            lblRetriggerMode.AutoSize = true;
            lblRetriggerMode.Text = "Retrigger";
            cbRetriggerMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRetriggerMode.SelectedIndexChanged += TriggerControlChanged;

            lblOnDuration.Anchor = AnchorStyles.Left;
            lblOnDuration.AutoSize = true;
            lblOnDuration.Text = "On ms";
            numOnDurationMs.Minimum = 1;
            numOnDurationMs.Maximum = 60000;
            numOnDurationMs.Value = 300;
            numOnDurationMs.Width = 90;
            numOnDurationMs.ValueChanged += TriggerControlChanged;

            layoutTriggerHeader.Controls.Add(lblEventType, 0, 0);
            layoutTriggerHeader.Controls.Add(cbEventType, 1, 0);
            layoutTriggerHeader.Controls.Add(lblRetriggerMode, 2, 0);
            layoutTriggerHeader.Controls.Add(cbRetriggerMode, 3, 0);
            layoutTriggerHeader.Controls.Add(lblOnDuration, 4, 0);
            layoutTriggerHeader.Controls.Add(numOnDurationMs, 5, 0);

            layoutTriggerPanels.AutoSize = true;
            layoutTriggerPanels.ColumnCount = 1;
            layoutTriggerPanels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutTriggerPanels.Dock = DockStyle.Fill;

            pnlVolume.AutoSize = true;
            pnlVolume.Dock = DockStyle.Fill;
            layoutVolume.AutoSize = true;
            layoutVolume.ColumnCount = 4;
            layoutVolume.ColumnStyles.Add(new ColumnStyle());
            layoutVolume.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutVolume.ColumnStyles.Add(new ColumnStyle());
            layoutVolume.ColumnStyles.Add(new ColumnStyle());
            layoutVolume.Dock = DockStyle.Fill;

            lblVolumeAudioDevice.Anchor = AnchorStyles.Left;
            lblVolumeAudioDevice.AutoSize = true;
            lblVolumeAudioDevice.Text = "Audio Device";
            cbVolumeAudioDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVolumeAudioDevice.SelectedIndexChanged += TriggerControlChanged;

            lblVolumeThreshold.Anchor = AnchorStyles.Left;
            lblVolumeThreshold.AutoSize = true;
            lblVolumeThreshold.Text = "Threshold %";
            numVolumeThreshold.Minimum = 0;
            numVolumeThreshold.Maximum = 100;
            numVolumeThreshold.Value = 65;
            numVolumeThreshold.Width = 80;
            numVolumeThreshold.ValueChanged += TriggerControlChanged;

            layoutVolume.Controls.Add(lblVolumeAudioDevice, 0, 0);
            layoutVolume.Controls.Add(cbVolumeAudioDevice, 1, 0);
            layoutVolume.Controls.Add(lblVolumeThreshold, 2, 0);
            layoutVolume.Controls.Add(numVolumeThreshold, 3, 0);
            pnlVolume.Controls.Add(layoutVolume);

            pnlSpectral.AutoSize = true;
            pnlSpectral.Dock = DockStyle.Fill;
            layoutSpectral.AutoSize = true;
            layoutSpectral.ColumnCount = 8;
            layoutSpectral.Dock = DockStyle.Fill;
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());
            layoutSpectral.ColumnStyles.Add(new ColumnStyle());

            lblSpectralAudioDevice.Anchor = AnchorStyles.Left;
            lblSpectralAudioDevice.AutoSize = true;
            lblSpectralAudioDevice.Text = "Audio Device";
            cbSpectralAudioDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSpectralAudioDevice.SelectedIndexChanged += TriggerControlChanged;

            lblSpectralLowHz.Anchor = AnchorStyles.Left;
            lblSpectralLowHz.AutoSize = true;
            lblSpectralLowHz.Text = "Low Hz";
            numSpectralLowHz.Minimum = 20;
            numSpectralLowHz.Maximum = 20000;
            numSpectralLowHz.Value = 60;
            numSpectralLowHz.Width = 80;
            numSpectralLowHz.ValueChanged += TriggerControlChanged;

            lblSpectralHighHz.Anchor = AnchorStyles.Left;
            lblSpectralHighHz.AutoSize = true;
            lblSpectralHighHz.Text = "High Hz";
            numSpectralHighHz.Minimum = 20;
            numSpectralHighHz.Maximum = 20000;
            numSpectralHighHz.Value = 250;
            numSpectralHighHz.Width = 80;
            numSpectralHighHz.ValueChanged += TriggerControlChanged;

            lblSpectralThresholdDb.Anchor = AnchorStyles.Left;
            lblSpectralThresholdDb.AutoSize = true;
            lblSpectralThresholdDb.Text = "Threshold dB";
            numSpectralThresholdDb.Minimum = -90;
            numSpectralThresholdDb.Maximum = 0;
            numSpectralThresholdDb.Value = -30;
            numSpectralThresholdDb.Width = 80;
            numSpectralThresholdDb.ValueChanged += TriggerControlChanged;

            layoutSpectral.Controls.Add(lblSpectralAudioDevice, 0, 0);
            layoutSpectral.Controls.Add(cbSpectralAudioDevice, 1, 0);
            layoutSpectral.Controls.Add(lblSpectralLowHz, 2, 0);
            layoutSpectral.Controls.Add(numSpectralLowHz, 3, 0);
            layoutSpectral.Controls.Add(lblSpectralHighHz, 4, 0);
            layoutSpectral.Controls.Add(numSpectralHighHz, 5, 0);
            layoutSpectral.Controls.Add(lblSpectralThresholdDb, 6, 0);
            layoutSpectral.Controls.Add(numSpectralThresholdDb, 7, 0);
            pnlSpectral.Controls.Add(layoutSpectral);

            pnlScreen.AutoSize = true;
            pnlScreen.Dock = DockStyle.Fill;
            layoutScreenContainer.AutoSize = true;
            layoutScreenContainer.ColumnCount = 1;
            layoutScreenContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutScreenContainer.Dock = DockStyle.Fill;
            layoutScreenContainer.RowCount = 2;
            layoutScreenContainer.RowStyles.Add(new RowStyle());
            layoutScreenContainer.RowStyles.Add(new RowStyle());
            layoutScreen.AutoSize = true;
            layoutScreen.ColumnCount = 10;
            layoutScreen.Dock = DockStyle.Fill;
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

            lblScreenMonitor.Anchor = AnchorStyles.Left;
            lblScreenMonitor.AutoSize = true;
            lblScreenMonitor.Text = "Monitor";
            cbScreenMonitor.DropDownStyle = ComboBoxStyle.DropDownList;
            cbScreenMonitor.SelectedIndexChanged += TriggerControlChanged;

            btnPickArea.AutoSize = true;
            btnPickArea.Text = "Pick Area";
            btnPickArea.Click += btnPickArea_Click;

            lblScreenX.Anchor = AnchorStyles.Left;
            lblScreenX.AutoSize = true;
            lblScreenX.Text = "X";
            numScreenX.Maximum = 10000;
            numScreenX.Width = 75;
            numScreenX.ValueChanged += TriggerControlChanged;

            lblScreenY.Anchor = AnchorStyles.Left;
            lblScreenY.AutoSize = true;
            lblScreenY.Text = "Y";
            numScreenY.Maximum = 10000;
            numScreenY.Width = 75;
            numScreenY.ValueChanged += TriggerControlChanged;

            lblScreenWidth.Anchor = AnchorStyles.Left;
            lblScreenWidth.AutoSize = true;
            lblScreenWidth.Text = "W";
            numScreenWidth.Minimum = 1;
            numScreenWidth.Maximum = 10000;
            numScreenWidth.Value = 100;
            numScreenWidth.Width = 75;
            numScreenWidth.ValueChanged += TriggerControlChanged;

            lblScreenHeight.Anchor = AnchorStyles.Left;
            lblScreenHeight.AutoSize = true;
            lblScreenHeight.Text = "H";

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

            panelScreenSecondRow.AutoSize = true;
            panelScreenSecondRow.Dock = DockStyle.Fill;
            panelScreenSecondRow.FlowDirection = FlowDirection.LeftToRight;

            numScreenHeight.Minimum = 1;
            numScreenHeight.Maximum = 10000;
            numScreenHeight.Value = 100;
            numScreenHeight.Width = 75;
            numScreenHeight.ValueChanged += TriggerControlChanged;

            lblScreenBrightness.AutoSize = true;
            lblScreenBrightness.Margin = new Padding(12, 6, 0, 0);
            lblScreenBrightness.Text = "Brightness %";

            numScreenBrightnessThreshold.Minimum = 0;
            numScreenBrightnessThreshold.Maximum = 100;
            numScreenBrightnessThreshold.Value = 70;
            numScreenBrightnessThreshold.Width = 75;
            numScreenBrightnessThreshold.ValueChanged += TriggerControlChanged;

            panelScreenSecondRow.Controls.Add(numScreenHeight);
            panelScreenSecondRow.Controls.Add(lblScreenBrightness);
            panelScreenSecondRow.Controls.Add(numScreenBrightnessThreshold);

            layoutScreenContainer.Controls.Add(layoutScreen, 0, 0);
            layoutScreenContainer.Controls.Add(panelScreenSecondRow, 0, 1);
            pnlScreen.Controls.Add(layoutScreenContainer);

            layoutTriggerPanels.Controls.Add(pnlVolume, 0, 0);
            layoutTriggerPanels.Controls.Add(pnlSpectral, 0, 1);
            layoutTriggerPanels.Controls.Add(pnlScreen, 0, 2);

            dgvChannels.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvChannels.Dock = DockStyle.Fill;

            panelActions.AutoSize = true;
            panelActions.Dock = DockStyle.Fill;
            panelActions.FlowDirection = FlowDirection.LeftToRight;

            btnAddRow.AutoSize = true;
            btnAddRow.Text = "Add Row";
            btnAddRow.Click += btnAddRow_Click;

            btnRemoveRow.AutoSize = true;
            btnRemoveRow.Text = "Remove Row";
            btnRemoveRow.Click += btnRemoveRow_Click;

            btnSend.AutoSize = true;
            btnSend.Text = "Send";
            btnSend.Click += btnSend_Click;

            panelActions.Controls.Add(btnAddRow);
            panelActions.Controls.Add(btnRemoveRow);
            panelActions.Controls.Add(btnSend);

            layoutRoot.Controls.Add(lblInfo, 0, 0);
            layoutRoot.Controls.Add(layoutTriggerHeader, 0, 1);
            layoutRoot.Controls.Add(layoutTriggerPanels, 0, 2);
            layoutRoot.Controls.Add(dgvChannels, 0, 3);
            layoutRoot.Controls.Add(panelActions, 0, 4);

            Controls.Add(layoutRoot);

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
            ((System.ComponentModel.ISupportInitialize)dgvChannels).EndInit();
            panelActions.ResumeLayout(false);
            panelActions.PerformLayout();
            ResumeLayout(false);
        }
    }
}
