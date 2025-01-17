#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Looplex.DotNet.Samples.WebApi/Looplex.DotNet.Samples.WebApi.csproj", "src/Looplex.DotNet.Samples.WebApi/"]
RUN dotnet restore "src/Looplex.DotNet.Samples.WebApi/Looplex.DotNet.Samples.WebApi.csproj"
COPY . .
WORKDIR "/src/Looplex.DotNet.Samples.WebApi"
RUN dotnet build "Looplex.DotNet.Samples.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Looplex.DotNet.Samples.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Looplex.DotNet.Samples.WebApi.dll"]