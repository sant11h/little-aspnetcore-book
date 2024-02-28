FROM mcr.microsoft.com/dotnet/sdk:8.0.2 AS build
WORKDIR /source

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore /p:PublishWithAspNetCoreTargetManifest="false"  

FROM mcr.microsoft.com/dotnet/aspnet:8.0.2
ENV ASPNETCORE_URLS http://+:80
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AspNetCoreTodo.dll"]
