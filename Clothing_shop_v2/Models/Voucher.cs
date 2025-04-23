using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Voucher
{
    public int Id { get; set; }

    public string VoucherCode { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? MaxUsage { get; set; }

    public int UsedCount { get; set; }

    public bool IsActive { get; set; }

    public int? CustomerTypeId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual CustomerType? CustomerType { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
