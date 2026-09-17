using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace testWPF.Controls;

/// <summary>
/// Drawing control for lung annotations. Provides freehand drawing with
/// undo/redo, pen size/color selection, and eraser mode.
/// </summary>
public partial class LungDrawingControl : UserControl
{
    private readonly Stack<Stroke> _undoStack = new();
    private readonly Stack<Stroke> _redoStack = new();
    private bool _suppressStrokeEvent;

    public LungDrawingControl()
    {
        InitializeComponent();
        SetDefaultDrawingAttributes();

        // Keep InkCanvas sized to match the rendered image
        LungImage.SizeChanged += (_, _) => SyncCanvasSize();
        Loaded += (_, _) => SyncCanvasSize();
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>
    /// Returns the InkCanvas for the service to read strokes.
    /// </summary>
    public InkCanvas GetInkCanvas() => LungInkCanvas;

    /// <summary>
    /// Returns the actual rendered size of the lung image in the control.
    /// Used for coordinate normalization.
    /// </summary>
    public (double Width, double Height) GetRenderedImageSize()
    {
        return (LungImage.ActualWidth, LungImage.ActualHeight);
    }

    /// <summary>
    /// Loads strokes into the InkCanvas for editing.
    /// </summary>
    public void LoadStrokes(StrokeCollection strokes)
    {
        _suppressStrokeEvent = true;
        LungInkCanvas.Strokes.Clear();
        _undoStack.Clear();
        _redoStack.Clear();

        foreach (var stroke in strokes)
        {
            LungInkCanvas.Strokes.Add(stroke);
        }
        _suppressStrokeEvent = false;
    }

    /// <summary>
    /// Clears all strokes and resets undo/redo stacks.
    /// Resets the image to the default lung diagram.
    /// </summary>
    public void ClearAll()
    {
        _suppressStrokeEvent = true;
        LungInkCanvas.Strokes.Clear();
        _undoStack.Clear();
        _redoStack.Clear();
        LungImage.Source = new BitmapImage(
            new Uri("pack://application:,,,/Assets/Lung_Labeled_Diagram2.jpg", UriKind.Absolute));
        _suppressStrokeEvent = false;
    }

    /// <summary>
    /// Returns true if there are any strokes on the canvas.
    /// </summary>
    public bool HasStrokes => LungInkCanvas.Strokes.Count > 0;

    // ── InkCanvas Sizing ────────────────────────────────────────────────

    /// <summary>
    /// Syncs the InkCanvas size to match the rendered image size so
    /// strokes align perfectly with the image.
    /// </summary>
    public void SyncCanvasSize()
    {
        if (LungImage.ActualWidth > 0 && LungImage.ActualHeight > 0)
        {
            LungInkCanvas.Width = LungImage.ActualWidth;
            LungInkCanvas.Height = LungImage.ActualHeight;
        }
    }

    // ── Drawing Attributes ──────────────────────────────────────────────

    private void SetDefaultDrawingAttributes()
    {
        LungInkCanvas.DefaultDrawingAttributes = new DrawingAttributes
        {
            Color = Colors.Red,
            Width = 3,
            Height = 3,
            StylusTip = StylusTip.Ellipse,
            FitToCurve = true,
            IsHighlighter = false
        };
    }

    // ── Event Handlers ──────────────────────────────────────────────────

    private void DrawMode_Checked(object sender, RoutedEventArgs e)
    {
        if (LungInkCanvas != null)
        {
            LungInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }
    }

    private void EraseMode_Checked(object sender, RoutedEventArgs e)
    {
        if (LungInkCanvas != null)
        {
            LungInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }
    }

    private void UndoButton_Click(object sender, RoutedEventArgs e)
    {
        if (LungInkCanvas.Strokes.Count > 0)
        {
            var lastStroke = LungInkCanvas.Strokes[^1];
            _redoStack.Push(lastStroke);

            _suppressStrokeEvent = true;
            LungInkCanvas.Strokes.Remove(lastStroke);
            _suppressStrokeEvent = false;
        }
    }

    private void RedoButton_Click(object sender, RoutedEventArgs e)
    {
        if (_redoStack.Count > 0)
        {
            var stroke = _redoStack.Pop();
            _suppressStrokeEvent = true;
            LungInkCanvas.Strokes.Add(stroke);
            _suppressStrokeEvent = false;
            _undoStack.Push(stroke);
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        if (LungInkCanvas.Strokes.Count == 0) return;

        // Push all current strokes to undo stack so clear can be undone
        foreach (var stroke in LungInkCanvas.Strokes)
        {
            _undoStack.Push(stroke);
        }

        _suppressStrokeEvent = true;
        LungInkCanvas.Strokes.Clear();
        _suppressStrokeEvent = false;

        _redoStack.Clear();
    }

    private void PenSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LungInkCanvas == null) return;
        if (PenSizeCombo?.SelectedItem is ComboBoxItem item)
        {
            var content = item.Content?.ToString()?.Replace(" px", "").Trim();
            if (double.TryParse(content, out var size))
            {
                LungInkCanvas.DefaultDrawingAttributes.Width = size;
                LungInkCanvas.DefaultDrawingAttributes.Height = size;
            }
        }
    }

    private void ColorButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button btn && btn.Tag is string colorStr)
        {
            var color = (Color)ColorConverter.ConvertFromString(colorStr);
            LungInkCanvas.DefaultDrawingAttributes.Color = color;
            if (DrawRadio != null)
            {
                DrawRadio.IsChecked = true;
            }
            LungInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }
    }

    /// <summary>
    /// Track new strokes for undo and clear the redo stack.
    /// </summary>
    private void LungInkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
    {
        if (!_suppressStrokeEvent)
        {
            _undoStack.Push(e.Stroke);
            _redoStack.Clear();
        }
    }
}