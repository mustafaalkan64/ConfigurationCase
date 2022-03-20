#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ConfigurationCase.ConfigurationSource/ConfigurationCase.ConfigurationSource.csproj", "ConfigurationCase.ConfigurationSource/"]
RUN dotnet restore "ConfigurationCase.ConfigurationSource/ConfigurationCase.ConfigurationSource.csproj"
COPY . .
WORKDIR "/src/ConfigurationCase.ConfigurationSource"
RUN dotnet build "ConfigurationCase.ConfigurationSource.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConfigurationCase.ConfigurationSource.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigurationCase.ConfigurationSource.dll"]