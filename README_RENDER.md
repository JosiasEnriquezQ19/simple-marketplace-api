# Configuración de Variables de Entorno para Render

Este documento contiene las variables de entorno exactas que necesitas configurar en tu servicio web de **Render** para desplegar la API de MiTiendaPlus (`simple-marketplace-api`).

En ASP.NET Core, la configuración jerárquica del archivo `appsettings.json` se traduce a variables de entorno utilizando doble guion bajo (`__`) para separar los niveles.

## Lista de Variables para copiar y pegar en Render (Environment Variables)

Agrega cada una de estas variables en la sección **"Environment"** de tu servicio web en el panel de control de Render.

| Clave (Key) | Valor (Value) |
| :--- | :--- |
| `ConnectionStrings__DefaultConnection` | `server=yamabiko.proxy.rlwy.net;port=40667;database=railway;user=root;password=PIsrEETywIAyCAERackhfPvXFhgbcExa;` |
| `Google__ClientId` | `565092651331-aq6gmrgbms3jci0jr2oan3l56d9ialqs.apps.googleusercontent.com` |
| `Jwt__Key` | `SimpleMarketplace2026SecretKeyForJWTTokenGeneration123456` |
| `Jwt__Issuer` | `SimpleMarketplaceApi` |
| `Jwt__Audience` | `SimpleMarketplaceClients` |
| `Jwt__ExpireMinutes` | `60` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

---

### Detalles de la configuración:

1. **Base de Datos (Railway):** 
   La clave `ConnectionStrings__DefaultConnection` sobrescribe la configuración local y le dice a tu API en producción que se conecte directamente al servidor MySQL de Railway que acabamos de vincular.

2. **Google Auth:**
   La clave `Google__ClientId` contiene el identificador público generado desde Google Cloud Console para permitir a tus usuarios iniciar sesión con su cuenta de Google.

3. **Autenticación (JWT):**
   Las claves `Jwt__*` configuran tu token de seguridad. Contienen la llave secreta, quién emite el token, el público esperado y el tiempo de expiración (60 minutos).

4. **Entorno de ASP.NET:**
   Es recomendable incluir la variable `ASPNETCORE_ENVIRONMENT` definida en `Production` para decirle a tu aplicación que deshabilite funciones locales de desarrollo y optimice su rendimiento para la web pública.
