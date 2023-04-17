#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
COPY ["ApplicationServices/ApplicationServices.csproj", "ApplicationServices/"]
COPY ["DB/DB.csproj", "DB/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Enums/Enums.csproj", "Enums/"]
COPY ["Interfaces/Interfaces.csproj", "Interfaces/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Utility/Utility.csproj", "Utility/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]