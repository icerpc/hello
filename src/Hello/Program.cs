// Copyright (c) ZeroC, Inc.

using IceRpc;
using IceRpc.Transports.Quic;
using Microsoft.Extensions.Logging;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

// The log level for the logger factory
var logLevel = Enum.Parse<LogLevel>(Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Debug");
// The path to the server certificate used for TLS.
var serverCert = Environment.GetEnvironmentVariable("SERVER_CERT") ?? "/certs/server.p12";
// Whether to use TLS with the TCP transport.
var useTlsWithTcp = bool.Parse(Environment.GetEnvironmentVariable("USE_TLS_WITH_TCP") ?? "true");

// Create server authentication options with the server certificate.
SslServerAuthenticationOptions sslAuthenticationOptions =  new SslServerAuthenticationOptions
{
    ServerCertificate = new X509Certificate2(serverCert)
};

// Create a simple console logger factory and configure the log level for category IceRpc.
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    builder
    .AddSimpleConsole()
    .AddFilter("IceRpc", logLevel));

// Create a router (dispatch pipeline), install two middleware and map the Slice and Protobuf Chatbot greeter services
// to their default paths.
var router = new Router()
    .UseLogger(loggerFactory)
    .UseDeadline()
    .Map<Greeter.Protobuf.IGreeterService>(new Greeter.Protobuf.Chatbot())
    .Map<Greeter.Slice.IGreeterService>(new Greeter.Slice.Chatbot());

// Create a server that uses the TCP transport on the default port (4062).
await using var tcpSever = new Server(
    router,
    useTlsWithTcp ? sslAuthenticationOptions: null,
    logger: loggerFactory.CreateLogger<Server>());

tcpSever.Listen();

// Create a server that uses the QUIC transport on the default port (4062).
await using var quicServer = new Server(
        router,
        sslAuthenticationOptions,
        logger: loggerFactory.CreateLogger<Server>(),
        multiplexedServerTransport: new QuicServerTransport());

quicServer.Listen();

// Wait until the console receives a Ctrl+C.
await CancelKeyPressed;
// Shutdown the servers.
await Task.WhenAll(tcpSever.ShutdownAsync(), quicServer.ShutdownAsync());
