@echo off
setlocal
cd /d "%~dp0"

echo ===================================================
echo     WhatsApp Bot Project Installer (Portable)
echo ===================================================

echo.
echo 1. Checking for Node.js...
node -v >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Node.js is not installed or not in PATH.
    echo Please install Node.js from the 'tools' folder or download from nodejs.org.
    echo After installing, restart this script.
    pause
    exit /b 1
) else (
    echo [OK] Node.js is installed.
)

echo.
echo 2. Installing Dependencies...
cd ..
call npm install
if %errorlevel% neq 0 (
    echo [ERROR] Failed to install dependencies.
    pause
    exit /b 1
)

echo.
echo 3. Installing PM2 (Process Manager)...
call npm install -g pm2 pm2-windows-startup
if %errorlevel% neq 0 (
    echo [WARNING] Global installation might have failed.
    echo Trying local fallback...
)

echo.
echo 4. Registering PM2 as a Windows Service...
call pm2-startup install
if %errorlevel% neq 0 (
    echo [WARNING] Startup registration might need Administrator privileges.
    echo Please ensure you ran this as Administrator.
)

echo.
echo 5. Starting Application...
call pm2 start ecosystem.config.js
call pm2 save

echo.
echo ===================================================
echo     Installation Complete!
echo ===================================================
echo.
pause
