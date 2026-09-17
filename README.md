# Registre de Déclaration des Cas de Tuberculose

Application de bureau médicale (WPF / .NET 8) pour l'enregistrement, le suivi et la déclaration des cas de tuberculose selon les 32 colonnes officielles, avec module d'annotation anatomique des poumons.

---

## 🚀 Fonctionnalités Clés

- **Registre officiel (32 colonnes) :** Saisie complète conforme au formulaire réglementaire (identification, régime thérapeutique, forme TP/TEP, type de patient N/R/E/REP/T/A, suivi bactériologique par mois, issue du traitement).
- **Schéma pulmonaire interactif :** 
  - Tracé libre des lésions sur diagramme anatomique (`InkCanvas`).
  - Outils intégrés : choix de couleurs, taille du trait (1–12 px), gomme, annuler/rétablir.
  - Sauvegarde double : image haute résolution (PNG) + vecteurs ré-éditables (JSON).
- **Tableau de bord & Recherche :** Statistiques en direct (Total, Nouveaux, Rechutes, Guéris) et filtrage instantané multi-critères.
- **100% Hors-ligne & Sécurisé :** Base de données locale SQLite (`%LocalAppData%/tb_registry.db`), aucune dépendance cloud.
- **Interface Sombre Moderne :** Thème Dark soigné limitant la fatigue visuelle en milieu clinique.

---

## 🛠 Technologies

| Composant | Technologie |
| :--- | :--- |
| **Framework** | .NET 8.0 Windows Desktop (WPF) |
| **Architecture** | MVVM via `CommunityToolkit.Mvvm` (8.2.2) |
| **Base de données** | SQLite via Entity Framework Core 8 |
| **Graphisme** | WPF `InkCanvas` & `DrawingVisual` |

---

## ⚡ Démarrage Rapide

### Prérequis
- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Lancer l'application
```powershell
dotnet run --project testWPF.csproj
```

---

## 📂 Structure

```text
testWPF/
├── Assets/        # Schéma anatomique de référence
├── Controls/      # Contrôle de dessin InkCanvas (LungDrawingControl)
├── Converters/    # Convertisseurs d'affichage XAML
├── Data/          # Contexte EF Core (AppDbContext)
├── Models/        # TuberculosisCase (32 colonnes), StrokeData
├── Services/      # RegistryService (CRUD), LungImageService (Rendu PNG/JSON)
├── Themes/        # Thème sombre (DarkTheme.xaml)
├── ViewModels/    # MainViewModel (Logique métier & filtres)
└── Views/         # MainWindow, LungAnnotationWindow (Modal)
```
