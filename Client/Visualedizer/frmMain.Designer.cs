namespace Ledqualizer
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;
        private SplitContainer splitContainerRoot;
        private Label lblDelay;
        private NumericUpDown numDelay;
        private Label lblRefreshRate;
        private Label lblDevices;
        private Button btnAddDevice;
        private Button btnRemoveDevice;
        private Button btnOtherDevices;
        private DataGridView dgvDevices;
        private DataGridViewCheckBoxColumn colEnabled;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewComboBoxColumn colAssignedScene;
        private DataGridViewTextBoxColumn colHost;
        private DataGridViewTextBoxColumn colPort;
        private DataGridViewTextBoxColumn colStripCount;
        private DataGridViewTextBoxColumn colLedCount;
        private DataGridViewTextBoxColumn colStatus;
        private SplitContainer splitContainerMain;
        private Label lblScenes;
        private Button btnAddScene;
        private Button btnDuplicateScene;
        private Button btnRemoveScene;
        private DataGridView dgvScenes;
        private DataGridViewTextBoxColumn colSceneName;
        private DataGridViewComboBoxColumn colSceneType;
        private DataGridViewTextBoxColumn colSceneSummary;
        private Panel panelSceneEditorHost;
        private Label lblEditorTitle;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statLblConnection;
        private ToolStripStatusLabel statLblRate;

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
            splitContainerRoot = new SplitContainer();
            dgvDevices = new DataGridView();
            colEnabled = new DataGridViewCheckBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            colAssignedScene = new DataGridViewComboBoxColumn();
            colHost = new DataGridViewTextBoxColumn();
            colPort = new DataGridViewTextBoxColumn();
            colStripCount = new DataGridViewTextBoxColumn();
            colLedCount = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            btnOtherDevices = new Button();
            btnRemoveDevice = new Button();
            btnAddDevice = new Button();
            lblDevices = new Label();
            lblRefreshRate = new Label();
            lblDelay = new Label();
            numDelay = new NumericUpDown();
            splitContainerMain = new SplitContainer();
            dgvScenes = new DataGridView();
            colSceneName = new DataGridViewTextBoxColumn();
            colSceneType = new DataGridViewComboBoxColumn();
            colSceneSummary = new DataGridViewTextBoxColumn();
            btnRemoveScene = new Button();
            btnDuplicateScene = new Button();
            btnAddScene = new Button();
            lblScenes = new Label();
            panelSceneEditorHost = new Panel();
            lblEditorTitle = new Label();
            statusStrip = new StatusStrip();
            statLblConnection = new ToolStripStatusLabel();
            statLblRate = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainerRoot).BeginInit();
            splitContainerRoot.Panel1.SuspendLayout();
            splitContainerRoot.Panel2.SuspendLayout();
            splitContainerRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDevices).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvScenes).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerRoot
            // 
            splitContainerRoot.BackColor = SystemColors.ControlDark;
            splitContainerRoot.Dock = DockStyle.Fill;
            splitContainerRoot.FixedPanel = FixedPanel.Panel1;
            splitContainerRoot.Location = new Point(0, 0);
            splitContainerRoot.Name = "splitContainerRoot";
            splitContainerRoot.Orientation = Orientation.Horizontal;
            // 
            // splitContainerRoot.Panel1
            // 
            splitContainerRoot.Panel1.BackColor = SystemColors.Control;
            splitContainerRoot.Panel1.Controls.Add(dgvDevices);
            splitContainerRoot.Panel1.Controls.Add(btnOtherDevices);
            splitContainerRoot.Panel1.Controls.Add(btnRemoveDevice);
            splitContainerRoot.Panel1.Controls.Add(btnAddDevice);
            splitContainerRoot.Panel1.Controls.Add(lblDevices);
            splitContainerRoot.Panel1.Controls.Add(lblRefreshRate);
            splitContainerRoot.Panel1.Controls.Add(lblDelay);
            splitContainerRoot.Panel1.Controls.Add(numDelay);
            // 
            // splitContainerRoot.Panel2
            // 
            splitContainerRoot.Panel2.BackColor = SystemColors.Control;
            splitContainerRoot.Panel2.Controls.Add(splitContainerMain);
            splitContainerRoot.Size = new Size(1103, 711);
            splitContainerRoot.SplitterDistance = 218;
            splitContainerRoot.SplitterWidth = 3;
            splitContainerRoot.TabIndex = 0;
            // 
            // dgvDevices
            // 
            dgvDevices.AllowUserToAddRows = false;
            dgvDevices.AllowUserToDeleteRows = false;
            dgvDevices.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvDevices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDevices.Columns.AddRange(new DataGridViewColumn[] { colEnabled, colName, colAssignedScene, colHost, colPort, colStripCount, colLedCount, colStatus });
            dgvDevices.Location = new Point(12, 59);
            dgvDevices.Name = "dgvDevices";
            dgvDevices.RowHeadersVisible = false;
            dgvDevices.Size = new Size(1079, 152);
            dgvDevices.TabIndex = 7;
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
            colName.FillWeight = 18F;
            colName.HeaderText = "Name";
            colName.Name = "colName";
            colName.ReadOnly = true;
            // 
            // colAssignedScene
            // 
            colAssignedScene.DataPropertyName = "AssignedSceneId";
            colAssignedScene.HeaderText = "Scene";
            colAssignedScene.Name = "colAssignedScene";
            colAssignedScene.Width = 180;
            // 
            // colHost
            // 
            colHost.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colHost.DataPropertyName = "Host";
            colHost.FillWeight = 22F;
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
            // colStripCount
            // 
            colStripCount.DataPropertyName = "StripCount";
            colStripCount.HeaderText = "Strips";
            colStripCount.Name = "colStripCount";
            colStripCount.ReadOnly = true;
            colStripCount.Width = 60;
            // 
            // colLedCount
            // 
            colLedCount.DataPropertyName = "LedCount";
            colLedCount.HeaderText = "LEDs";
            colLedCount.Name = "colLedCount";
            colLedCount.ReadOnly = true;
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
            // btnOtherDevices
            // 
            btnOtherDevices.Location = new Point(220, 30);
            btnOtherDevices.Name = "btnOtherDevices";
            btnOtherDevices.Size = new Size(96, 23);
            btnOtherDevices.TabIndex = 6;
            btnOtherDevices.Text = "Other devices";
            btnOtherDevices.UseVisualStyleBackColor = true;
            btnOtherDevices.Click += btnOtherDevices_Click;
            // 
            // btnRemoveDevice
            // 
            btnRemoveDevice.Location = new Point(106, 30);
            btnRemoveDevice.Name = "btnRemoveDevice";
            btnRemoveDevice.Size = new Size(108, 23);
            btnRemoveDevice.TabIndex = 5;
            btnRemoveDevice.Text = "Remove selected";
            btnRemoveDevice.UseVisualStyleBackColor = true;
            btnRemoveDevice.Click += btnRemoveDevice_Click;
            // 
            // btnAddDevice
            // 
            btnAddDevice.Location = new Point(12, 30);
            btnAddDevice.Name = "btnAddDevice";
            btnAddDevice.Size = new Size(88, 23);
            btnAddDevice.TabIndex = 4;
            btnAddDevice.Text = "Add device";
            btnAddDevice.UseVisualStyleBackColor = true;
            btnAddDevice.Click += btnAddDevice_Click;
            // 
            // lblDevices
            // 
            lblDevices.AutoSize = true;
            lblDevices.Location = new Point(12, 9);
            lblDevices.Name = "lblDevices";
            lblDevices.Size = new Size(47, 15);
            lblDevices.TabIndex = 3;
            lblDevices.Text = "Devices";
            // 
            // lblRefreshRate
            // 
            lblRefreshRate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblRefreshRate.AutoSize = true;
            lblRefreshRate.Location = new Point(1042, 36);
            lblRefreshRate.Name = "lblRefreshRate";
            lblRefreshRate.Size = new Size(38, 15);
            lblRefreshRate.TabIndex = 2;
            lblRefreshRate.Text = "(5 Hz)";
            // 
            // lblDelay
            // 
            lblDelay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDelay.AutoSize = true;
            lblDelay.Location = new Point(894, 34);
            lblDelay.Name = "lblDelay";
            lblDelay.Size = new Size(63, 15);
            lblDelay.TabIndex = 1;
            lblDelay.Text = "Delay (ms)";
            // 
            // numDelay
            // 
            numDelay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numDelay.Location = new Point(974, 32);
            numDelay.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numDelay.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDelay.Name = "numDelay";
            numDelay.Size = new Size(60, 23);
            numDelay.TabIndex = 0;
            numDelay.Value = new decimal(new int[] { 20, 0, 0, 0 });
            numDelay.ValueChanged += numDelay_ValueChanged;
            // 
            // splitContainerMain
            // 
            splitContainerMain.BackColor = SystemColors.ControlDark;
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(0, 0);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.BackColor = SystemColors.Control;
            splitContainerMain.Panel1.Controls.Add(dgvScenes);
            splitContainerMain.Panel1.Controls.Add(btnRemoveScene);
            splitContainerMain.Panel1.Controls.Add(btnDuplicateScene);
            splitContainerMain.Panel1.Controls.Add(btnAddScene);
            splitContainerMain.Panel1.Controls.Add(lblScenes);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.BackColor = SystemColors.Control;
            splitContainerMain.Panel2.Controls.Add(panelSceneEditorHost);
            splitContainerMain.Panel2.Controls.Add(lblEditorTitle);
            splitContainerMain.Size = new Size(1103, 490);
            splitContainerMain.SplitterDistance = 392;
            splitContainerMain.SplitterWidth = 3;
            splitContainerMain.TabIndex = 1;
            // 
            // dgvScenes
            // 
            dgvScenes.AllowUserToAddRows = false;
            dgvScenes.AllowUserToDeleteRows = false;
            dgvScenes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvScenes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvScenes.Columns.AddRange(new DataGridViewColumn[] { colSceneName, colSceneType, colSceneSummary });
            dgvScenes.Location = new Point(12, 65);
            dgvScenes.Name = "dgvScenes";
            dgvScenes.RowHeadersVisible = false;
            dgvScenes.Size = new Size(367, 413);
            dgvScenes.TabIndex = 4;
            // 
            // colSceneName
            // 
            colSceneName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colSceneName.DataPropertyName = "Name";
            colSceneName.FillWeight = 25F;
            colSceneName.HeaderText = "Name";
            colSceneName.Name = "colSceneName";
            // 
            // colSceneType
            // 
            colSceneType.DataPropertyName = "Type";
            colSceneType.HeaderText = "Type";
            colSceneType.Name = "colSceneType";
            colSceneType.Width = 130;
            // 
            // colSceneSummary
            // 
            colSceneSummary.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colSceneSummary.DataPropertyName = "Summary";
            colSceneSummary.FillWeight = 40F;
            colSceneSummary.HeaderText = "Summary";
            colSceneSummary.Name = "colSceneSummary";
            colSceneSummary.ReadOnly = true;
            // 
            // btnRemoveScene
            // 
            btnRemoveScene.Location = new Point(219, 36);
            btnRemoveScene.Name = "btnRemoveScene";
            btnRemoveScene.Size = new Size(96, 23);
            btnRemoveScene.TabIndex = 3;
            btnRemoveScene.Text = "Remove";
            btnRemoveScene.UseVisualStyleBackColor = true;
            btnRemoveScene.Click += btnRemoveScene_Click;
            // 
            // btnDuplicateScene
            // 
            btnDuplicateScene.Location = new Point(117, 36);
            btnDuplicateScene.Name = "btnDuplicateScene";
            btnDuplicateScene.Size = new Size(96, 23);
            btnDuplicateScene.TabIndex = 2;
            btnDuplicateScene.Text = "Duplicate";
            btnDuplicateScene.UseVisualStyleBackColor = true;
            btnDuplicateScene.Click += btnDuplicateScene_Click;
            // 
            // btnAddScene
            // 
            btnAddScene.Location = new Point(12, 36);
            btnAddScene.Name = "btnAddScene";
            btnAddScene.Size = new Size(99, 23);
            btnAddScene.TabIndex = 1;
            btnAddScene.Text = "Add scene";
            btnAddScene.UseVisualStyleBackColor = true;
            btnAddScene.Click += btnAddScene_Click;
            // 
            // lblScenes
            // 
            lblScenes.AutoSize = true;
            lblScenes.Location = new Point(12, 15);
            lblScenes.Name = "lblScenes";
            lblScenes.Size = new Size(43, 15);
            lblScenes.TabIndex = 0;
            lblScenes.Text = "Scenes";
            // 
            // panelSceneEditorHost
            // 
            panelSceneEditorHost.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelSceneEditorHost.BorderStyle = BorderStyle.FixedSingle;
            panelSceneEditorHost.Location = new Point(13, 36);
            panelSceneEditorHost.Name = "panelSceneEditorHost";
            panelSceneEditorHost.Size = new Size(685, 442);
            panelSceneEditorHost.TabIndex = 1;
            // 
            // lblEditorTitle
            // 
            lblEditorTitle.AutoSize = true;
            lblEditorTitle.Location = new Point(13, 15);
            lblEditorTitle.Name = "lblEditorTitle";
            lblEditorTitle.Size = new Size(83, 15);
            lblEditorTitle.TabIndex = 0;
            lblEditorTitle.Text = "Scene Settings";
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statLblConnection, statLblRate });
            statusStrip.Location = new Point(0, 711);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1103, 22);
            statusStrip.TabIndex = 2;
            // 
            // statLblConnection
            // 
            statLblConnection.Name = "statLblConnection";
            statLblConnection.Size = new Size(79, 17);
            statLblConnection.Text = "Disconnected";
            // 
            // statLblRate
            // 
            statLblRate.Margin = new Padding(12, 3, 0, 2);
            statLblRate.Name = "statLblRate";
            statLblRate.Size = new Size(41, 17);
            statLblRate.Text = "Rate: -";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1103, 733);
            Controls.Add(splitContainerRoot);
            Controls.Add(statusStrip);
            Name = "FrmMain";
            Text = "Visualedizer";
            FormClosing += FrmMain_FormClosing;
            Load += frmMain_Load;
            splitContainerRoot.Panel1.ResumeLayout(false);
            splitContainerRoot.Panel1.PerformLayout();
            splitContainerRoot.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerRoot).EndInit();
            splitContainerRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvDevices).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDelay).EndInit();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel1.PerformLayout();
            splitContainerMain.Panel2.ResumeLayout(false);
            splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvScenes).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
