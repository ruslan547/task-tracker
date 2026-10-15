FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ../../TaskTracker.sln ./
COPY ../TaskTracker.Domain/TaskTracker.Domain.csproj src/TaskTracker.Domain/
COPY ../TaskTracker.Application/TaskTracker.Application.csproj src/TaskTracker.Application/
COPY ../TaskTracker.Infrastructure/TaskTracker.Infrastructure.csproj src/TaskTracker.Infrastructure/
COPY TaskTracker.API.csproj src/TaskTracker.API/
RUN dotnet restore src/TaskTracker.API/TaskTracker.API.csproj

COPY ../TaskTracker.Domain/ src/TaskTracker.Domain/
COPY ../TaskTracker.Application/ src/TaskTracker.Application/
COPY ../TaskTracker.Infrastructure/ src/TaskTracker.Infrastructure/
COPY . src/TaskTracker.API/
RUN dotnet publish src/TaskTracker.API/TaskTracker.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskTracker.API.dll"]
