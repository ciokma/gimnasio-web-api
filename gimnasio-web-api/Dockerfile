# Usa la imagen base de .NET 8 SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establece el directorio de trabajo dentro del contenedor
WORKDIR /app

# Copia el archivo .csproj y restaura las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código fuente y publica la aplicación
COPY . ./

RUN dotnet publish "gimnasio-web-api.csproj" -c Release -o out


# Usa la imagen base de ASP.NET Core Runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Establece el directorio de trabajo dentro del contenedor
WORKDIR /app

# Copia los archivos publicados desde la etapa de construcción
COPY --from=build /app/out ./

# Expone el puerto 5211
EXPOSE 5211

# Define el comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "gimnasio-web-api.dll"]