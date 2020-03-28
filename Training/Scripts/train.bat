@echo off
set "TEMP_VAR=0"
if [%1]==[p] set TEMP_VAR=1
if [%1]==[s] set TEMP_VAR=1

if %TEMP_VAR% equ 1 (
	start /min train_copy.bat %*
) else (
	echo on
	echo Missing p or s parameter
)