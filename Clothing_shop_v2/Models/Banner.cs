using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Banner
{
    public int Id { get; set; }

    public string BannerName { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string LinkUrl { get; set; } = null!;

    public int? ProductId { get; set; }

    public int? CategoryId { get; set; }

    public int? PromotionId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Promotion? Promotion { get; set; }
}
