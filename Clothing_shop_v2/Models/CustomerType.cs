using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class CustomerType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? MinOrderCount { get; set; }

    public decimal? MinTotalAmount { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
