using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clothing_shop_v2.Controllers
{
    public class VoucherController : Controller
    {
        private readonly IVoucherService _voucherService;
        private readonly ClothingShopDbContext _context;
        public VoucherController(IVoucherService voucherService, ClothingShopDbContext context)
        {
            _voucherService = voucherService;
            _context = context;
        }
        public async Task<ActionResult<PaginationModel<VoucherGetVModel>>> Index([FromQuery] VoucherFilterParams parameters)
        {
            var response = await _voucherService.GetAll(parameters);
            return View(response.Value);
        }
        public async Task<ActionResult<VoucherGetVModel>> Details(int id)
        {
            var response = await _voucherService.GetById(id);
            if (response.Value == null)
            {
                return NotFound();
            }
            return View(response.Value);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var customerTypes = _context.CustomerTypes.ToList();
            ViewBag.CustomerTypes = new SelectList(customerTypes, "Id", "TypeName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ResponseResult>> Create(VoucherCreateVModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _voucherService.Create(model);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = response.Message;
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = response.Message;
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _voucherService.Delete(id);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Message);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var voucher = await _voucherService.GetById(id);
            if (voucher == null)
            {
                return NotFound();
            }
            var customerTypes = _context.CustomerTypes.ToList();
            ViewBag.CustomerTypes = new SelectList(customerTypes, "Id", "TypeName");
            return View(voucher.Value);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VoucherUpdateVModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _voucherService.Update(model);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = response.Message;
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = response.Message;
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, bool isActive)
        {
            var response = await _voucherService.ChangeStatus(id, isActive);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Message);
            return RedirectToAction("Index");
        }
    }
}
