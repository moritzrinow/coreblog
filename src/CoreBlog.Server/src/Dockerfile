FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

COPY Directory.Build.props ./Directory.Build.props

COPY CoreBlog.Shared/src/*.csproj ./CoreBlog.Shared/src/
COPY CoreBlog.Server/src/*.csproj ./CoreBlog.Server/src/

RUN dotnet restore CoreBlog.Server/src/CoreBlog.Server.csproj

FROM restore AS build
WORKDIR /src

COPY CoreBlog.Shared ./CoreBlog.Shared
COPY CoreBlog.Server ./CoreBlog.Server

RUN dotnet publish CoreBlog.Server/src/CoreBlog.Server.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /coreblog
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CoreBlog.Server.dll"]
