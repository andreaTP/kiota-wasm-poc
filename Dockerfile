FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /workdir

RUN dotnet workload install wasm-tools

RUN git clone --depth 1 --branch main https://github.com/microsoft/kiota.git

WORKDIR /workdir/kiotajs

COPY . .

# Hack to statically inject the Kiota version String
RUN KIOTA_VERSION=$(cat /workdir/kiota/src/Kiota.Builder/Kiota.Builder.csproj | grep \<Version\> | sed s'/[[:blank:]]<Version>//' | sed s'/<\/Version>//') && \
    sed -i s"/Assembly.GetEntryAssembly().GetName().Version.ToString()/\"$KIOTA_VERSION\"/" /workdir/kiota/src/Kiota.Builder/Lock/KiotaLock.cs && \
    echo $KIOTA_VERSION

RUN dotnet build --configuration Release

FROM registry.access.redhat.com/ubi8/nginx-120

COPY --from=build --chown=1001:0 /workdir/kiotajs/dist /opt/app-root/src

USER 1001

CMD ["nginx", "-g", "daemon off;"]
