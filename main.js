import { dotnet } from './dotnet.js'

let exports;

export async function generateOnElement(name, element) {
    if (exports === undefined) {
        const { getAssemblyExports, getConfig } = await dotnet
        .withDiagnosticTracing(false)
        .withApplicationArgumentsFromQuery()
        .create();
    
        const config = getConfig();
        exports = await getAssemblyExports(config.mainAssemblyName);
    }

    try {
        const data = await exports.KiotaJs.Generate(name);
        // Base64 approach from: https://stackoverflow.com/a/51759464
        element.setAttribute('href', 'data:text/plain;base64,' + data);
        element.download = 'kiota-test.zip';
        element.innerText = 'Download';
    } catch(e) {
        element.innerText = 'Error';
        element.setAttribute('aria-disabled', 'true');
        console.error("Exception generating API client");
        console.error(e);
    }
}

var element = document.createElement('a');
document.body.appendChild(element);
generateOnElement("test", element);
