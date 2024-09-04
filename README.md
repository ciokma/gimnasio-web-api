# gimnasio
Web API del sistema de gimnasio que contendra los metodos relevantes para ser consumidos desde otra aplicacion creada en REACTJS.

## Crear proyecto
dotnet new webapi -n gimnasio-web-api

### Crear Controladores
dotnet-aspnet-codegenerator controller -name "NombreControlador"Controller -async -api -m "Modelo" -dc "NombreContexto" -outDir Controllers

## Construir Proyecto
dotnet build

## Correr proyecto
dotnet run

## VERSION ACTUAL
1.0.1