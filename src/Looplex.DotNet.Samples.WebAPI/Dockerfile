#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Looplex.DotNet.Samples.WebAPI/Looplex.DotNet.Samples.WebAPI.csproj", "src/Looplex.DotNet.Samples.WebAPI/"]
RUN dotnet restore "src/Looplex.DotNet.Samples.WebAPI/Looplex.DotNet.Samples.WebAPI.csproj"
COPY . .
WORKDIR "/src/Looplex.DotNet.Samples.WebAPI"
RUN dotnet build "Looplex.DotNet.Samples.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Looplex.DotNet.Samples.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Looplex.DotNet.Samples.WebAPI.dll"]