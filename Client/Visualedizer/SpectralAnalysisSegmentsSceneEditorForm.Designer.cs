namespace Ledqualizer
{
    partial class SpectralAnalysisSegmentsSceneEditorForm
    {
        private System.ComponentModel.IContainer? components = null;
        private TableLayoutPanel layoutRoot = null!;
        private TableLayoutPanel layoutHeader = null!;
        private Label lblAudioDevice = null!;
        private ComboBox cmbAudioDevices = null!;
        private ProgressBar prgAudioLevel = null!;
        private DataGridView dgvSegments = null!;
        private FlowLayoutPanel panelActions = null!;
        private Button btnAddRow = null!;
        private Button btnRemoveRow = null!;
        private Button btnMoveUp = null!;
        private Button btnMoveDown = null!;
        private Button btnResetDefaults = null!;

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
            layoutHeader = new TableLayoutPanel();
            lblAudioDevice = new Label();
            cmbAudioDevices = new ComboBox();
            prgAudioLevel = new ProgressBar();
            dgvSegments = new DataGridView();
            panelActions = new FlowLayoutPanel();
            btnAddRow = new Button();
            btnRemoveRow = new Button();
            btnMoveUp = new Button();
            btnMoveDown = new Button();
            btnResetDefaults = new Button();

            SuspendLayout();
            layoutRoot.SuspendLayout();
            layoutHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSegments).BeginInit();
            panelActions.SuspendLayout();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 620);
            Name = "SpectralAnalysisSegmentsSceneEditorForm";
            Text = "Spectral Analysis Segments";

            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Padding = new Padding(12);
            layoutRoot.RowCount = 3;
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoot.RowStyles.Add(new RowStyle());

            layoutHeader.AutoSize = true;
            layoutHeader.ColumnCount = 4;
            layoutHeader.ColumnStyles.Add(new ColumnStyle());
            layoutHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            layoutHeader.ColumnStyles.Add(new ColumnStyle());
            layoutHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            layoutHeader.Dock = DockStyle.Fill;
            layoutHeader.Margin = new Padding(0, 0, 0, 8);
            layoutHeader.RowCount = 1;
            layoutHeader.RowStyles.Add(new RowStyle());

            lblAudioDevice.Anchor = AnchorStyles.Left;
            lblAudioDevice.AutoSize = true;
            lblAudioDevice.Margin = new Padding(0, 0, 8, 0);
            lblAudioDevice.Name = "lblAudioDevice";
            lblAudioDevice.Text = "Audio device";

            cmbAudioDevices.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbAudioDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAudioDevices.Margin = new Padding(0, 0, 16, 0);
            cmbAudioDevices.Name = "cmbAudioDevices";
            cmbAudioDevices.SelectedIndexChanged += cmbAudioDevices_SelectedIndexChanged;

            prgAudioLevel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            prgAudioLevel.Margin = Padding.Empty;
            prgAudioLevel.Name = "prgAudioLevel";
            prgAudioLevel.Size = new Size(300, 23);

            Label lblLevel = new()
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Margin = new Padding(0, 0, 8, 0),
                Text = "Level"
            };

            layoutHeader.Controls.Add(lblAudioDevice, 0, 0);
            layoutHeader.Controls.Add(cmbAudioDevices, 1, 0);
            layoutHeader.Controls.Add(lblLevel, 2, 0);
            layoutHeader.Controls.Add(prgAudioLevel, 3, 0);

            dgvSegments.Dock = DockStyle.Fill;
            dgvSegments.Margin = Padding.Empty;
            dgvSegments.Name = "dgvSegments";

            panelActions.AutoSize = true;
            panelActions.Dock = DockStyle.Fill;
            panelActions.FlowDirection = FlowDirection.LeftToRight;
            panelActions.Margin = new Padding(0, 8, 0, 0);
            panelActions.WrapContents = false;

            btnAddRow.AutoSize = true;
            btnAddRow.Name = "btnAddRow";
            btnAddRow.Text = "Add";
            btnAddRow.UseVisualStyleBackColor = true;
            btnAddRow.Click += btnAddRow_Click;

            btnRemoveRow.AutoSize = true;
            btnRemoveRow.Name = "btnRemoveRow";
            btnRemoveRow.Text = "Remove";
            btnRemoveRow.UseVisualStyleBackColor = true;
            btnRemoveRow.Click += btnRemoveRow_Click;

            btnMoveUp.AutoSize = true;
            btnMoveUp.Name = "btnMoveUp";
            btnMoveUp.Text = "Move Up";
            btnMoveUp.UseVisualStyleBackColor = true;
            btnMoveUp.Click += btnMoveUp_Click;

            btnMoveDown.AutoSize = true;
            btnMoveDown.Name = "btnMoveDown";
            btnMoveDown.Text = "Move Down";
            btnMoveDown.UseVisualStyleBackColor = true;
            btnMoveDown.Click += btnMoveDown_Click;

            btnResetDefaults.AutoSize = true;
            btnResetDefaults.Name = "btnResetDefaults";
            btnResetDefaults.Text = "Reset Defaults";
            btnResetDefaults.UseVisualStyleBackColor = true;
            btnResetDefaults.Click += btnResetDefaults_Click;

            panelActions.Controls.Add(btnAddRow);
            panelActions.Controls.Add(btnRemoveRow);
            panelActions.Controls.Add(btnMoveUp);
            panelActions.Controls.Add(btnMoveDown);
            panelActions.Controls.Add(btnResetDefaults);

            layoutRoot.Controls.Add(layoutHeader, 0, 0);
            layoutRoot.Controls.Add(dgvSegments, 0, 1);
            layoutRoot.Controls.Add(panelActions, 0, 2);

            Controls.Add(layoutRoot);

            layoutRoot.ResumeLayout(false);
            layoutRoot.PerformLayout();
            layoutHeader.ResumeLayout(false);
            layoutHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSegments).EndInit();
            panelActions.ResumeLayout(false);
            panelActions.PerformLayout();
            ResumeLayout(false);
        }
    }
}
