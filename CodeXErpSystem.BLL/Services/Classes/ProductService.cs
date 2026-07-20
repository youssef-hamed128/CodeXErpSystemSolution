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
            var entities = _unitOfWork.GetRepository<Product>().FindAsync(includeProperties: "Category").Result;
            return _mapper.Map<IEnumerable<ProductViewModel>>(entities);
        }

        public async Task CreateAsync(CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model)
        {
            var entity = _mapper.Map<Product>(model);
            _unitOfWork.GetRepository<Product>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model)
        {
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


