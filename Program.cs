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
    private static readonly ILogger<KiotaBuilder> consoleLogger = new ConsoleLogger();
    private static readonly CancellationTokenSource source = new CancellationTokenSource();
    private static readonly CancellationToken token = source.Token;

    [JSExport]
    internal async static Task<string> Generate(string url, string language, string clientClassName, string namespaceName)
    {
        Console.WriteLine($"Starting to Generate with parameters: {url}, {language}, {clientClassName}, {namespaceName}");

        var defaultConfiguration = new GenerationConfiguration();

        var hashedUrl = BitConverter.ToString(HashAlgorithm.Value!.ComputeHash(Encoding.UTF8.GetBytes(url))).Replace("-", string.Empty);
        string OutputPath = Path.Combine(Path.GetTempPath(), "kiota", "generation", hashedUrl);

        if (File.Exists(OutputPath))
        {
            Console.WriteLine("Deleting OutputPath");
            File.Delete(OutputPath);
        }
        Directory.CreateDirectory(OutputPath);

        if (!Enum.TryParse<GenerationLanguage>(language, out var parsedLanguage)) {
            throw new ArgumentOutOfRangeException($"Not supported language: {language}");
        }

        var generationConfiguration = new GenerationConfiguration
        {
            OpenAPIFilePath = url,
            IncludePatterns = defaultConfiguration.IncludePatterns,
            ExcludePatterns = defaultConfiguration.ExcludePatterns,
            Language = parsedLanguage,
            OutputPath = OutputPath,
            ClientClassName = clientClassName,
            ClientNamespaceName = namespaceName,
            IncludeAdditionalData = false,
            UsesBackingStore = false,
            Serializers = defaultConfiguration.Serializers,
            Deserializers = defaultConfiguration.Deserializers,
            StructuredMimeTypes = defaultConfiguration.StructuredMimeTypes,
            DisabledValidationRules = new(),
            CleanOutput = true,
            ClearCache = true,
        };

        var builder = new KiotaBuilder(consoleLogger, generationConfiguration, new HttpClient());

        var result = await builder.GenerateClientAsync(token).ConfigureAwait(false);

        var zipFilePath = Path.Combine(Path.GetTempPath(), "kiota", "clients", hashedUrl, "client.zip");
        if (File.Exists(zipFilePath))
            File.Delete(zipFilePath);
        else
            Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath)!);

        ZipFile.CreateFromDirectory(OutputPath, zipFilePath);

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
