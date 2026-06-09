using static Ledqualizer.AcVolume;

namespace Ledqualizer
{
    internal sealed class AuxiliaryMonitorItem
    {
        public int Index { get; init; }
        public string Display { get; init; } = string.Empty;
        public override string ToString() => Display;
    }

    internal static class AuxiliaryTriggerEditorSupport
    {
        public static void LoadAudioDevices(ComboBox comboBox, string? selectedDeviceId)
        {
            AcVolume.LoadAudioDevicesToComboBox(comboBox);
            comboBox.Items.Insert(0, new DeviceDescriptor
            {
                DeviceId = string.Empty,
                Text = "System Default"
            });
            SelectAudioDevice(comboBox, selectedDeviceId);
        }

        public static void SelectAudioDevice(ComboBox comboBox, string? deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }

                return;
            }

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is DeviceDescriptor descriptor
                    && string.Equals(descriptor.DeviceId, deviceId, StringComparison.Ordinal))
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }

            if (comboBox.Items.Count > 0 && comboBox.SelectedIndex < 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        public static string? GetSelectedAudioDeviceId(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as DeviceDescriptor)?.DeviceId;
        }

        public static List<AuxiliaryMonitorItem> LoadMonitors(ComboBox comboBox)
        {
            List<AuxiliaryMonitorItem> monitors = Screen.AllScreens
                .Select((screen, index) => new AuxiliaryMonitorItem
                {
                    Index = index,
                    Display = $"Display {index + 1} ({screen.Bounds.Width}x{screen.Bounds.Height})"
                })
                .ToList();

            comboBox.DataSource = null;
            comboBox.DataSource = monitors;
            comboBox.DisplayMember = nameof(AuxiliaryMonitorItem.Display);
            comboBox.ValueMember = nameof(AuxiliaryMonitorItem.Index);
            if (monitors.Count > 0 && comboBox.SelectedIndex < 0)
            {
                comboBox.SelectedIndex = 0;
            }

            return monitors;
        }

        public static void SelectMonitor(ComboBox comboBox, int monitorIndex)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is AuxiliaryMonitorItem item && item.Index == monitorIndex)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        public static int GetSelectedMonitorIndex(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as AuxiliaryMonitorItem)?.Index ?? 0;
        }

        public static Rectangle PickScreenRectangle(IWin32Window owner, int monitorIndex, Rectangle initialRelativeBounds)
        {
            using var picker = new ScreenAreaPickerForm(monitorIndex, initialRelativeBounds);
            return picker.ShowDialog(owner) == DialogResult.OK ? picker.SelectedRelativeBounds : initialRelativeBounds;
        }

        public static Screen ResolveScreen(int monitorIndex)
        {
            Screen[] screens = Screen.AllScreens;
            if (screens.Length == 0)
            {
                return Screen.PrimaryScreen ?? throw new InvalidOperationException("No screens are available.");
            }

            if (monitorIndex >= 0 && monitorIndex < screens.Length)
            {
                return screens[monitorIndex];
            }

            return Screen.PrimaryScreen ?? screens[0];
        }
    }

    internal sealed class ScreenAreaPickerForm : Form
    {
        private readonly Screen screen;
        private Point dragStart;
        private bool dragging;

        public ScreenAreaPickerForm(int monitorIndex, Rectangle initialRelativeBounds)
        {
            screen = AuxiliaryTriggerEditorSupport.ResolveScreen(monitorIndex);
            SelectedRelativeBounds = Normalize(initialRelativeBounds, screen.Bounds.Size);

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Bounds = screen.Bounds;
            BackColor = Color.Black;
            Opacity = 0.30;
            TopMost = true;
            ShowInTaskbar = false;
            DoubleBuffered = true;
            KeyPreview = true;
            Cursor = Cursors.Cross;

            MouseDown += ScreenAreaPickerForm_MouseDown;
            MouseMove += ScreenAreaPickerForm_MouseMove;
            MouseUp += ScreenAreaPickerForm_MouseUp;
            KeyDown += ScreenAreaPickerForm_KeyDown;
            Paint += ScreenAreaPickerForm_Paint;
        }

        public Rectangle SelectedRelativeBounds { get; private set; }

        private void ScreenAreaPickerForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ScreenAreaPickerForm_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            dragging = true;
            dragStart = e.Location;
            SelectedRelativeBounds = Normalize(new Rectangle(e.X, e.Y, 1, 1), screen.Bounds.Size);
            Invalidate();
        }

        private void ScreenAreaPickerForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!dragging)
            {
                return;
            }

            int left = Math.Min(dragStart.X, e.X);
            int top = Math.Min(dragStart.Y, e.Y);
            int width = Math.Abs(e.X - dragStart.X);
            int height = Math.Abs(e.Y - dragStart.Y);
            SelectedRelativeBounds = Normalize(new Rectangle(left, top, Math.Max(1, width), Math.Max(1, height)), screen.Bounds.Size);
            Invalidate();
        }

        private void ScreenAreaPickerForm_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!dragging || e.Button != MouseButtons.Left)
            {
                return;
            }

            dragging = false;
            if (SelectedRelativeBounds.Width <= 0 || SelectedRelativeBounds.Height <= 0)
            {
                SelectedRelativeBounds = Normalize(new Rectangle(e.X, e.Y, 1, 1), screen.Bounds.Size);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ScreenAreaPickerForm_Paint(object? sender, PaintEventArgs e)
        {
            using var shadeBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            using var selectionBrush = new SolidBrush(Color.FromArgb(45, Color.White));
            using var borderPen = new Pen(Color.White, 2);
            using var textBrush = new SolidBrush(Color.White);

            e.Graphics.FillRectangle(shadeBrush, ClientRectangle);
            Rectangle selected = SelectedRelativeBounds;
            e.Graphics.FillRectangle(selectionBrush, selected);
            e.Graphics.DrawRectangle(borderPen, selected);

            string caption = "Drag to select area. Enter accepts, Esc cancels.";
            SizeF textSize = e.Graphics.MeasureString(caption, Font);
            RectangleF textBounds = new(
                12,
                12,
                textSize.Width + 12,
                textSize.Height + 8);
            using var textBackBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            e.Graphics.FillRectangle(textBackBrush, textBounds);
            e.Graphics.DrawString(caption, Font, textBrush, textBounds.Left + 6, textBounds.Top + 4);
        }

        private static Rectangle Normalize(Rectangle bounds, Size screenSize)
        {
            int x = Math.Clamp(bounds.X, 0, Math.Max(0, screenSize.Width - 1));
            int y = Math.Clamp(bounds.Y, 0, Math.Max(0, screenSize.Height - 1));
            int width = Math.Clamp(bounds.Width, 1, Math.Max(1, screenSize.Width - x));
            int height = Math.Clamp(bounds.Height, 1, Math.Max(1, screenSize.Height - y));
            return new Rectangle(x, y, width, height);
        }
    }
}
