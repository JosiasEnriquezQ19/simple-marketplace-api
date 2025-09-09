# Instrucciones para corregir el frontend (Carrito.jsx)

Según los logs de error proporcionados, hay varios problemas en el frontend que están causando que los pedidos fallen. Aquí están los problemas y cómo solucionarlos:

## Problema 1: URL incorrecta de la API

El frontend está intentando usar `http://localhost:5173` como base para la API, lo cual es incorrecto. `5173` es probablemente el puerto donde se ejecuta Vite (el servidor de desarrollo del frontend), no donde se ejecuta tu API de .NET.

### Solución:

1. Localiza en tu código dónde estás definiendo la URL base para las peticiones a la API:

```javascript
// Busca algo como esto (probablemente en un archivo de configuración o servicio)
const API_URL = 'http://localhost:5173'; // INCORRECTO
```

2. Cámbialo al puerto correcto donde tu API de .NET está ejecutándose:

```javascript
// Cambia a algo como esto:
const API_URL = 'http://localhost:5000'; // O el puerto que uses para tu API
```

## Problema 2: El frontend intenta métodos de pago inexistentes

Según el log, el frontend está intentando automáticamente con varios IDs de método de pago (2, 3, 4, 5) y recibe el error "Método de pago X no existe".

### Solución:

1. Localiza la función `hacerPedido` en `Carrito.jsx` (alrededor de la línea 99-100)
2. Elimina el código que intenta diferentes IDs de método de pago
3. Modifica para usar `null` como método de pago:

```javascript
// Cambia este tipo de código:
async function hacerPedido() {
  // Elimina cualquier bucle o lógica que intente diferentes IDs (2, 3, 4, 5)
  
  const pedidoData = {
    DireccionId: 1,
    // NO incluyas MetodoPagoId o establécelo como null explícitamente
    // MetodoPagoId: null,  // Opción 1: explícitamente null
    // La opción 2 es simplemente no incluirlo
    Items: carrito.map(item => ({
      ProductoId: item.productoId,
      Cantidad: item.cantidad
    }))
  };
  
  // Usa la URL correcta con el API_URL adecuado
  const url = `${API_URL}/api/Pedidos/usuario/${usuarioId}`;
  
  // Resto del código...
}
```

## Problema 3: Intentando rutas incorrectas

El log muestra intentos de hacer POST a `/api/Pedidos` directamente, lo cual no es correcto según tu API.

### Solución:

1. Verifica que todas las peticiones a la API usen la ruta correcta:

```javascript
// CORRECTO: POST a /api/Pedidos/usuario/{usuarioId}
const response = await axios.post(`${API_URL}/api/Pedidos/usuario/${usuarioId}`, pedidoData);

// INCORRECTO: No uses estas variantes
// await axios.post(`${API_URL}/api/Pedidos`, pedidoData);
// await axios.post(`${API_URL}/api/Pedidos`, { dto: pedidoData });
```

## Problema 4: Objetos adicionales en el cuerpo de la petición

En los logs aparece que estás enviando campos adicionales como `DescuentoTotal` y `DescuentosPorItem` que no están definidos en el DTO del backend.

### Solución:

1. Asegúrate de que el objeto que envías coincida exactamente con lo que espera el backend:

```javascript
// CORRECTO: Solo incluye los campos esperados por el backend
const pedidoData = {
  DireccionId: direccionId,
  // No incluyas MetodoPagoId si quieres que sea null
  Items: carrito.map(item => ({
    ProductoId: item.productoId,
    Cantidad: item.cantidad
  }))
  // No incluyas campos adicionales como DescuentoTotal o DescuentosPorItem
};
```

## Ejemplo de código correcto completo

Aquí tienes un ejemplo completo de cómo debería ser la función `hacerPedido` en tu `Carrito.jsx`:

```javascript
async function hacerPedido() {
  try {
    // Obtener el ID del usuario actual (desde el contexto de autenticación o donde lo tengas)
    const usuarioId = 1; // Cambia esto por el ID real del usuario
    
    // Obtener ID de dirección seleccionada
    const direccionId = 1; // Cambia esto según la selección del usuario
    
    // Preparar el objeto de datos del pedido
    const pedidoData = {
      DireccionId: direccionId,
      // No incluimos MetodoPagoId para que sea null (método de pago opcional)
      Items: carrito.map(item => ({
        ProductoId: item.productoId,
        Cantidad: item.cantidad
      }))
    };
    
    console.log('Enviando pedido:', pedidoData);
    
    // URL correcta para la API
    const url = `http://localhost:5000/api/Pedidos/usuario/${usuarioId}`;
    
    const response = await axios.post(url, pedidoData);
    
    console.log('Pedido creado exitosamente:', response.data);
    
    // Vaciar carrito, mostrar mensaje de éxito, redirigir, etc.
    
  } catch (error) {
    console.error('Error al crear pedido:', error);
    // Mostrar mensaje de error al usuario
  }
}
```

Recuerda ajustar las URLs y valores según tu configuración específica.
