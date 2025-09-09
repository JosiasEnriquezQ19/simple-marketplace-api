/**
 * Ejemplo de cómo hacer un pedido sin método de pago desde el frontend
 * 
 * PROBLEMA DETECTADO: 
 * El frontend está intentando enviar pedidos a http://localhost:5173/api/Pedidos/usuario/1
 * cuando debería enviarlos a http://localhost:[PUERTO_API]/api/Pedidos/usuario/1
 * 
 * También está intentando usar metodoPagoId que no existen (2,3,4,5) en lugar de NULL
 */

// Importación de axios (asumiendo que estás usando axios)
// import axios from 'axios';

// CORRECTO: URL base de la API - DEBE ser la URL de tu API, no la del frontend
const API_URL = 'http://localhost:5000'; // Cambia esto al puerto correcto de tu API

/**
 * Función para hacer un pedido SIN método de pago
 */
async function hacerPedidoSinMetodoPago() {
  try {
    const usuarioId = 1; // ID del usuario que está haciendo el pedido
    
    // CORRECTO: Estructura del pedido sin metodoPagoId
    const pedidoData = {
      DireccionId: 1, // ID de la dirección seleccionada
      Items: [
        { ProductoId: 1, Cantidad: 2 },
        { ProductoId: 3, Cantidad: 1 }
      ]
      // Nota: No incluimos MetodoPagoId para que sea null
    };
    
    // CORRECTO: URL del endpoint con el usuarioId
    const url = `${API_URL}/api/Pedidos/usuario/${usuarioId}`;
    
    console.log('Enviando pedido sin método de pago:', pedidoData);
    console.log('URL:', url);
    
    // Ejemplo con axios
    // const response = await axios.post(url, pedidoData);
    
    // Ejemplo con fetch
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(pedidoData)
    });
    
    if (!response.ok) {
      throw new Error('Error al crear pedido: ' + response.statusText);
    }
    
    const data = await response.json();
    console.log('Pedido creado exitosamente:', data);
    return data;
    
  } catch (error) {
    console.error('Error al crear pedido:', error);
    throw error;
  }
}

/**
 * Función para hacer un pedido CON valor cero como método de pago
 * (se convertirá automáticamente a NULL en el backend)
 */
async function hacerPedidoConMetodoPagoCero() {
  try {
    const usuarioId = 1;
    
    // CORRECTO: Estructura del pedido con metodoPagoId = 0 (se convertirá a null)
    const pedidoData = {
      DireccionId: 1,
      MetodoPagoId: 0, // El backend lo convertirá a null
      Items: [
        { ProductoId: 1, Cantidad: 2 },
        { ProductoId: 3, Cantidad: 1 }
      ]
    };
    
    const url = `${API_URL}/api/Pedidos/usuario/${usuarioId}`;
    
    console.log('Enviando pedido con método de pago cero:', pedidoData);
    console.log('URL:', url);
    
    // Implementa el resto del código como en el ejemplo anterior
    
  } catch (error) {
    console.error('Error al crear pedido:', error);
    throw error;
  }
}
