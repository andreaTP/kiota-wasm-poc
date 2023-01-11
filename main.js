import { dotnet } from './dotnet.js'

const { getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

// const msg = await exports.KiotaJs.Generate("test");
// console.log(msg);
const data = await exports.KiotaJs.Generate("test");

var element = document.createElement('a');
// Base64 approach from: https://stackoverflow.com/a/51759464
element.setAttribute('href', 'data:text/plain;base64,' + data);
element.download = 'kiota-test.zip';
element.innerText = 'Download';

document.body.appendChild(element);
