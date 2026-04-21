namespace Ledqualizer
{
    public partial class GradientSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblGradient;
        private Visualedizer.UcHueRangeSaturationBrightness ucGradient;

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
            ucGradient = new Visualedizer.UcHueRangeSaturationBrightness();
            SuspendLayout();
            // 
            // lblGradient
            // 
            lblGradient.AutoSize = true;
            lblGradient.Location = new Point(12, 12);
            lblGradient.Name = "lblGradient";
            lblGradient.Size = new Size(82, 15);
            lblGradient.TabIndex = 0;
            lblGradient.Text = "Gradient hues";
            // 
            // ucGradient
            // 
            ucGradient.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucGradient.Brightness = 100;
            ucGradient.HueEnd = 360;
            ucGradient.HueStart = 0;
            ucGradient.Location = new Point(12, 28);
            ucGradient.Name = "ucGradient";
            ucGradient.Saturation = 100;
            ucGradient.Size = new Size(626, 84);
            ucGradient.TabIndex = 1;
            // 
            // GradientSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(654, 129);
            Controls.Add(ucGradient);
            Controls.Add(lblGradient);
            Name = "GradientSceneEditorForm";
            Text = "Gradient";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
