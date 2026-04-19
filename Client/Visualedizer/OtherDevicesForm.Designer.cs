namespace Ledqualizer
{
    partial class OtherDevicesForm
    {
        private System.ComponentModel.IContainer components = null;
        private GroupBox gbStrobe;
        private NumericUpDown numStrobeY;
        private NumericUpDown numStrobeX;
        private Label lblStrobeTrigger;
        private GroupBox gbLaser;
        private NumericUpDown numLaserColorY;
        private NumericUpDown numLaserColorX;
        private Label lblLaserColor;
        private NumericUpDown numLaserPatternY;
        private NumericUpDown numLaserPatternX;
        private Label lblLaserPattern;
        private NumericUpDown numLaserTriggerY;
        private NumericUpDown numLaserTriggerX;
        private Label lblLaserTrigger;

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
            gbStrobe = new GroupBox();
            numStrobeY = new NumericUpDown();
            numStrobeX = new NumericUpDown();
            lblStrobeTrigger = new Label();
            gbLaser = new GroupBox();
            numLaserColorY = new NumericUpDown();
            numLaserColorX = new NumericUpDown();
            lblLaserColor = new Label();
            numLaserPatternY = new NumericUpDown();
            numLaserPatternX = new NumericUpDown();
            lblLaserPattern = new Label();
            numLaserTriggerY = new NumericUpDown();
            numLaserTriggerX = new NumericUpDown();
            lblLaserTrigger = new Label();
            gbStrobe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numStrobeY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStrobeX).BeginInit();
            gbLaser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numLaserColorY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserColorX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserPatternY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserPatternX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserTriggerY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLaserTriggerX).BeginInit();
            SuspendLayout();
            // 
            // gbStrobe
            // 
            gbStrobe.Controls.Add(numStrobeY);
            gbStrobe.Controls.Add(numStrobeX);
            gbStrobe.Controls.Add(lblStrobeTrigger);
            gbStrobe.Location = new Point(12, 12);
            gbStrobe.Name = "gbStrobe";
            gbStrobe.Size = new Size(236, 84);
            gbStrobe.TabIndex = 0;
            gbStrobe.TabStop = false;
            gbStrobe.Text = "Strobe";
            // 
            // numStrobeY
            // 
            numStrobeY.Location = new Point(161, 35);
            numStrobeY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numStrobeY.Name = "numStrobeY";
            numStrobeY.Size = new Size(54, 23);
            numStrobeY.TabIndex = 2;
            numStrobeY.ValueChanged += ControlValueChanged;
            // 
            // numStrobeX
            // 
            numStrobeX.Location = new Point(101, 35);
            numStrobeX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numStrobeX.Name = "numStrobeX";
            numStrobeX.Size = new Size(54, 23);
            numStrobeX.TabIndex = 1;
            numStrobeX.ValueChanged += ControlValueChanged;
            // 
            // lblStrobeTrigger
            // 
            lblStrobeTrigger.AutoSize = true;
            lblStrobeTrigger.Location = new Point(20, 37);
            lblStrobeTrigger.Name = "lblStrobeTrigger";
            lblStrobeTrigger.Size = new Size(72, 15);
            lblStrobeTrigger.TabIndex = 0;
            lblStrobeTrigger.Text = "Trigger [x, y]";
            // 
            // gbLaser
            // 
            gbLaser.Controls.Add(numLaserColorY);
            gbLaser.Controls.Add(numLaserColorX);
            gbLaser.Controls.Add(lblLaserColor);
            gbLaser.Controls.Add(numLaserPatternY);
            gbLaser.Controls.Add(numLaserPatternX);
            gbLaser.Controls.Add(lblLaserPattern);
            gbLaser.Controls.Add(numLaserTriggerY);
            gbLaser.Controls.Add(numLaserTriggerX);
            gbLaser.Controls.Add(lblLaserTrigger);
            gbLaser.Location = new Point(12, 102);
            gbLaser.Name = "gbLaser";
            gbLaser.Size = new Size(236, 135);
            gbLaser.TabIndex = 1;
            gbLaser.TabStop = false;
            gbLaser.Text = "Laser";
            // 
            // numLaserColorY
            // 
            numLaserColorY.Location = new Point(159, 91);
            numLaserColorY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserColorY.Name = "numLaserColorY";
            numLaserColorY.Size = new Size(54, 23);
            numLaserColorY.TabIndex = 8;
            numLaserColorY.ValueChanged += ControlValueChanged;
            // 
            // numLaserColorX
            // 
            numLaserColorX.Location = new Point(99, 91);
            numLaserColorX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserColorX.Name = "numLaserColorX";
            numLaserColorX.Size = new Size(54, 23);
            numLaserColorX.TabIndex = 7;
            numLaserColorX.ValueChanged += ControlValueChanged;
            // 
            // lblLaserColor
            // 
            lblLaserColor.AutoSize = true;
            lblLaserColor.Location = new Point(17, 93);
            lblLaserColor.Name = "lblLaserColor";
            lblLaserColor.Size = new Size(64, 15);
            lblLaserColor.TabIndex = 6;
            lblLaserColor.Text = "Color [x, y]";
            // 
            // numLaserPatternY
            // 
            numLaserPatternY.Location = new Point(159, 62);
            numLaserPatternY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserPatternY.Name = "numLaserPatternY";
            numLaserPatternY.Size = new Size(54, 23);
            numLaserPatternY.TabIndex = 5;
            numLaserPatternY.ValueChanged += ControlValueChanged;
            // 
            // numLaserPatternX
            // 
            numLaserPatternX.Location = new Point(99, 62);
            numLaserPatternX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserPatternX.Name = "numLaserPatternX";
            numLaserPatternX.Size = new Size(54, 23);
            numLaserPatternX.TabIndex = 4;
            numLaserPatternX.ValueChanged += ControlValueChanged;
            // 
            // lblLaserPattern
            // 
            lblLaserPattern.AutoSize = true;
            lblLaserPattern.Location = new Point(17, 64);
            lblLaserPattern.Name = "lblLaserPattern";
            lblLaserPattern.Size = new Size(73, 15);
            lblLaserPattern.TabIndex = 3;
            lblLaserPattern.Text = "Pattern [x, y]";
            // 
            // numLaserTriggerY
            // 
            numLaserTriggerY.Location = new Point(159, 33);
            numLaserTriggerY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserTriggerY.Name = "numLaserTriggerY";
            numLaserTriggerY.Size = new Size(54, 23);
            numLaserTriggerY.TabIndex = 2;
            numLaserTriggerY.ValueChanged += ControlValueChanged;
            // 
            // numLaserTriggerX
            // 
            numLaserTriggerX.Location = new Point(99, 33);
            numLaserTriggerX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numLaserTriggerX.Name = "numLaserTriggerX";
            numLaserTriggerX.Size = new Size(54, 23);
            numLaserTriggerX.TabIndex = 1;
            numLaserTriggerX.ValueChanged += ControlValueChanged;
            // 
            // lblLaserTrigger
            // 
            lblLaserTrigger.AutoSize = true;
            lblLaserTrigger.Location = new Point(17, 35);
            lblLaserTrigger.Name = "lblLaserTrigger";
            lblLaserTrigger.Size = new Size(72, 15);
            lblLaserTrigger.TabIndex = 0;
            lblLaserTrigger.Text = "Trigger [x, y]";
            // 
            // OtherDevicesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(262, 252);
            Controls.Add(gbLaser);
            Controls.Add(gbStrobe);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OtherDevicesForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Other Devices";
            gbStrobe.ResumeLayout(false);
            gbStrobe.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numStrobeY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStrobeX).EndInit();
            gbLaser.ResumeLayout(false);
            gbLaser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numLaserColorY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserColorX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserPatternY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserPatternX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserTriggerY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLaserTriggerX).EndInit();
            ResumeLayout(false);
        }
    }
}
