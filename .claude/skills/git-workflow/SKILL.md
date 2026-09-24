---
name: git-workflow
description: Git-Branching-Regeln für dieses Repo (Dashboard-ESP32). Immer laden/beachten, bevor Änderungen committed oder gepusht werden.
---

# Git-Workflow für dieses Projekt

Reza hat das explizit festgelegt (24.09.2026): **Es wird nur auf `develop` gearbeitet, nicht direkt auf `main`.**

## Regeln

1. **Alle Commits und Pushes gehen auf `develop`.** Das ist der aktive Arbeitsbranch.
2. **`main` NIEMALS automatisch mit `develop` synchronisieren.** Kein "nach jedem Push auch main aktualisieren" — das würde `develop` seinen Sinn nehmen (main wäre dann nur ein Spiegel, keine eigene "stabiler Stand"-Ebene).
3. **`main` wird nur auf explizite Anweisung aktualisiert.** Erst wenn Reza sagt sowas wie "bring das nach main", "release", "main aktualisieren" o. ä. — dann und nur dann übernehmen.
4. **Warum kein normaler `git merge`**: `main` wurde bewusst per History-Reset auf einen einzigen sauberen Commit zurückgesetzt (Sicherheit — alte Commits enthielten mal Secrets). `main` und `develop` haben dadurch **unrelated histories** (`git merge` schlägt fehl: "refusing to merge unrelated histories"). Zum Übertragen von `develop` nach `main`, wenn gewünscht:
   ```bash
   git checkout main
   git checkout develop -- .
   git add -A
   git commit -m "..."
   git push origin main
   git checkout develop
   ```
   Das kopiert nur den Dateistand, ohne die Historien zu vermischen — main bleibt bei seiner eigenen, kurzen History.
5. Nach jeder Aktion auf main: **zurück auf `develop` wechseln**, damit nicht versehentlich weiter auf main gearbeitet wird.

## Kontext, falls relevant

- Grund für den ganzen Aufbau: Reza will das Projekt öffentlich als Portfolio zeigen (Jobsuche .NET/C#). `main` soll der saubere, vorzeigbare Stand sein; `develop` ist die Werkstatt.
- Die GitHub-Actions-CI (`test-api.yml`) läuft auf Pushes zu `main` **und** `develop` — das bleibt so, unabhängig von diesem Sync-Verhalten.
