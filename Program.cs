using System;
// using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Kiota.Builder;
using Kiota.Builder.Configuration;
using Microsoft.Extensions.Logging;
// using System.IO.Compression;
// using Microsoft.Extensions.Logging;

Console.WriteLine("Hello browser!");

public partial class KiotaJs
{
    [JSExport]
    internal static Task<string> Generate(string name)
    {
        Console.WriteLine("Starting to Generate " + name);
        var generationConfiguration = new GenerationConfiguration{
            OpenAPIFilePath = "https://raw.githubusercontent.com/swagger-api/swagger-petstore/master/src/main/resources/openapi.yaml",
            IncludePatterns = new HashSet<string>(),
            ExcludePatterns = new HashSet<string>(),
            Language = GenerationLanguage.Java,
            OutputPath = "./test.zip",
            ClientClassName = "ApiClient",
            ClientNamespaceName = "io.dummy",
            IncludeAdditionalData = false,
            UsesBackingStore = false,
            Serializers = new HashSet<string>(),
            Deserializers = new HashSet<string>(),
            StructuredMimeTypes = new HashSet<string>(),
        };

        var consoleLogger = new ConsoleLogger();
        var builder = new KiotaBuilder(consoleLogger, generationConfiguration, new System.Net.Http.HttpClient());

        // bool result = builder.GenerateClientAsync(new()).Result;

        return builder.GenerateClientAsync(new()).ContinueWith(res => "Hello");
        // bool result = Task.Run(() => builder.GenerateClientAsync(new())).Result;
        // try {
        //     bool result = Task.Run(() => builder.GenerateClientAsync(new())).Result;
        //     var zipFilePath = Path.Combine(Path.GetTempPath(), "kiota", "clients", hashedUrl, "client.zip");
        //     if (File.Exists(zipFilePath))
        //         File.Delete(zipFilePath);
        //     else
        //         Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath)!);
        //     ZipFile.CreateFromDirectory(OutputPath, zipFilePath);

        //     Console.WriteLine("DEBUG!");
        //     Console.WriteLine(logBuilder.ToString());

        //     Console.WriteLine("ZipFile: " + zipFilePath);
        //     return zipFilePath;
        // } catch (Exception ex) {
        //     Console.WriteLine("Error " + ex);
        //     throw ex;
        // }
        // return "Hello world, " + name + " result is " + result;
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
