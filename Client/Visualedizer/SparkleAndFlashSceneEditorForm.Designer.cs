namespace Ledqualizer
{
    partial class SparkleAndFlashSceneEditorForm
    {
        private System.ComponentModel.IContainer components = null;
        private GroupBox gbSparkles;
        private Label lblSegmentSizeMin;
        private NumericUpDown numSegmentSizeMin;
        private Label lblSegmentSizeMax;
        private NumericUpDown numSegmentSizeMax;
        private Label lblSegmentHoldMs;
        private NumericUpDown numSegmentHoldMs;
        private Label lblSegmentIntervalMinMs;
        private NumericUpDown numSegmentIntervalMinMs;
        private Label lblSegmentIntervalMaxMs;
        private NumericUpDown numSegmentIntervalMaxMs;
        private Label lblMaxActiveSparkles;
        private NumericUpDown numMaxActiveSparkles;
        private GroupBox gbSparkleColor;
        private Label lblSparkleHueMin;
        private NumericUpDown numSparkleHueMin;
        private Label lblSparkleHueMax;
        private NumericUpDown numSparkleHueMax;
        private Label lblSparkleHueChangeIntervalMinMs;
        private NumericUpDown numSparkleHueChangeIntervalMinMs;
        private Label lblSparkleHueChangeIntervalMaxMs;
        private NumericUpDown numSparkleHueChangeIntervalMaxMs;
        private CheckBox chkContinuousSparkleHueChange;
        private GroupBox gbSmoothing;
        private CheckBox chkSmoothFadeAndBlur;
        private Label lblFadeDurationMs;
        private NumericUpDown numFadeDurationMs;
        private Label lblBlurRadius;
        private NumericUpDown numBlurRadius;
        private GroupBox gbFullStrip;
        private CheckBox chkFullStripFlashEnabled;
        private Label lblFullStripHoldMs;
        private NumericUpDown numFullStripFlashHoldMs;
        private CheckBox chkFullStripSmoothFade;
        private Label lblFullStripFadeDurationMs;
        private NumericUpDown numFullStripFadeDurationMs;
        private Label lblFullStripIntervalMinMs;
        private NumericUpDown numFullStripFlashIntervalMinMs;
        private Label lblFullStripIntervalMaxMs;
        private NumericUpDown numFullStripFlashIntervalMaxMs;

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
            gbSparkles = new GroupBox();
            lblSegmentSizeMin = new Label();
            numSegmentSizeMin = new NumericUpDown();
            lblSegmentSizeMax = new Label();
            numSegmentSizeMax = new NumericUpDown();
            lblSegmentHoldMs = new Label();
            numSegmentHoldMs = new NumericUpDown();
            lblSegmentIntervalMinMs = new Label();
            numSegmentIntervalMinMs = new NumericUpDown();
            lblSegmentIntervalMaxMs = new Label();
            numSegmentIntervalMaxMs = new NumericUpDown();
            lblMaxActiveSparkles = new Label();
            numMaxActiveSparkles = new NumericUpDown();
            gbSparkleColor = new GroupBox();
            lblSparkleHueMin = new Label();
            numSparkleHueMin = new NumericUpDown();
            lblSparkleHueMax = new Label();
            numSparkleHueMax = new NumericUpDown();
            lblSparkleHueChangeIntervalMinMs = new Label();
            numSparkleHueChangeIntervalMinMs = new NumericUpDown();
            lblSparkleHueChangeIntervalMaxMs = new Label();
            numSparkleHueChangeIntervalMaxMs = new NumericUpDown();
            chkContinuousSparkleHueChange = new CheckBox();
            gbSmoothing = new GroupBox();
            chkSmoothFadeAndBlur = new CheckBox();
            lblFadeDurationMs = new Label();
            numFadeDurationMs = new NumericUpDown();
            lblBlurRadius = new Label();
            numBlurRadius = new NumericUpDown();
            gbFullStrip = new GroupBox();
            chkFullStripFlashEnabled = new CheckBox();
            lblFullStripHoldMs = new Label();
            numFullStripFlashHoldMs = new NumericUpDown();
            chkFullStripSmoothFade = new CheckBox();
            lblFullStripFadeDurationMs = new Label();
            numFullStripFadeDurationMs = new NumericUpDown();
            lblFullStripIntervalMinMs = new Label();
            numFullStripFlashIntervalMinMs = new NumericUpDown();
            lblFullStripIntervalMaxMs = new Label();
            numFullStripFlashIntervalMaxMs = new NumericUpDown();
            gbSparkles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSegmentSizeMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSegmentSizeMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSegmentHoldMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSegmentIntervalMinMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSegmentIntervalMaxMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxActiveSparkles).BeginInit();
            gbSparkleColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSparkleHueMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSparkleHueMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSparkleHueChangeIntervalMinMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSparkleHueChangeIntervalMaxMs).BeginInit();
            gbSmoothing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numFadeDurationMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBlurRadius).BeginInit();
            gbFullStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numFullStripFlashHoldMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFullStripFadeDurationMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFullStripFlashIntervalMinMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFullStripFlashIntervalMaxMs).BeginInit();
            SuspendLayout();
            // 
            // gbSparkles
            // 
            gbSparkles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbSparkles.Controls.Add(lblSegmentSizeMin);
            gbSparkles.Controls.Add(numSegmentSizeMin);
            gbSparkles.Controls.Add(lblSegmentSizeMax);
            gbSparkles.Controls.Add(numSegmentSizeMax);
            gbSparkles.Controls.Add(lblSegmentHoldMs);
            gbSparkles.Controls.Add(numSegmentHoldMs);
            gbSparkles.Controls.Add(lblSegmentIntervalMinMs);
            gbSparkles.Controls.Add(numSegmentIntervalMinMs);
            gbSparkles.Controls.Add(lblSegmentIntervalMaxMs);
            gbSparkles.Controls.Add(numSegmentIntervalMaxMs);
            gbSparkles.Controls.Add(lblMaxActiveSparkles);
            gbSparkles.Controls.Add(numMaxActiveSparkles);
            gbSparkles.Location = new Point(12, 12);
            gbSparkles.Name = "gbSparkles";
            gbSparkles.Size = new Size(626, 132);
            gbSparkles.TabIndex = 0;
            gbSparkles.TabStop = false;
            gbSparkles.Text = "Segment sparkles";
            // 
            // lblSegmentSizeMin
            // 
            lblSegmentSizeMin.AutoSize = true;
            lblSegmentSizeMin.Location = new Point(16, 28);
            lblSegmentSizeMin.Name = "lblSegmentSizeMin";
            lblSegmentSizeMin.Size = new Size(100, 15);
            lblSegmentSizeMin.TabIndex = 0;
            lblSegmentSizeMin.Text = "Segment min LED";
            // 
            // numSegmentSizeMin
            // 
            numSegmentSizeMin.Location = new Point(16, 46);
            numSegmentSizeMin.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSegmentSizeMin.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSegmentSizeMin.Name = "numSegmentSizeMin";
            numSegmentSizeMin.Size = new Size(90, 23);
            numSegmentSizeMin.TabIndex = 1;
            numSegmentSizeMin.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numSegmentSizeMin.ValueChanged += ControlValueChanged;
            // 
            // lblSegmentSizeMax
            // 
            lblSegmentSizeMax.AutoSize = true;
            lblSegmentSizeMax.Location = new Point(124, 28);
            lblSegmentSizeMax.Name = "lblSegmentSizeMax";
            lblSegmentSizeMax.Size = new Size(102, 15);
            lblSegmentSizeMax.TabIndex = 2;
            lblSegmentSizeMax.Text = "Segment max LED";
            // 
            // numSegmentSizeMax
            // 
            numSegmentSizeMax.Location = new Point(124, 46);
            numSegmentSizeMax.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numSegmentSizeMax.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSegmentSizeMax.Name = "numSegmentSizeMax";
            numSegmentSizeMax.Size = new Size(90, 23);
            numSegmentSizeMax.TabIndex = 3;
            numSegmentSizeMax.Value = new decimal(new int[] { 3, 0, 0, 0 });
            numSegmentSizeMax.ValueChanged += ControlValueChanged;
            // 
            // lblSegmentHoldMs
            // 
            lblSegmentHoldMs.AutoSize = true;
            lblSegmentHoldMs.Location = new Point(232, 28);
            lblSegmentHoldMs.Name = "lblSegmentHoldMs";
            lblSegmentHoldMs.Size = new Size(50, 15);
            lblSegmentHoldMs.TabIndex = 4;
            lblSegmentHoldMs.Text = "Hold ms";
            // 
            // numSegmentHoldMs
            // 
            numSegmentHoldMs.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numSegmentHoldMs.Location = new Point(232, 46);
            numSegmentHoldMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numSegmentHoldMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSegmentHoldMs.Name = "numSegmentHoldMs";
            numSegmentHoldMs.Size = new Size(100, 23);
            numSegmentHoldMs.TabIndex = 5;
            numSegmentHoldMs.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            numSegmentHoldMs.ValueChanged += ControlValueChanged;
            // 
            // lblSegmentIntervalMinMs
            // 
            lblSegmentIntervalMinMs.AutoSize = true;
            lblSegmentIntervalMinMs.Location = new Point(350, 28);
            lblSegmentIntervalMinMs.Name = "lblSegmentIntervalMinMs";
            lblSegmentIntervalMinMs.Size = new Size(86, 15);
            lblSegmentIntervalMinMs.TabIndex = 6;
            lblSegmentIntervalMinMs.Text = "Interval min ms";
            // 
            // numSegmentIntervalMinMs
            // 
            numSegmentIntervalMinMs.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numSegmentIntervalMinMs.Location = new Point(350, 46);
            numSegmentIntervalMinMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numSegmentIntervalMinMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSegmentIntervalMinMs.Name = "numSegmentIntervalMinMs";
            numSegmentIntervalMinMs.Size = new Size(100, 23);
            numSegmentIntervalMinMs.TabIndex = 7;
            numSegmentIntervalMinMs.Value = new decimal(new int[] { 250, 0, 0, 0 });
            numSegmentIntervalMinMs.ValueChanged += ControlValueChanged;
            // 
            // lblSegmentIntervalMaxMs
            // 
            lblSegmentIntervalMaxMs.AutoSize = true;
            lblSegmentIntervalMaxMs.Location = new Point(468, 28);
            lblSegmentIntervalMaxMs.Name = "lblSegmentIntervalMaxMs";
            lblSegmentIntervalMaxMs.Size = new Size(88, 15);
            lblSegmentIntervalMaxMs.TabIndex = 8;
            lblSegmentIntervalMaxMs.Text = "Interval max ms";
            // 
            // numSegmentIntervalMaxMs
            // 
            numSegmentIntervalMaxMs.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numSegmentIntervalMaxMs.Location = new Point(468, 46);
            numSegmentIntervalMaxMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numSegmentIntervalMaxMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSegmentIntervalMaxMs.Name = "numSegmentIntervalMaxMs";
            numSegmentIntervalMaxMs.Size = new Size(100, 23);
            numSegmentIntervalMaxMs.TabIndex = 9;
            numSegmentIntervalMaxMs.Value = new decimal(new int[] { 1500, 0, 0, 0 });
            numSegmentIntervalMaxMs.ValueChanged += ControlValueChanged;
            // 
            // lblMaxActiveSparkles
            // 
            lblMaxActiveSparkles.AutoSize = true;
            lblMaxActiveSparkles.Location = new Point(16, 82);
            lblMaxActiveSparkles.Name = "lblMaxActiveSparkles";
            lblMaxActiveSparkles.Size = new Size(105, 15);
            lblMaxActiveSparkles.TabIndex = 10;
            lblMaxActiveSparkles.Text = "Max active sparkles";
            // 
            // numMaxActiveSparkles
            // 
            numMaxActiveSparkles.Location = new Point(16, 100);
            numMaxActiveSparkles.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numMaxActiveSparkles.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numMaxActiveSparkles.Name = "numMaxActiveSparkles";
            numMaxActiveSparkles.Size = new Size(90, 23);
            numMaxActiveSparkles.TabIndex = 11;
            numMaxActiveSparkles.Value = new decimal(new int[] { 8, 0, 0, 0 });
            numMaxActiveSparkles.ValueChanged += ControlValueChanged;
            // 
            // gbSparkleColor
            // 
            gbSparkleColor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbSparkleColor.Controls.Add(lblSparkleHueMin);
            gbSparkleColor.Controls.Add(numSparkleHueMin);
            gbSparkleColor.Controls.Add(lblSparkleHueMax);
            gbSparkleColor.Controls.Add(numSparkleHueMax);
            gbSparkleColor.Controls.Add(lblSparkleHueChangeIntervalMinMs);
            gbSparkleColor.Controls.Add(numSparkleHueChangeIntervalMinMs);
            gbSparkleColor.Controls.Add(lblSparkleHueChangeIntervalMaxMs);
            gbSparkleColor.Controls.Add(numSparkleHueChangeIntervalMaxMs);
            gbSparkleColor.Controls.Add(chkContinuousSparkleHueChange);
            gbSparkleColor.Location = new Point(12, 154);
            gbSparkleColor.Name = "gbSparkleColor";
            gbSparkleColor.Size = new Size(626, 92);
            gbSparkleColor.TabIndex = 1;
            gbSparkleColor.TabStop = false;
            gbSparkleColor.Text = "Sparkle color";
            // 
            // lblSparkleHueMin
            // 
            lblSparkleHueMin.AutoSize = true;
            lblSparkleHueMin.Location = new Point(16, 28);
            lblSparkleHueMin.Name = "lblSparkleHueMin";
            lblSparkleHueMin.Size = new Size(50, 15);
            lblSparkleHueMin.TabIndex = 0;
            lblSparkleHueMin.Text = "Hue min";
            // 
            // numSparkleHueMin
            // 
            numSparkleHueMin.DecimalPlaces = 1;
            numSparkleHueMin.Location = new Point(16, 46);
            numSparkleHueMin.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numSparkleHueMin.Name = "numSparkleHueMin";
            numSparkleHueMin.Size = new Size(90, 23);
            numSparkleHueMin.TabIndex = 1;
            numSparkleHueMin.ValueChanged += ControlValueChanged;
            // 
            // lblSparkleHueMax
            // 
            lblSparkleHueMax.AutoSize = true;
            lblSparkleHueMax.Location = new Point(124, 28);
            lblSparkleHueMax.Name = "lblSparkleHueMax";
            lblSparkleHueMax.Size = new Size(52, 15);
            lblSparkleHueMax.TabIndex = 2;
            lblSparkleHueMax.Text = "Hue max";
            // 
            // numSparkleHueMax
            // 
            numSparkleHueMax.DecimalPlaces = 1;
            numSparkleHueMax.Location = new Point(124, 46);
            numSparkleHueMax.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numSparkleHueMax.Name = "numSparkleHueMax";
            numSparkleHueMax.Size = new Size(90, 23);
            numSparkleHueMax.TabIndex = 3;
            numSparkleHueMax.Value = new decimal(new int[] { 360, 0, 0, 0 });
            numSparkleHueMax.ValueChanged += ControlValueChanged;
            // 
            // lblSparkleHueChangeIntervalMinMs
            // 
            lblSparkleHueChangeIntervalMinMs.AutoSize = true;
            lblSparkleHueChangeIntervalMinMs.Location = new Point(232, 28);
            lblSparkleHueChangeIntervalMinMs.Name = "lblSparkleHueChangeIntervalMinMs";
            lblSparkleHueChangeIntervalMinMs.Size = new Size(117, 15);
            lblSparkleHueChangeIntervalMinMs.TabIndex = 4;
            lblSparkleHueChangeIntervalMinMs.Text = "Color interval min ms";
            // 
            // numSparkleHueChangeIntervalMinMs
            // 
            numSparkleHueChangeIntervalMinMs.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            numSparkleHueChangeIntervalMinMs.Location = new Point(232, 46);
            numSparkleHueChangeIntervalMinMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numSparkleHueChangeIntervalMinMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSparkleHueChangeIntervalMinMs.Name = "numSparkleHueChangeIntervalMinMs";
            numSparkleHueChangeIntervalMinMs.Size = new Size(112, 23);
            numSparkleHueChangeIntervalMinMs.TabIndex = 5;
            numSparkleHueChangeIntervalMinMs.Value = new decimal(new int[] { 5000, 0, 0, 0 });
            numSparkleHueChangeIntervalMinMs.ValueChanged += ControlValueChanged;
            // 
            // lblSparkleHueChangeIntervalMaxMs
            // 
            lblSparkleHueChangeIntervalMaxMs.AutoSize = true;
            lblSparkleHueChangeIntervalMaxMs.Location = new Point(362, 28);
            lblSparkleHueChangeIntervalMaxMs.Name = "lblSparkleHueChangeIntervalMaxMs";
            lblSparkleHueChangeIntervalMaxMs.Size = new Size(119, 15);
            lblSparkleHueChangeIntervalMaxMs.TabIndex = 6;
            lblSparkleHueChangeIntervalMaxMs.Text = "Color interval max ms";
            // 
            // numSparkleHueChangeIntervalMaxMs
            // 
            numSparkleHueChangeIntervalMaxMs.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            numSparkleHueChangeIntervalMaxMs.Location = new Point(362, 46);
            numSparkleHueChangeIntervalMaxMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numSparkleHueChangeIntervalMaxMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSparkleHueChangeIntervalMaxMs.Name = "numSparkleHueChangeIntervalMaxMs";
            numSparkleHueChangeIntervalMaxMs.Size = new Size(112, 23);
            numSparkleHueChangeIntervalMaxMs.TabIndex = 7;
            numSparkleHueChangeIntervalMaxMs.Value = new decimal(new int[] { 15000, 0, 0, 0 });
            numSparkleHueChangeIntervalMaxMs.ValueChanged += ControlValueChanged;
            // 
            // chkContinuousSparkleHueChange
            // 
            chkContinuousSparkleHueChange.AutoSize = true;
            chkContinuousSparkleHueChange.Location = new Point(494, 48);
            chkContinuousSparkleHueChange.Name = "chkContinuousSparkleHueChange";
            chkContinuousSparkleHueChange.Size = new Size(86, 19);
            chkContinuousSparkleHueChange.TabIndex = 8;
            chkContinuousSparkleHueChange.Text = "Continuous";
            chkContinuousSparkleHueChange.UseVisualStyleBackColor = true;
            chkContinuousSparkleHueChange.CheckedChanged += ControlValueChanged;
            // 
            // gbSmoothing
            // 
            gbSmoothing.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbSmoothing.Controls.Add(chkSmoothFadeAndBlur);
            gbSmoothing.Controls.Add(lblFadeDurationMs);
            gbSmoothing.Controls.Add(numFadeDurationMs);
            gbSmoothing.Controls.Add(lblBlurRadius);
            gbSmoothing.Controls.Add(numBlurRadius);
            gbSmoothing.Location = new Point(12, 256);
            gbSmoothing.Name = "gbSmoothing";
            gbSmoothing.Size = new Size(626, 88);
            gbSmoothing.TabIndex = 2;
            gbSmoothing.TabStop = false;
            gbSmoothing.Text = "Smoothing";
            // 
            // chkSmoothFadeAndBlur
            // 
            chkSmoothFadeAndBlur.AutoSize = true;
            chkSmoothFadeAndBlur.Checked = true;
            chkSmoothFadeAndBlur.CheckState = CheckState.Checked;
            chkSmoothFadeAndBlur.Location = new Point(16, 36);
            chkSmoothFadeAndBlur.Name = "chkSmoothFadeAndBlur";
            chkSmoothFadeAndBlur.Size = new Size(136, 19);
            chkSmoothFadeAndBlur.TabIndex = 0;
            chkSmoothFadeAndBlur.Text = "Smooth fade and blur";
            chkSmoothFadeAndBlur.UseVisualStyleBackColor = true;
            chkSmoothFadeAndBlur.CheckedChanged += ControlValueChanged;
            // 
            // lblFadeDurationMs
            // 
            lblFadeDurationMs.AutoSize = true;
            lblFadeDurationMs.Location = new Point(184, 24);
            lblFadeDurationMs.Name = "lblFadeDurationMs";
            lblFadeDurationMs.Size = new Size(49, 15);
            lblFadeDurationMs.TabIndex = 1;
            lblFadeDurationMs.Text = "Fade ms";
            // 
            // numFadeDurationMs
            // 
            numFadeDurationMs.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numFadeDurationMs.Location = new Point(184, 42);
            numFadeDurationMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numFadeDurationMs.Name = "numFadeDurationMs";
            numFadeDurationMs.Size = new Size(100, 23);
            numFadeDurationMs.TabIndex = 2;
            numFadeDurationMs.Value = new decimal(new int[] { 700, 0, 0, 0 });
            numFadeDurationMs.ValueChanged += ControlValueChanged;
            // 
            // lblBlurRadius
            // 
            lblBlurRadius.AutoSize = true;
            lblBlurRadius.Location = new Point(302, 24);
            lblBlurRadius.Name = "lblBlurRadius";
            lblBlurRadius.Size = new Size(60, 15);
            lblBlurRadius.TabIndex = 3;
            lblBlurRadius.Text = "Blur radius";
            // 
            // numBlurRadius
            // 
            numBlurRadius.Location = new Point(302, 42);
            numBlurRadius.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numBlurRadius.Name = "numBlurRadius";
            numBlurRadius.Size = new Size(90, 23);
            numBlurRadius.TabIndex = 4;
            numBlurRadius.Value = new decimal(new int[] { 2, 0, 0, 0 });
            numBlurRadius.ValueChanged += ControlValueChanged;
            // 
            // gbFullStrip
            // 
            gbFullStrip.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbFullStrip.Controls.Add(chkFullStripFlashEnabled);
            gbFullStrip.Controls.Add(lblFullStripHoldMs);
            gbFullStrip.Controls.Add(numFullStripFlashHoldMs);
            gbFullStrip.Controls.Add(chkFullStripSmoothFade);
            gbFullStrip.Controls.Add(lblFullStripFadeDurationMs);
            gbFullStrip.Controls.Add(numFullStripFadeDurationMs);
            gbFullStrip.Controls.Add(lblFullStripIntervalMinMs);
            gbFullStrip.Controls.Add(numFullStripFlashIntervalMinMs);
            gbFullStrip.Controls.Add(lblFullStripIntervalMaxMs);
            gbFullStrip.Controls.Add(numFullStripFlashIntervalMaxMs);
            gbFullStrip.Location = new Point(12, 354);
            gbFullStrip.Name = "gbFullStrip";
            gbFullStrip.Size = new Size(626, 132);
            gbFullStrip.TabIndex = 3;
            gbFullStrip.TabStop = false;
            gbFullStrip.Text = "Full strip flash";
            // 
            // chkFullStripFlashEnabled
            // 
            chkFullStripFlashEnabled.AutoSize = true;
            chkFullStripFlashEnabled.Checked = true;
            chkFullStripFlashEnabled.CheckState = CheckState.Checked;
            chkFullStripFlashEnabled.Location = new Point(16, 40);
            chkFullStripFlashEnabled.Name = "chkFullStripFlashEnabled";
            chkFullStripFlashEnabled.Size = new Size(68, 19);
            chkFullStripFlashEnabled.TabIndex = 0;
            chkFullStripFlashEnabled.Text = "Enabled";
            chkFullStripFlashEnabled.UseVisualStyleBackColor = true;
            chkFullStripFlashEnabled.CheckedChanged += ControlValueChanged;
            // 
            // lblFullStripHoldMs
            // 
            lblFullStripHoldMs.AutoSize = true;
            lblFullStripHoldMs.Location = new Point(118, 28);
            lblFullStripHoldMs.Name = "lblFullStripHoldMs";
            lblFullStripHoldMs.Size = new Size(50, 15);
            lblFullStripHoldMs.TabIndex = 1;
            lblFullStripHoldMs.Text = "Hold ms";
            // 
            // numFullStripFlashHoldMs
            // 
            numFullStripFlashHoldMs.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numFullStripFlashHoldMs.Location = new Point(118, 46);
            numFullStripFlashHoldMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numFullStripFlashHoldMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numFullStripFlashHoldMs.Name = "numFullStripFlashHoldMs";
            numFullStripFlashHoldMs.Size = new Size(100, 23);
            numFullStripFlashHoldMs.TabIndex = 2;
            numFullStripFlashHoldMs.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            numFullStripFlashHoldMs.ValueChanged += ControlValueChanged;
            // 
            // chkFullStripSmoothFade
            // 
            chkFullStripSmoothFade.AutoSize = true;
            chkFullStripSmoothFade.Checked = true;
            chkFullStripSmoothFade.CheckState = CheckState.Checked;
            chkFullStripSmoothFade.Location = new Point(236, 48);
            chkFullStripSmoothFade.Name = "chkFullStripSmoothFade";
            chkFullStripSmoothFade.Size = new Size(90, 19);
            chkFullStripSmoothFade.TabIndex = 3;
            chkFullStripSmoothFade.Text = "Smooth fade";
            chkFullStripSmoothFade.UseVisualStyleBackColor = true;
            chkFullStripSmoothFade.CheckedChanged += ControlValueChanged;
            // 
            // lblFullStripFadeDurationMs
            // 
            lblFullStripFadeDurationMs.AutoSize = true;
            lblFullStripFadeDurationMs.Location = new Point(350, 28);
            lblFullStripFadeDurationMs.Name = "lblFullStripFadeDurationMs";
            lblFullStripFadeDurationMs.Size = new Size(49, 15);
            lblFullStripFadeDurationMs.TabIndex = 4;
            lblFullStripFadeDurationMs.Text = "Fade ms";
            // 
            // numFullStripFadeDurationMs
            // 
            numFullStripFadeDurationMs.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numFullStripFadeDurationMs.Location = new Point(350, 46);
            numFullStripFadeDurationMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numFullStripFadeDurationMs.Name = "numFullStripFadeDurationMs";
            numFullStripFadeDurationMs.Size = new Size(100, 23);
            numFullStripFadeDurationMs.TabIndex = 5;
            numFullStripFadeDurationMs.Value = new decimal(new int[] { 700, 0, 0, 0 });
            numFullStripFadeDurationMs.ValueChanged += ControlValueChanged;
            // 
            // lblFullStripIntervalMinMs
            // 
            lblFullStripIntervalMinMs.AutoSize = true;
            lblFullStripIntervalMinMs.Location = new Point(16, 82);
            lblFullStripIntervalMinMs.Name = "lblFullStripIntervalMinMs";
            lblFullStripIntervalMinMs.Size = new Size(86, 15);
            lblFullStripIntervalMinMs.TabIndex = 6;
            lblFullStripIntervalMinMs.Text = "Interval min ms";
            // 
            // numFullStripFlashIntervalMinMs
            // 
            numFullStripFlashIntervalMinMs.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            numFullStripFlashIntervalMinMs.Location = new Point(16, 100);
            numFullStripFlashIntervalMinMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numFullStripFlashIntervalMinMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numFullStripFlashIntervalMinMs.Name = "numFullStripFlashIntervalMinMs";
            numFullStripFlashIntervalMinMs.Size = new Size(112, 23);
            numFullStripFlashIntervalMinMs.TabIndex = 7;
            numFullStripFlashIntervalMinMs.Value = new decimal(new int[] { 15000, 0, 0, 0 });
            numFullStripFlashIntervalMinMs.ValueChanged += ControlValueChanged;
            // 
            // lblFullStripIntervalMaxMs
            // 
            lblFullStripIntervalMaxMs.AutoSize = true;
            lblFullStripIntervalMaxMs.Location = new Point(146, 82);
            lblFullStripIntervalMaxMs.Name = "lblFullStripIntervalMaxMs";
            lblFullStripIntervalMaxMs.Size = new Size(88, 15);
            lblFullStripIntervalMaxMs.TabIndex = 8;
            lblFullStripIntervalMaxMs.Text = "Interval max ms";
            // 
            // numFullStripFlashIntervalMaxMs
            // 
            numFullStripFlashIntervalMaxMs.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            numFullStripFlashIntervalMaxMs.Location = new Point(146, 100);
            numFullStripFlashIntervalMaxMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numFullStripFlashIntervalMaxMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numFullStripFlashIntervalMaxMs.Name = "numFullStripFlashIntervalMaxMs";
            numFullStripFlashIntervalMaxMs.Size = new Size(112, 23);
            numFullStripFlashIntervalMaxMs.TabIndex = 9;
            numFullStripFlashIntervalMaxMs.Value = new decimal(new int[] { 45000, 0, 0, 0 });
            numFullStripFlashIntervalMaxMs.ValueChanged += ControlValueChanged;
            // 
            // SparkleAndFlashSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(654, 502);
            Controls.Add(gbFullStrip);
            Controls.Add(gbSmoothing);
            Controls.Add(gbSparkleColor);
            Controls.Add(gbSparkles);
            Name = "SparkleAndFlashSceneEditorForm";
            Text = "Sparkle and Flash";
            gbSparkles.ResumeLayout(false);
            gbSparkles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSegmentSizeMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSegmentSizeMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSegmentHoldMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSegmentIntervalMinMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSegmentIntervalMaxMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxActiveSparkles).EndInit();
            gbSparkleColor.ResumeLayout(false);
            gbSparkleColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSparkleHueMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSparkleHueMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSparkleHueChangeIntervalMinMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSparkleHueChangeIntervalMaxMs).EndInit();
            gbSmoothing.ResumeLayout(false);
            gbSmoothing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numFadeDurationMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBlurRadius).EndInit();
            gbFullStrip.ResumeLayout(false);
            gbFullStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numFullStripFlashHoldMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFullStripFadeDurationMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFullStripFlashIntervalMinMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFullStripFlashIntervalMaxMs).EndInit();
            ResumeLayout(false);
        }
    }
}
