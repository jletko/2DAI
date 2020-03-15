echo off

TASKLIST | FINDSTR "tensorboard" || start /min tensorboard --logdir=summaries --port 6006"
"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --new-window "http://localhost:6006"