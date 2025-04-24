# syntax=docker/dockerfile:1
ARG DOTNET_VERSION=8.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS builder
WORKDIR /source

COPY "src/Application/VideoScreenshot.MessageConsumer/VideoScreenshot.MessageConsumer.csproj" "src/Application/VideoScreenshot.MessageConsumer/"
COPY "src/Application/VideoScreenshot.Application/VideoScreenshot.Application.csproj" "src/Application/VideoScreenshot.Application/"
COPY "src/Application/VideoScreenshot.Domain/VideoScreenshot.Domain.csproj" "src/Application/VideoScreenshot.Domain/"
COPY "src/Application/VideoScreenshot.Infrastructure/VideoScreenshot.Infrastructure.csproj" "src/Application/VideoScreenshot.Infrastructure/"

RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet restore "./src/Application/VideoScreenshot.MessageConsumer/VideoScreenshot.MessageConsumer.csproj"

COPY . .

RUN dotnet publish "/source/src/Application//VideoScreenshot.MessageConsumer/VideoScreenshot.MessageConsumer.csproj" \
      -c Release \
      -o /app/publish \
      -v:m \
      -p:TreatWarningsAsErrors=false \
      -p:NoWarn=NU1701

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

COPY --from=builder /app/publish .

RUN useradd -m appuser
USER appuser

EXPOSE 5148

ENTRYPOINT ["dotnet", "VideoScreenshot.MessageConsumer.dll"]