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

-- Tabela de Categorias
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categorias')
BEGIN
    CREATE TABLE Categorias (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nome NVARCHAR(50) NOT NULL UNIQUE,
        Descricao NVARCHAR(200)
    );
END
GO

-- Tabela de Produtos
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

-- Tabela de Pedidos
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

-- Tabela de Itens do Pedido
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

-- Reset dos Identity seeds
DBCC CHECKIDENT ('PedidoItens', RESEED, 0);
DBCC CHECKIDENT ('Pedidos', RESEED, 0);
DBCC CHECKIDENT ('Produtos', RESEED, 0);
DBCC CHECKIDENT ('Categorias', RESEED, 0);
DBCC CHECKIDENT ('Clientes', RESEED, 0);
GO

-- Inserir Categorias
INSERT INTO Categorias (Nome, Descricao) VALUES
('Eletrônicos', 'Produtos eletrônicos e gadgets'),
('Livros', 'Livros físicos e digitais'),
('Roupas', 'Vestuário em geral'),
('Alimentos', 'Produtos alimentícios'),
('Esportes', 'Artigos esportivos');
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
