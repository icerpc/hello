FROM mcr.microsoft.com/devcontainers/dotnet:10.0-noble

USER vscode
WORKDIR /home/vscode

# Install necessary packages
RUN set -eux \
    && wget -q https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && sudo dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && sudo apt update \
    && sudo apt-get install -y libmsquic \
    && sudo rm -rf /var/lib/apt/lists/* \
    && sudo apt-get clean
