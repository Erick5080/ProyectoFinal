USE [ProyectoFinal]
GO

/****** Object:  Table [dbo].[Administradores]    Script Date: 12/17/2025 12:41:33 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Administradores](
	[AdminID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[PasswordHash] [varchar](255) NOT NULL,
	[NombreCompleto] [varchar](150) NULL,
	[Rol] [varchar](50) NULL,
	[Activo] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[AdminID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Administradores] ADD  DEFAULT ('SuperAdmin') FOR [Rol]
GO

ALTER TABLE [dbo].[Administradores] ADD  DEFAULT ((1)) FOR [Activo]
GO


/****** Object:  Table [dbo].[Clientes]    Script Date: 12/17/2025 12:44:05 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Clientes](
	[ClienteID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Email] [varchar](100) NULL,
	[Telefono] [varchar](20) NULL,
	[DireccionPrincipal] [varchar](200) NULL,
	[FechaRegistro] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ClienteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Clientes] ADD  DEFAULT (getdate()) FOR [FechaRegistro]
GO

/****** Object:  Table [dbo].[DetalleOrdenes]    Script Date: 12/17/2025 12:45:29 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DetalleOrdenes](
	[DetalleOrdenID] [int] IDENTITY(1,1) NOT NULL,
	[OrdenID] [int] NOT NULL,
	[ProductoID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitarioEnVenta] [decimal](10, 2) NOT NULL,
	[SubTotal] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleOrdenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DetalleOrdenes]  WITH NOCHECK ADD FOREIGN KEY([OrdenID])
REFERENCES [dbo].[Ordenes] ([OrdenID])
GO

ALTER TABLE [dbo].[DetalleOrdenes]  WITH NOCHECK ADD FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Productos] ([ProductoID])
GO
/****** Object:  Table [dbo].[DetallesOrden]    Script Date: 12/17/2025 12:47:18 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DetallesOrden](
	[DetalleOrdenID] [int] IDENTITY(1,1) NOT NULL,
	[OrdenID] [int] NOT NULL,
	[ProductoID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitarioEnVenta] [decimal](10, 2) NOT NULL,
	[SubTotal] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleOrdenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DetallesOrden]  WITH CHECK ADD FOREIGN KEY([OrdenID])
REFERENCES [dbo].[Ordenes] ([OrdenID])
GO

ALTER TABLE [dbo].[DetallesOrden]  WITH CHECK ADD FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Productos] ([ProductoID])
GO

ALTER TABLE [dbo].[DetallesOrden]  WITH CHECK ADD CHECK  (([Cantidad]>(0)))
GO
/****** Object:  Table [dbo].[Ordenes]    Script Date: 12/17/2025 12:50:03 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Ordenes](
	[OrdenID] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [int] NULL,
	[FechaOrden] [datetime] NOT NULL,
	[Total] [decimal](10, 2) NOT NULL,
	[Estado] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrdenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Ordenes] ADD  DEFAULT (getdate()) FOR [FechaOrden]
GO

ALTER TABLE [dbo].[Ordenes] ADD  DEFAULT ('Completado') FOR [Estado]
GO
/****** Object:  Table [dbo].[Productos]    Script Date: 12/17/2025 12:54:03 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Productos](
	[ProductoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Descripcion] [varchar](max) NULL,
	[PrecioUnitario] [decimal](10, 2) NOT NULL,
	[Stock] [int] NOT NULL,
	[FechaRegistro] [datetime] NULL,
	[ImagenURL] [varchar](255) NULL,
	[VentasAcumuladas] [int] NULL,
	[Activo] [bit] NULL,
	[ProveedorID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Productos] ADD  DEFAULT (getdate()) FOR [FechaRegistro]
GO

ALTER TABLE [dbo].[Productos] ADD  DEFAULT ((0)) FOR [VentasAcumuladas]
GO

ALTER TABLE [dbo].[Productos] ADD  DEFAULT ((1)) FOR [Activo]
GO

/****** Object:  Table [dbo].[ProductosProveedores]    Script Date: 12/17/2025 1:05:13 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductosProveedores](
	[ProductoProveedorID] [int] IDENTITY(1,1) NOT NULL,
	[ProductoID] [int] NOT NULL,
	[ProveedorID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductoProveedorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ProductoID] ASC,
	[ProveedorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductosProveedores]  WITH NOCHECK ADD FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Productos] ([ProductoID])
GO

ALTER TABLE [dbo].[ProductosProveedores]  WITH NOCHECK ADD FOREIGN KEY([ProveedorID])
REFERENCES [dbo].[Proveedores] ([ProveedorID])
GO

/****** Object:  Table [dbo].[Proveedores]    Script Date: 12/17/2025 1:11:42 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Proveedores](
	[ProveedorID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[ContactoNombre] [varchar](100) NULL,
	[Telefono] [varchar](20) NULL,
	[Email] [varchar](100) NULL,
	[Activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProveedorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Proveedores] ADD  DEFAULT ((1)) FOR [Activo]
GO


/****** Object:  Table [dbo].[Usuarios]    Script Date: 12/17/2025 1:19:16 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Usuarios](
	[UsuarioID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[PasswordHash] [varchar](255) NOT NULL,
	[NombreCompleto] [varchar](150) NULL,
	[FechaRegistro] [datetime] NULL,
	[Activo] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UsuarioID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT (getdate()) FOR [FechaRegistro]
GO

ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((1)) FOR [Activo]
GO
