using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Controllers
{
    public class CustomerTypeController : Controller
    {
        private readonly ICustomerTypeService _customerTypeService;
        public CustomerTypeController(ICustomerTypeService customerTypeService)
        {
            _customerTypeService = customerTypeService;
        }
        [HttpGet]
        public async Task<ActionResult<PaginationModel<CustomerTypeGetVModel>>> Index([FromQuery] CustomerTypeFilterParams parameters)
        {
            var response = await _customerTypeService.GetAll(parameters);
            return View(response.Value);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerTypeCreateVModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(vmodel);
            }
            var response = await _customerTypeService.Create(vmodel);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Message);
            return View(vmodel);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _customerTypeService.Delete(id);
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
            var customerType = await _customerTypeService.GetById(id);
            if (customerType == null)
            {
                return NotFound();
            }
            return View(customerType.Value);
        }
        [HttpPost]
        public async Task<IActionResult> Update(CustomerTypeUpdateVModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(vmodel);
            }
            var response = await _customerTypeService.Update(vmodel);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", response.Message);
            return View(vmodel);
        }
    }
}
