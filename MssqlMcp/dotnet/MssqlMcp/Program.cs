// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Mssql.McpServer;

internal static class Program
{
    /// <summary>
    /// Entry point for the MCP server application.
    /// Sets up logging, configures the MCP server with standard I/O transport and tool discovery,
    /// builds the host, and runs the server asynchronously.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    private static async Task Main(string[] args)
    {
        // Create the application host builder
        var builder = WebApplication.CreateBuilder(args);

        // Configure console logging with Trace level
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        // Register ISqlConnectionFactory and Tools for DI
        builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        builder.Services.AddSingleton<Tools>();

        // Register MCP server and tools (instance-based)
        builder.Services
            .AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();

        // Build the host
        var app = builder.Build();

        app.MapMcp();

        await app.RunAsync();
    }
}
