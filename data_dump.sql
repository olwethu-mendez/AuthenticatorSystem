-- MySQL dump 10.13  Distrib 8.0.37, for Win64 (x86_64)
--
-- Host: localhost    Database: authprojectdb
-- ------------------------------------------------------
-- Server version	8.0.37

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
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20251221140522_dbInit','8.0.22');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES ('4057e387-ba16-47f4-ac7a-750f47146ceb','Admin','ADMIN',NULL),('b0f3b685-2ca6-4fb5-a016-8d2e3936b11c','General','GENERAL',NULL);
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES ('691af53e-dca1-4f6d-9839-a34e16fd384c','4057e387-ba16-47f4-ac7a-750f47146ceb'),('014699d5-1063-4fb9-97e4-defc595739a3','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('015dab30-e9c9-49b9-a431-134269fb4f0f','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('066d0191-6284-48aa-83e7-a03abcd18589','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('1c0eac8f-ac1c-404b-b6c4-9765c1b980a6','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('25dac695-d1f9-4c4b-a48e-d773ef8544bd','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('3d9d1e98-b06d-46c2-b091-abe9eed67120','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('402d761c-379a-45ca-990d-8c0f0b91f5dd','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('6482474c-7f35-4981-9430-1126edc34fa3','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('7b756e4a-754f-4806-8843-0c144cb0cad1','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('9686ce47-28cf-4001-ba94-871d909363a4','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('a370f1c8-6949-44b2-80bd-6ae4cc69fdd0','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('b4b8e5b6-078e-4a21-9ad0-84217ed83831','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('b5694290-07db-46a1-a6fc-c014cd4fa632','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('be57202a-2c55-4f52-b834-355c0a7bd869','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('c94dadd6-b92e-4e0f-a80f-ca028d7cd45c','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('cad0a2d5-4720-4feb-b59b-3abbe4597628','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('cb9389e4-fd67-4ffe-a848-95caea7317a7','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('d8803d96-0b47-43c5-9a7a-40320d744729','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c'),('e48899cb-44db-4723-983e-65ff3b86aa41','b0f3b685-2ca6-4fb5-a016-8d2e3936b11c');
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CountryCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` varchar(9) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsDeactivated` tinyint(1) NOT NULL,
  `IsDeactivatedByAdmin` tinyint(1) NOT NULL,
  `DeactivatedAt` datetime(6) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES ('014699d5-1063-4fb9-97e4-defc595739a3','+27','731122344',0,0,NULL,'2026-01-02 14:19:55.161235','731122344','731122344',NULL,NULL,0,'AQAAAAIAAYagAAAAEAUoYUe3h6ieV7VjVqKNPn4n03PhEc7QqJ8QKLZGcdcLkePSyOhnsGRiEwf6LCC0hA==','2RNRCM3OGQGUBTDER6BNB4CGFEWMQAI5','389135d3-ba29-4b43-ba66-0b00738fb508',1,0,NULL,0,0),('015dab30-e9c9-49b9-a431-134269fb4f0f','+27','473829274',0,0,NULL,'2026-01-13 07:50:05.723174','rob.king@mail.com','ROB.KING@MAIL.COM','rob.king@mail.com','ROB.KING@MAIL.COM',1,'AQAAAAIAAYagAAAAEBiXAZVRU+oZda5DgrgMCab5yR1xZiz+kSQX9EQMWZFgA3/Efw23Yo/XefjF3F2t/Q==','FE6EAITYOOMVAC3GTZX2U4L44FRXHWIA','9f5866ce-4bc3-4e1a-8e09-4dafe2dd6b9a',1,0,NULL,0,0),('066d0191-6284-48aa-83e7-a03abcd18589','+27','711111111',0,0,NULL,'2025-12-30 23:22:29.651723','user.test@mail.com','USER.TEST@MAIL.COM','user.test@mail.com','USER.TEST@MAIL.COM',1,'AQAAAAIAAYagAAAAENBhNBLVomwTJ1QH7L05qdHHaTNIA+re/0UAsJn76hlrrC3DEziLtgHePDPiS9ND/Q==','NZGO7UTIFMZYUFD3WJ2XFZF25B7ETOS4','d236fe00-2f29-4230-ac0a-188a625c9134',1,0,NULL,0,0),('0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','+27','608001139',0,0,NULL,'2026-01-01 00:55:19.200496','608001139','608001139','o.mzileni@mail.com','O.MZILENI@MAIL.COM',1,'AQAAAAIAAYagAAAAEH+s6BWIzkEdt0Dwe7HUuNhHVIkkc+gLYG3VkdjDTFj+wQR3P7Ln5hhIBMpoEdF6Wg==','I737RDKWUP3TTB6IEUIQIBJE4KMQ4EPE','2d86c2ae-e79f-4cd3-8209-09f27554da23',1,0,NULL,0,0),('1c0eac8f-ac1c-404b-b6c4-9765c1b980a6','+27',NULL,0,0,NULL,'2025-12-31 21:10:45.290476','email.registrar@mail.com','EMAIL.REGISTRAR@MAIL.COM','email.registrar@mail.com','EMAIL.REGISTRAR@MAIL.COM',1,'AQAAAAIAAYagAAAAEI0eJl1Nawz4QFKBozCNxdeihDKoKu8tPIvbK7m/B6id1wCeePuMOz8nqk8HWxoaMA==','7UWMFH7KKS6TXIWDI3UIPXQHXTENQOKC','7dfc4456-13f0-4fa9-b0b3-33d72399408c',0,0,NULL,0,0),('25dac695-d1f9-4c4b-a48e-d773ef8544bd','+27','878983647',0,0,NULL,'2026-01-23 15:33:08.020705','878983647','878983647','mail123@mail.co.za','MAIL123@MAIL.CO.ZA',0,'AQAAAAIAAYagAAAAEFXZOtFWhKfUYVceiP/l8gClN5C7TlxUjiGcWrHtuHVBScAipJKQwH+rbnqhHmBfjA==','QHRZK4QF3AZDWP5NDR3IEO244E4GYDZ5','11547666-e581-410f-bdbf-977d634f0d46',1,0,NULL,0,0),('3d9d1e98-b06d-46c2-b091-abe9eed67120','+27','839474837',0,0,NULL,'2026-01-13 07:36:44.686271','testee.debouchie@mail.com','TESTEE.DEBOUCHIE@MAIL.COM','testee.debouchie@mail.com','TESTEE.DEBOUCHIE@MAIL.COM',0,'AQAAAAIAAYagAAAAEOmowd13xEmCTuwMGmKvwEOdp0F4RlSeqXTlqiqRmPl9jr15xJQj5KaKXR9hPWMSww==','KOAXP7BFMATVQI7RYNGW34S64CYZBT45','7412fd22-8b8e-4ea8-a756-93adea7504bb',0,0,NULL,0,0),('402d761c-379a-45ca-990d-8c0f0b91f5dd','+27','123456789',0,0,NULL,'2025-12-29 14:07:48.048171','test1@mail.com','TEST1@MAIL.COM','test1@mail.com','TEST1@MAIL.COM',1,'AQAAAAIAAYagAAAAEFPkUzw9HwydXL28Ef9ubN8E6MPNEE8jB3Vts3URrI+Hjm+n4Jn1MzmO7cUATervrA==','R44MNLOAHOXAVYMQACUB73VNZIEVMGI4','871b62a6-47b1-4215-8cde-43afaa060434',1,0,NULL,1,0),('6482474c-7f35-4981-9430-1126edc34fa3','+27','601234567',1,1,NULL,'2026-01-09 23:24:33.744610','601234567','601234567','olwethu.mendez@mail.com','OLWETHU.MENDEZ@MAIL.COM',0,'AQAAAAIAAYagAAAAEA2HB7FCv2spQHVh9OyMDsf6ahM/1WkdBQrwxr0x6Z3bclwm9ovsCXhxAnvDItT9PQ==','SENIYIN7OFTDQD625ONMRCUV2VQGEAPK','80e0b64e-3aaf-47e1-9896-680b83b9fa9b',1,0,NULL,0,0),('691af53e-dca1-4f6d-9839-a34e16fd384c','+27','812345678',0,0,NULL,'2025-12-21 14:10:04.813214','admin@authenticator-systems.com','ADMIN@AUTHENTICATOR-SYSTEMS.COM','admin@authenticator-systems.com','ADMIN@AUTHENTICATOR-SYSTEMS.COM',1,'AQAAAAIAAYagAAAAEJIq+Bc4ToZLwHQUvZ7cWNcXB4jlptrAwAYnxIz714qDV2dpyg3cWZjHNLAK/67EQw==','I7R6MZ34S4JNAUZUA3FZ6NOLLE4AUHZN','68a03f59-54ce-48a5-a17f-dd5dc1d2484e',1,0,NULL,0,0),('7b756e4a-754f-4806-8843-0c144cb0cad1','+27','678123098',0,0,NULL,'2026-01-26 14:15:14.510314','678123098','678123098','tester.userton@mail.com','TESTER.USERTON@MAIL.COM',1,'AQAAAAIAAYagAAAAEFmactmMAxftqyRUPtJndtrqKa0QX8N/gIL7v8UV4dF4CPBM0BfVkno2MKlkRSRCww==','KNHNWOHWYIGKSYX26UTXQLLB4U22M5IO','fca22671-4552-4f1b-963e-78e4caa5f4cf',1,0,NULL,0,0),('9686ce47-28cf-4001-ba94-871d909363a4','+27','721345689',0,0,NULL,'2025-12-29 13:59:20.264053','olwethu.mzileni@mail.com','OLWETHU.MZILENI@MAIL.COM','olwethu.mzileni@mail.com','OLWETHU.MZILENI@MAIL.COM',1,'AQAAAAIAAYagAAAAEJfiCabY3C0DuabkrDHpG2xjscBWGVa2rmueVy3gxh61bTJHGE7xB5gP/NBmmoAbjQ==','I7HMMWFJRF7SOTDVSMX5HDDCXNPN7XAX','7c7bdb28-1fde-48a5-be44-673bc31024a1',1,0,NULL,1,0),('a370f1c8-6949-44b2-80bd-6ae4cc69fdd0','+27',NULL,0,0,NULL,'2026-01-26 14:22:45.958586','userman@mail.co.za','USERMAN@MAIL.CO.ZA','userman@mail.co.za','USERMAN@MAIL.CO.ZA',1,'AQAAAAIAAYagAAAAEOipnAqv8ycEQZrS2zHfFXBw7hiS+RaBy+GrF350HY4iOF8hotqMA7yxUix7EjUzOQ==','XN7TBRWI5FVYXO6JYO6DFVW4SYZ7ZQKS','fde804be-109f-4595-b965-5c6e2c35c284',0,0,NULL,0,0),('b4b8e5b6-078e-4a21-9ad0-84217ed83831','+27','712345689',0,0,NULL,'2025-12-29 12:34:33.315755','user@mail.com','USER@MAIL.COM','user@mail.com','USER@MAIL.COM',1,'AQAAAAIAAYagAAAAEHOGXqw3FgLOibruMWeIaQpgLbayOdGcgTyLFh32uTxeKSQp+ETI2gApbHx9qzlFhQ==','NITI2JV2KFDCCTANYXM62NWCPZKNRMYT','952e52e4-9c3e-4b02-90ca-42761b649b6d',1,0,NULL,1,0),('b5694290-07db-46a1-a6fc-c014cd4fa632','+27','611231234',0,0,NULL,'2026-01-30 14:20:17.913612','MrExplorer@BestCompany.com','MREXPLORER@BESTCOMPANY.COM','MrExplorer@BestCompany.com','MREXPLORER@BESTCOMPANY.COM',1,'AQAAAAIAAYagAAAAEB8dzznaGEQG9ZPitn9IIjejyTWi8ZI5eLcgUCMdJHsolYgvXeuXsR0cKH868A2DDw==','D6X4DEKCVHBLBIF7IM75WEFRJOY5TDIW','9e7b18d8-d559-49b6-8129-6e4a425fbb80',0,0,NULL,0,0),('be57202a-2c55-4f52-b834-355c0a7bd869','+27',NULL,0,0,NULL,'2025-12-30 21:20:30.488562','test3@mail.com','TEST3@MAIL.COM','test3@mail.com','TEST3@MAIL.COM',1,'AQAAAAIAAYagAAAAEECzW+P8EXV3x9N5YKnR3WgsGlV2pDMFYgSlqXDQK9abGiu7Bs8KicL8axK5+OB3Nw==','5VZY3KQKOPVECXDFZHQDCUSJPZT4VDAH','b6df8e98-48bc-4f39-9855-6e5a54fcb6fb',0,0,NULL,1,0),('c94dadd6-b92e-4e0f-a80f-ca028d7cd45c','+27','727439949',0,0,NULL,'2026-01-02 13:23:17.050927','727439949','727439949',NULL,NULL,0,'AQAAAAIAAYagAAAAEIrpKh5h/t5QQ8+JFpHFZOe5Z4Uofou3TmLMqD79CkYr8paRRJBoQ9BCGJeEeo7XVA==','7AQLTNED3QMLXAZXTPQ5WA5OSWNUDYOA','3c38cc1c-53e7-49a9-9ed6-aa34e7b22bb7',1,0,NULL,0,0),('cad0a2d5-4720-4feb-b59b-3abbe4597628','+27','',0,0,NULL,'2025-12-29 14:24:07.021043','test2@mail.com','TEST2@MAIL.COM','test2@mail.com','TEST2@MAIL.COM',1,'AQAAAAIAAYagAAAAEFn4OPv0hGsaTJTVrzkUP/tuJUKZ+25UihOaAztenjAN5MoILS2X8nXI9+zcr+dA0A==','2RUSI3OZXLRKJ4P7QF5UTSC3ZSCQM5QM','6787e6c6-ca7b-46db-8f0f-a4b47891050c',0,0,NULL,1,0),('cb9389e4-fd67-4ffe-a848-95caea7317a7','+21','783947263',0,0,NULL,'2026-01-13 07:24:18.329842','tester.j1@mail.com','TESTER.J1@MAIL.COM','tester.j1@mail.com','TESTER.J1@MAIL.COM',0,'AQAAAAIAAYagAAAAEN+a/HEPdGlYCl6uhBvudP5j2/TsKiF52v1dj9PxhDMv3/5GZUtzMNIdkT9XOL9xxg==','AVY3BF7CQU7X2CMGYRODQYR73L7VDYWN','98b40baa-dafe-469a-ab9a-1640f9d9bcee',0,0,NULL,0,0),('d8803d96-0b47-43c5-9a7a-40320d744729','+27','712345689',0,0,NULL,'2025-12-29 13:16:24.157093','user.name@mail.com','USER.NAME@MAIL.COM','user.name@mail.com','USER.NAME@MAIL.COM',1,'AQAAAAIAAYagAAAAEOmMRPkp7DY4J2X0aPSCP4NwhTG3UzIpAJeThbauKxc7ccs2vybgWONNZpApDwyjVQ==','4G4IAAOLZEAHJEJMZW3RGAKZ545VCGJ4','d87fa8be-a00c-41cf-af74-50725db0e356',1,0,NULL,1,0),('e48899cb-44db-4723-983e-65ff3b86aa41','+27','772233440',0,0,NULL,'2026-01-02 14:05:00.777889','772233440','772233440',NULL,NULL,0,'AQAAAAIAAYagAAAAEFq4/aqTuY5Qb3zV+oYrSooiojeSHaceaLWJAiRrZ2LX3nALKhCm1j4vDiRZUV3zvw==','YJCATZPPNFVO7LGI7QOKEXOG7VUGN4PH','a9d5ff67-07cb-4ad5-9ced-42fb9b7c95f4',0,0,NULL,0,0);
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `previouspasswords`
--

DROP TABLE IF EXISTS `previouspasswords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `previouspasswords` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DateSet` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PreviousPasswords_UserId` (`UserId`),
  CONSTRAINT `FK_PreviousPasswords_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `previouspasswords`
--

LOCK TABLES `previouspasswords` WRITE;
/*!40000 ALTER TABLE `previouspasswords` DISABLE KEYS */;
INSERT INTO `previouspasswords` VALUES ('029395e0-fef7-4f01-b994-cbe2ec7b7aa5','014699d5-1063-4fb9-97e4-defc595739a3','AQAAAAIAAYagAAAAEAUoYUe3h6ieV7VjVqKNPn4n03PhEc7QqJ8QKLZGcdcLkePSyOhnsGRiEwf6LCC0hA==','2026-01-02 14:19:55.444673'),('063627be-8ebc-496d-bb1f-7fdec3f0e946','cad0a2d5-4720-4feb-b59b-3abbe4597628','AQAAAAIAAYagAAAAEFn4OPv0hGsaTJTVrzkUP/tuJUKZ+25UihOaAztenjAN5MoILS2X8nXI9+zcr+dA0A==','2025-12-29 14:24:07.740135'),('08833947-5fed-41b1-a60c-ae0096821993','402d761c-379a-45ca-990d-8c0f0b91f5dd','AQAAAAIAAYagAAAAEFPkUzw9HwydXL28Ef9ubN8E6MPNEE8jB3Vts3URrI+Hjm+n4Jn1MzmO7cUATervrA==','2025-12-29 14:07:48.222123'),('0af2ab26-b934-4b54-a2e5-49c0d9c6c735','9686ce47-28cf-4001-ba94-871d909363a4','AQAAAAIAAYagAAAAEE0j1vWOURAMl/1JUiVfQpUPOw+KrvFnVCZ6PXFZzyuT2OHx6pCz2vK39Ydfb+UMVA==','2026-01-12 14:09:26.242265'),('0c3a86e9-9fcb-4263-b229-ccf9a54030f6','9686ce47-28cf-4001-ba94-871d909363a4','AQAAAAIAAYagAAAAEE0j1vWOURAMl/1JUiVfQpUPOw+KrvFnVCZ6PXFZzyuT2OHx6pCz2vK39Ydfb+UMVA==','2025-12-29 13:59:20.554604'),('1c790d0b-cbb1-4c8c-bb79-0a55d848aff5','015dab30-e9c9-49b9-a431-134269fb4f0f','AQAAAAIAAYagAAAAEBiXAZVRU+oZda5DgrgMCab5yR1xZiz+kSQX9EQMWZFgA3/Efw23Yo/XefjF3F2t/Q==','2026-01-13 07:50:06.028951'),('42773e92-94fc-42ca-8eeb-2bd4a7dca32e','25dac695-d1f9-4c4b-a48e-d773ef8544bd','AQAAAAIAAYagAAAAEFXZOtFWhKfUYVceiP/l8gClN5C7TlxUjiGcWrHtuHVBScAipJKQwH+rbnqhHmBfjA==','2026-01-23 15:33:08.270307'),('5dd34420-c762-4cc5-814b-6d4da770d0c2','c94dadd6-b92e-4e0f-a80f-ca028d7cd45c','AQAAAAIAAYagAAAAEIrpKh5h/t5QQ8+JFpHFZOe5Z4Uofou3TmLMqD79CkYr8paRRJBoQ9BCGJeEeo7XVA==','2026-01-02 13:23:17.466179'),('6373b630-8326-465d-a3e2-f61578f2dce5','d8803d96-0b47-43c5-9a7a-40320d744729','AQAAAAIAAYagAAAAEOmMRPkp7DY4J2X0aPSCP4NwhTG3UzIpAJeThbauKxc7ccs2vybgWONNZpApDwyjVQ==','2025-12-29 13:16:24.378183'),('6999b09f-49e6-40bb-8b5f-ce8fc6a19171','7b756e4a-754f-4806-8843-0c144cb0cad1','AQAAAAIAAYagAAAAEO9lAUtByq54v4/YGcDzt5IIoy3gu9Mjar31G6ixnlCDljjvlbI542wK67tYWhNBdA==','2026-01-26 14:37:03.501593'),('72e3a3e4-74bb-499f-898d-07fb43ccefed','e48899cb-44db-4723-983e-65ff3b86aa41','AQAAAAIAAYagAAAAEFq4/aqTuY5Qb3zV+oYrSooiojeSHaceaLWJAiRrZ2LX3nALKhCm1j4vDiRZUV3zvw==','2026-01-02 14:05:01.032977'),('77f75e48-f811-4e92-a034-23e8ae777208','be57202a-2c55-4f52-b834-355c0a7bd869','AQAAAAIAAYagAAAAEECzW+P8EXV3x9N5YKnR3WgsGlV2pDMFYgSlqXDQK9abGiu7Bs8KicL8axK5+OB3Nw==','2025-12-30 21:20:31.204242'),('804a273c-664d-498d-bbde-9fd90aa2c477','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','AQAAAAIAAYagAAAAEMPywbwpwvF/1MMyTx952VSw6fGS7OZkHj3hYM8wi1PPKFgouKOeBuDj2+wDhh/Sdw==','2026-01-01 00:55:19.631484'),('94578d04-3f20-48ec-97b7-761793951d67','7b756e4a-754f-4806-8843-0c144cb0cad1','AQAAAAIAAYagAAAAEO9lAUtByq54v4/YGcDzt5IIoy3gu9Mjar31G6ixnlCDljjvlbI542wK67tYWhNBdA==','2026-01-26 14:15:14.833745'),('9a52455c-cc8c-4a6b-ae34-d1f4f8675ea9','a370f1c8-6949-44b2-80bd-6ae4cc69fdd0','AQAAAAIAAYagAAAAEOipnAqv8ycEQZrS2zHfFXBw7hiS+RaBy+GrF350HY4iOF8hotqMA7yxUix7EjUzOQ==','2026-01-26 14:22:46.138250'),('9aff706e-c22e-4593-aa2b-6c68ac2c2fdc','6482474c-7f35-4981-9430-1126edc34fa3','AQAAAAIAAYagAAAAEA2HB7FCv2spQHVh9OyMDsf6ahM/1WkdBQrwxr0x6Z3bclwm9ovsCXhxAnvDItT9PQ==','2026-01-09 23:24:34.283825'),('af08c90f-9e12-4f5b-bd72-e2eba229edc7','066d0191-6284-48aa-83e7-a03abcd18589','AQAAAAIAAYagAAAAENBhNBLVomwTJ1QH7L05qdHHaTNIA+re/0UAsJn76hlrrC3DEziLtgHePDPiS9ND/Q==','2025-12-30 23:22:30.153605'),('b815e856-546f-4d22-b35a-c152e1b26b70','b5694290-07db-46a1-a6fc-c014cd4fa632','AQAAAAIAAYagAAAAEFQBNMPMD/wb9HMI93mbnwto5ewWkhxNQkyKhyJH/Tziau42HcalDm6iG7ma3tgZZQ==','2026-01-30 14:24:24.477480'),('bd64f77a-7737-47c3-b12b-6162da65d5f2','cb9389e4-fd67-4ffe-a848-95caea7317a7','AQAAAAIAAYagAAAAEN+a/HEPdGlYCl6uhBvudP5j2/TsKiF52v1dj9PxhDMv3/5GZUtzMNIdkT9XOL9xxg==','2026-01-13 07:24:18.671698'),('c2857635-ab3b-432c-8e12-a4157019e5ae','b5694290-07db-46a1-a6fc-c014cd4fa632','AQAAAAIAAYagAAAAEFQBNMPMD/wb9HMI93mbnwto5ewWkhxNQkyKhyJH/Tziau42HcalDm6iG7ma3tgZZQ==','2026-01-30 14:20:18.651870'),('d4420aaa-f5e2-4ee4-9cdc-9f1377186465','3d9d1e98-b06d-46c2-b091-abe9eed67120','AQAAAAIAAYagAAAAEOmowd13xEmCTuwMGmKvwEOdp0F4RlSeqXTlqiqRmPl9jr15xJQj5KaKXR9hPWMSww==','2026-01-13 07:36:44.924611'),('e573212b-6c22-4210-8b49-39b58c6e0236','1c0eac8f-ac1c-404b-b6c4-9765c1b980a6','AQAAAAIAAYagAAAAEI0eJl1Nawz4QFKBozCNxdeihDKoKu8tPIvbK7m/B6id1wCeePuMOz8nqk8HWxoaMA==','2025-12-31 21:10:45.549440'),('f63f0030-d3e6-4e80-acd6-31409d4673b5','b4b8e5b6-078e-4a21-9ad0-84217ed83831','AQAAAAIAAYagAAAAEHOGXqw3FgLOibruMWeIaQpgLbayOdGcgTyLFh32uTxeKSQp+ETI2gApbHx9qzlFhQ==','2025-12-29 12:34:33.563966');
/*!40000 ALTER TABLE `previouspasswords` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `profiles`
--

DROP TABLE IF EXISTS `profiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `profiles` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FirstName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LastName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Gender` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `GenderSelfDescription` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProfilePictureUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ProfilePictureName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Bio` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DateOfBirth` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Profiles_UserId` (`UserId`),
  CONSTRAINT `FK_Profiles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `profiles`
--

LOCK TABLES `profiles` WRITE;
/*!40000 ALTER TABLE `profiles` DISABLE KEYS */;
INSERT INTO `profiles` VALUES ('06a45a36-2d6a-4da7-bc4b-f17a6f9af1fd','Testee','Testerton','Prefer Not To Say',NULL,'014699d5-1063-4fb9-97e4-defc595739a3',NULL,NULL,NULL,NULL),('13455af2-9e52-45c9-a80b-f000fc182961','Test','User3','Male',NULL,'be57202a-2c55-4f52-b834-355c0a7bd869',NULL,NULL,NULL,NULL),('34ccca47-eb79-4379-82a8-54a0971d194f','Tester','Userton','Male',NULL,'7b756e4a-754f-4806-8843-0c144cb0cad1',NULL,NULL,NULL,NULL),('482e0059-3c5b-4fe9-94d5-7fa73b96a82f','Olwethu','Mzileni','Male',NULL,'9686ce47-28cf-4001-ba94-871d909363a4',NULL,NULL,NULL,NULL),('4a5424b1-73d3-4828-9314-d453f3c8aca8','tester','jester','male',NULL,'cb9389e4-fd67-4ffe-a848-95caea7317a7',NULL,NULL,NULL,NULL),('6089004c-e675-4aea-9480-398b165962a2','Olwethu','Mendez','Male',NULL,'6482474c-7f35-4981-9430-1126edc34fa3','https://pub-679b98df876b4e2aa5566e07fa809475.r2.dev/profile_images/42.jpg','profile_images/42.jpg',NULL,NULL),('6e7be261-8593-4a72-b071-e5ea1fa02360','test 2','user','Other','gender neutral','cad0a2d5-4720-4feb-b59b-3abbe4597628',NULL,NULL,NULL,NULL),('75943a61-b70a-48d1-94c1-6802f435b04a','Robbie','King','Male',NULL,'015dab30-e9c9-49b9-a431-134269fb4f0f',NULL,NULL,NULL,NULL),('7bcb73eb-2597-4f34-b028-7f206c98753d','Olwethu','Mzileni','Male',NULL,'0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','https://pub-679b98df876b4e2aa5566e07fa809475.r2.dev/profile_images/1714132182630.jpg','profile_images/1714132182630.jpg',NULL,NULL),('9275cf6f-d856-4093-9eaa-6f9e67ed2b2b','whatwhat','whatsisname','Male',NULL,'25dac695-d1f9-4c4b-a48e-d773ef8544bd',NULL,NULL,NULL,NULL),('98994dd0-05d1-4357-8704-675e2afcaaf8','System','Admin','Prefer Not To Say',NULL,'691af53e-dca1-4f6d-9839-a34e16fd384c','https://pub-679b98df876b4e2aa5566e07fa809475.r2.dev/profile_images/User-admin-icon.png','profile_images/User-admin-icon.png',NULL,NULL),('b223e44d-d26b-42b3-abad-33d9644cd0d9','ExplorerKhadime','TheeExplorer','Female',NULL,'b5694290-07db-46a1-a6fc-c014cd4fa632',NULL,NULL,NULL,NULL),('b9490e66-c057-4fff-8092-89f857b44087','Test','User','Male',NULL,'066d0191-6284-48aa-83e7-a03abcd18589','https://pub-679b98df876b4e2aa5566e07fa809475.r2.dev/profile_images/33.jpg','profile_images/33.jpg',NULL,NULL),('d623974a-b85f-4bf3-94c0-e422948b2b36','Testee','McTees','Prefer Not To Say',NULL,'a370f1c8-6949-44b2-80bd-6ae4cc69fdd0','https://pub-679b98df876b4e2aa5566e07fa809475.r2.dev/profile_images/33.jpg','profile_images/33.jpg',NULL,NULL),('e94f4b04-e8c0-4210-9505-95b02c94f3a0','Testee','Debouchie','Male',NULL,'3d9d1e98-b06d-46c2-b091-abe9eed67120',NULL,NULL,NULL,NULL),('f1f08d15-b6f4-412c-9731-3b476dee3326','Olwethu','Mzileni','Male',NULL,'c94dadd6-b92e-4e0f-a80f-ca028d7cd45c',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `profiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `refreshtokens`
--

DROP TABLE IF EXISTS `refreshtokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `refreshtokens` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Token` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `JwtId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsRevoked` tinyint(1) NOT NULL,
  `StayLoggedIn` tinyint(1) NOT NULL,
  `DateCreated` datetime(6) NOT NULL,
  `DateExpires` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RefreshTokens_UserId` (`UserId`),
  CONSTRAINT `FK_RefreshTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `refreshtokens`
--

LOCK TABLES `refreshtokens` WRITE;
/*!40000 ALTER TABLE `refreshtokens` DISABLE KEYS */;
INSERT INTO `refreshtokens` VALUES ('0508e7da-8b60-4d82-a5d8-67a364c99418','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','57965a5f-dfa5-40d9-b7fa-4991efb1d861.dTOhIksdNgi/0bGc7KeFYFiv9nilIdN7EvXIk7MXdNZSve+SkL2gRsTGPIMGJ/9U5MwDCC0j8aN3Kn7DPKMj3Q==.b79e9be3-e97f-49d3-bf85-c10ecbed6244','b345b3ec-5e76-44f7-a160-985297473fee',1,0,'2026-01-28 07:50:05.013454','2026-01-28 19:50:05.013455'),('065470f5-43e7-4a6c-afce-e1559ac6d6b2','6482474c-7f35-4981-9430-1126edc34fa3','84a0cbd2-3065-4c9a-a6bf-541f2012c1e3.DwhEoygLpcLnroLN/bccbRk21wZCc/JhaRA3ChvBKpiuivOffzEoz8H2aNpon1jx1QJVKDSVxt0Vl9a2UUaD8Q==.61b945c4-fd79-4b8a-a266-4afad778aa54','36186641-4b99-4953-aef4-cb76c97abef1',1,0,'2026-02-13 19:33:39.267905','2026-02-14 07:33:39.267905'),('073619c4-633f-4622-aa54-622f9810a57a','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','6e6124c7-eb02-46f7-aac1-40d3ba966031.RKfpI3AfqsLnmsXKMA5CO80o6suJYmwPjPYEiaSHp4PBjaHIZbmR/M46ReLTI4G444+Pltj9k6AI0Ss8dyQDJg==.6124d8a5-c424-4132-a262-176e0096a165','a3141934-e2f1-44b8-a4f8-dcce7298ce68',1,0,'2026-01-28 07:44:06.934566','2026-01-28 19:44:06.934567'),('0c798a6b-5da7-4611-a74a-7e7c7a730fc0','691af53e-dca1-4f6d-9839-a34e16fd384c','da167c3e-fcff-4be4-9bc6-1a1356331971.oSPOU4G0RLRCjhUn39qMsTAtmdsAbSCYCLHMUXKNGLB770HhOQWkYnhenQofGEMEdyJ2/GtZAg6lDG85Z+gr2A==.11f42a77-9314-4b59-bac3-7c4f7dfb2fc1','378bb329-34e0-4a20-8131-b24671bdc03a',0,0,'2026-02-26 14:34:29.796202','2026-02-27 02:34:29.796292'),('0fe01454-09d8-4072-8ef9-a385bdf13b1c','b5694290-07db-46a1-a6fc-c014cd4fa632','5b9a7a11-6dfa-4f99-b301-53f3dc198677.+r9sVhgmhFR/M7ALT4NIE9xCrRY6aOEzZSJ6wHJUQ1F3km1hwjQmQKHkFXoJsLAn+EK+rG9mEZcyA8uMLCzt/g==.91d9c2c9-af2c-4555-9db3-8e4a63a6a4c7','20eee556-0985-44cf-ae25-aa2f66445a9f',1,0,'2026-01-30 14:20:58.127532','2026-01-31 02:20:58.127532'),('137f4bd1-fda2-4d31-ac21-adc697e8e80f','a370f1c8-6949-44b2-80bd-6ae4cc69fdd0','a60f1d03-9dd3-40f1-98cb-32dddd23bb97.sh0jWglvnxMrVAUyrrgMHQ0eXoIYAmj54x2TmmAEcsJeC5oetWOa1h9zgitjWw1l6Za6eR4v+uPNBrBqW+EyEQ==.f54d9fb3-d4a0-4521-8b16-34aa4efe2819','bbb04ab8-8552-4bd1-a1c1-2f70b2536d44',1,1,'2026-01-26 14:33:46.077791','2026-02-25 14:33:46.077791'),('20358eef-94c9-4502-aa65-9c76aaee4d62','691af53e-dca1-4f6d-9839-a34e16fd384c','51fd52e1-fbb0-40c4-b9a7-e6d31c156443.iJx0f45jc1+SWzfTCLq6jnzriowR//bxdvLjJS6/m99S+1vj4khRZtcvh6VmgfQ9VHGFLSVbDW3gcFAvDNF2gw==.25084825-7eed-4ba1-9eda-be678b85b715','68d1add1-ce15-4b00-b366-e0e1e47f38c4',1,1,'2026-01-21 10:35:14.158481','2026-02-20 10:35:14.158481'),('27e9f491-0bc3-4e85-9356-c052562e5c81','015dab30-e9c9-49b9-a431-134269fb4f0f','6cedd08f-4ec7-4717-8533-1831c9b41db5.8mn2fj1PyK7qpZLtnwff5f1Dem8qbSR66dT3Q65Nlas0TuAZjcXnjACvj18wnq3ed0xVoffunTpJLbVlpp1Jvw==.23d4ddb5-3856-411b-9a5f-e7166822a8fa','ad6c07cc-9b28-451f-923e-9a3998ea8b74',1,0,'2026-01-29 10:08:10.669562','2026-01-29 22:08:10.669562'),('30fe9dde-3a57-40fa-8a92-9b7f29599cc8','691af53e-dca1-4f6d-9839-a34e16fd384c','10ffa63f-58d3-45c2-9a40-fdd0525551d7.nHKvTchjwmq/7LG8Qs/Hrwf4L8dzH+U/nzHCye5xwEpbRpzzDVGRRAHk6geTkKnzlAGpzrjhmPK5COI6lmx9iQ==.e15d0762-829b-4c99-bc5e-84b22583ff4a','e491bcb2-1784-49b3-9d71-2ac8bda7369f',0,1,'2026-02-13 19:22:59.938469','2026-03-15 19:22:59.938686'),('32a34a4b-601e-4b42-91d1-2729ca66f3ed','b5694290-07db-46a1-a6fc-c014cd4fa632','c98e6a85-9923-4d81-8a35-b0a6e7bc70e8.ivQkeA9vXfaERBbQZ+dWpjeJWahIbKQazlbNs2h4NirMn3eEBdlgNOWaWveyWEy1a/9wzF7jDI14/lpEpXk+PA==.73a3029e-5251-45ba-98be-5524545571ac','a28fbfb0-b4a9-4e5a-a4f2-878c957e44a3',0,0,'2026-01-30 14:50:26.534821','2026-01-31 02:50:26.535005'),('369e0a7b-ad72-4a94-a7dc-b42d132d6e43','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','47ca7f7d-6dd4-4e1b-b266-20008ba65e84.hK4ac8R9U9TdZFHEK5vgyEbzKUgbxwBaYIVxbaKIOMUNvNL7MdhPW0HC/Bs8opLmQsIrk0kT6+pT+PHa3PbpOg==.03369d53-f42b-462d-8370-631f7c94d414','376dc42d-7a34-41b9-82e7-5ecd2cde7b35',1,0,'2026-01-28 09:55:04.282335','2026-01-28 21:55:04.282467'),('375bf14d-60c7-4dcb-8f52-b71baab748a7','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','08eea258-2020-4957-8740-783f67ae6b1a.EWt2kPLD7KE6EKxtaLootBZo3OCFuuSSYY8/olxNZVtIoDFQz5ArOG7fuaoYvfPrcoPfqSKArZt1fUgnwTDp1w==.cfee5568-a97e-476c-9660-eaac438e066b','b1ef97d4-6d6e-47e4-83c6-9bf5760955ec',1,0,'2026-01-28 12:09:24.650864','2026-01-29 00:09:24.650996'),('3e5e5973-fc87-427e-ae7b-35c7f39389d7','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','d483ec0d-ad65-43e1-a05c-d7879719cb38.WrkhaOifLxGeXjD8gWMDlwGzxCoDFnqQRKrKBLv0CP6bgVSeUB6ULNcMk7EO5V8qRtZ5g65r7h3MwHkl7sgJKQ==.5c05846f-96e0-4521-8098-52a9ae95d50e','7db0f370-8764-4572-8a09-a480a8c2b9b7',1,0,'2026-01-29 09:13:13.325171','2026-01-29 21:13:13.325171'),('3fa3cade-b950-4871-a23a-5a81a08d6572','015dab30-e9c9-49b9-a431-134269fb4f0f','d938972a-7ace-4eab-8314-99efc7af95ea.OP/ZLHIbVzXtIrxFAKeWPhcSPTvFovZOljh1oOwEuZ1R36LX6tjRpMVusnSXxVerx2t6bcjQBzgsTKmmd3d0Sg==.ca2f8133-d977-4a67-8575-254d1639a7a2','6946da33-d041-4ce3-9839-5476d1e34888',0,0,'2026-01-29 12:39:53.649830','2026-01-30 00:39:53.649831'),('47e3cc58-ef36-4acf-9b13-7843380af5bc','7b756e4a-754f-4806-8843-0c144cb0cad1','e0a74b4f-eaa5-45d4-b710-349712f20cbe.Zhx6j0vLWiI+tJ+3I6B+QZKiqTHwSg5uLer9uSjQY5/U4kgfqrwia7vbzl8R3375OVI8D2OEYndM44irgwhOMA==.a8c8d026-b3c3-44b7-83f6-b58cdd8d9610','8c0ee7c8-1395-40d5-a4c7-6abcb319e6cf',1,0,'2026-01-27 15:23:38.948611','2026-01-28 03:23:38.948612'),('4ed719d7-6dd1-4c64-8d11-806df6c69a1e','7b756e4a-754f-4806-8843-0c144cb0cad1','8e093bc7-2ee9-4846-a296-87f18979256f.wcyZHVkVFezB1PK3qXizO9ucypJZAZ/yCuwf7SgOKnqApXDfcqsttJoXXFsGoCeECd6tz3/BlcsVVXwSbpnO3Q==.09f4f20d-bf5e-48f1-915f-b5e32a580280','611a80de-9a9d-458d-a43f-166454257ce9',1,1,'2026-01-26 14:19:41.990253','2026-02-25 14:19:41.990253'),('509a3f25-7df0-40cf-a1dd-b19457c6c5a8','b5694290-07db-46a1-a6fc-c014cd4fa632','9cdb2bc2-adff-47f9-9d30-14b75730f57a.jjaqOV5vWcWpc0hB70lclFf5Efo12xv2Rg1QcLhbN5qbLWPjop1rFZno6B98KgpTZsmre0vvpFd6+AS1M5qalw==.6d3383a1-910e-4dec-941b-ab67111ad4e0','059f31cb-162d-441c-81f2-300200566030',1,1,'2026-01-30 14:21:14.650859','2026-03-01 14:21:14.650860'),('58e725e0-5803-4127-b003-186f60bdb2b9','6482474c-7f35-4981-9430-1126edc34fa3','051a02d2-0d98-497c-b468-103ac8e7e190.ezo6jfF84FnT/1JQbut3YUCBTKZTsV48FwNlkXfUMrvpUJiJEoh0QfnwcPKZVOSxNv6iGv8E6m1qNvchqT1dYg==.61312791-1994-4630-99ec-0e5b90303d16','ed4afa3e-e6e2-423b-aff3-c9eaf453b86d',1,1,'2026-01-10 00:05:22.790889','2026-02-09 00:05:22.790889'),('59701736-95fd-4285-8f61-c21333111b04','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','bf94968f-41b4-4b91-8b4c-0245656745b3.DlbM0nWO/90aXiIfFFdP0BdLMfNXN8pkCGJfiYFa5tbGhRFySZd+wMJ71+81Orx983jB/z54tGKkZBrhORedwQ==.f5ab7517-dd00-4617-b73d-01ea3a2c4779','4da66c98-a87d-4f5d-83cb-e250f33482f3',1,0,'2026-01-28 11:00:37.286564','2026-01-28 23:00:37.286704'),('6b22f431-56d9-461a-a8e8-b4646f6afa3a','015dab30-e9c9-49b9-a431-134269fb4f0f','18014c67-ab16-43eb-9d7d-7d523dc94d22.JbxSe9D8y+a0A0QJGUHJ5EJjDNPpc+044kuqQZAPAMjSbqRlsXVt8xvGJNYyAO+tY/1/e7qEfFAULbyCT6O7yQ==.38b1e3a0-096a-4c6a-a91c-02cd772da524','a93c59bc-ec24-40fc-a1b9-8820b64e2a08',1,0,'2026-01-29 09:58:45.498187','2026-01-29 21:58:45.498187'),('6c60be3c-6c1b-46ee-93b4-890573e1e7e6','691af53e-dca1-4f6d-9839-a34e16fd384c','ad9b6150-1dcc-49ed-ae6c-152272ec0a9a.jKcIl9Uui06vqqZrNwyIN8oP4/nbOy/NZTPQ/oxQK1sSdjm3xB0/ZIdbDFzPu6Iz7AJ6PIJB+xOm5gyith2x/w==.e963cb8c-159b-4032-af46-735117abf116','163b7c14-105e-44b6-a5f9-c5b7a1486936',1,0,'2026-01-28 11:39:15.426983','2026-01-28 23:39:15.426984'),('78220067-f4f1-494d-afb3-d3cc7fdcf6be','b5694290-07db-46a1-a6fc-c014cd4fa632','fc525dd0-0378-4e24-976d-412e50f1af31.hygWU8IVt6EtQrjox8wSSf6WLdo/ddYDbqV4YgZW6e/pHsgs2U4Dpvk9v543YXlhHrHWpvH/S5IY9oDoSCXTdw==.0c45558b-f12b-4484-8c6d-6ca4db135ec6','6acc316a-bd81-4f81-ae06-3ca5c7ce3ce2',1,0,'2026-01-30 14:20:26.189199','2026-01-31 02:20:26.189199'),('7b2f6830-c59b-4ce5-953a-50066da60f6c','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','29e9beb7-20f7-4c67-afa0-0df881f612df.VR2kgpLBtJ21NDm/lZnqwpl8XMYia887zu5bJMCVHCzrWxi5y0dFS1HANXJ+JnnWeJksKtKpIoiFxxE+SDgubg==.ff93c6da-dfa1-425b-8ffc-ebc6f7609f62','5b9cf5c2-500f-40a6-b66c-7b7de28e4f1f',1,0,'2026-01-28 15:35:12.629755','2026-01-29 03:35:12.629756'),('8734d3aa-b25f-4a6e-a48b-32f8fb2f10da','b5694290-07db-46a1-a6fc-c014cd4fa632','d315e6a4-c28e-40bd-8826-84cfb6ac83f2.mlgPb6h2BP3pSa26y/tqIbFRddPu0ByL8XJ4awoTMXLKv3h0zZ+D4k8A7fEjED88pC7B5wLz8EI2POCXP+1D1Q==.4ed53e61-4487-49f5-9033-83eb352b0912','d3a5503e-1e63-41bb-b07d-64bb4a96699e',0,0,'2026-01-30 14:25:16.663510','2026-01-31 02:25:16.663510'),('8734e620-fce5-4401-bb6a-2210d59aaff5','014699d5-1063-4fb9-97e4-defc595739a3','f8117132-66b6-4fed-8266-acf957e51c58.IblwDdodf5VjO1aRvvuj5xiOcTAeOO5493BeoxV5IaEaCH1uYX3KyQ5dxNAEwlMkyFZlaFM0n6fvbiN7AqffLw==.1de98e71-6bcc-47bd-b97a-5a1920472b1d','e604af86-c87f-49b7-998c-e3e309393e0c',1,1,'2026-01-02 19:15:20.400636','2026-02-01 19:15:20.400636'),('89db1ca7-8200-4f63-8934-e00c7f9c9aa1','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','6ad4f6b9-fa86-4db3-ab0b-8e31f5aec8ba.YyPddhhs9DWxBi9yQ1yzHXAMjUkZiwL0oORbr5AaMz7s7jb7cvp3TMatpEBTWVoCrA2uNaTdTIN2j/LbkUyJMA==.2268fb1b-fca3-41e6-8645-6b15b13a989e','36cb8172-f4f5-4cb1-b05a-fb1603e4387b',1,0,'2026-01-28 14:56:53.892574','2026-01-29 02:56:53.892575'),('8ac43159-dd8f-4c7e-9639-90a4b7d3d6c6','7b756e4a-754f-4806-8843-0c144cb0cad1','221c6f80-9bff-44e0-bb9b-f8440aa22ca0.0FcLa4CrCrmFar1NlUNjyvV/DHSoS5WSgzfLkCnK1zSY5TUCv4H2Ajxp6sm9e7k/yxsIUBnN3iiyCnJrXjjRkg==.d0bba8c3-470f-4434-8c0c-727244d5c742','14193cae-f984-4566-b8d4-008c0c7b8cf6',1,0,'2026-01-27 14:52:08.720770','2026-01-28 02:52:08.720903'),('8c936e4e-3f24-41a8-b4b9-fcab6fd006ef','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','c320d366-1ee9-43da-bb06-5b20dcf1a9bd.nVcKQmQSAMwcnv89D3UMsz0lfYGDTzNE3BdmtmOiNiS0Lx9K9URbAwRrxWqxRxj/IU/89XXVYEQsL/dZJCh05w==.e6e8a3aa-3982-446a-b79f-26ceb8be598a','212eea16-b15c-4bf7-8642-4acdb1fb823d',1,0,'2026-01-28 15:02:21.584851','2026-01-29 03:02:21.584852'),('8d0abe04-0048-45b0-aa13-de7015431654','25dac695-d1f9-4c4b-a48e-d773ef8544bd','fc9ac856-cc2a-42c7-a7e0-5340d045bc7c.CfYZAjx9MB52ZaG7wdHcBVk+hvVqNcSzfmWxudxPMZaBQDPZdDt6FYKFL5tQi1zO5HXnfuOI4/UfPQWmT5ogng==.efe53679-504d-4501-89e4-0c643f549dbf','199dbb4d-b243-4dce-bbc4-b26606fd4a07',1,1,'2026-01-23 15:34:56.817901','2026-02-22 15:34:56.817901'),('984cf6c1-c2b7-439e-a3ab-1477187bca1f','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','588ada01-e824-4fe3-8562-734fca29b7aa.DgjbFJp1Fxc9B0vGrSdZ5CnkqTf1TPaSGA80eHmD6pQoCHFk4m0n3o+WFHMwvU7ttca89gT20t/HeDInwayAJQ==.7009b54a-3886-4238-a3b3-41b339e0c4fa','bfd7d53b-91ca-4214-924a-4663af3c87c8',1,0,'2026-01-28 09:11:05.824775','2026-01-28 21:11:05.824775'),('9b44a9aa-06cb-4f30-b7a6-429f991906a8','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','0c4845d3-be8f-4293-81d6-370a78505dfc.CMma2JAG2at7czzkYwTwDXtGuZ6qIUfYtFJ4VKr1OMD1dCZPPS4SyhOVzo1xmFFPxTHrWQhvKhKC9j4Ee5M48g==.9f5e7238-6cbd-4925-880c-a1dfcde175b4','e183eac6-5e6f-4222-a69e-a745a9010eb1',1,0,'2026-01-30 10:11:11.043336','2026-01-30 22:11:11.043487'),('ac5eb280-6298-45c1-bdf1-9506095a8dec','7b756e4a-754f-4806-8843-0c144cb0cad1','89612d1f-9c8f-4ea3-ae8f-a44f4a041370.3PYkUNMfJ7DqbSg0KClHE970DsZeBNHIRb219DdzfZhHtiw7qgU/z+05F7w2Exx9/yJS6izl31guw3jF2MDYHg==.50c1a8aa-994b-4004-be34-ba08fc79d6f7','f321e91c-c46f-4636-b978-a0a82976ae9e',1,0,'2026-01-28 12:14:53.606335','2026-01-29 00:14:53.606335'),('b019379a-16b5-46dc-9c45-6ebe31cd427b','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','a71727ea-f980-499c-8ed7-4c8bbdbfb641.JvPpEtwfKf2X0HpgWrpdf2i44rT73f217kuVh/ppKm2ANLlLFJ9UKRPMtu73AJ5wb4O0o5bSei1EUxx12+zHaQ==.ffe04f25-af2a-4944-bf23-1c4f99db1bac','05b91877-cdf9-4ff9-869a-d8709b7a5bc6',1,0,'2026-01-28 15:07:10.805885','2026-01-29 03:07:10.805964'),('b4a2c9e3-893b-41cb-8d4f-72e0abdd16f4','7b756e4a-754f-4806-8843-0c144cb0cad1','94cc3957-2fec-41d6-a7b4-5f7fdb7408fa.P7sHoSZaCzY9dFpxGRkcipMQzplhPAOkKFcPrb+ZvYEHa6OpFOd4/dgr9t0e/rf2ALM9lX2hSQLp7JhUJoY0lw==.4631fa93-0c7c-4d0d-b8cc-d71ebc45b166','f73350a8-97b7-4546-8a8e-d4148e218500',1,0,'2026-01-27 08:57:51.058832','2026-01-27 20:57:51.058940'),('b552d7b0-2f0c-47cf-a173-1586f29159e2','7b756e4a-754f-4806-8843-0c144cb0cad1','e15d5081-5595-4b55-b1d7-39103f46bd63.nhWn7fTUxpvLwDcpMAWJGAWhhcYRru9U5CJWaqJN/QPUj6CmisrNKKeS52PagjE49zQiNZP6wBGg9H399ZKgAA==.49d9ff41-545c-4024-8086-feda9dd85620','57ebd2ba-19c2-4885-8997-16e5acd61847',1,0,'2026-01-27 15:25:44.080222','2026-01-28 03:25:44.080222'),('b959f131-c82b-4f0d-9280-7d0a46e624cb','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','00edfdc3-db51-48ec-87d0-913427e902d2.aO8fSjrxF0UG797nQWAtUeumnPzi61p3ZQxfLe2WNmQpto37wYInWna2w9j++AWIdxi//2lSBVSlwnoXCuMrxw==.5408ad5b-4db6-48f7-9fcc-293994413609','0334ffb0-8b53-412f-a95f-c1bdc660e82e',1,0,'2026-01-28 09:13:33.236465','2026-01-28 21:13:33.236465'),('d51ea5d2-fdc4-4d52-a194-321236058870','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','97948e40-b5df-41ad-8588-d02fd95783a0.ho04Nke6f2Y1tO4It9lBu0TLcjeWW+xXEVMttikpv+icz0T3dpAKP0/G8nuxntVZ1d2d8Nx32nWDwnKjxmHhiA==.7db693a7-19a6-406c-a658-a62c6e55599e','30590876-0714-4c74-ac72-0257a32ee962',1,0,'2026-01-28 13:56:52.594250','2026-01-29 01:56:52.594250'),('e0264b5e-8a07-4eb0-8248-51024f863633','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','33b5d025-6fce-4f5c-8fde-bbd8fcdd8e56.m23NcPL+6vE48bzOVKJDwHvmssYFUqITGDYVQY4b24TmDHKaqPh0M9r7g3f4mShczQS8tl2Dkqg/JtCOhdL78A==.6ac5abaa-6b16-4686-97f2-f8e8617712d4','8c57570f-9516-4d11-b9e5-bd824d2d7b63',1,0,'2026-01-28 09:58:35.787292','2026-01-28 21:58:35.787292'),('f5c3ba62-9ac2-48a9-9aa8-9ac8fdabc8ce','7b756e4a-754f-4806-8843-0c144cb0cad1','6dcf39c9-b0da-4656-800c-64130f66ec9d.I5pKxcL0VOIY9m16jUvhW5hOE/6K1phigkLnxLA1eaJfB3vQlr2yPahlgLMHenuU/4Q0B48WdtYLynpb+7E+pg==.fa869d49-da64-4b5d-b43e-8689158508d0','13ac7b0f-6941-4e41-8b16-7fabaa80d852',1,0,'2026-01-27 15:21:56.213735','2026-01-28 03:21:56.213735'),('f73ab142-9a3c-447f-af9e-49fd077ac769','7b756e4a-754f-4806-8843-0c144cb0cad1','65117d42-f3c6-451b-9ebd-b40f3b18546a.zSnPSIqoBBxeh7o56xHBKyPutDicI42KyXGoWBcGlXPcMptNNmGfWtR3V7LrN0GFXuCxjpOhnDa62yyOE21bZw==.e549cb4b-53d2-4094-8757-435ede173bcd','7743963f-5de0-467f-b61e-2656d243ceba',1,0,'2026-01-27 11:56:07.409467','2026-01-27 23:56:07.409546'),('f77dbccd-332f-4e9a-9abd-6e533ece24de','0c9aac0a-ac3f-4b1b-8251-5eb1ca671d0e','7522f646-0f11-4fa7-b798-2acbc671eba0.Perbvf0rt4Z9aa186A6CaJLkRhVr1uKrnGHJEqO1stEjWgEM4QM3eNFWRU+wlgB2LWkbzGc8q3+vcwV7eBxuyw==.afa43224-02e2-4a96-8dad-abe2304d50f1','60856f38-22d1-4545-a925-78edf1bac025',1,0,'2026-01-28 12:05:57.017683','2026-01-29 00:05:57.017684'),('fa40dc9e-c11f-4362-839b-09d4c6f8a195','691af53e-dca1-4f6d-9839-a34e16fd384c','afa5e043-67ca-4dd1-a5e2-f98b6ad5f6ee.SM9ZVfBLmJsVjRz5L+c+zJr3owMox7XZ3VQCnJVSWGZBXh2I12FKC4ZcfQ07KcLZEJQwdnvW+gkkBDeEA9zYZQ==.83518e57-2847-49e8-8e63-3a87d79e75ab','3c15f199-9d9c-4814-87da-ce47179d2522',1,0,'2026-01-29 09:56:09.194303','2026-01-29 21:56:09.194304'),('fe6dbef4-b351-4a9c-9da4-0c036e8c52cc','6482474c-7f35-4981-9430-1126edc34fa3','af64d34e-0ffd-4077-8664-b0d97776e995.yX2gVRvrbZLnZCNkG8zhX3ojUOIpXQJ2h8zlx20tR1wLjnTXRajEMb4ZzQ17ihvehO5SOjsELlCamRRvS2c5xA==.7f80df5e-4a22-408e-9dc6-797a06317fcc','a5488716-086c-4f25-903a-014e8f64a426',1,0,'2026-02-13 19:30:49.532255','2026-02-14 07:30:49.532256');
/*!40000 ALTER TABLE `refreshtokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tokenblacklists`
--

DROP TABLE IF EXISTS `tokenblacklists`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tokenblacklists` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Token` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ExpirationDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tokenblacklists`
--

LOCK TABLES `tokenblacklists` WRITE;
/*!40000 ALTER TABLE `tokenblacklists` DISABLE KEYS */;
/*!40000 ALTER TABLE `tokenblacklists` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-02-26 17:01:28
