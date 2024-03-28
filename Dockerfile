FROM ubuntu:22.04 as base

# Ensure the Microsoft packages are preferred over the Ubuntu packages
RUN echo "Package: *\nPin: origin packages.microsoft.com\nPin-Priority: 1001" > /etc/apt/preferences.d/dotnet

# Install the base packages needed to install the Microsoft packages
RUN apt-get update && apt-get upgrade -y && apt-get install -y --no-install-recommends \
    curl ca-certificates \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Install the Microsoft deb
RUN curl -sSL https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -o packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb

# Install MsQuic
RUN apt-get update && apt-get install -y --no-install-recommends \
    libmsquic \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

FROM base as build

# Install the .NET 8 SDK
RUN apt-get update && apt-get install -y --no-install-recommends \
    dotnet-sdk-8.0 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy the source code
WORKDIR /app
COPY . .

# Build the hello application
RUN dotnet build -c Release

FROM base

# Install the .NET 8 runtime
RUN apt-get update && apt-get install -y --no-install-recommends \
    dotnet-runtime-8.0 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Copy the built application from the build stage
WORKDIR /app
COPY --from=build /app/src/Hello/bin/Release/net8.0/ .

# Expose the port for the TCP and QUIC transports
EXPOSE 4062/tcp
EXPOSE 4062/udp

# Run the hello application
ENTRYPOINT [ "./Hello" ]
