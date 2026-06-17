# AzubiLog

Webanwendung zur Erstellung und Verwaltung von Ausbildungsnachweisen (Berichtsheften).

## Features

- **Tagesberichte** – Betrieb, Berufsschule, Feiertag, Urlaub, Krank erfassen
- **Wochenberichte** – Wöchentliche Zusammenfassungen erstellen und einreichen
- **Dashboard** – Übersicht über Stunden, offene Aufgaben und Berichte
- **Stundenplan** – Berufsschulstundenplan einsehen (wird vom Klassensprecher gepflegt)
- **Aufgaben** – Todo-Liste für persönliche Aufgaben
- **PDF-Export** – Wochenberichte als PDF generieren
- **Profil** – Persönliche Daten, Schule, Klasse und Arbeitszeiten verwalten

## Rollen

| Rolle | Beschreibung |
|---|---|
| **Azubi** | Standard-Rolle. Tagesberichte, Wochenberichte, Aufgaben, PDF-Export |
| **Klassensprecher** | Alles vom Azubi + Stundenplan für die Klasse verwalten |
| **Ausbilder** | Zugewiesene Azubis sehen, Wochenberichte prüfen und genehmigen/ablehnen |

## Tech-Stack

- **Framework:** Blazor Server (.NET 10)
- **Sprache:** C#
- **Datenbank:** SQLite via Entity Framework Core
- **PDF:** QuestPDF
- **Auth:** ASP.NET Core Identity

## Voraussetzungen

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Setup

```bash
git clone https://github.com/Tamarazbl/AzubiLog.git
cd AzubiLog
dotnet restore
dotnet ef database update
dotnet run
```

Die App startet standardmäßig auf `http://localhost:5179`.

## Entwicklung

```bash
# Migration erstellen
dotnet ef migrations add <Name> --output-dir Data/Migrations

# Datenbank aktualisieren
dotnet ef database update
```

## Projektstruktur

```
AzubiLog/
├── Components/
│   ├── Layout/          # MainLayout mit Sidebar + Header
│   └── Pages/
│       ├── Account/     # Login, Register, Profile
│       ├── Admin/       # Stundenplan-Verwaltung (Klassensprecher)
│       └── Trainer/     # Azubi-Übersicht (Ausbilder)
├── Data/                # DbContext, Migrations
├── Models/              # Entity-Klassen
├── Services/            # Business-Logik
│   ├── Dashboard/
│   ├── Identity/
│   ├── ReportEntries/
│   ├── Timetable/
│   ├── Todos/
│   ├── Trainer/
│   └── WeeklyReports/
└── wwwroot/             # CSS, statische Dateien
```

## Team

- Ahmad Almohammad – Backend
- Tamara Zobel – Frontend, Dokumentation
- Paul Ketschik – Datenbank, Frontend

## Lizenz

Schulprojekt – OSZ IMT Berlin
