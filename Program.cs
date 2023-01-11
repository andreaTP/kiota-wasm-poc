using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Kiota.Builder;
using Kiota.Builder.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.Versioning;

Console.WriteLine("Hello browser!");

[SupportedOSPlatform("browser")]
public partial class KiotaJs
{
    private static readonly ThreadLocal<HashAlgorithm> HashAlgorithm = new(() => SHA256.Create());
    private static string DescriptionUrl = "https://raw.githubusercontent.com/microsoft/kiota/main/tests/Kiota.Builder.IntegrationTests/ToDoApi.yaml";

    [JSExport]
    internal async static Task<string> Generate(string name)
    {
        Console.WriteLine("Starting to Generate " + name);

        var defaultConfiguration = new GenerationConfiguration();

        var hashedUrl = BitConverter.ToString(HashAlgorithm.Value!.ComputeHash(Encoding.UTF8.GetBytes(DescriptionUrl))).Replace("-", string.Empty);
        string OutputPath = Path.Combine(Path.GetTempPath(), "kiota", "generation", hashedUrl);

        if (File.Exists(OutputPath))
        {
            Console.WriteLine("Deleting OutputPath");
            File.Delete(OutputPath);
        }
        Directory.CreateDirectory(OutputPath);

        var generationConfiguration = new GenerationConfiguration
        {
            OpenAPIFilePath = DescriptionUrl,
            IncludePatterns = defaultConfiguration.IncludePatterns,
            ExcludePatterns = defaultConfiguration.ExcludePatterns,
            Language = GenerationLanguage.CSharp,
            OutputPath = OutputPath,
            ClientClassName = "ApiClient",
            ClientNamespaceName = "io.dummy",
            IncludeAdditionalData = false,
            UsesBackingStore = false,
            Serializers = defaultConfiguration.Serializers,
            Deserializers = defaultConfiguration.Deserializers,
            StructuredMimeTypes = defaultConfiguration.StructuredMimeTypes,
            CleanOutput = true,
            ClearCache = true,
        };

        var consoleLogger = new ConsoleLogger();

        var builder = new KiotaBuilder(consoleLogger, generationConfiguration, new HttpClient());

        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;

        try
        {
            var result = await builder.GenerateClientAsync(token).ConfigureAwait(false);
            // it's failing to write the final lock file ...
        }
        catch (Exception ex)
        {
            // TODO: it's failing to write the lock file need more investigation!
            Console.WriteLine(ex.StackTrace);
        }

        var zipFilePath = Path.Combine(Path.GetTempPath(), "kiota", "clients", hashedUrl, "client.zip");
        if (File.Exists(zipFilePath))
            File.Delete(zipFilePath);
        else
            Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath)!);

        ZipFile.CreateFromDirectory(OutputPath, zipFilePath);

        // Return Base64???
        byte[] fileBytes = File.ReadAllBytes(zipFilePath);
        string base64Content = System.Convert.ToBase64String(fileBytes);
        return base64Content;
    }
}

class ConsoleLogger : ILogger<KiotaBuilder>
{
    IDisposable? ILogger.BeginScope<TState>(TState state)
    {
        return new DummyDisposable();
    }

    bool ILogger.IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // dumb implementation
        Console.WriteLine(formatter(state, exception));
    }
}

class DummyDisposable : IDisposable
{
    public void Dispose()
    {
        // Do nothing
    }
}
