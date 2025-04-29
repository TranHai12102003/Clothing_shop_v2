using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }
        [HttpGet]
        public async Task<ActionResult<PaginationModel<PromotionGetVmodel>>> Index([FromQuery] PromotionFilterParams parameters)
        {
            var response = await _promotionService.GetAll(parameters);
            return View(response.Value);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PromotionCreateVmodel vmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(vmodel);
            }
            var response = await _promotionService.Create(vmodel);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Message);
            return View(vmodel);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _promotionService.Delete(id);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Message);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var promotion = await _promotionService.GetById(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotion.Value);
        }
        [HttpPost]
        public async Task<IActionResult> Update(PromotionUpdateVmodel vmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(vmodel);
            }
            var response = await _promotionService.Update(vmodel);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Message);
            return View(vmodel);
        }
        #region
        //[HttpPost]
        //public async Task<IActionResult> ToggleActive(int id, bool isActive = false) // Mặc định là false nếu không có giá trị
        //{
        //    var promotion = await _context.Promotions.FindAsync(id);
        //    if (promotion == null)
        //    {
        //        TempData["ErrorMessage"] = "Không tìm thấy khuyến mãi.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    promotion.IsActive = isActive;
        //    promotion.UpdatedDate = DateTime.Now; // Cập nhật thời gian sửa đổi
        //    _context.Promotions.Update(promotion);
        //    await _context.SaveChangesAsync();

        //    TempData["SuccessMessage"] = "Cập nhật trạng thái khuyến mãi thành công.";
        //    return RedirectToAction(nameof(Index));
        //}
        #endregion
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id, bool isActive = false)
        {
            var response = await _promotionService.ToggleActive(id, isActive);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
            }
            else
            {
                TempData["ErrorMessage"] = response.Message; // Lưu thông báo lỗi
            }
            return RedirectToAction("Index");
        }

    }
}
