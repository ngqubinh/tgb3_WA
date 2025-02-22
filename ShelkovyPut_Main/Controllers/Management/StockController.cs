using Application.DTOs.Request.Management;
using Application.Interfaces.Management;
using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers.Management
{
    public class StockController : Controller
    {
        private readonly IStockService _stock;

        public StockController(IStockService stock)
        {
            _stock = stock;
        }

        public async Task<IActionResult> Stock(string sTerm = "")
        {
            var stocks = await _stock.GetStocks(sTerm);
            return View(stocks);
        }

        public async Task<IActionResult> ManangeStock(int productId)
        {
            var existingStock = await _stock.GetStockProductId(productId);
            var stock = new StockRequest
            {
                ProductId = productId,
                Quantity = existingStock != null
            ? existingStock.Quantity : 0
            };
            return View(stock);
        }

        [HttpPost]
        public async Task<ActionResult> ManangeStock(StockRequest stock)
        {
            if (!ModelState.IsValid)
                return View(stock);
            try
            {
                await _stock.ManageStock(stock);
                TempData["successMessage"] = "Stock is updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Something went wrong!!";
                throw new Exception(ex.Message.ToString());
            }

            return RedirectToAction(nameof(Stock));
        }
    }
}
