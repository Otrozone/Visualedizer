namespace Visualedizer
{
    partial class UcHueMinMax
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UcHueMinMax));
            pictureHue = new PictureBox();
            trackBarHueMax = new TrackBar();
            trackBarHueMin = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)pictureHue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHueMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHueMin).BeginInit();
            SuspendLayout();
            // 
            // pictureHue
            // 
            pictureHue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureHue.Image = (Image)resources.GetObject("pictureHue.Image");
            pictureHue.Location = new Point(12, 22);
            pictureHue.Name = "pictureHue";
            pictureHue.Size = new Size(274, 15);
            pictureHue.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureHue.TabIndex = 3;
            pictureHue.TabStop = false;
            pictureHue.MouseDown += pictureHue_MouseDown;
            // 
            // trackBarHueMax
            // 
            trackBarHueMax.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarHueMax.Location = new Point(0, 0);
            trackBarHueMax.Maximum = 360;
            trackBarHueMax.Name = "trackBarHueMax";
            trackBarHueMax.Size = new Size(300, 45);
            trackBarHueMax.TabIndex = 2;
            trackBarHueMax.Value = 360;
            trackBarHueMax.ValueChanged += trackBarHueMax_ValueChanged;
            // 
            // trackBarHueMin
            // 
            trackBarHueMin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarHueMin.Location = new Point(0, 35);
            trackBarHueMin.Maximum = 360;
            trackBarHueMin.Name = "trackBarHueMin";
            trackBarHueMin.Size = new Size(300, 45);
            trackBarHueMin.TabIndex = 4;
            trackBarHueMin.ValueChanged += trackBarHueMin_ValueChanged;
            // 
            // UcHueMinMax
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pictureHue);
            Controls.Add(trackBarHueMin);
            Controls.Add(trackBarHueMax);
            Name = "UcHueMinMax";
            Size = new Size(300, 60);
            ((System.ComponentModel.ISupportInitialize)pictureHue).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHueMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHueMin).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureHue;
        private TrackBar trackBarHueMax;
        private TrackBar trackBarHueMin;
    }
}
