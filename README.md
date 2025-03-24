# SiteManager - Advanced Programming Language - UNICT - AA 2024-2025
- Daniele Lucifora
- Fernando Riccioli

# Descrizione
SiteManager è un'applicazione multi-piattaforma sviluppata utilizzando tre linguaggi di programmazione: C#, C++ e Python. L’obiettivo dell’applicazione è fornire una gestione efficace dei cantieri edili attraverso un’interfaccia utente intuitiva e funzionalità di
analisi dei dati, come la generazione di report e statistiche.

# Build & Deploy

## Prerequisiti:
- .NET SDK v9
- .NET workload MAUI
- Docker v24

## Build:
1. Clonare la repository dal seguente [link](https://github.com/DanieleLucifora/SiteManager.git) e spostarsi all'interno della cartella `/SiteManager`.
2. Eseguire il comando `docker compose up -d` per avviare i servizi.
3. Spostarsi all'interno della cartella `/SiteManager`.
4. Eseguire il comando `dotnet build` per compilare l'applicazione.

## Deploy:
1. Avviare l'applicazione eseguendo il comando appropriato per il sistema operativo:
   - **MacOS**: `dotnet run -f net9.0-maccatalyst`
   - **Windows**: `dotnet run -f net9.0-windowsv10.0.19041.0`