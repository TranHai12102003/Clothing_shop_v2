using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int VariantId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
