using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Variant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int SizeId { get; set; }

    public int ColorId { get; set; }

    public decimal Price { get; set; }

    public decimal? SalePrice { get; set; }

    public int QuantityInStock { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Color Color { get; set; } = null!;

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual Size Size { get; set; } = null!;

    public virtual ICollection<VariantPromotion> VariantPromotions { get; set; } = new List<VariantPromotion>();
}
