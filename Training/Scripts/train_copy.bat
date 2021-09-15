for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "Y=%dt:~3,1%" & set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"
set "datestamp=%YY%%MM%%DD%" & set "timestamp=%HH%%Min%%Sec%"
set "fullstamp=%Y%%MM%%DD%_%HH%%Min%%Sec%"

set UNITY_PROJECT_DIR_PATH=%PROJECTS_DIRECTORY_PATH%\%PROJECT_DIRECTORY%
set TRAINING_DIR_PATH=%PROJECTS_DIRECTORY_PATH%\%PROJECT_DIRECTORY%\Training
set BRAINS_DIR_PATH=%PROJECTS_DIRECTORY_PATH%\%PROJECT_DIRECTORY%\Assets\BrainsTmp
set RUN_DIR_NAME=Run%fullstamp%

if [%1]==[] (
	mlagents-learn %TRAINING_DIR_PATH%\trainer_config.yaml --time-scale 10 --run-id=%RUN_DIR_NAME%
) else (
	mlagents-learn %TRAINING_DIR_PATH%\trainer_config.yaml --time-scale 10 --run-id=%RUN_DIR_NAME% --env=%UNITY_PROJECT_DIR_PATH%\bin\%PROJECT_DIRECTORY%.exe --num-envs=%1 --no-graphics
)

if not exist %BRAINS_DIR_PATH% MkDir %BRAINS_DIR_PATH%
for /F %%b in ('dir /b results\%RUN_DIR_NAME%\*.onnx') do copy results\Run%fullstamp%\%%b %BRAINS_DIR_PATH%\%%~nb%fullstamp%%%~xb

if %ERRORLEVEL% EQU 0 exit




