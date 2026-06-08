namespace Ledqualizer
{
    public sealed class StrobeTestRequestedEventArgs : EventArgs
    {
        public StrobeTestRequestedEventArgs(string sceneId)
        {
            SceneId = sceneId;
        }

        public string SceneId { get; }
    }

    public sealed class StrobeLiveSceneEditorForm : Form, ISceneEditorForm
    {
        private readonly Button btnToggleTest = new();

        public StrobeLiveSceneEditorForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;

            TableLayoutPanel root = new()
            {
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                RowCount = 2
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "This scene enables strobe when assigned and active on a device, and disables it when the scene or device is deactivated."
            }, 0, 0);

            btnToggleTest.AutoSize = true;
            btnToggleTest.Text = "Toggle Test";
            btnToggleTest.Click += btnToggleTest_Click;
            root.Controls.Add(btnToggleTest, 0, 1);

            Controls.Add(root);
        }

        public event EventHandler? SceneChanged;
        public event EventHandler<StrobeTestRequestedEventArgs>? ToggleTestRequested;

        public SceneType SceneType => SceneType.StrobeLive;

        public SceneConfig? CurrentScene { get; private set; }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
        }

        private void btnToggleTest_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CurrentScene?.Id))
            {
                ToggleTestRequested?.Invoke(this, new StrobeTestRequestedEventArgs(CurrentScene.Id));
            }
        }
    }
}
