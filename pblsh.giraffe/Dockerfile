FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN git clone https://github.com/RReventlov/pblsh.giraffe.git -o pblsh.giraffe
RUN dotnet restore "pblsh.giraffe/pblsh.giraffe/pblsh.giraffe.fsproj"
COPY . .
WORKDIR "/src/pblsh.giraffe/pblsh.giraffe"
RUN dotnet build "pblsh.giraffe.fsproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "pblsh.giraffe.fsproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pblsh.giraffe.App.dll"]
