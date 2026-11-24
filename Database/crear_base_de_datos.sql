/* =====================================================================
   Sistema de Logística de Trailers - Script de base de datos (SQL Server)
   OPCIONAL: la aplicación crea la base automáticamente al iniciar
   (EnsureCreated). Use este script solo si prefiere crearla a mano.
   Si ejecuta este script, hágalo ANTES de iniciar la aplicación.
   ===================================================================== */

IF DB_ID(N'LogisticaTrailers') IS NULL
    CREATE DATABASE LogisticaTrailers;
GO

USE LogisticaTrailers;
GO

CREATE TABLE Cliente (
    id_cliente  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cliente PRIMARY KEY,
    nombre      NVARCHAR(100) NOT NULL,
    telefono    NVARCHAR(15)  NULL,
    correo      NVARCHAR(100) NULL,
    direccion   NVARCHAR(150) NULL
);

CREATE TABLE Trailer (
    id_trailer     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Trailer PRIMARY KEY,
    placas         NVARCHAR(15)  NOT NULL,
    capacidad_ton  DECIMAL(6,2)  NOT NULL,
    tipo_unidad    NVARCHAR(30)  NOT NULL,
    estado         NVARCHAR(20)  NOT NULL CONSTRAINT DF_Trailer_estado DEFAULT N'Disponible'
);
CREATE UNIQUE INDEX IX_Trailer_Placas ON Trailer (placas);

CREATE TABLE Conductor (
    id_conductor       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Conductor PRIMARY KEY,
    nombre             NVARCHAR(100) NOT NULL,
    licencia           NVARCHAR(20)  NOT NULL,
    vigencia_licencia  DATE          NOT NULL,
    telefono           NVARCHAR(15)  NULL,
    estado             NVARCHAR(20)  NOT NULL CONSTRAINT DF_Conductor_estado DEFAULT N'Disponible'
);
CREATE UNIQUE INDEX IX_Conductor_Licencia ON Conductor (licencia);

CREATE TABLE Viaje (
    id_viaje       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Viaje PRIMARY KEY,
    id_cliente     INT           NOT NULL,
    id_trailer     INT           NOT NULL,
    id_conductor   INT           NOT NULL,
    origen         NVARCHAR(100) NOT NULL,
    destino        NVARCHAR(100) NOT NULL,
    fecha_salida   DATETIME2     NOT NULL,
    fecha_llegada  DATETIME2     NOT NULL,
    estado_viaje   NVARCHAR(20)  NOT NULL CONSTRAINT DF_Viaje_estado DEFAULT N'Programado',
    CONSTRAINT FK_Viaje_Cliente   FOREIGN KEY (id_cliente)   REFERENCES Cliente (id_cliente),
    CONSTRAINT FK_Viaje_Trailer   FOREIGN KEY (id_trailer)   REFERENCES Trailer (id_trailer),
    CONSTRAINT FK_Viaje_Conductor FOREIGN KEY (id_conductor) REFERENCES Conductor (id_conductor),
    CONSTRAINT CK_Viaje_Fechas    CHECK (fecha_llegada > fecha_salida)
);
CREATE INDEX IX_Viaje_id_cliente   ON Viaje (id_cliente);
CREATE INDEX IX_Viaje_id_trailer   ON Viaje (id_trailer);
CREATE INDEX IX_Viaje_id_conductor ON Viaje (id_conductor);

CREATE TABLE Carga (
    id_carga         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Carga PRIMARY KEY,
    id_viaje         INT            NOT NULL,
    descripcion      NVARCHAR(150)  NOT NULL,
    peso_kg          DECIMAL(8,2)   NOT NULL,
    valor_declarado  DECIMAL(10,2)  NOT NULL,
    CONSTRAINT FK_Carga_Viaje FOREIGN KEY (id_viaje) REFERENCES Viaje (id_viaje) ON DELETE CASCADE
);
CREATE INDEX IX_Carga_id_viaje ON Carga (id_viaje);
GO
