using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string PaymentGateway { get; set; } = null!;

    public string TransactionId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public virtual Order Order { get; set; } = null!;
}
