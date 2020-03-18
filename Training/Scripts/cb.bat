@echo off

taskkill /IM tensorboard.exe /f >nul 2>&1

RMDIR %PROJECTS_DIRECTORY_PATH%\ml-agents\models /S /Q
MKDIR %PROJECTS_DIRECTORY_PATH%\ml-agents\models
RMDIR %PROJECTS_DIRECTORY_PATH%\ml-agents\summaries /S /Q
MKDIR %PROJECTS_DIRECTORY_PATH%\ml-agents\summaries
