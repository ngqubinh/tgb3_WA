using Application.ViewModels.Setting;
using Domain.Models.Management;

namespace Application.Interfaces.Setting
{
    public interface IExcelFileImportService
    {
        Task<List<CategoryForExcelImportVM>> ImportCategories(Stream fileStream);
        Task<List<BrandExelImporter>> ImportBrands(Stream fileStream);
        Task<List<ProductExcelImporter>> ImportProducts(Stream fileStream);
    }
}
