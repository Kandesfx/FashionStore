# Script to push code to GitHub
git config user.name "Kandesfx"
git config user.email "kandesfx@users.noreply.github.com"

# Check if remote exists
$remoteExists = git remote get-url origin 2>$null
if ($LASTEXITCODE -ne 0) {
    git remote add origin https://github.com/Kandesfx/FashionStore.git
}

# Force push to overwrite remote
Write-Host "Pushing code to GitHub..."
git push -u origin main --force

if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully pushed to GitHub!" -ForegroundColor Green
} else {
    Write-Host "Failed to push. Please check your GitHub credentials." -ForegroundColor Red
}

