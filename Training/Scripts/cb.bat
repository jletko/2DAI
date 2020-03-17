@echo off

taskkill /IM tensorboard.exe /f >nul 2>&1

RMDIR "models" /S /Q
MKDIR "models"
RMDIR "summaries" /S /Q
MKDIR "summaries"
