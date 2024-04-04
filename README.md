# Hello, IceRPC

This repository contains the source code for `icerpc://hello.icerpc.dev` — a publicly available
[IceRPC](https://docs.icerpc.dev) server intended for the testing and development of IceRPC clients.

## Available Services

The following services are mapped to their default service paths and are available using the `TCP` and `QUIC`
transports on the default port (`4062`):

| Service                                       | Path                                               | Description                           | Example Clients                                                                                                                                                                                 |
| --------------------------------------------- | -------------------------------------------------- | ------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [Slice Greeter](./src//slice/Greeter.slice)   | `/VisitorCenter.Greeter`  | A simple service that greets visitors | [C# Secure](https://github.com/icerpc/icerpc-csharp/tree/0.3.x/examples/slice/Secure/Client) <br>[C# QUIC](https://github.com/icerpc/icerpc-csharp/tree/0.3.x/examples/slice/Quic/Client)       |
| [Protobuf Greeter](./src/proto/greeter.proto) | `/visitor_center.Greeter` | A simple service that greets visitors | [C# Secure](https://github.com/icerpc/icerpc-csharp/tree/0.3.x/examples/protobuf/Secure/Client) <br>[C# QUIC](https://github.com/icerpc/icerpc-csharp/tree/0.3.x/examples/protobuf/Quic/Client) |

## Running the server using Docker

The `hello` server is available as a Docker image on [Docker Hub](https://hub.docker.com/r/icerpc/hello). The server
requires TLS certificates to run, so you will need to provide the server certificate and private key. This repository
contains a set of self-signed [certificates](./certs) that can be used for testing purposes.

The default server certificate and private key paths are `/certs/server_cert.pem` and `/certs/server_key.pem`
respectively. These paths can be overridden using the `SERVER_CERT` and `SERVER_KEY` environment variables.

### Docker CLI

```bash
docker run --name hello -p 4062:4062/tcp -p 4062:4062/udp -v /path/to/certificates/:/certs icerpc/hello
```

Optional environment variables can be used to configure the server:

```bash
docker run \
  --name hello \
  -p 4062:4062/tcp -p 4062:4062/udp \
  -v /path/to/certificates/:/certs \
  -e LOG_LEVEL= \
  -e SERVER_CERT= \
  -e SERVER_KEY= \
  -e USE_TLS_WITH_TCP=true icerpc/hello
```

### Docker Compose

```yml
services:
  hello:
    image: icerpc/hello
    ports:
      - "4062:4062/tcp"
      - "4062:4062/udp"
    environment:
      - LOG_LEVEL= #optional
      - SERVER_CERT= #optional
      - SERVER_KEY= #optional
      - USE_TLS_WITH_TCP= #optional
    volumes:
      - /path/to/certificates/:/certs

```

### Configuration

The server can be configured through several environment variables:

| Environment Variable | Description                                                                                                   | Default Value            |
| -------------------- | ------------------------------------------------------------------------------------------------------------- | ------------------------ |
| `LOG_LEVEL`          | The [log level](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#log-level) | `Debug`                  |
| `SERVER_CERT`        | Path to the server certificate                                                                                | `/certs/server_cert.pem` |
| `SERVER_KEY`         | Path to the server private key                                                                                | `/certs/server_key.pem`  |
| `USE_TLS_WITH_TCP`   | Configure the usage of TLS with the TCP transport                                                             | `true`                   |

Certificates are **required** to run the server as the QUIC transport requires TLS.

## Building with Docker Compose

This repository contains a [`Dockerfile`](./Dockerfile) that can be used to build the server as well as a
[`compose.yaml`](./compose.yaml) file that can be used to build and run the server locally
using Docker Compose.

*The following commands must be run from the root directory of the repository.*

```bash
# Start the server (builds the server if it does not exist)
docker compose up

# Rebuild the server
docker compose build
```

Once running, the server will be available at `icerpc://localhost` using the `TCP` and `QUIC` transports on the default
port (4062).

Please refer to the [Docker Compose documentation](https://docs.docker.com/compose/) for more information on how to
use Docker Compose.
