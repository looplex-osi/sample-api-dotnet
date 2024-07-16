# Samples WebApi

This project is ready to run/debug on docker (using docker-compose profile). 

## Dependencies

### Looplex OSI
1. [OpenForExtensions](https://github.com/looplex-osi/open-for-extension-dotnet)
1. [BackendCore](https://github.com/looplex-osi/backend-core-dotnet)
1. [Middlewares](https://github.com/looplex-osi/middlewares-dotnet)
1. [Services](https://github.com/looplex-osi/services-dotnet)
1. [Extensions](https://github.com/looplex-osi/extensions-dotnet)

## References

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

1. Choose a local folder and add it as a system variable named HOME.
1. Run
   ```
   cd ${HOME}
   mkdir .aspnet\https
   cd .aspnet\https
   dotnet dev-certs https -ep aspnetapp.pfx -p looplex
   dotnet dev-certs https --trust
   ```

### Mac

1. Make sure openssh is installed.
1. Run
   ```
   mkdir -p ~/.aspnet/https
   cd ~/.aspnet/https
   openssl genrsa -out aspnetapp.key 2048
   ```
   For the next command, you can leave all field empty:
   ```
   openssl req -new -key aspnetapp.key -out aspnetapp.csr
   openssl x509 -req -days 365 -in aspnetapp.csr -signkey aspnetapp.key -out aspnetapp.crt
   
   ```
   When exporting the pfx, set looplex as password:
   ```
   openssl pkcs12 -export -out aspnetapp.pfx -inkey aspnetapp.key -in aspnetapp.crt
   openssl pkcs12 -in aspnetapp.pfx -out aspnetapp.pem -nodes
   sudo security import aspnetapp.pem -k /Library/Keychains/System.keychain
   ```