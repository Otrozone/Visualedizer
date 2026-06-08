using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;

namespace Ledqualizer
{
    public partial class FrmMain
    {
        private bool uiEnhancementsInitialized;
        private bool pendingSplitterRestore;
        private SceneType? pendingNewSceneType;
        private SceneAddDropDownButton? sceneAddDropDownButton;
        private Button? originalAddSceneButton;
        private readonly HashSet<string> trackedSplitContainers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> customStripNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private TextBox? activeStripNameEditor;
        private DataGridView? activeStripNameGrid;
        private int activeStripNameRowIndex = -1;
        private int activeStripNameColumnIndex = -1;
        private string? activeStripNameKey;
        private string? activeStripDefaultName;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            EnsureUiEnhancementsInitialized();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (pendingSplitterRestore)
            {
                ApplyPendingSplitterRestore();
            }
        }

        private void EnsureUiEnhancementsInitialized()
        {
            if (uiEnhancementsInitialized || IsDisposed)
            {
                return;
            }

            uiEnhancementsInitialized = true;
            InitializeSceneAddDropDown();
            InitializeStripNameEditing();
            RestoreWindowLayout();
            HookWindowLayoutTracking();
        }

        private void InitializeSceneAddDropDown()
        {
            originalAddSceneButton = FindControl<Button>(this, "btnAddScene")
                ?? FindButtonByText(this, "Add scene");

            if (originalAddSceneButton == null || originalAddSceneButton.Parent == null)
            {
                return;
            }

            if (sceneAddDropDownButton != null)
            {
                sceneAddDropDownButton.DropDownMenu = BuildSceneAddMenu();
                return;
            }

            sceneAddDropDownButton = new SceneAddDropDownButton
            {
                Name = "btnAddSceneDropDown",
                Text = originalAddSceneButton.Text,
                Font = originalAddSceneButton.Font,
                Size = originalAddSceneButton.Size,
                Location = originalAddSceneButton.Location,
                Anchor = originalAddSceneButton.Anchor,
                Dock = originalAddSceneButton.Dock,
                Margin = originalAddSceneButton.Margin,
                Padding = originalAddSceneButton.Padding,
                TabIndex = originalAddSceneButton.TabIndex,
                FlatStyle = originalAddSceneButton.FlatStyle,
                UseVisualStyleBackColor = originalAddSceneButton.UseVisualStyleBackColor,
                BackColor = originalAddSceneButton.BackColor,
                ForeColor = originalAddSceneButton.ForeColor,
                DropDownMenu = BuildSceneAddMenu()
            };

            var parent = originalAddSceneButton.Parent;
            parent.Controls.Add(sceneAddDropDownButton);
            parent.Controls.SetChildIndex(sceneAddDropDownButton, parent.Controls.GetChildIndex(originalAddSceneButton));

            originalAddSceneButton.Visible = false;
            originalAddSceneButton.Enabled = true;
        }

        private ContextMenuStrip BuildSceneAddMenu()
        {
            var menu = new ContextMenuStrip();

            foreach (var sceneType in Enum.GetValues(typeof(SceneType)).Cast<SceneType>().OrderBy(value => FormatSceneTypeName(value), StringComparer.CurrentCultureIgnoreCase))
            {
                var item = new ToolStripMenuItem(FormatSceneTypeName(sceneType))
                {
                    Tag = sceneType
                };
                item.Click += (_, _) => AddSceneFromDropDown(sceneType);
                menu.Items.Add(item);
            }

            return menu;
        }

        private void AddSceneFromDropDown(SceneType sceneType)
        {
            var existingScenes = new HashSet<SceneConfig>(appConfig.Scenes);

            pendingNewSceneType = sceneType;

            try
            {
                TriggerOriginalAddSceneButton();
            }
            finally
            {
                pendingNewSceneType = null;
            }

            var scene = appConfig.Scenes.FirstOrDefault(item => !existingScenes.Contains(item));
            if (scene == null)
            {
                return;
            }

            scene.Type = sceneType;
            scene.Name = FormatSceneTypeName(sceneType);

            ResetSceneBindings();
            SelectSceneInUi(scene);
        }

        private SceneType ConsumePendingNewSceneType(SceneType fallback)
        {
            if (pendingNewSceneType.HasValue)
            {
                var sceneType = pendingNewSceneType.Value;
                pendingNewSceneType = null;
                return sceneType;
            }

            return fallback;
        }

        private void ResetSceneBindings()
        {
            foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.GetValue(this) is BindingSource bindingSource)
                {
                    bindingSource.ResetBindings(true);
                }
            }

            foreach (var control in GetAllControls(this))
            {
                switch (control)
                {
                    case DataGridView grid when grid.DataSource is BindingSource gridBindingSource:
                        gridBindingSource.ResetBindings(true);
                        grid.Refresh();
                        break;
                    case ListControl listControl when listControl.DataSource is BindingSource listBindingSource:
                        listBindingSource.ResetBindings(true);
                        listControl.Refresh();
                        break;
                    case ListControl listControl when ReferenceEquals(listControl.DataSource, appConfig.Scenes):
                        var dataSource = listControl.DataSource;
                        var displayMember = listControl.DisplayMember;
                        var valueMember = listControl.ValueMember;
                        listControl.DataSource = null;
                        listControl.DisplayMember = displayMember;
                        listControl.ValueMember = valueMember;
                        listControl.DataSource = dataSource;
                        listControl.Refresh();
                        break;
                    case DataGridView grid when ReferenceEquals(grid.DataSource, appConfig.Scenes):
                        var gridDataSource = grid.DataSource;
                        grid.DataSource = null;
                        grid.DataSource = gridDataSource;
                        grid.Refresh();
                        break;
                    default:
                        control.Refresh();
                        break;
                }
            }
        }

        private void SelectSceneInUi(SceneConfig scene)
        {
            foreach (var control in GetAllControls(this))
            {
                switch (control)
                {
                    case ListBox listBox:
                        for (var index = 0; index < listBox.Items.Count; index++)
                        {
                            if (ReferenceEquals(listBox.Items[index], scene))
                            {
                                listBox.SelectedIndex = index;
                                listBox.Focus();
                                return;
                            }
                        }
                        break;
                    case ComboBox comboBox:
                        for (var index = 0; index < comboBox.Items.Count; index++)
                        {
                            if (ReferenceEquals(comboBox.Items[index], scene))
                            {
                                comboBox.SelectedIndex = index;
                                comboBox.Focus();
                                return;
                            }
                        }
                        break;
                    case DataGridView grid:
                        foreach (DataGridViewRow row in grid.Rows)
                        {
                            if (ReferenceEquals(row.DataBoundItem, scene))
                            {
                                row.Selected = true;
                                if (row.Cells.Count > 0)
                                {
                                    grid.CurrentCell = row.Cells[0];
                                }
                                grid.Focus();
                                return;
                            }
                        }
                        break;
                }
            }
        }

        private void HookWindowLayoutTracking()
        {
            FormClosing -= FrmMain_SaveWindowLayoutOnClose;
            FormClosing += FrmMain_SaveWindowLayoutOnClose;

            foreach (var splitContainer in GetAllControls(this).OfType<SplitContainer>())
            {
                if (trackedSplitContainers.Add(splitContainer.Name))
                {
                    splitContainer.SplitterMoved += (_, _) => SaveWindowLayout();
                }
            }
        }

        private void FrmMain_SaveWindowLayoutOnClose(object? sender, FormClosingEventArgs e)
        {
            SaveStripNames();
            SaveWindowLayout();
        }

        private void InitializeStripNameEditing()
        {
            foreach (var pair in StripNameStore.Load(StripNamesFilePath))
            {
                customStripNames[pair.Key] = pair.Value;
            }

            var deviceGrid = FindControl<DataGridView>(this, "dgvDevices");
            if (deviceGrid == null)
            {
                return;
            }

            deviceGrid.CellFormatting -= DeviceGrid_CellFormatting;
            deviceGrid.CellFormatting += DeviceGrid_CellFormatting;
            deviceGrid.CellDoubleClick -= DeviceGrid_CellDoubleClick;
            deviceGrid.CellDoubleClick += DeviceGrid_CellDoubleClick;
            deviceGrid.Scroll -= DeviceGrid_EndInlineEditOnLayoutChange;
            deviceGrid.Scroll += DeviceGrid_EndInlineEditOnLayoutChange;
            deviceGrid.ColumnWidthChanged -= DeviceGrid_EndInlineEditOnColumnWidthChanged;
            deviceGrid.ColumnWidthChanged += DeviceGrid_EndInlineEditOnColumnWidthChanged;
            deviceGrid.RowHeightChanged -= DeviceGrid_EndInlineEditOnRowHeightChanged;
            deviceGrid.RowHeightChanged += DeviceGrid_EndInlineEditOnRowHeightChanged;
            deviceGrid.Resize -= DeviceGrid_EndInlineEditOnResize;
            deviceGrid.Resize += DeviceGrid_EndInlineEditOnResize;
        }

        private void DeviceGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is not DataGridView grid || e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (!TryGetStripNameCell(grid, e.RowIndex, e.ColumnIndex, out var defaultName, out var stripKey))
            {
                return;
            }

            if (!customStripNames.TryGetValue(stripKey, out var customName) || string.IsNullOrWhiteSpace(customName))
            {
                customName = defaultName;
            }

            e.Value = customName;
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Padding = new Padding(16, 0, 0, 0);
            e.FormattingApplied = true;
        }

        private void DeviceGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridView grid || e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (!TryGetStripNameCell(grid, e.RowIndex, e.ColumnIndex, out var defaultName, out var stripKey))
            {
                return;
            }

            BeginInlineStripNameEdit(grid, e.RowIndex, e.ColumnIndex, defaultName, stripKey);
        }

        private bool TryGetStripNameCell(DataGridView grid, int rowIndex, int columnIndex, out string defaultName, out string stripKey)
        {
            defaultName = string.Empty;
            stripKey = string.Empty;

            var row = grid.Rows[rowIndex];
            var cellValue = row.Cells[columnIndex].Value?.ToString()?.Trim() ?? string.Empty;
            if (!IsDefaultStripName(cellValue))
            {
                return false;
            }

            var firstTextColumn = grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(column => column.Visible)
                .OrderBy(column => column.DisplayIndex)
                .FirstOrDefault(column => column.ValueType == null || column.ValueType == typeof(string));

            if (firstTextColumn == null || columnIndex != firstTextColumn.Index)
            {
                return false;
            }

            var stripNumber = ParseStripNumber(cellValue);
            if (stripNumber == null)
            {
                return false;
            }

            var deviceKey = ResolveStripDeviceKey(grid, rowIndex);
            if (string.IsNullOrWhiteSpace(deviceKey))
            {
                return false;
            }

            defaultName = cellValue;
            stripKey = $"{deviceKey}|{stripNumber.Value.ToString(CultureInfo.InvariantCulture)}";
            return true;
        }

        private string ResolveStripDeviceKey(DataGridView grid, int rowIndex)
        {
            var row = grid.Rows[rowIndex];
            var dataBoundItem = row.DataBoundItem;
            var deviceKey = TryReadStringProperty(dataBoundItem, "DeviceId", "RootDeviceId", "ParentDeviceId", "ControllerId", "IpAddress", "Address", "Host", "Name");
            if (!string.IsNullOrWhiteSpace(deviceKey))
            {
                return deviceKey;
            }

            for (var index = rowIndex - 1; index >= 0; index--)
            {
                var candidate = grid.Rows[index].Cells
                    .Cast<DataGridViewCell>()
                    .Select(cell => cell.Value?.ToString()?.Trim())
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

                if (!string.IsNullOrWhiteSpace(candidate) && !IsDefaultStripName(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        private static bool IsDefaultStripName(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("Strip ", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return ParseStripNumber(value).HasValue;
        }

        private static int? ParseStripNumber(string value)
        {
            var suffix = value.Substring("Strip ".Length).Trim();
            return int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out var stripNumber)
                ? stripNumber
                : null;
        }

        private static string TryReadStringProperty(object? instance, params string[] propertyNames)
        {
            if (instance == null)
            {
                return string.Empty;
            }

            foreach (var propertyName in propertyNames)
            {
                var property = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var value = property?.GetValue(instance)?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private void SaveStripNames()
        {
            StripNameStore.Save(StripNamesFilePath, customStripNames);
        }

        private void BeginInlineStripNameEdit(DataGridView grid, int rowIndex, int columnIndex, string defaultName, string stripKey)
        {
            EndInlineStripNameEdit(commit: true);

            var currentName = customStripNames.TryGetValue(stripKey, out var customName) && !string.IsNullOrWhiteSpace(customName)
                ? customName
                : defaultName;

            var cellBounds = grid.GetCellDisplayRectangle(columnIndex, rowIndex, true);
            if (cellBounds.Width <= 4 || cellBounds.Height <= 4)
            {
                return;
            }

            var editor = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Text = currentName
            };

            var leftInset = 16;
            editor.SetBounds(
                cellBounds.Left + leftInset,
                cellBounds.Top + 1,
                Math.Max(24, cellBounds.Width - leftInset - 2),
                Math.Max(20, cellBounds.Height - 2));

            editor.KeyDown += InlineStripNameEditor_KeyDown;
            editor.LostFocus += InlineStripNameEditor_LostFocus;

            activeStripNameEditor = editor;
            activeStripNameGrid = grid;
            activeStripNameRowIndex = rowIndex;
            activeStripNameColumnIndex = columnIndex;
            activeStripNameKey = stripKey;
            activeStripDefaultName = defaultName;

            grid.Controls.Add(editor);
            editor.Focus();
            editor.SelectAll();
        }

        private void InlineStripNameEditor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                EndInlineStripNameEdit(commit: true);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                EndInlineStripNameEdit(commit: false);
            }
        }

        private void InlineStripNameEditor_LostFocus(object? sender, EventArgs e)
        {
            EndInlineStripNameEdit(commit: true);
        }

        private void DeviceGrid_EndInlineEditOnLayoutChange(object? sender, ScrollEventArgs e)
        {
            EndInlineStripNameEdit(commit: true);
        }

        private void DeviceGrid_EndInlineEditOnColumnWidthChanged(object? sender, DataGridViewColumnEventArgs e)
        {
            EndInlineStripNameEdit(commit: true);
        }

        private void DeviceGrid_EndInlineEditOnRowHeightChanged(object? sender, DataGridViewRowEventArgs e)
        {
            EndInlineStripNameEdit(commit: true);
        }

        private void DeviceGrid_EndInlineEditOnResize(object? sender, EventArgs e)
        {
            EndInlineStripNameEdit(commit: true);
        }

        private void EndInlineStripNameEdit(bool commit)
        {
            if (activeStripNameEditor == null)
            {
                return;
            }

            var editor = activeStripNameEditor;
            var grid = activeStripNameGrid;
            var rowIndex = activeStripNameRowIndex;
            var stripKey = activeStripNameKey;
            var defaultName = activeStripDefaultName ?? string.Empty;

            activeStripNameEditor = null;
            activeStripNameGrid = null;
            activeStripNameRowIndex = -1;
            activeStripNameColumnIndex = -1;
            activeStripNameKey = null;
            activeStripDefaultName = null;

            editor.KeyDown -= InlineStripNameEditor_KeyDown;
            editor.LostFocus -= InlineStripNameEditor_LostFocus;

            if (commit && !string.IsNullOrWhiteSpace(stripKey))
            {
                var trimmed = editor.Text.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || string.Equals(trimmed, defaultName, StringComparison.Ordinal))
                {
                    customStripNames.Remove(stripKey);
                }
                else
                {
                    customStripNames[stripKey] = trimmed;
                }

                SaveStripNames();
            }

            if (grid != null && !grid.IsDisposed)
            {
                grid.Controls.Remove(editor);
                if (rowIndex >= 0 && rowIndex < grid.Rows.Count)
                {
                    grid.InvalidateRow(rowIndex);
                }
                grid.Refresh();
            }

            editor.Dispose();
        }

        private void RestoreWindowLayout()
        {
            var layout = WindowLayoutStore.Load(LayoutFilePath);
            if (layout == null)
            {
                return;
            }

            var bounds = new Rectangle(layout.Left, layout.Top, layout.Width, layout.Height);
            if (layout.Width > 200 && layout.Height > 200 && IsUsableOnAnyScreen(bounds))
            {
                StartPosition = FormStartPosition.Manual;
                DesktopBounds = bounds;
            }

            pendingSplitterRestore = false;
            foreach (var splitContainer in GetAllControls(this).OfType<SplitContainer>())
            {
                if (layout.SplitterDistances.TryGetValue(splitContainer.Name, out var distance))
                {
                    splitContainer.Tag = distance;
                    pendingSplitterRestore = true;
                }
            }

            if (layout.WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        private void ApplyPendingSplitterRestore()
        {
            pendingSplitterRestore = false;

            foreach (var splitContainer in GetAllControls(this).OfType<SplitContainer>())
            {
                if (splitContainer.Tag is int distance)
                {
                    TryApplySplitterDistance(splitContainer, distance);
                    splitContainer.Tag = null;
                }
            }
        }

        private void SaveWindowLayout()
        {
            if (IsDisposed)
            {
                return;
            }

            var bounds = WindowState == FormWindowState.Normal ? DesktopBounds : RestoreBounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            var layout = new WindowLayoutData
            {
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height,
                WindowState = WindowState == FormWindowState.Maximized
                    ? FormWindowState.Maximized
                    : FormWindowState.Normal
            };

            foreach (var splitContainer in GetAllControls(this).OfType<SplitContainer>())
            {
                layout.SplitterDistances[splitContainer.Name] = splitContainer.SplitterDistance;
            }

            WindowLayoutStore.Save(LayoutFilePath, layout);
        }

        private static bool IsUsableOnAnyScreen(Rectangle bounds)
        {
            return Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(bounds));
        }

        private static string FormatSceneTypeName(SceneType sceneType)
        {
            var raw = sceneType.ToString();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            var chars = new List<char>(raw.Length + 8);
            for (var index = 0; index < raw.Length; index++)
            {
                var current = raw[index];
                if (index > 0 && char.IsUpper(current) && !char.IsUpper(raw[index - 1]))
                {
                    chars.Add(' ');
                }

                chars.Add(current);
            }

            return new string(chars.ToArray());
        }

        private static void TryApplySplitterDistance(SplitContainer splitContainer, int distance)
        {
            try
            {
                var containerExtent = splitContainer.Orientation == Orientation.Vertical
                    ? splitContainer.Width
                    : splitContainer.Height;

                var maxDistance = containerExtent - splitContainer.Panel2MinSize - splitContainer.SplitterWidth;
                if (containerExtent <= 0 || maxDistance <= splitContainer.Panel1MinSize)
                {
                    return;
                }

                var clamped = Math.Max(splitContainer.Panel1MinSize, Math.Min(distance, maxDistance));
                splitContainer.SplitterDistance = clamped;
            }
            catch
            {
            }
        }

        private void TriggerOriginalAddSceneButton()
        {
            if (originalAddSceneButton == null)
            {
                return;
            }

            var onClick = typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
            onClick?.Invoke(originalAddSceneButton, new object[] { EventArgs.Empty });
        }

        private static string LayoutFilePath =>
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "window-layout.ini");

        private static string StripNamesFilePath =>
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "strip-names.ini");

        private static IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;

                foreach (var nested in GetAllControls(child))
                {
                    yield return nested;
                }
            }
        }

        private static T? FindControl<T>(Control parent, string name) where T : Control
        {
            foreach (var control in GetAllControls(parent).OfType<T>())
            {
                if (string.Equals(control.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return control;
                }
            }

            return null;
        }

        private static Button? FindButtonByText(Control parent, string text)
        {
            return GetAllControls(parent)
                .OfType<Button>()
                .FirstOrDefault(button => string.Equals(button.Text?.Trim(), text, StringComparison.OrdinalIgnoreCase));
        }
    }

    internal sealed class SceneAddDropDownButton : Button
    {
        private const int ArrowAreaWidth = 18;

        public ContextMenuStrip? DropDownMenu { get; set; }

        public void ShowDropDown()
        {
            DropDownMenu?.Show(this, new Point(0, Height));
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);

            if (mevent.Button == MouseButtons.Left)
            {
                ShowDropDown();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            var bounds = ClientRectangle;
            var arrowBounds = new Rectangle(bounds.Right - ArrowAreaWidth, bounds.Top, ArrowAreaWidth, bounds.Height);

            using (var pen = new Pen(SystemColors.ControlDark))
            {
                pevent.Graphics.DrawLine(pen, arrowBounds.Left, arrowBounds.Top + 4, arrowBounds.Left, arrowBounds.Bottom - 4);
            }

            var centerX = arrowBounds.Left + (arrowBounds.Width / 2);
            var centerY = arrowBounds.Top + (arrowBounds.Height / 2);
            var arrowColor = Enabled ? ForeColor : SystemColors.GrayText;

            using (var brush = new SolidBrush(arrowColor))
            {
                var points = new[]
                {
                    new Point(centerX - 4, centerY - 1),
                    new Point(centerX + 4, centerY - 1),
                    new Point(centerX, centerY + 3)
                };
                pevent.Graphics.FillPolygon(brush, points);
            }
        }
    }

    internal sealed class WindowLayoutData
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public FormWindowState WindowState { get; set; } = FormWindowState.Normal;
        public Dictionary<string, int> SplitterDistances { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    }

    internal static class WindowLayoutStore
    {
        public static WindowLayoutData? Load(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            var layout = new WindowLayoutData();

            foreach (var rawLine in System.IO.File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line.Substring(0, separatorIndex).Trim();
                var value = line.Substring(separatorIndex + 1).Trim();

                switch (key)
                {
                    case "left":
                        int.TryParse(value, out var left);
                        layout.Left = left;
                        break;
                    case "top":
                        int.TryParse(value, out var top);
                        layout.Top = top;
                        break;
                    case "width":
                        int.TryParse(value, out var width);
                        layout.Width = width;
                        break;
                    case "height":
                        int.TryParse(value, out var height);
                        layout.Height = height;
                        break;
                    case "windowState":
                        if (Enum.TryParse(value, true, out FormWindowState state))
                        {
                            layout.WindowState = state;
                        }
                        break;
                    default:
                        if (key.StartsWith("splitter.", StringComparison.OrdinalIgnoreCase)
                            && int.TryParse(value, out var distance))
                        {
                            layout.SplitterDistances[key.Substring("splitter.".Length)] = distance;
                        }
                        break;
                }
            }

            return layout;
        }

        public static void Save(string path, WindowLayoutData layout)
        {
            var lines = new List<string>
            {
                $"left={layout.Left}",
                $"top={layout.Top}",
                $"width={layout.Width}",
                $"height={layout.Height}",
                $"windowState={layout.WindowState}"
            };

            foreach (var pair in layout.SplitterDistances.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
            {
                lines.Add($"splitter.{pair.Key}={pair.Value}");
            }

            System.IO.File.WriteAllLines(path, lines);
        }
    }

    internal static class StripNameStore
    {
        public static Dictionary<string, string> Load(string path)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!System.IO.File.Exists(path))
            {
                return result;
            }

            foreach (var rawLine in System.IO.File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = Uri.UnescapeDataString(line.Substring(0, separatorIndex));
                var value = Uri.UnescapeDataString(line.Substring(separatorIndex + 1));
                if (!string.IsNullOrWhiteSpace(key))
                {
                    result[key] = value;
                }
            }

            return result;
        }

        public static void Save(string path, IReadOnlyDictionary<string, string> names)
        {
            var lines = names
                .Where(pair => !string.IsNullOrWhiteSpace(pair.Key))
                .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value ?? string.Empty)}")
                .ToArray();

            System.IO.File.WriteAllLines(path, lines);
        }
    }

}
