using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clothing_shop_v2.Controllers
{
    public class SizeController : Controller
    {
        private readonly ILogger<SizeController> _logger;
        private readonly ClothingShopDbContext _context;

        public SizeController(ILogger<SizeController> logger, ClothingShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Sizes.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.SizeName.Contains(searchString));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            pageNumber = Math.Max(1, pageNumber);
            pageNumber = Math.Min(pageNumber, totalPages > 0 ? totalPages : 1);

            var sizes = await query
                .OrderBy(s => s.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new SizeListViewModel
            {
                Sizes = sizes,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems,
                SearchString = searchString
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SizeCreateVModel size)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newSize = SizeMapping.VModelToEntity(size);
                    _context.Sizes.Add(newSize);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Kích thước '{size.SizeName}' đã được tạo thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo kích thước");
                    TempData["ErrorMessage"] = "Không thể tạo kích thước. Vui lòng thử lại.";
                }
            }
            return View(size);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var size = await _context.Sizes.FindAsync(id);
            if (size == null)
            {
                TempData["ErrorMessage"] = "Kích thước không tồn tại.";
                return RedirectToAction("Index");
            }
            return View(size);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, SizeUpdateVModel sizeVModel)
        {
            if (id != sizeVModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSize = await _context.Sizes.FindAsync(id);
                    if (existingSize == null)
                    {
                        TempData["ErrorMessage"] = "Kích thước không tồn tại.";
                        return RedirectToAction("Index");
                    }

                    var updatedSize = SizeMapping.VModelToEntity(sizeVModel, existingSize);
                    _context.Update(updatedSize);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Kích thước '{updatedSize.SizeName}' đã được cập nhật thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật kích thước với Id {Id}", id);
                    TempData["ErrorMessage"] = "Không thể cập nhật kích thước. Vui lòng thử lại.";
                }
            }
            return View(sizeVModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var size = await _context.Sizes.FindAsync(id);
            if (size == null)
            {
                TempData["ErrorMessage"] = "Kích thước không tồn tại.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.Sizes.Remove(size);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Kích thước '{size.SizeName}' đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa kích thước với Id {Id}", id);
                TempData["ErrorMessage"] = "Không thể xóa kích thước. Vui lòng thử lại.";
            }

            return RedirectToAction("Index");
        }
    }
}