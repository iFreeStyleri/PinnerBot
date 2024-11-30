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
RUN apt-get install -y glob
RUN apt-get install -y regex
RUN apt-get install -y python

RUN pip install --upgrade pip
RUN pip freeze > requirements.txt
RUN pip install -r requirements.txt


ENTRYPOINT ["dotnet", "Pinner.dll"]


