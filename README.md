# Hello, IceRPC

This repository contains the source code for `icerpc://hello.icerpc.dev`. This is a public IceRPC server that
is intended to be used by developers implementing an IceRPC library to test their implementation against a
a working "reference" server.

## Available Services

The following services are mapped to their default service paths and are available using the `TCP` and `QUIC` transports.

- [Slice Greeter](./src/Greeter.Slice/slice/Greeter.slice) - `icerpc://hello.icerpc.dev/VisitorCenter.Greeter`
  - Example client [C#](./src/Greeter.Slice/Client/Program.cs)
- [Protobuf Greeter](./src/Greeter.Protobuf/proto/greeter.proto) - `icerpc://hello.icerpc.dev/visitor_center.Greeter`

## Running the server locally with Docker Compose

To run the server locally, you can use Docker Compose. The following command will start the server using the
configuration in the [`docker-compose.yml`](./docker-compose.yml) file.

```bash
docker compose up
```

Once running the server will be available at `icerpc://localhost` using the `TCP` and `QUIC` transports on the default
port (4062).

## Configuration

The server can be configured though several environment variables:

- `LOG_LEVEL` - The [log level](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#log-level). Default: `Debug`
- `SERVER_CERT` - Path to the server certificate. Default: `/certs/server.p12`
- `USE_TLS_WITH_TCP` - Configure the usage of TLS with the TCP transport. Default: `true`

The QUIC transport requires TLS and is always enabled.
