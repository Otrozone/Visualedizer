namespace Ledqualizer
{
    partial class ImageRowCaptureSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblSourceMode;
        private ComboBox cbSourceMode;
        private Label lblImagePath;
        private TextBox txtImagePath;
        private Button btnBrowseImage;
        private Label lblFolderPath;
        private TextBox txtFolderPath;
        private Button btnBrowseFolder;
        private CheckBox chbRecursive;
        private CheckBox chbLoop;
        private Label lblDirection;
        private ComboBox cbDirection;
        private Label lblSpeedMin;
        private NumericUpDown numSpeedMin;
        private Label lblSpeedMax;
        private NumericUpDown numSpeedMax;
        private Button btnPausePlay;
        private Label lblCurrentFile;
        private Label lblCurrentFileValue;
        private Label lblImagePreview;
        private PictureBox pictureBoxImagePreview;
        private Label lblStripPreview;
        private PictureBox pictureBoxStripPreview;

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
            lblSourceMode = new Label();
            cbSourceMode = new ComboBox();
            lblImagePath = new Label();
            txtImagePath = new TextBox();
            btnBrowseImage = new Button();
            lblFolderPath = new Label();
            txtFolderPath = new TextBox();
            btnBrowseFolder = new Button();
            chbRecursive = new CheckBox();
            chbLoop = new CheckBox();
            lblDirection = new Label();
            cbDirection = new ComboBox();
            lblSpeedMin = new Label();
            numSpeedMin = new NumericUpDown();
            lblSpeedMax = new Label();
            numSpeedMax = new NumericUpDown();
            btnPausePlay = new Button();
            lblCurrentFile = new Label();
            lblCurrentFileValue = new Label();
            lblImagePreview = new Label();
            pictureBoxImagePreview = new PictureBox();
            lblStripPreview = new Label();
            pictureBoxStripPreview = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)numSpeedMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSpeedMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImagePreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStripPreview).BeginInit();
            SuspendLayout();
            // 
            // lblSourceMode
            // 
            lblSourceMode.AutoSize = true;
            lblSourceMode.Location = new Point(12, 12);
            lblSourceMode.Name = "lblSourceMode";
            lblSourceMode.Size = new Size(76, 15);
            lblSourceMode.TabIndex = 0;
            lblSourceMode.Text = "Source mode";
            // 
            // cbSourceMode
            // 
            cbSourceMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSourceMode.FormattingEnabled = true;
            cbSourceMode.Location = new Point(12, 28);
            cbSourceMode.Name = "cbSourceMode";
            cbSourceMode.Size = new Size(180, 23);
            cbSourceMode.TabIndex = 1;
            cbSourceMode.SelectedIndexChanged += ControlValueChanged;
            // 
            // lblImagePath
            // 
            lblImagePath.AutoSize = true;
            lblImagePath.Location = new Point(12, 60);
            lblImagePath.Name = "lblImagePath";
            lblImagePath.Size = new Size(63, 15);
            lblImagePath.TabIndex = 2;
            lblImagePath.Text = "Image path";
            // 
            // txtImagePath
            // 
            txtImagePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtImagePath.Location = new Point(12, 76);
            txtImagePath.Name = "txtImagePath";
            txtImagePath.Size = new Size(546, 23);
            txtImagePath.TabIndex = 3;
            txtImagePath.TextChanged += ControlValueChanged;
            // 
            // btnBrowseImage
            // 
            btnBrowseImage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseImage.Location = new Point(564, 75);
            btnBrowseImage.Name = "btnBrowseImage";
            btnBrowseImage.Size = new Size(74, 25);
            btnBrowseImage.TabIndex = 4;
            btnBrowseImage.Text = "Browse...";
            btnBrowseImage.UseVisualStyleBackColor = true;
            btnBrowseImage.Click += btnBrowseImage_Click;
            // 
            // lblFolderPath
            // 
            lblFolderPath.AutoSize = true;
            lblFolderPath.Location = new Point(12, 108);
            lblFolderPath.Name = "lblFolderPath";
            lblFolderPath.Size = new Size(64, 15);
            lblFolderPath.TabIndex = 5;
            lblFolderPath.Text = "Folder path";
            // 
            // txtFolderPath
            // 
            txtFolderPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFolderPath.Location = new Point(12, 124);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.Size = new Size(546, 23);
            txtFolderPath.TabIndex = 6;
            txtFolderPath.TextChanged += ControlValueChanged;
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseFolder.Location = new Point(564, 123);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(74, 25);
            btnBrowseFolder.TabIndex = 7;
            btnBrowseFolder.Text = "Browse...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // chbRecursive
            // 
            chbRecursive.AutoSize = true;
            chbRecursive.Location = new Point(12, 156);
            chbRecursive.Name = "chbRecursive";
            chbRecursive.Size = new Size(133, 19);
            chbRecursive.TabIndex = 8;
            chbRecursive.Text = "Include subfolders";
            chbRecursive.UseVisualStyleBackColor = true;
            chbRecursive.CheckedChanged += ControlValueChanged;
            // 
            // chbLoop
            // 
            chbLoop.AutoSize = true;
            chbLoop.Location = new Point(162, 156);
            chbLoop.Name = "chbLoop";
            chbLoop.Size = new Size(85, 19);
            chbLoop.TabIndex = 9;
            chbLoop.Text = "Loop scan";
            chbLoop.UseVisualStyleBackColor = true;
            chbLoop.CheckedChanged += ControlValueChanged;
            // 
            // lblDirection
            // 
            lblDirection.AutoSize = true;
            lblDirection.Location = new Point(12, 184);
            lblDirection.Name = "lblDirection";
            lblDirection.Size = new Size(55, 15);
            lblDirection.TabIndex = 10;
            lblDirection.Text = "Direction";
            // 
            // cbDirection
            // 
            cbDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDirection.FormattingEnabled = true;
            cbDirection.Location = new Point(12, 200);
            cbDirection.Name = "cbDirection";
            cbDirection.Size = new Size(180, 23);
            cbDirection.TabIndex = 11;
            cbDirection.SelectedIndexChanged += ControlValueChanged;
            // 
            // lblSpeedMin
            // 
            lblSpeedMin.AutoSize = true;
            lblSpeedMin.Location = new Point(214, 184);
            lblSpeedMin.Name = "lblSpeedMin";
            lblSpeedMin.Size = new Size(94, 15);
            lblSpeedMin.TabIndex = 12;
            lblSpeedMin.Text = "Speed min (r/t)";
            // 
            // numSpeedMin
            // 
            numSpeedMin.DecimalPlaces = 2;
            numSpeedMin.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            numSpeedMin.Location = new Point(214, 200);
            numSpeedMin.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            numSpeedMin.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            numSpeedMin.Name = "numSpeedMin";
            numSpeedMin.Size = new Size(100, 23);
            numSpeedMin.TabIndex = 13;
            numSpeedMin.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numSpeedMin.ValueChanged += ControlValueChanged;
            // 
            // lblSpeedMax
            // 
            lblSpeedMax.AutoSize = true;
            lblSpeedMax.Location = new Point(332, 184);
            lblSpeedMax.Name = "lblSpeedMax";
            lblSpeedMax.Size = new Size(97, 15);
            lblSpeedMax.TabIndex = 14;
            lblSpeedMax.Text = "Speed max (r/t)";
            // 
            // numSpeedMax
            // 
            numSpeedMax.DecimalPlaces = 2;
            numSpeedMax.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            numSpeedMax.Location = new Point(332, 200);
            numSpeedMax.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            numSpeedMax.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            numSpeedMax.Name = "numSpeedMax";
            numSpeedMax.Size = new Size(100, 23);
            numSpeedMax.TabIndex = 15;
            numSpeedMax.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numSpeedMax.ValueChanged += ControlValueChanged;
            // 
            // btnPausePlay
            // 
            btnPausePlay.Location = new Point(454, 198);
            btnPausePlay.Name = "btnPausePlay";
            btnPausePlay.Size = new Size(86, 27);
            btnPausePlay.TabIndex = 16;
            btnPausePlay.Text = "Pause";
            btnPausePlay.UseVisualStyleBackColor = true;
            btnPausePlay.Click += btnPausePlay_Click;
            // 
            // lblCurrentFile
            // 
            lblCurrentFile.AutoSize = true;
            lblCurrentFile.Location = new Point(12, 235);
            lblCurrentFile.Name = "lblCurrentFile";
            lblCurrentFile.Size = new Size(65, 15);
            lblCurrentFile.TabIndex = 17;
            lblCurrentFile.Text = "Current file";
            // 
            // lblCurrentFileValue
            // 
            lblCurrentFileValue.AutoEllipsis = true;
            lblCurrentFileValue.Location = new Point(87, 235);
            lblCurrentFileValue.Name = "lblCurrentFileValue";
            lblCurrentFileValue.Size = new Size(551, 15);
            lblCurrentFileValue.TabIndex = 18;
            lblCurrentFileValue.Text = "No file selected";
            // 
            // lblImagePreview
            // 
            lblImagePreview.AutoSize = true;
            lblImagePreview.Location = new Point(12, 262);
            lblImagePreview.Name = "lblImagePreview";
            lblImagePreview.Size = new Size(82, 15);
            lblImagePreview.TabIndex = 19;
            lblImagePreview.Text = "Image preview";
            // 
            // pictureBoxImagePreview
            // 
            pictureBoxImagePreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxImagePreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxImagePreview.Location = new Point(12, 278);
            pictureBoxImagePreview.Name = "pictureBoxImagePreview";
            pictureBoxImagePreview.Size = new Size(626, 120);
            pictureBoxImagePreview.TabIndex = 20;
            pictureBoxImagePreview.TabStop = false;
            // 
            // lblStripPreview
            // 
            lblStripPreview.AutoSize = true;
            lblStripPreview.Location = new Point(12, 408);
            lblStripPreview.Name = "lblStripPreview";
            lblStripPreview.Size = new Size(67, 15);
            lblStripPreview.TabIndex = 21;
            lblStripPreview.Text = "Strip preview";
            // 
            // pictureBoxStripPreview
            // 
            pictureBoxStripPreview.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxStripPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxStripPreview.Location = new Point(12, 424);
            pictureBoxStripPreview.Name = "pictureBoxStripPreview";
            pictureBoxStripPreview.Size = new Size(626, 24);
            pictureBoxStripPreview.TabIndex = 22;
            pictureBoxStripPreview.TabStop = false;
            // 
            // ImageRowCaptureSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(654, 460);
            Controls.Add(pictureBoxStripPreview);
            Controls.Add(lblStripPreview);
            Controls.Add(pictureBoxImagePreview);
            Controls.Add(lblImagePreview);
            Controls.Add(lblCurrentFileValue);
            Controls.Add(lblCurrentFile);
            Controls.Add(btnPausePlay);
            Controls.Add(numSpeedMax);
            Controls.Add(lblSpeedMax);
            Controls.Add(numSpeedMin);
            Controls.Add(lblSpeedMin);
            Controls.Add(cbDirection);
            Controls.Add(lblDirection);
            Controls.Add(chbLoop);
            Controls.Add(chbRecursive);
            Controls.Add(btnBrowseFolder);
            Controls.Add(txtFolderPath);
            Controls.Add(lblFolderPath);
            Controls.Add(btnBrowseImage);
            Controls.Add(txtImagePath);
            Controls.Add(lblImagePath);
            Controls.Add(cbSourceMode);
            Controls.Add(lblSourceMode);
            Name = "ImageRowCaptureSceneEditorForm";
            Text = "Image Row Capture";
            ((System.ComponentModel.ISupportInitialize)numSpeedMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSpeedMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImagePreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStripPreview).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
