# Dashboard-ESP32

Ein End-to-End IoT-Dashboard: ein ESP32-Sensor misst Temperatur, Luftfeuchtigkeit und Luftdruck im Wohnraum, eine ASP.NET Core API nimmt die Werte entgegen und speichert sie in Postgres, und eine WPF-Desktop-App zeigt sie als Dashboard an — inklusive Verlaufs-Chart, konfigurierbarer Städteliste mit Live-Wetterdaten, automatischer Datenbereinigung und automatischem Self-Update auf Windows.

Kein Tutorial-Projekt: alle vier Ebenen (Embedded, Backend, Datenbank, Desktop-Client) laufen produktiv im 15-Minuten-Takt, inklusive CI/CD auf Azure und GitHub Actions.

## Architektur

```
┌─────────────┐   HTTPS POST alle     ┌──────────────────┐   EF Core    ┌──────────────┐
│  ESP32 +    │   15 Min. (JSON)      │  ASP.NET Core 8  │  (Npgsql)    │  Postgres    │
│  BME280     ├──────────────────────▶│  Web API         ├─────────────▶│  (Neon,      │
│  (C++/      │                       │  (Azure App      │              │  serverless) │
│  PlatformIO)│                       │  Service, F1)    │              │              │
└─────────────┘                       └────────┬─────────┘              └──────────────┘
                                               │ HTTPS GET/DELETE
                                               ▼
                                       ┌──────────────────┐        ┌─────────────────────┐
                                       │  WPF-Dashboard   │───────▶│  Open-Meteo API     │
                                       │  (Windows, mit   │        │  (Städte-Wetter)    │
                                       │  Auto-Update via │        └─────────────────────┘
                                       │  Velopack)       │
                                       └──────────────────┘
```

## Komponenten

| Ordner | Was | Tech-Stack |
|---|---|---|
| `ESP32Dev/` | Firmware für den ESP32, liest BME280-Sensordaten aus und postet sie per HTTPS an die API | C++, PlatformIO, Arduino-Framework, WiFiClientSecure, ArduinoJson |
| `ASP.Net/` | REST-API (`Api`), Domänenmodell (`Core`), Datenzugriff (`Persistence`), Seed-Tool (`ConsoleFillDb`) | ASP.NET Core 8, Entity Framework Core, Npgsql |
| `WPF/` | Desktop-Dashboard mit Live-Chart, Städte-Wetter-Widget und Self-Update | .NET 8, WPF, LiveCharts2, Velopack |
| `.github/workflows/` | CI/CD: Azure-Deploy der API bei Push, Windows-Installer-Release der WPF-App bei Git-Tag | GitHub Actions |

## Features

- **Sensor-Firmware** mit robustem WLAN-Reconnect, gebundenen HTTP-Timeouts und Hardware-Watchdog (`esp_task_wdt`) — läuft dauerhaft ohne manuellen Eingriff, auch bei WLAN-Aussetzern oder blockierenden I2C-Reads.
- **Automatische Datenbereinigung**: die API löscht Messwerte, die älter als 48 Stunden sind, und kappt zusätzlich die Gesamtzeilenzahl als Sicherheitsnetz — angestoßen vom WPF-Client bei jedem Laden.
- **Konfigurierbare Städteliste** im Dashboard (bis zu 5 Städte, live editierbar) mit aktuellen Min/Max-Temperaturen über die Open-Meteo-API.
- **Self-Updating Windows-Installer**: die WPF-App prüft bei jedem Start selbstständig auf GitHub Releases nach einer neueren Version, lädt sie im Hintergrund und wendet sie beim nächsten Neustart an (Velopack) — kein manuelles Neuinstallieren nötig.
- **CI/CD**: Push auf `main` (im `ASP.Net`-Pfad) deployt die API automatisch auf Azure App Service; ein Git-Tag (`vX.Y.Z`) baut und veröffentlicht automatisch einen neuen Windows-Installer als GitHub Release.

## Lokal aufsetzen

### Sensor (ESP32 + BME280)

```bash
cd ESP32Dev
cp include/secrets.h.example include/secrets.h   # eigene WLAN-Zugangsdaten eintragen
pio run --target upload --target monitor --upload-port /dev/cu.wchusbserial1130
```

### API

```bash
cd ASP.Net/Api
# eigene Postgres-Connection-String via dotnet user-secrets oder appsettings.Development.json setzen
dotnet run
```

### WPF-Dashboard

```bash
cd WPF/Wpf
# ApiBaseUrl in appsettings.json auf die eigene API-Instanz setzen
dotnet run
```

Fertige Windows-Installer gibt es unter [Releases](../../releases) — einmal installieren, danach aktualisiert sich die App selbst.

## Roadmap

- [ ] Automatisierte Tests für API (xUnit) und WPF-ViewModels
- [ ] Angular-Web-Dashboard als Alternative zum WPF-Client
- [ ] Nächste Sensor-Generation auf ESP32-C6

## Über dieses Projekt

Ich bin Reza Jaghori und baue dieses Projekt, um mich in der Praxis über den gesamten Stack hinweg weiterzuentwickeln — von Embedded-C++ über eine .NET-Backend-API bis zum Desktop-Client mit eigenem Update-Mechanismus. Es ist gleichzeitig mein Lern- und Referenzprojekt für eine Entwicklerstelle im .NET-/C#-Umfeld.

Was dieses Projekt zeigt: einen vollständigen Stack eigenständig aufsetzen und produktiv am Laufen halten, echte Betriebsprobleme selbst diagnostizieren und beheben (WLAN-Ausfälle, Hardware-Watchdogs, CI/CD-Fehler, sauberer Umgang mit Secrets), und ein Projekt von der ersten Idee bis zum automatisierten Release durchziehen.

Ich bin auf Jobsuche als Entwickler im .NET-/C#-Umfeld, gerne mit Embedded- oder Cloud-Bezug, im Raum Linz/Steyr.

<!-- Ergänze hier gern: Ausbildung/Studium, Kontakt (LinkedIn/E-Mail), Link zu weiteren Projekten. -->

## Lizenz

Dieses Projekt ist unter der [MIT-Lizenz](LICENSE) veröffentlicht.
