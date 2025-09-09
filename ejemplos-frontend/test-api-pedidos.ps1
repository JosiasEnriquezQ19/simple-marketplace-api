# Prueba de API con cURL

# Este archivo contiene comandos curl para probar directamente la API
# Puedes copiar y ejecutar estos comandos en tu terminal PowerShell

# 1. Crear un pedido SIN método de pago
# Reemplaza el puerto 5000 por el puerto real donde corre tu API
$usuarioId = 1
$url = "http://localhost:5000/api/Pedidos/usuario/$usuarioId"

$body = @{
    DireccionId = 1
    Items = @(
        @{
            ProductoId = 1
            Cantidad = 2
        }
    )
} | ConvertTo-Json

Write-Host "Enviando pedido sin método de pago a: $url"
Write-Host "Cuerpo de la petición:"
Write-Host $body

Invoke-RestMethod -Uri $url -Method Post -ContentType "application/json" -Body $body

# 2. Crear un pedido con método de pago CERO (se convertirá a null)
$body = @{
    DireccionId = 1
    MetodoPagoId = 0
    Items = @(
        @{
            ProductoId = 1
            Cantidad = 1
        }
    )
} | ConvertTo-Json

Write-Host "Enviando pedido con método de pago cero a: $url"
Write-Host "Cuerpo de la petición:"
Write-Host $body

Invoke-RestMethod -Uri $url -Method Post -ContentType "application/json" -Body $body

# Notas importantes:
# 1. Asegúrate de reemplazar el puerto en la URL por el puerto correcto de tu API
# 2. Asegúrate de que exista un usuario con ID 1, una dirección con ID 1 y un producto con ID 1
# 3. Si obtienes errores, revisa los mensajes para entender qué sucede
