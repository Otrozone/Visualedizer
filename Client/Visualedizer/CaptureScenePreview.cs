namespace Ledqualizer
{
    internal sealed class CaptureScenePreview
    {
        public string SceneId { get; init; } = string.Empty;
        public IReadOnlyList<Color> Colors { get; init; } = Array.Empty<Color>();
        public string? SourcePath { get; init; }
        public Size SourceSize { get; init; }
        public int SampleIndex { get; init; }
        public ImageScanDirection Direction { get; init; }
    }
}
