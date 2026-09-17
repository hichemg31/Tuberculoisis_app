using System.Windows;
using testWPF.Models;
using testWPF.ViewModels;

namespace testWPF.Views;

/// <summary>
/// Main window code-behind. Handles opening modal dialogs
/// when requested by the ViewModel.
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        // Hook up DataContext immediately if already initialized from XAML
        if (DataContext is MainViewModel vm)
        {
            SetViewModel(vm);
        }

        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is MainViewModel newVm)
        {
            SetViewModel(newVm);
        }
        else if (e.OldValue is MainViewModel)
        {
            SetViewModel(null);
        }
    }

    private void SetViewModel(MainViewModel? vm)
    {
        if (_viewModel != null)
        {
            _viewModel.OpenLungAnnotationRequested -= OnOpenLungAnnotationRequested;
        }

        _viewModel = vm;

        if (_viewModel != null)
        {
            _viewModel.OpenLungAnnotationRequested += OnOpenLungAnnotationRequested;
        }
    }

    /// <summary>
    /// Opens the dedicated modal LungAnnotationWindow centered on top of MainWindow.
    /// </summary>
    private void OnOpenLungAnnotationRequested(TuberculosisCase editingCase)
    {
        if (_viewModel == null) return;

        var dialog = new LungAnnotationWindow(editingCase, _viewModel.LungImageService)
        {
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (dialog.ShowDialog() == true)
        {
            editingCase.AnnotationsJson = dialog.ResultAnnotationsJson;
            editingCase.LungDrawing = dialog.ResultLungDrawing;
        }
    }
}
