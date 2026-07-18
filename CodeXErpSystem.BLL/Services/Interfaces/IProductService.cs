using System.Collections.Generic;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.BLL.ViewModels.Products;

namespace CodeXErpSystem.BLL.Services.Interfaces
{
    public interface IProductService
    {
        IEnumerable<ProductViewModel> GetAllProducts();
        Task CreateAsync(CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model);
        Task UpdateAsync(CodeXErpSystem.BLL.ViewModels.Products.ProductCreateViewModel model);
        Task DeleteAsync(int id);
    }
}


