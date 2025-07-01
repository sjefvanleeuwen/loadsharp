# 🔧 GitHub Actions Troubleshooting Guide

## ✅ Permission Issues Fixed

The original error:
```
remote: Permission to sjefvanleeuwen/loadsharp.git denied to github-actions[bot].
fatal: unable to access 'https://github.com/sjefvanleeuwen/loadsharp/': The requested URL returned error: 403
```

Has been resolved with the following changes:

### 🛠️ Applied Fixes

1. **Added Workflow Permissions** in `.github/workflows/build-and-test.yml`:
   ```yaml
   permissions:
     contents: write
     pull-requests: read
     actions: read
   ```

2. **Updated Checkout Configuration**:
   ```yaml
   - name: Checkout code
     uses: actions/checkout@v4
     with:
       token: ${{ secrets.GITHUB_TOKEN }}
       fetch-depth: 0
   ```

3. **Replaced Manual Git Commands** with Reliable Action:
   ```yaml
   - name: Commit test results
     uses: stefanzweifel/git-auto-commit-action@v5
     with:
       commit_message: "📊 Update test results and badges [skip ci]"
       file_pattern: "docs/test-results.md readme.md"
       commit_user_name: "github-actions[bot]"
       commit_user_email: "41898282+github-actions[bot]@users.noreply.github.com"
   ```

## 🔍 Repository Settings to Verify

### 1. Actions Permissions
Go to **Settings > Actions > General** and ensure:
- ✅ **Actions permissions**: "Allow all actions and reusable workflows"
- ✅ **Workflow permissions**: "Read and write permissions" 
- ✅ **Allow GitHub Actions to create and approve pull requests**: Checked

### 2. Branch Protection Rules
If you have branch protection on `main`:
- ✅ **Allow force pushes**: Enabled for administrators
- ✅ **Restrict pushes that create files**: Disabled
- ✅ **Do not allow bypassing the above settings**: Consider unchecking for GitHub Actions

### 3. Token Scopes
The default `GITHUB_TOKEN` should have sufficient permissions with the new setup.

## 🧪 Testing the Fix

1. **Make a small change** to any file
2. **Commit and push** to the main branch
3. **Check Actions tab** to see if the workflow runs successfully
4. **Verify** that test-results.md and README badges are updated automatically

## 🚨 Alternative Approaches

If issues persist, you can:

### Option A: Manual Badge Updates
Remove the auto-commit step and update badges manually or in separate PRs.

### Option B: Use Personal Access Token
1. Create a **Personal Access Token** with `repo` scope
2. Add it as repository secret named `PAT_TOKEN`  
3. Update checkout to use:
   ```yaml
   with:
     token: ${{ secrets.PAT_TOKEN }}
   ```

### Option C: Separate Workflow
Create a separate workflow that only runs badge updates on a schedule or manual trigger.

## ✅ Expected Behavior

After the fix, the workflow should:
1. ✅ Run tests and generate reports
2. ✅ Update badges in README.md automatically
3. ✅ Commit changes back to the repository
4. ✅ Skip CI on the auto-commit (due to `[skip ci]` message)

The `[skip ci]` prevents infinite loops of the workflow triggering itself.
