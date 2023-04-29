@echo off

TASKLIST | FINDSTR "tensorboard" || start /min tensorboard --logdir=%PROJECTS_DIRECTORY_PATH%\%PROJECT_DIRECTORY%\Training\scripts\results --port 6006"
start "Chrome" "C:\Program Files\Google\Chrome\Application\chrome.exe" --new-window "http://localhost:6006"