# 🔑 Guía de Tokens JWT en Simple Marketplace API

## ¿Qué cambió?

Ahora **TODOS** los endpoints de autenticación devuelven un JWT token junto con los datos del usuario:

- ✅ `POST /api/Auth/register` → Devuelve token + usuario
- ✅ `POST /api/Auth/login` → Devuelve token + usuario  
- ✅ `POST /api/Auth/google-login` → Devuelve token + usuario

## 📦 Estructura de Respuesta

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c3VhcmlvQGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJKdWFuIFDDqXJleiIsInByb3ZpZGVyIjoiZ29vZ2xlIiwiZXhwIjoxNzA5MzE2NjAwLCJpc3MiOiJTaW1wbGVNYXJrZXRwbGFjZUFwaSIsImF1ZCI6IlNpbXBsZU1hcmtldHBsYWNlQ2xpZW50cyJ9.signature",
  "usuario": {
    "usuarioId": 1,
    "email": "usuario@gmail.com",
    "nombre": "Juan",
    "apellido": "Pérez",
    "telefono": null,
    "estado": "activo",
    "provider": "google",
    "profilePictureUrl": "https://lh3.googleusercontent.com/a/photo.jpg",
    "fechaCreacion": "2026-03-01T14:30:00Z",
    "fechaActualizacion": "2026-03-01T14:30:00Z"
  },
  "expiresAt": "2026-03-01T15:30:00Z"
}
```

## 🔍 Decodificando el Token

El token JWT contiene tres partes separadas por puntos (`.`):

1. **Header** (información del algoritmo)
2. **Payload** (datos del usuario)
3. **Signature** (firma para verificar autenticidad)

### Contenido del Payload:

```json
{
  "nameid": "1",                    // ID del usuario
  "email": "usuario@gmail.com",     // Email del usuario
  "name": "Juan Pérez",             // Nombre completo
  "provider": "google",              // Proveedor de autenticación
  "exp": 1709316600,                 // Timestamp de expiración
  "iss": "SimpleMarketplaceApi",     // Emisor del token
  "aud": "SimpleMarketplaceClients"  // Audiencia
}
```

Puedes decodificar el token en [jwt.io](https://jwt.io) para ver su contenido.

## 💻 Uso en el Frontend

### 1. Guardar el token después del login

```javascript
const login = async (email, password) => {
  const response = await fetch('http://localhost:5000/api/Auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });

  const data = await response.json();
  
  // Guardar en localStorage
  localStorage.setItem('token', data.token);
  localStorage.setItem('user', JSON.stringify(data.usuario));
  localStorage.setItem('tokenExpires', data.expiresAt);
  
  return data;
};
```

### 2. Login con Google (igual)

```javascript
const googleLogin = async (googleIdToken) => {
  const response = await fetch('http://localhost:5000/api/Auth/google-login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ idToken: googleIdToken })
  });

  const data = await response.json();
  
  localStorage.setItem('token', data.token);
  localStorage.setItem('user', JSON.stringify(data.usuario));
  localStorage.setItem('tokenExpires', data.expiresAt);
  
  return data;
};
```

### 3. Hacer peticiones autenticadas

```javascript
// Helper function
const apiCall = async (endpoint, options = {}) => {
  const token = localStorage.getItem('token');
  
  const config = {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    }
  };
  
  // Agregar token si existe
  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  
  const response = await fetch(`http://localhost:5000${endpoint}`, config);
  
  // Si el token expiró (401), redirigir al login
  if (response.status === 401) {
    localStorage.clear();
    window.location.href = '/login';
    throw new Error('Token expirado');
  }
  
  return response;
};

// Ejemplos de uso:
const obtenerProductos = async () => {
  const response = await apiCall('/api/Productos');
  return response.json();
};

const crearPedido = async (pedidoData) => {
  const response = await apiCall('/api/Pedidos', {
    method: 'POST',
    body: JSON.stringify(pedidoData)
  });
  return response.json();
};

const obtenerMisPedidos = async (usuarioId) => {
  const response = await apiCall(`/api/Pedidos/usuario/${usuarioId}`);
  return response.json();
};
```

### 4. Verificar expiración del token

```javascript
const isTokenExpired = () => {
  const expiresAt = localStorage.getItem('tokenExpires');
  if (!expiresAt) return true;
  
  return new Date() > new Date(expiresAt);
};

// Usar en App.js o en un route guard
const checkAuth = () => {
  const token = localStorage.getItem('token');
  
  if (!token || isTokenExpired()) {
    // Redirigir al login
    window.location.href = '/login';
    return false;
  }
  
  return true;
};
```

### 5. Logout

```javascript
const logout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
  localStorage.removeItem('tokenExpires');
  window.location.href = '/login';
};
```

## 🛡️ Ejemplo con React Context

```jsx
import { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Cargar datos guardados al iniciar
    const savedToken = localStorage.getItem('token');
    const savedUser = localStorage.getItem('user');
    const expiresAt = localStorage.getItem('tokenExpires');

    if (savedToken && savedUser && new Date() < new Date(expiresAt)) {
      setToken(savedToken);
      setUser(JSON.parse(savedUser));
    } else {
      localStorage.clear();
    }
    
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    const response = await fetch('http://localhost:5000/api/Auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });

    if (!response.ok) throw new Error('Login failed');

    const data = await response.json();
    
    setToken(data.token);
    setUser(data.usuario);
    
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(data.usuario));
    localStorage.setItem('tokenExpires', data.expiresAt);
  };

  const googleLogin = async (googleIdToken) => {
    const response = await fetch('http://localhost:5000/api/Auth/google-login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ idToken: googleIdToken })
    });

    if (!response.ok) throw new Error('Google login failed');

    const data = await response.json();
    
    setToken(data.token);
    setUser(data.usuario);
    
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(data.usuario));
    localStorage.setItem('tokenExpires', data.expiresAt);
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.clear();
  };

  const apiCall = async (endpoint, options = {}) => {
    const config = {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      }
    };

    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`http://localhost:5000${endpoint}`, config);

    if (response.status === 401) {
      logout();
      throw new Error('Token expirado');
    }

    return response;
  };

  return (
    <AuthContext.Provider value={{ user, token, login, googleLogin, logout, apiCall, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth debe usarse dentro de AuthProvider');
  }
  return context;
};

// Uso:
// En App.js:
// <AuthProvider>
//   <App />
// </AuthProvider>

// En cualquier componente:
// const { user, login, googleLogin, logout, apiCall } = useAuth();
```

## 🔐 Seguridad

### Token en el localStorage

- **Ventaja:** Simple y persiste entre sesiones
- **Desventaja:** Vulnerable a XSS (Cross-Site Scripting)

### Mejores prácticas:

1. **Usa HTTPS** en producción
2. **No expongas el token** en URLs o logs
3. **Implementa refresh tokens** para sesiones largas
4. **Valida el token** en cada petición del backend
5. **Usa httpOnly cookies** para mayor seguridad (requiere cambios en el backend)

## 📝 Resumen

✅ Tres endpoints devuelven tokens JWT  
✅ Token válido por 60 minutos  
✅ Token debe incluirse en header `Authorization: Bearer {token}`  
✅ Frontend debe manejar expiración y logout  
✅ Usar helpers para facilitar peticiones autenticadas  

¡Ya tienes autenticación completa con JWT! 🎉
