@echo off

set /p PROJECT_DIRECTORY="Enter project directory: "
SET PATH=%PATH%;%PROJECTS_DIRECTORY_PATH%\%PROJECT_DIRECTORY%\Training\Scripts
cd %PROJECTS_DIRECTORY_PATH%\%PROJECT_DIRECTORY%\Training
cmd /k %ML_PYTHON_DIRECTORY_PATH%\Scripts\activate



