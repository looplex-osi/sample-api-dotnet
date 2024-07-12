
AutoMapper
https://docs.automapper.org/en/stable/Getting-started.html

Handling errors
https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-8.0

Microsoft.Data.SqlClient (instead of System.Data.SqlClient)
https://devblogs.microsoft.com/dotnet/introducing-the-new-microsoftdatasqlclient/

Restfull API
https://restfulapi.net/
https://restfulapi.net/resource-naming/

Telemetry
https://opentelemetry.io/docs/languages/net/getting-started/

Serilog
https://serilog.net/
https://www.c-sharpcorner.com/article/how-to-implement-serilog-in-asp-net-core-web-api/

## Configure HTTPS docker

### Windows

### Mac

1. Make sure openssh is installed.
2. Run
    ```
   mkdir -p ~/.aspnet/https
   cd ~/.aspnet/https
   openssl genrsa -out aspnetapp.key 2048
   openssl req -new -key aspnetapp.key -out aspnetapp.csr
   openssl x509 -req -days 365 -in aspnetapp.csr -signkey aspnetapp.key -out aspnetapp.crt
   openssl pkcs12 -export -out aspnetapp.pfx -inkey aspnetapp.key -in aspnetapp.crt
   openssl pkcs12 -in aspnetapp.pfx -out aspnetapp.pem -nodes
   sudo security import aspnetapp.pem -k /Library/Keychains/System.keychain
    ```