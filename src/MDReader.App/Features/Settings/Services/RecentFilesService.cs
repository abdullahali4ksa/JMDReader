namespace MDReader.App.Features.Settings.Services;

public sealed class RecentFilesService
{
    private readonly List<string> _paths = new();
    public int MaxCount => 10;

    public IReadOnlyList<string> RecentPaths => _paths.AsReadOnly();

    public void Add(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        _paths.Remove(path);
        _paths.Insert(0, path);
        if (_paths.Count > MaxCount) _paths.RemoveRange(MaxCount, _paths.Count - MaxCount);
    }

    public void Clear() => _paths.Clear();
}
