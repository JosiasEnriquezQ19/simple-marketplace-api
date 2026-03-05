-- MySQL dump 10.13  Distrib 8.0.45, for Linux (x86_64)
--
-- Host: localhost    Database: SimpleMarketplaceDB3
-- ------------------------------------------------------
-- Server version	8.0.45-0ubuntu0.24.04.1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Administradores`
--

DROP TABLE IF EXISTS `Administradores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Administradores` (
  `adminId` int NOT NULL AUTO_INCREMENT,
  `email` varchar(100) NOT NULL,
  `contraseñaHash` varchar(255) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `fechaCreacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fechaUltimoAcceso` datetime DEFAULT NULL,
  `nivelAcceso` enum('basico','medio','avanzado') DEFAULT 'basico',
  `estado` varchar(20) DEFAULT 'activo',
  PRIMARY KEY (`adminId`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Banners`
--

DROP TABLE IF EXISTS `Banners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Banners` (
  `bannerId` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `imagenDesktopUrl` varchar(255) NOT NULL,
  `imagenMobileUrl` varchar(255) NOT NULL,
  `linkUrl` varchar(255) DEFAULT NULL,
  `orden` int DEFAULT '0',
  `activo` tinyint(1) DEFAULT '1',
  `fechaCreacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fechaActualizacion` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`bannerId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Carrito`
--

DROP TABLE IF EXISTS `Carrito`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Carrito` (
  `carritoId` int NOT NULL AUTO_INCREMENT,
  `usuarioId` int NOT NULL,
  `productoId` int NOT NULL,
  `cantidad` int NOT NULL DEFAULT '1',
  `fechaAgregado` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`carritoId`),
  UNIQUE KEY `item_unico_carrito` (`usuarioId`,`productoId`),
  KEY `productoId` (`productoId`),
  KEY `idx_carrito_usuario` (`usuarioId`),
  CONSTRAINT `Carrito_ibfk_1` FOREIGN KEY (`usuarioId`) REFERENCES `Usuarios` (`usuarioId`) ON DELETE CASCADE,
  CONSTRAINT `Carrito_ibfk_2` FOREIGN KEY (`productoId`) REFERENCES `Productos` (`productoId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Categorias`
--

DROP TABLE IF EXISTS `Categorias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Categorias` (
  `categoriaId` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `descripcion` text,
  `imagenUrl` varchar(255) DEFAULT NULL,
  `estado` enum('activo','inactivo') DEFAULT 'activo',
  `fechaCreacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fechaActualizacion` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`categoriaId`),
  UNIQUE KEY `nombre` (`nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Comentarios`
--

DROP TABLE IF EXISTS `Comentarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Comentarios` (
  `comentarioId` int NOT NULL AUTO_INCREMENT,
  `productoId` int NOT NULL,
  `usuarioId` int NOT NULL,
  `comentario` text NOT NULL,
  `estrellas` int NOT NULL,
  `fechaComentario` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`comentarioId`),
  KEY `productoId` (`productoId`),
  KEY `usuarioId` (`usuarioId`),
  CONSTRAINT `Comentarios_ibfk_1` FOREIGN KEY (`productoId`) REFERENCES `Productos` (`productoId`) ON DELETE CASCADE,
  CONSTRAINT `Comentarios_ibfk_2` FOREIGN KEY (`usuarioId`) REFERENCES `Usuarios` (`usuarioId`) ON DELETE CASCADE,
  CONSTRAINT `Comentarios_chk_1` CHECK ((`estrellas` between 1 and 5))
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Configuraciones`
--

DROP TABLE IF EXISTS `Configuraciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Configuraciones` (
  `configId` int NOT NULL AUTO_INCREMENT,
  `clave` varchar(50) NOT NULL,
  `valor` text NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  `fechaActualizacion` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`configId`),
  UNIQUE KEY `clave` (`clave`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `DetallesPedido`
--

DROP TABLE IF EXISTS `DetallesPedido`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DetallesPedido` (
  `detallePedidoId` int NOT NULL AUTO_INCREMENT,
  `pedidoId` int NOT NULL,
  `productoId` int NOT NULL,
  `cantidad` int NOT NULL,
  `precioUnitario` decimal(10,2) NOT NULL,
  PRIMARY KEY (`detallePedidoId`),
  KEY `pedidoId` (`pedidoId`),
  KEY `productoId` (`productoId`),
  CONSTRAINT `DetallesPedido_ibfk_1` FOREIGN KEY (`pedidoId`) REFERENCES `Pedidos` (`pedidoId`) ON DELETE CASCADE,
  CONSTRAINT `DetallesPedido_ibfk_2` FOREIGN KEY (`productoId`) REFERENCES `Productos` (`productoId`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Direcciones`
--

DROP TABLE IF EXISTS `Direcciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Direcciones` (
  `direccionId` int NOT NULL AUTO_INCREMENT,
  `usuarioId` int NOT NULL,
  `calle` varchar(255) NOT NULL,
  `ciudad` varchar(100) NOT NULL,
  `estado` varchar(100) NOT NULL,
  `codigoPostal` varchar(20) NOT NULL,
  `pais` varchar(100) NOT NULL DEFAULT 'Perú',
  `esPrincipal` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`direccionId`),
  KEY `usuarioId` (`usuarioId`),
  CONSTRAINT `Direcciones_ibfk_1` FOREIGN KEY (`usuarioId`) REFERENCES `Usuarios` (`usuarioId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Facturas`
--

DROP TABLE IF EXISTS `Facturas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Facturas` (
  `facturaId` int NOT NULL AUTO_INCREMENT,
  `pedidoId` int NOT NULL,
  `numeroFactura` varchar(50) NOT NULL,
  `fechaEmision` datetime DEFAULT CURRENT_TIMESTAMP,
  `subtotal` decimal(10,2) NOT NULL,
  `impuestos` decimal(10,2) NOT NULL,
  `total` decimal(10,2) NOT NULL,
  `estadoPago` enum('pagado','pendiente','reembolsado') DEFAULT 'pendiente',
  PRIMARY KEY (`facturaId`),
  UNIQUE KEY `numeroFactura` (`numeroFactura`),
  KEY `pedidoId` (`pedidoId`),
  CONSTRAINT `Facturas_ibfk_1` FOREIGN KEY (`pedidoId`) REFERENCES `Pedidos` (`pedidoId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `LogsAdministrativos`
--

DROP TABLE IF EXISTS `LogsAdministrativos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LogsAdministrativos` (
  `logId` int NOT NULL AUTO_INCREMENT,
  `adminId` int DEFAULT NULL,
  `accion` varchar(100) NOT NULL,
  `detalles` text,
  `ipAddress` varchar(45) DEFAULT NULL,
  `userAgent` varchar(255) DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`logId`),
  KEY `adminId` (`adminId`),
  CONSTRAINT `LogsAdministrativos_ibfk_1` FOREIGN KEY (`adminId`) REFERENCES `Administradores` (`adminId`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `MetodosPago`
--

DROP TABLE IF EXISTS `MetodosPago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MetodosPago` (
  `metodoPagoId` int NOT NULL AUTO_INCREMENT,
  `usuarioId` int NOT NULL,
  `tipoTarjeta` varchar(50) NOT NULL,
  `ultimosCuatroDigitos` varchar(4) NOT NULL,
  `mesExpiracion` int NOT NULL,
  `añoExpiracion` int NOT NULL,
  `esPrincipal` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`metodoPagoId`),
  KEY `usuarioId` (`usuarioId`),
  CONSTRAINT `MetodosPago_ibfk_1` FOREIGN KEY (`usuarioId`) REFERENCES `Usuarios` (`usuarioId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Pedidos`
--

DROP TABLE IF EXISTS `Pedidos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Pedidos` (
  `pedidoId` int NOT NULL AUTO_INCREMENT,
  `usuarioId` int NOT NULL,
  `direccionId` int NOT NULL,
  `metodoPagoId` int DEFAULT NULL,
  `subtotal` decimal(10,2) NOT NULL,
  `costoEnvio` decimal(10,2) NOT NULL DEFAULT '0.00',
  `impuestos` decimal(10,2) NOT NULL DEFAULT '0.00',
  `total` decimal(10,2) NOT NULL,
  `estado` enum('pendiente','procesando','enviado','entregado','cancelado') DEFAULT 'pendiente',
  `fechaPedido` datetime DEFAULT CURRENT_TIMESTAMP,
  `numeroSeguimiento` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`pedidoId`),
  KEY `direccionId` (`direccionId`),
  KEY `metodoPagoId` (`metodoPagoId`),
  KEY `idx_pedidos_usuario` (`usuarioId`),
  KEY `idx_pedidos_estado` (`estado`),
  CONSTRAINT `Pedidos_ibfk_1` FOREIGN KEY (`usuarioId`) REFERENCES `Usuarios` (`usuarioId`),
  CONSTRAINT `Pedidos_ibfk_2` FOREIGN KEY (`direccionId`) REFERENCES `Direcciones` (`direccionId`),
  CONSTRAINT `Pedidos_ibfk_3` FOREIGN KEY (`metodoPagoId`) REFERENCES `MetodosPago` (`metodoPagoId`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Productos`
--

DROP TABLE IF EXISTS `Productos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Productos` (
  `productoId` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(200) NOT NULL,
  `descripcion` text,
  `marca` varchar(100) DEFAULT NULL,
  `precio` decimal(10,2) NOT NULL,
  `stock` int NOT NULL DEFAULT '0',
  `imagenUrl` varchar(255) NOT NULL,
  `imagenUrl2` varchar(255) DEFAULT NULL,
  `imagenUrl3` varchar(255) DEFAULT NULL,
  `imagenUrl4` varchar(255) DEFAULT NULL,
  `imagenUrl5` varchar(255) DEFAULT NULL,
  `imagenUrl6` varchar(255) DEFAULT NULL,
  `imagenUrl7` varchar(255) DEFAULT NULL,
  `categoriaId` int NOT NULL,
  `estado` enum('disponible','agotado','descontinuado','oculto') DEFAULT 'disponible',
  `fechaCreacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fechaActualizacion` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `precioAntes` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`productoId`),
  KEY `idx_productos_categoria` (`categoriaId`),
  CONSTRAINT `Productos_ibfk_1` FOREIGN KEY (`categoriaId`) REFERENCES `Categorias` (`categoriaId`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Usuarios`
--

DROP TABLE IF EXISTS `Usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Usuarios` (
  `usuarioId` int NOT NULL AUTO_INCREMENT,
  `email` varchar(100) NOT NULL,
  `contraseñaHash` varchar(255) DEFAULT NULL,
  `nombre` varchar(50) NOT NULL,
  `apellido` varchar(50) NOT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `estado` enum('activo','inactivo','suspendido') DEFAULT 'activo',
  `fechaCreacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fechaActualizacion` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `GoogleId` varchar(255) DEFAULT NULL,
  `Provider` varchar(50) DEFAULT NULL,
  `ProfilePictureUrl` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`usuarioId`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-05 17:28:00
