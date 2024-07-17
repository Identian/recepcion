using System;
using System.Collections.Generic;

namespace Domain.Entity;

public partial class InvoiceReceptionExtensible
{
    public int Id { get; set; }

    public int IdInvoiceReception { get; set; }

    public int IdEnterpriseCatReceptionExtensible { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Internal1 { get; set; }

    public string? Internal2 { get; set; }

    public string? Value { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool Active { get; set; }

    public virtual EnterpriseCatReceptionExtensible IdEnterpriseCatReceptionExtensibleNavigation { get; set; } = null!;

    public virtual InvoiceReception IdInvoiceReceptionNavigation { get; set; } = null!;
}
