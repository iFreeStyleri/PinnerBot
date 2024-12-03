#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Pinner/Pinner.csproj", "Pinner/"]
COPY ["Pinner.DAL/Pinner.DAL.csproj", "Pinner.DAL/"]
RUN dotnet restore "Pinner/Pinner.csproj"
COPY . .
WORKDIR "/src/Pinner"
RUN dotnet build "Pinner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pinner.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

WORKDIR /app/publish
RUN apt-get update
RUN apt install -y software-properties-common
RUN apt-get update -y \
    && apt-get install python3 -y \
    && apt-get install python3-pip -y \
    && apt-get install python3-venv -y \
    && python3 -m venv venv

WORKDIR /src

COPY ["Pinner/requirements.txt", "./"]
RUN pip install --no-cache-dir -r requirements.txt


ENTRYPOINT ["dotnet", "Pinner.dll"]


