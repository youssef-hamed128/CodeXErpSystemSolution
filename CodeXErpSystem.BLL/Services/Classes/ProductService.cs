using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Products;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<ProductViewModel> GetAllProducts()
        {
            var entities = _unitOfWork.GetRepository<Product>().FindAsync(includeProperties: "Category,StockQuantities,StockQuantities.Warehouse").Result;
            var viewModels = _mapper.Map<IEnumerable<ProductViewModel>>(entities).ToList();
            
            // Map the warehouse names manually
            foreach (var vm in viewModels)
            {
                var entity = entities.FirstOrDefault(e => e.Id == vm.Id);
                if (entity != null && entity.StockQuantities.Any())
                {
                    vm.WarehouseName = string.Join(", ", entity.StockQuantities.Select(sq => sq.Warehouse?.Name).Where(n => !string.IsNullOrEmpty(n)).Distinct());
                    vm.AvailableQuantity = entity.StockQuantities.Sum(sq => sq.Quantity);
                    if (vm.AvailableQuantity == 0) vm.Status = "نفذت الكمية";
                    else if (vm.AvailableQuantity <= vm.MinQuantity) vm.Status = "منخفض";
                    else vm.Status = "متاح";
                }
                else
                {
                    vm.WarehouseName = "غير محدد";
                    vm.AvailableQuantity = 0;
                    vm.Status = "نفذت الكمية";
                }
            }
            return viewModels;
        }

        public async Task CreateAsync(CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model)
        {
            var existingCode = await _unitOfWork.GetRepository<Product>().FindAsync(p => p.Code == model.Code);
            if (existingCode.Any())
                throw new System.InvalidOperationException("رقم الكود مكرر. يرجى إدخال كود مختلف.");

            var existingName = await _unitOfWork.GetRepository<Product>().FindAsync(p => p.Name == model.Name);
            if (existingName.Any())
                throw new System.InvalidOperationException("اسم المنتج مكرر. يرجى إدخال اسم مختلف.");

            var entity = _mapper.Map<Product>(model);
            _unitOfWork.GetRepository<Product>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model)
        {
            var existingCode = await _unitOfWork.GetRepository<Product>().FindAsync(p => p.Code == model.Code && p.Id != model.Id);
            if (existingCode.Any())
                throw new System.InvalidOperationException("رقم الكود مكرر لمنتج آخر. يرجى إدخال كود مختلف.");

            var existingName = await _unitOfWork.GetRepository<Product>().FindAsync(p => p.Name == model.Name && p.Id != model.Id);
            if (existingName.Any())
                throw new System.InvalidOperationException("اسم المنتج مكرر لمنتج آخر. يرجى إدخال اسم مختلف.");

            var entity = _mapper.Map<Product>(model);
            _unitOfWork.GetRepository<Product>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _unitOfWork.GetRepository<Product>().Delete(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}


