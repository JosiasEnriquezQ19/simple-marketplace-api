// Archivo de prueba para verificar las APIs de comentarios

## Endpoints simplificados del Controller de Comentarios

### 1. Obtener comentarios de un producto
**GET** `/api/productos/{productoId}/comentarios`

Ejemplo:
```
GET http://localhost:5184/api/productos/1/comentarios
```

Respuesta esperada:
```json
[
  {
    "comentarioId": 1,
    "productoId": 1,
    "usuarioId": 1,
    "texto": "Excelente producto",
    "estrellas": 5,
    "fechaComentario": "2025-09-07T10:30:00"
  }
]
```

### 2. Crear un nuevo comentario
**POST** `/api/productos/{productoId}/comentarios`

Ejemplo:
```
POST http://localhost:5184/api/productos/1/comentarios
Content-Type: application/json

{
  "texto": "Me gusta mucho este producto",
  "estrellas": 4,
  "usuarioId": 1
}
```

Respuesta esperada: 201 Created con el comentario creado

### 3. Obtener promedio de estrellas
**GET** `/api/productos/{productoId}/comentarios/promedio`

Ejemplo:
```
GET http://localhost:5184/api/productos/1/comentarios/promedio
```

Respuesta esperada:
```json
{
  "promedio": 4.5,
  "totalComentarios": 10
}
```

## Cambios realizados en el Controller:

1. **Simplificado**: Removido AutoMapper, ILogger y dependencias complejas
2. **Sin autorización**: Los endpoints están abiertos para pruebas
3. **Validaciones básicas**: Solo valida datos esenciales
4. **Manejo de errores**: Respuestas JSON claras en caso de error
5. **Proyección directa**: Usa objetos anónimos para evitar problemas de serialización

## Para usar en tu frontend (JavaScript):

```javascript
// Obtener comentarios
async function getComentarios(productoId) {
    const response = await fetch(`http://localhost:5184/api/productos/${productoId}/comentarios`);
    return await response.json();
}

// Crear comentario
async function crearComentario(productoId, texto, estrellas, usuarioId = 1) {
    const response = await fetch(`http://localhost:5184/api/productos/${productoId}/comentarios`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            texto: texto,
            estrellas: estrellas,
            usuarioId: usuarioId
        })
    });
    return await response.json();
}

// Obtener promedio
async function getPromedio(productoId) {
    const response = await fetch(`http://localhost:5184/api/productos/${productoId}/comentarios/promedio`);
    return await response.json();
}
```

## Problemas potenciales y soluciones:

1. **Base de datos no conecta**: Verifica que MySQL esté corriendo en puerto 3306
2. **Tablas no existen**: Ejecuta migraciones de Entity Framework
3. **Puerto ocupado**: Cambia el puerto en la URL

## Para probar sin base de datos:
Si tienes problemas de conectividad, puedo crear una versión que use datos en memoria.
