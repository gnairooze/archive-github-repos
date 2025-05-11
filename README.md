# ArchiveRepos

A .NET 8 console application to clone all repositories from your GitHub account to your local file system.

## Features
- Authenticates with GitHub using a personal access token
- Fetches all repositories from your GitHub account
- Clones each repository to the current directory (skips if already cloned)

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Git](https://git-scm.com/downloads) installed and available in your system PATH
- A GitHub [personal access token](https://github.com/settings/tokens) with `repo` scope

## Configuration

### 1. Set GitHub Credentials as User Environment Variables
Open PowerShell and run the following commands (replace values with your actual credentials):

```powershell
[System.Environment]::SetEnvironmentVariable("GITHUB_USER", "your_github_username", "User")
[System.Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "your_github_token", "User")
```

- `GITHUB_USER`: Your GitHub username
- `GITHUB_TOKEN`: Your GitHub personal access token (keep this secret!)

**Note:** You may need to restart your terminal or log out and back in for the changes to take effect.

### 2. Verify Environment Variables
To confirm the variables are set:

```powershell
[Environment]::GetEnvironmentVariable("GITHUB_USER", "User")
[Environment]::GetEnvironmentVariable("GITHUB_TOKEN", "User")
```

## Build and Run

1. Open a terminal in the project root directory.
2. Build the project:
   ```powershell
   dotnet build ArchiveRepos
   ```
3. Run the app:
   ```powershell
   dotnet run --project ArchiveRepos
   ```

All your repositories will be cloned into the current directory. Existing folders will be skipped.

## Notes
- The app uses the GitHub API and will only fetch repositories accessible to your account.
- For organizations or more advanced filtering, you may need to modify the code.
- Your token should have at least `repo` scope for private repositories.

## Troubleshooting
- If you see authentication errors, double-check your token and username.
- Ensure `git` is installed and available in your system PATH.
- If you have many repositories, the app will paginate through them automatically.

## License
MIT 