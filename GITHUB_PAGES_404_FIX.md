# 🔧 GitHub Pages 404 Error - Immediate Fix Guide

## 🚨 GitHub Pages 404 Error at https://sjefvanleeuwen.github.io/loadsharp/

The 404 error indicates that GitHub Pages is not properly configured or the documentation workflow hasn't deployed successfully.

## 🎯 IMMEDIATE ACTIONS REQUIRED

### Step 1: Configure GitHub Repository Settings

**CRITICAL:** You must configure these settings in your GitHub repository:

#### A. Enable GitHub Pages
1. Go to: `https://github.com/sjefvanleeuwen/loadsharp/settings/pages`
2. **Source:** Select `GitHub Actions` (NOT "Deploy from a branch")
3. Click **Save**

#### B. Set Workflow Permissions
1. Go to: `https://github.com/sjefvanleeuwen/loadsharp/settings/actions`
2. Under **Workflow permissions:**
   - ☑️ Select `Read and write permissions`
   - ☑️ Check `Allow GitHub Actions to create and approve pull requests`
3. Click **Save**

### Step 2: Check GitHub Actions Status

Visit: `https://github.com/sjefvanleeuwen/loadsharp/actions`

Look for:
- ✅ **Documentation** workflow runs
- ✅ **Deploy Documentation to Pages** workflow runs
- ❌ Any failed workflow runs

### Step 3: Trigger Documentation Deployment

If no documentation workflows have run, trigger one manually:

```bash
# Option 1: Push an empty commit to trigger workflows
git commit --allow-empty -m "🚀 Trigger GitHub Pages deployment"
git push origin main

# Option 2: Manually trigger the workflow
# Go to: https://github.com/sjefvanleeuwen/loadsharp/actions
# Click on "Documentation" or "Deploy Documentation to Pages"
# Click "Run workflow" button
```

## 🔍 Troubleshooting Steps

### Check 1: Repository Settings
- Pages Source must be "GitHub Actions"
- Workflow permissions must be "Read and write"

### Check 2: Workflow Files
We have two documentation workflows:
- `documentation.yml` (DocFX-based)
- `simple-docs.yml` (Simple HTML)

### Check 3: Workflow Execution
Look in Actions tab for:
- Build job success ✅
- Deploy job success ✅
- Pages deployment success ✅

## 🚀 Quick Fix: Use Simple Documentation

If the DocFX workflow is failing, we can enable the simple documentation workflow:

1. **Disable documentation.yml** (rename it temporarily)
2. **Enable simple-docs.yml** workflow
3. **Push a commit** to trigger deployment

## 📊 Expected Timeline

After configuring settings and triggering a workflow:
- **Workflow execution:** 2-5 minutes
- **Pages deployment:** 2-10 minutes
- **DNS propagation:** Up to 10 minutes

Total time: **5-25 minutes** for the site to be accessible

## ✅ Verification

Once fixed, you should see:
- ✅ Successful workflow runs in Actions tab
- ✅ Pages URL shows content instead of 404
- ✅ Documentation displays correctly

---

**The main issue is likely that GitHub Pages hasn't been enabled with "GitHub Actions" as the source in repository settings.**
