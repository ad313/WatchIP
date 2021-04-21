#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WatchIP/WatchIP.csproj", "WatchIP/"]
RUN dotnet restore "WatchIP/WatchIP.csproj"
COPY . .
WORKDIR "/src/WatchIP"
RUN dotnet build "WatchIP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WatchIP.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime    
RUN echo "Asia/Shanghai" > /etc/timezone  
ENTRYPOINT ["dotnet", "WatchIP.dll"]