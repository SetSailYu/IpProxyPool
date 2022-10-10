#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["ProxyPoolAPI/ProxyPoolAPI.csproj", "ProxyPoolAPI/"]
#COPY ["ProxyPool.Common/ProxyPool.Common.csproj", "ProxyPool.Common/"]
#COPY ["ProxyPool.Services/ProxyPool.Services.csproj", "ProxyPool.Services/"]
#COPY ["ProxyPool.Repository/ProxyPool.Repository.csproj", "ProxyPool.Repository/"]
#RUN dotnet restore "ProxyPoolAPI/ProxyPoolAPI.csproj"
#COPY . .
#WORKDIR "/src/ProxyPoolAPI"
#RUN dotnet build "ProxyPoolAPI.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "ProxyPoolAPI.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .

COPY . .
ENTRYPOINT ["dotnet", "ProxyPoolAPI.dll"]