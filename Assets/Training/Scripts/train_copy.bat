@echo off

for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"
set "datestamp=%YY%%MM%%DD%" & set "timestamp=%HH%%Min%%Sec%"
set "fullstamp=%YY%%MM%%DD%_%HH%%Min%%Sec%"

set TRAINING_DIR=D:\Documents\_git\2DAI\Assets\Training
set RUN_DIR=Run%fullstamp%
set BRAINS_DIR=Brains

mlagents-learn %TRAINING_DIR%\trainer_config.yaml --run-id=%RUN_DIR% --train
for /F %%b in ('dir /b models\Run%fullstamp%\*.nn') do copy models\Run%fullstamp%\%%b %TRAINING_DIR%\%BRAINS_DIR%\%%~nb_%fullstamp%%%~xb

if %ERRORLEVEL% EQU 0 exit