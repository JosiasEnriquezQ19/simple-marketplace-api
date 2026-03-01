# Configuración de Autenticación con Google

## ✅ Cambios Implementados

Se ha integrado exitosamente el inicio de sesión con Google OAuth 2.0 en tu API REST. Los cambios incluyen:

### 1. **Base de Datos**
- Se agregaron nuevas columnas a la tabla `Usuarios`:
  - `GoogleId` (varchar 255) - ID único de Google del usuario
  - `Provider` (varchar 50) - Proveedor de autenticación ("google", "local", etc.)
  - `ProfilePictureUrl` (varchar 500) - URL de la foto de perfil
  - `contraseñaHash` ahora es **nullable** para usuarios de Google

### 2. **Nuevo Endpoint**
```http
POST /api/Auth/google-login
Content-Type: application/json

{
  "idToken": "TOKEN_DE_GOOGLE_AQUI"
}
```

**Respuesta exitosa:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "usuario": {
    "usuarioId": 1,
    "email": "usuario@gmail.com",
    "nombre": "Juan",
    "apellido": "Pérez",
    "telefono": null,
    "estado": "activo",
    "provider": "google",
    "profilePictureUrl": "https://lh3.googleusercontent.com/...",
    "fechaCreacion": "2026-03-01T...",
    "fechaActualizacion": "2026-03-01T..."
  },
  "expiresAt": "2026-03-01T15:30:00Z"
}
```

**Nota:** El campo `token` es un JWT que debes incluir en el header `Authorization: Bearer {token}` para futuras peticiones autenticadas.

### 3. **Paquetes Instalados**
- `Google.Apis.Auth` v1.73.0

---

## 🔧 Configuración Requerida

### Paso 1: Obtener credenciales de Google

1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Crea un nuevo proyecto o selecciona uno existente
3. Habilita la **Google+ API** o **Google Identity**
4. Ve a **Credenciales** → **Crear credenciales** → **ID de cliente de OAuth 2.0**
5. Configura la pantalla de consentimiento:
   - Tipo de usuario: **Externo**
   - Nombre de la aplicación: `Simple Marketplace`
   - Email de soporte: tu email
6. Crea las credenciales de OAuth 2.0:
   - Tipo de aplicación: **Aplicación web**
   - Orígenes autorizados de JavaScript:
     ```
     http://localhost:3000
     http://localhost:5173
     https://tu-dominio.com
     ```
   - URIs de redirección autorizados:
     ```
     http://localhost:3000/auth/google/callback
     http://localhost:5173/auth/google/callback
     ```
7. Copia el **Client ID** (algo como: `123456789-abc123.apps.googleusercontent.com`)

### Paso 2: Configurar appsettings.json

Abre el archivo [appsettings.json](appsettings.json) y reemplaza el Client ID:

```json
{
  "Google": {
    "ClientId": "TU_GOOGLE_CLIENT_ID_AQUI.apps.googleusercontent.com"
  }
}
```

---

## 🚀 Uso en el Frontend

### Opción 1: React con Google Identity Services

```bash
npm install @react-oauth/google
```

```jsx
import { GoogleOAuthProvider, GoogleLogin } from '@react-oauth/google';

function App() {
  const handleGoogleSuccess = async (credentialResponse) => {
    try {
      const response = await fetch('http://localhost:5000/api/Auth/google-login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          idToken: credentialResponse.credential
        })
      });

      const userData = await response.json();
      console.log('Usuario autenticado:', userData);
      
      // Guardar datos del usuario en tu estado/contexto
      localStorage.setItem('user', JSON.stringify(userData));
      
    } catch (error) {
      console.error('Error al autenticar:', error);
    }
  };

  return (
    <GoogleOAuthProvider clientId="TU_GOOGLE_CLIENT_ID">
      <GoogleLogin
        onSuccess={handleGoogleSuccess}
        onError={() => console.log('Login Failed')}
        useOneTap
        text="signin_with"
        shape="rectangular"
        theme="filled_blue"
      />
    </GoogleOAuthProvider>
  );
}
```

### Opción 2: JavaScript Vanilla

```html
<!DOCTYPE html>
<html>
<head>
  <script src="https://accounts.google.com/gsi/client" async defer></script>
</head>
<body>
  <div id="g_id_onload"
       data-client_id="TU_GOOGLE_CLIENT_ID"
       data-callback="handleCredentialResponse">
  </div>
  <div class="g_id_signin" data-type="standard"></div>

  <script>
    async function handleCredentialResponse(response) {
      try {
        const res = await fetch('http://localhost:5000/api/Auth/google-login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            idToken: response.credential
          })
        });

        const userData = await res.json();
        console.log('Usuario autenticado:', userData);
              localStorage.setItem('user', JSON.stringify(userData.usuario));
              localStorage.setItem('token', userData.token);
        window.location.href = '/dashboard';
        
      } catch (error) {
        console.error('Error:', error);
      }
    }
  </script>
</body>
</html>
```

### Opción 3: Angular

```bash
npm install @abacritt/angularx-social-login
```

```typescript
import { SocialAuthService, GoogleLoginProvider, SocialUser } from '@abacritt/angularx-social-login';
import { HttpClient } from '@angular/common/http';

export class LoginComponent {
  constructor(
    private authService: SocialAuthService,
    private http: HttpClient
  ) {
    this.authService.authState.subscribe((user: SocialUser) => {
      if (user) {
        this.loginWithGoogle(user.idToken);
      }
    });
  }

  loginWithGoogle(idToken: string) {
    this.http.post('http://localhost:5000/api/Auth/google-login', { idToken })
      .subscribe({
        next: (userData) => {
          console.log('Usuario autenticado:', userData);
          localStorage.setItem('user', JSON.stringify(userData));
        },
        error: (error) => console.error('Error:', error)
      });
  }
}
```

---

## 🔒 Flujo de Autenticación

1. **Usuario hace clic en "Iniciar sesión con Google"** en el frontend
2. Google muestra el diálogo de selección de cuenta
3. Usuario selecciona su cuenta y autoriza la aplicación
4. Google devuelve un **ID Token** al frontend
5. Frontend envía el ID Token a tu API: `POST /api/Auth/google-login`
6. Tu API verifica el token con Google
7. **Si es nuevo usuario**: Se crea automáticamente en la base de datos
8. **Si ya existe**: Se devuelve la información del usuario existente
9. Frontend recibe los datos del usuario y guarda la sesión

---

## 🔐 Uso del Token JWT

Después del login exitoso, recibirás un JWT token que debes usar en futuras peticiones autenticadas.

### Cómo usar el token

```javascript
// Guardar el token después del login
const loginResponse = await fetch('http://localhost:5000/api/Auth/google-login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ idToken: googleToken })
});

const data = await loginResponse.json();
localStorage.setItem('token', data.token);
localStorage.setItem('user', JSON.stringify(data.usuario));
```

### Hacer peticiones autenticadas

```javascript
// Función helper para peticiones autenticadas
const fetchWithAuth = async (url, options = {}) => {
  const token = localStorage.getItem('token');
  
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  };
  
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  
  return fetch(url, { ...options, headers });
};

// Ejemplo: Obtener pedidos del usuario
const obtenerMisPedidos = async () => {
  const response = await fetchWithAuth('http://localhost:5000/api/Pedidos/usuario/1');
  const pedidos = await response.json();
  return pedidos;
};

// Ejemplo: Crear un nuevo pedido
const crearPedido = async (pedidoData) => {
  const response = await fetchWithAuth('http://localhost:5000/api/Pedidos', {
    method: 'POST',
    body: JSON.stringify(pedidoData)
  });
  return response.json();
};
```

### Verificar si el token está vigente

```javascript
// El token expira en 60 minutos por defecto
const tokenExpirado = () => {
  const expiresAt = localStorage.getItem('tokenExpires');
  if (!expiresAt) return true;
  return new Date() > new Date(expiresAt);
};

// Renovar token si está próximo a expirar
if (tokenExpirado()) {
  // Redirigir al login o renovar el token
  window.location.href = '/login';
}
```

---

## 🧪 Pruebas

### Con cURL (necesitas un token válido)
```bash
# 1. Hacer login con Google (necesitas un idToken real)
curl -X POST http://localhost:5000/api/Auth/google-login \
  -H "Content-Type: application/json" \
  -d '{"idToken": "TOKEN_REAL_DE_GOOGLE"}'

# Respuesta incluye el token JWT:
# {"token": "eyJhbGc...", "usuario": {...}, "expiresAt": "..."}

# 2. Usar el token para peticiones autenticadas
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

curl -X GET http://localhost:5000/api/Pedidos/usuario/1 \
  -H "Authorization: Bearer $TOKEN"
```

### Verificar cambios en la BD
```sql
SELECT UsuarioId, Email, Nombre, GoogleId, Provider, ProfilePictureUrl 
FROM Usuarios 
WHERE Provider = 'google';
```

---

## ⚠️ Notas Importantes

1. **Contraseñas**: Los usuarios que se registran con Google no tienen contraseña. Si intentan usar el endpoint `/api/Auth/login` normal, recibirán el mensaje "Esta cuenta usa autenticación de Google".

2. **Migración de usuarios**: Si un usuario ya existe con el mismo email pero sin GoogleId, al hacer login con Google se actualizará automáticamente agregando su GoogleId y Provider.

3. **Seguridad**: El token de Google se verifica en el servidor contra los servicios de Google, no se confía solo en el cliente.

4. **Email único**: Google garantiza que los emails son únicos y verificados, por lo que no necesitas validación adicional.

---

## 🔄 Próximos Pasos Sugeridos

1. **Implementar JWT**: Devolver un token JWT propio después del login de Google para mantener la sesión
2. **Agregar más proveedores**: Facebook, GitHub, Microsoft, etc.
3. **Vincular cuentas**: Permitir que usuarios conecten múltiples métodos de autenticación
4. **Refresh tokens**: Implementar tokens de actualización para sesiones largas
5. **Roles y permisos**: Integrar con tu sistema de autorización existente

---

## 📞 Soporte

Si encuentras algún problema:
1. Verifica que el Client ID en `appsettings.json` sea correcto
2. Asegúrate de que los orígenes autorizados en Google Cloud Console coincidan con tu frontend
3. Revisa los logs del servidor para más detalles sobre errores
4. Verifica que la base de datos tenga las columnas correctas

¡La integración está completa y lista para usar! 🎉
