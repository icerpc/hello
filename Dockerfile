FROM debian:13 AS base

# Install the base packages
RUN apt-get update \
    && apt-get upgrade -y \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        wget \
    && wget https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

FROM base AS build

# Install .NET 10 SDK on the build stage
RUN apt-get update \
    && apt-get install -y --no-install-recommends dotnet-sdk-10.0 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy the source code
WORKDIR /app
COPY . .

# Build the hello application
RUN dotnet build -c Release

FROM base

# Install .NET 10 runtime on the final stage
RUN apt-get update \
    && apt-get install -y --no-install-recommends libmsquic dotnet-runtime-10.0 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy the built application from the build stage
WORKDIR /app
COPY --from=build /app/src/Hello/bin/Release/net10.0/ .

# Expose the port for the TCP and QUIC transports
EXPOSE 4062/tcp
EXPOSE 4062/udp

# Run the hello application
ENTRYPOINT [ "./Hello" ]
