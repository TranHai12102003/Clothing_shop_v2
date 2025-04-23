using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? VariantId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Variant? Variant { get; set; }
}
