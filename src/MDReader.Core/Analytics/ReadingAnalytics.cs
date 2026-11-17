namespace MDReader.Core.Analytics;

/// <summary>
/// Detailed document reading metrics including words, characters, speech time, and readability.
/// </summary>
public sealed class ReadingAnalytics
{
    public int WordCount { get; set; }
    public int CharacterCountWithSpaces { get; set; }
    public int CharacterCountWithoutSpaces { get; set; }
    public int LineCount { get; set; }
    public int ParagraphCount { get; set; }
    public double SilentReadingMinutes { get; set; }
    public double SpeakingMinutes { get; set; }
    public string ReadabilityLevel { get; set; } = "Standard / متوسط";

    public string ReadingTimeDisplay => SilentReadingMinutes < 1.0 ? "< 1 min" : $"~{(int)System.Math.Ceiling(SilentReadingMinutes)} min";
    public string SpeakingTimeDisplay => SpeakingMinutes < 1.0 ? "< 1 min" : $"~{(int)System.Math.Ceiling(SpeakingMinutes)} min";
}
