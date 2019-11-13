FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["GuiaTuristicaManager/GuiaTuristicaManager.csproj", "GuiaTuristicaManager/"]
RUN dotnet restore "GuiaTuristicaManager/GuiaTuristicaManager.csproj"
COPY . .
WORKDIR "/src/GuiaTuristicaManager"
RUN dotnet build "GuiaTuristicaManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GuiaTuristicaManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GuiaTuristicaManager.dll"]