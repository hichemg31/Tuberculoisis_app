using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using testWPF.Models;

namespace testWPF.Services;

/// <summary>
/// Service for lung annotation rendering, serialization, and image compositing.
/// All image/stroke operations go through here — the View and ViewModel never
/// directly handle rendering or JSON serialization.
/// </summary>
public class LungImageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Loads the default lung diagram from application assets.
    /// </summary>
    public BitmapImage LoadDefaultImage()
    {
        var uri = new Uri("pack://application:,,,/Assets/Lung_Labeled_Diagram2.jpg", UriKind.Absolute);
        var image = new BitmapImage();
        image.BeginInit();
        image.UriSource = uri;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        image.Freeze();
        return image;
    }

    /// <summary>
    /// Converts InkCanvas strokes to normalized StrokeData for JSON storage.
    /// Coordinates are normalized to 0.0–1.0 relative to the provided canvas dimensions.
    /// </summary>
    public StrokeData StrokesToStrokeData(StrokeCollection strokes, double canvasWidth, double canvasHeight)
    {
        var data = new StrokeData();

        foreach (var stroke in strokes)
        {
            var entry = new StrokeEntry
            {
                Color = stroke.DrawingAttributes.Color.ToString(),
                Thickness = stroke.DrawingAttributes.Width
            };

            foreach (var point in stroke.StylusPoints)
            {
                entry.Points.Add(new PointData
                {
                    X = canvasWidth > 0 ? point.X / canvasWidth : 0,
                    Y = canvasHeight > 0 ? point.Y / canvasHeight : 0
                });
            }

            data.Strokes.Add(entry);
        }

        return data;
    }

    /// <summary>
    /// Converts StrokeData back to a WPF StrokeCollection, scaling normalized
    /// coordinates to the provided canvas dimensions.
    /// </summary>
    public StrokeCollection StrokeDataToStrokes(StrokeData data, double canvasWidth, double canvasHeight)
    {
        var strokes = new StrokeCollection();

        foreach (var entry in data.Strokes)
        {
            if (entry.Points.Count == 0) continue;

            var points = new StylusPointCollection();
            foreach (var point in entry.Points)
            {
                points.Add(new StylusPoint(
                    point.X * canvasWidth,
                    point.Y * canvasHeight));
            }

            var color = (Color)ColorConverter.ConvertFromString(entry.Color);
            var drawingAttributes = new DrawingAttributes
            {
                Color = color,
                Width = entry.Thickness,
                Height = entry.Thickness,
                StylusTip = StylusTip.Ellipse,
                FitToCurve = true,
                IsHighlighter = false
            };

            var stroke = new Stroke(points, drawingAttributes);
            strokes.Add(stroke);
        }

        return strokes;
    }

    /// <summary>
    /// Serializes StrokeData to a JSON string.
    /// </summary>
    public string SerializeStrokes(StrokeData data)
    {
        return JsonSerializer.Serialize(data, JsonOptions);
    }

    /// <summary>
    /// Deserializes a JSON string to StrokeData. Returns null on failure.
    /// </summary>
    public StrokeData? DeserializeStrokes(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        try
        {
            return JsonSerializer.Deserialize<StrokeData>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Renders the original lung image with annotation strokes overlaid
    /// at the original image resolution. Returns the composite as PNG bytes.
    /// </summary>
    public byte[] RenderCompositeImage(BitmapSource originalImage, StrokeData strokeData)
    {
        int pixelWidth = originalImage.PixelWidth;
        int pixelHeight = originalImage.PixelHeight;

        // Scale strokes from normalized (0–1) to original image pixels
        var scaledStrokes = StrokeDataToStrokes(strokeData, pixelWidth, pixelHeight);

        // Draw everything onto a DrawingVisual at original resolution
        var drawingVisual = new DrawingVisual();
        using (var dc = drawingVisual.RenderOpen())
        {
            // Draw the original lung image
            dc.DrawImage(originalImage, new Rect(0, 0, pixelWidth, pixelHeight));

            // Draw each stroke
            foreach (var stroke in scaledStrokes)
            {
                var geometry = stroke.GetGeometry();
                var brush = new SolidColorBrush(stroke.DrawingAttributes.Color);
                brush.Freeze();

                var pen = new Pen(brush, stroke.DrawingAttributes.Width)
                {
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round,
                    LineJoin = PenLineJoin.Round
                };
                pen.Freeze();

                dc.DrawGeometry(brush, pen, geometry);
            }
        }

        // Render to bitmap at original image resolution
        var renderBitmap = new RenderTargetBitmap(
            pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32);
        renderBitmap.Render(drawingVisual);

        // Encode as PNG
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

        using var stream = new MemoryStream();
        encoder.Save(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Converts a byte array (PNG) to a BitmapImage for display.
    /// Returns null if the data is null or empty.
    /// </summary>
    public BitmapImage? BytesToBitmap(byte[]? data)
    {
        if (data == null || data.Length == 0) return null;

        try
        {
            var image = new BitmapImage();
            using var stream = new MemoryStream(data);
            image.BeginInit();
            image.StreamSource = stream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch
        {
            return null;
        }
    }
}
