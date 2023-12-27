namespace Visualedizer
{
    partial class UcBrightness
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UcBrightness));
            pictureBrightness = new PictureBox();
            trackBarBrightness = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)pictureBrightness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).BeginInit();
            SuspendLayout();
            // 
            // pictureBrightness
            // 
            pictureBrightness.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBrightness.Image = (Image)resources.GetObject("pictureBrightness.Image");
            pictureBrightness.Location = new Point(12, 22);
            pictureBrightness.Name = "pictureBrightness";
            pictureBrightness.Size = new Size(693, 15);
            pictureBrightness.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBrightness.TabIndex = 3;
            pictureBrightness.TabStop = false;
            pictureBrightness.MouseDown += pictureBrightness_MouseDown;
            // 
            // trackBarBrightness
            // 
            trackBarBrightness.Dock = DockStyle.Fill;
            trackBarBrightness.Location = new Point(0, 0);
            trackBarBrightness.Maximum = 100;
            trackBarBrightness.Name = "trackBarBrightness";
            trackBarBrightness.Size = new Size(722, 45);
            trackBarBrightness.TabIndex = 2;
            // 
            // UcBrightness
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pictureBrightness);
            Controls.Add(trackBarBrightness);
            Name = "UcBrightness";
            Size = new Size(722, 45);
            ((System.ComponentModel.ISupportInitialize)pictureBrightness).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarBrightness).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBrightness;
        private TrackBar trackBarBrightness;
    }
}
