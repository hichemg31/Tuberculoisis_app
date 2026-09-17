# Registre de Déclaration des Cas de Tuberculose (TB Registry)

> **Application médicale de bureau moderne pour la gestion, le suivi et la déclaration officielle des cas de tuberculose, dotée d'un module interactif d'annotation anatomique pulmonaire.**

[![Platform](https://img.shields.io/badge/Platform-Windows%20(WPF)-blue.svg)](https://dotnet.microsoft.com/)
[![Target Framework](https://img.shields.io/badge/.NET-8.0--windows-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Database](https://img.shields.io/badge/Database-SQLite%20via%20EF%20Core%208-green.svg)](https://learn.microsoft.com/ef/core/)
[![Architecture](https://img.shields.io/badge/Architecture-MVVM%20(CommunityToolkit)-orange.svg)](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
[![Theme](https://img.shields.io/badge/Theme-Modern%20Dark%20Mode-22272e.svg)](#interface-et-ergonomie)

---

## 📋 Table des Matières

- [Présentation](#-présentation)
- [Fonctionnalités Principales](#-fonctionnalités-principales)
  - [1. Registre Officiel à 32 Colonnes](#1-registre-officiel-à-32-colonnes)
  - [2. Module de Schéma et d'Annotation Pulmonaire](#2-module-de-schéma-et-dannotation-pulmonaire)
  - [3. Tableau de Bord & Statistiques en Temps Réel](#3-tableau-de-bord--statistiques-en-temps-réel)
  - [4. Recherche Rapide et Filtrage](#4-recherche-rapide-et-filtrage)
  - [5. Persistance Locale Robuste (SQLite)](#5-persistance-locale-robuste-sqlite)
- [Architecture Technique](#-architecture-technique)
- [Structure du Projet](#-structure-du-projet)
- [Prérequis et Installation](#-prérequis-et-installation)
  - [Prérequis](#prérequis)
  - [Cloner et Compiler](#cloner-et-compiler)
  - [Exécution](#exécution)
- [Base de Données et Migrations](#-base-de-données-et-migrations)
- [Guide d'Utilisation](#-guide-dutilisation)
- [Licence et Confidentialité](#-licence-et-confidentialité)

---

## 🩺 Présentation

Le **Registre de Déclaration des Cas de Tuberculose** est une application Windows Presentation Foundation (WPF) conçue spécifiquement pour les pneumologues, médecins traitants et services hospitaliers de lutte antituberculeuse.

Elle dématérialise fidèlement le registre officiel papier de déclaration de tuberculose (32 colonnes réglementaires), tout en éliminant les risques de perte de données et en accélérant les saisies cliniques. L'application intègre un outil de dessin vectoriel permettant aux médecins de cartographier directement les lésions pulmonaires (infiltrats, cavernes, épanchements, nodules) sur un diagramme anatomique de référence.

---

## ✨ Fonctionnalités Principales

### 1. Registre Officiel à 32 Colonnes
L'application respecte à l'identique la nomenclature et l'ordre des colonnes du registre officiel de déclaration :

| Section | Colonnes | Description |
| :--- | :--- | :--- |
| **Identification** | (1) à (6) | N° d'ordre, Date d'enregistrement, Nom et Prénom, Sexe (H/F), Âge, Adresse complète |
| **Traitement** | (7) à (9) | Date de début de traitement, Lieu de prise en charge, Régime thérapeutique (`2RHZE/4RH`, `2RHZ/4RH`, etc.) |
| **Classification** | (10) | Forme de la maladie : **TP** (Tuberculose Pulmonaire) ou **TEP** (Tuberculose Extra-Pulmonaire avec localisation) |
| **Type de Malade** | (11) à (16) | Cases à cocher exclusives : **N** (Nouveau), **R** (Rechute), **E** (Échec), **REP** (Reprise après abandon), **T** (Transfert), **A** (Autre) |
| **Examens Microscopiques** | (17) à (25) | Suivi bactériologique au cours du temps (Frottis et Culture) : Pré-traitement, 2e/3e mois, 4e/5e mois, 6e/8e mois, Au-delà |
| **Résultat du Traitement** | (26) à (31) | Issue finale : Guéris / Traitement terminé, Pas d'examen, Décédé, Échec, Perdu de vue, Transféré |
| **Observations** | (32) | Notes cliniques complémentaires |

### 2. Module de Schéma et d'Annotation Pulmonaire
Chaque fiche patient peut comporter son propre schéma pulmonaire annoté :
- **Bouton Schéma Interactif :** Dans le formulaire patient, une vignette miniature interactive affiche le statut de l'annotation (`Non annoté` ou `✓ Annoté`) avec un aperçu visuel direct. Un simple clic ouvre l'éditeur en fenêtre modale dédiée centrée.
- **Support de dessin `InkCanvas` vectoriel :**
  - Tracé à main levée fluide avec précision sous-pixélique.
  - Palette d'outils médicaux : choix des couleurs (Rouge lésionnel, Orange, Jaune, Bleu, Vert, Blanc), curseur d'épaisseur du trait (1 px à 12 px) et mode gomme sélective.
  - Historique complet avec fonctions **Annuler** (*Undo*), **Rétablir** (*Redo*) et **Effacer tout**.
- **Conservation du schéma original :** L'image source anatomique (`Assets/Lung_Labeled_Diagram2.jpg`) n'est jamais écrasée ni altérée.
- **Double Persistance Avancée :**
  - **Rendu composite haute résolution (PNG) :** Stocké en base de données sous forme de blob binaire (`byte[] LungDrawing`) pour affichage immédiat et impression.
  - **Vecteurs normalisés (JSON) :** Sérialisés sous forme de coordonnées relatives `[0.0, 1.0]` (`AnnotationsJson`), garantissant la ré-éditabilité totale des traits indépendamment de la résolution ou du redimensionnement de la fenêtre.

### 3. Tableau de Bord & Statistiques en Temps Réel
Un volet latéral donne une vision synthétique instantanée de la cohorte active :
- **Total des cas** enregistrés.
- **Nouveaux cas (N)**.
- **Rechutes (R)**.
- **Patients guéris / traitements terminés**.

### 4. Recherche Rapide et Filtrage
- Barre de recherche instantanée multi-critères : filtrage dynamique par nom du patient, adresse, lieu de traitement ou numéro d'ordre.
- Réinitialisation en un clic.

### 5. Persistance Locale Robuste (SQLite)
- Stockage local zéro-configuration : la base SQLite `tb_registry.db` est automatiquement créée et migrée dans le répertoire `%LocalAppData%` de l'utilisateur.
- Données entièrement confinées au poste de travail médical (conformité et confidentialité des données de santé hors-ligne).

---

## 🏗 Architecture Technique

Le projet est développé selon les standards professionnels WPF et .NET 8 :

- **Pattern MVVM (Model-View-ViewModel) :** Implémenté via **`CommunityToolkit.Mvvm`** avec source-generators (`[ObservableProperty]`, `[RelayCommand]`).
- **Accès aux Données (ORM) :** **Entity Framework Core 8** avec pilote SQLite (`Microsoft.EntityFrameworkCore.Sqlite`).
- **Moteur Graphique :** `InkCanvas` couplé à un service de projection dédié (`LungImageService`) utilisant `DrawingVisual`, `RenderTargetBitmap` et `PngBitmapEncoder`.
- **Thème & Design :** Thème sombre médical complet (`Themes/DarkTheme.xaml`) avec typographie soignée, contrastes accessibles, barres de défilement stylisées et animations de transition douces.

---

## 📁 Structure du Projet

```text
testWPF/
├── Assets/
│   └── Lung_Labeled_Diagram2.jpg     # Image anatomique pulmonaire de référence (Resource)
├── Controls/
│   ├── LungDrawingControl.xaml       # Composant de dessin vectoriel sur toile
│   └── LungDrawingControl.xaml.cs
├── Converters/
│   └── Converters.cs                 # Convertisseurs XAML (ByteArray to Image, Visibilité)
├── Data/
│   └── AppDbContext.cs               # Contexte EF Core configuré vers %LocalAppData%/tb_registry.db
├── Migrations/                       # Historique des migrations EF Core (Code-First)
│   ├── 20260828200000_InitialCreate.cs
│   └── 20260828210000_AddAnnotationsJson.cs
├── Models/
│   ├── StrokeData.cs                 # Modèle de sérialisation JSON des traits de dessin
│   └── TuberculosisCase.cs           # Modèle de données complet (32 colonnes + Schéma)
├── Services/
│   ├── LungImageService.cs           # Service de rendu composite, normalisation et sérialisation
│   └── RegistryService.cs            # Service CRUD d'accès aux cas de tuberculose
├── Themes/
│   └── DarkTheme.xaml                # Dictionnaire de ressources du thème sombre (Styles & Brushes)
├── ViewModels/
│   └── MainViewModel.cs              # Logique de présentation, commandes CRUD, stats et recherche
├── Views/
│   ├── MainWindow.xaml               # Fenêtre principale (Grille 32 colonnes, volet latéral, formulaire)
│   ├── MainWindow.xaml.cs
│   ├── LungAnnotationWindow.xaml     # Boîte de dialogue modale d'annotation pulmonaire
│   └── LungAnnotationWindow.xaml.cs
├── App.xaml / App.xaml.cs            # Point d'entrée de l'application et gestion globale des erreurs
├── testWPF.csproj                    # Fichier de projet .NET 8 WPF
└── README.md                         # Documentation du projet
```

---

## 🚀 Prérequis et Installation

### Prérequis
- **Système d'exploitation :** Windows 10 (1809+) ou Windows 11 (x64 / ARM64).
- **Runtime / SDK :** [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou supérieur.
- **Environnement de développement recommandé :**
  - Visual Studio 2022 (version 17.8 ou supérieure) avec la charge de travail **Développement .NET Desktop**.
  - Ou Visual Studio Code avec l'extension C# Dev Kit.
  - Ou JetBrains Rider.

### Cloner et Compiler
Ouvrez un terminal PowerShell ou une invite de commandes :

```powershell
# Cloner le dépôt
git clone <url-du-depot>
cd testWPF

# Restaurer les dépendances NuGet
dotnet restore

# Compiler le projet en mode Release ou Debug
dotnet build -c Release
```

### Exécution
Pour lancer directement l'application :

```powershell
dotnet run
```

Ou ouvrez la solution dans Visual Studio et appuyez sur **`F5`**.

---

## 🗄 Base de Données et Migrations

L'application initialise et applique automatiquement les migrations de schéma lors du premier démarrage si la base de données n'existe pas encore.

- **Emplacement par défaut :**
  `C:\Users\<NomUtilisateur>\AppData\Local\tb_registry.db`

- **Commandes EF Core utiles (si modification du modèle) :**
  ```powershell
  # Ajouter une nouvelle migration
  dotnet ef migrations add <NomDeLaMigration>

  # Mettre à jour la base manuellement
  dotnet ef database update
  ```

---

## 📖 Guide d'Utilisation

1. **Créer un nouveau cas :**
   - Cliquez sur le bouton **`+ Nouveau Cas`** dans la barre d'outils supérieure.
   - Le panneau latéral s'ouvre avec un numéro d'ordre pré-rempli et la date du jour.
   - Remplissez les données administratives, cliniques et bactériologiques.

2. **Annoter le schéma pulmonaire :**
   - Dans le formulaire patient, localisez la section **Schéma Pulmonaire**.
   - Cliquez sur la vignette du poumon (carte interactive avec effet de survol).
   - Une fenêtre dédiée s'affiche au centre de l'écran avec le diagramme anatomique haute résolution.
   - Sélectionnez la couleur et l'épaisseur voulues pour tracer les foyers d'infection.
   - Cliquez sur **`Enregistrer l'annotation`** pour valider. Le badge passe à `✓ Annoté` et la miniature se met à jour immédiatement.

3. **Enregistrer la fiche :**
   - Cliquez sur **`Enregistrer`** au bas du formulaire.
   - Le cas apparaît instantanément dans la grille principale et les statistiques du tableau de bord s'actualisent.

4. **Modifier ou Consulter un cas existant :**
   - Sélectionnez une ligne dans le tableau principal et cliquez sur **`Modifier`** (ou double-cliquez sur le cas).
   - Les annotations pulmonaires existantes sont automatiquement rechargées et peuvent être modifiées à tout moment.

---

## 🔒 Licence et Confidentialité

Ce logiciel a été conçu pour un usage médical et respecte la confidentialité des dossiers patients :
- Fonctionnement entièrement autonome et **hors ligne** (aucun transfert de données vers des serveurs tiers).
- Données persistées localement dans l'espace utilisateur sécurisé de la machine Windows hôte.
