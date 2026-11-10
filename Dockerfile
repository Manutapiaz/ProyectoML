FROM mcr.microsoft.com/dotnet/sdk:10.0 as build

WORKDIR /src

COPY ProyectoML.Api/ProyectoML.Api.csproj 			ProyectoML.Api/
COPY ProyectoML.Application/ProyectoML.Application.csproj 	ProyectoML.Application/
COPY ProyectoML.Domain/ProyectoML.Domain.csproj 		ProyectoML.Domain/
COPY ProyectoML.Infrastructure/ProyectoML.Infrastructure.csproj ProyectoML.Infrastructure/

run dotnet restore ProyectoML.Api/

COPY . .



run dotnet publish ProyectoML.Api/ -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS aspnet

WORKDIR /app

COPY --from=build /app .

expose 8080

ENTRYPOINT ["dotnet","ProyectoML.Api.dll"]
