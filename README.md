# SiteManager - Advanced Programming Language - UNICT - AA 2024-2025
- Daniele Lucifora
- Fernando Riccioli

# Build & Deploy
## Prerequisiti:
- .NET SDK v9
- .NET workload MAUI
- Docker v24

# Build:
- Clonare la repository dal seguente [link](https://github.com/DanieleLucifora/SiteManager.git) e spostarsi all'interno della cartella /SiteManager
- Eseguire il comando *docker compose up -d*
- Spostarsi all'interno della cartella /SiteManager
- Eseguire il comando *dotnet build* per compilare l'applicazione
- Avviare l'applicazione eseguendo il comando *dotnet run -f net9.0-maccatalyst* (in ambiente MacOs) oppure *dotnet run -f net9.0-windowsv10.0.19041.0* (in ambiente Windows)