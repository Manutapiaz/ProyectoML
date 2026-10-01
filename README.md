\# ProyectoML



API REST para gestionar productos e inventario de un vendedor de Mercado Libre: catálogo con stock trazable, publicación de productos y sincronización automática de stock a partir de las ventas.



\## Funcionalidades

\- \[x] Categorías y productos

\- \[x] Stock trazable por movimientos (ingresos, ventas, ajustes)

\- \[x] Manejo de errores estandarizado (ProblemDetails)

\- \[x] Tests unitarios de reglas de negocio

\- \[ ] Publicación de productos en Mercado Libre (OAuth)

\- \[ ] Webhook de órdenes y sincronización de stock



\## Stack

\- .NET 10 / ASP.NET Core Web API

\- Entity Framework Core 10 (Code First + migraciones)

\- SQL Server

\- xUnit



\## Arquitectura

Clean Architecture: el dominio no depende de ninguna capa externa, y las dependencias apuntan hacia adentro.



```mermaid

graph LR

&#x20;   Api --> Application

&#x20;   Api --> Infrastructure

&#x20;   Infrastructure --> Application

&#x20;   Application --> Domain

```



| Capa | Responsabilidad |

|---|---|

| \*\*Domain\*\* | Entidades con sus reglas de negocio y validaciones. Sin dependencias. |

| \*\*Application\*\* | Casos de uso (services), DTOs e interfaces de repositorios. |

| \*\*Infrastructure\*\* | Acceso a datos con EF Core: DbContext, configuraciones y repositorios. |

| \*\*Api\*\* | Controllers, manejo global de errores y configuración de inyección de dependencias. |



\## Decisiones de diseño



\*\*El stock solo cambia a través de movimientos.\*\*

No existe un setter de stock: todo cambio pasa por `Producto.RegistrarMovimiento()`, que valida la operación y registra un `MovimientoStock`. Así el stock siempre se puede auditar y reconstruir desde su historial.



\*\*Entidades que se protegen a sí mismas.\*\*

Las propiedades tienen `private set` y las validaciones están en los constructores y métodos de negocio. No se puede crear ni dejar una entidad en un estado inválido.



\*\*Separación entre entidades y DTOs.\*\*

Los repositorios trabajan solo con entidades; los services convierten a DTOs; los controllers nunca exponen entidades. La API puede cambiar sin afectar al dominio, y viceversa.



\*\*Errores de negocio mapeados a códigos HTTP.\*\*

Excepciones propias (`DomainException` → 400, `NotFoundException` → 404, `ConflictException` → 409) manejadas por un `IExceptionHandler` global que responde con ProblemDetails (RFC 9457). Sin `try/catch` repetidos en los controllers.



\*\*Integraciones detrás de interfaces.\*\*

El acceso a datos y los servicios externos se definen como interfaces en Application y se implementan en Infrastructure. La lógica de negocio se puede testear sin base de datos ni servicios externos.



\## Cómo ejecutarlo



\*\*Requisitos:\*\* .NET 10 SDK y SQL Server LocalDB (incluido con Visual Studio).



```bash

git clone https://github.com/Manutapiaz/ProyectoML.git

cd ProyectoML



dotnet tool install --global dotnet-ef

dotnet ef database update --project ProyectoML.Infrastructure --startup-project ProyectoML.Api

dotnet run --project ProyectoML.Api

```



La API queda disponible en la URL que muestra la consola. El archivo `ProyectoML.Api/ProyectoML.Api.http` tiene ejemplos de requests listos para ejecutar.



\## Endpoints



| Método | Ruta | Descripción |

|---|---|---|

| GET | `/api/categorias` | Lista todas las categorías |

| GET | `/api/categorias/{id}` | Obtiene una categoría |

| POST | `/api/categorias` | Crea una categoría |

| GET | `/api/productos` | Lista todos los productos |

| GET | `/api/productos/{id}` | Obtiene un producto |

| POST | `/api/productos` | Crea un producto, con stock inicial opcional |



\## Tests



```bash

dotnet test

```



Cubren las reglas de negocio de `Producto`: validaciones del constructor, cambios de precio y registro de movimientos de stock (ingresos, ventas, ajustes y casos inválidos, verificando que la entidad no quede en un estado inconsistente).



\## Próximos pasos

\- Integración con Mercado Libre: OAuth, publicación y webhook de órdenes

\- Docker Compose para levantar API y base de datos con un comando

\- Autenticación JWT

\- Emisión de documentos tributarios electrónicos (SII)

