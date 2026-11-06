# Etapa 1: compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 as build

# 2. Párate en /src
WORKDIR /src
# 3. Copia los 4 .csproj de src, cada uno a su carpeta (ej: ProyectoML.Api/ProyectoML.Api.csproj -> ProyectoML.Api/)
COPY ProyectoML.Api/ProyectoML.Api.csproj 			ProyectoML.Api/
COPY ProyectoML.Application/ProyectoML.Application.csproj 	ProyectoML.Application/
COPY ProyectoML.Domain/ProyectoML.Domain.csproj 		ProyectoML.Domain/
COPY ProyectoML.Infrastructure/ProyectoML.Infrastructure.csproj ProyectoML.Infrastructure/

# 4. Ejecuta el restore SOLO del proyecto Api
run dotnet restore ProyectoML.Api/

# 5. Copia todo el resto del repo
COPY . .

# 6. Publica el proyecto Api en Release hacia /app (pista: dotnet publish ... -c Release -o /app --no-restore)
run dotnet publish ProyectoML.Api/ -c Release -o /app --no-restore

# Etapa 2: ejecutar

# 7. Parte desde la imagen aspnet
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS aspnet
# 8. Párate en /app
WORKDIR /app
# 9. Copia /app desde la etapa build
COPY --from=build /app .
# 10. Expón el 8080
expose 8080
# 11. Arranca con dotnet ProyectoML.Api.dll
ENTRYPOINT ["dotnet","ProyectoML.Api.dll"]
