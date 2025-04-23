using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class VariantPromotion
{
    public int Id { get; set; }

    public int VariantId { get; set; }

    public int PromotionId { get; set; }

    public decimal AppliedDiscount { get; set; }

    public virtual Promotion Promotion { get; set; } = null!;

    public virtual Variant Variant { get; set; } = null!;
}
