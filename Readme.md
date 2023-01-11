# Example project to run Kiota generation in the browser [EXPERIMENTAL]

Here the following is going on:

- import Kiota as a dependency in a C# project
- expose the Client generation functionality to the Javascript environment
- run Kiota as a .NET WASM application in the browser

## Build

To build this project you need `dotnet` version 7+ and the `wasm-tools`:

```bash
dotnet workload install wasm-tools
```

The main [Kiota project](https://github.com/microsoft/kiota) is supposed to be available as a sibling folder.

Alternatively you can build it using Docker:

```bash
docker build . -t kiota-wasm-poc:latest
```

## Run

There are helper scripts in the `package.json` file, but, you simply need to serve the generated `./dist` folder with your web server of choice.
The Dockerfile is starting a default `nginx` server.

```bash
docker run --rm -p 8080:8080 kiota-wasm-poc:latest
```

## Use

With your browser of choice navigate to `http://localhost:8080` and use the simple demo UI.

## Debug

This project is highly experimental and, at the moment the output is exposed only through browser logs.
