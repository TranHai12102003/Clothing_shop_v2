using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        public async Task<ActionResult<List<RoleGetVModel>>> Index()
        {
            var roles = await _roleService.GetAll();
            return View(roles.Value);
        }
        public async Task<ActionResult<RoleGetVModel>> Details(int id)
        {
            var role = await _roleService.GetById(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role.Value);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleCreateVModel vmodel)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleService.Create(vmodel);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = result.Message; // Lưu thông báo thành công
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(vmodel);
        }
        public async Task<IActionResult> Update(int id)
        {
            var role = await _roleService.GetById(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role.Value);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RoleUpdateVModel vmodel)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleService.Update(vmodel);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = result.Message; // Lưu thông báo thành công
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(vmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleService.Delete(id);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message);
            return RedirectToAction("Index");
        }
    }
}
