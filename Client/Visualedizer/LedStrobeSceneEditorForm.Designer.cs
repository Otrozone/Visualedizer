namespace Ledqualizer
{
    partial class LedStrobeSceneEditorForm
    {
        private System.ComponentModel.IContainer? components = null;
        private TableLayoutPanel layoutRoot = null!;
        private Label lblInfo = null!;
        private GroupBox gbOnTiming = null!;
        private TableLayoutPanel layoutOnTiming = null!;
        private Label lblOnDurationMode = null!;
        private ComboBox cbOnDurationMode = null!;
        private Label lblOnDurationMs = null!;
        private NumericUpDown numOnDurationMs = null!;
        private Label lblOnDurationMinMs = null!;
        private NumericUpDown numOnDurationMinMs = null!;
        private Label lblOnDurationMaxMs = null!;
        private NumericUpDown numOnDurationMaxMs = null!;
        private GroupBox gbOffTiming = null!;
        private TableLayoutPanel layoutOffTiming = null!;
        private Label lblOffDurationMode = null!;
        private ComboBox cbOffDurationMode = null!;
        private Label lblOffDurationMs = null!;
        private NumericUpDown numOffDurationMs = null!;
        private Label lblOffDurationMinMs = null!;
        private NumericUpDown numOffDurationMinMs = null!;
        private Label lblOffDurationMaxMs = null!;
        private NumericUpDown numOffDurationMaxMs = null!;
        private GroupBox gbColor = null!;
        private TableLayoutPanel layoutColor = null!;
        private Label lblHueMode = null!;
        private ComboBox cbHueMode = null!;
        private Label lblHue = null!;
        private Visualedizer.UcHue ucHue = null!;
        private Label lblHueRange = null!;
        private Visualedizer.UcHueMinMax ucHueRange = null!;
        private Label lblSaturation = null!;
        private NumericUpDown numSaturation = null!;
        private Label lblBrightness = null!;
        private NumericUpDown numBrightness = null!;

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
            layoutRoot = new TableLayoutPanel();
            lblInfo = new Label();
            gbOnTiming = new GroupBox();
            layoutOnTiming = new TableLayoutPanel();
            lblOnDurationMode = new Label();
            cbOnDurationMode = new ComboBox();
            lblOnDurationMs = new Label();
            numOnDurationMs = new NumericUpDown();
            lblOnDurationMinMs = new Label();
            numOnDurationMinMs = new NumericUpDown();
            lblOnDurationMaxMs = new Label();
            numOnDurationMaxMs = new NumericUpDown();
            gbOffTiming = new GroupBox();
            layoutOffTiming = new TableLayoutPanel();
            lblOffDurationMode = new Label();
            cbOffDurationMode = new ComboBox();
            lblOffDurationMs = new Label();
            numOffDurationMs = new NumericUpDown();
            lblOffDurationMinMs = new Label();
            numOffDurationMinMs = new NumericUpDown();
            lblOffDurationMaxMs = new Label();
            numOffDurationMaxMs = new NumericUpDown();
            gbColor = new GroupBox();
            layoutColor = new TableLayoutPanel();
            lblHueMode = new Label();
            cbHueMode = new ComboBox();
            lblHue = new Label();
            ucHue = new Visualedizer.UcHue();
            lblHueRange = new Label();
            ucHueRange = new Visualedizer.UcHueMinMax();
            lblSaturation = new Label();
            numSaturation = new NumericUpDown();
            lblBrightness = new Label();
            numBrightness = new NumericUpDown();
            layoutRoot.SuspendLayout();
            gbOnTiming.SuspendLayout();
            layoutOnTiming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMinMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMaxMs).BeginInit();
            gbOffTiming.SuspendLayout();
            layoutOffTiming.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMinMs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMaxMs).BeginInit();
            gbColor.SuspendLayout();
            layoutColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSaturation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numBrightness).BeginInit();
            SuspendLayout();
            // 
            // layoutRoot
            // 
            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Controls.Add(lblInfo, 0, 0);
            layoutRoot.Controls.Add(gbOnTiming, 0, 1);
            layoutRoot.Controls.Add(gbOffTiming, 0, 2);
            layoutRoot.Controls.Add(gbColor, 0, 3);
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Location = new Point(0, 0);
            layoutRoot.Name = "layoutRoot";
            layoutRoot.Padding = new Padding(12);
            layoutRoot.RowCount = 4;
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.RowStyles.Add(new RowStyle());
            layoutRoot.Size = new Size(760, 340);
            layoutRoot.TabIndex = 0;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(15, 12);
            lblInfo.Margin = new Padding(3, 0, 3, 8);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(586, 15);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "Configure an LED strip strobe loop. Random ranges are sampled at each phase or flash transition.";
            // 
            // gbOnTiming
            // 
            gbOnTiming.AutoSize = true;
            gbOnTiming.Controls.Add(layoutOnTiming);
            gbOnTiming.Dock = DockStyle.Fill;
            gbOnTiming.Location = new Point(15, 38);
            gbOnTiming.Name = "gbOnTiming";
            gbOnTiming.Size = new Size(730, 72);
            gbOnTiming.TabIndex = 1;
            gbOnTiming.TabStop = false;
            gbOnTiming.Text = "On time";
            // 
            // layoutOnTiming
            // 
            layoutOnTiming.AutoSize = true;
            layoutOnTiming.ColumnCount = 8;
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOnTiming.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutOnTiming.Controls.Add(lblOnDurationMode, 0, 0);
            layoutOnTiming.Controls.Add(cbOnDurationMode, 1, 0);
            layoutOnTiming.Controls.Add(lblOnDurationMs, 2, 0);
            layoutOnTiming.Controls.Add(numOnDurationMs, 3, 0);
            layoutOnTiming.Controls.Add(lblOnDurationMinMs, 4, 0);
            layoutOnTiming.Controls.Add(numOnDurationMinMs, 5, 0);
            layoutOnTiming.Controls.Add(lblOnDurationMaxMs, 6, 0);
            layoutOnTiming.Controls.Add(numOnDurationMaxMs, 7, 0);
            layoutOnTiming.Dock = DockStyle.Fill;
            layoutOnTiming.Location = new Point(3, 19);
            layoutOnTiming.Name = "layoutOnTiming";
            layoutOnTiming.Padding = new Padding(8);
            layoutOnTiming.RowCount = 1;
            layoutOnTiming.RowStyles.Add(new RowStyle());
            layoutOnTiming.Size = new Size(724, 50);
            layoutOnTiming.TabIndex = 0;
            // 
            // lblOnDurationMode
            // 
            lblOnDurationMode.Anchor = AnchorStyles.Left;
            lblOnDurationMode.AutoSize = true;
            lblOnDurationMode.Location = new Point(11, 17);
            lblOnDurationMode.Name = "lblOnDurationMode";
            lblOnDurationMode.Size = new Size(38, 15);
            lblOnDurationMode.TabIndex = 0;
            lblOnDurationMode.Text = "Mode";
            // 
            // cbOnDurationMode
            // 
            cbOnDurationMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOnDurationMode.FormattingEnabled = true;
            cbOnDurationMode.Location = new Point(55, 11);
            cbOnDurationMode.Name = "cbOnDurationMode";
            cbOnDurationMode.Size = new Size(130, 23);
            cbOnDurationMode.TabIndex = 1;
            cbOnDurationMode.SelectedIndexChanged += ControlValueChanged;
            // 
            // lblOnDurationMs
            // 
            lblOnDurationMs.Anchor = AnchorStyles.Left;
            lblOnDurationMs.AutoSize = true;
            lblOnDurationMs.Location = new Point(191, 17);
            lblOnDurationMs.Name = "lblOnDurationMs";
            lblOnDurationMs.Size = new Size(22, 15);
            lblOnDurationMs.TabIndex = 2;
            lblOnDurationMs.Text = "ms";
            // 
            // numOnDurationMs
            // 
            numOnDurationMs.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numOnDurationMs.Location = new Point(219, 11);
            numOnDurationMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numOnDurationMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numOnDurationMs.Name = "numOnDurationMs";
            numOnDurationMs.Size = new Size(90, 23);
            numOnDurationMs.TabIndex = 3;
            numOnDurationMs.Value = new decimal(new int[] { 80, 0, 0, 0 });
            numOnDurationMs.ValueChanged += ControlValueChanged;
            // 
            // lblOnDurationMinMs
            // 
            lblOnDurationMinMs.Anchor = AnchorStyles.Left;
            lblOnDurationMinMs.AutoSize = true;
            lblOnDurationMinMs.Location = new Point(315, 17);
            lblOnDurationMinMs.Name = "lblOnDurationMinMs";
            lblOnDurationMinMs.Size = new Size(45, 15);
            lblOnDurationMinMs.TabIndex = 4;
            lblOnDurationMinMs.Text = "Min ms";
            // 
            // numOnDurationMinMs
            // 
            numOnDurationMinMs.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numOnDurationMinMs.Location = new Point(366, 11);
            numOnDurationMinMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numOnDurationMinMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numOnDurationMinMs.Name = "numOnDurationMinMs";
            numOnDurationMinMs.Size = new Size(90, 23);
            numOnDurationMinMs.TabIndex = 5;
            numOnDurationMinMs.Value = new decimal(new int[] { 80, 0, 0, 0 });
            numOnDurationMinMs.ValueChanged += ControlValueChanged;
            // 
            // lblOnDurationMaxMs
            // 
            lblOnDurationMaxMs.Anchor = AnchorStyles.Left;
            lblOnDurationMaxMs.AutoSize = true;
            lblOnDurationMaxMs.Location = new Point(462, 17);
            lblOnDurationMaxMs.Name = "lblOnDurationMaxMs";
            lblOnDurationMaxMs.Size = new Size(47, 15);
            lblOnDurationMaxMs.TabIndex = 6;
            lblOnDurationMaxMs.Text = "Max ms";
            // 
            // numOnDurationMaxMs
            // 
            numOnDurationMaxMs.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numOnDurationMaxMs.Location = new Point(515, 11);
            numOnDurationMaxMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numOnDurationMaxMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numOnDurationMaxMs.Name = "numOnDurationMaxMs";
            numOnDurationMaxMs.Size = new Size(90, 23);
            numOnDurationMaxMs.TabIndex = 7;
            numOnDurationMaxMs.Value = new decimal(new int[] { 160, 0, 0, 0 });
            numOnDurationMaxMs.ValueChanged += ControlValueChanged;
            // 
            // gbOffTiming
            // 
            gbOffTiming.AutoSize = true;
            gbOffTiming.Controls.Add(layoutOffTiming);
            gbOffTiming.Dock = DockStyle.Fill;
            gbOffTiming.Location = new Point(15, 116);
            gbOffTiming.Name = "gbOffTiming";
            gbOffTiming.Size = new Size(730, 72);
            gbOffTiming.TabIndex = 2;
            gbOffTiming.TabStop = false;
            gbOffTiming.Text = "Off time";
            // 
            // layoutOffTiming
            // 
            layoutOffTiming.AutoSize = true;
            layoutOffTiming.ColumnCount = 8;
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle());
            layoutOffTiming.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutOffTiming.Controls.Add(lblOffDurationMode, 0, 0);
            layoutOffTiming.Controls.Add(cbOffDurationMode, 1, 0);
            layoutOffTiming.Controls.Add(lblOffDurationMs, 2, 0);
            layoutOffTiming.Controls.Add(numOffDurationMs, 3, 0);
            layoutOffTiming.Controls.Add(lblOffDurationMinMs, 4, 0);
            layoutOffTiming.Controls.Add(numOffDurationMinMs, 5, 0);
            layoutOffTiming.Controls.Add(lblOffDurationMaxMs, 6, 0);
            layoutOffTiming.Controls.Add(numOffDurationMaxMs, 7, 0);
            layoutOffTiming.Dock = DockStyle.Fill;
            layoutOffTiming.Location = new Point(3, 19);
            layoutOffTiming.Name = "layoutOffTiming";
            layoutOffTiming.Padding = new Padding(8);
            layoutOffTiming.RowCount = 1;
            layoutOffTiming.RowStyles.Add(new RowStyle());
            layoutOffTiming.Size = new Size(724, 50);
            layoutOffTiming.TabIndex = 0;
            // 
            // lblOffDurationMode
            // 
            lblOffDurationMode.Anchor = AnchorStyles.Left;
            lblOffDurationMode.AutoSize = true;
            lblOffDurationMode.Location = new Point(11, 17);
            lblOffDurationMode.Name = "lblOffDurationMode";
            lblOffDurationMode.Size = new Size(38, 15);
            lblOffDurationMode.TabIndex = 0;
            lblOffDurationMode.Text = "Mode";
            // 
            // cbOffDurationMode
            // 
            cbOffDurationMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOffDurationMode.FormattingEnabled = true;
            cbOffDurationMode.Location = new Point(55, 11);
            cbOffDurationMode.Name = "cbOffDurationMode";
            cbOffDurationMode.Size = new Size(130, 23);
            cbOffDurationMode.TabIndex = 1;
            cbOffDurationMode.SelectedIndexChanged += ControlValueChanged;
            // 
            // lblOffDurationMs
            // 
            lblOffDurationMs.Anchor = AnchorStyles.Left;
            lblOffDurationMs.AutoSize = true;
            lblOffDurationMs.Location = new Point(191, 17);
            lblOffDurationMs.Name = "lblOffDurationMs";
            lblOffDurationMs.Size = new Size(22, 15);
            lblOffDurationMs.TabIndex = 2;
            lblOffDurationMs.Text = "ms";
            // 
            // numOffDurationMs
            // 
            numOffDurationMs.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numOffDurationMs.Location = new Point(219, 11);
            numOffDurationMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numOffDurationMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numOffDurationMs.Name = "numOffDurationMs";
            numOffDurationMs.Size = new Size(90, 23);
            numOffDurationMs.TabIndex = 3;
            numOffDurationMs.Value = new decimal(new int[] { 80, 0, 0, 0 });
            numOffDurationMs.ValueChanged += ControlValueChanged;
            // 
            // lblOffDurationMinMs
            // 
            lblOffDurationMinMs.Anchor = AnchorStyles.Left;
            lblOffDurationMinMs.AutoSize = true;
            lblOffDurationMinMs.Location = new Point(315, 17);
            lblOffDurationMinMs.Name = "lblOffDurationMinMs";
            lblOffDurationMinMs.Size = new Size(45, 15);
            lblOffDurationMinMs.TabIndex = 4;
            lblOffDurationMinMs.Text = "Min ms";
            // 
            // numOffDurationMinMs
            // 
            numOffDurationMinMs.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numOffDurationMinMs.Location = new Point(366, 11);
            numOffDurationMinMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numOffDurationMinMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numOffDurationMinMs.Name = "numOffDurationMinMs";
            numOffDurationMinMs.Size = new Size(90, 23);
            numOffDurationMinMs.TabIndex = 5;
            numOffDurationMinMs.Value = new decimal(new int[] { 40, 0, 0, 0 });
            numOffDurationMinMs.ValueChanged += ControlValueChanged;
            // 
            // lblOffDurationMaxMs
            // 
            lblOffDurationMaxMs.Anchor = AnchorStyles.Left;
            lblOffDurationMaxMs.AutoSize = true;
            lblOffDurationMaxMs.Location = new Point(462, 17);
            lblOffDurationMaxMs.Name = "lblOffDurationMaxMs";
            lblOffDurationMaxMs.Size = new Size(47, 15);
            lblOffDurationMaxMs.TabIndex = 6;
            lblOffDurationMaxMs.Text = "Max ms";
            // 
            // numOffDurationMaxMs
            // 
            numOffDurationMaxMs.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numOffDurationMaxMs.Location = new Point(515, 11);
            numOffDurationMaxMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numOffDurationMaxMs.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numOffDurationMaxMs.Name = "numOffDurationMaxMs";
            numOffDurationMaxMs.Size = new Size(90, 23);
            numOffDurationMaxMs.TabIndex = 7;
            numOffDurationMaxMs.Value = new decimal(new int[] { 160, 0, 0, 0 });
            numOffDurationMaxMs.ValueChanged += ControlValueChanged;
            // 
            // gbColor
            // 
            gbColor.AutoSize = true;
            gbColor.Controls.Add(layoutColor);
            gbColor.Dock = DockStyle.Fill;
            gbColor.Location = new Point(15, 194);
            gbColor.Name = "gbColor";
            gbColor.Size = new Size(730, 122);
            gbColor.TabIndex = 3;
            gbColor.TabStop = false;
            gbColor.Text = "Color";
            // 
            // layoutColor
            // 
            layoutColor.AutoSize = true;
            layoutColor.ColumnCount = 6;
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.ColumnStyles.Add(new ColumnStyle());
            layoutColor.Controls.Add(lblHueMode, 0, 0);
            layoutColor.Controls.Add(cbHueMode, 1, 0);
            layoutColor.Controls.Add(lblSaturation, 4, 0);
            layoutColor.Controls.Add(numSaturation, 5, 0);
            layoutColor.Controls.Add(lblHue, 0, 1);
            layoutColor.Controls.Add(ucHue, 1, 1);
            layoutColor.Controls.Add(lblBrightness, 4, 1);
            layoutColor.Controls.Add(numBrightness, 5, 1);
            layoutColor.Controls.Add(lblHueRange, 0, 2);
            layoutColor.Controls.Add(ucHueRange, 1, 2);
            layoutColor.Dock = DockStyle.Fill;
            layoutColor.Location = new Point(3, 19);
            layoutColor.Name = "layoutColor";
            layoutColor.Padding = new Padding(8);
            layoutColor.RowCount = 3;
            layoutColor.RowStyles.Add(new RowStyle());
            layoutColor.RowStyles.Add(new RowStyle());
            layoutColor.RowStyles.Add(new RowStyle());
            layoutColor.Size = new Size(724, 100);
            layoutColor.TabIndex = 0;
            // 
            // lblHueMode
            // 
            lblHueMode.Anchor = AnchorStyles.Left;
            lblHueMode.AutoSize = true;
            lblHueMode.Location = new Point(11, 17);
            lblHueMode.Name = "lblHueMode";
            lblHueMode.Size = new Size(38, 15);
            lblHueMode.TabIndex = 0;
            lblHueMode.Text = "Mode";
            // 
            // cbHueMode
            // 
            cbHueMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbHueMode.FormattingEnabled = true;
            cbHueMode.Location = new Point(58, 11);
            cbHueMode.Name = "cbHueMode";
            cbHueMode.Size = new Size(130, 23);
            cbHueMode.TabIndex = 1;
            cbHueMode.SelectedIndexChanged += ControlValueChanged;
            // 
            // lblHue
            // 
            lblHue.Anchor = AnchorStyles.Left;
            lblHue.AutoSize = true;
            lblHue.Location = new Point(11, 45);
            lblHue.Name = "lblHue";
            lblHue.Size = new Size(29, 15);
            lblHue.TabIndex = 4;
            lblHue.Text = "Hue";
            // 
            // ucHue
            // 
            ucHue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            layoutColor.SetColumnSpan(ucHue, 3);
            ucHue.Hue = 0;
            ucHue.Location = new Point(58, 40);
            ucHue.MaxVal = 360;
            ucHue.MinVal = 0;
            ucHue.Name = "ucHue";
            ucHue.Size = new Size(420, 26);
            ucHue.TabIndex = 5;
            ucHue.ValueChanged += ControlValueChanged;
            // 
            // lblHueRange
            // 
            lblHueRange.Anchor = AnchorStyles.Left;
            lblHueRange.AutoSize = true;
            lblHueRange.Location = new Point(11, 73);
            lblHueRange.Name = "lblHueRange";
            lblHueRange.Size = new Size(41, 15);
            lblHueRange.TabIndex = 8;
            lblHueRange.Text = "Range";
            // 
            // ucHueRange
            // 
            ucHueRange.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            layoutColor.SetColumnSpan(ucHueRange, 3);
            ucHueRange.HueEnd = 360;
            ucHueRange.HueStart = 0;
            ucHueRange.Location = new Point(58, 68);
            ucHueRange.Name = "ucHueRange";
            ucHueRange.Size = new Size(420, 26);
            ucHueRange.TabIndex = 9;
            ucHueRange.ValueChanged += ControlValueChanged;
            // 
            // lblSaturation
            // 
            lblSaturation.Anchor = AnchorStyles.Left;
            lblSaturation.AutoSize = true;
            lblSaturation.Location = new Point(484, 17);
            lblSaturation.Name = "lblSaturation";
            lblSaturation.Size = new Size(72, 15);
            lblSaturation.TabIndex = 2;
            lblSaturation.Text = "Saturation %";
            // 
            // numSaturation
            // 
            numSaturation.Location = new Point(562, 11);
            numSaturation.Name = "numSaturation";
            numSaturation.Size = new Size(80, 23);
            numSaturation.TabIndex = 3;
            numSaturation.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numSaturation.ValueChanged += ControlValueChanged;
            // 
            // lblBrightness
            // 
            lblBrightness.Anchor = AnchorStyles.Left;
            lblBrightness.AutoSize = true;
            lblBrightness.Location = new Point(484, 45);
            lblBrightness.Name = "lblBrightness";
            lblBrightness.Size = new Size(74, 15);
            lblBrightness.TabIndex = 6;
            lblBrightness.Text = "Brightness %";
            // 
            // numBrightness
            // 
            numBrightness.Location = new Point(562, 40);
            numBrightness.Name = "numBrightness";
            numBrightness.Size = new Size(80, 23);
            numBrightness.TabIndex = 7;
            numBrightness.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numBrightness.ValueChanged += ControlValueChanged;
            // 
            // LedStrobeSceneEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(760, 340);
            Controls.Add(layoutRoot);
            Name = "LedStrobeSceneEditorForm";
            Text = "Strobe";
            layoutRoot.ResumeLayout(false);
            layoutRoot.PerformLayout();
            gbOnTiming.ResumeLayout(false);
            gbOnTiming.PerformLayout();
            layoutOnTiming.ResumeLayout(false);
            layoutOnTiming.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMinMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOnDurationMaxMs).EndInit();
            gbOffTiming.ResumeLayout(false);
            gbOffTiming.PerformLayout();
            layoutOffTiming.ResumeLayout(false);
            layoutOffTiming.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMinMs).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOffDurationMaxMs).EndInit();
            gbColor.ResumeLayout(false);
            gbColor.PerformLayout();
            layoutColor.ResumeLayout(false);
            layoutColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSaturation).EndInit();
            ((System.ComponentModel.ISupportInitialize)numBrightness).EndInit();
            ResumeLayout(false);
        }
    }
}
