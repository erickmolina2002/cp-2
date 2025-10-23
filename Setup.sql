IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LojaDB')
BEGIN
    CREATE DATABASE LojaDB;
END
GO

USE LojaDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
BEGIN
    CREATE TABLE Clientes (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nome NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Telefone NVARCHAR(20),
        DataCadastro DATETIME DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categorias')
BEGIN
    CREATE TABLE Categorias (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nome NVARCHAR(50) NOT NULL UNIQUE,
        Descricao NVARCHAR(200)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Produtos')
BEGIN
    CREATE TABLE Produtos (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nome NVARCHAR(100) NOT NULL,
        Preco DECIMAL(10,2) NOT NULL CHECK (Preco >= 0),
        Estoque INT NOT NULL DEFAULT 0 CHECK (Estoque >= 0),
        CategoriaId INT NOT NULL,
        DataCadastro DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Pedidos')
BEGIN
    CREATE TABLE Pedidos (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ClienteId INT NOT NULL,
        DataPedido DATETIME NOT NULL DEFAULT GETDATE(),
        ValorTotal DECIMAL(10,2) NOT NULL CHECK (ValorTotal >= 0),
        Status NVARCHAR(20) DEFAULT 'Pendente',
        FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PedidoItens')
BEGIN
    CREATE TABLE PedidoItens (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PedidoId INT NOT NULL,
        ProdutoId INT NOT NULL,
        Quantidade INT NOT NULL CHECK (Quantidade > 0),
        PrecoUnitario DECIMAL(10,2) NOT NULL CHECK (PrecoUnitario >= 0),
        FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id) ON DELETE CASCADE,
        FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
    );
END
GO

DELETE FROM PedidoItens;
DELETE FROM Pedidos;
DELETE FROM Produtos;
DELETE FROM Categorias;
DELETE FROM Clientes;
GO

DBCC CHECKIDENT ('PedidoItens', RESEED, 0);
DBCC CHECKIDENT ('Pedidos', RESEED, 0);
DBCC CHECKIDENT ('Produtos', RESEED, 0);
DBCC CHECKIDENT ('Categorias', RESEED, 0);
DBCC CHECKIDENT ('Clientes', RESEED, 0);
GO

INSERT INTO Categorias (Nome, Descricao) VALUES
('Eletrônicos', 'Produtos eletrônicos e gadgets'),
('Livros', 'Livros físicos e digitais'),
('Roupas', 'Vestuário em geral'),
('Alimentos', 'Produtos alimentícios'),
('Esportes', 'Artigos esportivos');
GO

INSERT INTO Clientes (Nome, Email, Telefone) VALUES
('Carlos Mendes', 'carlos.mendes@empresa.com', '11955554444'),
('Ana Paula Costa', 'ana.costa@tech.com', '21988887777'),
('Roberto Almeida', 'roberto.almeida@negocio.com', '11966665555');
GO

INSERT INTO Produtos (Nome, Preco, Estoque, CategoriaId) VALUES
('Smartphone Samsung Galaxy S24', 4200.00, 15, 1),
('Fone Bluetooth JBL Tune 510', 280.00, 30, 1),
('Smartwatch Xiaomi Band 8', 199.90, 20, 1),
('Arquitetura Limpa - Robert Martin', 92.00, 25, 2),
('Refatoração - Martin Fowler', 135.00, 18, 2),
('Jaqueta Jeans Masculina', 159.90, 40, 3),
('Tênis Adidas Ultraboost', 599.00, 22, 3),
('Café Pilão Torrado 500g', 18.50, 80, 4),
('Creatina Universal 300g', 89.00, 35, 5),
('Luva de Boxe Everlast 14oz', 179.90, 18, 5);
GO

INSERT INTO Pedidos (ClienteId, DataPedido, ValorTotal, Status) VALUES
(1, GETDATE(), 4480.00, 'Pendente'),
(2, GETDATE(), 227.00, 'Processando'),
(3, GETDATE(), 448.80, 'Entregue');
GO

INSERT INTO PedidoItens (PedidoId, ProdutoId, Quantidade, PrecoUnitario) VALUES
(1, 1, 1, 4200.00),
(1, 2, 1, 280.00),
(2, 4, 1, 92.00),
(2, 5, 1, 135.00),
(3, 6, 2, 159.90),
(3, 9, 1, 89.00),
(3, 10, 1, 179.90);
GO

IF OBJECT_ID('vw_ProdutosCompleto', 'V') IS NOT NULL
    DROP VIEW vw_ProdutosCompleto;
GO

CREATE VIEW vw_ProdutosCompleto AS
SELECT
    p.Id,
    p.Nome,
    p.Preco,
    p.Estoque,
    c.Nome AS Categoria,
    p.DataCadastro
FROM Produtos p
INNER JOIN Categorias c ON p.CategoriaId = c.Id;
GO

IF OBJECT_ID('vw_PedidosCompleto', 'V') IS NOT NULL
    DROP VIEW vw_PedidosCompleto;
GO

CREATE VIEW vw_PedidosCompleto AS
SELECT
    p.Id AS PedidoId,
    p.DataPedido,
    p.ValorTotal,
    p.Status,
    c.Id AS ClienteId,
    c.Nome AS ClienteNome,
    c.Email AS ClienteEmail
FROM Pedidos p
INNER JOIN Clientes c ON p.ClienteId = c.Id;
GO
