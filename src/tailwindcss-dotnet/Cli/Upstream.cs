using System.Runtime.InteropServices;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tailwindcss.DotNetTool.Cli;

public class Upstream
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task<string> GetLatestReleaseVersionAsync()
    {
        var fallbackVersion = "v4.0.0";
        try
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("dotnet-tailwindcss");
            var response = await httpClient.GetAsync("https://api.github.com/repos/tailwindlabs/tailwindcss/releases/latest");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);
            var version = document.RootElement.GetProperty("tag_name").GetString();

            return version ?? fallbackVersion;
        }
       
        catch
        {
            return fallbackVersion;
        }
    }
    public static string Version{ get; } = GetLatestReleaseVersionAsync().GetAwaiter().GetResult();

    public static string? GetNativeExecutableName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "tailwindcss-windows-x64.exe";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => "tailwindcss-macos-arm64",
                Architecture.X64 => "tailwindcss-macos-x64",
                _ => null
            };
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => "tailwindcss-linux-arm64",
                Architecture.Arm => "tailwindcss-linux-armv7",
                Architecture.X64 => "tailwindcss-linux-x64",
                _ => null
            };
        }

        return null;
    }
}