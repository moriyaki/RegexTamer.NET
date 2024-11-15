@echo off
setlocal

REM set project path
set "ProjectPath=RegexTamer.NET.sln"

REM Specify the build configuration (Debug or Release)
set "Configuration=Release"

REM Specifying the output directory
set "OutputDirectory=.\RegexTamer.NET\bin\Release\net9.0-windows7.0"

REM Name of the ZIP output file
set "OutputZip=..\RegexTamer.NET.zip"

REM Build using dotnet
echo Building is starting...
dotnet build "%ProjectPath%" /p:Configuration=%Configuration% /t:Build
if %ERRORLEVEL% neq 0 (
    echo Build failed.
    pause
    exit /b %ERRORLEVEL%
)

REM Delete old ZIP files
if exist "%OutputZip%" del "%OutputZip%"

REM Use PowerShell to ZIP the output directory
echo The build is complete. The release file is being compressed into a ZIP...
powershell -command "Compress-Archive -Path '%OutputDirectory%\*' -DestinationPath '%OutputZip%'"

REM Completion message
echo The release file has been compressed into a ZIP file: %OutputZip%
pause