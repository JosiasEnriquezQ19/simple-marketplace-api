# SimpleMarketplace API — Documentación para integración con React

Este documento explica cómo ejecutar y usar la API backend (ASP.NET Core + EF Core + MySQL) desde una aplicación React. Incluye rutas principales, DTOs, ejemplos fetch y opciones para cargar datos iniciales (seed).

## Resumen rápido
- Base URL por defecto: `http://localhost:5184`
- Swagger: `http://localhost:5184/swagger` (solo en Development)
- CORS: ya permite `http://localhost:3000` (dev server de React)
- Autenticación: desactivada para pruebas; `Auth/login` devuelve `UsuarioDto` sin token
 - Soft-delete: DELETE no borra físicamente en varias entidades — marca `Estado = "eliminado"` (ver sección "Soft-delete").

---

## Requisitos previos
- .NET 8 SDK instalado
- MySQL corriendo (por defecto en localhost:3306)
- Cliente `mysql` si vas a importar `seed.sql` manualmente (opcional)

## Ejecutar la API (PowerShell)
1. Abrir PowerShell en el directorio del proyecto:

```powershell
cd "D:\4 CICLO\HERRAMIENTAS DE PROGRAMACION 2\Tienda\SimpleMarketplace.Api"
```

2. Compilar:

```powershell
dotnet build "D:\4 CICLO\HERRAMIENTAS DE PROGRAMACION 2\Tienda\SimpleMarketplace.Api\SimpleMarketplace.Api.csproj"
```

3. Ejecutar en primer plano y ver logs:

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --no-launch-profile --urls "http://localhost:5184" --project "D:\4 CICLO\HERRAMIENTAS DE PROGRAMACION 2\Tienda\SimpleMarketplace.Api\SimpleMarketplace.Api.csproj"
```

Si la app se inicia correctamente verás "Now listening on: http://localhost:5184" y Swagger disponible.

---

## Base URL y CORS
- URL base: `http://localhost:5184/api`
- CORS ya configurado para `http://localhost:3000`. Si tu React corre en otro puerto, edita `Program.cs` (política `AllowReactLocal`).

---

## Endpoints principales (resumen)
Rutas y cuerpos de ejemplo (JSON). Todas las rutas empiezan con `/api/`.

### Auth
- POST `/api/Auth/register`
  - Body: `{ "email": "x@x.com", "password": "Secret123!", "nombre": "Juan", "apellido": "Perez", "telefono": "987654321" }`
  - Respuesta: `201 Created` -> `UsuarioDto`

- POST `/api/Auth/login`
  - Body: `{ "email": "x@x.com", "password": "Secret123!" }`
  - Respuesta: `200 OK` -> `UsuarioDto` (sin token)

- GET `/api/Auth/me/{id}`
  - Respuesta: `200 OK` -> `UsuarioDto`

### Productos
- GET `/api/Productos` (opcional `?categoria=...`)
- GET `/api/Productos/{id}`
- POST `/api/Productos` (CrearProductoDto)
- PUT `/api/Productos/{id}`
- DELETE `/api/Productos/{id}`
  
Nota: DELETE en `Productos` realiza soft-delete (marca `Estado = "eliminado"). Los GETs list y GET by id filtran registros con `Estado == "eliminado"`.

### Carrito
- GET `/api/Carrito/usuario/{usuarioId}`
- POST `/api/Carrito/usuario/{usuarioId}` (AgregarCarritoDto: `{ productoId, cantidad }`)
- DELETE `/api/Carrito/{carritoId}`

### Direcciones
- GET `/api/Direcciones/usuario/{usuarioId}`
- POST `/api/Direcciones` (CrearDireccionDto)
- DELETE `/api/Direcciones/{id}`

Nota: DELETE en `Direcciones` es soft-delete (`Estado = "eliminado").

### Métodos de pago
- GET `/api/MetodosPago/usuario/{usuarioId}`
- POST `/api/MetodosPago` (CrearMetodoPagoDto)
- DELETE `/api/MetodosPago/{id}`

### Pedidos
- GET `/api/Pedidos/{id}`
- POST `/api/Pedidos/usuario/{usuarioId}` (CrearPedidoCompletoDto con `items`)
  - El servidor decrementa stock dentro de una transacción y calcula impuestos (18%).
  
Nota: la anulación de pedidos usa `Estado = "cancelado"` (soft-cancel); no borra físicamente el registro.

### Facturas
- GET `/api/Facturas/{id}`

### Administradores
- GET `/api/Administradores`
- GET `/api/Administradores/{id}`
- POST `/api/Administradores` (CrearAdministradorDto: incluye `password` que se hashea)
- DELETE `/api/Administradores/{id}`

Nota: DELETE en `Administradores` realiza soft-delete (`Estado = "eliminado"). `AdministradorDto` incluye la propiedad `Estado`.

### Configuraciones
- GET `/api/Configuraciones`
- POST `/api/Configuraciones` (CrearConfiguracionDto)

### LogsAdministrativos
- GET `/api/LogsAdministrativos`
- POST `/api/LogsAdministrativos` (CrearLogAdministrativoDto)

---

## DTOs clave (resumen)
- UsuarioDto: `{ UsuarioId, Email, Nombre, Apellido, Telefono?, Estado, FechaCreacion, FechaActualizacion }`
- CrearUsuarioDto: `{ Email, Password, Nombre, Apellido, Telefono? }`
- ProductoDto: `{ ProductoId, Nombre, Descripcion?, Precio, Stock, ImagenUrl, Categoria, Estado }`
- CrearProductoDto: `{ Nombre, Descripcion?, Precio, Stock, ImagenUrl, Categoria }`
- CrearPedidoCompletoDto: `{ DireccionId, MetodoPagoId, Items: [ { ProductoId, Cantidad } ] }`
- AdministradorDto / CrearAdministradorDto: ver `DTOs/AdministradorDtos.cs`
- ConfiguracionDto / CrearConfiguracionDto: ver `DTOs/ConfiguracionDtos.cs`
- LogAdministrativoDto / CrearLogAdministrativoDto: ver `DTOs/LogAdministrativoDtos.cs`

Soft-delete: las respuestas devuelven `Estado` para las entidades que lo usan (Usuarios, Productos, Direcciones, Administradores). Los endpoints GET filtran registros con `Estado = "eliminado"`.

---

## Archivos importantes (para editar o extender)
- `Program.cs` — CORS, DI, Swagger
- `Data/ApplicationDbContext.cs` — DbSets, índices, mapeos
- `Entities/*.cs` — modelos: `Usuario.cs`, `Producto.cs`, `Administrador.cs`, `Configuracion.cs`, `LogAdministrativo.cs`, etc.
- `DTOs/*.cs` — contratos JSON (peticiones/respuestas)
- `Mapping/MappingProfile.cs` — AutoMapper
- `Controllers/*.cs` — endpoints

---

## Cargar datos iniciales (seed)
Tienes dos opciones:

A) Ejecutar el script SQL directamente (más rápido):
- Guardar el script como `seed.sql` y ejecutar:
```powershell
mysql -u root -p SimpleMarketplaceDB < "C:\ruta\a\seed.sql"
```

B) Usar un DataSeeder en C# (recomendado si quieres hashing correcto y control desde la app):
- Puedo generar un `DataSeeder` que inserte los registros con BCrypt y lo ejecutes al arrancar. Dime si quieres que lo añada.

**Nota sobre hashes**: si ejecutas tu SQL con hashes de ejemplo, asegúrate que son hashes bcrypt válidos; si no, usa la opción B para generar hashes correctamente.

---

## Ejemplos de fetch desde React
Configurar base URL:
```js
const API = 'http://localhost:5184/api';
```
- Obtener productos:
```js
const res = await fetch(`${API}/Productos`);
const productos = await res.json();
```
- Registrar usuario:
```js
await fetch(`${API}/Auth/register`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password, nombre, apellido, telefono })
});
```
- Crear pedido (checkout):
```js
await fetch(`${API}/Pedidos/usuario/${userId}`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ direccionId, metodoPagoId, items })
});
```

---

## Notas y próximos pasos recomendados
- Si deseas seguridad real, reimplementa JWT y protege rutas sensibles (`Administradores`, `LogsAdministrativos`, etc.).
- Añadir paginación y filtros en `/api/Productos` para rendimiento.
- Implementar subida de imágenes si quieres almacenar imágenes locales o S3.
- Si quieres, puedo generar:
  - un `DataSeeder` que inserte los INSERTs usando EF y BCrypt (opción recomendada),
  - o una app React de ejemplo con páginas: listados, detalle, carrito y checkout.

Opciones relacionadas con soft-delete:
- Restauración (undo): puedo añadir endpoints `POST /api/{entity}/{id}/restore` para cambiar `Estado` de `eliminado` a `activo`.
- Filtro global: puedo implementar un filtro global en `ApplicationDbContext` para excluir `eliminado` automáticamente (requiere una interfaz base para las entidades con `Estado`).
- Extender soft-delete: si quieres, convierto también `MetodosPago`, `Configuraciones` o `CarritoItem` a soft-delete.

---

Si quieres que guarde también tu script SQL en el proyecto (ej. `seed/seed.sql`) o que genere el `DataSeeder`, dime cuál de las dos opciones prefieres y lo hago ahora.
