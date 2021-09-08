@echo off

taskkill /IM tensorboard.exe /f >nul 2>&1

RMDIR %PROJECTS_DIRECTORY_PATH%\%PROJECT_DIRECTORY%\Training\results /S /Q
MKDIR %PROJECTS_DIRECTORY_PATH%\%PROJECT_DIRECTORY%\Training\results
