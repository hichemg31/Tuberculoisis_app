using CommunityToolkit.Mvvm.ComponentModel;

namespace testWPF.Models;

/// <summary>
/// Represents a single case entry in the Tuberculosis Case Declaration Registry.
/// Columns (1) through (32) match the official paper form.
/// </summary>
public partial class TuberculosisCase : ObservableObject
{
    // ── Primary Key ──────────────────────────────────────────────────
    [ObservableProperty] private int _id;

    // ── Identification (1–6) ─────────────────────────────────────────
    /// <summary>(1) Numéro d'ordre</summary>
    [ObservableProperty] private int? _numeroOrdre;

    /// <summary>(2) Date d'enregistrement</summary>
    [ObservableProperty] private DateTime? _dateEnregistrement;

    /// <summary>(3) Nom et Prénom</summary>
    [ObservableProperty] private string _nomPrenom = string.Empty;

    /// <summary>(4) Sexe H/F</summary>
    [ObservableProperty] private string _sexe = string.Empty;

    /// <summary>(5) Age</summary>
    [ObservableProperty] private int? _age;

    /// <summary>(6) Adresse complète</summary>
    [ObservableProperty] private string _adresseComplete = string.Empty;

    // ── Traitement (7–9) ─────────────────────────────────────────────
    /// <summary>(7) Date du début du traitement</summary>
    [ObservableProperty] private DateTime? _dateDebutTraitement;

    /// <summary>(8) Lieu du début du traitement</summary>
    [ObservableProperty] private string _lieuDebutTraitement = string.Empty;

    /// <summary>(9) Régime de traitement</summary>
    [ObservableProperty] private string _regimeTraitement = string.Empty;

    // ── Classification (10) ──────────────────────────────────────────
    /// <summary>(10) TP ou TEP – Localisation / preuve</summary>
    [ObservableProperty] private string _tpOuTep = string.Empty;

    // ── Type des malades (11–16) ─────────────────────────────────────
    /// <summary>(11) N – Nouveau cas</summary>
    [ObservableProperty] private bool _typeN;

    /// <summary>(12) R – Rechute</summary>
    [ObservableProperty] private bool _typeR;

    /// <summary>(13) E – Échec</summary>
    [ObservableProperty] private bool _typeE;

    /// <summary>(14) REP – Reprise après abandon</summary>
    [ObservableProperty] private bool _typeREP;

    /// <summary>(15) T – Transfert</summary>
    [ObservableProperty] private bool _typeT;

    /// <summary>(16) A – Autre</summary>
    [ObservableProperty] private bool _typeA;

    // ── Résultats des examens microscopiques (17–25) ─────────────────
    /// <summary>(17) Pré-traitement – Frottis</summary>
    [ObservableProperty] private string _preTraitFrottis = string.Empty;

    /// <summary>(18) Pré-traitement – Culture</summary>
    [ObservableProperty] private string _preTraitCult = string.Empty;

    /// <summary>(19) Fin du 2ème mois (nouv. cas) / 3ème mois (retraité) – Frottis</summary>
    [ObservableProperty] private string _fin2eMoisFrottis = string.Empty;

    /// <summary>(20) Fin du 2ème mois (nouv. cas) / 3ème mois (retraité) – Culture</summary>
    [ObservableProperty] private string _fin2eMoisCult = string.Empty;

    /// <summary>(21) Fin du 4ème ou 5ème mois – Frottis</summary>
    [ObservableProperty] private string _fin4eMoisFrottis = string.Empty;

    /// <summary>(22) Fin du 4ème ou 5ème mois – Culture</summary>
    [ObservableProperty] private string _fin4eMoisCult = string.Empty;

    /// <summary>(23) 6ème mois ou 8ème mois – Frottis</summary>
    [ObservableProperty] private string _mois6ou8Frottis = string.Empty;

    /// <summary>(24) 6ème mois ou 8ème mois – Culture</summary>
    [ObservableProperty] private string _mois6ou8Cult = string.Empty;

    /// <summary>(25) Au-delà du 6ème ou 8ème mois – Frottis</summary>
    [ObservableProperty] private string _auDelaFrottis = string.Empty;

    // ── Résultat du traitement / Date d'arrêt (26–31) ────────────────
    /// <summary>(26) Guéris / Traitement terminé</summary>
    [ObservableProperty] private string _guerisTraitTermine = string.Empty;

    /// <summary>(27) Pas d'examen bactériologique</summary>
    [ObservableProperty] private string _pasExamenBacteriol = string.Empty;

    /// <summary>(28) Décédé</summary>
    [ObservableProperty] private string _decede = string.Empty;

    /// <summary>(29) Échec</summary>
    [ObservableProperty] private string _echec = string.Empty;

    /// <summary>(30) Perdu de vue</summary>
    [ObservableProperty] private string _perduDeVue = string.Empty;

    /// <summary>(31) Transféré</summary>
    [ObservableProperty] private string _transfere = string.Empty;

    // ── Observation (32) ─────────────────────────────────────────────
    /// <summary>(32) Observation</summary>
    [ObservableProperty] private string _observation = string.Empty;

    // ── Schéma pulmonaire ────────────────────────────────────────────
    /// <summary>
    /// Composite lung image (original + annotations) stored as PNG bytes.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasLungAnnotation))]
    private byte[]? _lungDrawing;

    /// <summary>
    /// JSON-serialized stroke data for re-editing annotations.
    /// Contains normalized coordinates, colors, and thicknesses.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasLungAnnotation))]
    private string? _annotationsJson;

    /// <summary>
    /// Indicates whether this case contains custom lung drawing annotations.
    /// </summary>
    public bool HasLungAnnotation => !string.IsNullOrWhiteSpace(AnnotationsJson) || (LungDrawing != null && LungDrawing.Length > 0);

    // ── Helpers ──────────────────────────────────────────────────────

    public TuberculosisCase Clone()
    {
        return new TuberculosisCase
        {
            Id = Id,
            NumeroOrdre = NumeroOrdre,
            DateEnregistrement = DateEnregistrement,
            NomPrenom = NomPrenom,
            Sexe = Sexe,
            Age = Age,
            AdresseComplete = AdresseComplete,
            DateDebutTraitement = DateDebutTraitement,
            LieuDebutTraitement = LieuDebutTraitement,
            RegimeTraitement = RegimeTraitement,
            TpOuTep = TpOuTep,
            TypeN = TypeN,
            TypeR = TypeR,
            TypeE = TypeE,
            TypeREP = TypeREP,
            TypeT = TypeT,
            TypeA = TypeA,
            PreTraitFrottis = PreTraitFrottis,
            PreTraitCult = PreTraitCult,
            Fin2eMoisFrottis = Fin2eMoisFrottis,
            Fin2eMoisCult = Fin2eMoisCult,
            Fin4eMoisFrottis = Fin4eMoisFrottis,
            Fin4eMoisCult = Fin4eMoisCult,
            Mois6ou8Frottis = Mois6ou8Frottis,
            Mois6ou8Cult = Mois6ou8Cult,
            AuDelaFrottis = AuDelaFrottis,
            GuerisTraitTermine = GuerisTraitTermine,
            PasExamenBacteriol = PasExamenBacteriol,
            Decede = Decede,
            Echec = Echec,
            PerduDeVue = PerduDeVue,
            Transfere = Transfere,
            Observation = Observation,

            LungDrawing = LungDrawing?.ToArray(),
            AnnotationsJson = AnnotationsJson
        };
    }

    public void CopyFrom(TuberculosisCase other)
    {
        NumeroOrdre = other.NumeroOrdre;
        DateEnregistrement = other.DateEnregistrement;
        NomPrenom = other.NomPrenom;
        Sexe = other.Sexe;
        Age = other.Age;
        AdresseComplete = other.AdresseComplete;
        DateDebutTraitement = other.DateDebutTraitement;
        LieuDebutTraitement = other.LieuDebutTraitement;
        RegimeTraitement = other.RegimeTraitement;
        TpOuTep = other.TpOuTep;
        TypeN = other.TypeN;
        TypeR = other.TypeR;
        TypeE = other.TypeE;
        TypeREP = other.TypeREP;
        TypeT = other.TypeT;
        TypeA = other.TypeA;
        PreTraitFrottis = other.PreTraitFrottis;
        PreTraitCult = other.PreTraitCult;
        Fin2eMoisFrottis = other.Fin2eMoisFrottis;
        Fin2eMoisCult = other.Fin2eMoisCult;
        Fin4eMoisFrottis = other.Fin4eMoisFrottis;
        Fin4eMoisCult = other.Fin4eMoisCult;
        Mois6ou8Frottis = other.Mois6ou8Frottis;
        Mois6ou8Cult = other.Mois6ou8Cult;
        AuDelaFrottis = other.AuDelaFrottis;
        GuerisTraitTermine = other.GuerisTraitTermine;
        PasExamenBacteriol = other.PasExamenBacteriol;
        Decede = other.Decede;
        Echec = other.Echec;
        PerduDeVue = other.PerduDeVue;
        Transfere = other.Transfere;
        Observation = other.Observation;

        LungDrawing = other.LungDrawing?.ToArray();
        AnnotationsJson = other.AnnotationsJson;
    }
}
