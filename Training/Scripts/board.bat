@echo off

TASKLIST | FINDSTR "tensorboard" || start /min tensorboard --logdir=%PROJECTS_DIRECTORY_PATH%\2DAI\Training\results --port 6006"
start "Chrome" "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --new-window "http://localhost:6006"