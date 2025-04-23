using System;
using System.Collections.Generic;

namespace Clothing_shop_v2.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public int? OrderId { get; set; }

    public int? PromotionId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ReadDate { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual User User { get; set; } = null!;
}
