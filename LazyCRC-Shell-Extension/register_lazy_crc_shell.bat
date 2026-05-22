@echo off
cd %~dp0

fltmc >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Administrator privileges are required.
    echo Please right-click the script and select "Run as administrator".
    pause
    exit
)

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm.exe lazy_crc_shell.dll /codebase
pause