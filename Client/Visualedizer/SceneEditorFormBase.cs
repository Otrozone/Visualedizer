namespace Ledqualizer
{
    internal interface ISceneEditorForm
    {
        SceneType SceneType { get; }
        event EventHandler? SceneChanged;
        void LoadScene(SceneConfig scene);
    }

    internal abstract class SceneEditorFormBase : Form, ISceneEditorForm
    {
        private bool isLoading;

        public abstract SceneType SceneType { get; }

        protected SceneConfig? CurrentScene { get; private set; }

        public event EventHandler? SceneChanged;

        protected SceneEditorFormBase()
        {
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
        }

        public void LoadScene(SceneConfig scene)
        {
            CurrentScene = scene;
            isLoading = true;
            try
            {
                OnLoadScene(scene);
            }
            finally
            {
                isLoading = false;
            }
        }

        protected bool IsLoadingScene => isLoading;

        protected void NotifySceneChanged()
        {
            if (!isLoading)
            {
                SceneChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected abstract void OnLoadScene(SceneConfig scene);
    }
}
