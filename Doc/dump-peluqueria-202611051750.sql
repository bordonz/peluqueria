-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: peluqueria
-- ------------------------------------------------------
-- Server version	26.7.0

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
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ 'b2a2087a-98e3-11f1-b534-0a0027000006:1-446';

--
-- Table structure for table `estilista_servicios`
--

DROP TABLE IF EXISTS `estilista_servicios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `estilista_servicios` (
  `id_estilista_servicios` int NOT NULL AUTO_INCREMENT,
  `id_estilista` int NOT NULL,
  `id_servicio` int NOT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_estilista_servicios`),
  KEY `FK_estilista_servicios_estilista` (`id_estilista`),
  KEY `FK_estilista_servicios_servicios` (`id_servicio`),
  CONSTRAINT `FK_estilista_servicios_estilista` FOREIGN KEY (`id_estilista`) REFERENCES `usuarios` (`id_usuario`),
  CONSTRAINT `FK_estilista_servicios_servicios` FOREIGN KEY (`id_servicio`) REFERENCES `servicios` (`id_servicio`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `estilista_servicios`
--

LOCK TABLES `estilista_servicios` WRITE;
/*!40000 ALTER TABLE `estilista_servicios` DISABLE KEYS */;
INSERT INTO `estilista_servicios` VALUES (1,4,1,0),(3,56,1,1),(4,57,1,1),(5,58,1,1),(7,57,3,1);
/*!40000 ALTER TABLE `estilista_servicios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pago`
--

DROP TABLE IF EXISTS `pago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pago` (
  `id_pago` int NOT NULL AUTO_INCREMENT,
  `fecha_pago` datetime NOT NULL,
  `monto` decimal(10,2) NOT NULL,
  `estado` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `metodo` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_turno` int NOT NULL,
  PRIMARY KEY (`id_pago`),
  KEY `FK_Pago_Turno` (`id_turno`),
  CONSTRAINT `FK_Pago_Turno` FOREIGN KEY (`id_turno`) REFERENCES `turnos` (`id_turno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pago`
--

LOCK TABLES `pago` WRITE;
/*!40000 ALTER TABLE `pago` DISABLE KEYS */;
/*!40000 ALTER TABLE `pago` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `servicios`
--

DROP TABLE IF EXISTS `servicios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `servicios` (
  `id_servicio` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `descripcion` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `duracion` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `precio` decimal(10,2) NOT NULL,
  `activo` tinyint(1) DEFAULT NULL,
  `avatar` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_servicio`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `servicios`
--

LOCK TABLES `servicios` WRITE;
/*!40000 ALTER TABLE `servicios` DISABLE KEYS */;
INSERT INTO `servicios` VALUES (1,'Corte masculino','Corte de pelo','1',10000.00,1,'/Uploads\\avatar_1.jpg'),(2,'Corte femenino','Corte de pelo femenino','1',15000.00,1,'/Uploads\\avatar_2.jpg'),(3,'Tinte Básico','Cambio de color de todo el cabello','2',60000.00,1,'/Uploads\\avatar_3.jpg'),(4,'Decoloración','Rubios intensos, fantasía o como base para color','4',150000.00,1,'/Uploads\\avatar_4.jpg'),(5,'Tratamiento Capilar','Cuidados nutritivos y reparadores','1',45000.00,1,'/Uploads\\avatar_5.jpg'),(6,'Extensión de cabello','Sumá largo y volumen con cabello 100% natural','3',200000.00,1,'/Uploads\\avatar_6.jpg'),(7,'Permanentes/ondas','Rulos u ondas permanentes según el largo: corto, melena o largo','2',100000.00,1,'/Uploads\\avatar_7.jpg'),(8,'edit','edit','1',10000.00,1,'/Uploads\\avatar_8.jpg'),(9,'Alisado/Keratina','Alisado progresivo o shock keratina para controlar el frizz y suavizar el cabello','2',100000.00,1,'/Uploads\\avatar_9.jpg'),(10,'prueba1','pruebades1','1',10000.00,0,NULL),(11,'pru2','prudesc2','2',10000.00,0,'/Uploads\\avatar_11.jpg'),(12,'Corte de niño','Corte de pelo para niños','1',15000.00,1,'/Uploads\\avatar_12.jpg');
/*!40000 ALTER TABLE `servicios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `turnos`
--

DROP TABLE IF EXISTS `turnos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `turnos` (
  `id_turno` int NOT NULL AUTO_INCREMENT,
  `fecha_emitido` datetime NOT NULL,
  `fecha_turno` datetime NOT NULL,
  `estado` varchar(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_cliente` int NOT NULL,
  `id_estilista_servicios` int DEFAULT NULL,
  PRIMARY KEY (`id_turno`),
  KEY `FK_Turno_Cliente` (`id_cliente`),
  KEY `fk_turnos_estilista_servicios` (`id_estilista_servicios`),
  CONSTRAINT `FK_Turno_Cliente` FOREIGN KEY (`id_cliente`) REFERENCES `usuarios` (`id_usuario`),
  CONSTRAINT `fk_turnos_estilista_servicios` FOREIGN KEY (`id_estilista_servicios`) REFERENCES `estilista_servicios` (`id_estilista_servicios`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `turnos`
--

LOCK TABLES `turnos` WRITE;
/*!40000 ALTER TABLE `turnos` DISABLE KEYS */;
INSERT INTO `turnos` VALUES (1,'0001-01-01 00:00:00','2026-10-03 09:00:00','Finalizado',1,3),(2,'0001-01-01 00:00:00','2026-10-03 11:00:00','Finalizado',1,4),(3,'0001-01-01 00:00:00','2026-10-03 09:00:00','Finalizado',1,5),(5,'2026-10-05 18:36:18','2026-10-06 09:00:00','Finalizado',1,3),(6,'2026-10-08 16:15:49','2026-10-08 12:00:00','Ocupado',1,4),(7,'2026-10-20 22:58:08','2026-10-21 17:00:00','Ocupado',2,7),(8,'2026-10-20 23:01:51','2026-10-22 10:00:00','Ocupado',5,7),(9,'2026-10-21 21:58:58','2026-10-26 18:00:00','Ocupado',5,7),(10,'2026-10-24 20:29:08','2026-10-26 14:00:00','Ocupado',2,1),(11,'2026-10-25 17:06:19','2026-10-26 18:00:00','Ocupado',5,1),(12,'2026-10-25 19:31:50','2026-10-26 18:00:00','Ocupado',5,5),(13,'2026-10-29 21:33:58','2026-10-31 17:00:00','Ocupado',5,7),(14,'2026-10-30 21:03:45','2026-11-02 16:00:00','Ocupado',2,7),(15,'2026-10-30 21:04:53','2026-11-02 18:00:00','Ocupado',2,1),(16,'2026-10-31 19:11:22','2026-11-02 18:00:00','Ocupado',5,7),(17,'2026-10-31 19:15:01','2026-11-03 18:00:00','Ocupado',2,7),(18,'2026-10-31 19:25:57','2026-11-04 18:00:00','Ocupado',5,7),(19,'2026-10-31 19:31:16','2026-11-05 18:00:00','Ocupado',5,7),(20,'2026-10-31 19:35:16','2026-11-06 18:00:00','Ocupado',5,7),(21,'2026-10-31 19:39:31','2026-11-07 18:00:00','Ocupado',5,7),(22,'2026-10-31 19:42:35','2026-11-08 18:00:00','Ocupado',5,7),(23,'2026-10-31 19:43:21','2026-11-08 17:00:00','Ocupado',5,4),(24,'2026-10-31 20:08:18','2026-11-09 17:00:00','Ocupado',5,7),(25,'2026-10-31 20:09:03','2026-11-09 16:00:00','Ocupado',5,4);
/*!40000 ALTER TABLE `turnos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `id_usuario` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `apellido` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `avatar` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `clave` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rol` int NOT NULL,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id_usuario`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=60 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,'Admin','Principal',NULL,'adminp@tuapp.com','claveHasheada',1,1),(2,'Admin','AAdmin',NULL,'admin@gmail.com','9sWmyqsrwhbLF+HVtSwrKdmJN+ZkyagQlHVjw+XqV7Y=',1,1),(4,'Harryy','Potterr','/Uploads\\avatar_4.jfif','harryp@gmail.com','gYQlWERzAYNFlltVIbrnegYpQ4UDqcn9w8TE5uAaw3g=',2,1),(5,'Dipper','Pines','/Uploads\\avatar_5.jfif','dipperp@gmail.com','iJULRzKIf2Luw+D+xC5clH6XaQO6lWzJKK+a+LbMHYA=',3,1),(56,'Stan','Pines','/Uploads\\avatar_56.jfif','stan@gmail.com','xiIJZfT8Dxc5k5nydriehzNxBYiZDhc8VYuZHX2mClw=',2,1),(57,'Mabell','Pines','/Uploads\\avatar_57.jfif','mabel@gmail.com','zXi23WhVTS2SXfdOGifREWeKu53Tgz0+q/w7XuHD73M=',2,1),(58,'Pacifica','Noroeste','/Uploads\\avatar_58.jfif','pacifica@gmail.com','r7coZZsqDvAq0ajOqzY38eWgtArarb59y7/Sg3m3uwU=',2,1),(59,'Bill','Cipher','/Uploads\\avatar_59.jfif','bill@gmail.com','N8LggtMf8xwBoyY5YhnZwtLfO/ZwiAMnM3+tKMydsXU=',2,0);
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'peluqueria'
--
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-05 17:50:38
