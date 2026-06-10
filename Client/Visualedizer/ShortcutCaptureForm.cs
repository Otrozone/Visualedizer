using System.Drawing;
using System.Windows.Forms;

namespace Ledqualizer
{
    internal sealed class ShortcutCaptureForm : Form
    {
        private readonly Label lblInstruction;
        private readonly Label lblShortcut;
        private readonly Button btnOk;
        private readonly Button btnClear;
        private readonly Button btnCancel;

        public ShortcutCaptureForm(string title, KeyboardShortcutConfig? current)
        {
            Shortcut = current?.Clone() ?? KeyboardShortcutConfig.Empty();

            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            KeyPreview = true;
            ClientSize = new Size(360, 140);

            lblInstruction = new Label
            {
                AutoSize = false,
                Location = new Point(12, 12),
                Size = new Size(336, 36),
                Text = "Press the shortcut keys, then confirm."
            };

            lblShortcut = new Label
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(12, 52),
                Size = new Size(336, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnOk = new Button
            {
                DialogResult = DialogResult.OK,
                Location = new Point(111, 101),
                Size = new Size(75, 27),
                Text = "OK"
            };

            btnClear = new Button
            {
                Location = new Point(192, 101),
                Size = new Size(75, 27),
                Text = "Clear"
            };
            btnClear.Click += (_, _) =>
            {
                Shortcut = KeyboardShortcutConfig.Empty();
                UpdateShortcutText();
            };

            btnCancel = new Button
            {
                DialogResult = DialogResult.Cancel,
                Location = new Point(273, 101),
                Size = new Size(75, 27),
                Text = "Cancel"
            };

            Controls.Add(lblInstruction);
            Controls.Add(lblShortcut);
            Controls.Add(btnOk);
            Controls.Add(btnClear);
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
            KeyDown += ShortcutCaptureForm_KeyDown;
            UpdateShortcutText();
        }

        public KeyboardShortcutConfig Shortcut { get; private set; }

        private void ShortcutCaptureForm_KeyDown(object? sender, KeyEventArgs e)
        {
            KeyboardShortcutConfig shortcut = KeyboardShortcutConfig.FromKeyEvent(e);
            if (shortcut.IsUsable)
            {
                Shortcut = shortcut;
                UpdateShortcutText();
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void UpdateShortcutText()
        {
            lblShortcut.Text = string.IsNullOrWhiteSpace(Shortcut.ToString())
                ? "none"
                : Shortcut.ToString();
        }
    }
}
