USE CodeXERP;
GO

-- Seed Customers
IF NOT EXISTS(SELECT 1 FROM Customers WHERE Name = N'مؤسسة النور للتجارة')
BEGIN
    INSERT INTO Customers (Name, Phone, Email, Address, TaxNumber, CreditLimit, Balance, OpeningBalance, CreatedAt, IsDeleted)
    VALUES (N'مؤسسة النور للتجارة', '01012345678', 'info@alnoor.com', N'القاهرة - مدينة نصر', '300-400-500', 50000, 0, 0, GETDATE(), 0);

    INSERT INTO Customers (Name, Phone, Email, Address, TaxNumber, CreditLimit, Balance, OpeningBalance, CreatedAt, IsDeleted)
    VALUES (N'شركة الحمد للمقاولات', '01123456789', 'contact@alhamd.com', N'الجيزة - الدقي', '123-456-789', 100000, 15000, 0, GETDATE(), 0);

    INSERT INTO Customers (Name, Phone, Email, Address, TaxNumber, CreditLimit, Balance, OpeningBalance, CreatedAt, IsDeleted)
    VALUES (N'صيدلية الشفاء', '01234567890', 'pharma@shifa.com', N'الإسكندرية - سموحة', '987-654-321', 20000, 5000, 0, GETDATE(), 0);

    INSERT INTO Customers (Name, Phone, Email, Address, TaxNumber, CreditLimit, Balance, OpeningBalance, CreatedAt, IsDeleted)
    VALUES (N'أحمد محمد مصطفى (فردي)', '01555555555', '', N'طنطا - شارع البحر', '', 0, 0, 0, GETDATE(), 0);
END
