# Guía de Ejecución Local de la API de SimpleMarketplace

Esta guía te guiará a través de los pasos necesarios para configurar la base de datos local y ejecutar la API de ASP.NET Core de forma local en tu máquina.

---

## 📋 Requisitos Previos

Asegúrate de tener instalados los siguientes componentes:

1. **.NET 8 SDK** (ya verificado e instalado en tu sistema, versión `8.0.126`).
2. **MySQL Server** (ya verificado y activo en tu sistema local, versión `8.0.45`).

---

## 🛠️ Paso 1: Configurar la Base de Datos Local

En Linux, el usuario `root` de MySQL suele usar autenticación por socket (`auth_socket`), lo que impide que la aplicación C# se conecte directamente. Para evitar esto, usaremos un usuario de base de datos dedicado llamado `marketplace_user` con contraseña `Marketplace123!`.

Hemos creado un script automatizado `restaurar_db.sh` en la raíz del proyecto para simplificar este proceso:

### Método Recomendado (Script Automático)

1. Ejecuta el script en tu terminal:
   ```bash
   ./restaurar_db.sh
   ```
2. Introduce tu contraseña de Linux/sudo cuando te lo solicite. El script creará la base de datos `SimpleMarketplaceDB3`, creará el usuario dedicado, le otorgará permisos, importará tu archivo `basededatos_actualizada_schema.sql` y configurará tu `appsettings.Development.json` automáticamente.

---

### Métodos Alternativos (Manuales)

#### Opción A: A través de EF Core

1. Crea un usuario con contraseña en tu MySQL local y agrégalo a tu cadena de conexión en `appsettings.Development.json`.
2. Aplica las migraciones:
   ```bash
   dotnet ef database update
   ```

#### Opción B: A través del script SQL

1. Crea la base de datos desde la CLI de MySQL usando sudo:
   ```bash
   sudo mysql -e "CREATE DATABASE IF NOT EXISTS SimpleMarketplaceDB3;"
   ```
2. Importa el esquema:
   ```bash
   sudo mysql SimpleMarketplaceDB3 < basededatos_actualizada_schema.sql
   ```
3. Crea un usuario con contraseña en MySQL, dale privilegios sobre `SimpleMarketplaceDB3` y actualiza la conexión en `appsettings.Development.json`.

---

## 🚀 Paso 2: Iniciar la API en Modo Desarrollo

Para levantar la API localmente y habilitar Swagger:

1. Define la variable de entorno:
   ```bash
   export ASPNETCORE_ENVIRONMENT=Development
   ```
2. Corre el proyecto:
   ```bash
   dotnet run
   ```

---

## 🔍 Paso 3: Probar la API

Una vez iniciada la aplicación, estará escuchando en el puerto local predeterminado:

- **Swagger UI:** [http://localhost:8080/swagger](http://localhost:8080/swagger) para probar los endpoints interactivamente.
- **Endpoints Base:** `http://localhost:8080/api/`
