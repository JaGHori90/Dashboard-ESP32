---
name: arbeitsweise
description: Rezas Arbeitsweise für dieses Projekt (Dashboard-ESP32) - er will selbst lernen/beherrschen, nicht nur fertigen Code bekommen. Immer beachten, bevor Code-Änderungen gemacht werden.
---

# Arbeitsweise mit Reza

Von Reza festgelegt (24.09.2026):

**Reza will die Materie selbst beherrschen, nicht nur ein fertiges Ergebnis bekommen.**

## Regel

1. **Erst Plan/Vorschlag zeigen, dann fragen.** Bei neuen Features oder größeren Änderungen: zuerst kurz erklären, was gemacht werden soll und wie (Plan/Vorplan), nicht direkt loscoden.
2. **Erst umsetzen, wenn Reza sich zufrieden zeigt.** Erst wenn er explizit sagt, dass er mit dem Plan einverstanden ist (z. B. "passt", "ja", "mach das") - vorher nicht schreiben/committen.
3. **Umsetzung passiert ausschließlich auf `develop`**, nie auf `main` (siehe Skill `git-workflow`).
4. Ausnahme: reine Lese-/Diagnose-Aktionen (Dateien lesen, Logs/CI prüfen, Status checken, Erklärungen geben) brauchen keine Vorab-Bestätigung - nur tatsächliche Code-Änderungen.
5. Bei kleinen, eindeutigen Bugfixes (z. B. "das hier ist kaputt, fix das") reicht die direkte Anfrage als Zustimmung - unnötig langes Nachfragen vermeiden.

## Warum

Reza baut dieses Projekt bewusst zum Lernen (siehe README "Über dieses Projekt") und für die Jobsuche im .NET-/C#-Umfeld. Er will selbst verstehen und mitentscheiden, nicht nur ein Blackbox-Ergebnis bekommen.
