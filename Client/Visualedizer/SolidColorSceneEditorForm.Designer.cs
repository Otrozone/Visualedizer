namespace Ledqualizer
{
    public partial class SolidColorSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblColor;
        private Visualedizer.UcHue ucHueSolid;
        private Label lblSaturation;
        private TrackBar trackBarSaturation;
        private Label lblBrightness;
        private TrackBar trackBarBrightness;

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
            lblColor = new Label();
            ucHueSolid = new Visualedizer.UcHue();
            lblSaturation = new Label();
            trackBarSaturation = new TrackBar();
            lblBrightness = new Label();
            trackBarBrightness = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)trackBarSaturation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            SuspendLayout();
            // 
            // lblColor
            // 
            lblColor.AutoSize = true;
            lblColor.Location = new Point(12, 12);
            lblColor.Name = "lblColor";
            lblColor.Size = new Size(36, 15);
            lblColor.TabIndex = 0;
            lblColor.Text = "Color";
            // 
            // ucHueSolid
            // 
            ucHueSolid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueSolid.Hue = 0;
            ucHueSolid.Location = new Point(12, 28);
            ucHueSolid.MaxVal = 360;
            ucHueSolid.MinVal = 0;
            ucHueSolid.Name = "ucHueSolid";
            ucHueSolid.Size = new Size(626, 42);
            ucHueSolid.TabIndex = 1;
            // 
            // lblSaturation
            // 
            lblSaturation.AutoSize = true;
            lblSaturation.Location = new Point(12, 76);
            lblSaturation.Name = "lblSaturation";
            lblSaturation.Size = new Size(61, 15);
            lblSaturation.TabIndex = 2;
            lblSaturation.Text = "Saturation";
            // 
            // trackBarSaturation
            // 
            trackBarSaturation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarSaturation.Location = new Point(12, 92);
            trackBarSaturation.Maximum = 100;
            trackBarSaturation.Name = "trackBarSaturation";
            trackBarSaturation.Size = new Size(626, 36);
            trackBarSaturation.TabIndex = 3;
            trackBarSaturation.Value = 100;
            trackBarSaturation.ValueChanged += ControlValueChanged;
            // 
            // lblBrightness
            // 
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(12, 132);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(62, 15);
            lblBrightness.TabIndex = 4;
            lblBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarBrightness.Location = new Point(12, 148);
            trackBarBrightness.Maximum = 100;
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new Size(626, 36);
            trackBarBrightness.TabIndex = 5;
            trackBarBrightness.Value = 50;
            trackBarBrightness.ValueChanged += ControlValueChanged;
            // 
            // SolidColorSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(654, 205);
            Controls.Add(trackBarBrightness);
            Controls.Add(lblBrightness);
            Controls.Add(trackBarSaturation);
            Controls.Add(lblSaturation);
            Controls.Add(ucHueSolid);
            Controls.Add(lblColor);
            Name = "SolidColorSceneEditorForm";
            Text = "Solid Color";
            ((System.ComponentModel.ISupportInitialize)trackBarSaturation).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
