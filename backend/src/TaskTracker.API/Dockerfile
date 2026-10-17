FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/TaskTracker.Domain/TaskTracker.Domain.csproj src/TaskTracker.Domain/
COPY src/TaskTracker.Application/TaskTracker.Application.csproj src/TaskTracker.Application/
COPY src/TaskTracker.Infrastructure/TaskTracker.Infrastructure.csproj src/TaskTracker.Infrastructure/
COPY src/TaskTracker.API/TaskTracker.API.csproj src/TaskTracker.API/
RUN dotnet restore src/TaskTracker.API/TaskTracker.API.csproj

COPY src/TaskTracker.Domain/ src/TaskTracker.Domain/
COPY src/TaskTracker.Application/ src/TaskTracker.Application/
COPY src/TaskTracker.Infrastructure/ src/TaskTracker.Infrastructure/
COPY src/TaskTracker.API/ src/TaskTracker.API/
RUN dotnet publish src/TaskTracker.API/TaskTracker.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskTracker.API.dll"]
