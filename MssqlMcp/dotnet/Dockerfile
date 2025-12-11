# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["MssqlMcp/MssqlMcp.csproj", "MssqlMcp/"]
RUN dotnet restore "MssqlMcp/MssqlMcp.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/MssqlMcp"
RUN dotnet build "MssqlMcp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MssqlMcp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Fix for SQL Server TLS/SSL handshake issues on Linux (OpenSSL 3)
# Create a custom openssl configuration that allows legacy renegotiation and lower security level
RUN echo 'openssl_conf = openssl_init' > /etc/ssl/openssl_custom.cnf && \
    echo '[openssl_init]' >> /etc/ssl/openssl_custom.cnf && \
    echo 'ssl_conf = ssl_sect' >> /etc/ssl/openssl_custom.cnf && \
    echo '[ssl_sect]' >> /etc/ssl/openssl_custom.cnf && \
    echo 'system_default = system_default_sect' >> /etc/ssl/openssl_custom.cnf && \
    echo '[system_default_sect]' >> /etc/ssl/openssl_custom.cnf && \
    echo 'Options = UnsafeLegacyRenegotiation' >> /etc/ssl/openssl_custom.cnf && \
    echo 'CipherString = DEFAULT@SECLEVEL=0' >> /etc/ssl/openssl_custom.cnf

# Set environment variable to use the custom configuration
ENV OPENSSL_CONF=/etc/ssl/openssl_custom.cnf

# Expose the port the app runs on
EXPOSE 3000
ENV ASPNETCORE_URLS=http://+:3000

# Set the entry point
ENTRYPOINT ["dotnet", "MssqlMcp.dll"]
