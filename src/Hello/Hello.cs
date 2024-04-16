// Copyright (c) ZeroC, Inc.

using IceRpc;
using IceRpc.Transports.Quic;
using Microsoft.Extensions.Logging;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

// The log level for the logger factory
LogLevel logLevel = Enum.Parse<LogLevel>(Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Debug");

// The private key for the server certificate.
string serverKey = Environment.GetEnvironmentVariable("SERVER_KEY") ?? "/certs/server_key.pem";

// The server certificate (with the full chain) file.
string serverCert = Environment.GetEnvironmentVariable("SERVER_CERT") ?? "/certs/server_cert.pem";

// Whether to use TLS with the TCP transport.
bool useTlsWithTcp = bool.Parse(Environment.GetEnvironmentVariable("USE_TLS_WITH_TCP") ?? "true");

// Load server and intermediate certificates from the server certificate file.
using var serverCertificate = X509Certificate2.CreateFromPemFile(serverCert, serverKey);

// Create a collection with the server certificate and any intermediate certificates.
var intermediates = new X509Certificate2Collection();
intermediates.ImportFromPemFile(serverCert);

// Create server authentication options with the server certificate.
var sslAuthenticationOptions = new SslServerAuthenticationOptions
{
    ServerCertificateContext = SslStreamCertificateContext.Create(serverCertificate, intermediates),
};

// Create a simple console logger factory and configure the log level for category IceRpc.
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    builder
    .AddSimpleConsole(options =>
        {
            options.TimestampFormat = "[yyyy/MM/dd HH:mm:ss] ";
        })
    .AddFilter("IceRpc", logLevel));

// Create a router (dispatch pipeline), install two middleware and map the Slice and Protobuf Chatbot greeter services
// to their default paths.
Router router = new Router()
    .UseLogger(loggerFactory)
    .UseDeadline()
    .Map<Hello.Greeter.Protobuf.IGreeterService>(new Hello.Greeter.Protobuf.Chatbot())
    .Map<Hello.Greeter.Slice.IGreeterService>(new Hello.Greeter.Slice.Chatbot())
    .Map<Hello.Stream.Protobuf.IGeneratorService>(new Hello.Stream.Protobuf.RandomGenerator())
    .Map<Hello.Stream.Slice.IGeneratorService>(new Hello.Stream.Slice.RandomGenerator());

// Create a server that uses the TCP transport on the default port (4062).
await using var tcpServer = new Server(
    router,
    useTlsWithTcp ? sslAuthenticationOptions : null,
    logger: loggerFactory.CreateLogger<Server>());

tcpServer.Listen();

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
await Task.WhenAll(tcpServer.ShutdownAsync(), quicServer.ShutdownAsync());
