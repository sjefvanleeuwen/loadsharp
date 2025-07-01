# 🔧 GitHub Pages Deployment Troubleshooting

## 🚨 Common Issues and Solutions

### Issue: "Permission denied to github-actions[bot]" (403 Error)

**Problem:** GitHub Actions doesn't have permission to deploy to GitHub Pages.

**Solution:** Configure repository settings and workflow permissions.

#### 1. Repository Settings Configuration

Navigate to your repository settings:

**GitHub Repository → Settings → Pages**
- **Source:** Select "GitHub Actions" (not "Deploy from a branch")
- This enables the new GitHub Actions-based deployment method

**GitHub Repository → Settings → Actions → General**
- **Workflow permissions:** Select "Read and write permissions"  
- **Allow GitHub Actions to create and approve pull requests:** ✅ Checked

#### 2. Workflow Permissions Fix

The updated `documentation.yml` workflow now includes proper permissions:

```yaml
permissions:
  contents: read
  pages: write      # Required for Pages deployment
  id-token: write   # Required for OIDC authentication
```

#### 3. Modern Deployment Actions

**Before (problematic):**
```yaml
- uses: peaceiris/actions-gh-pages@v3
  with:
    github_token: ${{ secrets.GITHUB_TOKEN }}
```

**After (fixed):**
```yaml
- name: Setup Pages
  uses: actions/configure-pages@v4
  
- name: Upload artifact  
  uses: actions/upload-pages-artifact@v3
  with:
    path: ./docs-site/_site
    
- name: Deploy to GitHub Pages
  uses: actions/deploy-pages@v4
```

### Issue: DocFX Installation Failures

**Problem:** DocFX tool installation or execution fails.

**Solutions:**

#### Option 1: Use Simple Documentation Workflow
Switch to the lightweight `simple-docs.yml` workflow:
- No external dependencies
- Pure HTML generation
- Faster deployment

#### Option 2: Fix DocFX Path Issues
```yaml
- name: Install DocFX
  run: |
    dotnet tool install -g docfx
    export PATH="$PATH:$HOME/.dotnet/tools"
    docfx --version
```

### Issue: CNAME Configuration

**Problem:** Custom domain configuration conflicts.

**Solution:** Remove or fix CNAME settings:

**For GitHub.io domain:**
```yaml
# Remove or comment out cname line
# cname: loadsharp.github.io
```

**For custom domain:**
```yaml
# Ensure you own this domain
cname: your-actual-domain.com
```

### Issue: Workflow Environment Configuration

**Problem:** Missing environment configuration for Pages.

**Solution:** Add environment block:
```yaml
jobs:
  generate-docs:
    runs-on: ubuntu-latest
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
```

## 🎯 Recommended Deployment Strategy

### Option 1: Simple HTML Documentation (Recommended)
Use `simple-docs.yml` for immediate deployment:
- ✅ No external dependencies
- ✅ Fast deployment
- ✅ Converts markdown to HTML
- ✅ Includes test results

### Option 2: DocFX API Documentation  
Use `documentation.yml` for comprehensive API docs:
- 📚 Full API reference generation
- 🔧 Requires DocFX setup
- 📖 Professional documentation site

## 🔍 Debugging Steps

### 1. Check Repository Settings
```bash
# Verify Pages source is set to "GitHub Actions"
GitHub Repository → Settings → Pages → Source: GitHub Actions
```

### 2. Verify Workflow Permissions
```bash
# Check if workflow has required permissions
GitHub Repository → Settings → Actions → General → Workflow permissions: Read and write
```

### 3. Monitor Workflow Logs
```bash
# Check deployment logs for specific errors
GitHub Repository → Actions → [Workflow Run] → Deploy to GitHub Pages
```

### 4. Test with Simple Workflow First
```bash
# Disable documentation.yml and enable simple-docs.yml
# This eliminates DocFX as a potential issue source
```

## 📊 Verification Steps

After fixing the configuration:

1. **Push a commit** to trigger the workflow
2. **Check Actions tab** for successful deployment
3. **Visit Pages URL** (found in repository Settings → Pages)
4. **Verify content** loads correctly

## 🔗 Expected Results

**Successful deployment should show:**
- ✅ Build job completes successfully
- ✅ Deploy job completes successfully  
- ✅ Pages URL becomes accessible
- ✅ Documentation content displays correctly

**GitHub Pages URL format:**
`https://[username].github.io/[repository-name]/`

For LoadSharp: `https://sjefvanleeuwen.github.io/loadsharp/`

## 🆘 Still Having Issues?

If problems persist:

1. **Enable simple-docs.yml** workflow (disable documentation.yml temporarily)
2. **Check repository permissions** are correctly set
3. **Verify GitHub Pages is enabled** in repository settings
4. **Review workflow logs** for specific error messages
5. **Test with a minimal HTML file** first

The simple documentation workflow should resolve most permission and dependency issues while providing immediate documentation deployment.
