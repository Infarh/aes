@echo off
dotnet publish -o .Release --self-contained -c Release -r win-x64 
rem /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeAllContentForSelfExtract=true

pause