// ============================================
// GoogleLogin.jsx - Componente de React
// ============================================

import { GoogleOAuthProvider, GoogleLogin } from '@react-oauth/google';
import { useState } from 'react';

// Instalar dependencias:
// npm install @react-oauth/google

const GOOGLE_CLIENT_ID = '565092651331-aq6gmrgbms3jci0jr2oan3l56d9ialqs.apps.googleusercontent.com';
const API_URL = 'http://localhost:5000/api/Auth/google-login';

function GoogleLoginComponent() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [token, setToken] = useState(null);

  const handleGoogleSuccess = async (credentialResponse) => {
    setLoading(true);
    setError(null);

    try {
      const response = await fetch(API_URL, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          idToken: credentialResponse.credential
        })
      });

      if (!response.ok) {
        throw new Error('Error al autenticar con la API');
      }

      const data = await response.json();
      
      // Guardar usuario y token en estado
      setUser(data.usuario);
      setToken(data.token);
      
      // Guardar en localStorage
      localStorage.setItem('token', data.token);
      localStorage.setItem('user', JSON.stringify(data.usuario));
      localStorage.setItem('tokenExpires', data.expiresAt);
      
      console.log('🔑 Token:', data.token);
      console.log('👤 Usuario:', data.usuario);
      console.log('⏰ Expira:', data.expiresAt);
      
    } catch (err) {
      setError(err.message);
      console.error('Error al autenticar:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleGoogleError = () => {
    setError('Error al iniciar sesión con Google');
  };

  const handleLogout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('tokenExpires');
  };

  // Verificar si hay usuario guardado al cargar
  useState(() => {
    const savedUser = localStorage.getItem('user');
    const savedToken = localStorage.getItem('token');
    if (savedUser && savedToken) {
      setUser(JSON.parse(savedUser));
      setToken(savedToken);
    }
  }, []);

  if (loading) {
    return (
      <div style={styles.container}>
        <div style={styles.loading}>
          <div style={styles.spinner}></div>
          <p>Autenticando...</p>
        </div>
      </div>
    );
  }

  if (user) {
    return (
      <div style={styles.container}>
        <div style={styles.userCard}>
          <h2 style={styles.title}>¡Bienvenido! 👋</h2>
          
          {user.profilePictureUrl && (
            <img 
              src={user.profilePictureUrl} 
              alt={user.nombre}
              style={styles.avatar}
            />
          )}
          
          <div style={styles.userInfo}>
            <p><strong>Nombre:</strong> {user.nombre} {user.apellido}</p>
            <p><strong>Email:</strong> {user.email}</p>
            <p><strong>ID:</strong> {user.usuarioId}</p>
            <p><strong>Proveedor:</strong> {user.provider}</p>
            <p><strong>Estado:</strong> {user.estado}</p>
          </div>

          <button 
            onClick={handleLogout}
            style={styles.logoutButton}
          >
            Cerrar Sesión
          </button>
        </div>
      </div>
    );
  }

  return (
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      <div style={styles.container}>
        <div style={styles.loginCard}>
          <h1 style={styles.title}>🔐 Simple Marketplace</h1>
          <p style={styles.subtitle}>Inicia sesión con tu cuenta de Google</p>
          
          <div style={styles.googleButton}>
            <GoogleLogin
              onSuccess={handleGoogleSuccess}
              onError={handleGoogleError}
              useOneTap
              text="signin_with"
              shape="rectangular"
              theme="filled_blue"
              size="large"
            />
          </div>

          {error && (
            <div style={styles.error}>
              <strong>❌ Error:</strong> {error}
            </div>
          )}
        </div>
      </div>
    </GoogleOAuthProvider>
  );
}

// Estilos inline (puedes moverlos a CSS)
const styles = {
  container: {
    minHeight: '100vh',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    padding: '20px',
  },
  loginCard: {
    background: 'white',
    borderRadius: '20px',
    boxShadow: '0 20px 60px rgba(0, 0, 0, 0.3)',
    padding: '40px',
    maxWidth: '400px',
    width: '100%',
    textAlign: 'center',
  },
  userCard: {
    background: 'white',
    borderRadius: '20px',
    boxShadow: '0 20px 60px rgba(0, 0, 0, 0.3)',
    padding: '40px',
    maxWidth: '500px',
    width: '100%',
    textAlign: 'center',
  },
  title: {
    color: '#333',
    marginBottom: '10px',
    fontSize: '28px',
  },
  subtitle: {
    color: '#666',
    marginBottom: '30px',
    fontSize: '14px',
  },
  avatar: {
    width: '100px',
    height: '100px',
    borderRadius: '50%',
    margin: '20px auto',
    border: '4px solid #667eea',
  },
  userInfo: {
    textAlign: 'left',
    background: '#f8f9fa',
    padding: '20px',
    borderRadius: '10px',
    margin: '20px 0',
  },
  googleButton: {
    display: 'flex',
    justifyContent: 'center',
    margin: '20px 0',
  },
  logoutButton: {
    background: '#dc3545',
    color: 'white',
    border: 'none',
    padding: '12px 30px',
    borderRadius: '8px',
    fontSize: '16px',
    cursor: 'pointer',
    fontWeight: 'bold',
    transition: 'all 0.3s ease',
  },
  error: {
    background: '#f8d7da',
    border: '1px solid #f5c6cb',
    color: '#721c24',
    padding: '15px',
    borderRadius: '8px',
    marginTop: '20px',
    textAlign: 'left',
  },
  loading: {
    textAlign: 'center',
    color: 'white',
  },
  spinner: {
    border: '4px solid rgba(255, 255, 255, 0.3)',
    borderTop: '4px solid white',
    borderRadius: '50%',
    width: '50px',
    height: '50px',
    animation: 'spin 1s linear infinite',
    margin: '20px auto',
  },
};

export default GoogleLoginComponent;


// ============================================
// App.jsx - Uso del componente
// ============================================

/*
import GoogleLoginComponent from './components/GoogleLoginComponent';

function App() {
  return (
    <div className="App">
      <GoogleLoginComponent />
    </div>
  );
}

export default App;
*/


// ============================================
// CONFIGURACIÓN ADICIONAL
// ============================================

// 1. Instalar dependencias:
//    npm install @react-oauth/google

// 2. Configurar CORS en tu API .NET si es necesario
//    Ver archivo Program.cs para agregar:
/*
   builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowReact",
           builder => builder
               .WithOrigins("http://localhost:3000", "http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader());
   });

   // Después de app.UseRouting():
   app.UseCors("AllowReact");
*/

// 3. Reemplazar GOOGLE_CLIENT_ID con tu Client ID real

// 4. Ajustar API_URL según tu configuración

// ============================================
// EJEMPLO: Cómo usar el token en otras peticiones
// ============================================

/*
// Función helper para hacer peticiones autenticadas
const fetchWithAuth = async (url, options = {}) => {
  const token = localStorage.getItem('token');
  
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  };
  
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  
  const response = await fetch(url, {
    ...options,
    headers,
  });
  
  // Si el token expiró, redirigir al login
  if (response.status === 401) {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
  }
  
  return response;
};

// Ejemplo de uso:
const obtenerPedidos = async () => {
  try {
    const response = await fetchWithAuth('http://localhost:5000/api/Pedidos');
    const pedidos = await response.json();
    console.log('Pedidos:', pedidos);
  } catch (error) {
    console.error('Error al obtener pedidos:', error);
  }
};

// Ejemplo de POST:
const crearPedido = async (pedidoData) => {
  try {
    const response = await fetchWithAuth('http://localhost:5000/api/Pedidos', {
      method: 'POST',
      body: JSON.stringify(pedidoData)
    });
    const nuevoPedido = await response.json();
    console.log('Pedido creado:', nuevoPedido);
  } catch (error) {
    console.error('Error al crear pedido:', error);
  }
};
*/

