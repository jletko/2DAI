@echo off

for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"
set "datestamp=%YY%%MM%%DD%" & set "timestamp=%HH%%Min%%Sec%"
set "fullstamp=%YY%%MM%%DD%_%HH%%Min%%Sec%"

set UNITY_PROJECT_DIR_PATH=%PROJECTS_DIRECTORY_PATH%\2DAI
set TRAINING_DIR_PATH=%PROJECTS_DIRECTORY_PATH%\2DAI\Training
set BRAINS_DIR_PATH=%PROJECTS_DIRECTORY_PATH%\2DAI\Assets\Brains
set RUN_DIR_NAME=Run%fullstamp%

mlagents-learn %TRAINING_DIR_PATH%\trainer_config.yaml --run-id=%RUN_DIR_NAME% --train
if not exist %BRAINS_DIR_PATH% MkDir %BRAINS_DIR_PATH%
for /F %%b in ('dir /b models\%RUN_DIR_NAME%\*.nn') do copy models\Run%fullstamp%\%%b %BRAINS_DIR_PATH%\%%~nb_%fullstamp%%%~xb

if %ERRORLEVEL% EQU 0 exit