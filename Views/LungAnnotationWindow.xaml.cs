using System.Windows;
using System.Windows.Threading;
using testWPF.Models;
using testWPF.Services;

namespace testWPF.Views;

/// <summary>
/// Interaction logic for LungAnnotationWindow.xaml
/// Modal dialog allowing the doctor to draw directly onto the lung diagram
/// and save the resulting annotations and composite image back to the patient case.
/// </summary>
public partial class LungAnnotationWindow : Window
{
    private readonly LungImageService _lungImageService;
    private readonly string? _initialAnnotationsJson;

    public string? ResultAnnotationsJson { get; private set; }
    public byte[]? ResultLungDrawing { get; private set; }

    public LungAnnotationWindow(TuberculosisCase tbCase, LungImageService lungImageService)
    {
        InitializeComponent();
        _lungImageService = lungImageService;
        _initialAnnotationsJson = tbCase.AnnotationsJson;

        // Display patient info header
        var patientName = string.IsNullOrWhiteSpace(tbCase.NomPrenom) ? "Nouveau patient" : tbCase.NomPrenom;
        var caseNum = tbCase.NumeroOrdre.HasValue ? $"N° {tbCase.NumeroOrdre}" : "Nouveau cas";
        PatientInfoText.Text = $"Cas {caseNum} — {patientName}";

        Loaded += OnWindowLoaded;
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_initialAnnotationsJson))
        {
            return;
        }

        // Defer loading until control layout and rendered image size are ready
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
        {
            var (width, height) = DrawingControl.GetRenderedImageSize();
            if (width <= 0 || height <= 0) return;

            var strokeData = _lungImageService.DeserializeStrokes(_initialAnnotationsJson);
            if (strokeData != null && strokeData.Strokes.Count > 0)
            {
                var strokes = _lungImageService.StrokeDataToStrokes(strokeData, width, height);
                DrawingControl.LoadStrokes(strokes);
            }
        });
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var (width, height) = DrawingControl.GetRenderedImageSize();
        var inkCanvas = DrawingControl.GetInkCanvas();

        if (inkCanvas.Strokes.Count > 0 && width > 0 && height > 0)
        {
            var strokeData = _lungImageService.StrokesToStrokeData(inkCanvas.Strokes, width, height);
            ResultAnnotationsJson = _lungImageService.SerializeStrokes(strokeData);

            var defaultImage = _lungImageService.LoadDefaultImage();
            ResultLungDrawing = _lungImageService.RenderCompositeImage(defaultImage, strokeData);
        }
        else
        {
            ResultAnnotationsJson = null;
            ResultLungDrawing = null;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
