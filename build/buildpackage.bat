@echo off
@cd /d "%~dp0"
del /f/q/s GenerativeAI\*
del /f/q/s GenerativeAI*
IF NOT EXIST GenerativeAI (mkdir GenerativeAI)
copy ..\bin\AnyCPU\Release\*.dll GenerativeAI
copy ..\bin\AnyCPU\Release\GenerativeAI*.xml GenerativeAI
tar.exe -a -c -f GenerativeAI.zip GenerativeAI\*