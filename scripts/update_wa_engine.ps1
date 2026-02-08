Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "   WhatsApp Bot Auto-Updater Tool v1.0    " -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Always run from repo root (script is located at <repo>\scripts\update_wa_engine.ps1)
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$botProcessName = "whatsapp-bot"

Write-Host "Stopping Bot Process..." -ForegroundColor Yellow
pm2 stop $botProcessName
if ($LASTEXITCODE -ne 0) {
    Write-Host "PM2 process '$botProcessName' not found or stop failed. Continuing..." -ForegroundColor DarkYellow
}

Write-Host "Backing up session data..." -ForegroundColor Yellow
$date = Get-Date -Format "yyyyMMdd-HHmm"
$backupPath = "backups\session_backup_$date"
if (Test-Path ".wwebjs_auth") {
    New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
    Copy-Item -Path ".wwebjs_auth" -Destination $backupPath -Recurse -Force
    Write-Host "Session backed up to $backupPath" -ForegroundColor Green
}

Write-Host "Updating WhatsApp Engine (whatsapp-web.js)..." -ForegroundColor Yellow
npm install whatsapp-web.js@latest
if ($LASTEXITCODE -eq 0) {
    Write-Host "Library updated successfully!" -ForegroundColor Green
} else {
    Write-Host "Update failed. Check internet connection." -ForegroundColor Red
    exit 1
}

Write-Host "Updating Browser Engine (Puppeteer)..." -ForegroundColor Yellow
npm install puppeteer@latest

Write-Host "Restarting Bot Process..." -ForegroundColor Yellow
pm2 restart $botProcessName
if ($LASTEXITCODE -ne 0) {
    # Try start if restart fails
    pm2 start index.js --name $botProcessName
}

Write-Host ""
Write-Host "✅ UPDATE COMPLETE! The system is running with the latest WhatsApp engine." -ForegroundColor Green
Write-Host "Waiting 5 seconds..."
Start-Sleep -Seconds 5
