namespace Ledqualizer
{
    partial class GradientSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblGradient;
        private Visualedizer.UcHueMinMax ucHueMinMaxGradient;
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
            lblGradient = new Label();
            ucHueMinMaxGradient = new Visualedizer.UcHueMinMax();
            lblSaturation = new Label();
            trackBarSaturation = new TrackBar();
            lblBrightness = new Label();
            trackBarBrightness = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)trackBarSaturation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            SuspendLayout();
            // 
            // lblGradient
            // 
            lblGradient.AutoSize = true;
            lblGradient.Location = new Point(16, 18);
            lblGradient.Name = "lblGradient";
            lblGradient.Size = new Size(82, 15);
            lblGradient.TabIndex = 0;
            lblGradient.Text = "Gradient hues";
            // 
            // ucHueMinMaxGradient
            // 
            ucHueMinMaxGradient.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucHueMinMaxGradient.HueMax = 360;
            ucHueMinMaxGradient.HueMin = 0;
            ucHueMinMaxGradient.Location = new Point(16, 36);
            ucHueMinMaxGradient.Name = "ucHueMinMaxGradient";
            ucHueMinMaxGradient.Size = new Size(620, 59);
            ucHueMinMaxGradient.TabIndex = 1;
            // 
            // lblSaturation
            // 
            lblSaturation.AutoSize = true;
            lblSaturation.Location = new Point(16, 116);
            lblSaturation.Name = "lblSaturation";
            lblSaturation.Size = new Size(61, 15);
            lblSaturation.TabIndex = 2;
            lblSaturation.Text = "Saturation";
            // 
            // trackBarSaturation
            // 
            trackBarSaturation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarSaturation.Location = new Point(16, 134);
            trackBarSaturation.Maximum = 100;
            trackBarSaturation.Name = "trackBarSaturation";
            trackBarSaturation.Size = new Size(620, 45);
            trackBarSaturation.TabIndex = 3;
            trackBarSaturation.Value = 100;
            trackBarSaturation.ValueChanged += ControlValueChanged;
            // 
            // lblBrightness
            // 
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(16, 197);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(62, 15);
            lblBrightness.TabIndex = 4;
            lblBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarBrightness.Location = new Point(16, 215);
            trackBarBrightness.Maximum = 100;
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new Size(620, 45);
            trackBarBrightness.TabIndex = 5;
            trackBarBrightness.Value = 50;
            trackBarBrightness.ValueChanged += ControlValueChanged;
            // 
            // GradientSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(654, 419);
            Controls.Add(trackBarBrightness);
            Controls.Add(lblBrightness);
            Controls.Add(trackBarSaturation);
            Controls.Add(lblSaturation);
            Controls.Add(ucHueMinMaxGradient);
            Controls.Add(lblGradient);
            Name = "GradientSceneEditorForm";
            Text = "Gradient";
            ((System.ComponentModel.ISupportInitialize)trackBarSaturation).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
