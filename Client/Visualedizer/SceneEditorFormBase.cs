namespace Ledqualizer
{
    public interface ISceneEditorForm
    {
        SceneType SceneType { get; }
        event EventHandler? SceneChanged;
        void LoadScene(SceneConfig scene);
    }

    public class SceneEditorFormBase : Form, ISceneEditorForm
    {
        private bool isLoading;

        public virtual SceneType SceneType => SceneType.SolidColor;

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

        protected virtual void OnLoadScene(SceneConfig scene)
        {
        }
    }
}
