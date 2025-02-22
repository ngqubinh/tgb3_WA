using Application.Interfaces.Setting;
using Application.ViewModels.Setting;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Infrastructure.Repositories.Setting
{
    public class ExcelFileImportService : IExcelFileImportService
    {
        private readonly ShelkobyPutDbContext _context;
        public ExcelFileImportService(ShelkobyPutDbContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<List<BrandExelImporter>> ImportBrands(Stream fileStream)
        {
            List<BrandExelImporter> brands = new List<BrandExelImporter>();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                {
                    throw new NullReferenceException("Worksheet not found.");
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0;
                if (rowCount == 0)
                {
                    throw new NullReferenceException("Worksheet is empty or not properly read.");
                }

                for (int row = 2; row <= rowCount; row++) // Assuming the first row contains headers
                {
                    string brandName = worksheet.Cells[row, 2]?.Text; // Read the first column
                    if (!string.IsNullOrWhiteSpace(brandName))
                    {
                        var brand = new BrandExelImporter { BrandName = brandName };
                        brands.Add(brand);
                        var brandEnity = new Brands()
                        {
                            BrandName = brandName,
                        };

                        _context.Brands.Add(brandEnity);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return brands;
        }

        public async Task<List<CategoryForExcelImportVM>> ImportCategories(Stream fileStream)
        {
            List<CategoryForExcelImportVM> categories = new List<CategoryForExcelImportVM>();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                {
                    throw new NullReferenceException("Worksheet not found.");
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0;
                if (rowCount == 0)
                {
                    throw new NullReferenceException("Worksheet is empty or not properly read.");
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    string categoryName = worksheet.Cells[row, 2]?.Text;
                    string userId = worksheet.Cells[row, 4]?.Text;
                    string createdDateText = worksheet.Cells[row, 3]?.Text;

                    DateTime createdDate = DateTime.TryParse(createdDateText, out DateTime parsedDate) ? parsedDate : DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(categoryName))
                    {
                        var brand = new CategoryForExcelImportVM
                        {
                            CategoryName = categoryName,
                            UserId = userId,
                            CreatedDate = createdDate
                        };
                        categories.Add(brand);
                        var categoryEnity = new Category()
                        {
                            CategoryName = categoryName,
                            UserId = userId,
                            CreatedDate = createdDate,
                        };

                        _context.Categories.Add(categoryEnity);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return categories;
        }

        public async Task<List<ProductExcelImporter>> ImportProducts(Stream fileStream)
        {
            List<ProductExcelImporter> products = new List<ProductExcelImporter>();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                {
                    throw new NullReferenceException("Worksheet not found.");
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0;
                if (rowCount == 0)
                {
                    throw new NullReferenceException("Worksheet is empty or not properly read.");
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    string productName = worksheet.Cells[row, 2]?.Text;                 
                    string description = worksheet.Cells[row, 3]?.Text;
                    string sizeId = worksheet.Cells[row, 4].Text;
                    string pictures = worksheet.Cells[row, 5].Text;
                    string productPrice = worksheet.Cells[row, 6].Text;
                    string isFeatured = worksheet.Cells[row, 7].Text;
                    string discountedProductPrice = worksheet.Cells[row, 8].Text;
                    string brandId = worksheet.Cells[row, 9].Text;
                    string createdDateText = worksheet.Cells[row, 10].Text;
                    string userId = worksheet.Cells[row, 11]?.Text;
                    string categoryId = worksheet.Cells[row, 12].Text;

                    DateTime createdDate = DateTime.TryParse(createdDateText, out DateTime parsedDate) ? parsedDate : DateTime.Now;
                    double parserProductPrice = double.TryParse(productPrice, out double parsedProductPrice) ? parsedProductPrice : 0;
                    bool parserIsFeatured = bool.TryParse(isFeatured, out bool parsedIsFeatured);
                    double parserDiscountedProductPrice = double.TryParse(discountedProductPrice, out double paserd) ? paserd : 0;
                    int parserBrandId = int.TryParse(brandId, out int parsedBrandId) ? parsedBrandId : 0;
                    int parserCategortId = int.TryParse(categoryId, out int parsedCategoryId) ? parsedCategoryId : 0;
                    int parserSizeId = int.TryParse(categoryId, out int parsedSizeId) ? parsedSizeId : 0;

                    if (!string.IsNullOrWhiteSpace(productName))
                    {
                        var existedProduct = _context.Products.FirstOrDefault(p => p.ProductName == productName);
                        if(existedProduct == null)
                        {
                            var product = new ProductExcelImporter
                            {
                                ProductName = productName,
                                Description = description,
                                SizeId = parserSizeId,
                                Pictures = pictures,
                                ProductPrice = parserProductPrice,
                                IsFeatured = parserIsFeatured,
                                DiscountProductprice = parserDiscountedProductPrice,
                                BrandId = parserBrandId,
                                CreatedDate = createdDate,
                                UserId = userId,
                                CategoryId = parserCategortId
                            };
                            products.Add(product);

                            var productEntity = new Product()
                            {
                                ProductName = productName,
                                Description = description,
                                SizeId = parsedSizeId,
                                Pictures = pictures,
                                ProductPrice = parserProductPrice,
                                IsFeatured = parserIsFeatured,
                                DiscountProductprice = parserDiscountedProductPrice,
                                BrandId = parserBrandId,
                                CreatedDate = createdDate,
                                UserId = userId,
                                CategoryId = parserCategortId
                            };
                            _context.Products.Add(productEntity);
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }

            return products;
        }
    }
}
