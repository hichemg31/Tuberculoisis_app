using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using testWPF.Models;
using testWPF.Services;

namespace testWPF.ViewModels;

/// <summary>
/// Main ViewModel for the Tuberculosis Case Declaration Registry.
/// Manages the case list, CRUD operations, search/filter, form state,
/// and lung annotation coordination.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly RegistryService _registryService;
    private readonly LungImageService _lungImageService;

    public LungImageService LungImageService => _lungImageService;

    // ── Collections ──────────────────────────────────────────────────
    public ObservableCollection<TuberculosisCase> Cases { get; }
    public ICollectionView CasesView { get; }

    public List<string> SexeOptions { get; } = new() { "H", "F" };
    public List<string> TpTepOptions { get; } = new() { "TP", "TEP" };
    public List<string> RegimeTraitementOptions { get; } = new()
    {
        "2RHZE / 4RH",
        "2RHZ / 4RH",
        "2SRHZE / RHZE / 5RHE",
        "2RHZped / 4RHped"
    };

    // ── Selected item ────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCaseCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCaseCommand))]
    private TuberculosisCase? _selectedCase;

    // ── Search ───────────────────────────────────────────────────────
    [ObservableProperty]
    private string _searchText = string.Empty;

    partial void OnSearchTextChanged(string value) => CasesView.Refresh();

    // ── Form state ───────────────────────────────────────────────────
    [ObservableProperty]
    private bool _isFormVisible;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _formTitle = "Nouveau Cas";

    /// <summary>
    /// The case object currently being edited in the form panel.
    /// The form fields bind directly to this object's properties.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCaseCommand))]
    private TuberculosisCase _editingCase = new();

    private int _editingCaseId;

    // ── Summary stats ────────────────────────────────────────────────
    public int TotalCases => Cases.Count;
    public int NewCases => Cases.Count(c => c.TypeN);
    public int RelapseCases => Cases.Count(c => c.TypeR);
    public int CuredCases => Cases.Count(c => !string.IsNullOrWhiteSpace(c.GuerisTraitTermine));

    // ── Lung Annotation Events ───────────────────────────────────────
    /// <summary>
    /// Fired when the doctor clicks the button to open the lung annotation window.
    /// </summary>
    public event Action<TuberculosisCase>? OpenLungAnnotationRequested;

    // ── Constructor ──────────────────────────────────────────────────
    public MainViewModel()
    {
        _registryService = new RegistryService();
        _lungImageService = new LungImageService();
        Cases = _registryService.GetAll();
        Cases.CollectionChanged += (_, _) => RefreshStats();

        CasesView = CollectionViewSource.GetDefaultView(Cases);
        CasesView.Filter = FilterCases;
    }

    // ── Filter logic ─────────────────────────────────────────────────
    private bool FilterCases(object obj)
    {
        if (obj is not TuberculosisCase tbCase) return false;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim().ToLowerInvariant();
            if (!tbCase.NomPrenom.Contains(search, StringComparison.OrdinalIgnoreCase) &&
                !tbCase.AdresseComplete.Contains(search, StringComparison.OrdinalIgnoreCase) &&
                !tbCase.LieuDebutTraitement.Contains(search, StringComparison.OrdinalIgnoreCase) &&
                !(tbCase.NumeroOrdre?.ToString().Contains(search) ?? false))
            {
                return false;
            }
        }

        return true;
    }

    // ── Commands ─────────────────────────────────────────────────────

    [RelayCommand]
    private void AddCase()
    {
        IsEditMode = false;
        FormTitle = "Nouveau Cas";
        EditingCase = new TuberculosisCase
        {
            NumeroOrdre = _registryService.GetNextNumeroOrdre(),
            DateEnregistrement = DateTime.Today,
            Sexe = "H"
        };
        IsFormVisible = true;
    }

    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private void EditCase()
    {
        if (SelectedCase == null) return;

        IsEditMode = true;
        FormTitle = "Modifier le Cas";
        _editingCaseId = SelectedCase.Id;
        EditingCase = SelectedCase.Clone();
        IsFormVisible = true;
    }

    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private void DeleteCase()
    {
        if (SelectedCase == null) return;

        var result = MessageBox.Show(
            $"Êtes-vous sûr de vouloir supprimer le cas N°{SelectedCase.NumeroOrdre} — {SelectedCase.NomPrenom} ?",
            "Confirmer la suppression",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            _registryService.Delete(SelectedCase.Id);
            Cases.Remove(SelectedCase);
            SelectedCase = null;
        }
    }

    private bool CanEditOrDelete() => SelectedCase != null;

    [RelayCommand]
    private void OpenLungAnnotation()
    {
        if (EditingCase != null)
        {
            OpenLungAnnotationRequested?.Invoke(EditingCase);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void SaveCase()
    {
        if (IsEditMode)
        {
            var existing = Cases.FirstOrDefault(c => c.Id == _editingCaseId);
            if (existing != null)
            {
                existing.CopyFrom(EditingCase);
                _registryService.Update(existing);
            }
        }
        else
        {
            _registryService.Add(EditingCase);
            Cases.Add(EditingCase);
        }

        IsFormVisible = false;
        CasesView.Refresh();
        RefreshStats();
    }

    private bool CanSave() => EditingCase != null;

    [RelayCommand]
    private void CancelForm()
    {
        IsFormVisible = false;
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
    }

    // ── Helpers ───────────────────────────────────────────────────────
    private void RefreshStats()
    {
        OnPropertyChanged(nameof(TotalCases));
        OnPropertyChanged(nameof(NewCases));
        OnPropertyChanged(nameof(RelapseCases));
        OnPropertyChanged(nameof(CuredCases));
    }
}
