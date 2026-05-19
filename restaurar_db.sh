#!/bin/bash
# Script para restaurar la base de datos local y configurar la conexión

DB_NAME="SimpleMarketplaceDB3"
SCHEMA_FILE="basededatos_actualizada_schema.sql"
APP_SETTINGS="appsettings.Development.json"
DB_USER="marketplace_user"
DB_PASS="Marketplace123!"

echo "=========================================================="
echo "   Restaurador de Base de Datos y Configuración Local"
echo "=========================================================="
echo ""
echo "Este script configurará un usuario de MySQL dedicado para la aplicación"
echo "para evitar problemas de permisos con el usuario 'root' en Linux."
echo ""

# Crear base de datos y usuario usando sudo mysql
echo "Configurando MySQL con privilegios de sudo..."
echo "(Introduce tu contraseña de Linux/sudo cuando te lo solicite)"
sudo mysql -e "
CREATE DATABASE IF NOT EXISTS $DB_NAME;
CREATE USER IF NOT EXISTS '$DB_USER'@'localhost' IDENTIFIED BY '$DB_PASS';
GRANT ALL PRIVILEGES ON $DB_NAME.* TO '$DB_USER'@'localhost';
FLUSH PRIVILEGES;
"

if [ $? -ne 0 ]; then
    echo "❌ Error: No se pudo configurar la base de datos o el usuario. Asegúrate de ingresar tu contraseña correcta de sudo."
    exit 1
fi

echo "✔ Base de datos y usuario '$DB_USER' configurados con éxito."

# Importar el esquema SQL usando el nuevo usuario
echo "Importando esquema de '$SCHEMA_FILE'..."
mysql -u "$DB_USER" -p"$DB_PASS" -h localhost "$DB_NAME" < "$SCHEMA_FILE"

if [ $? -ne 0 ]; then
    echo "❌ Error: Falló la importación del archivo '$SCHEMA_FILE' con el nuevo usuario."
    exit 1
fi

echo "✔ Esquema importado con éxito."

# Configurar el archivo appsettings.Development.json
echo "Configurando conexión en '$APP_SETTINGS'..."

cat <<EOF > "$APP_SETTINGS"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=$DB_NAME;user=$DB_USER;password=$DB_PASS;"
  }
}
EOF

echo "✔ Archivo '$APP_SETTINGS' actualizado correctamente."
echo ""
echo "=========================================================="
echo "🎉 ¡Todo listo! Ya puedes iniciar la aplicación localmente:"
echo "   export ASPNETCORE_ENVIRONMENT=Development"
echo "   dotnet run"
echo "=========================================================="
