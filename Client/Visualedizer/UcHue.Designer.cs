namespace Visualedizer
{
    partial class UcHue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UcHue));
            trackBarHue = new TrackBar();
            pictureHue = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)trackBarHue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureHue).BeginInit();
            SuspendLayout();
            // 
            // trackBarHue
            // 
            trackBarHue.Dock = DockStyle.Fill;
            trackBarHue.Location = new Point(0, 0);
            trackBarHue.Maximum = 360;
            trackBarHue.Name = "trackBarHue";
            trackBarHue.Size = new Size(300, 45);
            trackBarHue.TabIndex = 0;
            // 
            // pictureHue
            // 
            pictureHue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureHue.Image = (Image)resources.GetObject("pictureHue.Image");
            pictureHue.Location = new Point(14, 22);
            pictureHue.Name = "pictureHue";
            pictureHue.Size = new Size(273, 15);
            pictureHue.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureHue.TabIndex = 1;
            pictureHue.TabStop = false;
            pictureHue.MouseDown += pictureHue_MouseDown;
            // 
            // UcHue
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pictureHue);
            Controls.Add(trackBarHue);
            Name = "UcHue";
            Size = new Size(300, 45);
            ((System.ComponentModel.ISupportInitialize)trackBarHue).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureHue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar trackBarHue;
        private PictureBox pictureHue;
    }
}
