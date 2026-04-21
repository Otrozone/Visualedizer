namespace Ledqualizer
{
    public partial class SolidColorSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblColor;
        private Visualedizer.UcHueSaturationBrightness ucColor;

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
            ucColor = new Visualedizer.UcHueSaturationBrightness();
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
            // ucColor
            // 
            ucColor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ucColor.Brightness = 100;
            ucColor.Hue = 0;
            ucColor.Location = new Point(12, 28);
            ucColor.MaxHue = 360;
            ucColor.MinHue = 0;
            ucColor.Name = "ucColor";
            ucColor.Saturation = 100;
            ucColor.Size = new Size(626, 84);
            ucColor.TabIndex = 1;
            // 
            // SolidColorSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(654, 129);
            Controls.Add(ucColor);
            Controls.Add(lblColor);
            Name = "SolidColorSceneEditorForm";
            Text = "Solid Color";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
