namespace Ledqualizer
{
    public interface ISceneEditorForm
    {
        SceneType SceneType { get; }
        event EventHandler? SceneChanged;
        void LoadScene(SceneConfig scene);
    }
}
