namespace Ledqualizer
{
    public partial class ScreenRowCaptureSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblMonitor;
        private ComboBox cbMonitors;
        private Label lblScreenRow;
        private NumericUpDown numScreenRow;
        private Panel pnlScreenRowSelector;
        private HScrollBar hsbScreenRowSelector;
        private CheckBox chbShowGuide;
        private CheckBox chbReverse;
        private Label lblPreview;
        private PictureBox pictureBoxPreview;

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
            lblMonitor = new Label();
            cbMonitors = new ComboBox();
            lblScreenRow = new Label();
            numScreenRow = new NumericUpDown();
            pnlScreenRowSelector = new Panel();
            hsbScreenRowSelector = new HScrollBar();
            chbShowGuide = new CheckBox();
            chbReverse = new CheckBox();
            lblPreview = new Label();
            pictureBoxPreview = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)numScreenRow).BeginInit();
            pnlScreenRowSelector.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).BeginInit();
            SuspendLayout();
            // 
            // lblMonitor
            // 
            lblMonitor.AutoSize = true;
            lblMonitor.Location = new Point(12, 12);
            lblMonitor.Name = "lblMonitor";
            lblMonitor.Size = new Size(46, 15);
            lblMonitor.TabIndex = 0;
            lblMonitor.Text = "Display";
            // 
            // cbMonitors
            // 
            cbMonitors.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbMonitors.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMonitors.FormattingEnabled = true;
            cbMonitors.Location = new Point(12, 28);
            cbMonitors.Name = "cbMonitors";
            cbMonitors.Size = new Size(626, 23);
            cbMonitors.TabIndex = 1;
            cbMonitors.SelectedIndexChanged += cbMonitors_SelectedIndexChanged;
            // 
            // lblScreenRow
            // 
            lblScreenRow.AutoSize = true;
            lblScreenRow.Location = new Point(12, 60);
            lblScreenRow.Name = "lblScreenRow";
            lblScreenRow.Size = new Size(65, 15);
            lblScreenRow.TabIndex = 2;
            lblScreenRow.Text = "Screen row";
            // 
            // numScreenRow
            // 
            numScreenRow.Location = new Point(12, 76);
            numScreenRow.Name = "numScreenRow";
            numScreenRow.ReadOnly = true;
            numScreenRow.Size = new Size(72, 23);
            numScreenRow.TabIndex = 3;
            numScreenRow.ValueChanged += numScreenRow_ValueChanged;
            // 
            // pnlScreenRowSelector
            // 
            pnlScreenRowSelector.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlScreenRowSelector.BorderStyle = BorderStyle.FixedSingle;
            pnlScreenRowSelector.Controls.Add(hsbScreenRowSelector);
            pnlScreenRowSelector.Location = new Point(90, 76);
            pnlScreenRowSelector.Name = "pnlScreenRowSelector";
            pnlScreenRowSelector.Size = new Size(548, 22);
            pnlScreenRowSelector.TabIndex = 4;
            // 
            // hsbScreenRowSelector
            // 
            hsbScreenRowSelector.Dock = DockStyle.Fill;
            hsbScreenRowSelector.Location = new Point(0, 0);
            hsbScreenRowSelector.Name = "hsbScreenRowSelector";
            hsbScreenRowSelector.Size = new Size(540, 22);
            hsbScreenRowSelector.TabIndex = 0;
            hsbScreenRowSelector.Scroll += hsbScreenRowSelector_Scroll;
            // 
            // chbShowGuide
            // 
            chbShowGuide.AutoSize = true;
            chbShowGuide.Location = new Point(12, 110);
            chbShowGuide.Name = "chbShowGuide";
            chbShowGuide.Size = new Size(115, 19);
            chbShowGuide.TabIndex = 5;
            chbShowGuide.Text = "Show guide line";
            chbShowGuide.UseVisualStyleBackColor = true;
            chbShowGuide.CheckedChanged += ControlValueChanged;
            // 
            // chbReverse
            // 
            chbReverse.AutoSize = true;
            chbReverse.Location = new Point(145, 110);
            chbReverse.Name = "chbReverse";
            chbReverse.Size = new Size(105, 19);
            chbReverse.TabIndex = 6;
            chbReverse.Text = "Reverse output";
            chbReverse.UseVisualStyleBackColor = true;
            chbReverse.CheckedChanged += ControlValueChanged;
            // 
            // lblPreview
            // 
            lblPreview.AutoSize = true;
            lblPreview.Location = new Point(12, 140);
            lblPreview.Name = "lblPreview";
            lblPreview.Size = new Size(45, 15);
            lblPreview.TabIndex = 7;
            lblPreview.Text = "Preview";
            // 
            // pictureBoxPreview
            // 
            pictureBoxPreview.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxPreview.Location = new Point(12, 156);
            pictureBoxPreview.Name = "pictureBoxPreview";
            pictureBoxPreview.Size = new Size(626, 24);
            pictureBoxPreview.TabIndex = 8;
            pictureBoxPreview.TabStop = false;
            // 
            // ScreenRowCaptureSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(654, 193);
            Controls.Add(cbMonitors);
            Controls.Add(lblMonitor);
            Controls.Add(pictureBoxPreview);
            Controls.Add(lblPreview);
            Controls.Add(chbReverse);
            Controls.Add(chbShowGuide);
            Controls.Add(pnlScreenRowSelector);
            Controls.Add(numScreenRow);
            Controls.Add(lblScreenRow);
            Name = "ScreenRowCaptureSceneEditorForm";
            Text = "Screen Row Capture";
            ((System.ComponentModel.ISupportInitialize)numScreenRow).EndInit();
            pnlScreenRowSelector.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
