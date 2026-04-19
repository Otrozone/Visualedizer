namespace Ledqualizer
{
    public partial class ScreenRowCaptureSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
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
            // lblScreenRow
            // 
            lblScreenRow.AutoSize = true;
            lblScreenRow.Location = new Point(12, 12);
            lblScreenRow.Name = "lblScreenRow";
            lblScreenRow.Size = new Size(65, 15);
            lblScreenRow.TabIndex = 0;
            lblScreenRow.Text = "Screen row";
            // 
            // numScreenRow
            // 
            numScreenRow.Location = new Point(12, 28);
            numScreenRow.Name = "numScreenRow";
            numScreenRow.ReadOnly = true;
            numScreenRow.Size = new Size(72, 23);
            numScreenRow.TabIndex = 1;
            numScreenRow.ValueChanged += numScreenRow_ValueChanged;
            // 
            // pnlScreenRowSelector
            // 
            pnlScreenRowSelector.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlScreenRowSelector.BorderStyle = BorderStyle.FixedSingle;
            pnlScreenRowSelector.Controls.Add(hsbScreenRowSelector);
            pnlScreenRowSelector.Location = new Point(90, 28);
            pnlScreenRowSelector.Name = "pnlScreenRowSelector";
            pnlScreenRowSelector.Size = new Size(548, 22);
            pnlScreenRowSelector.TabIndex = 2;
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
            chbShowGuide.Location = new Point(12, 62);
            chbShowGuide.Name = "chbShowGuide";
            chbShowGuide.Size = new Size(115, 19);
            chbShowGuide.TabIndex = 3;
            chbShowGuide.Text = "Show guide line";
            chbShowGuide.UseVisualStyleBackColor = true;
            chbShowGuide.CheckedChanged += ControlValueChanged;
            // 
            // chbReverse
            // 
            chbReverse.AutoSize = true;
            chbReverse.Location = new Point(145, 62);
            chbReverse.Name = "chbReverse";
            chbReverse.Size = new Size(105, 19);
            chbReverse.TabIndex = 4;
            chbReverse.Text = "Reverse output";
            chbReverse.UseVisualStyleBackColor = true;
            chbReverse.CheckedChanged += ControlValueChanged;
            // 
            // lblPreview
            // 
            lblPreview.AutoSize = true;
            lblPreview.Location = new Point(12, 92);
            lblPreview.Name = "lblPreview";
            lblPreview.Size = new Size(45, 15);
            lblPreview.TabIndex = 5;
            lblPreview.Text = "Preview";
            // 
            // pictureBoxPreview
            // 
            pictureBoxPreview.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxPreview.Location = new Point(12, 108);
            pictureBoxPreview.Name = "pictureBoxPreview";
            pictureBoxPreview.Size = new Size(626, 24);
            pictureBoxPreview.TabIndex = 6;
            pictureBoxPreview.TabStop = false;
            // 
            // ScreenRowCaptureSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(654, 150);
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
