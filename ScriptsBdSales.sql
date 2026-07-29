CREATE DATABASE VentaRapida;
GO
USE VentaRapida;
GO

CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Email NVARCHAR(300) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Description NVARCHAR(1000) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Sales (
    SaleId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Sales_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
GO

CREATE TABLE SaleItems (
    SaleItemId INT IDENTITY(1,1) PRIMARY KEY,
    SaleId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_SaleItems_Sales FOREIGN KEY (SaleId) REFERENCES Sales(SaleId),
    CONSTRAINT FK_SaleItems_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO

-- Datos de prueba
INSERT INTO Customers (Name, Email, IsActive) VALUES
('Carlos Perez', 'carlos@email.com', 1),
('Maria Lopez', 'maria@email.com', 1),
('Juan Inactivo', 'juan@email.com', 0);
GO

INSERT INTO Products (Name, Price, IsActive) VALUES
('Camiseta Azul', 25000.00, 1),
('Pantalon Negro', 45000.00, 1),
('Zapatos Rojos', 80000.00, 1),
('Producto Viejo', 10000.00, 0);
GO

CREATE PROCEDURE sp_CreateSale
    @CustomerId INT,
    @ItemsXml NVARCHAR(MAX),
    @SaleId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1 FROM Customers
            WHERE CustomerId = @CustomerId AND IsActive = 1
        )
        BEGIN
            RAISERROR('Cliente inactivo o no existe.', 16, 1);
            RETURN;
        END

        DECLARE @Items TABLE (
            ProductId INT,
            Quantity INT
        );

        INSERT INTO @Items (ProductId, Quantity)
        SELECT
            item.value('@productId', 'INT'),
            item.value('@quantity', 'INT')
        FROM (SELECT CAST(@ItemsXml AS XML)) AS x(XmlData)
        CROSS APPLY XmlData.nodes('/items/item') AS t(item);

        IF EXISTS (
            SELECT 1 FROM @Items i
            LEFT JOIN Products p ON p.ProductId = i.ProductId AND p.IsActive = 1
            WHERE p.ProductId IS NULL
        )
        BEGIN
            RAISERROR('Uno o mas productos estan inactivos o no existen.', 16, 1);
            RETURN;
        END

        DECLARE @Total DECIMAL(10,2);
        SELECT @Total = SUM(p.Price * i.Quantity)
        FROM @Items i
        INNER JOIN Products p ON p.ProductId = i.ProductId;

        INSERT INTO Sales (CustomerId, TotalAmount, CreatedAt)
        VALUES (@CustomerId, @Total, GETDATE());

        SET @SaleId = SCOPE_IDENTITY();

        INSERT INTO SaleItems (SaleId, ProductId, Quantity, UnitPrice, Subtotal)
        SELECT
            @SaleId,
            i.ProductId,
            i.Quantity,
            p.Price,
            p.Price * i.Quantity
        FROM @Items i
        INNER JOIN Products p ON p.ProductId = i.ProductId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO