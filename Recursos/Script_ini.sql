IF DB_ID('ClubPolideportivoDB') IS NOT NULL
BEGIN
    ALTER DATABASE ClubPolideportivoDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ClubPolideportivoDB;
END
GO

CREATE DATABASE ClubPolideportivoDB;
GO

USE ClubPolideportivoDB;
GO

-- =========================================
-- TABLA: Socios
-- =========================================
CREATE TABLE Socios
(
    SocioId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(150) NOT NULL,
    Dni NVARCHAR(20) NOT NULL,
    Telefono NVARCHAR(20) NULL,
    Email NVARCHAR(150) NULL,
    FechaAlta DATE NOT NULL CONSTRAINT DF_Socios_FechaAlta DEFAULT (CAST(GETDATE() AS DATE)),
    Activo BIT NOT NULL CONSTRAINT DF_Socios_Activo DEFAULT (1)
);
GO

ALTER TABLE Socios
ADD CONSTRAINT UQ_Socios_Dni UNIQUE (Dni);
GO

-- =========================================
-- TABLA: TiposInstalacion
-- =========================================
CREATE TABLE TiposInstalacion
(
    TipoInstalacionId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(200) NULL
);
GO

ALTER TABLE TiposInstalacion
ADD CONSTRAINT UQ_TiposInstalacion_Nombre UNIQUE (Nombre);
GO

-- =========================================
-- TABLA: Instalaciones
-- =========================================
CREATE TABLE Instalaciones
(
    InstalacionId INT IDENTITY(1,1) PRIMARY KEY,
    TipoInstalacionId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    PrecioHora DECIMAL(10,2) NOT NULL,
    Disponible BIT NOT NULL CONSTRAINT DF_Instalaciones_Disponible DEFAULT (1),
    Observaciones NVARCHAR(300) NULL,

    CONSTRAINT FK_Instalaciones_TiposInstalacion
        FOREIGN KEY (TipoInstalacionId) REFERENCES TiposInstalacion(TipoInstalacionId),

    CONSTRAINT CK_Instalaciones_PrecioHora
        CHECK (PrecioHora > 0)
);
GO

ALTER TABLE Instalaciones
ADD CONSTRAINT UQ_Instalaciones_Nombre UNIQUE (Nombre);
GO

-- =========================================
-- TABLA: Cuotas
-- Una cuota por socio, mes y año
-- =========================================
CREATE TABLE Cuotas
(
    CuotaId INT IDENTITY(1,1) PRIMARY KEY,
    SocioId INT NOT NULL,
    Anio INT NOT NULL,
    Mes INT NOT NULL,
    Importe DECIMAL(10,2) NOT NULL,
    FechaVencimiento DATE NOT NULL,
    FechaPago DATE NULL,
    Pagada BIT NOT NULL CONSTRAINT DF_Cuotas_Pagada DEFAULT (0),

    CONSTRAINT FK_Cuotas_Socios
        FOREIGN KEY (SocioId) REFERENCES Socios(SocioId),

    CONSTRAINT CK_Cuotas_Anio
        CHECK (Anio >= 2020 AND Anio <= 2100),

    CONSTRAINT CK_Cuotas_Mes
        CHECK (Mes >= 1 AND Mes <= 12),

    CONSTRAINT CK_Cuotas_Importe
        CHECK (Importe > 0)
);
GO

ALTER TABLE Cuotas
ADD CONSTRAINT UQ_Cuotas_Socio_Anio_Mes UNIQUE (SocioId, Anio, Mes);
GO

-- =========================================
-- TABLA: Reservas
-- Usamos FechaHoraInicio y FechaHoraFin
-- =========================================
CREATE TABLE Reservas
(
    ReservaId INT IDENTITY(1,1) PRIMARY KEY,
    SocioId INT NOT NULL,
    InstalacionId INT NOT NULL,
    FechaHoraInicio DATETIME NOT NULL,
    FechaHoraFin DATETIME NOT NULL,
    PrecioTotal DECIMAL(10,2) NOT NULL,
    Estado NVARCHAR(20) NOT NULL CONSTRAINT DF_Reservas_Estado DEFAULT ('ACTIVA'),
    Observaciones NVARCHAR(300) NULL,

    CONSTRAINT FK_Reservas_Socios
        FOREIGN KEY (SocioId) REFERENCES Socios(SocioId),

    CONSTRAINT FK_Reservas_Instalaciones
        FOREIGN KEY (InstalacionId) REFERENCES Instalaciones(InstalacionId),

    CONSTRAINT CK_Reservas_Fechas
        CHECK (FechaHoraFin > FechaHoraInicio),

    CONSTRAINT CK_Reservas_PrecioTotal
        CHECK (PrecioTotal >= 0),

    CONSTRAINT CK_Reservas_Estado
        CHECK (Estado IN ('ACTIVA', 'CANCELADA', 'COMPLETADA'))
);
GO

-- =========================================
-- TABLA: PagosReserva
-- Opcional pero útil para dar algo más de nivel
-- Un pago asociado a una reserva
-- =========================================
CREATE TABLE PagosReserva
(
    PagoReservaId INT IDENTITY(1,1) PRIMARY KEY,
    ReservaId INT NOT NULL,
    FechaPago DATETIME NOT NULL CONSTRAINT DF_PagosReserva_FechaPago DEFAULT (GETDATE()),
    Importe DECIMAL(10,2) NOT NULL,
    MetodoPago NVARCHAR(30) NOT NULL,
    Observaciones NVARCHAR(200) NULL,

    CONSTRAINT FK_PagosReserva_Reservas
        FOREIGN KEY (ReservaId) REFERENCES Reservas(ReservaId),

    CONSTRAINT CK_PagosReserva_Importe
        CHECK (Importe > 0),

    CONSTRAINT CK_PagosReserva_MetodoPago
        CHECK (MetodoPago IN ('EFECTIVO', 'TARJETA', 'BIZUM'))
);
GO

-- =========================================
-- ÍNDICES RECOMENDADOS
-- =========================================
CREATE INDEX IX_Cuotas_SocioId ON Cuotas(SocioId);
CREATE INDEX IX_Reservas_SocioId ON Reservas(SocioId);
CREATE INDEX IX_Reservas_InstalacionId ON Reservas(InstalacionId);
CREATE INDEX IX_Reservas_FechaHoraInicio ON Reservas(FechaHoraInicio);
CREATE INDEX IX_Reservas_FechaHoraFin ON Reservas(FechaHoraFin);
CREATE INDEX IX_PagosReserva_ReservaId ON PagosReserva(ReservaId);
GO

-- =========================================
-- DATOS INICIALES
-- =========================================
INSERT INTO TiposInstalacion (Nombre, Descripcion)
VALUES 
('Pádel', 'Pistas de pádel del club'),
('Tenis', 'Pistas de tenis del club'),
('Gimnasio', 'Sala de musculación y fitness');
GO

INSERT INTO Instalaciones (TipoInstalacionId, Nombre, PrecioHora, Disponible, Observaciones)
VALUES
(1, 'Pista Pádel 1', 12.00, 1, NULL),
(1, 'Pista Pádel 2', 12.00, 1, NULL),
(2, 'Pista Tenis 1', 10.00, 1, NULL),
(3, 'Gimnasio Principal', 8.00, 1, 'Acceso por franjas horarias');
GO