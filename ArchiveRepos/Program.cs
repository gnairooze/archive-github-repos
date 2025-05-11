using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

// Replace the default Main with an async Main
await MainAsync();

static async Task MainAsync()
{
    const string githubApiUrl = "https://api.github.com/user/repos?per_page=100";
    string? githubUser = Environment.GetEnvironmentVariable("GITHUB_USER");
    string? githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

    if (string.IsNullOrWhiteSpace(githubUser) || string.IsNullOrWhiteSpace(githubToken))
    {
        Console.WriteLine("Please set GITHUB_USER and GITHUB_TOKEN environment variables.");
        return;
    }

    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ArchiveRepos", "1.0"));
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", githubToken);

    Console.WriteLine($"Fetching repositories for user: {githubUser}");

    var repos = new List<(string name, string cloneUrl)>();
    int page = 1;
    bool hasMore = true;

    while (hasMore)
    {
        var response = await httpClient.GetAsync($"{githubApiUrl}&page={page}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var arr = doc.RootElement;
        if (arr.GetArrayLength() == 0) break;
        foreach (var repo in arr.EnumerateArray())
        {
            string name = repo.GetProperty("name").GetString()!;
            string cloneUrl = repo.GetProperty("clone_url").GetString()!;
            repos.Add((name, cloneUrl));
        }
        page++;
        hasMore = arr.GetArrayLength() == 100;
    }

    Console.WriteLine($"Found {repos.Count} repositories. Starting clone...");

    foreach (var (name, cloneUrl) in repos)
    {
        string targetDir = Path.Combine(Directory.GetCurrentDirectory(), name);
        if (Directory.Exists(targetDir))
        {
            Console.WriteLine($"Skipping {name}: already exists.");
            continue;
        }
        Console.WriteLine($"Cloning {name}...");
        var psi = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"clone {cloneUrl} \"{targetDir}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        var proc = Process.Start(psi)!;
        proc.WaitForExit();
        if (proc.ExitCode == 0)
            Console.WriteLine($"Cloned {name}.");
        else
            Console.WriteLine($"Failed to clone {name}: {proc.StandardError.ReadToEnd()}");
    }

    Console.WriteLine("Done.");
}
