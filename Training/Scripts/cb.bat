@echo off

taskkill /IM tensorboard.exe /f >nul 2>&1

RMDIR %PROJECTS_DIRECTORY_PATH%\2DAI\Training\results /S /Q
MKDIR %PROJECTS_DIRECTORY_PATH%\2DAI\Training\results
