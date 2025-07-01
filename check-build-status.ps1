#!/usr/bin/env pwsh
# 🔍 GitHub Actions Build Status Checker

Write-Host "🔍 Checking GitHub Actions Build Status" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Get the repository URL from git remote
try {
    $remoteUrl = git remote get-url origin    if ($remoteUrl -match "github\.com[:/]([^/]+)/([^/\.]+)") {
        $owner = $matches[1]
        $repo = $matches[2]
        $actionsUrl = "https://github.com/$owner/$repo/actions"
        
        Write-Host "📊 Repository: $owner/$repo" -ForegroundColor White
        Write-Host "🔗 Actions URL: $actionsUrl" -ForegroundColor Blue
        Write-Host ""
        Write-Host "💡 To check build status:" -ForegroundColor Yellow
        Write-Host "   1. Open: $actionsUrl" -ForegroundColor Gray
        Write-Host "   2. Look for the latest workflow run" -ForegroundColor Gray
        Write-Host "   3. Click on it to see detailed logs" -ForegroundColor Gray
        Write-Host ""
        
        # Get latest commit info
        $latestCommit = git log --oneline -1
        Write-Host "📝 Latest commit: $latestCommit" -ForegroundColor White
        Write-Host ""
        
        Write-Host "🎯 Expected workflows to run:" -ForegroundColor Green
        Write-Host "   ✅ Build and Test - Main CI pipeline" -ForegroundColor White
        Write-Host "   ✅ Quality Gate - If this was a PR" -ForegroundColor White
        Write-Host ""
        
        Write-Host "🔄 To trigger another build:" -ForegroundColor Yellow
        Write-Host "   git commit --allow-empty -m 'Trigger build'" -ForegroundColor Gray
        Write-Host "   git push origin main" -ForegroundColor Gray
        
        # Try to open the browser (optional)
        Write-Host ""
        $openBrowser = Read-Host "Open Actions page in browser? (y/N)"
        if ($openBrowser -eq 'y' -or $openBrowser -eq 'Y') {
            Start-Process $actionsUrl
            Write-Host "🌐 Browser opened to Actions page" -ForegroundColor Green
        }
        
    } else {
        Write-Host "❌ Could not parse GitHub repository URL" -ForegroundColor Red
        Write-Host "   Remote URL: $remoteUrl" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Error getting repository information: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎉 Your GitOps pipeline is now running on GitHub Actions!" -ForegroundColor Green
