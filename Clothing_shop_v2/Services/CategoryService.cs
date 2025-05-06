using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ClothingShopDbContext _context;
        public CategoryService(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult> Create(CategoryCreateVModel model,string image)
        {
            var response = new ResponseResult();
            try
            {
                var category = CategoryMapping.VModelToEntity(model);
                if (image != null)
                {
                    category.ImageUrl = image;
                }
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(category, "Thêm danh mục thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Delete(int id)
        {
            var response = new ResponseResult();
            try
            {
                var entity = _context.Categories.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy danh mục");
                }
                _context.Categories.Remove(entity);
                _context.SaveChanges();
                response = new SuccessResponseResult(entity, "Xóa danh mục thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult($"Không thể xóa danh mục: {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                // Xử lý lỗi khóa ngoại hoặc lỗi database
                return new ErrorResponseResult("Không thể xóa danh mục do có dữ liệu liên quan hoặc lỗi database.");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug
                Console.WriteLine($"Lỗi khi xóa sản phẩm ID {id}: {ex}");
                return new ErrorResponseResult("Đã xảy ra lỗi khi xóa danh mục. Vui lòng thử lại sau.");
            }
        }

        public async Task<ActionResult<PaginationModel<CategoryGetVModel>>> GetAll(CategoryFilterParams parameters)
        {
            IQueryable<Category> query = _context.Categories
                .Include(x => x.ParentCategory)
                .Where(BuildQueryable(parameters));
            var categories = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => CategoryMapping.EntityToVModel(x))
                .ToListAsync();
            var totalItems = await query.CountAsync();
            return new PaginationModel<CategoryGetVModel>
            {
                Records = categories,
                TotalRecords = totalItems,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
                // TotalPages tự tính nên không cần gán!
            };
        }

        public async Task<ActionResult<CategoryGetVModel>> GetById(int id)
        {
                var entity = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return null;
                }
                var category = CategoryMapping.EntityToVModel(entity);
                return category; 
        }

        public async Task<ResponseResult> ToggleActive(int id, bool isActive = false)
        {
            var response = new ResponseResult();
            try
            {
                var entity = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy danh mục");
                }
                entity.IsActive = isActive;
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Thay đổi trạng thánh thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Update(CategoryUpdateVModel model, string image)
        {
            var response = new ResponseResult();
            try
            {
                var entity = _context.Categories.FirstOrDefault(x => x.Id == model.Id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy danh mục");
                }
                var category = CategoryMapping.VModelToEntity(model,entity);
                category.ImageUrl = image;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Cập nhật danh mục thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }
        private Expression<Func<Category, bool>> BuildQueryable(CategoryFilterParams fParams)
        {
            return x =>
                (fParams.SearchString == null || (x.CategoryName != null && x.CategoryName.Contains(fParams.SearchString))) &&
                (fParams.IsActive == null || x.IsActive == fParams.IsActive);
        }
    }
}
