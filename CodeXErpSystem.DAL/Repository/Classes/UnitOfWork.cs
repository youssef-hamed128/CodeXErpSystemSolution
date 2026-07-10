using CodeXErpSystem.DAL.Contexts;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Classes;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Repository.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CodeXDbContext dbContext;

        // 1. قاموس المستودعات (Collection Expression C# 12)
        private readonly Dictionary<string, object> _Repos = [];

        // 2. حقل خاص بالمعاملة المالية والمخزنية
        private IDbContextTransaction? _transaction;

        public IInvoiceRepository InvoiceRepository { get; }

        public IProductRepository ProductRepository { get; }

        public UnitOfWork(CodeXDbContext dbContext)
        {
            this.dbContext = dbContext;
            InvoiceRepository = new InvoiceRepository(dbContext);
            ProductRepository = new ProductRepository(dbContext);
        }

        // إنشاء المستودعات عند الطلب فقط (Dynamic & Lazy Initialization)
        public IGenaricRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if (_Repos.TryGetValue(typeName, out object OldRepo))
                return (IGenaricRepository<TEntity>)OldRepo;

            // يمكنك هنا دمج التقييم الذكي للمستودعات الخاصة لاحقاً إذا أردت
            var NewRepo = new GenaricRepository<TEntity>(dbContext);
            _Repos[typeName] = NewRepo;
            return NewRepo;
        }

        // حفظ التغييرات دفعة واحدة
        public async Task<int> CompleteAsync(CancellationToken ct = default)
        {
            return await dbContext.SaveChangesAsync(ct);
        }

        // بدء معاملة مالية/مخزنية مركبة
        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            // تم التصحيح لاستخدام _transaction
            _transaction = await dbContext.Database.BeginTransactionAsync(ct);
        }

        // اعتماد التغييرات كلها
        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // إلغاء المعاملة بالكامل في حال حدوث أي خطأ مالي/مخزني
        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // تنظيف الموارد من الذاكرة
        public void Dispose()
        {
            dbContext.Dispose(); // تم التصحيح لاستخدام dbContext
            _transaction?.Dispose(); // تم التصحيح لاستخدام _transaction
        }
    }
}