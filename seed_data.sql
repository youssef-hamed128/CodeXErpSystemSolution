USE CodeXERP;
GO

-- Seed Categories
IF NOT EXISTS(SELECT 1 FROM ProductCategory WHERE Name = N'أجهزة منزلية')
BEGIN
    INSERT INTO ProductCategory (Name, Description, CreatedAt, IsDeleted) VALUES (N'أجهزة منزلية', N'ثلاجات، غسالات، بوتاجازات', GETDATE(), 0);
    INSERT INTO ProductCategory (Name, Description, CreatedAt, IsDeleted) VALUES (N'إلكترونيات', N'شاشات، لابتوبات، هواتف', GETDATE(), 0);
    INSERT INTO ProductCategory (Name, Description, CreatedAt, IsDeleted) VALUES (N'أدوات مكتبية', N'أقلام، دفاتر، طابعات', GETDATE(), 0);
END

DECLARE @Cat1 INT = (SELECT Id FROM ProductCategory WHERE Name = N'أجهزة منزلية');
DECLARE @Cat2 INT = (SELECT Id FROM ProductCategory WHERE Name = N'إلكترونيات');
DECLARE @Cat3 INT = (SELECT Id FROM ProductCategory WHERE Name = N'أدوات مكتبية');

-- Seed Products
IF NOT EXISTS(SELECT 1 FROM Products WHERE Code = 'REF-TOS-14')
BEGIN
    INSERT INTO Products (Name, Code, Barcode, CategoryId, PurchasePrice, SalePrice, UnitOfMeasure, CreatedAt, IsDeleted, HasStockTracking, MinStockLevel)
    VALUES (N'ثلاجة توشيبا 14 قدم', 'REF-TOS-14', '1234567890123', @Cat1, 15000, 17500, N'قطعة', GETDATE(), 0, 1, 5);
    
    INSERT INTO Products (Name, Code, Barcode, CategoryId, PurchasePrice, SalePrice, UnitOfMeasure, CreatedAt, IsDeleted, HasStockTracking, MinStockLevel)
    VALUES (N'غسالة سامسونج 8 كيلو', 'WAS-SAM-08', '1234567890124', @Cat1, 12000, 14000, N'قطعة', GETDATE(), 0, 1, 5);

    INSERT INTO Products (Name, Code, Barcode, CategoryId, PurchasePrice, SalePrice, UnitOfMeasure, CreatedAt, IsDeleted, HasStockTracking, MinStockLevel)
    VALUES (N'شاشة إل جي 55 بوصة سمارت', 'TV-LG-55', '1234567890125', @Cat2, 18000, 21000, N'قطعة', GETDATE(), 0, 1, 5);

    INSERT INTO Products (Name, Code, Barcode, CategoryId, PurchasePrice, SalePrice, UnitOfMeasure, CreatedAt, IsDeleted, HasStockTracking, MinStockLevel)
    VALUES (N'لابتوب ديل كور i7', 'LAP-DELL-I7', '1234567890126', @Cat2, 25000, 28500, N'قطعة', GETDATE(), 0, 1, 2);
    
    INSERT INTO Products (Name, Code, Barcode, CategoryId, PurchasePrice, SalePrice, UnitOfMeasure, CreatedAt, IsDeleted, HasStockTracking, MinStockLevel)
    VALUES (N'طابعة إتش بي ليزر', 'PRN-HP-LSR', '1234567890127', @Cat3, 3000, 3800, N'قطعة', GETDATE(), 0, 1, 10);
    
    INSERT INTO Products (Name, Code, Barcode, CategoryId, PurchasePrice, SalePrice, UnitOfMeasure, CreatedAt, IsDeleted, HasStockTracking, MinStockLevel)
    VALUES (N'ورق تصوير A4 دوبلكس', 'PPR-A4-DBX', '1234567890128', @Cat3, 150, 180, N'كرتونة', GETDATE(), 0, 1, 20);
END

-- Seed Customers
IF NOT EXISTS(SELECT 1 FROM Customers WHERE Name = N'مؤسسة النور للتجارة')
BEGIN
    INSERT INTO Customers (Name, Phone, Email, Address, TaxNumber, CreditLimit, Balance, CreatedAt, IsDeleted)
    VALUES (N'مؤسسة النور للتجارة', '01012345678', 'info@alnoor.com', N'القاهرة - مدينة نصر', '300-400-500', 50000, 0, GETDATE(), 0);

    INSERT INTO Customers (Name, Phone, Email, Address, TaxNumber, CreditLimit, Balance, CreatedAt, IsDeleted)
    VALUES (N'شركة الحمد للمقاولات', '01123456789', 'contact@alhamd.com', N'الجيزة - الدقي', '123-456-789', 100000, 15000, GETDATE(), 0);

    INSERT INTO Customers (Name, Phone, Email, Address, TaxNumber, CreditLimit, Balance, CreatedAt, IsDeleted)
    VALUES (N'صيدلية الشفاء', '01234567890', 'pharma@shifa.com', N'الإسكندرية - سموحة', '987-654-321', 20000, 5000, GETDATE(), 0);

    INSERT INTO Customers (Name, Phone, Email, Address, TaxNumber, CreditLimit, Balance, CreatedAt, IsDeleted)
    VALUES (N'أحمد محمد مصطفى (فردي)', '01555555555', '', N'طنطا - شارع البحر', '', 0, 0, GETDATE(), 0);
END

-- Seed Suppliers
IF NOT EXISTS(SELECT 1 FROM Suppliers WHERE Name = N'مصنع توشيبا العربي')
BEGIN
    INSERT INTO Suppliers (Name, Phone, Email, Address, TaxNumber, Balance, CreatedAt, IsDeleted)
    VALUES (N'مصنع توشيبا العربي', '19319', 'sales@elarabygroup.com', N'المنطقة الصناعية بقويسنا', '111-222-333', 0, GETDATE(), 0);

    INSERT INTO Suppliers (Name, Phone, Email, Address, TaxNumber, Balance, CreatedAt, IsDeleted)
    VALUES (N'شركة سامسونج مصر', '16580', 'info@samsung.eg', N'القاهرة الجديدة', '444-555-666', 25000, GETDATE(), 0);

    INSERT INTO Suppliers (Name, Phone, Email, Address, TaxNumber, Balance, CreatedAt, IsDeleted)
    VALUES (N'وكالة الجريسي للأدوات المكتبية', '0223456789', 'info@jeraisy.com', N'وسط البلد - القاهرة', '777-888-999', 0, GETDATE(), 0);
END
