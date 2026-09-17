using System.Text.Json.Serialization;

namespace testWPF.Models;

/// <summary>
/// Root object for JSON serialization of annotation strokes.
/// </summary>
public class StrokeData
{
    [JsonPropertyName("strokes")]
    public List<StrokeEntry> Strokes { get; set; } = new();
}

/// <summary>
/// Represents a single drawing stroke with its visual properties.
/// Points are normalized to 0.0–1.0 relative to the original image dimensions.
/// </summary>
public class StrokeEntry
{
    [JsonPropertyName("points")]
    public List<PointData> Points { get; set; } = new();

    [JsonPropertyName("color")]
    public string Color { get; set; } = "#FFFF0000"; // Default red

    [JsonPropertyName("thickness")]
    public double Thickness { get; set; } = 3.0;
}

/// <summary>
/// A single point in a stroke, with normalized coordinates (0.0–1.0).
/// </summary>
public class PointData
{
    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }
}
